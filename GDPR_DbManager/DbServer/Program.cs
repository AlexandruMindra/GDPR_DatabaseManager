using System;
using System.Collections.Concurrent;
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

        public static NetConnection server = new NetConnection();
        public static int port = 55555;

        public static Dictionary<String, User> UserDB = new Dictionary<string, User>();
        public static Dictionary<String, String> EmailList = new Dictionary<string, string>();
        
        
        public static DateTime SavedTime;
        public static ConcurrentBag<string> LogsList = new ConcurrentBag<string>();
        public static bool WannaReadLogs = false;
        public static void StartLogging()
        {
            new Thread((ThreadStart)(() =>
            {
                while (ProgramIsRunning)
                {
                    try
                    {
                        if (!Tools.General.IsFileLocked(new FileInfo(GetLogFilePath())))
                        {
                            if (WannaReadLogs)
                            {
                                using (var reader = new StreamReader(GetLogFilePath(), true))
                                {
                                    string line;
                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        WriteOnColor(line, ConsoleColor.DarkGreen, true);
                                    }
                                }
                                WannaReadLogs = false;
                            }
                            if (!LogsList.IsEmpty)
                            {
                                try
                                {
                                    while (LogsList.TryTake(out string logString))
                                    {

                                        File.AppendAllText(GetLogFilePath(),
                                            DateTime.Now.ToShortDateString() + "/" +
                                            DateTime.Now.ToShortTimeString() + ":" +
                                            logString + Environment.NewLine);

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                            Thread.Sleep(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    
                }
            })).Start();
        }

        public static void LogData(string logString)
        {
            LogsList.Add(logString);
        }

        static void Main(string[] args)
        {

            if (!File.Exists(GetLogFilePath()))
                File.Create(GetLogFilePath());

            // Startup <---------------------------------------.
            ProgramIsRunning = true;//                         |
            LoadData();//                                      |
            SaveDataAsync(5);//                                |
            StartLogging();//                                  |
            //  <----------------------------------------------'

            // Console Events <--------------------------------.
            //                                                 |
            server.OnConnect += server_OnConnect;//            |
            server.OnDataReceived += server_OnDataReceived;//  |
            server.OnDisconnect += server_OnDisconnect;//      |
                                                       //      |
            OnCommand += Program_OnCommand;//                  |
            OnClosing += Program_OnClosing;//                  |
            // <-----------------------------------------------'

            server.Start(port);

            // Command Loop <--------------------------------------.
            while (ProgramIsRunning)//                             |
            {//                                                    |
                WriteOnColor(">>", ConsoleColor.Yellow, false);//  |
                OnCommand(Console.ReadLine());//                   |
            }//                                                    |
             //  <-------------------------------------------------'

            OnClosing();
        }

        private static void Program_OnClosing()
        {
            SaveData();
        }

        private static void Program_OnCommand(string command)
        {
            switch (command)
            {
                case "show user":

                    User cl = new User();
                    GetName:
                    {
                        WriteOnColor("Enter User Name : ", ConsoleColor.Blue, false);
                        string userName = Console.ReadLine();

                        if (userName == ">exit")
                        {
                            break;
                        }
                        else if (userName != "")
                        {
                            if (UserDB.ContainsKey(userName))
                            {
                                cl = UserDB[userName];
                                goto ShowUser;
                            }
                            else
                            {
                                WriteOnColor("  User not found!", ConsoleColor.Red, true);
                                goto GetName;
                            }
                            
                        }
                        else if(userName == "")
                        {
                            WriteOnColor("  User name cannot be empty!", ConsoleColor.Red, true);
                            goto GetName;
                        }
                    }
                    ShowUser:
                    {

                        WriteOnColor("  ::: User Details :::", ConsoleColor.Yellow, true);
                        Console.WriteLine("      User : " + cl.account + Environment.NewLine +
                                          "  Password : " + cl.passwd + Environment.NewLine +
                                          "     Email : " + cl.email + Environment.NewLine +
                                          "Privat Key : " + CreativeCommons.Transcoder.Base32Encode(cl.code));
                    }
                    
                    break;
                case "list users":
                    foreach (var user in UserDB.Values)
                    {
                        WriteOnColor("--------------------------------", ConsoleColor.Blue, true);
                        Console.WriteLine("    User : " + user.account + Environment.NewLine +
                                          "   Email : " + user.email + Environment.NewLine);
                    }
                    break;

                case "add user":
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
                    Console.Write("show user            "); WriteOnColor("Show user's details", ConsoleColor.Magenta, true);
                    Console.Write("list users           "); WriteOnColor("List users from database", ConsoleColor.Magenta, true);
                    Console.Write("add user             "); WriteOnColor("Create a new client", ConsoleColor.Magenta, true);
                    Console.Write("show logs            "); WriteOnColor("List the logs", ConsoleColor.Magenta, true);
                    Console.Write("Exit                 "); WriteOnColor("Exit the application", ConsoleColor.Magenta, true);
                    WriteOnColor("==========================================", ConsoleColor.Green, true); 
                    break;

                case "show logs":
                    WannaReadLogs = true;
                    break;
                default:
                    
                    break;
            }

            if (command == "Exit" || command == "exit")
            {
                SaveAndExit = true; // Save and exit
            }
        }

        #region ServerEvents
        private static void server_OnConnect(object sender, NetConnection connection) => LogData(connection.RemoteEndPoint + " connected");

        private static void server_OnDisconnect(object sender, NetConnection connection)
        { 
            LogData(connection.RemoteEndPoint + " disconnected");
        }

        private static void server_OnDataReceived(object sender, NetConnection connection, byte[] e)
        {
            if (e.Length > 0)
            {
                NetworkData ReceivedData = (NetworkData)Convertor.ByteArrayToObject(e);
                switch (ReceivedData.ComReason)
                {
                    case Reason.Login:
                        GoogleTOTP tf = new GoogleTOTP();
                        LoginData LoginCredentials = (LoginData)ReceivedData.Data;

                        if (UserDB.ContainsKey(LoginCredentials.User))
                        {
                            User client = UserDB[LoginCredentials.User];
                            if (client.passwd == LoginCredentials.Password) //  && tf.GeneratePin(client.code) == LoginCredentials.Code
                            {
                                LogData(connection.RemoteEndPoint + " login as " + LoginCredentials.User);
                                connection.Send(Tools.Convertor.ObjectToByteArray(new Tools.NetworkData()
                                {
                                    ComReason = Tools.Reason.Response,
                                    Data = "lgins"
                                }));
                            }
                            else
                            {
                                LogData(connection.RemoteEndPoint + " failed to login as " + LoginCredentials.User);
                                connection.Send(Tools.Convertor.ObjectToByteArray(new Tools.NetworkData()
                                {
                                    ComReason = Tools.Reason.Response,
                                    Data = "lginf"
                                }));
                            }
                        }
                        else
                        {
                            LogData(connection.RemoteEndPoint + " user not found " + LoginCredentials.User);
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
        }
        #endregion

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

        #region Save & Load DataFile Functions
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

        private static bool SaveAndExit = false;
        public static void SaveDataAsync(int Seconds)
        {
            SavedTime = DateTime.Now;
            
            new Thread((ThreadStart)(() =>
            {

                while (ProgramIsRunning)// Writing loop
                {
                    if ((DateTime.Now - SavedTime).TotalMilliseconds >= Seconds)
                    {
                        if(UserDB.Count > 0)
                            SaveData();
                    }
                    if (SaveAndExit)
                    {
                        if (UserDB.Count > 0)
                            SaveData();
                        ProgramIsRunning = false;
                        Environment.Exit(0);
                    }
                    Thread.Sleep(10);
                }
            })).Start();
        }

        private static string GetLogFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData) + @"\GDPRDatabase", "Log.txt");
        }
        #endregion

        public static void WriteOnColor(string Text, ConsoleColor color, bool Newline)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = color;
            Console.Write(Text);
            if (Newline) Console.WriteLine();
            Console.ResetColor();
        }

        public static User SearchByEmail(string email)
        {
            return UserDB[EmailList[email]];
        }
    }

    [Serializable]
    public class User
    {
        public String account { get; set; }
        public String passwd { get; set; }
        public byte[] code { get; set; }
        public String email { get; set; }
    }
}
