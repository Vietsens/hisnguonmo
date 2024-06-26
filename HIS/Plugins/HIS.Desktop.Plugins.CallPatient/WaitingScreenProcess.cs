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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatient
{
    enum MyEnum
    {

    }
    internal class WaitingScreenProcess
    {
        internal static string[] FileName, FilePath;


        internal static List<HisServiceReqViewSDO> serviceReqStatics = new List<HisServiceReqViewSDO>();

        internal static void FillDataToGridWaitingPatient(frmWaitingScreen control, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts)
        {
            try
            {
                if (control != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqViewFilter searchMVC = new MOS.Filter.HisServiceReqViewFilter();
                    //HisServiceReqLogic serviceReqLogic = new HisServiceReqLogic(param);
                    if (control.room != null)
                    {
                        searchMVC.EXECUTE_ROOM_ID = control.room.ID;
                    }
                    List<long> lstServiceReqSTT = new List<long>();
                    long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
                    long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                    searchMVC.INTRUCTION_TIME_FROM = startDay;
                    searchMVC.INTRUCTION_TIME_TO = endDay;
                    searchMVC.ORDER_FIELD = "INTRUCTION_DATE";
                    searchMVC.ORDER_FIELD1 = "SERVICE_REQ_STT_ID";
                    searchMVC.ORDER_FIELD2 = "PRIORITY";
                    searchMVC.ORDER_FIELD3 = "NUM_ORDER";
                    searchMVC.ORDER_FIELD4 = "BUSY_COUNT";

                    searchMVC.ORDER_DIRECTION = "DESC";
                    searchMVC.ORDER_DIRECTION1 = "ASC";
                    searchMVC.ORDER_DIRECTION2 = "DESC";
                    searchMVC.ORDER_DIRECTION3 = "ASC";
                    searchMVC.ORDER_DIRECTION4 = "ASC";
                    if (serviceReqStts != null && serviceReqStts.Count > 0)
                    {
                        List<long> serviceReqSttIds = serviceReqStts.Select(o => o.ID).ToList();
                        searchMVC.SERVICE_REQ_STT_IDs = serviceReqSttIds;

                        if (serviceReqStatics != null && serviceReqStatics.Count > 0)
                        {
                            serviceReqStatics.Clear();
                        }
                        //var result = serviceReqLogic.ROListVWithHospitalFeeInfo<Inventec.Core.ApiResultObject<List<HisServiceReqViewSDO>>>(searchMVC);
                        var result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET_VIEW_WITH_HOSPITAL_FEE_INFO, ApiConsumers.MosConsumer, searchMVC, param);
                        if (result != null)
                        {
                            //serviceReqStatics = result.Data;
                            control.gridControlWaitingPatient.BeginUpdate();
                            control.gridControlWaitingPatient.DataSource = null;
                            control.gridControlWaitingPatient.DataSource = serviceReqStatics;
                            control.gridControlWaitingPatient.EndUpdate();
                        }
                        else
                        {
                            control.gridControlWaitingPatient.BeginUpdate();
                            control.gridControlWaitingPatient.DataSource = null;
                            control.gridControlWaitingPatient.EndUpdate();
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

        internal static void FillDataToGridWaitingPatient(frmWaitingScreen_QY control, List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts)
        {
            try
            {
                if (control != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqViewFilter searchMVC = new MOS.Filter.HisServiceReqViewFilter();
                    //HisServiceReqLogic serviceReqLogic = new HisServiceReqLogic(param);

                    if (control.room != null)
                    {
                        searchMVC.EXECUTE_ROOM_ID = control.room.ID;
                    }
                    List<long> lstServiceReqSTT = new List<long>();
                    long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
                    long endDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
                    searchMVC.INTRUCTION_TIME_FROM = startDay;
                    searchMVC.INTRUCTION_TIME_TO = endDay;
                    searchMVC.ORDER_FIELD = "INTRUCTION_DATE";
                    searchMVC.ORDER_FIELD1 = "SERVICE_REQ_STT_ID";
                    searchMVC.ORDER_FIELD2 = "PRIORITY";
                    searchMVC.ORDER_FIELD3 = "NUM_ORDER";
                    searchMVC.ORDER_FIELD4 = "BUSY_COUNT";

                    searchMVC.ORDER_DIRECTION = "DESC";
                    searchMVC.ORDER_DIRECTION1 = "ASC";
                    searchMVC.ORDER_DIRECTION2 = "DESC";
                    searchMVC.ORDER_DIRECTION3 = "ASC";
                    searchMVC.ORDER_DIRECTION4 = "ASC";


                    if (serviceReqStts != null && serviceReqStts.Count > 0)
                    {
                        List<long> serviceReqSttIds = serviceReqStts.Select(o => o.ID).ToList();
                        if (serviceReqStatics == null)
                        {
                            serviceReqStatics = new List<HisServiceReqViewSDO>();
                        }
                        serviceReqStatics.Clear();
                        //var result = serviceReqLogic.ROListVWithHospitalFeeInfo<Inventec.Core.ApiResultObject<List<HisServiceReqViewSDO>>>(searchMVC);
                        var result = new BackendAdapter(param).Get<List<HisServiceReqViewSDO>>(HisRequestUriStore.HIS_SERVICE_REQ_GET_VIEW_WITH_HOSPITAL_FEE_INFO, ApiConsumers.MosConsumer, searchMVC, param);
                        if (result != null)
                        {
                            serviceReqStatics = result;
                            if (serviceReqStatics != null && serviceReqStatics.Count > 0)
                            {
                                int countPatient = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
                                // danh sách chờ kết quả cận lâm sàng
                                var serviceReqForCls = serviceReqStatics.Where(o => o.DEPENDENCIES_COUNT > 0 && o.BUSY_COUNT == 0 && o.SERVICE_REQ_STT_ID == HisServiceReqSttCFG.SERVICE_REQ_STT_ID__INPROCESS).Take(countPatient);
                                control.gridControlWaitingCls.BeginUpdate();
                                control.gridControlWaitingCls.DataSource = serviceReqForCls.ToList();
                                control.gridControlWaitingCls.EndUpdate();
                                // danh sách chờ khám
                                List<long> serviceReqForClsIds = serviceReqForCls.Select(o => o.ID).ToList();
                              var serviceReqForExams = serviceReqStatics.Where(o => !serviceReqForClsIds.Contains(o.ID) && serviceReqSttIds.Contains(o.SERVICE_REQ_STT_ID)).ToList().Take(countPatient);
                              //var serviceReqForExams = serviceReqStatics.Skip(0).Take(4);

                                control.gridControlWatingExams.BeginUpdate();
                                control.gridControlWatingExams.DataSource = serviceReqForExams.ToList();
                                control.gridControlWatingExams.EndUpdate();
                            }
                            else
                            {
                                control.gridControlWatingExams.BeginUpdate();
                                control.gridControlWatingExams.DataSource = null;
                                control.gridControlWatingExams.EndUpdate();
                            }
                        }
                        else
                        {
                            control.gridControlWatingExams.BeginUpdate();
                            control.gridControlWatingExams.DataSource = null;
                            control.gridControlWatingExams.EndUpdate();
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

        internal static void ShowFormInExtendMonitor(frmWaitingScreen control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
                    control.Show();
                }
                else
                {
                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = sc[1].Bounds.Width;
                    control.Top = sc[1].Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = sc[1].Bounds.Location;
                    Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
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
                FilePath = Directory.GetFiles(ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__DUONG_DAN_CHAY_FILE_VIDEO));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
