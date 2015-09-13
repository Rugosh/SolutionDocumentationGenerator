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

        public override string ToString() {
            var retString = new StringBuilder();
            //retString.AppendLine(base.ToString());

            retString.AppendLine(string.Format("Solution: {0}", Name));
            foreach (var bo in this.BusinessObjects) {
                retString.AppendLine(string.Format("{0}", bo.ToString()));
            }

            return retString.ToString();
        }
    }
}
