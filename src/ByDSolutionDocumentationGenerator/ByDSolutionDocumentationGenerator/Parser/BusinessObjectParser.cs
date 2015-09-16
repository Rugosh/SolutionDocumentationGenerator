using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByDSolutionDocumentationGenerator.Model;

namespace ByDSolutionDocumentationGenerator.Parser {
    public class BusinessObjectParser {

        private char[] SPACER;
        private Configuration configuration;

        public BusinessObjectParser(Configuration configuration) {
            this.configuration = configuration;
            SPACER = new char[1] { ' ' };
        }

        private string preProcessText(string boText) {
            if (configuration.Verbose) {
                Console.WriteLine("BO text before preprocessing:");
                Console.WriteLine(boText);
            }

            boText = boText.Replace("\n", "");
            boText = boText.Replace("\r", "");
            boText = boText.Replace("{", "{\n");
            boText = boText.Replace("}", "}\n");
            boText = boText.Replace(";", ";\n");

            var newBoText = new StringBuilder();
            foreach (var line in boText.Split('\n')) {
                newBoText.AppendLine(line.Trim());
            }

            if (configuration.Verbose) {
                Console.WriteLine("\nBO text after preprocessing:");
                Console.WriteLine(newBoText.ToString());
            }

            return newBoText.ToString();
        }

        private string CleanLineEnding(string line) {
            return line.TrimEnd(new char[] { ';', '{' });
        }

        public Node ParseBusinessObject(string boText) {
            boText = preProcessText(boText);

            var nodeHeap = new LinkedList<Node>();
            var boNode = new Node();
            boNode.NodeType = NodeType.BusinessObject;
            nodeHeap.AddLast(boNode);

            var currenctAnnotations = new LinkedList<Annotation>();
            foreach (var line in boText.Split('\n')) {
                var parseLine = line.Trim();
                // Skip emty lines
                if (parseLine.Length == 0) {
                    continue;
                }

                // Line Handling
                while (parseLine.StartsWith("[")) {
                    var splitLine = parseLine.Split(new char[1] { ']' }, 2);

                    currenctAnnotations = ParseAnnotations(splitLine.First() + "]");

                    parseLine = splitLine.Last().Trim();
                }

                if (parseLine.StartsWith("businessobject")) {
                    nodeHeap.Last.Value.Name = CleanLineEnding(parseLine.Split(SPACER)[1]);
                    nodeHeap.Last.Value.Annotation = currenctAnnotations;

                } else if (parseLine.StartsWith("element")) {
                    nodeHeap.Last.Value.Element.AddLast(ParseElement(parseLine, currenctAnnotations));

                } else if (parseLine.StartsWith("association")) {
                    nodeHeap.Last.Value.Association.AddLast(ParseAssociation(parseLine, currenctAnnotations));

                } else if (parseLine.StartsWith("message")) {
                    nodeHeap.Last.Value.Message.AddLast(ParseMessage(parseLine));

                } else if (parseLine.StartsWith("action")) {
                    nodeHeap.Last.Value.Action.AddLast(ParseAction(parseLine));

                } else if (parseLine.StartsWith("node")) {
                    var newNode = ParseNode(parseLine);
                    nodeHeap.Last.Value.ChildNode.AddLast(newNode);
                    nodeHeap.AddLast(newNode);

                } else if (parseLine.StartsWith("}")) {
                    // Node Close
                    if (nodeHeap.Count > 1) {
                        nodeHeap.RemoveLast();
                    }

                }

                if (currenctAnnotations.Count != 0) {
                    currenctAnnotations = new LinkedList<Annotation>();
                }
            }

            return boNode;
        }

        private LinkedList<Annotation> ParseAnnotations(string line) {
            var annotations = new LinkedList<Annotation>();

            annotations.AddLast(new Annotation(line.Split('[').Last().Split(']').First()));

            return annotations;
        }

        private Element ParseElement(string line, LinkedList<Annotation> annotation) {
            var newElement = new Element();

            var elementNameAndDataType = line.Split(SPACER, 2, StringSplitOptions.RemoveEmptyEntries)[1];

            var elementSplitValues = elementNameAndDataType.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            newElement.Name = elementSplitValues[0].Trim();
            newElement.DataType = CleanLineEnding(elementSplitValues[1]).Trim();

            newElement.Annotation = annotation;
            return newElement;
        }

