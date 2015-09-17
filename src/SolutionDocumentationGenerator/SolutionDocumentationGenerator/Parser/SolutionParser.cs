using SolutionDocumentationGenerator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDocumentationGenerator.Parser {
    public class SolutionParser {

        private Configuration configuration;

        public SolutionParser(Configuration configuration) {
            this.configuration = configuration;
        }

        public Solution ParseSolution() {
            var solutionFileParser = new SolutionFileParser(configuration);
            var solutionFilePath = Path.Combine(configuration.SolutionPath, string.Format("{0}.myproj", Path.GetFileName(configuration.SolutionPath)));
            var solution = solutionFileParser.ParseSolutionFile(LoadFileContent(solutionFilePath));

            var boParser = new BusinessObjectParser(configuration);
            foreach (var boFile in solution.BusinessObjectFiles) {
                var fullBoPath = Path.Combine(configuration.SolutionPath, boFile);
                var bo = boParser.ParseBusinessObject(LoadFileContent(fullBoPath));
                solution.BusinessObjects.AddLast(bo);
            }

            return solution;
        }

        private string LoadFileContent(string path) {
            using (var reader = new StreamReader(path)) {
                return reader.ReadToEnd();
            }
        }
    }
}
