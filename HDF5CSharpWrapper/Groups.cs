using HDF.PInvoke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
            => H5G.open(groupLocationId, groupName);

        /// <summary>
        /// Create a specific group
        /// </summary>
        /// <param name="groupLocationId">Group location id to be created</param>
        /// <param name="groupName">Group name to be created</param>
        /// <returns>Group Location Id</returns>
        public long CreateGroup(long groupLocationId, string groupName)
            => H5G.create(groupLocationId, groupName);

        /// <summary>
        /// Creates all groups seperated with /
        /// </summary>
        /// <param name="groupLocationId">Starting group id</param>
        /// <param name="groupNames">Groups names</param>
        /// <returns>Group Location Id</returns>
        public long CreateAllGroupsSeperatedWithForwardslash(long groupLocationId, string groupNames)
        {
            IEnumerable<string> groups = groupNames.Split('/');
            long gLId = groupLocationId;
            groupNames = "";
            foreach (var name in groups)
            {
                groupNames = string.Concat(groupNames, "/", name);
                gLId = IsGroupExisting(gLId, groupNames) ? OpenGroup(gLId, groupNames) : CreateGroup(gLId, groupNames);
            }
            return gLId;
        }

        /// <summary>
        /// Removes a specific group
        /// NOTE: Also remove all the content of a group
        /// </summary>
        /// <param name="groupLocationId">Group Id to be removed</param>
        /// <param name="groupName">Group name to be removed</param>
        /// <returns>0 if successful, -1 if not</returns>
        public long DeleteGroup(long groupLocationId, string groupName)
            => H5L.delete(groupLocationId, groupName);

        /// <summary>
        /// Check if group already exist
        /// </summary>
        /// <param name="groupLocationId"></param>
        /// <param name="groupName"></param>
        /// <returns>True if existing False if not</returns>
        public bool IsGroupExisting(long groupLocationId, string groupName)
            => H5L.exists(groupLocationId, groupName) > 0;

        /// <summary>
        /// Get list of groups in specified location
        /// </summary>
        /// <param name="locationId">Group or file id</param>
        /// <param name="oneLevelDepth">True if only one level groups depth should be shown/param>
        /// <returns>List of containing groups names</returns>
        public List<string> GetGroups(long locationId, bool oneLevelDepth = true)
        {
            var elements = new List<string>();

            try
            {
                ulong idx = 0;
                H5L.iterate(locationId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, Marshal.StringToHGlobalAnsi(""));
            }
            catch (Exception e)
            {
                throw new Exception($"Error durning reading groups structure of {locationId}. Error: {e}");
            }

            int Callback(long elementId, IntPtr intPtrName, ref H5L.info_t info, IntPtr intPtrUserData)
            {
                ulong idx2 = 0;
                long datasetId = -1;
                long groupId;
                H5O.type_t objectType = H5O.type_t.GROUP;
                var name = Marshal.PtrToStringAnsi(intPtrName);
                var userData = Marshal.PtrToStringAnsi(intPtrUserData);
                var fullName = userData + "/" + name;

                var gInfo = new H5O.info_t();
                H5O.get_info_by_name(elementId, fullName, ref gInfo);

                if (gInfo.type == H5O.type_t.GROUP && (H5L.exists(elementId, name) >= 0))
                    groupId = H5G.open(elementId, name);
                else
                    groupId = -1L;

                if (H5I.is_valid(groupId) > 0)
                    objectType = H5O.type_t.GROUP;
                else
                {
                    datasetId = H5D.open(elementId, name);
                    if ((H5I.is_valid(datasetId) > 0))
                        objectType = H5O.type_t.DATASET;
                    else
                        objectType = H5O.type_t.UNKNOWN;
                }

                var parent = elements.FirstOrDefault(e =>
                {
                    var index = fullName.LastIndexOf("/", StringComparison.Ordinal);
                    var partial = fullName.Substring(0, index);
                    return partial.Equals(e);

                });

                if (parent == null && objectType == H5O.type_t.GROUP)
                    elements.Add(fullName);
                else if (objectType == H5O.type_t.GROUP)
                    elements.Add(fullName);

                if (objectType == H5O.type_t.GROUP && !oneLevelDepth)
                    H5L.iterate(groupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx2, Callback, Marshal.StringToHGlobalAnsi(fullName));

                if (H5I.is_valid(groupId) > 0)
                    H5G.close(groupId);
                if (H5I.is_valid(datasetId) > 0)
                    H5D.close(datasetId);

                return 0;
            }

            return elements;
        }

        /// <summary>
        /// Closes spiecified group
        /// </summary>
        /// <param name="groupId">Group id to be closed</param>
        /// <returns>0 if successful, -1 if not</returns>
        public int CloseGroup(long groupId)
            => H5G.close(groupId);
    }
}
