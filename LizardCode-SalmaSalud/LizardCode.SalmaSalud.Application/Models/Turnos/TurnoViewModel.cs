using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class TurnoViewModel
    {
        #region Filtros

        public int IdEspecialidad { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public int IdProfesional { get; set; }

        #endregion

        public SelectList MaestroEspecialidades { get; set; }
        public SelectList MaestroProfesionales { get; set; }
    }
}
