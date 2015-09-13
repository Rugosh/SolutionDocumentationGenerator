using ByDSolutionDocumentationGenerator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDSolutionDocumentationGenerator.Parser {
    public class SolutionParser {

        private Configuration configuration;

        public SolutionParser(Configuration configuration) {
            this.configuration = configuration;
        }

        public Solution ParseSolution() {
            var solutionFileParser = new SolutionFileParser(configuration);
            var solutionFilePath = Path.Combine(configuration.SolutionPath, string.Format("{0}.myproj", Path.GetDirectoryName(configuration.SolutionPath)));
            var solution = solutionFileParser.ParseSolutionFile(solutionFilePath);

            // TODO

            return solution;
        }

        private string LoadFileContent(string path) {
            // TODO
            return null;
        }
    }
}
