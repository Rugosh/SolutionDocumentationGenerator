using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionDocumentationGenerator.Parser;

namespace SolutionDocumentationGenerator.Test.Parser.Test {
    [TestClass]
    public class SolutionFileParserTest {
        private Configuration TestConfiguration;

        public SolutionFileParserTest(){
            TestConfiguration = new Configuration();
            TestConfiguration.Verbose = true;
        }

        [TestMethod]
        public void BasicSolutionFileParseTest() {
            var parser = new SolutionFileParser(TestConfiguration);
            var testSolutionContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>{19ed7839-87cb-407a-9e4c-c860fcb79702}</ProjectGuid>
    <ProjectType>CopernicusProject</ProjectType>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Name>YEKRNL1PY</Name>
    <RootNamespace>YEKRNL1PY</RootNamespace>
    <RepositoryNamespace>http://0012345678-one-off.sap.com/YEKRNL1PY_</RepositoryNamespace>
    <RuntimeNamespacePrefix>YEKRNL1PY_</RuntimeNamespacePrefix>
    <RepositoryRootFolder>/YEKRNL1PY_MAIN</RepositoryRootFolder>
    <DefaultProcessComponent>YEKRNL1PY_YEKRNL1PY</DefaultProcessComponent>
    <DevelopmentPackage>$YEKRNL1PY_DEV</DevelopmentPackage>
    <XRepSolution>YEKRNL1PY_MAIN</XRepSolution>
    <BCSourceFolderInXRep>/YEKRNL1PY_BC/SRC</BCSourceFolderInXRep>
    <ProjectSourceFolderinXRep>/YEKRNL1PY_MAIN/SRC</ProjectSourceFolderinXRep>
    <DeploymentUnit>CUSTOMER_RELATIONSHIP_MGMT</DeploymentUnit>
    <CompilerVersion>1302_FP15</CompilerVersion>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)' == 'Debug' "">
    <OutputPath>bin</OutputPath>
  </PropertyGroup>
  <ItemGroup>
    <Content Include=""SomeBO.bo"">
      <SubType>Content</SubType>
    </Content>
    <Content Include=""OtherBO.bo"">
      <SubType>Content</SubType>
    </Content>
  </ItemGroup>
  <Import Project=""$(MSBuildBinPath)\Microsoft.CSharp.targets"" />
</Project>";

            var parsedSolution = parser.ParseSolutionFile(testSolutionContent);
            Assert.AreEqual("YEKRNL1PY", parsedSolution.Name);
            Assert.AreEqual(2, parsedSolution.BusinessObjectFiles.Count);
        }
    }
}
