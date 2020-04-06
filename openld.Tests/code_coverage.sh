dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/ /p:CoverletOutputFormat=lcov /p:Exclude="[*]openld.Migrations.*%2c[*]openld.Program*%2c[*]openld.Startup*%2c[*]openld.Pages.*%2c[*]openld.Models.*%2c[*]openld.Controllers.*%2c[*]openld.Hubs.*%2c[*]openld.Data.*%2c[*]openld.Mapping.*"
reportgenerator -reports:TestResults/coverage.info -reporttypes:Html -targetdir:TestResults/coverage

xdg-open TestResults/coverage/index.htm