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
using HIS.Desktop.Plugins.InsuranceExpertise.Base;
using HIS.Desktop.Plugins.InsuranceExpertise.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InsuranceExpertise
{
    public partial class UCInsuranceExpertise
    {

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridTreatment();
                if (listTreatment != null && listTreatment.Count == 1)
                {
                    this.currentTreatment = listTreatment.First();
                    ClickTreatment(currentTreatment);
                    FillDataToGridHeinCardAndHeinApproval();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLockHein_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnLockHein.Enabled || this.currentTreatment == null)
                    return;
                CommonParam param = new CommonParam();
                bool success = false;

                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                if (this.currentTreatment.IS_LOCK_HEIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    param.Messages.Add(Base.ResourceMessageLang.HoSoDieuTriDaDuocKhoaBhyt);
                    goto End;
                }

                if (!this.currentTreatment.COUNT_HEIN_APPROVAL.HasValue || this.currentTreatment.COUNT_HEIN_APPROVAL <= 0)
                {
                    param.Messages.Add(Base.ResourceMessageLang.HoSoDieuTriChuaDuocDuyetGiamDinhBhyt);
                    goto End;
                }
                HisTreatmentLockHeinSDO sdo = new HisTreatmentLockHeinSDO();
                sdo.TreatmentId = this.currentTreatment.ID;
                if (dtHeinLockTime.EditValue != null)
                {
                    sdo.HeinLockTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHeinLockTime.DateTime);
                }
                else
                {
                    sdo.HeinLockTime = null;
                }
                if (!String.IsNullOrWhiteSpace(txtStoreBordereauCode.Text))
                {
                    string code = txtStoreBordereauCode.Text.Trim();
                    if (code.Length < 5)
                    {
                        code = string.Format("{0:00000}", Convert.ToInt64(code));
                        txtStoreBordereauCode.Text = code;
                    }
                    sdo.StoreBordereauCode = code;
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>(HisRequestUriStore.HIS_TREATMENT_LOCK_HEIN, ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    this.currentTreatment.IS_LOCK_HEIN = rs.IS_LOCK_HEIN;
                    this.currentTreatment.STORE_BORDEREAU_CODE = rs.STORE_BORDEREAU_CODE;
                    if (this.currentTreatment.IS_LOCK_HEIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        btnUnLockHein.Enabled = true;
                        btnLockHein.Enabled = false;
                    }
                    else
                    {
                        btnLockHein.Enabled = true;
                        btnUnLockHein.Enabled = false;
                    }
                    if (Config.HisConfigCFG.OptionStoreBordereauCode == "2")
                    {
                        ChangeEnableButtonOption2();
                    }
                }

            End:
                WaitingManager.Hide();
                if (success)
                {
                    FillDataToGridTreatment();
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

        private void btnUnLockHein_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnUnLockHein.Enabled || this.currentTreatment == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                if (this.currentTreatment.IS_LOCK_HEIN != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    param.Messages.Add(Base.ResourceMessageLang.HoSoDieuTriDangDuocMoKhoaBhyt);
                    goto End;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>(HisRequestUriStore.HIS_TREATMENT_UNLOCK_HEIN, ApiConsumers.MosConsumer, this.currentTreatment.ID, param);
                if (rs != null)
                {
                    success = true;
                    this.currentTreatment.IS_LOCK_HEIN = rs.IS_LOCK_HEIN;
                    this.currentTreatment.STORE_BORDEREAU_CODE = rs.STORE_BORDEREAU_CODE;
                    if (this.currentTreatment.IS_LOCK_HEIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        btnUnLockHein.Enabled = true;
                        btnLockHein.Enabled = false;
                    }
                    else
                    {
                        btnLockHein.Enabled = true;
                        btnUnLockHein.Enabled = false;
                    }
                    if (Config.HisConfigCFG.OptionStoreBordereauCode == "2")
                    {
                        ChangeEnableButtonOption2();
                    }
                    gridControlTreatment.BeginUpdate();
                    gridControlTreatment.DataSource = listTreatment;
                    gridControlTreatment.EndUpdate();
                }

            End:
                WaitingManager.Hide();
                if (success)
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

        private void repositoryItemBtnApprovalOne_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewHeinCard.FocusedRowHandle >= 0 && this.cashierRoom != null)
                {
                    var data = (HIS_PATIENT_TYPE_ALTER)gridViewHeinCard.GetFocusedRow();
                    if (data != null)
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        HIS_HEIN_APPROVAL obj = new HIS_HEIN_APPROVAL();
                        obj.TREATMENT_ID = this.currentTreatment.ID;
                        if (dtExecuteTime.EditValue != null && dtExecuteTime.DateTime != DateTime.MinValue)
                        {
                            obj.EXECUTE_TIME = Convert.ToInt64(dtExecuteTime.DateTime.ToString("yyyyMMddHHmmss"));
                        }
                        else
                        {
                            obj.EXECUTE_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        }
                        obj.HEIN_CARD_FROM_TIME = data.HEIN_CARD_FROM_TIME ?? 0;
                        obj.HEIN_CARD_NUMBER = data.HEIN_CARD_NUMBER;
                        obj.HEIN_CARD_TO_TIME = data.HEIN_CARD_TO_TIME ?? 0;
                        obj.HEIN_MEDI_ORG_CODE = data.HEIN_MEDI_ORG_CODE;
                        obj.HEIN_MEDI_ORG_NAME = data.HEIN_MEDI_ORG_NAME;
                        obj.LEVEL_CODE = data.LEVEL_CODE;
                        obj.LIVE_AREA_CODE = data.LIVE_AREA_CODE;
                        obj.RIGHT_ROUTE_CODE = data.RIGHT_ROUTE_CODE;
                        obj.RIGHT_ROUTE_TYPE_CODE = data.RIGHT_ROUTE_TYPE_CODE;
                        obj.JOIN_5_YEAR = data.JOIN_5_YEAR;
                        obj.PAID_6_MONTH = data.PAID_6_MONTH;
                        obj.HAS_BIRTH_CERTIFICATE = data.HAS_BIRTH_CERTIFICATE;
                        obj.ADDRESS = data.ADDRESS;
                        obj.FREE_CO_PAID_TIME = data.FREE_CO_PAID_TIME;
                        obj.CASHIER_ROOM_ID = cashierRoom.ID;
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_HEIN_APPROVAL>("api/HisHeinApproval/Create", ApiConsumers.MosConsumer, obj, param);
                        if (rs != null)
                        {
                            success = true;
                            this.currentTreatment.COUNT_HEIN_APPROVAL = this.currentTreatment.COUNT_HEIN_APPROVAL.HasValue ? this.currentTreatment.COUNT_HEIN_APPROVAL.Value + 1 : 1;
                            if (Config.AutoLockAfterApprovalBHYTCFG.IsAutoLockAfterApprovalBHYT)
                            {
                                this.currentTreatment.IS_LOCK_HEIN = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                            }
                            gridControlTreatment.BeginUpdate();
                            gridControlTreatment.DataSource = listTreatment;
                            gridControlTreatment.EndUpdate();
                            FillDataToGridHeinCardAndHeinApproval();
                            GenerateXml(rs);
                        }
                        WaitingManager.Hide();
                        if (success)
                        {
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                        else
                        {
                            MessageManager.Show(param, success);
                        }
                        SessionManager.ProcessTokenLost(param);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_GiamDinhHSDT_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Fire repositoryItemButtonEdit_GiamDinhHSDT_ButtonClick");
                if (gridViewTreatment.FocusedRowHandle >= 0 && this.cashierRoom != null)
                {
                    var data = (V_HIS_TREATMENT_1)gridViewTreatment.GetFocusedRow();
                    if (data != null)
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        HIS_HEIN_APPROVAL obj = new HIS_HEIN_APPROVAL();
                        obj.TREATMENT_ID = this.currentTreatment.ID;
                        if (dtExecuteTime.EditValue != null && dtExecuteTime.DateTime != DateTime.MinValue)
                        {
                            obj.EXECUTE_TIME = Convert.ToInt64(dtExecuteTime.DateTime.ToString("yyyyMMddHHmmss"));
                        }
                        else
                        {
                            obj.EXECUTE_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        }
                        obj.CASHIER_ROOM_ID = cashierRoom.ID;
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_HEIN_APPROVAL>("api/HisHeinApproval/Create", ApiConsumers.MosConsumer, obj, param);
                        if (rs != null)
                        {
                            success = true;
                            this.currentTreatment.COUNT_HEIN_APPROVAL = this.currentTreatment.COUNT_HEIN_APPROVAL.HasValue ? this.currentTreatment.COUNT_HEIN_APPROVAL.Value + 1 : 1;
                            if (Config.AutoLockAfterApprovalBHYTCFG.IsAutoLockAfterApprovalBHYT)
                            {
                                this.currentTreatment.IS_LOCK_HEIN = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                            }
                            gridControlTreatment.BeginUpdate();
                            gridControlTreatment.DataSource = listTreatment;
                            gridControlTreatment.EndUpdate();
                            FillDataToGridHeinCardAndHeinApproval();
                            GenerateXml(rs);
                        }
                        WaitingManager.Hide();
                        if (success)
                        {
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                        else
                        {
                            MessageManager.Show(param, success);
                        }
                        SessionManager.ProcessTokenLost(param);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnCancelApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HIS_HEIN_APPROVAL)gridViewHeinApproval.GetFocusedRow();
                if (data == null) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisHeinApproval/Delete", ApiConsumers.MosConsumer, data.ID, param);
                if (success)
                {
                    this.currentTreatment.IS_LOCK_HEIN = 0;
                    this.currentTreatment.COUNT_HEIN_APPROVAL -= 1;
                    gridControlTreatment.BeginUpdate();
                    gridControlTreatment.DataSource = listTreatment;
                    gridControlTreatment.EndUpdate();
                    if (this.currentTreatment.COUNT_HEIN_APPROVAL > 0)
                    {
                        FillDataToGridHeinCardAndHeinApproval();
                    }
                    else
                    {
                        FillDataToGridTreatment(new CommonParam(start, limit));
                    }
                }
                WaitingManager.Hide();
                if (success)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDownXml_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (this.currentTreatment == null) return;
                var row = (HIS_HEIN_APPROVAL)gridViewHeinApproval.GetFocusedRow();
                if (row != null && !String.IsNullOrEmpty(row.XML_URL))
                {
                    WaitingManager.Show();
                    MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(row.XML_URL);
                    if (stream == null)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.KhongTaiDuocFileXml, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                    else
                    {
                        string name = this.currentTreatment.TDL_HEIN_CARD_NUMBER + "_" + this.currentTreatment.TREATMENT_CODE + "_" + row.HEIN_APPROVAL_CODE;
                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                        saveFileDialog1.FileName = name + ".xml";
                        WaitingManager.Hide();
                        if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            string path = saveFileDialog1.FileName;
                            var enc = Encoding.UTF8;
                            var fileXml = enc.GetString(stream.ToArray());
                            using (var file = new StreamWriter(path))
                            {
                                file.Write(fileXml);
                            }
                            MessageManager.ShowAlert(this.ParentForm, new CommonParam(), true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemViewXmlEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (this.currentTreatment == null) return;
                var row = (HIS_HEIN_APPROVAL)gridViewHeinApproval.GetFocusedRow();
                if (row != null && !String.IsNullOrEmpty(row.XML_URL))
                {
                    MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(row.XML_URL);
                    if (stream == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.KhongTaiDuocFileXml, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                    else
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer'");
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            moduleData.RoomId = this.RoomId;
                            moduleData.RoomTypeId = this.RoomTypeId;
                            List<object> listArgs = new List<object>();
                            listArgs.Add(moduleData);
                            listArgs.Add(stream);
                            var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                            if (extenceInstance == null)
                            {
                                throw new ArgumentNullException("moduleData is null");
                            }

                            ((Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            MessageManager.Show(Base.ResourceMessageLang.ChucNangChuaHoTroPhienBanHienTai);
                        }
                    }
                }
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
                var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
                if (branch == null)
                {
                    return result;
                }
                GlobalConfigStore.Branch = branch;

                GlobalConfigStore.ListIcdCode_Nds = HisConfigCFG.GetListValue(HisConfigCFG.MRS_HIS_REPORT_BHYT_NDS_ICD_CODE__OTHER);
                GlobalConfigStore.ListIcdCode_Nds_Te = HisConfigCFG.GetListValue(HisConfigCFG.MRS_HIS_REPORT_BHYT_NDS_ICD_CODE__TE);

                GlobalConfigStore.PathSaveXml = path;

                GlobalConfigStore.IsInit = true;
                isInitXmlLocalData = true;
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void GenerateXml(V_HIS_HEIN_APPROVAL heinApproval)
        {
            try
            {
                string pathSave = Base.ConfigStore.GetFolderSaveXml + "\\ExportXml\\Xml" + DateTime.Now.ToString("ddMMyyyy");
                var dicInfo = System.IO.Directory.CreateDirectory(pathSave);
                CommonParam param = new CommonParam();
                if (!GlobalConfigStore.IsInit)
                    this.SetDataToLocalXml(pathSave);
                HisTreatmentView3Filter treatFilter = new HisTreatmentView3Filter();
                treatFilter.ID = heinApproval.TREATMENT_ID;
                var hisTreatments = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_3>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_3, ApiConsumers.MosConsumer, treatFilter, param);
                if (hisTreatments == null || hisTreatments.Count != 1)
                {
                    return;
                }

                HisSereServView2Filter ssFilter = new HisSereServView2Filter();
                ssFilter.HEIN_APPROVAL_ID = heinApproval.ID;
                var listSereServ = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW_2, ApiConsumers.MosConsumer, ssFilter, param);
                if (listSereServ != null || listSereServ.Count > 0)
                {
                    listSereServ = listSereServ.Where(o => o.HEIN_APPROVAL_ID.HasValue && o.TDL_HEIN_SERVICE_TYPE_ID.HasValue && o.HEIN_APPROVAL_ID.Value == heinApproval.ID && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.AMOUNT > 0 && o.PRICE > 0).ToList();
                }
                if (listSereServ == null || listSereServ.Count == 0)
                {
                    return;
                }

                InputADO ado = new InputADO();
                ado.HeinApproval = heinApproval;
                ado.Treatment = hisTreatments.First();
                ado.ListSereServ = listSereServ;

                var songaysinh = Inventec.Common.DateTime.Calculation.DifferenceDate(heinApproval.TDL_PATIENT_DOB, ado.Treatment.IN_TIME);
                if (songaysinh < 365)
                {
                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.TREATMENT_ID = heinApproval.TREATMENT_ID;
                    var listDhst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);
                    if (listDhst != null && listDhst.Count > 0)
                    {
                        ado.Dhst = listDhst.First();
                    }
                }

                His.Bhyt.ExportXml.CreateXmlMain xmlMain = new His.Bhyt.ExportXml.CreateXmlMain(ado);
                var success = xmlMain.Run917();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GenerateXml(List<V_HIS_TREATMENT_1> listTreatment)
        {
            try
            {
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    string pathSave = Base.ConfigStore.GetFolderSaveXml + "\\ExportXml\\Xml" + DateTime.Now.ToString("ddMMyyyy");
                    var dicInfo = System.IO.Directory.CreateDirectory(pathSave);

                    if (!GlobalConfigStore.IsInit)
                        this.SetDataToLocalXml(pathSave);
                    int start = 0;
                    int count = listTreatment.Count;
                    CommonParam param = new CommonParam();
                    while (count > 0)
                    {
                        int limit = (count <= GlobalVariables.MAX_REQUEST_LENGTH_PARAM) ? count : GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                        var listSub = listTreatment.Skip(start).Take(limit).ToList();
                        HisHeinApprovalViewFilter appBhytFilter = new HisHeinApprovalViewFilter();
                        appBhytFilter.TREATMENT_IDs = listSub.Select(s => s.ID).ToList();
                        List<V_HIS_HEIN_APPROVAL> ListHeinApproval = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_HEIN_APPROVAL>>("api/HisHeinApproval/GetView", ApiConsumers.MosConsumer, appBhytFilter, param);
                        HisSereServView2Filter ssFilter = new HisSereServView2Filter();
                        ssFilter.HEIN_APPROVAL_IDs = ListHeinApproval.Select(s => s.ID).ToList();
                        List<V_HIS_SERE_SERV_2> ListSereServ = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW_2, ApiConsumers.MosConsumer, ssFilter, param);

                        HisTreatmentView3Filter treatmentFilter = new HisTreatmentView3Filter();
                        treatmentFilter.IDs = appBhytFilter.TREATMENT_IDs;
                        List<V_HIS_TREATMENT_3> hisTreatments = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_3>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_3, ApiConsumers.MosConsumer, treatmentFilter, param);

                        HisDhstFilter dhstFilter = new HisDhstFilter();
                        dhstFilter.TREATMENT_IDs = appBhytFilter.TREATMENT_IDs;
                        List<HIS_DHST> listDhst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);

                        if (param.HasException)
                        {
                            throw new DataMisalignedException("Co exception xay ra tai DAOGET trong qua trinh lay du lieu xuat xml");
                        }
                        ProcessExportXmlDetail(hisTreatments, ListHeinApproval, ListSereServ, listDhst);
                        start += GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                        count -= GlobalVariables.MAX_REQUEST_LENGTH_PARAM;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessExportXmlDetail(List<V_HIS_TREATMENT_3> hisTreatments, List<V_HIS_HEIN_APPROVAL> ListHeinApproval, List<V_HIS_SERE_SERV_2> ListSereServ, List<HIS_DHST> listDhst)
        {
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
                        continue;
                    }
                    if (!dicSereServ.ContainsKey(heinApproval.ID))
                    {
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFlieXML_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (HIS_HEIN_APPROVAL)gridViewHeinApproval.GetFocusedRow();
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(row.XML_URL);
                if (stream == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.ChuaCoFile);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_GiamDinhHSDT_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridViewTreatment.FocusedRowHandle >= 0 && this.cashierRoom != null)
                {
                    var data = (V_HIS_TREATMENT_1)gridViewTreatment.GetFocusedRow();
                    if (data != null)
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        HisTreatmentHeinApprovalSDO obj = new HisTreatmentHeinApprovalSDO();
                        obj.TreatmentId = data.ID;
                        if (dtExecuteTime.EditValue != null && dtExecuteTime.DateTime != DateTime.MinValue)
                        {
                            obj.ExecuteTime = Convert.ToInt64(dtExecuteTime.DateTime.ToString("yyyyMMddHHmmss"));
                        }
                        else
                        {
                            obj.ExecuteTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        }

                        obj.RequestRoomId = this.RoomId;
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTreatment/HeinApproval", ApiConsumers.MosConsumer, obj, param);
                        if (rs)
                        {
                            this.currentTreatment = data;
                            success = true;
                            this.currentTreatment.COUNT_HEIN_APPROVAL = this.currentTreatment.COUNT_HEIN_APPROVAL.HasValue ? this.currentTreatment.COUNT_HEIN_APPROVAL.Value + 1 : 1;
                            if (Config.AutoLockAfterApprovalBHYTCFG.IsAutoLockAfterApprovalBHYT)
                            {
                                this.currentTreatment.IS_LOCK_HEIN = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                            }
                            gridControlTreatment.BeginUpdate();
                            gridControlTreatment.DataSource = listTreatment;
                            gridControlTreatment.EndUpdate();
                            FillDataToGridHeinCardAndHeinApproval();
                            WaitingManager.Hide();
                            if (HisConfigCFG.GetValue(HisConfigCFG.MOS_HIS_HEIN_APPROVAL__IS_AUTO_EXPORT_XML) == "1")
                            {
                                List<V_HIS_TREATMENT_1> treatment1s = new List<V_HIS_TREATMENT_1>();
                                treatment1s.Add(this.currentTreatment);
                                FolderBrowserDialog fbd = new FolderBrowserDialog();
                                string path = "";
                                if (fbd.ShowDialog() == DialogResult.OK)
                                {
                                    path = fbd.SelectedPath;
                                }
                                WaitingManager.Show();
                                GenerateXml4210(ref param, treatment1s, path);
                                WaitingManager.Hide();
                            }
                        }

                        if (success)
                        {
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                        else
                        {
                            WaitingManager.Hide();
                            MessageManager.Show(param, success);
                        }
                        SessionManager.ProcessTokenLost(param);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool GenerateXml4210(ref CommonParam paramExport, List<V_HIS_TREATMENT_1> listSelection, string pathSave)
        {
            bool result = false;
            try
            {
                string message = "";
                if (listSelection != null && listSelection.Count > 0)
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

                    List<V_HIS_TREATMENT_3> hisTreatments = new List<V_HIS_TREATMENT_3>();
                    HisTreatmentView3Filter treatmentFilter = new HisTreatmentView3Filter();
                    treatmentFilter.IDs = listSelection.Select(o => o.ID).ToList();
                    var resultTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_3>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_3, ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (resultTreatment != null && resultTreatment.Count > 0)
                    {
                        hisTreatments = resultTreatment;
                    }

                    if (param.HasException) throw new DataMisalignedException("Co exception xay ra tai DAOGET trong qua trinh lay du lieu xuat xml");

                    message = ProcessExportXmlDetail(ref result, hisTreatments, listViewHeinApproval, ListSereServ, listDhst, hisSereServTeins, hisTrackings, hisSereServPttts, ListEkipUser, ListBedlog);

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

        private void CreateThreadGetData(List<V_HIS_TREATMENT_1> listSelection)
        {
            System.Threading.Thread HeinApproval = new System.Threading.Thread(ThreadGetHeinApproval);
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
                        if (ekipIds != null && ekipIds.Count > 1)//null sẽ có 1 id bằng 0
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

        private void ThreadGetHeinApproval(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                listViewHeinApproval = new List<V_HIS_HEIN_APPROVAL>();

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
                        listViewHeinApproval.AddRange(resultHeinApproval);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string ProcessExportXmlDetail(ref bool isSuccess, List<V_HIS_TREATMENT_3> hisTreatments, List<V_HIS_HEIN_APPROVAL> hisHeinApprvals, List<V_HIS_SERE_SERV_2> ListSereServ, List<HIS_DHST> listDhst, List<V_HIS_SERE_SERV_TEIN> listSereServTein, List<HIS_TRACKING> hisTrackings, List<V_HIS_SERE_SERV_PTTT> hisSereServPttts, List<HIS_EKIP_USER> ListEkipUser, List<V_HIS_BED_LOG> ListBedlog)
        {
            string result = "";
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
                    //sap xep thoi gian tang dan de trong th co nhieu dhst se lay cai co thoi gian thuc hien lon nhat
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
                    ado.Branch = this._Branch;
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

                    His.Bhyt.ExportXml.CreateXmlMain xmlMain = new His.Bhyt.ExportXml.CreateXmlMain(ado);
                    var success = xmlMain.Run4210();
                    if (!success)
                    {
                        listResult.Add(treatment.TREATMENT_CODE);
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

        private void repositoryItem_Dowload_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (this.currentTreatment == null) return;
                if (this.currentTreatment != null && !String.IsNullOrEmpty(this.currentTreatment.XML4210_URL))
                {
                    WaitingManager.Show();
                    MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(this.currentTreatment.XML4210_URL);
                    if (stream == null)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.KhongTaiDuocFileXml, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                    else
                    {
                        string name = this.currentTreatment.TDL_HEIN_CARD_NUMBER + "_" + this.currentTreatment.TREATMENT_CODE + "_" + this.currentTreatment.FEE_LOCK_TIME;
                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                        saveFileDialog1.FileName = name + ".xml";
                        WaitingManager.Hide();
                        if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            string path = saveFileDialog1.FileName;
                            var enc = Encoding.UTF8;
                            var fileXml = enc.GetString(stream.ToArray());
                            using (var file = new StreamWriter(path))
                            {
                                file.Write(fileXml);
                            }
                            MessageManager.ShowAlert(this.ParentForm, new CommonParam(), true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItem_XMLView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (this.currentTreatment == null) return;
                if (this.currentTreatment != null && !String.IsNullOrEmpty(this.currentTreatment.XML4210_URL))
                {
                    MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(this.currentTreatment.XML4210_URL);
                    if (stream == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.KhongTaiDuocFileXml, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                    else
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer'");
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            moduleData.RoomId = this.RoomId;
                            moduleData.RoomTypeId = this.RoomTypeId;
                            List<object> listArgs = new List<object>();
                            listArgs.Add(moduleData);
                            listArgs.Add(stream);
                            var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                            if (extenceInstance == null)
                            {
                                throw new ArgumentNullException("moduleData is null");
                            }

                            ((Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            MessageManager.Show(Base.ResourceMessageLang.ChucNangChuaHoTroPhienBanHienTai);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
