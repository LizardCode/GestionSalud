using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class PrestacionProfesional
    {
        public int IdEvolucionPrestacion { get; set; }
        public int IdProfesional { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime Fecha { get; set; }

        public string Profesional { get; set; }
        public string Especialidad { get; set; }
        public string Prestacion { get; set; }
        public string Codigo { get; set; }

        public double Valor { get; set; }
    }
}
