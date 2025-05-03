using LizardCode.Framework.Services.Nvidia.Exceptions;
using LizardCode.Framework.Services.Nvidia.Interfaces;
using LizardCode.Framework.Services.Nvidia.Models;
using NvAPIWrapper;
using NvAPIWrapper.GPU;
using System.Text;

namespace LizardCode.Framework.Services.Nvidia
{
    public class NvidiaService : INvidiaService
    {
        public bool Initialized { get; private set; }
        public string Error { get; private set; }


        public NvidiaService()
        {
            Initialize();
        }


        private void Initialize()
        {
            Initialized = false;

            try
            {
                NVIDIA.Initialize();
                Initialized = true;
            }
            catch (DllNotFoundException)
            {
                Error = "Missing nVidia SDK (nvapi64.dll)";
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }
        }


        public string GPUSummary()
        {
            if (!Initialized)
                return default;

            var sb = new StringBuilder();
            var physicalGPUs = PhysicalGPU.GetPhysicalGPUs();

            if (physicalGPUs.Length == 0)
                throw new GPUNotFoundException();

            sb.AppendLine($"Detected nVidia GPUs: [{physicalGPUs.Length}]");

            foreach (var gpu in physicalGPUs)
            {
                sb.Append($" ++ ");
                sb.Append($"[GPUId: {gpu.GPUId}] ");
                sb.Append($"[Type: {gpu.GPUType}] ");
                sb.Append($"[Name: {gpu.FullName}] ");
                sb.Append($"[Memory: {gpu.MemoryInformation.RAMType.ToString()} {gpu.MemoryInformation.DedicatedVideoMemoryInkB / 1048576}Gb ]");
                sb.Append($" ++ ");
            }

            return sb.ToString();
        }

        public List<NvidiaGPU> GPUDetails()
        {
            if (!Initialized)
                return default;

            var physicalGPUs = PhysicalGPU.GetPhysicalGPUs();

            if (physicalGPUs.Length == 0)
                throw new GPUNotFoundException();

            return physicalGPUs
                .OrderBy(gpu => gpu.GPUId)
                .Select((gpu, iGPU) => new NvidiaGPU
                {
                    Id = Convert.ToInt32(gpu.GPUId),
                    LogicalId = iGPU,
                    Type = gpu.GPUType.ToString(),
                    Name = gpu.FullName,
                    MemoryType = gpu.MemoryInformation.RAMType.ToString(),
                    MemorySizeKb = Convert.ToInt32(gpu.MemoryInformation.DedicatedVideoMemoryInkB),
                    MemorySizeGb = Convert.ToInt32(gpu.MemoryInformation.DedicatedVideoMemoryInkB / 1048576),
                    MemoryAvailableKb = Convert.ToInt32(gpu.MemoryInformation.AvailableDedicatedVideoMemoryInkB),
                    MemoryAvailableGb = Convert.ToInt32(gpu.MemoryInformation.AvailableDedicatedVideoMemoryInkB / 1048576),
                    MemoryUsePercentage = 100 - Convert.ToInt32(gpu.MemoryInformation.CurrentAvailableDedicatedVideoMemoryInkB * 100 / gpu.MemoryInformation.AvailableDedicatedVideoMemoryInkB),
                    Temperature = gpu.ThermalInformation.CurrentThermalLevel,
                    Load = gpu.UsageInformation.GPU.Percentage,
                    InUse = 100 - gpu.MemoryInformation.CurrentAvailableDedicatedVideoMemoryInkB * 100 / gpu.MemoryInformation.AvailableDedicatedVideoMemoryInkB >= 80
                })
                .ToList();
        }
    }
}
