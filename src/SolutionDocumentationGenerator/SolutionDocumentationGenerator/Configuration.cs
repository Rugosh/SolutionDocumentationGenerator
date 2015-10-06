﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDocumentationGenerator {
    public class Configuration {

        public Configuration() {
            Verbose = false;
            SolutionPath = string.Empty;
            OutputDir = string.Empty;
            Theme = string.Empty;
        }

        public bool Verbose;

        public string SolutionPath;

        public string OutputDir;

        public string Theme;
    }
}
