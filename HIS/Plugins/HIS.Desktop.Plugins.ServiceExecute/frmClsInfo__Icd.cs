/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using HIS.Desktop.Plugins.ServiceExecute.EkipTemp;
using HIS.Desktop.Plugins.ServiceExecute.ValidationRule;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmClsInfo : Form
    {
        //private async Task ComboMethodICD()
        //{
        //    try
        //    {
        //        this.autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
        //        chkIcd1.Enabled = (this.autoCheckIcd != 2);
        //        chkIcd2.Enabled = (this.autoCheckIcd != 2);
        //        chkIcd3.Enabled = (this.autoCheckIcd != 2);
        //        List<HIS_ICD> datas = null;
        //        if (BackendDataWorker.IsExistsKey<HIS_ICD>())
        //        {
        //            datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>();
        //        }
        //        else
        //        {
        //            CommonParam paramCommon = new CommonParam();
        //            dynamic filter = new System.Dynamic.ExpandoObject();
        //            datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_ICD>>("api/HisIcd/Get", ApiConsumers.MosConsumer, filter, paramCommon);

        //            if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_ICD), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
        //        }

        //        this.dataIcds = new List<HIS_ICD>();
        //        if (datas != null && datas.Count > 0)
        //            this.dataIcds = datas.Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

        //        DataToComboChuanDoanTD(cboIcd1);
        //        DataToComboChuanDoanTD(cboIcd2);
        //        DataToComboChuanDoanTD(cboIcd3);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}s

        private void DataToComboChuanDoanTD(CustomGridLookUpEditWithFilterMultiColumn cbo)
        {
            try
            {
                List<IcdADO> listADO = new List<IcdADO>();
                foreach (var item in dataIcds)
                {
                    IcdADO icd = new IcdADO();
                    icd.ID = item.ID;
                    icd.ICD_CODE = item.ICD_CODE;
                    icd.ICD_NAME = item.ICD_NAME;
                    icd.ICD_NAME_UNSIGN = convertToUnSign3(item.ICD_NAME);
                    listADO.Add(icd);
                }

                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "ICD_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("ICD_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["ICD_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private void FillDataToCboIcd(
            TextEdit txtIcdCode,
            TextEdit txtIcdMain,
            CustomGridLookUpEditWithFilterMultiColumn cbo,
            CheckEdit chkEdit,
            string _ICD_CODE,
            string _ICD_NAME)
        {
            try
            {
                if (!string.IsNullOrEmpty(_ICD_CODE))
                {
                    var icd = this.dataIcds.Where(p => p.ICD_CODE == _ICD_CODE).FirstOrDefault();
                    if (icd != null)
                    {
                        txtIcdCode.Text = icd.ICD_CODE;
                        cbo.EditValue = icd.ID;
                        if (this.autoCheckIcd == 1
                            || (!String.IsNullOrEmpty(_ICD_NAME)
                            && (_ICD_NAME ?? "").Trim().ToLower() != (icd.ICD_NAME ?? "").Trim().ToLower()))
                        {
                            chkEdit.Checked = (this.autoCheckIcd != 2);
                            txtIcdMain.Text = _ICD_NAME;
                        }
                        else
                        {
                            chkEdit.Checked = false;
                            txtIcdMain.Text = icd.ICD_NAME;
                        }
                    }
                    else
                    {
                        txtIcdCode.Text = null;
                        cbo.EditValue = null;
                        txtIcdMain.Text = null;
                        chkEdit.Checked = false;
                    }
                }
                else
                {
                    txtIcdCode.Text = null;
                    cbo.EditValue = null;
                    txtIcdMain.Text = null;
                    chkEdit.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ReadOnlyICD(bool isReadOnly, TextEdit txtIcdCode,
            TextEdit txtIcdMain,
            CustomGridLookUpEditWithFilterMultiColumn cbo,
            CheckEdit chkEdit)
        {
            try
            {
                if (isReadOnly)
                {
                    txtIcdCode.ReadOnly = true;
                    txtIcdMain.ReadOnly = true;
                    cbo.ReadOnly = true;
                    chkEdit.ReadOnly = true;
                    cbo.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    txtIcdCode.ReadOnly = false;
                    txtIcdMain.ReadOnly = false;
                    cbo.ReadOnly = false;
                    chkEdit.ReadOnly = false;
                    cbo.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void EnableICD(bool isEnable, TextEdit txtIcdCode,
           TextEdit txtIcdMain,
           CustomGridLookUpEditWithFilterMultiColumn cbo,
           CheckEdit chkEdit)
        {
            try
            {
                txtIcdCode.Enabled = isEnable;
                txtIcdMain.Enabled = isEnable;
                cbo.Enabled = isEnable;
                chkEdit.Enabled = isEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadIcdCombo(string searchCode,
            TextEdit txtIcdCode,
           TextEdit txtIcdMain,
           CustomGridLookUpEditWithFilterMultiColumn cbo,
           CheckEdit chkEdit)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = this.dataIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtIcdCode.Text = result.First().ICD_CODE;
                        txtIcdMain.Text = result.First().ICD_NAME;
                        cbo.EditValue = listData.First().ID;
                        chkEdit.Checked = (chkEdit.Enabled ? (this.autoCheckIcd == 1) : false);

                        if (chkEdit.Checked)
                        {
                            txtIcdMain.Focus();
                            txtIcdMain.SelectAll();
                        }
                        else
                        {
                            cbo.Focus();
                            cbo.SelectAll();
                        }

                        //if (this.refeshIcd != null)
                        //{
                        //    Inventec.Common.Logging.LogSystem.Debug("this.refeshIcd.execute");
                        //    this.refeshIcd(listData.First());
                        //}
                    }
                }

                if (showCbo)
                {
                    cbo.Focus();
                    cbo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ValidationICD(int? maxLengthCode,
            int? maxLengthText, bool isRequired,
            TextEdit txtIcdCode,
           TextEdit txtIcdMain,
           CustomGridLookUpEditWithFilterMultiColumn cbo,
            CheckEdit chkEdit,
            DevExpress.XtraLayout.LayoutControlItem lciIcdText)
        {
            try
            {
                if (isRequired)
                {
                    lciIcdText.AppearanceItemCaption.ForeColor = Color.Maroon;

                    ICDValidationRuleControl icdMainRule = new ICDValidationRuleControl();
                    icdMainRule.txtIcdCode = txtIcdCode;
                    icdMainRule.btnBenhChinh = cbo;
                    icdMainRule.txtMainText = txtIcdMain;
                    icdMainRule.chkCheck = chkEdit;
                    icdMainRule.maxLengthCode = maxLengthCode;
                    icdMainRule.maxLengthText = maxLengthText;
                    icdMainRule.ErrorText = "Trường dữ liệu bắt buộc";
                    icdMainRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(txtIcdCode, icdMainRule);
                }
                else
                {
                    lciIcdText.AppearanceItemCaption.ForeColor = new System.Drawing.Color();
                    txtIcdCode.ErrorText = "";
                    dxValidationProvider1.RemoveControlError(txtIcdCode);
                    dxValidationProvider1.SetValidationRule(txtIcdCode, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD(TextEdit txtIcdCode,
           TextEdit txtIcdMain,
           CustomGridLookUpEditWithFilterMultiColumn cbo,
            CheckEdit chkEdit,
            TextEdit nextFocus)
        {
            try
            {
                cbo.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_ICD icd = dataIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbo.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtIcdCode.Text = icd.ICD_CODE;
                    txtIcdMain.Text = icd.ICD_NAME;
                    chkEdit.Checked = (chkEdit.Enabled ? (this.autoCheckIcd == 1) : false);
                    if (chkEdit.Checked && nextFocus != null)
                    {
                        nextFocus.Focus();
                        nextFocus.SelectAll();
                    }
                    else if (chkEdit.Enabled)
                    {
                        chkEdit.Focus();
                    }
                    else
                    {
                        if (nextFocus != null)
                        {
                            nextFocus.Focus();
                            nextFocus.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        
    }
}
