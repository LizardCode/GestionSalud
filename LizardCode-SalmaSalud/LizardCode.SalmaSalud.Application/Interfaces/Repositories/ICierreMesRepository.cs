using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICierreMesRepository
    {
        Task<IList<TCierreMes>> GetAll<TCierreMes>(IDbTransaction transaction = null);

        Task<TCierreMes> GetById<TCierreMes>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCierreMes>(TCierreMes entity, IDbTransaction transaction = null);

        Task<bool> Update<TCierreMes>(TCierreMes entity, IDbTransaction transaction = null);

        Task<List<Custom.CierreMes>> GetAllByIdEjercicio(int id, int idEmpresa);

        Task<CierreMes> GetByAnnoMesModulo(int idEjercicio, int anno, int mes, string modulo, int idEmpresa);

        Task<bool> MesCerrado(int idEjercicio, DateTime fecha, string modulo, int idEmpresa);
    }
}