using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDSolutionDocumentationGenerator {
    public class Configuration {

        public Configuration() {
            Verbose = false;
            SolutionPath = string.Empty;
            OutputDir = string.Empty;
        }

        public bool Verbose;

        public string SolutionPath;

        public string OutputDir;
    }
}
