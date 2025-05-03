namespace GenerateDBClass.Dapper
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.clbTable = new System.Windows.Forms.CheckedListBox();
            this.txtNameSpace = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDirectory = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblBase = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblNameServer = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBase = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ckbEntitiesCustom = new System.Windows.Forms.CheckBox();
            this.ckbEntities = new System.Windows.Forms.CheckBox();
            this.lbDetaill = new System.Windows.Forms.ListBox();
            this.pgBGenerator = new System.Windows.Forms.ProgressBar();
            this.btnUnAll = new System.Windows.Forms.Button();
            this.btnAll = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ckbInjection = new System.Windows.Forms.CheckBox();
            this.ckbControllers = new System.Windows.Forms.CheckBox();
            this.ckbModels = new System.Windows.Forms.CheckBox();
            this.ckbIRepository = new System.Windows.Forms.CheckBox();
            this.ckbIBaseRepository = new System.Windows.Forms.CheckBox();
            this.ckbIBusiness = new System.Windows.Forms.CheckBox();
            this.ckbClassBusiness = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ckbInjectionInfra = new System.Windows.Forms.CheckBox();
            this.ChkRepository = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // clbTable
            // 
            this.clbTable.FormattingEnabled = true;
            this.clbTable.Location = new System.Drawing.Point(12, 151);
            this.clbTable.Name = "clbTable";
            this.clbTable.Size = new System.Drawing.Size(387, 439);
            this.clbTable.TabIndex = 1;
            // 
            // txtNameSpace
            // 
            this.txtNameSpace.Location = new System.Drawing.Point(417, 171);
            this.txtNameSpace.Name = "txtNameSpace";
            this.txtNameSpace.Size = new System.Drawing.Size(360, 20);
            this.txtNameSpace.TabIndex = 5;
            this.txtNameSpace.Text = "Nombre del Proyecto";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(414, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "NameSpace (Class - Data)";
            // 
            // txtDirectory
            // 
            this.txtDirectory.Location = new System.Drawing.Point(417, 127);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(427, 20);
            this.txtDirectory.TabIndex = 7;
            this.txtDirectory.Text = "C:\\Users\\Leonardo\\Desktop\\Generate";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(414, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Directorio Destino";
            // 
            // btnDirectory
            // 
            this.btnDirectory.Location = new System.Drawing.Point(850, 124);
            this.btnDirectory.Name = "btnDirectory";
            this.btnDirectory.Size = new System.Drawing.Size(75, 23);
            this.btnDirectory.TabIndex = 9;
            this.btnDirectory.Text = "Seleccionar";
            this.btnDirectory.UseVisualStyleBackColor = true;
            this.btnDirectory.Click += new System.EventHandler(this.btnDirectory_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.btnBase);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.panel1.Size = new System.Drawing.Size(968, 75);
            this.panel1.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblUser);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lblBase);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lblNameServer);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(64, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(860, 59);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(328, 32);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(10, 13);
            this.lblUser.TabIndex = 5;
            this.lblUser.Text = "-";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label7.Location = new System.Drawing.Point(280, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Usuario:";
            // 
            // lblBase
            // 
            this.lblBase.AutoSize = true;
            this.lblBase.Location = new System.Drawing.Point(60, 32);
            this.lblBase.Name = "lblBase";
            this.lblBase.Size = new System.Drawing.Size(10, 13);
            this.lblBase.TabIndex = 3;
            this.lblBase.Text = "-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(19, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Base:";
            // 
            // lblNameServer
            // 
            this.lblNameServer.AutoSize = true;
            this.lblNameServer.Location = new System.Drawing.Point(125, 15);
            this.lblNameServer.Name = "lblNameServer";
            this.lblNameServer.Size = new System.Drawing.Size(10, 13);
            this.lblNameServer.TabIndex = 1;
            this.lblNameServer.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Location = new System.Drawing.Point(20, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Nombre del Server:";
            // 
            // btnBase
            // 
            this.btnBase.Image = global::GenerateDBClass.Dapper.Properties.Resources.connect_icon;
            this.btnBase.Location = new System.Drawing.Point(11, 13);
            this.btnBase.Name = "btnBase";
            this.btnBase.Size = new System.Drawing.Size(47, 35);
            this.btnBase.TabIndex = 10;
            this.btnBase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBase.UseVisualStyleBackColor = true;
            this.btnBase.Click += new System.EventHandler(this.btnBase_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ckbEntitiesCustom);
            this.groupBox2.Controls.Add(this.ckbEntities);
            this.groupBox2.Location = new System.Drawing.Point(417, 297);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(211, 53);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Domain";
            // 
            // ckbEntitiesCustom
            // 
            this.ckbEntitiesCustom.AutoSize = true;
            this.ckbEntitiesCustom.Checked = true;
            this.ckbEntitiesCustom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbEntitiesCustom.Location = new System.Drawing.Point(97, 19);
            this.ckbEntitiesCustom.Name = "ckbEntitiesCustom";
            this.ckbEntitiesCustom.Size = new System.Drawing.Size(98, 17);
            this.ckbEntitiesCustom.TabIndex = 7;
            this.ckbEntitiesCustom.Text = "Entities Custom";
            this.ckbEntitiesCustom.UseVisualStyleBackColor = true;
            // 
            // ckbEntities
            // 
            this.ckbEntities.AutoSize = true;
            this.ckbEntities.Checked = true;
            this.ckbEntities.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbEntities.Location = new System.Drawing.Point(16, 19);
            this.ckbEntities.Name = "ckbEntities";
            this.ckbEntities.Size = new System.Drawing.Size(60, 17);
            this.ckbEntities.TabIndex = 6;
            this.ckbEntities.Text = "Entities";
            this.ckbEntities.UseVisualStyleBackColor = true;
            // 
            // lbDetaill
            // 
            this.lbDetaill.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.lbDetaill.FormattingEnabled = true;
            this.lbDetaill.Location = new System.Drawing.Point(417, 361);
            this.lbDetaill.Name = "lbDetaill";
            this.lbDetaill.Size = new System.Drawing.Size(539, 225);
            this.lbDetaill.TabIndex = 15;
            // 
            // pgBGenerator
            // 
            this.pgBGenerator.Dock = System.Windows.Forms.DockStyle.Top;
            this.pgBGenerator.Location = new System.Drawing.Point(0, 75);
            this.pgBGenerator.Name = "pgBGenerator";
            this.pgBGenerator.Size = new System.Drawing.Size(968, 18);
            this.pgBGenerator.TabIndex = 16;
            // 
            // btnUnAll
            // 
            this.btnUnAll.Enabled = false;
            this.btnUnAll.Image = global::GenerateDBClass.Dapper.Properties.Resources.Delete;
            this.btnUnAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUnAll.Location = new System.Drawing.Point(288, 105);
            this.btnUnAll.Name = "btnUnAll";
            this.btnUnAll.Size = new System.Drawing.Size(111, 39);
            this.btnUnAll.TabIndex = 13;
            this.btnUnAll.Tag = "";
            this.btnUnAll.Text = "Deselecionar";
            this.btnUnAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUnAll.UseVisualStyleBackColor = true;
            this.btnUnAll.Click += new System.EventHandler(this.btnUnAll_Click);
            // 
            // btnAll
            // 
            this.btnAll.Enabled = false;
            this.btnAll.Image = global::GenerateDBClass.Dapper.Properties.Resources.Select;
            this.btnAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAll.Location = new System.Drawing.Point(171, 105);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(111, 39);
            this.btnAll.TabIndex = 12;
            this.btnAll.Tag = "";
            this.btnAll.Text = "Seleccionar";
            this.btnAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Image = global::GenerateDBClass.Dapper.Properties.Resources.Save;
            this.btnGenerate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenerate.Location = new System.Drawing.Point(798, 155);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(127, 50);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generar";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnRefrescar
            // 
            this.btnRefrescar.Enabled = false;
            this.btnRefrescar.Image = global::GenerateDBClass.Dapper.Properties.Resources.Refresh2;
            this.btnRefrescar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefrescar.Location = new System.Drawing.Point(12, 105);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new System.Drawing.Size(135, 39);
            this.btnRefrescar.TabIndex = 0;
            this.btnRefrescar.Tag = "";
            this.btnRefrescar.Text = "Refrescar Tablas";
            this.btnRefrescar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefrescar.UseVisualStyleBackColor = true;
            this.btnRefrescar.Click += new System.EventHandler(this.btnRefrescar_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ckbInjection);
            this.groupBox3.Controls.Add(this.ckbControllers);
            this.groupBox3.Controls.Add(this.ckbModels);
            this.groupBox3.Controls.Add(this.ckbIRepository);
            this.groupBox3.Controls.Add(this.ckbIBaseRepository);
            this.groupBox3.Controls.Add(this.ckbIBusiness);
            this.groupBox3.Controls.Add(this.ckbClassBusiness);
            this.groupBox3.Location = new System.Drawing.Point(417, 214);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(539, 77);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "DLL Application";
            // 
            // ckbInjection
            // 
            this.ckbInjection.AutoSize = true;
            this.ckbInjection.Checked = true;
            this.ckbInjection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbInjection.Location = new System.Drawing.Point(288, 42);
            this.ckbInjection.Name = "ckbInjection";
            this.ckbInjection.Size = new System.Drawing.Size(66, 17);
            this.ckbInjection.TabIndex = 7;
            this.ckbInjection.Text = "Injection";
            this.ckbInjection.UseVisualStyleBackColor = true;
            // 
            // ckbControllers
            // 
            this.ckbControllers.AutoSize = true;
            this.ckbControllers.Checked = true;
            this.ckbControllers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbControllers.Location = new System.Drawing.Point(82, 42);
            this.ckbControllers.Name = "ckbControllers";
            this.ckbControllers.Size = new System.Drawing.Size(75, 17);
            this.ckbControllers.TabIndex = 6;
            this.ckbControllers.Text = "Controllers";
            this.ckbControllers.UseVisualStyleBackColor = true;
            // 
            // ckbModels
            // 
            this.ckbModels.AutoSize = true;
            this.ckbModels.Checked = true;
            this.ckbModels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbModels.Location = new System.Drawing.Point(16, 42);
            this.ckbModels.Name = "ckbModels";
            this.ckbModels.Size = new System.Drawing.Size(60, 17);
            this.ckbModels.TabIndex = 5;
            this.ckbModels.Text = "Models";
            this.ckbModels.UseVisualStyleBackColor = true;
            this.ckbModels.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ckbIRepository
            // 
            this.ckbIRepository.AutoSize = true;
            this.ckbIRepository.Checked = true;
            this.ckbIRepository.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbIRepository.Location = new System.Drawing.Point(288, 19);
            this.ckbIRepository.Name = "ckbIRepository";
            this.ckbIRepository.Size = new System.Drawing.Size(120, 17);
            this.ckbIRepository.TabIndex = 4;
            this.ckbIRepository.Text = "Interfase Repository";
            this.ckbIRepository.UseVisualStyleBackColor = true;
            // 
            // ckbIBaseRepository
            // 
            this.ckbIBaseRepository.AutoSize = true;
            this.ckbIBaseRepository.Checked = true;
            this.ckbIBaseRepository.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbIBaseRepository.Location = new System.Drawing.Point(135, 19);
            this.ckbIBaseRepository.Name = "ckbIBaseRepository";
            this.ckbIBaseRepository.Size = new System.Drawing.Size(147, 17);
            this.ckbIBaseRepository.TabIndex = 3;
            this.ckbIBaseRepository.Text = "Interfase Base Repository";
            this.ckbIBaseRepository.UseVisualStyleBackColor = true;
            // 
            // ckbIBusiness
            // 
            this.ckbIBusiness.AutoSize = true;
            this.ckbIBusiness.Checked = true;
            this.ckbIBusiness.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbIBusiness.Location = new System.Drawing.Point(16, 19);
            this.ckbIBusiness.Name = "ckbIBusiness";
            this.ckbIBusiness.Size = new System.Drawing.Size(113, 17);
            this.ckbIBusiness.TabIndex = 1;
            this.ckbIBusiness.Text = "Interface Business";
            this.ckbIBusiness.UseVisualStyleBackColor = true;
            // 
            // ckbClassBusiness
            // 
            this.ckbClassBusiness.AutoSize = true;
            this.ckbClassBusiness.Checked = true;
            this.ckbClassBusiness.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbClassBusiness.Location = new System.Drawing.Point(174, 42);
            this.ckbClassBusiness.Name = "ckbClassBusiness";
            this.ckbClassBusiness.Size = new System.Drawing.Size(96, 17);
            this.ckbClassBusiness.TabIndex = 0;
            this.ckbClassBusiness.Text = "Class Business";
            this.ckbClassBusiness.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.ckbInjectionInfra);
            this.groupBox4.Controls.Add(this.ChkRepository);
            this.groupBox4.Location = new System.Drawing.Point(645, 297);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(311, 53);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Infrastructure";
            // 
            // ckbInjectionInfra
            // 
            this.ckbInjectionInfra.AutoSize = true;
            this.ckbInjectionInfra.Checked = true;
            this.ckbInjectionInfra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbInjectionInfra.Location = new System.Drawing.Point(97, 19);
            this.ckbInjectionInfra.Name = "ckbInjectionInfra";
            this.ckbInjectionInfra.Size = new System.Drawing.Size(66, 17);
            this.ckbInjectionInfra.TabIndex = 7;
            this.ckbInjectionInfra.Text = "Injection";
            this.ckbInjectionInfra.UseVisualStyleBackColor = true;
            // 
            // ChkRepository
            // 
            this.ChkRepository.AutoSize = true;
            this.ChkRepository.Checked = true;
            this.ChkRepository.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkRepository.Location = new System.Drawing.Point(16, 19);
            this.ChkRepository.Name = "ChkRepository";
            this.ChkRepository.Size = new System.Drawing.Size(76, 17);
            this.ChkRepository.TabIndex = 6;
            this.ChkRepository.Text = "Repository";
            this.ChkRepository.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(968, 615);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.pgBGenerator);
            this.Controls.Add(this.lbDetaill);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnUnAll);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnDirectory);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtNameSpace);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.clbTable);
            this.Controls.Add(this.btnRefrescar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generate Class - Dapper Versión: 0.3";
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtNameSpace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
        private System.Windows.Forms.TextBox txtDirectory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDirectory;
        private System.Windows.Forms.Button btnBase;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblNameServer;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblBase;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.CheckedListBox clbTable;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnUnAll;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lbDetaill;
        private System.Windows.Forms.ProgressBar pgBGenerator;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox ckbIBusiness;
        private System.Windows.Forms.CheckBox ckbClassBusiness;
        private System.Windows.Forms.CheckBox ckbIBaseRepository;
        private System.Windows.Forms.CheckBox ckbIRepository;
        private System.Windows.Forms.CheckBox ckbModels;
        private System.Windows.Forms.CheckBox ckbControllers;
        private System.Windows.Forms.CheckBox ckbEntitiesCustom;
        private System.Windows.Forms.CheckBox ckbEntities;
        private System.Windows.Forms.CheckBox ckbInjection;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox ckbInjectionInfra;
        private System.Windows.Forms.CheckBox ChkRepository;
    }
}

