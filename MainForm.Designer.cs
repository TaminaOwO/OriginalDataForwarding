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
            this.Timer_Repaint = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.DataGridView_Clients = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button_RemoveClients = new System.Windows.Forms.Button();
            this.checkedListBox_Clients = new System.Windows.Forms.CheckedListBox();
            this.button_GetAllClients = new System.Windows.Forms.Button();
            this.tabPage_Statu = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.Label_ClientCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Label_HeartCountDown = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Label_ForwardingCount = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label_MaxSendMs = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label_AvgSendMs = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label_SendCount = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_Clients)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage_Statu.SuspendLayout();
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
            // Timer_Repaint
            // 
            this.Timer_Repaint.Enabled = true;
            this.Timer_Repaint.Interval = 1000;
            this.Timer_Repaint.Tick += new System.EventHandler(this.Timer_Repaint_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage_Statu);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(10, 34);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(429, 211);
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
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button_RemoveClients);
            this.tabPage3.Controls.Add(this.checkedListBox_Clients);
            this.tabPage3.Controls.Add(this.button_GetAllClients);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(421, 119);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Tool";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button_RemoveClients
            // 
            this.button_RemoveClients.Location = new System.Drawing.Point(317, 96);
            this.button_RemoveClients.Name = "button_RemoveClients";
            this.button_RemoveClients.Size = new System.Drawing.Size(89, 19);
            this.button_RemoveClients.TabIndex = 2;
            this.button_RemoveClients.Text = "剔除勾選連線";
            this.button_RemoveClients.UseVisualStyleBackColor = true;
            this.button_RemoveClients.Click += new System.EventHandler(this.button_RemoveClients_Click);
            // 
            // checkedListBox_Clients
            // 
            this.checkedListBox_Clients.FormattingEnabled = true;
            this.checkedListBox_Clients.Location = new System.Drawing.Point(18, 9);
            this.checkedListBox_Clients.Name = "checkedListBox_Clients";
            this.checkedListBox_Clients.Size = new System.Drawing.Size(279, 106);
            this.checkedListBox_Clients.TabIndex = 1;
            // 
            // button_GetAllClients
            // 
            this.button_GetAllClients.Location = new System.Drawing.Point(317, 9);
            this.button_GetAllClients.Name = "button_GetAllClients";
            this.button_GetAllClients.Size = new System.Drawing.Size(89, 19);
            this.button_GetAllClients.TabIndex = 0;
            this.button_GetAllClients.Text = "取得目前連線";
            this.button_GetAllClients.UseVisualStyleBackColor = true;
            this.button_GetAllClients.Click += new System.EventHandler(this.button_GetAllClients_Click);
            // 
            // tabPage_Statu
            // 
            this.tabPage_Statu.Controls.Add(this.label_AvgSendMs);
            this.tabPage_Statu.Controls.Add(this.label_SendCount);
            this.tabPage_Statu.Controls.Add(this.label6);
            this.tabPage_Statu.Controls.Add(this.label2);
            this.tabPage_Statu.Controls.Add(this.label_MaxSendMs);
            this.tabPage_Statu.Controls.Add(this.label7);
            this.tabPage_Statu.Controls.Add(this.label8);
            this.tabPage_Statu.Controls.Add(this.Label_ClientCount);
            this.tabPage_Statu.Controls.Add(this.label3);
            this.tabPage_Statu.Controls.Add(this.Label_HeartCountDown);
            this.tabPage_Statu.Controls.Add(this.label4);
            this.tabPage_Statu.Controls.Add(this.Label_ForwardingCount);
            this.tabPage_Statu.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Statu.Name = "tabPage_Statu";
            this.tabPage_Statu.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Statu.Size = new System.Drawing.Size(421, 185);
            this.tabPage_Statu.TabIndex = 3;
            this.tabPage_Statu.Text = "狀態";
            this.tabPage_Statu.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "有效連線數 : ";
            // 
            // Label_ClientCount
            // 
            this.Label_ClientCount.AutoSize = true;
            this.Label_ClientCount.Location = new System.Drawing.Point(85, 12);
            this.Label_ClientCount.Name = "Label_ClientCount";
            this.Label_ClientCount.Size = new System.Drawing.Size(11, 12);
            this.Label_ClientCount.TabIndex = 0;
            this.Label_ClientCount.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "心跳倒數 : ";
            // 
            // Label_HeartCountDown
            // 
            this.Label_HeartCountDown.AutoSize = true;
            this.Label_HeartCountDown.Location = new System.Drawing.Point(73, 40);
            this.Label_HeartCountDown.Name = "Label_HeartCountDown";
            this.Label_HeartCountDown.Size = new System.Drawing.Size(11, 12);
            this.Label_HeartCountDown.TabIndex = 0;
            this.Label_HeartCountDown.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(199, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "轉發數 : ";
            // 
            // Label_ForwardingCount
            // 
            this.Label_ForwardingCount.AutoSize = true;
            this.Label_ForwardingCount.Location = new System.Drawing.Point(255, 10);
            this.Label_ForwardingCount.Name = "Label_ForwardingCount";
            this.Label_ForwardingCount.Size = new System.Drawing.Size(11, 12);
            this.Label_ForwardingCount.TabIndex = 0;
            this.Label_ForwardingCount.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 12);
            this.label8.TabIndex = 4;
            this.label8.Text = "MaxUseTime : ";
            // 
            // label_MaxSendMs
            // 
            this.label_MaxSendMs.AutoSize = true;
            this.label_MaxSendMs.Location = new System.Drawing.Point(87, 70);
            this.label_MaxSendMs.Name = "label_MaxSendMs";
            this.label_MaxSendMs.Size = new System.Drawing.Size(11, 12);
            this.label_MaxSendMs.TabIndex = 3;
            this.label_MaxSendMs.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "AvgUseTime : ";
            // 
            // label_AvgSendMs
            // 
            this.label_AvgSendMs.AutoSize = true;
            this.label_AvgSendMs.Location = new System.Drawing.Point(86, 100);
            this.label_AvgSendMs.Name = "label_AvgSendMs";
            this.label_AvgSendMs.Size = new System.Drawing.Size(11, 12);
            this.label_AvgSendMs.TabIndex = 1;
            this.label_AvgSendMs.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(199, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "送出數 : ";
            // 
            // label_SendCount
            // 
            this.label_SendCount.AutoSize = true;
            this.label_SendCount.Location = new System.Drawing.Point(255, 40);
            this.label_SendCount.Name = "label_SendCount";
            this.label_SendCount.Size = new System.Drawing.Size(11, 12);
            this.label_SendCount.TabIndex = 5;
            this.label_SendCount.Text = "0";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 257);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.TextBox_DpscKeyId);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "OriginalDataForwarding";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_Clients)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage_Statu.ResumeLayout(false);
            this.tabPage_Statu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox TextBox_Status;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_DpscKeyId;
        private System.Windows.Forms.Timer Timer_Repaint;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView DataGridView_Clients;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button_RemoveClients;
        private System.Windows.Forms.CheckedListBox checkedListBox_Clients;
        private System.Windows.Forms.Button button_GetAllClients;
        private System.Windows.Forms.TabPage tabPage_Statu;
        private System.Windows.Forms.Label label_AvgSendMs;
        private System.Windows.Forms.Label label_SendCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_MaxSendMs;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label Label_ClientCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label Label_HeartCountDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label Label_ForwardingCount;
    }
}

