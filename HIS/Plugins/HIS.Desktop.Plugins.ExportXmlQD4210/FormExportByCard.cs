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
using His.Bhyt.ExportXml.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExportXmlQD4210.Base;
using HIS.Desktop.Plugins.ExportXmlQD4210.Config;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD4210
{
    public partial class FormExportByCard : Form
    {
        CommonParam param = new CommonParam();
        List<V_HIS_SERE_SERV_TEIN> hisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
        List<V_HIS_SERE_SERV_PTTT> hisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
        List<HIS_DHST> listDhst = new List<HIS_DHST>();
        List<HIS_TRACKING> hisTrackings = new List<HIS_TRACKING>();
        List<V_HIS_TREATMENT_3> hisTreatments = new List<V_HIS_TREATMENT_3>();
        List<V_HIS_SERE_SERV_2> ListSereServ = new List<V_HIS_SERE_SERV_2>();
        List<HIS_EKIP_USER> ListEkipUser = new List<HIS_EKIP_USER>();
        List<V_HIS_BED_LOG> ListBedlog = new List<V_HIS_BED_LOG>();
        List<V_HIS_HEIN_APPROVAL> listHeinApproval = new List<V_HIS_HEIN_APPROVAL>();

        List<V_HIS_TREATMENT_1> listSelection = new List<V_HIS_TREATMENT_1>();

        HIS_BRANCH _Branch = null;
        string FilePath = "";

        public FormExportByCard()
        {
            InitializeComponent();
        }

        public FormExportByCard(HIS_BRANCH branch)
            : this()
        {
            this._Branch = branch;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtListCard.Text))
                {
                    string[] listCard = txtListCard.Text.Split(',');
                    CreateThreadGetHeinApproval(listCard);

                    if (listSelection != null && listSelection.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        bool success = false;
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        string path = "";
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            path = fbd.SelectedPath;
                        }
                        WaitingManager.Show();
                        success = this.GenerateXml(ref param, listSelection, path);
                        WaitingManager.Hide();
                        if (success && param.Messages.Count == 0)
                        {
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                        else
                        {
                            MessageManager.Show(param, success);
                        }
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    }
                }
                else
                {
                    MessageBox.Show("Bạn chưa nhập số thẻ");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadGetHeinApproval(string[] listCard)
        {
            Thread get1 = new Thread(ThreadGetHeinApproval);
            Thread get2 = new Thread(ThreadGetHeinApproval);
            Thread get3 = new Thread(ThreadGetHeinApproval);
            Thread get4 = new Thread(ThreadGetHeinApproval);
            Thread get5 = new Thread(ThreadGetHeinApproval);

            try
            {
                List<string> lst1 = new List<string>();
                List<string> lst2 = new List<string>();
                List<string> lst3 = new List<string>();
                List<string> lst4 = new List<string>();
                List<string> lst5 = new List<string>();

                var count = listCard.Count() / 5 + 1;
                var skip = 0;
                lst1 = listCard.Skip(skip).Take(count).ToList();
                skip = skip + count;
                lst2 = listCard.Skip(skip).Take(count).ToList();
                skip = skip + count;
                lst3 = listCard.Skip(skip).Take(count).ToList();
                skip = skip + count;
                lst4 = listCard.Skip(skip).Take(count).ToList();
                skip = skip + count;
                lst5 = listCard.Skip(skip).Take(count).ToList();
                skip = skip + count;

                get1.Start(lst1);
                get2.Start(lst2);
                get3.Start(lst3);
                get4.Start(lst4);
                get5.Start(lst5);

                get1.Join();
                get2.Join();
                get3.Join();
                get4.Join();
                get5.Join();
            }
            catch (Exception ex)
            {
                get1.Abort();
                get2.Abort();
                get3.Abort();
                get4.Abort();
                get5.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ThreadGetHeinApproval(object ListCard)
        {
            try
            {
                if (ListCard != null && ListCard.GetType() == typeof(List<string>))
                {
                    ProcessGetHeinApproval((List<string>)ListCard);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetHeinApproval(List<string> ListCard)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                foreach (var item in ListCard)
                {
                    HisTreatmentView1Filter filter = new HisTreatmentView1Filter();
                    filter.TDL_HEIN_CARD_NUMBER__EXACT = item;

                    if (dtFromExecuteTime.EditValue != null && dtFromExecuteTime.DateTime != DateTime.MinValue)
                    {
                        filter.FEE_LOCK_TIME_FROM = Convert.ToInt64(dtFromExecuteTime.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    if (dtToExecuteTime.EditValue != null && dtToExecuteTime.DateTime != DateTime.MinValue)
                    {
                        filter.FEE_LOCK_TIME_TO = Convert.ToInt64(dtToExecuteTime.DateTime.ToString("yyyyMMdd") + "235959");

                    }
                    var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (result != null)
                    {
                        listSelection.AddRange(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool GenerateXml(ref CommonParam paramExport, List<V_HIS_TREATMENT_1> listSelection, string pathSave)
        {
            bool result = false;
            try
            {
                string message = "";
                if (listSelection.Count > 0)
                {
                    if (String.IsNullOrEmpty(pathSave))
                    {
                        pathSave = ConfigStore.GetFolderSaveXml + "\\ExportXmlPlus\\Xml" + DateTime.Now.ToString("ddMMyyyy");
                        var dicInfo = System.IO.Directory.CreateDirectory(pathSave);
                        if (dicInfo == null)
                        {
                            paramExport.Messages.Add(Base.ResourceMessageLang.KhongTaoDuocFolderLuuXml);
                            return result;
                        }
                    }
                    if (!GlobalConfigStore.IsInit)
                        if (!this.SetDataToLocalXml(pathSave))
                        {
                            paramExport.Messages.Add(Base.ResourceMessageLang.KhongThieLapDuocCauHinhDuLieuXuatXml);
                            return result;
                        }
                    GlobalConfigStore.PathSaveXml = pathSave;

                    param = new CommonParam();

                    CreateThreadGetData(listSelection);

                    if (param.HasException) throw new DataMisalignedException("Co exception xay ra tai DAOGET trong qua trinh lay du lieu xuat xml");

                    message = ProcessExportXmlDetail(ref result, hisTreatments, listHeinApproval, ListSereServ, listDhst, hisSereServTeins, hisTrackings, hisSereServPttts, ListEkipUser, ListBedlog);

                    if (!String.IsNullOrEmpty(message))
                    {
                        paramExport.Messages.Add(String.Format(Base.ResourceMessageLang.CacMaDieuTriKhongXuatDuocXml, message));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        string ProcessExportXmlDetail(ref bool isSuccess, List<V_HIS_TREATMENT_3> hisTreatments, List<V_HIS_HEIN_APPROVAL> hisHeinApprvals, List<V_HIS_SERE_SERV_2> ListSereServ, List<HIS_DHST> listDhst, List<V_HIS_SERE_SERV_TEIN> listSereServTein, List<HIS_TRACKING> hisTrackings, List<V_HIS_SERE_SERV_PTTT> hisSereServPttts, List<HIS_EKIP_USER> ListEkipUser, List<V_HIS_BED_LOG> ListBedlog)
        {
            string result = "";
            this.FilePath = "";
            List<string> listResult = new List<string>();
            try
            {
                Dictionary<long, List<V_HIS_HEIN_APPROVAL>> dicHeinApproval = new Dictionary<long, List<V_HIS_HEIN_APPROVAL>>();
                Dictionary<long, List<V_HIS_SERE_SERV_2>> dicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV_2>>();
                Dictionary<long, List<V_HIS_SERE_SERV_TEIN>> dicSereServTein = new Dictionary<long, List<V_HIS_SERE_SERV_TEIN>>();
                Dictionary<long, HIS_DHST> dicDhst = new Dictionary<long, HIS_DHST>();
                Dictionary<long, List<HIS_TRACKING>> dicTracking = new Dictionary<long, List<HIS_TRACKING>>();
                Dictionary<long, List<V_HIS_SERE_SERV_PTTT>> dicSereServPttt = new Dictionary<long, List<V_HIS_SERE_SERV_PTTT>>();
                Dictionary<long, List<HIS_EKIP_USER>> dicEkipUser = new Dictionary<long, List<HIS_EKIP_USER>>();
                Dictionary<long, List<V_HIS_BED_LOG>> dicBedLog = new Dictionary<long, List<V_HIS_BED_LOG>>();

                if (hisHeinApprvals != null && hisHeinApprvals.Count > 0)
                {
                    foreach (var item in hisHeinApprvals)
                    {
                        if (!dicHeinApproval.ContainsKey(item.TREATMENT_ID))
                            dicHeinApproval[item.TREATMENT_ID] = new List<V_HIS_HEIN_APPROVAL>();
                        dicHeinApproval[item.TREATMENT_ID].Add(item);
                    }
                }

                if (ListSereServ != null && ListSereServ.Count > 0)
                {
                    foreach (var sereServ in ListSereServ)
                    {
                        if (sereServ.TDL_HEIN_SERVICE_TYPE_ID.HasValue && sereServ.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT && sereServ.AMOUNT > 0 && sereServ.PRICE > 0 && sereServ.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.TDL_TREATMENT_ID.HasValue)
                        {
                            if (!dicSereServ.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                dicSereServ[sereServ.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_2>();
                            dicSereServ[sereServ.TDL_TREATMENT_ID.Value].Add(sereServ);
                        }

                        if (sereServ.EKIP_ID.HasValue && ListEkipUser != null && ListEkipUser.Count > 0 && sereServ.TDL_TREATMENT_ID.HasValue)
                        {
                            var ekips = ListEkipUser.Where(o => o.EKIP_ID == sereServ.EKIP_ID).ToList();
                            if (ekips != null && ekips.Count > 0)
                            {
                                foreach (var item in ekips)
                                {
                                    if (!dicEkipUser.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                        dicEkipUser[sereServ.TDL_TREATMENT_ID.Value] = new List<HIS_EKIP_USER>();
                                    dicEkipUser[sereServ.TDL_TREATMENT_ID.Value].Add(item);
                                }
                            }
                        }
                    }
                }

                if (listSereServTein != null && listSereServTein.Count > 0)
                {
                    foreach (var ssTein in listSereServTein)
                    {
                        if (!ssTein.TDL_TREATMENT_ID.HasValue)
                            continue;
                        if (!dicSereServTein.ContainsKey(ssTein.TDL_TREATMENT_ID.Value))
                            dicSereServTein[ssTein.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_TEIN>();
                        dicSereServTein[ssTein.TDL_TREATMENT_ID.Value].Add(ssTein);
                    }
                }

                if (hisTrackings != null && hisTrackings.Count > 0)
                {
                    foreach (var tracking in hisTrackings)
                    {
                        if (!dicTracking.ContainsKey(tracking.TREATMENT_ID))
                            dicTracking[tracking.TREATMENT_ID] = new List<HIS_TRACKING>();
                        dicTracking[tracking.TREATMENT_ID].Add(tracking);
                    }
                }

                if (hisSereServPttts != null && hisSereServPttts.Count > 0)
                {
                    foreach (var ssPttt in hisSereServPttts)
                    {
                        if (!ssPttt.TDL_TREATMENT_ID.HasValue)
                            continue;
                        if (!dicSereServPttt.ContainsKey(ssPttt.TDL_TREATMENT_ID.Value))
                            dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_PTTT>();
                        dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value].Add(ssPttt);
                    }
                }

                if (listDhst != null && listDhst.Count > 0)
                {
                    listDhst = listDhst.OrderBy(o => o.EXECUTE_TIME).ToList();
                    foreach (var item in listDhst)
                    {
                        dicDhst[item.TREATMENT_ID] = item;
                    }
                }

                if (ListBedlog != null && ListBedlog.Count > 0)
                {
                    foreach (var bed in ListBedlog)
                    {
                        if (!dicBedLog.ContainsKey(bed.TREATMENT_ID))
                            dicBedLog[bed.TREATMENT_ID] = new List<V_HIS_BED_LOG>();
                        dicBedLog[bed.TREATMENT_ID].Add(bed);
                    }
                }

                foreach (var treatment in hisTreatments)
                {
                    InputADO ado = new InputADO();
                    ado.Treatment = treatment;
                    if (!dicHeinApproval.ContainsKey(treatment.ID))
                    {
                        listResult.Add(treatment.TREATMENT_CODE);
                        continue;
                    }
                    if (!dicSereServ.ContainsKey(treatment.ID))
                    {
                        listResult.Add(treatment.TREATMENT_CODE);
                        continue;
                    }

                    ado.HeinApprovals = dicHeinApproval[treatment.ID];
                    ado.HeinApproval = ado.HeinApprovals.FirstOrDefault();
                    ado.ListSereServ = dicSereServ[treatment.ID];
                    ado.Branch = GetByCashierRoomId(ado.HeinApproval.CASHIER_ROOM_ID);
                    if (dicDhst.ContainsKey(treatment.ID))
                    {
                        ado.Dhst = dicDhst[treatment.ID];
                    }
                    if (dicSereServTein.ContainsKey(treatment.ID))
                    {
                        ado.SereServTeins = dicSereServTein[treatment.ID];
                    }
                    if (dicTracking.ContainsKey(treatment.ID))
                    {
                        ado.Trackings = dicTracking[treatment.ID];
                    }
                    if (dicSereServPttt.ContainsKey(treatment.ID))
                    {
                        ado.SereServPttts = dicSereServPttt[treatment.ID];
                    }
                    if (dicBedLog.ContainsKey(treatment.ID))
                    {
                        ado.BedLogs = dicBedLog[treatment.ID];
                    }
                    if (dicEkipUser.ContainsKey(treatment.ID))
                    {
                        ado.EkipUsers = dicEkipUser[treatment.ID].Distinct().ToList();
                    }

                    ado.MaterialTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                    ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
                    ado.TotalSericeData = BackendDataWorker.Get<V_HIS_SERVICE>();
                    ado.ListHeinMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>();
                    ado.ConfigData = BackendDataWorker.Get<HIS_CONFIG>();
                    ado.ListHeinServiceType = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();
                    His.Bhyt.ExportXml.CreateXmlMain xmlMain = new His.Bhyt.ExportXml.CreateXmlMain(ado);
                    var fullFileName = xmlMain.Run4210Path();
                    if (String.IsNullOrWhiteSpace(fullFileName))
                    {
                        listResult.Add(treatment.TREATMENT_CODE);
                    }
                    else
                    {
                        this.FilePath += fullFileName + ";";
                        isSuccess = true;
                    }
                }
                if (listResult.Count > 0)
                    result = String.Join(",", listResult);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private HIS_BRANCH GetByCashierRoomId(long cashierRoomId)
        {
            try
            {
                V_HIS_CASHIER_ROOM cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>() != null ? BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == cashierRoomId) : null;
                if (cashierRoom != null)
                {
                    return (BackendDataWorker.Get<HIS_BRANCH>() != null ? BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == cashierRoom.BRANCH_ID) : null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private bool SetDataToLocalXml(string path)
        {
            bool result = false;
            try
            {
                if (this._Branch == null)
                {
                    return result;
                }

                GlobalConfigStore.Branch = this._Branch;

                GlobalConfigStore.ListIcdCode_Nds = HisConfigCFG.GetListValue(HisConfigCFG.MRS_HIS_REPORT_BHYT_NDS_ICD_CODE__OTHER);
                GlobalConfigStore.ListIcdCode_Nds_Te = HisConfigCFG.GetListValue(HisConfigCFG.MRS_HIS_REPORT_BHYT_NDS_ICD_CODE__TE);

                GlobalConfigStore.IsInit = true;
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void FormExportByCard_Load(object sender, EventArgs e)
        {
            try
            {
                dtFromExecuteTime.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                dtToExecuteTime.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #region Thread
        private void CreateThreadGetData(List<V_HIS_TREATMENT_1> listSelection)
        {
            System.Threading.Thread HeinApproval = new System.Threading.Thread(ThreadGetHeinApprovals);
            System.Threading.Thread SereServ2 = new System.Threading.Thread(ThreadGetSereServ2);
            System.Threading.Thread Treatment3 = new System.Threading.Thread(ThreadGetTreatment3);
            System.Threading.Thread Dhst_Tracking = new System.Threading.Thread(ThreadGetDhst_Tracking);
            System.Threading.Thread SereServTein_PTTT = new System.Threading.Thread(ThreadGetSereServTein_PTTT);
            try
            {
                HeinApproval.Start(listSelection);
                SereServ2.Start(listSelection);
                Treatment3.Start(listSelection);
                Dhst_Tracking.Start(listSelection);
                SereServTein_PTTT.Start(listSelection);

                HeinApproval.Join();
                SereServ2.Join();
                Treatment3.Join();
                Dhst_Tracking.Join();
                SereServTein_PTTT.Join();
            }
            catch (Exception ex)
            {
                HeinApproval.Abort();
                SereServ2.Abort();
                Treatment3.Abort();
                Dhst_Tracking.Abort();
                SereServTein_PTTT.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetSereServTein_PTTT(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                hisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                hisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    HisSereServTeinViewFilter ssTeinFilter = new HisSereServTeinViewFilter();
                    ssTeinFilter.TDL_TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resulTein = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, ssTeinFilter, param);
                    if (resulTein != null && resulTein.Count > 0)
                    {
                        hisSereServTeins.AddRange(resulTein);
                    }

                    HisSereServPtttViewFilter ssPtttFilter = new HisSereServPtttViewFilter();
                    ssPtttFilter.TDL_TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    ssPtttFilter.ORDER_DIRECTION = "DESC";
                    ssPtttFilter.ORDER_FIELD = "ID";
                    var resultPttt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, ssPtttFilter, param);
                    if (resultPttt != null && resultPttt.Count > 0)
                    {
                        hisSereServPttts.AddRange(resultPttt);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetDhst_Tracking(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                listDhst = new List<HIS_DHST>();
                hisTrackings = new List<HIS_TRACKING>();

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultDhst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);
                    if (resultDhst != null && resultDhst.Count > 0)
                    {
                        listDhst.AddRange(resultDhst);
                    }

                    HisTrackingFilter trackingFilter = new HisTrackingFilter();
                    trackingFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultTracking = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, trackingFilter, param);
                    if (resultTracking != null && resultTracking.Count > 0)
                    {
                        hisTrackings.AddRange(resultTracking);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetTreatment3(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                hisTreatments = new List<V_HIS_TREATMENT_3>();

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    HisTreatmentView3Filter treatmentFilter = new HisTreatmentView3Filter();
                    treatmentFilter.IDs = limit.Select(o => o.ID).ToList();
                    var resultTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_3>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_3, ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (resultTreatment != null && resultTreatment.Count > 0)
                    {
                        hisTreatments.AddRange(resultTreatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetSereServ2(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                ListSereServ = new List<V_HIS_SERE_SERV_2>();
                ListEkipUser = new List<HIS_EKIP_USER>();
                ListBedlog = new List<V_HIS_BED_LOG>();

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    HisSereServView2Filter ssFilter = new HisSereServView2Filter();
                    ssFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultSS = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW_2, ApiConsumers.MosConsumer, ssFilter, param);
                    if (resultSS != null && resultSS.Count > 0)
                    {
                        ListSereServ.AddRange(resultSS);

                        var ekipIds = resultSS.Select(o => o.EKIP_ID ?? 0).Distinct().ToList();
                        if (ekipIds != null && ekipIds.Count > 1)
                        {
                            HisEkipUserFilter ekipFilter = new HisEkipUserFilter();
                            ekipFilter.EKIP_IDs = ekipIds;
                            var resultEkip = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EKIP_USER>>("api/HisEkipUser/Get", ApiConsumers.MosConsumer, ekipFilter, param);
                            if (resultEkip != null && resultEkip.Count > 0)
                            {
                                ListEkipUser.AddRange(resultEkip);
                            }
                        }
                    }

                    HisBedLogViewFilter bedFilter = new HisBedLogViewFilter();
                    bedFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultBed = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedFilter, param);
                    if (resultBed != null && resultBed.Count > 0)
                    {
                        ListBedlog.AddRange(resultBed);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetHeinApprovals(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                listHeinApproval = new List<V_HIS_HEIN_APPROVAL>();

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    HisHeinApprovalViewFilter approvalFilter = new HisHeinApprovalViewFilter();
                    approvalFilter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    approvalFilter.ORDER_DIRECTION = "DESC";
                    approvalFilter.ORDER_FIELD = "EXECUTE_TIME";
                    var resultHeinApproval = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_HEIN_APPROVAL>>("api/HisHeinApproval/GetView", ApiConsumers.MosConsumer, approvalFilter, param);
                    if (resultHeinApproval != null && resultHeinApproval.Count > 0)
                    {
                        listHeinApproval.AddRange(resultHeinApproval);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
