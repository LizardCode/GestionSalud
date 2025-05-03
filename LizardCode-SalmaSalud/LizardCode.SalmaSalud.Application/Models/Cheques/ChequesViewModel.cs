using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using System;
using System.Collections.Generic;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Cheques
{
    public class ChequesViewModel
    {
        public int IdCheque { get; set; }

        [RequiredEx]
        public int IdBanco { get; set; }

        [RequiredEx]
        public int IdTipoCheque { get; set; }

        #region Filtros

        public int? IdEstadoCheque { get; set; }
        public string NumeroCheque { get; set; }

        #endregion

        [RequiredEx]
        [StringLengthEx(10)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaNumeric)]
        [Remote("ValidarNumeroCheque", "Cheques", AdditionalFields = "IdBanco,IdTipoCheque,NroHasta", ErrorMessage = "Error en el Rango de Cheques", HttpMethod = "POST")]
        public string NroDesde { get; set; }

        [RequiredEx]
        [StringLengthEx(10)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaNumeric)]
        [Remote("ValidarNumeroCheque", "Cheques", AdditionalFields = "IdBanco,IdTipoCheque,NroDesde", ErrorMessage = "Error en el Rango de Cheques", HttpMethod = "POST")]
        public string NroHasta { get; set; }

        [RequiredEx]
        public int IdBancoDebitar { get; set; }
        public List<ChequesADebitarViewModel> ChequesADebitar { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        public SelectList MaestroTipoCheque { get; set; }

        public SelectList MaestroEjercicios { get; set; }

        public SelectList MaestroTipoChequePropios { get; set; }

        public SelectList MaestroEstadoCheque { get; set; }

        public SelectList MaestroBancos { get; set; }

        

    }
}