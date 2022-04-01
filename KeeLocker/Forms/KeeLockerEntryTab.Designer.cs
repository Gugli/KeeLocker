
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
			this.lbl_DriveMountPoint = new System.Windows.Forms.Label();
			this.cbx_DriveMountPoint = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// lbl_DriveMountPoint
			// 
			this.lbl_DriveMountPoint.AutoSize = true;
			this.lbl_DriveMountPoint.Location = new System.Drawing.Point(12, 13);
			this.lbl_DriveMountPoint.Name = "lbl_DriveMountPoint";
			this.lbl_DriveMountPoint.Size = new System.Drawing.Size(90, 13);
			this.lbl_DriveMountPoint.TabIndex = 0;
			this.lbl_DriveMountPoint.Text = "Drive mountpoint:";
			// 
			// cbx_DriveMountPoint
			// 
			this.cbx_DriveMountPoint.FormattingEnabled = true;
			this.cbx_DriveMountPoint.Items.AddRange(new object[] {
            "A:\\",
            "B:\\",
            "C:\\",
            "D:\\",
            "E:\\",
            "F:\\",
            "G:\\",
            "H:\\"});
			this.cbx_DriveMountPoint.Location = new System.Drawing.Point(120, 10);
			this.cbx_DriveMountPoint.Name = "cbx_DriveMountPoint";
			this.cbx_DriveMountPoint.Size = new System.Drawing.Size(208, 21);
			this.cbx_DriveMountPoint.TabIndex = 1;
			this.cbx_DriveMountPoint.SelectedIndexChanged += new System.EventHandler(this.cbx_DriveMountPoint_SelectedIndexChanged);
			// 
			// KeeLockerEntryTab
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.cbx_DriveMountPoint);
			this.Controls.Add(this.lbl_DriveMountPoint);
			this.Name = "KeeLockerEntryTab";
			this.Size = new System.Drawing.Size(483, 204);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbl_DriveMountPoint;
		private System.Windows.Forms.ComboBox cbx_DriveMountPoint;
	}
}
