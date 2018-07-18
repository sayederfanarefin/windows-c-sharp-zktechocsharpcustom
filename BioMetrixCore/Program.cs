using System;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace BioMetrixCore
{
    static class Program
    {

        static void OnTimedEvent(object source, ElapsedEventArgs e) {

            Boolean internet = CheckForInternetConnection();
            if (internet)
            {
                Console.WriteLine("We are online!");
                try
                {
                    guy g = new guy();
                    g.init(config);
                }
                catch (Exception exception) {
                    Console.WriteLine("I got an exception: "+ exception);
                }
               
            }
            else
            {

                Console.WriteLine("No internet. Trying again");
            }
           
        }
        static string config = "";
        static void Main(string[] args)
        {
            Console.WriteLine("Starting..." + timeStampString());

           

            var fileStream = new FileStream(@"config.txt", FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                config = streamReader.ReadToEnd();
                streamReader.Close();
            }
            
            if (config.Length < 2)
            {
                var fileStream2 = new FileStream(@"config.txt", FileMode.Open, FileAccess.ReadWrite);
                Console.WriteLine("Please enter the office code:");
                String officeCode = Console.ReadLine();
                byte[] officeCodeBytes = new UTF8Encoding(true).GetBytes(officeCode);

                fileStream2.Write(officeCodeBytes, 0, officeCodeBytes.Length);
                fileStream2.Close();
                fileStream.Close();
                fileStream = new FileStream(@"config.txt", FileMode.Open, FileAccess.Read);
            }
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                config = streamReader.ReadToEnd();
            }
            fileStream.Close();
            
            Console.WriteLine("Configured with:" + config);

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 30000;
            aTimer.Enabled = true;


            Console.WriteLine("Please press enter to stop");
            Console.ReadLine();

        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new System.Net.WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static string timeStampString()
        {
            return DateTime.Now.ToString("yyyy/MM/dd::HH:mm:ss:ffff");
        }

    }
}
