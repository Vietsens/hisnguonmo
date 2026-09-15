using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.AssignBed.ADO;
using HIS.Desktop.Plugins.AssignBed.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignBed.AssignBed
{
    public partial class frmAssignBed : HIS.Desktop.Utility.FormBase
    {

        private async Task InitComboRepositoryPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(this.repositoryItemGridLookUpEditPatientType, (data != null ? data.OrderBy(o => o.PRIORITY).ToList() : null), controlEditorADO);
                }
                else
                {
                    ControlEditorLoader.Load(this.repositoryItemGridLookUpEditPatientType, this.currentPatientTypeWithPatientTypeAlter, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region combo Giường
        // Hàm load dữ liệu giường theo SERVICE_ID từ Dictionary (rất nhanh)
        private void LoadAllBedData()
        {
            try
            {
                allHisBedBstys = BackendDataWorker.Get<HIS_BED_BSTY>().Where(o => o.IS_ACTIVE == 1).ToList();

                List<V_HIS_BED_ROOM> bedRooms = null;

                // Giu nguyen cach lay giuong theo khoa lam viec: dicBedByServiceId con duoc BuildTree dung
                // de loc DANH SACH DICH VU giuong tren cay, loc hep o day se lam bien mat ca dich vu.
                // Viec gioi han theo buong benh nhan dang nam duoc lam rieng o buoc nap combo chon giuong.
                bedRooms = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.DEPARTMENT_ID == this.currentDepartment.ID).ToList();

                var hisBedIds = allHisBedBstys.Select(o => o.BED_ID).ToList();

                allBeds = BackendDataWorker.Get<V_HIS_BED>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && bedRooms.Exists(s => s.ID == o.BED_ROOM_ID) && hisBedIds.Contains(o.ID)).ToList();

                this.dicBedByServiceId = allHisBedBstys.GroupBy(o => o.BED_SERVICE_TYPE_ID).ToDictionary(g => g.Key,g => allBeds.Where(t => g.Any(b => b.BED_ID == t.ID)).ToList());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gioi han danh sach giuong cho chon theo dung buong benh nhan DANG nam.
        /// Chi ap dung khi bat khoa cau hinh va lay duoc buong dang nam, con lai giu nguyen danh sach cu.
        /// </summary>
        private List<V_HIS_BED> FilterBedByCurrentBedRoom(List<V_HIS_BED> listBed)
        {
            try
            {
                if (!HisConfigCFG.DefaultBedByLastAssigned)
                    return listBed;

                if (listBed == null || listBed.Count <= 0)
                    return listBed;

                if (this.currentTreatmentBedRooms == null || this.currentTreatmentBedRooms.Count <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("FilterBedByCurrentBedRoom => khong lay duoc buong benh nhan dang nam, giu nguyen danh sach giuong theo khoa lam viec.");
                    return listBed;
                }

                List<long> currentBedRoomIds = this.currentTreatmentBedRooms.Select(o => o.BED_ROOM_ID).Distinct().ToList();
                return listBed.Where(o => currentBedRoomIds.Contains(o.BED_ROOM_ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return listBed;
            }
        }

        // Thêm các field cache vào class
        private long _lastLoadedBedServiceId = -1;
        private DateTime _lastLoadedTimeFrom = DateTime.MinValue;
        private DateTime _lastLoadedTimeTo = DateTime.MinValue;

        private void LoadBedDataByServiceId(long serviceId, DateTime timeFrom, DateTime timeTo)
        {
            try
            {
                // ✅ Chỉ load lại nếu tham số thay đổi
                if (serviceId == _lastLoadedBedServiceId
                    && timeFrom == _lastLoadedTimeFrom
                    && timeTo == _lastLoadedTimeTo)
                {
                    return; // Không load lại, tránh trigger repaint grid 
                }

                _lastLoadedBedServiceId = serviceId;
                _lastLoadedTimeFrom = timeFrom;
                _lastLoadedTimeTo = timeTo;

                List<V_HIS_BED> listBed = null;

                if (repositoryItemGridLookUpEditBed != null)
                {
                    repositoryItemGridLookUpEditBed.NullText = "";
                    repositoryItemGridLookUpEditBed.NullValuePrompt = "";
                    repositoryItemGridLookUpEditBed.NullValuePromptShowForEmptyValue = false;
                }

                if (dicBedByServiceId != null && dicBedByServiceId.ContainsKey(serviceId))
                {
                    listBed = dicBedByServiceId[serviceId];
                    listBed = this.FilterBedByCurrentBedRoom(listBed);
                    this.dataBedADOs = ProcessDataBedAdo(listBed, timeFrom, timeTo);
                }
                else
                {
                    listBed = new List<V_HIS_BED>();
                    this.dataBedADOs = null;
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>
                {
                    new ColumnInfo("BED_CODE", "", 50, 1),
                    new ColumnInfo("BED_NAME", "", 250, 2),
                    new ColumnInfo("AMOUNT_STR", "", 60, 3)
                };
                ControlEditorADO controlEditorADO = new ControlEditorADO("BED_NAME", "ID", columnInfos, false, 370);
                ControlEditorLoader.Load(repositoryItemGridLookUpEditBed, this.dataBedADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HisBedADO> ProcessDataBedAdo(List<V_HIS_BED> datas, DateTime timeFrom, DateTime timeTo)
        {
            List<HisBedADO> result = null;
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    result = new List<HisBedADO>();
                    result.AddRange((from r in datas select new HisBedADO(r)).ToList());

                    long? startTimeFilter = null;
                    long? finishTimeFilter = null;
                    if (timeFrom != null && timeFrom != DateTime.MinValue)
                    {
                        startTimeFilter = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(timeFrom) ?? 0;
                    }
                    if (timeTo != null && timeTo != DateTime.MinValue)
                    {
                        finishTimeFilter = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(timeTo) ?? 0;
                    }

                    List<long> bedIds = datas.Select(p => p.ID).Distinct().ToList();

                    //api moi

                    MOS.SDO.TakeBedsInUseSDO sdo = new TakeBedsInUseSDO();
                    sdo.BedIds = bedIds;
                    sdo.StartTime = startTimeFilter ?? 0;
                    sdo.FinishTime = finishTimeFilter;
                    CommonParam param = new CommonParam();
                    Inventec.Common.Logging.LogSystem.Debug("Du lieu goi den api: HisBedLog/TakeBedsInUse. TakeBedsInUseSDO: " + Inventec.Common.Logging.LogUtil.TraceData("TakeBedsInUseSDO", sdo));
                    List<HIS_BED_LOG> dataBedLogs = new BackendAdapter(param).Post<List<HIS_BED_LOG>>("/api/HisBedLog/TakeBedsInUse", ApiConsumers.MosConsumer, sdo, param);

                    // Giu lai de tinh giuong duoc chi dinh gan nhat cua ho so dang thao tac.
                    this.dataBedLogsInUse = dataBedLogs;

                    if (dataBedLogs != null && dataBedLogs.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Du lieu goi den api tra ve: HisBedLog/TakeBedsInUse. dataBedLogs: " + Inventec.Common.Logging.LogUtil.TraceData("dataBedLogs", dataBedLogs.Select(s => s.BED_ID).ToList()));

                        foreach (var itemADO in result)
                        {
                            var dataByBedLogs_onStartTime = dataBedLogs
                                .Where(p => p.BED_ID == itemADO.ID
                                        && (p.START_TIME <= startTimeFilter
                                            && (!p.FINISH_TIME.HasValue || p.FINISH_TIME.Value >= startTimeFilter)
                                            || (p.START_TIME >= startTimeFilter && (!p.FINISH_TIME.HasValue || p.START_TIME <= finishTimeFilter))))
                                .ToList() ?? new List<HIS_BED_LOG>();

                            List<HIS_BED_LOG> dataByBedLogs_onFinishTime = new List<HIS_BED_LOG>();

                            if (finishTimeFilter != null)
                            {
                                // Nếu finishTimeFilter có giá trị, tìm các log có thời gian phù hợp trong khoảng từ startTime đến finishTime
                                dataByBedLogs_onFinishTime = dataBedLogs
                                    .Where(p => p.BED_ID == itemADO.ID
                                            && (p.START_TIME <= finishTimeFilter
                                                && (!p.FINISH_TIME.HasValue || p.FINISH_TIME.Value >= finishTimeFilter)
                                                || (p.START_TIME >= startTimeFilter && (!p.FINISH_TIME.HasValue || p.START_TIME <= finishTimeFilter))))
                                    .ToList() ?? new List<HIS_BED_LOG>();
                            }
                            else
                            {
                                // Nếu finishTimeFilter không có giá trị, chỉ xét các log có finishTime >= startTimeFilter
                                dataByBedLogs_onFinishTime = dataBedLogs
                                    .Where(p => p.BED_ID == itemADO.ID
                                            && (!p.FINISH_TIME.HasValue || p.FINISH_TIME.Value >= startTimeFilter))
                                    .ToList() ?? new List<HIS_BED_LOG>();
                            }

                            List<HIS_BED_LOG> dataByBedLogs = new List<HIS_BED_LOG>();
                            dataByBedLogs.AddRange(dataByBedLogs_onStartTime);
                            dataByBedLogs.AddRange(dataByBedLogs_onFinishTime);
                            dataByBedLogs = dataByBedLogs.Distinct().ToList();
                            if (dataByBedLogs_onStartTime != null && dataByBedLogs_onStartTime.Count > 0)
                            {
                                itemADO.BedLogStartIds = dataByBedLogs_onStartTime.Select(o => o.ID).ToList();
                            }
                            if (dataByBedLogs_onFinishTime != null && dataByBedLogs_onFinishTime.Count > 0)
                            {
                                itemADO.BedLogFinishIds = dataByBedLogs_onFinishTime.Select(o => o.ID).ToList();
                            }

                            if (dataByBedLogs != null && dataByBedLogs.Count > 0)
                            {
                                if (itemADO.MAX_CAPACITY.HasValue)
                                {
                                    if (dataByBedLogs.Count >= itemADO.MAX_CAPACITY)
                                    {
                                        itemADO.IsKey = 2;
                                    }
                                    else if (dataByBedLogs.Count == 0)
                                    {
                                        itemADO.IsKey = 0;
                                    }
                                    else
                                    {
                                        itemADO.IsKey = 1;
                                    }

                                }
                                else
                                {
                                    itemADO.IsKey = 0;
                                }    
                                itemADO.BedLogAllIds = dataByBedLogs.Select(o => o.ID).ToList();
                                itemADO.AMOUNT = dataByBedLogs.Count;

                                itemADO.AMOUNT_STR = dataByBedLogs.Count + "/" + itemADO.MAX_CAPACITY;
                                itemADO.TREATMENT_BED_ROOM_IDs = dataByBedLogs.Select(o => o.TREATMENT_BED_ROOM_ID).ToList();
                                //dicTreatmentBedRoom[itemADO.ID] = itemADO.TREATMENT_BED_ROOM_IDs;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        #endregion
    }
}
