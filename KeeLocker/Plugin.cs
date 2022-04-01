using System;

namespace KeeLocker
{
	public class KeeLockerExt : KeePass.Plugins.Plugin
	{
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

		void OnEntryFormShown(object sender, EventArgs e)
		{
			KeePass.Forms.PwEntryForm form = sender as KeePass.Forms.PwEntryForm;
			if (form == null)
				return;

			KeePassLib.PwEntry entry = form.EntryRef;
			if (entry == null)
				return;

			KeePassLib.Collections.ProtectedStringDictionary strings = form.EntryStrings;
			if (strings == null)
				return;

			System.Windows.Forms.Control[] cs = form.Controls.Find("m_tabMain", true);
			if (cs.Length == 0)
				return;

			System.Windows.Forms.TabControl tabMain = cs[0] as System.Windows.Forms.TabControl;

			System.Windows.Forms.UserControl KeeLockerEntryTab = new KeeLocker.Forms.KeeLockerEntryTab(m_host, this, entry, strings);

			System.Windows.Forms.TabPage KeeLockerEntryTabContainer = new System.Windows.Forms.TabPage("KeeLocker");
			KeeLockerEntryTabContainer.Controls.Add(KeeLockerEntryTab);
			KeeLockerEntryTab.Dock = System.Windows.Forms.DockStyle.Fill;
			tabMain.TabPages.Add(KeeLockerEntryTabContainer);
		}


		private void OnKPDBOpen(object sender, KeePass.Forms.FileOpenedEventArgs e)
		{
			KeePassLib.PwDatabase Database = e.Database;
			KeePassLib.Collections.PwObjectList<KeePassLib.PwEntry> AllEntries = Database.RootGroup.GetEntries(true);


			foreach (KeePassLib.PwEntry Entry in AllEntries)
			{
				KeePassLib.Collections.ProtectedStringDictionary Strings = Entry.Strings;
				KeePassLib.Security.ProtectedString DriveMountPoint = Strings.Get("DriveMountPoint");
				if (DriveMountPoint == null)
					continue;
				KeePassLib.Security.ProtectedString Password = Strings.Get("Password");
				if (Password == null)
					continue;

				try
				{
					FveApi.UnlockVolume(DriveMountPoint.ReadString(), Password.ReadString());
				}
				catch (Exception Ex)
				{
					string Messages = Ex.ToString();
				}
			}
		}

	}
}
