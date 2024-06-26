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
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.ApprovalPrescriptionPK.ADO;
using Inventec.Common.Integrate.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.SDO;
using MOS.Filter;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using HIS.Desktop.Plugins.ApprovalPrescriptionPK.Resources;
using DevExpress.Utils.Menu;
using HIS.Desktop.Print;
using System.Threading;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.ApprovalPrescriptionPK.Config;

namespace HIS.Desktop.Plugins.ApprovalPrescriptionPK
{
    public partial class UCApprovalPrescriptionPK : UserControlBase
    {
        #region Declare

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        List<ErrorADO> lstErrorADO = new List<ErrorADO>();
        int Finished = 0;


        List<HisExpMestGroupByTreatmentSDO> lstExpMestGroupByTreatment;
        HisExpMestGroupByTreatmentSDO rightClickData;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string MODULE_LINK = "HIS.Desktop.Plugins.ApprovalPrescriptionPK";

        List<V_HIS_EXP_MEST> ListHisExpMest;
        List<HIS_SERVICE_REQ> ListHisServiceReq;
        List<V_HIS_EXP_MEST_MATERIAL> ListExpMestMaterial;
        List<V_HIS_EXP_MEST_MEDICINE> ListExpMestMedicine;
        List<ADO.PrescriptionADO> listDataGrid;

        List<HoSoADO> lstHoSoADO;
        bool PrintNow = false;

        #endregion

        public UCApprovalPrescriptionPK(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.currentModule = module;
            this.roomId = module.RoomId;
        }

        private void UCApprovalPrescriptionPK_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                HisConfigCFG.LoadConfig();

                InitCombo();

                InitMenuToButtonPrint();
                //khởi tại giá trị mặc định
                SetDefaultValueControl();
                
                InitControlState();

