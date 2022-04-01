using System;
using System.Windows.Forms;
using KeePass.Plugins;

namespace KeeLocker.Forms
{
	public partial class KeeLockerEntryTab : UserControl
	{
		private KeePass.Plugins.IPluginHost m_host;
		private KeeLockerExt m_plugin;
		private KeePassLib.PwEntry m_entry;
		KeePassLib.Collections.ProtectedStringDictionary m_entrystrings;

		public KeeLockerEntryTab(IPluginHost host, KeeLockerExt plugin, KeePassLib.PwEntry entry, KeePassLib.Collections.ProtectedStringDictionary strings)
		{
			m_host = host;
			m_plugin = plugin;
			m_entry = entry;
			m_entrystrings = strings;

			InitializeComponent();

			cbx_DriveMountPoint.Text = m_entrystrings.Get("DriveMountPoint").ReadString();
		}

		private void cbx_DriveMountPoint_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cbx_DriveMountPoint.Text != m_entrystrings.Get("DriveMountPoint").ReadString())
			{
				KeePassLib.Security.ProtectedString DriveMountPoint = new KeePassLib.Security.ProtectedString(false, cbx_DriveMountPoint.Text);
				m_entrystrings.Set("DriveMountPoint", DriveMountPoint);
			}
		}
	}
}
