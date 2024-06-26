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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientCLS
{
    enum MyEnum
    {

    }
    internal class WaitingScreenProcess
    {
        internal static string[] FileName, FilePath;


        internal static List<V_HIS_SERVICE_REQ_1> serviceReqStatics = new List<V_HIS_SERVICE_REQ_1>();

        internal static void FillDataToGridWaitingPatient(frmWaitingScreen_QY control, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts)
        {
            try
            {
                if (control != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqView1Filter hisServiceReqFilter = new HisServiceReqView1Filter();
                    //HisServiceReqLogic serviceReqLogic = new HisServiceReqLogic(param);

                    if (control.room != null)
                    {
                        hisServiceReqFilter.EXECUTE_ROOM_ID = control.room.ID;
                    }
                    hisServiceReqFilter.HAS_EXECUTE = true;

                    List<long> lstServiceReqSTT = new List<long>();
                    long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
                    long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                    hisServiceReqFilter.INTRUCTION_TIME_FROM = startDay;
                    hisServiceReqFilter.INTRUCTION_TIME_TO = endDay;

                    hisServiceReqFilter.ORDER_FIELD = "INTRUCTION_DATE";
                    hisServiceReqFilter.ORDER_FIELD3 = "NUM_ORDER";

                    hisServiceReqFilter.ORDER_DIRECTION = "DESC";
                    hisServiceReqFilter.ORDER_DIRECTION3 = "ASC";

                    if (serviceReqStts != null && serviceReqStts.Count > 0)
                    {
                        List<long> lstServiceReqSTTFilter = serviceReqStts.Select(o => o.ID).ToList();
                        hisServiceReqFilter.SERVICE_REQ_STT_IDs = lstServiceReqSTTFilter;
                        if (serviceReqStatics == null)
                        {
                            serviceReqStatics = new List<V_HIS_SERVICE_REQ_1>();
                        }
                        serviceReqStatics.Clear();
                        //var result = serviceReqLogic.ROListVWithHospitalFeeInfo<Inventec.Core.ApiResultObject<List<HisServiceReqViewSDO>>>(searchMVC);
                        var result = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ_1>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW_1, ApiConsumers.MosConsumer, hisServiceReqFilter, param);
                        if (result != null)
                        {
                            serviceReqStatics = result;
                            if (serviceReqStatics != null && serviceReqStatics.Count > 0)
                            {
                                int countPatient = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
                                // danh sách chờ kết quả cận lâm sàng
                                var serviceReqForCls = serviceReqStatics.Take(countPatient);
                                control.gridControlWaitingCls.BeginUpdate();
                                control.gridControlWaitingCls.DataSource = serviceReqForCls.ToList();
                                control.gridControlWaitingCls.EndUpdate();
                            }
                        }
                    }

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public static void GetFilePath()
        {
            try
            {
                FilePath = Directory.GetFiles( ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__DUONG_DAN_CHAY_FILE_VIDEO));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
