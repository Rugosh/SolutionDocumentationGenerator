﻿using System;
using SolutionDocumentationGenerator.Model;
using SolutionDocumentationGenerator.Parser;
using NUnit.Framework;

namespace SolutionDocumentationGenerator.Test.Parser.Test {
    [TestFixture]
    public class BusinessObjectParserTest {

        private Configuration TestConfiguration;

        public BusinessObjectParserTest() {
            TestConfiguration = new Configuration();
            TestConfiguration.Verbose = true;
        }

        [Test]
        public void SimpleBusinessObjectTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

businessobject TestBO {

		element TestBO_ID :ID;
}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);
            Assert.AreEqual(0, e.Annotation.Count);
        }


        [Test]
        public void SimpleUgliefiedBusinessObjectTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;businessobject TestBO{element TestBO_ID:ID;}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);
            Assert.AreEqual(0, e.Annotation.Count);
        }

        [Test]
        public void SimpleBusinessObjectWithAssociationTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

businessobject TestBO {

		element TestBO_ID :ID;

        association ToCustomer [0,1] to Customer;
}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);
            Assert.AreEqual(0, e.Annotation.Count);

            // Check Assoziation
            Assert.AreEqual(1, parsedBo.Association.Count);
            var a = parsedBo.Association.First.Value;
            Assert.AreEqual("ToCustomer", a.Name);
            Assert.AreEqual(Multiplicity.ZeroToOne, a.Multiplicity);
            Assert.AreEqual("Customer", a.Target);
        }

        [Test]
        public void SimpleBusinessObjectWithAssociationAndValuationTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

businessobject TestBO {

		element TestBO_ID :ID;

        element ParentID :ID;

        association ToChild [0,n] to TestBO valuation (ParentID == TestBO_ID);
}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(2, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);
            Assert.AreEqual(0, e.Annotation.Count);

            // Check Assoziation
            Assert.AreEqual(1, parsedBo.Association.Count);
            var a = parsedBo.Association.First.Value;
            Assert.AreEqual("ToChild", a.Name);
            Assert.AreEqual(Multiplicity.ZeroToN, a.Multiplicity);
            Assert.AreEqual(true, a.ContainsValuation);
            Assert.AreEqual("ParentID == TestBO_ID", a.Valuation);
            Assert.AreEqual("TestBO", a.Target);
        }

        [Test]
        public void SimpleBusinessObjectWithAnnotaionTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

businessobject TestBO {

        [AlternativeKey]
		element TestBO_ID :ID;

        association ToCustomer [0,1] to Customer;
}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);
            Assert.AreEqual(1, e.Annotation.Count);
            Assert.AreEqual("AlternativeKey", e.Annotation.First.Value.Name);
        }

        [Test]
        public void SimpleBusinessObjectWithMessageTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

businessobject TestBO {

        message TEST_MESSAGE_ABC text ""Some message text"";

		element TestBO_ID :ID;
}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);

            // Check Message
            Assert.AreEqual(1, parsedBo.Message.Count);
            var m = parsedBo.Message.First.Value;
            Assert.AreEqual("TEST_MESSAGE_ABC", m.Name);
            Assert.AreEqual("Some message text", m.Text);
            Assert.AreEqual(0, m.PlaceHolderDataTypes.Count);
        }

        [Test]
        public void SimpleBusinessObjectWithMessageAndParametersTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

businessobject TestBO {

        message TEST_MESSAGE_ABC text ""Some message with Para1: &  and para 2 : & and #3 &"": LANGUAGEINDEPENDENT_EXTENDED_Text, LANGUAGEINDEPENDENT_EXTENDED_Text, LANGUAGEINDEPENDENT_EXTENDED_Text;

		element TestBO_ID :ID;
}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);

            // Check Message
            Assert.AreEqual(1, parsedBo.Message.Count);
            var m = parsedBo.Message.First.Value;
            Assert.AreEqual("TEST_MESSAGE_ABC", m.Name);
            Assert.AreEqual("Some message with Para1: &  and para 2 : & and #3 &", m.Text);
            Assert.AreEqual(3, m.PlaceHolderDataTypes.Count);
            Assert.AreEqual("LANGUAGEINDEPENDENT_EXTENDED_Text", m.PlaceHolderDataTypes.First.Value);
        }

        [Test]
        public void SimpleBusinessObjectWithNodeTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

