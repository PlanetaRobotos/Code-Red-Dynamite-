using System;

namespace Core.Logging
{
    public readonly struct LogMessage
    {
        private static string[] _logTypeSymbols = {"log  ", "info ", "WARN ", "ERROR"};

        internal readonly string Text;
        internal readonly LogType Type;
        internal readonly string TimeString;

        internal LogMessage(string text, LogType type)
        {
            Text = text;
            Type = type;
            TimeString = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");
        }

        public string ToFileFormatString() =>
            $"{TimeString} \t {_logTypeSymbols[(int) Type]}\t {Text}\n";
    }
}