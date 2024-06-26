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

namespace HIS.Desktop.Plugins.InteractiveGrade.Run
{
    partial class frmInteractiveGrade
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.InteractiveGrade.Resources.Lang", typeof(HIS.Desktop.Plugins.InteractiveGrade.Run.frmInteractiveGrade).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                /*
                this.lciCheckItemTypeName.Text = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.lciCheckItemTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.STT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLock.Caption = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnLock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLock.ToolTip = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnLock.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDelete.Caption = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDelete.ToolTip = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnDelete.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCheckItemTypeName.Caption = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnCheckItemTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCheckItemTypeName.ToolTip = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnCheckItemTypeName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnNumOrder.Caption = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnNumOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnNumOrder.ToolTip = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnNumOrder.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnIsActive.Caption = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnIsActive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnIsActive.ToolTip = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnIsActive.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreator.Caption = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifier.Caption = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.gridColumnModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmInteractiveGrade.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                */
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
