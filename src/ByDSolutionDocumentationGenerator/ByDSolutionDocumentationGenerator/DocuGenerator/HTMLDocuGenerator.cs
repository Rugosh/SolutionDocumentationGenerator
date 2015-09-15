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

        private const string htmlClassBoElementCollection = "elementCollection";
        private const string htmlClassBoElement = "element";

        private const string htmlClassAnnotationCollection = "annotationcollection";
        private const string htmlClassAnnntation = "annotation";

        private const string htmlClassMessageCollection = "messagecollection";
        private const string htmlClassMessage = "message";
        private const string htmlClassMessageText = "messageText";
        private const string htmlClassMessageDataTypes = "messagedatatypes";

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

                if (bo.Element.Count > 0) {
                    body.AppendChild(GenerateElemementPart(htmlDoc, bo.Element));
                }

                if (bo.Message.Count > 0) {
                    body.AppendChild(GenerateMessagePart(htmlDoc, bo.Message));
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

        private XmlElement GenerateElemementPart(XmlDocument baseDocument, LinkedList<Element> elements) {
            var elementDiv = GetDiv(baseDocument, htmlClassBoElementCollection);

            foreach (var e in elements) {
                var element = GetDiv(baseDocument, htmlClassBoElement);
                element.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, e.Name, htmlClassName));
                element.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, e.DataType, htmlClassDataType));

                if (e.Annotation.Count > 0) {
                    var annotation = GetDiv(baseDocument, htmlClassAnnotationCollection);

                    foreach (var a in e.Annotation) {
                        annotation.AppendChild(GetSimpleHTMLElement(baseDocument, htmlTextOutputElement, a.Name, htmlClassAnnntation));
                    }

                    element.AppendChild(annotation);
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
