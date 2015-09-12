using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDSolutionDocumentationGenerator.Model {
    public class Association {
        public string Name;

        public Multiplicity Multiplicity;

        public string Target;

        public LinkedList<Annotation> Annotation;

        public Association() {
            this.Name = string.Empty;
            this.Target = string.Empty;
            Multiplicity = Multiplicity.ZeroToOne;
            this.Annotation = new LinkedList<Annotation>();
        }

        public Association(string name, Multiplicity multiplicity, string target) {
            Name = name;
            Multiplicity = multiplicity;
            Target = target;
            this.Annotation = new LinkedList<Annotation>();
        }

        public override string ToString() {
            var retString = new StringBuilder();
            retString.AppendLine(base.ToString());

            retString.AppendLine(string.Format("Association: {0}", Name));
            retString.AppendLine(string.Format("\tMultiplicity: {0}", Multiplicity));
            retString.AppendLine(string.Format("\t{0}", Annotation));

            return retString.ToString();
        }
    }
}
