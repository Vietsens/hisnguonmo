using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.KskServiceEditList.ADO;
using HIS.Desktop.Plugins.KskServiceEditList.Worker;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.KskServiceEditList
{
    public partial class frmKskServiceEditList
    {
        private List<ACS_USER> listAcsUser;

        private void InitComboLogin()
        {
            try
            {
                this.listAcsUser = BackendDataWorker.Get<ACS_USER>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 120, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 370);
                ControlEditorLoader.Load(this.cboLogin, this.listAcsUser, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.lblContract.Text = String.Format("{0} - {1}", this.kskContract.KSK_CONTRACT_CODE, this.kskContract.WORK_PLACE_NAME);
                this.lblPatientCount.Text = String.Format(Resources.ResourceMessage.ApDungChoBenhNhan, this.treatments.Count);
                this.cboLogin.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.dtIntructionTime.DateTime = DateTime.Now;
                this.toggleSwitch.IsOn = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dựng danh sách dòng dịch vụ (giống allSereServ của "Sửa chỉ định dịch vụ"):
        /// dịch vụ các hồ sơ đang có (tick sẵn) + dịch vụ trong nhóm dịch vụ KSK của hợp đồng (chưa tick).
        /// </summary>
        private void LoadServiceRows()
        {
            try
            {
                WaitingManager.Show();
                List<HIS_SERE_SERV> sereServs = new KskServiceEditWorker().GetSereServs(this.treatments.Select(o => o.ID).ToList());
                this.roomsByService = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToLookup(o => o.SERVICE_ID, o => new RoomADO { ID = o.ROOM_ID, ROOM_CODE = o.ROOM_CODE, ROOM_NAME = o.ROOM_NAME });
                Dictionary<long, HIS_KSK_SERVICE> kskServiceDic = this.GetKskServiceOfContract();
                this.allServiceRows = this.BuildServiceRows(sereServs, kskServiceDic);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dịch vụ trong nhóm dịch vụ KSK của hợp đồng (ưu tiên) và nhóm dùng chung (không gắn hợp đồng).
        /// 1 dịch vụ lấy 1 dòng nhóm KSK để thêm (giá, số lượng, phòng mặc định — giống Import).
        /// </summary>
        private Dictionary<long, HIS_KSK_SERVICE> GetKskServiceOfContract()
        {
            Dictionary<long, HIS_KSK> kskDic = BackendDataWorker.Get<HIS_KSK>()
                .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && (o.KSK_CONTRACT_ID == this.kskContract.ID || !o.KSK_CONTRACT_ID.HasValue))
                .ToDictionary(o => o.ID);
            return BackendDataWorker.Get<HIS_KSK_SERVICE>()
                .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && kskDic.ContainsKey(o.KSK_ID))
                .GroupBy(o => o.SERVICE_ID)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(o => kskDic[o.KSK_ID].KSK_CONTRACT_ID.HasValue).ThenBy(o => o.KSK_ID).First());
        }

        private List<ServiceRowADO> BuildServiceRows(List<HIS_SERE_SERV> sereServs, Dictionary<long, HIS_KSK_SERVICE> kskServiceDic)
        {
            Dictionary<long, V_HIS_SERVICE> serviceDic = BackendDataWorker.Get<V_HIS_SERVICE>().ToDictionary(o => o.ID);
            Dictionary<long, V_HIS_ROOM> roomDic = BackendDataWorker.Get<V_HIS_ROOM>().ToDictionary(o => o.ID);
            this.roomNameDic = roomDic.ToDictionary(o => o.Key, o => o.Value.ROOM_NAME);
            ILookup<long, HIS_SERE_SERV> sereServByService = (sereServs ?? new List<HIS_SERE_SERV>()).ToLookup(o => o.SERVICE_ID);
            HashSet<long> serviceIds = new HashSet<long>(sereServByService.Select(g => g.Key));
            serviceIds.UnionWith(kskServiceDic.Keys);
            int total = this.treatments.Count;

            List<ServiceRowADO> result = new List<ServiceRowADO>();
            foreach (long serviceId in serviceIds)
            {
                V_HIS_SERVICE service;
                serviceDic.TryGetValue(serviceId, out service);
                List<HIS_SERE_SERV> ss = sereServByService[serviceId].ToList();
                if (service == null && !ss.Any()) continue;

                ServiceRowADO row = new ServiceRowADO();
                row.SERVICE_ID = serviceId;
                row.SERVICE_CODE = service != null ? service.SERVICE_CODE : ss.First().TDL_SERVICE_CODE;
                row.SERVICE_NAME = service != null ? service.SERVICE_NAME : ss.First().TDL_SERVICE_NAME;
                row.SERVICE_TYPE_NAME = service != null ? service.SERVICE_TYPE_NAME : "";

                HIS_KSK_SERVICE ks;
                if (kskServiceDic.TryGetValue(serviceId, out ks))
                {
                    row.KSK_ID = ks.KSK_ID;
                    row.AMOUNT = ks.AMOUNT;
                    row.PRICE = ks.PRICE.HasValue ? ks.PRICE * (1 + (ks.VAT_RATIO ?? 0)) : null;
                }

                row.PatientCount = ss.Select(o => o.TDL_TREATMENT_ID).Distinct().Count();
                row.PatientCountDisplay = row.PatientCount + "/" + total;
                row.ExecutedCount = ss.Where(o => o.EXECUTE_TIME.HasValue).Select(o => o.TDL_TREATMENT_ID).Distinct().Count();
                row.IsExisting = row.PatientCount > 0;
                row.IsPartial = row.IsExisting && row.PatientCount < total;
                row.IsChecked = !row.IsExisting ? false : (row.IsPartial ? (bool?)null : true);

                List<long> roomIds = ss.Select(o => o.TDL_EXECUTE_ROOM_ID).Distinct().ToList();
                row.OriginalRoomId = roomIds.Count == 1 ? (long?)roomIds[0] : null;
                row.CurrentRoomDisplay = roomIds.Count > 1 ? Resources.ResourceMessage.NhieuPhong + ": "
                    + String.Join(", ", roomIds.Select(id => { V_HIS_ROOM r; return roomDic.TryGetValue(id, out r) ? r.ROOM_NAME : id.ToString(); })) : "";
                // Dịch vụ chưa có: phòng mặc định theo nhóm dịch vụ KSK (giống Import)
                row.RoomId = row.IsExisting ? row.OriginalRoomId : (ks != null ? (long?)ks.ROOM_ID : null);
                result.Add(row);
            }
            // Giống "Sửa chỉ định dịch vụ": dịch vụ đã chọn lên đầu, rồi theo tên
            return result.OrderByDescending(o => o.IsExisting).ThenBy(o => o.SERVICE_NAME).ToList();
        }

        /// <summary>
        /// "Phòng thực hiện": lọc lưới theo dịch vụ phòng thực hiện được; là phòng mặc định khi tick thêm dịch vụ
        /// </summary>
        private void InitComboRoom()
        {
            try
            {
                Dictionary<long, V_HIS_ROOM> roomDic = BackendDataWorker.Get<V_HIS_ROOM>().ToDictionary(o => o.ID);
                List<RoomADO> rooms = this.allServiceRows
                    .SelectMany(o => this.roomsByService[o.SERVICE_ID])
                    .GroupBy(o => o.ID).Select(g => g.First())
                    .Where(o => roomDic.ContainsKey(o.ID) && roomDic[o.ID].IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.ROOM_CODE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 80, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 330);
                ControlEditorLoader.Load(this.cboRoom, rooms, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
