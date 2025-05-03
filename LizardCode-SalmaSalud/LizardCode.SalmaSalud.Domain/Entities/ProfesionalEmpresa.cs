using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ProfesionalesEmpresas")]
    public class ProfesionalEmpresa
    {
        [ExplicitKey]
        public virtual int IdProfesional { get; set; }
        [ExplicitKey]
        public virtual int IdEmpresa { get; set; }

    }
}
