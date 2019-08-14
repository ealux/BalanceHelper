namespace БалансДанные
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.TxtCount = new System.Windows.Forms.TextBox();
            this.LblCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pathExcel = new System.Windows.Forms.TextBox();
            this.opnExcel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pathTempl = new System.Windows.Forms.TextBox();
            this.opnTempl = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.dataPresenter = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.pathTempl2 = new System.Windows.Forms.TextBox();
            this.opnTempl2 = new System.Windows.Forms.Button();
            this.num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.input = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.output = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.input_excel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.output_excel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPresenter)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(795, 593);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.TxtCount);
            this.tabPage1.Controls.Add(this.LblCount);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.pathExcel);
            this.tabPage1.Controls.Add(this.opnExcel);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.pathTempl);
            this.tabPage1.Controls.Add(this.opnTempl);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.dataPresenter);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(787, 567);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Создание";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // TxtCount
            // 
            this.TxtCount.Location = new System.Drawing.Point(629, 154);
            this.TxtCount.Name = "TxtCount";
            this.TxtCount.Size = new System.Drawing.Size(90, 20);
            this.TxtCount.TabIndex = 14;
            this.TxtCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LblCount
            // 
            this.LblCount.AutoSize = true;
            this.LblCount.Location = new System.Drawing.Point(653, 125);
            this.LblCount.Name = "LblCount";
            this.LblCount.Size = new System.Drawing.Size(10, 13);
            this.LblCount.TabIndex = 13;
            this.LblCount.Text = " ";
            this.LblCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(539, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Обработать:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(511, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Столбцов данных:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Документ Excel";
            // 
            // pathExcel
            // 
            this.pathExcel.Location = new System.Drawing.Point(203, 76);
            this.pathExcel.Name = "pathExcel";
            this.pathExcel.Size = new System.Drawing.Size(383, 20);
            this.pathExcel.TabIndex = 9;
            // 
            // opnExcel
            // 
            this.opnExcel.Location = new System.Drawing.Point(616, 75);
            this.opnExcel.Name = "opnExcel";
            this.opnExcel.Size = new System.Drawing.Size(103, 23);
            this.opnExcel.TabIndex = 8;
            this.opnExcel.Text = "Обзор";
            this.opnExcel.UseVisualStyleBackColor = true;
            this.opnExcel.Click += new System.EventHandler(this.opnExcel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Шаблон базы режимов";
            // 
            // pathTempl
            // 
            this.pathTempl.Location = new System.Drawing.Point(203, 24);
            this.pathTempl.Name = "pathTempl";
            this.pathTempl.Size = new System.Drawing.Size(383, 20);
            this.pathTempl.TabIndex = 4;
            // 
            // opnTempl
            // 
            this.opnTempl.Location = new System.Drawing.Point(616, 23);
            this.opnTempl.Name = "opnTempl";
            this.opnTempl.Size = new System.Drawing.Size(103, 23);
            this.opnTempl.TabIndex = 3;
            this.opnTempl.Text = "Обзор";
            this.opnTempl.UseVisualStyleBackColor = true;
            this.opnTempl.Click += new System.EventHandler(this.opnTempl_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(358, 135);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Обработать!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataPresenter
            // 
            this.dataPresenter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataPresenter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.num,
            this.name,
            this.type,
            this.input,
            this.output,
            this.input_excel,
            this.output_excel});
            this.dataPresenter.Location = new System.Drawing.Point(19, 181);
            this.dataPresenter.Name = "dataPresenter";
            this.dataPresenter.Size = new System.Drawing.Size(749, 380);
            this.dataPresenter.TabIndex = 6;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.pathTempl2);
            this.tabPage2.Controls.Add(this.opnTempl2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(787, 567);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Выгрузка";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(65, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Шаблон базы режимов";
            // 
            // pathTempl2
            // 
            this.pathTempl2.Location = new System.Drawing.Point(201, 23);
            this.pathTempl2.Name = "pathTempl2";
            this.pathTempl2.Size = new System.Drawing.Size(383, 20);
            this.pathTempl2.TabIndex = 12;
            // 
            // opnTempl2
            // 
            this.opnTempl2.Location = new System.Drawing.Point(614, 22);
            this.opnTempl2.Name = "opnTempl2";
            this.opnTempl2.Size = new System.Drawing.Size(103, 23);
            this.opnTempl2.TabIndex = 11;
            this.opnTempl2.Text = "Обзор";
            this.opnTempl2.UseVisualStyleBackColor = true;
            this.opnTempl2.Click += new System.EventHandler(this.opnTempl2_Click);
            // 
            // num
            // 
            this.num.HeaderText = "Номер";
            this.num.Name = "num";
            // 
            // name
            // 
            this.name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.name.HeaderText = "Название";
            this.name.Name = "name";
            this.name.Width = 82;
            // 
            // type
            // 
            this.type.HeaderText = "Тип";
            this.type.Name = "type";
            // 
            // input
            // 
            this.input.HeaderText = "Pизм прием";
            this.input.Name = "input";
            // 
            // output
            // 
            this.output.HeaderText = "Pизм отдача";
            this.output.Name = "output";
            // 
            // input_excel
            // 
            this.input_excel.HeaderText = "Pпирем_Excel";
            this.input_excel.Name = "input_excel";
            // 
            // output_excel
            // 
            this.output_excel.HeaderText = "Pотдача_Excel";
            this.output_excel.Name = "output_excel";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 605);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPresenter)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pathTempl;
        private System.Windows.Forms.Button opnTempl;
        private System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.DataGridView dataPresenter;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox pathExcel;
        private System.Windows.Forms.Button opnExcel;
        private System.Windows.Forms.Label LblCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox pathTempl2;
        private System.Windows.Forms.Button opnTempl2;
        private System.Windows.Forms.DataGridViewTextBoxColumn num;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn input;
        private System.Windows.Forms.DataGridViewTextBoxColumn output;
        private System.Windows.Forms.DataGridViewTextBoxColumn input_excel;
        private System.Windows.Forms.DataGridViewTextBoxColumn output_excel;
    }
}

