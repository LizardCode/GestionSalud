using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPacientesRepository
    {
        Task<IList<TPaciente>> GetAll<TPaciente>(IDbTransaction transaction = null);

        Task<TPaciente> GetById<TPaciente>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPaciente>(TPaciente entity, IDbTransaction transaction = null);

        Task<bool> Update<TPaciente>(TPaciente entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<bool> ValidarCUITExistente(string cuit, int? id, IDbTransaction transaction = null);

        Task<Paciente> GetPacienteByCUIT(string cuit, IDbTransaction transaction = null);

        Task<Paciente> GetPacienteByDocumento(string documento, IDbTransaction transaction = null);
        Task<Domain.EntitiesCustom.Paciente> GetLikeDocumentoCustom(string documento, IDbTransaction transaction = null);
        Task<Domain.EntitiesCustom.Paciente> GetLikePhoneCustom(string phone, IDbTransaction transaction = null);
        Task<Domain.EntitiesCustom.Paciente> GetCustomById(int idPaciente, IDbTransaction transaction = null);
        Task<bool> ValidarDocumentoExistente(string documento, int? id, IDbTransaction transaction = null);
        Task<bool> ValidarNroFinanciadorExistente(string financiadorNro, int? id, IDbTransaction transaction = null);
    }
}