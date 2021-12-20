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
            var openedGroupId2 = groups.CreateGroup(openedGroupId, "Arrays2");
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
            File file = new File();
            Groups groups = new Groups();

            var openedFileId = file.Open(DirectoryName + "groupsTest.h5");
            var openedGroupId = groups.OpenGroup(openedFileId, "Arrays");
            var openedGroupId2 = groups.OpenGroup(openedGroupId, "Arrays2");
            file.Close(openedFileId);
            Assert.IsTrue(openedGroupId != -1 && openedGroupId2 != -1);
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
            File file = new File();
            Groups groups = new Groups();

            var createdFileId = file.Create(DirectoryName + "groupsTest3.h5");
            var createdGroupId = groups.CreateGroup(createdFileId, "groupName");
            var createdGroupId2 = groups.CreateGroup(createdGroupId, "second");
            var createdGroupId3 = groups.CreateGroup(createdGroupId2, "third");
            file.Close(createdFileId);
            Assert.IsTrue(createdGroupId != -1 && createdGroupId2 != -1 && createdGroupId3 != -1);
        }

        [TestMethod]
        public void RemoveGroup()
        {
            File file = new File();
            Groups groups = new Groups();

            var createdFileId = file.Open(DirectoryName + "groupsTest.h5");
            var removedGroupId = groups.RemoveGroup(createdFileId, "Arrays");
            file.Close(createdFileId);
            Assert.IsTrue(removedGroupId != -1);
        }
    }
}
