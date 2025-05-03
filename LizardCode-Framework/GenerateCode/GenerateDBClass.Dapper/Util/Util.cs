using System.Windows.Forms;

namespace GenerateDBClass.Dapper
{
    public static class Util
    {

        public static void GenerateText(ListBox lbDetaill, string process, string text)
        {
            lbDetaill.Items.Add(string.Format("Generate {0} {1}", process, text));


        }

    }
}
