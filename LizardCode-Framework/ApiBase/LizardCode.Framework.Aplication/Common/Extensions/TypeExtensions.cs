using LizardCode.Framework.Helpers.Utilities;

namespace LizardCode.Framework.Application.Common.Extensions
{
    public static class TypeExtensions
    {
        #region Propiedades

        public static string DateTimeFormat
        {
            get
            {
                return "Views.DateTime.Formato".FromAppSettings("dd-MM-yyyy HH:mm");
            }
        }

        public static string DateFormat
        {
            get
            {
                return "Views.Date.Formato".FromAppSettings("dd-MM-yyyy");
            }
        }

        public static string DateNull
        {
            get
            {
                return "Views.Date.Formato.Nulo".FromAppSettings("N/A");
            }
        }

        #endregion

        #region Extensiones

        public static string ToDateTime(this DateTime dateTime) => dateTime.ToString(DateTimeFormat);
        public static string ToDateTime(this DateTime? dateTime) => dateTime.HasValue ? dateTime.Value.ToString(DateTimeFormat) : DateNull;
        public static string ToDate(this DateTime dateTime) => dateTime.ToString(DateFormat);
        public static string ToDate(this DateTime? dateTime) => dateTime.HasValue ? dateTime.Value.ToString(DateFormat) : DateNull;

        #endregion
    }
}