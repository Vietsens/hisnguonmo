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

namespace HIS.Desktop.Plugins.ListPresBloodConfirm
{
    public partial class UCListPresBloodConfirm : UserControl
    {
        private void repositoryItemButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                V_HIS_EXP_MEST ExpMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(ExpMestData, rowDataExpMest);

                if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL)
                {
                    HIS.Desktop.ADO.ApproveAggrExpMestSDO exeMestView = new HIS.Desktop.ADO.ApproveAggrExpMestSDO(ExpMestData.ID, ExpMestData.EXP_MEST_STT_ID);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(exeMestView);
                    CallModule callModule = new CallModule(CallModule.ApproveAggrExpMest, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.AggrExpMestDetail, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    new CallModule("HIS.Desktop.Plugins.ExpMestDetailBCS", this.roomId, this.roomTypeId, listArgs);
                    WaitingManager.Hide();
                }
                else
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ExpMestData);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.ExpMestViewDetail, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void FillDataApterSave(object data)
        {
            try
            {
                if (data != null)
                {
                    FillDataExpMestList();
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
                ViewImportMest = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
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

                if (impMest1View != null )
                {
                    if (ViewImportMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || ViewImportMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK || ViewImportMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK || ViewImportMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)

                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ViewImportMest.ID);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)FillDataExpMestList);
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
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, row);

                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            ("api/HisImpMest/Delete", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult)
                        {
                            success = true;
                            FillDataExpMestList();
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
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 VImportMest = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    (EVImportMest, VImportMest);

                EVImportMest.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataExpMestList();
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
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 VImportMest = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    (EVImportMest, VImportMest);
                EVImportMest.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataExpMestList();
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
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonThucNhapDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map
                            <MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            (data, row);
                        data.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            ("api/HisImpMest/Import", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataExpMestList();
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
                WaitingManager.Hide();
            }
        }

        private void repositoryItemButtonRequest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 VImportMest = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    (EVImportMest, VImportMest);

                EVImportMest.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataExpMestList();
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
