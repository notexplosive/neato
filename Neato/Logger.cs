using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public interface Logger
    {
        public void Info(string message);
        public void Error(string message);
        public void Warning(string message);
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public class BufferedLogger : Logger
    {

        public struct Message
        {
            public readonly string Content;
            public readonly LogLevel LogLevel;

            public Message(string content, LogLevel logLevel)
            {
                Content = content;
                LogLevel = logLevel;
            }

            public override string ToString()
            {
                return $"[{LogLevel}]: {Content}";
            }
        }

        readonly List<Message> Messages = new List<Message>();

        public void Error(string message)
        {
            Messages.Add(new Message(message, LogLevel.Error));
        }

        public void Info(string message)
        {
            Messages.Add(new Message(message, LogLevel.Info));
        }

        public void Warning(string message)
        {
            Messages.Add(new Message(message, LogLevel.Warning));
        }

        public string[] GetMessagesOfLevel(LogLevel logLevel)
        {
            var result = new List<string>();
            foreach (var message in Messages)
            {
                if (message.LogLevel == logLevel)
                {
                    result.Add(message.Content);
                }
            }

            return result.ToArray();
        }
    }

    public class HumanFacingConsoleLogger : Logger
    {
        public void Info(string message)
        {
            Console.WriteLine($"🔵 {message}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"🟥 {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"🔶 {message}");
        }
    }
}
