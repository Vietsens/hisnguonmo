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
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AllocateLiquExpMestCreate
{
    public partial class UCAllocateLiquExpMestCreate : UserControl
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AllocateLiquExpMestCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.AllocateLiquExpMestCreate.UCAllocateLiquExpMestCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMedistock.Properties.NullText = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.cboMedistock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageRequestMedicineResult.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.xtraTabPageExpMestMedi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageRequestMaterialResult.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.xtraTabPageExpMestMate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.xtraTabPageExportMedicineResult.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.xtraTabPageExportMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.xtraTabPageExportMaterialResult.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.xtraTabPageExportMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.xtraTabPageMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.xtraTabPageMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.ddBtnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestDetail_Delete.Caption = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.gridColumn_ExpMestDetail_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestDetail_MediMateTypeName.Caption = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.gridColumn_ExpMestDetail_MediMateTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestDetail_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.gridColumn_ExpMestDetail_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestDetail_ExpAmount.Caption = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.gridColumn_ExpMestDetail_ExpAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestDetail_NationalName.Caption = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.gridColumn_ExpMestDetail_NationalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestDetail_ManufacturerName.Caption = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.gridColumn_ExpMestDetail_ManufacturerName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.lciAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpPrice.Text = Inventec.Common.Resource.Get.Value("lciExpPrice", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciVat.Text = Inventec.Common.Resource.Get.Value("lciVat", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestDetail_ExpPrice.Caption = Inventec.Common.Resource.Get.Value("gridColumn_ExpMestDetail_ExpPrice", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestDetail_VatRatio.Caption = Inventec.Common.Resource.Get.Value("gridColumn_ExpMestDetail_VatRatio", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpMestMedistock.Text = Inventec.Common.Resource.Get.Value("UCAllocateLiquExpMestCreate.lciExpMestMedistock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
