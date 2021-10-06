using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;

namespace BananaModManager.Loader.IL2Cpp
{
    internal class UnhollowerDetour : IManagedDetour
    {
        [DllImport("BananaModManager.Detours.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void hook_attach(IntPtr from, IntPtr to);

        public unsafe T Detour<T>(IntPtr from, T to) where T : Delegate
        {
            var fromPtr = &from;
            var fromDel = Marshal.GetDelegateForFunctionPointer<T>(from);
            var toPtr = Marshal.GetFunctionPointerForDelegate(to);

            hook_attach((IntPtr) fromPtr, toPtr);

            return fromDel;
        }
    }
}
