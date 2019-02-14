using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3aj_lib;

namespace testlib1
{
    class Program
    {
        static void Main(string[] args)
        {
            _3aj._3ajinit();

            Console.WriteLine("Enter domain:");
            string domain = Console.ReadLine();
            Console.WriteLine("Record Type [Default: A]:");
            string type = Console.ReadLine();
            if (type == "")
            {
                type = "A";
            }
            var result = _3aj._3ajclient(String.Format("https://dns.google.com/resolve?name={0}&type={1}", domain, type));
            foreach (KeyValuePair<int, string> entry in result)
            {
                Console.WriteLine("[{0}] {1}", entry.Key, entry.Value);
            }

            Console.Read();

            _3aj._3ajclose();
        }
    }
}
