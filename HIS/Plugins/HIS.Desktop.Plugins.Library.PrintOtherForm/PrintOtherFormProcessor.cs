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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000227;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000242;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000246;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000345;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000400;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000402;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000403;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000404;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000405;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000406;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000407;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000408;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000409;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000410;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000411;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000412;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000413;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000414;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000416_1_;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000417_1_;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000418;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000419;
using HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000476;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm
{
    public partial class PrintOtherFormProcessor
    {
        private long treatmentId { get; set; }
        private long patientId { get; set; }
        private long servceReqId { get; set; }
        private long sereServId { get; set; }
        private long departmentId { get; set; }
        private long? treatmentBedRoomId { get; set; }
        private PrintOtherInputADO inputAdo { get; set; }

        private UpdateType.TYPE updateType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_id">Dua vao Update type</param>
        /// <param name="_updateType"></param>
        public PrintOtherFormProcessor(long? _serviceReqId, long? _sereServId, long _treatmentId, long _patientId, UpdateType.TYPE _updateType)
        {
            this.updateType = _updateType;
            this.treatmentId = _treatmentId;
            this.servceReqId = _serviceReqId ?? 0;
            this.sereServId = _sereServId ?? 0;
            this.patientId = _patientId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_id">Dua vao Update type</param>
        /// <param name="_updateType"></param>
        public PrintOtherFormProcessor(long _treatmentId, long _patientId, long _departmentId, UpdateType.TYPE _updateType)
        {
            this.updateType = _updateType;
            this.treatmentId = _treatmentId;
            this.departmentId = _departmentId;
            this.patientId = _patientId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_id">Dua vao Update type</param>
        /// <param name="_updateType"></param>
        public PrintOtherFormProcessor(long _serviceReqId, UpdateType.TYPE _updateType)
        {
            this.updateType = _updateType;
            this.servceReqId = _serviceReqId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_id">Dua vao Update type</param>
        /// <param name="_updateType"></param>
        public PrintOtherFormProcessor(PrintOtherInputADO _InputAdo, UpdateType.TYPE _updateType)
        {
            this.updateType = _updateType;
            this.treatmentId = _InputAdo.TreatmentId;
            this.servceReqId = _InputAdo.ServiceReqId;
            this.patientId = _InputAdo.PatientId;
            this.departmentId = _InputAdo.DepartmentId ?? 0;
            this.treatmentBedRoomId = _InputAdo.TreatmentBedRoomId;
            this.inputAdo = _InputAdo;
        }

        public void Print(PrintType.TYPE printType)
        {
            try
            {
                switch (printType)
                {
                    case PrintType.TYPE.PHIEU_PHAU_THUAT_THU_THUAT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_PHAU_THUAT_THU_THUAT);
                        break;
                    case PrintType.TYPE.PHIEU_THEO_DOI_THUOC:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_THOI_DOI_THUOC);
                        break;
                    case PrintType.TYPE.PHIEU_PHCN:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_PHUC_HOI_CHUC_NANG);
                        break;
                    case PrintType.TYPE.MPS000345_PHIEU_KHAM_GAY_ME_TRUOC_MO:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_KHAM_GAY_ME_TRUOC_MO);
                        break;
                    case PrintType.TYPE.MPS000402_PHIEU_DKSD_THUOC_DVKT_NGOAI_BHYT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_DKSD_THUOC_DVKT_NGAOAI_BHYT);
                        break;
                    case PrintType.TYPE.MPS000400_PHIEU_TU_VAN:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_TU_VAN);
                        break;
                    case PrintType.TYPE.MPS000403_CAM_KET_NAM_GIUONG_TU_NGUYEN:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___BAN_CAM_KET_NAM_GIUONG_TU_NGUYEN);
                        break;
                    case PrintType.TYPE.MPS000404_BAN_GIAO_BN_TRUOC_PTTT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___BAN_GIAO_BN_TRUOC_PTTT);
                        break;
                    case PrintType.TYPE.MPS000405_PHIEU_KHAI_THAC_TIEN_SU_DI_UNG:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_KHAI_THAC_TIEN_SU_DI_UNG);
                        break;
                    case PrintType.TYPE.MPS000406_PHIEU_XN_DUONG_MAU_MAO_MACH:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_XN_DUONG_MAU_MAO_MACH);
                        break;
                    case PrintType.TYPE.MPS000407_PHIEU_KHAM_BENH_CHUYEN_KHAM_PHONG_PTTT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_KHAM_BENH_CHUYEN_KHAM_PHONG_PTTT);
                        break;
                    case PrintType.TYPE.MPS000408_BANG_KIEN_AN_TOAN_PHAU_THUAT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KIEM_AN_TOAN_PTTT);
                        break;
                    case PrintType.TYPE.MPS000409_PHIEU_SANG_LOC_DINH_DUONG_NGUOI_BENH:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_SANG_LOC_DINH_DUONG_NGUOI_BENH);
                        break;
                    case PrintType.TYPE.MPS000410_BENH_AN_DIEU_TRI_NGOAI_TRU_PTTT_PHONG_KHAM_PTTT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___BENH_AN_DIEU_TRI_NGOAI_TRU_PTTT_PHONG_KHAM_PTTT);
                        break;
                    case PrintType.TYPE.MPS000411_BANG_GAY_ME_HOI_SUC:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_GAY_ME_HOI_SUC);
                        break;
                    case PrintType.TYPE.MPS000412_BANG_TRAC_NGHIEM_CO_VA_CAM_GIAC:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_TRAC_NGHIEM_CO_VA_CAM_GIAC);
                        break;
                    case PrintType.TYPE.MPS000413_PHIEU_PHAU_THUAT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_PHAU_THUAT);
                        break;
                    case PrintType.TYPE.MPS000414_PHIEU_THU_THUAT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_THU_THUAT);
                        break;
                    case PrintType.TYPE.MPS000416_SO_KET_BENH_AN_TRUOC_PHAU_THUAT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___SO_KET_BENH_AN_TRUOC_PHAU_THUAT);
                        break;
                    case PrintType.TYPE.MPS000417_SO_KET_BENH_AN_TRUOC_THU_THUAT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___SO_KET_BENH_AN_TRUOC_THU_THUAT);
                        break;
                    case PrintType.TYPE.MPS000418_BENH_AN_NGOAI_TRU_DAY_MAT:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___BENH_AN_NGOAI_TRU_DAY_MAT);
                        break;
                    case PrintType.TYPE.MPS000419_BENH_AN_NGOAI_TRU_GLAUCOMA:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___BENH_AN_NGOAI_TRU_GLAUCOMA);
                        break;
                    case PrintType.TYPE.MPS000476_CHAN_DOAN_NGUYEN_NHAN_TU_VONG:
                        this.RunPrint(PrintTypeCodeWorker.PRINT_TYPE_CODE___CHAN_DOAN_TU_VONG);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RunPrint(string printCode)
        {
            try
            {
                ILoad loadMps = null;
                switch (printCode)
                {
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_PHAU_THUAT_THU_THUAT:
                        loadMps = new Mps000227Behavior(servceReqId, sereServId, treatmentId, patientId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_THOI_DOI_THUOC:
                        loadMps = new Mps000242Behavior(treatmentId, patientId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_PHUC_HOI_CHUC_NANG:
                        loadMps = new Mps000246Behavior(servceReqId, sereServId, treatmentId, patientId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_KHAM_GAY_ME_TRUOC_MO:
                        loadMps = new Mps000345Behavior(servceReqId, sereServId, treatmentId, patientId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_DKSD_THUOC_DVKT_NGAOAI_BHYT:
                        loadMps = new Mps000402Behavior(treatmentId, patientId, departmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_TU_VAN:
                        loadMps = new Mps000400Behavior(servceReqId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BAN_CAM_KET_NAM_GIUONG_TU_NGUYEN:
                        loadMps = new Mps000403Behavior(treatmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BAN_GIAO_BN_TRUOC_PTTT:
                        loadMps = new Mps000404Behavior(treatmentId, departmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_KHAI_THAC_TIEN_SU_DI_UNG:
                        loadMps = new Mps000405Behavior(treatmentId, departmentId, treatmentBedRoomId ?? 0);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_XN_DUONG_MAU_MAO_MACH:
                        loadMps = new Mps000406Behavior(this.inputAdo);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_KHAM_BENH_CHUYEN_KHAM_PHONG_PTTT:
                        loadMps = new Mps000407Behavior(this.inputAdo);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_KIEM_AN_TOAN_PTTT:
                        loadMps = new Mps000408Behavior(this.inputAdo);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_SANG_LOC_DINH_DUONG_NGUOI_BENH:
                        loadMps = new Mps000409Behavior(treatmentId, departmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BENH_AN_DIEU_TRI_NGOAI_TRU_PTTT_PHONG_KHAM_PTTT:
                        loadMps = new Mps000410Behavior(treatmentId, departmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_GAY_ME_HOI_SUC:
                        loadMps = new Mps000411Behavior(this.inputAdo);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BANG_TRAC_NGHIEM_CO_VA_CAM_GIAC:
                        loadMps = new Mps000412Behavior(treatmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_PHAU_THUAT:
                        loadMps = new Mps000413Behavior(this.inputAdo);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___PHIEU_THU_THUAT:
                        loadMps = new Mps000414Behavior(this.inputAdo);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___SO_KET_BENH_AN_TRUOC_PHAU_THUAT:
                        loadMps = new Mps000416Behavior(treatmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___SO_KET_BENH_AN_TRUOC_THU_THUAT:
                        loadMps = new Mps000417Behavior(treatmentId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BENH_AN_NGOAI_TRU_DAY_MAT:
                        loadMps = new Mps000418Behavior(treatmentId, patientId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___BENH_AN_NGOAI_TRU_GLAUCOMA:
                        loadMps = new Mps000419Behavior(treatmentId, patientId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___CHAN_DOAN_TU_VONG:
                        loadMps = new Mps000476Behavior(treatmentId);
                        break;
                    default:
                        break;
                }

                bool result = loadMps != null ? loadMps.Load(printCode, updateType) : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
