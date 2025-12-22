using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MchTreatmentExamService.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Init Combo Trình độ
        
        /// <summary>
        /// Khởi tạo combo cho tất cả các GridLookUpEdit trình độ
        /// </summary>
        private void InitAllComboDiploma()
        {
            try
            {
                // Tạo data source chung cho tất cả combo trình độ
                List<KeyValueADO> diplomaData = CreateDiplomaDataSource();

                // Tab 1: Sàng lọc ung thư cổ tử cung
                InitComboKeyValue(cboDiploma1, diplomaData);

                // Tab 2: Khám thai
                InitComboKeyValue(cboDiploma2, diplomaData);

                // Tab 3: Sinh đẻ (Mẹ)
                InitComboKeyValue(cboDiploma3, diplomaData);

                // Tab 4: Tránh thai
                InitComboKeyValue(cboDiploma4, diplomaData);

                // Tab 5: Phá thai
                InitComboKeyValue(cboDiploma5, diplomaData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tạo data source cho trình độ
        /// Mã "1": Bác sĩ
        /// Mã "2": Y sĩ
        /// Mã "3": Hộ sinh
        /// Mã "4": Cô đỡ thôn bản
        /// Mã "9": Khác
        /// </summary>
        private List<KeyValueADO> CreateDiplomaDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Bác sĩ"));
                result.Add(new KeyValueADO("2", "Y sĩ"));
                result.Add(new KeyValueADO("3", "Hộ sinh"));
                result.Add(new KeyValueADO("4", "Cô đỡ thôn bản"));
                result.Add(new KeyValueADO("9", "Khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Lấy tên trình độ từ mã
        /// </summary>
        private string GetDiplomaNameByCode(string code)
        {
            try
            {
                var diplomaData = CreateDiplomaDataSource();
                var diploma = diplomaData.FirstOrDefault(x => x.CODE == code);
                return diploma != null ? diploma.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        #endregion

        #region Init Combo User

        /// <summary>
        /// Khởi tạo tất cả combo User trong các tab
        /// </summary>
        private void InitAllComboUser()
        {
            try
            {
                // Lấy data source từ V_HIS_EMPLOYEE
                List<V_HIS_EMPLOYEE> employeeData = GetEmployeeDataSource();

                // Tab 1: Sàng lọc ung thư cổ tử cung
                InitComboUser(cboUser1, employeeData);

                // Tab 2: Khám thai
                InitComboUser(cboUser2, employeeData);

                // Tab 3: Sinh đẻ (Mẹ)
                InitComboUser(cboUser3, employeeData);

                // Tab 4: Tránh thai
                InitComboUser(cboUser4, employeeData);

                // Tab 5: Phá thai
                InitComboUser(cboUser5, employeeData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên từ BackendDataWorker
        /// </summary>
        private List<V_HIS_EMPLOYEE> GetEmployeeDataSource()
        {
            List<V_HIS_EMPLOYEE> result = new List<V_HIS_EMPLOYEE>();
            try
            {
                // Lấy dữ liệu từ RAM
                var employees = BackendDataWorker.Get<V_HIS_EMPLOYEE>();
                
                if (employees != null && employees.Count > 0)
                {
                    // Lọc nhân viên đang hoạt động
                    result = employees.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                     .OrderBy(o => o.TDL_USERNAME)
                                     .ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Khởi tạo combo User với data từ V_HIS_EMPLOYEE
        /// </summary>
        private void InitComboUser(GridLookUpEdit combo, List<V_HIS_EMPLOYEE> data)
        {
            try
            {
                if (combo == null) return;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Tên", 250, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;

                ControlEditorLoader.Load(combo, data, controlEditorADO);

                // Set giá trị mặc định là user hiện tại
                SetDefaultUser(combo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Set giá trị mặc định cho combo User là user đang đăng nhập
        /// </summary>
        private void SetDefaultUser(GridLookUpEdit combo)
        {
            try
            {
                if (combo == null) return;

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (!string.IsNullOrEmpty(loginName))
                {
                    combo.EditValue = loginName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Init Combo Chẩn đoán và Điều trị UT cổ tử cung

        /// <summary>
        /// Khởi tạo combo Chẩn đoán và Điều trị UT cổ tử cung
        /// </summary>
        private void InitAllComboCervicalCancer()
        {
            try
            {
                // Tab 1: Sàng lọc ung thư cổ tử cung
                
                // Combo Chẩn đoán UT cổ tử cung
                List<KeyValueADO> cervicalCancerDxData = CreateCervicalCancerDxDataSource();
                InitComboKeyValue(cboCervicalCancerDx1, cervicalCancerDxData);
                
                // Combo Điều trị tiền UT cổ tử cung
                List<KeyValueADO> preCervicalCancerTreatData = CreatePreCervicalCancerTreatDataSource();
                InitComboKeyValue(cboPreCervicalCancerTreat1, preCervicalCancerTreatData);

                // Tab 2: Khám thai
                
                // Combo Tiền sử nội khoa
                InitComboMedicalHistoryInternalCheck();
                InitComboMedicalHistoryInternal();

                // Tab 3: Sinh đẻ (Mẹ)

                // Combo Nơi đẻ
                List<KeyValueADO> birthplaceTypeData = CreateBirthplaceTypeDataSource();
                InitComboKeyValue(cboBirthplaceType3, birthplaceTypeData);
                
                // Combo Cách đẻ
                List<KeyValueADO> birthMethodData = CreateBirthMethodDataSource();
                InitComboKeyValue(cboBirthMethod3, birthMethodData);
                
                // Combo Tai biến sản khoa
                List<KeyValueADO> maternalComplicationData = CreateMaternalComplicationDataSource();
                InitComboKeyValue(cboMaternalComplication3, maternalComplicationData);
                
                // Tab 3: Sinh đẻ (Trẻ)
                
                // Combo Nơi tìm thấy con
                List<KeyValueADO> foundLocationData = CreateFoundLocationDataSource();
                InitComboKeyValue(cboFoundLocation3, foundLocationData);
                
                // Combo Tình trạng con (Child Status)
                List<KeyValueADO> childStatusData = CreateChildStatusDataSource();
                InitComboKeyValue(cboChildStatus3, childStatusData);
                
                // Combo Da kề da
                List<KeyValueADO> skinToSkinData = CreateSkinToSkinDataSource();
                InitComboKeyValue(cboSkinToSkin3, skinToSkinData);
                
                // Combo Dân tộc (từ SDA_ETHNIC)
                List<SDA.EFMODEL.DataModels.SDA_ETHNIC> ethnicData = GetEthnicDataSource();
                InitComboEthnic(cboEthnic3, ethnicData);
                
                // Combo Tình trạng con (Newborn Condition) - đã có sẵn
                List<KeyValueADO> newbornConditionData = CreateNewbornConditionDataSource();
                InitComboKeyValue(cboNewbornCondition3, newbornConditionData);

                // NEW: Combo Giới tính (từ HIS_GENDER) cho Trẻ
                List<HIS_GENDER> genderData = GetGenderDataSource();
                InitComboGender(cboChildGender3, genderData);

                // Tab 4: Tránh thai
                
                // Combo Biện pháp tránh thai
                List<KeyValueADO> contraceptionMethodData = CreateContraceptionMethodDataSource();
                InitComboKeyValue(cboContraceptionMethod4, contraceptionMethodData);
                
                // Combo Tai biến tránh thai
                List<KeyValueADO> contraceptionComplicationData = CreateContraceptionComplicationDataSource();
                InitComboKeyValue(cboContraceptionComplication4, contraceptionComplicationData);

                // Tab 5: Phá thai
                
                // Combo Phương pháp phá thai
                List<KeyValueADO> abortionMethodData = CreateAbortionMethodDataSource();
                InitComboKeyValue(cboAbortionMethod5, abortionMethodData);
                
                // Combo Loại tai biến phá thai
                List<KeyValueADO> abortionComplicationData = CreateAbortionComplicationDataSource();
                InitComboKeyValue(cboTissueExaminationResult5, abortionComplicationData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboMedicalHistoryInternal()
        {
            try
            {
                cboMedicalHistoryInternal2.Properties.DataSource = CreateMedicalHistoryInternalDataSource();
                cboMedicalHistoryInternal2.Properties.DisplayMember = "NAME";
                cboMedicalHistoryInternal2.Properties.ValueMember = "CODE";
                DevExpress.XtraGrid.Columns.GridColumn column = cboMedicalHistoryInternal2.Properties.View.Columns.AddField("NAME");
                column.VisibleIndex = 1;
                column.Width = 200;
                column.Caption = "Tất cả";
                cboMedicalHistoryInternal2.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboMedicalHistoryInternal2.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMedicalHistoryInternalCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboMedicalHistoryInternal2.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboMedicalHistoryInternal2.Properties.Tag = gridCheck;
                cboMedicalHistoryInternal2.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboMedicalHistoryInternal2.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboMedicalHistoryInternal2.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<KeyValueADO> MedicalHistoryInternal2Selected = new List<KeyValueADO>();
        private void Event_Check(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                MedicalHistoryInternal2Selected = new List<KeyValueADO>();
                if (gridCheckMark != null)
                {
                    List<KeyValueADO> erSelectedNews = new List<KeyValueADO>();
                    foreach (KeyValueADO er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.NAME);
                            erSelectedNews.Add(er);
                        }
                    }
                    this.MedicalHistoryInternal2Selected = new List<KeyValueADO>();
                    this.MedicalHistoryInternal2Selected.AddRange(erSelectedNews);
                }
                this.cboMedicalHistoryInternal2.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tạo data source cho Chẩn đoán UT cổ tử cung
        /// Mã "0": Chưa chẩn đoán
        /// Mã "1": Tiền ung thư cổ tử cung
        /// Mã "2": Ung thư cổ tử cung
        /// Mã "3": Bình thường
        /// </summary>
        private List<KeyValueADO> CreateCervicalCancerDxDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("0", "Chưa chẩn đoán"));
                result.Add(new KeyValueADO("1", "Tiền ung thư cổ tử cung"));
                result.Add(new KeyValueADO("2", "Ung thư cổ tử cung"));
                result.Add(new KeyValueADO("3", "Bình thường"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Điều trị tiền UT cổ tử cung
        /// Mã "1": Đốt điện/đốt laser/áp lạnh cổ tử cung
        /// Mã "2": LEEP cổ tử cung
        /// Mã "3": Khoét chóp cổ tử cung
        /// Mã "9": Khác
        /// </summary>
        private List<KeyValueADO> CreatePreCervicalCancerTreatDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Đốt điện/đốt laser/áp lạnh cổ tử cung"));
                result.Add(new KeyValueADO("2", "LEEP cổ tử cung"));
                result.Add(new KeyValueADO("3", "Khoét chóp cổ tử cung"));
                result.Add(new KeyValueADO("9", "Khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Tiền sử nội khoa
        /// Mã "0": Bình thường
        /// Mã "1": Bệnh tim
        /// Mã "2": Bệnh tăng huyết áp
        /// Mã "3": Bệnh đái tháo đường
        /// Mã "4": Có tiền sử sản khoa (sẩy thai/thai chết/phá thai, mổ lấy thai/foóc xép/giác hút, đẻ non, hiếm muộn, tai biến sản khoa...)
        /// Mã "5": Có tiền sử phụ khoa (u sinh dục, dị dạng sinh dục...)
        /// Mã "9": Khác
        /// </summary>
        private List<KeyValueADO> CreateMedicalHistoryInternalDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("0", "Bình thường"));
                result.Add(new KeyValueADO("1", "Bệnh tim"));
                result.Add(new KeyValueADO("2", "Bệnh tăng huyết áp"));
                result.Add(new KeyValueADO("3", "Bệnh đái tháo đường"));
                result.Add(new KeyValueADO("4", "Có tiền sử sản khoa (sẩy thai/thai chết/phá thai, mổ lấy thai/foóc xép/giác hút, đẻ non, hiếm muộn, tai biến sản khoa...)"));
                result.Add(new KeyValueADO("5", "Có tiền sử phụ khoa (u sinh dục, dị dạng sinh dục...)"));
                result.Add(new KeyValueADO("9", "Khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Nơi đẻ
        /// Mã "1": Trạm y tế/Điểm trạm y tế
        /// Mã "2": Phòng khám/Nhà hộ sinh
        /// Mã "3": Bệnh viện khu vực/Trung tâm y tế
        /// Mã "4": Bệnh viện tỉnh
        /// Mã "5": Bệnh viện trung ương
        /// Mã "6": Bệnh viện/CSYT tư nhân
        /// Mã "7": Ngoài CSYT (tại nhà, trên đường, ruộng, nương, ...)
        /// Mã "9": Nơi khác
        /// </summary>
        private List<KeyValueADO> CreateBirthplaceTypeDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Trạm y tế/Điểm trạm y tế"));
                result.Add(new KeyValueADO("2", "Phòng khám/Nhà hộ sinh"));
                result.Add(new KeyValueADO("3", "Bệnh viện khu vực/Trung tâm y tế"));
                result.Add(new KeyValueADO("4", "Bệnh viện tỉnh"));
                result.Add(new KeyValueADO("5", "Bệnh viện trung ương"));
                result.Add(new KeyValueADO("6", "Bệnh viện/CSYT tư nhân"));
                result.Add(new KeyValueADO("7", "Ngoài CSYT (tại nhà, trên đường, ruộng, nương, ...)"));
                result.Add(new KeyValueADO("9", "Nơi khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Cách đẻ
        /// Mã "1": Đẻ thường
        /// Mã "2": Mổ lấy thai
        /// Mã "3": Giác hút
        /// Mã "4": Fooc xép
        /// </summary>
        private List<KeyValueADO> CreateBirthMethodDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Đẻ thường"));
                result.Add(new KeyValueADO("2", "Mổ lấy thai"));
                result.Add(new KeyValueADO("3", "Giác hút"));
                result.Add(new KeyValueADO("4", "Fooc xép"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Tai biến sản khoa
        /// Mã "1": Băng huyết
        /// Mã "2": Tiền sản giật
        /// Mã "3": Sản giật
        /// Mã "4": Vỡ tử cung
        /// Mã "5": Nhiễm trùng
        /// Mã "6": Tai biến do phá thai
        /// Mã "7": Uốn ván sơ sinh
        /// Mã "8": Tắc mạch
        /// Mã "9": Tai biến khác
        /// </summary>
        private List<KeyValueADO> CreateMaternalComplicationDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Băng huyết"));
                result.Add(new KeyValueADO("2", "Tiền sản giật"));
                result.Add(new KeyValueADO("3", "Sản giật"));
                result.Add(new KeyValueADO("4", "Vỡ tử cung"));
                result.Add(new KeyValueADO("5", "Nhiễm trùng"));
                result.Add(new KeyValueADO("6", "Tai biến do phá thai"));
                result.Add(new KeyValueADO("7", "Uốn ván sơ sinh"));
                result.Add(new KeyValueADO("8", "Tắc mạch"));
                result.Add(new KeyValueADO("9", "Tai biến khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Tình trạng con
        /// Mã "1": Bình thường
        /// Mã "2": Ngạt
        /// Mã "3": Dị tật bẩm sinh
        /// Mã "9": Khác
        /// </summary>
        private List<KeyValueADO> CreateNewbornConditionDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Bình thường"));
                result.Add(new KeyValueADO("2", "Ngạt"));
                result.Add(new KeyValueADO("3", "Dị tật bẩm sinh"));
                result.Add(new KeyValueADO("9", "Khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Biện pháp tránh thai
        /// Mã "1": Đặt DCTC
        /// Mã "2": Bao cao su
        /// Mã "3": Thuốc viên
        /// Mã "4": Thuốc tiêm
        /// Mã "5": Thuốc cấy
        /// Mã "6": Triệt sản
        /// Mã "9": Khác
        /// </summary>
        private List<KeyValueADO> CreateContraceptionMethodDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Đặt DCTC"));
                result.Add(new KeyValueADO("2", "Bao cao su"));
                result.Add(new KeyValueADO("3", "Thuốc viên"));
                result.Add(new KeyValueADO("4", "Thuốc tiêm"));
                result.Add(new KeyValueADO("5", "Thuốc cấy"));
                result.Add(new KeyValueADO("6", "Triệt sản"));
                result.Add(new KeyValueADO("9", "Khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Tai biến tránh thai
        /// Mã "1": Chảy máu
        /// Mã "2": Nhiễm trùng
        /// Mã "3": Sốt
        /// Mã "4": Đau bụng
        /// Mã "9": Khác
        /// </summary>
        private List<KeyValueADO> CreateContraceptionComplicationDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Chảy máu"));
                result.Add(new KeyValueADO("2", "Nhiễm trùng"));
                result.Add(new KeyValueADO("3", "Sốt"));
                result.Add(new KeyValueADO("4", "Đau bụng"));
                result.Add(new KeyValueADO("9", "Khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Phương pháp phá thai
        /// Mã "1": Phá thai bằng thuốc
        /// Mã "2": Phá thai bằng phương pháp hút chân không
        /// Mã "3": Phá thai bằng phương pháp nong và gắp
        /// Mã "4": Phá thai ngoài cơ sở y tế
        /// Mã "9": Khác
        /// </summary>
        private List<KeyValueADO> CreateAbortionMethodDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Phá thai bằng thuốc"));
                result.Add(new KeyValueADO("2", "Phá thai bằng phương pháp hút chân không"));
                result.Add(new KeyValueADO("3", "Phá thai bằng phương pháp nong và gắp"));
                result.Add(new KeyValueADO("4", "Phá thai ngoài cơ sở y tế"));
                result.Add(new KeyValueADO("9", "Khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Loại tai biến phá thai
        /// Mã "1": Ứ máu trong buồng tử cung
        /// Mã "2": Nhiễm khuẩn
        /// Mã "3": Rách cổ tử cung, thủng tử cung do chọc hoặc rách
        /// Mã "4": Còn thai
        /// Mã "5": Sót rau thai
        /// Mã "6": Băng huyết do sót rau, chấn thương và thủng tử cung
        /// Mã "9": Khác
        /// </summary>
        private List<KeyValueADO> CreateAbortionComplicationDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Ứ máu trong buồng tử cung"));
                result.Add(new KeyValueADO("2", "Nhiễm khuẩn"));
                result.Add(new KeyValueADO("3", "Rách cổ tử cung, thủng tử cung do chọc hoặc rách"));
                result.Add(new KeyValueADO("4", "Còn thai"));
                result.Add(new KeyValueADO("5", "Sót rau thai"));
                result.Add(new KeyValueADO("6", "Băng huyết do sót rau, chấn thương và thủng tử cung"));
                result.Add(new KeyValueADO("9", "Khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Lấy tên chẩn đoán từ mã
        /// </summary>
        private string GetCervicalCancerDxNameByCode(string code)
        {
            try
            {
                var data = CreateCervicalCancerDxDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên điều trị từ mã
        /// </summary>
        private string GetPreCervicalCancerTreatNameByCode(string code)
        {
            try
            {
                var data = CreatePreCervicalCancerTreatDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên tiền sử nội khoa từ mã
        /// </summary>
        private string GetMedicalHistoryInternalNameByCode(string code)
        {
            try
            {
                var data = CreateMedicalHistoryInternalDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên nơi đẻ từ mã
        /// </summary>
        private string GetBirthplaceTypeNameByCode(string code)
        {
            try
            {
                var data = CreateBirthplaceTypeDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên cách đẻ từ mã
        /// </summary>
        private string GetBirthMethodNameByCode(string code)
        {
            try
            {
                var data = CreateBirthMethodDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên tai biến sản khoa từ mã
        /// </summary>
        private string GetMaternalComplicationNameByCode(string code)
        {
            try
            {
                var data = CreateMaternalComplicationDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên tình trạng con từ mã
        /// </summary>
        private string GetNewbornConditionNameByCode(string code)
        {
            try
            {
                var data = CreateNewbornConditionDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên biện pháp tránh thai từ mã
        /// </summary>
        private string GetContraceptionMethodNameByCode(string code)
        {
            try
            {
                var data = CreateContraceptionMethodDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên tai biến tránh thai từ mã
        /// </summary>
        private string GetContraceptionComplicationNameByCode(string code)
        {
            try
            {
                var data = CreateContraceptionComplicationDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên phương pháp phá thai từ mã
        /// </summary>
        private string GetAbortionMethodNameByCode(string code)
        {
            try
            {
                var data = CreateAbortionMethodDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên tai biến phá thai từ mã
        /// </summary>
        private string GetAbortionComplicationNameByCode(string code)
        {
            try
            {
                var data = CreateAbortionComplicationDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên nơi tìm thấy con từ mã
        /// </summary>
        private string GetFoundLocationNameByCode(string code)
        {
            try
            {
                var data = CreateFoundLocationDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên tình trạng con từ mã
        /// </summary>
        private string GetChildStatusNameByCode(string code)
        {
            try
            {
                var data = CreateChildStatusDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy tên da kề da từ mã
        /// </summary>
        private string GetSkinToSkinNameByCode(string code)
        {
            try
            {
                var data = CreateSkinToSkinDataSource();
                var item = data.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Tạo data source cho Nơi tìm thấy con
        /// Mã "1": Trạm y tế/Điểm trạm y tế
        /// Mã "2": Phòng khám/Nhà hộ sinh
        /// Mã "3": Bệnh viện khu vực/Trung tâm y tế
        /// Mã "4": Bệnh viện tỉnh
        /// Mã "5": Bệnh viện trung ương
        /// Mã "6": Bệnh viện/CSYT tư nhân
        /// Mã "7": Ngoài CSYT (tại nhà, trên đường, ruộng, nương, ...)
        /// Mã "9": Nơi khác
        /// </summary>
        private List<KeyValueADO> CreateFoundLocationDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Trạm y tế/Điểm trạm y tế"));
                result.Add(new KeyValueADO("2", "Phòng khám/Nhà hộ sinh"));
                result.Add(new KeyValueADO("3", "Bệnh viện khu vực/Trung tâm y tế"));
                result.Add(new KeyValueADO("4", "Bệnh viện tỉnh"));
                result.Add(new KeyValueADO("5", "Bệnh viện trung ương"));
                result.Add(new KeyValueADO("6", "Bệnh viện/CSYT tư nhân"));
                result.Add(new KeyValueADO("7", "Ngoài CSYT (tại nhà, trên đường, ruộng, nương, ...)"));
                result.Add(new KeyValueADO("9", "Nơi khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Tình trạng con (Child Status)
        /// Mã "1": Bình thường
        /// Mã "2": Ngạt
        /// Mã "3": Dị tật bẩm sinh
        /// Mã "9": Khác
        /// </summary>
        private List<KeyValueADO> CreateChildStatusDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("1", "Bình thường"));
                result.Add(new KeyValueADO("2", "Ngạt"));
                result.Add(new KeyValueADO("3", "Dị tật bẩm sinh"));
                result.Add(new KeyValueADO("9", "Khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tạo data source cho Da kề da
        /// Mã "0": Trẻ không được thực hiện da kề da ngay sau sinh
        /// Mã "1": Trẻ được thực hiện da kề da trong vòng 30 phút
        /// Mã "2": Trẻ được thực hiện da kề da từ trên 30 đến dưới 90 phút
        /// Mã "3": Trẻ được thực hiện da kề da từ 90 phút trở lên
        /// </summary>
        private List<KeyValueADO> CreateSkinToSkinDataSource()
        {
            List<KeyValueADO> result = new List<KeyValueADO>();
            try
            {
                result.Add(new KeyValueADO("0", "Trẻ không được thực hiện da kề da ngay sau sinh"));
                result.Add(new KeyValueADO("1", "Trẻ được thực hiện da kề da trong vòng 30 phút"));
                result.Add(new KeyValueADO("2", "Trẻ được thực hiện da kề da từ trên 30 đến dưới 90 phút"));
                result.Add(new KeyValueADO("3", "Trẻ được thực hiện da kề da từ 90 phút trở lên"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách dân tộc từ BackendDataWorker
        /// </summary>
        private List<SDA.EFMODEL.DataModels.SDA_ETHNIC> GetEthnicDataSource()
        {
            List<SDA.EFMODEL.DataModels.SDA_ETHNIC> result = new List<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
            try
            {
                // Lấy dữ liệu từ RAM
                var ethnics = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                
                if (ethnics != null && ethnics.Count > 0)
                {
                    // Lọc dân tộc đang hoạt động
                    result = ethnics.Where(o => o.IS_ACTIVE == 1)
                                   .OrderBy(o => o.ETHNIC_NAME)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Khởi tạo combo Dân tộc với data từ SDA_ETHNIC
        /// </summary>
        private void InitComboEthnic(GridLookUpEdit combo, List<SDA.EFMODEL.DataModels.SDA_ETHNIC> data)
        {
            try
            {
                if (combo == null) return;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ETHNIC_CODE", "Mã", 80, 1));
                columnInfos.Add(new ColumnInfo("ETHNIC_NAME", "Tên", 200, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO("ETHNIC_NAME", "ETHNIC_CODE", columnInfos, false, 280);
                controlEditorADO.ImmediatePopup = true;

                ControlEditorLoader.Load(combo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Init Combo Giới tính (HIS_GENDER)

        /// <summary>
        /// Lấy danh sách giới tính từ BackendDataWorker (HIS_GENDER)
        /// </summary>
        private List<HIS_GENDER> GetGenderDataSource()
        {
            List<HIS_GENDER> result = new List<HIS_GENDER>();
            try
            {
                var genders = BackendDataWorker.Get<HIS_GENDER>();
                if (genders != null && genders.Count > 0)
                {
                    result = genders
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        .OrderBy(o => o.GENDER_NAME)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Khởi tạo combo giới tính với data từ HIS_GENDER
        /// </summary>
        private void InitComboGender(GridLookUpEdit combo, List<HIS_GENDER> data)
        {
            try
            {
                if (combo == null) return;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "Mã", 80, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "Tên", 200, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "GENDER_CODE", columnInfos, false, 280);
                controlEditorADO.ImmediatePopup = true;

                ControlEditorLoader.Load(combo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        
        #region Hàm chung cho các combo KeyValue (Mặc định 2 cột: Mã và Tên)
        
        /// <summary>
        /// Khởi tạo combo chung từ danh sách KeyValueADO
        /// Mặc định luôn có 2 cột: Mã và Tên
        /// </summary>
        private void InitComboKeyValue(GridLookUpEdit combo, List<KeyValueADO> data)
        {
            try
            {
                if (combo == null) return;

                // Mặc định 2 cột: Mã và Tên
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CODE", "Mã", 50, 1));
                columnInfos.Add(new ColumnInfo("NAME", "Tên", 200, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "CODE", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;

                ControlEditorLoader.Load(combo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        /// Lấy tên (NAME) từ mã (CODE) trong datasource
        /// </summary>
        private string GetNameByCode(List<KeyValueADO> dataSource, string code)
        {
            try
            {
                if (dataSource == null || string.IsNullOrEmpty(code)) 
                    return string.Empty;

                var item = dataSource.FirstOrDefault(x => x.CODE == code);
                return item != null ? item.NAME : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return string.Empty;
            }
        }

        #endregion
    }
}