businessobject TestBO {

		element TestBO_ID :ID;

	    node SomeNode [0,n]{
	
		    [Transient]
		    [Label (""element1"")]
		    element element1 : Percent;

		    [Label (""element2"")]
		    element element2 :ID;

		    association ToSomething to Something;
	    }
}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);
            Assert.AreEqual(0, e.Annotation.Count);

            // Check ChildNode
            Assert.AreEqual(1, parsedBo.ChildNode.Count);
            Assert.AreEqual(Multiplicity.ZeroToN, parsedBo.ChildNode.First.Value.Multiplicity);

            // Check Child Node Content
            Assert.AreEqual(2, parsedBo.ChildNode.First.Value.Element.Count);
            Assert.AreEqual(1, parsedBo.ChildNode.First.Value.Association.Count);
        }

        [Test]
        public void SimpleBusinessObjectWithNodesTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

businessobject TestBO {

		element TestBO_ID :ID;

	    node SomeNode [0,n]{

		    [Label (""element1"")]
		    element element1 :ID;
	    }

	    node SomeOtherNode [0,n]{

		    [Label (""element1"")]
		    element element1 :ID;
	    }
}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);
            Assert.AreEqual(0, e.Annotation.Count);

            // Check ChildNodes
            Assert.AreEqual(2, parsedBo.ChildNode.Count);
        }

        [Test]
        public void SimpleBusinessObjectWithNodeHierarchieTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

businessobject TestBO {

		element TestBO_ID :ID;

	    node SomeNode [0,n]{

		    [Label (""element1"")]
		    element element1 :ID;

	        node SomeOtherNode [0,n]{

		        [Label (""element1"")]
		        element element1 :ID;

                node AgainSomeOtherNode [0,n]{

		            [Label (""element1"")]
		            element element1 :ID;
	            }
	        }
	    }


}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);
            Assert.AreEqual(0, e.Annotation.Count);

            // Check ChildNodes
            Assert.AreEqual(1, parsedBo.ChildNode.Count);
            Assert.AreEqual(1, parsedBo.ChildNode.First.Value.ChildNode.Count);
            Assert.AreEqual(1, parsedBo.ChildNode.First.Value.ChildNode.First.Value.ChildNode.Count);
        }

        [Test]
        public void SimpleBusinessObjectWithSpecialNodesTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

businessobject TestBO {

		element TestBO_ID :ID;

	    [DependentObject(TextCollection)]
	    [MultipleTexts]
	    node TextCollection;

	    [DependentObject(AttachmentFolder)]
	    node AttachementFolder;
}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);
            Assert.AreEqual(0, e.Annotation.Count);

            // Check ChildNodes
            Assert.AreEqual(2, parsedBo.ChildNode.Count);
        }

        [Test]
        public void BusinessObjectWithDocumentationTest() {
            var boText = @"import AP.Common.GDT as apCommonGDT;

/// Some BO Docu Text
businessobject TestBO {
        
        /// Some TestBO_ID Element Docu Test
		element TestBO_ID :ID;

        /// Docu Line 1 for SomeNode
        /// Docu Line 2 for SomeNode
	    node SomeNode [0,n]{

            /// Docu Line 1 for element1
            /// Docu Line 2 for element1
            /// Docu Line 3 for element1
		    [Label (""element1"")]
		    element element1 :ID;
	    }


}";

            var parser = new BusinessObjectParser(TestConfiguration);
            var parsedBo = parser.ParseBusinessObject(boText);

            // Check BO Root Node
            Assert.AreEqual("TestBO", parsedBo.Name);
            Assert.AreEqual(NodeType.BusinessObject, parsedBo.NodeType);
            Assert.AreEqual(1, parsedBo.DocumentationLines.Count);

            // Check Element
            Assert.AreEqual(1, parsedBo.Element.Count);
            var e = parsedBo.Element.First.Value;
            Assert.AreEqual("TestBO_ID", e.Name);
            Assert.AreEqual("ID", e.DataType);
            Assert.AreEqual(0, e.Annotation.Count);
            Assert.AreEqual(1, e.DocumentationLines.Count);

            // Check ChildNode
            Assert.AreEqual(1, parsedBo.ChildNode.Count);
            Assert.AreEqual(2, parsedBo.ChildNode.First.Value.DocumentationLines.Count);
            Assert.AreEqual(3, parsedBo.ChildNode.First.Value.Element.First.Value.DocumentationLines.Count);
        }
    }
}
