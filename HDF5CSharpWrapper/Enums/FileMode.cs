using HDF.PInvoke;

namespace HDF5CSharpWrapper
{
    /// <summary>
    /// The way in which file can be opened/created
    /// </summary>
    public enum FileMode
    {
        /// <summary>
        /// Specifies that the file is in read mode but will not be allowed to write any data.
        /// </summary>
        ReadOnly = (int)H5F.ACC_RDONLY,

        /// <summary>
        /// Specifies that the file is in read and write mode.
        /// </summary>
        ReadWrite = (int)H5F.ACC_RDWR,

        /// <summary>
        /// Specifies that if the file already exists, the current contents will be deleted
        /// so that the application can rewrite the file with new data.
        /// </summary>
        WriteNew = (int)H5F.ACC_TRUNC,

        /// <summary>
        /// Specifies that the open will fail if the file already exists. 
        /// If the file does not already exist, the file is created with read and write mode.
        /// </summary>
        WriteIfNew = (int)H5F.ACC_EXCL
    }
}