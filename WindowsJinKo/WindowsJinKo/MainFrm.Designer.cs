namespace WindowsJinKo
{
    partial class MainFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.tcSet = new System.Windows.Forms.TabControl();
            this.tpRun = new System.Windows.Forms.TabPage();
            this.lbTotalNum = new System.Windows.Forms.Label();
            this.lb_NG = new System.Windows.Forms.Label();
            this.lb_OK = new System.Windows.Forms.Label();
            this.tpModel = new System.Windows.Forms.TabPage();
            this.btnSaveImg = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.nudRectUp = new System.Windows.Forms.NumericUpDown();
            this.nudSpaceSecond = new System.Windows.Forms.NumericUpDown();
            this.label125 = new System.Windows.Forms.Label();
            this.cbShowRect = new System.Windows.Forms.CheckBox();
            this.nudWeldY = new System.Windows.Forms.NumericUpDown();
            this.nudRectW = new System.Windows.Forms.NumericUpDown();
            this.nudWeldX = new System.Windows.Forms.NumericUpDown();
            this.nudRectH = new System.Windows.Forms.NumericUpDown();
            this.nudSpaceMain = new System.Windows.Forms.NumericUpDown();
            this.nudRectDown = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSaveShapeRegion = new System.Windows.Forms.Button();
            this.cbDrawShap = new System.Windows.Forms.ComboBox();
            this.btnCreateShapeModel = new System.Windows.Forms.Button();
            this.btnSaveShapeModel = new System.Windows.Forms.Button();
            this.btnOneShot = new System.Windows.Forms.Button();
            this.tpParam = new System.Windows.Forms.TabPage();
            this.cbDefectRegion = new System.Windows.Forms.CheckBox();
            this.nudHoleMaxArea = new System.Windows.Forms.NumericUpDown();
            this.nudBulkiness = new System.Windows.Forms.NumericUpDown();
            this.nudHoleMinWidth = new System.Windows.Forms.NumericUpDown();
            this.nudHoleMaxHeight = new System.Windows.Forms.NumericUpDown();
            this.label174 = new System.Windows.Forms.Label();
            this.nudHoleMinHeight = new System.Windows.Forms.NumericUpDown();
            this.pictureBox15 = new System.Windows.Forms.PictureBox();
            this.tpPath = new System.Windows.Forms.TabPage();
            this.tbImgResultPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnImgResultPath = new System.Windows.Forms.Button();
            this.cbUser = new System.Windows.Forms.ComboBox();
            this.tbPass = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsslLocation = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslGray = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslPath = new System.Windows.Forms.ToolStripStatusLabel();
            this.hWindowMainID = new HalconDotNet.HWindowControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCamPath = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLog = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.LOGO = new System.Windows.Forms.PictureBox();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.tmrPath = new System.Windows.Forms.Timer(this.components);
            this.tcSet.SuspendLayout();
            this.tpRun.SuspendLayout();
            this.tpModel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRectUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpaceSecond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeldY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRectW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeldX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRectH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpaceMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRectDown)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tpParam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMaxArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBulkiness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMinWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMaxHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMinHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).BeginInit();
            this.tpPath.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LOGO)).BeginInit();
            this.SuspendLayout();
            // 
            // tcSet
            // 
            this.tcSet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcSet.Controls.Add(this.tpRun);
            this.tcSet.Controls.Add(this.tpModel);
            this.tcSet.Controls.Add(this.tpParam);
            this.tcSet.Controls.Add(this.tpPath);
            this.tcSet.Location = new System.Drawing.Point(0, 276);
            this.tcSet.Margin = new System.Windows.Forms.Padding(4);
            this.tcSet.Name = "tcSet";
            this.tcSet.SelectedIndex = 0;
            this.tcSet.Size = new System.Drawing.Size(424, 490);
            this.tcSet.TabIndex = 0;
            // 
            // tpRun
            // 
            this.tpRun.Controls.Add(this.lbTotalNum);
            this.tpRun.Controls.Add(this.lb_NG);
            this.tpRun.Controls.Add(this.lb_OK);
            this.tpRun.Location = new System.Drawing.Point(4, 28);
            this.tpRun.Margin = new System.Windows.Forms.Padding(4);
            this.tpRun.Name = "tpRun";
            this.tpRun.Padding = new System.Windows.Forms.Padding(4);
            this.tpRun.Size = new System.Drawing.Size(416, 458);
            this.tpRun.TabIndex = 0;
            this.tpRun.Text = "运行信息";
            this.tpRun.UseVisualStyleBackColor = true;
            // 
            // lbTotalNum
            // 
            this.lbTotalNum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbTotalNum.AutoSize = true;
            this.lbTotalNum.BackColor = System.Drawing.Color.Transparent;
            this.lbTotalNum.Font = new System.Drawing.Font("黑体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbTotalNum.ForeColor = System.Drawing.Color.Lime;
            this.lbTotalNum.Location = new System.Drawing.Point(4, 51);
            this.lbTotalNum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbTotalNum.Name = "lbTotalNum";
            this.lbTotalNum.Size = new System.Drawing.Size(123, 36);
            this.lbTotalNum.TabIndex = 344;
            this.lbTotalNum.Text = "总量:0";
            // 
            // lb_NG
            // 
            this.lb_NG.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_NG.AutoSize = true;
            this.lb_NG.BackColor = System.Drawing.Color.Transparent;
            this.lb_NG.Font = new System.Drawing.Font("黑体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_NG.ForeColor = System.Drawing.Color.Red;
            this.lb_NG.Location = new System.Drawing.Point(40, 245);
            this.lb_NG.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_NG.Name = "lb_NG";
            this.lb_NG.Size = new System.Drawing.Size(87, 36);
            this.lb_NG.TabIndex = 343;
            this.lb_NG.Text = "NG:0";
            // 
            // lb_OK
            // 
            this.lb_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_OK.AutoSize = true;
            this.lb_OK.BackColor = System.Drawing.Color.Transparent;
            this.lb_OK.Font = new System.Drawing.Font("黑体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_OK.ForeColor = System.Drawing.Color.Lime;
            this.lb_OK.Location = new System.Drawing.Point(40, 145);
            this.lb_OK.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_OK.Name = "lb_OK";
            this.lb_OK.Size = new System.Drawing.Size(87, 36);
            this.lb_OK.TabIndex = 342;
            this.lb_OK.Text = "OK:0";
            // 
            // tpModel
            // 
            this.tpModel.Controls.Add(this.btnSaveImg);
            this.tpModel.Controls.Add(this.groupBox3);
            this.tpModel.Controls.Add(this.groupBox2);
            this.tpModel.Controls.Add(this.btnOneShot);
            this.tpModel.Location = new System.Drawing.Point(4, 28);
            this.tpModel.Margin = new System.Windows.Forms.Padding(4);
            this.tpModel.Name = "tpModel";
            this.tpModel.Padding = new System.Windows.Forms.Padding(4);
            this.tpModel.Size = new System.Drawing.Size(416, 458);
            this.tpModel.TabIndex = 1;
            this.tpModel.Text = "模板创建";
            this.tpModel.UseVisualStyleBackColor = true;
            // 
            // btnSaveImg
            // 
            this.btnSaveImg.Location = new System.Drawing.Point(229, 10);
            this.btnSaveImg.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveImg.Name = "btnSaveImg";
            this.btnSaveImg.Size = new System.Drawing.Size(154, 52);
            this.btnSaveImg.TabIndex = 387;
            this.btnSaveImg.Text = "保存图像";
            this.btnSaveImg.UseVisualStyleBackColor = true;
            this.btnSaveImg.Click += new System.EventHandler(this.btnSaveImg_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.nudRectUp);
            this.groupBox3.Controls.Add(this.nudSpaceSecond);
            this.groupBox3.Controls.Add(this.label125);
            this.groupBox3.Controls.Add(this.cbShowRect);
            this.groupBox3.Controls.Add(this.nudWeldY);
            this.groupBox3.Controls.Add(this.nudRectW);
            this.groupBox3.Controls.Add(this.nudWeldX);
            this.groupBox3.Controls.Add(this.nudRectH);
            this.groupBox3.Controls.Add(this.nudSpaceMain);
            this.groupBox3.Controls.Add(this.nudRectDown);
            this.groupBox3.Location = new System.Drawing.Point(6, 237);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(403, 213);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "焊点位置";
            // 
            // nudRectUp
            // 
            this.nudRectUp.Location = new System.Drawing.Point(115, 140);
            this.nudRectUp.Margin = new System.Windows.Forms.Padding(4);
            this.nudRectUp.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRectUp.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRectUp.Name = "nudRectUp";
            this.nudRectUp.Size = new System.Drawing.Size(100, 28);
            this.nudRectUp.TabIndex = 454;
            this.nudRectUp.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRectUp.ValueChanged += new System.EventHandler(this.nudRectUp_ValueChanged);
            // 
            // nudSpaceSecond
            // 
            this.nudSpaceSecond.Location = new System.Drawing.Point(251, 68);
            this.nudSpaceSecond.Margin = new System.Windows.Forms.Padding(4);
            this.nudSpaceSecond.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nudSpaceSecond.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSpaceSecond.Name = "nudSpaceSecond";
            this.nudSpaceSecond.Size = new System.Drawing.Size(100, 28);
            this.nudSpaceSecond.TabIndex = 410;
            this.nudSpaceSecond.Value = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudSpaceSecond.ValueChanged += new System.EventHandler(this.nudSpaceSecond_ValueChanged);
            // 
            // label125
            // 
            this.label125.AutoSize = true;
            this.label125.Location = new System.Drawing.Point(16, 38);
            this.label125.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label125.Name = "label125";
            this.label125.Size = new System.Drawing.Size(80, 126);
            this.label125.TabIndex = 403;
            this.label125.Text = "检测位置\r\n\r\n焊点间距\r\n\r\n检测大小\r\n\r\n矩形位置";
            this.label125.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbShowRect
            // 
            this.cbShowRect.AutoSize = true;
            this.cbShowRect.Location = new System.Drawing.Point(115, 183);
            this.cbShowRect.Margin = new System.Windows.Forms.Padding(4);
            this.cbShowRect.Name = "cbShowRect";
            this.cbShowRect.Size = new System.Drawing.Size(142, 22);
            this.cbShowRect.TabIndex = 409;
            this.cbShowRect.Text = "显示焊点区域";
            this.cbShowRect.UseVisualStyleBackColor = true;
            // 
            // nudWeldY
            // 
            this.nudWeldY.Location = new System.Drawing.Point(251, 31);
            this.nudWeldY.Margin = new System.Windows.Forms.Padding(4);
            this.nudWeldY.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nudWeldY.Minimum = new decimal(new int[] {
            3000,
            0,
            0,
            -2147483648});
            this.nudWeldY.Name = "nudWeldY";
            this.nudWeldY.Size = new System.Drawing.Size(100, 28);
            this.nudWeldY.TabIndex = 404;
            this.nudWeldY.Value = new decimal(new int[] {
            778,
            0,
            0,
            0});
            this.nudWeldY.ValueChanged += new System.EventHandler(this.nudWeldY_ValueChanged);
            // 
            // nudRectW
            // 
            this.nudRectW.Location = new System.Drawing.Point(115, 103);
            this.nudRectW.Margin = new System.Windows.Forms.Padding(4);
            this.nudRectW.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudRectW.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRectW.Name = "nudRectW";
            this.nudRectW.Size = new System.Drawing.Size(100, 28);
            this.nudRectW.TabIndex = 408;
            this.nudRectW.Value = new decimal(new int[] {
            95,
            0,
            0,
            0});
            this.nudRectW.ValueChanged += new System.EventHandler(this.nudRectW_ValueChanged);
            // 
            // nudWeldX
            // 
            this.nudWeldX.Location = new System.Drawing.Point(115, 31);
            this.nudWeldX.Margin = new System.Windows.Forms.Padding(4);
            this.nudWeldX.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nudWeldX.Minimum = new decimal(new int[] {
            3000,
            0,
            0,
            -2147483648});
            this.nudWeldX.Name = "nudWeldX";
            this.nudWeldX.Size = new System.Drawing.Size(100, 28);
            this.nudWeldX.TabIndex = 405;
            this.nudWeldX.Value = new decimal(new int[] {
            1360,
            0,
            0,
            0});
            this.nudWeldX.ValueChanged += new System.EventHandler(this.nudWeldX_ValueChanged);
            // 
            // nudRectH
            // 
            this.nudRectH.Location = new System.Drawing.Point(251, 103);
            this.nudRectH.Margin = new System.Windows.Forms.Padding(4);
            this.nudRectH.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nudRectH.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRectH.Name = "nudRectH";
            this.nudRectH.Size = new System.Drawing.Size(100, 28);
            this.nudRectH.TabIndex = 407;
            this.nudRectH.Value = new decimal(new int[] {
            110,
            0,
            0,
            0});
            this.nudRectH.ValueChanged += new System.EventHandler(this.nudRectH_ValueChanged);
            // 
            // nudSpaceMain
            // 
            this.nudSpaceMain.Location = new System.Drawing.Point(115, 67);
            this.nudSpaceMain.Margin = new System.Windows.Forms.Padding(4);
            this.nudSpaceMain.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nudSpaceMain.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSpaceMain.Name = "nudSpaceMain";
            this.nudSpaceMain.Size = new System.Drawing.Size(100, 28);
            this.nudSpaceMain.TabIndex = 406;
            this.nudSpaceMain.Value = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudSpaceMain.ValueChanged += new System.EventHandler(this.nudSpaceMain_ValueChanged);
            // 
            // nudRectDown
            // 
            this.nudRectDown.Location = new System.Drawing.Point(251, 140);
            this.nudRectDown.Margin = new System.Windows.Forms.Padding(4);
            this.nudRectDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRectDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRectDown.Name = "nudRectDown";
            this.nudRectDown.Size = new System.Drawing.Size(100, 28);
            this.nudRectDown.TabIndex = 450;
            this.nudRectDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRectDown.ValueChanged += new System.EventHandler(this.nudRectDown_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSaveShapeRegion);
            this.groupBox2.Controls.Add(this.cbDrawShap);
            this.groupBox2.Controls.Add(this.btnCreateShapeModel);
            this.groupBox2.Controls.Add(this.btnSaveShapeModel);
            this.groupBox2.Location = new System.Drawing.Point(7, 78);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(402, 160);
            this.groupBox2.TabIndex = 386;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "创建模板";
            // 
            // btnSaveShapeRegion
            // 
            this.btnSaveShapeRegion.Location = new System.Drawing.Point(222, 92);
            this.btnSaveShapeRegion.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveShapeRegion.Name = "btnSaveShapeRegion";
            this.btnSaveShapeRegion.Size = new System.Drawing.Size(154, 52);
            this.btnSaveShapeRegion.TabIndex = 387;
            this.btnSaveShapeRegion.Text = "保存区域";
            this.btnSaveShapeRegion.UseVisualStyleBackColor = true;
            this.btnSaveShapeRegion.Click += new System.EventHandler(this.btnSaveShapeRegion_Click);
            // 
            // cbDrawShap
            // 
            this.cbDrawShap.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbDrawShap.FormattingEnabled = true;
            this.cbDrawShap.Items.AddRange(new object[] {
            "齐次矩形",
            "圆",
            "仿射矩形",
            "椭圆"});
            this.cbDrawShap.Location = new System.Drawing.Point(15, 42);
            this.cbDrawShap.Name = "cbDrawShap";
            this.cbDrawShap.Size = new System.Drawing.Size(154, 32);
            this.cbDrawShap.TabIndex = 386;
            this.cbDrawShap.Text = "齐次矩形";
            this.cbDrawShap.SelectedIndexChanged += new System.EventHandler(this.cbDrawShap_SelectedIndexChanged);
            // 
            // btnCreateShapeModel
            // 
            this.btnCreateShapeModel.Location = new System.Drawing.Point(222, 28);
            this.btnCreateShapeModel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateShapeModel.Name = "btnCreateShapeModel";
            this.btnCreateShapeModel.Size = new System.Drawing.Size(154, 52);
            this.btnCreateShapeModel.TabIndex = 385;
            this.btnCreateShapeModel.Text = "创建模板";
            this.btnCreateShapeModel.UseVisualStyleBackColor = true;
            this.btnCreateShapeModel.Click += new System.EventHandler(this.btnCreateShapeModel_Click);
            // 
            // btnSaveShapeModel
            // 
            this.btnSaveShapeModel.Location = new System.Drawing.Point(15, 92);
            this.btnSaveShapeModel.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveShapeModel.Name = "btnSaveShapeModel";
            this.btnSaveShapeModel.Size = new System.Drawing.Size(154, 52);
            this.btnSaveShapeModel.TabIndex = 384;
            this.btnSaveShapeModel.Text = "保存模板";
            this.btnSaveShapeModel.UseVisualStyleBackColor = true;
            this.btnSaveShapeModel.Click += new System.EventHandler(this.btnSaveShapeModel_Click);
            // 
            // btnOneShot
            // 
            this.btnOneShot.Location = new System.Drawing.Point(15, 10);
            this.btnOneShot.Margin = new System.Windows.Forms.Padding(4);
            this.btnOneShot.Name = "btnOneShot";
            this.btnOneShot.Size = new System.Drawing.Size(154, 52);
            this.btnOneShot.TabIndex = 383;
            this.btnOneShot.Text = "单帧采集";
            this.btnOneShot.UseVisualStyleBackColor = true;
            this.btnOneShot.Click += new System.EventHandler(this.btnOneShot_Click);
            // 
            // tpParam
            // 
            this.tpParam.Controls.Add(this.cbDefectRegion);
            this.tpParam.Controls.Add(this.nudHoleMaxArea);
            this.tpParam.Controls.Add(this.nudBulkiness);
            this.tpParam.Controls.Add(this.nudHoleMinWidth);
            this.tpParam.Controls.Add(this.nudHoleMaxHeight);
            this.tpParam.Controls.Add(this.label174);
            this.tpParam.Controls.Add(this.nudHoleMinHeight);
            this.tpParam.Controls.Add(this.pictureBox15);
            this.tpParam.Location = new System.Drawing.Point(4, 28);
            this.tpParam.Margin = new System.Windows.Forms.Padding(4);
            this.tpParam.Name = "tpParam";
            this.tpParam.Padding = new System.Windows.Forms.Padding(4);
            this.tpParam.Size = new System.Drawing.Size(416, 458);
            this.tpParam.TabIndex = 2;
            this.tpParam.Text = "调试参数";
            this.tpParam.UseVisualStyleBackColor = true;
            // 
            // cbDefectRegion
            // 
            this.cbDefectRegion.AutoSize = true;
            this.cbDefectRegion.Location = new System.Drawing.Point(167, 101);
            this.cbDefectRegion.Margin = new System.Windows.Forms.Padding(4);
            this.cbDefectRegion.Name = "cbDefectRegion";
            this.cbDefectRegion.Size = new System.Drawing.Size(142, 22);
            this.cbDefectRegion.TabIndex = 461;
            this.cbDefectRegion.Text = "显示缺陷区域";
            this.cbDefectRegion.UseVisualStyleBackColor = true;
            // 
            // nudHoleMaxArea
            // 
            this.nudHoleMaxArea.Location = new System.Drawing.Point(167, 280);
            this.nudHoleMaxArea.Margin = new System.Windows.Forms.Padding(4);
            this.nudHoleMaxArea.Maximum = new decimal(new int[] {
            -159383552,
            46653770,
            5421,
            0});
            this.nudHoleMaxArea.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHoleMaxArea.Name = "nudHoleMaxArea";
            this.nudHoleMaxArea.Size = new System.Drawing.Size(75, 28);
            this.nudHoleMaxArea.TabIndex = 460;
            this.nudHoleMaxArea.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudHoleMaxArea.ValueChanged += new System.EventHandler(this.nudHoleMaxArea_ValueChanged);
            // 
            // nudBulkiness
            // 
            this.nudBulkiness.DecimalPlaces = 2;
            this.nudBulkiness.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudBulkiness.Location = new System.Drawing.Point(167, 316);
            this.nudBulkiness.Margin = new System.Windows.Forms.Padding(4);
            this.nudBulkiness.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudBulkiness.Name = "nudBulkiness";
            this.nudBulkiness.Size = new System.Drawing.Size(75, 28);
            this.nudBulkiness.TabIndex = 458;
            this.nudBulkiness.Value = new decimal(new int[] {
            19,
            0,
            0,
            0});
            this.nudBulkiness.ValueChanged += new System.EventHandler(this.nudBulkiness_ValueChanged);
            // 
            // nudHoleMinWidth
            // 
            this.nudHoleMinWidth.Location = new System.Drawing.Point(167, 244);
            this.nudHoleMinWidth.Margin = new System.Windows.Forms.Padding(4);
            this.nudHoleMinWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHoleMinWidth.Name = "nudHoleMinWidth";
            this.nudHoleMinWidth.Size = new System.Drawing.Size(75, 28);
            this.nudHoleMinWidth.TabIndex = 457;
            this.nudHoleMinWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudHoleMinWidth.ValueChanged += new System.EventHandler(this.nudHoleMinWidth_ValueChanged);
            // 
            // nudHoleMaxHeight
            // 
            this.nudHoleMaxHeight.Location = new System.Drawing.Point(167, 206);
            this.nudHoleMaxHeight.Margin = new System.Windows.Forms.Padding(4);
            this.nudHoleMaxHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHoleMaxHeight.Name = "nudHoleMaxHeight";
            this.nudHoleMaxHeight.Size = new System.Drawing.Size(75, 28);
            this.nudHoleMaxHeight.TabIndex = 456;
            this.nudHoleMaxHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudHoleMaxHeight.ValueChanged += new System.EventHandler(this.nudHoleMaxHeight_ValueChanged);
            // 
            // label174
            // 
            this.label174.AutoSize = true;
            this.label174.Location = new System.Drawing.Point(43, 173);
            this.label174.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label174.Name = "label174";
            this.label174.Size = new System.Drawing.Size(116, 162);
            this.label174.TabIndex = 455;
            this.label174.Text = "孔的最小高度\r\n\r\n孔的最大高度\r\n\r\n孔的最小宽度\r\n\r\n孔的最小面积\r\n\r\n孔的彭松度";
            this.label174.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudHoleMinHeight
            // 
            this.nudHoleMinHeight.Location = new System.Drawing.Point(167, 169);
            this.nudHoleMinHeight.Margin = new System.Windows.Forms.Padding(4);
            this.nudHoleMinHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHoleMinHeight.Name = "nudHoleMinHeight";
            this.nudHoleMinHeight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.nudHoleMinHeight.Size = new System.Drawing.Size(75, 28);
            this.nudHoleMinHeight.TabIndex = 451;
            this.nudHoleMinHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudHoleMinHeight.ValueChanged += new System.EventHandler(this.nudHoleMinHeight_ValueChanged);
            // 
            // pictureBox15
            // 
            this.pictureBox15.BackgroundImage = global::WindowsJinKo.Properties.Resources.英利;
            this.pictureBox15.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox15.Location = new System.Drawing.Point(17, 10);
            this.pictureBox15.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox15.Name = "pictureBox15";
            this.pictureBox15.Size = new System.Drawing.Size(142, 141);
            this.pictureBox15.TabIndex = 452;
            this.pictureBox15.TabStop = false;
            // 
            // tpPath
            // 
            this.tpPath.Controls.Add(this.tbImgResultPath);
            this.tpPath.Controls.Add(this.label6);
            this.tpPath.Controls.Add(this.btnImgResultPath);
            this.tpPath.Location = new System.Drawing.Point(4, 28);
            this.tpPath.Name = "tpPath";
            this.tpPath.Padding = new System.Windows.Forms.Padding(3);
            this.tpPath.Size = new System.Drawing.Size(416, 458);
            this.tpPath.TabIndex = 3;
            this.tpPath.Text = "路径设置";
            this.tpPath.UseVisualStyleBackColor = true;
            // 
            // tbImgResultPath
            // 
            this.tbImgResultPath.Location = new System.Drawing.Point(13, 60);
            this.tbImgResultPath.Margin = new System.Windows.Forms.Padding(4);
            this.tbImgResultPath.Name = "tbImgResultPath";
            this.tbImgResultPath.ReadOnly = true;
            this.tbImgResultPath.Size = new System.Drawing.Size(295, 28);
            this.tbImgResultPath.TabIndex = 390;
            this.tbImgResultPath.Text = "D:\\Image\\";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 21);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(134, 18);
            this.label6.TabIndex = 389;
            this.label6.Text = "图像结果路径：";
            // 
            // btnImgResultPath
            // 
            this.btnImgResultPath.Location = new System.Drawing.Point(316, 55);
            this.btnImgResultPath.Margin = new System.Windows.Forms.Padding(4);
            this.btnImgResultPath.Name = "btnImgResultPath";
            this.btnImgResultPath.Size = new System.Drawing.Size(95, 38);
            this.btnImgResultPath.TabIndex = 388;
            this.btnImgResultPath.Text = "选择路径";
            this.btnImgResultPath.UseVisualStyleBackColor = true;
            this.btnImgResultPath.Click += new System.EventHandler(this.btnImgResultPath_Click);
            // 
            // cbUser
            // 
            this.cbUser.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbUser.FormattingEnabled = true;
            this.cbUser.Items.AddRange(new object[] {
            "用户登录",
            "售后调试",
            "软件调试"});
            this.cbUser.Location = new System.Drawing.Point(94, 96);
            this.cbUser.Margin = new System.Windows.Forms.Padding(4);
            this.cbUser.Name = "cbUser";
            this.cbUser.Size = new System.Drawing.Size(320, 29);
            this.cbUser.TabIndex = 405;
            this.cbUser.Text = "用户登录";
            // 
            // tbPass
            // 
            this.tbPass.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPass.Location = new System.Drawing.Point(94, 133);
            this.tbPass.Margin = new System.Windows.Forms.Padding(4);
            this.tbPass.Name = "tbPass";
            this.tbPass.Size = new System.Drawing.Size(320, 31);
            this.tbPass.TabIndex = 404;
            this.tbPass.UseSystemPasswordChar = true;
            this.tbPass.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPass_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.statusStrip);
            this.groupBox1.Controls.Add(this.hWindowMainID);
            this.groupBox1.Location = new System.Drawing.Point(425, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(861, 762);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "显示窗口";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslLocation,
            this.tsslGray,
            this.toolStripStatusLabel1,
            this.tsslPath});
            this.statusStrip.Location = new System.Drawing.Point(4, 729);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip.Size = new System.Drawing.Size(853, 29);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tsslLocation
            // 
            this.tsslLocation.Name = "tsslLocation";
            this.tsslLocation.Size = new System.Drawing.Size(100, 24);
            this.tsslLocation.Text = "图像坐标：";
            // 
            // tsslGray
            // 
            this.tsslGray.Name = "tsslGray";
            this.tsslGray.Size = new System.Drawing.Size(118, 24);
            this.tsslGray.Text = "图像灰度值：";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(86, 24);
            this.toolStripStatusLabel1.Text = "图像路径:";
            // 
            // tsslPath
            // 
            this.tsslPath.Name = "tsslPath";
            this.tsslPath.Size = new System.Drawing.Size(46, 24);
            this.tsslPath.Text = "路径";
            // 
            // hWindowMainID
            // 
            this.hWindowMainID.BackColor = System.Drawing.Color.Black;
            this.hWindowMainID.BorderColor = System.Drawing.Color.Black;
            this.hWindowMainID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowMainID.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowMainID.Location = new System.Drawing.Point(4, 25);
            this.hWindowMainID.Margin = new System.Windows.Forms.Padding(4);
            this.hWindowMainID.Name = "hWindowMainID";
            this.hWindowMainID.Size = new System.Drawing.Size(853, 733);
            this.hWindowMainID.TabIndex = 0;
            this.hWindowMainID.WindowSize = new System.Drawing.Size(853, 733);
            this.hWindowMainID.HMouseMove += new HalconDotNet.HMouseEventHandler(this.hWindowMainID_HMouseMove);
            this.hWindowMainID.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowMainID_HMouseDown);
            this.hWindowMainID.HMouseWheel += new HalconDotNet.HMouseEventHandler(this.hWindowMainID_HMouseWheel);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCamPath);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnLog);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.cbUser);
            this.panel1.Controls.Add(this.btnTest);
            this.panel1.Controls.Add(this.tbPass);
            this.panel1.Controls.Add(this.btnOpen);
            this.panel1.Controls.Add(this.LOGO);
            this.panel1.Controls.Add(this.btnHome);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Controls.Add(this.btnQuit);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(424, 269);
            this.panel1.TabIndex = 2;
            // 
            // btnCamPath
            // 
            this.btnCamPath.BackgroundImage = global::WindowsJinKo.Properties.Resources.相机1;
            this.btnCamPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCamPath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCamPath.Location = new System.Drawing.Point(7, 188);
            this.btnCamPath.Margin = new System.Windows.Forms.Padding(4);
            this.btnCamPath.Name = "btnCamPath";
            this.btnCamPath.Size = new System.Drawing.Size(75, 74);
            this.btnCamPath.TabIndex = 407;
            this.btnCamPath.TabStop = false;
            this.btnCamPath.UseVisualStyleBackColor = true;
            this.btnCamPath.Click += new System.EventHandler(this.btnCamPath_Click);
            this.btnCamPath.MouseEnter += new System.EventHandler(this.btnCamPath_MouseEnter);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = global::WindowsJinKo.Properties.Resources.保存;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Location = new System.Drawing.Point(256, 188);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 74);
            this.btnSave.TabIndex = 56;
            this.btnSave.TabStop = false;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnSave.MouseEnter += new System.EventHandler(this.btnSave_MouseEnter);
            // 
            // btnLog
            // 
            this.btnLog.BackgroundImage = global::WindowsJinKo.Properties.Resources.用户2;
            this.btnLog.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLog.Location = new System.Drawing.Point(7, 94);
            this.btnLog.Margin = new System.Windows.Forms.Padding(4);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(75, 75);
            this.btnLog.TabIndex = 406;
            this.btnLog.UseVisualStyleBackColor = true;
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            //this.btnLog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnLog_KeyDown);
            // 
            // btnDelete
            // 
            this.btnDelete.BackgroundImage = global::WindowsJinKo.Properties.Resources.删除;
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.Location = new System.Drawing.Point(339, 188);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 74);
            this.btnDelete.TabIndex = 58;
            this.btnDelete.TabStop = false;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnDelete.MouseEnter += new System.EventHandler(this.btnDelete_MouseEnter);
            // 
            // btnTest
            // 
            this.btnTest.BackgroundImage = global::WindowsJinKo.Properties.Resources.测试;
            this.btnTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTest.Location = new System.Drawing.Point(172, 188);
            this.btnTest.Margin = new System.Windows.Forms.Padding(4);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 74);
            this.btnTest.TabIndex = 59;
            this.btnTest.TabStop = false;
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            this.btnTest.MouseEnter += new System.EventHandler(this.btnTest_MouseEnter);
            // 
            // btnOpen
            // 
            this.btnOpen.BackgroundImage = global::WindowsJinKo.Properties.Resources.文件;
            this.btnOpen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpen.Location = new System.Drawing.Point(88, 188);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 74);
            this.btnOpen.TabIndex = 57;
            this.btnOpen.TabStop = false;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            this.btnOpen.MouseEnter += new System.EventHandler(this.btnOpen_MouseEnter);
            // 
            // LOGO
            // 
            this.LOGO.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("LOGO.BackgroundImage")));
            this.LOGO.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.LOGO.Location = new System.Drawing.Point(4, 8);
            this.LOGO.Margin = new System.Windows.Forms.Padding(4);
            this.LOGO.Name = "LOGO";
            this.LOGO.Size = new System.Drawing.Size(164, 74);
            this.LOGO.TabIndex = 52;
            this.LOGO.TabStop = false;
            // 
            // btnHome
            // 
            this.btnHome.BackgroundImage = global::WindowsJinKo.Properties.Resources.home;
            this.btnHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnHome.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHome.Location = new System.Drawing.Point(176, 8);
            this.btnHome.Margin = new System.Windows.Forms.Padding(4);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(74, 74);
            this.btnHome.TabIndex = 55;
            this.btnHome.TabStop = false;
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            this.btnHome.MouseEnter += new System.EventHandler(this.btnHome_MouseEnter);
            // 
            // btnStart
            // 
            this.btnStart.BackgroundImage = global::WindowsJinKo.Properties.Resources.启动1;
            this.btnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.Location = new System.Drawing.Point(258, 8);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(74, 74);
            this.btnStart.TabIndex = 53;
            this.btnStart.TabStop = false;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            this.btnStart.MouseEnter += new System.EventHandler(this.btnStart_MouseEnter);
            // 
            // btnQuit
            // 
            this.btnQuit.BackgroundImage = global::WindowsJinKo.Properties.Resources.停止1;
            this.btnQuit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnQuit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQuit.Location = new System.Drawing.Point(341, 8);
            this.btnQuit.Margin = new System.Windows.Forms.Padding(4);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(75, 74);
            this.btnQuit.TabIndex = 54;
            this.btnQuit.TabStop = false;
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            this.btnQuit.MouseEnter += new System.EventHandler(this.btnQuit_MouseEnter);
            // 
            // tmrPath
            // 
            this.tmrPath.Interval = 999;
            this.tmrPath.Tick += new System.EventHandler(this.tmrPath_Tick);
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1290, 766);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tcSet);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainFrm";
            this.Text = "欧普泰 V1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFrm_FormClosing);
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.tcSet.ResumeLayout(false);
            this.tpRun.ResumeLayout(false);
            this.tpRun.PerformLayout();
            this.tpModel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRectUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpaceSecond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeldY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRectW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeldX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRectH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpaceMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRectDown)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tpParam.ResumeLayout(false);
            this.tpParam.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMaxArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBulkiness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMinWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMaxHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMinHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).EndInit();
            this.tpPath.ResumeLayout(false);
            this.tpPath.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LOGO)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcSet;
        private System.Windows.Forms.TabPage tpRun;
        private System.Windows.Forms.TabPage tpModel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private HalconDotNet.HWindowControl hWindowMainID;
        private System.Windows.Forms.TabPage tpParam;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox LOGO;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCreateShapeModel;
        private System.Windows.Forms.Button btnSaveShapeModel;
        private System.Windows.Forms.Button btnOneShot;
        private System.Windows.Forms.NumericUpDown nudSpaceSecond;
        public System.Windows.Forms.CheckBox cbShowRect;
        private System.Windows.Forms.NumericUpDown nudRectW;
        private System.Windows.Forms.NumericUpDown nudRectH;
        private System.Windows.Forms.NumericUpDown nudSpaceMain;
        private System.Windows.Forms.NumericUpDown nudWeldX;
        private System.Windows.Forms.NumericUpDown nudWeldY;
        private System.Windows.Forms.Label label125;
        private System.Windows.Forms.ComboBox cbDrawShap;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ToolStripStatusLabel tsslLocation;
        private System.Windows.Forms.ToolStripStatusLabel tsslGray;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.ComboBox cbUser;
        private System.Windows.Forms.TextBox tbPass;
        private System.Windows.Forms.Button btnSaveImg;
        private System.Windows.Forms.Button btnSaveShapeRegion;
        private System.Windows.Forms.Button btnCamPath;
        private System.Windows.Forms.ToolStripStatusLabel tsslPath;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        public System.Windows.Forms.Label lb_NG;
        public System.Windows.Forms.Label lb_OK;
        private System.Windows.Forms.NumericUpDown nudHoleMaxArea;
        private System.Windows.Forms.NumericUpDown nudBulkiness;
        private System.Windows.Forms.NumericUpDown nudHoleMinWidth;
        private System.Windows.Forms.NumericUpDown nudHoleMaxHeight;
        private System.Windows.Forms.Label label174;
        private System.Windows.Forms.NumericUpDown nudHoleMinHeight;
        private System.Windows.Forms.NumericUpDown nudRectDown;
        private System.Windows.Forms.PictureBox pictureBox15;
        private System.Windows.Forms.NumericUpDown nudRectUp;
        public System.Windows.Forms.CheckBox cbDefectRegion;
        public System.Windows.Forms.Label lbTotalNum;
        private System.Windows.Forms.Timer tmrPath;
        private System.Windows.Forms.TabPage tpPath;
        private System.Windows.Forms.TextBox tbImgResultPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnImgResultPath;
    }
}

