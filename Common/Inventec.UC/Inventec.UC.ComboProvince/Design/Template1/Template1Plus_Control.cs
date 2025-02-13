﻿using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboProvince.Design.Template1
{
    internal partial class Template1
    {
        private void txtMaTinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadTinhThanhCombo(strValue.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTinh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTinh.EditValue != null && cboTinh.EditValue != cboTinh.OldEditValue)
                    {
                        string str = cboTinh.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listData.SingleOrDefault(o => o.PROVINCE_CODE == cboTinh.EditValue.ToString());
                        if (province != null)
                        {
                            if (_LoadHuyenFromTinh != null) _LoadHuyenFromTinh(province.PROVINCE_CODE);
                            txtMaTinh.Text = province.PROVINCE_CODE;
                            if (_FocusCboHuyen != null) _FocusCboHuyen(true);
                            //txtMaHuyen.Text = "";
                            //txtMaHuyen.Focus();
                        }
                    }
                    else
                    {
                        if (_FocusCboHuyen != null) _FocusCboHuyen(false);
                        //txtMaHuyen.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTinh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTinh.EditValue != null)
                    {
                        string str = cboTinh.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listData.SingleOrDefault(o => o.PROVINCE_CODE == cboTinh.EditValue.ToString());
                        if (province != null)
                        {
                            if (_LoadHuyenFromTinh != null) _LoadHuyenFromTinh(province.PROVINCE_CODE);
                            txtMaTinh.Text = province.PROVINCE_CODE;
                            if (_FocusCboHuyen != null) _FocusCboHuyen(true);
                            //txtMaHuyen.Text = "";
                            //txtMaHuyen.Focus();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    cboTinh.EditValue = null;
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
