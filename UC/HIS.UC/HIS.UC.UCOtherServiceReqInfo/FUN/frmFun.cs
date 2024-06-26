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
using HIS.UC.UCOtherServiceReqInfo.Resources;
using HIS.UC.UCOtherServiceReqInfo.Valid;
using Inventec.Desktop.Common.LanguageManager;
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

namespace HIS.UC.UCOtherServiceReqInfo.FUN
{
    public partial class frmFun : HIS.Desktop.Utility.FormBase
    {
        public delegate void GetString(MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment);
        public GetString MyGetData;
        private int positionHandle = -1;
        private HIS_TREATMENT _HisTreatment;

        public frmFun()
        {
            InitializeComponent();
        }

        public frmFun(HIS_TREATMENT _hisTreatment)
        {
            InitializeComponent();
            try
            {
                this._HisTreatment = _hisTreatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmFun_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetIcon();
                SetData();
                //ValidationTime(this.dtThoiHanTu);//21504 - bo
                //ValidationTime(this.dtThoiHanDen);
                //ValidationSpin(this.spinHanMuc);
                ValidTextControlMaxlength(this.txtSoThe, 255, true);
                ValidTextControlMaxlength(this.txtTenKhachHang, 200, false);
                ValidTextControlMaxlength(this.txtCongTy, 200, false);
                ValidTextControlMaxlength(this.txtSanPham, 200, false);
                this.txtSoThe.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmFun
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCOtherServiceReqInfo.Resources.Lang", typeof(frmFun).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmFun.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAddInFor.ToolTip = Inventec.Common.Resource.Get.Value("frmFun.btnAddInFor.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmFun.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmFun.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmFun.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmFun.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmFun.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmFun.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmFun.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmFun.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmFun.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmFun.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmFun.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frmFun.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmFun.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void SetData()
        {
            try
            {
                this.txtSoThe.Text = _HisTreatment.FUND_NUMBER;
                if (_HisTreatment.FUND_BUDGET > 0)
                {
                    this.spinHanMuc.Value = _HisTreatment.FUND_BUDGET ?? 0;
                }
                else
                {
                    this.spinHanMuc.EditValue = null;
                }
                this.txtCongTy.Text = _HisTreatment.FUND_COMPANY_NAME;
                if (_HisTreatment.FUND_FROM_TIME > 0)
                {
                    dtThoiHanTu.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_HisTreatment.FUND_FROM_TIME ?? 0) ?? DateTime.Now;
                }
                if (_HisTreatment.FUND_TO_TIME > 0)
                {
                    dtThoiHanDen.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_HisTreatment.FUND_TO_TIME ?? 0) ?? DateTime.Now;
                }
                if (_HisTreatment.FUND_ISSUE_TIME > 0)
                {
                    dtNgayCap.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_HisTreatment.FUND_ISSUE_TIME ?? 0) ?? DateTime.Now;
                }

                this.txtSanPham.Text = _HisTreatment.FUND_TYPE_NAME;
                this.txtTenKhachHang.Text = _HisTreatment.FUND_CUSTOMER_NAME;
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

        private void ValidationTime(DevExpress.XtraEditors.DateEdit dtTime)
        {
            try
            {
                TimeValidationRule _Rule = new TimeValidationRule();
                _Rule.dtTime = dtTime;
                _Rule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _Rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtTime, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationText(DevExpress.XtraEditors.TextEdit txtText)
        {
            try
            {
                TextValidationRule _Rule = new TextValidationRule();
                _Rule.txtText = txtText;
                _Rule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _Rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtText, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSpin(DevExpress.XtraEditors.SpinEdit spinEdit)
        {
            try
            {
                SpinValidationRule _Rule = new SpinValidationRule();
                _Rule.spinEdit = spinEdit;
                _Rule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _Rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(spinEdit, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Focus();
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate()) return;

                MOS.EFMODEL.DataModels.HIS_TREATMENT _treatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                _treatment.FUND_NUMBER = this.txtSoThe.Text;
                _treatment.FUND_BUDGET = this.spinHanMuc.Value;
                _treatment.FUND_COMPANY_NAME = this.txtCongTy.Text;
                if (dtThoiHanTu.EditValue != null && dtThoiHanTu.DateTime != DateTime.MinValue)
                {
                    _treatment.FUND_FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtThoiHanTu.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtThoiHanDen.EditValue != null && dtThoiHanDen.DateTime != DateTime.MinValue)
                {
                    _treatment.FUND_TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                             Convert.ToDateTime(dtThoiHanDen.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtNgayCap.EditValue != null && dtNgayCap.DateTime != DateTime.MinValue)
                {
                    _treatment.FUND_ISSUE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                             Convert.ToDateTime(dtNgayCap.EditValue).ToString("yyyyMMdd") + "000000");
                }
                _treatment.FUND_TYPE_NAME = this.txtSanPham.Text;
                _treatment.FUND_CUSTOMER_NAME = this.txtTenKhachHang.Text;

                MyGetData(_treatment);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
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

        private void txtSoThe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void spinHanMuc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnSave.Focus();
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

        private void btnAddInFor_Click(object sender, EventArgs e)
        {
            try
            {
                string mess = "";
                if (!string.IsNullOrEmpty(txtSoThe.Text))
                {
                    string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktopn.UCOtherServiceReqInfo.FUN.Value_According_To_Card _Number");
                    if (string.IsNullOrEmpty(key))
                    {
                        mess = "Không tìm thấy giá trị hạn mức với số thẻ " + txtSoThe.Text;
                        Inventec.Common.Logging.LogSystem.Error("--- Giá trị key HIS.Desktopn.UCOtherServiceReqInfo.FUN.Value_According_To_Card _Number ---------- null");
                    }
                    else
                    {
                        string[] strSplit = key.Split(',');
                        string keyValue = "";
                        foreach (var item in strSplit)
                        {
                            string[] strSplitV2 = item.Split(':');
                            if (txtSoThe.Text.Trim() == strSplitV2[0].Trim())// item.Substring(0,item.IndexOf(':')))
                            {
                                keyValue = strSplitV2[1].Trim();
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(keyValue))
                        {
                            mess = "Không tìm thấy giá trị hạn mức với số thẻ " + txtSoThe.Text;
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
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                _HisTreatment = null;
                positionHandle = 0;
                MyGetData = null;
                this.btnAddInFor.Click -= new System.EventHandler(this.btnAddInFor_Click);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.dtThoiHanDen.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtThoiHanDen_Closed);
                this.dtThoiHanDen.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtThoiHanDen_KeyDown);
                this.dtThoiHanTu.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtThoiHanTu_Closed);
                this.dtThoiHanTu.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtThoiHanTu_KeyDown);
                this.dtNgayCap.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtNgayCap_Closed);
                this.dtNgayCap.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtNgayCap_KeyDown);
                this.txtCongTy.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCongTy_PreviewKeyDown);
                this.spinHanMuc.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinHanMuc_PreviewKeyDown);
                this.txtTenKhachHang.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTenKhachHang_PreviewKeyDown);
                this.txtSanPham.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSanPham_PreviewKeyDown);
                this.txtSoThe.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSoThe_PreviewKeyDown);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.barButtonItem__Save.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__Save_ItemClick);
                this.Load -= new System.EventHandler(this.frmFun_Load);
                layoutControlItem11 = null;
                btnAddInFor = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem__Save = null;
                bar1 = null;
                barManager1 = null;
                dxValidationProvider1 = null;
                layoutControlItem10 = null;
                labelControl1 = null;
                emptySpaceItem1 = null;
                layoutControlItem9 = null;
                layoutControlItem8 = null;
                layoutControlItem7 = null;
                layoutControlItem6 = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                txtSoThe = null;
                txtSanPham = null;
                txtTenKhachHang = null;
                spinHanMuc = null;
                txtCongTy = null;
                dtNgayCap = null;
                dtThoiHanTu = null;
                dtThoiHanDen = null;
                btnSave = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
