using HDF.PInvoke;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Xml.Linq;

namespace HDF5CSharpWrapper
{
    /// <summary>
    /// Responsible for datasets operations
    /// </summary>
    public class Datasets
    {
        /// <summary>
        /// Get specified dataset to Array
        /// </summary>
        /// <typeparam name="T">Generic type parameter</typeparam>
        /// <param name="locationId">Location id from which array will be created</param>
        /// <param name="datasetName">Path to dataset</param>
        /// <returns>Result Array or -1 if failed</returns>
        public Array GetDataset<T>(long locationId, string datasetName)
        {
            if (typeof(T) == typeof(string)) 
                return GetStringDataset(locationId, datasetName);

            var dataType = DataTypeHelpers.GetDataType(typeof(T));
            var datasetId = H5D.open(locationId, datasetName);
            Array dataSet;
            var spaceId = H5D.get_space(datasetId);
            int rank = H5S.get_simple_extent_ndims(spaceId);
            long count = H5S.get_simple_extent_npoints(spaceId);
            Type type = typeof(T);

            if (rank >= 0 && count >= 0)
            {
                int rankChunk;
                ulong[] maxDims = new ulong[rank];
                ulong[] dims = new ulong[rank];
                ulong[] chunkDims = new ulong[rank];
                long memId = H5S.get_simple_extent_dims(spaceId, dims, maxDims);
                long[] lengths = dims.Select(d => Convert.ToInt64(d)).ToArray();
                dataSet = Array.CreateInstance(type, lengths);

                if (dataType == H5T.C_S1)
                    H5T.set_size(dataType, new IntPtr(2));

                var propId = H5D.get_create_plist(datasetId);

                if (H5D.layout_t.CHUNKED == H5P.get_layout(propId))
                    rankChunk = H5P.get_chunk(propId, rank, chunkDims);

                memId = H5S.create_simple(rank, dims, maxDims);
                GCHandle gcHandle = GCHandle.Alloc(dataSet, GCHandleType.Pinned);
                H5D.read(datasetId, dataType, memId, spaceId, H5P.DEFAULT, gcHandle.AddrOfPinnedObject());
                gcHandle.Free();
            }
            else
                dataSet = Array.CreateInstance(type, new long[1] { 0 });

            H5D.close(datasetId);
            H5S.close(spaceId);
            return dataSet;
        }

        /// <summary>
        /// Get string dataset to Array
        /// </summary>
        /// <param name="locationId">Location id from which array will be created</param>
        /// <param name="datasetName">Path to dataset</param>
        /// <returns>Result Array or -1 if failed</returns>
        public Array GetStringDataset(long locationId, string datasetName)
        {
            var datasetId = H5D.open(locationId, datasetName);
            var typeId = H5D.get_type(datasetId);
            var spaceId = H5D.get_space(datasetId);
            int rank = H5S.get_simple_extent_ndims(spaceId);
            long count = H5S.get_simple_extent_npoints(spaceId);
            ulong[] maxDims = new ulong[rank];
            ulong[] dims = new ulong[rank];
            long memId = H5S.get_simple_extent_dims(spaceId, dims, maxDims);
            long[] lengths = dims.Select(d => Convert.ToInt64(d)).ToArray();

            IntPtr[] rdata = new IntPtr[count];

            GCHandle gcHandle = GCHandle.Alloc(rdata, GCHandleType.Pinned);
            H5D.read(datasetId, typeId, H5S.ALL, H5S.ALL, H5P.DEFAULT, gcHandle.AddrOfPinnedObject());

            var resultArray = new List<string>();
            for (int i = 0; i < rdata.Length; ++i)
            {
                int attrLength = 0;
                while (Marshal.ReadByte(rdata[i], attrLength) != 0)
                {
                    ++attrLength;
                }

                byte[] buffer = new byte[attrLength];
                Marshal.Copy(rdata[i], buffer, 0, buffer.Length);
                string stringPart = Encoding.UTF8.GetString(buffer);
                resultArray.Add(stringPart);
                H5.free_memory(rdata[i]);
            }

            gcHandle.Free();
            H5S.close(spaceId);
            H5D.close(datasetId);

            if (rank == 2)
            {
                string[,] resultArray2 = new string[lengths[0], lengths[1]];

                var index = 0;
                for (long i = 0; i < lengths[0]; i++)
                    for (long j = 0; j < lengths[1]; j++)
                    {
                        resultArray2[i, j] = resultArray[index];
                        index++;
                    }
                return resultArray2;
            }
            else
                return resultArray.ToArray();
        }

