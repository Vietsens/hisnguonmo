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
using HIS.Desktop.LibraryMessage;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.HisExportMestMedicine.Base;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;

namespace HIS.Desktop.Plugins.HisExportMestMedicine
{
    public partial class UCHisExportMestMedicine : UserControlBase
    {
        bool isExpWithExpTime = false;
        long? ExpTime = null;
        private void repositoryItemButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
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

        private void ButtonEnableEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST expMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMestData, rowDataExpMest);
                if (expMestData != null)
                {
                    if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData.ID);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                        CallModule callModule = new CallModule(CallModule.ExpMestOtherExport, this.roomId, this.roomTypeId, listArgs);


                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData.ID);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                        if (HisConfigCFG.EXP_MEST_SALE__MODULE_UPDATE_OPTION_SELECT == "1")
                        {
                            CallModule callModule = new CallModule(CallModule.ExpMestSaleCreateV2, this.roomId, this.roomTypeId, listArgs);
                        }
                        else
                        {
                            CallModule callModule = new CallModule(CallModule.ExpMestSaleCreate, this.roomId, this.roomTypeId, listArgs);
                        }
                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData);
                        CallModule callModule = new CallModule(CallModule.ExpMestChmsUpdate, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData);
                        CallModule callModule = new CallModule(CallModule.ExpMestDepaUpdate, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(expMestData);
                        CallModule callModule = new CallModule(CallModule.ManuExpMestCreate, this.roomId, this.roomTypeId, listArgs);

                        RefreshData();
                    }
                    else
                        MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                }
                else
                    MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEnableDiscard_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                    V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                    if (row != null)
                    {
                        WaitingManager.Show();
                        if (row.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            HisExpMestSDO sdo = new HisExpMestSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.ReqRoomId = this.roomId;
                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                            {
                                if (row.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES_DETAIL)
                                {
                                    var apiresul = new BackendAdapter(param).Post<bool>("/api/HisExpMest/BaseCompensationDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        RefreshData();
                                    }
                                }
                                else if (row.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__BASE)
                                {
                                    var apiresul = new BackendAdapter(param).Post<bool>("/api/HisExpMest/CompensationByBaseDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        RefreshData();
                                    }
                                }
                                else
                                {
                                    var apiresul = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        RefreshData();
                                    }
                                }
                            }
                            else
                            {
                                var apiresul = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                if (apiresul)
                                {
                                    success = true;
                                    RefreshData();
                                }
                            }
                        }
                        else
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>("api/HisExpMest/AggrExamDelete", ApiConsumer.ApiConsumers.MosConsumer, row.ID, param);
                            if (apiresul)
                            {
                                success = true;
                                RefreshData();
                            }
                        }

                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEnableApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                bool success = false;
                bool success_ = true;
                if (row != null)
                {
                    if (row.EXP_MEST_TYPE_CODE == "01")
                    {  // Đơn phòng khám
                        DateTime? t1 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.CREATE_TIME ?? 0);
                        DateTime? t2 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.MODIFY_TIME ?? 0);
                        string TB;

                        if (t1 != null && t2 != null)
                        {
                            TimeSpan? diff = t2 - t1;
                            TimeSpan interval = new TimeSpan(0, 0, 60);
                            if (diff > interval)
                            {
                                if (row.EXP_MEST_TYPE_CODE == "01")
                                {

                                    if (HisConfigCFG.WARM_MODIFIEDPRESCRIPTIONOPTION == "1")
                                    {
                                        TB = "Phiếu xuất " + row.EXP_MEST_CODE + " đã có sự chỉnh sửa. Bạn có chắc muốn duyệt không?";
                                        if (DevExpress.XtraEditors.XtraMessageBox.Show(TB, "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            success_ = true;
                                        }
                                        else
                                        {
                                            success_ = false;
                                        }
                                    }
                                    else
                                    {
                                        success_ = true;
                                    }
                                }

                            }
                        }
                    }
                    if (success_)
                    {
                        if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP
                             || row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM
                             || (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK && rowDataExpMest.IS_REQUEST_BY_PACKAGE != 1)
                        )
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(row.ID);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.BrowseExportTicket, this.roomId, this.roomTypeId, listArgs);
                            WaitingManager.Hide();

                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(row.ID);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.ApprovalExpMestBcs, this.roomId, this.roomTypeId, listArgs);
                            WaitingManager.Hide();
                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            WaitingManager.Show();
                            //bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();
                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                           "api/HisExpMest/InPresApprove", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                if (rs != null && rs.ID != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == rs.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_APPROVAL_LOGINNAME = rs.LAST_APPROVAL_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            WaitingManager.Show();
                            //bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestSDO = new MOS.SDO.HisExpMestSDO();
                            hisExpMestSDO.ExpMestId = row.ID;
                            hisExpMestSDO.ReqRoomId = this.roomId;

                            var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_EXP_MEST>>("api/HisExpMest/AggrExamApprove", ApiConsumers.MosConsumer, hisExpMestSDO, param);
                            if (rs != null && rs.Count > 0)
                            {
                                success = true;
                                RefreshData();
                            }

                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else
                        {
                            WaitingManager.Show();
                            //bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/Approve", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                if (rs != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == rs.ExpMest.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.ExpMest.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_APPROVAL_LOGINNAME = rs.ExpMest.LAST_APPROVAL_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }

                        CommonParam paramMediStock = new CommonParam();
                        HisMediStockExtyFilter extyFilter = new HisMediStockExtyFilter();
                        extyFilter.MEDI_STOCK_ID = row.MEDI_STOCK_ID;
                        var listMediStockExty = new BackendAdapter(paramMediStock).Get<List<HIS_MEDI_STOCK_EXTY>>("api/HisMediStockExty/Get", ApiConsumers.MosConsumer, extyFilter, paramMediStock).ToList();
                        if (listMediStockExty != null && listMediStockExty.Count > 0)
                        {
                            if (listMediStockExty.FirstOrDefault().IS_AUTO_EXECUTE == 1 && success && chkInHDSD.Checked)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Thực hiện gọi hàm in HDSD ");
                                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                                store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, deletePrintTemplate);
                            }
                        }
                    }





                    //WaitingManager.Show();
                    //bool success = false;
                    //CommonParam param = new CommonParam();
                    //MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                    ////if (row.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__CHMS)
                    ////{
                    ////    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BrowseExportTicket").FirstOrDefault();
                    ////    if (moduleData == null)
                    ////    {
                    ////        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.BrowseExportTicket");
                    ////        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    ////    }
                    ////    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    ////    {
                    ////        List<object> listArgs = new List<object>();
                    ////        listArgs.Add(row.ID);
                    ////        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    ////        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    ////        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ////        WaitingManager.Hide();
                    ////        ((Form)extenceInstance).ShowDialog();
                    ////    }
                    ////    else
                    ////    {
                    ////        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    ////    }
                    ////}
                    ////else
                    ////{
                    //MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                    //Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, row);
                    //data.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                    //var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    //if (apiresul != null)
                    //{
                    //    success = true;
                    //    RefreshData();
                    //}
                    //WaitingManager.Hide();
                    //#region Show message
                    //Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    //#endregion

                    //#region Process has exception
                    //HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    //#endregion
                    ////}
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEnableDisApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                CommonParam param = new CommonParam();
                bool success = false;
                if (row != null)
                {
                    WaitingManager.Show();

                    HisExpMestSDO ado = new HisExpMestSDO();
                    ado.ExpMestId = row.ID;
                    ado.ReqRoomId = this.roomId;
                    if (gridControl.DataSource != null)
                    {
                        var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Decline", ApiConsumer.ApiConsumers.MosConsumer, ado, param);
                        if (apiresul != null)
                        {
                            foreach (var item in datagridcontrol)
                            {
                                if (item.ID == apiresul.ID)
                                {
                                    var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                    item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                    item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                    item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                    item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                    break;
                                }
                            }
                            success = true;
                            gridView.BeginUpdate();
                            datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                            gridControl.DataSource = datagridcontrol;
                            gridView.EndUpdate();
                        }
                    }
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void checkExport(long? expTime)
        {
            try
            {
                isExpWithExpTime = false;
                ExpTime = expTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonEnableActualExport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                isExpWithExpTime = false;
                bool success = false;
                CommonParam param = new CommonParam();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                if (row != null)
                {
                    if (rowDataExpMest != null && rowDataExpMest.EXP_MEST_TYPE_ID == 1 && HisConfigCFG.AllowEditExpTime == "1")
                    {
                        isExpWithExpTime = true;
                        frmMessage frm = new frmMessage(row, checkExport);
                        frm.ShowDialog();
                    }
                    if (isExpWithExpTime == false)
                    {
                        if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                        {
                            bool IsFinish = false;
                            if (row.IS_EXPORT_EQUAL_APPROVE == 1)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Đã xuất hết số lượng duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            else if (row.IS_EXPORT_EQUAL_APPROVE == null || row.IS_EXPORT_EQUAL_APPROVE != 1)
                            {
                                HisExpMestMetyReqFilter expMestMetyReqFilter = new HisExpMestMetyReqFilter();
                                expMestMetyReqFilter.EXP_MEST_ID = row.ID;

                                var listExpMestMetyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, expMestMetyReqFilter, param);

                                HisExpMestMatyReqFilter expMestMatyReqFilter = new HisExpMestMatyReqFilter();
                                expMestMatyReqFilter.EXP_MEST_ID = row.ID;

                                var listExpMestMatyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, expMestMatyReqFilter, param);

                                List<AmountADO> amountAdo = new List<AmountADO>();

                                if (listExpMestMetyReq != null && listExpMestMetyReq.Count > 0)
                                {
                                    foreach (var item in listExpMestMetyReq)
                                    {
                                        var ado = new AmountADO(item);
                                        amountAdo.Add(ado);
                                    }
                                }

                                if (listExpMestMatyReq != null && listExpMestMatyReq.Count > 0)
                                {
                                    foreach (var item in listExpMestMatyReq)
                                    {
                                        var ado = new AmountADO(item);
                                        amountAdo.Add(ado);
                                    }
                                }

                                if (amountAdo != null && amountAdo.Count > 0)
                                {
                                    var dataAdo = amountAdo.Where(o => o.Amount > o.Dd_Amount || o.Dd_Amount == null).ToList();
                                    //if (dataAdo != null && dataAdo.Count > 0)
                                    //{
                                    //    if (XtraMessageBox.Show("Phiếu chưa duyệt đủ số lượng yêu cầu. Bạn có muốn hoàn thành phiếu xuất?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    //    {
                                    //        IsFinish = true;
                                    //    }
                                    //}
                                    //else
                                    IsFinish = true;
                                }

                            }

                            HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.ReqRoomId = this.roomId;
                            sdo.IsFinish = IsFinish;
                            if (ExpTime != null && ExpTime > 0)
                            {
                                sdo.ExpTime = ExpTime;
                            }
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                    (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                if (apiresult != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresult.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_EXP_LOGINNAME = apiresult.LAST_EXP_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            HisExpMestSDO sdo = new HisExpMestSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.ReqRoomId = this.roomId;
                            //sdo.IsFinish = true;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                    ("api/HisExpMest/InPresExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                if (apiresult != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresult.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_EXP_LOGINNAME = apiresult.LAST_EXP_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            HisExpMestSDO sdo = new HisExpMestSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.ReqRoomId = this.roomId;
                            //sdo.IsFinish = true;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                    ("api/HisExpMest/AggrExamExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                if (apiresult != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresult.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_EXP_LOGINNAME = apiresult.LAST_EXP_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else
                        {
                            HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.ReqRoomId = this.roomId;
                            sdo.IsFinish = true;
                            if (ExpTime != null && ExpTime > 0)
                            {
                                sdo.ExpTime = ExpTime;
                            }
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                    (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                if (apiresult != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresult.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.LAST_EXP_LOGINNAME = apiresult.LAST_EXP_LOGINNAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                    }
                }
                if (success && chkInHDSD.Checked)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Thực hiện gọi hàm in HDSD ");
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, deletePrintTemplate);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ButtonCopyExportMest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var expMestData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                //SessionManager.GetFormMain().ExportMedicineForCopyExpMestClick(expMestData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEnableMobaImpCreate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST ExpMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(ExpMestData, rowDataExpMest);

                if (ExpMestData != null)
                {
                    if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ExpMestData.ID);
                        CallModule callModule = new CallModule(CallModule.MobaDepaCreate, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                        FillDataApterClose(ExpMestData);
                    }
                    else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ExpMestData.ID);
                        CallModule callModule = new CallModule(CallModule.MobaBloodCreate, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                        FillDataApterClose(ExpMestData);
                    }
                    else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ExpMestData.ID);
                        CallModule callModule = new CallModule(CallModule.MobaSaleCreate, this.roomId, this.roomTypeId, listArgs);

                        WaitingManager.Hide();
                        FillDataApterClose(ExpMestData);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterClose(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST ExpMestData)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterSave(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    if (gridControl.DataSource != null)
                    {
                        HisExpMestResultSDO ado = new HisExpMestResultSDO();
                        if (prescription is HisExpMestResultSDO)
                            ado = (HisExpMestResultSDO)prescription;
                        else if (prescription is HIS_EXP_MEST)
                        {
                            ado.ExpMest = new HIS_EXP_MEST();
                            ado.ExpMest = (HIS_EXP_MEST)prescription;
                        }

                        List<V_HIS_EXP_MEST_2> datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                        foreach (var item in datagridcontrol)
                        {
                            if (item.ID == ado.ExpMest.ID)
                            {
                                var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == ado.ExpMest.EXP_MEST_STT_ID);
                                item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                item.TDL_BLOOD_CODE = ado.ExpMest.TDL_BLOOD_CODE;
                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                break;
                            }
                        }
                        gridView.BeginUpdate();
                        datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                        gridControl.DataSource = datagridcontrol;
                        gridView.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonRequest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy duyệt không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                    V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                    if (row != null)
                    {
                        if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();

                            HisExpMestSDO data = new HisExpMestSDO();
                            data.ExpMestId = row.ID;
                            data.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/AggrExamUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                if (apiresul != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresul.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();

                            HisExpMestSDO data = new HisExpMestSDO();
                            data.ExpMestId = row.ID;
                            data.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/InPresUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                if (apiresul != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresul.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();

                            HisExpMestSDO data = new HisExpMestSDO();
                            data.ExpMestId = row.ID;
                            data.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var datagridcontrol = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                if (apiresul != null)
                                {
                                    foreach (var item in datagridcontrol)
                                    {
                                        if (item.ID == apiresul.ID)
                                        {
                                            var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                            item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                            item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                            item.TDL_BLOOD_CODE = apiresul.TDL_BLOOD_CODE;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    datagridcontrol = datagridcontrol.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = datagridcontrol;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonAssignTest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST expMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMestData, rowDataExpMest);
                if (expMestData != null)
                {
                    List<object> listArgs = new List<object>();
                    AssignServiceTestADO assignBloodADO = new AssignServiceTestADO(0, 0, 0, null);
                    GetTreatmentIdFromResultData(expMestData, ref assignBloodADO);
                    listArgs.Add(assignBloodADO);
                    CallModule callModule = new CallModule(CallModule.AssignServiceTest, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatmentIdFromResultData(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST resultExpMest, ref AssignServiceTestADO assignBloodADO)
        {
            try
            {
                long __expMestId = ((resultExpMest != null && resultExpMest.ID > 0) ? resultExpMest.ID : 0);
                MOS.Filter.HisExpMestViewFilter expFilter = new MOS.Filter.HisExpMestViewFilter();
                expFilter.ID = __expMestId;
                var listExp = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expFilter, null);
                if (resultExpMest != null && listExp.Count == 1)
                {
                    MOS.Filter.HisTreatmentView2Filter treatmentView2Filter = new MOS.Filter.HisTreatmentView2Filter();
                    treatmentView2Filter.PATIENT_ID = listExp.First().TDL_PATIENT_ID;
                    var listTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW_2, ApiConsumer.ApiConsumers.MosConsumer, treatmentView2Filter, null);
                    if (listTreatment != null && listTreatment.Count == 1)
                    {
                        assignBloodADO.TreatmentId = listTreatment.First().ID;
                        assignBloodADO.GenderName = listTreatment.First().TDL_PATIENT_GENDER_NAME;
                        assignBloodADO.PatientDob = (listTreatment.First().TDL_PATIENT_DOB);
                        assignBloodADO.PatientName = listTreatment.First().TDL_PATIENT_NAME;
                        assignBloodADO.ExpMestId = __expMestId;
                        assignBloodADO.ServiceReqId = listExp.First().SERVICE_REQ_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_Bill_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                if (rowDataExpMest != null && !rowDataExpMest.BILL_ID.HasValue && rowDataExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(rowDataExpMest.ID);
                    DelegateSelectData dl = new DelegateSelectData(this.ProcessRefressDelefate);
                    listArgs.Add(dl);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.MedicineSaleBill", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_CancelBill_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                if (rowDataExpMest != null
                    && rowDataExpMest.BILL_ID.HasValue
                    && rowDataExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                {
                    if (!HisConfigCFG.EXPORT_SALE__MUST_BILL || rowDataExpMest.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {

                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowDataExpMest.BILL_ID.Value);
                        listArgs.Add(rowDataExpMest);
                        DelegateSelectData dl = new DelegateSelectData(this.ProcessRefressDelefate);
                        listArgs.Add(dl);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TransactionCancel", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                        Inventec.Common.Logging.LogSystem.Debug("End call  HIS.Desktop.Plugins.TransactionCancel");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("NOT call 1  HIS.Desktop.Plugins.TransactionCancel");
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("NOT call 2  HIS.Desktop.Plugins.TransactionCancel");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ProcessRefressDelefate(object data)
        {
            try
            {
                if (data != null)
                {
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool deletePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099:
                            InHuongDanSuDungThuoc(printTypeCode, fileName);
                            break;
                        default:
                            break;
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

        private void InHuongDanSuDungThuoc(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();

                if (rowDataExpMest != null)
                {
                    CommonParam param = new CommonParam();

                    V_HIS_EXP_MEST ExpMestData = new V_HIS_EXP_MEST();
                    //List<V_HIS_EXP_MEST> lstExpMestData = new List<V_HIS_EXP_MEST>();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(ExpMestData, rowDataExpMest);

                    //lstExpMestData.Add(ExpMestData);

                    HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_ID = rowDataExpMest.ID;
                    List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);


                    //HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                    //expMestMaterialFilter.EXP_MEST_ID = rowDataExpMest.ID;
                    //List<V_HIS_EXP_MEST_MATERIAL> expMestMaterial = new BackendAdapter(param)
                    //    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                    MPS.Processor.Mps000099.PDO.Mps000099PDO rdo = new MPS.Processor.Mps000099.PDO.Mps000099PDO(ExpMestData, expMestMedicines);


                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public int data { get; set; }
    }
}
