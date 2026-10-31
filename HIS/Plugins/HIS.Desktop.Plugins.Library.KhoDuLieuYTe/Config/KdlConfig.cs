/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Library.KhoDuLieuYTe.Config
{
    /// <summary>
    /// Khóa cấu hình kết nối Kho dữ liệu ngành Y tế (SYT), mỗi cơ sở một cụm, các cụm cách nhau bằng "|":
    /// <c>HIS.SYT_KDL.CONNECTION_INFO</c> = MaCSKCB;Username;Password;BaseUrl|MaCSKCB;...
    /// (cùng quy ước với MOS.HIS_BABY.CONNECTION_INFO). MaCSKCB so với HIS_BRANCH.HEIN_MEDI_ORG_CODE.
    /// </summary>
    public class KdlConfig
    {
        public const string CONNECTION_INFO_KEY = "HIS.SYT_KDL.CONNECTION_INFO";

        public string MaCskcb { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }

        /// <summary>Có ít nhất 1 cụm kết nối hợp lệ -> hiện nút gửi.</summary>
        public static bool IsConfigured()
        {
            List<KdlConfig> list = GetAll();
            return list != null && list.Count > 0;
        }

        public static KdlConfig GetByMaCskcb(string maCskcb)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maCskcb)) return null;
                List<KdlConfig> list = GetAll();
                return list != null ? list.FirstOrDefault(o => o.MaCskcb == maCskcb.Trim()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private static List<KdlConfig> GetAll()
        {
            List<KdlConfig> result = new List<KdlConfig>();
            try
            {
                string value = HisConfigs.Get<string>(CONNECTION_INFO_KEY);
                if (string.IsNullOrWhiteSpace(value)) return result;
                foreach (string item in value.Split('|'))
                {
                    string[] f = item.Split(';');
                    if (f.Length < 4) continue;
                    KdlConfig cfg = new KdlConfig();
                    cfg.MaCskcb = f[0].Trim();
                    cfg.Username = f[1].Trim();
                    cfg.Password = f[2].Trim();
                    cfg.BaseUrl = f[3].Trim().TrimEnd('/');
                    if (string.IsNullOrEmpty(cfg.MaCskcb) || string.IsNullOrEmpty(cfg.Username)
                        || string.IsNullOrEmpty(cfg.Password) || string.IsNullOrEmpty(cfg.BaseUrl))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("KDL-SYT: cum cau hinh " + CONNECTION_INFO_KEY + " khong hop le, can MaCSKCB;Username;Password;BaseUrl");
                        continue;
                    }
                    result.Add(cfg);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
