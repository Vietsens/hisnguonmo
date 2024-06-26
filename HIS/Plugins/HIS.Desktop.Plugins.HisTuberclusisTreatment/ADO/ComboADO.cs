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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisTuberclusisTreatment.ADO
{
    public class ComboADO
    {
        public long Value { get; set; }

        public string Name { get; set; }
        public ComboADO()
        {

        }
        public ComboADO(long value, string name)
        {
            this.Value = value;
            this.Name = name;
        }
        public List<ComboADO> ListTuberculosisClassifyLocation()
        {
            return new List<ComboADO>()
            {
                new ComboADO(1, "Lao phổi" ),
                new ComboADO(2, "Lao ngoài phổi")
            };
        }
        public List<ComboADO> ListTuberculosisClassifyTs()
        {
            return new List<ComboADO>()
            {
                new ComboADO(1, "Lao mới" ),
                new ComboADO(2, "Tái phát"),
                new ComboADO(3, "Thất bại" ),
                new ComboADO(4, "Điều trị lại sau bỏ trị"),
                new ComboADO(5, "Điều trị lại khác" ),
                new ComboADO(6, "Không rõ tiền sử điều trị"),
            };
        }
        public List<ComboADO> ListTuberculosisClassifyHiv()
        {
            return new List<ComboADO>()
            {
                new ComboADO(1, "Lao/HIV (+)" ),
                new ComboADO(2, "Lao/HIV (-)"),
                new ComboADO(3, "BN lao không rõ tình trạng HIV"),
            };
        }
        public List<ComboADO> ListTuberculosisClassifyVk()
        {
            return new List<ComboADO>()
            {
                new ComboADO(1, "Lao có bằng chứng vi khuẩn học" ),
                new ComboADO(2, "Lao không có bằng chứng vi khuẩn họ"),
            };
        }
        public List<ComboADO> ListTuberculosisClassifyKt()
        {
            return new List<ComboADO>()
            {
                new ComboADO(1, "Lao kháng đơn thuốc" ),
                new ComboADO(2, "Lao kháng nhiều thuốc"),
                new ComboADO(3, "Lao đa kháng thuốc"),
                new ComboADO(4, "Lao kháng Rifampicin-Lao kháng R" ),
                new ComboADO(5, "Lao tiền siêu kháng"),
                new ComboADO(6, "Lao siêu kháng thuốc"),
            };
        }
        public List<ComboADO> ListTuberculosisTreatmentType()
        {
            return new List<ComboADO>()
            {
                new ComboADO(0, "Không điều trị lao" ),
                new ComboADO(1, "Điều trị lao tiềm ẩn"),
                new ComboADO(2, "Điều trị lao nhạy cảm thuốc"),
                new ComboADO(3, "Điều trị lao kháng thuốc" ),
            };
        }
        public List<ComboADO> ListTuberculosisTreatmentResult()
        {
            return new List<ComboADO>()
            {
                new ComboADO(1, "Khỏi: người bệnh lao phổi có bằng chứng vi khuẩn học tại thời điểm bắt đầu điều trị, có kết quả xét nghiệm đờm trực tiếp hoặc nuôi cấy âm tính tháng cuối của quá trình điều trị và ít nhất 1 lần trước đó" ),
                new ComboADO(2, "Hoàn thành điều trị: người bệnh lao hoàn thành liệu trình điều trị, không có bằng chứng thất bại, nhưng cũng không có xét nghiệm đờm trực tiếp hoặc nuôi cấy âm tính vào tháng cuối của quá trình điều trị và ít nhất 1 lần trước đó, bất kể không làm xét nghiệm hay không có kết quả xét nghiệm"),
                new ComboADO(3, "Thất bại: người bệnh lao có kết quả xét nghiệm đờm trực tiếp hoặc nuôi cấy dương tính từ tháng thứ 5 trở đi của quá trình điều trị"),
                new ComboADO(4, "Chết: người bệnh lao chết do bất cứ nguyên nhân gì trước hoặc trong quá trình điều trị lao" ),
                new ComboADO(5, "Không theo dõi được (bỏ): người bệnh lao ngừng điều trị liên tục từ 2 tháng trở lên" ),
                new ComboADO(6, "Không đánh giá: người bệnh lao không được đánh giá kết quả điều trị. Bao gồm các trường hợp chuyển tới đơn vị điều trị khác và không có phản hồi kết quả điều trị, cũng như các trường hợp đơn vị báo cáo không biết kết quả điều trị của bệnh nhân" ),
            };
        }
    }
}
