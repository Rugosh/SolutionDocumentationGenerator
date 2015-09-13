using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDSolutionDocumentationGenerator.Model {
    public class Node {

        public NodeType NodeType;

        public Multiplicity Multiplicity;

        public string Name;

        public LinkedList<Annotation> Annotation;

        public LinkedList<Element> Element;

        public LinkedList<Node> ChildNode;

        public LinkedList<Message> Message;

        public LinkedList<string> Action;

        public LinkedList<Association> Association;

        public Node() {
            this.NodeType = NodeType.Node;
            this.Annotation = new LinkedList<Annotation>();
            this.Element = new LinkedList<Element>();
            this.ChildNode = new LinkedList<Node>();
            this.Message = new LinkedList<Message>();
            this.Action = new LinkedList<string>();
            this.Association = new LinkedList<Association>();
        }

        public override string ToString() {
            var retString = new StringBuilder();

            //retString.AppendLine(base.ToString());
            retString.AppendLine(string.Format("{0}: {1}", NodeType == NodeType.BusinessObject ? "Business Object" : "Node", Name));
            foreach (var a in Annotation) {
                retString.AppendLine(string.Format("\t{0}", a.ToString()));
            }

            foreach (var m in Message) {
                retString.AppendLine(string.Format("\t{0}", m.ToString()));
            }

            foreach (var e in Element) {
                retString.AppendLine(string.Format("\t{0}", e.ToString()));
            }

            foreach (var n in ChildNode) {
                retString.AppendLine(string.Format("\t{0}", n.ToString()));
            }

            foreach (var a in Action) {
                retString.AppendLine(string.Format("\tAction: {0}", a.ToString()));
            }

            foreach (var a in Association) {
                retString.AppendLine(string.Format("\t{0}", a.ToString()));
            }

            return retString.ToString();
        }
    
    }
}
