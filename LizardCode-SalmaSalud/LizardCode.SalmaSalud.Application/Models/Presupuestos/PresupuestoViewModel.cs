using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.Presupuestos
{
    public class PresupuestoViewModel
    {
        public int IdPresupuesto { get; set; }

        [RequiredEx]
        public int IdPaciente { get; set; }

        [RequiredEx]
        public DateTime FechaVencimiento { get; set; }

        //[RequiredEx]
        public string Observaciones { get; set; }

        public List<PresupuestoPrestacionViewModel> Prestaciones { get; set; }
        public List<PresupuestoOtraPrestacionViewModel> OtrasPrestaciones { get; set; }

        #region Filtros

        public string FiltroNombre { get; set; }

        public string FiltroDocumento { get; set; }

        #endregion
        public SelectList MaestroPrestaciones { get; set; }
        public SelectList MaestroOtrasPrestaciones { get; set; }
        public SelectList MaestroPacientes { get; set; }
    }
}
