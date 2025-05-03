using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.CargaManual
{
    public class CargaManualViewModel
    {
        public int IdComprobanteCompra { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }
        [RequiredEx]
        public DateTime FechaReal { get; set; }
        public DateTime? Vto { get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        [RequiredEx]
        public int IdProveedor { get; set; }

        [RequiredEx]
        public int IdComprobante { get; set; }

        [RequiredEx]
        public string NumeroComprobante { get; set; }

        public int? IdCentroCosto { get; set; }

        [RequiredEx]
        public string IdMonedaComprobante { get; set; }

        public string Moneda { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Cotizacion { get; set; }

        public CargaManualDetalle Detalle { get; set; }

        public List<CargaManualDetalle> Items { get; set; }

        public CargaManualPercepciones Percepcion { get; set; }

        public List<CargaManualPercepciones> ListaPercepciones { get; set; }

        [MaskConstraint(MaskConstraintType.String, blocks: "[14]", numericOnly: true)]
        public string CAE { get; set; }

        public DateTime? VenciminetoCAE { get; set; }

        public SelectList MaestroCuentas { get; set; }
        public SelectList MaestroCuentasPercepciones { get; set; }
        public SelectList MaestroProveedores { get; set; }
        public SelectList MaestroAlicuotas { get; set; }
        public SelectList MaestroMonedas { get; set; }
        public SelectList MaestroComprobantes { get; set; }
        public SelectList MaestroEjercicios { get; set; }
        public SelectList MaestroCentroCostos { get; set; }
        public SelectList MaestroInterfaz { get; set; }

        public int IdCondicion { get; set; }
        public SelectList MaestroCondicion { get; set; }

        #region Datos Modal Interfaz

        public int IdTipoInterfaz { get; set; }
        public int IdEjercicioInterfaz { get; set; }
        public DateTime FechaInterfaz { get; set; }
        public int IdCuentaContable { get; set; }
        public IFormFile FileCSV { get; set; }

        #endregion

    }
}
