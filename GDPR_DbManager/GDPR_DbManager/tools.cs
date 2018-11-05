using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tools
{
    [Serializable]
    public  class LoginData
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
    }

    [Serializable]
    public enum Reason
    {
        Login,
        Response,
        Com,
    }

    [Serializable]
    public class NetworkData
    {
        public Reason ComReason { get; set; }
        public object Data { get; set; }
    }

    public class Convertor
    {
        // Convert an Object to a byte array
        public static byte[] ObjectToByteArray(object obj)
        {

            //if (obj == null)
            //    return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }

        }

        // Convert a byte array to an Object
        public static object ByteArrayToObject(byte[] arrBytes)
        {
            if (arrBytes.Length > 0)
            {
                MemoryStream memStream = new MemoryStream();
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                return binForm.Deserialize(memStream);
            }
            return null;
        }
    }
}
