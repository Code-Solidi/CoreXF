﻿using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Extension4.TagHelpers
{
    //[Export]
    [HtmlTargetElement("email", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class EmailTagHelper : TagHelper/*Component*/
    {
        private const string emailDomain = "codesolidi.com";

        // Can be passed via <email mail-to="..." />.
        // Pascal case gets translated into lower-kebab-case.
        public string MailTo { get; set; }  // not used!

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";    // Replaces <email> with <a> tag
            var content = output.GetChildContentAsync().Result.GetContent().ToLowerInvariant();
            var address = content + "@" + EmailTagHelper.emailDomain;
            output.Attributes.SetAttribute("href", "mailto:" + address);
            output.Content.SetContent(content);
        }
    }
}