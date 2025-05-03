namespace LizardCode.SalmaSalud.Application.Models.Plantillas
{
    public class TagMapping
    {
        public int Id { get; set; }
        public string FontFamily { get; set; }
        public double FontSize { get; set; }
        public string FontStyle { get; set; }
        public string FontWeight { get; set; }
        public double Height { get; set; }
        public double Left { get; set; }
        public double LineHeight { get; set; }
        public bool Linethrough { get; set; }
        public double MinWidth { get; set; }
        public double Opacity { get; set; }
        public string Text { get; set; }
        public string TextAlign { get; set; }
        public double Top { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public bool Visible { get; set; }
        public double Width { get; set; }

        public bool LockRotation { get; set; }
        public bool LockScalingX { get; set; }
        public bool LockScalingY { get; set; }
        public bool LockSkewingX { get; set; }
        public bool LockSkewingY { get; set; }

        public TagMapping()
        {
            this.FontFamily = "Quicksand";
            this.FontSize = 15;
            this.FontStyle = "normal";
            this.FontWeight = "normal";
            this.LineHeight = 1.16;
            this.Linethrough = false;
            this.MinWidth = 20;
            this.Opacity = 1;
            this.TextAlign = "left";
            this.Type = "textbox";
            this.Version = "5.2.4";
            this.Visible = true;
            this.LockRotation = true;
            this.LockScalingX = true;
            this.LockScalingY = true;
            this.LockSkewingX = true;
            this.LockSkewingY = true;
        }
    }
}