using LizardCode.Framework.Application.Common.Annotations;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Evoluciones
{
    public class EvolucionOrdenViewModel
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdEvolucion { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Descripcion", Width = 0, Position = 2)]
        public string Descripcion { get; set; }

        //[RequiredEx]
        [MasterDetailColumn(Header = "Indicaciones", Width = 0, Position = 3, Hidden = true)]
        public string Indicaciones { get; set; }
    }
}