using HDF5CSharpWrapper.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Linq;

namespace HDF5CSharpWrapper.Tests
{
    [TestClass]
    public class DatasetsTests
    {
        private static string DirectoryName { get; set; } = "../../../TestFiles/";

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            File file = new File();
            Groups groups = new Groups();
            Datasets datasets = new Datasets();
            int[] Array = { 1 };

            var openedFileId = file.Create(DirectoryName + "datasetsSetsTest.h5");
            var datasetsId = datasets.SetDataset<int>(openedFileId, "datasetname", Array);
            file.Close(openedFileId);
        }

        #region GetDataset

        [TestMethod]
        public void GetDataset_TypeInt()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsGetsTest.h5");
            var datasetResult = datasets.GetDataset<Int32>(openedFileId, "/int");

            file.Close(openedFileId);

            int[] datasetResultArray = (int[])datasetResult;
            int[] expectedValue = { 255 };

            Assert.IsTrue(datasetResultArray.SequenceEqual(expectedValue));
        }

        [TestMethod]
        public void GetDataset_TypeIntTwoDim()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsGetsTest.h5");
            var datasetResult = datasets.GetDataset<Int32>(openedFileId, "/Arrays/int");

            file.Close(openedFileId);

            int[,] datasetResultArray = (int[,])datasetResult;
            int[,] expectedValue = { { 1, 2 },
                                     { 3, 4 } };

            Assert.IsTrue(SequenceEqualsExtension.SequenceEquals(expectedValue, datasetResultArray));
        }

        [TestMethod]
        public void GetDataset_TypeFloat()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsGetsTest.h5");
            var datasetResult = datasets.GetDataset<Double>(openedFileId, "/float");

            file.Close(openedFileId);

            double[] datasetResultArray = (double[])datasetResult;
            double[] expectedValue = { 25.0 };

            Assert.IsTrue(datasetResultArray.SequenceEqual(expectedValue));
        }

        [TestMethod]
        public void GetDataset_TypeFloatTwoDim()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsGetsTest.h5");
            var datasetResult = datasets.GetDataset<Double>(openedFileId, "/Arrays/float");

            file.Close(openedFileId);

            double[,] datasetResultArray = (double[,])datasetResult;

            double[,] expectedValue = { { 1.1000000238418579, 2.2000000476837158 },
                                        { 3.2999999523162842, 4.4000000953674316} };

            //double[,] expectedValue = { { 1.1, 2.2 },
            //                         { 3.3, 4.4} };

            Assert.IsTrue(SequenceEqualsExtension.SequenceEquals(expectedValue, datasetResultArray));
        }

        [TestMethod]
        public void GetDataset_TypeBoolean()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsGetsTest.h5");
            var datasetResult = datasets.GetDataset<Byte>(openedFileId, "/bool");

            file.Close(openedFileId);

            byte[] datasetResultArray = (byte[])datasetResult;
            byte[] expectedValue = { 1 };

            Assert.IsTrue(datasetResultArray.SequenceEqual(expectedValue));
        }

        [TestMethod]
        public void GetDataset_TypeCharTwoDim()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsGetsTest.h5");
            var datasetResult = datasets.GetDataset<string>(openedFileId, "/Arrays/char");

            file.Close(openedFileId);

            byte[,] datasetResultArray = (byte[,])datasetResult;

            byte[,] expectedValue = { { 32, 33 },
                                      { 34, 35 } };

            Assert.IsTrue(SequenceEqualsExtension.SequenceEquals(expectedValue, datasetResultArray));
        }

        [TestMethod]
        public void GetDataset_TypeImageThreeDim()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsGetsTest.h5");
            var datasetResult = datasets.GetDataset<SByte>(openedFileId, "/color_image");

            file.Close(openedFileId);

            sbyte[,,] datasetResultArray = (sbyte[,,])datasetResult;

            sbyte[,,] expectedValue = new sbyte[2048, 2560, 3];

            for (int x = 0; x < expectedValue.GetLength(0); x++)
                for (int y = 0; y < expectedValue.GetLength(1); y++)
                    for (int z = 0; z < expectedValue.GetLength(2); z++)
                        expectedValue[x, y, z] = 0;

            Assert.IsTrue(SequenceEqualsExtension.SequenceEqualsThreeDim(expectedValue, datasetResultArray));
        }

        [TestMethod]
        public void GetDataset_TypeString()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsGetsTest.h5");
            var datasetResult = datasets.GetDataset<string>(openedFileId, "/vlen_string");

            file.Close(openedFileId);

            string[] datasetResultArray = (string[])datasetResult;
            string[] expectedValue = { "tu jest napis śćąęłóżźń" };

            Assert.IsTrue(datasetResultArray.SequenceEqual(expectedValue));
        }

        [TestMethod]
        public void GetDataset_TypeStringTwoDim()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsGetsTest.h5");
            var datasetResult = datasets.GetDataset<string>(openedFileId, "/Arrays/vlen_string");

            file.Close(openedFileId);

            string[,] datasetResultArray = (string[,])datasetResult;

            string[,] expectedValue = { { "a", "bbbbbb" },
                                      { "cc", "ddd" } };

            Assert.IsTrue(SequenceEqualsExtension.SequenceEquals(expectedValue, datasetResultArray));
        }

        #endregion

        #region SetDataset

        [TestMethod]
        public void SetDataset_TypeInt()
        {
            File file = new File();
            Datasets datasets = new Datasets();
            int[] intArray = { 1, 2, 3, 4, 5 };

            var openedFileId = file.Open(DirectoryName + "datasetsSetsTest.h5");
            var dataset = datasets.SetDataset<int>(openedFileId, "int", intArray);

            file.Close(openedFileId);

            Assert.IsTrue(dataset != -1);
        }
        [TestMethod]

        public void SetDataset_TypeIntTwoDim()
        {
            File file = new File();
            Datasets datasets = new Datasets();
            int[,] intTwoDimArray = { { 1, 2 },
                                      { 3, 4 } };

            var openedFileId = file.Open(DirectoryName + "datasetsSetsTest.h5");
            var dataset = datasets.SetDataset<int>(openedFileId, "intTwoDim", intTwoDimArray);

            file.Close(openedFileId);

            Assert.IsTrue(dataset != -1);
        }
        [TestMethod]

        public void SetDataset_TypeFloat()
        {
            File file = new File();
            Datasets datasets = new Datasets();
            double[,] doubleTwoDimArray = { { 1.1, 2.2 },
                                            { 3.3, 4.4} };

            var openedFileId = file.Open(DirectoryName + "datasetsSetsTest.h5");
            var dataset = datasets.SetDataset<double>(openedFileId, "float", doubleTwoDimArray);

            file.Close(openedFileId);

            Assert.IsTrue(dataset != -1);
        }
        [TestMethod]

        public void SetDataset_TypeFloatTwoDim()
        {
            File file = new File();
            Datasets datasets = new Datasets();
            double[] doubleArray = { 25.0 };

            var openedFileId = file.Open(DirectoryName + "datasetsSetsTest.h5");
            var dataset = datasets.SetDataset<double>(openedFileId, "floatTwoDim", doubleArray);

            file.Close(openedFileId);

            Assert.IsTrue(dataset != -1);
        }
        [TestMethod]

        public void SetDataset_TypeBoolean()
        {
            File file = new File();
            Datasets datasets = new Datasets();
            byte[] byteArray = { 1, 0, 1 };

            var openedFileId = file.Open(DirectoryName + "datasetsSetsTest.h5");
            var dataset = datasets.SetDataset<byte>(openedFileId, "byte", byteArray);

            file.Close(openedFileId);

            Assert.IsTrue(dataset != -1);
        }
        [TestMethod]

        public void SetDataset_TypeBooleanTwoDim()
        {
            File file = new File();
            Datasets datasets = new Datasets();
            byte[,] byteTwoDimArray = { { 1, 0 },
                                        { 1, 1 } };

            var openedFileId = file.Open(DirectoryName + "datasetsSetsTest.h5");
            var dataset = datasets.SetDataset<byte>(openedFileId, "byteTwoDim", byteTwoDimArray);

            file.Close(openedFileId);

            Assert.IsTrue(dataset != -1);
        }

        [TestMethod]
        public void SetDataset_TypeChar()
        {
            File file = new File();
            Datasets datasets = new Datasets();
            byte[,] charArray = { { 32, 33 },
                                  { 34, 35 } };

            var openedFileId = file.Open(DirectoryName + "datasetsSetsTest.h5");
            var dataset = datasets.SetDataset<byte>(openedFileId, "charTwoDim", charArray);

            file.Close(openedFileId);

            Assert.IsTrue(dataset != -1);
        }

        [TestMethod]
        public void SetDataset_TypeColorImage()
        {
            File file = new File();
            Datasets datasets = new Datasets();
            sbyte[,,] imageColorArray = { { { 1, 2, 3}, {4, 5, 6} },
                                          { { 7, 8, 9}, {10, 11, 12} } };

            var openedFileId = file.Open(DirectoryName + "datasetsSetsTest.h5");
            var dataset = datasets.SetDataset<sbyte>(openedFileId, "imageColor", imageColorArray);

            file.Close(openedFileId);

            Assert.IsTrue(dataset != -1);
        }

        #endregion

        #region RemoveDataset

        [TestMethod]
        public void RemoveDataset()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsSetsTest.h5");
            var dataset = datasets.RemoveDataset(openedFileId, "datasetname");

            file.Close(openedFileId);

            Assert.IsTrue(dataset != -1);
        }

        [TestMethod]
        public void RemoveNotExistingDataset()
        {
            File file = new File();
            Datasets datasets = new Datasets();

            var openedFileId = file.Open(DirectoryName + "datasetsSetsTest.h5");
            var dataset = datasets.RemoveDataset(openedFileId, "datasetnameee");

            file.Close(openedFileId);

            Assert.IsTrue(dataset == -1);
        }

        #endregion
    }
}
