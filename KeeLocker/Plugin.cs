using KeePass.Forms;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace KeeLocker
{
    using NTSTATUS = Int32;
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
	delegate int CallbackDelegate(
					ulong StateName,
					int ChangeStamp,
					IntPtr TypeId,
					IntPtr CallbackContext,
					IntPtr Buffer,
					int BufferSize);
    public class KeeLockerExt : KeePass.Plugins.Plugin
	{
        CallbackDelegate mCallBAck;
		int callback_count = 0;

        [DllImport("ntdll.dll")]
        public static extern NTSTATUS RtlSubscribeWnfStateChangeNotification(
			out IntPtr Subscription,
			ulong StateName,
			int ChangeStamp,
			IntPtr Callback,
			IntPtr CallbackContext,
			IntPtr TypeId,
			int SerializationGroup,
			int Unknown);

        private int NotifyCallback(
			ulong stateName,
			int nChangeStamp,
			IntPtr pTypeId,
			IntPtr pCallbackContext,
			IntPtr pBuffer,
			int nBufferSize)
        {
			if (callback_count++ == 0)
				return 0 ;
			if (pBuffer == IntPtr.Zero && nBufferSize == 0 && nChangeStamp == 0)
			{
			}
			else
			{
				if (m_host.Database != null)
				{
					if (!m_host.Database.IsOpen && KeePass.UI.GlobalWindowManager.WindowCount == 0)
					{
						m_host.MainWindow.BeginInvoke(new Action(() => 
						m_host.MainWindow.OpenDatabase(m_host.MainWindow.DocumentManager.ActiveDocument.LockedIoc, null, false)));
					}
					else
					{
						m_host.MainWindow.BeginInvoke(new Action(() =>
						{
							UnlockDatabase(m_host.MainWindow.ActiveDatabase, false);
						}));
					}
				}
			}
            return 0;

        }
        public NTSTATUS Listen()
		{
			NTSTATUS ntstatus = 0;
			IntPtr hEvent = IntPtr.Zero;
			IntPtr pContextBuffer = IntPtr.Zero;
			IntPtr pSubscription = IntPtr.Zero;
			mCallBAck = new CallbackDelegate(NotifyCallback);
			ntstatus = RtlSubscribeWnfStateChangeNotification(
				out pSubscription,
                0x4183182BA3BC3875UL,
				0,
                Marshal.GetFunctionPointerForDelegate(mCallBAck),
                IntPtr.Zero,
				IntPtr.Zero,
				0,
				0);
			return ntstatus;
		}

        public const string StringName_DriveMountPoint = "KeeLockerMountPoint";
		public const string StringName_DriveGUID = "KeeLockerGUID";
		public const string StringName_DriveIdType = "KeeLockerType";
		public const string StringName_UnlockOnOpening = "KeeLockerOnOpening";
		public const string StringName_IsRecoveryKey = "KeeLockerIsRecoveryKey";

		public const string StringName_Password = "Password";

		public enum EDriveIdType
		{
			MountPoint,
			GUID
		}
		internal KeePass.Plugins.IPluginHost m_host;

		public override string UpdateUrl
		{
			get
			{
				return "https://raw.github.com/Gugli/KeeLocker/main/VersionInfo.txt";
			}
		}

		public override bool Initialize(KeePass.Plugins.IPluginHost host)
		{
			if (host == null)
				return false;
			m_host = host;

			Listen();
			// Signed update checks
			KeePass.Util.UpdateCheckEx.SetFileSigKey(UpdateUrl, "<RSAKeyValue><Modulus>0N6jerZiraXQTGZ2kqbQHCOs1pjyFRmHwG6zVQwWQ5M0YONrT5nEJGBCOJ8gliJ+/ONerm8JfrB9eycsvq6cYNGC9WvGTVt81KDhnOlCSPdHkB3qtPU5Vin4UIFNjCmb0/Bnz7hyoVjACqNQUSeIWFSTPtNw2/H7EK+YZpGbdD540QxdRzZUWi50AxS1kCYUzvj1zYjuXBHw7YMP/GFQIuFBJrZUv1nQwVG1+j4u6aWe8wP5RXzm0LpdLtc9JeoVfP1DBujuugKxpOXXDzB+YPI5RIIAOEc3qd4BNZkLOU3JEdGu/MCWL7GgHQOlGjR+jWpKGGkUWFplkCA7YRtKAlRQRQY3Id9wKjinhTyhhZ7r9qkHK8m2dCVaL8F2dXj8KTSZZWIZHV56a6Kou2Kw0Vq9ra6Wt6uZH1lLX3h05ygDe3Gm5rxax150ScjQHBhHxTo03xzaif5AP1zW0eCeCDfH37dPjZBUQb/zEy0pqbKATwMAFdMLWKCS5hy+a5L5xhd+WIf0OW6AgapA4O/xFABucSFVh9Ugpzvy9j5Gb4+9+aygGlnktprZDBAI5t9QEZz8Vkjxv+nKplPPH37f01K7mIzSjsxnGmcBM4CFVPjfG0i9eAa+4pVqFgXaW3TNQjWON8sMrslCqaFB+0s79MuJbps2awevB+hyssCOacE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

			// register FileOpened event : needed to open locked storages
			m_host.MainWindow.FileOpened += OnKPDBOpen;

			// register WindowAdded event : needed to add the options tab in the EditEntry window
			KeePass.UI.GlobalWindowManager.WindowAdded += OnWindowAdded;
			
			return true;
		}

		public override void Terminate()
		{
			// remove event listeners
			KeePass.UI.GlobalWindowManager.WindowAdded -= OnWindowAdded;

			m_host.MainWindow.FileOpened -= OnKPDBOpen;
		}

		public override System.Windows.Forms.ToolStripMenuItem GetMenuItem(KeePass.Plugins.PluginMenuType t)
		{
			if (t == KeePass.Plugins.PluginMenuType.Main)
			{
				System.Windows.Forms.ToolStripMenuItem UnlockThisDB = new System.Windows.Forms.ToolStripMenuItem();
				UnlockThisDB.Text = "Keelocker unlock volumes in this DB";
				UnlockThisDB.Click += this.UnlockThisDB;
				UnlockThisDB.Paint += delegate (object sender, System.Windows.Forms.PaintEventArgs e)
				{
					bool DBIsOpen = ((m_host.MainWindow.ActiveDatabase != null) && m_host.MainWindow.ActiveDatabase.IsOpen);
					UnlockThisDB.Enabled = DBIsOpen;
				};
				return UnlockThisDB;
				/*
				System.Windows.Forms.ToolStripMenuItem UnlockAll = new System.Windows.Forms.ToolStripMenuItem();
				UnlockAll.Text = "Unlock volumes in all DBs";
				UnlockAll.Click += this.UnlockAll;

				System.Windows.Forms.ToolStripMenuItem Keelocker = new System.Windows.Forms.ToolStripMenuItem("Keelocker");
				Keelocker.DropDownItems.Add(UnlockThisDB);
				Keelocker.DropDownItems.Add(UnlockAll);

				return Keelocker;
				*/
			}
			else if (t == KeePass.Plugins.PluginMenuType.Entry)
			{
				System.Windows.Forms.ToolStripMenuItem UnlockEntry = new System.Windows.Forms.ToolStripMenuItem();
				UnlockEntry.Click += this.UnlockEntries;
				UnlockEntry.Paint += delegate (object sender, System.Windows.Forms.PaintEventArgs e)
				{
					KeePassLib.PwEntry[] Entries = m_host.MainWindow.GetSelectedEntries();
					int SelectedCount = Entries == null ? 0 : Entries.Length;
					UnlockEntry.Enabled = SelectedCount > 0;
					UnlockEntry.Text = SelectedCount > 1 ? "Keelocker unlock volumes" : "Keelocker unlock volume";
				};
				return UnlockEntry;
			}
			else if (t == KeePass.Plugins.PluginMenuType.Group)
			{
				System.Windows.Forms.ToolStripMenuItem UnlockGroup = new System.Windows.Forms.ToolStripMenuItem();
				UnlockGroup.Text = "Keelocker unlock volumes in this group";
				UnlockGroup.Click += this.UnlockGroup;
				UnlockGroup.Paint += delegate (object sender, System.Windows.Forms.PaintEventArgs e)
				{
					UnlockGroup.Enabled = m_host.MainWindow.GetSelectedGroup() != null;
				};
				return UnlockGroup;
			}
			else
			{
				return null;
			}
		}

		private void OnWindowAdded(object sender, KeePass.UI.GwmWindowEventArgs e)
		{
			if (e.Form.Name == "PwEntryForm")
			{
				var ef = e.Form as KeePass.Forms.PwEntryForm;
				if (ef != null)
				{
					ef.Shown += OnEntryFormShown;
					return;
				}
			}
		}

		private Type GetControl<Type>(KeePass.Forms.PwEntryForm Form, string Name) where Type: System.Windows.Forms.Control
		{
			System.Windows.Forms.Control[] Controls = Form.Controls.Find(Name, true);
			if (Controls.Length == 0)
				return default(Type);
			
			return Controls[0] as Type;
		}

		void OnEntryFormShown(object sender, EventArgs e)
		{
			KeePass.Forms.PwEntryForm Form = sender as KeePass.Forms.PwEntryForm;
			if (Form == null)
				return;

			KeePassLib.PwEntry Entry = Form.EntryRef;
			if (Entry == null)
				return;

			KeePassLib.Collections.ProtectedStringDictionary strings = Form.EntryStrings;
			if (strings == null)
				return;

			System.Windows.Forms.TabControl tabMain = GetControl<System.Windows.Forms.TabControl>(Form, "m_tabMain");
			System.Windows.Forms.Button btnOk = GetControl<System.Windows.Forms.Button>(Form, "m_btnOK");

			KeeLocker.Forms.KeeLockerEntryTab KeeLockerEntryTab = new KeeLocker.Forms.KeeLockerEntryTab(m_host, this, Entry, strings);

			System.Windows.Forms.TabPage KeeLockerEntryTabContainer = new System.Windows.Forms.TabPage("KeeLocker");
			KeeLockerEntryTabContainer.Controls.Add(KeeLockerEntryTab);
			KeeLockerEntryTab.Dock = System.Windows.Forms.DockStyle.Fill;
			tabMain.TabPages.Add(KeeLockerEntryTabContainer);

			btnOk.Click += KeeLockerEntryTab.OnSave;
		}

		public bool TryUnlockVolume(string DriveMountPoint, string DriveGUID, string Password, bool IsRecoveryKey)
		{
			string DriveMountPointString;
			string DriveGUIDString;
			if (DriveGUID.Length > 0)
			{
				DriveMountPointString = "";
				DriveGUIDString = DriveGUID;
			}
			else if (DriveMountPoint.Length > 0)
			{
				DriveMountPointString = DriveMountPoint;
				DriveGUIDString = "";
			}
			else 
			{
				return false;
			}


			try
			{
				FveApi.UnlockVolume(DriveMountPointString, DriveGUIDString, Password, IsRecoveryKey);
			}
			catch (Exception Ex)
			{
				string Messages = Ex.ToString();
				return false;
			}
			return true;
		}
			
		private void OnKPDBOpen(object sender, KeePass.Forms.FileOpenedEventArgs e)
		{
			const bool IsDatabaseOpening = true;
			UnlockDatabase(e.Database, IsDatabaseOpening);
		}

		private void UnlockAll(object sender, EventArgs e)
		{
			/*
			foreach (KeePassLib.PwDatabase DB in Entries)
			{
				UnlockDatabase(DB);
			}*/
		}

		private void UnlockThisDB(object sender, EventArgs e)
		{
			UnlockDatabase(m_host.MainWindow.ActiveDatabase, false);
		}

		private void UnlockGroup(object sender, EventArgs e)
		{
			UnlockGroup(m_host.MainWindow.GetSelectedGroup(), false);
		}

		private void UnlockEntries(object sender, EventArgs e)
		{
			KeePassLib.PwEntry[] Entries = m_host.MainWindow.GetSelectedEntries();
			if (Entries == null) return;
			foreach (KeePassLib.PwEntry Entry in Entries)
			{
				UnlockEntry(Entry, false);
			}
		}

		private void UnlockDatabase(KeePassLib.PwDatabase Database, bool IsDatabaseOpening)
		{
			UnlockGroup(Database.RootGroup, IsDatabaseOpening);
		}

		private void UnlockGroup(KeePassLib.PwGroup Group, bool IsDatabaseOpening)
		{
			if (Group == null) return;
			KeePassLib.Collections.PwObjectList<KeePassLib.PwEntry> AllEntries = Group.GetEntries(true);
			foreach (KeePassLib.PwEntry Entry in AllEntries)
			{
				UnlockEntry(Entry, IsDatabaseOpening);
			}
		}

		private void UnlockEntry(KeePassLib.PwEntry Entry, bool IsDatabaseOpening)
		{
			if (Entry == null) return;
			KeePassLib.Collections.ProtectedStringDictionary Strings = Entry.Strings;
			KeePassLib.Security.ProtectedString DriveIdTypeStr = Strings.Get(StringName_DriveIdType);
			KeePassLib.Security.ProtectedString DriveMountPoint = Strings.Get(StringName_DriveMountPoint);
			KeePassLib.Security.ProtectedString DriveGUID = Strings.Get(StringName_DriveGUID);
			KeePassLib.Security.ProtectedString UnlockOnOpening = Strings.Get(StringName_UnlockOnOpening);
			KeePassLib.Security.ProtectedString IsRecoveryKey = Strings.Get(StringName_IsRecoveryKey);
			KeePassLib.Security.ProtectedString Password = Strings.Get(StringName_Password);
			bool UnlockOnOpening_bool = Forms.KeeLockerEntryTab.GetUnlockOnOpeningFromString(UnlockOnOpening);
			bool IsRecoveryKey_bool = Forms.KeeLockerEntryTab.GetIsRecoveryKeyFromString(IsRecoveryKey);

			if (Password == null)
				return;

			if (IsDatabaseOpening && !UnlockOnOpening_bool)
				return;

			EDriveIdType DriveIdType = Forms.KeeLockerEntryTab.GetDriveIdTypeFromString(DriveIdTypeStr);

			TryUnlockVolume(
				DriveIdType == EDriveIdType.MountPoint && DriveMountPoint != null ? DriveMountPoint.ReadString() : "",
				DriveIdType == EDriveIdType.GUID && DriveGUID != null ? DriveGUID.ReadString() : "",
				Password.ReadString(),
				IsRecoveryKey_bool
				);
		}
	}
}
