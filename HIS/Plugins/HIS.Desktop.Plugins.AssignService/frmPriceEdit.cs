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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService
{
    public partial class frmPriceEdit : Form
    {
        SereServADO sereServADOOld;
        Action<SereServADO> actEditedHandler;
        GetPriceBypackageDelegate getPriceBypackageDelegate;
        GetPriceBySurgDelegate getPriceBySurgDelegate;
        int ActionType;

		public frmPriceEdit(SereServADO data, Action<SereServADO> actEdited, int actionType, GetPriceBypackageDelegate getPriceBypackageDelegate, GetPriceBySurgDelegate getPriceBySurgDelegate)
        {
            InitializeComponent();
            this.sereServADOOld = data;
            this.actEditedHandler = actEdited;
            this.ActionType = actionType;
            this.getPriceBypackageDelegate = getPriceBypackageDelegate;
            this.getPriceBySurgDelegate = getPriceBySurgDelegate;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.actEditedHandler != null)
                {
                    switch (this.ActionType)
                    {
                        case PriceEditType.EditTypeSurgPrice:
                            sereServADOOld.AssignSurgPriceEdit = this.spinPrice.Value > 0 ? (decimal?)this.spinPrice.Value : null;
                            break;
                        case PriceEditType.EditTypePackagePrice:
                            sereServADOOld.AssignPackagePriceEdit = this.spinPrice.Value > 0 ? (decimal?)this.spinPrice.Value : null;
                            break;
                        default:
                            break;
                    }
                    this.actEditedHandler(sereServADOOld);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmPriceEdit_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                switch (this.ActionType)
                {
                    case PriceEditType.EditTypeSurgPrice:
                        InitPriceBySurg();
                        break;
                    case PriceEditType.EditTypePackagePrice:
                        InitPriceByPackage();
                        break;                
                    default:
                        break;
                }
                this.spinPrice.Focus();
                this.spinPrice.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmPriceEdit
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(frmPriceEdit).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPriceEdit.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnOK.Text = Inventec.Common.Resource.Get.Value("frmPriceEdit.btnOK.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmPriceEdit.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmPriceEdit.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barbtnOK.Caption = Inventec.Common.Resource.Get.Value("frmPriceEdit.barbtnOK.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPriceEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void InitPriceByPackage()
        {
            if (this.sereServADOOld.AssignPackagePriceEdit > 0)
            {
                this.spinPrice.EditValue = this.sereServADOOld.AssignPackagePriceEdit;
            }
            else
                this.spinPrice.EditValue = this.getPriceBypackageDelegate(this.sereServADOOld);
        }
      
        private void InitPriceBySurg()
        {
            if (this.sereServADOOld.AssignSurgPriceEdit > 0)
            {
                this.spinPrice.EditValue = this.sereServADOOld.AssignSurgPriceEdit;
            }
            else if (this.getPriceBySurgDelegate != null)
            {
                this.spinPrice.EditValue = this.getPriceBySurgDelegate(this.sereServADOOld);
            }
        }

        private void barbtnOK_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnOK_Click(null, null);
        }
    }

    internal class PriceEditType
    {
        internal const int EditTypeSurgPrice = 1;
        internal const int EditTypePackagePrice = 2;
    }

    public delegate decimal? GetPriceBypackageDelegate(SereServADO sereServADOOld);
    public delegate decimal? GetPriceBySurgDelegate(SereServADO sereServADOOld);
}
