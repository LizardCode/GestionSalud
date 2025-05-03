using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Feriados
{
    public class FeriadoViewModel
    {
        public int IdFeriado { get; set; }

        [RequiredEx]
        public int IdTipoFeriado { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }

        [RequiredEx]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public FeriadoViewModel()
        {
            Fecha = DateTime.Now.Date;
        }

        public SelectList MaestroTiposFeriado { get; set; }

        //Filtros
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
    }
}
