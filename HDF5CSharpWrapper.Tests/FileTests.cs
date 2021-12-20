using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HDF5CSharpWrapper.Tests
{
    [TestClass]
    public class FileTests
    {
        private static string DirectoryName { get; set; } = "../../../TestFiles/";
        static File file;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            file = new File();
            var createdfileId = file.Create(DirectoryName + "test.h5");
            var closeResult = file.Close(createdfileId);
        }

        [TestMethod]
        public void OpenAndCloseFile()
        {
            var createdFileId = file.Open(DirectoryName + "test.h5");
            Assert.IsTrue(createdFileId > 0, "File not opened correctly.");
            var closeResult = file.Close(createdFileId);
            Assert.IsTrue(closeResult == 0, "File not closed correctly.");
        }

        [TestMethod]
        public void OpenAndCloseMissingFile()
        {
            var createdFileId = file.Open(DirectoryName + "testtt.h5");
            Assert.IsTrue(createdFileId == -1, "Opened not exisitng file, should fail.");
        }

        [TestMethod]
        public void CreateAndCloseFile()
        {
            var createdFileId = file.Create(DirectoryName + "fileTest.h5");
            Assert.IsTrue(createdFileId > 0, "File not created correctly.");
            var closeResult = file.Close(createdFileId);
            Assert.IsTrue(closeResult == 0, "File not closed correctly.");
        }

        [TestMethod]
        public void CreateExistingFileWithFileModeWriteIfNew()
        {
            var createdFileId = file.Create(DirectoryName + "test.h5", FileMode.WriteIfNew);
            Assert.IsTrue(createdFileId == -1, "Already eisiting file created, should fail.");
        }
    }
}
