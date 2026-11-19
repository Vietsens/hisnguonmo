using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.AssignBed.ADO;
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

                var bedRooms = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.DEPARTMENT_ID == this.currentDepartment.ID).ToList();

                var hisBedIds = allHisBedBstys.Select(o => o.BED_ID).ToList();

                allBeds = BackendDataWorker.Get<V_HIS_BED>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && bedRooms.Exists(s => s.ID == o.BED_ROOM_ID) && hisBedIds.Contains(o.ID)).ToList();

                this.dicBedByServiceId = allHisBedBstys.GroupBy(o => o.BED_SERVICE_TYPE_ID).ToDictionary(g => g.Key,g => allBeds.Where(t => g.Any(b => b.BED_ID == t.ID)).ToList());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Thêm các field cache vào class
        private long _lastLoadedBedServiceId = -1;
        private DateTime _lastLoadedTimeFrom = DateTime.MinValue;
        private DateTime _lastLoadedTimeTo = DateTime.MinValue;
        //Viec 63227: cac ho so dang chi dinh cua lan nap danh sach giuong gan nhat (chi dung khi khoa 01412 = 2)
        private string _lastLoadedAssigningKey = "";

        private void LoadBedDataByServiceId(long serviceId, DateTime timeFrom, DateTime timeTo)
        {
            try
            {
                // ✅ Chỉ load lại nếu tham số thay đổi
                //Viec 63227: khoa 01412 = 2 thi so dang nam phu thuoc cac ho so dang chi dinh (khong tinh chinh ho), doi danh sach
                //nguoi benh duoc chon thi phai nap lai
                string assigningKey = HeldBedProcessor.IsHoldBedOn ? string.Join(",", this.GetAssigningTreatmentIds().OrderBy(o => o)) : "";
                if (serviceId == _lastLoadedBedServiceId
                    && timeFrom == _lastLoadedTimeFrom
                    && timeTo == _lastLoadedTimeTo
                    && assigningKey == _lastLoadedAssigningKey)
                {
                    return; // Không load lại, tránh trigger repaint grid 
                }

                _lastLoadedBedServiceId = serviceId;
                _lastLoadedTimeFrom = timeFrom;
                _lastLoadedTimeTo = timeTo;
                _lastLoadedAssigningKey = assigningKey;

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

        /// <summary>
        /// Cac ho so dang chi dinh, dung de khong tinh chinh cac nguoi benh nay la nguoi dang giu giuong (viec 63227).
        /// Dung dung cac ho so se duoc luu, giong dieu kien chi dinh nhieu nguoi benh khi luu: mo tu Buong benh va chon tu
        /// 2 nguoi benh tro len thi la cac nguoi benh duoc chon, con lai la ho so dang mo (khong lay them nguoi benh khac:
        /// Backend van tinh ho la nguoi dang nam).
        /// </summary>
        private List<long> GetAssigningTreatmentIds()
        {
            List<long> ids = new List<long>();
            try
            {
                if (this.workingAssignServiceADO != null && this.workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                    if (lstPatientSelect != null && lstPatientSelect.Count > 1)
                    {
                        foreach (var item in lstPatientSelect)
                        {
                            if (item != null && item.TREATMENT_ID > 0 && !ids.Contains(item.TREATMENT_ID))
                                ids.Add(item.TREATMENT_ID);
                        }
                        return ids;
                    }
                }
                if (this.treatmentId > 0)
                    ids.Add(this.treatmentId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ids;
        }

        /// <summary>
        /// So nguoi benh se duoc gui chi dinh (viec 63227): mo tu Buong benh va chon tu 2 nguoi benh tro len thi la so nguoi
        /// benh duoc chon (giong dieu kien chi dinh nhieu nguoi benh khi luu), con lai la 1.
        /// </summary>
        private int GetAssigningPatientCount()
        {
            try
            {
                if (this.workingAssignServiceADO != null && this.workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                    if (lstPatientSelect != null && lstPatientSelect.Count > 1)
                        return lstPatientSelect.Count;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 1;
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

                    //Viec 63227: khoa 01412 = 2 thi tinh ca nguoi dang giu giuong (chua roi buong, chua chuyen khoa,
                    //chua ra vien) ke ca khi chua ke giuong cho ngay tiep theo. Khong tinh cac nguoi benh dang chi dinh.
                    if (HeldBedProcessor.IsHoldBedOn)
                    {
                        dataBedLogs = HeldBedProcessor.AppendHeldBedLogs(dataBedLogs, bedIds, this.GetAssigningTreatmentIds(), startTimeFilter, finishTimeFilter);
                    }

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
                                //Khoa 01412 = 2: giuong chua khai bao so nguoi nam toi da coi la 1 nguoi, giong Backend
                                long? capacity = HeldBedProcessor.GetCapacity(itemADO.MAX_CAPACITY);
                                if (capacity.HasValue)
                                {
                                    if (dataByBedLogs.Count >= capacity)
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
                                //Viec 63227: ban ghi chep cua nguoi dang giu giuong mang ID am, chi lay ID that
                                itemADO.BedLogAllIds = dataByBedLogs.Where(o => o.ID > 0).Select(o => o.ID).ToList();
                                itemADO.AMOUNT = dataByBedLogs.Count;

                                itemADO.AMOUNT_STR = dataByBedLogs.Count + "/" + capacity;
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
