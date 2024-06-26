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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.SwapService
{
    public partial class frmSwapService : HIS.Desktop.Utility.FormBase
    {
        internal V_HIS_SERE_SERV currentSereServ { get; set; }
        internal List<HisSereServADO> sereServADOs { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter { get; set; }
        internal List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        internal DelegateSelectData _delegateSwapService;

        internal V_HIS_SERVICE_REQ serviceReq { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        string checkage = "";
        string chekGender = "";
        public frmSwapService()
        {
            InitializeComponent();
        }

        public frmSwapService(Inventec.Desktop.Common.Modules.Module _module, V_HIS_SERVICE_REQ serviceReq, V_HIS_SERE_SERV currentSereServ, DelegateSelectData _delegateSwapService)
            : base(_module)
        {
            InitializeComponent();
            this.currentSereServ = currentSereServ;
            this.serviceReq = serviceReq;
            this._delegateSwapService = _delegateSwapService;
            this.currentModule = _module;
        }

        public frmSwapService(V_HIS_SERVICE_REQ serviceReq, V_HIS_SERE_SERV currentSereServ, DelegateSelectData _delegateSwapService)
        {
            InitializeComponent();
            this.currentSereServ = currentSereServ;
            this.serviceReq = serviceReq;
            this._delegateSwapService = _delegateSwapService;
        }

        private void frmSwapService_Load(object sender, EventArgs e)
        {
            WaitingManager.Show();
            InitLanguage();
            LoadGridSereServ();
            LoadCurrentPatientTypeAlter(serviceReq.TREATMENT_ID, serviceReq.INTRUCTION_TIME);
            PatientTypeWithPatientTypeAlter();
            LoadDataToPatientType(repositoryItemcboPatientType, currentPatientTypeWithPatientTypeAlter);
            WaitingManager.Hide();
        }

        private void gridViewSwapService_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "IsExpend")
                    {
                        e.RepositoryItem = repositoryItemChkIsExpend;
                    }

                    if (e.Column.FieldName == "AMOUNT")
                    {
                        e.RepositoryItem = repositoryItemSpinEditAmount;
                    }

                    if (e.Column.FieldName == "IsOutKtcFee")
                    {
                        e.RepositoryItem = repositoryItemChkOutKtcFee;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSwapService_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisSereServADO data_ServiceSDO = (HisSereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data_ServiceSDO != null)
                    {
                        if (e.Column.FieldName == "PRICE_DISPLAY")
                        {
                            if (data_ServiceSDO.PATIENT_TYPE_ID != 0)
                            {
                                var data_ServicePrice = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(o => o.SERVICE_ID == data_ServiceSDO.SERVICE_ID && o.PATIENT_TYPE_ID == data_ServiceSDO.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                                if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString((data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO)), ConfigApplications.NumberSeperator);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSwapService_Click(object sender, EventArgs e)
        {
            try
            {
                //Check thoi gian chi dinh cau hinh


                HisSereServADO sereServSwap = null;
                CommonParam param = new CommonParam();
                bool success = false;

                //List<HisSereServADO> sereServADOs = gridViewSwapService.DataSource as List<HisSereServADO>;
                List<HisSereServADO> sereServADOs = gridControlSwapService.DataSource as List<HisSereServADO>;
                if (sereServADOs != null && sereServADOs.Count() > 0)
                    foreach (var item in sereServADOs)
                    {
                        if (item.checkService)
                        {
                            sereServSwap = item;
                            break;
                        }
                    }

                //for (int i = 0; i < gridViewSwapService.SelectedRowsCount; i++)
                //{
                //    if (gridViewSwapService.GetSelectedRows()[i] >= 0)
                //    {
                //        var sereServSwapCheck = (HisSereServADO)gridViewSwapService.GetRow(gridViewSwapService.GetSelectedRows()[i]);
                //        if (sereServSwapCheck.checkService)
                //        {
                //            sereServSwap = sereServSwapCheck;
                //            break;
                //        }

                //    }
                //}


                if (sereServSwap == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 dịch vụ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {


                    if (currentSereServ.IS_NO_EXECUTE != 1)
                    {
                        List<long> serviceIds = new List<long> { sereServSwap.SERVICE_ID };
                        List<HIS_SERE_SERV> sereServWithMinDurations = this.GetSereServWithMinDuration(currentSereServ.TDL_PATIENT_ID.Value, serviceIds);
                        if (sereServWithMinDurations != null && sereServWithMinDurations.Count > 0)
                        {
                            string sereServMinDurationStr = "";
                            foreach (var item in sereServWithMinDurations)
                            {
                                sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                            }

                            if (MessageBox.Show(string.Format("Các dịch vụ sau có thời gian chỉ định nằm trong khoảng thời gian không cho phép: {0} .Bạn có muốn tiếp tục?", sereServMinDurationStr), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                                return;
                        }


                        List<V_HIS_SERVICE> services_ = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>()
                    .Where(o => serviceIds.Contains(o.ID)).ToList();
                        if (services_ != null || services_.Count > 0)
                        {
                            List<HIS_PATIENT> hispatient = new List<HIS_PATIENT>();
                            CommonParam param_ = new CommonParam();
                            HisPatientFilter Filter = new HisPatientFilter();
                            Filter.ID = currentSereServ.TDL_PATIENT_ID.Value;
                            hispatient = new BackendAdapter(param_).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, Filter, param_);
                            if (hispatient != null && hispatient.Count > 0)
                            {

                                foreach (var item in services_)
                                {
                                    if (item.AGE_FROM != null && item.AGE_TO != null && hispatient.FirstOrDefault().DOB != null)
                                    {
                                        long DatetimeNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                                        long age = Inventec.Common.DateTime.Calculation.DifferenceMonth(hispatient.FirstOrDefault().DOB, DatetimeNow);

                                        if (age > 0)
                                        {
                                            if (age > item.AGE_TO || age < item.AGE_FROM)
                                            {
                                                if (services_.Count == 1)
                                                {
                                                    checkage += item.SERVICE_CODE + " - " + item.SERVICE_NAME;
                                                }
                                                else
                                                {
                                                    checkage += item.SERVICE_CODE + " - " + item.SERVICE_NAME + "; ";
                                                }
                                            }
                                        }
                                    }

                                    if (hispatient.FirstOrDefault().GENDER_ID != item.GENDER_ID && hispatient.FirstOrDefault().GENDER_ID != null && item.GENDER_ID != null)
                                    {
                                        var Gender = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.IS_ACTIVE == 1 && o.ID == item.GENDER_ID).GENDER_NAME;
                                        chekGender = "Dịch vụ chỉ sử dụng cho giới tính: " + Gender + " .Vui lòng chọn dịch vụ khác!";
                                    }
                                }

                            }
                        }

                        if (!string.IsNullOrEmpty(checkage))
                        {
                            MessageBox.Show(string.Format("Độ tuổi của bệnh nhân không phù hợp với điều kiện của dịch vụ: {0}  . Vui lòng chọn dịch vụ khác!", checkage), "Thông báo", MessageBoxButtons.OK);
                            checkage = "";
                            return;
                        }
                        if (!string.IsNullOrEmpty(chekGender))
                        {
                            MessageBox.Show(chekGender, "Thông báo", MessageBoxButtons.OK);
                            chekGender = "";
                            return;
                        }

                    }

                    DialogResult myResult;
                    myResult = MessageBox.Show("Bạn có muốn đổi sang dịch vụ này không?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (myResult == DialogResult.OK)
                    {

                        WaitingManager.Show();
                        SwapServiceSDO swapServiceSDO = new SwapServiceSDO();
                        swapServiceSDO.SereServId = currentSereServ.ID;

                        swapServiceSDO.NewService = new ServiceReqDetailSDO();
                        swapServiceSDO.NewService.ServiceId = sereServSwap.SERVICE_ID;
                        swapServiceSDO.NewService.PatientTypeId = sereServSwap.PATIENT_TYPE_ID;
                        swapServiceSDO.NewService.ParentId = currentSereServ.PARENT_ID;
                        if (sereServSwap.IsOutKtcFee == true)
                            swapServiceSDO.NewService.IsOutParentFee = 1;
                        if (sereServSwap.IsExpend == true)
                            swapServiceSDO.NewService.IsExpend = 1;
                        swapServiceSDO.NewService.EkipId = currentSereServ.EKIP_ID;
                        swapServiceSDO.NewService.Amount = sereServSwap.AMOUNT;
                        swapServiceSDO.ExecuteRoomId = this.currentModule.RoomId;

                        HIS_SERE_SERV data = new BackendAdapter(param)
                    .Post<HIS_SERE_SERV>("api/HisSereServ/SwapService", ApiConsumers.MosConsumer, swapServiceSDO, param);
                        if (data != null)
                        {
                            if (_delegateSwapService != null) _delegateSwapService(data);
                        }

                        WaitingManager.Hide();
                        if (data != null)
                        {
                            success = true;
                            this.Close();
                        }
                        #region Show message
                        MessageManager.Show(this, param, success);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SERE_SERV> GetSereServWithMinDuration(long patientId, List<long> serviceIds)
        {
            List<HIS_SERE_SERV> results = new List<HIS_SERE_SERV>();
            try
            {
                if (serviceIds == null || serviceIds.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong truyen danh sach serviceids");
                    return null;
                }

                List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>()
                    .Where(o => serviceIds.Contains(o.ID) && o.MIN_DURATION.HasValue).ToList();
                if (services == null || services.Count == 0)
                {
                    return null;
                }

                //List<HIS_PATIENT> hispatient = new List<HIS_PATIENT>();
                //CommonParam param_ = new CommonParam();
                //HisPatientFilter Filter = new HisPatientFilter();
                //Filter.ID = patientId;
                //hispatient = new BackendAdapter(param_).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, Filter, param_);
                //if (hispatient != null && hispatient.Count > 0)
                //{

                //    foreach (var item in services)
                //    {
                //        if (item.AGE_FROM != null && item.AGE_TO != null && hispatient.FirstOrDefault().DOB != null)
                //        {
                //           long DatetimeNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                //           long age = Inventec.Common.DateTime.Calculation.DifferenceMonth(hispatient.FirstOrDefault().DOB, DatetimeNow);

                //            if (age > 0)
                //            {
                //                if (age > item.AGE_TO || age < item.AGE_FROM)
                //                {
                //                    if (services.Count == 1)
                //                    {
                //                        checkage += item.SERVICE_CODE + " - " + item.SERVICE_NAME;
                //                    }
                //                    else
                //                    {
                //                        checkage += item.SERVICE_CODE + " - " + item.SERVICE_NAME + "; ";
                //                    }
                //                }
                //            }
                //        }

                //        if (hispatient.FirstOrDefault().GENDER_ID != item.GENDER_ID && hispatient.FirstOrDefault().GENDER_ID != null && item.GENDER_ID != null)
                //        {
                //            var Gender = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.IS_ACTIVE == 1 && o.ID == item.GENDER_ID).GENDER_NAME;
                //                chekGender = "Dịch vụ chỉ sử dụng cho giới tính: " + Gender + " .Vui lòng chọn dịch vụ khác!";
                //        }
                //    }

                //}

                List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                foreach (var item in services)
                {
                    ServiceDuration sd = new ServiceDuration();
                    sd.MinDuration = item.MIN_DURATION.Value;
                    sd.ServiceId = item.ID;
                    serviceDurations.Add(sd);
                }


                CommonParam param = new CommonParam();
                HisSereServMinDurationFilter hisSereServMinDurationFilter = new HisSereServMinDurationFilter();
                hisSereServMinDurationFilter.ServiceDurations = serviceDurations;
                hisSereServMinDurationFilter.PatientId = patientId;
                hisSereServMinDurationFilter.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                results = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, hisSereServMinDurationFilter, param);

                if (results != null && results.Count > 0)
                {
                    var listSereServResultTemp = from SereServResult in results
                                                 group SereServResult by SereServResult.SERVICE_ID into g
                                                 orderby g.Key
                                                 select g.FirstOrDefault();
                    results = listSereServResultTemp.ToList();
                }
            }
            catch (Exception ex)
            {
                results = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return results;
        }

        private void gridViewSwapService_Click(object sender, EventArgs e)
        {
            try
            {
                var sereServAdo = (HisSereServADO)gridViewSwapService.GetFocusedRow();
                foreach (var item in sereServADOs)
                {
                    if (sereServAdo.SERVICE_ID == item.SERVICE_ID)
                    {
                        item.checkService = true;
                    }
                    else
                    {
                        item.checkService = false;
                    }
                }
                gridControlSwapService.RefreshDataSource();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEditService_Click(object sender, EventArgs e)
        {
            try
            {
                var sereServAdo = (HisSereServADO)gridViewSwapService.GetFocusedRow();

                foreach (var item in sereServADOs)
                {
                    if (sereServAdo.SERVICE_ID == item.SERVICE_ID)
                    {

                        item.checkService = true;
                    }
                    else
                    {
                        item.checkService = false;
                    }
                }

                gridControlSwapService.RefreshDataSource();
                gridViewSwapService.LayoutChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                List<HisSereServADO> sereServADOTemps = sereServADOs.Where(o =>
                    o.IS_ACTIVE == 1 &&
                    (o.TDL_SERVICE_NAME.ToUpper().Contains(txtKeyword.Text.Trim().ToUpper())
                    || o.TDL_SERVICE_CODE.ToUpper().Contains(txtKeyword.Text.Trim().ToUpper()))).ToList();
                gridControlSwapService.DataSource = sereServADOTemps;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
