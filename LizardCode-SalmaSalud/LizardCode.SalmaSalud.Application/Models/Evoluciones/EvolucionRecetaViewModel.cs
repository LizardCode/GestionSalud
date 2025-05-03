using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.Evoluciones
{
    public class EvolucionRecetaViewModel
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdEvolucion { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Medicamento", Width = 0, Position = 2)]
        public int IdVademecum { get; set; }
        public string Descripcion { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Cantidad", Width = 100, Position = 3)]
        public int Cantidad { get; set; } = 1;

        [RequiredEx]
        [MasterDetailColumn(Header = "Dosis", Width = 200, Position = 4)]
        public string Dosis { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Frecuencia", Width = 200, Position = 5)]
        public string Frecuencia { get; set; }

        //[RequiredEx]
        [MasterDetailColumn(Header = "Indicaciones", Width = 0, Position = 6, Hidden = true)]
        public string Indicaciones { get; set; }

        public bool NoSustituir { get; set; }
    }
}
