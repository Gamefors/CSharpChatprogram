using System;
using System.IO;

namespace Server
{
    enum LogType
    {
        Info,
        Error
    }

    class LogHelper
    {
        private static readonly string infoPrefix = "[SERVER|INFO] ";
        private static readonly string errorPrefix = "[SERVER|ERROR] ";

        private static string loggingPath = Directory.GetCurrentDirectory() + "/logs/";
        public LogHelper()
        {
            //create initial log folder.
            if (!Directory.Exists(loggingPath))
            {
                Directory.CreateDirectory(loggingPath);
            }
            //create initial log file
            loggingPath += DateTime.Today.ToString("d") + ".txt";
            if (!File.Exists(loggingPath))
            {
                using StreamWriter logFile = File.CreateText(loggingPath);
            }
        }

        public void Log(string msg, LogType type)
        {
            string timePrefix = DateTime.Now.ToString("[hh:mm:ss]");
            string loggedMsg;
            switch (type)
            {
                case LogType.Info:
                    loggedMsg = timePrefix + infoPrefix + msg;
                    break;
                case LogType.Error:
                    loggedMsg = timePrefix + errorPrefix + msg;
                    break;
                default:
                    loggedMsg = "";
                    break;
            }
            Console.WriteLine(loggedMsg);
            using StreamWriter logFile = File.AppendText(loggingPath);
            logFile.WriteLine(msg);
        }
    }
}
