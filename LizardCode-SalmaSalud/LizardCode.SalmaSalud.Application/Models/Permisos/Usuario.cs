namespace LizardCode.SalmaSalud.Application.Models.Permissions
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string Login { get; private set; }
        public string Nombre { get; private set; }
        public int IdTipoUsuario { get; private set; }
        public bool Admin { get; set; }
        public int IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public string CUIT { get; set; }
        public int IdProfesional { get; set; }
        public int IdPaciente { get; set; }
        public string Profesional { get; set; }

        public Usuario(int id, string login, string nombre, int idTipoUsuario, bool admin, int idEmpresa, string empresa, string cuit, int idProfesional, int idPaciente, string profesional)
        {
            Id = id;
            Login = login;
            Nombre = nombre;
            IdTipoUsuario = idTipoUsuario;
            Admin = admin;
            IdEmpresa = idEmpresa;
            Empresa = empresa;
            CUIT = cuit;
            IdProfesional = idProfesional;
            IdPaciente = idPaciente;
            Profesional = profesional;
        }
    }
}
