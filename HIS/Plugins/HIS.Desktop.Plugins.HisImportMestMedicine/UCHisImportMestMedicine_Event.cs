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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.HisImportMestMedicine.Base;

namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
    public partial class UCHisImportMestMedicine : UserControlBase
    {
        private void repositoryItemButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ViewImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                //hien thi popup chi tiet
                WaitingManager.Show();

                if (ViewImportMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH)
                {
                    ImpMestViewDetailADO impMestView = new ImpMestViewDetailADO(ViewImportMest.ID, ViewImportMest.IMP_MEST_TYPE_ID, ViewImportMest.IMP_MEST_STT_ID);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(impMestView);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.ImpMestViewDetail, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                else
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ViewImportMest.ID);
                    CallModule callModule = new CallModule(CallModule.ApproveAggrImpMest, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterSave(object data)
        {
            try
            {
                if (data != null)
                {
                    FillDataImportMestList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ViewImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                WaitingManager.Show();
                V_HIS_IMP_MEST_1 impMest1View = null;
                CommonParam param = new CommonParam();
                HisImpMestView1Filter ipmMestView1Filter = new HisImpMestView1Filter();
                ipmMestView1Filter.ID = ViewImportMest.ID;
                var listImpMestView1 = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_1>>("api/HisImpMest/GetView1", ApiConsumer.ApiConsumers.MosConsumer, ipmMestView1Filter, param);
                if (listImpMestView1 != null && listImpMestView1.Count > 0)
                {
                    impMest1View = listImpMestView1.FirstOrDefault();
                }

                if (impMest1View != null)
                {
                    if (ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC || ViewImportMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HM)

                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ViewImportMest.ID);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)FillDataImportMestList);
                        if (impMest1View.IS_BLOOD != 1)
                        {
                            CallModule callModule = new CallModule(CallModule.ManuImpMestUpdate, this.roomId, this.roomTypeId, listArgs);
                        }
                        else
                        {
                            CallModule callModule = new CallModule(CallModule.BloodImpMestUpdate, this.roomId, this.roomTypeId, listArgs);
                        }

                        WaitingManager.Hide();
                    }
                    else
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDiscardEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_IMP_MEST>(data, row);

                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            ("api/HisImpMest/Delete", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult)
                        {
                            success = true;
                            FillDataImportMestList();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                WaitingManager.Hide();
            }
        }

        private void repositoryItemButtonApprovalEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (EVImportMest, VImportMest);

                EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataImportMestList();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDisApprovalEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (EVImportMest, VImportMest);
                EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataImportMestList();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void repositoryItemButtonActualImportEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                if (VImportMest != null && VImportMest.HAS_IDENTITY_MATERIAL == 1 && HisConfigCFG.IDENTITY_MATERIAL_OPTION == "2")
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.IdentityMaterialInformation").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.IdentityMaterialInformation'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        moduleData.RoomId = this.currentModule.RoomId;
                        moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                        List<object> listArgs = new List<object>();
                        listArgs.Add(true);
                        listArgs.Add(VImportMest.ID);
                        listArgs.Add((HIS.Desktop.Common.DelegateImpTime)ProcessImpMest);
                        listArgs.Add(moduleData);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
                else
                {
                    frmMessage frm = new frmMessage(CheckSayYes, (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow(), (RefeshReference)FillDataImportMestList);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        private void ProcessImpMest(long? impTime)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (VImportMest != null)
                {
                    WaitingManager.Show();
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map
                        <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        (data, VImportMest);
                    data.IMP_TIME = impTime;
                    data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        ("api/HisImpMest/Import", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresult != null)
                    {
                        success = true;
                        FillDataImportMestList();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                CommonParam param = new CommonParam();
            }
        }
        private void CheckSayYes(bool IsYes)
        {
            try
            {
                if (IsYes)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map
                            <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                            (data, row);
                        data.IMP_TIME = TimeImpFromMessage;
                        data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                            ("api/HisImpMest/Import", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataImportMestList();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                CommonParam param = new CommonParam();
            }
        }

        private void repositoryItemButtonRequest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy duyệt không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)gridViewImportMestList.GetFocusedRow();
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map
                        <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        (EVImportMest, VImportMest);

                    EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresul != null)
                    {
                        success = true;
                        FillDataImportMestList();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
