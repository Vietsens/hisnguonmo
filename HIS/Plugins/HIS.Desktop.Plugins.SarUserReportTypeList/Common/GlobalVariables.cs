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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;

namespace HIS.Desktop.Plugins.SarUserReportTypeList
{
    public class GlobalVariables
    {
        public static readonly string APPLICATION_CODE = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];//Ma ung dung khai bao tren aaa
        public static Dictionary<string, object> DicConfig { get; set; }
        private static object syncRoot = new Object();

        public const int ActionAdd = 1;//1 -> Add
        public const int ActionEdit = 2;//2 -> Edit
        public const int ActionView = 3;//3 -> View
        public const int ActionViewForEdit = 4;//4 -> View for edit

        public const int MaxImage = 12;//10 -> Max Image Camera

        public const short CommonNumberTrue = (short)1;
        public const string CommonStringTrue = "1";

        public const string HIS_PAY_FORM_CODE__CONSTANT = "01";
        public static long ROOM_ID_FOR_WAITING_SCREEN { get; set; }
        public static string TemnplatePathFolder = "PrintTemplate";
        public static bool isLogouter = false;
        public static bool IsLostToken = false;
        public static string CurrentTabPage { get; set; }
        public static int SelectedTabPageIndex { get; set; }
        public static string ChooseRoomMultiChoseByCode { get; set; }
        public static List<Inventec.Desktop.Common.Modules.Module> currentModules { get; set; }
        public static List<Inventec.Desktop.Common.Modules.Module> currentModuleRaws { get; set; }
        public static List<ACS.EFMODEL.DataModels.ACS_CONTROL> currentControls { get; set; }
        public static List<Image> listImage { get; set; }
        public static Dictionary<long, decimal> dicNumOrderInAccountBook { get; set; }
        static ACS.SDO.AcsAuthorizeSDO acsAuthorizeSDO { get; set; }
        public static ACS.SDO.AcsAuthorizeSDO AcsAuthorizeSDO
        {
            get
            {
                lock (syncRoot)
                    return acsAuthorizeSDO;
            }
            set
            {
                lock (syncRoot)
                    acsAuthorizeSDO = value;
            }
        }
        static long currentRoomTypeId { get; set; }
        public static long CurrentRoomTypeId
        {
            get
            {
                lock (syncRoot)
                    return currentRoomTypeId;
            }
            set
            {
                lock (syncRoot)
                    currentRoomTypeId = value;
            }
        }
        static string currentRoomTypeCode { get; set; }
        public static string CurrentRoomTypeCode
        {
            get
            {
                lock (syncRoot)
                    return currentRoomTypeCode;
            }
            set
            {
                lock (syncRoot)
                    currentRoomTypeCode = value;
            }
        }
        static List<long> currentRoomTypeIds { get; set; }
        public static List<long> CurrentRoomTypeIds
        {
            get
            {
                lock (syncRoot)
                    return currentRoomTypeIds;
            }
            set
            {
                lock (syncRoot)
                    currentRoomTypeIds = value;
            }
        }
        static List<string> currentRoomTypeCodes { get; set; }
        public static List<string> CurrentRoomTypeCodes
        {
            get
            {
                lock (syncRoot)
                    return currentRoomTypeCodes;
            }
            set
            {
                lock (syncRoot)
                    currentRoomTypeCodes = value;
            }
        }
        public static List<Image> ListImage
        {
            get
            {
                lock (syncRoot)
                    return listImage;
            }
            set
            {
                lock (syncRoot)
                    listImage = value;
            }
        }

        public static string IsUseRoomCounter { get; set; }
        public static long PrintFullTemplate { get; set; }
        public static long ChonManHinhGoiBenhNhan { get; set; }

        public static long HAS_PRIORITY = 1;
        public static string IsNotRequireFee { get; set; }
        public static long AlertExpriedTimeHeinCardBhyt { get; set; }
        public static string DIALOG_FILTER_STRING = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
        public static long CheDoInChoCacChucNangTrongPhanMem { get; set; }
        public static long CheDoInPhieuThuocGayNghienHuongTamThan { get; set; }
        public static string UserDelegateLoginName { get; set; }

        public const int MAX_REQUEST_LENGTH_PARAM = 100;

        public static Dictionary<long, List<V_HIS_SERVICE_REQ_1>> dicServiceReqForCallPatient { get; set; }

        public static Dictionary<string, string> dicPrinter { get; set; }//Kieu may in
        private const string HIS_CONFIG__PRINT_TYPE__PRINTER = "His.Config.PrintType.Printer";
        public static Dictionary<string, string> LoadConfigPrintType()
        {
            dicPrinter = new Dictionary<string, string>();
            string value = (System.Configuration.ConfigurationSettings.AppSettings[HIS_CONFIG__PRINT_TYPE__PRINTER] ?? "");
            if (!String.IsNullOrEmpty(value))
            {
                string[] configs = value.Split(';');
                if (configs == null || configs.Length <= 0)
                {
                    throw new NullReferenceException("Khong cat duoc du lieu cau hinh: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => value), value));
                }

                foreach (var item in configs)
                {
                    if (String.IsNullOrEmpty(item))
                        continue;
                    var data = item.Split(':');
                    if (data == null || data.Length != 2)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Du lieu cau hinh khong chinh xac: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                        continue;
                    }
                    if (String.IsNullOrEmpty(data[0]) || String.IsNullOrEmpty(data[0].Trim()) || String.IsNullOrEmpty(data[1]) || String.IsNullOrEmpty(data[1].Trim()))
                    {
                        Inventec.Common.Logging.LogSystem.Info("Ma loai in hoac ten may in trong: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                        continue;
                    }
                    dicPrinter[data[0].Trim()] = data[1].Trim();
                }
            }

            return dicPrinter;
        }


        private static List<ACS.EFMODEL.DataModels.ACS_USER> acsUser;
        internal static List<ACS.EFMODEL.DataModels.ACS_USER> ACS_USER_DATAs
        {
            get
            {
                if (acsUser == null || acsUser.Count <= 0)
                {
                    Inventec.Core.CommonParam paramCommon = new Inventec.Core.CommonParam();
                    ACS.Filter.AcsUserFilter acsFilter = new ACS.Filter.AcsUserFilter();
                    acsFilter.IS_ACTIVE = 1;
                    acsUser = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, acsFilter, paramCommon);
                }
                return acsUser;
            }
            set
            {
                acsUser = value;
            }
        }
    }
}
