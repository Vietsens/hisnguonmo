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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using HIS.Desktop.Plugins.ServiceExecute.EkipTemp;
using HIS.Desktop.Plugins.ServiceExecute.ValidationRule;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmClsInfo : Form
    {
        public void ValidateControl()
        {
            try
            {
                ValidationPTTTGroup();
                ValiVuotQuaKyTu();
                if (HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.IsRequiredPtttPriority)
                    ValidationHinhThucPTTT();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationPTTTGroup()
        {
            try
            {
                LookupEditWithTextEditValidationRule icdMainRule = new LookupEditWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtPtttGroupCode;
                icdMainRule.cbo = cbbPtttGroup;
                icdMainRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtPtttGroupCode, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationHinhThucPTTT()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule icdMainRule = new GridLookupEditWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtLoaiPT;
                icdMainRule.cbo = cboLoaiPT;
                icdMainRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtLoaiPT, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void ValidationStartTime()
        //{
        //    try
        //    {
        //        StartTimeValidationRule mainRule = new StartTimeValidationRule();
        //        mainRule.startTime = dtStart;
        //        mainRule.finishTime = dtFinish;
        //        mainRule.instructionTime = serviceReq.INTRUCTION_TIME;
        //        mainRule.treatmentOutTime = this.vhisTreatment.OUT_TIME ?? 0;
        //        mainRule.keyCheck = this.isAllowEditInfo;
        //        mainRule.keyCheckStatsTime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.PLUGINS.SURGSERVICEREQEXECUTE.BIG_START_TIME_CURRENT_TIME") == "1"; ;
        //        mainRule.ErrorType = ErrorType.Warning;
        //        this.dxValidationProvider1.SetValidationRule(dtStart, mainRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void ValidationFinishTime()
        //{
        //    try
        //    {
        //        FinishTimeValidationRule mainRule = new FinishTimeValidationRule();
        //        mainRule.startTime = dtStart;
        //        mainRule.finishTime = dtFinish;
        //        mainRule.instructionTime = serviceReq.INTRUCTION_TIME;
        //        mainRule.treatmentOutTime = this.vhisTreatment.OUT_TIME ?? 0;
        //        mainRule.keyCheck = this.isAllowEditInfo;
        //        mainRule.keyCheckStatsTime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.PLUGINS.SURGSERVICEREQEXECUTE.BIG_START_TIME_CURRENT_TIME") == "1"; ;
        //        mainRule.ErrorType = ErrorType.Warning;
        //        this.dxValidationProvider1.SetValidationRule(dtFinish, mainRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void ValiVuotQuaKyTu()
        {
            try
            {
                ValidationControlMaxLength(txtMANNER, 500);
                //ValidationControlMaxLength(txtConclude, 1000);
                //ValidationControlMaxLength(txtResultNote, 500);
                //ValidationControlMaxLength(txtDescription, 4000);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.ErrorText = ResourceMessage.TruongDuLieuVuotQuaKyTu;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(control, validate);
        }
    }
}
