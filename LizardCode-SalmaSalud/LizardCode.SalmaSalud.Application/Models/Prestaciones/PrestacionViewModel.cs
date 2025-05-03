using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.SalmaSalud.Application.Models.PrestacionesFinanciador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Prestaciones
{
    public class PrestacionViewModel
    {
        public int IdPrestacion { get; set; }

        [RequiredEx]
        [Remote("ValidarCodigo", "Prestaciones", HttpMethod = "Post", ErrorMessage = "Código en uso")]
        public string Codigo { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        [RequiredEx]
        //[AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Valor { get; set; }
        public IFormFile FileExcel { get; set; }

        //[RequiredEx]
        public double ValorFijo { get; set; }

        [RequiredEx]
        public double Porcentaje { get; set; }

        public SelectList MaestroProfesionales { get; set; }
        public List<PrestacionProfesionalViewModel> Profesionales { get; set; }
    }
}
