using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RocketToy
{
    public class MemoryEditor
    {
        [DllImport("kernel32.dll")]
        static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
            int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress,
            byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        private const int MEM_COMMIT = 0x00001000;
        private const int PAGE_READWRITE = 0x04;
        private const int PROCESS_QUERY_INFORMATION = 0x0400;
        private const int PROCESS_VM_READ = 0x0010;
        private const int PROCESS_VM_WRITE = 0x0020;
        private const int PROCESS_VM_OPERATION = 0x0008;

        private struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;
            public IntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }

        public struct MEMORY_BASIC_INFORMATION
        {
            public int BaseAddress;
            public int AllocationBase;
            public int AllocationProtect;
            public int RegionSize;
            public int State;
            public int Protect;
            public int lType;
        }

        private int hProcess;

        public MemoryEditor(Process Proc)
        {
            hProcess = (int)OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION, false, Proc.Id);
        }

        public byte[] ReadMemory(int Address, int Length)
        {
            byte[] result = new byte[Length];
            int bytesRead = 0;
            ReadProcessMemory(hProcess, Address, result, Length, ref bytesRead);
            return result;
        }

        public void WriteMemory(int Address, byte[] Data)
        {
            int bytesWritten = 0;
            WriteProcessMemory(hProcess, Address, Data, Data.Length, ref bytesWritten);
        }

        public int[] GetAddressesOf(byte[] Data)
        {
            SYSTEM_INFO sys_info = new SYSTEM_INFO();
            GetSystemInfo(out sys_info);

            IntPtr procMinAddress = sys_info.minimumApplicationAddress;
            IntPtr procMaxAddress = sys_info.maximumApplicationAddress;

            long procMinAddressL = (long)procMinAddress;
            long procMaxAddressL = (long)procMaxAddress;

            List<MEMORY_BASIC_INFORMATION> scannableChunks = getScannableChunks(procMinAddressL, procMaxAddressL);
            List<int> results = new List<int>();
            foreach (MEMORY_BASIC_INFORMATION chunk in scannableChunks)
            {
                byte[] chunkData = ReadMemory(chunk.BaseAddress, chunk.RegionSize);
                int streak = 0;
                for (int i = 0; i < chunkData.Length; i++)
                {
                    if (chunkData[i] == Data[streak])
                    {
                        streak++;
                        if (streak == Data.Length)
                        {
                            results.Add(chunk.BaseAddress + (i - (Data.Length - 1)));
                            streak = 0;
                        }
                    }
                    else
                    {
                        streak = 0;
                    }
                }
            }
            return results.ToArray();
        }

        private List<MEMORY_BASIC_INFORMATION> getScannableChunks(long procStart, long procMaxAddressL)
        {
            List<MEMORY_BASIC_INFORMATION> result = new List<MEMORY_BASIC_INFORMATION>();
            while (procStart < procMaxAddressL)
            {
                MEMORY_BASIC_INFORMATION temp = new MEMORY_BASIC_INFORMATION();
                VirtualQueryEx((IntPtr)hProcess, (IntPtr)procStart, out temp, 28);
                if (temp.Protect == PAGE_READWRITE && temp.State == MEM_COMMIT)
                {
                    result.Add(temp);
                }
                procStart += temp.RegionSize;
            }
            return result;
        }
    }
}
