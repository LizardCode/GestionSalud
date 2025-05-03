using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.SalmaSalud.Application.Models.OrdenesPago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Evoluciones
{
    public class EvolucionExternaViewModel
    {
        
        public EvolucionExternaViewModel()
        {
            Fecha = DateTime.Now.Date;
        }

        public int IdEvolucion { get; set; }

        public int IdEmpresa { get; set; }
        public int IdTurno { get; set; }
        public string TipoTurno { get; set; }
        public int IdEspecialidad { get; set; }
        public int IdProfesional { get; set; }
        public int IdPaciente { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }

        [RequiredEx]
        [StringLengthEx(10)]
        public string Documento { get; set; }

        [RequiredEx]
        [StringLengthEx(50)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        public string Nombre { get; set; }

        [RequiredEx]
        public int IdTipoTelefono { get; set; }

        [StringLengthEx(50)]
        [RequiredEx]
        public string Telefono { get; set; }

        [EmailAddressEx]
        [StringLengthEx(100)]
        [RequiredEx]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "_.-@")]
        public string Email { get; set; }

        [RequiredEx]
        public int? IdFinanciador { get; set; }

        [RequiredEx]
        public int? IdFinanciadorPlan { get; set; }

        [RequiredEx]
        public string FinanciadorNro { get; set; }

        public bool SinCobertura { get; set; }

        [RequiredEx]
        public DateTime? FechaNacimiento { get; set; }

        [RequiredEx]
        public string Nacionalidad { get; set; }

        public SelectList MaestroFinanciadores { get; set; }

        [RequiredEx]
        public string Diagnostico { get; set; }
        public string Observaciones { get; set; }

        public List<EvolucionPrestacionViewModel> Prestaciones { get; set; }

        public List<EvolucionOtraPrestacionViewModel> OtrasPrestaciones{ get; set; }

		public List<Odontograma.OdontogramaPiezaViewModel> Piezas { get; set; }

        public List<EvolucionRecetaViewModel> Recetas { get; set; }

        public List<EvolucionOrdenViewModel> Ordenes { get; set; }

        #region Filtros
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        //public int IdEspecialidad { get; set; }
        //public int IdProfesional { get; set; }

        public SelectList MaestroEspecialidades { get; set; }
        public SelectList MaestroProfesionales { get; set; }

        #endregion

        public SelectList MaestroPrestaciones { get; set; }
        public SelectList MaestroOtrasPrestaciones { get; set; }
        public SelectList MaestroPiezas { get; set; }
        public SelectList MaestroVademecum { get; set; }

        //public int? IdFinanciador { get; set; }
        //public int? IdFinanciadorPlan { get; set; }
        //public string FinanciadorNro { get; set; }

        public List<IFormFile> Imagenes { get; set; }

        public string Presupuestos { get; set; }

        public EvolucionRecetaViewModel Receta { get; set; }
        public EvolucionOrdenViewModel Orden { get; set; }

        public bool ForzarParticular { get; set; }

        public IFormFile FileExcel { get; set; }
    }
}