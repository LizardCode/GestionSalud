using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.SaldoInicioBanco
{
    public class SaldoInicioBancoViewModel
    {
        public int IdSaldoInicioBanco { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        #region Anticipos Clientes

        public SaldoInicioBancoAnticiposClientes AnticipoCliente { get; set; }

        public List<SaldoInicioBancoAnticiposClientes> AnticiposClientes { get; set; }

        #endregion

        #region Anticipos Proveedores

        public SaldoInicioBancoAnticiposProveedores AnticipoProveedor { get; set; }

        public List<SaldoInicioBancoAnticiposProveedores> AnticiposProveedores { get; set; }

        #endregion

        #region Cheques (Propios y Terceros)

        public SaldoInicioBancoCheques Cheque { get; set; }

        public List<SaldoInicioBancoCheques> Cheques { get; set; }


        #endregion

        #region Lookups

        public SelectList MaestroMonedas { get; set; }

        public SelectList MaestroClientes { get; set; }

        public SelectList MaestroProveedores { get; set; }

        public SelectList MaestroBancos { get; set; }

        public SelectList MaestroTipoCheques { get; set; }

        #endregion

    }
}