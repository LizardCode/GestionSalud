using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mysqlx.Cursor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Especialidades
{
    public class EspecialidadViewModel
    { 
        public int IdEspecialidad { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        public EspecialidadViewModel()
        {
        }
    }
}