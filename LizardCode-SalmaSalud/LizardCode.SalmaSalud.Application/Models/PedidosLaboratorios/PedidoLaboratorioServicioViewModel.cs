using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.PedidosLaboratorios
{
    public class PedidoLaboratorioServicioViewModel
    {
        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdPedidoLaboratorio { get; set; }

        [RepeaterColumn(Header = "#", Position = 2, Width = 40, Readonly = true, Hidden = true)]
        public int Item { get; set; }

        [RepeaterColumn(Header = "Servicio", ControlType = RepeaterColumnType.Select2, Width = 0, Position = 3)]
        [RequiredEx]
        public int IdLaboratorioServicio { get; set; }
        public string Servicio { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Valor", Width = 75, Position = 4, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".", decimalPlaces: 0)]
        public double Valor { get; set; }
    }
}