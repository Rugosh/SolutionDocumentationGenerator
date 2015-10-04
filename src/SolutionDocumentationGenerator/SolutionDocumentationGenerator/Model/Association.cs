using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDocumentationGenerator.Model {
    public class Association : DocumentableElement {
        public string Name;

        public Multiplicity Multiplicity;

        public string Target;

        public LinkedList<Annotation> Annotation;

        public bool ContainsValuation {
            get {
                var retValue = false;
                if (Valuation != null && Valuation.Trim().Length > 0) {
                    retValue = true;
                }
                return retValue;
            }
        }

        public string Valuation;

        public Association()
            : base() {
            this.Name = string.Empty;
            this.Target = string.Empty;
            Multiplicity = Multiplicity.ZeroToOne;
            this.Annotation = new LinkedList<Annotation>();
            Valuation = string.Empty;
        }

        public Association(string name, Multiplicity multiplicity, string target)
            : base() {
            Name = name;
            Multiplicity = multiplicity;
            Target = target;
            this.Annotation = new LinkedList<Annotation>();
        }

        public override string ToString() {
            var retString = new StringBuilder();
            //retString.AppendLine(base.ToString());

            retString.AppendLine(string.Format("Association: {0}", Name));
            retString.AppendLine(string.Format("\tMultiplicity: {0}", Multiplicity));
            if (ContainsValuation) {
                retString.AppendLine(string.Format("\tValuation: {0}", Valuation));
            }
            foreach (var a in Annotation) {
                retString.AppendLine(string.Format("\t{0}", a));
            }

            return retString.ToString();
        }
    }
}
