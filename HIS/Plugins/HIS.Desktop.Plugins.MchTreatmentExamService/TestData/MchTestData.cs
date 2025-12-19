using MCH.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.TestData
{
    /// <summary>
    /// Dữ liệu test cho MCH
    /// </summary>
    public class MchTestData
    {
        // Patient
        public MCH_PATIENT Patient { get; set; }
        
        // Treatment
        public MCH_TREATMENT Treatment { get; set; }
        
        // Exam Services
        public List<MCH_EXAM_SERVICE> ExamServices { get; set; }
        
        // Tab 1: Sàng lọc
        public List<MCH_SCREENING> Screenings { get; set; }
        
        // Tab 2: Khám thai
        public List<MCH_ANTENATAL_VISIT> AntenatalVisits { get; set; }
        
        // Tab 3: Sinh đẻ
        public List<MCH_BIRTH_INFO> BirthInfos { get; set; }
        public List<MCH_CHILD> Children { get; set; }
        
        // Tab 4: Tránh thai
        public List<MCH_CONTRACEPTION> Contraceptions { get; set; }
        
        // Tab 5: Phá thai
        public List<MCH_ABORTION> Abortions { get; set; }
        
        // View Data
        public List<V_MCH_EXAM_SERVICE> ViewExamServices { get; set; }

        public MchTestData()
        {
            InitializeTestData();
        }

        private void InitializeTestData()
        {
            long currentTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            long currentDate = long.Parse(DateTime.Now.ToString("yyyyMMdd") + "000000");
            
            // 1. Tạo Patient
            Patient = new MCH_PATIENT
            {
                ID = 1,
                CREATE_TIME = currentTime,
                CREATOR = "admin",
                MODIFIER = "admin",
                MODIFY_TIME = currentTime,
                IS_ACTIVE = 1,
                IS_DELETE = 0,
                
                PATIENT_CODE = "0000158188",
                FIRST_NAME = "Thị B",
                LAST_NAME = "Nguyễn",
                DOB = 19900515000000,
                IS_HAS_NOT_DAY_DOB = 0,
                GENDER_CODE = "02",
                GENDER_NAME = "Nữ",
                
                CCCD_NUMBER = "001090001234",
                CCCD_DATE = 20200101000000,
                CCCD_PLACE = "Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư",
                
                ETHNIC_CODE = "01",
                ETHNIC_NAME = "Kinh",
                
                CAREER_CODE = "01",
                CAREER_NAME = "Công nhân",
                
                ADDRESS = "Số 1 Đường Láng",
                COMMUNE_CODE = "001001",
                COMMUNE_NAME = "Phường Láng Thượng",
                DISTRICT_CODE = "0010",
                DISTRICT_NAME = "Quận Đống Đa",
                PROVINCE_CODE = "001",
                PROVINCE_NAME = "Hà Nội",
                
                PHONE_NUMBER = "0987654321",
                
                HT_ADDRESS = "Số 2 Đường Kim Mã",
                HT_COMMUNE_CODE = "001002",
                HT_COMMUNE_NAME = "Phường Kim Mã",
                HT_DISTRICT_CODE = "0011",
                HT_DISTRICT_NAME = "Quận Ba Đình",
                HT_PROVINCE_CODE = "001",
                HT_PROVINCE_NAME = "Hà Nội"
            };

            // 2. Tạo Treatment
            Treatment = new MCH_TREATMENT
            {
                ID = 1,
                CREATE_TIME = currentTime,
                CREATOR = "admin",
                MODIFIER = "admin",
                MODIFY_TIME = currentTime,
                IS_ACTIVE = 1,
                IS_DELETE = 0,
                
                TREATMENT_CODE = "000000158188",
                PATIENT_ID = 1,
                
                BRANCH_MEDI_ORG_CODE = "01001",
                BRANCH_MEDI_ORG_NAME = "Bệnh viện Phụ Sản Trung Ương",
                
                DIRECTOR_LOGINNAME = "bsgiam.doc",
                DIRECTOR_USERNAME = "BS. Nguyễn Văn Giám Đốc",
                
                HOSP_SUBS_DIRECTOR_LOGINNAME = "bspho.gd",
                HOSP_SUBS_DIRECTOR_USERNAME = "BS. Trần Thị Phó",
                
                IN_TIME = currentDate,
                IN_DATE = currentDate,
                
                HEIN_CARD_NUMBER = "DN4011234567890",
                HEIN_CARD_ADDRESS = "Phường Láng Thượng, Quận Đống Đa, Hà Nội",
                HEIN_CARD_FROM_TIME = 20240101000000,
                HEIN_CARD_TO_TIME = 20241231000000,
                HEIN_MEDI_ORG_CODE = "01001",
                HEIN_MEDI_ORG_NAME = "Bệnh viện Phụ Sản Trung Ương",
                
                JOIN_5_YEAR = "1",
                LIVE_AREA_CODE = "K1",
                PAID_6_MONTH = "1"
            };

            // 3. Tạo Exam Services
            ExamServices = new List<MCH_EXAM_SERVICE>();
            
            // Exam Service 1: Sàng lọc ung thư cổ tử cung (Treatment 1)
            ExamServices.Add(new MCH_EXAM_SERVICE
            {
                ID = 1,
                CREATE_TIME = currentTime,
                CREATOR = "admin",
                MODIFIER = "admin",
                MODIFY_TIME = currentTime,
                IS_ACTIVE = 1,
                IS_DELETE = 0,
                
                TREATMENT_ID = 1,
                EXAM_SERVICE_TYPE_ID = 1, // Sàng lọc
                
                MEDI_ORG_CODE = "01001",
                MEDI_ORG_NAME = "Bệnh viện Phụ Sản Trung Ương",
                
                EXECUTE_LOGINNAME = "bsnguyen.a",
                EXECUTE_USERNAME = "BS. Nguyễn Văn A",
                EXECUTE_TYPE = "Bác sỹ chuyên khoa I",
                
                SYNC_STATUS = 2,
                SYNC_DESCRIPTION = null
            });
            
            // Exam Service 2: Khám thai (Treatment 1)
            ExamServices.Add(new MCH_EXAM_SERVICE
            {
                ID = 2,
                CREATE_TIME = currentTime,
                CREATOR = "admin",
                MODIFIER = "admin",
                MODIFY_TIME = currentTime,
                IS_ACTIVE = 1,
                IS_DELETE = 0,
                
                TREATMENT_ID = 1,
                EXAM_SERVICE_TYPE_ID = 2, // Khám thai
                
                MEDI_ORG_CODE = "01001",
                MEDI_ORG_NAME = "Bệnh viện Phụ Sản Trung Ương",
                
                EXECUTE_LOGINNAME = "bstran.b",
                EXECUTE_USERNAME = "BS. Trần Thị B",
                EXECUTE_TYPE = "Bác sỹ chuyên khoa II",
                
                SYNC_STATUS = null,
                SYNC_DESCRIPTION = null
            });
            
            // Exam Service 3: Sinh đẻ (Treatment 1)
            ExamServices.Add(new MCH_EXAM_SERVICE
            {
                ID = 3,
                CREATE_TIME = currentTime,
                CREATOR = "admin",
                MODIFIER = "admin",
                MODIFY_TIME = currentTime,
                IS_ACTIVE = 1,
                IS_DELETE = 0,
                
                TREATMENT_ID = 1,
                EXAM_SERVICE_TYPE_ID = 3, // Sinh đẻ
                
                MEDI_ORG_CODE = "01001",
                MEDI_ORG_NAME = "Bệnh viện Phụ Sản Trung Ương",
                
                EXECUTE_LOGINNAME = "bsle.c",
                EXECUTE_USERNAME = "BS. Lê Văn C",
                EXECUTE_TYPE = "Bác sỹ chuyên khoa II",
                
                SYNC_STATUS = 1,
                SYNC_DESCRIPTION = "Đồng bộ thành công"
            });
            
            // Exam Service 4: Tránh thai (Treatment 1)
            ExamServices.Add(new MCH_EXAM_SERVICE
            {
                ID = 4,
                CREATE_TIME = currentTime,
                CREATOR = "admin",
                MODIFIER = "admin",
                MODIFY_TIME = currentTime,
                IS_ACTIVE = 1,
                IS_DELETE = 0,
                
                TREATMENT_ID = 1,
                EXAM_SERVICE_TYPE_ID = 4, // Tránh thai
                
                MEDI_ORG_CODE = "01001",
                MEDI_ORG_NAME = "Bệnh viện Phụ Sản Trung Ương",
                
                EXECUTE_LOGINNAME = "bspham.d",
                EXECUTE_USERNAME = "BS. Phạm Thị D",
                EXECUTE_TYPE = "Bác sỹ chuyên khoa I",
                
                SYNC_STATUS = 2,
                SYNC_DESCRIPTION = null
            });
            
            // Exam Service 5: Phá thai (Treatment 1)
            ExamServices.Add(new MCH_EXAM_SERVICE
            {
                ID = 5,
                CREATE_TIME = currentTime,
                CREATOR = "admin",
                MODIFIER = "admin",
                MODIFY_TIME = currentTime,
                IS_ACTIVE = 1,
                IS_DELETE = 0,
                
                TREATMENT_ID = 1,
                EXAM_SERVICE_TYPE_ID = 5, // Phá thai
                
                MEDI_ORG_CODE = "01001",
                MEDI_ORG_NAME = "Bệnh viện Phụ Sản Trung Ương",
                
                EXECUTE_LOGINNAME = "bshoang.e",
                EXECUTE_USERNAME = "BS. Hoàng Văn E",
                EXECUTE_TYPE = "Bác sỹ chuyên khoa II",
                
                SYNC_STATUS = null,
                SYNC_DESCRIPTION = null
            });
            
            // ======= THÊM CÁC EXAM SERVICE VỚI TREATMENT_ID KHÁC (2, 3, 4) =======
            
            // Exam Service 6: Khám thai (Treatment 2 - Lần khám trước đó)
            ExamServices.Add(new MCH_EXAM_SERVICE
            {
                ID = 6,
                CREATE_TIME = long.Parse(DateTime.Now.AddDays(-30).ToString("yyyyMMddHHmmss")),
                CREATOR = "admin",
                MODIFIER = "admin",
                MODIFY_TIME = long.Parse(DateTime.Now.AddDays(-30).ToString("yyyyMMddHHmmss")),
                IS_ACTIVE = 1,
                IS_DELETE = 0,
                
                TREATMENT_ID = 2,
                EXAM_SERVICE_TYPE_ID = 2, // Khám thai
                
                MEDI_ORG_CODE = "01001",
                MEDI_ORG_NAME = "Bệnh viện Phụ Sản Trung Ương",
                
                EXECUTE_LOGINNAME = "bstran.b",
                EXECUTE_USERNAME = "BS. Trần Thị B",
                EXECUTE_TYPE = "Bác sỹ chuyên khoa II",
                
                SYNC_STATUS = 1,
                SYNC_DESCRIPTION = "Đồng bộ thành công"
            });
            
            // Exam Service 7: Sàng lọc (Treatment 3 - Lần khám trước đó 60 ngày)
            ExamServices.Add(new MCH_EXAM_SERVICE
            {
                ID = 7,
                CREATE_TIME = long.Parse(DateTime.Now.AddDays(-60).ToString("yyyyMMddHHmmss")),
                CREATOR = "admin",
                MODIFIER = "admin",
                MODIFY_TIME = long.Parse(DateTime.Now.AddDays(-60).ToString("yyyyMMddHHmmss")),
                IS_ACTIVE = 1,
                IS_DELETE = 0,
                
                TREATMENT_ID = 3,
                EXAM_SERVICE_TYPE_ID = 1, // Sàng lọc
                
                MEDI_ORG_CODE = "01001",
                MEDI_ORG_NAME = "Bệnh viện Phụ Sản Trung Ương",
                
                EXECUTE_LOGINNAME = "bsnguyen.a",
                EXECUTE_USERNAME = "BS. Nguyễn Văn A",
                EXECUTE_TYPE = "Bác sỹ chuyên khoa I",
                
                SYNC_STATUS = 1,
                SYNC_DESCRIPTION = "Đồng bộ thành công"
            });
            
            // Exam Service 8: Tránh thai (Treatment 4 - Lần khám trước đó 90 ngày)
            ExamServices.Add(new MCH_EXAM_SERVICE
            {
                ID = 8,
                CREATE_TIME = long.Parse(DateTime.Now.AddDays(-90).ToString("yyyyMMddHHmmss")),
                CREATOR = "admin",
                MODIFIER = "admin",
                MODIFY_TIME = long.Parse(DateTime.Now.AddDays(-90).ToString("yyyyMMddHHmmss")),
                IS_ACTIVE = 1,
                IS_DELETE = 0,
                
                TREATMENT_ID = 4,
                EXAM_SERVICE_TYPE_ID = 4, // Tránh thai
                
                MEDI_ORG_CODE = "01001",
                MEDI_ORG_NAME = "Bệnh viện Phụ Sản Trung Ương",
                
                EXECUTE_LOGINNAME = "bspham.d",
                EXECUTE_USERNAME = "BS. Phạm Thị D",
                EXECUTE_TYPE = "Bác sỹ chuyên khoa I",
                
                SYNC_STATUS = 1,
                SYNC_DESCRIPTION = "Đồng bộ thành công"
            });

            // 4. Tạo Screening (Tab 1)
            Screenings = new List<MCH_SCREENING>
            {
                new MCH_SCREENING
                {
                    ID = 1,
                    CREATE_TIME = currentTime,
                    CREATOR = "admin",
                    MODIFIER = "admin",
                    MODIFY_TIME = currentTime,
                    IS_ACTIVE = 1,
                    IS_DELETE = 0,
                    
                    EXAM_SERVICE_ID = 1,
                    
                    SCREENING_PURPOSE = "2", // Khám sàng lọc
                    GYNECOLOGY_TREAT = "0", // Không điều trị
                    CERVICAL_CANCER_DX = "1", // Bình thường
                    PRE_CERVICAL_CANCER_TREAT = "0", // Không điều trị
                    VIA_VILI_TEST = "1", // Âm tính
                    CYTOLOGY_TEST = "1", // Âm tính
                    HPV_TEST = "1", // Âm tính
                    BREAST_EXAM = "1", // Bình thường
                    BREAST_ULTRASOUND = "1", // Bình thường
                    MAMMOGRAPHY = "1" // Bình thường
                }
            };

            // 5. Tạo Antenatal Visit (Tab 2)
            AntenatalVisits = new List<MCH_ANTENATAL_VISIT>
            {
                new MCH_ANTENATAL_VISIT
                {
                    ID = 1,
                    CREATE_TIME = currentTime,
                    CREATOR = "admin",
                    MODIFIER = "admin",
                    MODIFY_TIME = currentTime,
                    IS_ACTIVE = 1,
                    IS_DELETE = 0,
                    
                    EXAM_SERVICE_ID = 2,
                    
                    MEDICAL_HISTORY_INTERNAL = "1", // Không có tiền sử
                    LAST_MENSTRUAL_PERIOD = 20240301000000,
                    GESTATIONAL_AGE = "20", // 20 tuần
                    ESTIMATED_BIRTH_DATE = 20241207000000,
                    GRAVIDA = "2", // Lần có thai thứ 2
                    
                    WEIGHT = "65", // kg
                    HEIGHT = "160", // cm
                    BLOOD_PRESSURE_SYSTOLIC = "120", // mmHg
                    BLOOD_PRESSURE_DIASTOLIC = "80", // mmHg
                    FUNDAL_HEIGHT = "20", // cm
                    ABDOMINAL_CIRCUMFERENCE = "90", // cm
                    
                    PELVIC_MEASUREMENT = "1", // Bình thường
                    ANEMIA_STATUS = "0", // Không thiếu máu
                    URINE_PROTEIN = "0", // Âm tính
                    
                    TEST_HIV = "1", // Âm tính
                    TEST_HEPATITIS_B = "1", // Âm tính
                    TEST_SYPHILIS = "1", // Âm tính
                    TEST_BLOOD_GLUCOSE = "1", // Bình thường
                    
                    PRENATAL_SCREENING = "1", // Đã làm
                    FETAL_HEART = "1", // Bình thường
                    FETAL_POSITION = "1", // Ngôi đầu
                    BIRTH_PREDICTION = "1" // Thuận lợi
                }
            };

            // 6. Tạo Birth Info (Tab 3 - Mẹ)
            BirthInfos = new List<MCH_BIRTH_INFO>
            {
                new MCH_BIRTH_INFO
                {
                    ID = 1,
                    CREATE_TIME = currentTime,
                    CREATOR = "admin",
                    MODIFIER = "admin",
                    MODIFY_TIME = currentTime,
                    IS_ACTIVE = 1,
                    IS_DELETE = 0,
                    
                    EXAM_SERVICE_ID = 3,
                    
                    GESTATIONAL_WEEKS = "39", // 39 tuần
                    BORN_TIME = 20241205000000,
                    BIRTHPLACE_TYPE = "1", // Bệnh viện
                    
                    BIRTH_ADDRESS = "Số 1 Phương Mai",
                    BIRTH_COMMUNE_CODE = "001003",
                    BIRTH_COMMUNE_NAME = "Phường Phương Mai",
                    BIRTH_DISTRICT_CODE = "0010",
                    BIRTH_DISTRICT_NAME = "Quận Đống Đa",
                    BIRTH_PROVINCE_CODE = "001",
                    BIRTH_PROVINCE_NAME = "Hà Nội",
                    
                    BIRTH_ORDER = 2, // Lần sinh thứ 2
                    ANTENATAL_VISITS = "1", // Đã khám thai 4 lần
                    
                    TEST_HIV_SCREEN = "1", // Âm tính
                    TEST_HIV_INTRAPARTUM = "1", // Âm tính
                    TEST_SYPHILIS_SCREEN = "1", // Âm tính
                    TEST_SYPHILIS_INTRAPARTUM = "1", // Âm tính
                    TEST_HEPB_SCREEN = "1", // Âm tính
                    TEST_HEPB_INTRAPARTUM = "1", // Âm tính
                    TEST_GLUCOSE_INTRAPARTUM = "1", // Bình thường
                    DIAGNOSIS_GDM = "0", // Không đái tháo đường
                    
                    FULL_TETANUS_DOSE = "1", // Đủ mũi
                    
                    TERM_BIRTHS = "1", // 1 lần sinh đủ tháng trước đây
                    PRETERM_BIRTH = "0", // Không sinh non
                    MISCARRIAGE = "0", // Không sảy thai
                    CHILD_COUNT = "1", // 1 con
                    
                    BIRTH_METHOD = "1", // Đẻ thường
                    MATERNAL_COMPLICATION = null, // Không có tai biến
                    
                    NUMBER_NEWBORN_BIRTH = "1", // 1 con
                    NEWBORN_ALIVE = "1", // 1 con sống
                    NEWBORN_CONDITION = "1", // Bình thường
                    
                    FIRST_WEEK_CARE = "1", // Có chăm sóc tuần đầu
                    WEEK_2_TO_6_CARE = "1", // Có chăm sóc tuần 2-6
                    
                    MIDWIFE_TYPE = "2", // Bác sỹ sản khoa
                    
                    MOTHER_DEATH = "0", // Mẹ không tử vong
                    ICD_SUB_CODE = "Z37.0",
                    ICD_TEXT = "Sinh con sống đơn thai",
                    MOTHER_DEATH_CAUSE = null
                }
            };

            // 7. Tạo Child (Tab 3 - Con)
            Children = new List<MCH_CHILD>
            {
                new MCH_CHILD
                {
                    ID = 1,
                    CREATE_TIME = currentTime,
                    CREATOR = "admin",
                    MODIFIER = "admin",
                    MODIFY_TIME = currentTime,
                    IS_ACTIVE = 1,
                    IS_DELETE = 0,
                    
                    EXAM_SERVICE_ID = 3,
                    
                    CCCD_NUMBER = null, // Trẻ sơ sinh chưa có CCCD
                    
                    MENTAL_STATUS = "1", // Bình thường
                    MOTION_STATUS = "1", // Bình thường
                    
                    WEIGHT = "3200", // gram
                    HEIGHT = "50", // cm
                    HEAD_CIRCUM = "34", // cm
                    
                    IS_DEATH = 0, // Không tử vong
                    ABANDONED_CHILD = "0", // Không bị bỏ rơi
                    FOUND_LOCATION = null,
                    
                    LIVE_BIRTH = "1", // Sinh ra sống
                    CHILD_STATUS = "1", // Khỏe mạnh
                    
                    CHILD_NAME = "Nguyễn Thị C",
                    CHILD_GENDER = "02", // Nữ
                    
                    NEWBORN_SCREENING = "1", // Đã sàng lọc
                    ESSENTIAL_NEWBORN_CARE = "1", // Đã chăm sóc thiết yếu
                    SKIN_TO_SKIN = "1", // Đã da kề da
                    EARLY_BREASTFEEDING = "1", // Đã bú mẹ sớm
                    HEPB_VACCINE = "1", // Đã tiêm vaccine VGB
                    VITAMIN_K1 = "1", // Đã tiêm vitamin K1
                    KANGAROO_CARE = "0", // Không cần KMC
                    
                    HAS_BIRTH_CERTIFICATE = "1", // Có giấy chứng sinh
                    BIRTH_CERTIFICATE_CODE = "GCS202412050001",
                    BIRTH_CERTIFICATE_ROUND = "0", // Cấp lần đầu
                    BIRTH_CERTIFICATE_DATE = 20241205000000,
                    
                    CHILD_BIRTH_DATE = 20241205083000,
                    
                    BIRTH_ADDRESS = "Số 1 Phương Mai",
                    BIRTH_COMMUNE_CODE = "001003",
                    BIRTH_COMMUNE_NAME = "Phường Phương Mai",
                    BIRTH_DISTRICT_CODE = "0010",
                    BIRTH_DISTRICT_NAME = "Quận Đống Đa",
                    BIRTH_PROVINCE_CODE = "001",
                    BIRTH_PROVINCE_NAME = "Hà Nội",
                    
                    TEMPORARY_HEIN_CARD_NUMBER = "DN4011234567891",
                    
                    DELIVERY_ASSISTANT = "BS. Lê Văn C",
                    
                    ETHNIC_CODE = "01",
                    ETHNIC_NAME = "Kinh",
                    
                    CARE_WEEK_1 = "1", // Có chăm sóc tuần 1
                    CARE_WEEK_2_TO_6 = "1", // Có chăm sóc tuần 2-6
                    
                    FATHER_NAME = "Nguyễn Văn X"
                }
            };

            // 8. Tạo Contraception (Tab 4)
            Contraceptions = new List<MCH_CONTRACEPTION>
            {
                new MCH_CONTRACEPTION
                {
                    ID = 1,
                    CREATE_TIME = currentTime,
                    CREATOR = "admin",
                    MODIFIER = "admin",
                    MODIFY_TIME = currentTime,
                    IS_ACTIVE = 1,
                    IS_DELETE = 0,
                    
                    EXAM_SERVICE_ID = 4,
                    
                    CONTRACEPTION_METHOD = "3", // Vòng tránh thai
                    CONTRACEPTION_COMPLICATION = null // Không có tai biến
                }
            };

            // 9. Tạo Abortion (Tab 5)
            Abortions = new List<MCH_ABORTION>
            {
                new MCH_ABORTION
                {
                    ID = 1,
                    CREATE_TIME = currentTime,
                    CREATOR = "admin",
                    MODIFIER = "admin",
                    MODIFY_TIME = currentTime,
                    IS_ACTIVE = 1,
                    IS_DELETE = 0,
                    
                    EXAM_SERVICE_ID = 5,
                    
                    GESTATIONAL_WEEKS = "8", // 8 tuần
                    ABORTION_METHOD = "1", // Hút chân không
                    TISSUE_EXAMINATION_RESULT = "1", // Bình thường
                    ABORTION_COMPLICATION = null // Không có tai biến
                }
            };

            // 10. Tạo View Exam Service (Join data)
            ViewExamServices = new List<V_MCH_EXAM_SERVICE>();
            
            foreach (var examService in ExamServices)
            {
                // Lấy thông tin treatment tương ứng (có thể là treatment khác)
                string treatmentCode = "000000158188"; // Default treatment code
                long inTime = currentDate;
                long inDate = currentDate;
                
                // Nếu là treatment khác (ID 2, 3, 4) thì tạo treatment code khác
                if (examService.TREATMENT_ID == 2)
                {
                    treatmentCode = "000000158189"; // Treatment 2
                    inTime = long.Parse(DateTime.Now.AddDays(-30).ToString("yyyyMMdd") + "000000");
                    inDate = long.Parse(DateTime.Now.AddDays(-30).ToString("yyyyMMdd") + "000000");
                }
                else if (examService.TREATMENT_ID == 3)
                {
                    treatmentCode = "000000158190"; // Treatment 3
                    inTime = long.Parse(DateTime.Now.AddDays(-60).ToString("yyyyMMdd") + "000000");
                    inDate = long.Parse(DateTime.Now.AddDays(-60).ToString("yyyyMMdd") + "000000");
                }
                else if (examService.TREATMENT_ID == 4)
                {
                    treatmentCode = "000000158191"; // Treatment 4
                    inTime = long.Parse(DateTime.Now.AddDays(-90).ToString("yyyyMMdd") + "000000");
                    inDate = long.Parse(DateTime.Now.AddDays(-90).ToString("yyyyMMdd") + "000000");
                }
                
                ViewExamServices.Add(new V_MCH_EXAM_SERVICE
                {
                    // Exam Service fields
                    ID = examService.ID,
                    CREATE_TIME = examService.CREATE_TIME,
                    MODIFY_TIME = examService.MODIFY_TIME,
                    CREATOR = examService.CREATOR,
                    MODIFIER = examService.MODIFIER,
                    APP_CREATOR = examService.APP_CREATOR,
                    APP_MODIFIER = examService.APP_MODIFIER,
                    IS_ACTIVE = examService.IS_ACTIVE,
                    IS_DELETE = examService.IS_DELETE,
                    GROUP_CODE = examService.GROUP_CODE,
                    
                    TREATMENT_ID = examService.TREATMENT_ID,
                    EXAM_SERVICE_TYPE_ID = examService.EXAM_SERVICE_TYPE_ID,
                    MEDI_ORG_CODE = examService.MEDI_ORG_CODE,
                    MEDI_ORG_NAME = examService.MEDI_ORG_NAME,
                    EXECUTE_LOGINNAME = examService.EXECUTE_LOGINNAME,
                    EXECUTE_USERNAME = examService.EXECUTE_USERNAME,
                    EXECUTE_TYPE = examService.EXECUTE_TYPE,
                    SYNC_STATUS = examService.SYNC_STATUS,
                    SYNC_DESCRIPTION = examService.SYNC_DESCRIPTION,
                    
                    // Treatment fields (có thể khác nhau cho mỗi exam service)
                    TREATMENT_CODE = treatmentCode,
                    PATIENT_ID = Treatment.PATIENT_ID,
                    BRANCH_MEDI_ORG_CODE = Treatment.BRANCH_MEDI_ORG_CODE,
                    BRANCH_MEDI_ORG_NAME = Treatment.BRANCH_MEDI_ORG_NAME,
                    DIRECTOR_LOGINNAME = Treatment.DIRECTOR_LOGINNAME,
                    DIRECTOR_USERNAME = Treatment.DIRECTOR_USERNAME,
                    HOSP_SUBS_DIRECTOR_LOGINNAME = Treatment.HOSP_SUBS_DIRECTOR_LOGINNAME,
                    HOSP_SUBS_DIRECTOR_USERNAME = Treatment.HOSP_SUBS_DIRECTOR_USERNAME,
                    IN_TIME = inTime,
                    IN_DATE = inDate,
                    HEIN_CARD_ADDRESS = Treatment.HEIN_CARD_ADDRESS,
                    HEIN_CARD_FROM_TIME = Treatment.HEIN_CARD_FROM_TIME,
                    HEIN_CARD_NUMBER = Treatment.HEIN_CARD_NUMBER,
                    HEIN_CARD_TO_TIME = Treatment.HEIN_CARD_TO_TIME,
                    HEIN_MEDI_ORG_CODE = Treatment.HEIN_MEDI_ORG_CODE,
                    HEIN_MEDI_ORG_NAME = Treatment.HEIN_MEDI_ORG_NAME,
                    JOIN_5_YEAR = Treatment.JOIN_5_YEAR,
                    LIVE_AREA_CODE = Treatment.LIVE_AREA_CODE,
                    PAID_6_MONTH = Treatment.PAID_6_MONTH,
                    
                    // Exam Service Type
                    EXAM_SERVICE_TYPE_CODE = GetExamServiceTypeCode(examService.EXAM_SERVICE_TYPE_ID),
                    EXAM_SERVICE_TYPE_NAME = GetExamServiceTypeName(examService.EXAM_SERVICE_TYPE_ID),
                    
                    // Patient fields (GIỐNG NHAU cho tất cả - cùng PATIENT_CODE)
                    PATIENT_CODE = Patient.PATIENT_CODE, // 0000158188 - GIỐNG NHAU
                    FIRST_NAME = Patient.FIRST_NAME,
                    LAST_NAME = Patient.LAST_NAME,
                    DOB = Patient.DOB,
                    IS_HAS_NOT_DAY_DOB = Patient.IS_HAS_NOT_DAY_DOB,
                    GENDER_CODE = Patient.GENDER_CODE,
                    GENDER_NAME = Patient.GENDER_NAME,
                    CCCD_DATE = Patient.CCCD_DATE,
                    CCCD_NUMBER = Patient.CCCD_NUMBER,
                    CCCD_PLACE = Patient.CCCD_PLACE,
                    ETHNIC_CODE = Patient.ETHNIC_CODE,
                    ETHNIC_NAME = Patient.ETHNIC_NAME,
                    CAREER_CODE = Patient.CAREER_CODE,
                    CAREER_NAME = Patient.CAREER_NAME,
                    ADDRESS = Patient.ADDRESS,
                    COMMUNE_CODE = Patient.COMMUNE_CODE,
                    COMMUNE_NAME = Patient.COMMUNE_NAME,
                    DISTRICT_CODE = Patient.DISTRICT_CODE,
                    DISTRICT_NAME = Patient.DISTRICT_NAME,
                    PROVINCE_CODE = Patient.PROVINCE_CODE,
                    PROVINCE_NAME = Patient.PROVINCE_NAME,
                    PHONE_NUMBER = Patient.PHONE_NUMBER,
                    HT_ADDRESS = Patient.HT_ADDRESS,
                    HT_COMMUNE_CODE = Patient.HT_COMMUNE_CODE,
                    HT_COMMUNE_NAME = Patient.HT_COMMUNE_NAME,
                    HT_DISTRICT_CODE = Patient.HT_DISTRICT_CODE,
                    HT_DISTRICT_NAME = Patient.HT_DISTRICT_NAME,
                    HT_PROVINCE_CODE = Patient.HT_PROVINCE_CODE,
                    HT_PROVINCE_NAME = Patient.HT_PROVINCE_NAME,
                    
                    // Virtual fields
                    VIR_ADDRESS = $"{Patient.ADDRESS}, {Patient.COMMUNE_NAME}, {Patient.DISTRICT_NAME}, {Patient.PROVINCE_NAME}",
                    VIR_HT_ADDRESS = $"{Patient.HT_ADDRESS}, {Patient.HT_COMMUNE_NAME}, {Patient.HT_DISTRICT_NAME}, {Patient.HT_PROVINCE_NAME}",
                    VIR_PATIENT_NAME = $"{Patient.LAST_NAME} {Patient.FIRST_NAME}"
                });
            }
        }

        private string GetExamServiceTypeCode(long typeId)
        {
            switch (typeId)
            {
                case 1: return "SANGLOCUTCTC";
                case 2: return "KHAMTHAI";
                case 3: return "SINHDE";
                case 4: return "TRANHTHAI";
                case 5: return "PHATHAI";
                default: return "";
            }
        }

        private string GetExamServiceTypeName(long typeId)
        {
            switch (typeId)
            {
                case 1: return "Sàng lọc ung thư cổ tử cung";
                case 2: return "Khám thai";
                case 3: return "Sinh đẻ";
                case 4: return "Tránh thai";
                case 5: return "Phá thai";
                default: return "";
            }
        }
    }
}
