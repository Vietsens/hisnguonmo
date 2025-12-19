Tạo bảng bệnh nhân MCH_PATIENT
PATIENT_CODE - VARCHAR2(10 BYTE)*- UK
FIRST_NAME - VARCHAR2(30 BYTE)*
LAST_NAME - VARCHAR2(70 BYTE)*
DOB - NUMBER(14,0)*
IS_HAS_NOT_DAY_DOB - NUMBER(2,0)
GENDER_CODE - VARCHAR2(2 BYTE)*
GENDER_NAME - VARCHAR2(100 BYTE)*
CCCD_DATE - NUMBER(14,0)
CCCD_NUMBER - VARCHAR2(12 BYTE)
CCCD_PLACE - VARCHAR2(100 BYTE)
ETHNIC_CODE - VARCHAR2(2 BYTE)
ETHNIC_NAME - VARCHAR2(100 BYTE)
CAREER_CODE - VARCHAR2(10 BYTE)
CAREER_NAME - VARCHAR2(1000 BYTE)
ADDRESS - VARCHAR2(200 BYTE)
COMMUNE_CODE - VARCHAR2(6 BYTE)
COMMUNE_NAME - VARCHAR2(100 BYTE)
DISTRICT_CODE - VARCHAR2(4 BYTE)
DISTRICT_NAME - VARCHAR2(100 BYTE)
PROVINCE_CODE - VARCHAR2(3 BYTE)
PROVINCE_NAME - VARCHAR2(100 BYTE)
PHONE_NUMBER - VARCHAR2(20 BYTE)
HT_ADDRESS - VARCHAR2(200 BYTE)
HT_COMMUNE_CODE - VARCHAR2(6 BYTE)
HT_COMMUNE_NAME - VARCHAR2(100 BYTE)
HT_DISTRICT_CODE - VARCHAR2(4 BYTE)
HT_DISTRICT_NAME - VARCHAR2(100 BYTE)
HT_PROVINCE_CODE - VARCHAR2(3 BYTE)
HT_PROVINCE_NAME - VARCHAR2(100 BYTE)
VIR_ADDRESS: Ghép thông tin địa chỉ bệnh nhân
VIR_HT_ADDRESS: Ghép thông tin địa chỉ hiện tại
VIR_PATIENT_NAME: Ghép thông tin họ và tên
Tạo bảng hồ sơ MCH_TREATMENT
TREATMENT_CODE - VARCHAR2(12 BYTE)* - UK
PATIENT_ID - NUMBER(19,0)*: FK MCH_PATIENT
BRANCH_MEDI_ORG_CODE - VARCHAR2(10 BYTE)*
BRANCH_MEDI_ORG_NAME - VARCHAR2(500 BYTE)*
DIRECTOR_LOGINNAME - VARCHAR2(50 BYTE)*
DIRECTOR_USERNAME - VARCHAR2(100 BYTE)*
HOSP_SUBS_DIRECTOR_LOGINNAME - VARCHAR2(50 BYTE)
HOSP_SUBS_DIRECTOR_USERNAME - VARCHAR2(100 BYTE)
IN_TIME - NUMBER(14,0)*
IN_DATE - NUMBER(14,0)*
HEIN_CARD_ADDRESS - VARCHAR2(1000 BYTE)
HEIN_CARD_FROM_TIME - NUMBER(14,0)
HEIN_CARD_NUMBER - VARCHAR2(15 BYTE)
HEIN_CARD_TO_TIME - NUMBER(14,0)
HEIN_MEDI_ORG_CODE - VARCHAR2(6 BYTE)
HEIN_MEDI_ORG_NAME - VARCHAR2(500 BYTE)
JOIN_5_YEAR - VARCHAR2(2 BYTE)
LIVE_AREA_CODE - VARCHAR2(2 BYTE)
PAID_6_MONTH - VARCHAR2(2 BYTE)
Tạo bảng loại khám MCH_EXAM_SERVICE_TYPE: 1-khám thai, 2-sinh đẻ; 3-tránh thai; 4-phá thai; 5-khám sàng lọc
EXAM_SERVICE_TYPE_CODE - VARCHAR2(20 BYTE)
EXAM_SERVICE_TYPE_NAME - VARCHAR2(500 BYTE)
Tạo bảng thông tin đợt khám MCH_EXAM_SERVICE
TREATMENT_ID - NUMBER(19,0)*
EXAM_SERVICE_TYPE_ID - NUMBER(19,0)*: FK MCH_EXAM_SERVICE_TYPE
MEDI_ORG_CODE - VARCHAR2(10 BYTE)*
MEDI_ORG_NAME - VARCHAR2(500 BYTE)*
EXECUTE_LOGINNAME - VARCHAR2(50 BYTE)
EXECUTE_USERNAME - VARCHAR2(100 BYTE)
EXECUTE_TYPE -  VARCHAR2(20 BYTE): NGUOI_THUC_HIEN/NGUOI_KHAM. Ghi trình độ chuyên môn của người khám
SYNC_STATUS - NUMBER(19,0):Trạng thái đồng bộ. 1-thành công; 2-thất bại
SYNC_DESCRIPTION - VARCHAR2(2000 BYTE): lý do thất bại
Tạo bảng khám sàng lọc MCH_SCREENING
EXAM_SERVICE_ID - NUMBER(19,0)*: FK MCH_EXAM_SERVICE
SCREENING_PURPOSE - VARCHAR2(20 BYTE)*: MUC_DICH_KHAM. 1-Khám bệnh; 2-Khám sàng lọc
GYNECOLOGY_TREAT - VARCHAR2(20 BYTE)*: DIEU_TRI_PK. Điều trị phụ khoa. 0-Không điều trị; 1-Có điều trị
CERVICAL_CANCER_DX - VARCHAR2(20 BYTE)*: CD_UTCT. Thông tin kết quả chẩn đoán ung thư cổ tử cun
PRE_CERVICAL_CANCER_TREAT - VARCHAR2(20 BYTE)*: DIEU_TRI_TIEN_UTCTC. Thông tin điều trị tiền ung thư cổ tử cung
VIA_VILI_TEST - VARCHAR2(20 BYTE)*: NGHIEM_PHAP_VIA_VILI. Thông tin về thực hiện nghiệm pháp VIA/VILI
CYTOLOGY_TEST - VARCHAR2(20 BYTE)*: XN_TE_BAO_HOC. Thông tin về xét nghiệm tế bào học cổ tử cung
HPV_TEST - VARCHAR2(20 BYTE)*: XN_HPV. Thông tin về xét nghiệm HPV.
BREAST_EXAM - VARCHAR2(20 BYTE)*: KHAM_VU. Thông tin về thực hiện khám lâm sàng vú để sàng lọc, phát hiện sớm ung thư vú
BREAST_ULTRASOUND - VARCHAR2(20 BYTE)*: SIEU_AM_TUYEN_VU. Thông tin về siêu âm tuyến vú
MAMMOGRAPHY - VARCHAR2(20 BYTE)*: CHUP_XQUANG_TUYEN_VU. Thông thông tin về chụp X-quang tuyến vú
Tạo bảng trẻ em MCH_CHILD
EXAM_SERVICE_ID - NUMBER(19,0)*: FK MCH_EXAM_SERVICE
CCCD_NUMBER - VARCHAR2(12 BYTE): SO_CCCD_NND / SO_CCCD_NND_TRE
MENTAL_STATUS - VARCHAR2(20 BYTE): PT_TINH_THAN. Thông tin về phát triển tinh thần của trẻ.
MOTION_STATUS - VARCHAR2(20 BYTE): PT_VAN_DONG. Thông tin về phát triển vận động của trẻ.
WEIGHT - VARCHAR2(20 BYTE): CAN_NANG_TRE/CAN_NANG_CON. Ghi số cân nặng của trẻ tính theo gram
HEIGHT - VARCHAR2(20 BYTE): CHIEU_CAO_TRE/CHIEU_DAI_CON. Ghi số đo chiều cao của trẻ tính theo cm
HEAD_CIRCUM - VARCHAR2(20 BYTE): VONG_DAU_TRE/VONG_DAU_CON. Ghi số đo vòng đầu của trẻ tính theo cm
IS_DEATH - NUMBER(2,0): TV_THAI_NHI. Thông tin tình trạng tử vong thai nhi
ABANDONED_CHILD - VARCHAR2(20 BYTE): BI_BO_ROI. Thông tin bị bỏ rơi
FOUND_LOCATION - NVARCHAR(1000 BYTE): NOI_TIM_THAY_TRE. Ghi nơi tìm thấy trẻ
LIVE_BIRTH - VARCHAR2(20 BYTE): TRE_DE_RA_SONG. Thông tin sống sót khi trẻ đẻ ra.
CHILD_STATUS - VARCHAR2(20 BYTE): TINH_TRANG_CON. Thông tin tình trạng con
CHILD_NAME - VARCHAR2(300 BYTE): HO_TEN_CON. Ghi họ tên dự định đặt cho con (Tên dự kiến này có thể thay đổi khi đăng ký khai sinh)
CHILD_GENDER - VARCHAR2(20 BYTE): GIOI_TINH_CON. Giới tính trẻ sơ sinh
NEWBORN_SCREENING - VARCHAR2(20 BYTE): SANG_LOC_SS. Thông tin về sàng lọc sơ sinh (xét nghiệm máu)
ESSENTIAL_NEWBORN_CARE - VARCHAR2(20 BYTE): CS_SO_SINH_TYS. Thông tin về chăm sóc thiết yếu trong và ngay sau đẻ/sau mổ lấy thai
SKIN_TO_SKIN - VARCHAR2(20 BYTE): DA_KE_DA. Thông tin về việc thực hiện da kề da ngay sau sinh
EARLY_BREASTFEEDING - VARCHAR2(20 BYTE): BU_ME_SOM. thông tin trẻ sơ sinh được bú mẹ sớm trong vòng 1 giờ đầu sau sinh.
HEPB_VACCINE - VARCHAR2(20 BYTE): TIEM_VACXIN_VGB. Thông tin về việc tiêm tiêm vắc xin viêm gan B cho trẻ sơ sinh
VITAMIN_K1 - VARCHAR2(20 BYTE): TIEM_VITAMIN_K1. Thông tin về tiêm vitamin K1 cho trẻ sơ sinh.
KANGAROO_CARE - VARCHAR2(20 BYTE): CS_KMC. thông tin trẻ được chăm sóc theo phương pháp Kangaroo
HAS_BIRTH_CERTIFICATE - VARCHAR2(20 BYTE): GIAY_CHUNG_SINH. Thông tin về cấp giấy chứng sinh\
BIRTH_CERTIFICATE_CODE - VARCHAR2(20 BYTE): MA_GCS. Mã giấy chứng sinh được quản lý tại cơ sở khám chữa bệnh theo quy định của Bộ Y tế.
BIRTH_CERTIFICATE_ROUND - VARCHAR2(20 BYTE): LAN_CAP. Ghi lần cấp Giấy chứng sinh: Mã “0”: Cấp lần đầu; Mã “1”: Cấp lại.
BIRTH_CERTIFICATE_DATE - NUMBER(14,0): NGAY_CHUNG_SINH. Ghi ngày cấp giấy chứng sinh
CHILD_BIRTH_DATE - NUMBER(14,0): NGAY_SINH_CON. Ghi ngày, giờ sinh của trẻ.
BIRTH_ADDRESS - VARCHAR2(200 BYTE):
BIRTH_COMMUNE_CODE - VARCHAR2(6 BYTE): mã xã
BIRTH_COMMUNE_NAME - VARCHAR2(120 BYTE)
BIRTH_DISTRICT_CODE - VARCHAR2(4 BYTE): mã huyện
BIRTH_DISTRICT_NAME - VARCHAR2(120 BYTE)
BIRTH_PROVINCE_CODE - VARCHAR2(3 BYTE): Mã tỉnh
BIRTH_PROVINCE_NAME - VARCHAR2(100 BYTE)
VIR_BIRTH_ADDRESS:  DIA_CHI_NOI_SINH_CON. Ghi địa chỉ nơi con được sinh ra theo cấu trúc: {mã tỉnh}:{mã xã}:{địa chỉ full text có cả tên tỉnh, xã theo chuẩn chung}
TEMPORARY_HEIN_CARD_NUMBER - VARCHAR2(15 BYTE): MA_THE_TAM. Ghi mã thẻ BHYT tạm thời của người con.
DELIVERY_ASSISTANT -  VARCHAR2(300 BYTE): HO_TEN_NGUOI_DO_DE. Ghi rõ họ, chữ đệm, tên của người đỡ đẻ
ETHNIC_CODE - VARCHAR2(2 BYTE): DAN_TOC_CON.
ETHNIC_NAME - VARCHAR2(100 BYTE): Tên dân tộc
CARE_WEEK_1 - VARCHAR2(20 BYTE): CS_TUAN_DAU. Chăm sóc tuần đầu. Ghi mã “1” nếu sản phụ và/hoặc sơ sinh được khám tại nhà trong vòng 1 tuần đầu sau khi rời cơ sở y tế
CARE_WEEK_2_TO_6 - VARCHAR2(20 BYTE): CS_TUAN_2_DEN_6. Ghi mã “1” nếu sản phụ và/hoặc sơ sinh được khám tại nhà từ tuần thứ 2 sau khi rời cơ sở y tế đến hết 6 tuần sau đẻ
FATHER_NAME -   VARCHAR2(300 BYTE): CHA. Họ tên cha
Tạo bảng thông tin sinh đẻ MCH_BIRTH_INFO
EXAM_SERVICE_ID - NUMBER(19,0)*: FK MCH_EXAM_SERVICE
GESTATIONAL_WEEKS - VARCHAR2(20 BYTE): TUAN_THAI
BORN_TIME - NUMBER(14,0): NGAY_DE. Ghi ngày đẻ của sản phụ gồm 8 ký tự yyyyMMdd
BIRTHPLACE_TYPE - VARCHAR2(20 BYTE): NOI_DE. Ghi nơi đẻ
BIRTH_ADDRESS - VARCHAR2(200 BYTE):
BIRTH_COMMUNE_CODE - VARCHAR2(6 BYTE): mã xã
BIRTH_COMMUNE_NAME - VARCHAR2(120 BYTE)
BIRTH_DISTRICT_CODE - VARCHAR2(4 BYTE): mã huyện
BIRTH_DISTRICT_NAME - VARCHAR2(120 BYTE)
BIRTH_PROVINCE_CODE - VARCHAR2(3 BYTE): Mã tỉnh
BIRTH_PROVINCE_NAME - VARCHAR2(100 BYTE)
VIR_BIRTH_ADDRESS: DIA_CHI_NOI_SINH_CON. Ghi địa chỉ nơi con được sinh ra theo cấu trúc: {mã tỉnh}:{mã xã}:{địa chỉ full text có cả tên tỉnh, xã theo chuẩn chung}
BIRTH_ORDER - NUMBER(19,0): LAN_SINH. Ghi số lần sinh con kể cả lần này
ANTENATAL_VISITS - VARCHAR2(20 BYTE): KHAM_THAI_4_LAN. Ghi thông tin về khám thai 4 lần trong 3 thời kỳ theo mã
TEST_HIV_SCREEN - VARCHAR2(20 BYTE): XN_HIV_MT. Thông tin về xét nghiệm HIV trước và trong thời gian mang thai của lần đẻ này
TEST_HIV_INTRAPARTUM - VARCHAR2(20 BYTE): XN_HIV_CD. Thông tin về xét nghiệm HIV khi chuyển dạ
TEST_SYPHILIS_SCREEN - VARCHAR2(20 BYTE): XN_GM_MT. Thông tin về xét nghiệm giang mai trước và trong thời gian mang thai của lần đẻ này
TEST_SYPHILIS_INTRAPARTUM - VARCHAR2(20 BYTE): XN_GM_CD. Thông tin về xét nghiệm giang mai khi chuyển dạ
TEST_HEPB_SCREEN- VARCHAR2(20 BYTE): XN_VGB_MT. Thông tin về xét nghiệm viêm gan B trước và trong thời gian mang thai của lần đẻ này
TEST_HEPB_INTRAPARTUM- VARCHAR2(20 BYTE): XN_VGB_CD. Thông tin về xét nghiệm viêm gan B khi chuyển dạ
TEST_GLUCOSE_INTRAPARTUM- VARCHAR2(20 BYTE): XN_DUONG_HUYET_CD. Thông tin về xét nghiệm đường huyết khi mang thai.
DIAGNOSIS_GDM -  VARCHAR2(20 BYTE): CHAN_DOAN_DTD. Thông tin về chẩn đoán đái tháo đường thai kỳ
FULL_TETANUS_DOSE - VARCHAR2(20 BYTE): TIEM_UV_DU_MUI. Thông tin về tiêm uốn ván đủ mũi
TERM_BIRTHS - VARCHAR2(20 BYTE): DE_DU_THANG. số lần sản phụ đẻ đủ tháng, không kể lần đẻ này
PRETERM_BIRTH - VARCHAR2(20 BYTE): DE_NON. Số lần sản phụ đẻ non
MISCARRIAGE - VARCHAR2(20 BYTE): SAY_THAI. Số lần sảy và phá thai
CHILD_COUNT - VARCHAR2(20 BYTE): SO_CON. Số con hiện có, không kể con của lần đẻ này
BIRTH_METHOD - VARCHAR2(20 BYTE): CACH_DE. Cách thức đẻ của sản phụ.
MATERNAL_COMPLICATION - VARCHAR2(100 BYTE): TAI_BIEN_SK. Ghi rõ các tai biến mã sản phụ/trẻ sơ sinh gặp phải trong khi đẻ và trong vòng 42 ngày sau đẻ theo mã
NUMBER_NEWBORN_BIRTH - VARCHAR2(20 BYTE): SO_CON_SINH_RA. Ghi số con sinh ra trong lần đẻ này
NEWBORN_ALIVE -  VARCHAR2(20 BYTE): TRE_DE_RA_SONG. Ghi số trẻ đẻ ra sống
NEWBORN_CONDITION - VARCHAR2(20 BYTE): TINH_TRANG_CON. Ghi thông tin tình trạng con
FIRST_WEEK_CARE - VARCHAR2(20 BYTE): CS_TUAN_DAU. Ghi mã “1” nếu sản phụ và/hoặc sơ sinh được khám tại nhà trong vòng 1 tuần đầu sau khi rời cơ sở y tế
WEEK_2_TO_6_CARE - VARCHAR2(20 BYTE): CS_TUAN_2_DEN_6. Ghi mã “1” nếu sản phụ và/hoặc sơ sinh được khám tại nhà từ tuần thứ 2 sau khi rời cơ sở y tế đến hết 6 tuần sau đẻ
MIDWIFE_TYPE - VARCHAR2(20 BYTE): NGUOI_DO_DE. Ghi trình độ chuyên môn của người đỡ đẻ (hoặc mổ lấy thai, thực hiện fooc xép, giác hút)
MOTHER_DEATH - VARCHAR2(20 BYTE): TU_VONG_ME. 
ICD_SUB_CODE - VARCHAR2(500 BYTE): MA_NHOM_ICD. Mã chẩn đoán
ICD_TEXT - VARCHAR2(4000 BYTE): Tên chẩn đoán
MOTHER_DEATH_CAUSE - VARCHAR2(4000 BYTE): NGUYEN_NHAN_TU_VONG_ME. Nguyên nhân mẹ tử vong.
Tạo bảng thông tin khám thai MCH_ANTENATAL_VISIT
EXAM_SERVICE_ID - NUMBER(19,0)*: FK MCH_EXAM_SERVICE
MEDICAL_HISTORY_INTERNAL -  VARCHAR2(100 BYTE): TIEN_SU_NOI_KHOA. Ghi thông tin về tiền sử theo mã. Trường hợp  có nhiều mã thì các mã được phân cách bằng dấu chấm phẩy “;”
LAST_MENSTRUAL_PERIOD - NUMBER(14,0): NGAY_DAU_KY_KINH_CUOI. Ghi ngày đầu kỳ kinh cuối cùng của thai phụ. yyyyMMdd.
GESTATIONAL_AGE -  VARCHAR2(100 BYTE): TUOI_THAI. Ghi tuổi thai thực tế (theo tuần), trong đó tuổi thai luôn luôn lớn hơn hoặc bằng 1 và nhỏ hơn hoặc bằng tuổi 42 tuần tuổi
ESTIMATED_BIRTH_DATE - NUMBER(14,0): NGAY_DU_KIEN_SINH. Ghi ngày dự sinh của thai phụ. yyyyMMdd
GRAVIDA - VARCHAR2(20 BYTE): LAN_CO_THAI. Ghi rõ đây là lần có thai thứ mấy, kể cả lần này và các lần đẻ, phá thai, sẩy thai trước đây
WEIGHT - VARCHAR2(20 BYTE): CAN_NANG. Ghi số cân nặng của thai phụ tính theo kg
HEIGHT - VARCHAR2(20 BYTE): CHIEU_CAO. Ghi số đo chiều cao của thai phụ tính theo cm
BLOOD_PRESSURE_SYSTOLIC - VARCHAR2(20 BYTE): HA_TAM_THU. Ghi số đo huyết áp tâm thu của thai phụ tính theo mmHg.
BLOOD_PRESSURE_DIASTOLIC - VARCHAR2(20 BYTE): HA_TAM_TRUONG. Ghi số đo huyết áp tâm trương của thai phụ tính theo mmHg.
FUNDAL_HEIGHT - VARCHAR2(20 BYTE): CAO_TC. Ghi chiều cao tử cung của thai phụ tính theo cm
ABDOMINAL_CIRCUMFERENCE -  VARCHAR2(20 BYTE): VONG_BUNG. Ghi kích thước vòng bụng của thai phụ tính theo cm
PELVIC_MEASUREMENT - VARCHAR2(20 BYTE): KHUNG_CHAU. Ghi thông tin khung chậu của thai phụ.
ANEMIA_STATUS - VARCHAR2(20 BYTE): THIEU_MAU. Ghi thông tin về tình trạng thiếu máu.
URINE_PROTEIN - VARCHAR2(20 BYTE): PROTEIN_NIEU. Ghi thông tin về xét nghiệm protein niệu
TEST_HIV -  VARCHAR2(20 BYTE): XN_HIV. Ghi thông tin về xét nghiệm HIV trong lần khám thai này.
TEST_HEPATITIS_B -  VARCHAR2(20 BYTE): XN_VGB. Ghi thông tin về xét nghiệm viêm gan B trong lần khám thai này.
TEST_SYPHILIS -  VARCHAR2(20 BYTE): XN_GIANG_MAI. Ghi thông tin về xét nghiệm giang mai trong lần khám thai này.
TEST_BLOOD_GLUCOSE -  VARCHAR2(20 BYTE): XN_DUONG_HUYET. Ghi thông tin về xét nghiệm đường huyết trong lần khám thai này.
PRENATAL_SCREENING -  VARCHAR2(20 BYTE): SANG_LOC_TRUOC_SINH. Ghi thông tin về khám sàng lọc trước sinh.
FETAL_HEART -  VARCHAR2(20 BYTE): TIM_THAI. Ghi thông tin về tim thai.
FETAL_POSITION -  VARCHAR2(20 BYTE): NGOI_THAI. Ghi thông tin về ngôi thai.
BIRTH_PREDICTION -  VARCHAR2(20 BYTE): TIEN_LUONG_DE. Ghi thông tin về tiên lượng cuộc đẻ.
Tạo bảng tránh thai MCH_CONTRACEPTION
EXAM_SERVICE_ID - NUMBER(19,0)*: FK MCH_EXAM_SERVICE
CONTRACEPTION_METHOD -  VARCHAR2(20 BYTE): BIEN_PHAP_TRANH_THAI. Ghi biện pháp tránh thai.
CONTRACEPTION_COMPLICATION -  VARCHAR2(20 BYTE): TAI_BIEN_TRANH_THAI. Ghi loại tai biến do thực hiện biện pháp tránh thai.
Tạo bảng phá thai MCH_ABORTION
EXAM_SERVICE_ID - NUMBER(19,0)*: FK MCH_EXAM_SERVICE
GESTATIONAL_WEEKS  -  VARCHAR2(20 BYTE): PT_TUAN_THAI. Ghi rõ tuần tuổi thai thực tế, trong đó tuổi thai luôn luôn lớn hơn hoặc bằng 1
ABORTION_METHOD -  VARCHAR2(20 BYTE): PHUONG_PHAP_PHA_THAI. Ghi phương pháp phá thai.
TISSUE_EXAMINATION_RESULT -  VARCHAR2(20 BYTE): KET_QUA_SOI_MO. Ghi thông tin về kết quả soi mô.
ABORTION_COMPLICATION -  VARCHAR2(20 BYTE): TAI_BIEN_PHA_THAI. Ghi loại tai biến phá thai.
//------------------------------------------------------------------------------------------------------
public class V_MCH_EXAM_SERVICE
{
    public long ID { get; set; }

