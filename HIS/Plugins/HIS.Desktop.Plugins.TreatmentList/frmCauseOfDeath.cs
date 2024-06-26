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
using HIS.Desktop.Utility;
using HIS.UC.UCCauseOfDeath;
using HIS.UC.UCCauseOfDeath.ADO;
using Inventec.Desktop.Common.LanguageManager;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class frmCauseOfDeath : FormBase
    {
        private UCCauseOfDeathProcessor DeathProcessor;
        UserControl ucDeath;
        CauseOfDeathADO DeathIniADO { get; set; }
        UCCauseOfDeathProcessor causeOfDeathProcessor { get; set; }
        UserControl ucCauseOfDeath { get; set; }
        CauseOfDeathADO causeOfDeathAdo { get; set; }
        bool isDeathDiagnosis { get; set; }
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        public frmCauseOfDeath(CauseOfDeathADO causeOfDeathADO, bool isDeathDiagnosis,Inventec.Desktop.Common.Modules.Module currentModule)
        {
            InitializeComponent();
            try
            {
                this.isDeathDiagnosis = isDeathDiagnosis;
                this.causeOfDeathAdo = causeOfDeathADO;
                this.Size = new Size(1100, 768);
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmCauseOfDeath_Load(object sender, EventArgs e)
        {
            try
            {
                    causeOfDeathProcessor = new UCCauseOfDeathProcessor();
                    ucCauseOfDeath = (UserControl)causeOfDeathProcessor.Run(causeOfDeathAdo);
                    causeOfDeathProcessor.SetValue(ucCauseOfDeath, causeOfDeathAdo);
                    ucCauseOfDeath.Dock = DockStyle.Fill;
                    xtraScrollableControl1.Controls.Clear();
                    xtraScrollableControl1.Controls.Add(this.ucCauseOfDeath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void PrintMps000485()
        {
            try
            {
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate("Mps000485", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000485":
                        ProcessPrintMps000485(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }


        private void ProcessPrintMps000485(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps000485");
                var treatmentId = causeOfDeathAdo.Treatment.ID;
                if (treatmentId > 0)
                {
                    WaitingManager.Show();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = treatmentId;
                    HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                        .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    HIS_SEVERE_ILLNESS_INFO SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    List<HIS_EVENTS_CAUSES_DEATH> lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    HIS_DEPARTMENT_TRAN departmentTran = new HIS_DEPARTMENT_TRAN();
                    HIS_DEPARTMENT department = new HIS_DEPARTMENT();
                    HIS_PATIENT_TYPE_ALTER patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    CommonParam param = new CommonParam();
                    HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                    filter.TREATMENT_ID = treatmentId;
                    var dtSevere = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dtSevere != null && dtSevere.Count > 0)
                    {
                        SevereIllnessInfo = dtSevere.FirstOrDefault(o => o.IS_DEATH == 1);
                        if (SevereIllnessInfo != null)
                        {
                            MOS.Filter.HisDepartmentTranFilter filterDepartmentTran = new HisDepartmentTranFilter();
                            filterDepartmentTran.TREATMENT_ID = treatmentId;
                            filterDepartmentTran.DEPARTMENT_ID = SevereIllnessInfo.DEPARTMENT_ID;
                            var datas = new BackendAdapter(null).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filterDepartmentTran, null);
                            if (datas != null && datas.Count > 0)
                                departmentTran = datas.Last();
                            MOS.Filter.HisDepartmentFilter filterDepartment = new HisDepartmentFilter();
                            filterDepartment.ID = SevereIllnessInfo.DEPARTMENT_ID;
                            var datasDepatment = new BackendAdapter(null).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filterDepartment, null);
                            if (datasDepatment != null && datasDepatment.Count > 0)
                                department = datasDepatment.First();
                            HisEventsCausesDeathFilter filterChild = new HisEventsCausesDeathFilter();
                            filterChild.SEVERE_ILLNESS_INFO_ID = SevereIllnessInfo.ID;
                            lstEvents = new BackendAdapter(param).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, filterChild, param);
                        }
                    }
                    HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                    patientTypeAlterFilter.TREATMENT_ID = treatmentId;
                    var patientTypeAlterData = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                    if (patientTypeAlterData != null && patientTypeAlterData.Count > 0)
                    {
                        patientTypeAlter = patientTypeAlterData[0];
                    }
                    HIS_PATIENT patient = GetPatientByID(treatment.PATIENT_ID);
                    HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    if (SevereIllnessInfo == null)
                        SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    if (lstEvents == null)
                        lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    if (patientTypeAlter == null)
                        patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    if (departmentTran == null)
                        departmentTran = new HIS_DEPARTMENT_TRAN();
                    if (department == null)
                        department = new HIS_DEPARTMENT();
                    List<HIS_ICD> currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                    WaitingManager.Hide();
                    MPS.Processor.Mps000485.PDO.Mps000485PDO pdo = new MPS.Processor.Mps000485.PDO.Mps000485PDO
                        (
                        SevereIllnessInfo,
                        lstEvents,
                        treatment,
                        patientTypeAlter,
                        patient,
                        departmentTran,
                        department,
                        branch,
                        currentIcds,
                        BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().ToList(),
                        BackendDataWorker.Get<HIS_TREATMENT_RESULT>().ToList());
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private HIS_PATIENT GetPatientByID(long id)
        {
            HIS_PATIENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = id;
                result = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            

        }

        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.xtraScrollableControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(682, 445);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(556, 421);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(124, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 5;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click_1);
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Location = new System.Drawing.Point(2, 2);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(678, 415);
            this.xtraScrollableControl1.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(682, 445);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.xtraScrollableControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(682, 419);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnPrint;
            this.layoutControlItem2.Location = new System.Drawing.Point(554, 419);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(128, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 419);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(554, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmCauseOfDeath
            // 
            this.ClientSize = new System.Drawing.Size(682, 445);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmCauseOfDeath";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phiếu chẩn đoán nguyên nhân tử vong";
            this.Load += new System.EventHandler(this.frmCauseOfDeath_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {
            try
            {
                PrintMps000485();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
