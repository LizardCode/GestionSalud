using LizardCode.Framework.Application.Models.MasterDetail;
using Microsoft.AspNetCore.Mvc;

namespace LizardCode.Framework.Application.Components
{
    public class MasterDetailViewComponent : ViewComponent
    {
        //private readonly IPermisosBusiness _permisos;

        public MasterDetailViewComponent() //(IPermisosBusiness permisos)
        {
            //_permisos = permisos;
        }

        public async Task<IViewComponentResult> InvokeAsync(MasterDetailViewModel model)
        {
            return await Task.FromResult(View("MasterDetail", model));
        }
    }
}
