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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServiceReqList.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmServiceReqList : HIS.Desktop.Utility.FormBase
    {
        // In tem barcode truc tiep qua BarTender (config HIS.Desktop.Plugins.IsPrintTemBarcodeBartender = 1):
        // ghi de datasource .txt trong Tmp/TempBartend/{MauIn}/ roi in file .btw tuong ung.
        // XN -> Mps000423, Mau (DONM) -> Mps000424, GPBL -> Mps000425.

        private void PrintTemBarcodeBartender()
        {
            try
            {
                List<ServiceReqADO> datas = gridControlServiceReq.DataSource as List<ServiceReqADO>;
                if (datas == null || datas.Count <= 0)
                {
                    XtraMessageBox.Show("Khong co y lenh nao duoc chon", "Thong bao");
                    return;
                }
                WaitingManager.Show();
                // in tem xet nghiem
                var xnList = datas.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && o.isCheck).ToList();
                if (xnList != null && xnList.Count > 0)
                {
                    InTemBarcodeXN(xnList);
                }
                // in tem mau
                var mauList = datas.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM && o.isCheck).ToList();
                if (mauList != null && mauList.Count > 0)
                {
                    InTemBarcodeMau(mauList);
                }
                // in tem giai phau benh ly
                var gpblList = datas.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL && o.isCheck).ToList();
                if (gpblList != null && gpblList.Count > 0)
                {
                    InTemBarcodeGpbl(gpblList);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InTemBarcodeXN(List<ServiceReqADO> datas)
        {
            try
            {
                string txt = "";
                if (datas != null && datas.Count > 0)
                {
                    HisSereServFilter hisSereServFilter = new HisSereServFilter();
                    hisSereServFilter.SERVICE_REQ_IDs = datas.Select(o => o.ID).ToList();
                    var listHisSereServ = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, hisSereServFilter, new CommonParam());
                    foreach (var item in datas)
                    {
                        var sereServBySRQ = listHisSereServ != null ? listHisSereServ.Where(o => o.SERVICE_REQ_ID == item.ID).ToList() : null;
                        if (sereServBySRQ != null && sereServBySRQ.Count() > 0)
                        {
                            GenTextTemBarcodeXN(item, sereServBySRQ.FirstOrDefault(), ref txt);
                        }
                    }
                    string resultPrint = new Bartender.PrintTestServiceReq.PrintTestServiceReq().StartPrintTestServiceReq(txt);
                    if (!string.IsNullOrWhiteSpace(resultPrint))
                    {
                        Inventec.Common.Logging.LogSystem.Warn(resultPrint);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InTemBarcodeMau(List<ServiceReqADO> datas)
        {
            try
            {
                string txt = "";
                if (datas != null && datas.Count > 0)
                {
                    foreach (var item in datas)
                    {
                        GenTextTemBarcodeMau(item, ref txt);
                    }
                    string resultPrint = new Bartender.PrintBloodServiceReq.PrintBloodServiceReq().StartPrintBloodServiceReq(txt);
                    if (!string.IsNullOrWhiteSpace(resultPrint))
                    {
                        Inventec.Common.Logging.LogSystem.Warn(resultPrint);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InTemBarcodeGpbl(List<ServiceReqADO> datas)
        {
            try
            {
                string txt = "";
                if (datas != null && datas.Count > 0)
                {
                    HisSereServFilter hisSereServFilter = new HisSereServFilter();
                    hisSereServFilter.SERVICE_REQ_IDs = datas.Select(o => o.ID).ToList();
                    var listHisSereServ = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, hisSereServFilter, new CommonParam());
                    foreach (var item in datas)
                    {
                        var sereServBySRQ = listHisSereServ != null ? listHisSereServ.Where(o => o.SERVICE_REQ_ID == item.ID).ToList() : null;
                        if (sereServBySRQ != null && sereServBySRQ.Count() > 0)
                        {
                            GenTextTemBarcodeGpbl(item, sereServBySRQ.FirstOrDefault(), ref txt);
                        }
                    }
                    string resultPrint = new Bartender.PrintGpblServiceReq.PrintGpblServiceReq().StartPrintGpblServiceReq(txt);
                    if (!string.IsNullOrWhiteSpace(resultPrint))
                    {
                        Inventec.Common.Logging.LogSystem.Warn(resultPrint);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GenTextTemBarcodeXN(ServiceReqADO serviceReq, HIS_SERE_SERV sereServ, ref string txt)
        {
            var service = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
            HIS_SERVICE servicePr = null;
            if (service != null && service.PARENT_ID.HasValue && service.PARENT_ID.Value > 0)
            {
                servicePr = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(o => o.ID == service.PARENT_ID);
            }
            txt += serviceReq.BARCODE;
            txt += "," + serviceReq.TDL_PATIENT_NAME;
            txt += "," + serviceReq.TDL_PATIENT_GENDER_NAME;
            txt += "," + serviceReq.TDL_PATIENT_DOB;
            txt += "," + (serviceReq.TDL_PATIENT_DOB > 10000000000000 ? serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4) : "");
            txt += "," + serviceReq.REQUEST_DEPARTMENT_NAME;
            txt += "," + serviceReq.REQUEST_ROOM_NAME;
            txt += "," + Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.DateTime.Get.Now() ?? 0);
            txt += "," + serviceReq.TDL_TREATMENT_CODE;

            if (serviceReq.TEST_SAMPLE_TYPE_ID.HasValue && serviceReq.TEST_SAMPLE_TYPE_ID.Value > 0)
            {
                var testSampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().FirstOrDefault(o => o.ID == serviceReq.TEST_SAMPLE_TYPE_ID);
                if (testSampleType != null)
                {
                    txt += "," + testSampleType.TEST_SAMPLE_TYPE_NAME.Replace(",", ";");
                }
                else
                {
                    txt += ",";
                }
            }
            else
            {
                txt += ",";
            }

            if (servicePr != null)
            {
                txt += "," + servicePr.SERVICE_NAME.Replace(",", ";");
            }
            else
            {
                txt += ",";
            }
            txt += "," + serviceReq.EXECUTE_ROOM_CODE;

            // xuong dong (1 row trong db)
            txt += "\n";
        }

        private void GenTextTemBarcodeGpbl(ServiceReqADO serviceReq, HIS_SERE_SERV sereServ, ref string txt)
        {
            var service = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
            HIS_SERVICE servicePr = null;
            if (service != null && service.PARENT_ID.HasValue && service.PARENT_ID.Value > 0)
            {
                servicePr = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(o => o.ID == service.PARENT_ID);
            }
            txt += serviceReq.SERVICE_REQ_CODE;
            txt += "," + serviceReq.TDL_PATIENT_NAME;
            txt += "," + serviceReq.TDL_PATIENT_GENDER_NAME;
            txt += "," + serviceReq.TDL_PATIENT_DOB;
            txt += "," + (serviceReq.TDL_PATIENT_DOB > 10000000000000 ? serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4) : "");
            txt += "," + serviceReq.REQUEST_DEPARTMENT_NAME;
            txt += "," + serviceReq.REQUEST_USERNAME;
            txt += "," + Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.DateTime.Get.Now() ?? 0);
            txt += "," + serviceReq.TDL_TREATMENT_CODE;

            if (serviceReq.TEST_SAMPLE_TYPE_ID.HasValue && serviceReq.TEST_SAMPLE_TYPE_ID.Value > 0)
            {
                var testSampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().FirstOrDefault(o => o.ID == serviceReq.TEST_SAMPLE_TYPE_ID);
                if (testSampleType != null)
                {
                    txt += "," + testSampleType.TEST_SAMPLE_TYPE_NAME.Replace(",", ";");
                }
                else
                {
                    txt += ",";
                }
            }
            else
            {
                txt += ",";
            }

            if (servicePr != null)
            {
                txt += "," + servicePr.SERVICE_NAME.Replace(",", ";");
            }
            else
            {
                txt += ",";
            }
            txt += "," + serviceReq.EXECUTE_ROOM_CODE;

            // xuong dong (1 row trong db)
            txt += "\n";
        }

        private void GenTextTemBarcodeMau(ServiceReqADO serviceReq, ref string txt)
        {
            HisExpMestFilter filter = new HisExpMestFilter();
            filter.SERVICE_REQ_ID = serviceReq.ID;
            var expMestList = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, new CommonParam());
            if (expMestList != null && expMestList.Count > 0)
            {
                txt += expMestList.FirstOrDefault().EXP_MEST_CODE;
            }

            txt += "," + serviceReq.TDL_PATIENT_NAME;
            txt += "," + serviceReq.TDL_PATIENT_GENDER_NAME;
            txt += "," + serviceReq.TDL_PATIENT_DOB;
            txt += "," + (serviceReq.TDL_PATIENT_DOB > 10000000000000 ? serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4) : "");
            txt += "," + serviceReq.REQUEST_DEPARTMENT_NAME;
            txt += "," + serviceReq.REQUEST_ROOM_NAME;
            txt += "," + Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.DateTime.Get.Now() ?? 0);
            txt += "," + serviceReq.TDL_TREATMENT_CODE;

            // xuong dong (1 row trong db)
            txt += "\n";
        }
    }
}
