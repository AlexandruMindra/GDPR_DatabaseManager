using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tools;

namespace DbServer
{
    class Program
    {

        #region Events
        public delegate void CommandDelegate(string command);
        public static event CommandDelegate OnCommand;

        public delegate void ClosingDelegate();
        public static event ClosingDelegate OnClosing;
        #endregion

        private static bool ProgramIsRunning = false;
        public static Dictionary<String, User> UserDB = new Dictionary<string, User>();
        public static Dictionary<String, String> EmailList = new Dictionary<string, string>();
        public static int port = 55555;
        public static NetConnection server = new NetConnection();

        static void Main(string[] args)
        {

            // Startup <---------------------------------------.
            ProgramIsRunning = true;//                         |
            LoadData();//                                      |
            //  <----------------------------------------------'

            // Console Events <--------------------------------.
            //                                                 |
            server.OnConnect += server_onConect;//             |
            server.OnDataReceived += server_OnDataReceived;//  |
            server.OnDisconnect += server_OnDisconnect;//      |
            //                                                 |
            OnCommand += Program_OnCommand;//                  |
            OnClosing += Program_OnClosing;//                  |
            // <-----------------------------------------------'
            server.Start(port);
            
            // Command Loop <----------------------------------.
            while (ProgramIsRunning)//                         |
            {//                                                |
                Console.Write("Enter Command : ");//           |
                OnCommand(Console.ReadLine());//               |
            }//                                                |
            //  <----------------------------------------------'

            OnClosing();
        }

        private static void Program_OnClosing()
        {
            // On Program Closing
        }

        private static void Program_OnCommand(string command)
        {
            switch (command)
            {
                case "list clients":
                    foreach (var user in UserDB.Values)
                    {
                        WriteOnColor("--------------------------------", ConsoleColor.Blue, true);
                        Console.WriteLine("    User : " + user.account + Environment.NewLine +
                                          "Password : " + user.passwd   + Environment.NewLine +
                                          "   Email : " + user.email + Environment.NewLine);
                    }
                    break;
                case "create client":
                    var newAccount = new User(); // New user to be added
                    goto Start;

                    Start:
                    {
                        Console.Clear();
                        WriteOnColor("::: Creating Client :::", ConsoleColor.Green, true);
                        goto Account;
                    }
                    Account:
                    {
                        Console.Write("User Name : ");
                        string Account = Console.ReadLine();
                        if (UserDB.ContainsKey(Account))
                        {
                            WriteOnColor("    User Already Exists!", ConsoleColor.Red, true);
                            goto Account;
                        }
                        else
                        {
                            if (Account != "")
                            {
                                newAccount.account = Account;
                                goto Password;
                            }
                            else
                            {
                                WriteOnColor("    User Name cannot be empty!", ConsoleColor.Red, true);
                                goto Account;
                            }
                        }
                    }
                    Password:
                    {
                        Console.Write(" Password : ");
                        string Password = Console.ReadLine();
                        if (Password != "")
                        {
                            newAccount.passwd = Password;
                            goto Email;
                        }
                        else
                        {
                            WriteOnColor("    Password cannot be empty!", ConsoleColor.Red, true);
                            goto Password;
                        }
                    }
                    Email:
                    {
                        Console.Write("    Email : ");
                        string Email = Console.ReadLine();
                        if (Email != "")
                        {
                            GoogleTOTP tf = new GoogleTOTP();
                            newAccount.email = Email;
                            newAccount.code = tf.getPrivateKey();
                            UserDB.Add(newAccount.account, newAccount);
                            WriteOnColor("Client " + newAccount.account + " was created successfully!", ConsoleColor.Yellow, true);
                            break;
                        }
                        else
                        {
                            WriteOnColor("    Email cannot be empty!", ConsoleColor.Red, true);
                            goto Email;

                        }
                    }
                case "help":

                    WriteOnColor("================== Help ==================", ConsoleColor.Green , true);
                    Console.Write("list clients             "); WriteOnColor("List clients from database", ConsoleColor.Magenta, true);
                    Console.Write("create client            "); WriteOnColor("Create a new client", ConsoleColor.Magenta, true);
                    Console.Write("Exit                     "); WriteOnColor("Exit the application", ConsoleColor.Magenta, true);
                    WriteOnColor("==========================================", ConsoleColor.Green, true); 
                    break;
            }

            if (command == "Exit" || command == "exit") ProgramIsRunning = false; // Exit Application
        }

