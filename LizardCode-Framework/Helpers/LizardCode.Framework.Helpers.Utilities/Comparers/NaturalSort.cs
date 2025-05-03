using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LizardCode.Framework.Helpers.Utilities.Comparers
{
    //
    // https://stackoverflow.com/a/40943999/1812392
    //
    public class NaturalSort : IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}
