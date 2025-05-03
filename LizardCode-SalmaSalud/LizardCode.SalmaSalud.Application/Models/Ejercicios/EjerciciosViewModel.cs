using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Ejercicios
{
    public class EjerciciosViewModel
    {
        public int IdEjercicio { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Date, datePattern: "['m','Y']", delimiters: "/", numericOnly: true)]
        [Remote("ValidarFechaInicio", "Ejercicios", ErrorMessage = "La Fecha de Inicio existe en otro Ejercicio", HttpMethod = "POST")]
        public string MesAnnoInicio { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Date, datePattern: "['m','Y']", delimiters: "/", numericOnly: true)]
        [Remote("ValidarFechaFin", "Ejercicios", ErrorMessage = "La Fecha de Fin existe en otro Ejercicio", HttpMethod = "POST")]
        public string MesAnnoFin { get; set; }

    }
}