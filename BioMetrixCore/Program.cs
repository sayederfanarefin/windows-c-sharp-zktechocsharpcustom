using System;
using System.Windows.Forms;

namespace BioMetrixCore
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting..." + timeStampString());

            guy guy = new guy();
            guy.start();


            Console.WriteLine("Please press enter to stop");
            Console.ReadLine();

        }

        private static string timeStampString()
        {
            return DateTime.Now.ToString("yyyy/MM/dd::HH:mm:ss:ffff");
        }

    }
}
