using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HDF5CSharpWrapper.Tests
{
    [TestClass]
    public class GroupsTests
    {
        private static string DirectoryName { get; set; } = "../../../TestFiles/";
        static File file;
        static Groups groups;
        static long openedFileId;
        static long openedFileId2;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            file = new File();
            groups = new Groups();

            openedFileId = file.CreateFile(DirectoryName + "groupsTest.h5");
            openedFileId2 = file.OpenFile(DirectoryName + "groupsTest3.h5");
            var openedGroupId = groups.CreateGroup(openedFileId, "Arrays");
            var openedGroupId2 = groups.CreateGroup(openedGroupId, "Arrays2");
            var openedGroupId3 = groups.CreateGroup(openedFileId, "Arrays3");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            file.CloseFile(openedFileId);
        }

        [TestMethod]
        public void OpenGroup()
        {
            var openedGroupId = groups.OpenGroup(openedFileId, "Arrays");
            Assert.IsTrue(openedGroupId > 0, "Group not opened correctly.");
        }

        [TestMethod]
        public void OpenGroupInsideGroup()
        {
            var openedGroupId = groups.OpenGroup(openedFileId, "Arrays");
            Assert.IsTrue(openedGroupId > 0, "Group not opened correctly.");
            var openedGroupId2 = groups.OpenGroup(openedGroupId, "Arrays2");
            Assert.IsTrue(openedGroupId2 > 0, "Group not opened correctly.");
        }

        [TestMethod]
        public void CreateGroup()
        {
            var createdGroupId = groups.CreateGroup(openedFileId, "CreateGroupName");
            Assert.IsTrue(createdGroupId > 0, "Group not created correctly.");
        }

        [TestMethod]
        public void CreateGroupInsideGroup()
        {
            var createdGroupId = groups.CreateGroup(openedFileId, "CreateGroupName2");
            Assert.IsTrue(createdGroupId > 0, "Group not created correctly.");
            var createdGroupId2 = groups.CreateGroup(createdGroupId, "second");
            Assert.IsTrue(createdGroupId2 > 0, "Group not created correctly.");
            var createdGroupId3 = groups.CreateGroup(createdGroupId2, "third");
            Assert.IsTrue(createdGroupId3 > 0, "Group not created correctly.");
        }

        [TestMethod]
        public void RemoveGroup()
        {
            var removedGroupId = groups.DeleteGroup(openedFileId, "Arrays3");
            Assert.IsTrue(removedGroupId == 0, "Removed group failed.");
        }

        [TestMethod]
        public void GetGroups()
        {
            var expectedResult = new List<string> { "/groupName" };

            var groupsList = groups.GetGroups(openedFileId2);
            Assert.IsTrue(groupsList.SequenceEqual(expectedResult), "GetGroups failed.");
        }

        [TestMethod]
        public void GetGroupsAllDepth()
        {
            var expectedResult = new List<string> { "/groupName", "/groupName/second", "/groupName/second/third" };

            var groupsList = groups.GetGroups(openedFileId2, false);
            Assert.IsTrue(groupsList.SequenceEqual(expectedResult), "GetGroups failed.");
        }
    }
}
