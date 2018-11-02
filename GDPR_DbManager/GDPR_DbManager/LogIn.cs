using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDPR_DbManager
{
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();
        }

        #region FieldChanges
        private void passwordField_TextChanged(object sender, EventArgs e)
        {
            passwordField.PasswordChar = '*';
        }

        private void userField_Click(object sender, EventArgs e)
        {
            userField.Text = "";
        }

        private void passwordField_Click(object sender, EventArgs e)
        {
            passwordField.Text = "";
        }

        private void keyField_Click(object sender, EventArgs e)
        {
            keyField.Text = "";
        }
        #endregion

        private void logInBtn_Click(object sender, EventArgs e)
        {
            if(userField.TextLength !=0 &&
                passwordField.TextLength !=0 &&
                keyField.TextLength !=0)
            {

            }
        }
    }
}
