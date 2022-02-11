using HDF.PInvoke;
using System;

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
        public long OpenFile(string filename, FileMode fileMode = FileMode.ReadWrite)
        {
            var stackid = H5E.get_current_stack();
            H5E.auto_t auto_cb = ErrorDelegateMethod;
            H5E.set_auto(H5E.DEFAULT, auto_cb, IntPtr.Zero);

            return H5F.open(filename, (uint)fileMode);
        }

        /// <summary>
        /// Creates a HDF5 file
        /// </summary>
        /// <param name="filename">A filename to be created</param>
        /// <param name="fileMode">A mode in which file is created WriteNew/WriteIfNew</param>
        /// <returns>File ID</returns>
        public long CreateFile(string filename, FileMode fileMode = FileMode.WriteNew)
        {
            var stackid = H5E.get_current_stack();
            H5E.auto_t auto_cb = ErrorDelegateMethod;
            H5E.set_auto(H5E.DEFAULT, auto_cb, IntPtr.Zero);

            return H5F.create(filename, (uint)fileMode);
        }

        /// <summary>
        /// Close a HDF5 file
        /// </summary>
        /// <param name="fileId">File ID to be closed</param>
        /// <returns>0 if successful, -1 if not</returns>
        public long CloseFile(long fileId)
            => H5F.close(fileId);


        private int CustomErrorMessage(uint n, ref H5E.error_t err_desc, IntPtr client_data)
        {
            //Console.WriteLine("Custom error list*");
            return 0;
        }
        private int ErrorDelegateMethod(long estack, IntPtr client_data)
        {
            H5E.walk(H5E.DEFAULT, H5E.direction_t.H5E_WALK_UPWARD, CustomErrorMessage, IntPtr.Zero);
            return -1;
        }
    }
}
