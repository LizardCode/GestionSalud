using GenerateClass.Dapper.Generator.Base;
using System.Collections.Generic;
using System.Text;

namespace GenerateClass.Dapper.Generator
{
    public class GenerateInfractructure : GenerateBase
    {
        public GenerateInfractructure(string connection) : base(connection)
        {
   
        }


        public void GenerateClassInfractructure(string directory, string nameSpace, List<string> selectedItems)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Infrastructure")}\\{ResxSetFolders.GetString("NameSpace_Repositories")}\\";

            foreach (var item in selectedItems)
            {
                var fileClass = $"{item.Split('.')[1]}";
                var fileName = $"{item.Split('.')[1]}{ResxSetFolders.GetString("Class_Repository")}";
                Generate(newDirectory, fileName, GetRepository(nameSpace, fileClass));
            }
        }

        public void GenerateInjection(string directory, string nameSpace, List<string> selectedItems)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Infrastructure")}\\";

            Generate(newDirectory, "Injection", GetInjection(nameSpace, selectedItems));
        }


        private StringBuilder GetRepository(string nameSpace, string gClass)
        {
            var columnsTable = Columns.GetAllColumn(gClass);
            var templates = new StringBuilder();

            templates.AppendLine($"using Dapper.DataTables.Interfaces;");
            templates.AppendLine($"using Dapper.DataTables.Models;");
            templates.AppendLine($"using DapperQueryBuilder;");
            templates.AppendLine($"using {nameSpace}.Application.Interfaces.Repositories;");
            templates.AppendLine($"using LizardCode.Framework.Infrastructure.Interfaces.Context;");
            templates.AppendLine($"using LizardCode.Framework.Infrastructure.Repositories;");
            templates.AppendLine("");
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Infrastructure")}.{ResxSetFolders.GetString("NameSpace_Repositories")}");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}public  class {gClass}Repository : BaseRepository, I{gClass}Repository, IDataTablesCustomQuery");
            templates.AppendLine($"{Tabulador}{LlaveAbre}");
            templates.AppendLine("");

            templates.AppendLine($"{Tabulador}{Tabulador}public {gClass}Repository(IDbContext context) : base(context)");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            templates.AppendLine("");

            templates.AppendLine($"{Tabulador}{Tabulador}public DataTablesCustomQuery GetAllCustomQuery()");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var query = _context.Connection");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}.QueryBuilder($@\"");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}SELECT");
            foreach (var itemColumn in columnsTable)
            {
                if (columnsTable.IndexOf(itemColumn) == columnsTable.Count - 1)
                    templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}[{itemColumn.ColumnName}]");
                else
                    templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}[{itemColumn.ColumnName}],");
            }
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador} FROM dbo.{gClass}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador} \");");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador} return base.GetAllCustomQuery(query);");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{LlaveCierra}");

            return templates;
        }


        private StringBuilder GetInjection(string nameSpace, List<string> selectedItems)
        {

            var templates = new StringBuilder();

            templates.AppendLine($"using Dapper.DataTables.Extensions;");
            templates.AppendLine($"using {nameSpace}.Application.Interfaces.Repositories;");
            templates.AppendLine($"using {nameSpace}.Application.Interfaces.Services;");
            templates.AppendLine($"using {nameSpace}.Infrastructure.Context;");
            templates.AppendLine($"using {nameSpace}.Infrastructure.Interfaces.Context;");
            templates.AppendLine($"using {nameSpace}.Infrastructure.Repositories;");
            templates.AppendLine($"using {nameSpace}.Infrastructure.Services;");
            templates.AppendLine($"using Microsoft.Extensions.Configuration;");
            templates.AppendLine($"using Microsoft.Extensions.DependencyInjection;");

            templates.AppendLine("");
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Infrastructure")}");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}public static class Injection");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");

            templates.AppendLine($"{Tabulador}{Tabulador}public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}services.AddScoped<IDbContext, DbContext>();");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}services.AddScoped<IUnitOfWork, UnitOfWork>();");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}services.AddDapperDataTables<IDbContext>();");
            templates.AppendLine("");
            foreach (var item in selectedItems)
            {
                var itemClass = item.Split('.')[1];
                templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}services.AddScoped<I{itemClass}Repository, {itemClass}Repository>();");
            }

            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            templates.AppendLine("");

            return templates;


        }
    }
}
