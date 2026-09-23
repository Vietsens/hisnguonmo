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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using HIS.Desktop.Utilities.Extensions;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void FillPrescriptionDataToControl()
        {
            try
            {
                //Trường hợp sửa đơn thuốc trong kho sẽ phải xử lý thêm việc load kho của đơn cũ
                if (this.oldExpMest != null)
                {
                    var mediStocks = this.currentMediStock.Where(o => o.MEDI_STOCK_ID == this.oldExpMest.MEDI_STOCK_ID).ToList();
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
                    GridCheckMarksSelection gridCheckMark = this.cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(this.cboMediStockExport.Properties.View); //ref this.currentMediStock nên phải add lại
                        this.currentMediStock.AddRange(mediStocks);
                        //Xử lý trường hợp nếu sửa đơn thuốc trong kho, mà kho đó lại chưa được chọn hoặc không có trong danh sách các kho được thiết lập cho phòng đang làm việc thì sẽ lấy kho trong danh mục ra => hiển thị kho của đơn cũ lên giao diện
                        gridCheckMark.SelectAll(this.currentMediStock);
                    }
                    else
                    {
                        if (this.currentMediStock != null && this.currentMediStock.Count > 0)
                            cboMediStockExport.EditValue = this.currentMediStock.First().MEDI_STOCK_ID;
                    }
                }

                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                dateInputADO.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.oldServiceReq.INTRUCTION_TIME) ?? new DateTime();
                dateInputADO.Dates = new List<DateTime?>();
                dateInputADO.Dates.Add(dateInputADO.Time);

                this.ucDateProcessor.Reload(this.ucDate, dateInputADO);
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(ucDate);
                this.isMultiDateState = false;

                if (this.oldServiceReq.USE_TIME == null)
                    this.oldServiceReq.USE_TIME = this.oldServiceReq.INTRUCTION_TIME;

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

                var icdCaus = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE == this.oldServiceReq.ICD_CODE);
                if (icdCaus != null)
                {
                    this.icdCauseProcessor.SetRequired(this.ucIcdCause, (icdCaus.IS_REQUIRE_CAUSE == 1));
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
                //Trường hợp sửa đơn thuốc, truyền dữ liệu đầu vào => Get dữ liệu đơn thuốc cũ => fill dữ liệu vào form kê đơn
                if (this.assignPrescriptionEditADO != null)
                {
                    if (this.assignPrescriptionEditADO.ServiceReq == null) throw new ArgumentNullException("ServiceReq");
                    if (this.assignPrescriptionEditADO.ServiceReq.ID == 0) throw new ArgumentNullException("ServiceReq.ID");
                   
                    WaitingManager.Show();
                    this.InitWorker();
                    this.idRow = 1;
                    this.gridControlServiceProcess.DataSource = null;
                    this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                    this.actionType = GlobalVariables.ActionEdit;
                    this.oldExpMest = this.assignPrescriptionEditADO.ExpMest;
                    this.oldExpMestId = (this.oldExpMest != null ? this.oldExpMest.ID : 0);
                    this.oldServiceReq = this.assignPrescriptionEditADO.ServiceReq;
                    if (this.oldServiceReq != null)
                    {
                        paramCommon = new CommonParam();
                        if (oldExpMestId > 0)
                        {
                            this.expMestMedicineEditPrints = this.GetExpMestMedicineByExpMestId(oldExpMestId);
                            this.ProcessGetExpMestMedicine(this.expMestMedicineEditPrints, true);
                            this.expMestMaterialEditPrints = this.GetExpMestMaterialByExpMestId(oldExpMestId);
                            this.ProcessGetExpMestMaterial(this.expMestMaterialEditPrints, true);
                        }

                        this.serviceReqMetys = this.GetServiceReqMetyByServiceReqId(this.oldServiceReq.ID);
                        this.ProcessGetServiceReqMety(this.serviceReqMetys, true);
                        this.serviceReqMatys = this.GetServiceReqMatyByServiceReqId(this.oldServiceReq.ID);
                        this.ProcessGetServiceReqMaty(this.serviceReqMatys, true);

                        //Nếu tất cả thuốc vật tư kê trước đấy là ngoài kho thì enabled kho
                        isMediMatyIsOutStock = CheckAllMediMatyIsOutStock();
                        if (isMediMatyIsOutStock)
                        {
                            cboMediStockExport.Enabled = true;
                            cboMediStockExport.Properties.Tag = null;
                            cboMediStockExport.Properties.View.OptionsSelection.MultiSelect = false;
                            GridColumn columnCheck = cboMediStockExport.Properties.View.Columns.First(o => o.FieldName == "CheckMarkSelection");
                            if (columnCheck != null)
                                cboMediStockExport.Properties.View.Columns.Remove(columnCheck);
                        }
                        else
                            cboMediStockExport.Enabled = false;

                        this.FillPrescriptionDataToControl();
                        this.ProcessInstructionTimeMediForEdit();
                        this.ProcessMergeDuplicateRowForListProcessing();
                        this.ValidDataMediMaty();

                        this.SetUseDayToPrescriptionEdit();
                        this.gridControlServiceProcess.DataSource = this.mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
                        this.SetEnableButtonControl(this.actionType);
                        this.SetTotalPrice__TrongDon();
                        this.idRow = (int)((this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0) ? this.mediMatyTypeADOs.Max(o => o.NUM_ORDER ?? 0) : 0 + stepRow);
                        this.InstructionTime = this.oldServiceReq.INTRUCTION_TIME;
                        if (this.oldServiceReq.TRACKING_ID.HasValue)
                        {
                            cboPhieuDieuTri.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(this.oldServiceReq.TRACKING_ID.Value.ToString());
                            cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
                        }

                        this.chkHomePres.Checked = (this.oldServiceReq.IS_HOME_PRES == 1);
                        bool isExistsExpend = this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Any(o => o.SereServParentId > 0) ? true : false;
                        if (HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && isExistsExpend)
                        {
                            this.grcExpend__TabMedicine.Visible = true;
                            this.grcIsExpendType.Visible = true;
                        }

                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageManager.Show(ResourceMessage.SuaDonThuocDuLieuTruyenVaoKhongHopLe);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckAllMediMatyIsOutStock()
        {
            bool result = false;
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    MediMatyTypeADO mediMatyTypeADO = mediMatyTypeADOs.FirstOrDefault(o =>
                        o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                        || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                        || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD);
                    if (mediMatyTypeADO == null)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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
                        TimeSpan diff__Day = (dtUseTimeTo.Date - dtInstructionTime.Date);
                        item.UseDays = diff__Day.Days + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        
        //private bool ValidMetyMatyType__Update()
        //{
        //    bool valid = true;
        //    try
        //    {
        //        if (this.ValidAddRow(this.currentMedicineTypeADOForEdit)
        //            && HIS.Desktop.Plugins.AssignPrescriptionKidney.ValidAcinInteractiveWorker.Valid(this.currentMedicineTypeADOForEdit, mediMatyTypeADOs)
        //            )
        //        {
        //            if (this.mediMatyTypeADOs == null)
        //                this.mediMatyTypeADOs = new List<MediMatyTypeADO>();

        //            var medicinetypeStockExists = this.mediMatyTypeADOs
        //               .FirstOrDefault(o => o.SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID && (o.PRE_AMOUNT ?? 0) == 0);
        //            if (medicinetypeStockExists != null)
        //            {
        //                if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThuocDaduocKe, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True) == DialogResult.No)
        //                {
        //                    return false;
        //                }
        //            }

        //            //Lay thuoc trong kho va kiem tra lai kha dung    
        //            var checkMatyInStock = GetDataAmountOutOfStock(this.currentMedicineTypeADOForEdit, this.currentMedicineTypeADOForEdit.SERVICE_ID, (this.currentMedicineTypeADOForEdit.MEDI_STOCK_ID ?? 0));
        //            if (checkMatyInStock == null)
        //                throw new ArgumentNullException("checkMatyInStock is null");
        //            MediMatyTypeADO checkMediMatyTypeADO = new MediMatyTypeADO();
        //            Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(checkMediMatyTypeADO, checkMatyInStock);
        //            if (this.GetAmount() > (checkMediMatyTypeADO.AMOUNT ?? 0))
        //            {
        //                MessageManager.Show(ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho);
        //                valid = false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        valid = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //        MessageManager.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
        //    }
        //    return valid;
        //}
    }
}
