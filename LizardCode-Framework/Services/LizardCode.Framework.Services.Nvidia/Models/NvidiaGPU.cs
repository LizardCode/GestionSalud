namespace LizardCode.Framework.Services.Nvidia.Models
{
    public class NvidiaGPU
    {
        public int Id { get; init; }
        public int LogicalId { get; init; }
        public string Type { get; init; }
        public string Name { get; init; }
        public string MemoryType { get; init; }
        public int MemorySizeKb { get; init; }
        public int MemorySizeGb { get; init; }
        public int MemoryAvailableKb { get; init; }
        public int MemoryAvailableGb { get; init; }
        public int MemoryUsePercentage { get; init; }
        public int Temperature { get; init; }
        public int Load { get; init; }
        public bool InUse { get; init; }
    }
}
