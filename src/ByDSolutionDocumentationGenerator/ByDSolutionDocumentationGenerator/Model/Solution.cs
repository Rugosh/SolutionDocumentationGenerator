using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDSolutionDocumentationGenerator.Model {
    public class Solution {

        public string Name;

        public LinkedList<string> BusinessObjectFiles;
        public LinkedList<Node> BusinessObjects;

        public Solution() {
            Name = string.Empty;

            BusinessObjectFiles = new LinkedList<string>();
            BusinessObjects = new LinkedList<Node>();
        }
    }
}
