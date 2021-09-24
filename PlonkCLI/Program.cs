using Neato;
using System;

namespace NeatoCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new CommandLineParser();

            parser.RegisterCommand("wave")
                .OnExecuted((parameters) => { Console.WriteLine("Hello!"); });

            parser.RegisterCommand("tick")
                .AddParameter(new Parameter("number of times", Parameter.PrimitiveType.Integer))
                .OnExecuted((parameters) =>
                {
                    for (int i = 0; i < parameters[0].AsInt(); i++)
                    {
                        Console.WriteLine("tick");
                    }
                });


            var api = new CommandLineHumanAPI(parser);
            api.UserInput(args);

            var error = api.NextErrorLine();
            while (error != null)
            {
                Console.Error.WriteLine($"[error] {error}");
                error = api.NextErrorLine();
            }
        }
    }
}
