using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.CommandLineParsing;
using Telerik.WebTestRunner.Cmd;


namespace Telerik.WebTestRunner
{
    public class Program
    {
       public static void Main(string[] args)
        {
            CmdRunner CmdRunner = new CmdRunner();
            var Arguments = $"run /RunName=TestRun1 /Url=http://dev.reengineering.com /TraceFilePath=D:\\results.txt /TimeOutInMinutes=70 /assemblyname=SitefinityWebApp /UserName=test@test.com /Password=admin@123".Split();
             CmdRunner.Main(Arguments);
            Console.WriteLine("khadija out put");
            Console.ReadLine();

        }
       
    }
}
