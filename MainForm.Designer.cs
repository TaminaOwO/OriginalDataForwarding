namespace OriginalDataForwarding
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TextBox_Status = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TextBox_DpscKeyId = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Label_ForwardingCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Label_HeartCountDown = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Label_ClientCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Timer_Repaint = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.DataGridView_Clients = new System.Windows.Forms.DataGridView();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_Clients)).BeginInit();
            this.SuspendLayout();
            // 
            // TextBox_Status
            // 
            this.TextBox_Status.Location = new System.Drawing.Point(0, 3);
            this.TextBox_Status.Name = "TextBox_Status";
            this.TextBox_Status.Size = new System.Drawing.Size(418, 112);
            this.TextBox_Status.TabIndex = 0;
            this.TextBox_Status.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "DpscKeyId :";
            // 
            // TextBox_DpscKeyId
            // 
            this.TextBox_DpscKeyId.Location = new System.Drawing.Point(81, 6);
            this.TextBox_DpscKeyId.Name = "TextBox_DpscKeyId";
            this.TextBox_DpscKeyId.ReadOnly = true;
            this.TextBox_DpscKeyId.Size = new System.Drawing.Size(358, 22);
            this.TextBox_DpscKeyId.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Label_ForwardingCount);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.Label_HeartCountDown);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.Label_ClientCount);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(14, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(425, 45);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "狀態";
            // 
            // Label_ForwardingCount
            // 
            this.Label_ForwardingCount.AutoSize = true;
            this.Label_ForwardingCount.Location = new System.Drawing.Point(217, 18);
            this.Label_ForwardingCount.Name = "Label_ForwardingCount";
            this.Label_ForwardingCount.Size = new System.Drawing.Size(11, 12);
            this.Label_ForwardingCount.TabIndex = 0;
            this.Label_ForwardingCount.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(161, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "轉發數 : ";
            // 
            // Label_HeartCountDown
            // 
            this.Label_HeartCountDown.AutoSize = true;
            this.Label_HeartCountDown.Location = new System.Drawing.Point(395, 18);
            this.Label_HeartCountDown.Name = "Label_HeartCountDown";
            this.Label_HeartCountDown.Size = new System.Drawing.Size(11, 12);
            this.Label_HeartCountDown.TabIndex = 0;
            this.Label_HeartCountDown.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(327, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "心跳倒數 : ";
            // 
            // Label_ClientCount
            // 
            this.Label_ClientCount.AutoSize = true;
            this.Label_ClientCount.Location = new System.Drawing.Point(96, 18);
            this.Label_ClientCount.Name = "Label_ClientCount";
            this.Label_ClientCount.Size = new System.Drawing.Size(11, 12);
            this.Label_ClientCount.TabIndex = 0;
            this.Label_ClientCount.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "有效連線數 : ";
            // 
            // Timer_Repaint
            // 
            this.Timer_Repaint.Enabled = true;
            this.Timer_Repaint.Interval = 1000;
            this.Timer_Repaint.Tick += new System.EventHandler(this.Timer_Repaint_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(10, 85);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(429, 145);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.TextBox_Status);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(421, 119);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "訊息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.DataGridView_Clients);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(421, 119);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "當前連線資訊";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // DataGridView_Clients
            // 
            this.DataGridView_Clients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView_Clients.Location = new System.Drawing.Point(0, 3);
            this.DataGridView_Clients.Name = "DataGridView_Clients";
            this.DataGridView_Clients.RowTemplate.Height = 24;
            this.DataGridView_Clients.Size = new System.Drawing.Size(418, 116);
            this.DataGridView_Clients.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 234);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.TextBox_DpscKeyId);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "OriginalDataForwarding";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_Clients)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox TextBox_Status;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_DpscKeyId;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Label_ForwardingCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label Label_HeartCountDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label Label_ClientCount;
        private System.Windows.Forms.Timer Timer_Repaint;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView DataGridView_Clients;
    }
}

