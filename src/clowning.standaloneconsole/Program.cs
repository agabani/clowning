using System;
using clowning.blyncclient;
using clowning.standaloneconsole.Models;

namespace clowning.standaloneconsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new BlyncClient();
            var optionsExecuter = new OptionsExecuter(client);
            var consoleDisplay = new ConsoleDisplay();

            Console.WriteLine("Blync stand alone console");

            if (client.NumberOfDevices <= 0)
            {
                consoleDisplay.DisplayDeviceNotFoundError();
                return;
            }

            consoleDisplay.DisplayConnectedDevices(client);

            bool keepAlive;
            do
            {
                keepAlive = Run(consoleDisplay, optionsExecuter);
            } while (keepAlive);
        }

        private static bool Run(ConsoleDisplay consoleDisplay, OptionsExecuter optionsExecuter)
        {
            var options = new Options();
            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                consoleDisplay.DisplayUsage(options);
            }
            else
            {
                var arguments = userInput.Split(' ');

                if (CommandLine.Parser.Default.ParseArguments(arguments, options))
                {
                    return optionsExecuter.Execute(options);
                }
            }
            return true;
        }
    }
}