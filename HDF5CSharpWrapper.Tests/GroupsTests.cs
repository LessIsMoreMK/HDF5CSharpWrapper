using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace HDF5CSharpWrapper.Tests
{
    [TestClass]
    public class GroupsTests
    {
        private static string DirectoryName { get; set; } = "../../../TestFiles/";

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            File file = new File();
            Groups groups = new Groups();

            var openedFileId = file.Create(DirectoryName + "groupsTest.h5");
            var openedGroupId = groups.CreateGroup(openedFileId, "Arrays");
            var openedGroupId2 = groups.CreateGroup(openedFileId, "Arrays2");
            file.Close(openedFileId);
        }

        [TestMethod]
        public void OpenGroup()
        {
            File file = new File();
            Groups groups = new Groups();

            var openedFileId = file.Open(DirectoryName + "groupsTest.h5");
            var openedGroupId = groups.OpenGroup(openedFileId, "Arrays");
            file.Close(openedFileId);
            Assert.IsTrue(openedGroupId != -1);
        }

        [TestMethod]
        public void OpenGroupInsideGroup()
        {
            //TODO
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void CreateGroup()
        {
            File file = new File();
            Groups groups = new Groups();

            var createdFileId = file.Create(DirectoryName + "groupsTest2.h5");
            var createdGroupId = groups.CreateGroup(createdFileId, "groupName");
            file.Close(createdFileId);
            Assert.IsTrue(createdGroupId != -1);
        }

        [TestMethod]
        public void CreateGroupInsideGroup()
        {
            //TODO
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void RemoveGroup()
        {
            File file = new File();
            Groups groups = new Groups();

            var createdFileId = file.Open(DirectoryName + "groupsTest.h5");
            var removedGroupId = groups.RemoveGroup(createdFileId, "Arrays2");
            file.Close(createdFileId);
            Assert.IsTrue(removedGroupId != -1);
        }
    }
}
