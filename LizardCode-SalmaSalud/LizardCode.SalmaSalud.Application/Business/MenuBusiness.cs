using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class MenuBusiness: BaseBusiness, IMenuBusiness
    {
        private readonly IMenuRepository _menuRepository;

        public MenuBusiness(
            IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<List<ItemMenu>> GetAll()
        {
            var menu = await _menuRepository.GetAllCustom();
            var itemsMenu = menu
                .GroupBy(m => new { m.IdMenu, m.Descripcion, m.Icono })
                .Select(sm => new ItemMenu()
                {
                    Texto = sm.Key.Descripcion,
                    Codigo = sm.Key.IdMenu.ToString(),
                    Action = "",
                    Icono = sm.Key.Icono,
                    SubMenu = sm.Select(gsm => new ItemMenu()
                    {
                        Texto = gsm.DescripcionSubmenu,
                        Codigo = gsm.IdSubmenu.ToString(),
                        Action = gsm.Accion,
                        Controller = gsm.EsReporte ? "Reportes" : ""
                    })
                    .ToList()
                })
                .ToList();


            return itemsMenu;

        }

        public async Task<List<ItemMenu>> GetAllByTipoUsuario(int idTipoUsuario)
        {
            var menu = await _menuRepository.GetByTipoUsuarioCustom(idTipoUsuario);
            var itemsMenu = menu
                .GroupBy(m => new { m.IdMenu, m.Descripcion, m.Icono })
                .Select(sm => new ItemMenu()
                {
                    Texto = sm.Key.Descripcion,
                    Codigo = sm.Key.IdMenu.ToString(),
                    Action = "",
                    Icono = sm.Key.Icono,
                    SubMenu = sm.Select(gsm => new ItemMenu()
                    {
                        Texto = gsm.DescripcionSubmenu,
                        Codigo = gsm.IdSubmenu.ToString(),
                        Action = gsm.Accion,
                        Controller = gsm.EsReporte ? "Reportes"  :""
                    })
                    .ToList()
                })
                .ToList();


            return itemsMenu;

        }
    }
}
