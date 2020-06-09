namespace WinFormsClient
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.messagesList = new System.Windows.Forms.ListBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.authLabel = new System.Windows.Forms.Label();
            this.loginPicBox = new System.Windows.Forms.PictureBox();
            this.signupPicBox = new System.Windows.Forms.PictureBox();
            this.sendPicBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.loginPicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.signupPicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sendPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "login";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "to";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "message";
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(86, 34);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(187, 27);
            this.txtFrom.TabIndex = 1;
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(116, 109);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(125, 27);
            this.txtTo.TabIndex = 1;
            this.txtTo.TextChanged += new System.EventHandler(this.txtTo_TextChanged);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(116, 145);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(380, 27);
            this.txtMessage.TabIndex = 1;
            // 
            // messagesList
            // 
            this.messagesList.FormattingEnabled = true;
            this.messagesList.ItemHeight = 20;
            this.messagesList.Location = new System.Drawing.Point(37, 196);
            this.messagesList.Name = "messagesList";
            this.messagesList.Size = new System.Drawing.Size(905, 304);
            this.messagesList.TabIndex = 2;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(357, 34);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(139, 27);
            this.txtPassword.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(279, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "password";
            this.label4.Click += new System.EventHandler(this.label1_Click);
            // 
            // authLabel
            // 
            this.authLabel.AutoSize = true;
            this.authLabel.Location = new System.Drawing.Point(202, 72);
            this.authLabel.Name = "authLabel";
            this.authLabel.Size = new System.Drawing.Size(145, 20);
            this.authLabel.TabIndex = 4;
            this.authLabel.Text = "Welcome to my chat";
            // 
            // loginPicBox
            // 
            this.loginPicBox.Image = ((System.Drawing.Image)(resources.GetObject("loginPicBox.Image")));
            this.loginPicBox.Location = new System.Drawing.Point(524, 34);
            this.loginPicBox.Name = "loginPicBox";
            this.loginPicBox.Size = new System.Drawing.Size(135, 58);
            this.loginPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.loginPicBox.TabIndex = 5;
            this.loginPicBox.TabStop = false;
            this.loginPicBox.Click += new System.EventHandler(this.loginPicBox_Click);
            // 
            // signupPicBox
            // 
            this.signupPicBox.Image = ((System.Drawing.Image)(resources.GetObject("signupPicBox.Image")));
            this.signupPicBox.Location = new System.Drawing.Point(684, 34);
            this.signupPicBox.Name = "signupPicBox";
            this.signupPicBox.Size = new System.Drawing.Size(135, 58);
            this.signupPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.signupPicBox.TabIndex = 5;
            this.signupPicBox.TabStop = false;
            this.signupPicBox.Click += new System.EventHandler(this.loginPicBox_Click);
            // 
            // sendPicBox
            // 
            this.sendPicBox.Image = ((System.Drawing.Image)(resources.GetObject("sendPicBox.Image")));
            this.sendPicBox.Location = new System.Drawing.Point(524, 114);
            this.sendPicBox.Name = "sendPicBox";
            this.sendPicBox.Size = new System.Drawing.Size(135, 58);
            this.sendPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.sendPicBox.TabIndex = 5;
            this.sendPicBox.TabStop = false;
            this.sendPicBox.Click += new System.EventHandler(this.loginPicBox_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 516);
            this.Controls.Add(this.sendPicBox);
            this.Controls.Add(this.signupPicBox);
            this.Controls.Add(this.loginPicBox);
            this.Controls.Add(this.authLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.messagesList);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtTo);
            this.Controls.Add(this.txtFrom);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.loginPicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.signupPicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sendPicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.ListBox messagesList;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label authLabel;
        private System.Windows.Forms.PictureBox loginPicBox;
        private System.Windows.Forms.PictureBox signupPicBox;
        private System.Windows.Forms.PictureBox sendPicBox;
    }
}