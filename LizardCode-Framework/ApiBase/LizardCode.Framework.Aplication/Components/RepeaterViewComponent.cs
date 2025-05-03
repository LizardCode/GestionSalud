using LizardCode.Framework.Application.Models.Repeater;
using Microsoft.AspNetCore.Mvc;

namespace LizardCode.Framework.Application.Components
{
    public class RepeaterViewComponent : ViewComponent
    {
        //private readonly IPermisosBusiness _permisos;

        public RepeaterViewComponent() //IPermisosBusiness permisos)
        {
            //_permisos = permisos;
        }

        public async Task<IViewComponentResult> InvokeAsync(RepeaterViewModel model)
        {
            return await Task.FromResult(View("Repeater", model));
        }
    }
}
