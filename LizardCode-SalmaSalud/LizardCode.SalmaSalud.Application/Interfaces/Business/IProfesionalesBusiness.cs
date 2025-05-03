using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.FullCalendar;
using LizardCode.SalmaSalud.Application.Models.Profesionales;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IProfesionalesBusiness
    {
        Task<ProfesionalViewModel> Get(int idProfesional);
        Task<DataTablesResponse<Custom.Profesional>> GetAll(DataTablesRequest request);
        Task New(ProfesionalViewModel model);
        Task Remove(int idProfesional);
        Task Update(ProfesionalViewModel model);
        Task<string> ValidarNroCUIT(string cuit, int? idProfesional);
        Task<Custom.Contribuyente> GetPadron(string cuit);
        Task<List<ProfesionalTurnoEvent>> GetAgenda(DateTime desde, DateTime hasta, int idProfesional);
        Task AddTurno(int idProfesional, DateTime fechaInicio);
        Task RemoveTurno(int idProfesionalTurno);
        Task<int> HabilitarCargaAgenda(int idProfesional);

        Task CopiarSemana(int idProfesional, DateTime desde);
        Task CopiarDia(int idProfesional, DateTime dia);
        Task<ProfesionalTurno> GetProfesionalTurnoById(int idProfesionalTurno);
    }
}