namespace ArcaliveCrawler
{
    partial class RankExportForm
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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.FileChooseButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.statSelectionComboBox = new System.Windows.Forms.ComboBox();
            this.statExportButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 66);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(132, 16);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "집계 기간 제한 사용";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "txt",
            "csv"});
            this.comboBox1.Location = new System.Drawing.Point(12, 257);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(73, 20);
            this.comboBox1.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 242);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "확장자";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 283);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "인코딩";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "UTF-8",
            "EUC-KR"});
            this.comboBox2.Location = new System.Drawing.Point(12, 298);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(73, 20);
            this.comboBox2.TabIndex = 10;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(240, 21);
            this.textBox1.TabIndex = 14;
            // 
            // FileChooseButton
            // 
            this.FileChooseButton.Location = new System.Drawing.Point(258, 24);
            this.FileChooseButton.Name = "FileChooseButton";
            this.FileChooseButton.Size = new System.Drawing.Size(66, 23);
            this.FileChooseButton.TabIndex = 15;
            this.FileChooseButton.Text = "파일 선택";
            this.FileChooseButton.UseVisualStyleBackColor = true;
            this.FileChooseButton.Click += new System.EventHandler(this.FileChooseButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "1. 데이터 파일을 불러오기";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(12, 167);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(128, 16);
            this.checkBox2.TabIndex = 17;
            this.checkBox2.Text = "게시글 필터링 사용";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(12, 190);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(303, 21);
            this.textBox2.TabIndex = 18;
            this.textBox2.Text = "뿌림,";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(174, 176);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 11);
            this.label6.TabIndex = 19;
            this.label6.Text = "(띄어쓰기x, \',\'로 구분)";
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(14, 88);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 22;
            this.button5.Text = "추가";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(14, 117);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 23;
            this.button6.Text = "초기화";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(96, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 11);
            this.label1.TabIndex = 24;
            // 
            // statSelectionComboBox
            // 
            this.statSelectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.statSelectionComboBox.FormattingEnabled = true;
            this.statSelectionComboBox.Location = new System.Drawing.Point(113, 257);
            this.statSelectionComboBox.Name = "statSelectionComboBox";
            this.statSelectionComboBox.Size = new System.Drawing.Size(202, 20);
            this.statSelectionComboBox.TabIndex = 26;
            this.statSelectionComboBox.SelectedIndexChanged += new System.EventHandler(this.statSelectionComboBox_SelectedIndexChanged);
            // 
            // statExportButton
            // 
            this.statExportButton.Location = new System.Drawing.Point(223, 290);
            this.statExportButton.Name = "statExportButton";
            this.statExportButton.Size = new System.Drawing.Size(92, 35);
            this.statExportButton.TabIndex = 27;
            this.statExportButton.Text = "통계 출력";
            this.statExportButton.UseVisualStyleBackColor = true;
            this.statExportButton.Click += new System.EventHandler(this.statExportButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(113, 242);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 12);
            this.label2.TabIndex = 28;
            this.label2.Text = "뽑을 통계를 선택하기";
            // 
            // RankExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 333);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statExportButton);
            this.Controls.Add(this.statSelectionComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.FileChooseButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.checkBox1);
            this.Name = "RankExportForm";
            this.Text = "통계 출력";
            this.Load += new System.EventHandler(this.RankExportForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button FileChooseButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox statSelectionComboBox;
        private System.Windows.Forms.Button statExportButton;
        private System.Windows.Forms.Label label2;
    }
}