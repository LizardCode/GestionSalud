using GenerateClass.Dapper.Generator.Base;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenerateClass.Dapper.Generator
{
    public class GenerateApplication : GenerateBase
    {

        public GenerateApplication(string connection) : base(connection)
        { 
        }

        public void GenerateBusiness(string directory, string nameSpace, List<string> selectedItems)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}\\{ResxSetFolders.GetString("NameSpace_Business")}\\";
    
            foreach (var item in selectedItems)
            {
                var fileClass = $"{item.Split('.')[1]}";
                var fileName = $"{item.Split('.')[1]}{ResxSetFolders.GetString("Class_Business")}";
                Generate(newDirectory, fileName, GetClassBusiness(nameSpace, fileClass));
            }
        }

        public void GenerateInterfaceBusiness(string directory, string nameSpace, List<string> selectedItems)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}\\{ResxSetFolders.GetString("NameSpace_Interfaces")}\\{ResxSetFolders.GetString("NameSpace_Business")}\\";

            foreach (var item in selectedItems)
            {
                var fileClass = $"{item.Split('.')[1]}";
                var fileName = $"I{item.Split('.')[1]}{ResxSetFolders.GetString("Class_Business")}";
                Generate(newDirectory, fileName, GetInterfaceBusiness(nameSpace, fileClass));
            }
        }

        public void GenerateIBaseRepository(string directory, string nameSpace)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}\\{ResxSetFolders.GetString("NameSpace_Interfaces")}\\{ResxSetFolders.GetString("NameSpace_Repositories")}\\{ResxSetFolders.GetString("NameSpace_Base")}\\";

            Generate(newDirectory, "IBaseRepository", GetBaseInterfaceRepository(nameSpace, "IBaseRepository"));      
        }

        public void GenerateInterfaceRepository(string directory, string nameSpace, List<string> selectedItems)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}\\{ResxSetFolders.GetString("NameSpace_Interfaces")}\\{ResxSetFolders.GetString("NameSpace_Repositories")}\\";

            foreach (var item in selectedItems)
            {
                var fileClass = $"{item.Split('.')[1]}";
                var fileName = $"I{item.Split('.')[1]}{ResxSetFolders.GetString("Class_Repository")}";
                Generate(newDirectory, fileName, GetInterfaceRepository(nameSpace, fileClass));
            }
        }

        public void GenerateModels(string directory, string nameSpace, List<string> selectedItems)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}\\{ResxSetFolders.GetString("NameSpace_Models")}\\";

            foreach (var item in selectedItems)
            {
                var fileClass = $"{item.Split('.')[1]}";
                var fileName = $"{item.Split('.')[1]}{ResxSetFolders.GetString("Class_ViewModel")}";
                Generate(newDirectory, fileName, GetModels(nameSpace, fileClass));
            }
        }

        public void GenerateControllers(string directory, string nameSpace, List<string> selectedItems)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}\\{ResxSetFolders.GetString("NameSpace_Controllers")}\\";

            foreach (var item in selectedItems)
            {
                var fileClass = $"{item.Split('.')[1]}";
                var fileName = $"{item.Split('.')[1]}{ResxSetFolders.GetString("Class_Controller")}";
                Generate(newDirectory, fileName, GetControllers(nameSpace, fileClass));
            }
        }

        public void GenerateInjection(string directory, string nameSpace, List<string> selectedItems)
        {
            var newDirectory = $"{directory}\\{nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}\\";

            Generate(newDirectory, "Injection", GetInjection(nameSpace, selectedItems));
        }

        #region Template

        private StringBuilder GetClassBusiness(string nameSpace, string gClass)
        {

            var columnsTable = Columns.GetAllColumn(gClass);

            var columnID = columnsTable.FirstOrDefault(x => x.IsPrimaryKey);

            var templates = new StringBuilder();

            templates.AppendLine($"using Dapper.DataTables.Interfaces;");
            templates.AppendLine($"using Dapper.DataTables.Models;");
            templates.AppendLine($"using LizardCode.Framework.Aplication.Models.ItemSelect;");
            templates.AppendLine($"using {nameSpace}.Application.Interfaces.Business;");
            templates.AppendLine($"using {nameSpace}.Application.Interfaces.Repositories;");
            templates.AppendLine($"using {nameSpace}.Application.Models;");
            templates.AppendLine($"using {nameSpace}.Domain.Entities;");
            templates.AppendLine($"using {nameSpace}.Domain.EntitiesCustom;");
            templates.AppendLine($"using LizardCode.Framework.Infrastructure.Interfaces.Context;");
            templates.AppendLine($"using MapsterMapper;");
            templates.AppendLine($"using System;");
            templates.AppendLine($"using System.Collections.Generic;");
            templates.AppendLine($"using System.Linq;");
            templates.AppendLine($"using System.Threading.Tasks;");
            templates.AppendLine("");
            //namespace
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}.{ResxSetFolders.GetString("NameSpace_Business")}");
            templates.AppendLine($"{LlaveAbre}");
            //Class
            templates.AppendLine($"{Tabulador}public class {gClass}{ResxSetFolders.GetString("Class_Business")} : I{gClass}{ResxSetFolders.GetString("Class_Business")}");
            templates.AppendLine($"{Tabulador}{LlaveAbre}");
            //Variables Globales
            templates.AppendLine($"{Tabulador}{Tabulador}private readonly IMapper _mapper;");
            templates.AppendLine($"{Tabulador}{Tabulador}private readonly IDbContext _dbContext;");
            templates.AppendLine($"{Tabulador}{Tabulador}private readonly IUnitOfWork _uow;");
            templates.AppendLine($"{Tabulador}{Tabulador}private readonly IDataTablesService _dataTablesService;");
            templates.AppendLine($"{Tabulador}{Tabulador}private readonly I{gClass}Repository _{gClass.ToLower()}Repository;");
            templates.AppendLine("");
            // Constructor
            templates.AppendLine($"{Tabulador}{Tabulador}public {gClass}Business(");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}IMapper mapper,");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}IDbContext dbContext,");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}IUnitOfWork uow,");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}IDataTablesService dataTablesService,");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}I{gClass}Repository {gClass.ToLower()}Repository)");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}_mapper = mapper;");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}_dbContext = dbContext;");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}_uow = uow;");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}_dataTablesService = dataTablesService;");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}_{gClass.ToLower()}Repository = {gClass.ToLower()}Repository;");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");

            //Metodo New
            templates.AppendLine($"{Tabulador}{Tabulador}public async Task New({gClass}{ResxSetFolders.GetString("Class_ViewModel")} model)");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var item = _mapper.Map<{gClass}>(model);");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var datetime = DateTime.Now;");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}Validate(item);");
            templates.AppendLine("");

            foreach (var itemColumn in columnsTable)
            {
                if (itemColumn.ColumnTypeNet == "string")
                     templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador} item.{itemColumn.ColumnName} = item.{itemColumn.ColumnName}?.ToUpper().Trim();");
            }

            templates.AppendLine("");

            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var tran = _uow.BeginTransaction();");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}await _{gClass.ToLower()}Repository.Insert(item, tran);");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}tran.Commit();");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            //Fin metodo New
            templates.AppendLine("");

            //Metodo Get
            templates.AppendLine($"{Tabulador}{Tabulador}public async Task<{gClass}{ResxSetFolders.GetString("Class_ViewModel")}> Get(int id)");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var item = await _{gClass.ToLower()}Repository.GetById<{gClass}>(id);");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}if (item == null)");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}return null;");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var model = _mapper.Map<{gClass}{ResxSetFolders.GetString("Class_ViewModel")}>(item);");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}return model;");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            //Fin metodo Get
            templates.AppendLine("");

            //Metodo Get all Grilla
            templates.AppendLine($"{Tabulador}{Tabulador}public async Task<DataTablesResponse<{gClass}Custom>> GetAll(DataTablesRequest request)");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var customQuery = _{gClass.ToLower()}Repository.GetAllCustomQuery();");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}return await _dataTablesService.Resolve<{gClass}Custom>(request, customQuery.Sql, customQuery.Parameters);");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            //Fin metodo Get all Grilla
            templates.AppendLine("");

            //Metodo Update

            var id = columnsTable.FirstOrDefault(c=>c.IsPrimaryKey);

            templates.AppendLine($"{Tabulador}{Tabulador}public async Task Update({gClass}{ResxSetFolders.GetString("Class_ViewModel")} model)");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var item = _mapper.Map<{gClass}>(model);");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}Validate(item);");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var dbItem = await _{gClass.ToLower()}Repository.GetById<{gClass}>(item.{id.ColumnName});");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}if (dbItem == null)");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}throw new ArgumentException(\"Item inexistente\");");
            templates.AppendLine($" {Tabulador}{Tabulador}{Tabulador}{LlaveCierra}");
            templates.AppendLine("");

            foreach (var itemColumn in columnsTable)
            {
                if (!itemColumn.IsIdentity)
                {
                    if (itemColumn.ColumnTypeNet != "string")
                        templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador} dbItem.{itemColumn.ColumnName} = item.{itemColumn.ColumnName};");
                    else
                        templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador} dbItem.{itemColumn.ColumnName} = item.{itemColumn.ColumnName}?.ToUpper().Trim();");
                }
            }

            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}using var tran = _uow.BeginTransaction();");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}await _{gClass.ToLower()}Repository.Update(dbItem, tran);");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}tran.Commit();");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            //Fin metodo Update
            templates.AppendLine("");

            //Metodo Remove
            templates.AppendLine($"{Tabulador}{Tabulador}public async Task Remove(int id)");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var item = await _{gClass.ToLower()}Repository.GetById<{gClass}>(id);");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}if (item == null)");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}throw new ArgumentException(\"Item inexistente\");");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{LlaveCierra}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}using var tran = _uow.BeginTransaction();");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}item.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}await _{gClass.ToLower()}Repository.Update(item, tran);");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}tran.Commit();");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            //Fin metodo Update
            templates.AppendLine("");

            //GetAllSelect

            var allselectID = columnsTable.FirstOrDefault(s=>s.ColumnName == columnID.ColumnName);
            var allselectDescription = columnsTable.FirstOrDefault(s => s.ColumnName == "Description" || s.ColumnName == "Descripcion");

            templates.AppendLine($"{Tabulador}{Tabulador}public async Task<List<ItemViewModel>> GetAllSelect()");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");

            if (allselectID != null && allselectDescription != null &&
                !string.IsNullOrEmpty(allselectID.ColumnName) && !string.IsNullOrEmpty(allselectDescription.ColumnName))
            {
                templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var items = await _{gClass.ToLower()}Repository.GetAll<{gClass}>();");
                templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}return items.Select(c => new ItemViewModel {LlaveAbre} Id = c.{allselectID.ColumnName}, Description = c.{allselectDescription.ColumnName} {LlaveCierra}).OrderBy(o => o.Description).ToList();");
            }
            else
            {
                templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}return await Task.Run(() => new List<ItemViewModel>());");
            }
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            //Fin metodo GetAllSelect
            templates.AppendLine("");

            //Metodo Validacion
            templates.AppendLine($"{Tabulador}{Tabulador}private void Validate({gClass} item)");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            //Fin metodo Validacion
            templates.AppendLine("");

            templates.AppendLine($"{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{LlaveCierra}");
            return templates;
        }

        private StringBuilder GetInterfaceBusiness(string nameSpace, string gClass)
        {

            var templates = new StringBuilder();

            templates.AppendLine($"using Dapper.DataTables.Models;");
            templates.AppendLine($"using {nameSpace}.Application.Models;");
            templates.AppendLine($"using LizardCode.Framework.Aplication.Models.ItemSelect;");
            templates.AppendLine($"using {nameSpace}.Domain.EntitiesCustom;");
            templates.AppendLine($"using System.Threading.Tasks;");
            templates.AppendLine($"using System.Collections.Generic;");
            templates.AppendLine("");
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}.{ResxSetFolders.GetString("NameSpace_Interfaces")}.{ResxSetFolders.GetString("NameSpace_Business")}");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}public interface I{gClass}{ResxSetFolders.GetString("Class_Business")}");
            templates.AppendLine($"{Tabulador}{LlaveAbre}");

            templates.AppendLine($"{Tabulador}{Tabulador}Task<{gClass}{ResxSetFolders.GetString("Class_ViewModel")}> Get(int id);");
            templates.AppendLine($"{Tabulador}{Tabulador}Task<DataTablesResponse<{gClass}Custom>> GetAll(DataTablesRequest request);");
            templates.AppendLine($"{Tabulador}{Tabulador}Task<List<ItemViewModel>> GetAllSelect();");
            templates.AppendLine($"{Tabulador}{Tabulador}Task New({gClass}{ResxSetFolders.GetString("Class_ViewModel")} model);");
            templates.AppendLine($"{Tabulador}{Tabulador}Task Remove(int id);");
            templates.AppendLine($"{Tabulador}{Tabulador}Task Update({gClass}{ResxSetFolders.GetString("Class_ViewModel")} model);");

            templates.AppendLine($"{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{LlaveCierra}");
            return templates;
        }

        private StringBuilder GetBaseInterfaceRepository(string nameSpace, string gClass)
        {

            var templates = new StringBuilder();

            templates.AppendLine($"using System.Collections.Generic;");
            templates.AppendLine($"using System.Data;");
            templates.AppendLine($"using System.Threading.Tasks;");
            templates.AppendLine("");
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}.{ResxSetFolders.GetString("NameSpace_Interfaces")}.{ResxSetFolders.GetString("NameSpace_Repositories")}.{ResxSetFolders.GetString("NameSpace_Base")}");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}public interface {gClass}");
            templates.AppendLine($"{Tabulador}{LlaveAbre}");

            templates.AppendLine($"{Tabulador}{Tabulador}Task<IList<T>> GetAll<T>();");
            templates.AppendLine($"{Tabulador}{Tabulador}Task<T> GetById<T>(int id);");
            templates.AppendLine($"{Tabulador}{Tabulador}Task<long> Insert<T>(T entity, IDbTransaction transaction = null);");
            templates.AppendLine($"{Tabulador}{Tabulador}Task<bool> Update<T>(T entity, IDbTransaction transaction = null);");
            templates.AppendLine($"{Tabulador}{Tabulador}Task<bool> Delete<T>(T entity, IDbTransaction transaction = null);");
            templates.AppendLine($"{Tabulador}{Tabulador}Task<bool> DeleteById<T>(int id, IDbTransaction transaction = null);");
            templates.AppendLine($"{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{LlaveCierra}");
            return templates;
        }

        private StringBuilder GetInterfaceRepository(string nameSpace, string gClass)
        {

            var templates = new StringBuilder();

            templates.AppendLine($"using Dapper.DataTables.Models;");
            templates.AppendLine($"using LizardCode.Framework.Aplication.Interfaces.Repositories;");
            templates.AppendLine("");
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}.{ResxSetFolders.GetString("NameSpace_Interfaces")}.{ResxSetFolders.GetString("NameSpace_Repositories")}");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}public interface I{gClass}{ResxSetFolders.GetString("Class_Repository")} : IBaseRepository");
            templates.AppendLine($"{Tabulador}{LlaveAbre}");

            templates.AppendLine($"{Tabulador}{Tabulador}DataTablesCustomQuery GetAllCustomQuery();");

            templates.AppendLine($"{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{LlaveCierra}");
            return templates;
        }

        private StringBuilder GetModels(string nameSpace, string gClass)
        {
            var columnsTable = Columns.GetAllColumn(gClass);
            var templates = new StringBuilder();

            templates.AppendLine($"using Microsoft.AspNetCore.Mvc.Rendering;");
            templates.AppendLine($"using System;");
            templates.AppendLine($"using System.ComponentModel.DataAnnotations;");
            templates.AppendLine("");
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}.{ResxSetFolders.GetString("NameSpace_Models")}");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}public class {gClass}{ResxSetFolders.GetString("Class_ViewModel")}");
            templates.AppendLine($"{Tabulador}{LlaveAbre}");

            foreach (var itemColumn in columnsTable)
            {
                if (!itemColumn.IsNullable && !itemColumn.IsIdentity || itemColumn.IsForeignKey)
                    templates.AppendLine($"{Tabulador}{Tabulador}[Required(ErrorMessage =\"Obligatorio\")]");

                if (itemColumn.ColumnTypeNet == "string")
                    templates.AppendLine($"{Tabulador}{Tabulador}[StringLength({itemColumn.MaxLengthString})]");

                if (!itemColumn.IsIdentity)
                    templates.AppendLine($"{Tabulador}{Tabulador} public {itemColumn.ColumnTypeNet} {itemColumn.ColumnName} {LlaveAbre} get; set; {LlaveCierra}");
                else
                    templates.AppendLine($"{Tabulador}{Tabulador} public {itemColumn.ColumnTypeNet}? {itemColumn.ColumnName} {LlaveAbre} get; set; {LlaveCierra}");
            }

            foreach (var itemFk in columnsTable.Where(c=>c.IsForeignKey).ToList())
            {
                templates.AppendLine($"{Tabulador}{Tabulador} public SelectList {itemFk.ReferencedTable} {LlaveAbre} get; set; {LlaveCierra}");
            }


            templates.AppendLine($"{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{LlaveCierra}");

            return templates;
        }

        private StringBuilder GetControllers(string nameSpace, string gClass)
        {

            var columnsTable = Columns.GetAllColumn(gClass);
            var templates = new StringBuilder();

            templates.AppendLine($"using Dapper.DataTables.Models;");
            templates.AppendLine($"using LizardCode.Framework.Aplication.Common.Extensions;");
            templates.AppendLine($"using LizardCode.Framework.Aplication.Controllers.Base;");
            templates.AppendLine($"using {nameSpace}.Application.Interfaces.Business;");
            templates.AppendLine($"using {nameSpace}.Application.Models;");
            templates.AppendLine($"using Microsoft.AspNetCore.Authorization;");
            templates.AppendLine($"using Microsoft.AspNetCore.Mvc;");
            templates.AppendLine($"using System.Threading.Tasks;");
            templates.AppendLine("");
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}.{ResxSetFolders.GetString("NameSpace_Controllers")}");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}public class {gClass}{ResxSetFolders.GetString("Class_Controller")} : BusinessController");
            templates.AppendLine($"{Tabulador}{LlaveAbre}");

            foreach (var itemBusiness in columnsTable.Where(c => c.IsForeignKey).Select(s => s.ReferencedTable).ToList())
            {
                templates.AppendLine($"{Tabulador}{Tabulador}private readonly I{itemBusiness}Business _{itemBusiness.ToLower()}Business;");
            }
            templates.AppendLine($"{Tabulador}{Tabulador}private readonly I{gClass}Business _{gClass.ToLower()}Business;");
            templates.AppendLine("");
            //Contructor
            templates.AppendLine($"{Tabulador}{Tabulador}public {gClass}{ResxSetFolders.GetString("Class_Controller")} (");

            var columnsFK = columnsTable.Where(c => c.IsForeignKey).Select(s => s.ReferencedTable).ToList();
            var coma = columnsFK.Count > 0 ? "," : "";
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}I{gClass}Business {gClass.ToLower()}Business{coma}");
            foreach (var itemConstructor in columnsFK)
            {

                if (columnsFK.IndexOf(itemConstructor) == columnsFK.Count - 1)
                    templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}I{itemConstructor}Business {itemConstructor.ToLower()}Business");
                else
                    templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}I{itemConstructor}Business {itemConstructor.ToLower()}Business,");
            }
            
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador}{Tabulador})");

            templates.AppendLine($"{Tabulador}{Tabulador}{ LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}_{gClass.ToLower()}Business = {gClass.ToLower()}Business;");
            foreach (var itemBusiness in columnsFK)
            {
                templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}_{itemBusiness.ToLower()}Business = {itemBusiness.ToLower()}Business;");
            }
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            templates.AppendLine("");

            //INDEX  
            templates.AppendLine($"{Tabulador}{Tabulador}[Authorize(Roles = \"ADMIN\")]");
            templates.AppendLine($"{Tabulador}{Tabulador}public async Task<ActionResult> Index()");
            templates.AppendLine($"{Tabulador}{Tabulador}{ LlaveAbre}");
            templates.AppendLine("");
            //Combos
            foreach (var itemFk in columnsFK)
            {
                templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var Item{itemFk} = await _{itemFk.ToLower()}{ResxSetFolders.GetString("Class_Business")}.GetAllSelect();");
            }
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var model = new {gClass}{ResxSetFolders.GetString("Class_ViewModel")}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine("");
            //carga combos    
            foreach (var itemFk in columnsFK)
            {

                if (columnsFK.IndexOf(itemFk) == columnsFK.Count - 1)
                    templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{itemFk} = Item{itemFk}.ToDropDownList(l => l.Id, l => l.Description, descriptionIncludesKey: false)");
                else
                    templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}{itemFk} = Item{itemFk}.ToDropDownList(l => l.Id, l => l.Description, descriptionIncludesKey: false),");
            }
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{LlaveCierra};");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}return ActivateMenuItem(model: model);");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            

            //GetAll
            templates.AppendLine($"{Tabulador}{Tabulador}[HttpPost]");
            templates.AppendLine($"{Tabulador}{Tabulador}[Authorize(Roles = \"ADMIN\")]");
            templates.AppendLine($"{Tabulador}{Tabulador}public async Task<JsonResult> GetAll([FromForm] DataTablesRequest request)");
            templates.AppendLine($"{Tabulador}{Tabulador}{ LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var results = await _{gClass.ToLower()}{ResxSetFolders.GetString("Class_Business")}.GetAll(request);");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}return Json(results);");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");

            //GetOne
            templates.AppendLine($"{Tabulador}{Tabulador}[HttpGet]");
            templates.AppendLine($"{Tabulador}{Tabulador}[Authorize(Roles = \"ADMIN\")]");
            templates.AppendLine($"{Tabulador}{Tabulador}public async Task<JsonResult> Get(int id)");
            templates.AppendLine($"{Tabulador}{Tabulador}{ LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}var result = await _{gClass.ToLower()}Business.Get(id);");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador} return Json(() => result);");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");

            //Insert
            templates.AppendLine($"{Tabulador}{Tabulador}[HttpPost]");
            templates.AppendLine($"{Tabulador}{Tabulador}[Authorize(Roles = \"ADMIN\")]");
            templates.AppendLine($"{Tabulador}{Tabulador}public async Task<JsonResult> Insert({gClass}{ResxSetFolders.GetString("Class_ViewModel")} model)");
            templates.AppendLine($"{Tabulador}{Tabulador}{ LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}await _{gClass.ToLower()}{ResxSetFolders.GetString("Class_Business")}.New(model);");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}return Json(() => true);");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");

            //Update
            templates.AppendLine($"{Tabulador}{Tabulador}[HttpPost]");
            templates.AppendLine($"{Tabulador}{Tabulador}[Authorize(Roles = \"ADMIN\")]");
            templates.AppendLine($"{Tabulador}{Tabulador}public async Task<JsonResult> Update({gClass}{ResxSetFolders.GetString("Class_ViewModel")} model)");
            templates.AppendLine($"{Tabulador}{Tabulador}{ LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}await _{gClass.ToLower()}{ResxSetFolders.GetString("Class_Business")}.Update(model);");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}return Json(() => true);");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");

            //Delete
            templates.AppendLine($"{Tabulador}{Tabulador}[HttpPost]");
            templates.AppendLine($"{Tabulador}{Tabulador}[Authorize(Roles = \"ADMIN\")]");
            templates.AppendLine($"{Tabulador}{Tabulador}public async Task<JsonResult> Delete(int id)");
            templates.AppendLine($"{Tabulador}{Tabulador}{ LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}await _{gClass.ToLower()}{ResxSetFolders.GetString("Class_Business")}.Remove(id);");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}return Json(() => true);");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");


            templates.AppendLine($"{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{LlaveCierra}");

            return templates;
            
        }

        private StringBuilder GetInjection(string nameSpace, List<string> selectedItems)
        {

            var templates = new StringBuilder();

            templates.AppendLine($"using {nameSpace}.Application.Business;");
            templates.AppendLine($"using {nameSpace}.Application.Common.Extensions;");
            templates.AppendLine($"using {nameSpace}.Application.Interfaces.Business;");
            templates.AppendLine($"using Mapster;");
            templates.AppendLine($"using MapsterMapper;");
            templates.AppendLine($"using Microsoft.Extensions.Configuration;");
            templates.AppendLine($"using Microsoft.Extensions.DependencyInjection;");
            templates.AppendLine($"using System.Reflection;");
            templates.AppendLine("");
            templates.AppendLine($"namespace {nameSpace}.{ResxSetFolders.GetString("NameSpace_Application")}");
            templates.AppendLine($"{LlaveAbre}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}public static class Injection");
            templates.AppendLine($"{Tabulador}{LlaveAbre}");
            templates.AppendLine("");

            templates.AppendLine($"{Tabulador}{Tabulador}public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}services.AddLazyResolution();");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}services.AddSingleton(GetConfiguredMappingConfig());");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}services.AddScoped<IMapper, ServiceMapper>();");
            templates.AppendLine("");
            foreach (var item in selectedItems)
            {
                var itemClass = item.Split('.')[1];
                templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}services.AddScoped<I{itemClass}Business, {itemClass}Business>();");
            }
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}return services;");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            templates.AppendLine("");
            templates.AppendLine($"{Tabulador}{Tabulador}private static TypeAdapterConfig GetConfiguredMappingConfig()");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveAbre}");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}var config = TypeAdapterConfig.GlobalSettings;");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}config.Scan(Assembly.GetExecutingAssembly());");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}//config.Apply(registers);");
            templates.AppendLine($"{Tabulador}{Tabulador}{Tabulador}{Tabulador}return config;");
            templates.AppendLine($"{Tabulador}{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{Tabulador}{LlaveCierra}");
            templates.AppendLine($"{LlaveCierra}");
            templates.AppendLine("");    

            return templates;


        }

        #endregion

    }
}
