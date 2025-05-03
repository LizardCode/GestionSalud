using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

namespace LizardCode.Framework.Application.Helpers
{
    public class DisposableBlockHelper : IDisposable
    {
        private readonly IHtmlHelper _htmlHelper;
        private readonly HtmlString _begin;
        private readonly HtmlString _end;


        public DisposableBlockHelper(IHtmlHelper htmlHelper, HtmlString beginHtml, HtmlString endHtml)
        {
            _htmlHelper = htmlHelper;
            _begin = beginHtml;
            _end = endHtml;

            Begin();
        }


        private void Begin()
            => _begin.WriteTo(_htmlHelper.ViewContext.Writer, HtmlEncoder.Default);

        private void End()
            => _end.WriteTo(_htmlHelper.ViewContext.Writer, HtmlEncoder.Default);


        public void Dispose()
            => End();
    }
}
