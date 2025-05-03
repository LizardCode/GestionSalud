using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.PlanCuentas
{
    public class PlanCuentasViewModel
    {
        public int? IdCuentaContable { get; set; }
        public string CodigoCuenta { get; set; }
        public string Descripcion { get; set; }
        public string RubroContable { get; set; }

        public List<RubroContable> Rubros { get; set; }
        public List<CuentaContable> Cuentas { get; set; }

    }
}