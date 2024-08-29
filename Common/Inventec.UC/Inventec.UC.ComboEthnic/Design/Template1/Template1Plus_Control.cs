using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboEthnic.Design.Template1
{
    public partial class Template1
    {

        private void txtMaDanToc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDanTocCombo(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDanToc_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDanToc.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_ETHNIC ethnic = listEthnic.SingleOrDefault(o => o.ETHNIC_CODE == (cboDanToc.EditValue ?? 0).ToString());
                        if (ethnic != null)
                        {
                            txtMaDanToc.Text = ethnic.ETHNIC_CODE;
                            if (_FocusNext != null)
                            {
                                _FocusNext();
                            }
                            //txtMaQuocTich.Focus();
                            //txtMaQuocTich.SelectAll();
                        }
                    }
                    else
                    {
                        if (_FocusNext != null)
                        {
                            _FocusNext();
                        }
                        //txtMaQuocTich.Focus();
                        //txtMaQuocTich.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDanToc_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDanToc.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_ETHNIC data = listEthnic.SingleOrDefault(o => o.ETHNIC_CODE == (cboDanToc.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            txtMaDanToc.Text = data.ETHNIC_CODE;
                            if (_FocusNext != null)
                            {
                                _FocusNext();
                            }
                            //txtMaQuocTich.Focus();
                            //txtMaQuocTich.SelectAll();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    cboDanToc.EditValue = null;
                    txtMaDanToc.Text = "";
                    if (_FocusNext != null)
                    {
                        _FocusNext();
                    }
                    //txtMaQuocTich.Focus();
                    //txtMaQuocTich.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