    public long? CREATE_TIME { get; set; }

    public long? MODIFY_TIME { get; set; }

    public string CREATOR { get; set; }

    public string MODIFIER { get; set; }

    public string APP_CREATOR { get; set; }

    public string APP_MODIFIER { get; set; }

    public short? IS_ACTIVE { get; set; }

    public short? IS_DELETE { get; set; }

    public string GROUP_CODE { get; set; }

    public long TREATMENT_ID { get; set; }

    public long EXAM_SERVICE_TYPE_ID { get; set; }

    public string MEDI_ORG_CODE { get; set; }

    public string MEDI_ORG_NAME { get; set; }

    public string EXECUTE_LOGINNAME { get; set; }

    public string EXECUTE_USERNAME { get; set; }

    public string EXECUTE_TYPE { get; set; }

    public long? SYNC_STATUS { get; set; }

    public string SYNC_DESCRIPTION { get; set; }

    public string TREATMENT_CODE { get; set; }

    public long PATIENT_ID { get; set; }

    public string BRANCH_MEDI_ORG_CODE { get; set; }

    public string BRANCH_MEDI_ORG_NAME { get; set; }

    public string DIRECTOR_LOGINNAME { get; set; }

    public string DIRECTOR_USERNAME { get; set; }

    public string HOSP_SUBS_DIRECTOR_LOGINNAME { get; set; }

