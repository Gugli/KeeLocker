using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass.Plugins;

namespace KeeLocker.Forms
{
	public partial class KeeLockerEntryTab : UserControl
	{
		private KeePass.Plugins.IPluginHost m_host;
		private KeeLockerExt m_plugin;
		public KeeLockerEntryTab(IPluginHost host, KeeLockerExt plugin)
		{
			m_host = host;
			m_plugin = plugin;
			InitializeComponent();
		}
	}
}
