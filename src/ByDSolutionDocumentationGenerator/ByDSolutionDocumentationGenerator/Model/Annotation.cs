using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDSolutionDocumentationGenerator.Model {
    public class Annotation {

        public string Name;

        public Annotation() {
            this.Name = string.Empty;
        }

        public Annotation(string Name) {
            this.Name = Name;
        }

        public override string ToString() {
            var retString = new StringBuilder();
            //retString.AppendLine(base.ToString());

            retString.AppendLine(string.Format("Annotation: {0}", Name));

            return retString.ToString();
        }
    }
}
