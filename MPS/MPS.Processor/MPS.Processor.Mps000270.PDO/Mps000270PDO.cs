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
using His.Bhyt.InsuranceExpertise.LDO;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000270.PDO
{
    public partial class Mps000270PDO : RDOBase
    {
        public ResultHistoryLDO _ResultHistoryLDO { get; set; }
        public List<HIS_MEDI_ORG> _MediOrgs { get; set; }

        public Mps000270PDO() { }

        public Mps000270PDO(ResultHistoryLDO resultHistoryLDO,
            List<HIS_MEDI_ORG> _mediOrgs)
        {
            this._ResultHistoryLDO = resultHistoryLDO;
            this._MediOrgs = _mediOrgs;
        }
    }

    public class Mps000270ADO : ExamHistoryLDO
    {
        public string tenCSKCB { get; set; }
        public string tinhTrangName { get; set; }
        public string ketQuaName { get; set; }

        public Mps000270ADO() { }

        public Mps000270ADO(ExamHistoryLDO data, List<HIS_MEDI_ORG> currentMediOrgs)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<Mps000270ADO>(this, data);

                var mediOrg = currentMediOrgs.FirstOrDefault(p => p.MEDI_ORG_CODE == data.maCSKCB);
                if (mediOrg != null)
                {
                    this.tenCSKCB = mediOrg.MEDI_ORG_NAME;
                }
                if (data.tinhTrang == "1")
                    this.tinhTrangName = "Ra viện";
                else if (data.tinhTrang == "2")
                    this.tinhTrangName = "Chuyển viện";
                else if (data.tinhTrang == "3")
                    this.tinhTrangName = "Trốn viện";
                else if (data.tinhTrang == "4")
                    this.tinhTrangName = "Xin ra viện";

                if (data.kqDieuTri == "1")
                    this.ketQuaName = "Khỏi";
                else if (data.kqDieuTri == "2")
                    this.ketQuaName = "Đỡ";
                else if (data.kqDieuTri == "3")
                    this.ketQuaName = "Không thay đổi";
                else if (data.kqDieuTri == "4")
                    this.ketQuaName = "Nặng hơn";
                else if (data.kqDieuTri == "5")
                    this.ketQuaName = "Tử vong";
            }
        }
    }
}
