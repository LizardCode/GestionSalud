namespace LizardCode.SalmaSalud.Appointments
{
    public interface IService
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}