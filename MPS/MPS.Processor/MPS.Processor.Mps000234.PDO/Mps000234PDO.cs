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
using ACS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000234.PDO
{
    public partial class Mps000234PDO : RDOBase
    {
        public const string PrintTypeCode = "Mps000234";
        public List<HIS_SERVICE_REQ> ListServiceReq;//chỉ có thông tin SERVICE_REQ_CODE
        public List<ACS_USER> ListAcsUser;
        public List<V_HIS_ROOM> ListRoom;
        public List<HIS_SERE_SERV> ListSereServCls { get; set; }

        public Mps000234PDO() { }

        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            List<ACS_USER> _ListAcsUser,
            List<V_HIS_ROOM> _ListRoom)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.ListAcsUser = _ListAcsUser;
                this.ListRoom = _ListRoom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            List<ACS_USER> _ListAcsUser,
            List<V_HIS_ROOM> _ListRoom,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.ListAcsUser = _ListAcsUser;
                this.ListRoom = _ListRoom;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        //Thêm list thuốc ngoài kho
        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            List<ACS_USER> _ListAcsUser,
            List<V_HIS_ROOM> _ListRoom,
            List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.ListAcsUser = _ListAcsUser;
                this.ListRoom = _ListRoom;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            List<ACS_USER> _ListAcsUser,
            List<V_HIS_ROOM> _ListRoom,
            List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock,
             HIS_SERVICE_REQ _hisServiceReq_CurentExam)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.ListAcsUser = _ListAcsUser;
                this.ListRoom = _ListRoom;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000234PDO(
           V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
           HIS_DHST hisDhst,
           HIS_SERVICE_REQ HisPrescription,
           List<ExpMestMedicineSDO> expMestMedicines,
           HIS_SERVICE_REQ hisServiceReq_Exam,
           Mps000234ADO mps000234ADO,
           List<HIS_SERVICE_REQ> _ListServiceReq,
           List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock,
          HIS_SERVICE_REQ _hisServiceReq_CurentExam)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }












        //mới
        public Mps000234PDO(
           V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
           HIS_DHST hisDhst,
           HIS_SERVICE_REQ HisPrescription,
           List<ExpMestMedicineSDO> expMestMedicines,
           HIS_SERVICE_REQ hisServiceReq_Exam,
           Mps000234ADO mps000234ADO,
           List<HIS_SERVICE_REQ> _ListServiceReq,
           List<ACS_USER> _ListAcsUser,
           List<V_HIS_ROOM> _ListRoom,
           List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.ListAcsUser = _ListAcsUser;
                this.ListRoom = _ListRoom;
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            List<ACS_USER> _ListAcsUser,
            List<V_HIS_ROOM> _ListRoom,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.ListAcsUser = _ListAcsUser;
                this.ListRoom = _ListRoom;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }




        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        //Thêm list thuốc ngoài kho
        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            List<ACS_USER> _ListAcsUser,
            List<V_HIS_ROOM> _ListRoom,
            List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.ListAcsUser = _ListAcsUser;
                this.ListRoom = _ListRoom;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            List<ACS_USER> _ListAcsUser,
            List<V_HIS_ROOM> _ListRoom,
            List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock,
             HIS_SERVICE_REQ _hisServiceReq_CurentExam,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.ListAcsUser = _ListAcsUser;
                this.ListRoom = _ListRoom;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<HIS_SERVICE_REQ> _ListServiceReq,
            List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000234PDO(
           V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
           HIS_DHST hisDhst,
           HIS_SERVICE_REQ HisPrescription,
           List<ExpMestMedicineSDO> expMestMedicines,
           HIS_SERVICE_REQ hisServiceReq_Exam,
           Mps000234ADO mps000234ADO,
           List<HIS_SERVICE_REQ> _ListServiceReq,
           List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock,
          HIS_SERVICE_REQ _hisServiceReq_CurentExam,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.ListServiceReq = _ListServiceReq;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000234PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST hisDhst,
            HIS_SERVICE_REQ HisPrescription,
            List<ExpMestMedicineSDO> expMestMedicines,
            HIS_SERVICE_REQ hisServiceReq_Exam,
            Mps000234ADO mps000234ADO,
            List<ExpMestMedicineSDO> expMestMedicineIncludeOutStock,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam,
            List<HIS_SERE_SERV> listSereServCls)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000234ADO = mps000234ADO;
                this.expMestMedicineIncludeOutStock = expMestMedicineIncludeOutStock.OrderBy(o => o.NUM_ORDER).ToList();
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
                this.ListSereServCls = listSereServCls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
