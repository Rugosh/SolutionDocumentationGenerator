using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ByDSolutionDocumentationGenerator.Model;
using ByDSolutionDocumentationGenerator.Parser;

namespace ByDSolutionDocumentationGenerator.Test.Parser.Test {
    [TestClass]
    public class BusinessObjectParserTest {

        private Configuration TestConfiguration;

        public BusinessObjectParserTest() {
            TestConfiguration = new Configuration();
            TestConfiguration.Verbose = true;
        }

        [TestMethod]
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


        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
    }
}
