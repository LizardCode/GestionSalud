using LizardCode.Framework.Helpers.Utilities;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class CierreMesBusiness : ICierreMesBusiness
    {
        private readonly IPermisosBusiness _permisosBusiness;
        private readonly ICierreMesRepository _cierreMesRepository;

        public CierreMesBusiness(IPermisosBusiness permisosBusiness, ICierreMesRepository cierreMesRepository)
        {
            _cierreMesRepository = cierreMesRepository;
            _permisosBusiness = permisosBusiness;
        }

        public async Task<bool> CierreMes(int idEjercicio, int anno, int mes, string modulo)
        {
            var cierreMes = await _cierreMesRepository.GetByAnnoMesModulo(idEjercicio, anno, mes, modulo, _permisosBusiness.User.IdEmpresa);

            if(cierreMes == default)
            {
                throw new BusinessException("El Mes/Año de Cierre para el Ejercicio seleccionado No Existe.");
            }

            cierreMes.Cierre = cierreMes.Cierre == Commons.Si.Description() ? Commons.No.Description() : Commons.Si.Description();

            return await _cierreMesRepository.Update(cierreMes);
        }

        public async Task<List<Custom.CierreMes>> GetDetalle(int id)
        {
            var cierresMes = await _cierreMesRepository.GetAllByIdEjercicio(id, _permisosBusiness.User.IdEmpresa);
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;

            foreach (var cierre in cierresMes)
            {
                var nombreMes = dtinfo.GetMonthName(cierre.Mes);
                cierre.NombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1);
            }

            return cierresMes;
        }
    }
}
