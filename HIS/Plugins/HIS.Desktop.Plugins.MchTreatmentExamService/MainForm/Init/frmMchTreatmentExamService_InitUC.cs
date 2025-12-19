using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MchTreatmentExamService.UCAdress;
using HIS.UC.SecondaryIcd;
using HIS.UC.TreeSereServ7;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Init UC and Load

        private void InitUcAddress()
        {
            try
            {
                addressMother = new UCAddress();
                panel2.Controls.Add(addressMother);
                addressMother.Dock = DockStyle.Fill;

                addressBaby = new UCAddress();
                panel1.Controls.Add(addressBaby);
                addressBaby.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SafeInitUc()
        {
            try
            {
                if (treeSereServ7Processor != null)
                {
                    pnSereServ.Controls.Clear();
                }
                treeSereServ7Processor = new TreeSereServ7Processor();
                TreeSereServ7ADO ado = new TreeSereServ7ADO();
                ado.SelectImageCollection = this.imageCollection1;
                ado.StateImageCollection = this.imageCollection1;
                ado.IsNotShowComboFilter = true;
                ado.IsGetOtherData = true;
                ado.TreeSereServ7_GetStateImage = treeSereServ_GetStateImage;
                ado.TreeSereServ7_StateImageClick = treeSereServ_StateImageClick;
                ado.TreeSereServ7_GetSelectImage = treeSereServ_GetSelectImage;
                ado.TreeSereServ7_CustomNodeCellEdit = treeSereServ_CustomNodeCellEdit;
                ado.SereServNodeCellStyle = treeSereServ_NodeCellStyle;
                ado.IsShowSearchPanel = false;
                ado.DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.moduleData.RoomId) != null ? HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.moduleData.RoomId).DepartmentId : 0;
                ado.TreatmentId = Treatment != null ? Treatment.ID : 0;
                ado.TreeSereServ7Columns = new List<TreeSereServ7Column>();

                TreeSereServ7Column serviceBtn = new TreeSereServ7Column("   ", "SendTestServiceReq", 30, true);
                serviceBtn.VisibleIndex = 1;
                ado.TreeSereServ7Columns.Add(serviceBtn);

                TreeSereServ7Column serviceCodeCol = new TreeSereServ7Column("Mã dịch vụ", "TDL_SERVICE_CODE", 150, false);
                serviceCodeCol.VisibleIndex = 2;
                ado.TreeSereServ7Columns.Add(serviceCodeCol);

                TreeSereServ7Column serviceNameCol = new TreeSereServ7Column("Tên dịch vụ", "TDL_SERVICE_NAME", 370, false);
                serviceNameCol.VisibleIndex = 3;
                ado.TreeSereServ7Columns.Add(serviceNameCol);

                TreeSereServ7Column AmountCol = new TreeSereServ7Column("Số lượng", "AMOUNT", 80, false);
                AmountCol.VisibleIndex = 4;
                ado.TreeSereServ7Columns.Add(AmountCol);

                TreeSereServ7Column serviceReqCodeCol = new TreeSereServ7Column("Mã yêu cầu", "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 5;
                ado.TreeSereServ7Columns.Add(serviceReqCodeCol);

                TreeSereServ7Column noteCol = new TreeSereServ7Column("Ghi chú", "NOTE_ADO", 250, false);
                noteCol.VisibleIndex = 6;
                ado.TreeSereServ7Columns.Add(noteCol);

                this.ucSereServ = (UserControl)treeSereServ7Processor.Run(ado);
                if (this.ucSereServ != null)
                {
                    pnSereServ.Controls.Add(ucSereServ);
                    ucSereServ.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcdSecondIcd()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == 1).ToList();
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), data);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.Height = 24;
                ado.TextLblIcd = "Nhóm chẩn đoán:";
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.TextSize = 100;
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panel3.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckEnableByTreatment()
        {
            try
            {
                bool hasExamService = ExamService != null && ExamService.ID > 0;
                bool hasTreatment = Treatment != null && Treatment.ID > 0;
                bool isSearchEnabled = !hasTreatment && !hasExamService;

                txtPatientCode.Enabled = isSearchEnabled;
                txtTreatmentCode.Enabled = isSearchEnabled;
                btnFind.Enabled = isSearchEnabled;

                if (hasTreatment)
                {
                    txtPatientCode.Text = Treatment.TDL_PATIENT_CODE ?? string.Empty;
                    txtTreatmentCode.Text = Treatment.TREATMENT_CODE ?? string.Empty;
                }
                else if (hasExamService)
                {
                    txtPatientCode.Text = ExamService.PATIENT_CODE ?? string.Empty;
                    txtTreatmentCode.Text = ExamService.TREATMENT_CODE ?? string.Empty;
                }
                else
                {
                    txtPatientCode.Text = string.Empty;
                    txtTreatmentCode.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
