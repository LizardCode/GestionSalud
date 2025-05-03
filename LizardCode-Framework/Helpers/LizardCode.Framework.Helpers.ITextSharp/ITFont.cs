using LizardCode.Framework.Helpers.ITextSharp.Properties;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;

namespace LizardCode.Framework.Helpers.ITextSharp
{
    public class ITFont
    {
        public float Size { get; set; }

        public Color Color { get; set; }

        public bool Bold { get; set; }

        public BaseFont BaseFont { get; private set; }

        public static ITFont Helvetica
        {
            get
            {
                return new ITFont { Size = 16f, BaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true) };
            }
        }

        /// <summary>
        /// Cousine Monospace
        /// </summary>
        public static ITFont Cousine
        {
            get
            {
                return FromResource("Cousine");
            }
        }

        public static ITFont Courier
        {
            get
            {
                return new ITFont { Size = 16f, BaseFont = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, true) };
            }
        }

        /// <summary>
        /// BPmono Monospace
        /// </summary>
        public static ITFont BPMono
        {
            get
            {
                return FromResource("BPmono");
            }
        }

        public static ITFont Times
        {
            get
            {
                return new ITFont { Size = 16f, BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, true) };
            }
        }

        public static ITFont Arial
        {
            get
            {
                return FromSystem("Arial");
            }
        }


        public ITFont()
        {
            Size = 16f;
            Color = Color.Black;
            Bold = false;
            BaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
        }


        public static ITFont FromBaseFont(BaseFont baseFont, float size)
        {
            return new ITFont { BaseFont = baseFont, Size = size };
        }


        private static ITFont FromResource(string name)
        {
            try
            {
                var buffer = (byte[])Resources.ResourceManager.GetObject(name);
                var font = BaseFont.CreateFont(name + ".ttf", BaseFont.CP1252, BaseFont.EMBEDDED, BaseFont.CACHED, buffer, null);

                return new ITFont
                {
                    Size = 16f,
                    BaseFont = font
                };
            }
            catch
            {
                return Helvetica;
            }
        }

        private static ITFont FromSystem(string name)
        {
            try
            {
                var font = FontFactory.GetFont(name);

                return new ITFont
                {
                    Size = 16f,
                    BaseFont = font.BaseFont
                };
            }
            catch
            {
                return Helvetica;
            }
        }
    }
}