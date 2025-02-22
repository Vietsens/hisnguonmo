﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using Inventec.UC.ComboDistrict.Process;

namespace Inventec.UC.ComboDistrict.Desgin.Template1
{
    public partial class Template1
    {
        private void LoadHuyenCombo(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = listData.Where(o => o.DISTRICT_CODE.Contains(searchCode) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();

                cboHuyen.Properties.DataSource = listResult;
                cboHuyen.Properties.DisplayMember = "DISTRICT_NAME";
                cboHuyen.Properties.ValueMember = "DISTRICT_CODE";
                cboHuyen.Properties.ForceInitialize();

                cboHuyen.Properties.Columns.Clear();
                cboHuyen.Properties.Columns.Add(new LookUpColumnInfo("DISTRICT_CODE", "", 100));
                cboHuyen.Properties.Columns.Add(new LookUpColumnInfo("DISTRICT_NAME", "", 200));

                cboHuyen.Properties.ShowHeader = false;
                cboHuyen.Properties.ImmediatePopup = true;
                cboHuyen.Properties.DropDownRows = 20;
                cboHuyen.Properties.PopupWidth = 300;

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    //cboXaPhuong.Properties.DataSource = null;
                    //cboXaPhuong.EditValue = null;
                    //txtMaXaPhuong.Text = "";
                    if (_SetValueCommune != null) _SetValueCommune();
                    txtMaHuyen.Text = "";
                    cboHuyen.EditValue = null;
                    cboHuyen.Focus();
                    cboHuyen.ShowPopup();
                    PopupProcess.SelectFirstRowPopup(cboHuyen);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboHuyen.EditValue = listResult[0].DISTRICT_CODE;
                        txtMaHuyen.Text = listResult[0].DISTRICT_CODE;
                        if (_LoadComboCommune != null) _LoadComboCommune(listResult[0].DISTRICT_CODE);
                        //LoadXaCombo("", listResult[0].DISTRICT_CODE, false);
                        if (isExpand)
                        {
                            if (_FocusComboCommune != null) _FocusComboCommune();
                            //txtMaXaPhuong.Focus();
                            //txtMaXaPhuong.SelectAll();
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        //cboXaPhuong.Properties.DataSource = null;
                        //cboXaPhuong.EditValue = null;
                        //txtMaXaPhuong.Text = "";
                        if (_SetValueCommune != null) _SetValueCommune();
                        txtMaHuyen.Text = "";
                        cboHuyen.EditValue = null;
                        if (isExpand)
                        {
                            cboHuyen.Focus();
                            cboHuyen.ShowPopup();
                            PopupProcess.SelectFirstRowPopup(cboHuyen);
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
