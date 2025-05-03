using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom.Excel
{
    public class Resultado<T>
    {
        public bool Procesado { get; set; }
        public int Cantidad { get; set; }
        public string Error { get; set; }
        public List<T> Registros { get; set; }
    }
}
