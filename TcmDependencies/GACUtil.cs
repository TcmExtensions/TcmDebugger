using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TcmDependencies
{
	public static class GACUtil
	{
		[DllImport("fusion.dll")]
		private static extern IntPtr CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);

		[ComImport]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
		private interface IAssemblyCache
		{
			int Dummy1();

			[PreserveSig()]
			IntPtr QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] String assemblyName, ref AssemblyInfo assemblyInfo);

			int Dummy2();
			int Dummy3();
			int Dummy4();
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct AssemblyInfo
		{
			public int cbAssemblyInfo;
			public int assemblyFlags;
			public long assemblySizeInKB;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string currentAssemblyPath;

			public int cchBuf;
		}

		public static String GetGACAssembly(String assemblyName)
		{
			AssemblyInfo assemblyInfo = new AssemblyInfo { cchBuf = 512 };
			assemblyInfo.currentAssemblyPath = new String('\0', assemblyInfo.cchBuf);

			IAssemblyCache assemblyCache;

			IntPtr hr = CreateAssemblyCache(out assemblyCache, 0);

			if (hr == IntPtr.Zero)
			{
				hr = assemblyCache.QueryAssemblyInfo(1, assemblyName, ref assemblyInfo);

				if (hr != IntPtr.Zero)
					return String.Empty;

				return assemblyInfo.currentAssemblyPath;
			}

			Marshal.ThrowExceptionForHR(hr.ToInt32());
			return String.Empty;
		}
	}
}
