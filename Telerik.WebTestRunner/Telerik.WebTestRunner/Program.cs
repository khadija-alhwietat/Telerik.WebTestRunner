using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WebTestRunner.Cmd;


namespace Telerik.WebTestRunner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CmdRunner CmdRunner = new CmdRunner();
            var arg = new List<string> { "Run" }.ToArray();
            CmdRunner.Main(arg);
        }
    }
}
