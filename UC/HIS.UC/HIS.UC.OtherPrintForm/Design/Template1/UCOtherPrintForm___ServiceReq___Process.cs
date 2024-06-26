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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.OtherPrintForm;
using EXE.DAL.Base;
using EXE.LOGIC.Base;
using EXE.LOGIC.HisTreatment;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Common.RichEditor.Base;

namespace HIS.UC.OtherPrintForm.Design.Template1
{
    public partial class UCOtherPrintForm : UserControl
    {

        private void ProcessServiceReqForUpdateJsonPrint(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq, List<long> jsonPrintId, SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
            {
                if (data.ServiceReq != null)
                {
                    if (jsonPrintId == null)
                    {
                        jsonPrintId = new List<long>();
                    }
                    //jsonPrintId.Add(sarPrintCreated.ID);

                    string printIds = "";
                    foreach (var item in jsonPrintId)
                    {
                        printIds += item.ToString() + ",";
                    }

                    hisServiceReq.JSON_PRINT_ID = printIds;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveServiceReq(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                var hisServiceReqWithFileResultSDO = new EXE.LOGIC.HisServiceReq.HisServiceReqLogic(param).UpdateJsonPrintId<HIS_SERVICE_REQ>(hisServiceReq);
                if (hisServiceReqWithFileResultSDO != null)
                {

                    data.ServiceReq.JSON_PRINT_ID = hisServiceReqWithFileResultSDO.JSON_PRINT_ID;
                    data.ServiceReq.MODIFIER = hisServiceReqWithFileResultSDO.MODIFIER;
                    data.ServiceReq.MODIFY_TIME = hisServiceReqWithFileResultSDO.MODIFY_TIME;

                    FillDataToGridControl();
                    success = true;
                }

                #region Show message
                ResultManager.ShowMessage(param, success);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        List<long> GetListPrintIdByServiceReqDelete()
        {
            List<long> result = new List<long>();
            try
            {
                List<V_HIS_SERVICE_REQ> serviceReqs = new List<V_HIS_SERVICE_REQ>();
                var sarDeleteID = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewSarPrint.GetFocusedRow();
                serviceReqs.Add(data.ServiceReq);
                if (serviceReqs != null && serviceReqs.Count > 0)
                {
                    foreach (var item in serviceReqs)
                    {
                        if (!String.IsNullOrEmpty(item.JSON_PRINT_ID))
                        {
                            var arrIds = item.JSON_PRINT_ID.Split(',', ';');
                            if (arrIds != null && arrIds.Length > 0)
                            {
                                foreach (var id in arrIds)
                                {
                                    long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                                    if (printId > 0 && printId != sarDeleteID.ID)
                                    {
                                        result.Add(printId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

    }
}
