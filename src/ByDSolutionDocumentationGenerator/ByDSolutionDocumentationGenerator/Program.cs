using ByDSolutionDocumentationGenerator.DocuGenerator;
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
                    if (configuration.SolutionPath == string.Empty) {
                        configuration.SolutionPath = i;
                    } else {
                        configuration.OutputDir = i;
                    }
                }
            }

            if (configuration.SolutionPath == string.Empty) {
                PrintHelp();
                return;
            } else {
                if (System.IO.Directory.Exists(configuration.SolutionPath) == false) {
                    Console.WriteLine("The following soultion path does not exist: {0}", configuration.SolutionPath);
                    return;
                }
            }

            if (configuration.OutputDir == string.Empty) {
                // TODO: set default

            } else {
                if (System.IO.Directory.Exists(configuration.OutputDir) == false) {
                    Console.WriteLine("The following output path does not exist: {0}", configuration.OutputDir);
                    return;
                }
            }

            // Parse Solution
            var solutionParser = new SolutionParser(configuration);
            var solution = solutionParser.ParseSolution();

            if (configuration.Verbose) {
                Console.WriteLine(solution.ToString());
            }

            // Generate Documentation
            var htmlGenerator = new HTMLDocuGenerator(configuration);
            htmlGenerator.GenerateDocumenation(solution);
        }

        private static void PrintHelp() {
            Console.WriteLine("Usage:");
            Console.WriteLine("<programm> [params] pathToStudioProjectFolder [outputDir]");
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
