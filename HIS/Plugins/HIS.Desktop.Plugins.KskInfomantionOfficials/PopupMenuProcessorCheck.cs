using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraBars;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials
{
    delegate void MouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessorCheck
    {
        internal enum ItemType
        {
            InMps507
        }
        BarManager _BarManager = null;
        MouseRightClick _MouseRightClick;
        PopupMenu _PopupMenu = null;
        List<ADO.ServiceReqADO> ListAdos;

        internal PopupMenuProcessorCheck(BarManager barmanager, MouseRightClick mouseRightClick, List<ADO.ServiceReqADO> listAdos)
        {
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
            this.ListAdos = listAdos;
        }

        internal void InitMenu()
        {
            try
            {
                if (this.ListAdos == null || this.ListAdos.Count == 0 || this._BarManager == null || this._MouseRightClick == null)
                    return;

                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();

                List<BarItem> barItems = new List<BarItem>();

                BarButtonItem bbtInKemKetQua = new BarButtonItem(this._BarManager, Resources.ResourceMessage.InPhieu, 0);
                bbtInKemKetQua.Tag = ItemType.InMps507;
                bbtInKemKetQua.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                barItems.Add(bbtInKemKetQua);

                this._PopupMenu.AddItems(barItems.ToArray());
                this._PopupMenu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
