namespace Daily_Fantasy_Fuel_NBA_Fanduel_Optimizer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.listViewFinalTeam = new System.Windows.Forms.ListView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dVPcheckBox1 = new System.Windows.Forms.CheckBox();
            this.OverUndercheckBox2 = new System.Windows.Forms.CheckBox();
            this.totalTeamPointscheckBox3 = new System.Windows.Forms.CheckBox();
            this.RestcheckBox4 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.StartcheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(776, 403);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.UseWaitCursor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1373, 421);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Optimize";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.UseWaitCursor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // listViewFinalTeam
            // 
            this.listViewFinalTeam.Location = new System.Drawing.Point(794, 12);
            this.listViewFinalTeam.Name = "listViewFinalTeam";
            this.listViewFinalTeam.Size = new System.Drawing.Size(654, 403);
            this.listViewFinalTeam.TabIndex = 2;
            this.listViewFinalTeam.UseCompatibleStateImageBehavior = false;
            this.listViewFinalTeam.UseWaitCursor = true;
            this.listViewFinalTeam.View = System.Windows.Forms.View.Details;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 490);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(776, 23);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "https://www.dailyfantasyfuel.com/nba/projections/fanduel";
            this.textBox1.UseWaitCursor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(657, 461);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(122, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Manual Import";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.UseWaitCursor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(794, 439);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(244, 23);
            this.textBox2.TabIndex = 5;
            this.textBox2.UseWaitCursor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(794, 421);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Exclude Player";
            this.label1.UseWaitCursor = true;
            // 
            // dVPcheckBox1
            // 
            this.dVPcheckBox1.AutoSize = true;
            this.dVPcheckBox1.Location = new System.Drawing.Point(1053, 468);
            this.dVPcheckBox1.Name = "dVPcheckBox1";
            this.dVPcheckBox1.Size = new System.Drawing.Size(48, 19);
            this.dVPcheckBox1.TabIndex = 7;
            this.dVPcheckBox1.Text = "DVP";
            this.dVPcheckBox1.UseVisualStyleBackColor = true;
            this.dVPcheckBox1.UseWaitCursor = true;
            // 
            // OverUndercheckBox2
            // 
            this.OverUndercheckBox2.AutoSize = true;
            this.OverUndercheckBox2.Location = new System.Drawing.Point(1183, 443);
            this.OverUndercheckBox2.Name = "OverUndercheckBox2";
            this.OverUndercheckBox2.Size = new System.Drawing.Size(86, 19);
            this.OverUndercheckBox2.TabIndex = 8;
            this.OverUndercheckBox2.Text = "Over Under";
            this.OverUndercheckBox2.UseVisualStyleBackColor = true;
            this.OverUndercheckBox2.UseWaitCursor = true;
            // 
            // totalTeamPointscheckBox3
            // 
            this.totalTeamPointscheckBox3.AutoSize = true;
            this.totalTeamPointscheckBox3.Location = new System.Drawing.Point(1053, 443);
            this.totalTeamPointscheckBox3.Name = "totalTeamPointscheckBox3";
            this.totalTeamPointscheckBox3.Size = new System.Drawing.Size(118, 19);
            this.totalTeamPointscheckBox3.TabIndex = 9;
            this.totalTeamPointscheckBox3.Text = "Total Team Points";
            this.totalTeamPointscheckBox3.UseVisualStyleBackColor = true;
            this.totalTeamPointscheckBox3.UseWaitCursor = true;
            // 
            // RestcheckBox4
            // 
            this.RestcheckBox4.AutoSize = true;
            this.RestcheckBox4.Location = new System.Drawing.Point(1183, 468);
            this.RestcheckBox4.Name = "RestcheckBox4";
            this.RestcheckBox4.Size = new System.Drawing.Size(48, 19);
            this.RestcheckBox4.TabIndex = 10;
            this.RestcheckBox4.Text = "Rest";
            this.RestcheckBox4.UseVisualStyleBackColor = true;
            this.RestcheckBox4.UseWaitCursor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1053, 421);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Modifiers ";
            this.label2.UseWaitCursor = true;
            // 
            // StartcheckBox
            // 
            this.StartcheckBox.AutoSize = true;
            this.StartcheckBox.Location = new System.Drawing.Point(1053, 490);
            this.StartcheckBox.Name = "StartcheckBox";
            this.StartcheckBox.Size = new System.Drawing.Size(67, 19);
            this.StartcheckBox.TabIndex = 12;
            this.StartcheckBox.Text = "Starting";
            this.StartcheckBox.UseVisualStyleBackColor = true;
            this.StartcheckBox.UseWaitCursor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1473, 525);
            this.Controls.Add(this.StartcheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RestcheckBox4);
            this.Controls.Add(this.totalTeamPointscheckBox3);
            this.Controls.Add(this.OverUndercheckBox2);
            this.Controls.Add(this.dVPcheckBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listViewFinalTeam);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "dailyfantasyfuel.com Value Finder";
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridView dataGridView1;
        private Button button1;
        private ListView listViewFinalTeam;
        private TextBox textBox1;
        private Button button2;
        private TextBox textBox2;
        private Label label1;
        private CheckBox dVPcheckBox1;
        private CheckBox OverUndercheckBox2;
        private CheckBox totalTeamPointscheckBox3;
        private CheckBox RestcheckBox4;
        private Label label2;
        private CheckBox StartcheckBox;
    }
}