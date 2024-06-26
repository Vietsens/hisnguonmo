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

namespace MPS.Processor.Mps000106.PDO
{
    public partial class Mps000106PDO : RDOBase
    {
        public const string printTypeCode = "Mps000106";

        public decimal _CanThu_Amount = 0;
        public decimal depositAmpount = 0;
        public HIS_TREATMENT HisTreatment;
        public Mps000106ADO Mps000106ADO;
        public List<HIS_DEPARTMENT> ListDepartment { get; set; }
        public bool ShowExpend { get; set; }

        public Mps000106PDO() { }

        public Mps000106PDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> hisSereServs, HIS_TREATMENT _HisTreatment, decimal _depositAmpount, decimal ctAmount, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, V_HIS_PATIENT patient, string ratioText, Mps000106ADO _Mps000106ADO, List<HIS_DEPARTMENT> listDepartment)
        {
            try
            {
                this.listSereServ = hisSereServs;
                this._Transaction = transaction;
                this._CanThu_Amount = ctAmount;
                this.HisTreatment = _HisTreatment;
                this.depositAmpount = _depositAmpount;
                this._patient = patient;
                this.RatioText = ratioText;
                this._PatientTypeAlter = patientTypeAlter;
                this.Mps000106ADO = _Mps000106ADO;
                this.ListDepartment = listDepartment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000106PDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> hisSereServs, List<V_HIS_SERE_SERV_BILL> sereServBills, HIS_TREATMENT _HisTreatment, decimal _depositAmpount, decimal ctAmount, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, V_HIS_PATIENT patient, string ratioText, Mps000106ADO _Mps000106ADO, List<HIS_DEPARTMENT> listDepartment)
        {
            try
            {
                this.listSereServ = hisSereServs;
                this._Transaction = transaction;
                this._CanThu_Amount = ctAmount;
                this.HisTreatment = _HisTreatment;
                this.depositAmpount = _depositAmpount;
                this._patient = patient;
                this.RatioText = ratioText;
                this._PatientTypeAlter = patientTypeAlter;
                this.Mps000106ADO = _Mps000106ADO;
                this.listSereServBill = sereServBills;
                this.ListDepartment = listDepartment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000106PDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> hisSereServs, List<V_HIS_SERE_SERV_BILL> sereServBills, List<HIS_BILL_GOODS> billGoods, HIS_TREATMENT _HisTreatment, decimal _depositAmpount, decimal ctAmount, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, V_HIS_PATIENT patient, string ratioText, Mps000106ADO _Mps000106ADO, List<HIS_DEPARTMENT> listDepartment)
        {
            try
            {
                this.listSereServ = hisSereServs;
                this._Transaction = transaction;
                this._CanThu_Amount = ctAmount;
                this.HisTreatment = _HisTreatment;
                this.depositAmpount = _depositAmpount;
                this._patient = patient;
                this.RatioText = ratioText;
                this._PatientTypeAlter = patientTypeAlter;
                this.Mps000106ADO = _Mps000106ADO;
                this.listSereServBill = sereServBills;
                this.BillGoods = billGoods;
                this.ListDepartment = listDepartment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class Mps000106ADO
    {
        public long PatientTypeBHYT { get; set; }
        public long PatientTypeVP { get; set; }
    }
}