    public string HOSP_SUBS_DIRECTOR_USERNAME { get; set; }

    public long IN_TIME { get; set; }

    public long IN_DATE { get; set; }

    public string HEIN_CARD_ADDRESS { get; set; }

    public long? HEIN_CARD_FROM_TIME { get; set; }

    public string HEIN_CARD_NUMBER { get; set; }

    public long? HEIN_CARD_TO_TIME { get; set; }

    public string HEIN_MEDI_ORG_CODE { get; set; }

    public string HEIN_MEDI_ORG_NAME { get; set; }

    public string JOIN_5_YEAR { get; set; }

    public string LIVE_AREA_CODE { get; set; }

    public string PAID_6_MONTH { get; set; }

    public string EXAM_SERVICE_TYPE_CODE { get; set; }

    public string EXAM_SERVICE_TYPE_NAME { get; set; }

    public string PATIENT_CODE { get; set; }

    public string FIRST_NAME { get; set; }

    public string LAST_NAME { get; set; }

    public long DOB { get; set; }

    public short? IS_HAS_NOT_DAY_DOB { get; set; }

    public string GENDER_CODE { get; set; }

    public string GENDER_NAME { get; set; }

    public long? CCCD_DATE { get; set; }

    public string CCCD_NUMBER { get; set; }

