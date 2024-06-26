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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisPtttTable.Resources;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisPtttTable.HisPtttTable
{
    public partial class frmHisPtttTable : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguagekey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisPtttTable.Resources.Lang", typeof(HIS.Desktop.Plugins.HisPtttTable.HisPtttTable.frmHisPtttTable).Assembly);
                this.lcPtttTableCode.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.lcPtttTableCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcPtttTableName.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.lcPtttTableName.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcExcuteID.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.lcExcuteID.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.btnAdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.btnEdit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.btnSearch.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRest.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.btnRest.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableCode.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColPtttTableCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColPtttTableCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableName.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColPtttTableName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColPtttTableName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColCreateTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColCreateTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColCreator.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColCreator.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsActive.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColIsActive.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsActive.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColIsActive.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColModifyTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColModifyTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColModifier.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColModifier.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExcuterRoomId.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColExcuterRoomId.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExcuterRoomId.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColExcuterRoomId.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.btnImport.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
               Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
