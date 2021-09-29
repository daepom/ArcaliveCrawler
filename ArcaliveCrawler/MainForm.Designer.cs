
namespace ArcaliveCrawler
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.CrawlButton = new System.Windows.Forms.Button();
            this.StatExportButton = new System.Windows.Forms.Button();
            this.versionLabel = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // CrawlButton
            // 
            this.CrawlButton.Location = new System.Drawing.Point(12, 12);
            this.CrawlButton.Name = "CrawlButton";
            this.CrawlButton.Size = new System.Drawing.Size(118, 48);
            this.CrawlButton.TabIndex = 2;
            this.CrawlButton.Text = "1. 크롤링";
            this.CrawlButton.UseVisualStyleBackColor = true;
            this.CrawlButton.Click += new System.EventHandler(this.CrawlButton_Click);
            // 
            // StatExportButton
            // 
            this.StatExportButton.Location = new System.Drawing.Point(143, 12);
            this.StatExportButton.Name = "StatExportButton";
            this.StatExportButton.Size = new System.Drawing.Size(117, 48);
            this.StatExportButton.TabIndex = 3;
            this.StatExportButton.Text = "2. 통계 출력";
            this.StatExportButton.UseVisualStyleBackColor = true;
            this.StatExportButton.Click += new System.EventHandler(this.StatExportButton_Click);
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(202, 249);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(58, 12);
            this.versionLabel.TabIndex = 4;
            this.versionLabel.Text = "VERSION";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(10, 249);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(61, 12);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "linkLabel1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 270);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.StatExportButton);
            this.Controls.Add(this.CrawlButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "아카크롤러 2";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button CrawlButton;
        private System.Windows.Forms.Button StatExportButton;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}

