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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000431.PDO
{
    public class Mps000431PDO : RDOBase
    {
        public const string printTypeCode = "Mps000431";

        public V_HIS_TRANSACTION HisTransaction { get; set; }

        public List<ProductADO> lstProductADO { get; set;}

        /// <summary>Danh sách dịch vụ của hóa đơn — bind cho key dạng list: &lt;#SereServ.VAT_RATIO;&gt;</summary>
        public List<V_HIS_SERE_SERV_5> SereServs { get; set; }

        /// <summary>Ngày sinh bệnh nhân dạng chuỗi dd/MM/yyyy — key &lt;#DOB_STR;&gt;</summary>
        public string DOB_STR { get; set; }

        /// <summary>Tên khoa của phòng khám đầu tiên — key &lt;#DEPARTMENT_NAME;&gt;</summary>
        public string DEPARTMENT_NAME { get; set; }

        /// <summary>Tên phòng khám đầu tiên — key &lt;#EXAM_EXECUTE_ROOM_NAME;&gt;</summary>
        public string EXAM_EXECUTE_ROOM_NAME { get; set; }

        /// <summary>Mã bệnh nhân — key &lt;#PATIENT_CODE;&gt;</summary>
        public string PATIENT_CODE { get; set; }



        public Mps000431PDO(V_HIS_TRANSACTION _HisTransaction, List<ProductADO> _lstProductADO)
        {
            try
            {
                this.HisTransaction = _HisTransaction;
                this.lstProductADO = _lstProductADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ProductADO 
    {
        public decimal Amount { get; set; }
        public string ProdName { get; set; }
        public decimal ProdPrice { get; set; }
        public decimal ProdQuantity { get; set; }
        public string ProdUnit { get; set; }


        //dangth
        public decimal? TaxAmount { get; set; }

        public decimal? AmountWithoutTax { get; set; }

        public int? TaxPercentage { get; set; }

        public decimal TaxConvert { get; set; }
    }
}
