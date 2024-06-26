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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServicePackageView
{
    public partial class frmServicePackageView : Form
    {
        public void loadServicePackages()
        {
            try
            {
                WaitingManager.Show();
                GetAllSereServ();
                //Dịch vụ kỹ thuật cao
                sereServHightechs = sereServs.Where(o => o.HEIN_SERVICE_TYPE_ID == HIS.Desktop.LocalStorage.SdaConfigKey.Config.HisHeinServiceTypeCFG.HisHeinServiceTypeId__HighTech).ToList();

                CreateTreeListNode();
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        private void CreateTreeListNode()
        {
            
            try
            {
                var servicePatyPrpos = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_PATY_PRPO>();

                treeListSereServDetail.BeginUnboundLoad();
                treeListSereServDetail.Nodes.Clear();
                TreeListNode parentForRootNodes = null;
                foreach (var sereServHightech in sereServHightechs)
                {

                    if (sereServHightech.PRICE_POLICY_ID.HasValue)
                    {
                        var servicePatyPrpo = servicePatyPrpos.Where(o => o.SERVICE_ID == sereServHightech.SERVICE_ID && o.PATIENT_TYPE_ID == sereServHightech.PATIENT_TYPE_ID && o.PRICE_POLICY_ID == sereServHightech.PRICE_POLICY_ID).ToList();
                        if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                        {
                            sereServHightech.PRICE = servicePatyPrpo.FirstOrDefault().PRICE;
                            sereServHightech.VIR_TOTAL_PRICE = sereServHightech.PRICE * sereServHightech.AMOUNT;
                        }
                    }
                    TreeListNode rootNode = treeListSereServDetail.AppendNode(
                     new object[] { sereServHightech.SERVICE_NAME, sereServHightech.AMOUNT, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sereServHightech.PRICE), Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sereServHightech.VIR_TOTAL_PRICE ?? 0), sereServHightech.PATIENT_TYPE_NAME, null, sereServHightech.EXECUTE_ROOM_NAME, sereServHightech.REQUEST_ROOM_NAME, sereServHightech.REQUEST_DEPARTMENT_NAME, null },
                     parentForRootNodes, null);
                    rootNode.Tag = "KTC";
                    var requestDepartments = sereServs.Where(o => o.PARENT_ID == sereServHightech.ID).OrderBy(o=>o.REQUEST_DEPARTMENT_ID).GroupBy(o=>o.REQUEST_DEPARTMENT_ID);
                    foreach (var requestDepartment in requestDepartments)
                    {
                        TreeListNode childNodeDepartment = treeListSereServDetail.AppendNode(
                        new object[] { requestDepartment.First().REQUEST_DEPARTMENT_NAME, null, null, null, null, null, null, null, null ,null},
                                                rootNode, null);
                        childNodeDepartment.Tag = "Department";

                          //Nhóm dịch vụ
                        var executeGroups = sereServs.Where(o => o.REQUEST_DEPARTMENT_ID == requestDepartment.First().REQUEST_DEPARTMENT_ID && o.PARENT_ID == sereServHightech.ID).OrderByDescending(o=>o.EXECUTE_GROUP_ID).GroupBy(o => o.EXECUTE_GROUP_ID);
                        foreach (var executeGroup in executeGroups)
                        {
                            string executeGroupName;
                            var sereServChildNodes = executeGroup.ToList();
                            if (sereServChildNodes.First().EXECUTE_GROUP_ID == null)
                            {
                                executeGroupName = "Khác";
                                TreeListNode childNodeExecuteGroup = treeListSereServDetail.AppendNode(
                        new object[] { executeGroupName, null, null, null, null, null, null, null, null,null },
                                                childNodeDepartment, null);
                                childNodeExecuteGroup.Tag = "ExecuteGroup";
                                foreach (var sereServChildNode in sereServChildNodes)
                                {
                                    CreateNodeChildSereServ(sereServChildNode, childNodeExecuteGroup);
                                }
                            }
                            else
                            {
                                executeGroupName = BackendDataWorker.Get<HIS_EXECUTE_GROUP>().SingleOrDefault(o => o.ID == executeGroup.First().EXECUTE_GROUP_ID).EXECUTE_GROUP_NAME;

                                TreeListNode childNodeExecuteGroup = treeListSereServDetail.AppendNode(
                        new object[] { executeGroupName, null, null, null, null, null, null, null, null,null },
                                                childNodeDepartment, null);
                                childNodeExecuteGroup.Tag = "ExecuteGroup";
                                var sereServInServicePackageExecuteGroups = sereServs.Where(o => o.PARENT_ID == sereServHightech.ID && o.REQUEST_DEPARTMENT_ID == requestDepartment.First().REQUEST_DEPARTMENT_ID && o.EXECUTE_GROUP_ID == executeGroup.First().EXECUTE_GROUP_ID).ToList();

                                if (sereServInServicePackageExecuteGroups.Count == 0 || sereServInServicePackageExecuteGroups == null)
                                    treeListSereServDetail.Nodes.Remove(childNodeExecuteGroup);

                                foreach (var sereServInServicePackageExecuteGroup in sereServInServicePackageExecuteGroups)
                                {
                                    CreateNodeChildSereServ(sereServInServicePackageExecuteGroup, childNodeExecuteGroup);
                                }
                            }
                        }
                    }
                }
                treeListSereServDetail.EndUnboundLoad();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateNodeChildSereServ(V_HIS_SERE_SERV sereServ,TreeListNode rootNode)
        {
            try
            {
                //RepositoryItemComboBox riCombo = new RepositoryItemComboBox();

                TreeListNode childNodeSereServ = treeListSereServDetail.AppendNode(
                    new object[] { sereServ.SERVICE_NAME, sereServ.AMOUNT, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sereServ.PRICE), Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sereServ.VIR_TOTAL_PRICE ?? 0), sereServ.PATIENT_TYPE_NAME, sereServ.IS_OUT_PARENT_FEE == 1 ? true : false, sereServ.EXECUTE_ROOM_NAME, sereServ.REQUEST_ROOM_NAME, sereServ.REQUEST_DEPARTMENT_NAME, sereServ },
                                                       rootNode, sereServ);
                childNodeSereServ.Tag = "SERESERV";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetAllSereServ()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServViewFilter hisSerwServFilter = new HisSereServViewFilter();
                hisSerwServFilter.TREATMENT_ID = treatmentId;
                hisSerwServFilter.ORDER_FIELD = "INTRUCTION_TIME";
                hisSerwServFilter.ORDER_DIRECTION = "DESC";
                //long departmentId = EXE.LOGIC.Token.TokenManager.GetDepartmentId();
                long departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == Module.RoomId).DepartmentId;
                sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView",ApiConsumers.MosConsumer,hisSerwServFilter,param).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
