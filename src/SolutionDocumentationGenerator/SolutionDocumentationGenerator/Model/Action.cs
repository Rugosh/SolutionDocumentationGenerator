using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDocumentationGenerator.Model {
    public class Action : DocumentableElement {

        public string Name;

        public Action()
            : base() {
            Name = string.Empty;
        }
    }
}