    public string CCCD_PLACE { get; set; }

    public string ETHNIC_CODE { get; set; }

    public string ETHNIC_NAME { get; set; }

    public string CAREER_CODE { get; set; }

    public string CAREER_NAME { get; set; }

    public string ADDRESS { get; set; }

    public string COMMUNE_CODE { get; set; }

    public string COMMUNE_NAME { get; set; }

    public string DISTRICT_CODE { get; set; }

    public string DISTRICT_NAME { get; set; }

    public string PROVINCE_CODE { get; set; }

    public string PROVINCE_NAME { get; set; }

    public string PHONE_NUMBER { get; set; }

    public string HT_ADDRESS { get; set; }

    public string HT_COMMUNE_CODE { get; set; }

    public string HT_COMMUNE_NAME { get; set; }

    public string HT_DISTRICT_CODE { get; set; }

    public string HT_DISTRICT_NAME { get; set; }

    public string HT_PROVINCE_CODE { get; set; }

    public string HT_PROVINCE_NAME { get; set; }

    public string VIR_ADDRESS { get; set; }

    public string VIR_HT_ADDRESS { get; set; }

    public string VIR_PATIENT_NAME { get; set; }
}
//------------------------------------------------------------------------------------------------------
public class HIS_BABY
{
    public long ID { get; set; }

