using GenerateClass.Dapper.Generator.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateClass.Dapper.Generator
{
    public class GenerateDomain : GenerateBase
    {
        public GenerateDomain(string connection) : base(connection)
        {
   
        }


        public void GenerateEntities(string directory, string nameSpace, List<string> selectedItems)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Domain")}\\{ResxSetFolders.GetString("NameSpace_Entities")}\\";

            foreach (var item in selectedItems)
            {
                var fileClass = $"{item.Split('.')[1]}";
                Generate(newDirectory, fileClass, GetEntities(nameSpace, fileClass));
            }
        }

        public void GenerateEntitiesCustom(string directory, string nameSpace, List<string> selectedItems)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Domain")}\\{ResxSetFolders.GetString("NameSpace_EntitiesCustom")}\\";

            foreach (var item in selectedItems)
            {
                var fileClass = $"{item.Split('.')[1]}Custom";
                Generate(newDirectory, fileClass, GetEntitiesCustom(nameSpace, fileClass));
            }
        }


        private StringBuilder GetEntities(string nameSpace, string gClass)
        {
            var columnsTable = Columns.GetAllColumn(gClass);
            var templates = new StringBuilder();

            templates.AppendLine($"using Dapper.Contrib.Extensions;");
            templates.AppendLine($"using System;");
            templates.AppendLine("");
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Domain")}.{ResxSetFolders.GetString("NameSpace_Entities")}");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}[Table(\"{ gClass}\")]");
            templates.AppendLine($"{Tabulador}public class {gClass}");
            templates.AppendLine($"{Tabulador}{LlaveAbre}");
            templates.AppendLine("");
            foreach (var itemColumn in columnsTable)
            {

                if (itemColumn.IsPrimaryKey)
                    templates.AppendLine($"{Tabulador}{Tabulador}[Key]");
              
               templates.AppendLine($"{Tabulador}{Tabulador}public virtual {itemColumn.ColumnTypeNet} {itemColumn.ColumnName} {LlaveAbre} get; set; {LlaveCierra}");
            }
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{LlaveCierra}");

            return templates;
        }


        private StringBuilder GetEntitiesCustom(string nameSpace, string gClass)
        {
            var columnsTable = Columns.GetAllColumn(gClass);

            var column = columnsTable.FirstOrDefault(x => x.IsPrimaryKey);

            var templates = new StringBuilder();

            templates.AppendLine($"using Dapper.Contrib.Extensions;");
            templates.AppendLine($"using System;");
            templates.AppendLine("");
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Domain")}.{ResxSetFolders.GetString("NameSpace_EntitiesCustom")}");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}public class {gClass}");
            templates.AppendLine($"{Tabulador}{LlaveAbre}");
            templates.AppendLine("");

            templates.AppendLine($"{Tabulador}{Tabulador}public virtual int {column} {LlaveAbre} get; set; {LlaveCierra}");
            templates.AppendLine($"{Tabulador}{Tabulador}public virtual string Description {LlaveAbre} get; set; {LlaveCierra}");
           
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{LlaveCierra}");

            return templates;
        }
    }
}
