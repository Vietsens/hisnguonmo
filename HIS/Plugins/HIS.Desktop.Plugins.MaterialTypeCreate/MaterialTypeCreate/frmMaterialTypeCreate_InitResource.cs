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
using Inventec.Common.Resource;
using HIS.Desktop.Plugins.MaterialTypeCreate.Resources;

namespace HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate
{
    public partial class frmMaterialTypeCreate : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MaterialTypeCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate.frmMaterialTypeCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lciMaterialTypeCode.Text = Get.Value("frmMaterialTypeCreate.lciMaterialTypeCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Get.Value("frmMaterialTypeCreate.layoutControlItem5.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceUnit.Text = Get.Value("frmMaterialTypeCreate.lciServiceUnit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Get.Value("frmMaterialTypeCreate.layoutControlItem8.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBhytNumOrder.Text = Get.Value("frmMaterialTypeCreate.lciBhytNumOrder.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinServiceBhytCode.Text = Get.Value("frmMaterialTypeCreate.lciHeinServiceBhytCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcMaterialGroupBHYT.Text = Get.Value("frmMaterialTypeCreate.lcMaterialGroupBHYT.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaterialTypeMap.Text = Get.Value("frmMaterialTypeCreate.lcMaterialTypeMapId.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciManufacture.Text = Get.Value("frmMaterialTypeCreate.lciManufacture.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPackingType.Text = Get.Value("frmMaterialTypeCreate.lciPackingType.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConcentra.Text = Get.Value("frmMaterialTypeCreate.lciConcentra.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Get.Value("frmMaterialTypeCreate.lciNumOrder.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsChemicalSubstance.Text = Get.Value("frmMaterialTypeCreate.lciIsChemicalSubstance.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsStent.Text = Get.Value("frmMaterialTypeCreate.lciIsStent.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsRawMaterial.Text = Get.Value("frmMaterialTypeCreate.lciIsRawMaterial.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChkMaxReuseCount.Text = Get.Value("frmMaterialTypeCreate.lciChkMaxReuseCount.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSpinMaxReuseCount.Text = Get.Value("frmMaterialTypeCreate.lciSpinMaxReuseCount.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Get.Value("frmMaterialTypeCreate.layoutControlItem42.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem43.Text = Get.Value("frmMaterialTypeCreate.layoutControlItem43.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Get.Value("frmMaterialTypeCreate.layoutControlItem22.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton2.Text = Get.Value("frmMaterialTypeCreate.simpleButton2.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton1.Text = Get.Value("frmMaterialTypeCreate.simpleButton1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Get.Value("frmMaterialTypeCreate.btnSave.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Get.Value("frmMaterialTypeCreate.btnRefresh.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcHeinLimitRatio.Text = Get.Value("frmMaterialTypeCreate.lcHeinLimitRatio.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcHeinLimitRatioOld.Text = Get.Value("frmMaterialTypeCreate.lcHeinLimitRatioOld.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcHeinLimitPriceInTime.Text = Get.Value("frmMaterialTypeCreate.lcHeinLimitPriceInTime.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcIsNoHeinLimitForSpecial.Text = Get.Value("frmMaterialTypeCreate.lcIsNoHeinLimitForSpecial.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcGender.Text = Get.Value("frmMaterialTypeCreate.lcGender.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcAlertMaxInPrescription.Text = Get.Value("frmMaterialTypeCreate.lcAlertMaxInPrescription.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAlertExpiredDate.Text = Get.Value("frmMaterialTypeCreate.lciAlertExpiredDate.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAlertMinInStock.Text = Get.Value("frmMaterialTypeCreate.lciAlertMinInStock.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsStopImp.Text = Get.Value("frmMaterialTypeCreate.lciIsStopImp.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcCPNG.Text = Get.Value("frmMaterialTypeCreate.lcCPNG.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcIsExprireDate.Text = Get.Value("frmMaterialTypeCreate.lcIsExprireDate.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsAllowOdd.Text = Get.Value("frmMaterialTypeCreate.lciIsAllowOdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcIsAllowExportOdd.Text = Get.Value("frmMaterialTypeCreate.lcIsAllowExportOdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcIsBusiness.Text = Get.Value("frmMaterialTypeCreate.lcIsBusiness.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcIsMustPrepare.Text = Get.Value("frmMaterialTypeCreate.lcIsMustPrepare.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcAutoExpend.Text = Get.Value("frmMaterialTypeCreate.lcAutoExpend.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpPrice.Text = Get.Value("frmMaterialTypeCreate.lciImpPrice.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInternalPrice.Text = Get.Value("frmMaterialTypeCreate.lciInternalPrice.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcLastExpPrice.Text = Get.Value("frmMaterialTypeCreate.lcLastExpPrice.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpVatRatio.Text = Get.Value("frmMaterialTypeCreate.lciImpVatRatio.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcLastExpVatPrice.Text = Get.Value("frmMaterialTypeCreate.lcLastExpVatPrice.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcIsSaleEqualImpPrice.Text = Get.Value("frmMaterialTypeCreate.lcIsSaleEqualImpPrice.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem144.Text = Get.Value("frmMaterialTypeCreate.layoutControlItem144.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcHeinLimitPrice.Text = Get.Value("frmMaterialTypeCreate.lcHeinLimitPrice.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcHeinLimitPriceOld.Text = Get.Value("frmMaterialTypeCreate.lcHeinLimitPriceOld.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox1.Text = Get.Value("frmMaterialTypeCreate.groupBox1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox2.Text = Get.Value("frmMaterialTypeCreate.groupBox2.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox3.Text = Get.Value("frmMaterialTypeCreate.groupBox3.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Get.Value("frmMaterialTypeCreate.gridColumn1.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Get.Value("frmMaterialTypeCreate.gridColumn1.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Get.Value("frmMaterialTypeCreate.gridColumn2.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Get.Value("frmMaterialTypeCreate.gridColumn2.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Get.Value("frmMaterialTypeCreate.gridColumn3.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Get.Value("frmMaterialTypeCreate.gridColumn3.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcMaterialType.Text = Get.Value("frmMaterialTypeCreate.lcMaterialType.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcRecordTransaction.Text = Get.Value("frmMaterialTypeCreate.lcRecordTransaction.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcRecordTransaction.OptionsToolTip.ToolTip = Get.Value("frmMaterialTypeCreate.lcRecordTransaction.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaterialTypeMap.OptionsToolTip.ToolTip = Get.Value("frmMaterialTypeCreate.lciMaterialTypeMap.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciCTK.Text = Get.Value("frmMaterialTypeCreate.lciCTK.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsSupported.Text = Get.Value("frmMaterialTypeCreate.lciIsSupported.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciVTDichDanh.Text = Get.Value("frmMaterialTypeCreate.lciVTDichDanh.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciVTDichDanh.OptionsToolTip.ToolTip = Get.Value("frmMaterialTypeCreate.lciVTDichDanh.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
              
                if (this.module != null && !String.IsNullOrEmpty(this.module.text))
                {
                    this.Text = this.module.text;
                }
                else
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void initializecomponent()
        //{
            //this.suspendlayout();
            //// 
            //// frmmaterialtypecreate
            //// 
            //this.clientsize = new system.drawing.size(284, 261);
            //this.name = "frmmaterialtypecreate";
            //this.load += new system.eventhandler(this.frmmaterialtypecreate_load);
            //this.resumelayout(false);

        //}


    }
}
