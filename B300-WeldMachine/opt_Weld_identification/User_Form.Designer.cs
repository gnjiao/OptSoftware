namespace opt_Weld_identification
{
    partial class User_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(User_Form));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.用户登陆 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(324, 335);
            this.textBox1.Margin = new System.Windows.Forms.Padding(6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(270, 39);
            this.textBox1.TabIndex = 28;
            this.textBox1.UseSystemPasswordChar = true;
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "用户",
            "调试",
            "admin"});
            this.comboBox1.Location = new System.Drawing.Point(324, 285);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(270, 36);
            this.comboBox1.TabIndex = 29;
            this.comboBox1.Text = "用户";
            // 
            // 用户登陆
            // 
            this.用户登陆.BackgroundImage = global::opt_Weld_identification.Properties.Resources.用户1;
            this.用户登陆.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.用户登陆.Cursor = System.Windows.Forms.Cursors.Hand;
            this.用户登陆.Location = new System.Drawing.Point(212, 283);
            this.用户登陆.Margin = new System.Windows.Forms.Padding(6);
            this.用户登陆.Name = "用户登陆";
            this.用户登陆.Size = new System.Drawing.Size(100, 100);
            this.用户登陆.TabIndex = 30;
            this.用户登陆.UseVisualStyleBackColor = true;
            this.用户登陆.Click += new System.EventHandler(this.用户登陆_Click);
            // 
            // User_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::opt_Weld_identification.Properties.Resources.登陆广告;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(834, 378);
            this.Controls.Add(this.用户登陆);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(860, 449);
            this.MinimumSize = new System.Drawing.Size(860, 449);
            this.Name = "User_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "用户登录";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.User_Form_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button 用户登陆;

    }
}