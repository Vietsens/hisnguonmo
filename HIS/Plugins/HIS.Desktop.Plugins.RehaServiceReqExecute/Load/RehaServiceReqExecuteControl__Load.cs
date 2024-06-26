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
using AutoMapper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.RehaServiceReqExecute.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RehaServiceReqExecute
{
    public partial class RehaServiceReqExecuteControl : HIS.Desktop.Utility.UserControlBase
    {
        internal void FillDataToGridSereServSurgServiceReq(V_HIS_SERVICE_REQ HisServiceReqWithOrderSDO)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServView12Filter hisSereServViewFilter = new MOS.Filter.HisSereServView12Filter();
                hisSereServViewFilter.SERVICE_REQ_ID = HisServiceReqWithOrderSDO.ID;

                SereServs = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12>>("api/HisSereServ/GetView12", ApiConsumers.MosConsumer, hisSereServViewFilter, param);

                if (SereServs == null)
                    throw new Exception("Không tìm thấy dịch vụ nào");

                List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>(); ;
                foreach (var item in SereServs)
                {
                    V_HIS_SERVICE service = services.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                    if (service != null)
                    {
                        item.TDL_SERVICE_NAME = service.SERVICE_NAME;
                        item.TDL_SERVICE_CODE = service.SERVICE_CODE;
                    }
                }

                gridControlSereServRehaReq.DataSource = SereServs;
                //Tu dong chon mot sereserv dau tien de hien thi danh sach ky thuat tap
                if (SereServs != null && SereServs.Count > 0)
                {
                    gridViewSereServRehaReq.FocusedRowHandle = 0;
                    SereServ = SereServs[0];
                }

                //Load du lieu ky thuat tap cua sereserv dang chon
                FillDataToSereServReha(SereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToSereServReha(V_HIS_SERE_SERV_12 sereServ)
        {
            try
            {
                CommonParam param = new CommonParam();
                gridControlRehaTrain.DataSource = null;
                gridControlSereServReha.DataSource = null;
                sereServRehas = new List<SereServRehaADO>();
                if (sereServ != null)
                {
                    MOS.Filter.HisSereServRehaViewFilter hisSereServRehaFilter = new MOS.Filter.HisSereServRehaViewFilter();
                    hisSereServRehaFilter.SERVICE_REQ_ID = HisServiceReqWithOrderSDO.ID;
                    hisSereServRehaFilter.SERE_SERV_IDs = new List<long>();
                    hisSereServRehaFilter.SERE_SERV_IDs.Add(sereServ.ID);
                    hisSereServRehaFilter.ORDER_FIELD = "NUM_ORDER";
                    hisSereServRehaFilter.ORDER_DIRECTION = "DESC";

                    var currentHisSereServRehas = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA>>(HisRequestUriStore.HIS_SERE_SERV_REHA_GETVIEW, ApiConsumers.MosConsumer, hisSereServRehaFilter, param);

                    HisRestRetrTypeViewFilter restRetrTypeViewFilter = new HisRestRetrTypeViewFilter();
                    restRetrTypeViewFilter.SERVICE_ID = sereServ.SERVICE_ID;

                    var restRetrTypes = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_REST_RETR_TYPE>>(HisRequestUriStore.HIS_REST_RETR_TYPE_GETVIEW, ApiConsumers.MosConsumer, restRetrTypeViewFilter, param);

                    if (restRetrTypes != null)
                    {
                        int dem = 0;
                        foreach (var rety in restRetrTypes)
                        {
                            MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA ssReha = new MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA();
                            Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_REST_RETR_TYPE, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA>();
                            ssReha = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_REST_RETR_TYPE, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA>(rety);

                            SereServRehaADO ssRehaADO = new SereServRehaADO();
                            var checkExists = currentHisSereServRehas.FirstOrDefault(o => o.REHA_TRAIN_TYPE_ID == rety.REHA_TRAIN_TYPE_ID);
                            if (checkExists != null)
                            {
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA, SereServRehaADO>();
                                ssRehaADO = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_REHA, SereServRehaADO>(checkExists);
                                ssRehaADO.choose = true;
                            }
                            else
                            {
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA, SereServRehaADO>();
                                ssRehaADO = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_REHA, SereServRehaADO>(ssReha);
                                ssRehaADO.SERE_SERV_ID = sereServ.ID;
                            }

                            sereServRehas.Add(ssRehaADO);
                        }
                        gridControlSereServReha.DataSource = sereServRehas;

                        EnableThemKyThuatTap();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableThemKyThuatTap()
        {
            try
            {
                List<SereServRehaADO> sereServRehas = gridControlSereServReha.DataSource as List<SereServRehaADO>;
                SereServRehaADO sereServRehaAdoChoose = sereServRehas != null ? sereServRehas.FirstOrDefault(o => o.choose == true) : null;
                if (sereServRehaAdoChoose != null)
                {
                    btnThemTTTap.Enabled = true;
                }
                else
                {
                    btnThemTTTap.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataDetailRehaServiceReq(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ data)
        {
            try
            {
                if (data == null) throw new ArgumentNullException("data is null");

                txtSymptom_Before.Text = data.SYMPTOM_BEFORE;
                txtSymptom_After.Text = data.SYMPTOM_AFTER;
                txtRespiratory_Before.Text = data.RESPIRATORY_BEFORE;
                txtRespiratory_After.Text = data.RESPIRATORY_AFTER;
                txtECG_Before.Text = data.ECG_BEFORE;
                txtECG_After.Text = data.ECG_AFTER;
                txtAdvice.Text = data.ADVISE;
                txtIcdExtraName.Text = data.ICD_TEXT;
                txtIcdExtraCode.Text = data.ICD_SUB_CODE;
                txtIcdMainCode.Text = data.ICD_CODE;
                txtIcdMainText.Text = data.ICD_NAME;
                if (!String.IsNullOrEmpty(data.ICD_CODE))
                {
                    HIS_ICD icd = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>()
                        .FirstOrDefault(o => o.ICD_CODE == data.ICD_CODE);

                    cboIcds.EditValue = icd.ID;
                    if (!String.IsNullOrEmpty(data.ICD_NAME) 
                        && icd != null ? icd.ICD_NAME != data.ICD_NAME : true)
                    {
                        chkIcds.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableButtonByServiceReqStt(long serviceReqSttId)
        {
            try
            {
                if (serviceReqSttId != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    gridControlSereServRehaReq.Enabled = true;
                    gridControlSereServReha.Enabled = true;
                    lciTrieuChungTruoc.Enabled = true;
                    lciTrieuChungSau.Enabled = true;
                    lciHoHapTruoc.Enabled = true;
                    lciHoHapSau.Enabled = true;
                    lciDienTimTruoc.Enabled = true;
                    lciDienTimSau.Enabled = true;
                    lciLoiKhuyen.Enabled = true;
                    gridControlRehaTrain.Enabled = true;
                    lciBenhChinh.Enabled = true;
                    cboIcds.Enabled = true;
                    txtIcdMainText.Enabled = true;
                    lciEditIcd.Enabled = true;
                    lciBenhPhu.Enabled = true;
                    txtIcdExtraName.Enabled = true;
                    btnThemTTTap.Enabled = true;
                    btnAssignService.Enabled = true;
                    btnAssignPre.Enabled = true;
                    btnSave.Enabled = true;
                    btnFinish.Enabled = true;

                }
                else
                {
                    gridControlSereServRehaReq.Enabled = false;
                    gridControlSereServReha.Enabled = false;
                    lciTrieuChungTruoc.Enabled = false;
                    lciTrieuChungSau.Enabled = false;
                    lciHoHapTruoc.Enabled = false;
                    lciHoHapSau.Enabled = false;
                    lciDienTimTruoc.Enabled = false;
                    lciDienTimSau.Enabled = false;
                    lciLoiKhuyen.Enabled = false;
                    gridControlRehaTrain.Enabled = false;
                    lciBenhChinh.Enabled = false;
                    cboIcds.Enabled = false;
                    txtIcdMainText.Enabled = false;
                    lciEditIcd.Enabled = false;
                    lciBenhPhu.Enabled = false;
                    txtIcdExtraName.Enabled = false;
                    btnThemTTTap.Enabled = false;
                    btnAssignService.Enabled = false;
                    btnAssignPre.Enabled = false;
                    btnSave.Enabled = false;
                    btnFinish.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientTypeAlter()
        {
            try
            {
                if (this.PatientTypeAlter == null)
                {
                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = treatmentId;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    this.PatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
