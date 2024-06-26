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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisServicePatyList.entity;
using HIS.Desktop.Plugins.HisServicePatyList.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisServicePatyList
{
    public partial class frmHisServicePatyList : HIS.Desktop.Utility.FormBase
    {
        V_HIS_SERVICE service { get; set; }
        HIS_PATIENT_TYPE patientType { get; set; }
        HIS_BRANCH branchCode { get; set; }
        List<V_HIS_ROOM> excuteRoom { get; set; }
        List<HIS_DEPARTMENT> deparmentCodes { get; set; }
        List<V_HIS_ROOM> requestRooms { get; set; }
        List<object> listArgs = new List<object>();


        private void bbtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnImport_Click(null,null);
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)RefreshData);
                if (this.moduleData != null)
                {
                    CallModule callModule = new CallModule(CallModule.HisImportServicePaty, moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisImportServicePaty, 0, 0, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addServicePatyToProcessList(List<ADO.ServicePatyAdo> servicePatyImports)
        {
            try
            {
                //var serviceParty = new V_HIS_SERVICE_PATY();
                if (servicePatyImports == null || servicePatyImports.Count == 0)
                    return;
                foreach (var item in servicePatyImports)
                {
                    V_HIS_SERVICE_PATY serviceParty = new V_HIS_SERVICE_PATY();
                    var ado = new ADO.ServicePatyAdo();

                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.ServicePatyAdo>(ado, item);
                    if (ado.SERVICE_CODE != null)
                    {
                        service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (service != null)
                        {
                            serviceParty.SERVICE_ID = service.ID;
                            serviceParty.SERVICE_CODE = service.SERVICE_CODE;
                            serviceParty.SERVICE_NAME = service.SERVICE_NAME;
                            serviceParty.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            serviceParty.SERVICE_CODE = ado.SERVICE_CODE;
                        }
                    }
                    if (ado.PATIENT_TYPE_CODE != null)
                    {
                        patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE);
                        if (patientType != null)
                        {
                            serviceParty.PATIENT_TYPE_ID = patientType.ID;
                            serviceParty.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                            serviceParty.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }
                        else
                        {
                            serviceParty.PATIENT_TYPE_CODE = ado.PATIENT_TYPE_CODE;
                        }
                    }
                    if (ado.BRANCH_CODE != null)
                    {
                        branchCode = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.BRANCH_CODE == item.BRANCH_CODE);
                        if (branchCode != null)
                        {
                            serviceParty.BRANCH_ID = branchCode.ID;
                            serviceParty.BRANCH_CODE = branchCode.BRANCH_CODE;
                            serviceParty.BRANCH_NAME = branchCode.BRANCH_NAME;
                        }
                        else
                        {
                            serviceParty.BRANCH_CODE = ado.BRANCH_CODE;
                        }
                    }
                    if (ado.EXECUTE_ROOM_CODES != null)
                    {
                        List<string> exeRoomCode = new List<string>();
                        string[] split = ado.EXECUTE_ROOM_CODES.Split(',');
                        if (split.Count() > 0)
                        {
                            for (int i = 0; i < split.Count(); i++)
                            {
                                exeRoomCode.Add(split[i]);
                            }
                        }

                        excuteRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => exeRoomCode.Contains(p.ROOM_CODE)).ToList();

                        if (excuteRoom != null && excuteRoom.Count > 0)
                        {
                            foreach (var itemExcute in excuteRoom)
                            {
                                serviceParty.EXECUTE_ROOM_IDS += itemExcute.ID.ToString() + ",";
                            }
                        }
                        else
                        {
                            serviceParty.EXECUTE_ROOM_IDS += ado.EXECUTE_ROOM_CODES.ToString() + ",";
                        }
                    }
                    if (ado.REQUEST_DEPARMENT_CODES != null)
                    {
                        List<string> reqDepartmentCode = new List<string>();
                        string[] split = ado.REQUEST_DEPARMENT_CODES.Split(',');
                        if (split.Count() > 0)
                        {
                            for (int i = 0; i < split.Count(); i++)
                            {
                                reqDepartmentCode.Add(split[i]);
                            }
                        }

                        deparmentCodes = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(
                            p => reqDepartmentCode.Contains(p.DEPARTMENT_CODE)).ToList();

                        if (deparmentCodes != null && deparmentCodes.Count > 0)
                        {
                            foreach (var itemDeparment in deparmentCodes)
                            {
                                serviceParty.REQUEST_DEPARMENT_IDS += itemDeparment.ID.ToString() + ",";
                            }
                        }
                        else
                        {
                            serviceParty.REQUEST_DEPARMENT_IDS += ado.REQUEST_DEPARMENT_CODES.ToString() + ",";
                        }
                    }
                    if (ado.REQUEST_ROOM_CODES != null)
                    {
                        List<string> reqRoomCode = new List<string>();
                        string[] split = ado.REQUEST_ROOM_CODES.Split(',');
                        if (split.Count() > 0)
                        {
                            for (int i = 0; i < split.Count(); i++)
                            {
                                reqRoomCode.Add(split[i]);
                            }
                        }

                        requestRooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(
                            p => reqRoomCode.Contains(p.ROOM_CODE)).ToList();
                        if (requestRooms != null && requestRooms.Count > 0)
                        {
                            foreach (var itemRequestRooms in requestRooms)
                            {
                                serviceParty.REQUEST_ROOM_IDS += itemRequestRooms.ID.ToString() + ",";
                            }
                        }
                        else
                        {
                            serviceParty.REQUEST_ROOM_IDS += ado.REQUEST_ROOM_CODES.ToString() + ",";
                        }
                    }

                    serviceParty.PRICE = ado.PRICE;
                    serviceParty.OVERTIME_PRICE = ado.OVERTIME_PRICE;
                    serviceParty.PRIORITY = ado.PRIORITY;
                    serviceParty.VAT_RATIO = ado.VAT_RATIO;
                    serviceParty.INTRUCTION_NUMBER_FROM = ado.INTRUCTION_NUMBER_FROM;
                    serviceParty.INTRUCTION_NUMBER_TO = ado.INTRUCTION_NUMBER_TO;
                    serviceParty.FROM_TIME = ado.FROM_TIME;
                    serviceParty.TO_TIME = ado.TO_TIME;
                    serviceParty.TREATMENT_FROM_TIME = ado.TREATMENT_FROM_TIME;
                    serviceParty.TREATMENT_TO_TIME = ado.TREATMENT_TO_TIME;
                    serviceParty.DAY_FROM = ado.DAY_FROM;
                    serviceParty.DAY_TO = ado.DAY_TO;
                    serviceParty.HOUR_FROM = ado.HOUR_FROM;
                    serviceParty.HOUR_TO = ado.HOUR_TO;
                    serviceParty.IS_ACTIVE = 1;
                    this.ListServicePatyprocess.Add(serviceParty);
                    listArgs.Add(serviceParty.ID);
                    listArgs.Add(this.ListServicePatyprocess);
                }
                if (requestRooms == null ||
                        service == null ||
                        patientType == null ||
                        branchCode == null ||
                        excuteRoom == null ||
                        deparmentCodes == null)
                {
                    gridControlServicePaty.DataSource = null;
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServicePatyListImport").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServicePatyListImport'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        //listArgs.AddRange(this.ListServicePatyprocess);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();

                    }
                    this.ListServicePatyprocess = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
