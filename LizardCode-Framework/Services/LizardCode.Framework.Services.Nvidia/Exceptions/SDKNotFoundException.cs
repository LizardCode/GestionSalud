using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.Framework.Services.Nvidia.Exceptions
{
    public class SDKNotFoundException : Exception
    {
        public SDKNotFoundException()
        {
        }

        public SDKNotFoundException(string? message) : base(message)
        {
        }
    }
}
