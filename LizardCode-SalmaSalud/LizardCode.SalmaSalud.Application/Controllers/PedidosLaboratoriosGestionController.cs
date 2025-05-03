using LizardCode.Framework.Application.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Models.PedidosLaboratorios;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class PedidosLaboratoriosGestionController : BusinessController
    {
        public async Task<ActionResult> Index()
            => RedirectToAction("Gestion", "PedidosLaboratorios");
    }
}
