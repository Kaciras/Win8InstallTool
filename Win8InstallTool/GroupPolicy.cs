using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace Win8InstallTool
{
	[ComImport, Guid("EA502722-A23D-11d1-A7D3-0000F87571E3")]
	internal class GPClass { }

	[ComImport, Guid("EA502723-A23D-11d1-A7D3-0000F87571E3")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IGroupPolicyObject
	{
		uint New([MarshalAs(UnmanagedType.LPWStr)] string domainName, [MarshalAs(UnmanagedType.LPWStr)] string displayName, uint flags);

		uint OpenDSGPO([MarshalAs(UnmanagedType.LPWStr)] string path, uint flags);

		uint OpenLocalMachineGPO(uint flags);

		uint OpenRemoteMachineGPO([MarshalAs(UnmanagedType.LPWStr)] string computerName, uint flags);

		uint Save([MarshalAs(UnmanagedType.Bool)] bool machine, [MarshalAs(UnmanagedType.Bool)] bool add, [MarshalAs(UnmanagedType.LPStruct)] Guid extension, [MarshalAs(UnmanagedType.LPStruct)] Guid app);

		uint Delete();

		uint GetName([MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, int maxLength);

		uint GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, int maxLength);

		uint SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name);

		uint GetPath([MarshalAs(UnmanagedType.LPWStr)] StringBuilder path, int maxPath);

		uint GetDSPath(uint section, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path, int maxPath);

		uint GetFileSysPath(uint section, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path, int maxPath);

		uint GetRegistryKey(uint section, out IntPtr key);

		uint GetOptions(out uint options);

		uint SetOptions(uint options, uint mask);

		uint GetType(out IntPtr gpoType);

		uint GetMachineName([MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, int maxLength);

		uint GetPropertySheetPages(out IntPtr pages);
	}


	public enum GroupPolicySection
	{
		Root = 0,
		User = 1,
		Machine = 2,
	}

	public abstract class GroupPolicyObject
	{
		protected const int MaxLength = 1024;

		/// <summary>
		/// The snap-in that processes .pol files
		/// </summary>
		private static readonly Guid RegistryExtension = Guid.Parse("{35378eac-683f-11d2-a89a-00c04fbbcfa2}");

		/// <summary>
		/// This application
		/// </summary>
		private static readonly Guid LocalGuid = new Guid(GetAssemblyAttribute<GuidAttribute>(Assembly.GetExecutingAssembly()).Value);

		protected IGroupPolicyObject Instance = null;

		static T GetAssemblyAttribute<T>(ICustomAttributeProvider assembly) where T : Attribute
		{
			object[] attributes = assembly.GetCustomAttributes(typeof(T), true);
			if (attributes.Length == 0)
				return null;

			return (T)attributes[0];
		}

		internal GroupPolicyObject()
		{
			Instance = (IGroupPolicyObject)new GPClass();
		}

		public void Save()
		{
			var result = Instance.Save(true, true, RegistryExtension, LocalGuid);
			if (result != 0)
			{
				throw new Exception("Error saving machine settings");
			}

			result = Instance.Save(false, true, RegistryExtension, LocalGuid);
			if (result != 0)
			{
				throw new Exception("Error saving user settings");
			}
		}

		public void Delete()
		{
			var result = Instance.Delete();
			if (result != 0)
			{
				throw new Exception("Error deleting the GPO");
			}
			Instance = null;
		}

		public RegistryKey GetRootRegistryKey(GroupPolicySection section)
		{
			var result = Instance.GetRegistryKey((uint)section, out IntPtr key);
			if (result != 0)
			{
				var name = Enum.GetName(typeof(GroupPolicySection), section);
				throw new Exception("Unable to get section: " + name);
			}

			var handle = new SafeRegistryHandle(key, true);
			return RegistryKey.FromHandle(handle, RegistryView.Default);
		}

		public abstract string GetPathTo(GroupPolicySection section);
	}

	public class GroupPolicyObjectSettings
	{
		public readonly bool LoadRegistryInformation;
		public readonly bool Readonly;

		public GroupPolicyObjectSettings(bool loadRegistryInfo = true, bool readOnly = false)
		{
			LoadRegistryInformation = loadRegistryInfo;
			Readonly = readOnly;
		}

		private const uint RegistryFlag = 0x00000001;
		private const uint ReadonlyFlag = 0x00000002;

		internal uint Flag
		{
			get
			{
				uint flag = 0x00000000;
				if (LoadRegistryInformation)
				{
					flag |= RegistryFlag;
				}

				if (Readonly)
				{
					flag |= ReadonlyFlag;
				}

				return flag;
			}
		}
	}

	public class ComputerGroupPolicyObject : GroupPolicyObject
	{
		public readonly bool IsLocal;

		public ComputerGroupPolicyObject(GroupPolicyObjectSettings options = null)
		{
			options ??= new GroupPolicyObjectSettings();
			var result = Instance.OpenLocalMachineGPO(options.Flag);
			if (result != 0)
			{
				throw new Exception("Unable to open local machine GPO");
			}
			IsLocal = true;
		}

		public ComputerGroupPolicyObject(string computerName, GroupPolicyObjectSettings options = null)
		{
			options ??= new GroupPolicyObjectSettings();
			var result = Instance.OpenRemoteMachineGPO(computerName, options.Flag);
			if (result != 0)
			{
				throw new Exception("Unable to open GPO on remote machine: " + computerName);
			}
			IsLocal = false;
		}

		public static void SetPolicySetting(string key, string item, object value, RegistryValueKind kind)
		{
			RunOnSTAThread(() =>
			{
				var gpo = new ComputerGroupPolicyObject();
				var section = Key(key, out string subkey);

				using (var rootRegistryKey = gpo.GetRootRegistryKey(section))
				{
					// Data can't be null so we can use this value to indicate key must be delete
					if (value == null)
					{
						using var subKey = rootRegistryKey.OpenSubKey(subkey, true);
						if (subKey != null)
						{
							subKey.DeleteValue(item);
						}
					}
					else
					{
						using var subKey = rootRegistryKey.CreateSubKey(subkey);
						subKey.SetValue(item, value, kind);
					}
				}

				gpo.Save();
			});
		}

		public static object GetPolicySetting(string key, string item)
		{
			return RunOnSTAThread(() =>
			{
				var gpo = new ComputerGroupPolicyObject();
				var section = Key(key, out string subkey);

				using var root = gpo.GetRootRegistryKey(section);
				using var subKey = root.OpenSubKey(subkey, true);
				return subKey?.GetValue(item);
			});
		}

		internal static R RunOnSTAThread<R>(Func<R> function)
		{
			R returnValue = default;
			var thread = new Thread(() => returnValue = function());
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
			return returnValue;
		}

		internal static void RunOnSTAThread(ThreadStart action)
		{
			var thread = new Thread(action);
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
		}

		private static GroupPolicySection Key(string path, out string subkey)
		{
			var i = path.IndexOf('\\');
			var hive = path.Substring(0, i);
			subkey = path.Substring(i + 1);

			return hive.ToUpper() switch
			{
				"HKEY_LOCAL_MACHINE" or "HKLM" => GroupPolicySection.Machine,
				"HKEY_CURRENT_USER" or "HKCU" => GroupPolicySection.User,
				_ => throw new Exception($"错误的注册表 Root key: {hive}"),
			};
		}

		/// <summary>
		///     Retrieves the file system path to the root of the specified GPO section.
		///     The path is in UNC format.
		/// </summary>
		public override string GetPathTo(GroupPolicySection section)
		{
			var sb = new StringBuilder(MaxLength);
			var result = Instance.GetFileSysPath((uint)section, sb, MaxLength);
			if (result != 0)
			{
				var name = Enum.GetName(typeof(GroupPolicySection), section);
				throw new Exception("Unable to retrieve path to section: "+ name);
			}

			return sb.ToString();
		}
	}
}
