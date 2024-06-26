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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;

namespace HIS.Desktop.Plugins.HisSurgServiceType.HisSurgServiceType
{
    public partial class frmHisSurgServiceType : Form
    {
        private HIS_SERVICE SetDataService()
        {
            HIS_SERVICE service = new HIS_SERVICE();
            try
            {

                if (lkServiceId.EditValue != null) service.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkServiceId.EditValue ?? "0").ToString());
                if (cboLoaiBH.EditValue != null) service.HEIN_SERVICE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiBH.EditValue ?? "0").ToString());
                if (cboPatientType.EditValue != null) service.BILL_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "0").ToString());
                if (cboLoaiDV.EditValue != null && !String.IsNullOrEmpty(cboLoaiDV.Text)) service.HEIN_SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiDV.EditValue ?? null).ToString());
                service.SERVICE_TYPE_ID = HisServiceTypeCFG.SERVICE_TYPE_ID__SURG;
                if (cboHoaDon.EditValue != null)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64((cboHoaDon.EditValue ?? 0).ToString()) == 1)
                    {
                        service.BILL_OPTION = null;
                    }
                    else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboHoaDon.EditValue ?? 0).ToString()) == 2)
                    {
                        service.BILL_OPTION = 1;
                    }
                    else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboHoaDon.EditValue ?? 0).ToString()) == 3)
                    {
                        service.BILL_OPTION = 2;
                    }
                }
                if (spGia.EditValue != null)
                {
                    service.COGS = (long)spGia.Value;
                }
                if (spTime.EditValue != null)
                {
                    service.ESTIMATE_DURATION = (long)spTime.Value;
                }
                if (spDinhMuc.EditValue != null)
                {
                    service.MAX_EXPEND = (long)spDinhMuc.Value;
                }
                service.IS_MULTI_REQUEST = (short)(chkIsLeaf.Checked ? 1 : 0);
                service.IS_OUT_PARENT_FEE = (short)(chkOutPack.Checked ? 1 : 0);
            }
            catch (Exception ex)
            {
                service = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return service;
        }
        private HIS_SURG_SERVICE_TYPE SetDataSurgService()
        {
            HIS_SURG_SERVICE_TYPE surgService = new HIS_SURG_SERVICE_TYPE();
            try
            {
                if (!String.IsNullOrEmpty(txtSurgServiceTypeCode.Text))
                    surgService.SURG_SERVICE_TYPE_CODE = txtSurgServiceTypeCode.Text;
                if (!String.IsNullOrEmpty(txtSurgServiceTypeName.Text))
                    surgService.SURG_SERVICE_TYPE_NAME = txtSurgServiceTypeName.Text;
                if (cboCha.EditValue != null) surgService.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboCha.EditValue ?? "0").ToString());
                if (cboMethod.EditValue != null) surgService.PTTT_METHOD_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMethod.EditValue ?? "0").ToString());
                if (lkPtttGroupId.EditValue != null) surgService.PTTT_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkPtttGroupId.EditValue ?? "0").ToString());
                if (cboICD.EditValue != null) surgService.ICD_CM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboICD.EditValue ?? "0").ToString());
                if (spNumOrder.EditValue != null)
                {
                    surgService.NUM_ORDER = (long)spNumOrder.Value;
                }
                
            }
            catch (Exception ex)
            {
                surgService = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return surgService;
        }
    }
}
