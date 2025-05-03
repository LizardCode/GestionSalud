using GenerateClass.Dapper.Data;
using System.IO;
using System.Resources;
using System.Text;

namespace GenerateClass.Dapper.Generator.Base
{
    public class GenerateBase: GenerateConst
    {
        private const string ResxFileFolders = @".\Temp\ResourceFolders.resx";
        protected ResXResourceSet ResxSetFolders { get; set; }
        protected  string connection { get; set; }

        protected ColumnData Columns { get; set; }

        public GenerateBase(string connectionBase)
        {
            ResxSetFolders = new ResXResourceSet(ResxFileFolders);
            connection = connectionBase;
            Columns = new ColumnData(connection);
        }


        protected void GenerateFile(string filename, string directory, StringBuilder classTemplate)
        {
            string dirPath = Path.GetDirectoryName(directory);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            var file = new StreamWriter($"{dirPath}\\{filename}.cs");
            file.Write(classTemplate.ToString());          
            file.Close(); 
        }

        protected void Generate(string directory, string fileName, StringBuilder classTemplate)
        {
            GenerateFile(fileName, directory, classTemplate);
        }


    }
}
