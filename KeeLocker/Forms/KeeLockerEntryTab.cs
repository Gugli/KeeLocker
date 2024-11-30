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
		private KeePassLib.Collections.ProtectedStringDictionary m_entrystrings;

		// settings
		private KeeLockerExt.EDriveIdType m_DriveIdType;
		private string m_DriveMountPoint;
		private string m_DriveGUID;
		private bool m_UnlockOnOpening;
		private bool m_UnlockOnConnection;
		private bool m_IsRecoveryKey;

		const KeeLockerExt.EDriveIdType DriveIdTypeDefault = KeeLockerExt.EDriveIdType.MountPoint;


		public static KeeLockerExt.EDriveIdType GetDriveIdTypeFromString(KeePassLib.Security.ProtectedString DriveIdType)
		{
			if (DriveIdType != null)
			{
				string DriveIdTypeString = DriveIdType.ReadString();
				if (DriveIdTypeString == KeeLockerExt.EDriveIdType.MountPoint.ToString())
					return KeeLockerExt.EDriveIdType.MountPoint;
				else if (DriveIdTypeString == KeeLockerExt.EDriveIdType.GUID.ToString())
					return KeeLockerExt.EDriveIdType.GUID;
			}
			return DriveIdTypeDefault;
		}

		public static bool GetUnlockOnOpeningFromString(KeePassLib.Security.ProtectedString UnlockOnOpening)
		{
			return UnlockOnOpening == null || UnlockOnOpening.ReadString().Trim().ToLower() != "false"; // default is to unlock;
		}

		public static bool GetIsRecoveryKeyFromString(KeePassLib.Security.ProtectedString IsRecoveryKey)
		{
			return IsRecoveryKey != null && IsRecoveryKey.ReadString().Trim().ToLower() == "true"; // default is standard password
		}

		public KeeLockerEntryTab(IPluginHost host, KeeLockerExt plugin, KeePassLib.PwEntry entry, KeePassLib.Collections.ProtectedStringDictionary strings)
		{
			m_host = host;
			m_plugin = plugin;
			m_entry = entry;
			m_entrystrings = strings;

			InitializeComponent();

			SettingsLoad();

			UpdateUi();
		}

		private void SettingsLoad()
		{
			{
				KeePassLib.Security.ProtectedString DriveIdType = m_entrystrings.Get(KeeLockerExt.StringName_DriveIdType);
				m_DriveIdType = GetDriveIdTypeFromString(DriveIdType);
			}

			{
				KeePassLib.Security.ProtectedString DriveMountPoint = m_entrystrings.Get(KeeLockerExt.StringName_DriveMountPoint);
				m_DriveMountPoint = DriveMountPoint != null ? DriveMountPoint.ReadString() : "";
			}

			{
				KeePassLib.Security.ProtectedString DriveGUID = m_entrystrings.Get(KeeLockerExt.StringName_DriveGUID);
				m_DriveGUID = DriveGUID != null ? DriveGUID.ReadString() : "";
			}

			{
				KeePassLib.Security.ProtectedString UnlockOnOpening = m_entrystrings.Get(KeeLockerExt.StringName_UnlockOnOpening);
				m_UnlockOnOpening = GetUnlockOnOpeningFromString(UnlockOnOpening);
            }
            {
                KeePassLib.Security.ProtectedString UnlockOnConnection = m_entrystrings.Get(KeeLockerExt.StringName_UnlockOnConnection);
                m_UnlockOnConnection = GetUnlockOnOpeningFromString(UnlockOnConnection);
            }
            {
				KeePassLib.Security.ProtectedString IsRecoveryKey = m_entrystrings.Get(KeeLockerExt.StringName_IsRecoveryKey);
				m_IsRecoveryKey = GetIsRecoveryKeyFromString(IsRecoveryKey);
			}
		}

		private void SettingsSave(string SettingName, string SettingValue)
		{
			if (SettingValue.Length == 0)
			{
				m_entrystrings.Remove(SettingName);
			}
			else
			{
				KeePassLib.Security.ProtectedString PreviousValue = m_entrystrings.Get(SettingName);
				if (PreviousValue == null || SettingValue != PreviousValue.ReadString())
				{
					m_entrystrings.Set(SettingName, new KeePassLib.Security.ProtectedString(false, SettingValue) );
				}
			}
		}

		private void SettingsSave()
		{
			SettingsSave(KeeLockerExt.StringName_DriveIdType, m_DriveIdType == DriveIdTypeDefault ? "" : m_DriveIdType.ToString());
			SettingsSave(KeeLockerExt.StringName_DriveMountPoint, m_DriveMountPoint);
			SettingsSave(KeeLockerExt.StringName_DriveGUID, m_DriveGUID);
			SettingsSave(KeeLockerExt.StringName_UnlockOnOpening, m_UnlockOnOpening ? "" : "false");
			SettingsSave(KeeLockerExt.StringName_UnlockOnConnection, m_UnlockOnConnection ? "" : "false");
			SettingsSave(KeeLockerExt.StringName_IsRecoveryKey, m_IsRecoveryKey ? "true" : "");
		}

		private void UpdateUi()
		{
			cbx_DriveMountPoint.Text = m_DriveMountPoint;
			txt_DriveGUID.Text = m_DriveGUID;

			rdo_MountPoint.Checked = m_DriveIdType == KeeLockerExt.EDriveIdType.MountPoint;
			rdo_DriveGUID.Checked = m_DriveIdType == KeeLockerExt.EDriveIdType.GUID;

			cbx_DriveMountPoint.Enabled = rdo_MountPoint.Checked;
			btn_DriveGUID.Enabled = rdo_MountPoint.Checked;
			txt_DriveGUID.Enabled = rdo_DriveGUID.Checked;

			chk_UnlockOnOpening.Checked = m_UnlockOnOpening;
            chk_UnlockOnConnection.Checked = m_UnlockOnConnection;
			chk_IsRecoveryKey.Checked = m_IsRecoveryKey;
		}

		public void OnSave(object sender, EventArgs e)
		{
			SettingsSave();
		}

		private void cbx_DriveMountPoint_Validated(object sender, EventArgs e)
		{
			m_DriveMountPoint = cbx_DriveMountPoint.Text;
			UpdateUi();
		}

		private void txt_DriveGUID_Validated(object sender, EventArgs e)
		{
			m_DriveGUID = txt_DriveGUID.Text;
			UpdateUi();
		}

		private void rdo_MountPoint_Click(object sender, EventArgs e)
		{
			m_DriveIdType = KeeLockerExt.EDriveIdType.MountPoint;
			UpdateUi();
		}

		private void rdo_DriveGUID_Click(object sender, EventArgs e)
		{
			m_DriveIdType = KeeLockerExt.EDriveIdType.GUID;
			UpdateUi();
        }

        private void chk_UnlockOnOpening_Click(object sender, EventArgs e)
		{
			m_UnlockOnOpening = !m_UnlockOnOpening;
			UpdateUi();
		}
		private void chk_UnlockOnConnection_Click(object sender, EventArgs e)
		{
            m_UnlockOnConnection = !m_UnlockOnConnection;
			UpdateUi();
		}

		private void btn_Unlock_Click(object sender, EventArgs e)
		{
			KeePassLib.Collections.ProtectedStringDictionary Strings = m_entry.Strings;
			KeePassLib.Security.ProtectedString Password = Strings.Get(KeeLockerExt.StringName_Password);
			KeePassLib.Security.ProtectedString IsRecoveryKey = Strings.Get(KeeLockerExt.StringName_IsRecoveryKey);

			m_plugin.TryUnlockVolume(
				m_DriveIdType == KeeLockerExt.EDriveIdType.MountPoint ? m_DriveMountPoint : "",
				m_DriveIdType == KeeLockerExt.EDriveIdType.GUID ? m_DriveGUID : "", 
				Password != null ? Password.ReadString() : "",
				GetIsRecoveryKeyFromString(IsRecoveryKey)
				);
		}

		private void btn_DriveGUID_Click(object sender, EventArgs e)
		{
			string DriveGUID;
			bool Ok = FveApi.GetDriveGUID(m_DriveMountPoint, out DriveGUID);
			if (Ok)
			{
				m_DriveGUID = DriveGUID;
				m_DriveIdType = KeeLockerExt.EDriveIdType.GUID;
			}
			else
			{
				m_DriveGUID = "Unable to get GUID";
				m_DriveIdType = KeeLockerExt.EDriveIdType.MountPoint;
			}
			UpdateUi();
		}

		private void chk_IsRecoveryKey_Click(object sender, EventArgs e)
		{
			m_IsRecoveryKey = !m_IsRecoveryKey;
			UpdateUi();
		}
	}
}
