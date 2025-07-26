using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Home
{
    public class HomeViewModel
    {
        public int IdEspecialidad { get; set; }    
        public SelectList MaestroEspecialidades { get; set; }
    }
}
