/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * NẠP / LƯU bảng dữ liệu riêng của mẫu M4 — Sở Y tế TP.HCM (HIS_KSK_SYT_HCM).
 *
 * Bảng này quan hệ MỘT-MỘT với bảng KSK người từ 18 tuổi trở lên, chỉ dùng cho viện đã khai
 * báo cấu hình cổng. Trình tự:
 *      Lưu hồ sơ KSK (đã có)  ->  BE trả về bản ghi KSK kèm mã  ->  lưu tiếp bảng này
 * Phải lưu SAU vì cần mã bản ghi KSK để làm khóa liên kết; hồ sơ mới chỉ có mã sau khi BE lưu.
 *
 * VÌ SAO CÓ CẢ PHẦN NẠP: nếu chỉ lưu mà không nạp thì mở lại hồ sơ cũ sẽ thấy các ô trống,
 * bấm Lưu lần nữa là ghi trống lên dữ liệu đã có — mất dữ liệu. Nạp và lưu phải đi cùng nhau.
 *
 * CÁCH ÁNH XẠ CỘT: 15 mục khám × 5 cột + 32 răng = 107 cột theo đúng quy luật đặt tên
 * (tiền tố của mục + hậu tố, "TOOTH_" + số răng). Viết tay 107 dòng gán cho cả nạp lẫn lưu
 * rất dễ sai một chỗ mà không ai phát hiện, nên dùng bảng ánh xạ + phản chiếu, kèm một bước
 * TỰ KIỂM TRA tên cột lúc chạy lần đầu: sai tên thì ghi rõ vào nhật ký thay vì im lặng bỏ qua.
 */
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Khai báo =====

        private const string API__SYT_HCM_GET = "api/HisKskSytHcm/Get";
        private const string API__SYT_HCM_CREATE = "api/HisKskSytHcm/Create";
        private const string API__SYT_HCM_UPDATE = "api/HisKskSytHcm/Update";

        /// <summary>Chỉ số tab "Ksk trên 18 tuổi" — bảng này chỉ đi kèm tab đó.</summary>
        private const int TAB_INDEX__OVER_EIGHTEEN = 1;

        /// <summary>Bản ghi mẫu M4 của hồ sơ đang mở. null = hồ sơ chưa có bản ghi nào.</summary>
        private HIS_KSK_SYT_HCM currentKskSytHcm;

        /// <summary>
        /// Khóa mục khám (dùng trong tab Khám lâm sàng HCM) -> tiền tố cột của mục đó.
        /// Mỗi mục có 5 cột: `_IS_NORMAL`, `_PRE_ICD_CODE`, `_PRE_ICD_NAME`, `_ICD_CODE`, `_ICD_NAME`.
        /// </summary>
        private static readonly Dictionary<string, string> SYT_HCM_SECTION_COLUMN =
            new Dictionary<string, string>
            {
                { "noikhoa",      "EXAM_CIRCULATION"  },   // a) Tuần hoàn
                { "hohap",        "EXAM_RESPIRATORY"  },
                { "tieuhoa",      "EXAM_DIGESTION"    },
                { "thantietnieu", "EXAM_KIDNEY_URO"   },
                { "noitiet",      "EXAM_OEND"         },
                { "coxuongkhop",  "EXAM_MUSCLE_BONE"  },
                { "thankinh",     "EXAM_NEUROLOGICAL" },
                { "tamthan",      "EXAM_MENTAL"       },
                { "ngoaikhoa",    "EXAM_SURGERY"      },
                { "dalieu",       "EXAM_DERMATOLOGY"  },
                { "sankhoa",      "EXAM_OBSTETRIC"    },
                { "phukhoa",      "EXAM_GYNECOLOGY"   },
                { "mat",          "EXAM_EYE"          },
                { "tmh",          "EXAM_ENT"          },
                { "rhm",          "EXAM_STOMATOLOGY"  }
            };

        /// <summary>
        /// Ô trị số MỚI của mục Mắt -> cột riêng của bảng này.
        ///
        /// CHỈ gồm 8 ô mà bảng KSK cũ KHÔNG có cột. Các ô còn lại của tab HCM (thị lực không kính,
        /// có kính, thính lực tai trái/phải) trùng nội dung với tab "Khám lâm sàng" cũ và đã có cột
        /// ở bảng KSK — KHÔNG ghi từ đây để không đè giá trị của tab cũ bằng ô đang để trống.
        /// </summary>
        private static readonly Dictionary<string, string> SYT_HCM_EYE_COLUMN =
            new Dictionary<string, string>
            {
                { "mat_kinhlo_mp", "EXAM_EYESIGHT_PINHOLE_RIGHT" },
                { "mat_kinhlo_mt", "EXAM_EYESIGHT_PINHOLE_LEFT"  },
                { "mat_docau_mp",  "EXAM_EYE_SPHERE_RIGHT"       },
                { "mat_docau_mt",  "EXAM_EYE_SPHERE_LEFT"        },
                { "mat_dotru_mp",  "EXAM_EYE_CYLINDER_RIGHT"     },
                { "mat_dotru_mt",  "EXAM_EYE_CYLINDER_LEFT"      },
                { "mat_truc_mp",   "EXAM_EYE_AXIS_RIGHT"         },
                { "mat_truc_mt",   "EXAM_EYE_AXIS_LEFT"          }
            };

        private const string SYT_HCM_SUFFIX__IS_NORMAL = "_IS_NORMAL";
        private const string SYT_HCM_SUFFIX__PRE_CODE = "_PRE_ICD_CODE";
        private const string SYT_HCM_SUFFIX__PRE_NAME = "_PRE_ICD_NAME";
        private const string SYT_HCM_SUFFIX__CODE = "_ICD_CODE";
        private const string SYT_HCM_SUFFIX__NAME = "_ICD_NAME";

        private const string SYT_HCM_KEY__OBSTETRIC = "sankhoa";
        private const string SYT_HCM_KEY__GYNECOLOGY = "phukhoa";

        #endregion


        /// <summary>
        /// Chép mọi giá trị của bản ghi mẫu M3 sang bản ghi sắp lưu.
        ///
        /// Cần vì dịch vụ cập nhật thay cả dòng, mà màn hình chỉ gán được các ô đang dựng. Chép
        /// bằng phản chiếu để cột mới bổ sung về sau tự được giữ, không phải sửa lại hàm này.
        /// Chỉ chép cột dữ liệu (kiểu đơn giản), bỏ qua các thuộc tính liên kết của mô hình.
        /// </summary>
        private static void CopyKskSytHcmValues(HIS_KSK_SYT_HCM from, HIS_KSK_SYT_HCM to)
        {
            try
            {
                if (from == null || to == null) return;
                foreach (System.Reflection.PropertyInfo pi in typeof(HIS_KSK_SYT_HCM).GetProperties())
                {
                    if (!pi.CanRead || !pi.CanWrite) continue;
                    Type t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                    if (!t.IsPrimitive && t != typeof(string) && t != typeof(decimal) && t != typeof(DateTime))
                        continue;
                    pi.SetValue(to, pi.GetValue(from, null), null);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #region ===== NẠP =====

        /// <summary>
        /// Nạp bản ghi mẫu M4 của một hồ sơ KSK. Gọi lúc lấy dữ liệu đầu vào của màn hình.
        /// Viện chưa khai báo cấu hình cổng -> KHÔNG gọi dịch vụ.
        /// </summary>
        private void LoadKskSytHcm(long kskOverEighteenId)
        {
            try
            {
                currentKskSytHcm = null;
                if (!IsSytHcmDeclared() || kskOverEighteenId <= 0) return;

                CommonParam param = new CommonParam();
                HisKskSytHcmFilter filter = new HisKskSytHcmFilter();
                filter.KSK_OVER_EIGHTEEN_ID = kskOverEighteenId;
                List<HIS_KSK_SYT_HCM> list = new BackendAdapter(param)
                    .Get<List<HIS_KSK_SYT_HCM>>(API__SYT_HCM_GET, ApiConsumers.MosConsumer, filter, param);

                currentKskSytHcm = (list != null) ? list.FirstOrDefault() : null;
                LogSystem.Debug("SytHcm: nap ban ghi mau M4 theo ho so KSK -> "
                    + (currentKskSytHcm != null ? "co du lieu" : "chua co"));
            }
            catch (Exception ex) { LogSystem.Warn(ex); currentKskSytHcm = null; }
        }

        /// <summary>
        /// Đổ các ô hành chính (nằm ở trang KSK trên 18 tuổi, dựng ngay khi mở màn hình).
        /// Gọi SAU khi đã đổ Nguồn chi trả, vì bước cập nhật khóa/mở sẽ xóa giá trị ô đang khóa.
        /// </summary>
        /// <summary>
        /// Hồ sơ đã nạp lúc danh mục của Sở CHƯA về. Khi đó ô nhập còn giữ danh sách tạm nên mã của
        /// Sở không khớp mục nào; phải nạp lại sau khi đổ xong danh mục, nếu không ô hiển thị trống
        /// và lượt lưu kế tiếp sẽ ghi đè null lên giá trị cũ.
        /// </summary>
        private bool sytAdminFilledBeforeCatalog;

        private void FillKskSytHcmAdminControls()
        {
            try
            {
                // Ghi nhận trạng thái NGAY ĐẦU: nạp xong mà danh mục chưa về thì còn phải nạp lại.
                sytAdminFilledBeforeCatalog = !sytCatalogApplied;
                // Hồ sơ chưa có bản ghi mẫu M4 -> XÓA TRẮNG. Thoát sớm thì các ô còn giữ dữ liệu
                // của hồ sơ mở trước đó, người dùng sẽ thấy dữ liệu của bệnh nhân khác.
                if (currentKskSytHcm == null)
                {
                    ClearKskSytHcmAdminControls();

                    // Tab hoi benh cung phai xoa, neu khong no giu nguyen cau tra loi cua ho so mo
                    // truoc do — va lan luu tiep theo se ghi nham sang benh nhan nay.
                    ApplyInterviewHcmData(null, null, null);
                    AutoTickElderlyObject();
                    return;
                }
                HIS_KSK_SYT_HCM d = currentKskSytHcm;

                // Đối tượng khám và Nguồn chi trả: mã của cổng nằm ở bảng này, cột dùng chung
                // của bảng KSK để trống -> phải đổ lại từ đây.
                if (!string.IsNullOrWhiteSpace(d.SYT_PATIENT_TYPES))
                    SetKskObjectValue(d.SYT_PATIENT_TYPES);
                if (d.SYT_PAYSOURCE_ID.HasValue)
                    cboPaymentSource.EditValue = d.SYT_PAYSOURCE_ID.Value;

                // Đổ Nguồn chi trả xong mới tính lại khóa/mở, rồi mới đổ 2 ô phụ thuộc nó.
                UpdatePaySourceExtraEnable();

                if (txtKskPaySourceOther != null && txtKskPaySourceOther.Enabled)
                    txtKskPaySourceOther.Text = d.PAY_SOURCE_OTHER;
                // Ô này luôn chọn được nên đổ giá trị không cần xét trạng thái khóa.
                SetKskPaySourceDetailValue(d.SYT_PAY_SOURCE_DETAIL_ID);
                SetKskExamPlaceValue(d.SYT_EXAM_PLACE_ID);

                // Tab hoi benh lam sang HCM — 3 cot rieng, xem InterviewHcmData.
                FillInterviewHcmFromEntity(d);

                AutoTickElderlyObject();

                // Cụm "Đề nghị" — 2 cột này chỉ có sau khi bổ sung vào bảng; chưa có thì hàm đọc
                // ghi cảnh báo "KHONG co cot" và trả null, không làm hỏng gì.
                SetKskSuggestValue(GetSytHcmLong(d, "SYT_SUGGEST_ID"),
                    GetSytHcmStr(d, "SUGGEST_OTHER"));
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Đổ dữ liệu tab Khám lâm sàng HCM. Gọi SAU khi tab đã dựng xong các ô chọn bệnh
        /// (các ô này dựng trễ ở lần mở tab đầu tiên).
        /// </summary>
        private void FillKskSytHcmClinicalControls()
        {
            try
            {
                if (!isClinicalExamHcmInited) return;
                if (currentKskSytHcm == null)
                {
                    ClearKskSytHcmClinicalControls();
                    return;
                }
                HIS_KSK_SYT_HCM d = currentKskSytHcm;

                foreach (ClinicalExamHcmRow row in clinicalExamHcmRows)
                {
                    string prefix;
                    if (row == null || !SYT_HCM_SECTION_COLUMN.TryGetValue(row.Key, out prefix)) continue;

                    // Đổ hai ô chọn bệnh TRƯỚC, ô tích "chưa phát hiện bất thường" SAU CÙNG —
                    // gán ô tích trước sẽ xóa trống hai ô chọn bệnh vừa đổ vào.
                    SetHcmIcdValue(row.UcPreIcd,
                        GetSytHcmStr(d, prefix + SYT_HCM_SUFFIX__PRE_CODE),
                        GetSytHcmStr(d, prefix + SYT_HCM_SUFFIX__PRE_NAME));
                    SetHcmIcdValue(row.UcFinalIcd,
                        GetSytHcmStr(d, prefix + SYT_HCM_SUFFIX__CODE),
                        GetSytHcmStr(d, prefix + SYT_HCM_SUFFIX__NAME));

                    short? isNormal = GetSytHcmShort(d, prefix + SYT_HCM_SUFFIX__IS_NORMAL);
                    if (row.ChkNormal != null) row.ChkNormal.Checked = (isNormal == 1);

                    if (row.ChkRefuse != null)
                    {
                        if (row.Key == SYT_HCM_KEY__OBSTETRIC)
                            row.ChkRefuse.Checked = (d.IS_REFUSE_OBSTETRIC == 1);
                        else if (row.Key == SYT_HCM_KEY__GYNECOLOGY)
                            row.ChkRefuse.Checked = (d.IS_REFUSE_GYNECOLOGY == 1);
                    }

                    // Phụ khoa là mục tách mới nên phân loại / người khám có cột riêng ở bảng này;
                    // các mục khác dùng cột sẵn có của bảng KSK, tab "Khám lâm sàng" cũ đã lo.
                    if (row.Key == SYT_HCM_KEY__GYNECOLOGY)
                    {
                        if (row.CboRank != null)
                            row.CboRank.EditValue = d.EXAM_GYNECOLOGY_RANK;
                        if (row.CboLoginName != null)
                            row.CboLoginName.EditValue = d.EXAM_GYNECOLOGY_LOGINNAME;
                    }

                    // Trị số mắt: 8 ô riêng của bảng này.
                    foreach (KeyValuePair<string, string> f in SYT_HCM_EYE_COLUMN)
                    {
                        if (!row.Fields.ContainsKey(f.Key)) continue;
                        row.Fields[f.Key].Text = GetSytHcmStr(d, f.Value);
                    }
                }

                FillKskSytHcmTeeth(d);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Xóa trắng các ô hành chính của mẫu M4 khi hồ sơ chưa có dữ liệu.</summary>
        private void ClearKskSytHcmAdminControls()
        {
            try
            {
                if (txtKskPaySourceOther != null) txtKskPaySourceOther.Text = string.Empty;
                SetKskPaySourceDetailValue(null);
                SetKskExamPlaceValue(null);
                SetKskSuggestValue(null, null);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Xóa trắng tab Khám lâm sàng HCM khi hồ sơ chưa có dữ liệu mẫu M4.
        /// Thứ tự: xóa hai ô chọn bệnh TRƯỚC, ô tích SAU — gán ô tích trước sẽ khóa và xóa hai ô kia,
        /// làm mất hiệu lực của lệnh xóa.
        /// </summary>
        private void ClearKskSytHcmClinicalControls()
        {
            try
            {
                foreach (ClinicalExamHcmRow row in clinicalExamHcmRows)
                {
                    if (row == null) continue;
                    SetHcmIcdValue(row.UcPreIcd, null, null);
                    SetHcmIcdValue(row.UcFinalIcd, null, null);
                    if (row.ChkNormal != null) row.ChkNormal.Checked = false;
                    if (row.ChkRefuse != null) row.ChkRefuse.Checked = false;

                    // Phân loại / người khám của Phụ khoa lưu ở bảng mẫu M4 nên phải xóa theo.
                    // 14 mục còn lại dùng cột của bảng KSK, tab "Khám lâm sàng" cũ tự lo.
                    if (row.Key == SYT_HCM_KEY__GYNECOLOGY)
                    {
                        if (row.CboRank != null) row.CboRank.EditValue = null;
                        if (row.CboLoginName != null) row.CboLoginName.EditValue = null;
                    }

                    // CHỈ xóa 8 ô trị số lưu ở bảng mẫu M3. Bốn ô thị lực không kính/có kính và
                    // bốn ô thính lực đã nối với tab cũ nên tab cũ tự lo — xóa ở đây là xóa luôn
                    // giá trị trong cột của bảng KSK.
                    foreach (KeyValuePair<string, string> f in SYT_HCM_EYE_COLUMN)
                        if (row.Fields.ContainsKey(f.Key)) row.Fields[f.Key].Text = string.Empty;
                }

                ClearKskSytHcmTeeth();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đưa sơ đồ răng về mặc định (Bình thường) khi hồ sơ chưa có dữ liệu.</summary>
        private void ClearKskSytHcmTeeth()
        {
            try
            {
                if (hcmToothButtons == null || hcmToothButtons.Count == 0) return;
                foreach (string[] quadrant in HCM_TOOTH_QUADRANTS)
                {
                    foreach (string toothNo in quadrant)
                    {
                        hcmToothStatus[toothNo] = HCM_TOOTH_STATUS_DEFAULT;
                        PaintHcmTooth(toothNo);
                    }
                }
                RefreshHcmToothLegend();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ sơ đồ răng. Răng chưa có dữ liệu thì giữ nguyên mặc định.</summary>
        private void FillKskSytHcmTeeth(HIS_KSK_SYT_HCM d)
        {
            try
            {
                if (hcmToothButtons == null || hcmToothButtons.Count == 0) return;

                bool any = false;
                foreach (string[] quadrant in HCM_TOOTH_QUADRANTS)
                {
                    foreach (string toothNo in quadrant)
                    {
                        long? v = GetSytHcmLong(d, "TOOTH_" + toothNo);
                        if (!v.HasValue) continue;
                        hcmToothStatus[toothNo] = (int)v.Value;
                        PaintHcmTooth(toothNo);
                        any = true;
                    }
                }
                if (any) RefreshHcmToothLegend();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== LƯU =====

        /// <summary>
        /// Lưu bảng dữ liệu riêng của mẫu M4, gọi NGAY SAU khi lưu hồ sơ KSK thành công.
        /// Chưa có bản ghi -> thêm mới; đã có -> cập nhật.
        /// </summary>
        private void SaveKskSytHcm(HIS_KSK_OVER_EIGHTEEN savedKsk)
        {
            try
            {
                if (!IsSytHcmDeclared()) return;
                if (xtraTabControl1.SelectedTabPageIndex != TAB_INDEX__OVER_EIGHTEEN) return;
                if (savedKsk == null || savedKsk.ID <= 0)
                {
                    LogSystem.Warn("SytHcm: KHONG luu duoc bang mau M4 — chua co ma ban ghi KSK tra ve");
                    return;
                }

                HIS_KSK_SYT_HCM data = BuildKskSytHcmFromControls(savedKsk.ID);
                if (data == null) return;

                CommonParam param = new CommonParam();
                HIS_KSK_SYT_HCM result;
                if (currentKskSytHcm != null && currentKskSytHcm.ID > 0)
                {
                    data.ID = currentKskSytHcm.ID;
                    result = new BackendAdapter(param).Post<HIS_KSK_SYT_HCM>(
                        API__SYT_HCM_UPDATE, ApiConsumers.MosConsumer, data, param);
                }
                else
                {
                    result = new BackendAdapter(param).Post<HIS_KSK_SYT_HCM>(
                        API__SYT_HCM_CREATE, ApiConsumers.MosConsumer, data, param);
                }

                if (result != null)
                {
                    currentKskSytHcm = result;
                    LogSystem.Debug("SytHcm: da luu bang mau M4, ma ban ghi = " + result.ID);
                    return;
                }

                // Hồ sơ KSK đã lưu xong nhưng phần mẫu M4 thì không — phải báo, im lặng thì
                // người dùng tưởng đã lưu đủ và dữ liệu vừa nhập biến mất khi mở lại.
                LogSystem.Warn("SytHcm: LUU BANG MAU M4 THAT BAI (dich vu tra ve rong)");
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Đã lưu hồ sơ khám sức khỏe, nhưng phần dữ liệu riêng của mẫu Sở Y tế TP.HCM "
                    + "chưa lưu được. Vui lòng bấm Lưu lại; nếu vẫn lỗi, báo bộ phận kỹ thuật.",
                    "Thông báo",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Gom giá trị trên màn hình thành một bản ghi để lưu.</summary>
        private HIS_KSK_SYT_HCM BuildKskSytHcmFromControls(long kskOverEighteenId)
        {
            try
            {
                CheckSytHcmColumnMapOnce();

                // BẮT ĐẦU TỪ BẢN GHI ĐÃ LƯU, không từ bản ghi trống.
                //
                // Dịch vụ cập nhật THAY CẢ DÒNG: cột nào không gán sẽ thành rỗng. Mà màn hình chỉ
                // gán được những ô ĐANG DỰNG — người dùng chưa mở tab Khám lâm sàng HCM thì toàn bộ
                // cột lâm sàng, sơ đồ răng... không có gì để gán, gửi lên là XOÁ SẠCH dữ liệu cũ.
                //
                // Chép sẵn giá trị cũ vào rồi mới ghi đè phần người dùng vừa sửa: ô nào chưa dựng thì
                // giữ nguyên giá trị đang có.
                HIS_KSK_SYT_HCM d = new HIS_KSK_SYT_HCM();
                CopyKskSytHcmValues(currentKskSytHcm, d);
                d.KSK_OVER_EIGHTEEN_ID = kskOverEighteenId;

                #region Ô hành chính

                d.PAY_SOURCE_OTHER = GetKskPaySourceOtherValue();
                d.SYT_EXAM_PLACE_ID = GetKskExamPlaceValue();

                // Tab hoi benh lam sang HCM — 3 cot rieng, xem InterviewHcmData.
                SetInterviewHcmToEntity(d);

                // Ba ô dưới đây chỉ ghi khi ô đang giữ MÃ CỦA CỔNG. Chưa đổ được danh mục của cổng
                // thì ô đang giữ mã của HIS (hoặc mã tạm) — ghi vào cột dành cho mã của cổng sẽ
                // thành dữ liệu sai, đẩy lên cổng bị từ chối hoặc hiểu sang giá trị khác.
                if (IsSytCodeInObjectCombo())
                {
                    string types = GetKskObjectValue();
                    d.SYT_PATIENT_TYPES = !string.IsNullOrEmpty(types) ? types : null;
                }
                if (IsSytCodeInPaySourceCombo() && cboPaymentSource.EditValue != null)
                {
                    long v;
                    if (long.TryParse(cboPaymentSource.EditValue.ToString(), out v))
                        d.SYT_PAYSOURCE_ID = v;
                }
                if (IsSytCodeInPaySourceDetail())
                {
                    d.SYT_PAY_SOURCE_DETAIL_ID = GetKskPaySourceDetailValue();
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: LUU Hinh thuc chi tra = "
                        + (d.SYT_PAY_SOURCE_DETAIL_ID.HasValue
                            ? d.SYT_PAY_SOURCE_DETAIL_ID.Value.ToString() : "rong"));
                }
                else
                {
                    // Cua gac: o nhap chua giu ma cua So thi KHONG ghi cot. Neu thay dong nay moi
                    // lan luu thi goc van de la danh muc cua So khong do duoc vao o, khong phai loi luu.
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: KHONG luu Hinh thuc chi tra —"
                        + " o nhap chua giu ma cua So (danh muc cua cong chua do duoc vao o)");
                }

                // Cụm "Đề nghị" ở tab Kết luận. Hai cột này chỉ có sau khi chạy 2.sql; chưa có
                // thì hàm ghi cảnh báo "KHONG co cot" và bỏ qua, không làm hỏng phần còn lại.
                SetSytHcmValue(d, "SYT_SUGGEST_ID", GetKskSuggestValue());
                SetSytHcmValue(d, "SUGGEST_OTHER", GetKskSuggestOtherValue());

                #endregion

                #region Tab Khám lâm sàng HCM

                // Tab chưa mở lần nào -> các ô chọn bệnh chưa dựng, chưa có gì để lấy. Giữ nguyên
                // dữ liệu đã lưu trước đó thay vì ghi trống lên.
                if (isClinicalExamHcmInited)
                {
                    foreach (ClinicalExamHcmRow row in clinicalExamHcmRows)
                    {
                        string prefix;
                        if (row == null || !SYT_HCM_SECTION_COLUMN.TryGetValue(row.Key, out prefix)) continue;

                        SetSytHcmValue(d, prefix + SYT_HCM_SUFFIX__IS_NORMAL,
                            (row.ChkNormal != null && row.ChkNormal.Checked) ? (short?)1 : null);

                        string code, name;
                        GetHcmIcdValue(row.UcPreIcd, out code, out name);
                        SetSytHcmValue(d, prefix + SYT_HCM_SUFFIX__PRE_CODE, code);
                        SetSytHcmValue(d, prefix + SYT_HCM_SUFFIX__PRE_NAME, name);

                        GetHcmIcdValue(row.UcFinalIcd, out code, out name);
                        SetSytHcmValue(d, prefix + SYT_HCM_SUFFIX__CODE, code);
                        SetSytHcmValue(d, prefix + SYT_HCM_SUFFIX__NAME, name);

                        if (row.ChkRefuse != null)
                        {
                            if (row.Key == SYT_HCM_KEY__OBSTETRIC)
                                d.IS_REFUSE_OBSTETRIC = row.ChkRefuse.Checked ? (short?)1 : null;
                            else if (row.Key == SYT_HCM_KEY__GYNECOLOGY)
                                d.IS_REFUSE_GYNECOLOGY = row.ChkRefuse.Checked ? (short?)1 : null;
                        }

                        if (row.Key == SYT_HCM_KEY__GYNECOLOGY)
                        {
                            if (row.CboRank != null && row.CboRank.EditValue != null)
                            {
                                long rank;
                                if (long.TryParse(row.CboRank.EditValue.ToString(), out rank))
                                    d.EXAM_GYNECOLOGY_RANK = rank;
                            }
                            if (row.CboLoginName != null && row.CboLoginName.EditValue != null)
                                d.EXAM_GYNECOLOGY_LOGINNAME = row.CboLoginName.EditValue.ToString();
                        }

                        foreach (KeyValuePair<string, string> f in SYT_HCM_EYE_COLUMN)
                        {
                            if (!row.Fields.ContainsKey(f.Key)) continue;
                            string txt = row.Fields[f.Key].Text;
                            SetSytHcmValue(d, f.Value, !string.IsNullOrWhiteSpace(txt) ? txt.Trim() : null);
                        }
                    }

                    BuildKskSytHcmTeeth(d);
                }

                #endregion

                return d;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Gom sơ đồ răng. CHỈ ghi khi bảng trạng thái răng đang dùng danh mục của cổng —
        /// các cột này chứa mã định danh của cổng, ghi mã tạm vào sẽ thành dữ liệu sai.
        /// </summary>
        private void BuildKskSytHcmTeeth(HIS_KSK_SYT_HCM d)
        {
            try
            {
                if (hcmToothStatus == null || hcmToothStatus.Count == 0) return;
                if (!sytToothUseSytCode)
                {
                    LogSystem.Warn("SytHcm: KHONG luu so do rang — bang trang thai rang chua nhan duoc "
                        + "danh muc cua cong nen dang dung ma tam, luu vao se sai he ma");
                    return;
                }

                foreach (KeyValuePair<string, int> t in hcmToothStatus)
                {
                    SetSytHcmValue(d, "TOOTH_" + t.Key, (long?)t.Value);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Ô "Nhịp thở" ở phần Khám thể lực =====

        /// <summary>
        /// Ô nhập nhịp thở, dựng bằng mã và chèn vào phần Khám thể lực (cạnh Mạch).
        ///
        /// VÌ SAO PHẢI THÊM: cột `HIS_DHST.BREATH_RATE` đã có sẵn, nhưng phần Khám thể lực của tab
        /// này chỉ lưu 5 chỉ số (huyết áp trên/dưới, chiều cao, cân nặng, mạch) — KHÔNG có chỗ nhập
        /// nhịp thở. Cổng Sở Y tế TP.HCM lại BẮT BUỘC chỉ tiêu này, thiếu là trả 400 cả hồ sơ.
        ///
        /// AN TOÀN ĐA VIỆN: chỉ dựng cho viện đã khai báo cấu hình cổng.
        /// </summary>
        private SpinEdit spnKskBreathRate2;

        private bool breathRateInited = false;

        private void InitBreathRateControl()
        {
            try
            {
                if (breathRateInited) return;
                if (this.layoutControl18 == null || this.layoutControlItem114 == null) return;
                if (!IsSytHcmDeclared())
                {
                    LogSystem.Debug("SytHcm: chua khai bao cau hinh cong -> KHONG dung o Nhip tho");
                    return;
                }
                breathRateInited = true;

                spnKskBreathRate2 = new SpinEdit();
                spnKskBreathRate2.Name = "spnKskBreathRate2";
                spnKskBreathRate2.Properties.MinValue = 0;
                spnKskBreathRate2.Properties.MaxValue = 200;
                spnKskBreathRate2.Properties.IsFloatValue = false;
                spnKskBreathRate2.Properties.Mask.EditMask = "N0";
                spnKskBreathRate2.EditValue = null;

                this.layoutControl18.BeginUpdate();
                try
                {
                    LayoutControlItem lci = (LayoutControlItem)this.layoutControl18.AddItem(
                        "Nhịp thở:", spnKskBreathRate2);
                    lci.Name = "lciKskBreathRate2";
                    // Canh nhãn y hệt ô Mạch để hai dòng thẳng cột.
                    lci.AppearanceItemCaption.Options.UseTextOptions = true;
                    lci.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    lci.TextSize = this.layoutControlItem114.TextSize;
                    lci.TextAlignMode = TextAlignModeItem.CustomSize;
                    lci.TextToControlDistance = this.layoutControlItem114.TextToControlDistance;
                    lci.Move(this.layoutControlItem114, InsertType.Bottom);
                }
                finally { this.layoutControl18.EndUpdate(); }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Nhịp thở đang nhập (null nếu để trống) — dùng khi lưu bản ghi sinh hiệu.</summary>
        private decimal? GetKskBreathRateValue()
        {
            try
            {
                if (spnKskBreathRate2 == null || spnKskBreathRate2.EditValue == null) return null;
                decimal v;
                return decimal.TryParse(spnKskBreathRate2.Value.ToString(), out v) ? (decimal?)v : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>Đổ nhịp thở đã lưu vào ô nhập.</summary>
        private void SetKskBreathRateValue(decimal? value)
        {
            try
            {
                if (spnKskBreathRate2 == null) return;
                spnKskBreathRate2.EditValue = value.HasValue ? (object)value.Value : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Phân loại / Người khám: nối hai tab về CÙNG MỘT cột =====

        /// <summary>
        /// Một cặp ô "Phân loại" + "Người khám" xuất hiện ở HAI tab: tab "Khám lâm sàng" cũ và tab
        /// "Khám lâm sàng HCM". Cả hai cùng lưu vào MỘT cột của bảng KSK.
        /// </summary>
        private class HcmRankMirror
        {
            public DevExpress.XtraEditors.GridLookUpEdit HcmRank { get; set; }
            public DevExpress.XtraEditors.GridLookUpEdit HcmLogin { get; set; }
            public DevExpress.XtraEditors.GridLookUpEdit OldRank { get; set; }
            public DevExpress.XtraEditors.GridLookUpEdit OldLogin { get; set; }
        }

        private List<HcmRankMirror> hcmRankMirrors;

        /// <summary>Đang đồng bộ hai chiều — cờ chặn vòng lặp vô tận giữa hai ô.</summary>
        private bool hcmRankSyncing = false;

        /// <summary>
        /// Nối ô "Phân loại" và "Người khám" của tab Khám lâm sàng HCM với ô tương ứng của tab
        /// "Khám lâm sàng" cũ.
        ///
        /// VÌ SAO PHẢI NỐI: bảng KSK đã có sẵn 14 cặp cột phân loại / người khám, tab cũ đang lưu
        /// vào đó. Tab HCM có ô riêng cho cùng nội dung. Nếu để rời nhau thì:
        ///   - bác sĩ nhập ở tab HCM -> không lưu được, mất dữ liệu;
        ///   - hoặc nếu cho tab HCM ghi đè -> bác sĩ nhập ở tab cũ lại bị ô rỗng của tab HCM xóa mất.
        /// Nối hai chiều thì hai tab luôn hiển thị cùng một giá trị, phần lưu KHÔNG phải sửa gì
        /// (vẫn do tab cũ ghi vào cột như trước) và không cần thêm cột mới nào.
        ///
        /// Riêng mục Phụ khoa KHÔNG có ở đây: bảng KSK cũ chỉ có một cặp cột dùng chung cho sản phụ
        /// khoa, nên phụ khoa dùng cặp cột riêng ở bảng mẫu M4.
        /// </summary>
        private void InitHcmRankMirrors()
        {
            try
            {
                if (hcmRankMirrors != null) return;
                hcmRankMirrors = new List<HcmRankMirror>();

                AddHcmRankMirror("noikhoa", cboExamCirculationRank2, cboExamCirculationLoginName2);
                AddHcmRankMirror("hohap", cboExamRespiratoryRank2, cboExamRespiratoryLoginName2);
                AddHcmRankMirror("tieuhoa", cboExamDigestionRank2, cboExamDigestionLoginName2);
                AddHcmRankMirror("thantietnieu", cboExamKidneyUrologyRank2, cboExamKidneyUrologyLoginName2);
                AddHcmRankMirror("noitiet", cboExamOend2, cboExamOendLoginName2);
                AddHcmRankMirror("coxuongkhop", cboExamMuscleBoneRank2, cboExamMuscleBoneLoginName2);
                AddHcmRankMirror("thankinh", cboExamNeurologicalRank2, cboExamNeurologicalLoginName2);
                AddHcmRankMirror("tamthan", cboExamMentalRank2, cboExamMentalLoginName2);
                AddHcmRankMirror("ngoaikhoa", cboExamSurgeryRank2, cboExamSurgeryLoginName2);
                AddHcmRankMirror("dalieu", cboExamDernatologyRank2, cboExamDermatologyLoginName2);
                AddHcmRankMirror(SYT_HCM_KEY__OBSTETRIC, cboExamObstetricRank2, cboExamObstetricLoginName2);
                AddHcmRankMirror("mat", cboExamEyeRank2, cboExamEyeLoginName2);
                AddHcmRankMirror("tmh", cboExamEntDiseaseRank2, cboExamEntLoginName2);
                AddHcmRankMirror("rhm", cboExamStomatologyRank2, cboExamStomatologyLoginName2);

                LogSystem.Debug("SytHcm: da noi " + hcmRankMirrors.Count
                    + " cap o Phan loai / Nguoi kham giua 2 tab (Phu khoa dung cot rieng)");

                InitHcmFieldMirrors();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void AddHcmRankMirror(string sectionKey,
            DevExpress.XtraEditors.GridLookUpEdit oldRank,
            DevExpress.XtraEditors.GridLookUpEdit oldLogin)
        {
            try
            {
                ClinicalExamHcmRow row = clinicalExamHcmRows.FirstOrDefault(r => r != null && r.Key == sectionKey);
                if (row == null) return;

                HcmRankMirror m = new HcmRankMirror();
                m.HcmRank = row.CboRank;
                m.HcmLogin = row.CboLoginName;
                m.OldRank = oldRank;
                m.OldLogin = oldLogin;
                hcmRankMirrors.Add(m);

                // Đổ giá trị hiện có từ tab cũ sang tab HCM — tab cũ đã nạp từ cơ sở dữ liệu.
                hcmRankSyncing = true;
                try
                {
                    if (m.HcmRank != null && m.OldRank != null) m.HcmRank.EditValue = m.OldRank.EditValue;
                    if (m.HcmLogin != null && m.OldLogin != null) m.HcmLogin.EditValue = m.OldLogin.EditValue;
                }
                finally { hcmRankSyncing = false; }

                HookHcmRankPair(m.HcmRank, m.OldRank);
                HookHcmRankPair(m.HcmLogin, m.OldLogin);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void HookHcmRankPair(DevExpress.XtraEditors.GridLookUpEdit a,
                                    DevExpress.XtraEditors.GridLookUpEdit b)
        {
            if (a == null || b == null) return;
            a.EditValueChanged -= HcmRankMirror_EditValueChanged;
            a.EditValueChanged += HcmRankMirror_EditValueChanged;
            b.EditValueChanged -= HcmRankMirror_EditValueChanged;
            b.EditValueChanged += HcmRankMirror_EditValueChanged;
        }

        /// <summary>Một cặp ô trị số trùng nội dung giữa hai tab.</summary>
        private class HcmFieldMirror
        {
            public TextEdit Hcm { get; set; }
            public TextEdit Old { get; set; }
        }

        private List<HcmFieldMirror> hcmFieldMirrors;

        /// <summary>
        /// Nối 8 ô trị số trùng nội dung giữa tab Khám lâm sàng HCM và tab "Khám lâm sàng" cũ:
        /// thị lực không kính / có kính (mỗi mắt) và thính lực nói thường / nói thầm (mỗi tai).
        ///
        /// VÌ SAO NỐI: đây là CÙNG MỘT chỉ tiêu ở hai tab và bảng KSK đã có cột sẵn. Tab HCM không
        /// ghi vào đâu cả nên nhập ở tab HCM là mất; còn nếu cho tab HCM ghi đè thì ô rỗng của nó
        /// lại xóa giá trị người dùng nhập ở tab cũ. Nối hai chiều thì nhập bên nào cũng vào đúng
        /// cột cũ, và phần lưu KHÔNG phải sửa gì.
        ///
        /// 8 ô còn lại của mục Mắt (kính lỗ, độ cầu, độ trụ, trục) là chỉ tiêu MỚI, bảng KSK không
        /// có cột, nên vẫn lưu ở bảng mẫu M3 — không nằm trong danh sách này.
        /// </summary>
        private void InitHcmFieldMirrors()
        {
            try
            {
                if (hcmFieldMirrors != null) return;
                hcmFieldMirrors = new List<HcmFieldMirror>();

                AddHcmFieldMirror("mat", "mat_khongkinh_mp", txtExamEyeSightRight2);
                AddHcmFieldMirror("mat", "mat_khongkinh_mt", txtExamEyeSightLeft2);
                AddHcmFieldMirror("mat", "mat_cokinh_mp", txtExamEyeSightGlassRight2);
                AddHcmFieldMirror("mat", "mat_cokinh_mt", txtExamEyeSightGlassLeft2);

                AddHcmFieldMirror("tmh", "tmh_taitrai_noithuong", txtExamEntLeftNormal2);
                AddHcmFieldMirror("tmh", "tmh_taitrai_noitham", txtExamEntLeftWhisper2);
                AddHcmFieldMirror("tmh", "tmh_taiphai_noithuong", txtExamEntRightNomal2);
                AddHcmFieldMirror("tmh", "tmh_taiphai_noitham", txtExamEntRightWhisper2);

                LogSystem.Debug("SytHcm: da noi " + hcmFieldMirrors.Count
                    + " cap o tri so (thi luc, thinh luc) giua 2 tab");
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void AddHcmFieldMirror(string sectionKey, string fieldKey, TextEdit oldEdit)
        {
            try
            {
                if (oldEdit == null) return;
                ClinicalExamHcmRow row = clinicalExamHcmRows
                    .FirstOrDefault(r => r != null && r.Key == sectionKey);
                if (row == null || !row.Fields.ContainsKey(fieldKey)) return;

                TextEdit hcmEdit = row.Fields[fieldKey];
                if (hcmEdit == null) return;

                hcmFieldMirrors.Add(new HcmFieldMirror { Hcm = hcmEdit, Old = oldEdit });

                // Đổ giá trị hiện có từ tab cũ sang tab HCM — tab cũ đã nạp từ cơ sở dữ liệu.
                hcmRankSyncing = true;
                try { hcmEdit.Text = oldEdit.Text; }
                finally { hcmRankSyncing = false; }

                hcmEdit.EditValueChanged -= HcmFieldMirror_EditValueChanged;
                hcmEdit.EditValueChanged += HcmFieldMirror_EditValueChanged;
                oldEdit.EditValueChanged -= HcmFieldMirror_EditValueChanged;
                oldEdit.EditValueChanged += HcmFieldMirror_EditValueChanged;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổi ở một ô trị số -> gán sang ô kia của cùng cặp.</summary>
        private void HcmFieldMirror_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (hcmRankSyncing || hcmFieldMirrors == null) return;

                TextEdit src = sender as TextEdit;
                if (src == null) return;

                TextEdit dst = null;
                foreach (HcmFieldMirror m in hcmFieldMirrors)
                {
                    if (ReferenceEquals(src, m.Hcm)) { dst = m.Old; break; }
                    if (ReferenceEquals(src, m.Old)) { dst = m.Hcm; break; }
                }
                if (dst == null) return;

                // So sánh trước khi gán: gán lại giá trị y cũ vẫn kích hoạt sự kiện của ô kia.
                if (string.Equals(src.Text, dst.Text)) return;

                hcmRankSyncing = true;
                try { dst.Text = src.Text; }
                finally { hcmRankSyncing = false; }
            }
            catch (Exception ex) { LogSystem.Warn(ex); hcmRankSyncing = false; }
        }

        /// <summary>Đổi ở một ô -> gán sang ô kia của cùng cặp.</summary>
        private void HcmRankMirror_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (hcmRankSyncing || hcmRankMirrors == null) return;

                DevExpress.XtraEditors.GridLookUpEdit src = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (src == null) return;

                DevExpress.XtraEditors.GridLookUpEdit dst = null;
                foreach (HcmRankMirror m in hcmRankMirrors)
                {
                    if (ReferenceEquals(src, m.HcmRank)) { dst = m.OldRank; break; }
                    if (ReferenceEquals(src, m.OldRank)) { dst = m.HcmRank; break; }
                    if (ReferenceEquals(src, m.HcmLogin)) { dst = m.OldLogin; break; }
                    if (ReferenceEquals(src, m.OldLogin)) { dst = m.HcmLogin; break; }
                }
                if (dst == null) return;

                // So sánh trước khi gán: gán lại giá trị y cũ vẫn kích hoạt sự kiện của ô kia.
                string sv = (src.EditValue != null) ? src.EditValue.ToString() : null;
                string dv = (dst.EditValue != null) ? dst.EditValue.ToString() : null;
                if (sv == dv) return;

                hcmRankSyncing = true;
                try { dst.EditValue = src.EditValue; }
                finally { hcmRankSyncing = false; }
            }
            catch (Exception ex) { LogSystem.Warn(ex); hcmRankSyncing = false; }
        }

        #endregion

        #region ===== Truy cập cột theo tên =====


        #region ===== Gợi ý đối tượng khám "Người cao tuổi" =====

        /// <summary>Mốc người cao tuổi theo Luật Người cao tuổi: ĐỦ 60, không phải "trên 60".</summary>
        private const int SYT_ELDERLY_AGE = 60;

        /// <summary>
        /// Bệnh nhân đủ 60 tuổi tại ngày khám thì tự tích sẵn đối tượng "Người cao tuổi", cho đỡ sót
        /// — vì chính ô này quyết định hồ sơ đẩy theo mẫu M3 hay M4.
        ///
        /// CHỈ GỢI Ý, KHÔNG ÉP:
        ///   - Chỉ tích khi hồ sơ CHƯA chọn đối tượng nào. Người dùng đã chọn rồi thì không đụng
        ///     vào, kể cả khi họ cố tình không chọn "Người cao tuổi" — một người 65 tuổi còn đi làm,
        ///     khám định kỳ theo diện người lao động, vẫn là hồ sơ M3.
        ///   - Tích xong người dùng bỏ tích được bình thường.
        ///
        /// TÌM THEO TÊN, KHÔNG VIẾT CỨNG MÃ: mã của "Người cao tuổi" ở danh mục của HIS là 1, ở danh
        /// mục của cổng là 5. Ô này nạp danh mục nào tùy viện đã khai báo cấu hình cổng hay chưa,
        /// nên viết cứng số là gán nhầm sang đối tượng khác.
        ///
        /// Tuổi tính tại NGÀY CHỈ ĐỊNH Y LỆNH, không phải hôm nay: cùng một hồ sơ mở lại sang năm
        /// vẫn phải ra cùng kết quả. Dùng lại AgeInMonthsUnderSix — hàm đang dùng để chọn tab mặc
        /// định theo tuổi, đã xử lý sẵn trường hợp bệnh nhân chỉ khai năm sinh.
        /// </summary>
        private void AutoTickElderlyObject()
        {
            try
            {
                if (!IsSytHcmDeclared()) return;
                if (cboObject == null || currentServiceReq == null) return;

                // Đã chọn rồi thì đây là quyết định của người dùng — không ghi đè.
                if (!string.IsNullOrWhiteSpace(GetKskObjectValue())) return;

                long dob = currentServiceReq.TDL_PATIENT_DOB;
                if (dob <= 0) return;

                long exam = (currentServiceReq.INTRUCTION_TIME > 0)
                    ? currentServiceReq.INTRUCTION_TIME
                    : Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));

                int months = AgeInMonthsUnderSix(dob, exam);
                if (months < SYT_ELDERLY_AGE * 12) return;

                List<KskCodeNameADO> ds = cboObject.Properties.DataSource as List<KskCodeNameADO>;
                if (ds == null) return;

                KskCodeNameADO row = ds.FirstOrDefault(
                    x => x.NAME != null && x.NAME.Trim() == "Người cao tuổi");
                if (row == null)
                {
                    LogSystem.Warn("SytHcm: danh muc doi tuong kham dang nap KHONG co muc"
                        + " 'Nguoi cao tuoi' -> bo qua goi y");
                    return;
                }

                SetKskObjectValue(row.ID.ToString());
                LogSystem.Warn("SytHcm: benh nhan du " + SYT_ELDERLY_AGE
                    + " tuoi tai ngay kham -> tu tich doi tuong 'Nguoi cao tuoi' (ma " + row.ID + ")");
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        private static readonly Dictionary<string, PropertyInfo> sytHcmColumnCache =
            new Dictionary<string, PropertyInfo>();

        private static PropertyInfo GetSytHcmProperty(string column)
        {
            try
            {
                PropertyInfo pi;
                if (sytHcmColumnCache.TryGetValue(column, out pi)) return pi;
                pi = typeof(HIS_KSK_SYT_HCM).GetProperty(column);
                sytHcmColumnCache[column] = pi;
                if (pi == null)
                    LogSystem.Warn("SytHcm: bang du lieu mau M4 KHONG co cot '" + column
                        + "' -> bo qua o nhap tuong ung, can sua lai bang anh xa cot");
                return pi;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private static void SetSytHcmValue(HIS_KSK_SYT_HCM d, string column, object value)
        {
            try
            {
                PropertyInfo pi = GetSytHcmProperty(column);
                if (pi == null || !pi.CanWrite) return;
                pi.SetValue(d, value, null);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static string GetSytHcmStr(HIS_KSK_SYT_HCM d, string column)
        {
            try
            {
                PropertyInfo pi = GetSytHcmProperty(column);
                return (pi != null) ? pi.GetValue(d, null) as string : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private static short? GetSytHcmShort(HIS_KSK_SYT_HCM d, string column)
        {
            try
            {
                PropertyInfo pi = GetSytHcmProperty(column);
                return (pi != null) ? pi.GetValue(d, null) as short? : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private static long? GetSytHcmLong(HIS_KSK_SYT_HCM d, string column)
        {
            try
            {
                PropertyInfo pi = GetSytHcmProperty(column);
                return (pi != null) ? pi.GetValue(d, null) as long? : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private static bool sytHcmColumnMapChecked = false;

        /// <summary>
        /// Tự kiểm tra bảng ánh xạ cột MỘT LẦN: mọi tên cột dùng trong mã phải có thật trong bảng.
        /// Sai tên thì ghi rõ ra nhật ký ngay lần lưu đầu, thay vì âm thầm mất dữ liệu của một mục.
        /// </summary>
        private static void CheckSytHcmColumnMapOnce()
        {
            try
            {
                if (sytHcmColumnMapChecked) return;
                sytHcmColumnMapChecked = true;

                List<string> missing = new List<string>();
                string[] suffixes = new string[]
                {
                    SYT_HCM_SUFFIX__IS_NORMAL, SYT_HCM_SUFFIX__PRE_CODE, SYT_HCM_SUFFIX__PRE_NAME,
                    SYT_HCM_SUFFIX__CODE, SYT_HCM_SUFFIX__NAME
                };
                foreach (KeyValuePair<string, string> s in SYT_HCM_SECTION_COLUMN)
                    foreach (string suf in suffixes)
                        if (typeof(HIS_KSK_SYT_HCM).GetProperty(s.Value + suf) == null)
                            missing.Add(s.Value + suf);

                foreach (KeyValuePair<string, string> f in SYT_HCM_EYE_COLUMN)
                    if (typeof(HIS_KSK_SYT_HCM).GetProperty(f.Value) == null)
                        missing.Add(f.Value);

                foreach (string[] quadrant in HCM_TOOTH_QUADRANTS)
                    foreach (string toothNo in quadrant)
                        if (typeof(HIS_KSK_SYT_HCM).GetProperty("TOOTH_" + toothNo) == null)
                            missing.Add("TOOTH_" + toothNo);

                if (missing.Count > 0)
                {
                    LogSystem.Warn("SytHcm: bang anh xa cot SAI — " + missing.Count
                        + " cot khong ton tai: " + string.Join(", ", missing.ToArray()));
                }
                else
                {
                    LogSystem.Debug("SytHcm: bang anh xa cot khop het voi bang du lieu mau M4");
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion
    }
}
