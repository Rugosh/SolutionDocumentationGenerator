using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDocumentationGenerator.Model {
    public class Element : DocumentableElement {

        public string Name;

        public string DataType;

        public LinkedList<Annotation> Annotation;

        public string DefaultValue;

        public Element()
            : base() {
            this.Name = string.Empty;
            this.DataType = string.Empty;
            this.Annotation = new LinkedList<Annotation>();
            this.DefaultValue = string.Empty;
        }

        public override string ToString() {
            var retString = new StringBuilder();
            //retString.AppendLine(base.ToString());

            retString.AppendLine(string.Format("Element: {0}", Name));
            retString.AppendLine(string.Format("\tData Type: {0}", DataType));

            foreach (var a in this.Annotation) {
                retString.AppendLine(string.Format("\t{0}", a.ToString()));
            }

            return retString.ToString();
        }
    }
}