    public long? CREATE_TIME { get; set; }

    public long? MODIFY_TIME { get; set; }

    public string CREATOR { get; set; }

    public string MODIFIER { get; set; }

    public string APP_CREATOR { get; set; }

    public string APP_MODIFIER { get; set; }

    public short? IS_ACTIVE { get; set; }

    public short? IS_DELETE { get; set; }

    public string GROUP_CODE { get; set; }

    public long TREATMENT_ID { get; set; }

    public string BABY_NAME { get; set; }

    public long? GENDER_ID { get; set; }

    public long? BORN_TYPE_ID { get; set; }

    public long? BORN_RESULT_ID { get; set; }

    public long? BORN_POSITION_ID { get; set; }

    public long? BORN_TIME { get; set; }

    public long? BABY_ORDER { get; set; }

    public long? MONTH_COUNT { get; set; }

    public long? WEEK_COUNT { get; set; }

    public decimal? HEIGHT { get; set; }

    public decimal? WEIGHT { get; set; }

    public decimal? HEAD { get; set; }

    public string MIDWIFE { get; set; }

    public string FATHER_NAME { get; set; }

    public string ETHNIC_CODE { get; set; }

    public string ETHNIC_NAME { get; set; }

    public long? BIRTH_CERT_NUM { get; set; }

