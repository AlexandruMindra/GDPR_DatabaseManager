namespace GDPR_DbManager
{
    partial class LogIn
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
            this.userField = new System.Windows.Forms.TextBox();
            this.passwordField = new System.Windows.Forms.TextBox();
            this.keyField = new System.Windows.Forms.TextBox();
            this.logInBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.logoutBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // userField
            // 
            this.userField.AcceptsTab = true;
            this.userField.Location = new System.Drawing.Point(28, 68);
            this.userField.Name = "userField";
            this.userField.Size = new System.Drawing.Size(175, 20);
            this.userField.TabIndex = 0;
            this.userField.Text = "User";
            this.userField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.userField.Click += new System.EventHandler(this.userField_Click);
            // 
            // passwordField
            // 
            this.passwordField.Location = new System.Drawing.Point(28, 94);
            this.passwordField.Name = "passwordField";
            this.passwordField.Size = new System.Drawing.Size(175, 20);
            this.passwordField.TabIndex = 1;
            this.passwordField.Text = "Password";
            this.passwordField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.passwordField.Click += new System.EventHandler(this.passwordField_Click);
            this.passwordField.TextChanged += new System.EventHandler(this.passwordField_TextChanged);
            // 
            // keyField
            // 
            this.keyField.Location = new System.Drawing.Point(28, 120);
            this.keyField.Name = "keyField";
            this.keyField.Size = new System.Drawing.Size(175, 20);
            this.keyField.TabIndex = 2;
            this.keyField.Text = "Code";
            this.keyField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.keyField.Click += new System.EventHandler(this.keyField_Click);
            // 
            // logInBtn
            // 
            this.logInBtn.Location = new System.Drawing.Point(121, 158);
            this.logInBtn.Name = "logInBtn";
            this.logInBtn.Size = new System.Drawing.Size(70, 23);
            this.logInBtn.TabIndex = 3;
            this.logInBtn.Text = "LogIn";
            this.logInBtn.UseVisualStyleBackColor = true;
            this.logInBtn.Click += new System.EventHandler(this.logInBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label1.Font = new System.Drawing.Font("Lucida Console", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(39, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "GDPR DbMaster";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logoutBtn
            // 
            this.logoutBtn.Enabled = false;
            this.logoutBtn.Location = new System.Drawing.Point(43, 158);
            this.logoutBtn.Name = "logoutBtn";
            this.logoutBtn.Size = new System.Drawing.Size(70, 23);
            this.logoutBtn.TabIndex = 5;
            this.logoutBtn.Text = "LogOut";
            this.logoutBtn.UseVisualStyleBackColor = true;
            this.logoutBtn.Click += new System.EventHandler(this.logoutBtn_Click);
            // 
            // LogIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(231, 204);
            this.Controls.Add(this.logoutBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logInBtn);
            this.Controls.Add(this.keyField);
            this.Controls.Add(this.passwordField);
            this.Controls.Add(this.userField);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LogIn";
            this.Text = "LogIn";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox userField;
        private System.Windows.Forms.TextBox passwordField;
        private System.Windows.Forms.TextBox keyField;
        private System.Windows.Forms.Button logInBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button logoutBtn;
    }
}

