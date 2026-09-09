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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        // In tem barcode xet nghiem truc tiep qua BarTender khi tich chon "In tem barcode" (gridView7_5)
        // trong bang thiet lap in (config HIS.Desktop.Plugins.IsPrintTemBarcodeBartender = 1).
        // Du lieu lay tu serviceReqComboResultSDO (ket qua luu chi dinh), mau Mps000423 (Tmp/TempBartend/Mps000423/).

        private void InTemBarcodeXN()
        {
            try
            {
                if (this.serviceReqComboResultSDO == null || this.serviceReqComboResultSDO.ServiceReqs == null)
                {
                    return;
                }
                var serviceReqTest = this.serviceReqComboResultSDO.ServiceReqs.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList();
                string txt = "";
                if (serviceReqTest != null && serviceReqTest.Count > 0)
                {
                    foreach (var item in serviceReqTest)
                    {
                        var sereServBySRQ = this.serviceReqComboResultSDO.SereServs != null ? this.serviceReqComboResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == item.ID).ToList() : null;
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

        private void GenTextTemBarcodeXN(V_HIS_SERVICE_REQ serviceReq, V_HIS_SERE_SERV sereServ, ref string txt)
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
            txt += "," + serviceReq.TREATMENT_CODE;

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
    }
}
