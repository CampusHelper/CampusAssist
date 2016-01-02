namespace CampusAssist
{
    partial class Email
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
            this.Connect = new System.Windows.Forms.Button();
            this.Username = new System.Windows.Forms.TextBox();
            this.Password = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Disconnect = new System.Windows.Forms.Button();
            this.Retrieve = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // Connect
            // 
            this.Connect.Location = new System.Drawing.Point(315, 69);
            this.Connect.Name = "Connect";
            this.Connect.Size = new System.Drawing.Size(75, 23);
            this.Connect.TabIndex = 0;
            this.Connect.Text = "连接";
            this.Connect.UseVisualStyleBackColor = true;
            // 
            // Username
            // 
            this.Username.Location = new System.Drawing.Point(113, 69);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(100, 21);
            this.Username.TabIndex = 1;
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(113, 116);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(100, 21);
            this.Password.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "用户名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "密码：";
            // 
            // Disconnect
            // 
            this.Disconnect.Location = new System.Drawing.Point(315, 116);
            this.Disconnect.Name = "Disconnect";
            this.Disconnect.Size = new System.Drawing.Size(75, 23);
            this.Disconnect.TabIndex = 5;
            this.Disconnect.Text = "断开连接";
            this.Disconnect.UseVisualStyleBackColor = true;
            // 
            // Retrieve
            // 
            this.Retrieve.Location = new System.Drawing.Point(463, 161);
            this.Retrieve.Name = "Retrieve";
            this.Retrieve.Size = new System.Drawing.Size(75, 23);
            this.Retrieve.TabIndex = 6;
            this.Retrieve.Text = "收取文件";
            this.Retrieve.UseVisualStyleBackColor = true;
            // 
            // Status
            // 
            this.Status.FormattingEnabled = true;
            this.Status.ItemHeight = 12;
            this.Status.Location = new System.Drawing.Point(92, 219);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(195, 88);
            this.Status.TabIndex = 7;
            // 
            // Email
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 369);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.Retrieve);
            this.Controls.Add(this.Disconnect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.Username);
            this.Controls.Add(this.Connect);
            this.Name = "Email";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Email_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Connect;
        private System.Windows.Forms.TextBox Username;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Disconnect;
        private System.Windows.Forms.Button Retrieve;
        private System.Windows.Forms.ListBox Status;
    }
}