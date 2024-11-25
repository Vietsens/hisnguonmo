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
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Utilities.Extensions;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void FillPrescriptionDataToControl()
        {
            try
            {
                //Trường hợp sửa đơn thuốc trong kho sẽ phải xử lý thêm việc load kho của đơn cũ
                Inventec.Common.Logging.LogSystem.Debug("FillPrescriptionDataToControl. 1");

                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                dateInputADO.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.oldServiceReq.INTRUCTION_TIME) ?? new DateTime();
                dateInputADO.Dates = new List<DateTime?>();
                dateInputADO.Dates.Add(dateInputADO.Time);

                UcDateSetValue(dateInputADO);

                this.isMultiDateState = false;

                if (this.oldServiceReq.USE_TIME != null)
                {
                    UC.DateEditor.ADO.DateInputADO dateInputADO1 = new UC.DateEditor.ADO.DateInputADO();

                    dateInputADO1.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.oldServiceReq.USE_TIME ?? 0).Value;
                    dateInputADO1.Dates = new List<DateTime?>();
                    dateInputADO1.Dates.Add(dateInputADO1.Time);

                    UcDateSetValueUseTime(dateInputADO1);
                }

                if (this.oldServiceReq.USE_TIME == null)
                    this.oldServiceReq.USE_TIME = this.oldServiceReq.INTRUCTION_TIME;

                if (this.oldServiceReq.REMEDY_COUNT.HasValue)
                    this.txtLadder.Text = this.oldServiceReq.REMEDY_COUNT.ToString();
                this.txtAdvise.Text = this.oldServiceReq.ADVISE;

                if (!String.IsNullOrEmpty(this.oldServiceReq.INTERACTION_REASON))
                {
                    this.txtInteractionReason.Enabled = true;
                    this.txtInteractionReason.Text = this.oldServiceReq.INTERACTION_REASON;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.oldExpMest), this.oldExpMest));
                if (this.oldExpMest != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("FillPrescriptionDataToControl. 2");
                    var mediStocks = this.currentMediStock != null ? this.currentMediStock.Where(o => o.MEDI_STOCK_ID == this.oldExpMest.MEDI_STOCK_ID).ToList() : null;
                    if (mediStocks == null || mediStocks.Count == 0)
                    {
                        mediStocks = new List<V_HIS_MEST_ROOM>();
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

                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                    this.currentMediStock.AddRange(mediStocks);
                    Inventec.Common.Logging.LogSystem.Debug("1. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock.Count), currentMediStock.Count));
                    GridCheckMarksSelection gridCheckMark = this.cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("FillPrescriptionDataToControl. 3");
                        gridCheckMark.ClearSelection(this.cboMediStockExport.Properties.View); //ref this.currentMediStock nên phải add lại
                        this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                        this.currentMediStock.AddRange(mediStocks);
                        //Xử lý trường hợp nếu sửa đơn thuốc trong kho, mà kho đó lại chưa được chọn hoặc không có trong danh sách các kho được thiết lập cho phòng đang làm việc thì sẽ lấy kho trong danh mục ra => hiển thị kho của đơn cũ lên giao diện
                        gridCheckMark.SelectAll(this.currentMediStock);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("FillPrescriptionDataToControl. 4");
                        if (this.currentMediStock != null && this.currentMediStock.Count > 0) {
                            InitializeComboMestRoom(currentMediStock);
                            cboMediStockExport.EditValue = this.currentMediStock.First().MEDI_STOCK_ID;
                        }
                    }
                }
                else if (this.serviceReqMain != null && this.serviceReqMain.EXECUTE_ROOM_ID > 0)
                {
                     var mediStocks = this.currentMediStock != null ? this.currentMediStock.Where(o => o.ROOM_ID == this.serviceReqMain.EXECUTE_ROOM_ID).ToList() : null;
                    if (mediStocks == null || mediStocks.Count == 0)
                    {
                        mediStocks = new List<V_HIS_MEST_ROOM>();
                        var tempMedi = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.serviceReqMain.EXECUTE_ROOM_ID);
                        if (tempMedi != null)
                        {
                            V_HIS_MEST_ROOM oldMedi = new V_HIS_MEST_ROOM();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEST_ROOM>(oldMedi, tempMedi);
                            oldMedi.ID = tempMedi.ID;
                            oldMedi.MEDI_STOCK_ID = tempMedi.ID;
                            mediStocks.Add(oldMedi);
                        }
                    }

                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                    this.currentMediStock.AddRange(mediStocks);
                    Inventec.Common.Logging.LogSystem.Debug("1. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock.Count), currentMediStock.Count));
                    GridCheckMarksSelection gridCheckMark = this.cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("FillPrescriptionDataToControl. 3.2");
                        gridCheckMark.ClearSelection(this.cboMediStockExport.Properties.View); //ref this.currentMediStock nên phải add lại
                        this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                        this.currentMediStock.AddRange(mediStocks);
                        //Xử lý trường hợp nếu sửa đơn thuốc trong kho, mà kho đó lại chưa được chọn hoặc không có trong danh sách các kho được thiết lập cho phòng đang làm việc thì sẽ lấy kho trong danh mục ra => hiển thị kho của đơn cũ lên giao diện
                        gridCheckMark.SelectAll(this.currentMediStock);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("FillPrescriptionDataToControl. 4.2");
                        if (this.currentMediStock != null && this.currentMediStock.Count > 0)
                        {
                            InitializeComboMestRoom(currentMediStock);
                            cboMediStockExport.EditValue = this.currentMediStock.First().MEDI_STOCK_ID;
                        }
                    }
                }

                //Inventec.Common.Logging.LogSystem.Debug("2. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock.Count), currentMediStock.Count));
                if (this.oldServiceReq.EXECUTE_ROOM_ID > 0)
                {
                    var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == this.oldServiceReq.EXECUTE_ROOM_ID).FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsAutoCreateSaleExpMest), HisConfigCFG.IsAutoCreateSaleExpMest) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediStock), mediStock));

                    if (HisConfigCFG.IsAutoCreateSaleExpMest && mediStock != null && mediStock.IS_BUSINESS == GlobalVariables.CommonNumberTrue)
                    {
                        if (rdOpionGroup.Properties.Items.Count > 1 && rdOpionGroup.Properties.Items[1].Enabled)
                        {
                            rdOpionGroup.Properties.Items[0].Enabled = false;
                            if (rdOpionGroup.SelectedIndex != 1)
                            {
                                rdOpionGroup.SelectedIndex = 1;
                                this.OpionGroupSelectedChanged();
                                Inventec.Common.Logging.LogSystem.Debug("FillPrescriptionDataToControl. 5.2");
                            }
                        }
                        cboNhaThuoc.Enabled = false;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("3. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock.Count), currentMediStock.Count));

                LoadIcdToControl(this.oldServiceReq.ICD_CODE, this.oldServiceReq.ICD_NAME);
                LoadIcdTranditionalToControl(this.oldServiceReq.TRADITIONAL_ICD_CODE, this.oldServiceReq.TRADITIONAL_ICD_NAME);
                LoadIcdSubTranditionalToControl(this.oldServiceReq.TRADITIONAL_ICD_SUB_CODE, this.oldServiceReq.TRADITIONAL_ICD_TEXT);
                LoadIcdCauseToControl(this.oldServiceReq.ICD_CAUSE_CODE, this.oldServiceReq.ICD_CAUSE_NAME);
                var icdCaus = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == this.oldServiceReq.ICD_CODE);
                if (icdCaus != null)
                {
                    LoadRequiredCause((icdCaus.IS_REQUIRE_CAUSE == 1));
                }
                LoadDataToIcdSub(this.oldServiceReq.ICD_SUB_CODE, this.oldServiceReq.ICD_TEXT);

                var user = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME.ToUpper().Equals(this.oldServiceReq.REQUEST_LOGINNAME.ToUpper()));
                if (user != null)
                {
                    this.cboUser.EditValue = user.LOGINNAME;
                    this.txtLoginName.Text = user.LOGINNAME;
                }
                Inventec.Common.Logging.LogSystem.Debug("FillPrescriptionDataToControl. 6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataForPrescriptionEdit()
        {
            try
            {
                if (this.assignPrescriptionEditADO != null)
                {
                    this.actionType = GlobalVariables.ActionEdit;
                    this.oldExpMest = this.assignPrescriptionEditADO.ExpMest;
                    this.oldExpMestId = (this.oldExpMest != null ? this.oldExpMest.ID : 0);
                    this.oldServiceReq = this.assignPrescriptionEditADO.ServiceReq;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task GetObeyContraindi()
        {
            try
            {
                if (HisConfigCFG.ContraindicaterWarningOption == "1" || HisConfigCFG.ContraindicaterWarningOption == "2")
                {
                    CommonParam param = new CommonParam();
                    HisObeyContraindiFilter filter = new HisObeyContraindiFilter();
                    filter.SERVICE_REQ_CODE__EXACT = oldServiceReq.SERVICE_REQ_CODE;
                    filter.TREATMENT_ID = oldServiceReq.TREATMENT_ID;
                    filter.REQUEST_LOGINNAME__EXACT = oldServiceReq.REQUEST_LOGINNAME;
                    ObeyContraindiEdit = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_OBEY_CONTRAINDI>>("api/HisObeyContraindi/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
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
                    if (this.oldServiceReq == null) throw new ArgumentNullException("ServiceReq");
                    if (this.oldServiceReq.ID == 0) throw new ArgumentNullException("ServiceReq.ID");
                    //if (this.assignPrescriptionEditADO.ExpMest == null) throw new ArgumentNullException("ExpMest");
                    WaitingManager.Show();
                    this.InitWorker();
                    this.idRow = 1;
                    this.gridControlServiceProcess.DataSource = null;
                    this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                    this.actionType = GlobalVariables.ActionEdit;
                    if (this.oldServiceReq != null)
                    {
                        this.isNotProcessWhileChangedTextSubIcd = true;
                        this.FillPrescriptionDataToControl();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock), currentMediStock));
                        this.OpionGroupSelectedChanged();
                        Inventec.Common.Logging.LogSystem.Debug("Sua don thuoc__ Kho thuoc:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentMediStock), this.currentMediStock));
                        paramCommon = new CommonParam();
                        if (oldExpMestId > 0)
                        {
                            //if (this.sereServWithTreatment == null || this.sereServWithTreatment.Count == 0)
                            //{
                            this.sereServsInTreatmentRaw = null;
                            this.LoadDataSereServWithTreatment(this.currentTreatmentWithPatientType, this.InstructionTime);
                            //}
                            Inventec.Common.Logging.LogSystem.Debug("LoadPrescriptionForEdit:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServWithTreatment), sereServWithTreatment));
                            this.expMestMedicineEditPrints = this.GetExpMestMedicineByExpMestId(oldExpMestId);
                            this.ProcessGetExpMestMedicine(this.expMestMedicineEditPrints, true);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("mediMatyTypeADOs.Count", mediMatyTypeADOs.Count));
                            this.expMestMaterialEditPrints = this.GetExpMestMaterialByExpMestId(oldExpMestId);
                            this.ProcessGetExpMestMaterial(this.expMestMaterialEditPrints, true);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("mediMatyTypeADOs.Count", mediMatyTypeADOs.Count));
                        }
                        else
                        {
                            //Trường hợp sửa đơn mà là đơn phụ thì mặc định check chọn thuốc trong kho
                            if (this.serviceReqMain != null && this.serviceReqMain.IS_SUB_PRES == 1)
                            {
                                if (rdOpionGroup.SelectedIndex != 0)
                                {
                                    rdOpionGroup.SelectedIndex = 0;
                                    this.OpionGroupSelectedChanged();
                                }
                            }
                            //Trường hợp sửa đơn mà đơn chỉ có thuốc - vật tư ngoài kho - mua ngoài và không phải là đơn phụ => mặc định check chọn thuốc ngoài kho 
                            else if (rdOpionGroup.Properties.Items.Count > 1 && rdOpionGroup.Properties.Items[1].Enabled)
                            {
                                if (rdOpionGroup.SelectedIndex != 1)
                                {
                                    rdOpionGroup.SelectedIndex = 1;
                                    this.OpionGroupSelectedChanged();
                                }
                            }
                        }

                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => oldServiceReq), oldServiceReq));
                        GetObeyContraindi();
                        this.serviceReqMetys = this.GetServiceReqMetyByServiceReqId(this.oldServiceReq.ID);
                        this.ProcessGetServiceReqMety(this.serviceReqMetys, true);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("mediMatyTypeADOs.Count", mediMatyTypeADOs.Count));
                        this.serviceReqMatys = this.GetServiceReqMatyByServiceReqId(this.oldServiceReq.ID);
                        this.ProcessGetServiceReqMaty(this.serviceReqMatys, true);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("mediMatyTypeADOs.Count", mediMatyTypeADOs.Count));
                        //Nếu tất cả thuốc vật tư kê trước đấy là ngoài kho thì enabled kho
                        isMediMatyIsOutStock = CheckAllMediMatyIsOutStock();
                        if (isMediMatyIsOutStock)
                        {
                            cboMediStockExport.Enabled = true;
                            cboMediStockExport.Properties.Tag = null;
                            cboMediStockExport.Properties.View.OptionsSelection.MultiSelect = false;
                            GridColumn columnCheck = cboMediStockExport.Properties.View.Columns.FirstOrDefault(o => o.FieldName == "CheckMarkSelection");
                            if (columnCheck != null)
                                cboMediStockExport.Properties.View.Columns.Remove(columnCheck);
                        }
                        else
                            cboMediStockExport.Enabled = false;
                        this.ProcessInstructionTimeMediForEdit();
                        this.mediMatyTypeADOBKs = new List<MediMatyTypeADO>();
                        this.mediMatyTypeADOBKs.AddRange(this.mediMatyTypeADOs);
                        this.ProcessMergeDuplicateRowForListProcessing();
                        this.PrescriptionPrevious = true;
                        this.ValidDataMediMaty();
                        this.SetUseDayToPrescriptionEdit();

                        if (CheckMediMatyType(this.mediMatyTypeADOs) == false)
                        {
                            return;
                        }
                        this.RefeshResourceGridMedicine();
                        this.TickAllMediMateAssignPresed();
                        this.SetEnableButtonControl(this.actionType);
                        this.SetTotalPrice__TrongDon();
                        this.idRow = (int)((this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0) ? (this.mediMatyTypeADOs.Max(o => o.NUM_ORDER ?? 0) + stepRow) : 0 + stepRow);
                        this.InstructionTime = this.oldServiceReq.INTRUCTION_TIME;
                        this.LoadMLCT();
                        this.EnableCheckTemporaryPres();
                        if (this.oldServiceReq.TRACKING_ID.HasValue)
                        {
                            cboPhieuDieuTri.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(this.oldServiceReq.TRACKING_ID.Value.ToString());
                            cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
                        }
                        this.spnPresPhaseNum.EditValue = this.oldServiceReq.PRESCRIPTION_PHASE_NUM.HasValue ? (long?)this.oldServiceReq.PRESCRIPTION_PHASE_NUM : null;
                        this.chkTemporayPres.Checked = (this.oldServiceReq.IS_TEMPORARY_PRES == 1);
                        this.chkHomePres.Checked = (this.oldServiceReq.IS_HOME_PRES == 1);
                        this.chkPreKidneyShift.Checked = (this.oldServiceReq.IS_KIDNEY == 1);
                        this.spinKidneyCount.EditValue = this.oldServiceReq.KIDNEY_TIMES;
                        this.txtProvisionalDiagnosis.Text = this.oldServiceReq.PROVISIONAL_DIAGNOSIS;
                        if (this.oldServiceReq.IS_EXECUTE_KIDNEY_PRES == 1)
                        {
                            lciForchkPreKidneyShift.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            lciForspinKidneyCount.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }

                        bool isExistsExpend = this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Any(o => o.SereServParentId > 0) ? true : false;
                        if (HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && isExistsExpend)
                        {
                            this.grcExpend__TabMedicine.Visible = true;
                            this.grcIsExpendType.Visible = true;
                        }
                        if (HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT && isExistsExpend)
                        {
                            this.isAutoCheckExpend = true;
                        }
                        this.isNotProcessWhileChangedTextSubIcd = false;
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageManager.Show(ResourceMessage.SuaDonThuocDuLieuTruyenVaoKhongHopLe);
                Inventec.Common.Logging.LogSystem.Error(ResourceMessage.SuaDonThuocDuLieuTruyenVaoKhongHopLe);
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
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0 && this.oldServiceReq != null && (this.oldServiceReq.USE_TIME > 0 || this.oldServiceReq.INTRUCTION_TIME > 0))
                {
                    foreach (var item in this.mediMatyTypeADOs)
                    {
                        System.DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.UseTimeTo ?? 0).Value;
                        System.DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.oldServiceReq.USE_TIME ?? this.oldServiceReq.INTRUCTION_TIME).Value;
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
                value = (decimal)this.GetValueSpinHasAround(this.spinAmount.Text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }

        internal decimal GetRawAmount()
        {
            decimal value = 0;
            try
            {
                value = this.CalculateRawValueAmount();
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
                    && HIS.Desktop.Plugins.AssignPrescriptionPK.ValidAcinInteractiveWorker.ValidGrade(this.currentMedicineTypeADOForEdit, mediMatyTypeADOs, ref this.txtInteractionReason, this)
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
                MessageManager.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc));
            }
            return valid;
        }
    }
}
