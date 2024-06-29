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
        List<long> PrintIdByJsonPrint(string json_Print_Id)
        {
            List<long> result = new List<long>();
            try
            {
                var arrIds = json_Print_Id.Split(',', ';');
                if (arrIds != null && arrIds.Length > 0)
                {
                    foreach (var id in arrIds)
                    {
                        long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                        if (printId > 0)
                        {
                            result.Add(printId);
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

        bool DeleteJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            bool success = false;
            try
            {

                //call api update JSON_PRINT_ID for current treatment
                bool valid = true;
                valid = valid && (this.data != null);
                if (valid)
                {
                    List<FileHolder> listFileHolder = new List<FileHolder>();
                    if (data.ServiceReq != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisService = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ();
                        hisService.ID = data.ServiceReq.ID;
                        hisService.JSON_PRINT_ID = data.ServiceReq.JSON_PRINT_ID;
                        var listOldPrintIdOfServiceReqs = GetListPrintIdByServiceReqDelete();
                        ProcessServiceReqForUpdateJsonPrint(hisService, listOldPrintIdOfServiceReqs, sarPrintCreated);
                        SaveServiceReq(hisService);
                    }

                    if (data.SereServ != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_SERE_SERV hisSereServ = new MOS.EFMODEL.DataModels.HIS_SERE_SERV();
                        hisSereServ.ID = data.SereServ.ID;
                        hisSereServ.JSON_PRINT_ID = data.SereServ.JSON_PRINT_ID;
                        var listOldPrintIdOfSereServs = GetListPrintIdBySereServDelete();
                        ProcessSereServForUpdateJsonPrint(hisSereServ, listOldPrintIdOfSereServs, sarPrintCreated);
                        SaveSereServ(hisSereServ);
                    }

                    if (data.Treatment != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                        hisTreatment.ID = data.Treatment.ID;
                        hisTreatment.JSON_PRINT_ID = data.Treatment.JSON_PRINT_ID;
                        var listOldPrintIdOfTreatments = GetListPrintIdByTreatmentDelete();
                        ProcessTreatmentForUpdateJsonPrint(hisTreatment, listOldPrintIdOfTreatments, sarPrintCreated);
                        SaveTreatment(hisTreatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }
    }
}
