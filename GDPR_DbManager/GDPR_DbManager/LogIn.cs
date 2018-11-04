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

//////////erori....
//////////Exception thrown: 'System.Runtime.Serialization.SerializationException' in mscorlib.dll
//////////Exception thrown: 'System.ObjectDisposedException' in mscorlib.dll
//////////Exception thrown: 'System.ObjectDisposedException' in GDPR_DbManager.exe
//////////Exception thrown: 'System.Runtime.Serialization.SerializationException' in DbServer.exe
//////////Exception thrown: 'System.IO.IOException' in mscorlib.dll
//////////Exception thrown: 'System.IO.IOException' in DbServer.exe
namespace GDPR_DbManager
{
    public partial class LogIn : Form
    {
        MainPanel mainForm = new MainPanel();
        bool isConnected = false;

        public LogIn()
        {
            InitializeComponent();
            mainForm.client.OnConnect += client_onConnect;
            mainForm.client.OnDataReceived += Client_OnDataReceived;
            mainForm.client.OnDisconnect += client_onDisconnect;
        }

        #region ClientEvents
        private void client_onDisconnect(object sender, NetConnection connection)
        {
                isConnected = false;
                logInBtn.Enabled = true;
                logoutBtn.Enabled = false;
            
        }

        private void client_onConnect(object sender, NetConnection connection)
        {
            isConnected = true;
        }

        private void Client_OnDataReceived(object sender, NetConnection connection, byte[] e)
        {
            var ConvertedMessage = (Tools.NetworkData)Tools.Convertor.ByteArrayToObject(e);
            switch (ConvertedMessage.ComReason)
            {
                case Reason.Login:
                    break;
                case Reason.Response:
                    if (((string)ConvertedMessage.Data) == "lgins")
                    {
                        logInBtn.Enabled = false;
                        logoutBtn.Enabled = true;
                        //mainForm.ShowDialog();
                    }
                    if (((string)ConvertedMessage.Data) == "lginf")
                        MessageBox.Show("Wrong password or auth code");
                    if (((string)ConvertedMessage.Data) == "lginnf")
                        MessageBox.Show("User not found");
                    break;
                case Reason.Com:
                    break;
                default:
                    break;
            }
        }
        #endregion

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
                if (!isConnected)
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

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            mainForm.client.Disconnect();            
        }
    }
}
