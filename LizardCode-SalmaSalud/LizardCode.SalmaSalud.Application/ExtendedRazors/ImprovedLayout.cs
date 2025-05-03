using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using System.IO;

namespace LizardCode.SalmaSalud.Application.ExtendedRazors
{
    public abstract class ImprovedLayout<TModel> : RazorPage<TModel>
    {
        public HtmlString RenderSectionInMemory(string name, bool required)
        {
            var oldWriter = ViewContext.Writer;
            var result = default(HtmlString);

            using (var sw = new StringWriter())
            {
                ViewContext.Writer = sw;
                RenderSection(name, required);

                result = new HtmlString(sw.ToString());
            }

            ViewContext.Writer = oldWriter;

            return result;
        }
    }
}
