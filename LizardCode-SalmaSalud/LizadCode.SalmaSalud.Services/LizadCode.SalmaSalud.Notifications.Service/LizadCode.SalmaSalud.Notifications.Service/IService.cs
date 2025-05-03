namespace LizadCode.SalmaSalud.Notifications
{
    public interface IService
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}