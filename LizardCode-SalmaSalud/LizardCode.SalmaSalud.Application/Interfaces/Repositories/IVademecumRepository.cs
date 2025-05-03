using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IVademecumRepository
    {
        Task<TVademecum> GetById<TVademecum>(int id, IDbTransaction transaction = null);
        Task<IList<TVademecum>> GetAll<TVademecum>(IDbTransaction transaction = null);
    }
}
