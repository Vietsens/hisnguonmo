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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Util;
using HIS.Desktop.Utilities.Extensions;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void FillPrescriptionDataToControl()
        {
            try
            {
                LogSystem.Debug("FillPrescriptionDataToControl => 1");

                this.cboMediStockExport.Enabled = false;
                GridCheckMarksSelection gridCheckMark = this.cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                //Trường hợp sửa đơn thuốc trong kho sẽ phải xử lý thêm việc load kho của đơn cũ
                if (this.oldExpMest != null)
                {
                    if (gridCheckMark != null)
                    {
                        var mediStocks = this.currentMediStock.Where(o => o.MEDI_STOCK_ID == this.oldExpMest.MEDI_STOCK_ID).ToList();
                        gridCheckMark.ClearSelection(this.cboMediStockExport.Properties.View);
                        //Xử lý trường hợp nếu sửa đơn thuốc trong kho, mà kho đó lại chưa được chọn hoặc không có trong danh sách các kho được thiết lập cho phòng đang làm việc thì sẽ lấy kho trong danh mục ra => hiển thị kho của đơn cũ lên giao diện
                        if (mediStocks == null || mediStocks.Count == 0)
                        {
                            var tempMedi = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.oldExpMest.MEDI_STOCK_ID);
                            if (tempMedi != null)
                            {
                                V_HIS_MEST_ROOM oldMedi = new V_HIS_MEST_ROOM();
                                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEST_ROOM>(oldMedi, tempMedi);
                                oldMedi.ID = tempMedi.ID;
                                oldMedi.MEDI_STOCK_ID = tempMedi.ID;
                                mediStocks.Add(oldMedi);
                            }
                        }
                        this.currentMediStock.Clear();
                        this.currentMediStock.AddRange(mediStocks);
                        gridCheckMark.SelectAll(this.currentMediStock);
                    }
                }

                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                dateInputADO.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.oldServiceReq.INTRUCTION_TIME) ?? new DateTime();
                dateInputADO.Dates = new List<DateTime?>();
                dateInputADO.Dates.Add(dateInputADO.Time);

                this.txtTimeTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.oldServiceReq.ASSIGN_TIME_TO??0);
                this.TIME_TO = this.oldServiceReq.ASSIGN_TIME_TO;
                this.ucDateProcessor.Reload(this.ucDate, dateInputADO);
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(ucDate);
                this.isMultiDateState = false;

                if (this.oldServiceReq.USE_TIME == null)
                    this.oldServiceReq.USE_TIME = this.oldServiceReq.INTRUCTION_TIME;
                //if (this.oldServiceReq.USE_TIME_TO.HasValue)
                //    spinSoNgay.Value = DateTimeUtil.TimeMinus(this.oldServiceReq.USE_TIME, this.oldServiceReq.USE_TIME_TO);

                if (this.oldServiceReq.REMEDY_COUNT.HasValue)
                    this.txtLadder.Text = this.oldServiceReq.REMEDY_COUNT.ToString();
                this.txtAdvise.Text = this.oldServiceReq.ADVISE;

                IcdInputADO icd = new IcdInputADO();
                icd.ICD_CODE = this.oldServiceReq.ICD_CODE;
                icd.ICD_NAME = this.oldServiceReq.ICD_NAME;
                if (ucIcd != null)
                {
                    icdProcessor.Reload(ucIcd, icd);
                }

                IcdInputADO icdCause = new IcdInputADO();
                icdCause.ICD_CODE = this.oldServiceReq.ICD_CAUSE_CODE;
                icdCause.ICD_NAME = this.oldServiceReq.ICD_CAUSE_NAME;
                if (ucIcdCause != null)
                {
                    icdCauseProcessor.Reload(ucIcdCause, icdCause);
                }

                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                subIcd.ICD_SUB_CODE = this.oldServiceReq.ICD_SUB_CODE;
                subIcd.ICD_TEXT = this.oldServiceReq.ICD_TEXT;
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                }

                var user = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME.ToUpper().Equals(this.oldServiceReq.REQUEST_LOGINNAME.ToUpper()));
                if (user != null)
                {
                    this.cboUser.EditValue = user.LOGINNAME;
                    this.txtLoginName.Text = user.LOGINNAME;
                }

                if (this.currentMediStock != null && this.currentMediStock.Count == 0)
                {
                    this.currentMediStock.AddRange(this.currentMestRoomByRooms);
                }
                this.InitDataMetyMatyTypeInStockD1(this.currentMediStock);
                this.RebuildMediMatyWithInControlContainer(this.GetDataMediMatyInStock());

                LogSystem.Debug("FillPrescriptionDataToControl => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPrescriptionForEdit()
        {
            try
            {
                LogSystem.Debug("LoadPrescriptionForEdit => 1");
                //Trường hợp sửa đơn thuốc, truyền dữ liệu đầu vào => Get dữ liệu đơn thuốc cũ => fill dữ liệu vào form kê đơn
                if (this.assignPrescriptionEditADO != null)
                {
                    if (this.assignPrescriptionEditADO.ServiceReq == null) throw new ArgumentNullException("ServiceReq");
                    if (this.assignPrescriptionEditADO.ServiceReq.ID == 0) throw new ArgumentNullException("ServiceReq.ID");
                    //if (this.assignPrescriptionEditADO.ExpMest == null) throw new ArgumentNullException("ExpMest");
                    WaitingManager.Show();

                    this.InitWorker();

                    this.isNotChangeTutorial = true;

                    this.idRow = 1;
                    this.gridControlServiceProcess.DataSource = null;
                    this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                    this.actionType = GlobalVariables.ActionEdit;
                    this.oldExpMest = this.assignPrescriptionEditADO.ExpMest;
                    this.oldExpMestId = (this.oldExpMest != null ? this.oldExpMest.ID : 0);
                    this.oldServiceReq = this.assignPrescriptionEditADO.ServiceReq;
                    this.IsManyDay = false;

                    if (this.USE_TIME == null)
                    {
                        this.USE_TIME = new List<long>();
                    }
                    if (this.assignPrescriptionEditADO.ServiceReq.USE_TIME != null && this.assignPrescriptionEditADO.ServiceReq.USE_TIME > 0)
                    {
                        DateTime use_time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.assignPrescriptionEditADO.ServiceReq.USE_TIME ?? 0) ?? DateTime.MinValue;

                        this.USE_TIME.Add(this.assignPrescriptionEditADO.ServiceReq.USE_TIME ?? 0);
                        this.txtDuTruTime.Text = use_time.ToString("dd/MM/yyyy");
                        //this.cboDuTruTime.DateTime = use_time;
                    }

                    if(this.assignPrescriptionEditADO.ServiceReq.ASSIGN_TIME_TO != null && this.assignPrescriptionEditADO.ServiceReq.ASSIGN_TIME_TO > 0)
                    {
                        DateTime to_time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.assignPrescriptionEditADO.ServiceReq.ASSIGN_TIME_TO ?? 0) ?? DateTime.MinValue;
                        this.txtTimeTo.Text = to_time.ToString("dd/MM/yyyy");
                        this.TIME_TO = this.assignPrescriptionEditADO.ServiceReq.ASSIGN_TIME_TO;
                        cboTimeTo.DateTime = to_time;
                    }
                    LogSystem.Debug("Du lieu use_time load len tu don cu,USE_TIME : " + this.assignPrescriptionEditADO.ServiceReq.USE_TIME + " TIME_TO: "+ this.assignPrescriptionEditADO.ServiceReq.ASSIGN_TIME_TO);
                    if (this.oldServiceReq != null)
                    {
                        LogSystem.Debug("LoadPrescriptionForEdit => 2");
                        this.FillPrescriptionDataToControl();
                        LogSystem.Debug("LoadPrescriptionForEdit => 3");
                        paramCommon = new CommonParam();
                        if (oldExpMestId > 0)
                        {
                            this.expMestMedicineEditPrints = this.GetExpMestMedicineByExpMestId(oldExpMestId);
                            this.ProcessGetExpMestMedicine(this.expMestMedicineEditPrints, true);
                            this.expMestMaterialEditPrints = this.GetExpMestMaterialByExpMestId(oldExpMestId);
                            this.ProcessGetExpMestMaterial(this.expMestMaterialEditPrints, true);
                            //this.GetBeanPrescriptionForEdit(sereServInServiceReqs, this.oldExpMest.MEDI_STOCK_ID);
                            LogSystem.Debug("LoadPrescriptionForEdit => 4");
                        }
                        else
                        {
                            //Trường hợp sửa đơn mà đơn chỉ có thuốc - vật tư ngoài kho - mua ngoài => mặc định check chọn thuốc ngoài kho
                            if (rdOpionGroup.Properties.Items.Count > 1)
                            {
                                rdOpionGroup.SelectedIndex = 1;
                            }
                        }

                        LogSystem.Debug("LoadPrescriptionForEdit => 5");
                        this.serviceReqMetys = this.GetServiceReqMetyByServiceReqId(this.oldServiceReq.ID);
                        this.ProcessGetServiceReqMety(this.serviceReqMetys, true);
                        this.serviceReqMatys = this.GetServiceReqMatyByServiceReqId(this.oldServiceReq.ID);
                        this.ProcessGetServiceReqMaty(this.serviceReqMatys, true);
                        this.ProcessMediStockByOldExpMest(mediMatyTypeADOs);
                        UpdateRemyCountForDetailEditPres();

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.mediMatyTypeADOs), this.mediMatyTypeADOs));
                        LogSystem.Debug("LoadPrescriptionForEdit => 6");
                        this.ProcessInstructionTimeMediForEdit();
                        this.ProcessMergeDuplicateRowForListProcessing();
                        this.ValidDataMediMaty();

                        LogSystem.Debug("LoadPrescriptionForEdit => 7");
                        this.SetUseDayToPrescriptionEdit();
                        this.gridControlServiceProcess.DataSource = this.mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
                        this.SetEnableButtonControl(this.actionType);
                        this.SetTotalPrice__TrongDon();
                        this.idRow = (int)((this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0) ? this.mediMatyTypeADOs.Max(o => o.NUM_ORDER ?? 0) + stepRow : 0 + stepRow);
                        this.InstructionTime = this.oldServiceReq.INTRUCTION_TIME;
                        if (this.oldServiceReq.TRACKING_ID.HasValue)
                        {
                            cboPhieuDieuTri.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(this.oldServiceReq.TRACKING_ID.Value.ToString());
                            cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
                        }
                        //this.chkHomePres.Checked = (this.oldServiceReq.IS_HOME_PRES == 1);  
                        bool isExistsExpend = this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Any(o => o.SereServParentId > 0) ? true : false;
                        if (HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && isExistsExpend)
                            this.grcExpend__TabMedicine.Visible = true;

                        this.SetTutorialAndSoNgay();
                        this.isNotChangeTutorial = false;
                        WaitingManager.Hide();
                        LogSystem.Debug("LoadPrescriptionForEdit => 8");
                    }
                    else
                        CheckAssignServiceSimultaneityOption();
                }else
                    CheckAssignServiceSimultaneityOption();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageManager.Show(ResourceMessage.SuaDonThuocDuLieuTruyenVaoKhongHopLe);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMediStockByOldExpMest(List<MediMatyTypeADO> mediMatyTypeADOs)
        {
            try
            {
                if (mediMatyTypeADOs == null || mediMatyTypeADOs.Count == 0)
                    return;
                var stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == oldExpMest.MEDI_STOCK_ID);
                foreach (var item in mediMatyTypeADOs)
                {
                    if(stock != null && stock.IS_EXPEND == 1)
                    {
                        item.IsExpend = true;
                        item.IsDisableExpend = true;
                    }    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTutorialAndSoNgay()
        {
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    MediMatyTypeADO mediMatyTypeADOHD = this.mediMatyTypeADOs.FirstOrDefault(o => !String.IsNullOrEmpty(o.TUTORIAL));
                    txtHuongDan.Text = mediMatyTypeADOHD != null ? mediMatyTypeADOHD.TUTORIAL : "";

                    MediMatyTypeADO mediMatyTypeADOSoNgay = this.mediMatyTypeADOs.FirstOrDefault(o => o.UseDays.HasValue);
                    spinSoNgay.Value = mediMatyTypeADOSoNgay != null ? mediMatyTypeADOSoNgay.UseDays.Value : 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetUseDayToPrescriptionEdit()
        {
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0 && this.oldServiceReq != null && this.oldServiceReq.INTRUCTION_TIME > 0)
                {
                    foreach (var item in this.mediMatyTypeADOs)
                    {
                        System.DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.UseTimeTo ?? 0).Value;
                        System.DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.oldServiceReq.INTRUCTION_TIME).Value;
                        TimeSpan diff__Day = (dtUseTimeTo - dtInstructionTime);
                        item.UseDays = diff__Day.Days + 1;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private string GetPatientTypeNameById(long patientTypeId)
        {
            string result = "";
            try
            {
                var pt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeId);
                if (pt != null)
                {
                    result = pt.PATIENT_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal decimal GetAmount()
        {
            decimal value = 0;
            try
            {
                value = this.spinAmount.Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }

        private bool ValidMetyMatyType__Update()
        {
            bool valid = true;
            try
            {
                if (this.ValidAddRow(this.currentMedicineTypeADOForEdit)
                    && HIS.Desktop.Plugins.AssignPrescriptionYHCT.ValidAcinInteractiveWorker.Valid(this.currentMedicineTypeADOForEdit, mediMatyTypeADOs, this.LstExpMestMedicine)
                    )
                {
                    if (this.mediMatyTypeADOs == null)
                        this.mediMatyTypeADOs = new List<MediMatyTypeADO>();

                    var medicinetypeStockExists = this.mediMatyTypeADOs
                       .FirstOrDefault(o => o.SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID && (o.PRE_AMOUNT ?? 0) == 0);
                    if (medicinetypeStockExists != null)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThuocDaduocKe, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True) == DialogResult.No)
                        {
                            return false;
                        }
                    }

                    //Lay thuoc trong kho va kiem tra lai kha dung    
                    var checkMatyInStock = GetDataAmountOutOfStock(this.currentMedicineTypeADOForEdit, this.currentMedicineTypeADOForEdit.SERVICE_ID, (this.currentMedicineTypeADOForEdit.MEDI_STOCK_ID ?? 0));
                    if (checkMatyInStock == null)
                        throw new ArgumentNullException("checkMatyInStock is null");
                    MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);
                    if (this.GetAmount() > (checkMediMatyTypeADO.AMOUNT ?? 0))
                    {
                        MessageManager.Show(ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho);
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
                MessageManager.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
            }
            return valid;
        }
    }
}
