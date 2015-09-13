using ByDSolutionDocumentationGenerator.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDSolutionDocumentationGenerator {
    public class Program {
        public static void Main(string[] args) {
            var configuration = new Configuration();

            if (args.Length < 1) {
                PrintHelp();
                return;
            }

            foreach (var i in args) {
                if (i == "-v" || i == "--verbose") {
                    configuration.Verbose = true;
                } else if (i == "-h" || i == "--help") {
                    
                }else {
                    configuration.SolutionPath = i;
                }
            }

            if (configuration.SolutionPath == string.Empty) {
                PrintHelp();
                return;
            }

            // Parse Solution
            var solutionParser = new SolutionParser(configuration);
            var solution = solutionParser.ParseSolution();

            if (configuration.Verbose) {
                Console.WriteLine(solution.ToString());
            }

            // TODO: Generate Documentation
        }

        private static void PrintHelp() {
            Console.WriteLine("Usage:");
            Console.WriteLine("<programm> [params] pathToStudioProjectFolder");
            Console.WriteLine("Parameters:");
            Console.WriteLine("-v|--verbose");
            Console.WriteLine("\tPrints more information out to the console");
            Console.WriteLine("-h|--help");
            Console.WriteLine("\tShow this help");
            Console.WriteLine("Usage Example");
            var examplePath = @"C:\Users\tok\Documents\CopernicusIsolatedShell\Projects\BYD_DEV\YEKRNL1PY";
            Console.WriteLine(string.Format("\tByDSolutionDocumentationGenerator.exe -v {0}", examplePath));
        }
    }
}
