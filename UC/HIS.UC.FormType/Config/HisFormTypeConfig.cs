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
using HIS.UC.FormType.HisMultiGetString;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Config
{
    public class HisFormTypeConfig
    {
        private static List<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT> ServiceUnit;
        public static List<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT> HisServiceUnit
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT));
                if (ServiceUnit == null || ServiceUnit.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceUnitFilter filter = new MOS.Filter.HisServiceUnitFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    ServiceUnit = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(RequestUriStore.HIS_SERVICE_UNIT_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return ServiceUnit.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                ServiceUnit = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE> bloodType;
        public static List<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE> HisBloodType
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE));
                if (bloodType == null || bloodType.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisBloodTypeFilter filter = new MOS.Filter.HisBloodTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    bloodType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>(RequestUriStore.HIS_BLOOD_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return bloodType.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                bloodType = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_FUND> fund;
        public static List<MOS.EFMODEL.DataModels.HIS_FUND> HisFund
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_FUND));
                if (fund == null || fund.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisFundFilter filter = new MOS.Filter.HisFundFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    fund = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_FUND>(RequestUriStore.HIS_FUND_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return fund.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                fund = value;
            }
        }

        //private static List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL> hisMaterials;
        //public static List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL> HisMaterials
        //{
        //    get
        //    {
        //        //if (hisMaterials == null || hisMaterials.Count == 0)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisMaterialViewFilter filter = new MOS.Filter.HisMaterialViewFilter();
        //            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        //            hisMaterials = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL>(RequestUriStore.HIS_MATERIAL_GETVIEW, ApiConsumerStore.MosConsumer, filter);

        //        }
        //        return hisMaterials;
        //    }
        //    set
        //    {
        //        hisMaterials = value;
        //    }
        //}


        //private static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE> hisMedicines;
        //public static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE> HisMedicines
        //{
        //    get
        //    {
        //        //if (hisMedicines == null || hisMedicines.Count == 0)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisMedicineViewFilter filter = new MOS.Filter.HisMedicineViewFilter();
        //            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        //            hisMedicines = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE>(RequestUriStore.HIS_MEDICINE_GETVIEW, ApiConsumerStore.MosConsumer, filter);

        //        }
        //        return hisMedicines;
        //    }
        //    set
        //    {
        //        hisMedicines = value;
        //    }
        //}

        private static List<MOS.EFMODEL.DataModels.HIS_SUPPLIER> hisSuppliers;
        public static List<MOS.EFMODEL.DataModels.HIS_SUPPLIER> HisSuppliers
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_SUPPLIER));
                if (hisSuppliers == null || hisSuppliers.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisSupplierFilter filter = new MOS.Filter.HisSupplierFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisSuppliers = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>(RequestUriStore.HIS_SUPPLIER_GET, ApiConsumerStore.MosConsumer, filter);
                    hisSuppliers = hisSuppliers.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
.ToList();
                }
                return hisSuppliers.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisSuppliers = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_INVOICE> hisInvoices;
        public static List<MOS.EFMODEL.DataModels.HIS_INVOICE> HisInvoices
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_INVOICE));
                if (hisInvoices == null || hisInvoices.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisInvoiceFilter filter = new MOS.Filter.HisInvoiceFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisInvoices = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_INVOICE>(RequestUriStore.HIS_INVOICE_GET, ApiConsumerStore.MosConsumer, filter);
                    hisInvoices = hisInvoices.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
.ToList();
                }
                return hisInvoices.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisInvoices = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK> hisInvoiceBooks;
        public static List<MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK> HisInvoiceBooks
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK));
                if (hisInvoiceBooks == null || hisInvoiceBooks.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisInvoiceBookFilter filter = new MOS.Filter.HisInvoiceBookFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisInvoiceBooks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK>(RequestUriStore.HIS_INVOICE_BOOK_GET, ApiConsumerStore.MosConsumer, filter);
                    hisInvoiceBooks = hisInvoiceBooks.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
.ToList();
                }
                return hisInvoiceBooks.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisInvoiceBooks = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_IMP_SOURCE> hisImpSources;
        public static List<MOS.EFMODEL.DataModels.HIS_IMP_SOURCE> HisImpSources
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_IMP_SOURCE));
                if (hisImpSources == null || hisImpSources.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisImpSourceFilter filter = new MOS.Filter.HisImpSourceFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisImpSources = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_SOURCE>(RequestUriStore.HIS_IMP_SOURCE_GET, ApiConsumerStore.MosConsumer, filter);
                    hisImpSources = hisImpSources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            .ToList();
                }
                return hisImpSources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisImpSources = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM> userRooms;
        public static List<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM> VHisUserRooms
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_USER_ROOM));
                if (userRooms == null || userRooms.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisUserRoomViewFilter filter = new MOS.Filter.HisUserRoomViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    userRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>(RequestUriStore.HIS_USER_ROOM_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                    userRooms = userRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
&& o.IS_PAUSE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            .ToList();
                }
                return userRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                userRooms = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> mestRooms;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> VHisMestRooms
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM));
                if (mestRooms == null || mestRooms.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMestRoomViewFilter filter = new MOS.Filter.HisMestRoomViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    mestRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>(RequestUriStore.HIS_MEST_ROOM_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                    mestRooms = mestRooms
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.IS_PAUSE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        .ToList();
                }
                return mestRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                mestRooms = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE> mestPatientTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE> HisMestPatientTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE));
                if (mestPatientTypes == null || mestPatientTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMestPatientTypeFilter filter = new MOS.Filter.HisMestPatientTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    mestPatientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>(RequestUriStore.HIS_MEST_PATIENT_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return mestPatientTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                mestPatientTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM> vBedRooms;
        public static List<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM> VHisBedRooms
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_BED_ROOM));
                if (vBedRooms == null || vBedRooms.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisBedRoomViewFilter filter = new MOS.Filter.HisBedRoomViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vBedRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>(RequestUriStore.HIS_BED_ROOM_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                    vBedRooms = vBedRooms
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.IS_PAUSE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        .ToList();
                }
                return vBedRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vBedRooms = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_BED> vBeds;
        public static List<MOS.EFMODEL.DataModels.V_HIS_BED> VHisBeds
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_BED));
                if (vBeds == null || vBeds.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisBedViewFilter filter = new MOS.Filter.HisBedViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vBeds = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED>(RequestUriStore.HIS_BED_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vBeds.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vBeds = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE> vExpMestTemplates;
        public static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE> VHisExpMestTemplates
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE));
                if (vExpMestTemplates == null || vExpMestTemplates.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestTemplateFilter filter = new MOS.Filter.HisExpMestTemplateFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vExpMestTemplates = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>(RequestUriStore.HIS_EXP_MEST_TEMPLATE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return vExpMestTemplates.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vExpMestTemplates = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> vExecuteRooms;
        public static List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> VHisExecuteRooms
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM));
                if (vExecuteRooms == null || vExecuteRooms.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExecuteRoomViewFilter filter = new MOS.Filter.HisExecuteRoomViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vExecuteRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>(RequestUriStore.HIS_EXECUTE_ROOM_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                    vExecuteRooms = vExecuteRooms
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.IS_PAUSE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        .ToList();
                }
                return vExecuteRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vExecuteRooms = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> departments;
        public static List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> HisDepartments
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_DEPARTMENT));
                if (departments == null || departments.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisDepartmentFilter filter = new MOS.Filter.HisDepartmentFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    departments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(RequestUriStore.HIS_DEPARTMENT_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return departments.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                departments = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> deactiveDepartments;
        public static List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> HisDeactiveDepartments
        {
            get
            {
                //if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_DEPARTMENT));
                if (deactiveDepartments == null || deactiveDepartments.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisDepartmentFilter filter = new MOS.Filter.HisDepartmentFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    deactiveDepartments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(RequestUriStore.HIS_DEPARTMENT_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return deactiveDepartments;
            }
            set
            {
                deactiveDepartments = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE> dEmployees;
        public static List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE> HisDEmployees
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_EMPLOYEE));
                if (dEmployees == null || dEmployees.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisEmployeeFilter filter = new MOS.Filter.HisEmployeeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    dEmployees = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>("api/HisEmployee/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return dEmployees.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                dEmployees = value;
            }
        }

        private static List<DataGet> doctorDepas;
        public static List<DataGet> HisDoctorDepas
        {
            get
            {
                if (doctorDepas == null || doctorDepas.Count == 0)
                {
                    Dictionary<long, string> dicDepa = Config.HisFormTypeConfig.HisDeactiveDepartments.ToDictionary(p => p.ID, q => q.DEPARTMENT_NAME);
                    Dictionary<string, string> dicDoctor = HisDEmployees.Where(o => o.IS_DOCTOR == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToDictionary(p => p.LOGINNAME, q => dicDepa.ContainsKey(q.DEPARTMENT_ID ?? 0) ? dicDepa[q.DEPARTMENT_ID ?? 0] : "");

                    doctorDepas = Config.AcsFormTypeConfig.HisAcsUser.Where(o => dicDoctor.ContainsKey(o.LOGINNAME ?? "")).Select(o => new DataGet()
                    {
                        ID = o.ID,
                        CODE = o.LOGINNAME,
                        NAME = string.IsNullOrWhiteSpace(dicDoctor[o.LOGINNAME ?? ""]) ? o.USERNAME : string.Format("{0} (Khoa: {1})", o.USERNAME, dicDoctor[o.LOGINNAME ?? ""])
                    }).ToList();
                }
                return doctorDepas;
            }
            set
            {
                doctorDepas = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_ACCIDENT_RESULT> hisAccidentResults;
        public static List<MOS.EFMODEL.DataModels.HIS_ACCIDENT_RESULT> HisAccidentResults
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_ACCIDENT_RESULT));
                if (hisAccidentResults == null || hisAccidentResults.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisAccidentResultFilter filter = new MOS.Filter.HisAccidentResultFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisAccidentResults = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ACCIDENT_RESULT>("api/HisAccidentResult/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return hisAccidentResults.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisAccidentResults = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT_TYPE> hisAccidentHurtTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT_TYPE> HisAccidentHurtTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT_TYPE));
                if (hisAccidentHurtTypes == null || hisAccidentHurtTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisAccidentHurtTypeFilter filter = new MOS.Filter.HisAccidentHurtTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisAccidentHurtTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT_TYPE>("api/HisAccidentHurtType/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return hisAccidentHurtTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisAccidentHurtTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT> kskContracts;
        public static List<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT> HisKskContracts
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT));
                if (kskContracts == null || kskContracts.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisKskContractFilter filter = new MOS.Filter.HisKskContractFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    kskContracts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>(RequestUriStore.HIS_KSK_CONTRACT_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return kskContracts.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                kskContracts = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_KSK_CONTRACT> kskContractsView;
        public static List<MOS.EFMODEL.DataModels.V_HIS_KSK_CONTRACT> HisKskContractsView
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_KSK_CONTRACT));
                if (kskContractsView == null || kskContractsView.Count == 0)
                {
                    //api getview lỗi tạm thời map bằng tay
                    kskContractsView = new List<MOS.EFMODEL.DataModels.V_HIS_KSK_CONTRACT>();
                    foreach (var item in HisKskContracts)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_KSK_CONTRACT ksk = new MOS.EFMODEL.DataModels.V_HIS_KSK_CONTRACT();

                        ksk.ID = item.ID;
                        ksk.KSK_CONTRACT_CODE = item.KSK_CONTRACT_CODE;
                        ksk.WORK_PLACE_ID = item.WORK_PLACE_ID;
                        ksk.CONTRACT_DATE = item.CONTRACT_DATE;
                        ksk.CONTRACT_VALUE = item.CONTRACT_VALUE;
                        ksk.DEPOSIT_AMOUNT = item.DEPOSIT_AMOUNT;
                        ksk.EFFECT_DATE = item.EFFECT_DATE;
                        ksk.EXPIRY_DATE = item.EXPIRY_DATE;
                        ksk.PAYMENT_RATIO = item.PAYMENT_RATIO;
                        ksk.IS_ACTIVE = item.IS_ACTIVE;

                        var work = HisWorkPlaces.FirstOrDefault(o => o.ID == item.WORK_PLACE_ID);
                        if (work != null)
                        {
                            ksk.WORK_PLACE_CODE = work.WORK_PLACE_CODE;
                            ksk.WORK_PLACE_NAME = work.WORK_PLACE_NAME;
                        }

                        kskContractsView.Add(ksk);
                    }
                }
                return kskContractsView.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                kskContractsView = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_BRANCH> hisBranchs;
        public static List<MOS.EFMODEL.DataModels.HIS_BRANCH> HisBranchs
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_BRANCH));
                if (hisBranchs == null || hisBranchs.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisBranchFilter filter = new MOS.Filter.HisBranchFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisBranchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>(RequestUriStore.HIS_BRANCH_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return hisBranchs.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisBranchs = value;
            }
        }

        //public static List<AgeADO> Ages
        //{
        //    get
        //    {
        //        List<AgeADO> ages = new List<AgeADO>();
        //        AgeADO kh1 = new AgeADO(1, "Tuổi");
        //        ages.Add(kh1);

        //        AgeADO kh2 = new AgeADO(2, "Tháng");
        //        ages.Add(kh2);

        //        AgeADO kh3 = new AgeADO(3, "Ngày");
        //        ages.Add(kh3);

        //        AgeADO kh4 = new AgeADO(4, "Giờ");
        //        ages.Add(kh4);

        //        return ages;
        //    }
        //}

        //private static List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE> examServiceTypes;
        //public static List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE> HisExamServiceTypes
        //{
        //    get
        //    {
        //        //if (examServiceTypes == null || examServiceTypes.Count == 0)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisExamServiceTypeFilter filter = new MOS.Filter.HisExamServiceTypeFilter();
        //            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        //            examServiceTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE>(RequestUriStore.HIS_EXAM_SERVICE_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
        //        }
        //        return examServiceTypes;
        //    }
        //    set
        //    {
        //        examServiceTypes = value;
        //    }
        //}

        private static List<MOS.EFMODEL.DataModels.HIS_CAREER> careers;
        public static List<MOS.EFMODEL.DataModels.HIS_CAREER> HisCareers
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_CAREER));
                if (careers == null || careers.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisCareerFilter filter = new MOS.Filter.HisCareerFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    careers = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>(RequestUriStore.HIS_CAREER_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return careers.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                careers = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE> deathCauses;
        public static List<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE> HisDeathCauses
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE));
                if (deathCauses == null || deathCauses.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisDeathCauseFilter filter = new MOS.Filter.HisDeathCauseFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    deathCauses = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE>(RequestUriStore.HIS_DEATH_CAUSE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return deathCauses.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                deathCauses = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_PROGRAM> nameProgram;
        public static List<MOS.EFMODEL.DataModels.HIS_PROGRAM> HisProgram
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_PROGRAM));
                if (nameProgram == null || nameProgram.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisProgramFilter filter = new MOS.Filter.HisProgramFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    nameProgram = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PROGRAM>(RequestUriStore.HIS_PROGRAM_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return nameProgram.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                nameProgram = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_PROGRAM> vPatientProgram;
        public static List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_PROGRAM> VHisPatientProgram
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_PATIENT_PROGRAM));
                if (vPatientProgram == null || vPatientProgram.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientProgramViewFilter filter = new MOS.Filter.HisPatientProgramViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vPatientProgram = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_PROGRAM>(RequestUriStore.HIS_PATIEN_PROGRAM_GET_VIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vPatientProgram.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vPatientProgram = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN> deathWithins;
        public static List<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN> HisDeathWithins
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN));
                if (deathWithins == null || deathWithins.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisDeathWithinFilter filter = new MOS.Filter.HisDeathWithinFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    deathWithins = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN>(RequestUriStore.HIS_DEATH_WITHIN_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return deathWithins.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                deathWithins = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_ROOM> vRooms;
        public static List<MOS.EFMODEL.DataModels.V_HIS_ROOM> VHisRooms
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_ROOM));
                if (vRooms == null || vRooms.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisRoomViewFilter filter = new MOS.Filter.HisRoomViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>(RequestUriStore.HIS_ROOM_GETVIEW, ApiConsumerStore.MosConsumer, filter);

                }
                return vRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vRooms = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> patientTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> HisPatientTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE));
                if (patientTypes == null || patientTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                    filter.ORDER_FIELD = "PATIENT_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(RequestUriStore.HIS_PATIENT_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return patientTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                patientTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_PTTT_GROUP> hisPTTTGroups;
        public static List<MOS.EFMODEL.DataModels.HIS_PTTT_GROUP> HisPTTTGroups
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_PTTT_GROUP));
                if (hisPTTTGroups == null || hisPTTTGroups.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPtttGroupFilter filter = new MOS.Filter.HisPtttGroupFilter();

                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisPTTTGroups = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_GROUP>(RequestUriStore.HIS_PTTT_GROUP_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return hisPTTTGroups.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisPTTTGroups = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE> impMestTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE> HisImpMestTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE));
                if (impMestTypes == null || impMestTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                    filter.ORDER_FIELD = "IMP_MEST_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    impMestTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE>(RequestUriStore.HIS_IMP_MEST_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return impMestTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                impMestTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT> impMestStts;
        public static List<MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT> HisImpMestStts
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT));
                if (impMestStts == null || impMestStts.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisImpMestSttFilter filter = new MOS.Filter.HisImpMestSttFilter();
                    filter.ORDER_FIELD = "IMP_MEST_STT_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    impMestStts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT>(RequestUriStore.HIS_IMP_MEST_STT_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return impMestStts.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                impMestStts = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE> roomTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE> HisRoomTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_ROOM_TYPE));
                if (roomTypes == null || roomTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisRoomTypeFilter filter = new MOS.Filter.HisRoomTypeFilter();
                    filter.ORDER_FIELD = "PATIENT_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    roomTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE>(RequestUriStore.HIS_ROOM_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return roomTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                roomTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW> vpatientTypeAllows;
        public static List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW> VHisPatientTypeAllows
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW));
                if (vpatientTypeAllows == null || vpatientTypeAllows.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                    filter.ORDER_FIELD = "PATIENT_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vpatientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>(RequestUriStore.HIS_PATIENT_TYPE_ALLOW_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return vpatientTypeAllows.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vpatientTypeAllows = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_ICD> icds;
        public static List<MOS.EFMODEL.DataModels.HIS_ICD> HisIcds
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_ICD));
                if (icds == null || icds.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisIcdFilter filter = new MOS.Filter.HisIcdFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    icds = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>(RequestUriStore.HIS_ICD_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return icds.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                icds = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP> executeGroups;
        public static List<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP> HisExecuteGroups
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP));
                if (executeGroups == null || executeGroups.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExecuteGroupFilter filter = new MOS.Filter.HisExecuteGroupFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    executeGroups = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP>(RequestUriStore.HIS_EXECUTE_GROUP_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return executeGroups.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                executeGroups = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> tranPatiForms;
        public static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> HisTranPatiForms
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM));
                if (tranPatiForms == null || tranPatiForms.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTranPatiFormFilter filter = new MOS.Filter.HisTranPatiFormFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    tranPatiForms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>(RequestUriStore.HIS_TRAN_PATI_FORM_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return tranPatiForms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                tranPatiForms = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> payForms;
        public static List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> HisPayForms
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM));
                if (payForms == null || payForms.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPayFormFilter filter = new MOS.Filter.HisPayFormFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    payForms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>(RequestUriStore.HIS_PAY_FORM_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return payForms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                payForms = value;
            }
        }

        //private static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TYPE> tranPatiTypes;
        //public static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TYPE> HisTranPatiTypes
        //{
        //    get
        //    {
        //        //if (tranPatiTypes == null || tranPatiTypes.Count == 0)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisTranPatiTypeFilter filter = new MOS.Filter.HisTranPatiTypeFilter();
        //            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        //            tranPatiTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TYPE>(RequestUriStore.HIS_TRAN_PATI_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
        //        }
        //        return tranPatiTypes;
        //    }
        //    set
        //    {
        //        tranPatiTypes = value;
        //    }
        //}

        private static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> tranPatiReasons;
        public static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> HisTranPatiReasons
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON));
                if (tranPatiReasons == null || tranPatiReasons.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTranPatiReasonFilter filter = new MOS.Filter.HisTranPatiReasonFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    tranPatiReasons = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>(RequestUriStore.HIS_TRAN_PATI_REASON_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return tranPatiReasons.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                tranPatiReasons = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON> expMestReasons;
        public static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON> HisExpMestReasons
        {
            get
            {
                //if (expMestReasons == null || expMestReasons.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestReasonFilter filter = new MOS.Filter.HisExpMestReasonFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    expMestReasons = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON>(RequestUriStore.HIS_EXP_MEST_REASON_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return expMestReasons;
            }
            set
            {
                expMestReasons = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY> vservicePatys;
        public static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY> VHisServicePatys
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY));
                if (vservicePatys == null || vservicePatys.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServicePatyViewFilter filter = new MOS.Filter.HisServicePatyViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vservicePatys = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>(RequestUriStore.HIS_SERVICE_PATY_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vservicePatys.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vservicePatys = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts;
        public static List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> HisServiceReqStts
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT));
                if (serviceReqStts == null || serviceReqStts.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqSttFilter filter = new MOS.Filter.HisServiceReqSttFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    serviceReqStts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>(RequestUriStore.HIS_SERVICE_REQ_STT_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return serviceReqStts.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                serviceReqStts = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE> serviceReqTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE> HisServiceReqTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE));
                if (serviceReqTypes == null || serviceReqTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqSttFilter filter = new MOS.Filter.HisServiceReqSttFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    serviceReqTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE>(RequestUriStore.HIS_SERVICE_REQ_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return serviceReqTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                serviceReqTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_GENDER> genders;
        public static List<MOS.EFMODEL.DataModels.HIS_GENDER> HisGenders
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_GENDER));
                if (genders == null || genders.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisGenderFilter filter = new MOS.Filter.HisGenderFilter();
                    filter.ORDER_FIELD = "GENDER_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    genders = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>(RequestUriStore.HIS_GENDER_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return genders.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                genders = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_TEXT_LIB> textLibs;
        public static List<MOS.EFMODEL.DataModels.HIS_TEXT_LIB> HisTextLibs
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_TEXT_LIB));
                if (textLibs == null || textLibs.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTextLibFilter filter = new MOS.Filter.HisTextLibFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    textLibs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>(RequestUriStore.HIS_TEXT_LIB_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return textLibs.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                textLibs = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> medicineUseForms;
        public static List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> HisMedicineUseForms
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM));
                if (medicineUseForms == null || medicineUseForms.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMedicineUseFormFilter filter = new MOS.Filter.HisMedicineUseFormFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    medicineUseForms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>(RequestUriStore.HIS_MEDICINE_USE_FORM_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return medicineUseForms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                medicineUseForms = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_DEBATE_TYPE> debateTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_DEBATE_TYPE> HisDebateTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_DEBATE_TYPE));
                if (debateTypes == null || debateTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisAreaFilter filter = new MOS.Filter.HisAreaFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    debateTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEBATE_TYPE>("api/HisDebateType/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return debateTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                debateTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_BID_TYPE> bidTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_BID_TYPE> HisBidTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_BID_TYPE));
                if (bidTypes == null || bidTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisBidTypeFilter filter = new MOS.Filter.HisBidTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    bidTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BID_TYPE>("api/HisBidType/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return bidTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                bidTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_DATA_STORE> dataStores;
        public static List<MOS.EFMODEL.DataModels.HIS_DATA_STORE> HisDataStores
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_DATA_STORE));
                if (dataStores == null || dataStores.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisDataStoreFilter filter = new MOS.Filter.HisDataStoreFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    dataStores = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DATA_STORE>("api/HisDataStore/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return dataStores.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                dataStores = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_NONE_MEDI_SERVICE> noneMediServices;
        public static List<MOS.EFMODEL.DataModels.HIS_NONE_MEDI_SERVICE> HisNoneMediServices
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_NONE_MEDI_SERVICE));
                if (noneMediServices == null || noneMediServices.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisNoneMediServiceFilter filter = new MOS.Filter.HisNoneMediServiceFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    noneMediServices = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_NONE_MEDI_SERVICE>("api/HisNoneMediService/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return noneMediServices.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                noneMediServices = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE> otherPaySources;
        public static List<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE> HisOtherPaySources
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE));
                if (otherPaySources == null || otherPaySources.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisOtherPaySourceFilter filter = new MOS.Filter.HisOtherPaySourceFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    otherPaySources = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>("api/HisOtherPaySource/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return otherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                otherPaySources = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY> patientClassifys;
        public static List<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY> HisPatientClassifys
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY));
                if (patientClassifys == null || patientClassifys.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientClassifyFilter filter = new MOS.Filter.HisPatientClassifyFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    patientClassifys = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>("api/HisPatientClassify/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return patientClassifys.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                patientClassifys = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_AREA> areas;
        public static List<MOS.EFMODEL.DataModels.HIS_AREA> HisAreas
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_AREA));
                if (areas == null || areas.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisAreaFilter filter = new MOS.Filter.HisAreaFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    areas = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_AREA>(RequestUriStore.HIS_AREA_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return areas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                areas = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_MEDICINE_GROUP> medicineGroups;
        public static List<MOS.EFMODEL.DataModels.HIS_MEDICINE_GROUP> HisMedicineGroups
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_GROUP));
                if (medicineGroups == null || medicineGroups.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMedicineGroupFilter filter = new MOS.Filter.HisMedicineGroupFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    medicineGroups = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_GROUP>(RequestUriStore.HIS_MEDICINE_GROUP_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return medicineGroups.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                medicineGroups = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE> mediReactTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE> HisMediReactTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE));
                if (mediReactTypes == null || mediReactTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMediReactTypeFilter filter = new MOS.Filter.HisMediReactTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    mediReactTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE>(RequestUriStore.HIS_MEDI_REACT_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return mediReactTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                mediReactTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT> expMestStts;
        public static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT> HisExpMestStts
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT));
                if (expMestStts == null || expMestStts.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestSttFilter filter = new MOS.Filter.HisExpMestSttFilter();
                    filter.ORDER_FIELD = "EXP_MEST_STT_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    expMestStts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT>(RequestUriStore.HIS_EXP_MEST_STT_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return expMestStts.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                expMestStts = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE> treatmentEndTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE> HisTreatmentEndTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE));
                if (treatmentEndTypes == null || treatmentEndTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentEndTypeFilter filter = new MOS.Filter.HisTreatmentEndTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    treatmentEndTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE>(RequestUriStore.HIS_TREATMENT_END_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return treatmentEndTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                treatmentEndTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT> treatmentResults;
        public static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT> HisTreatmentResults
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT));
                if (treatmentResults == null || treatmentResults.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentResultFilter filter = new MOS.Filter.HisTreatmentResultFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    treatmentResults = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT>(RequestUriStore.HIS_TREATMENT_RESULT_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return treatmentResults.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                treatmentResults = value;
            }
        }

        //private static List<DiseaseRelationADO> diseaseRelations;
        //public static List<DiseaseRelationADO> HisDiseaseRelations
        //{
        //    get
        //    {
        //        //if (diseaseRelations == null || diseaseRelations.Count == 0)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisDiseaseRelationFilter filter = new MOS.Filter.HisDiseaseRelationFilter();
        //            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        //            diseaseRelations = new HisDiseaseRelationDAL(param).GetRequest<List<DiseaseRelationADO>(RequestUriStore.HIS_DISEASE_RELATION_GET, ApiConsumerStore.MosConsumer, filter);
        //            //if (diseaseRelations != null)
        //            {
        //                foreach (var item in diseaseRelations)
        //                {
        //                    item.DISEASE_RELATION_ID = item.ID;
        //                    item.ID = 0;
        //                }
        //            }
        //        }
        //        return diseaseRelations;
        //    }
        //    set
        //    {
        //        diseaseRelations = value;
        //    }
        //}

        private static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PACKAGE> vServicePackages;
        public static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PACKAGE> VHisServicePackages
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PACKAGE));
                if (vServicePackages == null || vServicePackages.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServicePackageViewFilter filter = new MOS.Filter.HisServicePackageViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vServicePackages = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PACKAGE>(RequestUriStore.HIS_SERVICE_PACKAGE_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vServicePackages.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vServicePackages = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> services;
        public static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> VHisServices
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE));
                if (services == null || services.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceViewFilter filter = new MOS.Filter.HisServiceViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    services = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>(RequestUriStore.HIS_SERVICE_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return services.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                services = value;
            }
        }

        private static List<DataGet> serviceDeactives;
        public static List<DataGet> VHisServiceDeactives
        {
            get
            {
                //if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE));
                if (serviceDeactives == null || serviceDeactives.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceViewFilter filter = new MOS.Filter.HisServiceViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>(RequestUriStore.HIS_SERVICE_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                    if (result != null)
                    {
                        serviceDeactives = result.Select(o => new DataGet() { ID = o.ID, CODE = o.SERVICE_CODE, NAME = o.SERVICE_NAME, PARENT = o.SERVICE_TYPE_ID }).ToList();
                    }
                }
                return serviceDeactives;//.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                serviceDeactives = value;
            }
        }

        private static List<DataGet> serviceFulls;
        public static List<DataGet> VHisServiceFulls
        {
            get
            {
                //if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE));
                if (serviceFulls == null || serviceFulls.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceFilter filter = new MOS.Filter.HisServiceFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var serviceTemps = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE>(RequestUriStore.HIS_SERVICE_GET, ApiConsumerStore.MosConsumer, filter);
                    serviceFulls = serviceTemps.Select(o => new DataGet() { ID = o.ID, CODE = o.SERVICE_CODE, NAME = o.SERVICE_NAME, PARENT = o.SERVICE_TYPE_ID }).ToList();
                }
                return serviceFulls;
            }
            set
            {
                serviceFulls = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> servicesRooms;
        public static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> VHisServiceRooms
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM));
                if (servicesRooms == null || servicesRooms.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceViewFilter filter = new MOS.Filter.HisServiceViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    servicesRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>(RequestUriStore.HIS_SERVICE_ROOM_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return servicesRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                servicesRooms = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP> serviceGroups;
        public static List<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP> VHisServiceGroups
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP));
                if (serviceGroups == null || serviceGroups.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceGroupFilter filter = new MOS.Filter.HisServiceGroupFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    serviceGroups = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>(RequestUriStore.HIS_SERVICE_GROUP_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return serviceGroups.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                serviceGroups = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_SERV_SEGR> servSegrs;
        public static List<MOS.EFMODEL.DataModels.V_HIS_SERV_SEGR> VHisServSegrs
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_SERV_SEGR));
                if (servSegrs == null || servSegrs.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServSegrViewFilter filter = new MOS.Filter.HisServSegrViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    servSegrs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERV_SEGR>(RequestUriStore.HIS_SERV_SEGR_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return servSegrs.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                servSegrs = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE> serviceTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE> HisServiceTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE));
                if (serviceTypes == null || serviceTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServSegrViewFilter filter = new MOS.Filter.HisServSegrViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    serviceTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>(RequestUriStore.HIS_SERVICE_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return serviceTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                serviceTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> vHisMediStocks;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> VHisMediStock
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK));
                if (vHisMediStocks == null || vHisMediStocks.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMediStockViewFilter filter = new MOS.Filter.HisMediStockViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vHisMediStocks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>(RequestUriStore.HIS_MEDI_STOCK_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vHisMediStocks.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vHisMediStocks = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK> hisMediStocks;
        public static List<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK> HisMediStocks
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK));
                if (hisMediStocks == null || hisMediStocks.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMediStockFilter filter = new MOS.Filter.HisMediStockFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisMediStocks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>(RequestUriStore.HIS_MEDI_STOCK_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return hisMediStocks.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisMediStocks = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_PERIOD> vHisMediStockPeriods;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_PERIOD> VHisMediStockPeriod
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_PERIOD));
                if (vHisMediStockPeriods == null || vHisMediStockPeriods.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMediStockPeriodViewFilter filter = new MOS.Filter.HisMediStockPeriodViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vHisMediStockPeriods = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_PERIOD>(RequestUriStore.HIS_MEDI_STOCK_PERIOD_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vHisMediStockPeriods.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vHisMediStockPeriods = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE> expMestTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE> HisExpMestTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE));
                if (expMestTypes == null || expMestTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestTypeFilter filter = new MOS.Filter.HisExpMestTypeFilter();
                    //filter.ORDER_FIELD = "EXP_MEST_TYPE_CODE";
                    //filter.ORDER_DIRECTION = "ASC";
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    expMestTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE>(RequestUriStore.HIS_EXP_MEST_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return expMestTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                expMestTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM> cashierRooms;
        public static List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM> HisCashierRooms
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM));
                if (cashierRooms == null || cashierRooms.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisCashierRoomViewFilter filter = new MOS.Filter.HisCashierRoomViewFilter();
                    //filter.ORDER_FIELD = "CASHIER_ROOM_CODE";
                    //filter.ORDER_DIRECTION = "ASC";
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    cashierRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>(RequestUriStore.HIS_CASHIER_ROOM_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return cashierRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                cashierRooms = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> vMedicineTypeAcins;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> VHisMedicineTypeAcins
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN));
                if (vMedicineTypeAcins == null || vMedicineTypeAcins.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMedicineTypeAcinViewFilter filter = new MOS.Filter.HisMedicineTypeAcinViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vMedicineTypeAcins = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>(RequestUriStore.HIS_MEDICINE_TYPE_ACIN_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vMedicineTypeAcins.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vMedicineTypeAcins = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE> vTestIndexRanges;
        public static List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE> VHisTestIndexRanges
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE));
                if (vTestIndexRanges == null || vTestIndexRanges.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTestIndexRangeViewFilter filter = new MOS.Filter.HisTestIndexRangeViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vTestIndexRanges = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE>(RequestUriStore.HIS_HIS_TEST_INDEX_RANGE_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vTestIndexRanges.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vTestIndexRanges = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> treatmentTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> HisTreatmentTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE));
                if (treatmentTypes == null || treatmentTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentTypeFilter filter = new MOS.Filter.HisTreatmentTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    treatmentTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>(RequestUriStore.HIS_TREATMENT_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return treatmentTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                treatmentTypes = value;
            }
        }

        //private static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_LOG_TYPE> treatmentLogTypes;
        //public static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_LOG_TYPE> HisTreatmentLogTypes
        //{
        //    get
        //    {
        //        //if (treatmentLogTypes == null || treatmentLogTypes.Count == 0)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisTreatmentLogTypeFilter filter = new MOS.Filter.HisTreatmentLogTypeFilter();
        //            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        //            treatmentLogTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_LOG_TYPE>(RequestUriStore.HIS_TREATMENT_LOG_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
        //        }
        //        return treatmentLogTypes;
        //    }
        //    set
        //    {
        //        treatmentLogTypes = value;
        //    }
        //}

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> medicineTypes;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> VHisMedicineTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE));
                if (medicineTypes == null || medicineTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMedicineTypeViewFilter filter = new MOS.Filter.HisMedicineTypeViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    medicineTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>(RequestUriStore.HIS_MEDICINE_TYPE_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return medicineTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                medicineTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN> medicineBeans;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN> VHisMedicineBeans
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN));
                if (medicineBeans == null || medicineBeans.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMedicineBeanViewFilter filter = new MOS.Filter.HisMedicineBeanViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    medicineBeans = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN>(RequestUriStore.HIS_MEDICINE_BEAN_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return medicineBeans.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                medicineBeans = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN> medicineBeanInStocks;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN> VHisMedicineBeanInStocks(long stock)
        {
            try
            {

                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineBeanViewFilter filter = new MOS.Filter.HisMedicineBeanViewFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.MEDI_STOCK_ID = stock;
                medicineBeanInStocks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN>(RequestUriStore.HIS_MEDICINE_BEAN_GETVIEW, ApiConsumerStore.MosConsumer, filter);

                return medicineBeanInStocks.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN> materialBeanInStocks;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN> VHisMaterialBeanInStocks(long stock)
        {
            try
            {

                CommonParam param = new CommonParam();
                MOS.Filter.HisMaterialBeanViewFilter filter = new MOS.Filter.HisMaterialBeanViewFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.MEDI_STOCK_ID = stock;
                materialBeanInStocks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN>(RequestUriStore.HIS_MATERIAL_BEAN_GETVIEW, ApiConsumerStore.MosConsumer, filter);

                return materialBeanInStocks.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> vHisExpMestMedicines;
        public static List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> VHisExpMestMedicines
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE));
                if (vHisExpMestMedicines == null || vHisExpMestMedicines.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestMedicineViewFilter filter = new MOS.Filter.HisExpMestMedicineViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vHisExpMestMedicines = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>(RequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vHisExpMestMedicines.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vHisExpMestMedicines = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN> materialBeans;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN> VHisMaterialBeans
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN));
                if (materialBeans == null || materialBeans.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMaterialBeanViewFilter filter = new MOS.Filter.HisMaterialBeanViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    materialBeans = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN>(RequestUriStore.HIS_MATERIAL_BEAN_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return materialBeans.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                materialBeans = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> materialTypes;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> VHisMaterialTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE));
                if (materialTypes == null || materialTypes.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMaterialTypeViewFilter filter = new MOS.Filter.HisMaterialTypeViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    materialTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>(RequestUriStore.HIS_MATERIAL_TYPE_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return materialTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                materialTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL> vHisExpMestMaterials;
        public static List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL> VHisExpMestMaterials
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL));
                if (vHisExpMestMaterials == null || vHisExpMestMaterials.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestMaterialViewFilter filter = new MOS.Filter.HisExpMestMaterialViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vHisExpMestMaterials = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>(RequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vHisExpMestMaterials.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vHisExpMestMaterials = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK> militaryRanks;
        public static List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK> HisMilitaryRanks
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK));
                if (militaryRanks == null || militaryRanks.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMaterialTypeViewFilter filter = new MOS.Filter.HisMaterialTypeViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    militaryRanks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>(RequestUriStore.HIS_MILITARY_RANK_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return militaryRanks.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                militaryRanks = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> workPlaces;
        public static List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> HisWorkPlaces
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_WORK_PLACE));
                if (workPlaces == null || workPlaces.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisWorkPlaceFilter filter = new MOS.Filter.HisWorkPlaceFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    workPlaces = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>(RequestUriStore.HIS_WORK_PLACE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return workPlaces.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                workPlaces = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK> accountBooks;
        public static List<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK> HisAccountBooks
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK));
                if (accountBooks == null || accountBooks.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisWorkPlaceFilter filter = new MOS.Filter.HisWorkPlaceFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    accountBooks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK>(RequestUriStore.HIS_ACCOUNT_BOOK_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return accountBooks.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                accountBooks = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT> hisTreatments;
        public static List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT> HisTreatments
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_TREATMENT));
                if (hisTreatments == null || hisTreatments.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentViewFilter filter = new MOS.Filter.HisTreatmentViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisTreatments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>(RequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return hisTreatments.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisTreatments = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_ICD_GROUP> vHisIcdGroups;
        public static List<MOS.EFMODEL.DataModels.HIS_ICD_GROUP> VHisIcdGroups
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_ICD_GROUP));
                if (vHisIcdGroups == null || vHisIcdGroups.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisIcdGroupFilter filter = new MOS.Filter.HisIcdGroupFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vHisIcdGroups = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD_GROUP>(RequestUriStore.HIS_ICD_GROUP, ApiConsumerStore.MosConsumer, filter);
                }
                return vHisIcdGroups.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vHisIcdGroups = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_RETY_CAT> vHisServiceRetyCatLists;
        public static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_RETY_CAT> VHisServiceRetyCatLists
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_RETY_CAT));
                if (vHisServiceRetyCatLists == null || vHisServiceRetyCatLists.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceRetyCatViewFilter filter = new MOS.Filter.HisServiceRetyCatViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vHisServiceRetyCatLists = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_RETY_CAT>(RequestUriStore.HIS_SERVICE_RETY_CAT_GETVIEW, ApiConsumerStore.MosConsumer, filter);
                }
                return vHisServiceRetyCatLists.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vHisServiceRetyCatLists = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_PTTT_METHOD> methodPttt;
        public static List<MOS.EFMODEL.DataModels.HIS_PTTT_METHOD> MethodPttt
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_PTTT_METHOD));
                if (methodPttt == null || vHisServiceRetyCatLists.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceRetyCatViewFilter filter = new MOS.Filter.HisServiceRetyCatViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    methodPttt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_METHOD>(RequestUriStore.HIS_PTTT_METHOD, ApiConsumerStore.MosConsumer, filter);
                }
                return methodPttt.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                methodPttt = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_PTTT_GROUP> ptttGroup;
        public static List<MOS.EFMODEL.DataModels.HIS_PTTT_GROUP> HisPtttGroup
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_PTTT_GROUP));
                if (ptttGroup == null || ptttGroup.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceRetyCatViewFilter filter = new MOS.Filter.HisServiceRetyCatViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    ptttGroup = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_GROUP>(RequestUriStore.HIS_PTTT_GROUP, ApiConsumerStore.MosConsumer, filter);
                }
                return ptttGroup.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                ptttGroup = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD> emotionlessMethod;
        public static List<MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD> HisEmotionlessMethod
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD));
                if (emotionlessMethod == null || emotionlessMethod.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceRetyCatViewFilter filter = new MOS.Filter.HisServiceRetyCatViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    emotionlessMethod = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD>(RequestUriStore.HIS_EMOTIONLESS_METHOD, ApiConsumerStore.MosConsumer, filter);
                }
                return emotionlessMethod.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                emotionlessMethod = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_BLOOD> blood;
        public static List<MOS.EFMODEL.DataModels.HIS_BLOOD> HisBlood
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_BLOOD));
                if (blood == null || blood.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisBloodFilter filter = new MOS.Filter.HisBloodFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    blood = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD>(RequestUriStore.HIS_BLOOD, ApiConsumerStore.MosConsumer, filter);
                }
                return blood.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                blood = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_BLOOD_RH> bloodRh;
        public static List<MOS.EFMODEL.DataModels.HIS_BLOOD_RH> HisBloodRh
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_RH));
                if (bloodRh == null || bloodRh.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceRetyCatViewFilter filter = new MOS.Filter.HisServiceRetyCatViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    bloodRh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>(RequestUriStore.HIS_BLOOD_RH, ApiConsumerStore.MosConsumer, filter);
                }
                return bloodRh.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                bloodRh = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION> ptttCondition;
        public static List<MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION> HisPtttCondition
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION));
                if (ptttCondition == null || ptttCondition.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceRetyCatViewFilter filter = new MOS.Filter.HisServiceRetyCatViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    ptttCondition = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION>(RequestUriStore.HIS_PTTT_CONDITION, ApiConsumerStore.MosConsumer, filter);
                }
                return ptttCondition.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                ptttCondition = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE> ptttCatastrophe;
        public static List<MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE> HIsPtttCatastrophe
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE));
                if (ptttCatastrophe == null || ptttCatastrophe.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceRetyCatViewFilter filter = new MOS.Filter.HisServiceRetyCatViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    ptttCatastrophe = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE>(RequestUriStore.HIS_PTTT_CATASTROPHE, ApiConsumerStore.MosConsumer, filter);
                }
                return ptttCatastrophe.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                ptttCatastrophe = value;
            }
        }

        //private static List<MOS.EFMODEL.DataModels.HIS_PRICE_POLICY> pricePolicy;
        //public static List<MOS.EFMODEL.DataModels.HIS_PRICE_POLICY> HisPricePolicy
        //{
        //    get
        //    {
        //        //if (pricePolicy == null || pricePolicy.Count == 0)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisServiceRetyCatViewFilter filter = new MOS.Filter.HisServiceRetyCatViewFilter();
        //            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        //            pricePolicy = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PRICE_POLICY>(RequestUriStore.HIS_PRICE_POLICY, ApiConsumerStore.MosConsumer, filter);
        //        }
        //        return pricePolicy;
        //    }
        //    set
        //    {
        //        pricePolicy = value;
        //    }
        //}

        private static List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> executeRole;
        public static List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> HisExecuteRole
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE));
                if (executeRole == null || executeRole.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceRetyCatViewFilter filter = new MOS.Filter.HisServiceRetyCatViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    executeRole = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>(RequestUriStore.HIS_EXECUTE_ROLE, ApiConsumerStore.MosConsumer, filter);
                }
                return executeRole.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                executeRole = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_BID> vHisBids;
        public static List<MOS.EFMODEL.DataModels.HIS_BID> VHisBids
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_BID));
                if (vHisBids == null || vHisBids.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisBidViewFilter filter = new MOS.Filter.HisBidViewFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vHisBids = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BID>(RequestUriStore.HIS_BID, ApiConsumerStore.MosConsumer, filter);
                }
                return vHisBids.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                vHisBids = value;
            }
        }

        //private static List<MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI> vHisTranPati;
        //public static List<MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI> VHisTranPati
        //{
        //    get
        //    {
        //        //if (vHisTranPati == null || vHisTranPati.Count == 0)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisTranPatiViewFilter filter = new MOS.Filter.HisTranPatiViewFilter();
        //            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        //            vHisTranPati = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI>(RequestUriStore.HIS_BID, ApiConsumerStore.MosConsumer, filter);
        //        }
        //        return vHisTranPati;
        //    }
        //    set
        //    {
        //        vHisTranPati = value;
        //    }
        //}

        private static List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE> vHisServiceReqs;
        public static List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE> VHisServiceReqs
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE));
                if (vHisServiceReqs == null || vHisServiceReqs.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    vHisServiceReqs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE>(RequestUriStore.HIS_SERVICE_REQ_TYPE_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return vHisServiceReqs;
            }
            set
            {
                vHisServiceReqs = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_REPORT_TYPE_CAT> hisReportTypeCats;
        public static List<MOS.EFMODEL.DataModels.HIS_REPORT_TYPE_CAT> HisReportTypeCats
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_REPORT_TYPE_CAT));
                //if (hisReportTypeCats == null || hisReportTypeCats.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisReportTypeCatFilter filter = new MOS.Filter.HisReportTypeCatFilter();
                    filter.REPORT_TYPE_CODE__EXACT = FormTypeConfig.ReportTypeCode;
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisReportTypeCats = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_REPORT_TYPE_CAT>(RequestUriStore.HIS_REPORT_TYPE_CAT_GET, ApiConsumerStore.MosConsumer, filter);
                }
                return hisReportTypeCats.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisReportTypeCats = value;
            }
        }

        private static Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>> dicSetyService = new Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>();
        internal static Dictionary<long, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>> DicSetyService
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE));
                Inventec.Common.Logging.LogSystem.Info("dicSetyService" + dicSetyService.Count);
                if (dicSetyService == null || dicSetyService.Count == 0)
                {
                    try
                    {
                        foreach (var item in VHisServices)
                        {
                            if (!dicSetyService.ContainsKey(item.SERVICE_TYPE_ID))
                                dicSetyService[item.SERVICE_TYPE_ID] = new List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
                            dicSetyService[item.SERVICE_TYPE_ID].Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
                return dicSetyService;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE> transactionType;
        public static List<MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE> HisTransactionType
        {
            get
            {
                //if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE));
                if (transactionType == null || transactionType.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTransactionTypeFilter filter = new MOS.Filter.HisTransactionTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    transactionType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE>("/api/HisTransactionType/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return transactionType.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                transactionType = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_MACHINE> machine;
        public static List<MOS.EFMODEL.DataModels.HIS_MACHINE> HisMachines
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_MACHINE));
                if (machine == null || machine.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMachineFilter filter = new MOS.Filter.HisMachineFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    machine = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MACHINE>("/api/HisMachine/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return machine.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                machine = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG> mediOrg;
        public static List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG> HisMediOrgs
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_MEDI_ORG));
                if (mediOrg == null || mediOrg.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMediOrgFilter filter = new MOS.Filter.HisMediOrgFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    mediOrg = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>("/api/HisMediOrg/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return mediOrg.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                mediOrg = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_WORKING_SHIFT> hisWorkingShifts;
        public static List<MOS.EFMODEL.DataModels.HIS_WORKING_SHIFT> HisWorkingShifts
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_WORKING_SHIFT));
                if (hisWorkingShifts == null || hisWorkingShifts.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisWorkingShiftFilter filter = new MOS.Filter.HisWorkingShiftFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    mediOrg = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>("/api/HisWorkingShift/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return hisWorkingShifts.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisWorkingShifts = value;
            }
        }
        private static List<MOS.EFMODEL.DataModels.HIS_CONFIG> hisConfig;
        public static List<MOS.EFMODEL.DataModels.HIS_CONFIG> HisConfig
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_CONFIG));
                if (hisConfig == null || hisConfig.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisConfigFilter filter = new MOS.Filter.HisConfigFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisConfig = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CONFIG>("/api/HisConfig/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return hisConfig.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode")).ToList();
            }
            set
            {
                hisConfig = value;
            }
        }
        private static List<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE> hisMedicineLine;

        public static List<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE> HisMedicineLine
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE));
                if (hisMedicineLine == null || hisMedicineLine.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMedicineLineFilter filter = new MOS.Filter.HisMedicineLineFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    hisMedicineLine = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>("/api/HisMedicineLine/Get", ApiConsumerStore.MosConsumer, filter);
                }
                return hisMedicineLine.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                hisMedicineLine = value;
            }
        }
    }
}
