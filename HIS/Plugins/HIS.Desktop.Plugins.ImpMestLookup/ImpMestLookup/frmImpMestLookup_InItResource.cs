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

namespace HIS.Desktop.Plugins.ImpMestLookup.ImpMestLookup
{
    public partial class frmImpMestLookup : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ImpMestLookup.Resources.Lang", typeof(HIS.Desktop.Plugins.ImpMestLookup.ImpMestLookup.frmImpMestLookup).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmImpMestLookup.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.bbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnApproval.Text = Inventec.Common.Resource.Get.Value("frmImpMestLookup.btnApproval.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmImpMestLookup.btnExport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.bbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmImpMestLookup.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmImpMestLookup.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPrint.Text = Inventec.Common.Resource.Get.Value("frmImpMestLookup.cboPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPageMedicine.Text = Inventec.Common.Resource.Get.Value("frmImpMestLookup.tabPageMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAmount.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("frmImpMestLookup.repositoryItemPictureEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPageMaterial.Text = Inventec.Common.Resource.Get.Value("frmImpMestLookup.tabPageMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateStt.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateMaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateMaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateMaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateMaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateAmount.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnTotalImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateMaterialTypeTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateMaterialTypeTotalImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateMaterialTypeImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateMaterialTypeImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateMaterialTypeImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnMateMaterialTypeImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit2.NullText = Inventec.Common.Resource.Get.Value("frmImpMestLookup.repositoryItemPictureEdit2.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPageBlood.Text = Inventec.Common.Resource.Get.Value("frmImpMestLookup.tabPageBlood.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBloodSTT.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnBloodSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBloodBloodTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnBloodBloodTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBloodBloodTypeName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnBloodBloodTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBloodBloodAboCode.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnBloodBloodAboCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBloodBloodRhCode.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnBloodBloodRhCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBloodServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnBloodServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBloodImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnBloodImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnVat.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnVat.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSumPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnSumPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmImpMestLookup.gridColumnExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
