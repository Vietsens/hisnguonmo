using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    /// <summary>
    /// Co che phan phong theo can bang tai: hoi truoc MOS phong xu ly du kien cho CA NHOM
    /// dich vu dang tick, roi dien ngay vao o "Phong xu ly" thay vi de trong cho den luc Luu.
    ///
    /// Vi sao phai goi theo NHOM: nghiep vu phan phong cua MOS uu tien phong lam duoc nhieu
    /// dich vu nhat de gom benh nhan vao 1 phong. Goi le tung dich vu se mat tac dung gom nhom
    /// va lap lai dung loi cu la moi dich vu roi vao mot phong khac nhau.
    ///
    /// Hai ham SetDefaultExcuteRoom/SetPriorityRequired chi TRA CACHE nay, khong tu goi API,
    /// vi chung nhan vao danh sach phong cua MOT dich vu nen khong biet ca nhom.
    /// </summary>
    public partial class frmAssignService
    {
        /// <summary>
        /// Ket qua phan phong du kien MOS tra ve: service_id -> room_id
        /// </summary>
        private Dictionary<long, long> suggestRoomCache = new Dictionary<long, long>();

        /// <summary>
        /// Cac dich vu ma nguoi dung TU chon phong: khong duoc ghi de o nhung lan lam moi sau
        /// </summary>
        private HashSet<long> suggestRoomUserPicked = new HashSet<long>();

        /// <summary>
        /// Chu ky gom cac lan tick lien tiep lai thanh 1 lan goi API (milli giay).
        /// User chot 07/09/2026: bo do tre, tick la hien phong ngay. De 1ms thay vi 0 vi
        /// Timer.Interval khong nhan 0; van giu Timer de lan goi chay ngoai event tick
        /// (goi thang trong CellValueChanged se lam grid dung han giua luc ve lai).
        /// Da bo duoc do tre vi phan cham cua API da sua: bo IsValidData (quet ca danh muc
        /// dich vu, do duoc co lan 11,5 giay) thay bang loc HashSet.
        /// </summary>
        private const int SUGGEST_ROOM_DEBOUNCE_MS = 1;

        private System.Windows.Forms.Timer suggestRoomTimer;

        /// <summary>
        /// Dau van tay cua danh sach dich vu dang tick o lan goi API gan nhat.
        /// SetEnableButtonControl duoc goi tu 26 cho khac nhau (moi thao tac tren form deu chay qua),
        /// nen phai so dau van tay de CHI goi API khi danh sach tick THUC SU doi.
        /// Khong co buoc nay thi form bi treo vi goi API lien tuc tren luong UI.
        /// </summary>
        private string suggestRoomLastKey = null;

        /// <summary>
        /// Dung dich vu dang tick thanh 1 chuoi de so sanh: service_id + doi tuong thanh toan
        /// (doi doi tuong thanh toan co the doi phong lam duoc nen phai tinh vao)
        /// </summary>
        private string BuildSuggestRoomKey(List<SereServADO> checkeds)
        {
            if (checkeds == null || checkeds.Count == 0)
            {
                return "";
            }
            return String.Join(";",
                checkeds.Select(o => o.SERVICE_ID + "_" + o.PATIENT_TYPE_ID).OrderBy(o => o).ToArray());
        }

        /// <summary>
        /// Dang o trong lan tu dien phong cua chuong trinh (khong phai bac si chon).
        /// Su kien CellValueChanged cua cot phong NO CA KHI chuong trinh gan gia tri, nen phai
        /// co co nay de khong ghi nham "bac si tu chon" cho dong do.
        /// BUG da gap 08/09/2026: thieu co nay thi ngay sau khi chuong trinh dien phong, dich vu
        /// bi danh dau la user picked => bo tich roi tich lai thi khong duoc phan phong theo nhom
        /// nua, roi xuong nhanh lay phong DAU DANH SACH (hien SA08).
        /// </summary>
        private bool isFillingSuggestRoom = false;

        /// <summary>
        /// Danh dau nguoi dung tu chon phong cho 1 dich vu, de lan lam moi sau khong ghi de.
        /// Bo qua khi chinh chuong trinh dang dien phong.
        /// </summary>
        private void MarkUserPickedRoom(long serviceId)
        {
            if (this.isFillingSuggestRoom)
            {
                return;
            }
            if (serviceId > 0)
            {
                this.suggestRoomUserPicked.Add(serviceId);
            }
        }

        /// <summary>
        /// Tra phong du kien da lay duoc cho 1 dich vu. 0 = chua co.
        /// </summary>
        private long GetSuggestedRoomId(long serviceId)
        {
            long roomId = 0;
            if (serviceId > 0 && this.suggestRoomCache != null)
            {
                this.suggestRoomCache.TryGetValue(serviceId, out roomId);
            }
            return roomId;
        }

        /// <summary>
        /// Dich vu ma FilterExecuteRoom vua loc danh sach phong cho. Dat o day de
        /// SetDefaultExcuteRoom/SetPriorityRequired biet dang xu ly dich vu nao ma tra dung
        /// phong trong cache: hai ham do chi nhan danh sach phong, khong nhan service_id.
        /// </summary>
        private long suggestRoomCurrentServiceId = 0;

        /// <summary>
        /// Ban tra cache dung cho SetDefaultExcuteRoom/SetPriorityRequired.
        /// Tra ve phong MOS da phan cho DUNG dich vu dang xu ly.
        ///
        /// KHONG tham dinh lai phong theo excuteRoomList cua form: bo loc cua form KHAC bo loc
        /// cua MOS (form loc them IS_PAUSE o ProcessExecuteRoom, con MOS chi loc IS_PAUSE_ENCLITIC;
        /// nguoc lai MOS loc IS_EXECUTE ma form thi khong). Hai bo loc lech nhau nen neu tham dinh lai
        /// thi phong MOS da phan se bi loai AM THAM, o "Phong xu ly" trong ma khong co canh bao gi
        /// - dung loi da gap khi test 07/09/2026: tick 6 dich vu, chi 3 dich vu hien phong.
        ///
        /// MOS la noi quyet dinh phan phong; form chi hien thi lai. Chua co ket qua thi tra 0
        /// de o trong, luc Luu MOS van tu phan phong nen phieu khong bi thieu phong.
        /// </summary>
        private long GetSuggestedRoomIdByExecuteRooms(List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> excuteRoomList)
        {
            try
            {
                long serviceId = this.suggestRoomCurrentServiceId;
                long roomId = this.GetSuggestedRoomId(serviceId);
                if (roomId <= 0)
                {
                    return 0;
                }

                //CHAN CHEO DICH VU: hai ham goi vao day chi nhan danh sach phong, khong nhan
                //service_id, nen phai dua vao suggestRoomCurrentServiceId. Bien do co the con
                //giu gia tri cua dich vu TRUOC (co cho goi khong di qua FilterExecuteRoom).
                //BUG da gap 08/09/2026: SIE5858 chi duoc cau hinh 4 phong (SA08/SA09/VR03/TM11)
                //nhung lai duoc gan Phong Sieu Am So 12 - la phong cua dich vu khac trong cache.
                //=> phai kiem phong tra ve co ĐUNG la phong duoc cau hinh cho dich vu nay khong.
                //Dung HIS_SERVICE_ROOM (nguon cau hinh dich vu-phong) chu KHONG dung excuteRoomList
                //cua form: danh sach do bi form loc chat hon MOS (them IS_PAUSE) nen se loai oan
                //phong MOS da phan - dung loi da gap 07/09/2026.
                if (!this.IsRoomConfiguredForService(serviceId, roomId))
                {
                    Inventec.Common.Logging.LogSystem.Warn("GetSuggestRoom: phong tra ve khong thuoc cau hinh cua dich vu, bo qua."
                        + Inventec.Common.Logging.LogUtil.TraceData("serviceId", serviceId)
                        + Inventec.Common.Logging.LogUtil.TraceData("roomId", roomId));
                    return 0;
                }
                return roomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        /// <summary>
        /// Phong co duoc cau hinh lam duoc dich vu nay khong (bang HIS_SERVICE_ROOM, ban IS_ACTIVE).
        /// Day la nguon RONG - khong ap them cac bo loc rieng cua form.
        /// </summary>
        private bool IsRoomConfiguredForService(long serviceId, long roomId)
        {
            try
            {
                if (serviceId <= 0 || roomId <= 0)
                {
                    return false;
                }
                var serviceRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                if (serviceRooms == null)
                {
                    //Khong doc duoc cau hinh thi khong chan, de MOS quyet dinh
                    return true;
                }
                return serviceRooms.Any(o => o.SERVICE_ID == serviceId
                    && o.ROOM_ID == roomId
                    && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
        }

        /// <summary>
        /// Hen lam moi phong du kien. Nguoi dung tick nhanh nhieu dich vu lien tiep thi
        /// chi phat sinh 1 luot goi API sau lan tick cuoi.
        /// </summary>
        private void ScheduleRefreshSuggestRoom()
        {
            try
            {
                if (!HisConfigCFG.IsAssignRoomByLoadBalance)
                {
                    return;
                }

                //Chi hen goi API khi danh sach dich vu dang tick THUC SU doi.
                //SetEnableButtonControl chay tu 26 cho nen khong co buoc nay se goi API lien tuc.
                List<SereServADO> checkeds = this.ServiceIsleafADOs != null
                    ? this.ServiceIsleafADOs.FindAll(o => o.IsChecked)
                    : null;
                string key = this.BuildSuggestRoomKey(checkeds);
                if (key == this.suggestRoomLastKey)
                {
                    return;
                }
                this.suggestRoomLastKey = key;

                if (String.IsNullOrEmpty(key))
                {
                    //Bo tick het thi xoa cache va xoa luon danh sach bac si tu chon:
                    //tick lai la bat dau lai, khong giu lua chon cu.
                    this.suggestRoomCache.Clear();
                    this.suggestRoomUserPicked.Clear();
                    return;
                }

                //Dich vu vua bi bo tick thi bo khoi danh sach "bac si tu chon", de neu tick lai
                //thi duoc phan phong theo nhom nhu binh thuong.
                if (checkeds != null)
                {
                    List<long> stillChecked = checkeds.Select(o => o.SERVICE_ID).ToList();
                    List<long> dropped = this.suggestRoomUserPicked.Where(o => !stillChecked.Contains(o)).ToList();
                    foreach (long id in dropped)
                    {
                        this.suggestRoomUserPicked.Remove(id);
                    }
                }

                if (this.suggestRoomTimer == null)
                {
                    this.suggestRoomTimer = new System.Windows.Forms.Timer();
                    this.suggestRoomTimer.Interval = SUGGEST_ROOM_DEBOUNCE_MS;
                    this.suggestRoomTimer.Tick += new EventHandler(this.SuggestRoomTimer_Tick);
                }
                this.suggestRoomTimer.Stop();
                this.suggestRoomTimer.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SuggestRoomTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.suggestRoomTimer != null)
                {
                    this.suggestRoomTimer.Stop();
                }
                this.RefreshSuggestRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Goi MOS lay phong du kien cho toan bo dich vu dang tick, roi dien vao grid.
        /// Loi API hoac khong co ket qua: de nguyen o trong, KHONG bao loi chen ngang bac si
        /// (luc Luu MOS van tu phan phong nen phieu khong bi thieu phong).
        /// </summary>
        private void RefreshSuggestRoom()
        {
            try
            {
                if (!HisConfigCFG.IsAssignRoomByLoadBalance)
                {
                    return;
                }

                //Cung nguon voi luc Luu (frmAssignService.cs:3964) de bo dich vu dua vao
                //bo phan phong giong het luc luu
                List<SereServADO> checkeds = this.ServiceIsleafADOs != null
                    ? this.ServiceIsleafADOs.FindAll(o => o.IsChecked)
                    : null;
                if (checkeds == null || checkeds.Count == 0)
                {
                    this.suggestRoomCache.Clear();
                    return;
                }

                SuggestRoomSDO sdo = new SuggestRoomSDO();
                sdo.TreatmentId = this.treatmentId;
                sdo.RequestRoomId = this.GetRoomId();
                //Giu dung y nghia voi luc Luu: __Save.cs cung gan RequestRoomId = GetRoomId()
                sdo.ManualRequestRoomId = true;
                sdo.InstructionTime = this.GetInstructionTimeForSuggestRoom();
                sdo.ServiceReqDetails = new List<ServiceReqDetailSDO>();
                foreach (SereServADO item in checkeds)
                {
                    ServiceReqDetailSDO detail = new ServiceReqDetailSDO();
                    detail.ServiceId = item.SERVICE_ID;
                    detail.PatientTypeId = item.PATIENT_TYPE_ID;
                    detail.Amount = item.AMOUNT;
                    //De 0 de MOS tu phan phong; day chinh la muc dich cua lan goi nay
                    detail.RoomId = 0;
                    sdo.ServiceReqDetails.Add(detail);
                }

                CommonParam param = new CommonParam();
                List<SuggestRoomResultSDO> rs = new BackendAdapter(param).Post<List<SuggestRoomResultSDO>>(
                    RequestUriStore.HIS_SERVICE_REQ__GET_SUGGEST_ROOM,
                    ApiConsumers.MosConsumer,
                    sdo,
                    ProcessLostToken,
                    param);

                if (rs == null || rs.Count == 0)
                {
                    //Cho phep thu lai o lan doi danh sach tiep theo
                    this.suggestRoomLastKey = null;
                    Inventec.Common.Logging.LogSystem.Warn("GetSuggestRoom khong tra ve phong nao. De trong cho MOS phan luc Luu.");
                    return;
                }

                this.suggestRoomCache.Clear();
                foreach (SuggestRoomResultSDO r in rs)
                {
                    if (r != null && r.ServiceId > 0 && r.RoomId > 0)
                    {
                        this.suggestRoomCache[r.ServiceId] = r.RoomId;
                    }
                }

                this.ApplySuggestRoomToGrid(checkeds);
            }
            catch (TypeLoadException ex)
            {
                //LECH PHIEN BAN DLL, khong phai loi nghiep vu. Ghi muc Error de nhin thay ngay.
                //Da gap 08/09/2026: MOS.SDO.dll o ReferencedAssemblies la ban cu, thieu 2 kieu
                //SuggestRoomSDO/SuggestRoomResultSDO => TypeLoadException ngay khi JIT vao ham nay
                //=> API khong bao gio duoc goi, o "Phong xu ly" trong, log chi co Warn khong ai de y.
                //Cach xu ly: dong bo MOS.SDO.dll (ban co 2 kieu tren) roi TAT HAN va mo lai HIS.Desktop.
                this.suggestRoomLastKey = null;
                Inventec.Common.Logging.LogSystem.Error(
                    "RefreshSuggestRoom: lech phien ban MOS.SDO.dll (thieu kieu du lieu). "
                    + "Can dong bo lai MOS.SDO.dll o thu muc ReferencedAssemblies va khoi dong lai HIS.Desktop.", ex);
            }
            catch (MissingMethodException ex)
            {
                //Cung ho lech phien ban DLL: thieu phuong thuc thay vi thieu kieu
                this.suggestRoomLastKey = null;
                Inventec.Common.Logging.LogSystem.Error(
                    "RefreshSuggestRoom: lech phien ban DLL (thieu phuong thuc). Can dong bo lai DLL.", ex);
            }
            catch (Exception ex)
            {
                //Khong chen ngang nguoi dung: loi lay phong du kien khong lam hong nghiep vu chi dinh.
                //Xoa dau van tay de lan doi danh sach sau con thu lai duoc.
                this.suggestRoomLastKey = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dien phong du kien vao cac dong dang tick. Bo qua dong ma nguoi dung tu chon phong.
        /// </summary>
        private void ApplySuggestRoomToGrid(List<SereServADO> checkeds)
        {
            try
            {
                if (checkeds == null || checkeds.Count == 0)
                {
                    return;
                }

                bool changed = false;
                //Bat co trong suot lan dien: su kien CellValueChanged se nO nhung khong duoc
                //ghi nhan la "bac si tu chon phong"
                this.isFillingSuggestRoom = true;
                try
                {
                    foreach (SereServADO item in checkeds)
                    {
                        if (item == null || this.suggestRoomUserPicked.Contains(item.SERVICE_ID))
                        {
                            continue;
                        }
                        long roomId = this.GetSuggestedRoomId(item.SERVICE_ID);
                        if (roomId > 0 && item.TDL_EXECUTE_ROOM_ID != roomId)
                        {
                            item.TDL_EXECUTE_ROOM_ID = roomId;
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        this.gridViewServiceProcess.RefreshData();
                    }
                }
                finally
                {
                    this.isFillingSuggestRoom = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thoi diem chi dinh dang chon tren form; loi thi lui ve thoi diem hien tai
        /// de van lay duoc phong du kien.
        /// </summary>
        private long GetInstructionTimeForSuggestRoom()
        {
            try
            {
                DateTime? dt = this.dtInstructionTime.EditValue as DateTime?;
                if (dt.HasValue)
                {
                    long? time = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt.Value);
                    if (time.HasValue && time.Value > 0)
                    {
                        return time.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            long? now = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
            return now.HasValue ? now.Value : 0;
        }
    }
}
