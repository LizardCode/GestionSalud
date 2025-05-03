using Microsoft.AspNetCore.Mvc;

namespace LizardCode.Framework.Application.Common.Annotations
{
    public class RepeaterRemoteAttribute : RemoteAttribute
    {
        private const string Message = "Valor incorrecto";

        public string Action { get; }
        public string Controller { get; }


        public RepeaterRemoteAttribute(string action, string controller) : base(action, controller)
        {
            Action = action;
            Controller = controller;
            ErrorMessage = Message;
            HttpMethod = "POST";
        }
    }
}
