using System.Runtime.InteropServices;

namespace SharedMemoryStorage
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectTable
    {
        public long ObjectTableSize()
        {
            return Marshal.SizeOf(typeof(ObjectTable));
        }
    }
}
