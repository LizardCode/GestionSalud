using GenerateClass.Dapper.Data;
using GenerateClass.Dapper.Generator;
using Microsoft.Data.ConnectionUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Windows.Forms;

namespace GenerateDBClass.Dapper
{
    public partial class FormMain : Form
    {
        public string connection { get; set; }


        DataConnectionDialog dialog = new DataConnectionDialog(); 

        public FormMain()
        {
            InitializeComponent();
 
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {

            connection = dialog.ConnectionString;

            if(!string.IsNullOrEmpty(connection))
            {
                TableData tableData = new TableData(connection);

                var tablelist = tableData.GetAll();

                this.clbTable.Items.Clear();

                foreach (var item in tablelist)
	            {
                    this.clbTable.Items.Add(item.Name);
	            }

            }

        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            lbDetaill.Items.Clear();

            List<string> classList = new List<string>();
                      
            foreach (var item in clbTable.CheckedItems)
            {
                classList.Add(item.ToString());
            }

            var generateApplication = new GenerateApplication(connection);
            var generateDomain = new GenerateDomain(connection);
            var generateInfrastructure = new GenerateInfractructure(connection);

            lbDetaill.Items.Add("Inicio del porceso...");

            if (ckbIBusiness.Checked)
            {
                generateApplication.GenerateInterfaceBusiness(txtDirectory.Text, txtNameSpace.Text, classList);
                lbDetaill.Items.Add("Generando Interface Business");
            }

            if (ckbClassBusiness.Checked)
            {
                generateApplication.GenerateBusiness(txtDirectory.Text, txtNameSpace.Text, classList);
                lbDetaill.Items.Add("Generando Business");
            }


            if (ckbIRepository.Checked)
            {
                generateApplication.GenerateInterfaceRepository(txtDirectory.Text, txtNameSpace.Text, classList);
                lbDetaill.Items.Add("Generando Interface  Repository");
            }

            if (ckbModels.Checked)
            {
                generateApplication.GenerateModels(txtDirectory.Text, txtNameSpace.Text, classList);
                lbDetaill.Items.Add("Generando Models");
            }

            if (ckbControllers.Checked)
            {
                generateApplication.GenerateControllers(txtDirectory.Text, txtNameSpace.Text, classList);
                lbDetaill.Items.Add("Generando Controllers");
            }

            if (ckbInjection.Checked)
            {
                generateApplication.GenerateInjection(txtDirectory.Text, txtNameSpace.Text, classList);
                lbDetaill.Items.Add("Generando Appication Injection");
            }
            
            if (ckbEntities.Checked)
            {
                generateDomain.GenerateEntities(txtDirectory.Text, txtNameSpace.Text, classList);
                lbDetaill.Items.Add("Generando Entities");
            }

            if (ckbEntitiesCustom.Checked)
            {
                generateDomain.GenerateEntitiesCustom(txtDirectory.Text, txtNameSpace.Text, classList);
                lbDetaill.Items.Add("Generando Entities Custom");
            }


            if (ChkRepository.Checked)
            {
                generateInfrastructure.GenerateClassInfractructure(txtDirectory.Text, txtNameSpace.Text, classList);
                lbDetaill.Items.Add("Generando Class Infractructure");
            }


            if (ckbInjection.Checked)
            {
                generateInfrastructure.GenerateInjection(txtDirectory.Text, txtNameSpace.Text, classList);
                lbDetaill.Items.Add("Generando infrastructure Injection");
            }


            lbDetaill.Items.Add("Fin del porceso...");
        }

        private void btnDirectory_Click(object sender, EventArgs e)
        {
            DialogResult result = folderDialog.ShowDialog();

            txtDirectory.Text = folderDialog.SelectedPath;
        }

        private void btnBase_Click(object sender, EventArgs e)
        {
            
            Microsoft.Data.ConnectionUI.DataSource.AddStandardDataSources(dialog); 
            dialog.SelectedDataSource = Microsoft.Data.ConnectionUI.DataSource.SqlDataSource; 
            dialog.SelectedDataProvider = Microsoft.Data.ConnectionUI.DataProvider.SqlDataProvider;
            dialog.ConnectionString = ConfigurationManager.AppSettings["GeneraConn"].ToString();

            if (DataConnectionDialog.Show(dialog) == DialogResult.OK)
            {
                bool isSavePasswordChecked = IsSavePasswordChecked(dialog);

                AddOrUpdateAppSettings("GeneraConn", dialog.ConnectionString);

                btnUnAll.Enabled = true;
                btnAll.Enabled = true;
                btnRefrescar.Enabled = true;

            }

            var data = dialog.DisplayConnectionString.Split(';');

            lblNameServer.Text = data[0].Split('=')[1].ToString();
            lblBase.Text = data[1].Split('=')[1].ToString();
            lblUser.Text = data[2].Split('=')[1].ToString();
        }

        private static bool IsSavePasswordChecked(DataConnectionDialog dialog)
        {
            var control = GetPropertyValue("ConnectionUIControl", dialog, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            if (control == null)
            {
                return false;
            }

            var properties = GetPropertyValue("Properties", control, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.DeclaredOnly);
            if (properties == null)
            {
                return false;
            }

            var savePassword = GetPropertyValue("SavePassword", properties, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            if (savePassword != null && savePassword is bool)
            {
                return (bool)savePassword;
            }

            return false;
        }

        private static object GetPropertyValue(string propertyName, object target, BindingFlags bindingFlags)
        {
            var propertyInfo = target.GetType().GetProperty(propertyName, bindingFlags);
            if (propertyInfo == null)
            {
                return null;
            }

            return propertyInfo.GetValue(target, null);
        }

        public static void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {

              for (int i = 0; i < clbTable.Items.Count; i++)
              {
                  clbTable.SetItemChecked(i, true);
              }

        }

        private void btnUnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbTable.Items.Count; i++)
            {
                clbTable.SetItemChecked(i, false);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
