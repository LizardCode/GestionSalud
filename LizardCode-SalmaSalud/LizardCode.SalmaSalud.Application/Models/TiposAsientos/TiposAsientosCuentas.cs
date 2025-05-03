using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.TiposAsientos
{
    public class TiposAsientosCuentas
    {
        public TiposAsientosCuentas()
        {
        }

        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdTiposAsientosCuenta { get; set; }

        [RepeaterColumn(Header = "#", Position = 2, Width = 50, Readonly = true)]
        public int Item { get; set; }

        [RepeaterColumn(Header = "Cuenta", ControlType = RepeaterColumnType.Select2, Width = 200, Position = 3)]
        [RequiredEx]
        public int IdCuentaContable { get; set; }

        [RepeaterColumn(Header = "Descripción", ControlType = RepeaterColumnType.Input, Width = 0, Position = 4)]
        [RequiredEx]
        public string Descripcion { get; set; }
    }
}
