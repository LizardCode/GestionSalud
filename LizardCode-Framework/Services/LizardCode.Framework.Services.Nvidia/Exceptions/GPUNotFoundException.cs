using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.Framework.Services.Nvidia.Exceptions
{
    public class GPUNotFoundException : Exception
    {
        public GPUNotFoundException()
        {
        }

        public GPUNotFoundException(string? message) : base(message)
        {
        }
    }
}
