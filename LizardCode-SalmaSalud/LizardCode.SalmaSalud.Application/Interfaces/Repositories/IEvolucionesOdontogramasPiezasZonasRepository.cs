using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
	public interface IEvolucionesOdontogramasPiezasZonasRepository
	{
		Task<IList<TEvolucionOdontogramaPiezaZona>> GetAll<TEvolucionOdontogramaPiezaZona>(IDbTransaction transaction = null);

		Task<TEvolucionOdontogramaPiezaZona> GetById<TEvolucionOdontogramaPiezaZona>(int id, IDbTransaction transaction = null);

		Task<long> Insert<TEvolucionOdontogramaPiezaZona>(TEvolucionOdontogramaPiezaZona entity, IDbTransaction transaction = null);

		Task<bool> Update<TEvolucionOdontogramaPiezaZona>(TEvolucionOdontogramaPiezaZona entity, IDbTransaction transaction = null);

		Task<IList<EvolucionOdontogramaPiezaZona>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);
	}
}
