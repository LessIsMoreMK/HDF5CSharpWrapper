using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HDF5CSharpWrapper.Tests
{
    [TestClass]
    public class DatasetsTests
    {
        private static string DirectoryName { get; set; } = "../../../TestFiles/";
        static File file;
        static Datasets datasets;
        static long fileIdSets;
        static long fileIdSets2;
        static long fileIdGets;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            file = new File();
            datasets = new Datasets();
            
            fileIdSets = file.CreateFile(DirectoryName + "datasetsSetsTest.h5");
            fileIdSets2 = file.CreateFile(DirectoryName + "datasetsSets2Test.h5");
            fileIdGets = file.OpenFile(DirectoryName + "datasetsGetsTest.h5");

            int[] Array = { 1 };
            datasets.SetDataset<int>(fileIdSets, "datasetname", Array);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            file.CloseFile(fileIdSets);
            file.CloseFile(fileIdSets2);
            file.CloseFile(fileIdGets);
        }

        #region GetDataset

        [TestMethod]
        public void GetDataset_TypeInt()
        {
            var datasetResult = datasets.GetDataset<Int32>(fileIdGets, "/int");
            int[] datasetResultArray = (int[])datasetResult;
            int[] expectedValue = { 255 };

            Assert.IsTrue(datasetResultArray.SequenceEqual(expectedValue), "Dataset not get correctly.");
        }

        [TestMethod]
        public void GetDataset_TypeIntTwoDim()
        {
            var datasetResult = datasets.GetDataset<Int32>(fileIdGets, "/Arrays/int");
            int[,] datasetResultArray = (int[,])datasetResult;
            int[,] expectedValue = { { 1, 2 },
                                     { 3, 4 } };

            Assert.IsTrue(SequenceEqualsExtension.SequenceEquals(expectedValue, datasetResultArray), "Dataset not get correctly.");
        }

        [TestMethod]
        public void GetDataset_TypeFloat()
        {
            var datasetResult = datasets.GetDataset<float>(fileIdGets, "/float");
            float[] datasetResultArray = (float[])datasetResult;
            float[] expectedValue = { 25.0F };

            Assert.IsTrue(datasetResultArray.SequenceEqual(expectedValue), "Dataset not get correctly.");
        }

        [TestMethod]
        public void GetDataset_TypeFloatTwoDim()
        {
            var datasetResult = datasets.GetDataset<float>(fileIdGets, "/Arrays/float");
            float[,] datasetResultArray = (float[,])datasetResult;
            float[,] expectedValue = { { 1.1F, 2.2F },
                                       { 3.3F, 4.4F } };

            Assert.IsTrue(SequenceEqualsExtension.SequenceEquals(expectedValue, datasetResultArray), "Dataset not get correctly.");
        }

        [TestMethod]
        public void GetDataset_TypeBoolean()
        {
            var datasetResult = datasets.GetDataset<Byte>(fileIdGets, "/bool");
            byte[] datasetResultArray = (byte[])datasetResult;
            byte[] expectedValue = { 1 };

            Assert.IsTrue(datasetResultArray.SequenceEqual(expectedValue), "Dataset not get correctly.");
        }

        [TestMethod]
        public void GetDataset_TypeCharTwoDim()
        {
            var datasetResult = datasets.GetDataset<Byte>(fileIdGets, "/Arrays/char");
            byte[,] datasetResultArray = (byte[,])datasetResult;
            byte[,] expectedValue = { { 32, 33 },
                                      { 34, 35 } };

            Assert.IsTrue(SequenceEqualsExtension.SequenceEquals(expectedValue, datasetResultArray), "Dataset not get correctly.");
        }

        [TestMethod]
        public void GetDataset_TypeImageThreeDim()
        {
            var datasetResult = datasets.GetDataset<SByte>(fileIdGets, "/color_image");
            sbyte[,,] datasetResultArray = (sbyte[,,])datasetResult;
            sbyte[,,] expectedValue = new sbyte[2048, 2560, 3];

            for (int x = 0; x < expectedValue.GetLength(0); x++)
                for (int y = 0; y < expectedValue.GetLength(1); y++)
                    for (int z = 0; z < expectedValue.GetLength(2); z++)
                        expectedValue[x, y, z] = 0;

            Assert.IsTrue(SequenceEqualsExtension.SequenceEqualsThreeDim(expectedValue, datasetResultArray), "Dataset not get correctly.");
        }

        [TestMethod]
        public void GetDataset_TypeString()
        {
            var datasetResult = datasets.GetDataset<string>(fileIdGets, "/vlen_string");
            string[] datasetResultArray = (string[])datasetResult;
            string[] expectedValue = { "tu jest napis śćąęłóżźń" };

            Assert.IsTrue(datasetResultArray.SequenceEqual(expectedValue), "Dataset not get correctly.");
        }

        [TestMethod]
        public void GetDataset_TypeStringTwoDim()
        {
            var datasetResult = datasets.GetDataset<string>(fileIdGets, "/Arrays/vlen_string");
            string[,] datasetResultArray = (string[,])datasetResult;
            string[,] expectedValue = { { "a", "bbbbbb" },
                                      { "cc", "ddd" } };

            Assert.IsTrue(SequenceEqualsExtension.SequenceEquals(expectedValue, datasetResultArray), "Dataset not get correctly.");
        }

        #endregion

        #region SetDataset

        [TestMethod]
        public void SetDataset_TypeInt()
        {
            int[] intArray = { 1, 2, 3, 4, 5 };
            var dataset = datasets.SetDataset<int>(fileIdSets, "int", intArray);
            Assert.IsTrue(dataset != -1, "Dataset not set correctly.");
        }

        [TestMethod]
        public void SetDataset_TypeIntTwoDim()
        {
            int[,] intTwoDimArray = { { 1, 2 },
                                      { 3, 4 } };
            var dataset = datasets.SetDataset<int>(fileIdSets, "intTwoDim", intTwoDimArray);
            Assert.IsTrue(dataset != -1, "Dataset not set correctly.");
        }

        [TestMethod]
        public void SetDataset_TypeFloat()
        {
            double[,] doubleTwoDimArray = { { 1.1, 2.2 },
                                            { 3.3, 4.4} };
            var dataset = datasets.SetDataset<float>(fileIdSets, "float", doubleTwoDimArray);
            Assert.IsTrue(dataset != -1, "Dataset not set correctly.");
        }

        [TestMethod]
        public void SetDataset_TypeFloatTwoDim()
        {
            double[] doubleArray = { 25.0 };
            var dataset = datasets.SetDataset<float>(fileIdSets, "floatTwoDim", doubleArray);
            Assert.IsTrue(dataset != -1, "Dataset not set correctly.");
        }

        [TestMethod]
        public void SetDataset_TypeBoolean()
        {
            byte[] byteArray = { 1, 0, 1 };
            var dataset = datasets.SetDataset<byte>(fileIdSets, "byte", byteArray);
            Assert.IsTrue(dataset != -1, "Dataset not set correctly.");
        }

        [TestMethod]
        public void SetDataset_TypeBooleanTwoDim()
        {
            byte[,] byteTwoDimArray = { { 1, 0 },
                                        { 1, 1 } };
            var dataset = datasets.SetDataset<byte>(fileIdSets, "byteTwoDim", byteTwoDimArray);
            Assert.IsTrue(dataset != -1, "Dataset not set correctly.");
        }

        [TestMethod]
        public void SetDataset_TypeChar()
        {
            byte[,] charArray = { { 32, 33 },
                                  { 34, 35 } };
            var dataset = datasets.SetDataset<byte>(fileIdSets, "charTwoDim", charArray);
            Assert.IsTrue(dataset != -1, "Dataset not set correctly.");
        }

        [TestMethod]
        public void SetDataset_TypeColorImage()
        {
            sbyte[,,] imageColorArray = { { { 1, 2, 3}, {4, 5, 6} },
                                          { { 7, 8, 9}, {10, 11, 12} } };
            var dataset = datasets.SetDataset<byte>(fileIdSets, "imageColor", imageColorArray);
            Assert.IsTrue(dataset != -1, "Dataset not set correctly.");
        }

        [TestMethod]
        public void SetDataset_TypeString()
        {
            string[] stringArray = { "tu jest napis śćąęłóżźń", "asdf", "asdf" };
            var dataset = datasets.SetDataset<string>(fileIdSets, "string", stringArray);
            Assert.IsTrue(dataset != -1, "Dataset not set correctly.");
        }

        #endregion

        #region RemoveDataset

        [TestMethod]
        public void RemoveDataset()
        {
            var removeResult = datasets.DeleteDataset(fileIdSets, "datasetname");
            Assert.IsTrue(removeResult == 0, "Group not removed correctly.");
        }

        [TestMethod]
        public void RemoveNotExistingDataset()
        {
            var removeResult = datasets.DeleteDataset(fileIdSets, "datasetnameee");
            Assert.IsTrue(removeResult == -1, "Group removed, should failed.");
        }

        #endregion

        [TestMethod]
        public void MultiThreadedTest()
        {
            double[,] doubleTwoDimArray = { { 1.1, 2.2 },
                                            { 3.3, 4.4} };
            long dataset0 = -1, dataset1 = -1, dataset2 = -1;
            Thread[] threads = new Thread[3];

            threads[0] = new Thread(() =>
            {
                dataset0 = datasets.SetDataset<float>(fileIdSets, "float0", doubleTwoDimArray);
            });
            threads[1] = new Thread(() =>
            {
                dataset1 = datasets.SetDataset<float>(fileIdSets, "float1", doubleTwoDimArray);
            });
            threads[2] = new Thread(() =>
            {
                dataset2 = datasets.SetDataset<float>(fileIdSets, "float2", doubleTwoDimArray);
            });

            foreach (Thread thread in threads)
                thread.Start();

            foreach (Thread thread in threads)
                thread.Join();

            Assert.IsTrue(dataset0 != -1 && dataset1 != -1 && dataset2 != -1, "MultiThreadedTest failed.");
        }

        [TestMethod]
        public void MultiThreadedTest2()
        {
            double[,] doubleTwoDimArray = { { 1.1, 2.2 },
                                            { 3.3, 4.4} };
            long dataset0 = -1, dataset1 = -1, dataset2 = -1,
                 dataset3 = -1, dataset4 = -1, dataset5 = -1; 
            Thread[] threads = new Thread[6];

            threads[0] = new Thread(() =>
            {
                dataset0 = datasets.SetDataset<float>(fileIdSets, "floatt0", doubleTwoDimArray);
            });
            threads[1] = new Thread(() =>
            {
                dataset1 = datasets.SetDataset<float>(fileIdSets2, "floatt1", doubleTwoDimArray);
            });
            threads[2] = new Thread(() =>
            {
                dataset2 = datasets.SetDataset<float>(fileIdSets, "floatt2", doubleTwoDimArray);
            });
            threads[3] = new Thread(() =>
            {
                dataset3 = datasets.SetDataset<float>(fileIdSets2, "floatt3", doubleTwoDimArray);
            });
            threads[4] = new Thread(() =>
            {
                dataset4 = datasets.SetDataset<float>(fileIdSets, "floatt4", doubleTwoDimArray);
            });
            threads[5] = new Thread(() =>
            {
                dataset5 = datasets.SetDataset<float>(fileIdSets2, "floatt5", doubleTwoDimArray);
            });

            foreach (Thread thread in threads)
                thread.Start();

            foreach (Thread thread in threads)
                thread.Join();

            Assert.IsTrue(dataset0 != -1 && dataset1 != -1 && dataset2 != -1 && 
                            dataset3 != -1 && dataset4 != -1 && dataset5 != -1, "MultiThreadedTest2 failed.");
        }

        [TestMethod]
        public void GetDatasets()
        {
            var expectedResult = new List<string>
            { "/bool", "/char", "/color_image", "/enum", "/float",
              "/float1", "/float3", "/float5", "/floatt1", "/floatt3",
              "/floatt5", "/int", "/mono_image", "/string", "/vlen_string", };

            var datasetsList = datasets.GetDetasets(fileIdGets);
            Assert.IsTrue(datasetsList.SequenceEqual(expectedResult), "GetDatasets failed.");
        }

        [TestMethod]
        public void GetDatasetsAllDepth()
        {
            var expectedResult = new List<string>
            { "/Arrays/char", "/Arrays/float", "/Arrays/int", "/Arrays/string", "/Arrays/vlen_string",
              "/bool", "/char", "/color_image", "/enum", "/float",
              "/float1", "/float3", "/float5", "/floatt1", "/floatt3",
              "/floatt5", "/int", "/mono_image", "/string", "/vlen_string", };

            var datasetsList = datasets.GetDetasets(fileIdGets, false);
            Assert.IsTrue(datasetsList.SequenceEqual(expectedResult), "GetDatasets failed.");
        }
    }
}
