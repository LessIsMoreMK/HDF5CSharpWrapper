using HDF.PInvoke;

namespace HDF5CSharpWrapper
{
    /// <summary>
    /// Responible for basic file operations
    /// </summary>
    public class File
    {
        /// <summary>
        /// Opens a HDF5 file
        /// </summary>
        /// <param name="filename">A filename to be opened</param>
        /// <param name="fileMode">A mode in which file is opened ReadWrite/ReadOnly</param>
        /// <returns>File ID</returns>
        public long Open(string filename, FileMode fileMode = FileMode.ReadWrite)
            => H5F.open(filename, (uint)fileMode);

        /// <summary>
        /// Creates a HDF5 file
        /// </summary>
        /// <param name="filename">A filename to be created</param>
        /// <param name="fileMode">A mode in which file is created WriteNew/WriteIfNew</param>
        /// <returns>File ID</returns>
        public long Create(string filename, FileMode fileMode = FileMode.WriteNew)
            => H5F.create(filename, (uint)fileMode);

        /// <summary>
        /// Close a HDF5 file
        /// </summary>
        /// <param name="fileId">File ID to be closed</param>
        /// <returns>0 if successful, -1 if not</returns>
        public long Close(long fileId)
            => H5F.close(fileId);
    }
}
