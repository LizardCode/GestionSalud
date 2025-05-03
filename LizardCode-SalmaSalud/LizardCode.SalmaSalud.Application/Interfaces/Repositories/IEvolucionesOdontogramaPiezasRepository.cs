using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
	public interface IEvolucionesOdontogramasPiezasRepository
	{
		Task<IList<TEvolucionOdontogramaPieza>> GetAll<TEvolucionOdontogramaPieza>(IDbTransaction transaction = null);

		Task<TEvolucionOdontogramaPieza> GetById<TEvolucionOdontogramaPieza>(int id, IDbTransaction transaction = null);

		Task<long> Insert<TEvolucionOdontogramaPieza>(TEvolucionOdontogramaPieza entity, IDbTransaction transaction = null);

		Task<bool> Update<TEvolucionOdontogramaPieza>(TEvolucionOdontogramaPieza entity, IDbTransaction transaction = null);

		Task<IList<EvolucionOdontogramaPieza>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);
	}
}
