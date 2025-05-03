using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Tareas
{
    public class TareasViewModel
    {
        public int IdTarea { get; set; }

        [RequiredEx]
        [StringLengthEx(50)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }

        [RequiredEx]
        public int IdTipoTarea { get; set; }

        public int? IdTareaGrupo { get; set; }

        public string Grupo { get; set; }

        [RequiredEx]
        public int? IdCuentaCliente { get; set; }

        [RequiredEx]
        public int? IdCuentaProveedor { get; set; }

        public SelectList MaestroTareasGrupo { get; set; }
        public SelectList MaestroTipoTareas { get; set; }
        public SelectList MaestroCuentasContables { get; set; }

    }
}
