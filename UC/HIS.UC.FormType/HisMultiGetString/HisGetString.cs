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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    public class HisGetString
    {
        public static List<DataGet> Get(string value, string key)
        {
            List<DataGet> datasuft = null;

            try
            {
                if (value == null) return new List<DataGet>();
                if (value == "HIS_DATA_STORE")
                {
                    datasuft = Config.HisFormTypeConfig.HisDataStores.Select(o => new DataGet { ID = o.ID, CODE = o.DATA_STORE_CODE, NAME = o.DATA_STORE_NAME }).ToList();
                }
                else if (value == "HIS_ALLOUT_MEDI_SERVICE")
                {
                    datasuft = new List<DataGet>();
                    var noneMediServices = Config.HisFormTypeConfig.HisNoneMediServices.Select(o => new DataGet { ID = o.ID, CODE = o.NONE_MEDI_SERVICE_CODE, NAME = o.NONE_MEDI_SERVICE_NAME }).ToList();
                    var services = Config.HisFormTypeConfig.VHisServices.Select(o => new DataGet { ID = o.ID+50000000, CODE = o.SERVICE_CODE, NAME = o.SERVICE_NAME, PARENT = o.SERVICE_TYPE_ID }).ToList();
                    if (noneMediServices != null)
                    {
                        datasuft.AddRange(noneMediServices);
                    }
                    if (services != null)
                    {
                        datasuft.AddRange(services);
                    }
                }
                else if (value == "HIS_NONE_MEDI_SERVICE")
                {
                    datasuft = Config.HisFormTypeConfig.HisNoneMediServices.Select(o => new DataGet { ID = o.ID, CODE = o.NONE_MEDI_SERVICE_CODE, NAME = o.NONE_MEDI_SERVICE_NAME }).ToList();
                }
                else if (value == "HIS_THIS_ROLE_DEPARTMENT")
                {
                    if (HIS.UC.FormType.FormTypeConfig.MyInfo == null)
                    {
                        datasuft = new List<DataGet>();
                    }
                    else
                    {
                        datasuft = Config.HisFormTypeConfig.HisDepartments.Where(p => p.ID == (HIS.UC.FormType.FormTypeConfig.MyInfo.DEPARTMENT_ID ?? 0) || HIS.UC.FormType.FormTypeConfig.MyInfo.IS_ADMIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.DEPARTMENT_CODE, NAME = o.DEPARTMENT_NAME, PARENT = o.BRANCH_ID }).ToList();
                    }
                }
                else if (value == "HIS_THIS_ROLE_ROOM")
                {
                    string currentLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (!string.IsNullOrWhiteSpace(currentLoginname))
                    {
                        var myRoom = Config.HisFormTypeConfig.VHisUserRooms.Where(o => o.LOGINNAME == currentLoginname).Select(o => o.ROOM_ID).ToList();
                        datasuft = Config.HisFormTypeConfig.VHisRooms.Where(o=> myRoom.Contains(o.ID)||HIS.UC.FormType.FormTypeConfig.MyInfo!= null && HIS.UC.FormType.FormTypeConfig.MyInfo.IS_ADMIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.ROOM_CODE, NAME = o.ROOM_NAME, PARENT = o.DEPARTMENT_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();
                    }
                    else
                    {
                        datasuft = new List<DataGet>();
                    }
                }
                else if (value == "HIS_THIS_ROLE_MEDI_STOCK")
                {
                    string currentLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (!string.IsNullOrWhiteSpace(currentLoginname))
                    {
                        var myRoom = Config.HisFormTypeConfig.VHisUserRooms.Where(o => o.LOGINNAME == currentLoginname).Select(p=>p.ROOM_ID).ToList();
                        datasuft = Config.HisFormTypeConfig.VHisMediStock.Where(o => myRoom.Contains(o.ROOM_ID)||HIS.UC.FormType.FormTypeConfig.MyInfo!= null && HIS.UC.FormType.FormTypeConfig.MyInfo.IS_ADMIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME }).ToList();
                    }
                    else
                    {
                        datasuft = new List<DataGet>();
                    }
                }
                else if (value == "HIS_OTHER_PAY_SOURCE")
                {
                    datasuft = Config.HisFormTypeConfig.HisOtherPaySources.Select(o => new DataGet { ID = o.ID, CODE = o.OTHER_PAY_SOURCE_CODE, NAME = o.OTHER_PAY_SOURCE_NAME }).ToList();
                }
                else if (value == "HIS_BID_TYPE")
                {
                    datasuft = Config.HisFormTypeConfig.HisBidTypes.Select(o => new DataGet { ID = o.ID, CODE = o.BID_TYPE_CODE, NAME = o.BID_TYPE_NAME }).ToList();
                }
                else if (value == "HIS_PATIENT_CLASSIFY")
                {
                    datasuft = Config.HisFormTypeConfig.HisPatientClassifys.Select(o => new DataGet { ID = o.ID, CODE = o.PATIENT_CLASSIFY_CODE, NAME = o.PATIENT_CLASSIFY_NAME }).ToList();
                }
                else if (value == "HIS_AREA")
                {
                    datasuft = Config.HisFormTypeConfig.HisAreas.Select(o => new DataGet { ID = o.ID, CODE = o.AREA_CODE, NAME = o.AREA_NAME, PARENT = o.PATIENT_TYPE_ID ?? 0, GRAND_PARENT = o.DEPARTMENT_ID ?? 0 }).ToList();
                }
                else if (value == "HIS_DEBATE_TYPE")
                {
                    datasuft = Config.HisFormTypeConfig.HisDebateTypes.Select(o => new DataGet { ID = o.ID, CODE = o.DEBATE_TYPE_CODE, NAME = o.DEBATE_TYPE_NAME }).ToList();
                }
                else if (value == "HIS_PATY_AREA")
                {
                    datasuft = Config.HisFormTypeConfig.HisAreas.Select(o => new DataGet { ID = o.ID, CODE = o.AREA_CODE, NAME = o.AREA_NAME, PARENT = o.PATIENT_TYPE_ID ?? 0 }).ToList();
                }
                else if (value == "HIS_DEPA_AREA")
                {
                    datasuft = Config.HisFormTypeConfig.HisAreas.Select(o => new DataGet { ID = o.ID, CODE = o.AREA_CODE, NAME = o.AREA_NAME, PARENT = o.DEPARTMENT_ID ?? 0 }).ToList();
                }
                else if (value == "HIS_MEDICINE_USE_FORM")
                {
                    datasuft = Config.HisFormTypeConfig.HisMedicineUseForms.Select(o => new DataGet { ID = o.ID, CODE = o.MEDICINE_USE_FORM_CODE, NAME = o.MEDICINE_USE_FORM_NAME }).ToList();
                }
                else if (value == "HIS_MEDICINE_GROUP")
                {
                    datasuft = Config.HisFormTypeConfig.HisMedicineGroups.Select(o => new DataGet { ID = o.ID, CODE = o.MEDICINE_GROUP_CODE, NAME = o.MEDICINE_GROUP_NAME }).ToList();
                }
                else if (value == "HIS_SERVICE_UNIT")
                {
                    datasuft = Config.HisFormTypeConfig.HisServiceUnit.Select(o => new DataGet { ID = o.ID, CODE = o.SERVICE_UNIT_CODE, NAME = o.SERVICE_UNIT_NAME }).ToList();
                }
                else if (value == "HIS_WORKING_SHIFT")
                {
                    datasuft = Config.HisFormTypeConfig.HisWorkingShifts.Select(o => new DataGet { ID = o.ID, CODE = o.WORKING_SHIFT_CODE, NAME = o.WORKING_SHIFT_NAME }).ToList();
                }
                else if (value == "HIS_MEDI_SUPPLIER_STOCK")
                {
                    datasuft = Config.HisFormTypeConfig.HisMediStocks.Where(p => p.IS_ALLOW_IMP_SUPPLIER == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME }).ToList();
                }
                else if (value == "HIS_EXT_CASHIER_ROOM")
                {
                    datasuft = Config.HisFormTypeConfig.HisCashierRooms.Select(o => new DataGet { ID = o.ID, CODE = o.CASHIER_ROOM_CODE, NAME = o.CASHIER_ROOM_NAME, PARENT = o.DEPARTMENT_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();
                    datasuft.Add(new DataGet { ID = -1, CODE = "\t", NAME = " Tại quầy - trong giờ" });
                    datasuft.Add(new DataGet { ID = -2, CODE = "\t", NAME = " Tại quầy - ngoài giờ" });
                }
                else if (value == "HIS_ACCIDENT_HURT_TYPE") datasuft = Config.HisFormTypeConfig.HisAccidentHurtTypes.Select(o => new DataGet { ID = o.ID, CODE = o.ACCIDENT_HURT_TYPE_CODE, NAME = o.ACCIDENT_HURT_TYPE_NAME }).ToList();
                else if (value == "HIS_ACCIDENT_RESULT") datasuft = Config.HisFormTypeConfig.HisAccidentResults.Select(o => new DataGet { ID = o.ID, CODE = o.ACCIDENT_RESULT_CODE, NAME = o.ACCIDENT_RESULT_NAME }).ToList();
                else if (value == "HIS_EMPLOYEE") datasuft = Config.HisFormTypeConfig.HisDEmployees.Select(o => new DataGet { ID = o.ID, CODE = o.DIPLOMA, NAME = o.DIPLOMA }).ToList();
                else if (value == "HIS_CURRENTBRANCH_MEDI_STOCK" && Config.HisFormTypeConfig.VHisMediStock != null && Config.HisFormTypeConfig.HisDepartments != null)
                {
                    datasuft = Config.HisFormTypeConfig.VHisMediStock.Where(o => Config.HisFormTypeConfig.HisDepartments.Exists(p => p.ID == o.DEPARTMENT_ID && p.BRANCH_ID == HIS.UC.FormType.FormTypeConfig.BranchId)).Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME }).ToList();
                }
                else if (value == "HIS_CURRENTBRANCH_DEPARTMENT" && Config.HisFormTypeConfig.HisDepartments != null)
                {
                    datasuft = Config.HisFormTypeConfig.HisDepartments.Where(o => o.BRANCH_ID == HIS.UC.FormType.FormTypeConfig.BranchId).Select(o => new DataGet { ID = o.ID, CODE = o.DEPARTMENT_CODE, NAME = o.DEPARTMENT_NAME }).ToList();
                }
                else if (value == "HIS_CURRENTBRANCH_ROOM" && Config.HisFormTypeConfig.VHisRooms != null)
                {
                    datasuft = Config.HisFormTypeConfig.VHisRooms.Where(o => o.BRANCH_ID == HIS.UC.FormType.FormTypeConfig.BranchId).Select(o => new DataGet { ID = o.ID, CODE = o.ROOM_CODE, NAME = o.ROOM_NAME }).ToList();
                }
                else if (value == "HIS_PAY_FORM") datasuft = Config.HisFormTypeConfig.HisPayForms.Select(o => new DataGet { ID = o.ID, CODE = o.PAY_FORM_CODE, NAME = o.PAY_FORM_NAME }).ToList();
                else if (value == "HIS_BLOOD_TYPE") datasuft = Config.HisFormTypeConfig.HisBloodType.Select(o => new DataGet { ID = o.ID, CODE = o.BLOOD_TYPE_CODE, NAME = o.BLOOD_TYPE_NAME }).ToList();
                else if (value == "HIS_FUND") datasuft = Config.HisFormTypeConfig.HisFund.Select(o => new DataGet { ID = o.ID, CODE = o.FUND_CODE, NAME = o.FUND_NAME }).ToList();
                else if (value == "HIS_MATERIAL") datasuft = HisMaterials(key).Select(o => new DataGet { ID = o.ID, NAME = o.PACKAGE_NUMBER + " - Thực nhập:" + o.IMP_TIME, PARENT = o.MATERIAL_TYPE_ID }).ToList();
                else if (value == "HIS_MEDICINE") datasuft = HisMedicines(key).Select(o => new DataGet { ID = o.ID, NAME = o.PACKAGE_NUMBER + " - Thực nhập:" + o.IMP_TIME, PARENT = o.MEDICINE_TYPE_ID }).ToList();
                else if (value == "HIS_CASHIER_LOGINNAME") datasuft = Config.AcsFormTypeConfig.HisAcsUserCashier.Select(o => new DataGet { ID = o.ID, CODE = o.LOGINNAME, NAME = o.USERNAME }).ToList();

                else if (value == "HIS_REQUEST_ROOM") datasuft = Config.HisFormTypeConfig.VHisRooms.Where(o => (new List<long>() { IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG, IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TD, IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL }).Contains(o.ROOM_TYPE_ID)).Select(o => new DataGet { ID = o.ID, CODE = o.ROOM_CODE, NAME = o.ROOM_NAME, PARENT = o.DEPARTMENT_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();
                else if (value == "HIS_EXAM_ROOM") datasuft = Config.HisFormTypeConfig.VHisExecuteRooms.Where(o => o.IS_EXAM == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ROOM_ID, CODE = o.EXECUTE_ROOM_CODE, NAME = o.EXECUTE_ROOM_NAME, PARENT = o.DEPARTMENT_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();
                else if (value == "HIS_CLINICAL_ROOM") datasuft = Config.HisFormTypeConfig.VHisRooms.Where(o => o.IS_EXAM != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG).Select(o => new DataGet { ID = o.ID, CODE = o.ROOM_CODE, NAME = o.ROOM_NAME, PARENT = o.DEPARTMENT_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();
                else if (value == "HIS_SURG_ROOM") datasuft = Config.HisFormTypeConfig.VHisExecuteRooms.Where(o => o.IS_SURGERY == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ROOM_ID, CODE = o.EXECUTE_ROOM_CODE, NAME = o.EXECUTE_ROOM_NAME, PARENT = o.DEPARTMENT_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();
                else if (value == "HIS_MY_SURG_ROOM")
                {
                    string currentLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (!string.IsNullOrWhiteSpace(currentLoginname))
                    {
                        var myRoomId = Config.HisFormTypeConfig.VHisUserRooms.Where(o => o.LOGINNAME == currentLoginname).Select(p => p.ROOM_ID).Distinct().ToList();
                        datasuft = Config.HisFormTypeConfig.VHisExecuteRooms.Where(o => o.IS_SURGERY == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && myRoomId.Contains(o.ROOM_ID)).Select(o => new DataGet { ID = o.ROOM_ID, CODE = o.EXECUTE_ROOM_CODE, NAME = o.EXECUTE_ROOM_NAME, PARENT = o.DEPARTMENT_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();
                    }
                    else
                    {
                        datasuft = new List<DataGet>();
                    }
                }

                else if (value == "HIS_MEDI_BIG_STOCK") datasuft = Config.HisFormTypeConfig.VHisMediStock.Where(o => !Config.HisFormTypeConfig.HisDepartments.Where(p => p.IS_CLINICAL == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(q => q.ID).ToList().Contains(o.DEPARTMENT_ID)).Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME }).ToList();

                else if (value == "HIS_MEDI_STOCK_NOT_CABIN") datasuft = Config.HisFormTypeConfig.VHisMediStock.Where(o => o.IS_CABINET != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME }).ToList();


                else if (value == "HIS_INVOICE") datasuft = Config.HisFormTypeConfig.HisInvoices.Select(o => new DataGet { ID = o.ID, CODE = o.VIR_NUM_ORDER, NAME = o.SELLER_NAME }).ToList();
                else if (value == "HIS_INVOICE_BOOK") datasuft = Config.HisFormTypeConfig.HisInvoiceBooks.Select(o => new DataGet { ID = o.ID, CODE = o.TEMPLATE_CODE, NAME = o.SYMBOL_CODE }).ToList();
                else if (value == "HIS_SUPPLIER") datasuft = Config.HisFormTypeConfig.HisSuppliers.Select(o => new DataGet { ID = o.ID, CODE = o.SUPPLIER_CODE, NAME = o.SUPPLIER_NAME }).ToList();
                else if (value == "HIS_IMP_SOURCE") datasuft = Config.HisFormTypeConfig.HisImpSources.Select(o => new DataGet { ID = o.ID, CODE = o.IMP_SOURCE_CODE, NAME = o.IMP_SOURCE_NAME }).ToList();
                else if (value == "HIS_STORE_MEDI_STOCK")
                {
                    string currentLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (!string.IsNullOrWhiteSpace(currentLoginname))
                    {
                        var myRoom = Config.HisFormTypeConfig.VHisUserRooms.Where(o => o.LOGINNAME == currentLoginname).ToList();
                        datasuft = Config.HisFormTypeConfig.VHisMediStock.Where(o => o.IS_DRUG_STORE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME, IS_OUTPUT0 = myRoom.Exists(p => p.ROOM_CODE == o.MEDI_STOCK_CODE) }).ToList();
                    }
                    else
                    {
                        datasuft = new List<DataGet>();
                    }
                }
                else if (value == "HIS_MEDI_STOCK_BUSINESS") datasuft = Config.HisFormTypeConfig.VHisMediStock.Where(o => o.IS_BUSINESS == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME }).ToList();
                else if (value == "HIS_MEDI_STOCK_NOT_BUSINESS") datasuft = Config.HisFormTypeConfig.VHisMediStock.Where(o => o.IS_BUSINESS != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME }).ToList();
                else if (value == "HIS_MEDI_STOCK_CABINET") datasuft = Config.HisFormTypeConfig.VHisMediStock.Where(o => o.IS_CABINET == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME }).ToList();
                else if (value == "HIS_MEST_ROOM") datasuft = Config.HisFormTypeConfig.VHisMestRooms.Select(o => new DataGet { ID = o.ROOM_ID, CODE = o.ROOM_CODE, NAME = o.ROOM_NAME, GRAND_PARENT = o.MEDI_STOCK_ID }).ToList();
                else if (value == "HIS_MEST_ROOM_MEDI_STOCK")
                {
                    var mestRoom = Config.HisFormTypeConfig.VHisMestRooms.Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__KHO && Config.HisFormTypeConfig.VHisRooms.Select(s => s.ID).Contains(o.ROOM_ID)).ToList();
                    //mestRoom = mestRoom.Where(o => Config.HisFormTypeConfig.VHisMediStock.Where(w => w.IS_CABINET != 1).Select(s => s.ROOM_ID).Contains(o.ROOM_ID)).ToList();
                    datasuft = new List<DataGet>();
                    foreach (var item in mestRoom)
                    {
                        DataGet dt = new DataGet();
                        dt.ID = item.MEDI_STOCK_ID;
                        dt.CODE = item.MEDI_STOCK_CODE;
                        dt.NAME = item.MEDI_STOCK_NAME;
                        var stock = Config.HisFormTypeConfig.VHisMediStock.FirstOrDefault(o => o.ROOM_ID == item.ROOM_ID);
                        if (stock != null) dt.GRAND_PARENT = stock.ID;

                        datasuft.Add(dt);
                    }
                }
                else if (value == "HIS_MEDICINE_TYPE") datasuft = Config.HisFormTypeConfig.VHisMedicineTypes.Select(o => new DataGet { ID = o.ID, CODE = o.MEDICINE_TYPE_CODE, NAME = o.MEDICINE_TYPE_NAME }).ToList();
                else if (value == "HIS_CONCENTRA_MEDICINE_TYPE") datasuft = Config.HisFormTypeConfig.VHisMedicineTypes.Select(o => new DataGet { ID = o.ID, CODE = o.MEDICINE_TYPE_CODE, NAME = string.Format("{0} (Nồng độ hàm lượng: {1})", o.MEDICINE_TYPE_NAME,o.CONCENTRA) }).ToList();

                else if (value == "HIS_BED_ROOM") datasuft = Config.HisFormTypeConfig.VHisBedRooms.Select(o => new DataGet { ID = o.ID, CODE = o.BED_ROOM_CODE, NAME = o.BED_ROOM_NAME, PARENT = o.DEPARTMENT_ID }).ToList();// IDs = Config.HisFormTypeConfig.VHisBedRooms.Select(o => o.ID).ToList(); }
                else if (value == "HIS_BED") datasuft = Config.HisFormTypeConfig.VHisBeds.Select(o => new DataGet { ID = o.ID, CODE = o.BED_NAME, NAME = o.BED_NAME, PARENT = o.BED_ROOM_ID }).ToList();// IDs = Config.HisFormTypeConfig.VHisBeds.Select(o => o.ID).ToList(); }
                else if (value == "HIS_EXP_MEST_TEMPLATE") datasuft = Config.HisFormTypeConfig.VHisExpMestTemplates.Select(o => new DataGet { ID = o.ID, CODE = o.EXP_MEST_TEMPLATE_CODE, NAME = o.EXP_MEST_TEMPLATE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisExpMestTemplates.Select(o => o.ID).ToList(); }

                else if (value == "HIS_EXECUTE_ROOM") datasuft = Config.HisFormTypeConfig.VHisExecuteRooms.Select(o => new DataGet { ID = o.ID, CODE = o.EXECUTE_ROOM_CODE, NAME = o.EXECUTE_ROOM_NAME, PARENT = o.DEPARTMENT_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();// IDs = Config.HisFormTypeConfig.VHisExecuteRooms.Select(o => o.ID).ToList(); }
                else if (value == "HIS_DEPARTMENT") datasuft = Config.HisFormTypeConfig.HisDepartments.Select(o => new DataGet { ID = o.ID, CODE = o.DEPARTMENT_CODE, NAME = o.DEPARTMENT_NAME, PARENT = o.BRANCH_ID }).ToList();
                else if (value == "HIS_DEACTIVE_DEPARTMENT") datasuft = Config.HisFormTypeConfig.HisDeactiveDepartments.Select(o => new DataGet { ID = o.ID, CODE = o.DEPARTMENT_CODE, NAME = o.DEPARTMENT_NAME, PARENT = o.BRANCH_ID }).ToList();
                else if (value == "HIS_CLINICAL_DEPARTMENT") datasuft = Config.HisFormTypeConfig.HisDepartments.Where(p => p.IS_CLINICAL == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.DEPARTMENT_CODE, NAME = o.DEPARTMENT_NAME, PARENT = o.BRANCH_ID }).ToList();
                else if (value == "HIS_MY_DEPARTMENT")
                {
                    if (HIS.UC.FormType.FormTypeConfig.MyInfo == null)
                    {
                        datasuft = new List<DataGet>();
                    }
                    else
                    {
                        datasuft = Config.HisFormTypeConfig.HisDepartments.Where(p => p.ID == (HIS.UC.FormType.FormTypeConfig.MyInfo.DEPARTMENT_ID ?? 0) || HIS.UC.FormType.FormTypeConfig.MyInfo.IS_ADMIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => new DataGet { ID = o.ID, CODE = o.DEPARTMENT_CODE, NAME = o.DEPARTMENT_NAME, PARENT = o.BRANCH_ID }).ToList();
                    }
                }
                else if (value == "HIS_KSK_CONTRACT") datasuft = Config.HisFormTypeConfig.HisKskContractsView.Select(o => new DataGet { ID = o.ID, CODE = o.KSK_CONTRACT_CODE, NAME = o.WORK_PLACE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisKskContracts.Select(o => o.ID).ToList(); }
                else if (value == "HIS_BRANCH") datasuft = Config.HisFormTypeConfig.HisBranchs.Select(o => new DataGet { ID = o.ID, CODE = o.BRANCH_CODE, NAME = o.BRANCH_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisBranchs.Select(o => o.ID).ToList(); }

                else if (value == "HIS_CAREER") datasuft = Config.HisFormTypeConfig.HisCareers.Select(o => new DataGet { ID = o.ID, CODE = o.CAREER_CODE, NAME = o.CAREER_NAME }).ToList();
                else if (value == "HIS_DEATH_CAUSE") datasuft = Config.HisFormTypeConfig.HisDeathCauses.Select(o => new DataGet { ID = o.ID, CODE = o.DEATH_CAUSE_CODE, NAME = o.DEATH_CAUSE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisDeathCauses.Select(o => o.ID).ToList(); }
                else if (value == "HIS_PROGRAM") datasuft = Config.HisFormTypeConfig.HisProgram.Select(o => new DataGet { ID = o.ID, CODE = o.PROGRAM_CODE, NAME = o.PROGRAM_NAME }).ToList();
                else if (value == "HIS_DEATH_WITHIN") datasuft = Config.HisFormTypeConfig.HisDeathWithins.Select(o => new DataGet { ID = o.ID, CODE = o.DEATH_WITHIN_CODE, NAME = o.DEATH_WITHIN_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisDeathWithins.Select(o => o.ID).ToList(); }

                else if (value == "HIS_ROOM") datasuft = Config.HisFormTypeConfig.VHisRooms.Select(o => new DataGet { ID = o.ID, CODE = o.ROOM_CODE, NAME = o.ROOM_NAME, PARENT = o.DEPARTMENT_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();

                else if (value == "HIS_BRANCH_ROOM") datasuft = Config.HisFormTypeConfig.VHisRooms.Select(o => new DataGet { ID = o.ID, CODE = o.ROOM_CODE, NAME = o.ROOM_NAME, PARENT = o.BRANCH_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();
                else if (value == "HIS_PATIENT_TYPE") datasuft = Config.HisFormTypeConfig.HisPatientTypes.Select(o => new DataGet { ID = o.ID, CODE = o.PATIENT_TYPE_CODE, NAME = o.PATIENT_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisPatientTypes.Select(o => o.ID).ToList(); }
                else if (value == "HIS_PARENT_SERVICE_RAW_MEDICINAL_HERBS") datasuft = Config.HisFormTypeConfig.HisPatientTypes.Select(o => new DataGet { ID = o.ID, CODE = o.PATIENT_TYPE_CODE, NAME = o.PATIENT_TYPE_NAME }).ToList();
                else if (value == "HIS_PTTT_GROUP") datasuft = Config.HisFormTypeConfig.HisPTTTGroups.Select(o => new DataGet { ID = o.ID, CODE = o.PTTT_GROUP_CODE, NAME = o.PTTT_GROUP_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisPTTTGroups.Select(o => o.ID).ToList(); }
                else if (value == "HIS_IMP_MEST_TYPE") datasuft = Config.HisFormTypeConfig.HisImpMestTypes.Select(o => new DataGet { ID = o.ID, CODE = o.IMP_MEST_TYPE_CODE, NAME = o.IMP_MEST_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisImpMestTypes.Select(o => o.ID).ToList(); }
                else if (value == "HIS_IMP_MEST_STT") datasuft = Config.HisFormTypeConfig.HisImpMestStts.Select(o => new DataGet { ID = o.ID, CODE = o.IMP_MEST_STT_CODE, NAME = o.IMP_MEST_STT_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisImpMestStts.Select(o => o.ID).ToList(); }
                else if (value == "HIS_ROOM_TYPE") datasuft = Config.HisFormTypeConfig.HisRoomTypes.Select(o => new DataGet { ID = o.ID, CODE = o.ROOM_TYPE_CODE, NAME = o.ROOM_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisRoomTypes.Select(o => o.ID).ToList(); }
                else if (value == "HIS_ICD") datasuft = Config.HisFormTypeConfig.HisIcds.Select(o => new DataGet { ID = o.ID, CODE = o.ICD_CODE, NAME = o.ICD_NAME, PARENT = o.ICD_GROUP_ID ?? 0 }).ToList();// IDs = Config.HisFormTypeConfig.HisIcds.Select(o => o.ID).ToList(); }
                else if (value == "HIS_EXECUTE_GROUP") datasuft = Config.HisFormTypeConfig.HisExecuteGroups.Select(o => new DataGet { ID = o.ID, CODE = o.EXECUTE_GROUP_CODE, NAME = o.EXECUTE_GROUP_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisExecuteGroups.Select(o => o.ID).ToList(); }

                else if (value == "HIS_TRAN_PATI_REASON") datasuft = Config.HisFormTypeConfig.HisTranPatiReasons.Select(o => new DataGet { ID = o.ID, CODE = o.TRAN_PATI_REASON_CODE, NAME = o.TRAN_PATI_REASON_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisTranPatiReasons.Select(o => o.ID).ToList(); }
                else if (value == "HIS_SERVICE_REQ_STT") datasuft = Config.HisFormTypeConfig.HisServiceReqStts.Select(o => new DataGet { ID = o.ID, CODE = o.SERVICE_REQ_STT_CODE, NAME = o.SERVICE_REQ_STT_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisServiceReqStts.Select(o => o.ID).ToList(); }
                else if (value == "HIS_SERVICE_REQ_TYPE") datasuft = Config.HisFormTypeConfig.HisServiceReqTypes.Select(o => new DataGet { ID = o.ID, CODE = o.SERVICE_REQ_TYPE_CODE, NAME = o.SERVICE_REQ_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisServiceReqTypes.Select(o => o.ID).ToList(); }
                else if (value == "HIS_GENDER") datasuft = Config.HisFormTypeConfig.HisGenders.Select(o => new DataGet { ID = o.ID, CODE = o.GENDER_CODE, NAME = o.GENDER_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisGenders.Select(o => o.ID).ToList(); }

                else if (value == "HIS_MEDI_REACT_TYPE") datasuft = Config.HisFormTypeConfig.HisMediReactTypes.Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_REACT_TYPE_CODE, NAME = o.MEDI_REACT_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisMediReactTypes.Select(o => o.ID).ToList(); }
                else if (value == "HIS_EXP_MEST_STT") datasuft = Config.HisFormTypeConfig.HisExpMestStts.Select(o => new DataGet { ID = o.ID, CODE = o.EXP_MEST_STT_CODE, NAME = o.EXP_MEST_STT_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisExpMestStts.Select(o => o.ID).ToList(); }
                else if (value == "HIS_TREATMENT_END_TYPE") datasuft = Config.HisFormTypeConfig.HisTreatmentEndTypes.Select(o => new DataGet { ID = o.ID, CODE = o.TREATMENT_END_TYPE_CODE, NAME = o.TREATMENT_END_TYPE_NAME }).ToList();

                else if (value == "HIS_TREATMENT_RESULT") datasuft = Config.HisFormTypeConfig.HisTreatmentResults.Select(o => new DataGet { ID = o.ID, CODE = o.TREATMENT_RESULT_CODE, NAME = o.TREATMENT_RESULT_NAME }).ToList();

                else if (value == "HIS_SERVICE") datasuft = Config.HisFormTypeConfig.VHisServices.Select(o => new DataGet { ID = o.ID, CODE = o.SERVICE_CODE, NAME = o.SERVICE_NAME, PARENT = o.SERVICE_TYPE_ID }).ToList();

                else if (value == "HIS_DEACTIVE_SERVICE") datasuft = Config.HisFormTypeConfig.VHisServiceDeactives.Select(o => new DataGet { ID = o.ID, CODE = o.CODE, NAME = o.NAME, PARENT = o.PARENT }).ToList();

                else if (value == "HIS_SERVICE_FULL") datasuft = Config.HisFormTypeConfig.VHisServiceFulls;
                else if (value == "HIS_PARENT_SERVICE")
                {
                    List<long> parentIds = Config.HisFormTypeConfig.VHisServices.Where(o => o.PARENT_ID != null).Select(o => o.PARENT_ID ?? 0).Distinct().ToList();
                    datasuft = Config.HisFormTypeConfig.VHisServices.Where(p => parentIds.Contains(p.ID)).Select(o => new DataGet { ID = o.ID, CODE = o.SERVICE_CODE, NAME = o.SERVICE_NAME, PARENT = o.SERVICE_TYPE_ID }).ToList();
                }
                else if (value == "HIS_PATIENT_RAW_MEDICINAL_HERBS_TYPE")// 24/02/2025HIS_PARENT_SERVICE_RAW_MEDICINAL_HERBS
                {
                    List<long> parentIds = Config.HisFormTypeConfig.VHisServices.Where(o => o.PARENT_ID != null).Select(o => o.PARENT_ID ?? 0).Distinct().ToList();
                    datasuft = Config.HisFormTypeConfig.VHisServices.Where(p => parentIds.Contains(p.ID)).Select(o => new DataGet { ID = o.ID, CODE = o.SERVICE_CODE, NAME = o.SERVICE_NAME, PARENT = o.SERVICE_TYPE_ID }).ToList();
                }
                else if (value == "HIS_PARENT_MEDICINE_TYPE") /// 22/04/2025 TKB00139 thận hà nội
                {
                    List<long> parentIds = Config.HisFormTypeConfig.VHisMedicineTypes.Where(o => o.PARENT_ID != null).Select(o => o.PARENT_ID ?? 0).Distinct().ToList();
                    datasuft = Config.HisFormTypeConfig.VHisMedicineTypes.Where(p => parentIds.Contains(p.ID)).Select(o => new DataGet { ID = o.ID, CODE = o.MEDICINE_TYPE_CODE, NAME = o.MEDICINE_TYPE_NAME, PARENT = o.ID }).ToList();
                }
                else if (value == "HIS_CHILD_SERVICE") datasuft = Config.HisFormTypeConfig.VHisServices.Where(p => p.PARENT_ID.HasValue).Select(o => new DataGet { ID = o.ID, CODE = o.SERVICE_CODE, NAME = o.SERVICE_NAME, PARENT = o.PARENT_ID.Value }).ToList();
                else if (value == "HIS_SERVICE_ROOM") datasuft = Config.HisFormTypeConfig.VHisServiceRooms.Select(o => new DataGet { ID = o.ID, CODE = o.ROOM_CODE, NAME = o.ROOM_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisServiceRooms.Select(o => o.ID).ToList(); }
                else if (value == "HIS_SERVICE_GROUP") datasuft = Config.HisFormTypeConfig.VHisServiceGroups.Select(o => new DataGet { ID = o.ID, CODE = o.SERVICE_GROUP_CODE, NAME = o.SERVICE_GROUP_NAME }).ToList();

                else if (value == "HIS_SERVICE_TYPE") datasuft = Config.HisFormTypeConfig.HisServiceTypes.Select(o => new DataGet { ID = o.ID, CODE = o.SERVICE_TYPE_CODE, NAME = o.SERVICE_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisServiceTypes.Select(o => o.ID).ToList(); }
                else if (value == "HIS_MEDI_STOCK") datasuft = Config.HisFormTypeConfig.VHisMediStock.Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME, PARENT = o.DEPARTMENT_ID }).ToList();
                else if (value == "OTHER_HIS_MEDI_STOCK") datasuft = Config.HisFormTypeConfig.HisMediStocks.Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_CODE, NAME = o.MEDI_STOCK_NAME }).ToList();
                else if (value == "HIS_MEDI_STOCK_PERIOD") datasuft = HisMediStockPeriod(key).Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_STOCK_PERIOD_NAME, NAME = o.MEDI_STOCK_PERIOD_NAME, PARENT = o.MEDI_STOCK_ID }).ToList();
                else if (value == "HIS_EXP_MEST_TYPE") datasuft = Config.HisFormTypeConfig.HisExpMestTypes.Select(o => new DataGet { ID = o.ID, CODE = o.EXP_MEST_TYPE_CODE, NAME = o.EXP_MEST_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisExpMestTypes.Select(o => o.ID).ToList(); }
                else if (value == "HIS_EXP_MEST_REASON") datasuft = Config.HisFormTypeConfig.HisExpMestReasons.Select(o => new DataGet { ID = o.ID, CODE = o.EXP_MEST_REASON_CODE, NAME = o.EXP_MEST_REASON_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisExpMestTypes.Select(o => o.ID).ToList(); }
                else if (value == "HIS_CASHIER_ROOM") datasuft = Config.HisFormTypeConfig.HisCashierRooms.Select(o => new DataGet { ID = o.ID, CODE = o.CASHIER_ROOM_CODE, NAME = o.CASHIER_ROOM_NAME, PARENT = o.DEPARTMENT_ID, GRAND_PARENT = o.BRANCH_ID }).ToList();
                else if (value == "HIS_MEDICINE_TYPE_ACIN") datasuft = Config.HisFormTypeConfig.VHisMedicineTypeAcins.Select(o => new DataGet { ID = o.ID, CODE = o.ACTIVE_INGREDIENT_CODE, NAME = o.ACTIVE_INGREDIENT_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisMedicineTypeAcins.Select(o => o.ID).ToList(); }
                else if (value == "HIS_TEST_INDEX_RANGE") datasuft = Config.HisFormTypeConfig.VHisTestIndexRanges.Select(o => new DataGet { ID = o.ID, CODE = o.TEST_INDEX_CODE, NAME = o.TEST_INDEX_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisTestIndexRanges.Select(o => o.ID).ToList(); }
                else if (value == "HIS_TREATMENT_TYPE") datasuft = Config.HisFormTypeConfig.HisTreatmentTypes.Select(o => new DataGet { ID = o.ID, CODE = o.TREATMENT_TYPE_CODE, NAME = o.TREATMENT_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisTreatmentTypes.Select(o => o.ID).ToList(); }

                else if (value == "HIS_MEDICINE_BEAN") datasuft = Config.HisFormTypeConfig.VHisMedicineBeans.Select(o => new DataGet { ID = o.ID, CODE = o.MEDICINE_TYPE_CODE, NAME = o.MEDICINE_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisMedicineBeans.Select(o => o.ID).ToList(); }
                else if (value == "HIS_EXP_MEST_MEDICINE") datasuft = Config.HisFormTypeConfig.VHisExpMestMedicines.Select(o => new DataGet { ID = o.ID, CODE = o.EXP_MEST_CODE, NAME = o.EXP_MEST_CODE }).ToList();// IDs = Config.HisFormTypeConfig.VHisExpMestMedicines.Select(o => o.ID).ToList(); }
                else if (value == "HIS_MATERIAL_BEAN") datasuft = Config.HisFormTypeConfig.VHisMaterialBeans.Select(o => new DataGet { ID = o.ID, CODE = o.MATERIAL_TYPE_CODE, NAME = o.MATERIAL_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisMaterialBeans.Select(o => o.ID).ToList(); }
                else if (value == "HIS_MATERIAL_TYPE") datasuft = Config.HisFormTypeConfig.VHisMaterialTypes.Select(o => new DataGet { ID = o.ID, CODE = o.MATERIAL_TYPE_CODE, NAME = o.MATERIAL_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisMaterialTypes.Select(o => o.ID).ToList(); }
                else if (value == "HIS_EXP_MEST_MATERIAL") datasuft = Config.HisFormTypeConfig.VHisExpMestMaterials.Select(o => new DataGet { ID = o.ID, CODE = o.MATERIAL_TYPE_CODE, NAME = o.MATERIAL_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisExpMestMaterials.Select(o => o.ID).ToList(); }
                else if (value == "HIS_MILITARY_RANK") datasuft = Config.HisFormTypeConfig.HisMilitaryRanks.Select(o => new DataGet { ID = o.ID, CODE = o.MILITARY_RANK_CODE, NAME = o.MILITARY_RANK_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisMilitaryRanks.Select(o => o.ID).ToList(); }
                else if (value == "HIS_WORK_PLACE") datasuft = Config.HisFormTypeConfig.HisWorkPlaces.Select(o => new DataGet { ID = o.ID, CODE = o.WORK_PLACE_CODE, NAME = o.WORK_PLACE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisWorlPlaces.Select(o => o.ID).ToList(); }
                else if (value == "HIS_ACCOUNT_BOOK") datasuft = Config.HisFormTypeConfig.HisAccountBooks.Select(o => new DataGet { ID = o.ID, CODE = o.ACCOUNT_BOOK_CODE, NAME = o.ACCOUNT_BOOK_NAME }).ToList();// IDs = Config.HisFormTypeConfig.HisAccountBooks.Select(o => o.ID).ToList(); }

                else if (value == "HIS_ICD_GROUP") datasuft = Config.HisFormTypeConfig.VHisIcdGroups.Select(o => new DataGet { ID = o.ID, CODE = o.ICD_GROUP_CODE, NAME = o.ICD_GROUP_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisIcdGroups.Select(o => o.ID).ToList(); }

                else if (value == "HIS_PTTT_METHOD") datasuft = Config.HisFormTypeConfig.MethodPttt.Select(o => new DataGet { ID = o.ID, CODE = o.PTTT_METHOD_CODE, NAME = o.PTTT_METHOD_NAME }).ToList();
                else if (value == "HIS_PTTT_GROUP") datasuft = Config.HisFormTypeConfig.HisPtttGroup.Select(o => new DataGet { ID = o.ID, CODE = o.PTTT_GROUP_CODE, NAME = o.PTTT_GROUP_NAME }).ToList();
                else if (value == "HIS_EMOTIONLESS_METHOD") datasuft = Config.HisFormTypeConfig.HisEmotionlessMethod.Select(o => new DataGet { ID = o.ID, CODE = o.EMOTIONLESS_METHOD_CODE, NAME = o.EMOTIONLESS_METHOD_NAME }).ToList();
                else if (value == "HIS_BLOOD") datasuft = Config.HisFormTypeConfig.HisBlood.Select(o => new DataGet { ID = o.ID, CODE = o.BLOOD_CODE, NAME = o.BLOOD_CODE }).ToList();
                else if (value == "HIS_BLOOD_RH") datasuft = Config.HisFormTypeConfig.HisBloodRh.Select(o => new DataGet { ID = o.ID, CODE = o.BLOOD_RH_CODE, NAME = o.BLOOD_RH_CODE }).ToList();
                else if (value == "HIS_PTTT_CONDITION") datasuft = Config.HisFormTypeConfig.HisPtttCondition.Select(o => new DataGet { ID = o.ID, CODE = o.PTTT_CONDITION_CODE, NAME = o.PTTT_CONDITION_NAME }).ToList();
                else if (value == "HIS_PTTT_CATASTROPHE") datasuft = Config.HisFormTypeConfig.HIsPtttCatastrophe.Select(o => new DataGet { ID = o.ID, CODE = o.PTTT_CATASTROPHE_CODE, NAME = o.PTTT_CATASTROPHE_NAME }).ToList();
                //if (value == "HIS_PRICE_POLICY") datasuft = Config.HisFormTypeConfig.HisPricePolicy.Select(o => new DataGet { ID = o.ID, CODE = o.PRICE_POLICY_CODE, NAME = o.PRICE_POLICY_NAME }).ToList();
                else if (value == "HIS_EXECUTE_ROLE") datasuft = Config.HisFormTypeConfig.HisExecuteRole.Select(o => new DataGet { ID = o.ID, CODE = o.EXECUTE_ROLE_CODE, NAME = o.EXECUTE_ROLE_NAME }).ToList();
                else if (value == "HIS_BID") datasuft = Config.HisFormTypeConfig.VHisBids.Select(o => new DataGet { ID = o.ID, CODE = o.BID_NUMBER, NAME = o.BID_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisBids.Select(o => o.ID).ToList(); }
                else if (value == "HIS_SERVICE_REQ_TYPE") datasuft = Config.HisFormTypeConfig.VHisServiceReqs.Select(o => new DataGet { ID = o.ID, CODE = o.SERVICE_REQ_TYPE_CODE, NAME = o.SERVICE_REQ_TYPE_NAME }).ToList();// IDs = Config.HisFormTypeConfig.VHisServiceReqs.Select(o => o.ID).ToList(); }
                else if (value == "HIS_REPORT_TYPE_CAT") datasuft = Config.HisFormTypeConfig.HisReportTypeCats.Select(o => new DataGet { ID = o.ID, CODE = o.CATEGORY_CODE, NAME = o.CATEGORY_NAME }).ToList();
                else if (value == "HIS_TRANSACTION_TYPE") datasuft = Config.HisFormTypeConfig.HisTransactionType.Select(o => new DataGet { ID = o.ID, CODE = o.TRANSACTION_TYPE_CODE, NAME = o.TRANSACTION_TYPE_NAME }).ToList();
                else if (value == "HIS_MACHINE") datasuft = Config.HisFormTypeConfig.HisMachines.Select(o => new DataGet { ID = o.ID, CODE = o.MACHINE_CODE, NAME = o.MACHINE_NAME }).ToList();
                else if (value == "HIS_MEDI_ORG") datasuft = Config.HisFormTypeConfig.HisMediOrgs.Select(o => new DataGet { ID = o.ID, CODE = o.MEDI_ORG_CODE, NAME = o.MEDI_ORG_NAME }).ToList();
                else if (value == "HIS_CONFIG")
                {
                    datasuft = Config.HisFormTypeConfig.HisConfig.Select(o => new DataGet { ID = o.ID, CODE = o.KEY.Replace("HIS.Desktop.Plugins.PaymentQrCode.", "").Replace("Info", ""), NAME = "Ngân hàng " + o.KEY.Replace("HIS.Desktop.Plugins.PaymentQrCode.", "").Replace("Info", "") }).ToList();
                }
                else if (value == "HIS_MEDICINE_LINE")
                {
                    datasuft = Config.HisFormTypeConfig.HisMedicineLine.Select(o => new DataGet { ID = o.ID, CODE = o.MEDICINE_LINE_CODE, NAME = o.MEDICINE_LINE_NAME }).ToList();
                }
                datasuft = datasuft.OrderBy(o => o.NAME).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return datasuft;
        }

        private static List<V_HIS_MATERIAL_1> HisMaterials(string key)
        {
            List<V_HIS_MATERIAL_1> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMaterialView1Filter filter = new MOS.Filter.HisMaterialView1Filter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.MATERIAL_TYPE_CODE__EXACT = key;
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1>(RequestUriStore.HIS_MATERIAL_GETVIEW1, ApiConsumerStore.MosConsumer, filter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private static List<V_HIS_MEDICINE_1> HisMedicines(string key)
        {
            List<V_HIS_MEDICINE_1> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineView1Filter filter = new MOS.Filter.HisMedicineView1Filter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.MEDICINE_TYPE_CODE__EXACT = key;
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_1>(RequestUriStore.HIS_MEDICINE_GETVIEW1, ApiConsumerStore.MosConsumer, filter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private static List<V_HIS_MEDI_STOCK_PERIOD> HisMediStockPeriod(string key)
        {
            List<V_HIS_MEDI_STOCK_PERIOD> result = new List<V_HIS_MEDI_STOCK_PERIOD>();
            try
            {
                if (String.IsNullOrEmpty(key))
                {
                    result = Config.HisFormTypeConfig.VHisMediStockPeriod;
                }
                else
                {
                    result = Config.HisFormTypeConfig.VHisMediStockPeriod.Where(o => o.MEDI_STOCK_CODE == key).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<V_HIS_MEDI_STOCK_PERIOD>();
            }
            return result;
        }
    }

}
