using LizardCode.Framework.Application.Interfaces.Business;

namespace LizardCode.Framework.Application.Models.Permissions
{
    public class Usuario : IPermisoUsuario
    {
        public int Id { get; private set; }
        public string Login { get; private set; }
        public string Nombre { get; private set; }
        public int IdTipoUsuario { get; private set; }
        public bool Admin { get; set; }


        public Usuario(int id, string login, string nombre, int idTipoUsuario, bool admin)
        {
            Id = id;
            Login = login;
            Nombre = nombre;
            IdTipoUsuario = idTipoUsuario;
            Admin = admin;
        }
    }
}
