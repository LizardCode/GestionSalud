using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.Login
{
    public class LoginViewModel
    {
        public string User { get; set; }

        public string Pass { get; set; }

        public string NewPass { get; set; }

        public string RepeatPass { get; set; }

        public int IdEmpresa { get; set; }

        public SelectList MaestroEmpresas { get; set; }


        #region Campos LogBrowser

        public string Platform { get; set; }
        public string OS { get; set; }
        public string Browser { get; set; }
        public string Version { get; set; }
        public string Resolucion { get; set; }

        #endregion

        public string SiteKey { get; set; }
    }
}