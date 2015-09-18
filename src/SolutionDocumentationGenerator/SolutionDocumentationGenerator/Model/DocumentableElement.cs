using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDocumentationGenerator.Model {
    public class DocumentableElement {

        public DocumentableElement() {
            DocumentationLines = new LinkedList<string>();
        }

        public LinkedList<string> DocumentationLines;
    }
}
