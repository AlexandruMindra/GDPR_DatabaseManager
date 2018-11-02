using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR_DatabaseManager
{
    class Program
    {

        static Server ServerObj = new Server();

        static void Main(string[] args)
        {
            ServerObj.Start();
            Console.ReadKey(true);
        }
    }
}
