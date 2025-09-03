using LizardCode.Framework.Helpers.ChatApi.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Helpers
{
    public interface IWAppApiHelper
    {
        Task<WAppApiResponse> SendMessage(string phone, string message, CancellationToken cancellationToken = default);
    }

    public class WAppApiResponse
    {
        public string Status { get; set; }
        public WAppApiResponseData Data { get; set; }
    }

    public class WAppApiResponseData
    {
        public string Status { get; set; }
        public string InstanceId { get; set; }
    }
}
