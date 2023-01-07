using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeLocker.Forms
{
	class RichComboBox : System.Windows.Forms.ComboBox
	{
		public enum EItemType
		{
			Active,
			Inactive
		};

		public class SItem
		{
			public string Text { get; set; }
			public EItemType Type { get; set; }

			public SItem(string _Text, EItemType _Type)
			{
				Text = _Text;
				Type = _Type;
			}
		};

		public List<EItemType> Items_Type { get; set; }
		public System.Drawing.Color InactiveColor = System.Drawing.SystemColors.GrayText;
		public int ActiveShift = 20;

		public RichComboBox()
		{
			DrawMode =System.Windows.Forms.DrawMode.OwnerDrawVariable;
			Items_Type = new List<EItemType>();
		}

		protected int LatestValidIndex = -1;

		public void Item_Add(SItem Item)
		{
			if (LatestValidIndex == -1 && Item.Type == EItemType.Active)
			{
				LatestValidIndex = Items.Count;
			}

			Items.Add(Item.Text);
			Items_Type.Add(Item.Type);
		}
	
		protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
		{

			EItemType Type = Items_Type[e.Index];
			System.Drawing.Color Color;
			bool ForceBg;
			System.Drawing.Rectangle Bounds;
			System.Drawing.Font Font;

			if (Type == EItemType.Active)
			{
				Color = ForeColor;
				ForceBg = false;
				Bounds = e.Bounds;
				Bounds.X += ActiveShift;
				Bounds.Width -= ActiveShift;
				Font = e.Font;
			} 
			else // (Type == EItemType.Inactive)
			{
				Color = InactiveColor;
				ForceBg = true;
				Bounds = e.Bounds;
				Font = new System.Drawing.Font( e.Font.FontFamily, e.Font.Size, System.Drawing.FontStyle.Italic);
			}

			e.DrawBackground();
			if (ForceBg)
			{
				e.Graphics.FillRectangle(new System.Drawing.SolidBrush(BackColor), e.Bounds);
			}
			e.Graphics.DrawString(Items[e.Index].ToString(), Font, new System.Drawing.SolidBrush(Color), Bounds);
			e.DrawFocusRectangle();
		}
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			EItemType Type = Items_Type[SelectedIndex];
			if (Type == EItemType.Inactive)
			{
				SelectedIndex = LatestValidIndex;
				return;
			}
			LatestValidIndex = SelectedIndex;
			base.OnSelectedIndexChanged(e);
		}
	}
}
