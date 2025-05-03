using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Usuario : Entities.Usuario
    {
        public List<UsuarioEmpresa> Empresas { get; set; }
    }
}
