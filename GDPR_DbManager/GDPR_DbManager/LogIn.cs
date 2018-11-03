using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;

namespace GDPR_DbManager
{
    public partial class LogIn : Form
    {
        MainPanel mainForm = new MainPanel();

        public LogIn()
        {
            InitializeComponent();
            mainForm.client.OnConnect += client_onConnect;
            mainForm.client.OnDataReceived += Client_OnDataReceived;
            mainForm.client.OnDisconnect += client_onDisconnect;
        }

        private void client_onDisconnect(object sender, NetConnection connection)
        {
            
        }

        private void client_onConnect(object sender, NetConnection connection)
        {

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
            if (userField.TextLength != 0 &&
                passwordField.TextLength != 0 &&
                keyField.TextLength != 0)
            {
                
                mainForm.client.Connect("localhost", 55555);
                mainForm.client.Send(Convertor.ObjectToByteArray(new Tools.NetworkData()
                {
                    ComReason = Tools.Reason.Login,
                    Data = new Tools.LoginData()
                    {
                        User = userField.Text,
                        Password = passwordField.Text,
                        Code = keyField.Text
                    }
                }));
                
            }

        }

        private void Client_OnDataReceived(object sender, NetConnection connection, byte[] e)
        {
            if (Encoding.UTF8.GetString(e) == "lginf")
                userField.Text = "azsdxfchgvjhbjnkm";
            //switch (Encoding.UTF8.GetString(e))
            //{
            //    case "lgins":
            //        break;
            //    case "lginf":
            //        MessageBox.Show("Datele de conectare sunt gresite! Va rugam incercati din nou!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        break;
            //    default:
            //        break;
            //}

        }
    }
}
