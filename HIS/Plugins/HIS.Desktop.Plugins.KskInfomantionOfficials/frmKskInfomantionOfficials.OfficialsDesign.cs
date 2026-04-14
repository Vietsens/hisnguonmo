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
                BuildDiseaseCheckMapping();
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

                // Thêm border cho các scrollable control để dễ nhìn
                DrawBorderAround(xtraScrollableControl4);
                DrawBorderAround(xtraScrollableControl5);
                DrawBorderAround(xtraScrollableControl1);
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

                // Conclusion doctor
                if (cboExamConcluderLoginName != null)
                {
                    var loginNames = acsUsers.Select(o => o.LOGINNAME).ToList();
                    cboExamConcluderLoginName.Properties.Items.AddRange(loginNames);
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
                // cboConclusionTime is a GridLookUpEdit used as a date picker
                // We initialize it for basic use
                if (cboConclusionTime != null)
                {
                    cboConclusionTime.EditValue = null;
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
                diseaseResults = data ?? new List<HIS_DISEASE_DETAIL_RESULT>();
            }
            catch (Exception ex)
            {
                diseaseResults = new List<HIS_DISEASE_DETAIL_RESULT>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BuildDiseaseCheckMapping()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("BuildDiseaseCheckMapping START - diseaseDetails.Count=" + diseaseDetails.Count);
                diseaseCheckMapping.Clear();
                diseaseTextMapping.Clear();

                // PARENT_TYPE=1: Thói quen sinh hoạt (Habits)
                // Checkboxes đã thiết kế sẵn trong panelControl5, map theo thứ tự DISEASE_TYPE_ID + NUM_ORDER_DETAIL
                MapByOrder(1, new CheckEdit[]
                {
                    // DISEASE_TYPE_ID=23: Một ngày ngủ mấy tiếng
                    chkLower5, chkLower7, chkLessOrEqual8, chkHigher8,
                    // DISEASE_TYPE_ID=24: Đêm ngủ
                    chkInsomnia, chkWakeup, chkWakeEarly, chkGoodSleep,
                    // DISEASE_TYPE_ID=25: Chơi môn thể thao
                    chkWalking, chkBadminton, chkSwiming, chkGolf,
                    // DISEASE_TYPE_ID=26: Thời gian trung bình chơi thể thao -> no checkbox (text only)
                    // DISEASE_TYPE_ID=27: Hút thuốc
                    chkCigarette, chkCigars, chkNon_Smoker,
                    // DISEASE_TYPE_ID=28: Uống rượu/bia
                    chkNon_Beer, chkSometimeBeer, chkMuchBeer,
                    // DISEASE_TYPE_ID=29: Ăn mặn
                    chkVerySalty, chkSalty, chkBlandEnough, chkBland,
                    // DISEASE_TYPE_ID=30: Ăn ngọt
                    chkVerySweet, chkSweet, chkSweetEnough, chkNoSweet
                },
                new Dictionary<string, Control>
                {
                    // TextEdit cho IS_OTHER=1: map theo tên control -> DISEASE_TYPE_ID
                    { "txtOtherSleep", txtOtherSleep },
                    { "txtOtherNightSleep", txtOtherNightSleep },
                    { "txtOtherSport", txtOtherSport },
                    { "txtSportAverageTime", txtSportAverageTime },
                    { "txtOtherCigarette", txtOtherCigarette },
                    { "txtOtherBeer", txtOtherBeer },
                    { "txtOtherSalty", txtOtherSalty },
                    { "txtOtherSweet", txtOtherSweet }
                });

                // PARENT_TYPE=2: Tiền sử bệnh (Disease History)
                MapByOrder(2, new CheckEdit[]
                {
                    // DISEASE_TYPE_ID=31: Tim mạch
                    chkHighBloodPressure, chkCoronaryArteries, chkHeartRhythmDisorders, chkHeartFailure,
                    // DISEASE_TYPE_ID=32: Hô hấp
                    chkTuberculosis, chkAsthma, chkLungDisease, chkPleuralEffusion,
                    // DISEASE_TYPE_ID=33: Nội tiết
                    chkDiabetes, chkHyperthyroidism, chkHypothyroidism, chkAdrenalInsufficiency,
                    // DISEASE_TYPE_ID=34: Tiêu hóa
                    chkStomachUlcers, chkGallstones, chkHepatitis, chkColitis,
                    // DISEASE_TYPE_ID=35: Thận tiết niệu
                    chkGlomerulonephritis, chkKidneyStones, chkKidneyFibroidsTLT, chkKidneyFailure,
                    // DISEASE_TYPE_ID=36: Thần kinh, Tâm thần
                    chkEpilepsy, chkParalysis, chkPsychosis, chkMemoryImpairment,
                    // DISEASE_TYPE_ID=37: Xương khớp
                    chkOsteoarthritis, chkGout, chkHerniatedDisc, chkHumpbackOrScoliosis,
                    // DISEASE_TYPE_ID=38: Da liễu
                    chkPsoriasis, chkLupus, chkUlcers, chkEczema,
                    // DISEASE_TYPE_ID=39: Bệnh về máu
                    chkAnemia, chkBoneMarrowFailure, chkLeukemia, chkThrombocytopenicPurpura,
                    // DISEASE_TYPE_ID=40: RHM
                    chkGingivitis, chkPeriperositis, chkDentalCaries,
                    // DISEASE_TYPE_ID=41: TMH
                    chkMiddleEarInfection, chkSinusitis, chkSoreThroat, chkTinnitus,
                    // DISEASE_TYPE_ID=42: Mắt
                    chkGlocom, chkCataract, chkEyeInjury, chkReflectiveError,
                    // DISEASE_TYPE_ID=43: Sản phụ khoa
                    chkMenstrualDisorders, chkCesareanSection, chkSterilization, chkInfertility,
                    // DISEASE_TYPE_ID=44: Ung thư các cơ quan
                    chkRespiratorySystem, chkDigestiveSystem, chkUrinarySystem, chkHematopoieticSystem
                },
                new Dictionary<string, Control>
                {
                    { "txtOtherHeart", txtOtherHeart },
                    { "txtOtherRespiration", txtOtherRespiration },
                    { "txtOtherEndocrineDisorders", txtOtherEndocrineDisorders },
                    { "txtOtherDigestiveProblems", txtOtherDigestiveProblems },
                    { "txtOtherUrinarySystemAndKidneys", txtOtherUrinarySystemAndKidneys },
                    { "txtOtherNervousSystem", txtOtherNervousSystem },
                    { "txtOtherJointAndBoneProblems", txtOtherJointAndBoneProblems },
                    { "txtOtherDermatology", txtOtherDermatology },
                    { "txtOtherBloodDisorders", txtOtherBloodDisorders },
                    { "txtOtherDentalAndMaxillofacialProblems", txtOtherDentalAndMaxillofacialProblems },
                    { "txtOtherEarNoseThroat", txtOtherEarNoseThroat },
                    { "txtOtherEye", txtOtherEye },
                    { "txtOtherObstetricsAndGynecology", txtOtherObstetricsAndGynecology },
                    { "txtOtherDiseases", txtOtherDiseases }
                });

                // PARENT_TYPE=3: Tiền sử gia đình (Family History)
                MapByOrder(3, new CheckEdit[]
                {
                    chkFamilyHistoryTangHA, chkFamilyHistoryBenhDMVanh, chkFamilyHistoryDaiThaoDuong,
                    chkFamilyHistoryLoetDaDay, chkFamilyHistoryHen, chkFamilyHistoryLao,
                    chkFamilyHistoryBenhThanKinh, chkFamilyHistoryRoiLoanTamThan, chkFamilyHistoryLoangXuong,
                    chkFamilyHistoryDiUng, chkFamilyHistoryUngThu
                },
                new Dictionary<string, Control>
                {
                    { "txtFamilyHistoryKhac", txtFamilyHistoryKhac }
                });

                // PARENT_TYPE=4: Triệu chứng cơ năng (Functional Symptoms)
                MapByOrder(4, new CheckEdit[]
                {
                    chkKhoTho, chkDanhTrongNguc, chkHo, chkKhanTieng,
                    chkUongNhieuDaiNhieu, chkOHoi, chkOChua, chkGiamTriNho,
                    chkMatNgu, chkHoaMatChongMat, chkUtai, chkNgheKem,
                    chkDauHong, chkNuotKho, chkNhinMo, chkDaiBuotDaiRat,
                    chkDaiTienNhay, chkDaiTienMau, chkTaoBon, chkRLKinhNguyet
                },
                new Dictionary<string, Control>
                {
                    { "txtTrieuChungCoNangKhac", txtTrieuChungCoNangKhac }
                });

                // PARENT_TYPE=5: Triệu chứng đau (Pain Symptoms)
                MapByOrder(5, new CheckEdit[]
                {
                    chkDau, chkCo, chkNguc, chkBung,
                    chkThatLung, chkCacKhop, chkXuong, chkMuscle,
                    chkTai, chkMat, chkRang, chkHong
                },
                new Dictionary<string, Control>
                {
                    { "txtTrieuChungDauKhac", txtTrieuChungDauKhac }
                });

                Inventec.Common.Logging.LogSystem.Debug(
                    "BuildDiseaseCheckMapping END - diseaseCheckMapping.Count=" + diseaseCheckMapping.Count
                    + " diseaseTextMapping.Count=" + diseaseTextMapping.Count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Map checkboxes theo thứ tự: lấy disease details có IS_CHECKBOX=1, sắp xếp theo NUM_ORDER_TYPE + NUM_ORDER_DETAIL,
        /// rồi map 1-1 vào mảng checkboxes theo đúng thứ tự.
        /// TextEdit IS_OTHER thì map theo thứ tự các disease detail có IS_OTHER=1.
        /// </summary>
        private void MapByOrder(long parentType, CheckEdit[] checkboxes, Dictionary<string, Control> otherTexts)
        {
            try
            {
                var details = diseaseDetails
                    .Where(o => o.PARENT_TYPE == parentType)
                    .OrderBy(o => o.NUM_ORDER_TYPE)
                    .ThenBy(o => o.NUM_ORDER_DETAIL)
                    .ToList();

                Inventec.Common.Logging.LogSystem.Debug("KskOfficials MapByOrder: PARENT_TYPE=" + parentType + " details.Count=" + details.Count + " checkboxes.Length=" + checkboxes.Length);

                // Map checkboxes: lọc ra các detail có IS_CHECKBOX=1, map theo thứ tự
                var checkDetails = details.Where(o => (o.IS_CHECKBOX ?? 0) == 1).ToList();
                Inventec.Common.Logging.LogSystem.Debug("KskOfficials MapByOrder: checkDetails.Count=" + checkDetails.Count);
                for (int i = 0; i < checkDetails.Count && i < checkboxes.Length; i++)
                {
                    if (checkboxes[i] != null)
                    {
                        diseaseCheckMapping[checkDetails[i].ID] = checkboxes[i];
                    }
                }

                // Map other text fields: lọc ra các detail có IS_OTHER=1, map theo thứ tự
                var otherDetails = details.Where(o => (o.IS_OTHER ?? 0) == 1).ToList();
                var otherTextList = new List<Control>(otherTexts.Values);
                for (int i = 0; i < otherDetails.Count && i < otherTextList.Count; i++)
                {
                    if (otherTextList[i] != null)
                    {
                        diseaseTextMapping[otherDetails[i].ID] = otherTextList[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<CheckEdit> GetAllCheckEdits(Control root)
        {
            var result = new List<CheckEdit>();
            if (root == null) return result;
            foreach (Control c in root.Controls)
            {
                if (c is CheckEdit) result.Add((CheckEdit)c);
                result.AddRange(GetAllCheckEdits(c));
            }
            return result;
        }

        private List<Control> GetAllTextInputs(Control root)
        {
            var result = new List<Control>();
            if (root == null) return result;
            foreach (Control c in root.Controls)
            {
                if (c is TextEdit || c is TextBox) result.Add(c);
                result.AddRange(GetAllTextInputs(c));
            }
            return result;
        }
        #endregion

        #region Load Data
        private void LoadOfficialsFormData(ADO.ServiceReqADO data)
        {
            try
            {
                InitializeOfficialsDesignIfNeeded();
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
                if (conclusionTime.HasValue && cboConclusionTime != null)
                {
                    cboConclusionTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(conclusionTime.Value);
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
                foreach (var result in diseaseResults)
                {
                    if (result.DISEASE_DETAIL_ID == null) continue;
                    long detailId = result.DISEASE_DETAIL_ID.Value;
                    if (diseaseCheckMapping.ContainsKey(detailId))
                    {
                        diseaseCheckMapping[detailId].Checked = (result.IS_CHECK ?? 0) == 1;
                    }
                    if (diseaseTextMapping.ContainsKey(detailId))
                    {
                        diseaseTextMapping[detailId].Text = result.OTHER ?? string.Empty;
                    }
                }
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

                // KskGeneral: entity HIS_KSK_GENERAL (không phải HisKskGeneralSDO)
                var kskGeneral = currentDTO.KskGeneral ?? new HIS_KSK_GENERAL();
                currentDTO.KskGeneral = kskGeneral;
                kskGeneral.SERVICE_REQ_ID = currentDTO.ServiceReqId;

                // DHST: gán lên cả root DTO và navigation property trong KskGeneral
                var dhst = currentDTO.Dhst ?? new HIS_DHST();
                currentDTO.Dhst = dhst;
                if (currentData != null)
                    dhst.TREATMENT_ID = currentData.TREATMENT_ID;
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
                    try
                    {
                        var dt = Convert.ToDateTime(cboConclusionTime.EditValue);
                        kskGeneral.CONCLUSION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        kskGeneral.CONCLUSION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    }
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
                dhst.BREATH_RATE = ParseNullableDecimal(ReadText(txtBreathRate));
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

                // Theo thiết kế: chỉ lưu dòng khi user CÓ check (IS_CHECK=1) hoặc có nhập OTHER.
                // Không check + không nhập gì → không lưu.
                int checkedCount = 0;
                foreach (var kv in diseaseCheckMapping)
                {
                    bool isChecked = kv.Value != null && kv.Value.Checked;
                    string otherText = diseaseTextMapping.ContainsKey(kv.Key) && diseaseTextMapping[kv.Key] != null
                        ? (diseaseTextMapping[kv.Key].Text ?? string.Empty).Trim()
                        : null;
                    bool hasOther = !string.IsNullOrEmpty(otherText);

                    if (!isChecked && !hasOther) continue;

                    if (isChecked) checkedCount++;
                    var row = new HIS_DISEASE_DETAIL_RESULT
                    {
                        DISEASE_DETAIL_ID = kv.Key,
                        IS_CHECK = (short)(isChecked ? 1 : 0),
                        OTHER = hasOther ? otherText : null,
                        KSK_GENERAL_ID = kskGeneralId
                    };
                    rows.Add(row);
                }
                Inventec.Common.Logging.LogSystem.Debug(
                    "AttachDiseaseDetailResults - checkedCount=" + checkedCount
                    + " rowsFromCheckboxes=" + rows.Count);

                // Collect text-only mappings (disease details với IS_OTHER nhưng không có IS_CHECKBOX)
                // Chỉ lưu nếu user có nhập text
                foreach (var kv in diseaseTextMapping)
                {
                    if (diseaseCheckMapping.ContainsKey(kv.Key)) continue;

                    string otherText = kv.Value != null ? (kv.Value.Text ?? string.Empty).Trim() : string.Empty;
                    if (string.IsNullOrEmpty(otherText)) continue;

                    var row = new HIS_DISEASE_DETAIL_RESULT
                    {
                        DISEASE_DETAIL_ID = kv.Key,
                        IS_CHECK = 0,
                        OTHER = otherText,
                        KSK_GENERAL_ID = kskGeneralId
                    };
                    rows.Add(row);
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
