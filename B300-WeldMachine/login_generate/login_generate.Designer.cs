namespace login_generate
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.LOGO = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.LOGO)).BeginInit();
            this.SuspendLayout();
            // 
            // LOGO
            // 
            this.LOGO.BackgroundImage = global::login_generate.Properties.Resources.LOGO;
            this.LOGO.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.LOGO.ErrorImage = null;
            this.LOGO.Location = new System.Drawing.Point(12, 12);
            this.LOGO.Name = "LOGO";
            this.LOGO.Size = new System.Drawing.Size(156, 50);
            this.LOGO.TabIndex = 37;
            this.LOGO.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(174, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(183, 50);
            this.button1.TabIndex = 38;
            this.button1.Text = "生成注册文件";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 72);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LOGO);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "login_generate";
            ((System.ComponentModel.ISupportInitialize)(this.LOGO)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox LOGO;
        private System.Windows.Forms.Button button1;
    }
}

