﻿using HDF.PInvoke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HDF5CSharpWrapper
{
    /// <summary>
    /// Responsible for groups operations
    /// </summary>
    public class Groups
    {
        /// <summary>
        /// Opens a specific group
        /// </summary>
        /// <param name="groupLocationId">Group location id to be opened</param>
        /// <param name="groupName">Group name to be opened</param>
        /// <returns>Group Location Id</returns>
        public long OpenGroup(long groupLocationId, string groupName)
            => H5G.open(groupLocationId, string.Concat("/", groupName));

        /// <summary>
        /// Create a specific group
        /// </summary>
        /// <param name="groupLocationId">Group location id to be created</param>
        /// <param name="groupName">Group name to be created</param>
        /// <returns>Group Location Id</returns>
        public long CreateGroup(long groupLocationId, string groupName)
            => H5G.create(groupLocationId, string.Concat("/", groupName));

        /// <summary>
        /// Creates group inside group
        /// </summary>
        /// <param name="groupLocationId"></param>
        /// <param name="groupName"></param>
        /// <returns>Group Location Id</returns>
        public long CreateGroupInsideGroup(long groupLocationId, string groupName)
        {
            //TODO 
            var gid = H5G.create(groupLocationId, groupName);

            return gid;
        }

        /// <summary>
        /// Removes a specific group
        /// NOTE: Also remove all the content of a group
        /// </summary>
        /// <param name="groupLocationId">Group Id to be removed</param>
        /// <param name="groupName">Group name to be removed</param>
        /// <returns>0 if successful, -1 if not</returns>
        public long RemoveGroup(long groupLocationId, string groupName)
            => H5L.delete(groupLocationId, groupName);

        /// <summary>
        /// Check if group already exist
        /// </summary>
        /// <param name="groupLocationId"></param>
        /// <param name="groupName"></param>
        /// <returns>True if existing False if not</returns>
        public static bool IsGroupExisting(long groupLocationId, string groupName)
            => H5L.exists(groupLocationId, groupName) > 0;

        /*public static int CloseGroup(long groupId)
            => H5G.close(groupId);*/
    }
}