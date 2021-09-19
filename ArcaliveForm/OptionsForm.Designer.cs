namespace ArcaliveForm
{
    partial class OptionsForm
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
            this.components = new System.ComponentModel.Container();
            this.PageFindingAlgorithmGroupBox = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.PFABinaryRadioButton = new System.Windows.Forms.RadioButton();
            this.PFASetPageButton = new System.Windows.Forms.RadioButton();
            this.SaveOptionsButton = new System.Windows.Forms.Button();
            this.ResetOptionsButton = new System.Windows.Forms.Button();
            this.SkippingTagsGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.PageFindingAlgorithmGroupBox.SuspendLayout();
            this.SkippingTagsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // PageFindingAlgorithmGroupBox
            // 
            this.PageFindingAlgorithmGroupBox.Controls.Add(this.textBox1);
            this.PageFindingAlgorithmGroupBox.Controls.Add(this.PFABinaryRadioButton);
            this.PageFindingAlgorithmGroupBox.Controls.Add(this.PFASetPageButton);
            this.PageFindingAlgorithmGroupBox.Location = new System.Drawing.Point(13, 13);
            this.PageFindingAlgorithmGroupBox.Name = "PageFindingAlgorithmGroupBox";
            this.PageFindingAlgorithmGroupBox.Size = new System.Drawing.Size(212, 73);
            this.PageFindingAlgorithmGroupBox.TabIndex = 0;
            this.PageFindingAlgorithmGroupBox.TabStop = false;
            this.PageFindingAlgorithmGroupBox.Text = "크롤링 시작 페이지 찾기 방식";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(112, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(67, 21);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "1";
            // 
            // PFABinaryRadioButton
            // 
            this.PFABinaryRadioButton.AutoSize = true;
            this.PFABinaryRadioButton.Location = new System.Drawing.Point(7, 44);
            this.PFABinaryRadioButton.Name = "PFABinaryRadioButton";
            this.PFABinaryRadioButton.Size = new System.Drawing.Size(161, 16);
            this.PFABinaryRadioButton.TabIndex = 1;
            this.PFABinaryRadioButton.TabStop = true;
            this.PFABinaryRadioButton.Text = "이진탐색 알고리즘 (권장)";
            this.PFABinaryRadioButton.UseVisualStyleBackColor = true;
            // 
            // PFASetPageButton
            // 
            this.PFASetPageButton.AutoSize = true;
            this.PFASetPageButton.Checked = true;
            this.PFASetPageButton.Location = new System.Drawing.Point(7, 21);
            this.PFASetPageButton.Name = "PFASetPageButton";
            this.PFASetPageButton.Size = new System.Drawing.Size(87, 16);
            this.PFASetPageButton.TabIndex = 0;
            this.PFASetPageButton.TabStop = true;
            this.PFASetPageButton.Text = "페이지 번호";
            this.PFASetPageButton.UseVisualStyleBackColor = true;
            // 
            // SaveOptionsButton
            // 
            this.SaveOptionsButton.Location = new System.Drawing.Point(12, 415);
            this.SaveOptionsButton.Name = "SaveOptionsButton";
            this.SaveOptionsButton.Size = new System.Drawing.Size(95, 23);
            this.SaveOptionsButton.TabIndex = 1;
            this.SaveOptionsButton.Text = "설정 저장";
            this.SaveOptionsButton.UseVisualStyleBackColor = true;
            this.SaveOptionsButton.Click += new System.EventHandler(this.SaveOptionsButton_Click);
            // 
            // ResetOptionsButton
            // 
            this.ResetOptionsButton.Location = new System.Drawing.Point(131, 415);
            this.ResetOptionsButton.Name = "ResetOptionsButton";
            this.ResetOptionsButton.Size = new System.Drawing.Size(94, 23);
            this.ResetOptionsButton.TabIndex = 2;
            this.ResetOptionsButton.Text = "설정 초기화";
            this.ResetOptionsButton.UseVisualStyleBackColor = true;
            this.ResetOptionsButton.Click += new System.EventHandler(this.ResetOptionsButton_Click);
            // 
            // SkippingTagsGroupBox
            // 
            this.SkippingTagsGroupBox.Controls.Add(this.label1);
            this.SkippingTagsGroupBox.Controls.Add(this.textBox2);
            this.SkippingTagsGroupBox.Location = new System.Drawing.Point(13, 92);
            this.SkippingTagsGroupBox.Name = "SkippingTagsGroupBox";
            this.SkippingTagsGroupBox.Size = new System.Drawing.Size(212, 73);
            this.SkippingTagsGroupBox.TabIndex = 3;
            this.SkippingTagsGroupBox.TabStop = false;
            this.SkippingTagsGroupBox.Text = "크롤링 스킵할 태그";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 11);
            this.label1.TabIndex = 1;
            this.label1.Text = "띄어쓰기x, \',\'로 구분";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(7, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(199, 21);
            this.textBox2.TabIndex = 0;
            this.textBox2.Text = "신문고,";
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(237, 450);
            this.Controls.Add(this.SkippingTagsGroupBox);
            this.Controls.Add(this.ResetOptionsButton);
            this.Controls.Add(this.SaveOptionsButton);
            this.Controls.Add(this.PageFindingAlgorithmGroupBox);
            this.Name = "OptionsForm";
            this.Text = "옵션";
            this.PageFindingAlgorithmGroupBox.ResumeLayout(false);
            this.PageFindingAlgorithmGroupBox.PerformLayout();
            this.SkippingTagsGroupBox.ResumeLayout(false);
            this.SkippingTagsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox PageFindingAlgorithmGroupBox;
        private System.Windows.Forms.RadioButton PFABinaryRadioButton;
        private System.Windows.Forms.RadioButton PFASetPageButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button SaveOptionsButton;
        private System.Windows.Forms.Button ResetOptionsButton;
        private System.Windows.Forms.GroupBox SkippingTagsGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}