        /// <summary>
        /// Write specified Array to file as dataset
        /// </summary>
        /// <typeparam name="T">Generic type parameter</typeparam>
        /// <param name="locationId">Location id in which dataset will be created</param>
        /// <param name="datasetName">Name of a dataset</param>
        /// <param name="dset">Array to be written</param>
        /// <returns>Dataset id or -1 if failed</returns>
        /// 
        public long SetDataset<T>(long locationId, string datasetName, Array dset)
        {
            if (typeof(T) == typeof(string))
                return SetStringDataset(locationId, datasetName, dset);

            ulong[] dims = Enumerable.Range(0, dset.Rank).Select(i => { return (ulong)dset.GetLength(i); }).ToArray();
            var spaceId = H5S.create_simple(dset.Rank, dims, null);
            var dataType = DataTypeHelpers.GetDataType(typeof(T));

            if (dataType == H5T.C_S1)
                H5T.set_size(dataType, new IntPtr(2));

            var datasetId = H5D.create(locationId, datasetName, dataType, spaceId);

            GCHandle hnd = GCHandle.Alloc(dset, GCHandleType.Pinned);
            var result = H5D.write(datasetId, dataType, H5S.ALL, H5S.ALL, H5P.DEFAULT, hnd.AddrOfPinnedObject());
            hnd.Free();
            H5D.close(datasetId);
            H5S.close(spaceId);
            return datasetId;
        }

        /// <summary>
        /// Write string Array to file as dataset
        /// </summary>
        /// <param name="locationId">Location id in which array will be created</param>
        /// <param name="datasetName">Path to dataset</param>
        /// <param name="dset">Array to be written</param>
        /// <returns>Dataset id or -1 if failed</returns>
        public long SetStringDataset(long locationId, string datasetName, Array dset)
        {
            long dataType = H5T.create(H5T.class_t.STRING, H5T.VARIABLE);
            H5T.set_cset(dataType, H5T.cset_t.UTF8);
            H5T.set_strpad(dataType, H5T.str_t.NULLTERM);
            int strSz = dset.Length;
            long spaceId = H5S.create_simple(1, new[] { (ulong)strSz }, null);

            var datasetId = H5D.create(locationId, datasetName, dataType, spaceId);
            
            if (datasetId == -1)
                return -1;

            GCHandle[] hnds = new GCHandle[strSz];
            IntPtr[] wdata = new IntPtr[strSz];
            int cntr = 0;
            foreach (string str in dset)
            {
                hnds[cntr] = GCHandle.Alloc(Encoding.UTF8.GetBytes(str), GCHandleType.Pinned);
                wdata[cntr] = hnds[cntr].AddrOfPinnedObject();
                cntr++;
            }

            var hnd = GCHandle.Alloc(wdata, GCHandleType.Pinned);
            var result = H5D.write(datasetId, dataType, H5S.ALL, H5S.ALL, H5P.DEFAULT, hnd.AddrOfPinnedObject());
            hnd.Free();

            for (int i = 0; i < strSz; ++i)
                hnds[i].Free();

            H5D.close(datasetId);
            H5S.close(spaceId);
            H5T.close(dataType);
            return datasetId;
        }

        /// <summary>
        /// Remove whole dataset
        /// </summary>
        /// <param name="locationId">Location id in which dataset is</param>
        /// <param name="datasetPath">Path with name to dataset</param>
        /// <returns>0 if successfull -1 if not</returns>
        public long DeleteDataset(long locationId, string datasetPath)
            => H5L.delete(locationId, datasetPath);

        /// <summary>
        /// Get list of datasets in specified location
        /// </summary>
        /// <param name="locationId">Group or file id</param>
        /// <param name="oneLevelDepth">True if only one level depth should be shown/param>
        /// <returns>List of containing datasets names</returns>
        public List<string> GetDetasets(long locationId, bool oneLevelDepth = true)
        {
            var elements = new List<string>();

            try
            {
                ulong idx = 0;
                H5L.iterate(locationId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, Marshal.StringToHGlobalAnsi(""));
            }
            catch (Exception e)
            {
                throw new Exception($"Error durning reading datasets structure of {locationId}. Error:{e}");
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
                if (fullName.StartsWith("/"))
                    fullName = fullName.Remove(0, 1);

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
                    var partial = String.Empty;
                    if (index != -1)
                        partial = fullName.Substring(0, index);
                    return partial.Equals(e);
                });

                if (parent == null && objectType == H5O.type_t.DATASET)
                    elements.Add(fullName);
                else if (objectType == H5O.type_t.DATASET)
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
    }
}
