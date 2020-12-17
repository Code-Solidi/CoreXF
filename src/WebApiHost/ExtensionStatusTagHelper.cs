using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

using System;
using System.Text;

using static CoreXF.Abstractions.Base.IExtension;

namespace CoreXF.WebApiHost
{
    [HtmlTargetElement("status")]
    public class ExtensionStatusTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var item = ((HtmlString)output.Attributes["item"]?.Value)?.Value;

            var builder = new StringBuilder();
            var value = ((HtmlString)output.Attributes["current"]?.Value)?.Value;
            if (Enum.TryParse<ExtensionStatus>(value, true, out var status))
            {
                switch (status)
                {
                    case ExtensionStatus.Running:
                        builder
                            .Append($"<button type='button' class='play btn btn-sm text-muted' disabled data-item-name='{item}' title='Stopped, press to run.'><i class='fas fa-play'></i></button>&nbsp;")
                            .Append($"<button type='button' class='pause btn btn-sm text-danger' data-item-name='{item}' title='Running, press to stop.'><i class='fas fa-stop'></i></button>");
                        break;

                    case ExtensionStatus.Stopped:
                        builder
                            .Append($"<button type='button' class='play btn btn-sm text-success' data-item-name='{item}' title='Stopped, press to run.'><i class='fas fa-play'></i></button>&nbsp;")
                            .Append($"<button type='button' class='pause btn btn-sm text-muted' disabled data-item-name='{item}' title='Running, press to stop.'><i class='fas fa-stop'></i></button>");
                        break;
                }
            }

            output.Content.SetHtmlContent(builder.ToString());
        }
    }
}