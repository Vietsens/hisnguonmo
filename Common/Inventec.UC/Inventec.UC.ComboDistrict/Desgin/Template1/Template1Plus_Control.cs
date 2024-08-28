using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Inventec.UC.ComboDistrict.Desgin.Template1
{
    public partial class Template1
    {

        private void txtMaHuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string provinceCode = "";
                    if (_GetValueProvince != null) //cboTinh.EditValue
                    {
                        provinceCode = _GetValueProvince().ToString();//cboTinh.EditValue.ToString();
                    }
                    LoadHuyenCombo(strValue.ToUpper(), provinceCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHuyen_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHuyen.EditValue != null && cboHuyen.EditValue != cboHuyen.OldEditValue)
                    {
                        string str = cboHuyen.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listData.SingleOrDefault(o => o.DISTRICT_CODE == cboHuyen.EditValue.ToString());
                        if (district != null)
                        {
                            if (_LoadComboCommune != null) _LoadComboCommune(district.DISTRICT_CODE);
                            //LoadXaCombo("", district.DISTRICT_CODE, false);
                            txtMaHuyen.Text = district.DISTRICT_CODE;
                            if (_SetValueCommune != null) _SetValueCommune();
                            //cboXaPhuong.EditValue = null;
                            //txtMaXaPhuong.Text = "";
                            if (_FocusComboCommune != null) _FocusComboCommune();
                            //txtMaXaPhuong.Focus();
                            //txtMaXaPhuong.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHuyen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHuyen.EditValue != null)
                    {
                        string str = cboHuyen.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listData.SingleOrDefault(o => o.DISTRICT_CODE == cboHuyen.EditValue.ToString());
                        if (district != null)
                        {
                            if (_LoadComboCommune != null) _LoadComboCommune(district.DISTRICT_CODE);
                            //LoadXaCombo("", district.DISTRICT_CODE, false);
                            txtMaHuyen.Text = district.DISTRICT_CODE;
                            if (_SetValueCommune != null) _SetValueCommune();
                            //cboXaPhuong.EditValue = null;
                            //txtMaXaPhuong.Text = "";
                            if (_FocusComboCommune != null) _FocusComboCommune();
                            //txtMaXaPhuong.Focus();
                            //txtMaXaPhuong.SelectAll();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    cboHuyen.EditValue = null;
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
