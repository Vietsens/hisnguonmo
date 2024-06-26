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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.UC.SecondaryIcd;
using Inventec.Core;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MPS.Processor.Mps000272.PDO.Config;
using HIS.Desktop.Plugins.Bordereau.Config;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.ApprovalSurgery
{
    public partial class UCApprovalSurgery : UserControlBase
    {
        private bool InLichDuyetMo(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                V_HIS_PTTT_CALENDAR ptttCalendar = gridViewPtttCalendar.GetFocusedRow() as V_HIS_PTTT_CALENDAR;
                if (ptttCalendar == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin lịch");
                    return false;
                }

                CommonParam param = new CommonParam();
                HisSereServView13Filter sereServFilter = new HisSereServView13Filter();
                sereServFilter.PTTT_CALENDAR_ID = ptttCalendar.ID;
                sereServFilter.PTTT_APPROVAL_STT_IDs = new List<long>();
                sereServFilter.PTTT_APPROVAL_STT_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__NEW);
                sereServFilter.PTTT_APPROVAL_STT_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__APPROVED);
                List<V_HIS_SERE_SERV_13> sereServ13s = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_13>>("api/HisSereServ/GetView13", ApiConsumers.MosConsumer, sereServFilter, param);
                if (sereServ13s == null || sereServ13s.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy yêu cầu nào trong lịch mổ!");
                    return false;
                }

                List<HIS_SERE_SERV_PTTT> lstHisSereServPttt = null;
                HIS_SERE_SERV_PTTT hisSereServPttt = null;
                List<V_HIS_EKIP_PLAN_USER> ekipPlanUsers = null;
                HIS_PTTT_METHOD method = null;
                HIS_EMOTIONLESS_METHOD emotionless = null;
                List<HIS_PTTT_METHOD> methods = BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<HIS_EMOTIONLESS_METHOD> lstEmotionless = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o => o.IS_ACTIVE == 1).ToList();

                if (sereServ13s != null && sereServ13s.Count > 0)
                {
                    HisEkipPlanUserViewFilter ekipPlanUserFilter = new HisEkipPlanUserViewFilter();
                    ekipPlanUserFilter.EKIP_PLAN_IDs = sereServ13s.Select(o => o.EKIP_PLAN_ID ?? 0).ToList();
                    ekipPlanUsers = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EKIP_PLAN_USER>>("api/HisEkipPlanUser/GetView", ApiConsumers.MosConsumer, ekipPlanUserFilter, param);

                    CommonParam paramCom = new CommonParam();
                    HisSereServPtttFilter ptttFilter = new HisSereServPtttFilter();
                    ptttFilter.SERE_SERV_ID = sereServ13s.FirstOrDefault().ID;
                    lstHisSereServPttt = new BackendAdapter(paramCom)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/Get", ApiConsumers.MosConsumer, ptttFilter, paramCom);

                    if (lstHisSereServPttt != null && lstHisSereServPttt.Count > 0)
                    {
                        hisSereServPttt = lstHisSereServPttt.FirstOrDefault();
                    }
                    if (methods != null && methods.Count > 0 && hisSereServPttt != null && (hisSereServPttt.PTTT_METHOD_ID != null && hisSereServPttt.PTTT_METHOD_ID != 0))
                    {
                        method = methods.Where(o => o.ID == hisSereServPttt.PTTT_METHOD_ID).FirstOrDefault();
                    }
                    if (lstEmotionless != null && lstEmotionless.Count > 0 && hisSereServPttt != null && (hisSereServPttt.EMOTIONLESS_METHOD_ID != null && hisSereServPttt.EMOTIONLESS_METHOD_ID != 0))
                    {
                        emotionless = lstEmotionless.Where(o => o.ID == hisSereServPttt.EMOTIONLESS_METHOD_ID).FirstOrDefault();
                    }
                }

                ExecuteRoleCFG executeRoleCFG = new ExecuteRoleCFG();
                executeRoleCFG.EXECUTE_ROLE__PTV = HisExecuteRoleCFG.EXECUTE_ROLE_CODE__ID_MAIN;

                List<HIS_EXECUTE_ROLE> HisExecuteRoles = new List<HIS_EXECUTE_ROLE>();
                HisExecuteRoles = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();


                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((sereServ13s != null ? sereServ13s.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.roomId);
                

                MPS.Processor.Mps000272.PDO.Mps000272PDO rdo = new MPS.Processor.Mps000272.PDO.Mps000272PDO(
                   ptttCalendar,
                   sereServ13s,
                   ekipPlanUsers,
                   executeRoleCFG,
                   method,
                   emotionless,
                   HisExecuteRoles
                   );

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }



        private bool InLichDuyetMo_(string printTypeCode, string fileName)
        {
            WaitingManager.Show();
            bool result = false;
            try
            {

                List<V_HIS_PTTT_CALENDAR> listSelection = new List<V_HIS_PTTT_CALENDAR>();
                var rowHandles = gridViewPtttCalendar.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_PTTT_CALENDAR)gridViewPtttCalendar.GetRow(i);
                        if (row != null)
                        {
                            listSelection.Add(row);
                        }
                    }
                }
                if (listSelection != null && listSelection.Count > 0)
                {
                    foreach (var item in listSelection)
                    {
                        //V_HIS_PTTT_CALENDAR ptttCalendar = gridViewPtttCalendar.GetFocusedRow() as V_HIS_PTTT_CALENDAR;
                        //if (item == null)
                        //{
                        //    MessageBox.Show("Không tìm thấy thông tin lịch");
                        //    return false;
                        //}

                        CommonParam param = new CommonParam();
                        HisSereServView13Filter sereServFilter = new HisSereServView13Filter();
                        sereServFilter.PTTT_CALENDAR_ID = item.ID;
                        sereServFilter.PTTT_APPROVAL_STT_IDs = new List<long>();
                        sereServFilter.PTTT_APPROVAL_STT_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__NEW);
                        sereServFilter.PTTT_APPROVAL_STT_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__APPROVED);
                        List<V_HIS_SERE_SERV_13> sereServ13s = new BackendAdapter(param)
                            .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_13>>("api/HisSereServ/GetView13", ApiConsumers.MosConsumer, sereServFilter, param);
                        if (sereServ13s == null || sereServ13s.Count == 0)
                        {
                            MessageBox.Show("Không tìm thấy yêu cầu nào trong lịch mổ!");
                            return false;
                        }

                        List<HIS_SERE_SERV_PTTT> lstHisSereServPttt = null;
                        HIS_SERE_SERV_PTTT hisSereServPttt = null;
                        List<V_HIS_EKIP_PLAN_USER> ekipPlanUsers = null;
                        HIS_PTTT_METHOD method = null;
                        HIS_EMOTIONLESS_METHOD emotionless = null;
                        List<HIS_PTTT_METHOD> methods = BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.IS_ACTIVE == 1).ToList();
                        List<HIS_EMOTIONLESS_METHOD> lstEmotionless = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o => o.IS_ACTIVE == 1).ToList();

                        if (sereServ13s != null && sereServ13s.Count > 0)
                        {
                            HisEkipPlanUserViewFilter ekipPlanUserFilter = new HisEkipPlanUserViewFilter();
                            ekipPlanUserFilter.EKIP_PLAN_IDs = sereServ13s.Select(o => o.EKIP_PLAN_ID ?? 0).ToList();
                            ekipPlanUsers = new BackendAdapter(param)
                                .Get<List<MOS.EFMODEL.DataModels.V_HIS_EKIP_PLAN_USER>>("api/HisEkipPlanUser/GetView", ApiConsumers.MosConsumer, ekipPlanUserFilter, param);

                            CommonParam paramCom = new CommonParam();
                            HisSereServPtttFilter ptttFilter = new HisSereServPtttFilter();
                            ptttFilter.SERE_SERV_ID = sereServ13s.FirstOrDefault().ID;
                            lstHisSereServPttt = new BackendAdapter(paramCom)
                            .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/Get", ApiConsumers.MosConsumer, ptttFilter, paramCom);

                            if (lstHisSereServPttt != null && lstHisSereServPttt.Count > 0)
                            {
                                hisSereServPttt = lstHisSereServPttt.FirstOrDefault();
                            }
                            if (methods != null && methods.Count > 0 && hisSereServPttt != null && (hisSereServPttt.PTTT_METHOD_ID != null && hisSereServPttt.PTTT_METHOD_ID != 0))
                            {
                                method = methods.Where(o => o.ID == hisSereServPttt.PTTT_METHOD_ID).FirstOrDefault();
                            }
                            if (lstEmotionless != null && lstEmotionless.Count > 0 && hisSereServPttt != null && (hisSereServPttt.EMOTIONLESS_METHOD_ID != null && hisSereServPttt.EMOTIONLESS_METHOD_ID != 0))
                            {
                                emotionless = lstEmotionless.Where(o => o.ID == hisSereServPttt.EMOTIONLESS_METHOD_ID).FirstOrDefault();
                            }
                        }

                        ExecuteRoleCFG executeRoleCFG = new ExecuteRoleCFG();
                        executeRoleCFG.EXECUTE_ROLE__PTV = HisExecuteRoleCFG.EXECUTE_ROLE_CODE__ID_MAIN;

                        List<HIS_EXECUTE_ROLE> HisExecuteRoles = new List<HIS_EXECUTE_ROLE>();
                        HisExecuteRoles = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((sereServ13s != null ? sereServ13s.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.roomId);
                
                        MPS.Processor.Mps000272.PDO.Mps000272PDO rdo = new MPS.Processor.Mps000272.PDO.Mps000272PDO(
                           item,
                           sereServ13s,
                           ekipPlanUsers,
                           executeRoleCFG,
                           method,
                           emotionless,
                           HisExecuteRoles
                           );

                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
                        
                    }
                    return result;
                    WaitingManager.Hide();
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
