using SolutionDocumentationGenerator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SolutionDocumentationGenerator.DocuGenerator {
    public class HTMLDocuGenerator {

        private const string contentPlaceHolder = "###CONTENT###";
        private const string titlePlaceHolder = "###TITLE###";
        private const string pathPlaceHolder = "###PATH###";
        private const string targetPlaceHolder = "###TARGET###";
        private const string dataTypePlaceHolder = "###DATATYPE###";
        private const string textPlaceHolder = "###TEXT###";
        private const string multiplicityPlaceHolder = "###MULTIPLICITY###";

        private const string baseDocumentSnippet = "BaseDocument";
        private const string boCollectionSnippet = "BusinessObjectCollection";
        private const string boCollectionItemSnippet = "BusinessObjectCollectionItem";

        private const string actionCollectionSnippet = "ActionCollection";
        private const string actionCollectionItemSnippet = "ActionCollectionItem";
        private const string annotationCollectionSnippet = "AnnotationCollection";
        private const string annotationCollectionItemSnippet = "AnnotationCollectionItem";
        private const string associationCollectionSnippet = "AssociationCollection";
        private const string associationCollectionItemSnippet = "AssociationCollectionItem";
        private const string dataTypeCollectionSnippet = "DataTypeCollection";
        private const string dataTypeCollectionItemSnippet = "DataTypeCollectionItem";
        private const string defaultValueSnippet = "DefaultValue";
        private const string documentationCollectionSnippet = "DocumentationCollection";
        private const string documentationCollectionItemSnippet = "DocumentationCollectionItem";
        private const string elementCollectionSnippet = "ElementCollection";
        private const string elementCollectionItemSnippet = "ElementCollectionItem";
        private const string messageCollectionSnippet = "MessageCollection";
        private const string messageCollectionItemSnippet = "MessageCollectionItem";
        private const string nodeCollectionSnippet = "NodeCollection";
        private const string nodeCollectionItemSnippet = "NodeCollectionItem";
        private const string valuationSnippet = "Valuation";

        private Configuration configuration;

        public HTMLDocuGenerator(Configuration configuration) {
            this.configuration = configuration;
        }

        public void GenerateDocumenation(Solution solution) {
            if (configuration.Verbose) {
                Console.WriteLine("Generate HTML Documentation");
                Console.WriteLine(string.Format("Use theme {0}", CurrentTheme));
            }

            var businessObjects = new Dictionary<string, string>();
            // Generate BO files
            foreach (var bo in solution.BusinessObjects) {
                var htmlDoc = GetBaseHtmlDocument(string.Format("Business Object: {0}", bo.Name));

                htmlDoc.Replace(contentPlaceHolder, GenerateNodeContent(bo).ToString());

                // Save BO file
                var filename = System.IO.Path.Combine(configuration.OutputDir, string.Format("bo_{0}.html", bo.Name));
                businessObjects.Add(bo.Name, filename);
                if (System.IO.File.Exists(filename)) {
                    System.IO.File.Delete(filename);
                }
                if (configuration.Verbose) {
                    Console.WriteLine(string.Format("Write BO file: {0}", filename));
                }

                WriteFile(filename, htmlDoc);
            }

            var indexDoc = GetBaseHtmlDocument(string.Format("Solution: {0}", solution.Name));

            var indexDocContent = new StringBuilder();
            // Insert BO information
            if (businessObjects.Keys.Count > 0) {
                var boCollection = GetHTMLSnippet(boCollectionSnippet);

                var boCollectionContent = new StringBuilder();
                foreach (var bo in businessObjects.Keys.OrderBy(b => b)) {
                    string hrefValue;
                    if (businessObjects.TryGetValue(bo, out hrefValue) == false) {
                        continue;
                    }

                    var boElement = GetHTMLSnippet(boCollectionItemSnippet);
                    boElement = boElement.Replace(titlePlaceHolder, bo);
                    boElement = boElement.Replace(pathPlaceHolder, hrefValue);

                    boCollectionContent.Append(boElement);
                }

                indexDocContent.Append(boCollection.Replace(contentPlaceHolder, boCollectionContent.ToString()).ToString());
            }

            indexDoc.Replace(contentPlaceHolder, indexDocContent.ToString());

            var indexFilename = System.IO.Path.Combine(configuration.OutputDir, "index.html");
            if (configuration.Verbose) {
                Console.WriteLine(string.Format("Write Solution file: {0}", indexFilename));
            }
            WriteFile(indexFilename, indexDoc);

            // Copy default stylesheets
            CopyStyleFiles();
        }

        private StringBuilder GenerateNodeContent(Node node) {
            var nodeContent = new StringBuilder();

            if (node.DocumentationLines.Count > 0) {
                nodeContent.AppendLine(GenerateDocumentationPart(node.DocumentationLines));
            }

            if (node.Annotation.Count > 0) {
                nodeContent.AppendLine(GenerateAnnotationPart(node.Annotation));
            }

            if (node.Element.Count > 0) {
                nodeContent.AppendLine(GenerateElemementPart(node.Element));
            }

            if (node.Message.Count > 0) {
                nodeContent.AppendLine(GenerateMessagePart(node.Message));
            }

            if (node.Association.Count > 0) {
                nodeContent.AppendLine(GenerateAssociationPart(node.Association));
            }

            if (node.Action.Count > 0) {
                nodeContent.AppendLine(GenerateActionPart(node.Action));
            }

            if (node.ChildNode.Count > 0) {
                nodeContent.AppendLine(GenerateNodePart(node.ChildNode));
            }

            return nodeContent;
        }

        private string GenerateDocumentationPart(LinkedList<string> documentationLines) {
            var documentationCollection = GetHTMLSnippet(documentationCollectionSnippet);

            var documentationContent = new StringBuilder();
            foreach (var d in documentationLines) {
                var docItem = GetHTMLSnippet(documentationCollectionItemSnippet);

                documentationContent.AppendLine(docItem.Replace(contentPlaceHolder, d));
            }

            return documentationCollection.Replace(contentPlaceHolder, documentationContent.ToString());
        }

        private string GenerateAnnotationPart(LinkedList<Annotation> annotations) {
            var annotationCollection = GetHTMLSnippet(annotationCollectionSnippet);

            var annotationContent = new StringBuilder();
            foreach (var a in annotations) {
                annotationContent.AppendLine(GetHTMLSnippet(annotationCollectionItemSnippet).Replace(contentPlaceHolder, a.Name));
            }

            return annotationCollection.Replace(contentPlaceHolder, annotationContent.ToString());
        }

        private string GenerateElemementPart(LinkedList<Element> elements) {
            var elementCollection = GetHTMLSnippet(elementCollectionSnippet);

            var elemetCollectionContent = new StringBuilder();
            foreach (var e in elements) {
                var element = GetHTMLSnippet(elementCollectionItemSnippet).Replace(titlePlaceHolder, e.Name).Replace(dataTypePlaceHolder, e.DataType);
                var elementContent = new StringBuilder();

                if (e.DefaultValue != null && e.DefaultValue != string.Empty) {
                    var defaultValue = GetHTMLSnippet(defaultValueSnippet).Replace(textPlaceHolder, e.DefaultValue);
                    elementContent.AppendLine(defaultValue);
                }

                if (e.DocumentationLines.Count > 0) {
                    elementContent.AppendLine(GenerateDocumentationPart(e.DocumentationLines));
                }

                if (e.Annotation.Count > 0) {
                    elementContent.AppendLine(GenerateAnnotationPart(e.Annotation));
                }

                elemetCollectionContent.AppendLine(element.Replace(contentPlaceHolder, elementContent.ToString()));
            }

            return elementCollection.Replace(contentPlaceHolder, elemetCollectionContent.ToString());
        }

        private string GenerateMessagePart(LinkedList<Message> messages) {
            var messageCollection = GetHTMLSnippet(messageCollectionSnippet);

            var messageCollectionContent = new StringBuilder();
            foreach (var m in messages) {
                var message = GetHTMLSnippet(messageCollectionItemSnippet).Replace(titlePlaceHolder, m.Name).Replace(textPlaceHolder, m.Text);
                var messageContent = new StringBuilder();

                if (m.PlaceHolderDataTypes.Count > 0) {
                    var dataTypesCollection = GetHTMLSnippet(dataTypeCollectionSnippet);

                    var dataTypesCollectionContent = new StringBuilder();
                    foreach (var dt in m.PlaceHolderDataTypes) {
                        dataTypesCollectionContent.AppendLine(GetHTMLSnippet(dataTypeCollectionItemSnippet).Replace(dataTypePlaceHolder, dt));
                    }

                    messageContent.AppendLine(dataTypesCollection.Replace(contentPlaceHolder, dataTypesCollectionContent.ToString()));
                }

                if (m.DocumentationLines.Count > 0) {
                    messageContent.AppendLine(GenerateDocumentationPart(m.DocumentationLines));
                }

                messageCollectionContent.AppendLine(message.Replace(contentPlaceHolder, messageContent.ToString()));
            }

            return messageCollection.Replace(contentPlaceHolder, messageCollectionContent.ToString());
        }

        private string GenerateAssociationPart(LinkedList<Association> associations) {
            var associationCollection = GetHTMLSnippet(associationCollectionSnippet);
            var associationCollectionContent = new StringBuilder();

            foreach (var a in associations) {
                var association = GetHTMLSnippet(associationCollectionItemSnippet).Replace(titlePlaceHolder, a.Name).Replace(targetPlaceHolder, a.Target).Replace(multiplicityPlaceHolder, GetMultiplicityText(a.Multiplicity));

                var associationContent = new StringBuilder();
                if (a.ContainsValuation) {
                    associationContent.AppendLine(GetHTMLSnippet(valuationSnippet).Replace(contentPlaceHolder, a.Valuation));
                }
                if (a.DocumentationLines.Count > 0) {
                    associationContent.AppendLine(GenerateDocumentationPart(a.DocumentationLines));
                }

                if (a.Annotation.Count > 0) {
                    associationContent.AppendLine(GenerateAnnotationPart(a.Annotation));
                }

                associationCollectionContent.AppendLine(association.Replace(contentPlaceHolder, associationContent.ToString()));
            }

            return associationCollection.Replace(contentPlaceHolder, associationCollectionContent.ToString());
        }

        private string GenerateActionPart(LinkedList<SolutionDocumentationGenerator.Model.Action> action) {
            var actionCollection = GetHTMLSnippet(actionCollectionSnippet);

            var actionCollectionContent = new StringBuilder();
            foreach (var a in action) {
                var actionElement = GetHTMLSnippet(actionCollectionItemSnippet).Replace(titlePlaceHolder, a.Name);
                var docu = string.Empty;
                if (a.DocumentationLines.Count > 0) {
                    docu = GenerateDocumentationPart(a.DocumentationLines);
                }

                actionCollectionContent.AppendLine(actionElement.Replace(contentPlaceHolder, docu));
            }

            return actionCollection.Replace(contentPlaceHolder, actionCollectionContent.ToString());
        }

        private string GenerateNodePart(LinkedList<Node> nodes) {
            var nodeCollection = GetHTMLSnippet(nodeCollectionSnippet);

            var nodeCollectionContent = new StringBuilder();
            foreach (var n in nodes) {
                var node = GetHTMLSnippet(nodeCollectionItemSnippet).Replace(titlePlaceHolder, n.Name).Replace(multiplicityPlaceHolder, GetMultiplicityText(n.Multiplicity));

                var nodeContent = new StringBuilder();
                nodeContent.Append(GenerateNodeContent(n));

                nodeCollectionContent.AppendLine(node.Replace(contentPlaceHolder, nodeContent.ToString()));
            }

            return nodeCollection.Replace(contentPlaceHolder, nodeCollectionContent.ToString());
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

        private StringBuilder GetBaseHtmlDocument(string title) {
            var str = new StringBuilder(GetHTMLSnippet(baseDocumentSnippet));
            return str.Replace(titlePlaceHolder, title);
        }

        private string currentTheme = null;
        private string CurrentTheme {
            get {
                if (currentTheme == null) {

                    if (configuration.Theme != null && configuration.Theme.Trim().Length > 0) {
                        var themeName = configuration.Theme.Trim();
                        var themePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Themes", themeName);
                        if (Directory.Exists(themePath)) {
                            currentTheme = themeName;
                        } else {
                            Console.WriteLine(string.Format("Theme {0} not found switching to Theme \"default\"", themeName));
                            currentTheme = "default";
                        }
                    } else {
                        if (configuration.Verbose) {
                            Console.WriteLine("No Theme set. Using Theme default.");
                        }
                        currentTheme = "default";
                    }
                }
                return currentTheme;
            }
        }

        private string GetHTMLSnippet(string snippetName) {
            var fileName = string.Format("{0}.html", snippetName);
            var snippetPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Themes", CurrentTheme, fileName);
            if (File.Exists(snippetPath)) {
                using (var reader = new StreamReader(snippetPath)) {
                    return reader.ReadToEnd();
                }
            } else {
                throw new FileNotFoundException(string.Format("Snippet File {0} in Theme {1} not found", snippetName, CurrentTheme));
            }
        }

        private void CopyStyleFiles() {
            var stylePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Themes", CurrentTheme, "style");
            var styleTargetPath = System.IO.Path.Combine(configuration.OutputDir, "style");
            CopyDirectory(stylePath, styleTargetPath);
        }

        private void CopyDirectory(string sourceDirPath, string targetDirPath) {
            if (Directory.Exists(targetDirPath)) {
                Directory.Delete(targetDirPath, true);
            }

            Directory.CreateDirectory(targetDirPath);

            var sourceDirInfo = new DirectoryInfo(sourceDirPath);
            foreach (DirectoryInfo d in sourceDirInfo.GetDirectories()) {
                string temppath = Path.Combine(targetDirPath, d.Name);
                CopyDirectory(d.FullName, temppath);
            }

            foreach (var f in sourceDirInfo.GetFiles()) {
                f.CopyTo(Path.Combine(targetDirPath, f.Name));
            }
        }

        private void WriteFile(string path, StringBuilder content) {
            if (File.Exists(path)) {
                File.Delete(path);
            }

            using (var writer = new StreamWriter(path)) {
                writer.Write(content.ToString());
            }
        }
    }
}
