using LizardCode.SalmaSalud.Appointments.Domain.Entities;

namespace LizardCode.SalmaSalud.Appointments.Application.Interfaces.Business
{
    public interface IPresupuestosBusiness
    {
        Task<List<Presupuesto>> GetPresupuestosAVencer();
        Task MarcarVencido(Presupuesto turno);
    }
}
