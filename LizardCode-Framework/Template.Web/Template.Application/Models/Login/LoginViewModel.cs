namespace Template.Application.Models.Login
{
    public class LoginViewModel
    {
        public string User { get; set; }

        public string Pass { get; set; }

        public string NewPass { get; set; }

        public string RepeatPass { get; set; }


        #region Campos LogBrowser

        public string OS { get; set; }
        public string OSVer { get; set; }
        public string Mobile { get; set; }
        public string Browser { get; set; }
        public string BrowserMajorVer { get; set; }
        public string BrowserVer { get; set; }
        public string Cookies { get; set; }
        public string ScreenWidth { get; set; }
        public string ScreenHeight { get; set; }

        #endregion
    }
}