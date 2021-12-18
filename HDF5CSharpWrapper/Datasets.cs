using HDF.PInvoke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// <param name="fileId">File id from which array will be created</param>
        /// <param name="pathToDataset">Path to dataset</param>
        /// <returns>Result Array or -1 if failed</returns>
        public Array GetDataset<T>(long fileId, string pathToDataset)
        {
            // TODO
            /*if (typeof(T) == typeof(string)) //String
                return GetStringDataset(fileID, pathToDataset);*/

            var dataType = GetDatatype(typeof(T));
            var datasetId = H5D.open(fileId, pathToDataset);
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

        public string GetStringDataset(long groupId, string name)
        {
            var datasetId = H5D.open(groupId, (name));
            var typeId = H5D.get_type(datasetId);

            if (H5T.is_variable_str(typeId) > 0)
            {
                var spaceId = H5D.get_space(datasetId);
                long count = H5S.get_simple_extent_npoints(spaceId);

                IntPtr[] rdata = new IntPtr[count];

                GCHandle hnd = GCHandle.Alloc(rdata, GCHandleType.Pinned);
                H5D.read(datasetId, typeId, H5S.ALL, H5S.ALL,
                    H5P.DEFAULT, hnd.AddrOfPinnedObject());

                var attrStrings = new List<string>();
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

                    attrStrings.Add(stringPart);

                    H5.free_memory(rdata[i]);
                }

                hnd.Free();
                H5S.close(spaceId);
                H5D.close(datasetId);

                return attrStrings[0];
            }



            // Must be a non-variable length string.
            int size = H5T.get_size(typeId).ToInt32();
            IntPtr iPtr = Marshal.AllocHGlobal(size);

            int result = H5D.read(datasetId, typeId, H5S.ALL, H5S.ALL,
                H5P.DEFAULT, iPtr);
            if (result < 0)
            {
                throw new IOException("Failed to read dataset");
            }

            var strDest = new byte[size];
            Marshal.Copy(iPtr, strDest, 0, size);
            Marshal.FreeHGlobal(iPtr);

            H5D.close(datasetId);

            return Encoding.UTF8.GetString(strDest).TrimEnd((char)0);
        }

        /// <summary>
        /// Write specified Array to file as dataset
        /// </summary>
        /// <typeparam name="T">Generic type parameter</typeparam>
        /// <param name="fileId">File id in which dataset will be created</param>
        /// <param name="datasetName">Name of a dataset</param>
        /// <param name="dset">Array to be written</param>
        /// <returns>Dataset id or -1 if failed</returns>
        /// 
        public long SetDataset<T>(long fileId, string datasetName, Array dset)
        {
            ulong[] dims = Enumerable.Range(0, dset.Rank).Select(i => { return (ulong)dset.GetLength(i); }).ToArray();
            var spaceId = H5S.create_simple(dset.Rank, dims, null);
            var dataType = GetDatatype(typeof(T));

            if (dataType == H5T.C_S1)
                H5T.set_size(dataType, new IntPtr(2));

            var datasetId = H5D.create(fileId, datasetName, dataType, spaceId);

            GCHandle hnd = GCHandle.Alloc(dset, GCHandleType.Pinned);
            var result = H5D.write(datasetId, dataType, H5S.ALL, H5S.ALL, H5P.DEFAULT, hnd.AddrOfPinnedObject());
            hnd.Free();
            H5D.close(datasetId);
            H5S.close(spaceId);
            return datasetId;
        }

        /// <summary>
        /// Remove whole dataset
        /// </summary>
        /// <param name="fileId">File id in which dataset is</param>
        /// <param name="datasetPath">Path with name to dataset</param>
        /// <returns>0 if successfull -1 if not</returns>
        public long RemoveDataset(long fileId, string datasetPath)
            => H5L.delete(fileId, datasetPath);

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
