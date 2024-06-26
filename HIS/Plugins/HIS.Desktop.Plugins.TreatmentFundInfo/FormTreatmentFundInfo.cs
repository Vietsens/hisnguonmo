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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.TreatmentFundInfo.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFundInfo
{
    public partial class FormTreatmentFundInfo : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module ModuleData;
        private HIS_TREATMENT Treatment;
        private List<HIS_FUND> DataFunds;
        private int positionHandle = -1;
        private RefeshReference Refesh;

        public FormTreatmentFundInfo()
        {
            InitializeComponent();
        }

        public FormTreatmentFundInfo(Inventec.Desktop.Common.Modules.Module moduleData, HIS_TREATMENT treatment, RefeshReference _refeshReference)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.Text = moduleData.text;
                this.ModuleData = moduleData;
                this.Treatment = treatment;
                this.Refesh = _refeshReference;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormTreatmentFundInfo_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                SetCaptionByLanguageKey();
                LoadCboFund();
                SetData();
                ValidTextControlMaxlength(this.TxtSoThe, 255, true);
                ValidTextControlMaxlength(this.txtTenKhachHang, 200, false);
                ValidTextControlMaxlength(this.txtCongTy, 200, false);
                ValidTextControlMaxlength(this.txtSanPham, 200, false);
                this.CboCCT.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidTextControlMaxlength(DevExpress.XtraEditors.TextEdit control, int maxlength, bool isVali)
        {
            try
            {
                TextEditMaxLengthValidationRule _rule = new TextEditMaxLengthValidationRule();
                _rule.txtEdit = control;
                _rule.maxlength = maxlength;
                _rule.isVali = isVali;
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(control, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetData()
        {
            try
            {
                this.CboCCT.EditValue = Treatment.FUND_ID;
                this.TxtSoThe.Text = Treatment.FUND_NUMBER;
                this.txtSanPham.Text = Treatment.FUND_TYPE_NAME;
                if (!String.IsNullOrWhiteSpace(Treatment.FUND_CUSTOMER_NAME))
                {
                    this.txtTenKhachHang.Text = Treatment.FUND_CUSTOMER_NAME;
                }
                else
                {
                    this.txtTenKhachHang.Text = Treatment.TDL_PATIENT_NAME;
                }

                this.txtCongTy.Text = Treatment.FUND_COMPANY_NAME;
                if (Treatment.FUND_BUDGET > 0)
                {
                    this.spinHanMuc.Value = Treatment.FUND_BUDGET ?? 0;
                }
                else
                {
                    this.spinHanMuc.EditValue = null;
                }

                if (Treatment.FUND_FROM_TIME > 0)
                {
                    dtThoiHanTu.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Treatment.FUND_FROM_TIME ?? 0) ?? DateTime.Now;
                }

                if (Treatment.FUND_TO_TIME > 0)
                {
                    dtThoiHanDen.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Treatment.FUND_TO_TIME ?? 0) ?? DateTime.Now;
                }

                if (Treatment.FUND_ISSUE_TIME > 0)
                {
                    dtNgayCap.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Treatment.FUND_ISSUE_TIME ?? 0) ?? DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboFund()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisFundFilter filter = new MOS.Filter.HisFundFilter();
                DataFunds = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_FUND>>("api/HisFund/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                InitComboCommon(this.CboCCT, DataFunds, "ID", "FUND_NAME", "FUND_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }

                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }

                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                BtnSave.Focus();
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate()) return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_TREATMENT _treatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                ProcessUpdateData(ref _treatment);
                CommonParam param = new CommonParam();
                bool success = false;

                var apiresult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateFundInfo", ApiConsumers.MosConsumer, _treatment, param);
                if (apiresult != null)
                {
                    success = true;
                    if (Refesh != null)
                    {
                        Refesh();
                    }

                    this.Close();
                }

                WaitingManager.Hide();

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessUpdateData(ref HIS_TREATMENT _treatment)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(_treatment, Treatment);

                _treatment.FUND_ID = Inventec.Common.TypeConvert.Parse.ToInt64((CboCCT.EditValue ?? "0").ToString());
                _treatment.FUND_NUMBER = this.TxtSoThe.Text;
                _treatment.FUND_BUDGET = this.spinHanMuc.Value;
                _treatment.FUND_COMPANY_NAME = this.txtCongTy.Text;
                _treatment.FUND_TYPE_NAME = this.txtSanPham.Text;
                _treatment.FUND_CUSTOMER_NAME = this.txtTenKhachHang.Text;
                if (dtThoiHanTu.EditValue != null && dtThoiHanTu.DateTime != DateTime.MinValue)
                {
                    _treatment.FUND_FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtThoiHanTu.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    _treatment.FUND_FROM_TIME = null;
                }

                if (dtThoiHanDen.EditValue != null && dtThoiHanDen.DateTime != DateTime.MinValue)
                {
                    _treatment.FUND_TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                             Convert.ToDateTime(dtThoiHanDen.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    _treatment.FUND_TO_TIME = null;
                }

                if (dtNgayCap.EditValue != null && dtNgayCap.DateTime != DateTime.MinValue)
                {
                    _treatment.FUND_ISSUE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                             Convert.ToDateTime(dtNgayCap.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    _treatment.FUND_ISSUE_TIME = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }

                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddInFor_Click(object sender, EventArgs e)
        {
            try
            {
                string mess = "";
                if (!string.IsNullOrEmpty(TxtSoThe.Text))
                {
                    string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktopn.UCOtherServiceReqInfo.FUN.Value_According_To_Card _Number");
                    if (string.IsNullOrEmpty(key))
                    {
                        mess = "Không tìm thấy giá trị hạn mức với số thẻ " + TxtSoThe.Text;
                        Inventec.Common.Logging.LogSystem.Error("--- Giá trị key HIS.Desktopn.UCOtherServiceReqInfo.FUN.Value_According_To_Card _Number ---------- null");
                    }
                    else
                    {
                        string[] strSplit = key.Split(',');
                        string keyValue = "";
                        foreach (var item in strSplit)
                        {
                            string[] strSplitV2 = item.Split(':');
                            if (TxtSoThe.Text.Trim() == strSplitV2[0].Trim())// item.Substring(0,item.IndexOf(':')))
                            {
                                keyValue = strSplitV2[1].Trim();
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(keyValue))
                        {
                            mess = "Không tìm thấy giá trị hạn mức với số thẻ " + TxtSoThe.Text;
                        }
                        else
                        {
                            this.spinHanMuc.Value = Inventec.Common.TypeConvert.Parse.ToDecimal(keyValue);
                        }
                    }
                }
                else
                {
                    mess = "Số thẻ không được để trống";
                }
                if (!string.IsNullOrEmpty(mess))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(mess, "Thông báo");
                    return;
                }
                //#17848
                //Sản phẩm: Điền mặc định là "Bảo Việt An Gia" 
                txtSanPham.Text = "Bảo Việt An Gia";
                //- Thời hạn từ: Điền mặc định là 01/01/2019
                dtThoiHanTu.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(20190101000000) ?? new DateTime();
                //- Thời hạn đến: Điền mặc định là 31/12/2019
                dtThoiHanDen.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(20191231000000) ?? new DateTime();
                // Ngày cấp: Điền mặc định là 01/01/2019
                dtNgayCap.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(20190101000000) ?? new DateTime();
                //- Hạn mức: lấy theo thẻ cấu hình, căn cứ vào "số thẻ" mà người dùng nhập
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtSoThe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtSanPham.Focus();
                    this.txtSanPham.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSanPham_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtThoiHanTu.Focus();
                    this.dtThoiHanTu.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtThoiHanTu_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtThoiHanDen.Focus();
                    this.dtThoiHanDen.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtThoiHanTu_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtThoiHanDen.Focus();
                    this.dtThoiHanDen.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtThoiHanDen_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.txtTenKhachHang.Focus();
                    this.txtTenKhachHang.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtThoiHanDen_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtTenKhachHang.Focus();
                    this.txtTenKhachHang.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTenKhachHang_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtCongTy.Focus();
                    this.txtCongTy.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCongTy_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtNgayCap.Focus();
                    this.dtNgayCap.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtNgayCap_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.spinHanMuc.Focus();
                    this.spinHanMuc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtNgayCap_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.spinHanMuc.Focus();
                    this.spinHanMuc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinHanMuc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.BtnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnSave_Click(null, null);
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentFundInfo.Resources.Lang", typeof(HIS.Desktop.Plugins.TreatmentFundInfo.FormTreatmentFundInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnSave.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.BtnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboCCT.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.CboCCT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciCboCct.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.LciCboCct.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciTxtSoThe.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.LciTxtSoThe.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciThoiHanDen.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.lciThoiHanDen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciSanPham.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.LciSanPham.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciThoiHanTu.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.LciThoiHanTu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciTenKhachHang.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.LciTenKhachHang.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciCongTy.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.LciCongTy.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciNgayCap.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.LciNgayCap.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciHanMuc.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.LciHanMuc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentFundInfo.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboCCT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtSoThe.Focus();
                    TxtSoThe.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    CboCCT.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