    public long? BIRTH_CERT_BOOK_ID { get; set; }

    public string ISSUER_LOGINNAME { get; set; }

    public string ISSUER_USERNAME { get; set; }

    public long? CURRENT_ALIVE { get; set; }

    public long? NUMBER_OF_PREGNANCIES { get; set; }

    public short? IS_DIFFICULT_BIRTH { get; set; }

    public short? IS_HAEMORRHAGE { get; set; }

    public short? IS_UTERINE_RUPTURE { get; set; }

    public short? IS_PUERPERAL { get; set; }

    public short? IS_BACTERIAL_CONTAMINATION { get; set; }

    public short? IS_TETANUS { get; set; }

    public short? IS_MOTHER_DEATH { get; set; }

    public string BIRTHPLACE { get; set; }

    public short? IS_FETAL_DEATH_22_WEEKS { get; set; }

    public short? IS_INJECT_K1 { get; set; }

    public short? IS_INJECT_B { get; set; }

    public long? POSTPARTUM_CARE { get; set; }

    public string METHOD_STYLE { get; set; }

    public string NOTE { get; set; }

    public long? DEATH_DATE { get; set; }

    public string HEIN_CARD_NUMBER_TMP { get; set; }

    public long? DEPARTMENT_ID { get; set; }

    public short? IS_SURGERY { get; set; }

