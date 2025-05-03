using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IARBARepository
    {
        Task<ARBA> GetByCUITFechaVig(string regimen, string cuit, DateTime fecha, IDbTransaction transaction = null);

    }
}