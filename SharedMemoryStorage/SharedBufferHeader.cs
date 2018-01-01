using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace SharedMemoryStorage
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SharedBufferHeader
    {
        /// <summary>
        /// The total size of the buffer including <see cref="SharedHeader"/>, i.e. <code>BufferSize + Marshal.SizeOf(typeof(SharedMemory.SharedHeader))</code>.
        /// </summary>
        public long SharedMemorySize;

        /// <summary>
        /// Flag indicating whether the owner of the buffer has closed its 
        /// <see cref="System.IO.MemoryMappedFiles.MemoryMappedFile"/> and <see cref="System.IO.MemoryMappedFiles.MemoryMappedViewAccessor"/>.
        /// </summary>
        public volatile int Shutdown;


        public ObjectTable* ObjectTable;

        public long ObjectsStart;
        public long ObjectsCurrentWrite;
        public long ObjectsLength;
        /// <summary>
        /// Pad to 16-bytes.
        /// </summary>
        int _padding0;

    }
}
