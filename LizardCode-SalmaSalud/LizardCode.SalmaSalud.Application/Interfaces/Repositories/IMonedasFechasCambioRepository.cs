using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IMonedasFechasCambioRepository
    {
        Task<IList<TMonedaFechasCambio>> GetAll<TMonedaFechasCambio>(IDbTransaction transaction = null);

        Task<TMonedaFechasCambio> GetById<TMonedaFechasCambio>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TMonedaFechasCambio>(TMonedaFechasCambio entity, IDbTransaction transaction = null);

        Task<bool> Update<TMonedaFechasCambio>(TMonedaFechasCambio entity, IDbTransaction transaction = null);

        Task<MonedaFechasCambio> GetByIdAndFecha(int idMoneda, DateTime fecha, int idEmpresa);
        
        Task<double?> GetFechaCambio(int idMoneda, DateTime fecha, int idEmpresa);

        Task<double?> GetFechaCambioByCodigo(string idMoneda, DateTime fecha, int idEmpresa);

        Task<IList<MonedaFechasCambio>> GetCotizaciones(int idMoneda, int idEmpresa);

        Task<double?> GetFechaCambioByCodigo(string idMoneda, DateTime fecha, int idEmpresa, IDbTransaction transaction = null);
    }
}