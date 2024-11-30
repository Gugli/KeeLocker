
namespace KeeLocker.Forms
{
	partial class KeeLockerEntryTab
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cbx_DriveMountPoint = new KeeLocker.Forms.RichComboBox();
			this.chk_UnlockOnOpening = new System.Windows.Forms.CheckBox();
			this.chk_UnlockOnConnection = new System.Windows.Forms.CheckBox();
			this.btn_Unlock = new System.Windows.Forms.Button();
			this.rdo_MountPoint = new System.Windows.Forms.RadioButton();
			this.rdo_DriveGUID = new System.Windows.Forms.RadioButton();
			this.txt_DriveGUID = new System.Windows.Forms.TextBox();
			this.grp_Drive = new System.Windows.Forms.GroupBox();
			this.btn_DriveGUID = new System.Windows.Forms.Button();
			this.lbl_DriveGUID = new System.Windows.Forms.Label();
			this.grp_Unlock = new System.Windows.Forms.GroupBox();
			this.chk_IsRecoveryKey = new System.Windows.Forms.CheckBox();
			this.grp_Drive.SuspendLayout();
			this.grp_Unlock.SuspendLayout();
			this.SuspendLayout();
			// 
			// rdo_MountPoint
			// 
			this.rdo_MountPoint.AutoSize = true;
			this.rdo_MountPoint.Location = new System.Drawing.Point(18, 22);
			this.rdo_MountPoint.Name = "rdo_MountPoint";
			this.rdo_MountPoint.Size = new System.Drawing.Size(108, 17);
			this.rdo_MountPoint.TabIndex = 101;
			this.rdo_MountPoint.Text = "Drive mountpoint:";
			this.rdo_MountPoint.UseVisualStyleBackColor = true;
			this.rdo_MountPoint.Click += new System.EventHandler(this.rdo_MountPoint_Click);
            // 
            // cbx_DriveMountPoint
            // 
            this.cbx_DriveMountPoint.Item_Add(new KeeLocker.Forms.RichComboBox.SItem("Can be a drive root like :", RichComboBox.EItemType.Inactive));
			this.cbx_DriveMountPoint.Item_Add(new KeeLocker.Forms.RichComboBox.SItem("C:\\", RichComboBox.EItemType.Active));
			this.cbx_DriveMountPoint.Item_Add(new KeeLocker.Forms.RichComboBox.SItem("D:\\", RichComboBox.EItemType.Active));
			this.cbx_DriveMountPoint.Item_Add(new KeeLocker.Forms.RichComboBox.SItem("E:\\", RichComboBox.EItemType.Active));
			this.cbx_DriveMountPoint.Item_Add(new KeeLocker.Forms.RichComboBox.SItem("F:\\", RichComboBox.EItemType.Active));
			this.cbx_DriveMountPoint.Item_Add(new KeeLocker.Forms.RichComboBox.SItem("Or any valid mountpoint path", RichComboBox.EItemType.Inactive));
			this.cbx_DriveMountPoint.Item_Add(new KeeLocker.Forms.RichComboBox.SItem("C:\\Path\\To\\MountPoint", RichComboBox.EItemType.Active));
			this.cbx_DriveMountPoint.Item_Add(new KeeLocker.Forms.RichComboBox.SItem("Or an exotic mountpoint", RichComboBox.EItemType.Inactive));
			this.cbx_DriveMountPoint.Item_Add(new KeeLocker.Forms.RichComboBox.SItem("like a network drive, etc...", RichComboBox.EItemType.Inactive));
			this.cbx_DriveMountPoint.MaxDropDownItems = this.cbx_DriveMountPoint.Items.Count;
			this.cbx_DriveMountPoint.Location = new System.Drawing.Point(135, 21);
			this.cbx_DriveMountPoint.Name = "cbx_DriveMountPoint";
			this.cbx_DriveMountPoint.Size = new System.Drawing.Size(218, 21);
			this.cbx_DriveMountPoint.TabIndex = 102;
			this.cbx_DriveMountPoint.Validated += new System.EventHandler(this.cbx_DriveMountPoint_Validated);
            // 
            // btn_DriveGUID
            // 
            this.btn_DriveGUID.Location = new System.Drawing.Point(135, 48);
            this.btn_DriveGUID.Name = "btn_DriveGUID";
            this.btn_DriveGUID.Size = new System.Drawing.Size(218, 25);
            this.btn_DriveGUID.TabIndex = 103;
            this.btn_DriveGUID.Text = "Convert mountpoint to GUID";
            this.btn_DriveGUID.UseVisualStyleBackColor = true;
            this.btn_DriveGUID.Click += new System.EventHandler(this.btn_DriveGUID_Click);
			// 
			// rdo_DriveGUID
			// 
			this.rdo_DriveGUID.AutoSize = true;
			this.rdo_DriveGUID.Location = new System.Drawing.Point(18, 99);
			this.rdo_DriveGUID.Name = "rdo_DriveGUID";
			this.rdo_DriveGUID.Size = new System.Drawing.Size(83, 17);
			this.rdo_DriveGUID.TabIndex = 104;
			this.rdo_DriveGUID.Text = "Drive GUID:";
			this.rdo_DriveGUID.UseVisualStyleBackColor = true;
            this.rdo_DriveGUID.Click += new System.EventHandler(this.rdo_DriveGUID_Click);
            // 
            // txt_DriveGUID
            // 
            this.txt_DriveGUID.Location = new System.Drawing.Point(135, 96);
			this.txt_DriveGUID.Name = "txt_DriveGUID";
			this.txt_DriveGUID.Size = new System.Drawing.Size(218, 20);
			this.txt_DriveGUID.TabIndex = 105;
			this.txt_DriveGUID.Validated += new System.EventHandler(this.txt_DriveGUID_Validated);
            // 
            // chk_UnlockOnOpening
            // 
            this.chk_UnlockOnOpening.AutoSize = true;
			this.chk_UnlockOnOpening.Location = new System.Drawing.Point(26, 36);
			this.chk_UnlockOnOpening.Name = "chk_UnlockOnOpening";
			this.chk_UnlockOnOpening.Size = new System.Drawing.Size(153, 17);
			this.chk_UnlockOnOpening.TabIndex = 106;
			this.chk_UnlockOnOpening.Text = "Unlock volume on database opening";
			this.chk_UnlockOnOpening.UseVisualStyleBackColor = true;
			this.chk_UnlockOnOpening.Click += new System.EventHandler(this.chk_UnlockOnOpening_Click);
            // 
            // chk_UnlockOnConnection
            // 
            this.chk_UnlockOnConnection.AutoSize = true;
			this.chk_UnlockOnConnection.Location = new System.Drawing.Point(26, 65);
			this.chk_UnlockOnConnection.Name = "chk_UnlockOnConnection";
			this.chk_UnlockOnConnection.Size = new System.Drawing.Size(153, 17);
			this.chk_UnlockOnConnection.TabIndex = 107;
			this.chk_UnlockOnConnection.Text = "Unlock volume when connected";
			this.chk_UnlockOnConnection.UseVisualStyleBackColor = true;
			this.chk_UnlockOnConnection.Click += new System.EventHandler(this.chk_UnlockOnConnection_Click);
			// 
			// chk_IsRecoveryKey
			// 
			this.chk_IsRecoveryKey.AutoSize = true;
			this.chk_IsRecoveryKey.Location = new System.Drawing.Point(26, 95);
			this.chk_IsRecoveryKey.Name = "chk_IsRecoveryKey";
			this.chk_IsRecoveryKey.Size = new System.Drawing.Size(180, 17);
			this.chk_IsRecoveryKey.TabIndex = 108;
			this.chk_IsRecoveryKey.Text = "Use password as a recovery key";
			this.chk_IsRecoveryKey.UseVisualStyleBackColor = true;
			this.chk_IsRecoveryKey.Click += new System.EventHandler(this.chk_IsRecoveryKey_Click);
			// 
			// btn_Unlock
			// 
			this.btn_Unlock.Location = new System.Drawing.Point(249, 30);
			this.btn_Unlock.Name = "btn_Unlock";
			this.btn_Unlock.Size = new System.Drawing.Size(112, 23);
			this.btn_Unlock.TabIndex = 109;
			this.btn_Unlock.Text = "Unlock Volume Now";
			this.btn_Unlock.UseVisualStyleBackColor = true;
			this.btn_Unlock.Click += new System.EventHandler(this.btn_Unlock_Click);
			// 
			// grp_Drive
			// 
			this.grp_Drive.Controls.Add(this.btn_DriveGUID);
			this.grp_Drive.Controls.Add(this.lbl_DriveGUID);
			this.grp_Drive.Controls.Add(this.txt_DriveGUID);
			this.grp_Drive.Controls.Add(this.rdo_DriveGUID);
			this.grp_Drive.Controls.Add(this.rdo_MountPoint);
			this.grp_Drive.Controls.Add(this.cbx_DriveMountPoint);
			this.grp_Drive.Location = new System.Drawing.Point(43, 19);
			this.grp_Drive.Name = "grp_Drive";
			this.grp_Drive.Size = new System.Drawing.Size(379, 162);
            this.grp_Drive.TabIndex = 110;
            this.grp_Drive.TabStop = false;
			this.grp_Drive.Text = "Drive info";
			// 
			// lbl_DriveGUID
			// 
			this.lbl_DriveGUID.AutoSize = true;
			this.lbl_DriveGUID.Location = new System.Drawing.Point(62, 125);
			this.lbl_DriveGUID.Name = "lbl_DriveGUID";
			this.lbl_DriveGUID.Size = new System.Drawing.Size(255, 26);
			this.lbl_DriveGUID.Text = "Use a GUID for removable drives that do not always \r\nmount on the same letter.";
			// 
			// grp_Unlock
			// 
			this.grp_Unlock.Controls.Add(this.chk_IsRecoveryKey);
			this.grp_Unlock.Controls.Add(this.chk_UnlockOnOpening);
			this.grp_Unlock.Controls.Add(this.chk_UnlockOnConnection);
			this.grp_Unlock.Controls.Add(this.btn_Unlock);
			this.grp_Unlock.Location = new System.Drawing.Point(43, 187);
			this.grp_Unlock.Name = "grp_Unlock";
			this.grp_Unlock.Size = new System.Drawing.Size(379, 121);
            this.grp_Unlock.TabIndex = 111;
            this.grp_Unlock.TabStop = false;
			this.grp_Unlock.Text = "Unlock settings";
			// 
			// KeeLockerEntryTab
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.grp_Unlock);
			this.Controls.Add(this.grp_Drive);
			this.Name = "KeeLockerEntryTab";
			this.Size = new System.Drawing.Size(483, 311);
            this.grp_Drive.TabIndex = 112;
            this.grp_Drive.ResumeLayout(false);
			this.grp_Drive.PerformLayout();
			this.grp_Unlock.ResumeLayout(false);
			this.grp_Unlock.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private RichComboBox cbx_DriveMountPoint;
		private System.Windows.Forms.CheckBox chk_UnlockOnOpening;
		private System.Windows.Forms.CheckBox chk_UnlockOnConnection;
		private System.Windows.Forms.Button btn_Unlock;
		private System.Windows.Forms.RadioButton rdo_MountPoint;
		private System.Windows.Forms.RadioButton rdo_DriveGUID;
		private System.Windows.Forms.TextBox txt_DriveGUID;
		private System.Windows.Forms.GroupBox grp_Drive;
		private System.Windows.Forms.GroupBox grp_Unlock;
		private System.Windows.Forms.Button btn_DriveGUID;
		private System.Windows.Forms.Label lbl_DriveGUID;
		private System.Windows.Forms.CheckBox chk_IsRecoveryKey;
	}
}
