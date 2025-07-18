using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IMailBusiness
    {
        Task EnviarMailBienvenidaPaciente(string pacienteEmail, string pacienteNombre);

        //Task EnviarMailAutogestionPaciente(string pacienteHash, string pacienteEmail, string pacienteNombre);
        Task EnviarMailCodigoAccesoPaciente(string codigo, string pacienteEmail, string pacienteNombre);
        Task EnviarMailRecetasPaciente(string pacienteEmail, string pacienteNombre, Dictionary<string, string> recetas);
        Task EnviarMailSolicitudTurnoCanceladaPaciente(string pacienteEmail, string pacienteNombre, string especialidad);
        Task EnviarMailTurnoAsignadoPaciente(string pacienteEmail, string pacienteNombre, string fechaAsigancion, string especialidad, string observaciones);
        Task EnviarMailTurnoReAsignadoPaciente(string pacienteEmail, string pacienteNombre, string fechaAsigancion, string especialidad, string observaciones);
    }
}