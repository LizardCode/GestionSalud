using System.ComponentModel;

namespace LizardCode.Framework.Application.Common.Enums
{
    public enum PermisoError
    {
        [Description("Error General")]
        ErrorGeneral = 2000,

        [Description("Sin autorización")]
        NoAutorizado = 2002,

        [Description("Url incorrecta")]
        UrlIncorrecta = 2003,

        [Description("Acceso restringido")]
        SinPermiso = 2100,

        [Description("Parámetro sin especificar")]
        ParametroNull = 2201,

        [Description("FormsAuthentications no implementado.")]
        FormsAuthenticationDisabled = 2202,

        [Description("FormsAuthentications configurado como cookieless.")]
        FormsAuthenticationCookieless = 2203,
    }
}
