using HDF.PInvoke;
using System;

namespace HDF5CSharpWrapper
{
    public class DataTypeHelpers
    {
        /// <summary>
        /// Get hdf5 variable type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long GetDataType(Type type)
        {
            long dataType;

            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Byte: //Bool Char
                case TypeCode.SByte: //Image
                    dataType = H5T.NATIVE_UINT8;
                    break;
                case TypeCode.Int32: //Int
                    dataType = H5T.NATIVE_INT32;
                    break;
                case TypeCode.Double: //Float
                    dataType = H5T.NATIVE_DOUBLE;
                    break;
                case TypeCode.Single:
                    dataType = H5T.NATIVE_FLOAT;
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
