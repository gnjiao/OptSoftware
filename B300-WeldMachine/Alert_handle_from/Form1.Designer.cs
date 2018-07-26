namespace Alert_handle_from
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.LOGO = new System.Windows.Forms.PictureBox();
            this.button_rest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.LOGO)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 22.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Fuchsia;
            this.label1.Location = new System.Drawing.Point(162, 108);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(228, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = "设备急停!";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LOGO
            // 
            this.LOGO.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("LOGO.BackgroundImage")));
            this.LOGO.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.LOGO.Location = new System.Drawing.Point(156, 19);
            this.LOGO.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LOGO.Name = "LOGO";
            this.LOGO.Size = new System.Drawing.Size(234, 74);
            this.LOGO.TabIndex = 36;
            this.LOGO.TabStop = false;
            // 
            // button_rest
            // 
            this.button_rest.Font = new System.Drawing.Font("宋体", 12F);
            this.button_rest.Location = new System.Drawing.Point(156, 167);
            this.button_rest.Margin = new System.Windows.Forms.Padding(4);
            this.button_rest.Name = "button_rest";
            this.button_rest.Size = new System.Drawing.Size(234, 60);
            this.button_rest.TabIndex = 364;
            this.button_rest.Text = "重启软件";
            this.button_rest.UseVisualStyleBackColor = true;
            this.button_rest.Click += new System.EventHandler(this.button_rest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 244);
            this.Controls.Add(this.button_rest);
            this.Controls.Add(this.LOGO);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximumSize = new System.Drawing.Size(560, 300);
            this.MinimumSize = new System.Drawing.Size(560, 300);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "重启程序";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LOGO)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox LOGO;
        private System.Windows.Forms.Button button_rest;
    }
}

