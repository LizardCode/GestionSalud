namespace LizardCode.Framework.Application.Interfaces.Business
{
    public interface IPermisoUsuario
    {
        int Id { get; }
        string Login { get; }
        public string Nombre { get; }
        public int IdTipoUsuario { get; }
        bool Admin { get; }
    }
}
