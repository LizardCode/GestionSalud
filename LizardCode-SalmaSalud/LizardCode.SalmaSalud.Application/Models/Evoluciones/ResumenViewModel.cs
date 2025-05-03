using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.Evoluciones
{
    public class ResumenViewModel
    {
        public int IdEvolucion { get; set; }
        public string Especialidad { get; set; }
        public string Profesional { get; set; }
        public string Diagnostico { get; set; }
        public string Financiador { get; set; }
        public string FinanciadorPlan { get; set; }
        public string NroAfiliadoSocio { get; set; }
        public string TipoTurno { get; set; }
        public string TipoTurnoDescripcion { get; set; }
        public string TurnoHora { get; set; }
        public List<ResumenPrestacionesViewModel> Prestaciones { get; set; } = new();
        public List<ResumenImagenesViewModel> Imagenes { get; set; } = new();
        public List<ResumenRecetasViewModel> Recetas { get; set; } = new();
        public List<ResumenOrdenesViewModel> Ordenes { get; set; } = new();

        public int IdTipoUsuario { get; set; }
    }

    public class ResumenPrestacionesViewModel
    {
        public int Pieza { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }

    public class ResumenImagenesViewModel
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Extension { get; set; }
        public string Imagen { get; set; }
    }
    public class ResumenRecetasViewModel
    {
        public int IdEvolucionReceta { get; set; }
        public string Descripcion { get; set; }
        public int Codigo { get; set; }
    }
    public class ResumenOrdenesViewModel
    {
        public int IdEvolucionOrden { get; set; }
        public string Descripcion { get; set; }
    }
}
