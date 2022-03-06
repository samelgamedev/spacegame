using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public static class Logger
    {
        // the logger wraps a private StreamWriter instance
        // that's closed after the application is quit
        private static StreamWriter writer;
        public static readonly string logsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\My Games\\space!!!!\\logs\\";

        public static bool initialized;
        public static void Init()
        {
            try
            {
                // create the logs directory if it doesn't exist
                if (!Directory.Exists(logsPath))
                    Directory.CreateDirectory(logsPath);

                // logs go under Documents/space!!!!/logs/
                writer = new StreamWriter($"{logsPath}log {DateTime.Now.ToString().Replace("/", ".").Replace(":", ".")}.txt");

                // call Done when the application is quitting
                Application.quitting += Done;

                WriteLine("logger initialized");
                initialized = true;
            }
            catch (Exception ex)
            {
                Debug.Log($"an exception was encountered in the logger initialization!\n{ex}");
            }
        }

        public static void WriteLine(string line)
        {
            try
            {
                writer.WriteLine(line);
            }
            catch (Exception ex)
            {
                Debug.Log($"an exception was encountered in the logger!\n{ex}");
            }
            finally
            {
                Debug.Log(line);
            }
        }

        public static void Done()
        {
            try
            {
                List<string> delete = new List<string>();
                foreach (FileInfo f in new DirectoryInfo(logsPath).GetFiles())
                {
                    // for every file in the /logs folder, check if it has been alive for more than 6 hours
                    DateTime lifespan = DateTime.Now - new TimeSpan(6, 0, 0);

                    if (f.CreationTime <= lifespan) delete.Add(f.FullName); // and if it has, add it to a collection
                }
                foreach (string s in delete) new FileInfo(s).Delete(); // then delete every file in that collection
            }
            catch (Exception ex)
            {
                Debug.Log($"an exception was encountered in the logger!\n{ex}");
            }
            finally
            {
                // close the stream writer
                writer.Close();
            }
        }
    }
}
