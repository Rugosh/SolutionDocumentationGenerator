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

            // Generate BO files
            foreach (var bo in solution.BusinessObjects) {
                var boContenten = new StringBuilder();

                var htmlDoc = GetBaseHtmlDocument(string.Format("BO: {0}", bo.Name));
                var body = htmlDoc.CreateElement(htmlBody);

                body.AppendChild(GetSimpleHTMLElement(htmlDoc, htmlH1, string.Format("Business Object: {0}", bo.Name)));

                if (bo.Annotation.Count > 0) {
                    body.AppendChild(GenerateAnnotationPart(htmlDoc, bo.Annotation));
                }

                if (bo.Element.Count > 0) {
                    body.AppendChild(GenerateElemementPart(htmlDoc, bo.Element));
                }

                if (bo.Message.Count > 0) {
                    body.AppendChild(GenerateMessagePart(htmlDoc, bo.Message));
                }

                if (bo.Association.Count > 0) {
                    body.AppendChild(GenerateAssociationPart(htmlDoc, bo.Association));
                }

                if (bo.Action.Count > 0) {
                    body.AppendChild(GenerateActionPart(htmlDoc, bo.Action));
                }

                htmlDoc.DocumentElement.AppendChild(body);

                // Save BO fie
                var filename = System.IO.Path.Combine(configuration.OutputDir, string.Format("bo_{0}.html", bo.Name));
                if (System.IO.File.Exists(filename)) {
                    System.IO.File.Delete(filename);
                }
                if (configuration.Verbose) {
                    Console.WriteLine(string.Format("Write BO file: {0}", filename));
                }
                htmlDoc.Save(filename);
            }
        }

        private XmlElement GenerateAnnotationPart(XmlDocument baseDocument, LinkedList<Annotation> annotations) {
            var annotationDiv = GetDiv(baseDocument, htmlClassAnnotationCollection);

            foreach (var a in annotations) {
                annotationDiv.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, a.Name, htmlClassAnnotation));
            }

            return annotationDiv;
        }

        private XmlElement GenerateElemementPart(XmlDocument baseDocument, LinkedList<Element> elements) {
            var elementDiv = GetDiv(baseDocument, htmlClassBoElementCollection);

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

            foreach (var m in messages) {
                var message = GetDiv(baseDocument, htmlClassMessage);
                message.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, m.Name, htmlClassName));
                message.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, m.Text, htmlClassMessageText));

                if (m.PlaceHolderDataTypes.Count > 0) {
                    var datatypes = GetDiv(baseDocument, htmlClassMessageDataTypes);

                    foreach (var dt in m.PlaceHolderDataTypes) {
                        message.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, dt, htmlClassDataType));
                    }

                    message.AppendChild(datatypes);
                }

                messageDiv.AppendChild(message);
            }

            return messageDiv;
        }

        private XmlElement GenerateAssociationPart(XmlDocument baseDocument, LinkedList<Association> associations) {
            var associationDiv = GetDiv(baseDocument, htmlClassAssociationCollection);

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

            foreach (var a in action) {
                actionDiv.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, a, htmlClassName));
            }

            return actionDiv;
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

            var titleElement = htmlDoc.CreateElement("title");
            titleElement.AppendChild(htmlDoc.CreateTextNode(title));
            head.AppendChild(titleElement);

            return htmlDoc;
        }
    }
}
