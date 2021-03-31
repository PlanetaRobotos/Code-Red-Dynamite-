using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Logging
{
    public static class Logger
    {
        public static string LogFilePattern { private get; set; } =
            Path.Combine(Application.persistentDataPath, "{time}.{ext}");

        public static string DateFormatting { private get; set; } = "yy-MM-dd-HH-mm";


        public static bool UseLogFile { private get; set; }
        public static bool UseUnityConsole { private get; set; } = true;
        public static bool UseGameConsole { private get; set; }

        public static int FileWritingThreshold { private get; set; } = 16;

        public static GameObject GameConsole { private get; set; }
        public static IGameLogger GameLogMessage { private get; set; }
        public static IGameLogger GameInfoMessage { private get; set; }
        public static IGameLogger GameWarnMessage { private get; set; }
        public static IGameLogger GameErrorMessage { private get; set; }


        private static readonly Queue<LogMessage> _messagesCache = new Queue<LogMessage>();


        public static void Log(object msg) => ApplyLog(msg, LogType.Log, GameLogMessage);
        public static void Info(object msg) => ApplyLog(msg, LogType.Info, GameInfoMessage);
        public static void Warn(object msg) => ApplyLog(msg, LogType.Warn, GameWarnMessage);
        public static void Error(object msg) => ApplyLog(msg, LogType.Error, GameErrorMessage);


        private static void ApplyLog(object message, LogType logType, IGameLogger gameMessage)
        {
            string stringMessage = message.ToString();
            var messageInstance = new LogMessage(stringMessage, logType);
            _messagesCache.Enqueue(new LogMessage(stringMessage, logType));

            if (UseLogFile && (_messagesCache.Count > FileWritingThreshold
                               || logType is LogType.Error)) CreateLogsFile();
            if (UseGameConsole) gameMessage.AddMessage(messageInstance, GameConsole);

#if UNITY_EDITOR
            if (!UseUnityConsole) return;
            switch (logType)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Info:
                    Debug.Log(message);
                    break;
                case LogType.Warn:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType));
            }
#endif
        }


        public static void CreateLogsFile()
        {
            string fileName = GetLogFileName();
            Task.Run(async () =>
            {
                // Write cache data to buffer
                var builder = new StringBuilder();
                foreach (var message in _messagesCache)
                    builder.Append(message.ToFileFormatString());

                // Reset cache
                // _messagesCache.Clear();

                // Append text to file
                using (var stream = new StreamWriter(fileName, true, Encoding.UTF8))
                    await stream.WriteAsync(builder.ToString());

#if UNITY_EDITOR
                Debug.Log($"Log file written: {fileName}");
#endif
            });
        }
        
        


        public static string GetLogFileName()
        {
            var regex = new Regex(@"({(?:{??[^{]*?}))");
            var used = new Dictionary<string, string>();

            foreach (Match match in regex.Matches(LogFilePattern))
            {
                switch (match.Value)
                {
                    case "{time}":
                        used.Add("{time}", DateTime.Now.ToString(DateFormatting));
                        break;

                    case "{ext}":
                        used.Add("{ext}", "log");
                        break;
                }
            }

            return used.Aggregate(LogFilePattern, (current, item) =>
                current.Replace(item.Key, item.Value));
        }
    }
}