using System;

namespace KeeLocker
{
	public class KeeLockerExt : KeePass.Plugins.Plugin
	{
		public const string StringName_DriveMountPoint = "KeeLockerMountPoint";
		public const string StringName_DriveGUID = "KeeLockerGUID";
		public const string StringName_DriveIdType = "KeeLockerType";

		public const string StringName_UnlockOnOpening = "KeeLockerOnOpening";

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
		/*
		private void SignVersionFile()
		{
			string VersionInfo = "KeeLocker:1.0\n";
			byte[] VersionInfoBytes = System.Text.Encoding.UTF8.GetBytes(VersionInfo);

			string PrivateKey = System.IO.File.ReadAllText(@"C:\Path\To\Private\Key.xml");

			System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
			RSA.FromXmlString(PrivateKey);
			RSA.PersistKeyInCsp = false;

			System.Security.Cryptography.SHA512 SHA = new System.Security.Cryptography.SHA512Managed();
			byte[] Signature = RSA.SignData(VersionInfoBytes, SHA);
			string SignatureB64 = Convert.ToBase64String(Signature);

			System.Diagnostics.Debug.WriteLine("Signature: " + SignatureB64);

			// Check 
			{
				if (!RSA.VerifyData(VersionInfoBytes, SHA, Signature))
				{
					System.Diagnostics.Debug.Assert(false);
				}
			}
		}
		*/

		public override bool Initialize(KeePass.Plugins.IPluginHost host)
		{
			if (host == null)
				return false;
			m_host = host;

			// Signed update checks
			KeePass.Util.UpdateCheckEx.SetFileSigKey(UpdateUrl, "<RSAKeyValue><Modulus>0N6jerZiraXQTGZ2kqbQHCOs1pjyFRmHwG6zVQwWQ5M0YONrT5nEJGBCOJ8gliJ+/ONerm8JfrB9eycsvq6cYNGC9WvGTVt81KDhnOlCSPdHkB3qtPU5Vin4UIFNjCmb0/Bnz7hyoVjACqNQUSeIWFSTPtNw2/H7EK+YZpGbdD540QxdRzZUWi50AxS1kCYUzvj1zYjuXBHw7YMP/GFQIuFBJrZUv1nQwVG1+j4u6aWe8wP5RXzm0LpdLtc9JeoVfP1DBujuugKxpOXXDzB+YPI5RIIAOEc3qd4BNZkLOU3JEdGu/MCWL7GgHQOlGjR+jWpKGGkUWFplkCA7YRtKAlRQRQY3Id9wKjinhTyhhZ7r9qkHK8m2dCVaL8F2dXj8KTSZZWIZHV56a6Kou2Kw0Vq9ra6Wt6uZH1lLX3h05ygDe3Gm5rxax150ScjQHBhHxTo03xzaif5AP1zW0eCeCDfH37dPjZBUQb/zEy0pqbKATwMAFdMLWKCS5hy+a5L5xhd+WIf0OW6AgapA4O/xFABucSFVh9Ugpzvy9j5Gb4+9+aygGlnktprZDBAI5t9QEZz8Vkjxv+nKplPPH37f01K7mIzSjsxnGmcBM4CFVPjfG0i9eAa+4pVqFgXaW3TNQjWON8sMrslCqaFB+0s79MuJbps2awevB+hyssCOacE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

			// register FileOpened event : needed to open locked storages
			m_host.MainWindow.FileOpened += OnKPDBOpen;

			// register WindowAdded event : needed to add the options tab in the EditEntry window
			KeePass.UI.GlobalWindowManager.WindowAdded += OnWindowAdded;
			
			//SignVersionFile();

			return true;
		}

		public override void Terminate()
		{
			// remove event listeners
			KeePass.UI.GlobalWindowManager.WindowAdded -= OnWindowAdded;

			m_host.MainWindow.FileOpened -= OnKPDBOpen;
		}

		private void OnWindowAdded(object sender, KeePass.UI.GwmWindowEventArgs e)
		{
			var ef = e.Form as KeePass.Forms.PwEntryForm;
			if (ef != null)
			{
				ef.Shown += OnEntryFormShown;
				return;
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

		public bool TryUnlockVolume(string DriveMountPoint, string DriveGUID, string Password)
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
				FveApi.UnlockVolume(DriveMountPointString, DriveGUIDString, Password);
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
			KeePassLib.PwDatabase Database = e.Database;
			KeePassLib.Collections.PwObjectList<KeePassLib.PwEntry> AllEntries = Database.RootGroup.GetEntries(true);

			foreach (KeePassLib.PwEntry Entry in AllEntries)
			{
				KeePassLib.Collections.ProtectedStringDictionary Strings = Entry.Strings;
				KeePassLib.Security.ProtectedString DriveIdTypeStr = Strings.Get(StringName_DriveIdType);
				KeePassLib.Security.ProtectedString DriveMountPoint = Strings.Get(StringName_DriveMountPoint);
				KeePassLib.Security.ProtectedString DriveGUID = Strings.Get(StringName_DriveGUID);
				KeePassLib.Security.ProtectedString Password = Strings.Get("Password");

				if (Password == null)
					continue;

				EDriveIdType DriveIdType = Forms.KeeLockerEntryTab.GetDriveIdTypeFromString(DriveIdTypeStr);

				TryUnlockVolume(
					DriveIdType == EDriveIdType.MountPoint && DriveMountPoint != null ? DriveMountPoint.ReadString() : "",
					DriveIdType == EDriveIdType.GUID && DriveGUID != null ? DriveGUID.ReadString() : "",
					Password.ReadString()
					);
			}
		}

	}
}
