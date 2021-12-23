using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Data;
using System.Linq;

namespace HDF5CSharpWrapper.Tests
{
    [TestClass]
    public class AttributesTests
    {
        private static string DirectoryName { get; set; } = "../../../TestFiles/";
        static File file;
        static Groups groups;
        static Datasets datasets;
        static Attributes attributes;

        static long groupId;
        static long fileIdSets;
        static long fileIdGets;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            file = new File();
            groups = new Groups();
            datasets = new Datasets();
            attributes = new Attributes();

            fileIdSets = file.CreateFile(DirectoryName + "attirbutesSetsTest.h5");
            groupId = groups.CreateGroup(fileIdSets, "GroupName");
            datasets.SetDataset<int>(fileIdSets, "int", new int[] { 1,2,3,4 });

            fileIdGets = file.OpenFile(DirectoryName + "test.h5");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            file.CloseFile(fileIdSets);
            file.CloseFile(fileIdGets);
        }

        [TestMethod]
        public void GetAttribute_TypeInt()
        {
            var attributeResult = attributes.GetAttribute<int>(fileIdGets, "IMAGE_MINMAXRANGE", "color_image");
            int[] attributeResultArray = (int[])attributeResult;
            int[] expectedValue = { 0, 255 };

            Assert.IsTrue(attributeResultArray.SequenceEqual(expectedValue), "Attributes not gets correctly.");
        }

        [TestMethod]
        public void GetAttribute_TypeString()
        {
            var attributeResult = attributes.GetAttribute<string>(fileIdGets, "CLASS", "color_image");
            string[] attributeResultArray = (string[])attributeResult;
            string[] expectedValue = { "IMAGE" };
            
            Assert.IsTrue(attributeResultArray.SequenceEqual(expectedValue), "Attributes not gets correctly.");
        }

        [TestMethod]
        public void SetAttributeOnGroup_TypeString()
        {
            string[] stringArray = { "śćąęłóżźń", "asdf", "asdf" };
            var attributeResult = attributes.SetAttribute<string>(groupId, "StringAttributeName", stringArray);
            Assert.IsTrue(attributeResult != -1, "Attributes not sets correctly.");
        }

        [TestMethod]
        public void SetAttributeOnDataset_TypeInt()
        {
            int[] intArray = { 1, 2, 3, 4, 5 };
            var attributeResult = attributes.SetAttribute<int>(fileIdSets, "IntAttributeName", intArray, "int");

            Assert.IsTrue(attributeResult != -1, "Attributes not sets correctly.");
        }
    }
}
