namespace LizardCode.SalmaSalud.Application.Models.Plantillas
{
    public class BackgroundImage
    {
        public double Height { get; set; }
        public double Left { get; set; }
        public double Opacity { get; set; }
        public double Top { get; set; }
        public string Src { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public bool Visible { get; set; }
        public double Width { get; set; }

        public BackgroundImage(string src, double top, double left, double width, double height)
        {
            this.Height = height;
            this.Left = left;
            this.Opacity = 1;
            this.Src = src;
            this.Top = top;
            this.Type = "image";
            this.Version = "5.2.4";
            this.Visible = true;
            this.Width = width;
        }

    }
}
