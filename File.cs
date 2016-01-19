using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprocessing
{
    internal static class File
    {
        private static string _fileName = ".\\Log.txt";

        public static void PrintLog(int xA, int yA, int proc, int method, string time)
        {
            using (var writetext = new StreamWriter(_fileName))
            {
                writetext.WriteLine("Noodle " + DateTime.Now);
                writetext.WriteLine("Adding two matrixes {0} x {1}", xA, yA);
                writetext.WriteLine("Using {0} processors (treads)", proc);
                string system;
                switch (method)
                {
                    case 0:
                        system = "a synchronous system";
                        break;
                    case 1:
                        system = "an asynchronous system with separated control units";
                        break;
                    default:
                        system = "an asynchronous system with common control unit";
                        break;
                }
                writetext.WriteLine("In {0}", system);
                writetext.WriteLine("Took time: {0}", time);
            }
        }
    }
}
