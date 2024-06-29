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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.DHST.ADO;
using HIS.UC.DHST.Base;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.DHST.Run
{
    public partial class UCDHST : UserControl
    {
        public void InitLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLangManager.InitResourceLanguageManager();

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControl1.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.labelControl12.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl12.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.labelControl10.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl10.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.labelControl9.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl9.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.labelControl8.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl8.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.labelControl7.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl7.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl6.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl5.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.labelControl4.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl4.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl3.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciMach.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciMach.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciNhietDo.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem2.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciHuyetAp.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem3.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciNhipTho.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem5.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciCanNang.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem6.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciChieuCao.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem7.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciVongNguc.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem8.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciVongBung.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem9.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lblSPO2.Text = Inventec.Common.Resource.Get.Value("UCDHST.lblSPO2.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem10.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem11.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem17.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciNote.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciNote.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());

                this.lciUrine.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciUrine.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciCapillaryBloodGlucose.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciCapillaryBloodGlucose.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciCapillaryBloodGlucose.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCDHST.lciCapillaryBloodGlucose.OptionsToolTip.ToolTip", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.spinCapillaryBloodGlucose.ToolTip = Inventec.Common.Resource.Get.Value("UCDHST.spinCapillaryBloodGlucose.ToolTip", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());

                this.btnSetDHST.ToolTip = Inventec.Common.Resource.Get.Value("UCDHST.btnSetDHST.ToolTip", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
