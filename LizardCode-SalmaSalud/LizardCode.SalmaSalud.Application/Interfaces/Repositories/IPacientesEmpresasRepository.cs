using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPacientesEmpresasRepository
    {
        Task<IList<TPacienteEmpresa>> GetAll<TPacienteEmpresa>(IDbTransaction transaction = null);

        Task<TPacienteEmpresa> GetById<TPacienteEmpresa>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPacienteEmpresa>(TPacienteEmpresa entity, IDbTransaction transaction = null);

        Task<bool> Update<TPacienteEmpresa>(TPacienteEmpresa entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdPaciente(int idPaciente, IDbTransaction transaction = null);

        Task<IList<PacienteEmpresa>> GetAllByIdPaciente(int idPaciente, IDbTransaction transaction = null);
    }
}
