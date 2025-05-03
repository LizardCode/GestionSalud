using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Ejercicios;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IEjerciciosBusiness
    {
        Task<EjerciciosViewModel> Get(int idEjercicio);
        Task<DataTablesResponse<Ejercicio>> GetAll(DataTablesRequest request);
        Task New(EjerciciosViewModel model);
        Task<string> ValidarFecha(string mesAnno, bool esFechaFin);
    }
}