    public long? NUMBER_CHILDREN_BIRTH { get; set; }

    public string SYNC_FAILD_REASON { get; set; }

    public long? SYNC_RESULT_TYPE { get; set; }

    public long? SYNC_TIME { get; set; }

    public long? ISSUED_DATE { get; set; }

    public short? BIRTHPLACE_TYPE { get; set; }

    public string BIRTH_PROVINCE_CODE { get; set; }

    public string BIRTH_PROVINCE_NAME { get; set; }

    public string BIRTH_DISTRICT_CODE { get; set; }

    public string BIRTH_DISTRICT_NAME { get; set; }

    public string BIRTH_COMMUNE_CODE { get; set; }

    public string BIRTH_COMMUNE_NAME { get; set; }

    public string BIRTH_HOSPITAL_CODE { get; set; }

    public string BIRTH_HOSPITAL_NAME { get; set; }

    public string DIRECTOR_LOGINNAME { get; set; }

    public string DIRECTOR_USERNAME { get; set; }

    public short? IS_REISSUED { get; set; }

    public virtual HIS_TREATMENT HIS_TREATMENT { get; set; }

    public virtual HIS_GENDER HIS_GENDER { get; set; }

    public virtual HIS_BORN_TYPE HIS_BORN_TYPE { get; set; }

    public virtual HIS_BORN_RESULT HIS_BORN_RESULT { get; set; }

    public virtual HIS_BORN_POSITION HIS_BORN_POSITION { get; set; }

    public virtual HIS_BIRTH_CERT_BOOK HIS_BIRTH_CERT_BOOK { get; set; }
}