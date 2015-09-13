using ByDSolutionDocumentationGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ByDSolutionDocumentationGenerator.Parser {
    public class SolutionFileParser {

        private Configuration configuration;

        public SolutionFileParser(Configuration configuration) {
            this.configuration = configuration;
        }

        public Solution ParseSolutionFile(string fileContent) {
            var solution = new Solution();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fileContent);
            var root = xmlDoc.DocumentElement;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("x", xmlDoc.DocumentElement.NamespaceURI);

            var nameNode = root.SelectSingleNode("/x:Project/x:PropertyGroup/x:Name", nsmgr);
            solution.Name = nameNode.InnerText;

            var solutionContent = root.SelectNodes("/x:Project/x:ItemGroup/x:Content", nsmgr);
            foreach (XmlNode i in solutionContent) {
                foreach (XmlAttribute a in i.Attributes) {
                    if (a.Name == "Include" && a.Value.EndsWith(".bo")) {
                        solution.BusinessObjectFiles.AddLast(a.Value);
                    }
                }
            }

            return solution;
        }
    }
}
