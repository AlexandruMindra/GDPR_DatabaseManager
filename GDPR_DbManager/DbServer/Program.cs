using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tools;

namespace DbServer
{
    class Program
    {
        public static Dictionary<String, User> UserDB = new Dictionary<string, User>();
        public static Dictionary<String, String> UserList = new Dictionary<string, string>();
        public static int port = 55555;
        public static NetConnection server = new NetConnection();

        static void Main(string[] args)
        {
            AddUser("alex", "alex", "alex", "alexandrualexandru");
            server.OnConnect += server_onConect;
            server.OnDataReceived += server_OnDataReceived;
            server.OnDisconnect += server_OnDisconnect;

            server.Start(port);
            Console.WriteLine("Server start on port " + port);

            while (true)
            {
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "stop server":
                        break;
                    case "start server":
                        break;
                    default:
                        break;
                }
            }
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
                    User client = SearchByUser(LoginCredentials.User);
                    if (client.passwd == LoginCredentials.Password &&
                        tf.GeneratePin(client.code) == LoginCredentials.Code)
                    {
                        Console.WriteLine("User " + LoginCredentials.User + " connected");
                        server.Send(Encoding.UTF8.GetBytes("lgins"));
                    }
                    else
                    {
                        Console.WriteLine("User " + LoginCredentials.User + " failed to login");
                        server.Send(Encoding.UTF8.GetBytes("lginf"));
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

        public static void AddUser(string acc, string pass, string em, string strKey)
        {
            UserDB.Add("key", new User()
            {
                account = acc,
                passwd = pass,
                email = em,
                code = strKey
            });
            UserList.Add(acc, "key");
        }

        public static User SearchByUser(string account)
        {
            return UserDB[UserList[account]];
        }

        //public static void LoadData()
        //{
        //    var fileName = Path.Combine(Environment.GetFolderPath(
        //    Environment.SpecialFolder.ApplicationData) + @"\Kozmo", "JRData.xml");
        //    if (File.Exists(fileName))
        //    {
        //        XmlSerializer serializer = new XmlSerializer(typeof(List<DayData>));
        //        StreamReader reader = new StreamReader(fileName);
        //        List<DayData> data = (List<DayData>)serializer.Deserialize(reader);
        //        reader.Close();
        //        MyDays.AddRange(data);
        //        // Clean
        //        serializer = null;
        //        reader = null;
        //        data = null;
        //    }
        //    fileName = null;
        //}

        //public void SaveData()
        //{
        //    MyDays[NewFocusedPanel.TabIndex].DailyText = MyJournalText.Text;
        //    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Kozmo";
        //    var fileName = Path.Combine(Environment.GetFolderPath(
        //    Environment.SpecialFolder.ApplicationData) + @"\Kozmo", "JRData.xml");
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }
        //    XmlSerializer serializer = new XmlSerializer(typeof(List<DayData>));
        //    StreamWriter Writer = new StreamWriter(fileName);
        //    serializer.Serialize(Writer, MyDays);
        //    Writer.Close();
        //    // cleanup
        //    path = null;
        //    serializer = null;
        //    Writer = null;
        //    fileName = null;
        //}
    }

    public class User
    {
        public String account { get; set; }
        public String passwd { get; set; }
        public String code { get; set; }
        public String email { get; set; }
    }
}
