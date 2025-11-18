using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TestSample.Popup
{
    delegate void MouseRightClick(object sender, ItemClickEventArgs e);
    public class PopupMenuProcessor
    {
        BarManager _BarManager = null;
        DevExpress.XtraBars.PopupMenu _Menu = null;
        MouseRightClick _MouseRightClick;
        internal PopupMenuProcessor(BarManager barManager, MouseRightClick mouseRightClick)
        {
            this._BarManager = barManager;
            this._MouseRightClick = mouseRightClick;
        }

        public enum ItemType
        {
            Copy,
            Paste
        }

        internal void InitMenu()
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);

                //Copy
                BarButtonItem bbtnCopy = new BarButtonItem(this._BarManager, "Copy", 0);
                bbtnCopy.Tag = ItemType.Copy;
                bbtnCopy.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnCopy });

                //Dán
                BarButtonItem bbtnDan = new BarButtonItem(this._BarManager, "Paste", 1);
                bbtnDan.Tag = ItemType.Paste;
                bbtnDan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnDan });

                this._Menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