        private static void server_OnDisconnect(object sender, NetConnection connection)
        {
            Console.WriteLine("Disconnection from " + connection.RemoteEndPoint);
        }

        private static void server_OnDataReceived(object sender, NetConnection connection, byte[] e)
        {
            NetworkData ReceivedData = (NetworkData)Convertor.ByteArrayToObject(e);
            switch (ReceivedData.ComReason)
            {
                case Reason.Login:

                    GoogleTOTP tf = new GoogleTOTP();
                    LoginData LoginCredentials = (LoginData)ReceivedData.Data;

                     
                    if(UserDB.ContainsKey(LoginCredentials.User))
                    {
                        User client = UserDB[LoginCredentials.User];
                        if (client.passwd == LoginCredentials.Password &&
                            tf.GeneratePin(client.code) == LoginCredentials.Code)
                        {
                            Console.WriteLine("User " + LoginCredentials.User + " connected");
                            connection.Send(Tools.Convertor.ObjectToByteArray(new Tools.NetworkData()
                            {
                                ComReason = Tools.Reason.Response,
                                Data = "lgins"
                            }));
                        }
                        else
                        {
                            Console.WriteLine("User " + LoginCredentials.User + " failed to login");
                            connection.Send(Tools.Convertor.ObjectToByteArray(new Tools.NetworkData()
                            {
                                ComReason = Tools.Reason.Response,
                                Data = "lginf"
                            }));
                        }
                    }
                    else
                    {
                        Console.WriteLine("User " + LoginCredentials.User + " not found");
                        connection.Send(Tools.Convertor.ObjectToByteArray(new Tools.NetworkData()
                        {
                            ComReason = Tools.Reason.Response,
                            Data = "lginnf"
                        }));
                    }
                        
                    break;

                case Reason.Com:
                    break;
                default:
                    break;
            }
        }

        private static void server_onConect(object sender, NetConnection connection)
        {
            Console.WriteLine("Connection from " + connection.RemoteEndPoint);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void WriteOnColor(string Text, ConsoleColor color, bool Newline)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = color;
            Console.Write(Text);
            if (Newline) Console.WriteLine();
            Console.ResetColor();
        }

        public static void AddUser(string acc, string pass, string em, byte[] strKey)
        {
            UserDB.Add(acc, new User()
            {
                account = acc,
                passwd = pass,
                email = em,
                code = strKey
            });
            EmailList.Add(em, acc);
        }

        public static User SearchByEmail(string email)
        {
            return UserDB[EmailList[email]];
        }

        public static void LoadData()
        {
            var fileName = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData) + @"\GDPRDatabase", "Users.xml");
            if (File.Exists(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
                StreamReader reader = new StreamReader(fileName);
                List<User> data = (List<User>)serializer.Deserialize(reader);
                reader.Close();
                foreach (var user in data)
                {
                    UserDB.Add(user.account, user);
                    EmailList.Add(user.email, user.account);
                }
                // Clean
                serializer = null;
                reader = null;
                data = null;
            }
            fileName = null;
        }

        public static void SaveData()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GDPRDatabase";
            var fileName = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData) + @"\GDPRDatabase", "Users.xml");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            StreamWriter Writer = new StreamWriter(fileName);

            List<User> ToSave = new List<User>();
            foreach (var user in UserDB.Values)
            {
                ToSave.Add(user);
            }

            serializer.Serialize(Writer, ToSave);
            Writer.Close();
            // cleanup
            path = null;
            serializer = null;
            Writer = null;
            fileName = null;
        }
    }

    public class User
    {
        public String account { get; set; }
        public String passwd { get; set; }
        public byte[] code { get; set; }
        public String email { get; set; }
    }
}
