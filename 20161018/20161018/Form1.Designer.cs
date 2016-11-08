namespace _20161018
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
            this.btn_inputFile = new System.Windows.Forms.Button();
            this.txtTest = new System.Windows.Forms.TextBox();
            this.txt_showPath = new System.Windows.Forms.TextBox();
            this.btn_convert = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lab_time = new System.Windows.Forms.Label();
            this.lab_time_show = new System.Windows.Forms.Label();
            this.btn_stop = new System.Windows.Forms.Button();
            this.radioBtnYY = new System.Windows.Forms.RadioButton();
            this.radioBtnYT = new System.Windows.Forms.RadioButton();
            this.radioBtnYF = new System.Windows.Forms.RadioButton();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lab_test = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btn_inputFile
            // 
            this.btn_inputFile.Location = new System.Drawing.Point(12, 12);
            this.btn_inputFile.Name = "btn_inputFile";
            this.btn_inputFile.Size = new System.Drawing.Size(75, 23);
            this.btn_inputFile.TabIndex = 0;
            this.btn_inputFile.Text = "选择文件";
            this.btn_inputFile.UseVisualStyleBackColor = true;
            this.btn_inputFile.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtTest
            // 
            this.txtTest.Location = new System.Drawing.Point(12, 185);
            this.txtTest.Multiline = true;
            this.txtTest.Name = "txtTest";
            this.txtTest.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTest.Size = new System.Drawing.Size(378, 139);
            this.txtTest.TabIndex = 1;
            // 
            // txt_showPath
            // 
            this.txt_showPath.Location = new System.Drawing.Point(93, 14);
            this.txt_showPath.Name = "txt_showPath";
            this.txt_showPath.ReadOnly = true;
            this.txt_showPath.Size = new System.Drawing.Size(297, 21);
            this.txt_showPath.TabIndex = 2;
            // 
            // btn_convert
            // 
            this.btn_convert.Enabled = false;
            this.btn_convert.Location = new System.Drawing.Point(234, 46);
            this.btn_convert.Name = "btn_convert";
            this.btn_convert.Size = new System.Drawing.Size(75, 23);
            this.btn_convert.TabIndex = 3;
            this.btn_convert.Text = "转换";
            this.btn_convert.UseVisualStyleBackColor = true;
            this.btn_convert.Click += new System.EventHandler(this.btn_convert_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lab_time
            // 
            this.lab_time.AutoSize = true;
            this.lab_time.Location = new System.Drawing.Point(232, 90);
            this.lab_time.Name = "lab_time";
            this.lab_time.Size = new System.Drawing.Size(65, 12);
            this.lab_time.TabIndex = 6;
            this.lab_time.Text = "运行时间：";
            // 
            // lab_time_show
            // 
            this.lab_time_show.AutoSize = true;
            this.lab_time_show.Location = new System.Drawing.Point(303, 90);
            this.lab_time_show.Name = "lab_time_show";
            this.lab_time_show.Size = new System.Drawing.Size(0, 12);
            this.lab_time_show.TabIndex = 7;
            // 
            // btn_stop
            // 
            this.btn_stop.Enabled = false;
            this.btn_stop.Location = new System.Drawing.Point(315, 46);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(75, 23);
            this.btn_stop.TabIndex = 8;
            this.btn_stop.Text = "停止";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // radioBtnYY
            // 
            this.radioBtnYY.AutoSize = true;
            this.radioBtnYY.Location = new System.Drawing.Point(13, 46);
            this.radioBtnYY.Name = "radioBtnYY";
            this.radioBtnYY.Size = new System.Drawing.Size(119, 16);
            this.radioBtnYY.TabIndex = 9;
            this.radioBtnYY.Text = "航线运输数据(YY)";
            this.radioBtnYY.UseVisualStyleBackColor = true;
            this.radioBtnYY.CheckedChanged += new System.EventHandler(this.radioBtnYY_CheckedChanged);
            // 
            // radioBtnYT
            // 
            this.radioBtnYT.AutoSize = true;
            this.radioBtnYT.Location = new System.Drawing.Point(13, 68);
            this.radioBtnYT.Name = "radioBtnYT";
            this.radioBtnYT.Size = new System.Drawing.Size(107, 16);
            this.radioBtnYT.TabIndex = 10;
            this.radioBtnYT.Text = "非生产数据(YT)";
            this.radioBtnYT.UseVisualStyleBackColor = true;
            this.radioBtnYT.CheckedChanged += new System.EventHandler(this.radioBtnYT_CheckedChanged);
            // 
            // radioBtnYF
            // 
            this.radioBtnYF.AutoSize = true;
            this.radioBtnYF.Location = new System.Drawing.Point(13, 90);
            this.radioBtnYF.Name = "radioBtnYF";
            this.radioBtnYF.Size = new System.Drawing.Size(131, 16);
            this.radioBtnYF.TabIndex = 11;
            this.radioBtnYF.Text = "飞机利用率数据(YF)";
            this.radioBtnYF.UseVisualStyleBackColor = true;
            this.radioBtnYF.CheckedChanged += new System.EventHandler(this.radioBtnYF_CheckedChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 112);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(377, 23);
            this.progressBar1.TabIndex = 12;
            // 
            // lab_test
            // 
            this.lab_test.AutoSize = true;
            this.lab_test.Location = new System.Drawing.Point(12, 146);
            this.lab_test.Name = "lab_test";
            this.lab_test.Size = new System.Drawing.Size(35, 12);
            this.lab_test.TabIndex = 14;
            this.lab_test.Text = "0/100";
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 336);
            this.Controls.Add(this.lab_test);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.radioBtnYF);
            this.Controls.Add(this.radioBtnYT);
            this.Controls.Add(this.radioBtnYY);
            this.Controls.Add(this.btn_stop);
            this.Controls.Add(this.lab_time_show);
            this.Controls.Add(this.lab_time);
            this.Controls.Add(this.btn_convert);
            this.Controls.Add(this.txt_showPath);
            this.Controls.Add(this.txtTest);
            this.Controls.Add(this.btn_inputFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "ConvertKid";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_inputFile;
        private System.Windows.Forms.TextBox txtTest;
        private System.Windows.Forms.TextBox txt_showPath;
        private System.Windows.Forms.Button btn_convert;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lab_time;
        private System.Windows.Forms.Label lab_time_show;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.RadioButton radioBtnYY;
        private System.Windows.Forms.RadioButton radioBtnYT;
        private System.Windows.Forms.RadioButton radioBtnYF;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lab_test;
        private System.Windows.Forms.Timer timer2;
    }
}

