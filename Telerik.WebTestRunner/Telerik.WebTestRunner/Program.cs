using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.CommandLineParsing;
using Telerik.WebTestRunner.Cmd;


namespace Telerik.WebTestRunner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CmdRunner CmdRunner = new CmdRunner();
            var Arguments = $"run /RunName=TestRun1 /Url=http://dev.reengineering.com /TraceFilePath=D:\\results.txt /TimeOutInMinutes=70 /assemblyname=SitefinityWebApp /UserName=test@test.com /Password=admin@123".Split();
            CmdRunner.Main(Arguments);
           // FromArguments(Arguments.Skip(1));
           // AddArgument("/UserName=\"test@test.com\"");

        }
        public static CommandLineDictionary FromArguments(IEnumerable<string> arguments)
        {
            return FromArguments(arguments, '/', '=');
        }

        public static CommandLineDictionary FromArguments(IEnumerable<string> arguments, char keyCharacter, char valueCharacter)
        {
            CommandLineDictionary commandLineDictionary = new CommandLineDictionary();
            commandLineDictionary.KeyCharacter = keyCharacter;
            commandLineDictionary.ValueCharacter = valueCharacter;
            foreach (string argument in arguments)
            {
                AddArgument(argument);
            }
            return commandLineDictionary;
        }
        private static void AddArgument(string argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            if (argument.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                string[] array = argument.Substring(1).Split('=');
                string key = array[0];
                string value = ((array.Length <= 1) ? string.Empty : string.Join("=", array, 1, array.Length - 1));
              //  Add(key, value);
                return;
            }

            throw new ArgumentException("Unsupported value line argument format.", argument);
        }
    }
}
