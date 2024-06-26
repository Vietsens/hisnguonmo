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
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using HIS.UC.UCTransPati;
using HIS.UC.UCTransPati.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Common;
using System.IO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.RegisterV2
{
    public partial class frmTransPati : FormBase
    {
        bool isValidate { get; set; }
        UCTransPatiADO UCTransPatiADO { get; set; }
        UpdateSelectedTranPati UpdateSelectedTranPati { get; set; }
        DelegateVisible dlgSetValidateForValidTTCT;
        bool _RunValidate { get; set; }
        bool isValidateAll { get; set; }

        public frmTransPati(
            bool _isValidate,
            bool _isValidateAll,
            UCTransPatiADO _ucTransPatiADO,
            UpdateSelectedTranPati _updateSelectedTranPati)
            : this(null)
        {
            this.isValidate = _isValidate;
            this.isValidateAll = _isValidateAll;
            this.UCTransPatiADO = _ucTransPatiADO;
            this.UpdateSelectedTranPati = _updateSelectedTranPati;
        }

        public frmTransPati(
            bool _isValidate,
            UCTransPatiADO _ucTransPatiADO,
            UpdateSelectedTranPati _updateSelectedTranPati,
            bool _runValidate, bool _isValidateAll = false)
            : this(null)
        {
            this._RunValidate = _runValidate;
            this.isValidate = _isValidate;
            this.UCTransPatiADO = _ucTransPatiADO;
            this.UpdateSelectedTranPati = _updateSelectedTranPati;
            this.isValidateAll = _isValidateAll;
        }

        public frmTransPati(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void frmTransPati_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                SetCaptionByLanguageKey();
                this.ucTransPati1.ResetRequiredField(this.isValidate, this.isValidateAll);
                this.ucTransPati1.SetValue(this.UCTransPatiADO);
                this.ucTransPati1.FocusNextUserControl(FocusToButtonSave);
                if (this._RunValidate)
                    this.ucTransPati1.ValidateRequiredField();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }    
        /// <summary>
                ///Hàm xét ngôn ngữ cho giao diện frmTransPati
                /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RegisterV2.Resources.Lang", typeof(frmTransPati).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTransPati.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnClose.Text = Inventec.Common.Resource.Get.Value("frmTransPati.btnClose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTransPati.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbntNhap.Caption = Inventec.Common.Resource.Get.Value("frmTransPati.bbntNhap.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmTransPati.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTransPati.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        ///// <summary>
        /////                 Sửa chức năng "Tiếp đón" (tiếp đón 1 và tiếp đón 2):
        /////Khi nhập thông tin chuyển tuyến, căn cứ vào tuyến của viện mà người dùng đang làm việc (LEVEL_CODE của HIS_BRANCH mà người dùng chọn làm việc) với tuyến của viện mà người dùng nhập "Nơi chuyển đến" để tự động điền "Hình thức chuyển" (LEVEL_CODE của HIS_MEDI_ORG), theo công thức sau:

        /////                - Nếu L2 - L1 = 1 --> chọn "Hình thức chuyển" mã "01" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE)
        /////                - Nếu L2 - L1 > 1 --> chọn "Hình thức chuyển" mã "02" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE)
        /////                - Nếu L2 - L1 < 0 --> chọn "Hình thức chuyển" mã "03" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG)
        /////                - Nếu L2 - L1 = 0 --> chọn "Hình thức chuyển" mã "04" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN)

        /////                Trong đó:
        /////                - LEVEL_CODE của "Tuyến của viện mà người dùng đang làm việc" là L1
        /////                - LEVEL_CODE của "Nơi chuyển đến" là L2

        /////                Lưu ý:
        /////                Hệ thống cũ, dữ liệu LEVEL_CODE của HIS_MEDI_ORG đang lưu dưới dạng text (TW, T, H, X), để tránh việc update cache có thể ảnh hưởng đến hiệu năng, lúc xử lý cần "if-else" để xử lý được với dữ liệu cũ, cụ thể cần check LEVEL_CODE của HIS_MEDI_ORG, gán lại giá trị trước khi tính toán:
        /////                - Nếu LEVEL_CODE = TW --> LEVEL_CODE = 1
        /////                - Nếu LEVEL_CODE = T --> LEVEL_CODE = 2
        /////                - Nếu LEVEL_CODE = H --> LEVEL_CODE = 3
        /////                - Nếu LEVEL_CODE = X --> LEVEL_CODE = 4
        /////                - Khác: --> giữ nguyên giá trị
        ///// </summary>
        //private void ProcessLevelOfMediOrg()
        //{
        //    try
        //    {
        //        string lvBranch = FixWrongLevelCode(BranchDataWorker.Branch.HEIN_LEVEL_CODE);

        //        if (!String.IsNullOrEmpty(txtMaNoiChuyenDen.Text) && cboNoiChuyenDen.EditValue != null)
        //        {
        //            var mediTrans = DataStore.MediOrgs.Where(o => o.MEDI_ORG_CODE == txtMaNoiChuyenDen.Text).FirstOrDefault();
        //            if (mediTrans != null)
        //            {
        //                string lvTrans = FixWrongLevelCode(mediTrans.LEVEL_CODE);

        //                int iLvBranch = int.Parse(lvBranch);
        //                int iLvTrans = int.Parse(lvTrans);
        //                int iKq = iLvTrans - iLvBranch;
        //                MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM tranPatiDefault = null;
        //                if (iKq == 1)
        //                {
        //                    tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE).FirstOrDefault();
        //                }
        //                else if (iKq > 1)
        //                {
        //                    tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE).FirstOrDefault();
        //                }
        //                else if (iKq < 0)
        //                {
        //                    tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG).FirstOrDefault();
        //                }
        //                else if (iKq == 0)
        //                {
        //                    tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN).FirstOrDefault();
        //                }

        //                cboHinhThucChuyen.EditValue = tranPatiDefault != null ? (long?)tranPatiDefault.ID : null;
        //                txtMaHinhThucChuyen.Text = tranPatiDefault != null ? tranPatiDefault.TRAN_PATI_FORM_CODE : "";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private string FixWrongLevelCode(string code)
        //{
        //    string rs = "";
        //    try
        //    {
        //        if (code == "TW")
        //        {
        //            rs = "1";
        //        }
        //        else if (code == "T")
        //        {
        //            rs = "2";
        //        }
        //        else if (code == "H")
        //        {
        //            rs = "3";
        //        }
        //        else if (code == "X")
        //        {
        //            rs = "4";
        //        }
        //        else
        //            rs = code;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return rs;
        //}

        private void FocusToButtonSave(object data)
        {
            try
            {
                btnClose.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbntNhap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnClose_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dlgSetValidateForValidTTCT != null)
                    this.dlgSetValidateForValidTTCT(ValidateRequiredField());

                var transPatiData = this.ucTransPati1.GetValue();
                this.UpdateSelectedTranPati(transPatiData);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessImageDataByControl(DevExpress.XtraEditors.PictureEdit pte, ref UCTransPatiADO data)
        {
            try
            {
                if (pte != null && pte.Image != null && pte.Image.Tag != null && !pte.Image.Tag.Equals("noImage"))
                {
                    MemoryStream memory = new MemoryStream();
                    var bitMap = new System.Drawing.Bitmap(pte.Image);
                    bitMap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);

                    data.ImgTransferInData = memory.ToArray();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {
                valid = this.ucTransPati1.ValidateRequiredField();
                Inventec.Common.Logging.LogSystem.Debug("Get validate tu form TransPati thanh cong. valid = " + valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                valid = false;
                Inventec.Common.Logging.LogSystem.Debug("Get validate tu form TransPati that bai.");
            }
            return valid;
        }

        public void RefreshFormTransPati()
        {
            try
            {
                this.ucTransPati1.RefreshUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValidForTTCT(DelegateVisible _dlgHideForm)
        {
            try
            {
                if (_dlgHideForm != null)
                    this.dlgSetValidateForValidTTCT = _dlgHideForm;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                isValidateAll = false;
                _RunValidate = false;
                dlgSetValidateForValidTTCT = null;
                UpdateSelectedTranPati = null;
                UCTransPatiADO = null;
                isValidate = false;
                this.btnClose.Click -= new System.EventHandler(this.btnClose_Click);
                this.bbntNhap.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbntNhap_ItemClick);
                this.Load -= new System.EventHandler(this.frmTransPati_Load);
                barButtonItem1 = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                btnClose = null;
                layoutControlItem1 = null;
                ucTransPati1 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                bbntNhap = null;
                bar1 = null;
                barManagerUCTransPati = null;
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
