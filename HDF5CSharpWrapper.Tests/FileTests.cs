using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HDF5CSharpWrapper.Tests
{
    [TestClass]
    public class FileTests
    {
        private static string DirectoryName { get; set; } = "../../../TestFiles/";

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            File file = new File();
            var openedFileId = file.Create(DirectoryName + "test.h5");
            file.Close(openedFileId);
        }

        [TestMethod]
        public void OpenFile()
        {
            File file = new File();
            var createdFileId = file.Open(DirectoryName + "test.h5");
            file.Close(createdFileId);
            Assert.IsTrue(createdFileId != -1);
        }

        [TestMethod]
        public void OpenMissingFile()
        {
            File file = new File();
            var createdFileId = file.Open(DirectoryName + "testtt.h5");
            file.Close(createdFileId);
            Assert.IsTrue(createdFileId == -1);
        }

        [TestMethod]
        public void CreateFile()
        {
            File file = new File();
            var createdFileId = file.Create(DirectoryName + "fileTest.h5");
            file.Close(createdFileId);
            Assert.IsTrue(createdFileId != -1);
        }

        [TestMethod]
        public void CreateExistingFileWithFileModeWriteIfNew()
        {
            File file = new File();
            var createdFileId = file.Create(DirectoryName + "test.h5", FileMode.WriteIfNew);
            file.Close(createdFileId);
            Assert.IsTrue(createdFileId == -1);
        }
    }
}
