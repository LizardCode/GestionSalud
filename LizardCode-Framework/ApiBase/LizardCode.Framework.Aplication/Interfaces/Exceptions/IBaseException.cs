namespace LizardCode.Framework.Application.Interfaces.Exceptions
{
    public interface IBaseException
    {
        int Code { get; }
        string Message { get; }
    }
}
