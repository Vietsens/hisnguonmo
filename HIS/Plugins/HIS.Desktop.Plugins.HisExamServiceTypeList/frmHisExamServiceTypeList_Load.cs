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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisExamServiceTypeList
{
    public partial class frmHisExamServiceTypeList : HIS.Desktop.Utility.FormBase
    {
        private HIS_SERVICE setDataService()
        {
            HIS_SERVICE service = new HIS_SERVICE();
            try
            {
                if (cboDVT.EditValue != null) service.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDVT.EditValue ?? null).ToString());
                service.SERVICE_TYPE_ID = HisServiceTypeCFG.SERVICE_TYPE_ID__EXAM;
                if (cboLDvBH.EditValue != null && !String.IsNullOrEmpty(cboLDvBH.Text)) service.HEIN_SERVICE_BHYT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboLDvBH.EditValue ?? 0).ToString());
                if (cboPatientType.EditValue != null) service.BILL_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString());
                if (cboLoaiBH.EditValue != null) service.HEIN_SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiBH.EditValue ?? 0).ToString());
                else
                    service.BILL_PATIENT_TYPE_ID = null;
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
                if (service.HEIN_SERVICE_BHYT_ID == 0)
                {
                    service.HEIN_SERVICE_BHYT_ID = null;
                }
                if (spGia.EditValue != null)
                {
                    service.COGS = (long)spGia.Value;
                }
                if (spTime.EditValue != null)
                {
                    service.ESTIMATE_DURATION = (long)spTime.Value;
                }
                service.IS_MULTI_REQUEST = (short)(chkCheck.Checked ? 1 : 0);
                service.IS_OUT_PARENT_FEE = (short)(chkOutPack.Checked ? 1 : 0);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return service;
        }
        private HIS_EXAM_SERVICE_TYPE setDataExamServiceType()
        {
            HIS_EXAM_SERVICE_TYPE examServiceType = new HIS_EXAM_SERVICE_TYPE();
            try
            {
                if (!String.IsNullOrEmpty(txtCode.Text))
                    examServiceType.EXAM_SERVICE_TYPE_CODE = txtCode.Text;
                if (!String.IsNullOrEmpty(txtName.Text))
                    examServiceType.EXAM_SERVICE_TYPE_NAME = txtName.Text;
                examServiceType.SPECIALITY_CODE = txtMaCK.Text;
                if (cboCha.EditValue != null) examServiceType.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboCha.EditValue ?? "0").ToString());
                if (examServiceType.PARENT_ID == 0)
                {
                    examServiceType.PARENT_ID = null;
                }
                if (spSTT.EditValue != null)
                {
                    examServiceType.NUM_ORDER = (long)spSTT.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return examServiceType;
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.SDO.HisExamServiceTypeSDO serviceSDO = new MOS.SDO.HisExamServiceTypeSDO();
                MOS.SDO.HisExamServiceTypeSDO examServiceTypeResultSDO = new MOS.SDO.HisExamServiceTypeSDO();

                serviceSDO.HisService = setDataService();
                serviceSDO.HisExamServiceType = setDataExamServiceType();


                if (ActionType == GlobalVariables.ActionAdd)
                {
                    examServiceTypeResultSDO = new BackendAdapter(param)
                    .Post<MOS.SDO.HisExamServiceTypeSDO>("api/HisExamServiceType/Create", ApiConsumers.MosConsumer, serviceSDO, param);

                }
                else
                {
                    if (serviceId > 0 && serviceTypeId > 0)
                    {
                        serviceSDO.HisService.ID = serviceId;
                        serviceSDO.HisExamServiceType.ID = serviceTypeId;
                        examServiceTypeResultSDO = new BackendAdapter(param)
                        .Post<HisExamServiceTypeSDO>("api/HisExamServiceType/Update", ApiConsumers.MosConsumer, serviceSDO, param);
                    }
                }
                if (examServiceTypeResultSDO != null)
                {
                    success = true;
                    LoaddataToTreeList(this);
                    ResetFormData();
                    InitComboCha();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
