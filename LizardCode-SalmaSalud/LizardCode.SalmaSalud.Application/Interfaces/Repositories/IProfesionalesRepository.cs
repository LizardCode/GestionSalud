using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IProfesionalesRepository
    {
        Task<IList<TProfesional>> GetAll<TProfesional>(IDbTransaction transaction = null);

        Task<TProfesional> GetById<TProfesional>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TProfesional>(TProfesional entity, IDbTransaction transaction = null);

        Task<bool> Update<TProfesional>(TProfesional entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<bool> ValidarCUITExistente(string cuit, int? id, int idEmpresa, IDbTransaction transaction = null);

        Task<List<Profesional>> GetAllProfesionalesByIdEmpresaLookup(int idEmpresa);

        Task<Profesional> GetProfesionalByCUIT(string cuit, IDbTransaction transaction = null);
    }
}