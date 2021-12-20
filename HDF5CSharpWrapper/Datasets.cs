using HDF.PInvoke;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;

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

            var dataType = GetDatatype(typeof(T));
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

            IntPtr[] rdata = new IntPtr[count];

            GCHandle gcHandle = GCHandle.Alloc(rdata, GCHandleType.Pinned);
            H5D.read(datasetId, typeId, H5S.ALL, H5S.ALL,  H5P.DEFAULT, gcHandle.AddrOfPinnedObject());

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
                string[,] resultArray2 = new string[resultArray.Count/2, resultArray.Count/2];

                var index = 0;
                for (int i=0; i < resultArray.Count/2;i++)
                {
                    resultArray2[i, 0] = resultArray[index];
                    resultArray2[i, 1] = resultArray[index+1];
                    index += 2;
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
            var dataType = GetDatatype(typeof(T));

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
            long datatype = H5T.create(H5T.class_t.STRING, H5T.VARIABLE);
            H5T.set_cset(datatype, H5T.cset_t.UTF8);
            H5T.set_strpad(datatype, H5T.str_t.NULLTERM);
            int strSz = dset.Length;
            long spaceId = H5S.create_simple(1, new[] { (ulong)strSz }, null);

            string normalizedName = datasetName;
            var datasetId = H5D.create(locationId, normalizedName, datatype, spaceId);
            
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
            var result = H5D.write(datasetId, datatype, H5S.ALL, H5S.ALL, H5P.DEFAULT, hnd.AddrOfPinnedObject());
            hnd.Free();

            for (int i = 0; i < strSz; ++i)
                hnds[i].Free();

            H5D.close(datasetId);
            H5S.close(spaceId);
            H5T.close(datatype);
            return datasetId;
        }

        /// <summary>
        /// Remove whole dataset
        /// </summary>
        /// <param name="locationId">Location id in which dataset is</param>
        /// <param name="datasetPath">Path with name to dataset</param>
        /// <returns>0 if successfull -1 if not</returns>
        public long RemoveDataset(long locationId, string datasetPath)
            => H5L.delete(locationId, datasetPath);

        /// <summary>
        /// Get hdf5 variable type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public long GetDatatype(Type type)
        {
            long dataType;

            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Byte: //Bool Char
                    dataType = H5T.NATIVE_UINT8;
                    break;
                case TypeCode.SByte: //Image
                    dataType = H5T.NATIVE_INT8;
                    break;
                case TypeCode.Int32: //Int
                    dataType = H5T.NATIVE_INT32;
                    break;
                case TypeCode.Double: //Float
                    dataType = H5T.NATIVE_DOUBLE;
                    break;
                case TypeCode.String:
                    dataType = H5T.NATIVE_UCHAR;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(type.Name, $"Data Type {type} not supported.");
            }
            return dataType;
        }
    }
}
