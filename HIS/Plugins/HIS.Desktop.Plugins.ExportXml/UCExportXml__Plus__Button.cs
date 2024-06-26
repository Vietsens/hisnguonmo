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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExportXml.Config;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXml
{
    public partial class UCExportXml
    {

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled)
                    return;
                WaitingManager.Show();
                FillDataToGridHeinApproval();
                if (listHeinAprroval != null && listHeinAprroval.Count == 1)
                {
                    this.currentHeinApproval = listHeinAprroval.First();
                    FillDateToTreeSereServByHeinApproval();
                }
                gridControlHeinApprovalBhyt.Focus();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportXml.Enabled || this.listSelection == null || this.listSelection.Count == 0)
                    return;
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
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                //isInitXmlLocalData = true;
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool GenerateXml(ref CommonParam paramExport, List<V_HIS_HEIN_APPROVAL> listApproval, string pathSave)
        {
            bool result = false;
            try
            {
                string message = "";
                if (listApproval != null && listApproval.Count > 0)
                {
                    if (String.IsNullOrEmpty(pathSave))
                    {
                        pathSave = Base.ConfigStore.GetFolderSaveXml + "\\ExportXmlPlus\\Xml" + DateTime.Now.ToString("ddMMyyyy");
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
                    int start = 0;
                    int count = listApproval.Count;
                    CommonParam param = new CommonParam();
                    while (count > 0)
                    {
                        int limit = (count <= GlobalVariables.MAX_REQUEST_LENGTH_PARAM) ? count : GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                        var listSub = listApproval.Skip(start).Take(limit).ToList();

                        HisSereServView2Filter ssFilter = new HisSereServView2Filter();
                        ssFilter.HEIN_APPROVAL_IDs = listSub.Select(s => s.ID).ToList();
                        List<V_HIS_SERE_SERV_2> ListSereServ = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW_2, ApiConsumers.MosConsumer, ssFilter, param);

                        HisTreatmentView3Filter treatmentFilter = new HisTreatmentView3Filter();
                        treatmentFilter.IDs = listSub.Select(s => s.TREATMENT_ID).ToList();
                        List<V_HIS_TREATMENT_3> hisTreatments = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_3>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_3, ApiConsumers.MosConsumer, treatmentFilter, param);

                        HisDhstFilter dhstFilter = new HisDhstFilter();
                        dhstFilter.TREATMENT_IDs = treatmentFilter.IDs;
                        List<HIS_DHST> listDhst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);

                        if (param.HasException)
                        {
                            throw new DataMisalignedException("Co exception xay ra tai DAOGET trong qua trinh lay du lieu xuat xml");
                        }
                        message += ProcessExportXmlDetail(ref result, hisTreatments, listSub, ListSereServ, listDhst);
                        start += GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                        count -= GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                    }
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

        private string ProcessExportXmlDetail(ref bool isSuccess, List<V_HIS_TREATMENT_3> hisTreatments, List<V_HIS_HEIN_APPROVAL> ListHeinApproval, List<V_HIS_SERE_SERV_2> ListSereServ, List<HIS_DHST> listDhst)
        {
            string result = "";
            List<string> listResult = new List<string>();
            try
            {
                Dictionary<long, V_HIS_TREATMENT_3> dicTreatment = new Dictionary<long, V_HIS_TREATMENT_3>();
                Dictionary<long, List<V_HIS_SERE_SERV_2>> dicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV_2>>();
                Dictionary<long, HIS_DHST> dicDhst = new Dictionary<long, HIS_DHST>();

                if (hisTreatments != null && hisTreatments.Count > 0)
                {
                    foreach (var treat in hisTreatments)
                    {
                        dicTreatment[treat.ID] = treat;
                    }
                }

                if (ListSereServ != null || ListSereServ.Count > 0)
                {
                    foreach (var sereServ in ListSereServ)
                    {
                        if (sereServ.TDL_HEIN_SERVICE_TYPE_ID.HasValue && sereServ.HEIN_APPROVAL_ID.HasValue && sereServ.AMOUNT > 0 && sereServ.PRICE > 0 && sereServ.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            if (!dicSereServ.ContainsKey(sereServ.HEIN_APPROVAL_ID.Value))
                                dicSereServ[sereServ.HEIN_APPROVAL_ID.Value] = new List<V_HIS_SERE_SERV_2>();
                            dicSereServ[sereServ.HEIN_APPROVAL_ID.Value].Add(sereServ);
                        }
                    }
                }

                if (listDhst != null && listDhst.Count > 0)
                {
                    //sap xep thoi gian tang dan de trong th co nhieu dhst se lay cai co thoi gian thuc hien lon nhat
                    listDhst = listDhst.OrderBy(o => o.EXECUTE_TIME).ToList();
                    foreach (var item in listDhst)
                    {
                        dicDhst[item.TREATMENT_ID] = item;
                    }
                }

                foreach (var heinApproval in ListHeinApproval)
                {
                    InputADO ado = new InputADO();
                    ado.HeinApproval = heinApproval;
                    if (!dicTreatment.ContainsKey(heinApproval.TREATMENT_ID))
                    {
                        Inventec.Common.Logging.LogSystem.Info("DicTreatment khong chu key HeinApproval.TreatmentId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heinApproval), heinApproval));
                        listResult.Add(heinApproval.TREATMENT_CODE);
                        continue;
                    }
                    if (!dicSereServ.ContainsKey(heinApproval.ID))
                    {
                        Inventec.Common.Logging.LogSystem.Info("heinApproval khong chua SereServ nao: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heinApproval), heinApproval));
                        listResult.Add(heinApproval.TREATMENT_CODE);
                        continue;
                    }

                    ado.Treatment = dicTreatment[heinApproval.TREATMENT_ID];
                    ado.ListSereServ = dicSereServ[heinApproval.ID];

                    if (dicDhst.ContainsKey(heinApproval.TREATMENT_ID))
                    {
                        ado.Dhst = dicDhst[heinApproval.TREATMENT_ID];
                    }

                    His.Bhyt.ExportXml.CreateXmlMain xmlMain = new His.Bhyt.ExportXml.CreateXmlMain(ado);
                    var success = xmlMain.Run917();
                    if (!success)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong tao duoc XML cho Ho so duyet bhyt TreatmentCode: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heinApproval.TREATMENT_CODE), heinApproval.TREATMENT_CODE));
                        listResult.Add(heinApproval.TREATMENT_CODE);
                    }
                    else
                    {
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
    }
}
