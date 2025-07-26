using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Menu;
using System.Linq;
using static iTextSharp.text.pdf.AcroFields;

namespace LizardCode.SalmaSalud.Application.Components
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IPermisosBusiness _permissions;
        private readonly IMenuBusiness _menuBusiness;


        public MenuViewComponent(IPermisosBusiness permissions, IMenuBusiness menuBusiness)
        {
            _permissions = permissions;
            _menuBusiness = menuBusiness;
        }

        public IViewComponentResult Invoke()
        {

            //var menu = _menuBusiness.GetAll().GetAwaiter().GetResult();
            var menu = _menuBusiness.GetAllByTipoUsuario(_permissions.User.IdTipoUsuario).GetAwaiter().GetResult();
            var model = new Menu
            {   
                Items = menu
            };

            // dashboard... por ahora solo dos perfiles...
            if (_permissions.User.IdTipoUsuario == (int)TipoUsuario.Administrador)
            {
                //model.Items.Clear();

                model.Items.Insert(0, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Dashboard",
                    Codigo = "DASHBOARD",
                    Action = "Home",
                    Icono = "activity"
                });

                model.Items.Insert(1, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Sol. Turnos",
                    Codigo = "TURNOSSOLICITUD",
                    Action = "TurnosSolicitud",
                    Icono = "calendar"
                });

                //model.Items.Insert(2, new ItemMenu(TipoMenuItem.Item, true)
                //{
                //    Texto = "Sala de Espera",
                //    Codigo = "TURNOSSALAESPERA",
                //    Action = "TurnosSalaEspera",
                //    Icono = "clock"
                //});

                //model.Items.Insert(3, new ItemMenu(TipoMenuItem.Item, true)
                //{
                //    Texto = "Guardia",
                //    Codigo = "GUARDIA",
                //    Action = "TurnosGuardia",
                //    Icono = "plus"
                //});

                model.Items.Insert(2, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Pacientes",
                    Codigo = "PACIENTES",
                    Action = "Pacientes",
                    Icono = "user"
                });

                model.Items.Insert(3, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Profesionales",
                    Codigo = "PROFESIONALES",
                    Action = "Profesionales",
                    Icono = "briefcase"
                });

                model.Items.Insert(4, new ItemMenu(TipoMenuItem.Espaciador, true)
                {
                    Texto = "",
                    Codigo = "",
                    Action = "",
                    Icono = ""
                });
            }
            else if (_permissions.User.IdTipoUsuario == (int)TipoUsuario.Recepcion)
            {
                model.Items.Clear();

                model.Items.Insert(0, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Dashboard",
                    Codigo = "DASHBOARD",
                    Action = "Home",
                    Icono = "activity"
                });

                model.Items.Insert(1, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Sol. Turnos",
                    Codigo = "TURNOSSOLICITUD",
                    Action = "TurnosSolicitud",
                    Icono = "calendar"
                });

                model.Items.Insert(2, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Pacientes",
                    Codigo = "PACIENTES",
                    Action = "Pacientes",
                    Icono = "user"
                });

                model.Items.Insert(3, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Profesionales",
                    Codigo = "PROFESIONALES",
                    Action = "Profesionales",
                    Icono = "briefcase"
                });

                model.Items.Insert(4, new ItemMenu(TipoMenuItem.Espaciador, true)
                {
                    Texto = "",
                    Codigo = "",
                    Action = "",
                    Icono = ""
                });
            }
            else if (_permissions.User.IdTipoUsuario == (int)TipoUsuario.Profesional)
            {
                model.Items.Insert(0, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Dashboard",
                    Codigo = "DASHBOARD",
                    Action = "Home",
                    Icono = "activity"
                });

                model.Items.Insert(1, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Sala de Espera",
                    Codigo = "TURNOSSALAESPERA",
                    Action = "TurnosSalaEspera",
                    Icono = "clock"
                });

                model.Items.Insert(2, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Guardia",
                    Codigo = "GUARDIA",
                    Action = "TurnosGuardia",
                    Icono = "plus"
                });

                model.Items.Insert(3, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Pacientes",
                    Codigo = "PACIENTES",
                    Action = "Pacientes",
                    Icono = "user"
                });


                model.Items.Insert(3, new ItemMenu(TipoMenuItem.Espaciador, true)
                {
                    Texto = "",
                    Codigo = "",
                    Action = "",
                    Icono = ""
                });
            }
            else if (_permissions.User.IdTipoUsuario == (int)TipoUsuario.ProfesionalExterno)
            {
                model.Items.Insert(0, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Dashboard",
                    Codigo = "DASHBOARD",
                    Action = "Home",
                    Icono = "activity"
                });

                model.Items.Insert(1, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Evoluciones",
                    Codigo = "EVOLUCIONES",
                    Action = "Index",
                    Controller = "/evolucionesExternas",
                    Icono = "file-text"
                });

                model.Items.Insert(2, new ItemMenu(TipoMenuItem.Espaciador, true)
                {
                    Texto = "",
                    Codigo = "",
                    Action = "",
                    Icono = ""
                });
            }
            else if (_permissions.User.IdTipoUsuario == (int)TipoUsuario.Paciente)
            {
                model.Items.Insert(0, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Inicio",
                    Codigo = "INICIO",
                    Action = "",
                    Controller = "/portal-pacientes",
                    Icono = "bar-chart"
                });

                model.Items.Insert(1, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Mis Turnos",
                    Codigo = "TURNOS",
                    Action = "turnos",
                    Controller = "/portal-pacientes",
                    Icono = "calendar"
                });

                //model.Items.Insert(2, new ItemMenu(TipoMenuItem.Item, true)
                //{
                //    Texto = "Hist. Clínica",
                //    Codigo = "HISTORIACLINICA",
                //    Action = "historia-clinica",
                //    Controller = "/portal-pacientes",
                //    Icono = "file-text"
                //});

                model.Items.Insert(2, new ItemMenu(TipoMenuItem.Item, true)
                {
                    Texto = "Mis Datos",
                    Codigo = "MISDATOS",
                    Action = "mis-datos",
                    Controller = "/portal-pacientes",
                    Icono = "settings"
                });
            }

            return View("Menu", model);
        }
    }
}
