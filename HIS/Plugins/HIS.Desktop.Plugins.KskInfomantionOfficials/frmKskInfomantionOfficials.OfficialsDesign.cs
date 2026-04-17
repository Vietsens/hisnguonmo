using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials
{
    partial class frmKskInfomantionOfficials
    {
        private bool officialsDesignInitialized;
        private List<V_HIS_DISEASE_DETAIL> diseaseDetails = new List<V_HIS_DISEASE_DETAIL>();
        private List<HIS_DISEASE_DETAIL_RESULT> diseaseResults = new List<HIS_DISEASE_DETAIL_RESULT>();

        // Mapping: DISEASE_DETAIL_ID -> CheckEdit control
        private Dictionary<long, CheckEdit> diseaseCheckMapping = new Dictionary<long, CheckEdit>();
        // Mapping: DISEASE_DETAIL_ID -> TextEdit control (for IS_OTHER fields)
        private Dictionary<long, Control> diseaseTextMapping = new Dictionary<long, Control>();

        // Grid data: PARENT_TYPE=3 (Tien su gia dinh) hien thi tren gridView48
        private List<ADO.DiseaseDetailGridADO> diseaseGridParent3 = new List<ADO.DiseaseDetailGridADO>();
        // Grid data: PARENT_TYPE=4 (Trieu chung co nang) hien thi tren gridView50
        private List<ADO.DiseaseDetailGridADO> diseaseGridParent4 = new List<ADO.DiseaseDetailGridADO>();
        // Grid data: PARENT_TYPE=5 (Trieu chung dau) hien thi tren gridView49
        private List<ADO.DiseaseDetailGridADO> diseaseGridParent5 = new List<ADO.DiseaseDetailGridADO>();

        private void InitializeOfficialsDesignIfNeeded()
        {
            try
            {
                if (officialsDesignInitialized)
                    return;

                ApplyHiddenTabOption();

                // Hide old panels if they still exist in designer
                if (panelControl4 != null) panelControl4.Visible = false;
                if (panelControl1 != null) panelControl1.Visible = false;
                if (panelControl2 != null) panelControl2.Visible = false;

                InitNewTabEvents();
                InitDoctorCombos();
                LoadDiseaseDefinitionData();
                Inventec.Common.Logging.LogSystem.Debug("KskOfficials: diseaseDetails.Count = " + (diseaseDetails != null ? diseaseDetails.Count.ToString() : "null"));
                GenerateDynamicDiseaseControls();
                Inventec.Common.Logging.LogSystem.Debug("KskOfficials: diseaseCheckMapping.Count = " + diseaseCheckMapping.Count + ", diseaseTextMapping.Count = " + diseaseTextMapping.Count);

                // Flag chỉ set khi đã build mappings thành công, tránh trường hợp
                // exception ở các bước đầu làm mappings rỗng mãi
                officialsDesignInitialized = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #region Init
        private void InitNewTabEvents()
        {
            try
            {
                // BMI auto-calculation
                spHeight.EditValueChanged += (s, e) => RecalcBmi();
                spWeight.EditValueChanged += (s, e) => RecalcBmi();

                // Choose subclinical result button
                btnChonKQ.Click += (s, e) => btnChonKQ_ClickHandler(s, e);

                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDoctorCombos()
        {
            try
            {
                var acsUsers = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                controlEditorADO.ImmediatePopup = true;

                // Internal medicine doctor
                ControlEditorLoader.Load(cboExamCirculationLoginName, acsUsers, controlEditorADO);
                // Surgery doctor
                ControlEditorLoader.Load(cboExamSurgeryLoginName, acsUsers, controlEditorADO);
                // Obstetric doctor
                ControlEditorLoader.Load(cboExamObstetricLoginName, acsUsers, controlEditorADO);
                // Dermatology doctor
                ControlEditorLoader.Load(cboExamDermatologyLoginName, acsUsers, controlEditorADO);
                // Stomatology doctor
                ControlEditorLoader.Load(cboExamStomatologyLoginName, acsUsers, controlEditorADO);
                // ENT doctor
                ControlEditorLoader.Load(cboExamENTLoginName, acsUsers, controlEditorADO);
                // Eye doctor
                ControlEditorLoader.Load(cboExamEyeLoginName, acsUsers, controlEditorADO);
                // Other exam doctor
                ControlEditorLoader.Load(cboExamOtherLoginName, acsUsers, controlEditorADO);

                // Conclusion doctor (GridLookUpEdit với 2 cột Mã + Tên)
                ControlEditorLoader.Load(cboExamConcluderLoginName, acsUsers, controlEditorADO);

                // Gắn event Delete button cho tất cả combo bác sĩ
                var doctorCombos = new GridLookUpEdit[]
                {
                    cboExamCirculationLoginName, cboExamSurgeryLoginName,
                    cboExamObstetricLoginName, cboExamDermatologyLoginName,
                    cboExamStomatologyLoginName, cboExamENTLoginName,
                    cboExamEyeLoginName, cboExamOtherLoginName,
                    cboExamConcluderLoginName
                };
                foreach (var cbo in doctorCombos)
                {
                    if (cbo != null)
                        cbo.Properties.ButtonClick += cboDoctor_Properties_ButtonClick;
                }

                // Health exam rank
                InitHealthExamRankCombo(cboHealthExamRank);

                // Conclusion time
                InitConclusionTimeCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitHealthExamRankCombo(GridLookUpEdit cbo)
        {
            try
            {
                var ranks = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>();
                if (ranks != null && cbo != null)
                {
                    cbo.Properties.DataSource = ranks;
                    cbo.Properties.DisplayMember = "HEALTH_EXAM_RANK_NAME";
                    cbo.Properties.ValueMember = "ID";
                    cbo.Properties.View.Columns.Clear();
                    var col = cbo.Properties.View.Columns.AddField("HEALTH_EXAM_RANK_NAME");
                    col.VisibleIndex = 0;
                    col.Width = 200;
                    col.Caption = "Phân loại";
                    cbo.Properties.PopupFormWidth = 250;
                    cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitConclusionTimeCombo()
        {
            try
            {
                if (cboConclusionTime != null)
                {
                    cboConclusionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    cboConclusionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    cboConclusionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    cboConclusionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    cboConclusionTime.Properties.Mask.EditMask = "dd/MM/yyyy";
                    cboConclusionTime.Properties.Mask.UseMaskAsDisplayFormat = true;
                    cboConclusionTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDoctor_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var editor = sender as GridLookUpEdit;
                    if (editor != null)
                        editor.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChonKQ_ClickHandler(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module mod = GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (mod == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                }
                else if (mod.IsPlugin && mod.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatmentId);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)delegate (object data)
                    {
                        if (data != null && data is string)
                        {
                            txtSuclinicalResult.Text = data as string;
                        }
                    });
                    listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(mod, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(
                        HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(mod, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RecalcBmi()
        {
            try
            {
                if (spHeight.EditValue != null && spWeight.EditValue != null
                    && spHeight.Value > 0 && spWeight.Value > 0)
                {
                    decimal heightM = spHeight.Value / 100m;
                    decimal bmi = spWeight.Value / (heightM * heightM);
                    lblBMIToanThan.Text = Math.Round(bmi, 2).ToString();
                }
                else
                {
                    lblBMIToanThan.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Disease Detail Mapping
        private void LoadDiseaseDefinitionData()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDiseaseDetailViewFilter filter = new HisDiseaseDetailViewFilter();
                var data = new BackendAdapter(param).Get<List<V_HIS_DISEASE_DETAIL>>(HisRequestUriStore.MOS_V_HIS_DISEASE_DETAIL_GET, ApiConsumers.MosConsumer, filter, param);
                diseaseDetails = data ?? new List<V_HIS_DISEASE_DETAIL>();
                Inventec.Common.Logging.LogSystem.Debug("LoadDiseaseDefinitionData - diseaseDetails.Count=" + diseaseDetails.Count);
            }
            catch (Exception ex)
            {
                diseaseDetails = new List<V_HIS_DISEASE_DETAIL>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDiseaseResults(long? kskGeneralId)
        {
            try
            {
                diseaseResults = new List<HIS_DISEASE_DETAIL_RESULT>();
                if (kskGeneralId == null || kskGeneralId <= 0)
                    return;
                CommonParam param = new CommonParam();
                var filter = new { KSK_GENERAL_ID = kskGeneralId };
                var data = new BackendAdapter(param).Get<List<HIS_DISEASE_DETAIL_RESULT>>(HisRequestUriStore.MOS_HIS_DISEASE_DETAIL_RESULT_GET, ApiConsumers.MosConsumer, filter, param);
                // Filter đúng KSK_GENERAL_ID + dedup theo DISEASE_DETAIL_ID (giữ bản mới nhất)
                diseaseResults = (data ?? new List<HIS_DISEASE_DETAIL_RESULT>())
                    .Where(r => r.DISEASE_DETAIL_ID != null && r.KSK_GENERAL_ID == kskGeneralId)
                    .GroupBy(r => r.DISEASE_DETAIL_ID)
                    .Select(g => g.OrderByDescending(r => r.ID).First())
                    .ToList();
                Inventec.Common.Logging.LogSystem.Debug("LoadDiseaseResults: raw=" + (data != null ? data.Count : 0) + " filtered+dedup=" + diseaseResults.Count);
            }
            catch (Exception ex)
            {
                diseaseResults = new List<HIS_DISEASE_DETAIL_RESULT>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gen dong disease controls tu V_HIS_DISEASE_DETAIL.
        /// - PARENT_TYPE=1 (Thoi quen sinh hoat) → xtraScrollableControl2
        /// - PARENT_TYPE=2 (Tien su benh ban than) → xtraScrollableControl3
        /// - PARENT_TYPE=3 (Tien su gia dinh) → gridView48 / gridControl1
        /// </summary>
        private void GenerateDynamicDiseaseControls()
        {
            try
            {
                if (diseaseDetails == null) return;

                // PARENT_TYPE = 1: Thoi quen sinh hoat → xtraScrollableControl2
                if (xtraScrollableControl2 != null)
                {
                    UC.ucDiseaseDetailContainer.Generate(
                        xtraScrollableControl2,
                        diseaseDetails,
                        1,
                        diseaseCheckMapping,
                        diseaseTextMapping);
                }

                // PARENT_TYPE = 2: Tien su benh ban than → xtraScrollableControl3
                if (xtraScrollableControl3 != null)
                {
                    UC.ucDiseaseDetailContainer.Generate(
                        xtraScrollableControl3,
                        diseaseDetails,
                        2,
                        diseaseCheckMapping,
                        diseaseTextMapping);
                }

                // PARENT_TYPE = 3: Tien su gia dinh → gridView48 / gridControl1
                if (gridView48 != null && gridControl1 != null)
                {
                    UC.DiseaseDetailGridHelper.SetupGridView(gridView48);
                    diseaseGridParent3 = UC.DiseaseDetailGridHelper.LoadToGrid(
                        gridControl1, gridView48, diseaseDetails, 3);
                }

                // PARENT_TYPE = 4: Trieu chung co nang → gridView50 / gridControl3
                if (gridView50 != null && gridControl3 != null)
                {
                    UC.DiseaseDetailGridHelper.SetupGridView(gridView50);
                    diseaseGridParent4 = UC.DiseaseDetailGridHelper.LoadToGrid(
                        gridControl3, gridView50, diseaseDetails, 4);
                }

                // PARENT_TYPE = 5: Trieu chung dau → gridView49 / gridControl2
                if (gridView49 != null && gridControl2 != null)
                {
                    UC.DiseaseDetailGridHelper.SetupGridView(gridView49);
                    diseaseGridParent5 = UC.DiseaseDetailGridHelper.LoadToGrid(
                        gridControl2, gridView49, diseaseDetails, 5);
                }

                Inventec.Common.Logging.LogSystem.Debug(
                    "GenerateDynamicDiseaseControls done. "
                    + "checkMapping=" + diseaseCheckMapping.Count
                    + " textMapping=" + diseaseTextMapping.Count
                    + " gridParent3=" + diseaseGridParent3.Count
                    + " gridParent4=" + diseaseGridParent4.Count
                    + " gridParent5=" + diseaseGridParent5.Count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Reset toan bo trang thai disease cho tat ca 5 PARENT_TYPE.
        /// Goi TRUOC khi load data moi — dam bao khong con data cu khi chuyen row.
        /// </summary>
        private void ResetAllDiseaseStates()
        {
            try
            {
                // PARENT_TYPE 1,2: UC gen dong — reset qua dictionary
                foreach (var kv in diseaseCheckMapping)
                    kv.Value.Checked = false;
                foreach (var kv in diseaseTextMapping)
                    kv.Value.Text = string.Empty;

                // PARENT_TYPE 3,4,5: Grid — reset qua helper
                UC.DiseaseDetailGridHelper.ResetAll(diseaseGridParent3, gridView48);
                UC.DiseaseDetailGridHelper.ResetAll(diseaseGridParent4, gridView50);
                UC.DiseaseDetailGridHelper.ResetAll(diseaseGridParent5, gridView49);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Load Data
        private void LoadOfficialsFormData(ADO.ServiceReqADO data)
        {
            try
            {
                // Clear tat ca truoc khi load moi — dam bao khong con data cu khi chuyen row
                ResetAllDiseaseStates();

                LoadDiseaseResults(data != null && data.KSK_GENERAL != null ? (long?)data.KSK_GENERAL.ID : null);

                var general = data != null ? data.KSK_GENERAL : null;
                var dhst = general != null ? general.HIS_DHST : null;

                // --- Tab 1: Tiền sử bệnh ---
                // Load disease detail checkbox states
                LoadDiseaseCheckStates();

                // Other history
                txtMoneyWithMediFood.Text = ReadString(general, "HISTORY_ALLERGY");
                txtTreatmentMedi.Text = ReadString(general, "HISTORY_DISEASE");
                txtProceduresAndSurgeriesPerformed.Text = ReadString(general, "HISTORY_SURGERY");
                txtDiseasesAndMedicalNow.Text = ReadString(general, "MEDICINE_USING");

                // --- Tab 2: Khám lâm sàng ---
                // Physical
                spHeight.EditValue = ReadDecimal(dhst, "HEIGHT");
                spWeight.EditValue = ReadDecimal(dhst, "WEIGHT");
                RecalcBmi();

                // Body
                txtDaNiemMac.Text = ReadString(general, "BODY_SKIN");
                txtLuoiHoiTho.Text = ReadString(general, "BODY_TONGUE");
                txtTinhThan.Text = ReadString(general, "BODY_SPIRIT");
                txtHachNgoaiVi.Text = ReadString(general, "BODY_LYMPH");
                txtToanThanKhac.Text = ReadString(general, "BODY_OTHER");

                // Internal medicine - Cardiovascular
                cboExamCirculationLoginName.EditValue = ReadString(general, "EXAM_CIRCULATION_LOGINNAME");
                txtHeartRhythm.Text = ReadString(dhst, "HEART_RHYTHM");
                txtHeartRate.Text = ReadString(dhst, "HEART_RATE");
                txtHeartBeat.Text = ReadString(dhst, "HEART_BEAT");
                txtBloodPressureMax.Text = ReadString(dhst, "BLOOD_PRESSURE_MAX");
                txtBloodPressureMin.Text = ReadString(dhst, "BLOOD_PRESSURE_MIN");
                txtOtherExamCirculation.Text = ReadString(general, "EXAM_CIRCULATION");

                // Blood vessels
                txtArtery.Text = ReadString(general, "ARTERY");
                txtVein.Text = ReadString(general, "VEIN");

                // Respiratory
                txtChest.Text = ReadString(general, "CHEST");
                txtBreathRate.Text = ReadString(dhst, "BREATH_RATE");
                txtLung.Text = ReadString(general, "LUNG");

                // Abdominal & specialties
                txtExamDigestion.Text = ReadString(general, "EXAM_DIGESTION");
                txtExamKidneyUrology.Text = ReadString(general, "EXAM_KIDNEY_UROLOGY");
                txtExamMuscleBone.Text = ReadString(general, "EXAM_MUSCLE_BONE");
                txtExamOEND.Text = ReadString(general, "EXAM_OEND");
                txtExamNeurological.Text = ReadString(general, "EXAM_NEUROLOGICAL");

                // Surgery
                cboExamSurgeryLoginName.EditValue = ReadString(general, "EXAM_SURGERY_LOGINNAME");
                txtExamSurgery.Text = ReadString(general, "EXAM_SURGERY");

                // Obstetric
                cboExamObstetricLoginName.EditValue = ReadString(general, "EXAM_OBSTETRIC_LOGINNAME");
                txtExamObstetric.Text = ReadString(general, "EXAM_OBSTETRIC");

                // Dermatology
                cboExamDermatologyLoginName.EditValue = ReadString(general, "EXAM_DERMATOLOGY_LOGINNAME");
                txtExamDermatology.Text = ReadString(general, "EXAM_DERMATOLOGY");

                // Stomatology
                cboExamStomatologyLoginName.EditValue = ReadString(general, "EXAM_STOMATOLOGY_LOGINNAME");
                txtExamStomatologyUpper.Text = ReadString(general, "EXAM_STOMATOLOGY_UPPER");
                txtExamStomatologyLower.Text = ReadString(general, "EXAM_STOMATOLOGY_LOWER");
                txtExamStomatology.Text = ReadString(general, "EXAM_STOMATOLOGY");

                // ENT
                cboExamENTLoginName.EditValue = ReadString(general, "EXAM_ENT_LOGINNAME");
                txtExamENTDisease.Text = ReadString(general, "EXAM_ENT_DISEASE");
                txtExamENTLeftNormal.Text = ReadString(general, "EXAM_ENT_LEFT_NORMAL");
                txtExamENTLeftWhisper.Text = ReadString(general, "EXAM_ENT_LEFT_WHISPER");
                txtExamENTRightNormal.Text = ReadString(general, "EXAM_ENT_RIGHT_NORMAL");
                txtExamENTRightWhisper.Text = ReadString(general, "EXAM_ENT_RIGHT_WHISPER");
                txtTestENTLeft.Text = ReadString(general, "TEST_ENT_LEFT");
                txtTestENTRight.Text = ReadString(general, "TEST_ENT_RIGHT");
                txtEndoscopyENT.Text = ReadString(general, "ENDOSCOPY_ENT");
                txtExamENT.Text = ReadString(general, "EXAM_ENT");

                // Eye
                cboExamEyeLoginName.EditValue = ReadString(general, "EXAM_EYE_LOGINNAME");
                txtExamEyeSightRight.Text = ReadString(general, "EXAM_EYESIGHT_RIGHT");
                txtExamEyeSightLeft.Text = ReadString(general, "EXAM_EYESIGHT_LEFT");
                txtExamEyeSightGlassRight.Text = ReadString(general, "EXAM_EYESIGHT_GLASS_RIGHT");
                txtExamEyeSightGlassLeft.Text = ReadString(general, "EXAM_EYESIGHT_GLASS_LEFT");
                txtIntraocularPressure.Text = ReadString(general, "INTRAOCULAR_PRESSURE");
                txtLens.Text = ReadString(general, "LENS");
                txtFundoscopy.Text = ReadString(general, "FUNDOSCOPY");
                txtExamEye.Text = ReadString(general, "EXAM_EYE");

                // Other exam
                cboExamOtherLoginName.EditValue = ReadString(general, "EXAM_OTHER_LOGINNAME");
                txtExamOther.Text = ReadString(general, "EXAM_OTHER");

                // --- Tab 3: Kết luận ---
                txtSuclinicalResult.Text = ReadString(general, "RESULT_SUBCLINICAL");
                txtDiseases.Text = ReadString(general, "DISEASES");
                txtTreatmentInstruction.Text = ReadString(general, "TREATMENT_INSTRUCTION");
                cboHealthExamRank.EditValue = ReadNullableLong(general, "HEALTH_EXAM_RANK_ID");

                var conclusionTime = ReadNullableLong(general, "CONCLUSION_TIME");
                if (cboConclusionTime != null)
                {
                    if (conclusionTime.HasValue)
                        cboConclusionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(conclusionTime.Value) ?? DateTime.Now;
                    else
                        cboConclusionTime.DateTime = DateTime.Now;
                }

                var concluderLoginName = ReadString(general, "CONCLUDER_LOGINNAME");
                if (cboExamConcluderLoginName != null && !string.IsNullOrEmpty(concluderLoginName))
                {
                    cboExamConcluderLoginName.EditValue = concluderLoginName;
                }

                // Pathological history
                memoPathologicalHistory.Text = ReadString(general, "PATHOLOGICAL_HISTORY");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDiseaseCheckStates()
        {
            try
            {
                // Reset all checkboxes
                foreach (var kv in diseaseCheckMapping)
                {
                    kv.Value.Checked = false;
                }
                foreach (var kv in diseaseTextMapping)
                {
                    kv.Value.Text = string.Empty;
                }

                // Apply saved results
                var sbLoad = new System.Text.StringBuilder();
                sbLoad.AppendLine("LoadDiseaseCheckStates: diseaseResults.Count=" + diseaseResults.Count);
                foreach (var result in diseaseResults)
                {
                    if (result.DISEASE_DETAIL_ID == null) continue;
                    long detailId = result.DISEASE_DETAIL_ID.Value;
                    bool isCheck = (result.IS_CHECK ?? 0) == 1;
                    if (diseaseCheckMapping.ContainsKey(detailId))
                    {
                        var chk = diseaseCheckMapping[detailId];
                        if (isCheck)
                        {
                            sbLoad.AppendLine(string.Format("  SET CHECKED: detailId={0}, IS_CHECK={1} -> {2} [{3}]",
                                detailId, result.IS_CHECK, chk.Name, chk.Properties.Caption));
                        }
                        chk.Checked = isCheck;
                    }
                    if (diseaseTextMapping.ContainsKey(detailId))
                    {
                        diseaseTextMapping[detailId].Text = result.OTHER ?? string.Empty;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(sbLoad.ToString());

                // PARENT_TYPE=3: Apply results len gridView48
                UC.DiseaseDetailGridHelper.ApplyResults(gridView48, diseaseGridParent3, diseaseResults);
                // PARENT_TYPE=4: Apply results len gridView50
                UC.DiseaseDetailGridHelper.ApplyResults(gridView50, diseaseGridParent4, diseaseResults);
                // PARENT_TYPE=5: Apply results len gridView49
                UC.DiseaseDetailGridHelper.ApplyResults(gridView49, diseaseGridParent5, diseaseResults);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Reset Data
        private void ResetOfficialsFormData()
        {
            try
            {
                if (!officialsDesignInitialized)
                    return;

                // Reset disease checkboxes
                foreach (var kv in diseaseCheckMapping)
                    kv.Value.Checked = false;
                foreach (var kv in diseaseTextMapping)
                    kv.Value.Text = string.Empty;

                // Reset PARENT_TYPE=3,4,5 grids
                UC.DiseaseDetailGridHelper.ResetAll(diseaseGridParent3, gridView48);
                UC.DiseaseDetailGridHelper.ResetAll(diseaseGridParent4, gridView50);
                UC.DiseaseDetailGridHelper.ResetAll(diseaseGridParent5, gridView49);

                // Reset Tab 1 controls
                txtMoneyWithMediFood.Text = string.Empty;
                txtTreatmentMedi.Text = string.Empty;
                txtProceduresAndSurgeriesPerformed.Text = string.Empty;
                txtDiseasesAndMedicalNow.Text = string.Empty;
                memoPathologicalHistory.Text = string.Empty;

                // Reset Tab 2 controls
                spHeight.EditValue = null;
                spWeight.EditValue = null;
                lblBMIToanThan.Text = string.Empty;
                txtDaNiemMac.Text = string.Empty;
                txtLuoiHoiTho.Text = string.Empty;
                txtTinhThan.Text = string.Empty;
                txtHachNgoaiVi.Text = string.Empty;
                txtToanThanKhac.Text = string.Empty;

                // Internal medicine
                cboExamCirculationLoginName.EditValue = null;
                txtHeartRhythm.Text = string.Empty;
                txtHeartRate.Text = string.Empty;
                txtHeartBeat.Text = string.Empty;
                txtBloodPressureMax.Text = string.Empty;
                txtBloodPressureMin.Text = string.Empty;
                txtOtherExamCirculation.Text = string.Empty;
                txtArtery.Text = string.Empty;
                txtVein.Text = string.Empty;
                txtChest.Text = string.Empty;
                txtBreathRate.Text = string.Empty;
                txtLung.Text = string.Empty;
                txtExamDigestion.Text = string.Empty;
                txtExamKidneyUrology.Text = string.Empty;
                txtExamMuscleBone.Text = string.Empty;
                txtExamOEND.Text = string.Empty;
                txtExamNeurological.Text = string.Empty;

                // Specialist exams
                cboExamSurgeryLoginName.EditValue = null;
                txtExamSurgery.Text = string.Empty;
                cboExamObstetricLoginName.EditValue = null;
                txtExamObstetric.Text = string.Empty;
                cboExamDermatologyLoginName.EditValue = null;
                txtExamDermatology.Text = string.Empty;
                cboExamStomatologyLoginName.EditValue = null;
                txtExamStomatologyUpper.Text = string.Empty;
                txtExamStomatologyLower.Text = string.Empty;
                txtExamStomatology.Text = string.Empty;
                cboExamENTLoginName.EditValue = null;
                txtExamENTDisease.Text = string.Empty;
                txtExamENTLeftNormal.Text = string.Empty;
                txtExamENTLeftWhisper.Text = string.Empty;
                txtExamENTRightNormal.Text = string.Empty;
                txtExamENTRightWhisper.Text = string.Empty;
                txtTestENTLeft.Text = string.Empty;
                txtTestENTRight.Text = string.Empty;
                txtEndoscopyENT.Text = string.Empty;
                txtExamENT.Text = string.Empty;
                cboExamEyeLoginName.EditValue = null;
                txtExamEyeSightRight.Text = string.Empty;
                txtExamEyeSightLeft.Text = string.Empty;
                txtExamEyeSightGlassRight.Text = string.Empty;
                txtExamEyeSightGlassLeft.Text = string.Empty;
                txtIntraocularPressure.Text = string.Empty;
                txtLens.Text = string.Empty;
                txtFundoscopy.Text = string.Empty;
                txtExamEye.Text = string.Empty;
                cboExamOtherLoginName.EditValue = null;
                txtExamOther.Text = string.Empty;

                // Reset Tab 3 controls
                txtSuclinicalResult.Text = string.Empty;
                txtDiseases.Text = string.Empty;
                txtTreatmentInstruction.Text = string.Empty;
                cboHealthExamRank.EditValue = null;
                if (cboConclusionTime != null)
                    cboConclusionTime.EditValue = null;
                if (cboExamConcluderLoginName != null)
                    cboExamConcluderLoginName.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Save Data
        private void UpdateOfficialsDTOFromDataForm(ref MOS.SDO.HisServiceReqKskOfficialsSDO currentDTO)
        {
            try
            {
                currentDTO.RequestRoomId = this.moduleData.RoomId;

                // KskGeneral: dùng entity đã tồn tại (UPDATE) hoặc tạo mới (CREATE)
                HIS_KSK_GENERAL kskGeneral;
                if (currentData != null && currentData.KSK_GENERAL != null && currentData.KSK_GENERAL.ID > 0)
                    kskGeneral = currentData.KSK_GENERAL;
                else
                    kskGeneral = new HIS_KSK_GENERAL();
                currentDTO.KskGeneral = kskGeneral;
                kskGeneral.SERVICE_REQ_ID = currentDTO.ServiceReqId;

                // DHST: dùng entity đã tồn tại hoặc tạo mới
                HIS_DHST dhst;
                if (kskGeneral.HIS_DHST != null && kskGeneral.HIS_DHST.ID > 0)
                    dhst = kskGeneral.HIS_DHST;
                else
                {
                    dhst = new HIS_DHST();
                    if (currentData != null)
                        dhst.TREATMENT_ID = currentData.TREATMENT_ID;
                }
                currentDTO.Dhst = dhst;
                kskGeneral.HIS_DHST = dhst;

                // --- Tab 1: Tiền sử bệnh ---
                kskGeneral.PATHOLOGICAL_HISTORY = ReadText(memoPathologicalHistory);
                kskGeneral.HISTORY_ALLERGY = ReadText(txtMoneyWithMediFood);
                kskGeneral.HISTORY_DISEASE = ReadText(txtTreatmentMedi);
                kskGeneral.HISTORY_SURGERY = ReadText(txtProceduresAndSurgeriesPerformed);
                kskGeneral.MEDICINE_USING = ReadText(txtDiseasesAndMedicalNow);

                // --- Tab 2: Khám lâm sàng ---
                // Body
                kskGeneral.BODY_SKIN = ReadText(txtDaNiemMac);
                kskGeneral.BODY_TONGUE = ReadText(txtLuoiHoiTho);
                kskGeneral.BODY_SPIRIT = ReadText(txtTinhThan);
                kskGeneral.BODY_LYMPH = ReadText(txtHachNgoaiVi);
                kskGeneral.BODY_OTHER = ReadText(txtToanThanKhac);

                // Internal medicine
                kskGeneral.EXAM_CIRCULATION = ReadText(txtOtherExamCirculation);
                kskGeneral.EXAM_CIRCULATION_LOGINNAME = ReadLookUpValue(cboExamCirculationLoginName);
                kskGeneral.ARTERY = ReadText(txtArtery);
                kskGeneral.VEIN = ReadText(txtVein);
                kskGeneral.CHEST = ReadText(txtChest);
                kskGeneral.LUNG = ReadText(txtLung);

                // Abdominal & specialties
                kskGeneral.EXAM_DIGESTION = ReadText(txtExamDigestion);
                kskGeneral.EXAM_KIDNEY_UROLOGY = ReadText(txtExamKidneyUrology);
                kskGeneral.EXAM_MUSCLE_BONE = ReadText(txtExamMuscleBone);
                kskGeneral.EXAM_OEND = ReadText(txtExamOEND);
                kskGeneral.EXAM_NEUROLOGICAL = ReadText(txtExamNeurological);

                // Surgery
                kskGeneral.EXAM_SURGERY = ReadText(txtExamSurgery);
                kskGeneral.EXAM_SURGERY_LOGINNAME = ReadLookUpValue(cboExamSurgeryLoginName);

                // Obstetric
                kskGeneral.EXAM_OBSTETRIC = ReadText(txtExamObstetric);
                kskGeneral.EXAM_OBSTETRIC_LOGINNAME = ReadLookUpValue(cboExamObstetricLoginName);

                // Dermatology
                kskGeneral.EXAM_DERMATOLOGY = ReadText(txtExamDermatology);
                kskGeneral.EXAM_DERMATOLOGY_LOGINNAME = ReadLookUpValue(cboExamDermatologyLoginName);

                // Stomatology
                kskGeneral.EXAM_STOMATOLOGY_UPPER = ReadText(txtExamStomatologyUpper);
                kskGeneral.EXAM_STOMATOLOGY_LOWER = ReadText(txtExamStomatologyLower);
                kskGeneral.EXAM_STOMATOLOGY = ReadText(txtExamStomatology);
                kskGeneral.EXAM_STOMATOLOGY_LOGINNAME = ReadLookUpValue(cboExamStomatologyLoginName);

                // ENT
                kskGeneral.EXAM_ENT_DISEASE = ReadText(txtExamENTDisease);
                kskGeneral.EXAM_ENT_LEFT_NORMAL = ReadText(txtExamENTLeftNormal);
                kskGeneral.EXAM_ENT_LEFT_WHISPER = ReadText(txtExamENTLeftWhisper);
                kskGeneral.EXAM_ENT_RIGHT_NORMAL = ReadText(txtExamENTRightNormal);
                kskGeneral.EXAM_ENT_RIGHT_WHISPER = ReadText(txtExamENTRightWhisper);
                kskGeneral.TEST_ENT_LEFT = ReadText(txtTestENTLeft);
                kskGeneral.TEST_ENT_RIGHT = ReadText(txtTestENTRight);
                kskGeneral.ENDOSCOPY_ENT = ReadText(txtEndoscopyENT);
                kskGeneral.EXAM_ENT = ReadText(txtExamENT);
                kskGeneral.EXAM_ENT_LOGINNAME = ReadLookUpValue(cboExamENTLoginName);

                // Eye
                kskGeneral.EXAM_EYESIGHT_RIGHT = ReadText(txtExamEyeSightRight);
                kskGeneral.EXAM_EYESIGHT_LEFT = ReadText(txtExamEyeSightLeft);
                kskGeneral.EXAM_EYESIGHT_GLASS_RIGHT = ReadText(txtExamEyeSightGlassRight);
                kskGeneral.EXAM_EYESIGHT_GLASS_LEFT = ReadText(txtExamEyeSightGlassLeft);
                kskGeneral.INTRAOCULAR_PRESSURE = ReadText(txtIntraocularPressure);
                kskGeneral.LENS = ReadText(txtLens);
                kskGeneral.FUNDOSCOPY = ReadText(txtFundoscopy);
                kskGeneral.EXAM_EYE = ReadText(txtExamEye);
                kskGeneral.EXAM_EYE_LOGINNAME = ReadLookUpValue(cboExamEyeLoginName);

                // Other exam
                kskGeneral.EXAM_OTHER = ReadText(txtExamOther);
                kskGeneral.EXAM_OTHER_LOGINNAME = ReadLookUpValue(cboExamOtherLoginName);

                // --- Tab 3: Kết luận ---
                kskGeneral.RESULT_SUBCLINICAL = ReadText(txtSuclinicalResult);
                kskGeneral.DISEASES = ReadText(txtDiseases);
                kskGeneral.TREATMENT_INSTRUCTION = ReadText(txtTreatmentInstruction);

                if (cboHealthExamRank.EditValue != null)
                    kskGeneral.HEALTH_EXAM_RANK_ID = Convert.ToInt64(cboHealthExamRank.EditValue);
                else
                    kskGeneral.HEALTH_EXAM_RANK_ID = null;

                if (cboConclusionTime != null && cboConclusionTime.EditValue != null)
                {
                    kskGeneral.CONCLUSION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(cboConclusionTime.DateTime);
                }
                else
                {
                    kskGeneral.CONCLUSION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                }

                if (cboExamConcluderLoginName != null && cboExamConcluderLoginName.EditValue != null)
                {
                    string loginName = cboExamConcluderLoginName.EditValue.ToString();
                    kskGeneral.CONCLUDER_LOGINNAME = loginName;
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME.ToUpper() == loginName.ToUpper());
                    if (user != null) kskGeneral.CONCLUDER_USERNAME = user.USERNAME;
                }

                // DHST
                dhst.HEIGHT = spHeight.EditValue != null ? (decimal?)spHeight.Value : null;
                dhst.WEIGHT = spWeight.EditValue != null ? (decimal?)spWeight.Value : null;
                decimal bpMax;
                dhst.BLOOD_PRESSURE_MAX = decimal.TryParse(txtBloodPressureMax.Text, out bpMax) ? (long?)bpMax : null;
                decimal bpMin;
                dhst.BLOOD_PRESSURE_MIN = decimal.TryParse(txtBloodPressureMin.Text, out bpMin) ? (long?)bpMin : null;
                string breathRateText = ReadText(txtBreathRate);
                dhst.BREATH_RATE = ParseNullableDecimal(breathRateText);
                Inventec.Common.Logging.LogSystem.Debug("SaveDhst: txtBreathRate.Text=[" + breathRateText + "] -> BREATH_RATE=" + dhst.BREATH_RATE);
                dhst.HEART_RHYTHM = ReadText(txtHeartRhythm);
                dhst.HEART_RATE = ReadText(txtHeartRate);
                dhst.HEART_BEAT = ReadText(txtHeartBeat);
                decimal bmi;
                dhst.VIR_BMI = decimal.TryParse(lblBMIToanThan.Text, out bmi) ? (decimal?)bmi : null;

                // Disease detail results
                AttachDiseaseDetailResults(ref currentDTO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AttachDiseaseDetailResults(ref MOS.SDO.HisServiceReqKskOfficialsSDO currentDTO)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    "AttachDiseaseDetailResults START - diseaseDetails.Count=" + diseaseDetails.Count
                    + " diseaseCheckMapping.Count=" + diseaseCheckMapping.Count
                    + " diseaseTextMapping.Count=" + diseaseTextMapping.Count
                    + " officialsDesignInitialized=" + officialsDesignInitialized);

                var rows = new List<HIS_DISEASE_DETAIL_RESULT>();
                long? kskGeneralId = currentData != null && currentData.KSK_GENERAL != null ? (long?)currentData.KSK_GENERAL.ID : null;

                // Luôn tạo mới (ID=0) — BE sẽ INSERT. FE dedup khi load (lấy bản mới nhất).
                int checkedCount = 0;
                foreach (var kv in diseaseCheckMapping)
                {
                    bool isChecked = kv.Value != null && kv.Value.Checked;
                    string otherText = diseaseTextMapping.ContainsKey(kv.Key) && diseaseTextMapping[kv.Key] != null
                        ? (diseaseTextMapping[kv.Key].Text ?? string.Empty).Trim()
                        : null;

                    if (isChecked) checkedCount++;
                    rows.Add(new HIS_DISEASE_DETAIL_RESULT
                    {
                        DISEASE_DETAIL_ID = kv.Key,
                        KSK_GENERAL_ID = kskGeneralId,
                        IS_CHECK = (short)(isChecked ? 1 : 0),
                        OTHER = !string.IsNullOrEmpty(otherText) ? otherText : null
                    });
                }
                Inventec.Common.Logging.LogSystem.Debug(
                    "AttachDiseaseDetailResults - checkedCount=" + checkedCount
                    + " totalRows=" + rows.Count);

                // Text-only mappings
                foreach (var kv in diseaseTextMapping)
                {
                    if (diseaseCheckMapping.ContainsKey(kv.Key)) continue;
                    string otherText = kv.Value != null ? (kv.Value.Text ?? string.Empty).Trim() : string.Empty;
                    rows.Add(new HIS_DISEASE_DETAIL_RESULT
                    {
                        DISEASE_DETAIL_ID = kv.Key,
                        KSK_GENERAL_ID = kskGeneralId,
                        IS_CHECK = 0,
                        OTHER = !string.IsNullOrEmpty(otherText) ? otherText : null
                    });
                }

                // PARENT_TYPE=3,4,5: Thu thap ket qua tu gridView48/gridView50/gridView49
                var gridResults = UC.DiseaseDetailGridHelper.CollectResults(diseaseGridParent3, kskGeneralId);
                gridResults.AddRange(UC.DiseaseDetailGridHelper.CollectResults(diseaseGridParent4, kskGeneralId));
                gridResults.AddRange(UC.DiseaseDetailGridHelper.CollectResults(diseaseGridParent5, kskGeneralId));
                foreach (var ado in gridResults)
                {
                    rows.Add(new HIS_DISEASE_DETAIL_RESULT
                    {
                        DISEASE_DETAIL_ID = ado.DISEASE_DETAIL_ID,
                        KSK_GENERAL_ID = ado.KSK_GENERAL_ID,
                        IS_CHECK = (short)(ado.IS_CHECK ?? 0),
                        OTHER = !string.IsNullOrEmpty(ado.OTHER) ? ado.OTHER : null
                    });
                }

                rows = rows.GroupBy(o => o.DISEASE_DETAIL_ID).Select(g => g.Last()).ToList();

                // Gán lên root DTO và navigation property của KskGeneral để EF cascade insert đúng FK
                currentDTO.DiseaseDetailResults = rows;
                if (currentDTO.KskGeneral != null)
                    currentDTO.KskGeneral.HIS_DISEASE_DETAIL_RESULT = rows;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Utility Methods
        private string ReadText(Control control)
        {
            return control == null ? string.Empty : control.Text == null ? string.Empty : control.Text.Trim();
        }

        private string ReadLookUpValue(GridLookUpEdit cbo)
        {
            if (cbo == null || cbo.EditValue == null) return null;
            return cbo.EditValue.ToString().Trim();
        }

        private decimal? ParseNullableDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            decimal result;
            return decimal.TryParse(text, out result) ? (decimal?)result : null;
        }

        private string ReadString(object obj, string propertyName)
        {
            var value = ReadPropertyValue(obj, propertyName);
            return value == null ? string.Empty : value.ToString();
        }

        private decimal? ReadDecimal(object obj, string propertyName)
        {
            var value = ReadPropertyValue(obj, propertyName);
            if (value == null) return null;
            decimal result;
            return decimal.TryParse(value.ToString(), out result) ? (decimal?)result : null;
        }

        private long? ReadNullableLong(object obj, string propertyName)
        {
            var value = ReadPropertyValue(obj, propertyName);
            if (value == null) return null;
            long result;
            return long.TryParse(value.ToString(), out result) ? (long?)result : null;
        }

        private object ReadPropertyValue(object obj, string propertyName)
        {
            try
            {
                if (obj == null || string.IsNullOrEmpty(propertyName)) return null;
                var type = obj.GetType();
                var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                return prop != null ? prop.GetValue(obj, null) : null;
            }
            catch
            {
                return null;
            }
        }

        private void SetPropertyValue(object obj, string propertyName, object value)
        {
            try
            {
                if (obj == null || string.IsNullOrEmpty(propertyName)) return;
                var type = obj.GetType();
                var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null || !prop.CanWrite) return;
                if (value == null)
                {
                    prop.SetValue(obj, null, null);
                    return;
                }
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (targetType.IsAssignableFrom(value.GetType()))
                {
                    prop.SetValue(obj, value, null);
                }
                else
                {
                    var converted = Convert.ChangeType(value, targetType);
                    prop.SetValue(obj, converted, null);
                }
            }
            catch { }
        }

        private IEnumerable<BaseEdit> GetAllEditors(Control root)
        {
            if (root == null) yield break;
            foreach (Control c in root.Controls)
            {
                if (c is BaseEdit) yield return (BaseEdit)c;
                foreach (var child in GetAllEditors(c)) yield return child;
            }
        }
        private void DrawBorderAround(Control ctrl)
        {
            if (ctrl == null || ctrl.Parent == null) return;
            var parent = ctrl.Parent;
            parent.Paint += (s, e) =>
            {
                var rect = new System.Drawing.Rectangle(
                    ctrl.Location.X - 1,
                    ctrl.Location.Y - 1,
                    ctrl.Width + 1,
                    ctrl.Height + 1);
                using (var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(180, 180, 180), 1))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };
        }
        #endregion
    }
}
