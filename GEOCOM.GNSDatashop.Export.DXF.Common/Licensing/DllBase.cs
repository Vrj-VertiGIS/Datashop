using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.Licensing
{
	public abstract class DllBase : IDisposable
	{
		// Infrastructure methods
		[DllImport("kernel32")]
        [SuppressUnmanagedCodeSecurity]
		private extern static IntPtr LoadLibrary(string librayName);
		[DllImport("kernel32")]
        [SuppressUnmanagedCodeSecurity]
		private extern static bool FreeLibrary(IntPtr hModule);
		[DllImport("kernel32", CharSet = CharSet.Ansi )]
        [SuppressUnmanagedCodeSecurity]
		private extern static IntPtr GetProcAddress(IntPtr hModule, string procedureName);
		[DllImport("kernel32", CharSet = CharSet.Ansi , EntryPoint =  "GetProcAddress")]
        [SuppressUnmanagedCodeSecurity]
		private extern static IntPtr GetProcAddressOrd(IntPtr hModule, uint procedureOrdinal);

		protected IntPtr _hDll;

		private String _dllPath;
		protected bool _disposed = false;

		private Dictionary<String, System.Delegate> _loadedSyms;

		/// <summary>
		/// Attach to library given (full path) by dllPath
		/// Throws Exception upon failure to load library
		/// </summary>
		/// <param name="dllPath">Full path of library to attach to</param>
		public void Load(String dllPath)
		{
			_loadedSyms = new Dictionary<string, Delegate>();   
			// If this were already existing it would not be valid anymore

			_dllPath = dllPath;
			_hDll = LoadLibrary(dllPath);
			if (_hDll == IntPtr.Zero)
				throw new Exception("Error loading '" + _dllPath);
		}

		/// <summary>
		/// Find procedure entry point in loaded dll's address space
		/// IntPtr.Zero is returned if entry not found
		/// N.B. This algorithm caches the already loaded symbols but
		/// it might be faster to keep a reference to a frequently used
		/// dll entry by the user class itself.
		/// </summary>
		/// <param name="procName">Name of the dll extern (procedure) to attach to</param>
		/// <returns></returns>
		public System.Delegate GetProc<T>(String procName)
		{
			System.Delegate pDelegate;
			if (_loadedSyms.TryGetValue(procName, out pDelegate))
				return pDelegate;
			else
			{
				IntPtr pAddress = GetProcAddress(_hDll, procName);
				if (pAddress != IntPtr.Zero)
				{
					pDelegate = Marshal.GetDelegateForFunctionPointer(pAddress, typeof(T));
					_loadedSyms[procName] = pDelegate;
					return pDelegate;
				}
				else
					throw new NotImplementedException("Function '" + procName + "' not implemented in GeoLmClient.dll");
			}
		}

        /// <summary>
        /// Calling dll entry point procId - returning delegate or throwing exception if procId nonexistent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procId"></param>
        /// <returns></returns>
        public System.Delegate GetProc<T>(uint procId)
        {
            System.Delegate pDelegate;
            if (_loadedSyms.TryGetValue(procId.ToString(), out pDelegate))
                return pDelegate;
            else
            {
                IntPtr pAddress = GetProcAddressOrd(_hDll, procId);
                if (pAddress != IntPtr.Zero)
                {
                    pDelegate = Marshal.GetDelegateForFunctionPointer(pAddress, typeof(T));
                    _loadedSyms[procId.ToString()] = pDelegate;
                    return pDelegate;
                }
                else
                    throw new NotImplementedException($"Function '@{procId.ToString()}' not implemented in GeoLmClient.dll");
            }
        }

		/// <summary>
		/// Release unmanaged ressources when GC cleans up
		/// </summary>
		~DllBase()
		{
			Dispose();
		}
    
		#region IDisposable Members
		/// <summary>
		/// Release unmanaged ressources upon IDIsposable.Dispose() invocation
		/// </summary>
		public virtual void Dispose()
		{
			if (!_disposed)
			{
				if (_hDll != IntPtr.Zero)
					FreeLibrary(_hDll);
				_disposed = true;
			}
		}
		#endregion
	}
}