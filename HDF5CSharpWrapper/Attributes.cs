using HDF.PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF5CSharpWrapper
{
    public class Attributes
    {
        /// <summary>
        /// Get specified attributes to array 
        /// </summary>
        /// <param name="groupLocationId">Attribute location</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="datasetName">If specified gets attribute form datasetName otherwise from group</param>
        /// <returns>Attributes Array or -1 if faild</returns>
        public Array GetAttribute<T>(long groupLocationId, string attributeName, string datasetName) 
        {
            if (typeof(T) == typeof(string))
                return GetStringAttribute(groupLocationId, attributeName, datasetName);

            long tempId = groupLocationId;
            if (!string.IsNullOrWhiteSpace(datasetName))
            {
                long datasetId = H5D.open(groupLocationId, datasetName);
                if (datasetId > 0)
                    groupLocationId = datasetId;
            }
           
            var attributeId = H5A.open(groupLocationId, attributeName);
            if (attributeId <= 0)
                attributeId = H5A.open(groupLocationId, datasetName);

            if (attributeId <= 0)
                return Array.Empty<T>();

            var dataType = DataTypeHelpers.GetDataType(typeof(T));
            var spaceId = H5A.get_space(attributeId);
            int rank = H5S.get_simple_extent_ndims(spaceId);
            ulong[] maxDims = new ulong[rank];
            ulong[] dims = new ulong[rank];
            long memId = H5S.get_simple_extent_dims(spaceId, dims, maxDims);
            long[] lengths = dims.Select(d => Convert.ToInt64(d)).ToArray();
            Array attributes = Array.CreateInstance(typeof(T), lengths);

            var typeId = H5A.get_type(attributeId);
            if (dataType == H5T.C_S1)
                H5T.set_size(dataType, new IntPtr(2));

            GCHandle hnd = GCHandle.Alloc(attributes, GCHandleType.Pinned);
            H5A.read(attributeId, dataType, hnd.AddrOfPinnedObject());
            hnd.Free();
            H5T.close(typeId);
            H5A.close(attributeId);
            H5S.close(spaceId);

            return attributes;
        }

        /// <summary>
        /// Get specified string attributes to array 
        /// </summary>
        /// <param name="groupLocationId">Attribute location</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="datasetName">If specified gets attribute form datasetName otherwise from group</param>
        /// <returns>Attributes Array or -1 if faild</returns>
        public Array GetStringAttribute(long groupLocationId, string attributeName, string datasetName)
        {
            var nameToUse = attributeName;
            if (!string.IsNullOrWhiteSpace(datasetName))
                groupLocationId = H5D.open(groupLocationId, datasetName);

            var datasetId = H5A.open(groupLocationId, attributeName);
            long typeId = H5A.get_type(datasetId);
            long spaceId = H5A.get_space(datasetId);
            long count = H5S.get_simple_extent_npoints(spaceId);
            H5S.close(spaceId);

            IntPtr[] rdata = new IntPtr[count];
            GCHandle hnd = GCHandle.Alloc(rdata, GCHandleType.Pinned);
            H5A.read(datasetId, typeId, hnd.AddrOfPinnedObject());

            var attributes = new List<string>();
            for (int i = 0; i < rdata.Length; ++i)
            {
                if (rdata[i] == IntPtr.Zero)
                {
                    continue;
                }
                int len = 0;
                while (Marshal.ReadByte(rdata[i], len) != 0) { ++len; }
                byte[] buffer = new byte[len];
                Marshal.Copy(rdata[i], buffer, 0, buffer.Length);
                string s = Encoding.ASCII.GetString(buffer);
                attributes.Add(s);

                H5.free_memory(rdata[i]);
            }

            hnd.Free();
            H5T.close(typeId);
            H5A.close(datasetId);
            return attributes.ToArray();
        }

        /// <summary>
        /// Set attribute on specified group or dataset
        /// </summary>
        /// <typeparam name="T">Generic type parameter</typeparam>
        /// <param name="groupLocationId"></param>
        /// <param name="attributeName">Attribute specified name</param>
        /// <param name="attributes">Attributes to be set</param>
        /// <param name="datasetName">If specified creates attribute on given dataset otherwise on group</param>
        /// <returns>Attribute id or -1 if failed</returns>
        public long SetAttribute<T>(long groupLocationId, string attributeName, Array attributes, string datasetName = null)
        {
            if (typeof(T) == typeof(string))
                return SetStringAttribute(groupLocationId, attributeName, attributes, datasetName);

            long tempId = groupLocationId;
            if (!string.IsNullOrWhiteSpace(datasetName))
            {
                long datasetId = H5D.open(groupLocationId, datasetName);
                if (datasetId > 0)
                    groupLocationId = datasetId;
            }

            int rank = attributes.Rank;
            ulong[] dims = Enumerable.Range(0, rank).Select(i => (ulong)attributes.GetLength(i)).ToArray();
            ulong[] maxDims = null;
            var spaceId = H5S.create_simple(rank, dims, maxDims);
            var dataType = DataTypeHelpers.GetDataType(typeof(T));
            var typeId = H5T.copy(dataType);
            var attributeId = H5A.create(groupLocationId, attributeName, dataType, spaceId);
            GCHandle hnd = GCHandle.Alloc(attributes, GCHandleType.Pinned);
            var result = H5A.write(attributeId, dataType, hnd.AddrOfPinnedObject());

            hnd.Free();
            H5A.close(attributeId);
            H5S.close(spaceId);
            H5T.close(typeId);
            if (tempId != groupLocationId)
                H5D.close(groupLocationId);

            return attributeId;
        }

        /// <summary>
        /// Set string attribute on specified group or dataset
        /// </summary>
        /// <param name="groupLocationId"></param>
        /// <param name="attributeName">Attribute specified name</param>
        /// <param name="attributes">Attributes to be set</param>
        /// <param name="datasetName">If specified creates attribute on given dataset otherwise on group</param>
        /// <returns>Attribute id or -1 if failed</returns>
        public long SetStringAttribute(long groupLocationId, string attributeName, Array attributes, string datasetName = null)
        {
            long tempId = groupLocationId;
            if (!string.IsNullOrWhiteSpace(datasetName))
            {
                long datasetId = H5D.open(groupLocationId, datasetName);
                if (datasetId > 0)
                    groupLocationId = datasetId;
            }

            long dataType = H5T.create(H5T.class_t.STRING, H5T.VARIABLE);
            H5T.set_cset(dataType, H5T.cset_t.UTF8);
            H5T.set_strpad(dataType, H5T.str_t.NULLPAD);
            int strSz = attributes.Length;
            long spaceId = H5S.create_simple(1, new[] { (ulong)strSz }, null);
            var attributeId = H5A.create(groupLocationId, attributeName, dataType, spaceId);
            GCHandle[] hnds = new GCHandle[strSz];
            IntPtr[] wdata = new IntPtr[strSz];

            int cntr = 0;
            foreach (string str in attributes)
            {
                hnds[cntr] = GCHandle.Alloc(Encoding.UTF8.GetBytes(str), GCHandleType.Pinned);
                wdata[cntr] = hnds[cntr].AddrOfPinnedObject();
                cntr++;
            }

            var hnd = GCHandle.Alloc(wdata, GCHandleType.Pinned);
            var result = H5A.write(attributeId, dataType, hnd.AddrOfPinnedObject());
            hnd.Free();

            for (int i = 0; i < strSz; ++i)
                hnds[i].Free();

            H5A.close(attributeId);
            H5S.close(spaceId);
            H5T.close(dataType);
            if (tempId != groupLocationId)
                H5D.close(groupLocationId);

            return attributeId;
        }
    }
}
