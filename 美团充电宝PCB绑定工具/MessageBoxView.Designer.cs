namespace 美团充电宝PCB绑定工具
{
    partial class MessageBoxView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Tips = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.No_Btn = new System.Windows.Forms.Button();
            this.Yes_Btn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.Tips, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 61.93548F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.06452F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(589, 310);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // Tips
            // 
            this.Tips.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tips.AutoSize = true;
            this.Tips.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Tips.Location = new System.Drawing.Point(263, 80);
            this.Tips.Name = "Tips";
            this.Tips.Size = new System.Drawing.Size(63, 32);
            this.Tips.TabIndex = 0;
            this.Tips.Text = "Tips";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.No_Btn, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.Yes_Btn, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 195);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(583, 112);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // No_Btn
            // 
            this.No_Btn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.No_Btn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.No_Btn.Location = new System.Drawing.Point(362, 31);
            this.No_Btn.Name = "No_Btn";
            this.No_Btn.Size = new System.Drawing.Size(150, 50);
            this.No_Btn.TabIndex = 0;
            this.No_Btn.Text = "取消";
            this.No_Btn.UseVisualStyleBackColor = true;
            this.No_Btn.Click += new System.EventHandler(this.No_Btn_Click);
            // 
            // Yes_Btn
            // 
            this.Yes_Btn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Yes_Btn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Yes_Btn.Location = new System.Drawing.Point(70, 31);
            this.Yes_Btn.Name = "Yes_Btn";
            this.Yes_Btn.Size = new System.Drawing.Size(150, 50);
            this.Yes_Btn.TabIndex = 1;
            this.Yes_Btn.Text = "确认";
            this.Yes_Btn.UseVisualStyleBackColor = true;
            this.Yes_Btn.Click += new System.EventHandler(this.Yes_Btn_Click);
            // 
            // MessageBoxView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 310);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MessageBoxView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "提示";
            this.TopMost = true;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label Tips;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button No_Btn;
        private System.Windows.Forms.Button Yes_Btn;
    }
}