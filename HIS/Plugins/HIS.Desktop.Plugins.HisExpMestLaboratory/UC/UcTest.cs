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
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisExpMestLaboratory.Resources;

namespace HIS.Desktop.Plugins.HisExpMestLaboratory.UC
{
    public partial class UcTest : UserControl
    {
        int positionHandle = -1;

        public UcTest()
        {
            InitializeComponent();
        }

        private void UcTest_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                dtFrom.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                dtTo.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

                ControlEditValidationRule validateFrom = new ControlEditValidationRule();
                validateFrom.editor = dtFrom;
                validateFrom.ErrorText = ResourceLanguageManager.DuLieuBatBuoc;
                validateFrom.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtFrom, validateFrom);

                ControlEditValidationRule validateTo = new ControlEditValidationRule();
                validateTo.editor = dtTo;
                validateTo.ErrorText = ResourceLanguageManager.DuLieuBatBuoc;
                validateTo.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtTo, validateTo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UcTest.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFrom.Text = Inventec.Common.Resource.Get.Value("UcTest.lciFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTo.Text = Inventec.Common.Resource.Get.Value("UcTest.lciTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void GetValue(ref long timeFrom, ref long timeTo)
        {
            try
            {
                if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
                {
                    timeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(dtFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                }

                if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
                {
                    timeTo = Inventec.Common.TypeConvert.Parse.ToInt64(dtTo.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool ValidateUc()
        {
            positionHandle = -1;
            return dxValidationProvider1.Validate();
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
