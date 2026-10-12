using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    /// <summary>
    /// Danh mục nhóm dịch vụ dùng cho bộ lọc và cho việc gom nhóm theo hồ sơ (R5, R6):
    /// tên nhóm = tên loại dịch vụ; riêng loại Chẩn đoán hình ảnh lấy tên nhóm thực hiện (X-Quang, CT, MRI...) khi bật tách.
    /// </summary>
    internal static class ServiceGroupCatalog
    {
        internal const string KEY_PREFIX_SERVICE_TYPE = "T";
        internal const string KEY_PREFIX_EXECUTE_GROUP = "E";
        internal const string CDHA_FALLBACK_NAME = "CĐHA";

        internal static List<HIS_SERVICE_TYPE> GetServiceTypes()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                return data != null ? data.Where(o => o.IS_ACTIVE == 1).ToList() : new List<HIS_SERVICE_TYPE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<HIS_SERVICE_TYPE>();
            }
        }

        internal static List<HIS_EXECUTE_GROUP> GetExecuteGroups()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_EXECUTE_GROUP>();
                return data != null ? data.Where(o => o.IS_ACTIVE == 1).ToList() : new List<HIS_EXECUTE_GROUP>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<HIS_EXECUTE_GROUP>();
            }
        }

        /// <summary>Thứ tự loại dịch vụ: Khám luôn đứng đầu, còn lại theo NUM_ORDER rồi ID của danh mục.</summary>
        internal static long GetServiceTypeOrder(HIS_SERVICE_TYPE serviceType)
        {
            if (serviceType == null) return long.MaxValue;
            if (serviceType.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH) return -1;
            return (serviceType.NUM_ORDER ?? 0) * 100000 + serviceType.ID;
        }

        /// <summary>Danh sách nhóm để chọn trong bộ lọc: mọi loại dịch vụ + (khi bật tách) mọi nhóm thực hiện, đặt sau loại CĐHA.</summary>
        internal static List<ServiceGroupItemADO> GetFilterItems(bool splitCdha)
        {
            List<ServiceGroupItemADO> result = new List<ServiceGroupItemADO>();
            var serviceTypes = GetServiceTypes();
            foreach (var st in serviceTypes)
            {
                result.Add(new ServiceGroupItemADO
                {
                    Key = KEY_PREFIX_SERVICE_TYPE + st.ID,
                    Name = st.SERVICE_TYPE_NAME,
                    SortOrder = GetServiceTypeOrder(st),
                    SubOrder = ""
                });
            }
            if (splitCdha)
            {
                var cdha = serviceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA);
                long cdhaOrder = GetServiceTypeOrder(cdha);
                foreach (var eg in GetExecuteGroups())
                {
                    result.Add(new ServiceGroupItemADO
                    {
                        Key = KEY_PREFIX_EXECUTE_GROUP + eg.ID,
                        Name = eg.EXECUTE_GROUP_NAME,
                        SortOrder = cdhaOrder,
                        SubOrder = eg.EXECUTE_GROUP_NAME ?? ""
                    });
                }
            }
            return result.OrderBy(o => o.SortOrder).ThenBy(o => o.SubOrder).ThenBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Gom nhóm dịch vụ của một hồ sơ từ các dòng dịch vụ (R5, R6):
        /// bỏ dịch vụ đã xóa hoặc đánh dấu "không thực hiện"; bỏ trùng; Khám trước, còn lại theo thứ tự danh mục.
        /// </summary>
        internal static List<ServiceGroupItemADO> BuildGroups(IEnumerable<V_HIS_SERE_SERV> sereServs, bool splitCdha,
            List<HIS_SERVICE_TYPE> serviceTypes, List<HIS_EXECUTE_GROUP> executeGroups)
        {
            Dictionary<string, ServiceGroupItemADO> dic = new Dictionary<string, ServiceGroupItemADO>();
            if (sereServs == null) return new List<ServiceGroupItemADO>();
            foreach (var ss in sereServs)
            {
                if (ss == null) continue;
                if (ss.IS_DELETE == 1) continue;
                if (ss.IS_NO_EXECUTE == 1) continue;

                HIS_SERVICE_TYPE st = serviceTypes != null ? serviceTypes.FirstOrDefault(o => o.ID == ss.TDL_SERVICE_TYPE_ID) : null;
                long typeOrder = GetServiceTypeOrder(st);
                string key;
                string name;
                string subOrder = "";

                if (splitCdha
                    && ss.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                    && ss.TDL_EXECUTE_GROUP_ID.HasValue
                    && executeGroups != null)
                {
                    HIS_EXECUTE_GROUP eg = executeGroups.FirstOrDefault(o => o.ID == ss.TDL_EXECUTE_GROUP_ID.Value);
                    if (eg != null && !string.IsNullOrWhiteSpace(eg.EXECUTE_GROUP_NAME))
                    {
                        key = KEY_PREFIX_EXECUTE_GROUP + eg.ID;
                        name = eg.EXECUTE_GROUP_NAME;
                        subOrder = eg.EXECUTE_GROUP_NAME;
                    }
                    else
                    {
                        key = KEY_PREFIX_SERVICE_TYPE + ss.TDL_SERVICE_TYPE_ID;
                        name = CDHA_FALLBACK_NAME;
                    }
                }
                else
                {
                    key = KEY_PREFIX_SERVICE_TYPE + ss.TDL_SERVICE_TYPE_ID;
                    name = st != null && !string.IsNullOrWhiteSpace(st.SERVICE_TYPE_NAME)
                        ? st.SERVICE_TYPE_NAME
                        : (!string.IsNullOrWhiteSpace(ss.SERVICE_TYPE_NAME) ? ss.SERVICE_TYPE_NAME : "KHÁC");
                    if (splitCdha && ss.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                    {
                        name = CDHA_FALLBACK_NAME;
                    }
                }

                if (!dic.ContainsKey(key))
                {
                    dic[key] = new ServiceGroupItemADO { Key = key, Name = name, SortOrder = typeOrder, SubOrder = subOrder };
                }
            }
            return dic.Values.OrderBy(o => o.SortOrder).ThenBy(o => o.SubOrder).ThenBy(o => o.Name).ToList();
        }
    }
}
