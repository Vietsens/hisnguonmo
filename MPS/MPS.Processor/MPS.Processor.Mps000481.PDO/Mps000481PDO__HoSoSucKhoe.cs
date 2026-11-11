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
using MOS.EFMODEL.DataModels;
using System.Collections.Generic;

namespace MPS.Processor.Mps000481.PDO
{
    /// <summary>
    /// Dữ liệu bổ sung cho mẫu "Hồ sơ sức khỏe" (báo cáo kết quả KSK theo bảng XN có khoảng tham chiếu,
    /// CĐHA &amp; TDCN, PTTT). Tất cả đều TÙY CHỌN — caller cũ không gán thì mẫu cũ vẫn chạy bình thường.
    /// </summary>
    public partial class Mps000481PDO
    {
        /// <summary>Dấu hiệu sinh tồn của các đợt điều trị (lấy theo DHST_ID của phần khám KSK, thiếu thì bản mới nhất).</summary>
        public List<V_HIS_DHST> Dhsts { get; set; }
        /// <summary>Khoảng tham chiếu chỉ số xét nghiệm — tính cột "Bình thường" và đánh dấu kết quả bất thường.</summary>
        public List<V_HIS_TEST_INDEX_RANGE> TestIndexRanges { get; set; }
        /// <summary>Danh mục loại xét nghiệm — tên nhóm "Xét nghiệm Huyết học / Sinh hóa / Nước tiểu ...".</summary>
        public List<HIS_TEST_TYPE> TestTypes { get; set; }
    }

    /// <summary>1 nhóm xét nghiệm (theo loại XN) của 1 đợt điều trị — band listXetNghiemNhom.</summary>
    public class TestGroupADO
    {
        public long TDL_TREATMENT_ID { get; set; }
        /// <summary>Khóa quan hệ nhóm → dòng: "{TREATMENT_ID}_{TEST_TYPE_ID}".</summary>
        public string GROUP_KEY { get; set; }
        public long? TEST_TYPE_ID { get; set; }
        public string TEST_TYPE_NAME { get; set; }
        /// <summary>Tiêu đề nhóm hiển thị, vd "Xét nghiệm Huyết học".</summary>
        public string GROUP_NAME { get; set; }
    }

    /// <summary>1 dòng bảng xét nghiệm — dịch vụ hoặc chỉ số con — band listXetNghiem.</summary>
    public class TestResultRowADO
    {
        public long TDL_TREATMENT_ID { get; set; }
        public string GROUP_KEY { get; set; }
        public long SERE_SERV_ID { get; set; }
        /// <summary>Số thứ tự trong nhóm (đánh lại từ 1 cho mỗi nhóm).</summary>
        public long STT { get; set; }
        /// <summary>Tên dịch vụ hoặc tên chỉ số (chỉ số con có thụt lề).</summary>
        public string NAME { get; set; }
        public string VALUE { get; set; }
        public string UNIT { get; set; }
        /// <summary>Khoảng bình thường, vd "4.0 - 10.0", "&lt; 5.2", "Negative".</summary>
        public string NORMAL_RANGE { get; set; }
        public string NOTE { get; set; }
        /// <summary>1 = dòng dịch vụ cha có nhiều chỉ số con.</summary>
        public short? IS_PARENT { get; set; }
        /// <summary>1 = dòng chỉ số con.</summary>
        public short? IS_CHILD { get; set; }
        /// <summary>"H" = cao hơn bình thường, "L" = thấp hơn bình thường, rỗng = bình thường / không so được.</summary>
        public string ABNORMAL_FLAG { get; set; }
        /// <summary>1 = kết quả nằm ngoài khoảng bình thường (dùng cho định dạng có điều kiện trên template).</summary>
        public short? IS_ABNORMAL { get; set; }
    }

    /// <summary>1 dịch vụ CĐHA &amp; TDCN hoặc PTTT của 1 đợt điều trị — band listCdhaTdcn / listPttt.</summary>
    public class SubclinicalResultADO
    {
        public long TDL_TREATMENT_ID { get; set; }
        public long SERE_SERV_ID { get; set; }
        public long STT { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        /// <summary>Kết luận (HIS_SERE_SERV_EXT.CONCLUDE).</summary>
        public string RESULT { get; set; }
        public string NOTE { get; set; }
    }
}
