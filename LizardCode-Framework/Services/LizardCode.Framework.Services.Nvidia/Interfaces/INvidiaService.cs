using LizardCode.Framework.Services.Nvidia.Models;

namespace LizardCode.Framework.Services.Nvidia.Interfaces
{
    public interface INvidiaService
    {
        bool Initialized { get; }
        string Error { get; }

        List<NvidiaGPU> GPUDetails();
        string GPUSummary();
    }
}