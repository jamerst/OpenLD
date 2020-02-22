using System.Threading.Tasks;

using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Npgsql;

using openld.Hubs;
using openld.Services;

namespace openld {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();

            services.AddDbContext<Data.OpenLDContext>(builder => {
                builder.UseNpgsql("Host=db; Database=openld_db; Username=openld; Password=openld");
                // use JSON.NET for JSON type mapping, not System.Text.Json
                NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
            });

            services.AddDefaultIdentity<Models.User>()
                .AddEntityFrameworkStores<Data.OpenLDContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<Models.User, Data.OpenLDContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.Configure<JwtBearerOptions>(
                IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
                options => {
                    var OnMessageReceived = options.Events.OnMessageReceived;
                    options.Authority = "https://www.openld.jtattersall.net";
                    options.TokenValidationParameters.ValidIssuers = new [] {
                        "https://www.openld.jtattersall.net",
                        "https://0.0.0.0:5000"
                    };

                    // add logic to allow signalr hubs to be authorized successfully
                    // token cannot be added in request headers, so it is sent as a parameter, so must be copied into context
                    options.Events.OnMessageReceived = async context => {
                        // run original handler first
                        await OnMessageReceived(context);

                        string token = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(token) &&
                        context.HttpContext.Request.Path.StartsWithSegments("/api/drawing/hub")) {
                            context.Token = token;
                        }
                    };
                }
            );

            services.AddControllersWithViews();
            services.AddRazorPages();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "client/build";
            });

            services.AddTransient<IDrawingService, DrawingService>();
            services.AddTransient<IFixtureService, FixtureService>();
            services.AddTransient<IFixtureTypeService, FixtureTypeService>();
            services.AddTransient<ILabelService, LabelService>();
            services.AddTransient<IRiggedFixtureService, RiggedFixtureService>();
            services.AddTransient<IStructureService, StructureService>();
            services.AddTransient<ITemplateService, TemplateService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IViewService, ViewService>();

            services.AddAutoMapper(typeof(Startup));

            services.AddControllers().AddNewtonsoftJson(options => {
                // ignore loops when serialising
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSignalR(options => {
                options.EnableDetailedErrors = true;
            }).AddNewtonsoftJsonProtocol(options => options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<DrawingHub>("/api/drawing/hub");
            });

            app.UseSpa(spa => {
                spa.Options.SourcePath = "client";

                if (env.IsDevelopment()) {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