        private Association ParseAssociation(string line, LinkedList<Annotation> annotation) {
            var newAssociation = new Association();

            line = line.Split(SPACER, 2, StringSplitOptions.RemoveEmptyEntries)[1];

            // handle Associaton
            var splittedLine = line.Split(new string[1] { " to " }, StringSplitOptions.RemoveEmptyEntries);
            newAssociation.Target = CleanLineEnding(splittedLine.Last());

            if (splittedLine.First().Contains('[')) {
                splittedLine = splittedLine.First().Split(new char[1] { '[' }, StringSplitOptions.RemoveEmptyEntries);
                var splittedMultiplicityLine = splittedLine.Last().Split(new char[1] { ']' }, StringSplitOptions.RemoveEmptyEntries);
                splittedMultiplicityLine = splittedMultiplicityLine.Last().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var multi1 = splittedMultiplicityLine.First();
                var multi2 = splittedMultiplicityLine.Last();

                newAssociation.Multiplicity = GetMultiplicity(multi1, multi2);
            } else {
                // Default Multiplicity
                newAssociation.Multiplicity = Multiplicity.ZeroToOne;
            }

            newAssociation.Name = splittedLine.First().Trim();

            newAssociation.Annotation = annotation;
            return newAssociation;
        }

        private Message ParseMessage(string line) {
            var message = new Message();

            line = line.Split(SPACER, 2, StringSplitOptions.RemoveEmptyEntries)[1];

            var splittedLine = line.Split(new string[1] { " text " }, 2, StringSplitOptions.RemoveEmptyEntries);

            message.Name = splittedLine.First();

            var messageText = CleanLineEnding(splittedLine.Last().Trim());

            var lastParamSpacer = messageText.LastIndexOf(',');
            var lastQuote = messageText.LastIndexOf('"');

            while (lastParamSpacer > lastQuote) {
                message.PlaceHolderDataTypes.AddLast(messageText.Substring(lastParamSpacer + 1).Trim());

                messageText = messageText.Substring(0, lastParamSpacer);

                lastParamSpacer = messageText.LastIndexOf(',');
                lastQuote = messageText.LastIndexOf('"');
            }

            lastParamSpacer = messageText.LastIndexOf(':');
            if (lastParamSpacer > lastQuote) {
                message.PlaceHolderDataTypes.AddLast(messageText.Substring(lastParamSpacer + 1).Trim());

                messageText = messageText.Substring(0, lastParamSpacer);
            }

            message.Text = messageText.Substring(1, messageText.Length - 2);

            return message;
        }

        private string ParseAction(string line) {
            var action = string.Empty;

            line = line.Split(SPACER, 2, StringSplitOptions.RemoveEmptyEntries)[1];
            action = CleanLineEnding(line);

            return action;
        }

        private Node ParseNode(string line) {
            var node = new Node();

            line = line.Split(SPACER, 2, StringSplitOptions.RemoveEmptyEntries)[1];
            line = CleanLineEnding(line);

            if (line.Contains("[")) {
                var splittedLine = line.Split(new string[1] { "[" }, 2, StringSplitOptions.RemoveEmptyEntries);
                node.Name = splittedLine.First();
                line = splittedLine.Last().Replace("[", "").Replace("]", "");
                splittedLine = line.Split(new string[1] { "," }, 2, StringSplitOptions.RemoveEmptyEntries);
                node.Multiplicity = GetMultiplicity(splittedLine.First(), splittedLine.Last());
            }

            return node;
        }

        private Multiplicity GetMultiplicity(string multi1, string multi2) {
            multi1 = multi1.Trim();
            multi2 = multi2.Trim();

            if (multi1 == "0" && multi2 == "1") {
                return Multiplicity.ZeroToOne;
            }

            if (multi1 == "0" && multi2 == "n") {
                return Multiplicity.ZeroToN;
            }

            if (multi1 == "1" && multi2 == "1") {
                return Multiplicity.OneToOne;
            }

            if (multi1 == "1" && multi2 == "n") {
                return Multiplicity.OneToN;
            }

            throw new Exception(string.Format("no matching Multiplicity found for the vars {0} and {1}", multi1, multi2));
        }
    }
}
