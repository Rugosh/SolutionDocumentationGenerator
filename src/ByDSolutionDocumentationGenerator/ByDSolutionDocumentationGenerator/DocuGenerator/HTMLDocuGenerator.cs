using ByDSolutionDocumentationGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ByDSolutionDocumentationGenerator.DocuGenerator {
    public class HTMLDocuGenerator {

        private const string html = "html";
        private const string htmlH1 = "h1";
        private const string htmlH2 = "h2";
        private const string htmlBody = "body";
        private const string htmlHead = "head";
        private const string htmlTextOutputElement = "span";
        private const string htmlDiv = "div";

        private const string htmlClass = "class";
        private const string htmlClassDataType = "datatype";
        private const string htmlClassName = "name";

        private const string htmlClassBoElementCollection = "elementcollection";
        private const string htmlClassBoElement = "element";

        private const string htmlClassAnnotationCollection = "annotationcollection";
        private const string htmlClassAnnotation = "annotation";

        private const string htmlClassMessageCollection = "messagecollection";
        private const string htmlClassMessage = "message";
        private const string htmlClassMessageText = "messageText";
        private const string htmlClassMessageDataTypes = "messagedatatypes";

        private const string htmlClassAssociationCollection = "associationcollection";
        private const string htmlClassAssociation = "association";
        private const string htmlClassAssociationTarget = "target";

        private const string htmlClassMultiplicity = "multiplicity";

        private const string htmlClassActionCollection = "actioncollection";

        private Configuration configuration;

        public HTMLDocuGenerator(Configuration configuration) {
            this.configuration = configuration;
        }

        public void GenerateDocumenation(Solution solution) {
            if (configuration.Verbose) {
                Console.WriteLine("Generate HTML Documentation");
            }

            var businessObjects = new Dictionary<string, string>();
            // Generate BO files
            foreach (var bo in solution.BusinessObjects) {
                var boContenten = new StringBuilder();

                var htmlDoc = GetBaseHtmlDocument(string.Format("BO: {0}", bo.Name));
                var body = htmlDoc.CreateElement(htmlBody);

                body.AppendChild(GetSimpleHTMLElement(htmlDoc, htmlH1, string.Format("Business Object: {0}", bo.Name)));

                GenerateNodeContent(bo, htmlDoc, body);

                htmlDoc.DocumentElement.AppendChild(body);

                // Save BO file
                var filename = System.IO.Path.Combine(configuration.OutputDir, string.Format("bo_{0}.html", bo.Name));
                businessObjects.Add(bo.Name, filename);
                if (System.IO.File.Exists(filename)) {
                    System.IO.File.Delete(filename);
                }
                if (configuration.Verbose) {
                    Console.WriteLine(string.Format("Write BO file: {0}", filename));
                }
                htmlDoc.Save(filename);
            }

            var indexDoc = GetBaseHtmlDocument(string.Format("Solution: {0}", solution.Name));
            var indexBody = indexDoc.CreateElement(htmlBody);

            indexBody.AppendChild(GetSimpleHTMLElement(indexDoc, htmlH1, string.Format("Solution {0}", solution.Name)));

            // Insert BO information
            if (businessObjects.Keys.Count > 0) {
                var boDiv = GetDiv(indexDoc, "businessobjectcollection");
                indexBody.AppendChild(GetSimpleHTMLElement(indexDoc, htmlH2, "Business Objects"));

                foreach (var bo in businessObjects.Keys.OrderBy(b => b)) {
                    var boElement = GetSimpleHTMLElement(indexDoc, "a", bo, "businessobject");
                    var href = indexDoc.CreateAttribute("href");
                    string hrefValue;
                    if (businessObjects.TryGetValue(bo, out hrefValue) == false) {
                        continue;
                    }
                    href.Value = string.Format(@"file:///{0}", hrefValue);
                    boElement.Attributes.Append(href);

                    boDiv.AppendChild(boElement);
                }

                indexBody.AppendChild(boDiv);
            }

            indexDoc.DocumentElement.AppendChild(indexBody);
            var indexFilename = System.IO.Path.Combine(configuration.OutputDir, "index.html");
            if (System.IO.File.Exists(indexFilename)) {
                System.IO.File.Delete(indexFilename);
            }
            if (configuration.Verbose) {
                Console.WriteLine(string.Format("Write Solution file: {0}", indexFilename));
            }
            indexDoc.Save(indexFilename);
        }

        private void GenerateNodeContent(Node node, XmlDocument baseDocument, XmlElement parentElement) {
            if (node.Annotation.Count > 0) {
                parentElement.AppendChild(GenerateAnnotationPart(baseDocument, node.Annotation));
            }

            if (node.Element.Count > 0) {
                parentElement.AppendChild(GenerateElemementPart(baseDocument, node.Element));
            }

            if (node.Message.Count > 0) {
                parentElement.AppendChild(GenerateMessagePart(baseDocument, node.Message));
            }

            if (node.Association.Count > 0) {
                parentElement.AppendChild(GenerateAssociationPart(baseDocument, node.Association));
            }

            if (node.Action.Count > 0) {
                parentElement.AppendChild(GenerateActionPart(baseDocument, node.Action));
            }

            if (node.ChildNode.Count > 0) {
                parentElement.AppendChild(GenerateNodePart(baseDocument, node.ChildNode));
            }
        }

        private XmlElement GenerateAnnotationPart(XmlDocument baseDocument, LinkedList<Annotation> annotations) {
            var annotationDiv = GetDiv(baseDocument, htmlClassAnnotationCollection);

            annotationDiv.AppendChild(GetSimpleHTMLElement(baseDocument, htmlH2, "Annotations", htmlClassName));

            foreach (var a in annotations) {
                annotationDiv.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, a.Name, htmlClassAnnotation));
            }

            return annotationDiv;
        }

        private XmlElement GenerateElemementPart(XmlDocument baseDocument, LinkedList<Element> elements) {
            var elementDiv = GetDiv(baseDocument, htmlClassBoElementCollection);

            elementDiv.AppendChild(GetSimpleHTMLElement(baseDocument, htmlH2, "Elements", htmlClassName));

            foreach (var e in elements) {
                var element = GetDiv(baseDocument, htmlClassBoElement);
                element.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, e.Name, htmlClassName));
                element.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, e.DataType, htmlClassDataType));

                if (e.Annotation.Count > 0) {
                    element.AppendChild(GenerateAnnotationPart(baseDocument, e.Annotation));
                }

                elementDiv.AppendChild(element);
            }

            return elementDiv;
        }

        private XmlElement GenerateMessagePart(XmlDocument baseDocument, LinkedList<Message> messages) {
            var messageDiv = GetDiv(baseDocument, htmlClassMessageCollection);

            messageDiv.AppendChild(GetSimpleHTMLElement(baseDocument, htmlH2, "Messages", htmlClassName));

            foreach (var m in messages) {
                var message = GetDiv(baseDocument, htmlClassMessage);
                message.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, m.Name, htmlClassName));
                message.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, m.Text, htmlClassMessageText));

                if (m.PlaceHolderDataTypes.Count > 0) {
                    var datatypes = GetDiv(baseDocument, htmlClassMessageDataTypes);

                    foreach (var dt in m.PlaceHolderDataTypes) {
                        datatypes.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, dt, htmlClassDataType));
                    }

                    message.AppendChild(datatypes);
                }

                messageDiv.AppendChild(message);
            }

            return messageDiv;
        }

        private XmlElement GenerateAssociationPart(XmlDocument baseDocument, LinkedList<Association> associations) {
            var associationDiv = GetDiv(baseDocument, htmlClassAssociationCollection);

            associationDiv.AppendChild(GetSimpleHTMLElement(baseDocument, htmlH2, "Associations", htmlClassName));

            foreach (var a in associations) {
                var association = GetDiv(baseDocument, htmlClassAssociation);

                association.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, a.Name, htmlClassName));
                association.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, a.Target, htmlClassAssociationTarget));
                association.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, GetMultiplicityText(a.Multiplicity), htmlClassMultiplicity));

                if (a.Annotation.Count > 0) {
                    association.AppendChild(GenerateAnnotationPart(baseDocument, a.Annotation));
                }

                associationDiv.AppendChild(association);
            }

            return associationDiv;
        }

        private XmlElement GenerateActionPart(XmlDocument baseDocument, LinkedList<string> action) {
            var actionDiv = GetDiv(baseDocument, htmlClassActionCollection);

            actionDiv.AppendChild(GetSimpleHTMLElement(baseDocument, htmlH2, "Actions", htmlClassName));

            foreach (var a in action) {
                actionDiv.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, a, htmlClassName));
            }

            return actionDiv;
        }

        private XmlElement GenerateNodePart(XmlDocument baseDocument, LinkedList<Node> nodes) {
            var nodeDiv = GetDiv(baseDocument, "nodecollection");

            nodeDiv.AppendChild(GetSimpleHTMLElement(baseDocument, htmlH2, "Nodes", htmlClassName));

            foreach (var n in nodes) {
                var node = GetDiv(baseDocument, "node");

                node.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, n.Name, htmlClassName));
                node.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, GetMultiplicityText(n.Multiplicity), htmlClassMultiplicity));
                GenerateNodeContent(n, baseDocument, node);

                nodeDiv.AppendChild(node);
            }

            return nodeDiv;
        }

        private string GetMultiplicityText(Multiplicity multiplicity) {
            var returnText = string.Empty;

            if (multiplicity == Multiplicity.OneToN) {
                returnText = "1 : n";

            } else if (multiplicity == Multiplicity.OneToOne) {
                returnText = "1 : 1";

            } else if (multiplicity == Multiplicity.ZeroToN) {
                returnText = "0 : n";

            } else if (multiplicity == Multiplicity.ZeroToOne) {
                returnText = "0 : 1";

            }

            return returnText;
        }

        private XmlElement GetDiv(XmlDocument baseDocument) {
            return baseDocument.CreateElement(htmlDiv);
        }

        private XmlElement GetDiv(XmlDocument baseDocument, string classValue) {
            var div = GetDiv(baseDocument);

            div.Attributes.Append(GetClass(baseDocument, classValue));

            return div;
        }

        private XmlNode GetSimpleHTMLElement(XmlDocument baseDocument, string title, string textValue) {
            var element = baseDocument.CreateElement(title);
            element.AppendChild(baseDocument.CreateTextNode(textValue));

            return element;
        }

        private XmlNode GetSimpleHTMLElement(XmlDocument baseDocument, string title, string textValue, string classValue) {
            var element = GetSimpleHTMLElement(baseDocument, title, textValue);


            element.Attributes.Append(GetClass(baseDocument, classValue));

            return element;
        }

        private XmlAttribute GetClass(XmlDocument baseDocument, string value) {
            var elementAttribute = baseDocument.CreateAttribute(htmlClass);
            elementAttribute.Value = value;

            return elementAttribute;
        }

        private XmlDocument GetBaseHtmlDocument(string title) {
            var htmlDoc = new XmlDocument();
            var docType = htmlDoc.CreateDocumentType(html, null, null, null);
            htmlDoc.AppendChild(docType);
            var root = htmlDoc.CreateElement(html);
            htmlDoc.AppendChild(root);

            var head = htmlDoc.CreateElement(htmlHead);
            root.AppendChild(head);

            var metadata = htmlDoc.CreateElement("meta");
            var charset = htmlDoc.CreateAttribute("charset");
            charset.Value = "utf-8";
            metadata.Attributes.Append(charset);
            head.AppendChild(metadata);

            var css = htmlDoc.CreateElement("link");
            var href = htmlDoc.CreateAttribute("href");
            href.Value = "./style.css";
            css.Attributes.Append(href);
            var type = htmlDoc.CreateAttribute("type");
            type.Value = "text/css";
            css.Attributes.Append(type);
            var rel = htmlDoc.CreateAttribute("rel");
            rel.Value = "stylesheet";
            css.Attributes.Append(rel);
            head.AppendChild(css);

            var titleElement = htmlDoc.CreateElement("title");
            titleElement.AppendChild(htmlDoc.CreateTextNode(title));
            head.AppendChild(titleElement);

            return htmlDoc;
        }
    }
}