                FillDataToControl();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }


        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "chkInDonThuoc")
                        {
                            chkInDonThuoc.Checked = item.VALUE == "1";
                        }

                        if (item.KEY == "chkInHDSD")
                        {
                            chkInHDSD.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                dtOutTimeFrom.DateTime = DateTime.Now;
                dtOutTimeTo.DateTime = DateTime.Now;

                cboHoSo.EditValue = "1";
                cboDonThuoc.EditValue = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST;

                BtnApproval.Enabled = false;

                lciForbtnErrorDetail.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lblDaXuLy.Text = "";
                lblError.Text = "";
                this.txtExpMestCode.Text = "";

                if (HisConfigCFG.AutoCreateAggrExamExpMest == "1")
                {
                    this.txtExpMestCode.Enabled = true;
                }
                else 
                {
                    this.txtExpMestCode.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }



        #region Nút in
        private void InitMenuToButtonPrint()
        {
            try
            {
                WaitingManager.Show();
                DXPopupMenu menu = new DXPopupMenu();

                DXMenuItem itemInHuongDanSuDung = new DXMenuItem("Hướng dẫn sử dụng", new EventHandler(OnClickInThucXuatThuoc));
                itemInHuongDanSuDung.Tag = "HUONG_DAN_SU_DUNG";
                menu.Items.Add(itemInHuongDanSuDung);

                DXMenuItem itemDonThuocVatTu = new DXMenuItem("Đơn thuốc / vật tư", new EventHandler(OnClickInThucXuatThuoc));
                itemDonThuocVatTu.Tag = "DON_THUOC_VAT_TU";
                menu.Items.Add(itemDonThuocVatTu);

                cboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInThucXuatThuoc(object sender, EventArgs e)
        {
            var bbtnItem = sender as DXMenuItem;
            if (bbtnItem.Tag == "HUONG_DAN_SU_DUNG")
            {
                PrintNow = false;
                ProcessPrintHDSD();
            }
            if (bbtnItem.Tag == "DON_THUOC_VAT_TU")
            {
                ProcessPrint(false);
            }
        }

        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Inventec.Common.SignLibrary.ADO.InputADO();
        string printTypeCode = "";
        string printerName = "";

        private void ProcessPrintHDSD()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, DelegateRunPrinter);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                InHuongDanSuDung(printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InHuongDanSuDung(string printTypeCode, string fileName, ref bool result)
        {

            try
            {
                CommonParam param = new CommonParam();
                string genderName = "";
                long? dob = null;
                if (ListHisExpMest == null || ListHisServiceReq == null) return;

                var listDataExport = new List<V_HIS_EXP_MEST>();
                if (listDataGrid != null && listDataGrid.Count > 0)
                {
                    var cineChecked = listDataGrid.Where(s => s.IsCheck && s.TUTORIAL != null).ToList();
                    Inventec.Common.Logging.LogSystem.Warn("hướng dẫn sử dụng thuốc: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cineChecked), cineChecked));
                    if (cineChecked != null && cineChecked.Count > 0)
                    {
                        var listExecute = this.ListHisExpMest.Where(o => cineChecked.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();

                        foreach (var expMest in listExecute)
                        {
                            //if (expMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            //{
                            //    var listExpChild = this.ListHisExpMest.Where(o => o.AGGR_EXP_MEST_ID == expMest.ID).ToList();
                            //    foreach (var item in listExpChild)
                            //    {
                            //        V_HIS_EXP_MEST exp = new V_HIS_EXP_MEST();
                            //        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(exp, item);
                            //        listDataExport.Add(exp);
                            //    }
                            //}
                            //else
                            //{
                                V_HIS_EXP_MEST exp = new V_HIS_EXP_MEST();
                                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(exp, expMest);
                                listDataExport.Add(exp);
                            //}
                        }
                    }
                }
                if (listDataExport != null && listDataExport.Count > 0)
                {
                    V_HIS_EXP_MEST ExpMests = listDataExport.FirstOrDefault();

                    if (ExpMests.TDL_PATIENT_ID != null)
                    {
                        genderName = ExpMests.TDL_PATIENT_GENDER_NAME;
                        dob = ExpMests.TDL_PATIENT_DOB;
                    }

                    inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((ExpMests != null ? ExpMests.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);

                    List<V_HIS_EXP_MEST_MEDICINE> medicines = new List<V_HIS_EXP_MEST_MEDICINE>();

                    if (ListExpMestMedicine != null && ListExpMestMedicine.Count > 0)
                    {
                        medicines = ListExpMestMedicine.Where(o => listDataExport.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();

                    }

                    List<V_HIS_EXP_MEST_MATERIAL> materials = new List<V_HIS_EXP_MEST_MATERIAL>();
                    if (ListExpMestMaterial != null && ListExpMestMaterial.Count > 0)
                    {
                        materials = ListExpMestMaterial.Where(o => listDataExport.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();

                    }

                    Inventec.Common.Logging.LogSystem.Warn("hướng dẫn sử dụng thuốc2: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicines), medicines));

                    MPS.Processor.Mps000099.PDO.Mps000099PDO pdo = new MPS.Processor.Mps000099.PDO.Mps000099PDO(
                    listDataExport,
                    medicines,
                    materials);
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        if (PrintNow) 
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);

                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrint(bool printNow)
        {
            try
            {
                if (ListHisExpMest == null || ListHisServiceReq == null) return;

                var listDataExport = new List<HIS_EXP_MEST>();
                if (listDataGrid != null && listDataGrid.Count > 0)
                {
                    var cineChecked = listDataGrid.Where(s => s.IsCheck).ToList();
                    if (cineChecked != null && cineChecked.Count > 0)
                    {
                        var listExecute = this.ListHisExpMest.Where(o => cineChecked.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                        foreach (var expMest in listExecute)
                        {
                            HIS_EXP_MEST exp = new HIS_EXP_MEST();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(exp, expMest);
                            listDataExport.Add(exp);
                        }
                    }
                }

                if (listDataExport != null && listDataExport.Count > 0)
                {

                    var lstDataGroup = listDataExport.GroupBy(o => o.TDL_PATIENT_ID);

                    foreach (var iGroup in lstDataGroup)
                    {
                        var ExpMestGroup = iGroup.ToList();

                        List<MOS.SDO.OutPatientPresResultSDO> listOutPatientPresResultSDO = new List<OutPatientPresResultSDO>();
                        MOS.SDO.OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
                        outPatientPresResultSDO.ExpMests = ExpMestGroup;

                        List<HIS_EXP_MEST_MEDICINE> medicines = new List<HIS_EXP_MEST_MEDICINE>();
                        List<HIS_EXP_MEST_MATERIAL> material = new List<HIS_EXP_MEST_MATERIAL>();
                        List<HIS_SERVICE_REQ> ServiceReqs = new List<HIS_SERVICE_REQ>();

                        ServiceReqs = ListHisServiceReq.Where(o => ExpMestGroup.Select(s => s.SERVICE_REQ_ID).Contains(o.ID)).ToList();

                        if (ListExpMestMaterial != null && ListExpMestMaterial.Count > 0)
                        {
                            var lstMaterial = ListExpMestMaterial.Where(o => ExpMestGroup.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                            if (lstMaterial != null && lstMaterial.Count > 0)
                            {
                                foreach (var item in lstMaterial)
                                {
                                    HIS_EXP_MEST_MATERIAL mate = new HIS_EXP_MEST_MATERIAL();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MATERIAL>(mate, item);
                                    material.Add(mate);
                                }
                            }
                        }

                        if (ListExpMestMedicine != null && ListExpMestMedicine.Count > 0)
                        {
                            var lstMedicine = ListExpMestMedicine.Where(o => ExpMestGroup.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                            if (lstMedicine != null && lstMedicine.Count > 0)
                            {
                                foreach (var item in lstMedicine)
                                {
                                    HIS_EXP_MEST_MEDICINE medi = new HIS_EXP_MEST_MEDICINE();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MEDICINE>(medi, item);
                                    medicines.Add(medi);
                                }
                            }
                        }

                        outPatientPresResultSDO.ServiceReqs = ServiceReqs;
                        outPatientPresResultSDO.Medicines = medicines;
                        outPatientPresResultSDO.Materials = material;

                        listOutPatientPresResultSDO.Add(outPatientPresResultSDO);

                        var PrintPresProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(listOutPatientPresResultSDO);
                        PrintPresProcessor.Print("Mps000234", printNow);
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.VuiLongChonPhieuXuat);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                GridPaging();

                listDataGrid = new List<PrescriptionADO>();
                gridControlMedicineDetail.BeginUpdate();
                gridControlMedicineDetail.DataSource = listDataGrid;
                gridControlMedicineDetail.EndUpdate();
                gridViewMedicineDetail.ExpandAllGroups();
                ProcessTotalPrice();
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    gridViewPatient.SelectAll();
                    if ((long)cboDonThuoc.EditValue == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                    {
                        BtnApproval.Enabled = true;
                    }
                }
                else
                {
                    BtnApproval.Enabled = false;
                }
               


                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging()
        {
            try
            {
                lstExpMestGroupByTreatment = new List<HisExpMestGroupByTreatmentSDO>();
                CommonParam paramCommon = new CommonParam();

                HisExpMestGroupByTreatmentFilter filter = new HisExpMestGroupByTreatmentFilter();

                SetFilter(ref filter);

                gridViewPatient.BeginUpdate();

                Inventec.Common.Logging.LogSystem.Info("filter: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                this.lstExpMestGroupByTreatment = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<HisExpMestGroupByTreatmentSDO>>
                    ("api/HisExpMest/GetExpMestGroupByTreatment", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                if (lstExpMestGroupByTreatment != null && lstExpMestGroupByTreatment.Count > 0)
                {
                    gridControlPatient.DataSource = lstExpMestGroupByTreatment;
                    if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        gridViewPatient.SelectAll();
                        if ((long)cboDonThuoc.EditValue == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            BtnApproval.Enabled = true;
                        }
                    }
                }
                else
                {
                    gridControlPatient.DataSource = null;
                }

                gridViewPatient.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void SetFilter(ref HisExpMestGroupByTreatmentFilter filter)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    string code = txtExpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpMestCode.Text = code;
                    }
                    filter.EXP_MEST_CODE__EXACT = code;
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        string code = txtTreatmentCode.Text.Trim();
                        if (code.Length < 12 && checkDigit(code))
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtTreatmentCode.Text = code;
                        }
                        filter.TREATMENT_CODE__EXACT = code;
                    }

                    else if (!string.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        string code = txtPatientCode.Text.Trim();
                        if (code.Length < 10 && checkDigit(code))
                        {
                            code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                            txtPatientCode.Text = code;
                        }
                        filter.PATIENT_CODE__EXACT = code;
                    }

                    if (cboHoSo.EditValue == "1")
                    {
                        filter.TREATMENT_IS_PAUSE = true;
                        filter.TREATMENT_IS_ACTIVE = null;
                    }
                    else if (cboHoSo.EditValue == "2")
                    {
                        filter.TREATMENT_IS_ACTIVE = false;

                    }

                //    filter.EXP_MEST_TYPE_IDs = new List<long>{
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK,
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK
                //};

                    if (cboDonThuoc.EditValue != null)
                    {
                        filter.EXP_MEST_STT_ID = (long)cboDonThuoc.EditValue;
                    }

                    if (dtOutTimeFrom.EditValue != null && dtOutTimeFrom.DateTime != DateTime.MinValue)
                        filter.OUT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtOutTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtOutTimeTo.EditValue != null && dtOutTimeTo.DateTime != DateTime.MinValue)
                        filter.OUT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtOutTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                    filter.KEYWORD = txtKeyWord.Text.Trim();
                    filter.IS_NOT_TAKEN = false;
                    var MediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.roomId);

                    if (MediStock != null && MediStock.ID > 0)
                    {
                        filter.MEDI_STOCK_ID = MediStock.ID;
                    }

                    if (HisConfigCFG.AutoCreateAggrExamExpMest == "1")
                    {
                        filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK;
                    }
                    else
                    {
                        filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        #region Khởi tạo combobox
        private void InitCombo()
        {
            try
            {
                InitComboHoSo();
                InitComboDonThuoc();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void InitComboHoSo()
        {
            try
            {

                lstHoSoADO = new List<HoSoADO>();

                lstHoSoADO.Add(new HoSoADO("1", "Kết thúc điều trị"));
                lstHoSoADO.Add(new HoSoADO("2", "Đã khóa viện phí"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HOSO_NAME", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HOSO_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(this.cboHoSo, lstHoSoADO, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDonThuoc()
        {
            try
            {
                var lstData = BackendDataWorker.Get<HIS_EXP_MEST_STT>().Where(o => (o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE || o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE) && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(p => p.ID).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_STT_NAME", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_STT_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(this.cboDonThuoc, lstData, controlEditorADO);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridViewPatient_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisExpMestGroupByTreatmentSDO data = (HisExpMestGroupByTreatmentSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridViewPatient_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                this.rightClickData = null;
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = gridViewPatient.GetVisibleRowHandle(hi.RowHandle);

                    var rowData = (HisExpMestGroupByTreatmentSDO)gridViewPatient.GetRow(rowHandle);
                    if (rowData != null)
                    {
                        this.rightClickData = rowData;

                        gridViewPatient.OptionsSelection.EnableAppearanceFocusedCell = true;
                        gridViewPatient.OptionsSelection.EnableAppearanceFocusedRow = true;

                        BarManager barManager = new BarManager();
                        barManager.Form = this.ParentForm;


                        PopupMenuProcessor popupMenuProcessor = new PopupMenuProcessor(long.Parse(cboDonThuoc.EditValue.ToString()), rowData, barManager, MouseRight_Click);
                        popupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.rightClickData != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.HuyDuyet:
                            HuyDuyet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.HuyThucXuat:
                            HuyThucXuat(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.InTongHop:
                            InTongHop(this.rightClickData, false);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void InTongHop(HisExpMestGroupByTreatmentSDO row, bool printNow)
        {
            try
            {
                List<HisExpMestGroupByTreatmentSDO> listPatientCheck = new List<HisExpMestGroupByTreatmentSDO>();

                if (gridViewPatient.GetSelectedRows().Count() > 0)
                {

                    var rowHandles = gridViewPatient.GetSelectedRows();
                    if (rowHandles != null && rowHandles.Count() > 0)
                    {
                        foreach (var i in rowHandles)
                        {
                            var rowCheck = (HisExpMestGroupByTreatmentSDO)gridViewPatient.GetRow(i);
                            if (rowCheck != null)
                            {
                                listPatientCheck.Add(rowCheck);
                            }
                        }
                    }

                    //if (listPatientCheck != null && listPatientCheck.Count > 0)
                    //{
                    //    WaitingManager.Show();

                    //    TypeConvert(listPatientCheck, ref lstExpMestIDs);
                    //}
                }
                else if (row != null)
                {
                    listPatientCheck.Add(row);
                    //TypeConvertOneObject(row, ref lstExpMestIDs);
                }

                if (listPatientCheck != null && listPatientCheck.Count > 0)
                {
                    foreach (var itemCheck in listPatientCheck)
                    {
                        List<long> lstExpMestIDs = new List<long>();
                        TypeConvertOneObject(itemCheck, ref lstExpMestIDs);

                        List<V_HIS_EXP_MEST> LstExpMest = new List<V_HIS_EXP_MEST>();
                        List<HIS_SERVICE_REQ> LstServiceReq = new List<HIS_SERVICE_REQ>();
                        List<V_HIS_EXP_MEST_MEDICINE> LstExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                        List<V_HIS_EXP_MEST_MATERIAL> LstExpMestMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();

                        if (lstExpMestIDs != null && lstExpMestIDs.Count > 0)
                        {
                            var paramCommon = new CommonParam();
                            HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                            expMestFilter.AGGR_EXP_MEST_ID__OR__IDs = lstExpMestIDs;
                            var ExpMest = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                            if (ExpMest != null && ExpMest.Count > 0)
                            {
                                LstExpMest.AddRange(ExpMest);
                            }
                        }

                        if (LstExpMest != null && LstExpMest.Count > 0)
                        {
                            var paramCommon = new CommonParam();
                            HisServiceReqFilter filter = new HisServiceReqFilter();
                            filter.IDs = LstExpMest.Select(s => s.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                            var apiResult = new BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                            if (apiResult != null && apiResult.Count > 0)
                            {
                                if (LstServiceReq == null) LstServiceReq = new List<HIS_SERVICE_REQ>();

                                LstServiceReq.AddRange(apiResult);
                            }
                        }

                        if (lstExpMestIDs != null && lstExpMestIDs.Count > 0)
                        {
                            var paramCommon = new CommonParam();
                            HisExpMestMaterialViewFilter mateFilter = new HisExpMestMaterialViewFilter();
                            mateFilter.TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_IDs = lstExpMestIDs;
                            var materials = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                            if (materials != null && materials.Count > 0)
                            {
                                LstExpMestMaterial.AddRange(materials);
                            }
                        }

                        if (lstExpMestIDs != null && lstExpMestIDs.Count > 0)
                        {
                            var paramCommon = new CommonParam();
                            HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                            mediFilter.TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_IDs = lstExpMestIDs;
                            var medicines = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                            if (medicines != null && medicines.Count > 0)
                            {
                                LstExpMestMedicine.AddRange(medicines);
                            }
                        }

                        try
                        {
                            if (LstExpMest == null || LstServiceReq == null) return;

                            var listDataExport = new List<HIS_EXP_MEST>();

                            if (LstExpMest != null && LstExpMest.Count > 0)
                            {
                                foreach (var expMest in LstExpMest)
                                {
                                    HIS_EXP_MEST exp = new HIS_EXP_MEST();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(exp, expMest);
                                    listDataExport.Add(exp);
                                }
                                List<MOS.SDO.OutPatientPresResultSDO> listOutPatientPresResultSDO = new List<OutPatientPresResultSDO>();
                                MOS.SDO.OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
                                outPatientPresResultSDO.ExpMests = listDataExport;

                                List<HIS_EXP_MEST_MEDICINE> medicines = new List<HIS_EXP_MEST_MEDICINE>();
                                List<HIS_EXP_MEST_MATERIAL> material = new List<HIS_EXP_MEST_MATERIAL>();
                                List<HIS_SERVICE_REQ> ServiceReqs = new List<HIS_SERVICE_REQ>();

                                ServiceReqs = LstServiceReq.Where(o => listDataExport.Select(s => s.SERVICE_REQ_ID).Contains(o.ID)).ToList();

                                if (LstExpMestMaterial != null && LstExpMestMaterial.Count > 0)
                                {
                                    var lstMaterial = LstExpMestMaterial.Where(o => listDataExport.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    if (lstMaterial != null && lstMaterial.Count > 0)
                                    {
                                        foreach (var item in lstMaterial)
                                        {
                                            HIS_EXP_MEST_MATERIAL mate = new HIS_EXP_MEST_MATERIAL();
                                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MATERIAL>(mate, item);
                                            material.Add(mate);
                                        }
                                    }
                                }

                                if (LstExpMestMedicine != null && LstExpMestMedicine.Count > 0)
                                {
                                    var lstMedicine = LstExpMestMedicine.Where(o => listDataExport.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    if (lstMedicine != null && lstMedicine.Count > 0)
                                    {
                                        foreach (var item in lstMedicine)
                                        {
                                            HIS_EXP_MEST_MEDICINE medi = new HIS_EXP_MEST_MEDICINE();
                                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MEDICINE>(medi, item);
                                            medicines.Add(medi);
                                        }
                                    }
                                }

                                outPatientPresResultSDO.ServiceReqs = ServiceReqs;
                                outPatientPresResultSDO.Medicines = medicines;
                                outPatientPresResultSDO.Materials = material;

                                listOutPatientPresResultSDO.Add(outPatientPresResultSDO);

                                Inventec.Common.Logging.LogSystem.Info("InTongHop: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listOutPatientPresResultSDO), listOutPatientPresResultSDO));

                                var PrintPresProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(listOutPatientPresResultSDO);
                                PrintPresProcessor.Print("Mps000234", printNow);
                            }


                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.VuiLongChonPhieuXuat);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void HuyThucXuat(HisExpMestGroupByTreatmentSDO row)
        {
            try
            {
                if (row != null)
                {
                    string ExpMestCode = "", PatientName = "";
                    List<V_HIS_EXP_MEST> LstHisExpMest = new List<V_HIS_EXP_MEST>();
                    List<long> lstExpMestIDs = new List<long>();
                    lstErrorADO = new List<ErrorADO>();

                    string PatientCode = "";
                    Finished = 0;

                    TypeConvertOneObject(row, ref lstExpMestIDs);

                    if (lstExpMestIDs != null && lstExpMestIDs.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                        expMestFilter.IDs = lstExpMestIDs;
                        var ExpMest = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (ExpMest != null && ExpMest.Count > 0)
                        {
                            LstHisExpMest.AddRange(ExpMest);
                        }
                    }

                    if (LstHisExpMest != null && LstHisExpMest.Count > 0)
                    {
                        var lstCode = LstHisExpMest.Select(o => o.EXP_MEST_CODE).Distinct().ToList();
                        ExpMestCode = String.Join(",", lstCode);
                        PatientName = LstHisExpMest.FirstOrDefault().TDL_PATIENT_NAME;
                    }

                    if (MessageBox.Show(String.Format(ResourceMessage.BanMuonHuyThucXuat, ExpMestCode, PatientName), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        bool success = true;
                        CommonParam param = new CommonParam();
                        param.Messages = new List<string>();
                        param.BugCodes = new List<string>();

                        foreach (var item in lstExpMestIDs)
                        {
                            bool resultExport = false;
                            CommonParam paramrResult = new CommonParam();

                            if (HisConfigCFG.AutoCreateAggrExamExpMest == "1")
                            {
                                AggrExamUnexportPrescription(item, ref resultExport, ref paramrResult);
                            }
                            else
                            {
                                HuyThucXuatPrescription(item, ref resultExport, ref paramrResult);
                            }

                            success = success & resultExport;
                            if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                            {
                                param.Messages.AddRange(paramrResult.Messages);
                            }
                            if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                            {
                                param.BugCodes.AddRange(paramrResult.BugCodes);
                            }

                            V_HIS_EXP_MEST ExpMest = LstHisExpMest.FirstOrDefault(o => o.ID == item);

                            if (ExpMest != null && PatientCode != ExpMest.TDL_PATIENT_CODE)
                            {
                                PatientCode = ExpMest.TDL_PATIENT_CODE;
                                Finished += 1;
                            }

                            if ((paramrResult.Messages != null && paramrResult.Messages.Count > 0) || (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0))
                            {
                                ErrorADO ado = new ErrorADO();
                                if (ExpMest != null)
                                {
                                    ado.ErrorCode = ExpMest.EXP_MEST_CODE;
                                }

                                if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                                {
                                    ado.ErrorReason = String.Join(",", paramrResult.Messages.Distinct().ToList());
                                }

                                if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                                {
                                    ado.ErrorReason += String.Join(",", paramrResult.BugCodes.Distinct().ToList());
                                }

                                lstErrorADO.Add(ado);
                            }

                            SetStatusDocProcess();

                        }

                        FillDataToControl();
                        WaitingManager.Hide();

                        if (param.Messages != null && param.Messages.Count > 0)
                        {
                            param.Messages = param.Messages.Distinct().ToList();
                        }

                        if (param.BugCodes != null && param.BugCodes.Count > 0)
                        {
                            param.BugCodes = param.BugCodes.Distinct().ToList();
                        }

                        #region Show message
                        if (success)
                        {
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        }
                        else
                        {
                            string Error = String.Join(",", param.Messages.ToList());
                            Inventec.Desktop.Common.Message.MessageManager.Show(String.Join(",", Error));
                        }
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
            }
        }

        private void HuyThucXuatPrescription(long prescriptionId, ref bool success, ref CommonParam param)
        {
            try
            {
                HisExpMestSDO data = new HisExpMestSDO();
                data.ExpMestId = prescriptionId;
                data.ReqRoomId = this.roomId;

                if (gridControlPatient.DataSource != null)
                {
                    var griddata = (List<HisExpMestGroupByTreatmentSDO>)gridControlPatient.DataSource;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unexport", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (apiresul != null)
                    {
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AggrExamUnexportPrescription(long prescriptionId, ref bool success, ref CommonParam param)
        {
            try
            {
                HisExpMestSDO data = new HisExpMestSDO();
                data.ExpMestId = prescriptionId;
                data.ReqRoomId = this.roomId;

                if (gridControlPatient.DataSource != null)
                {
                    var griddata = (List<HisExpMestGroupByTreatmentSDO>)gridControlPatient.DataSource;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(RequestUriStore.HIS_EXP_MEST_AGGR_EXAM_UNEXPORT, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (apiresul != null)
                    {
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HuyDuyet(HisExpMestGroupByTreatmentSDO row)
        {
            try
            {
                if (row != null)
                {
                    string ExpMestCode = "", PatientName = "";
                    List<V_HIS_EXP_MEST> LstHisExpMest = new List<V_HIS_EXP_MEST>();
                    List<long> lstExpMestIDs = new List<long>();
                    lstErrorADO = new List<ErrorADO>();

                    string PatientCode = "";
                    Finished = 0;

                    TypeConvertOneObject(row, ref lstExpMestIDs);

                    if (lstExpMestIDs != null && lstExpMestIDs.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                        expMestFilter.IDs = lstExpMestIDs;
                        var ExpMest = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (ExpMest != null && ExpMest.Count > 0)
                        {
                            LstHisExpMest.AddRange(ExpMest);
                        }
                    }

                    if (LstHisExpMest != null && LstHisExpMest.Count > 0)
                    {
                        var lstCode = LstHisExpMest.Select(o => o.EXP_MEST_CODE).Distinct().ToList();
                        ExpMestCode = String.Join(",",lstCode);
                        PatientName = LstHisExpMest.FirstOrDefault().TDL_PATIENT_NAME;
                    }

                    if (MessageBox.Show(String.Format(ResourceMessage.BanMuonHuyDuyetPhieu, ExpMestCode, PatientName), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        bool success = true;
                        CommonParam param = new CommonParam();
                        param.Messages = new List<string>();
                        param.BugCodes = new List<string>();

                        foreach (var item in lstExpMestIDs)
                        {
                            bool resultExport = false;
                            CommonParam paramrResult = new CommonParam();

                            if (HisConfigCFG.AutoCreateAggrExamExpMest == "1")
                            {
                                AggrExamUnapprovePrescription(item, ref resultExport, ref paramrResult);
                            }
                            else
                            {
                                HuyDuyetPrescription(item, ref resultExport, ref paramrResult);
                            }
                            success = success & resultExport;
                            if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                            {
                                param.Messages.AddRange(paramrResult.Messages);
                            }
                            if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                            {
                                param.BugCodes.AddRange(paramrResult.BugCodes);
                            }

                            V_HIS_EXP_MEST ExpMest = LstHisExpMest.FirstOrDefault(o => o.ID == item);

                            if (ExpMest != null && PatientCode != ExpMest.TDL_PATIENT_CODE)
                            {
                                PatientCode = ExpMest.TDL_PATIENT_CODE;
                                Finished += 1;
                            }

                            if ((paramrResult.Messages != null && paramrResult.Messages.Count > 0) || (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0))
                            {
                                ErrorADO ado = new ErrorADO();
                                if (ExpMest != null)
                                {
                                    ado.ErrorCode = ExpMest.EXP_MEST_CODE;
                                }

                                if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                                {
                                    ado.ErrorReason = String.Join(",", paramrResult.Messages.Distinct().ToList());
                                }

                                if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                                {
                                    ado.ErrorReason += String.Join(",", paramrResult.BugCodes.Distinct().ToList());
                                }

                                lstErrorADO.Add(ado);
                            }

                            SetStatusDocProcess();
                        }
                        FillDataToControl();
                        WaitingManager.Hide();

                        if (param.Messages != null && param.Messages.Count > 0)
                        {
                            param.Messages = param.Messages.Distinct().ToList();
                        }

                        if (param.BugCodes != null && param.BugCodes.Count > 0)
                        {
                            param.BugCodes = param.BugCodes.Distinct().ToList();
                        }

                        #region Show message
                        if (success)
                        {
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        }
                        else
                        {
                            string Error = String.Join(",", param.Messages.ToList());
                            Inventec.Desktop.Common.Message.MessageManager.Show(String.Join(",", Error));
                        }
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
            }
        }

        private void HuyDuyetPrescription(long prescriptionId, ref bool success, ref CommonParam param)
        {
            try
            {
                HisExpMestSDO data = new HisExpMestSDO();
                data.ExpMestId = prescriptionId;
                data.ReqRoomId = this.roomId;
                if (gridControlPatient.DataSource != null)
                {
                    var griddata = (List<HisExpMestGroupByTreatmentSDO>)gridControlPatient.DataSource;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (apiresul != null)
                    {
                        success = true;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void AggrExamUnapprovePrescription(long prescriptionId, ref bool success, ref CommonParam param)
        {
            try
            {
                HisExpMestSDO data = new HisExpMestSDO();
                data.ExpMestId = prescriptionId;
                data.ReqRoomId = this.roomId;
                if (gridControlPatient.DataSource != null)
                {
                    var griddata = (List<HisExpMestGroupByTreatmentSDO>)gridControlPatient.DataSource;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(RequestUriStore.HIS_EXP_MEST_AGGR_EXAM_UNAPPROVE, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (apiresul != null)
                    {
                        success = true;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPatient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (gridViewPatient.GetSelectedRows().Count() > 0)
                {
                    if ((long)cboDonThuoc.EditValue == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                    {
                        BtnApproval.Enabled = true;
                    }
                    LoadGridMedicineDetail();
                }
                else 
                {
                    BtnApproval.Enabled = false;
                    listDataGrid = new List<PrescriptionADO>();
                    gridControlMedicineDetail.BeginUpdate();
                    gridControlMedicineDetail.DataSource = listDataGrid;
                    gridControlMedicineDetail.EndUpdate();
                    gridViewMedicineDetail.ExpandAllGroups();
                    ProcessTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void LoadGridMedicineDetail()
        {
            try
            {
                if (gridViewPatient.GetSelectedRows().Count() > 0)
                {
                    List<HisExpMestGroupByTreatmentSDO> listPatientCheck = new List<HisExpMestGroupByTreatmentSDO>();
                    var rowHandles = gridViewPatient.GetSelectedRows();
                    if (rowHandles != null && rowHandles.Count() > 0)
                    {
                        foreach (var i in rowHandles)
                        {
                            var row = (HisExpMestGroupByTreatmentSDO)gridViewPatient.GetRow(i);
                            if (row != null)
                            {
                                listPatientCheck.Add(row);
                            }
                        }
                    }

                    if (listPatientCheck != null && listPatientCheck.Count > 0)
                    {
                        WaitingManager.Show();
                        List<long> lstExpMestIDs = new List<long>();

                        TypeConvert(listPatientCheck, ref lstExpMestIDs);

                        ListHisExpMest = new List<V_HIS_EXP_MEST>();
                        ListHisServiceReq = new List<HIS_SERVICE_REQ>();
                        ListExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                        ListExpMestMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();

                        LoadExpMest(lstExpMestIDs);

                        CreateThreadLoadData(lstExpMestIDs, ListHisExpMest);

                        if ((ListExpMestMedicine == null || ListExpMestMedicine.Count <= 0) && (ListExpMestMaterial == null || ListExpMestMaterial.Count <= 0))
                        {
                            return;
                        }

                        List<ADO.PrescriptionADO> ado = ProcessDataGrid(ListExpMestMedicine, ListExpMestMaterial);
                        WaitingManager.Hide();
                        if (ado != null && ado.Count > 0)
                        {
                            ado = ado.OrderBy(o => o.EXP_MEST_CODE).ToList();

                            FillDataToGrid(ado);

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMest(List<long> data)
        {
            try
            {
                if (data != null && data.Count > 0)
                {
                    var paramCommon = new CommonParam();
                    HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                    expMestFilter.AGGR_EXP_MEST_ID__OR__IDs = data;
                    var ExpMest = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (ExpMest != null && ExpMest.Count > 0)
                    {
                        ListHisExpMest = ExpMest;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void FillDataToGrid(List<ADO.PrescriptionADO> data)
        {
            try
            {
                listDataGrid = new List<ADO.PrescriptionADO>();

                if (data != null)
                {
                    listDataGrid.AddRange(data);
                }

                listDataGrid = listDataGrid.OrderBy(o => o.type).ThenBy(o => o.NUM_ORDER ?? 99999).ThenBy(o => o.DETAIL_NAME).ToList();

                gridControlMedicineDetail.BeginUpdate();
                gridControlMedicineDetail.DataSource = listDataGrid;
                gridControlMedicineDetail.EndUpdate();
                gridViewMedicineDetail.ExpandAllGroups();
                ProcessTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessTotalPrice()
        {
            try
            {
                if (listDataGrid != null && listDataGrid.Count > 0)
                {
                    var selectData = listDataGrid.Where(o => o.IsCheck).ToList();
                    if (selectData != null && selectData.Count > 0)
                    {
                        lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(selectData.Sum(o => o.TOTAL_PRICE), ConfigApplications.NumberSeperator);
                    }
                    else
                    {
                        lblTotalPrice.Text = "0";
                    }
                }
                else
                {
                    lblTotalPrice.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<PrescriptionADO> ProcessDataGrid(List<V_HIS_EXP_MEST_MEDICINE> ListExpMestMedicine, List<V_HIS_EXP_MEST_MATERIAL> ListExpMestMaterial)
        {
            List<ADO.PrescriptionADO> result = new List<ADO.PrescriptionADO>();
            try
            {
                if (ListExpMestMedicine != null && ListExpMestMedicine.Count > 0)
                {
                    var expMestMetyGroups = ListExpMestMedicine.GroupBy(o => new { o.TDL_MEDICINE_TYPE_ID, o.EXP_MEST_ID, o.EXP_MEST_CODE, o.PRICE, o.TDL_SERVICE_REQ_ID }).ToList();
                    foreach (var item in expMestMetyGroups)
                    {
                        ADO.PrescriptionADO pres = new ADO.PrescriptionADO(item.First());

                        pres.EXP_MEST_CODE = item.Key.EXP_MEST_CODE;

                        pres.type = 1;
                        pres.NUM_ORDER = item.First().NUM_ORDER ?? 999999;
                        pres.AMOUNT = item.Sum(o => o.AMOUNT);
                        pres.TOTAL_PRICE = ((pres.PRICE ?? 0) * pres.AMOUNT * (1 + (pres.VAT_RATIO ?? 0))) - (pres.DISCOUNT ?? 0);

                        pres.DETAIL_NAME = item.First().MEDICINE_TYPE_NAME;
                        pres.DETAIL_CODE = item.First().MEDICINE_TYPE_CODE;

                        var serviceReq = ListHisServiceReq.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_REQ_ID);
                        if (serviceReq != null)
                        {
                            pres.SERVICE_REQ_CODE = serviceReq.SERVICE_REQ_CODE;
                            pres.INTRUCTION_TIME = serviceReq.INTRUCTION_TIME;
                            pres.INTRUCTION_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.INTRUCTION_TIME);
                            pres.REQUEST_LOGINNAME = serviceReq.REQUEST_LOGINNAME;
                            pres.REQUEST_USERNAME = serviceReq.REQUEST_USERNAME;
                            var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                            if (room != null)
                            {
                                pres.REQUEST_ROOM_NAME = room.ROOM_NAME;
                            }
                        }

                        result.Add(pres);
                    }
                }

                if (ListExpMestMaterial != null && ListExpMestMaterial.Count > 0)
                {
                    var expMestMatyGroups = ListExpMestMaterial.GroupBy(o => new { o.TDL_MATERIAL_TYPE_ID, o.EXP_MEST_ID, o.EXP_MEST_CODE, o.PRICE, o.TDL_SERVICE_REQ_ID }).ToList();
                    foreach (var item in expMestMatyGroups)
                    {
                        ADO.PrescriptionADO pres = new ADO.PrescriptionADO(item.First());

                        ////gán bằng phiếu tổng hợp để duyệt và thực xuất trên phiếu tổng hợp
                        //if (item.First().AGGR_EXP_MEST_ID.HasValue)
                        //{
                        //    pres.EXP_MEST_ID = item.First().AGGR_EXP_MEST_ID;
                        //}

                        pres.EXP_MEST_CODE = item.Key.EXP_MEST_CODE;

                        pres.type = 2;
                        pres.NUM_ORDER = item.First().NUM_ORDER ?? 999999;
                        pres.AMOUNT = item.Sum(o => o.AMOUNT);
                        pres.TOTAL_PRICE = ((pres.PRICE ?? 0) * pres.AMOUNT * (1 + (pres.VAT_RATIO ?? 0))) - (pres.DISCOUNT ?? 0);

                        pres.DETAIL_CODE = item.First().MATERIAL_TYPE_CODE;
                        pres.DETAIL_NAME = item.First().MATERIAL_TYPE_NAME;

                        var serviceReq = ListHisServiceReq.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_REQ_ID);
                        if (serviceReq != null)
                        {
                            pres.SERVICE_REQ_CODE = serviceReq.SERVICE_REQ_CODE;
                            pres.INTRUCTION_TIME = serviceReq.INTRUCTION_TIME;
                            pres.INTRUCTION_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.INTRUCTION_TIME);
                            pres.REQUEST_LOGINNAME = serviceReq.REQUEST_LOGINNAME;
                            pres.REQUEST_USERNAME = serviceReq.REQUEST_USERNAME;
                            var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                            if (room != null)
                            {
                                pres.REQUEST_ROOM_NAME = room.ROOM_NAME;
                            }
                        }

                        result.Add(pres);
                    }
                }

            }
            catch (Exception ex)
            {
                result = new List<ADO.PrescriptionADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void CreateThreadLoadData(List<long> lstExpMestIDs, List<V_HIS_EXP_MEST> lstExpMest)
        {
            Thread serviceReq = new Thread(ThreadLoadServiceReq);
            Thread Material = new Thread(ThreadLoadMaterial);
            Thread Medicine = new Thread(ThreadLoadMedicine);

            //Material.Priority = ThreadPriority.Highest;
            //Medicine.Priority = ThreadPriority.Highest;
            try
            {
                serviceReq.Start(lstExpMest);
                Material.Start(lstExpMestIDs);
                Medicine.Start(lstExpMestIDs);

                serviceReq.Join();
                Material.Join();
                Medicine.Join();
            }
            catch (Exception ex)
            {
                serviceReq.Abort();
                Material.Abort();
                Medicine.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadServiceReq(object obj)
        {
            try
            {
                if (obj != null && obj.GetType() == typeof(List<V_HIS_EXP_MEST>))
                {
                    var data = (List<V_HIS_EXP_MEST>)obj;
                    if (data != null && data.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisServiceReqFilter filter = new HisServiceReqFilter();
                        filter.IDs = data.Select(s => s.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                        var apiResult = new BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            if (ListHisServiceReq == null) ListHisServiceReq = new List<HIS_SERVICE_REQ>();

                            ListHisServiceReq.AddRange(apiResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadMaterial(object obj)
        {
            try
            {
                if (obj != null && obj.GetType() == typeof(List<long>))
                {
                    var data = (List<long>)obj;
                    if (data != null && data.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisExpMestMaterialViewFilter mateFilter = new HisExpMestMaterialViewFilter();
                        //mateFilter.EXP_MEST_IDs = data;
                        mateFilter.TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_IDs = data;
                        var materials = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (materials != null && materials.Count > 0)
                        {
                            ListExpMestMaterial.AddRange(materials);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadMedicine(object obj)
        {
            try
            {
                if (obj != null && obj.GetType() == typeof(List<long>))
                {
                    var data = (List<long>)obj;
                    if (data != null && data.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                        //mediFilter.EXP_MEST_IDs = data;
                        mediFilter.TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_IDs = data;
                        var medicines = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (medicines != null && medicines.Count > 0)
                        {
                            ListExpMestMedicine.AddRange(medicines);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region chuyển list<string> => list<long>
        private void TypeConvert(List<HisExpMestGroupByTreatmentSDO> lstInput, ref List<long> lstOutput)
        {
            try
            {
                if (lstInput != null && lstInput.Count > 0)
                {
                    var lstString = lstInput.Select(o => o.EXP_MEST_IDS).ToList();

                    var lstStr = String.Join(",", lstString);

                    var lstStr1 = lstStr.Split(',').ToList();

                    foreach (var item in lstStr1)
                    {
                        lstOutput.Add(Inventec.Common.TypeConvert.Parse.ToInt64(item));
                    }
                    lstOutput.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void TypeConvertOneObject(HisExpMestGroupByTreatmentSDO Input, ref List<long> lstOutput)
        {
            try
            {
                if (Input != null && !String.IsNullOrEmpty(Input.EXP_MEST_IDS))
                {
                    var lstStr1 = Input.EXP_MEST_IDS.Split(',').ToList();

                    foreach (var item in lstStr1)
                    {
                        lstOutput.Add(Inventec.Common.TypeConvert.Parse.ToInt64(item));
                    }
                    lstOutput.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridViewMedicineDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.PrescriptionADO dataRow = (ADO.PrescriptionADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.VAT_RATIO ?? 0) * 100, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.IMP_VAT_RATIO * 100, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXP_MEST_ID_STR")
                        {
                            try
                            {
                                e.Value = string.Format("{0} - Mã y lệnh: {1} - Ngày y lệnh: {2} - Phòng khám: {3} - Người kê đơn: {4}({5})", dataRow.EXP_MEST_CODE, dataRow.SERVICE_REQ_CODE, dataRow.INTRUCTION_TIME_STR, dataRow.REQUEST_ROOM_NAME, dataRow.REQUEST_USERNAME, dataRow.REQUEST_LOGINNAME);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                
                lciForbtnErrorDetail.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lblDaXuLy.Text = "";
                lblError.Text = "";

                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void chkInDonThuoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "chkInDonThuoc" && o.MODULE_LINK == MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInDonThuoc.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "chkInDonThuoc";
                    csAddOrUpdate.VALUE = (chkInDonThuoc.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkInHDSD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "chkInHDSD" && o.MODULE_LINK == MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInDonThuoc.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "chkInHDSD";
                    csAddOrUpdate.VALUE = (chkInDonThuoc.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnApproval_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnApproval.Enabled) return;


                if (gridViewPatient.GetSelectedRows().Count() > 0)
                {
                    List<HisExpMestGroupByTreatmentSDO> listPatientCheck = new List<HisExpMestGroupByTreatmentSDO>();
                    var rowHandles = gridViewPatient.GetSelectedRows();
                    if (rowHandles != null && rowHandles.Count() > 0)
                    {
                        foreach (var i in rowHandles)
                        {
                            var row = (HisExpMestGroupByTreatmentSDO)gridViewPatient.GetRow(i);
                            if (row != null)
                            {
                                listPatientCheck.Add(row);
                            }
                        }
                    }

                    if (HisConfigCFG.AutoCreateAggrExamExpMest == "1")
                    {
                        if (ProcessAggrExamApprove(listPatientCheck))
                        {
                            if (chkInDonThuoc.Checked)
                            {
                                InTongHop(null, true);
                            }

                            FillDataToControl();
                        }
                    }
                    else
                    {
                        if (ProcessApproval(listPatientCheck))
                        {
                            if (chkInDonThuoc.Checked)
                            {
                                InTongHop(null, true);
                            }

                            FillDataToControl();
                        }
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.VuiLongChonPhieuXuat);
                    Inventec.Common.Logging.LogSystem.Error("listDataExport null");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ApprovalPrescription(long prescriptionID, ref bool success, ref CommonParam param)
        {
            try
            {
                if (prescriptionID > 0)
                {
                    HisExpMestResultSDO apiResult = null;
                    WaitingManager.Show();

                    MOS.SDO.HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                    hisExpMestApproveSDO.ExpMestId = prescriptionID;
                    hisExpMestApproveSDO.IsFinish = true;
                    hisExpMestApproveSDO.ReqRoomId = this.roomId;

                        apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(RequestUriStore.HIS_EXP_MEST_APPROVA, ApiConsumer.ApiConsumers.MosConsumer, hisExpMestApproveSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        if (apiResult != null)
                        {
                            success = true;
                        }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private bool ProcessApproval(List<HisExpMestGroupByTreatmentSDO> listPatientCheck)
        {
            bool result = false;
            try
            {
                if (listPatientCheck != null && listPatientCheck.Count > 0)
                {
                    WaitingManager.Show();
                    List<long> lstExpMestIDs = new List<long>();
                    List<V_HIS_EXP_MEST> lstExpMest = new List<V_HIS_EXP_MEST>();
                    lstErrorADO = new List<ErrorADO>();

                    string PatientCode = "";
                    Finished = 0;

                    TypeConvert(listPatientCheck, ref lstExpMestIDs);

                    if (lstExpMestIDs != null && lstExpMestIDs.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                        expMestFilter.IDs = lstExpMestIDs;
                        var ExpMests = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (ExpMests != null && ExpMests.Count > 0)
                        {
                            lstExpMest = ExpMests;
                        }

                        bool success = true;
                        CommonParam param = new CommonParam();
                        param.Messages = new List<string>();
                        param.BugCodes = new List<string>();

                        foreach (var item in lstExpMestIDs)
                        {
                            bool resultExport = false;
                            CommonParam paramrResult = new CommonParam();
                            ApprovalPrescription(item, ref resultExport, ref paramrResult);

                            V_HIS_EXP_MEST ExpMest = lstExpMest.FirstOrDefault(o => o.ID == item);

                            if (ExpMest != null && PatientCode != ExpMest.TDL_PATIENT_CODE)
                            {
                                PatientCode = ExpMest.TDL_PATIENT_CODE;
                                Finished += 1;
                            }

                            if ((paramrResult.Messages != null && paramrResult.Messages.Count > 0) || (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0))
                            {
                                ErrorADO ado = new ErrorADO();
                                if (ExpMest != null)
                                {
                                    ado.ErrorCode = ExpMest.EXP_MEST_CODE;
                                }

                                if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                                {
                                    ado.ErrorReason = String.Join(",", paramrResult.Messages.Distinct().ToList());
                                }

                                if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                                {
                                    ado.ErrorReason += String.Join(",", paramrResult.BugCodes.Distinct().ToList());
                                }

                                lstErrorADO.Add(ado);
                            }

                            result = success & resultExport;


                            SetStatusDocProcess();
                        }

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, result);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void AggrExamApprovePrescription(long prescriptionID, ref bool success, ref CommonParam param)
        {
            try
            {
                if (prescriptionID > 0)
                {
                    List<HIS_EXP_MEST> apiResult = new List<HIS_EXP_MEST>();
                    WaitingManager.Show();

                    HisExpMestSDO hisExpMestApproveSDO = new HisExpMestSDO();

                    hisExpMestApproveSDO.ExpMestId = prescriptionID;
                    hisExpMestApproveSDO.ReqRoomId = this.roomId;

                    apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_EXP_MEST>>(RequestUriStore.HIS_EXP_MEST_AGGR_EXAM_APPROVE, ApiConsumer.ApiConsumers.MosConsumer, hisExpMestApproveSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (apiResult != null && apiResult.Count > 0)
                    {
                        success = true;
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private bool ProcessAggrExamApprove(List<HisExpMestGroupByTreatmentSDO> listPatientCheck)
        {
            bool result = false;
            try
            {
                if (listPatientCheck != null && listPatientCheck.Count > 0)
                {
                    WaitingManager.Show();
                    List<long> lstExpMestIDs = new List<long>();
                    List<V_HIS_EXP_MEST> lstExpMest = new List<V_HIS_EXP_MEST>();
                    lstErrorADO = new List<ErrorADO>();

                    string PatientCode = "";
                    this.Finished = 0;

                    TypeConvert(listPatientCheck, ref lstExpMestIDs);

                    if (lstExpMestIDs != null && lstExpMestIDs.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                        expMestFilter.IDs = lstExpMestIDs;
                        var ExpMests = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (ExpMests != null && ExpMests.Count > 0)
                        {
                            lstExpMest = ExpMests;
                        }

                        bool success = true;
                        CommonParam param = new CommonParam();
                        param.Messages = new List<string>();
                        param.BugCodes = new List<string>();

                        foreach (var item in lstExpMestIDs)
                        {
                            bool resultExport = false;
                            CommonParam paramrResult = new CommonParam();
                            AggrExamApprovePrescription(item, ref resultExport, ref paramrResult);

                            if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                            {
                                param.Messages.AddRange(paramrResult.Messages);
                            }
                            if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                            {
                                param.BugCodes.AddRange(paramrResult.BugCodes);
                            }

                            V_HIS_EXP_MEST ExpMest = lstExpMest.FirstOrDefault(o => o.ID == item);

                            if (ExpMest != null && PatientCode != ExpMest.TDL_PATIENT_CODE)
                            {
                                PatientCode = ExpMest.TDL_PATIENT_CODE;
                                Finished += 1;
                            }

                            if ((paramrResult.Messages != null && paramrResult.Messages.Count > 0) || (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0))
                            {
                                ErrorADO ado = new ErrorADO();
                                if (ExpMest != null)
                                {
                                    ado.ErrorCode = ExpMest.EXP_MEST_CODE;
                                }

                                if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                                {
                                    ado.ErrorReason = String.Join(",", paramrResult.Messages.Distinct().ToList());
                                }

                                if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                                {
                                    ado.ErrorReason += String.Join(",", paramrResult.BugCodes.Distinct().ToList());
                                }

                                lstErrorADO.Add(ado);
                            }

                            result = success & resultExport;


                            SetStatusDocProcess();
                        }

                        if 
                            (param.Messages != null && param.Messages.Count > 0)
                        {
                            param.Messages = param.Messages.Distinct().ToList();
                        }

                        if (param.BugCodes != null && param.BugCodes.Count > 0)
                        {
                            param.BugCodes = param.BugCodes.Distinct().ToList();
                        }

                        #region Show message
                        if (result)
                        {
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, result);
                        }
                        else 
                        {
                            string Error = String.Join(",", param.Messages.ToList());
                            Inventec.Desktop.Common.Message.MessageManager.Show(String.Join(",", Error));
                        }
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnExported_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExported.Enabled) return;

                if (HisConfigCFG.AutoCreateAggrExamExpMest == "1")
                {
                    if (gridViewPatient.GetSelectedRows().Count() > 0)
                    {
                        List<HisExpMestGroupByTreatmentSDO> listPatientCheck = new List<HisExpMestGroupByTreatmentSDO>();
                        var rowHandles = gridViewPatient.GetSelectedRows();
                        if (rowHandles != null && rowHandles.Count() > 0)
                        {
                            foreach (var i in rowHandles)
                            {
                                var row = (HisExpMestGroupByTreatmentSDO)gridViewPatient.GetRow(i);
                                if (row != null)
                                {
                                    listPatientCheck.Add(row);
                                }
                            }
                        }

                        if (ProcessAggrExamExport(listPatientCheck))
                        {
                            if (chkInHDSD.Checked)
                            {
                                PrintNow = true;
                                ProcessPrintHDSD();

                            }
                            FillDataToControl();
                        }
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.VuiLongChonBenhNhan);
                        Inventec.Common.Logging.LogSystem.Error("gridViewPatient.GetSelectedRows() null");
                        WaitingManager.Hide();
                    }
                }
                else
                {
                    List<ADO.PrescriptionADO> lstData = new List<PrescriptionADO>();
                    lstData = listDataGrid.Where(s => s.IsCheck).ToList();
                    if (lstData != null && lstData.Count > 0)
                    {
                        if (ProcessExport(lstData))
                        {
                            if (chkInHDSD.Checked)
                            {
                                PrintNow = true;
                                ProcessPrintHDSD();

                            }
                            //LoadGridMedicineDetail();
                            FillDataToControl();
                        }
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.VuiLongChonPhieuXuat);
                        Inventec.Common.Logging.LogSystem.Error("listDataExport null");
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessExport(List<ADO.PrescriptionADO> lstData)
        {
            bool result = false;
            try
            {

                if (lstData != null && lstData.Count > 0)
                    {
                        WaitingManager.Show();
                        List<long> lstExpMestIDs = new List<long>();
                        List<V_HIS_EXP_MEST> lstExpMest = new List<V_HIS_EXP_MEST>();
                        lstErrorADO = new List<ErrorADO>();
                        string PatientCode = "";
                        this.Finished = 0;

                        lstExpMestIDs = lstData.Select(o => o.EXP_MEST_ID ?? 0).Distinct().ToList();
                        //TypeConvert(listPatientCheck, ref lstExpMestIDs);

                        if (lstExpMestIDs != null && lstExpMestIDs.Count > 0)
                        {
                            var paramCommon = new CommonParam();
                            HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                            expMestFilter.IDs = lstExpMestIDs;
                            var ExpMests = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                            if (ExpMests != null && ExpMests.Count > 0)
                            {
                                lstExpMest = ExpMests;
                            }

                            bool success = true;
                            CommonParam param = new CommonParam();
                            param.Messages = new List<string>();
                            param.BugCodes = new List<string>();

                            foreach (var item in lstExpMestIDs)
                            {
                                bool resultExport = false;
                                CommonParam paramrResult = new CommonParam();
                                ExportPrescription(item, ref resultExport, ref paramrResult);

                                V_HIS_EXP_MEST ExpMest = lstExpMest.FirstOrDefault(o => o.ID == item);

                                if (ExpMest != null && PatientCode != ExpMest.EXP_MEST_CODE)
                                {
                                    PatientCode = ExpMest.EXP_MEST_CODE;
                                    Finished += 1;
                                }

                                success = success & resultExport;
                                if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                                {
                                    param.Messages.AddRange(paramrResult.Messages);
                                }
                                if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                                {
                                    param.BugCodes.AddRange(paramrResult.BugCodes);
                                }

                                if ((param.Messages != null && param.Messages.Count > 0) || (param.BugCodes != null && param.BugCodes.Count > 0))
                                {
                                    ErrorADO ado = new ErrorADO();
                                    if (ExpMest != null)
                                    {
                                        ado.ErrorCode = ExpMest.EXP_MEST_CODE;
                                    }

                                    if (param.Messages != null && param.Messages.Count > 0)
                                    {
                                        ado.ErrorReason = String.Join(",", param.Messages.Distinct().ToList());
                                    }

                                    if (param.BugCodes != null && param.BugCodes.Count > 0)
                                    {
                                        ado.ErrorReason += String.Join(",", param.BugCodes.Distinct().ToList());
                                    }

                                    lstErrorADO.Add(ado);
                                }

                                SetStatusExpMestProcess();
                            }

                            if (param.Messages != null && param.Messages.Count > 0)
                            {
                                param.Messages = param.Messages.Distinct().ToList();
                            }

                            if (param.BugCodes != null && param.BugCodes.Count > 0)
                            {
                                param.BugCodes = param.BugCodes.Distinct().ToList();
                            }
                            result = success;
                            WaitingManager.Hide();

                            #region Show message
                            if (result)
                            {
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, result);
                            }
                            else
                            {
                                string Error = String.Join(",", param.Messages.ToList());
                                Inventec.Desktop.Common.Message.MessageManager.Show(String.Join(",", Error));
                            }
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ExportPrescription(long prescriptionId, ref bool success, ref CommonParam param)
        {
            try
            {
                if (prescriptionId != null && prescriptionId > 0)
                {

                    MOS.SDO.HisExpMestExportSDO sdo = new MOS.SDO.HisExpMestExportSDO();
                    sdo.ExpMestId = prescriptionId;
                    sdo.ReqRoomId = this.roomId;
                    sdo.IsFinish = true;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                        (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresult != null)
                    {
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private bool ProcessAggrExamExport(List<HisExpMestGroupByTreatmentSDO> listPatientCheck)
        {
            bool result = false;
            try
            {

                if (listPatientCheck != null && listPatientCheck.Count > 0)
                {
                    WaitingManager.Show();
                    List<long> lstExpMestIDs = new List<long>();
                    List<V_HIS_EXP_MEST> lstExpMest = new List<V_HIS_EXP_MEST>();
                    lstErrorADO = new List<ErrorADO>();
                    string PatientCode = "";
                    this.Finished = 0;

                    TypeConvert(listPatientCheck, ref lstExpMestIDs);
                    
                    if (lstExpMestIDs != null && lstExpMestIDs.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                        expMestFilter.IDs = lstExpMestIDs;
                        var ExpMests = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (ExpMests != null && ExpMests.Count > 0)
                        {
                            lstExpMest = ExpMests;
                        }

                        bool success = true;
                        CommonParam param = new CommonParam();
                        param.Messages = new List<string>();
                        param.BugCodes = new List<string>();

                        foreach (var item in lstExpMestIDs)
                        {
                            bool resultExport = false;
                            CommonParam paramrResult = new CommonParam();
                            AggrExamExportPrescription(item, ref resultExport, ref paramrResult);

                            V_HIS_EXP_MEST ExpMest = lstExpMest.FirstOrDefault(o => o.ID == item);

                            if (ExpMest != null && PatientCode != ExpMest.TDL_PATIENT_CODE)
                            {
                                PatientCode = ExpMest.TDL_PATIENT_CODE;
                                Finished += 1;
                            }

                            success = success & resultExport;
                            if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                            {
                                param.Messages.AddRange(paramrResult.Messages);
                            }
                            if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                            {
                                param.BugCodes.AddRange(paramrResult.BugCodes);
                            }


                         if ((param.Messages != null && param.Messages.Count > 0) || (param.BugCodes != null && param.BugCodes.Count > 0))
                            {
                                ErrorADO ado = new ErrorADO();
                                if (ExpMest != null)
                                {
                                    ado.ErrorCode = ExpMest.EXP_MEST_CODE;
                                }
                             
                                if (param.Messages != null && param.Messages.Count > 0)
                                {
                                    ado.ErrorReason = String.Join(", ", param.Messages.Distinct().ToList());
                                }
                            
                                if (param.BugCodes != null && param.BugCodes.Count > 0)
                                {
                                    ado.ErrorReason += String.Join(", ", param.BugCodes.Distinct().ToList());
                                }

                                lstErrorADO.Add(ado);
                            }

                            SetStatusDocProcess();
                        }

                        if (param.Messages != null && param.Messages.Count > 0)
                        {
                            param.Messages = param.Messages.Distinct().ToList();
                        }

                        if (param.BugCodes != null && param.BugCodes.Count > 0)
                        {
                            param.BugCodes = param.BugCodes.Distinct().ToList();
                        }
                        result = success;
                        WaitingManager.Hide();

                        #region Show message
                        if (result)
                        {
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, result);
                        }
                        else
                        {
                            string Error = String.Join(",", param.Messages.ToList());
                            Inventec.Desktop.Common.Message.MessageManager.Show(String.Join(", ", Error));
                        }
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void AggrExamExportPrescription(long prescriptionId, ref bool success, ref CommonParam param)
        {
            try
            {
                if (prescriptionId != null && prescriptionId > 0)
                {

                    HisExpMestSDO sdo = new HisExpMestSDO();
                    sdo.ExpMestId = prescriptionId;
                    sdo.ReqRoomId = this.roomId;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                        (RequestUriStore.HIS_EXP_MEST_AGGR_EXAM_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresult != null)
                    {
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessUncheck()
        {
            try
            {
                foreach (var item in listDataGrid)
                {
                    if (item.IsCheck)
                    {
                        item.IsCheck = false;
                    }
                }

                gridControlMedicineDetail.RefreshDataSource();
                ProcessTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheck_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ADO.PrescriptionADO)gridViewMedicineDetail.GetFocusedRow();
                if (row != null && listDataGrid != null)
                {
                    var checkedit = (DevExpress.XtraEditors.CheckEdit)sender;
                    foreach (var item in listDataGrid)
                    {
                        if (item.EXP_MEST_ID == row.EXP_MEST_ID)
                        {
                            item.IsCheck = checkedit.Checked;
                        }
                    }

                    gridControlMedicineDetail.RefreshDataSource();
                    ProcessTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null,null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void bBtnApproval_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                BtnApproval_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnExported_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                btnExported_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnErrror_Click(object sender, EventArgs e)
        {
            try
            {
                frmError frm = new frmError(this.currentModule, lstErrorADO);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        void SetStatusDocProcess()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        lciForbtnErrorDetail.Visibility = (lstErrorADO != null && lstErrorADO.Count > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                        lblDaXuLy.Text = "Đã xử lý: " + Finished + "/" + gridViewPatient.GetSelectedRows().Count();
                        if (this.lstErrorADO != null && this.lstErrorADO.Count > 0)
                        {
                            lblError.Text = "Lỗi: " + (this.lstErrorADO != null ? this.lstErrorADO.Count : 0) + "";
                        }
                    }));
                }
                else
                {
                    lciForbtnErrorDetail.Visibility = (lstErrorADO != null && lstErrorADO.Count > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lblDaXuLy.Text = "Đã xử lý: " + Finished + "/" + gridViewPatient.GetSelectedRows().Count();
                    if (this.lstErrorADO != null && this.lstErrorADO.Count > 0)
                    {
                        lblError.Text = "Lỗi: " + (this.lstErrorADO != null ? this.lstErrorADO.Count : 0) + "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetStatusExpMestProcess()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        lciForbtnErrorDetail.Visibility = (lstErrorADO != null && lstErrorADO.Count > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                        lblDaXuLy.Text = "Đã xử lý: " + Finished + "/" + listDataGrid.Where(s => s.IsCheck).GroupBy(o => o.EXP_MEST_ID).Count();
                        if (this.lstErrorADO != null && this.lstErrorADO.Count > 0)
                        {
                            lblError.Text = "Lỗi: " + (this.lstErrorADO != null ? this.lstErrorADO.Count : 0) + "";
                        }
                    }));
                }
                else
                {
                    lciForbtnErrorDetail.Visibility = (lstErrorADO != null && lstErrorADO.Count > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lblDaXuLy.Text = "Đã xử lý: " + Finished + "/" + listDataGrid.Where(s => s.IsCheck).GroupBy(o => o.EXP_MEST_ID).Count();
                    if (this.lstErrorADO != null && this.lstErrorADO.Count > 0)
                    {
                        lblError.Text = "Lỗi: " + (this.lstErrorADO != null ? this.lstErrorADO.Count : 0) + "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPatient_Click(object sender, EventArgs e)
        {
            try
            {
                List<HisExpMestGroupByTreatmentSDO> listPatientCheck = new List<HisExpMestGroupByTreatmentSDO>();

                var rowData = (HisExpMestGroupByTreatmentSDO)gridViewPatient.GetFocusedRow();

                LoadGridMedicineDetailClick(rowData);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void LoadGridMedicineDetailClick(HisExpMestGroupByTreatmentSDO row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();
                    List<long> lstExpMestIDs = new List<long>();

                    TypeConvertOneObject(row, ref lstExpMestIDs);

                    ListHisExpMest = new List<V_HIS_EXP_MEST>();
                    ListHisServiceReq = new List<HIS_SERVICE_REQ>();
                    ListExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                    ListExpMestMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();

                    LoadExpMest(lstExpMestIDs);

                    CreateThreadLoadData(lstExpMestIDs, ListHisExpMest);

                    if ((ListExpMestMedicine == null || ListExpMestMedicine.Count <= 0) && (ListExpMestMaterial == null || ListExpMestMaterial.Count <= 0))
                    {
                        return;
                    }

                    List<ADO.PrescriptionADO> ado = ProcessDataGrid(ListExpMestMedicine, ListExpMestMaterial);
                    WaitingManager.Hide();
                    if (ado != null && ado.Count > 0)
                    {
                        ado = ado.OrderBy(o => o.EXP_MEST_CODE).ToList();

                        FillDataToGrid(ado);

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHoSo_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHoSo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void cboDonThuoc_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if ((long)cboDonThuoc.EditValue == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && gridViewPatient.GetSelectedRows().Count() > 0)
                {
                    BtnApproval.Enabled = true;
                }
                else 
                {
                    BtnApproval.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
