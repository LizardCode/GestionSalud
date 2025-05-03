namespace LizardCode.SalmaSalud.Application.Startup
{
    public static class GlobalSettings
    {
        // El valor de esta propiedad aparecerá en la esquina superior izquierda
        // en homeLayout.cshtml
        public const string TituloSistema = "Salma Salud";

        // El valor de esta propiedad aparecerá en el título de la pestaña en
        // el navegador durante la pantalla de login
        public const string TituloPestañaLogin = "Salma Salud";

        // El valor de esta propiedad se utiliza para obtener la versión
        // y mostrarla en homeLayout.cshtml y en Login/index.cshtml
        public const string NombreEnsamblado = "LizardCode.SalmaSalud";

        // Especifica cual será la vista inicial del sistema
        public static string PaginaInicial = "Bienvenida";
    }
}
