using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class CommandLineParser
    {
        public string Consume(string[] fullArgs)
        {
            return string.Format("Usage {0}", Path.Join(Directory.GetCurrentDirectory(), System.AppDomain.CurrentDomain.FriendlyName));
        }
    }
}
