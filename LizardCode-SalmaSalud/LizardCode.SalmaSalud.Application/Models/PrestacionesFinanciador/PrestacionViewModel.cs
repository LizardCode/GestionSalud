using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.PrestacionesFinanciador
{
    public class PrestacionViewModel
    {
        public int IdFinanciadorPrestacion { get; set; }

        [RequiredEx]
        public int IdFinanciadorPlan { get; set; }

        [RequiredEx]
        [Remote("ValidarCodigo", "PrestacionesFinanciador", AdditionalFields = "IdFinanciadorPrestacion,IdFinanciadorPlan", HttpMethod = "Post", ErrorMessage = "Código en uso")]
        public string Codigo { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        //[RequiredEx]
        public int IdPrestacion { get; set; }

        [RequiredEx]
        //[AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Valor { get; set; }

        //[RequiredEx]
        public double ValorFijo { get; set; }

        [RequiredEx]
        public double Porcentaje { get; set; }

        public double CoPago { get; set; }

        public SelectList MaestroFinanaciadorPlanes { get; set; }
        public SelectList MaestroPrestaciones { get; set; }
        public SelectList MaestroProfesionales { get; set; }

        public IFormFile FileExcel { get; set; }
        public string CodigoPlan { get; set; }
        public string NomencladorInterno { get; set; }

        public List<PrestacionFinanciadorProfesionalViewModel> Profesionales { get; set; }
    }
}