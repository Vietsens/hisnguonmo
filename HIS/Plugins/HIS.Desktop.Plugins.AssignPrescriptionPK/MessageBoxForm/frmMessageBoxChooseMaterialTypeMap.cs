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
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Resources;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm
{
    public partial class frmMessageBoxChooseMaterialTypeMap : DevExpress.XtraEditors.XtraForm
    {
        Action<EnumOptionChonVatTuTuongDuong> optionChonVatTuTuongDuong;
        public frmMessageBoxChooseMaterialTypeMap(Action<EnumOptionChonVatTuTuongDuong> _optionChonVatTuTuongDuong, string materialTypeName)
        {
            InitializeComponent();
            this.optionChonVatTuTuongDuong = _optionChonVatTuTuongDuong;
            SetCaptionByLanguageKey();
            lblDescription.Text = String.Format(lblDescription.Text, materialTypeName);
        }

        private void frmMessageBoxChooseMedicineTypeAcin_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.optionChonVatTuTuongDuong == null)
                {
                    throw new ArgumentNullException("chonThuocTrongKhoCungHoatChat is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmMessageBoxChooseMaterialTypeMap
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseMaterialTypeMap = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmMessageBoxChooseMaterialTypeMap).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lblDescription.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxChooseMaterialTypeMap.lblDescription.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseMaterialTypeMap, LanguageManager.GetCulture());
                this.btnChonThuocCungHoatChat.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxChooseMaterialTypeMap.btnChonThuocCungHoatChat.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseMaterialTypeMap, LanguageManager.GetCulture());
                this.btnChonThuocNgoaiKho.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxChooseMaterialTypeMap.btnChonThuocNgoaiKho.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseMaterialTypeMap, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxChooseMaterialTypeMap.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseMaterialTypeMap, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMessageBoxChooseMaterialTypeMap.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMessageBoxChooseMaterialTypeMap, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void btnChonThuocCungHoatChat_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.optionChonVatTuTuongDuong != null)
                {
                    this.optionChonVatTuTuongDuong(EnumOptionChonVatTuTuongDuong.VatTuTUongDuong);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChonThuocNgoaiKho_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.optionChonVatTuTuongDuong != null)
                {
                    this.optionChonVatTuTuongDuong(EnumOptionChonVatTuTuongDuong.VatTuMuaNgoai);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.optionChonVatTuTuongDuong != null)
                {
                    this.optionChonVatTuTuongDuong(EnumOptionChonVatTuTuongDuong.None);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
