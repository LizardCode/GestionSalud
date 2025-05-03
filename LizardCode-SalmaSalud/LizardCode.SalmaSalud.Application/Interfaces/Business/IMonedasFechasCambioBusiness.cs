using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.MonedasFechasCambio;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IMonedasFechasCambioBusiness
    {
        Task<MonedasFechasCambioViewModel> Get(int idMoneda);
        Task<DataTablesResponse<Moneda>> GetAll(DataTablesRequest request);
        Task Update(MonedasFechasCambioViewModel model);
        Task<double?> GetFechaCambio(int idMoneda, DateTime fecha);
        Task<IList<MonedaFechasCambio>> GetCotizaciones(int id);
    }
}