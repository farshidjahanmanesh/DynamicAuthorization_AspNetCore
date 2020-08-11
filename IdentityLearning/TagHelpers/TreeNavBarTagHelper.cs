using IdentityLearning.Models.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityLearning.TagHelpers
{

    [HtmlTargetElement("div")]
    public class TreeNavBarTagHelper : TagHelper
    {
        public string test { get; set; }
        public List<NavBarViewModel> model { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (model == null)
                base.Process(context, output);
            else
            {
                output.TagName = "section";
                output.TagMode = TagMode.StartTagAndEndTag;
                StringBuilder sb = new StringBuilder();
                sb.Append(@"<ul id=""myUL"">");
                sb.Append(@"<li>");
                sb.Append(@"<span class=""caret"">دسترسی ها</span>");
                sb.Append(@"<ul class=""nested"">");

                foreach (var item in model.GroupBy(x => x.ControllerName))
                {
                    
                    if (item.Where(x => x.ActionName != "\\").Count() > 0)
                    {
                        sb.Append(@"<li><span class=""caret"">"+item.Key+ "</span>");
                        sb.Append(@"<ul class=""nested"">");
                        foreach (var sub in item)
                        {
                            if (sub.ActionName != "\\")
                            {

                                sb.Append($"<li><a asp-action='{sub.ActionName}'  asp-controller='{sub.ControllerName}'>{sub.PersianAccessLevelName}<a/>");
                            }
                        }
                        sb.Append("</ul>");
                        sb.Append($"</li>");
                    }
                    else
                    {
                        sb.Append($"<li>{item.Key}</li>");
                        sb.Append($"</li>");
                    }
                       
                    
                    //foreach(var sub in item.Key)
                    //    sub.
                }
                sb.Append($"</ul>");
                sb.Append($"</li>");
                sb.Append($"</ul>");
                output.PreContent.SetHtmlContent(sb.ToString());
            }


            //< ul id = "myUL" >

            //             < li >

            //                 < span class="caret">Beverages</span>
            //                <ul class="nested">
            //                    <li>Water</li>
            //                    <li>Coffee</li>
            //                    <li>
            //                        <span class="caret">Tea</span>
            //                        <ul class="nested">
            //                            <li>Black Tea</li>
            //                            <li>White Tea</li>
            //                            <li>
            //                                <span class="caret">Green Tea</span>
            //                                <ul class="nested">
            //                                    <li>Sencha</li>
            //                                    <li>Gyokuro</li>
            //                                    <li>Matcha</li>
            //                                    <li>Pi Lo Chun</li>
            //                                </ul>
            //                            </li>
            //                        </ul>
            //                    </li>
            //                </ul>
            //            </li>
            //        </ul>




            base.Process(context, output);
        }
    }
}
