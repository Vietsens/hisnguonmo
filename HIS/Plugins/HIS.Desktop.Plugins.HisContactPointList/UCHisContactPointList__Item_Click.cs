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
using DevExpress.XtraEditors.Controls;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraBars.Docking2010.Views;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ADO;
using HIS.Desktop.Utility;
using HIS.Desktop.Common;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using MOS.SDO;

namespace HIS.Desktop.Plugins.HisContactPointList
{
    public partial class UCHisContactPointList : UserControlBase
    {
        /// <summary>
        /// Danh sach yeu cau
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnServiceReqList_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_CONTACT_POINT)gridViewHisContactPointList.GetFocusedRow();
                repositoryItembtnServiceReqList_Click(row);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItembtnServiceReqList_Click(V_HIS_CONTACT_POINT data)
        {
            try
            {
                var row = data;
                if (row != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    listArgs.Add(data);
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ContactDeclaration", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ket thuc dieu tri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_CONTACT_POINT)gridViewHisContactPointList.GetFocusedRow();
                repositoryItembtnFinish_Click(row);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnFinish_Click(MOS.EFMODEL.DataModels.V_HIS_CONTACT_POINT data)
        {
            try
            {
                var row = data;
                if (row != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có chắc muốn cập nhật người bệnh là F0 không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        bool success = false;

                        if (row.CONTACT_LEVEL == 0)
                        {
                            MessageManager.Show("Người bệnh đã là F0");
                            return;
                        }
                        WaitingManager.Show();
                        HisContactLevelSDO contactLevelSDO = new HisContactLevelSDO();
                        contactLevelSDO.ContactLevel = 0;
                        contactLevelSDO.ContactPointId = data.ID;
                        HIS_CONTACT_POINT rsUpdate = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_CONTACT_POINT>("/api/HisContactPoint/SetContactLevel", ApiConsumers.MosConsumer, contactLevelSDO, param);
                        WaitingManager.Hide();
                        if (rsUpdate != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }

                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dong thoi gian
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnTimeLine_btnClick(object sender, ButtonPressedEventArgs e)
        {

        }

        /// <summary>
        /// Sua thong tin benh nhan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnEditTreatment_btnClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)gridViewHisContactPointList.GetFocusedRow();

                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                treatmentFilter.ID = row.ID;
                var currentTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentFilter, param);
                if (currentTreatment != null && currentTreatment.Count > 0)
                {
                    //List<object> listArgs = new List<object>();
                    //listArgs.Add(currentTreatment.FirstOrDefault());
                    //listArgs.Add((DelegateSelectData)Search);
                    //HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.PatientUpdate", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PatientUpdate").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.PatientUpdate'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(currentTreatment.FirstOrDefault());
                        listArgs.Add((DelegateSelectData)Search);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("extenceInstance is null");
                        }
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Search(object data)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        if (btnFind.Enabled)
                        {
                            btnFind_Click(null, null);
                        }
                        HIS.Desktop.Plugins.Library.PrintPatientUpdate.PrintPatientUpdateProcessor rm = new Library.PrintPatientUpdate.PrintPatientUpdateProcessor(data, this.currentModule.RoomId);
                        rm.Print();
                    }));
                }
                else
                {
                    if (btnFind.Enabled)
                    {
                        btnFind_Click(null, null);
                    }
                    HIS.Desktop.Plugins.Library.PrintPatientUpdate.PrintPatientUpdateProcessor rm = new Library.PrintPatientUpdate.PrintPatientUpdateProcessor(data, this.currentModule.RoomId);
                    rm.Print();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }        
    }
}
