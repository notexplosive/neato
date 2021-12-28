using Neato;
using System;
using System.Collections.Generic;
using System.IO;

namespace NeatoCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var neato = new NeatoApp(new HumanFacingConsoleLogger());
            neato.ConsumeUserInput(args);
            neato.EmitErrorsIfApplicable();
        }
    }
}
