using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboEthnic.Design.Template1
{
    public partial class Template1
    {
        private void LoadDataToComboDanToc()
        {
            try
            {
                cboDanToc.Properties.DataSource = listEthnic;
                cboDanToc.Properties.DisplayMember = "ETHNIC_NAME";
                cboDanToc.Properties.ValueMember = "ETHNIC_CODE";
                cboDanToc.Properties.ForceInitialize();
                cboDanToc.Properties.Columns.Clear();
                cboDanToc.Properties.Columns.Add(new LookUpColumnInfo("ETHNIC_CODE", "", 100));
                cboDanToc.Properties.Columns.Add(new LookUpColumnInfo("ETHNIC_NAME", "", 200));
                cboDanToc.Properties.ShowHeader = false;
                cboDanToc.Properties.ImmediatePopup = true;
                cboDanToc.Properties.DropDownRows = 20;
                cboDanToc.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDanTocCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDanToc.EditValue = null;
                    cboDanToc.Focus();
                    cboDanToc.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboDanToc);
                }
                else
                {
                    var data = listEthnic.Where(o => o.ETHNIC_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDanToc.EditValue = data[0].ETHNIC_CODE;
                            txtMaDanToc.Text = data[0].ETHNIC_CODE;
                            if (_FocusNext != null)
                            {
                                _FocusNext();
                            }
                            //txtMaQuocTich.Focus();
                            //txtMaQuocTich.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            cboDanToc.EditValue = null;
                            cboDanToc.Focus();
                            cboDanToc.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboDanToc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
