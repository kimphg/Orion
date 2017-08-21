namespace Camera_PTZ
{
    partial class GuiMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GuiMain));
            this.button5 = new System.Windows.Forms.Button();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.ConnectingTimer = new System.Windows.Forms.Timer(this.components);
            this.timerPelcoCommand = new System.Windows.Forms.Timer(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label_radar_stat = new System.Windows.Forms.Label();
            this.textBox_t_num = new System.Windows.Forms.TextBox();
            this.textBox_t_range = new System.Windows.Forms.TextBox();
            this.textBox_t_bearing = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button_add_target = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(279, 393);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(122, 36);
            this.button5.TabIndex = 12;
            this.button5.Text = "Thoát";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipText = "Nhấn vào đây để khôi phục chương trình";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Camera control";
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // ConnectingTimer
            // 
            this.ConnectingTimer.Interval = 1000;
            this.ConnectingTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timerPelcoCommand
            // 
            this.timerPelcoCommand.Tick += new System.EventHandler(this.timerPelcoCommand_Tick);
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 24;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(391, 316);
            this.listBox1.TabIndex = 27;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 437);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(389, 20);
            this.textBox1.TabIndex = 20;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label_radar_stat
            // 
            this.label_radar_stat.AutoSize = true;
            this.label_radar_stat.Location = new System.Drawing.Point(10, 333);
            this.label_radar_stat.Name = "label_radar_stat";
            this.label_radar_stat.Size = new System.Drawing.Size(94, 13);
            this.label_radar_stat.TabIndex = 28;
            this.label_radar_stat.Text = "Chưa kết nối radar";
            // 
            // textBox_t_num
            // 
            this.textBox_t_num.Location = new System.Drawing.Point(11, 364);
            this.textBox_t_num.Name = "textBox_t_num";
            this.textBox_t_num.Size = new System.Drawing.Size(74, 20);
            this.textBox_t_num.TabIndex = 1;
            this.textBox_t_num.Text = "1";
            // 
            // textBox_t_range
            // 
            this.textBox_t_range.Location = new System.Drawing.Point(249, 364);
            this.textBox_t_range.MaxLength = 10;
            this.textBox_t_range.Name = "textBox_t_range";
            this.textBox_t_range.Size = new System.Drawing.Size(72, 20);
            this.textBox_t_range.TabIndex = 3;
            this.textBox_t_range.Text = "10";
            // 
            // textBox_t_bearing
            // 
            this.textBox_t_bearing.Location = new System.Drawing.Point(114, 364);
            this.textBox_t_bearing.MaxLength = 10;
            this.textBox_t_bearing.Name = "textBox_t_bearing";
            this.textBox_t_bearing.Size = new System.Drawing.Size(111, 20);
            this.textBox_t_bearing.TabIndex = 2;
            this.textBox_t_bearing.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 348);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "MT";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(246, 348);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Cự ly";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(111, 348);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "Phương vị";
            // 
            // button_add_target
            // 
            this.button_add_target.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button_add_target.Location = new System.Drawing.Point(11, 392);
            this.button_add_target.Margin = new System.Windows.Forms.Padding(2);
            this.button_add_target.Name = "button_add_target";
            this.button_add_target.Size = new System.Drawing.Size(122, 36);
            this.button_add_target.TabIndex = 4;
            this.button_add_target.Text = "Thêm mục tiêu";
            this.button_add_target.UseVisualStyleBackColor = false;
            this.button_add_target.Click += new System.EventHandler(this.button_add_target_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(348, 348);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(53, 36);
            this.button1.TabIndex = 29;
            this.button1.Text = "Mô phỏng";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // GuiMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(420, 463);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_add_target);
            this.Controls.Add(this.textBox_t_bearing);
            this.Controls.Add(this.textBox_t_range);
            this.Controls.Add(this.textBox_t_num);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label_radar_stat);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "GuiMain";
            this.Text = "Phần mềm điều khiển camera";
            this.Load += new System.EventHandler(this.PtzControl_Load);
            this.Resize += new System.EventHandler(this.ResizeEvent);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Timer ConnectingTimer;
        private System.Windows.Forms.Timer timerPelcoCommand;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label_radar_stat;
        private System.Windows.Forms.TextBox textBox_t_num;
        private System.Windows.Forms.TextBox textBox_t_range;
        private System.Windows.Forms.TextBox textBox_t_bearing;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button_add_target;
        private System.Windows.Forms.Button button1;

    }
}

