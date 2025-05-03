namespace LizardCode.SalmaSalud.Application.Models.Base
{
    public class Usuario
    {
        public int Id { get; private set; }

        public string NombreCompleto { get; private set; }


        public Usuario(int id, string nombreCompleto)
        {
            Id = id;
            NombreCompleto = nombreCompleto;
        }
    }
}