#pragma checksum "/home/jtattersall/src/OpenLD/src/collab-prototype/Pages/Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b0496aa176d1436bdb8b7b82493bb2b7b84b5a32"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(collab_prototype.Pages.Pages_Index), @"mvc.1.0.razor-page", @"/Pages/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure.RazorPageAttribute(@"/Pages/Index.cshtml", typeof(collab_prototype.Pages.Pages_Index), null)]
namespace collab_prototype.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "/home/jtattersall/src/OpenLD/src/collab-prototype/Pages/_ViewImports.cshtml"
using collab_prototype;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b0496aa176d1436bdb8b7b82493bb2b7b84b5a32", @"/Pages/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2762105853b23bc3923633a6ee0274b3d4494e59", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Index : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 2 "/home/jtattersall/src/OpenLD/src/collab-prototype/Pages/Index.cshtml"
  
    ViewData["Title"] = "Collaborative Prototype";

#line default
#line hidden
            DefineSection("Scripts", async() => {
                BeginContext(84, 13, true);
                WriteLiteral("\r\n    <script");
                EndContext();
                BeginWriteAttribute("src", " src=\"", 97, "\"", 147, 1);
#line 6 "/home/jtattersall/src/OpenLD/src/collab-prototype/Pages/Index.cshtml"
WriteAttributeValue("", 103, Url.Content("~/lib/signalr/signalr.min.js"), 103, 44, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(148, 23, true);
                WriteLiteral("></script>\r\n    <script");
                EndContext();
                BeginWriteAttribute("src", " src=\"", 171, "\"", 211, 1);
#line 7 "/home/jtattersall/src/OpenLD/src/collab-prototype/Pages/Index.cshtml"
WriteAttributeValue("", 177, Url.Content("~/js/texteditor.js"), 177, 34, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(212, 12, true);
                WriteLiteral("></script>\r\n");
                EndContext();
            }
            );
            BeginContext(227, 231, true);
            WriteLiteral("\r\n\r\n<div>\r\n    <br />\r\n    <input id=\"group\" />\r\n    <input type=\"button\" value=\"Join Group\" onclick=\"join()\" />\r\n    <br /><br />\r\n    <textarea style=\"width:100%;height:300px;\" id=\"editor\" onkeyup=\"change()\"></textarea>\r\n</div>\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Pages_Index> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<Pages_Index> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<Pages_Index>)PageContext?.ViewData;
        public Pages_Index Model => ViewData.Model;
    }
}
#pragma warning restore 1591
