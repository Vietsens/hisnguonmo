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

namespace HIS.Desktop.Plugins.HisDiimServiceType.HisDiimServiceType
{
    public partial class frmHisDiimServiceType : HIS.Desktop.Utility.FormBase
    {
        private void InitComboServiceId()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(lkServiceId, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboLoaiBHYT()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_BHYT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBHYT, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboLoaiBH()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboLoaiBH, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboCha()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDiimServiceReqViewFilter filter = new HisDiimServiceReqViewFilter();
                filter.IS_ACTIVE = 1;
                var serviceTypeParents = new BackendAdapter(param)
                    .Get<List<V_HIS_DIIM_SERVICE_TYPE>>("api/HisDiimServiceType/GetView", ApiConsumers.MosConsumer, filter, param);
                //var serviceTypeParents = BackendDataWorker.Get<V_HIS_DIIM_SERVICE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DIIM_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DIIM_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DIIM_SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCha, serviceTypeParents, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboPatientType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void initComboBill()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Hóa đơn thường"));
                status.Add(new Status(2, "Tách tiền chênh lệch vào hóa đơn dịch vụ"));
                status.Add(new Status(3, "Hóa đơn dịch vụ"));

                List<Inventec.Common.Controls.EditorLoader.ColumnInfo> columnInfos = new List<Inventec.Common.Controls.EditorLoader.ColumnInfo>();
                columnInfos.Add(new Inventec.Common.Controls.EditorLoader.ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBill, status, controlEditorADO);
                cboBill.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private HIS_SERVICE SetDataService()
        {
            HIS_SERVICE service = new HIS_SERVICE();
            try
            {

                if (lkServiceId.EditValue != null) service.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkServiceId.EditValue ?? "0").ToString());
                service.SERVICE_TYPE_ID = HisServiceTypeCFG.SERVICE_TYPE_ID__DIIM;
                if (cboBHYT.EditValue != null && !String.IsNullOrEmpty(cboBHYT.Text)) service.HEIN_SERVICE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboBHYT.EditValue ?? null).ToString());
                if (cboPatientType.EditValue != null) service.BILL_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "0").ToString());
                if (cboLoaiBH.EditValue != null) service.HEIN_SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiBH.EditValue ?? "0").ToString());
                service.IS_MULTI_REQUEST = (short)(chkIsLeaf.Checked ? 1 : 0);
                service.IS_OUT_PARENT_FEE = (short)(chkOutPack.Checked ? 1 : 0);
                if (cboBill.EditValue != null)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBill.EditValue ?? 0).ToString()) == 1)
                    {
                        service.BILL_OPTION = null;
                    }
                    else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBill.EditValue ?? 0).ToString()) == 2)
                    {
                        service.BILL_OPTION = 1;
                    }
                    else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBill.EditValue ?? 0).ToString()) == 3)
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
            }
            catch (Exception ex)
            {
                service = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return service;
        }
        private HIS_DIIM_SERVICE_TYPE SetDataDiimService()
        {
            HIS_DIIM_SERVICE_TYPE diimService = new HIS_DIIM_SERVICE_TYPE();
            try
            {
                diimService.IS_ACTIVE = 1;
                if (!String.IsNullOrEmpty(txtDiimServiceTypeCode.Text))
                    diimService.DIIM_SERVICE_TYPE_CODE = txtDiimServiceTypeCode.Text;
                if (!String.IsNullOrEmpty(txtDiimServiceTypeName.Text))
                    diimService.DIIM_SERVICE_TYPE_NAME = txtDiimServiceTypeName.Text;
                if (cboCha.EditValue != null) diimService.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboCha.EditValue ?? "0").ToString());
                if (diimService.PARENT_ID == 0)
                {
                    diimService.PARENT_ID = null;
                }
                if (spNumOrder.EditValue != null)
                {
                    diimService.NUM_ORDER = (long)spNumOrder.Value;
                }
            }
            catch (Exception ex)
            {
                diimService = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return diimService;
        }
    }
}
