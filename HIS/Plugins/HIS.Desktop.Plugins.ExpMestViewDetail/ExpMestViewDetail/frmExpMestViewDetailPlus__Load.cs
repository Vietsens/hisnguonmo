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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestViewDetail.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestViewDetail.ExpMestViewDetail
{
    public partial class frmExpMestViewDetail : HIS.Desktop.Utility.FormBase
    {

        /// <summary>
        /// load dữ liệu vào các control chung
        /// </summary>
        private void LoadDataToControlCommon()
        {
            try
            {
                CommonParam param = new CommonParam();
                SetDataToCommonControlDetail(this._CurrentExpMest);
                if (this._CurrentExpMest != null && this._CurrentExpMest.SERVICE_REQ_ID != null)
                {
                    //Review
                    MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.ID = this._CurrentExpMest.SERVICE_REQ_ID;
                    var serviceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUri.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param);

                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        //_serviceReq = serviceReqs.FirstOrDefault();
                        LoadDataToPrescriptionCommonInfo(serviceReqs.FirstOrDefault());
                    }
                }
                else
                {
                    SetDataToExpmestControl(this._CurrentExpMest);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// load dữ liệu vào thông tin chung nếu phiếu xuất là xuất đơn thuốc
        /// </summary>
        /// <param name="_serviceReq"></param>
        private void LoadDataToPrescriptionCommonInfo(HIS_SERVICE_REQ _serviceReq)
        {
            try
            {
                if (_serviceReq != null)
                {
                    // thông tin expMest
                    //Review
                    if (this._CurrentExpMest != null)
                    {
                        lblExpMestCode.Text = this._CurrentExpMest.EXP_MEST_CODE;
                        lblExpMedistock.Text = this._CurrentExpMest.MEDI_STOCK_CODE + " - " + this._CurrentExpMest.MEDI_STOCK_NAME;
                        //lblExpUserName.Text = this._CurrentExpMest.EXP_LOGINNAME + " - " + this._CurrentExpMest.EXP_USERNAME;
                        //lblExpTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this._CurrentExpMest.EXP_TIME ?? 0);
                    }

                    lblDescription.Text = _serviceReq.DESCRIPTION;

                    lblAdvise.Text = _serviceReq.ADVISE;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_serviceReq.TDL_PATIENT_DOB);
                    lblGenderName.Text = _serviceReq.TDL_PATIENT_GENDER_NAME;
                    lblIcdCode.Text = _serviceReq.ICD_CODE;
                    lblIcdText.Text = _serviceReq.ICD_TEXT;
                    if (!String.IsNullOrEmpty(_serviceReq.ICD_NAME))
                    {
                        lblIcdName.Text = _serviceReq.ICD_NAME;
                    }
                    else
                    {
                        lblIcdName.Text = "";
                    }
                    lblInstructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(_serviceReq.INTRUCTION_TIME);
                    lblPatientCode.Text = _serviceReq.TDL_PATIENT_CODE;
                    lblUserTimeFromTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(_serviceReq.USE_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(_serviceReq.USE_TIME_TO ?? 0);
                    lblVirAddress.Text = _serviceReq.TDL_PATIENT_ADDRESS;
                    lblVirPatientName.Text = _serviceReq.TDL_PATIENT_NAME;
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == _serviceReq.REQUEST_ROOM_ID).FirstOrDefault();
                    if (room != null)
                    {
                        lblRequestRoom.Text = room.ROOM_CODE + " - " + room.ROOM_NAME;
                    }
                    else
                    {
                        lblRequestRoom.Text = "";
                    }
                    var department = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().Where(o => o.ID == _serviceReq.REQUEST_DEPARTMENT_ID).FirstOrDefault();
                    if (department != null)
                    {
                        lblRequestDepartment.Text = department.DEPARTMENT_CODE + " - " + department.DEPARTMENT_NAME;
                    }
                    else
                    {
                        lblRequestDepartment.Text = "";
                    }
                }
                else
                {
                    ResetControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<string> GetListStringApprovalLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }

                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpTimeLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => p.EXP_TIME != null)
                      .GroupBy(o => o.EXP_TIME)
                      .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                      .ToList();
                }
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        void SetDataToCommonControlDetail(V_HIS_EXP_MEST expMest)
        {
            try
            {
                if (expMest != null)
                {
                    lblExpMestCode.Text = expMest.EXP_MEST_CODE;
                    lblExpMedistock.Text = expMest.MEDI_STOCK_CODE + " - " + expMest.MEDI_STOCK_NAME;
                    var approvalLoginnameList = GetListStringApprovalLogFromExpMestMedicineMaterial(this._ExpMestMedicines_Print, this._ExpMestMaterials_Print, this._ExpMestBloods_Print);
                    lblApprovalUserName.Text = String.Join(", ", approvalLoginnameList.Where(p => !String.IsNullOrEmpty(p)).Select(o => o));
                    var expLoginnameList = GetListStringExpLogFromExpMestMedicineMaterial(this._ExpMestMedicines_Print, this._ExpMestMaterials_Print, this._ExpMestBloods_Print);
                    lblExpUserName.Text = String.Join(", ", expLoginnameList);
                    var expTimeList = GetListStringExpTimeLogFromExpMestMedicineMaterial(this._ExpMestMedicines_Print, this._ExpMestMaterials_Print, this._ExpMestBloods_Print);
                    lblExpTime.Text = String.Join(", ", expTimeList);
                    lblDescription.Text = expMest.DESCRIPTION;
                    lblExpMestSttName.Text = expMest.EXP_MEST_STT_NAME;
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == expMest.REQ_ROOM_ID).FirstOrDefault();
                    if (room != null)
                    {
                        lblRequestRoom.Text = room.ROOM_CODE + " - " + room.ROOM_NAME;
                    }
                    else
                    {
                        lblRequestRoom.Text = "";
                    }
                    lblRequestDepartment.Text = expMest.REQ_DEPARTMENT_CODE + " - " + expMest.REQ_DEPARTMENT_NAME;
                    lblReqLoginName.Text = expMest.REQ_LOGINNAME + " - " + expMest.REQ_USERNAME;
                    var reason = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.ID == expMest.EXP_MEST_REASON_ID).FirstOrDefault();
                    if (reason != null)
                    {
                        lblExpMestReasonName.Text = reason.EXP_MEST_REASON_NAME;
                    }
                    else
                    {
                        lblExpMestReasonName.Text = "";
                    }
                    //
                    lblRecipient.Text = expMest.RECIPIENT;
                    lblRecevingPlace.Text = expMest.RECEIVING_PLACE;
                    HisExpMestViewFilter filter = new HisExpMestViewFilter();
                    filter.ID = _CurrentExpMest.ID;
                    var aggrExpMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, new CommonParam());
                    if (aggrExpMest != null && aggrExpMest.Count > 0)
                    {
                        cboExpMestReason.EditValue = aggrExpMest[0].EXP_MEST_REASON_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Set data control nếu k phải là đơn thuốc
        /// </summary>
        /// <param name="expMest"></param>
        private void SetDataToExpmestControl(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST expMest)
        {
            try
            {
                if (expMest != null)
                {
                    //Review
                    lblVirPatientName.Text = expMest.TDL_PATIENT_NAME;
                    lblPatientCode.Text = expMest.TDL_PATIENT_CODE;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(expMest.TDL_PATIENT_DOB ?? 0);
                    lblGenderName.Text = expMest.TDL_PATIENT_GENDER_NAME;
                    lblVirAddress.Text = expMest.TDL_PATIENT_ADDRESS;
                    lblInstructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(expMest.TDL_INTRUCTION_TIME ?? 0);
                    lblRecipient.Text = expMest.RECIPIENT;
                    lblRecevingPlace.Text = expMest.RECEIVING_PLACE;
                }
                else
                {
                    lblExpMestCode.Text = "";
                    lblExpMedistock.Text = "";
                    lblExpTime.Text = "";
                    lblExpUserName.Text = "";
                    lblDescription.Text = "";
                    lblExpMestSttName.Text = "";
                    lblApprovalUserName.Text = "";
                    lblRequestRoom.Text = "";
                    lblRequestDepartment.Text = "";
                    lblReqLoginName.Text = "";
                    lblExpMestReasonName.Text = "";
                    lblRecipient.Text = "";
                    lblRecevingPlace.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToGridControl()
        {
            try
            {
                gridControlRequestMedicine.DataSource = null;
                gridControlApprovalMedicine.DataSource = null;
                gridControlRequestMaterial.DataSource = null;
                gridControlApprovalMaterial.DataSource = null;
                gridControlRequestExpMestBlood.DataSource = null;
                gridControlAprroveExpMestBlood.DataSource = null;
                gridControlServiceReqMaty.DataSource = null;
                gridControlServiceReqMety.DataSource = null;
                gridControlTestService.DataSource = null;

                //Thuốc ở trạng thái yêu cầu với các loại xuất trừ bean trực tiếp (đơn pk, đơn điều trị, đơn tủ trực, xuất khác...)
                if (_CurrentExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                {
                    List<V_HIS_EXP_MEST_MEDICINE_1> expMestMetyReqSubs = _ExpMestMedicines != null && _ExpMestMedicines.Count > 0 ? _ExpMestMedicines.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList() : _ExpMestMedicines;

                    if (_ExpMestMetyReqs != null && expMestMetyReqSubs != null && expMestMetyReqSubs.Count > 0)
                    {
                        _ExpMestMetyReqs.AddRange(expMestMetyReqSubs);
                    }
                }

                var expMestMedicineReqs = GroupExpMestMedicine(_ExpMestMetyReqs);
                expMestMedicineReqs = (expMestMedicineReqs != null && expMestMedicineReqs.Count() > 0) ? expMestMedicineReqs.OrderBy(o => o.NUM_ORDER).ToList() : expMestMedicineReqs;
                gridControlRequestMedicine.DataSource = expMestMedicineReqs;
                if (expMestMedicineReqs != null)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE_1, V_HIS_EXP_MEST_MEDICINE>();
                    expMestMedicinePrint165 = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MEDICINE>>(expMestMedicineReqs);
                }
                //Thuốc ở trạng thái đang thực hiện, duyệt với các loại xuất trừ bean trực tiếp (đơn pk, đơn điều trị, đơn tủ trực, xuất khác...)
                List<V_HIS_EXP_MEST_MEDICINE_1> expMestMedicineSubs = _ExpMestMedicines != null && _ExpMestMedicines.Count > 0 ? _ExpMestMedicines.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE).ToList() : _ExpMestMedicines;

                var expMestMedicine = GroupExpMestMedicine(expMestMedicineSubs);
                expMestMedicine = (expMestMedicine != null && expMestMedicine.Count() > 0) ? expMestMedicine.OrderBy(o => o.NUM_ORDER).ToList() : expMestMedicine;
                gridControlApprovalMedicine.DataSource = expMestMedicine;
                if (expMestMedicinePrint165 == null || expMestMedicinePrint165.Count == 0)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE_1, V_HIS_EXP_MEST_MEDICINE>();
                    expMestMedicinePrint165 = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MEDICINE>>(expMestMedicine);
                }

                // Vat tu ở trạng thái yêu cầu với các loại xuất trừ bean trực tiếp (đơn pk, đơn điều trị, đơn tủ trực, xuất khác...)
                if (_CurrentExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                {
                    List<V_HIS_EXP_MEST_MATERIAL_1> expMestMatyReqSubs = _ExpMestMaterials != null && _ExpMestMaterials.Count > 0 ? _ExpMestMaterials.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList() : _ExpMestMaterials;
                    if (_ExpMestMatyReqs != null && expMestMatyReqSubs != null && expMestMatyReqSubs.Count > 0)
                    {
                        _ExpMestMatyReqs.AddRange(expMestMatyReqSubs);
                    }
                }

                var expMestMaterialRequests = GroupExpMestMaterial(_ExpMestMatyReqs);
                expMestMaterialRequests = (expMestMaterialRequests != null && expMestMaterialRequests.Count() > 0) ? expMestMaterialRequests.OrderBy(o => o.NUM_ORDER).ToList() : expMestMaterialRequests;
                gridControlRequestMaterial.DataSource = expMestMaterialRequests;

                if (expMestMaterialRequests != null)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL_1, V_HIS_EXP_MEST_MATERIAL>();
                    expMestMaterialPrint165 = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MATERIAL>>(expMestMaterialRequests);
                }

                //Vat tu ở trạng thái đang thực hiện, duyệt với các loại xuất trừ bean trực tiếp (đơn pk, đơn điều trị, đơn tủ trực, xuất khác...)
                List<V_HIS_EXP_MEST_MATERIAL_1> expMestMaterialSubs = _ExpMestMaterials != null && _ExpMestMaterials.Count > 0 ? _ExpMestMaterials.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE).ToList() : _ExpMestMaterials;
                var expMestMaterialAroval = GroupExpMestMaterial(expMestMaterialSubs);
                expMestMaterialAroval = (expMestMaterialAroval != null && expMestMaterialAroval.Count() > 0) ? expMestMaterialAroval.OrderBy(o => o.NUM_ORDER).ToList() : expMestMaterialAroval;
                gridControlApprovalMaterial.DataSource = expMestMaterialAroval;

                if (expMestMaterialPrint165 == null || expMestMaterialPrint165.Count == 0)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL_1, V_HIS_EXP_MEST_MATERIAL>();
                    expMestMaterialPrint165 = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MATERIAL>>(expMestMaterialAroval);
                }

                _ExpMestBltyReqs = (_ExpMestBltyReqs != null && _ExpMestBltyReqs.Count > 0) ? _ExpMestBltyReqs.OrderBy(o => o.NUM_ORDER).ToList() : _ExpMestBltyReqs;
                gridControlRequestExpMestBlood.DataSource = _ExpMestBltyReqs;
                _ExpMestBloods = (_ExpMestBloods != null && _ExpMestBloods.Count > 0) ? _ExpMestBloods.OrderBy(o => o.NUM_ORDER).ToList() : _ExpMestBloods;
                gridControlAprroveExpMestBlood.DataSource = _ExpMestBloods;
                ServiceReqMatys = (ServiceReqMatys != null && ServiceReqMatys.Count > 0) ? ServiceReqMatys.OrderBy(o => o.NUM_ORDER).ToList() : ServiceReqMatys;
                gridControlServiceReqMaty.DataSource = ServiceReqMatys;
                ServiceReqMetys = (ServiceReqMetys != null && ServiceReqMetys.Count > 0) ? ServiceReqMetys.OrderBy(o => o.NUM_ORDER).ToList() : ServiceReqMetys;
                gridControlServiceReqMety.DataSource = ServiceReqMetys;

                gridControlTestService.DataSource = this.SereServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<V_HIS_EXP_MEST_MEDICINE_1> GroupExpMestMedicine(List<V_HIS_EXP_MEST_MEDICINE_1> expMestMedicine1s)
        {
            if (expMestMedicine1s == null || expMestMedicine1s.Count == 0)
            {
                return new List<V_HIS_EXP_MEST_MEDICINE_1>();
            }

            List<V_HIS_EXP_MEST_MEDICINE_1> expMestmedicineTemps = new List<V_HIS_EXP_MEST_MEDICINE_1>();

            AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE_1, V_HIS_EXP_MEST_MEDICINE_1>();
            expMestmedicineTemps = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MEDICINE_1>>(expMestMedicine1s);

            List<V_HIS_EXP_MEST_MEDICINE_1> result = new List<V_HIS_EXP_MEST_MEDICINE_1>();
            try
            {
                var dataGroups = expMestmedicineTemps.GroupBy(o => new
                {
                    o.MEDICINE_TYPE_ID,
                    o.PRICE,
                    o.IMP_PRICE,
                    o.IMP_VAT_RATIO,
                    o.IS_NOT_PRES,
                    o.PATIENT_TYPE_ID,
                    o.OTHER_PAY_SOURCE_ID,
                    o.IS_EXPEND
                }).ToList();

                foreach (var dataGroup in dataGroups)
                {
                    V_HIS_EXP_MEST_MEDICINE_1 expMestmedicine = new V_HIS_EXP_MEST_MEDICINE_1();
                    expMestmedicine = dataGroup.First();
                    expMestmedicine.AMOUNT = dataGroup.Sum(o => o.AMOUNT);
                    expMestmedicine.PRES_AMOUNT = dataGroup.Sum(o => o.PRES_AMOUNT ?? o.AMOUNT);
                    expMestmedicine.SUM_BY_MEDICINE_IN_STOCK = dataGroup.Sum(o => o.SUM_BY_MEDICINE_IN_STOCK);
                    result.Add(expMestmedicine);
                }

                result = result.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_EXP_MEST_MEDICINE_1>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<V_HIS_EXP_MEST_MATERIAL_1> GroupExpMestMaterial(List<V_HIS_EXP_MEST_MATERIAL_1> expMestMedicine1s)
        {
            if (expMestMedicine1s == null || expMestMedicine1s.Count == 0)
            {
                return new List<V_HIS_EXP_MEST_MATERIAL_1>();
            }

            List<V_HIS_EXP_MEST_MATERIAL_1> result = new List<V_HIS_EXP_MEST_MATERIAL_1>();
            try
            {
                List<V_HIS_EXP_MEST_MATERIAL_1> expMestMaterialTemp = new List<V_HIS_EXP_MEST_MATERIAL_1>();
                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL_1, V_HIS_EXP_MEST_MATERIAL_1>();
                expMestMaterialTemp = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MATERIAL_1>>(expMestMedicine1s);

                var dataGroups = expMestMaterialTemp.GroupBy(o => new
                {
                    o.MATERIAL_TYPE_ID,
                    o.PRICE,
                    o.IMP_PRICE,
                    o.IMP_VAT_RATIO,
                    o.IS_NOT_PRES,
                    o.PATIENT_TYPE_ID,
                    o.OTHER_PAY_SOURCE_ID,
                    o.IS_EXPEND
                }).ToList();

                foreach (var dataGroup in dataGroups)
                {
                    V_HIS_EXP_MEST_MATERIAL_1 expMestmedicine = new V_HIS_EXP_MEST_MATERIAL_1();
                    expMestmedicine = dataGroup.First();
                    expMestmedicine.AMOUNT = dataGroup.Sum(o => o.AMOUNT);
                    expMestmedicine.PRES_AMOUNT = dataGroup.Sum(o => o.PRES_AMOUNT?? o.AMOUNT);
                    result.Add(expMestmedicine);
                }
                result = result != null ? result.OrderBy(o => o.MATERIAL_TYPE_NAME).ToList() : result;
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_EXP_MEST_MATERIAL_1>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<V_HIS_EXP_MEST_MEDICINE_1> _ExpMestMetyReqs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE_1> _ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL_1> _ExpMestMatyReqs { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL_1> _ExpMestMaterials { get; set; }
        List<ExpMestBloodADODetail> _ExpMestBltyReqs { get; set; }
        List<ExpMestBloodADODetail> _ExpMestBloods { get; set; }

        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs_Print { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs_Print { get; set; }
        List<V_HIS_EXP_MEST_BLTY_REQ_1> _ExpMestBltyReqs_Print { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials_Print { get; set; }
        List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods_Print { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines_Print { get; set; }
        List<HIS_SERVICE_REQ_MATY> ServiceReqMatys { get; set; }
        List<HIS_SERVICE_REQ_METY> ServiceReqMetys { get; set; }
        List<HIS_SERVICE_REQ> ServiceReqTests { get; set; }
        List<V_HIS_SERE_SERV> SereServs { get; set; }


        private void CreateThread()
        {
            Thread thread = new Thread(LoadMedistock);
            Thread thread1 = new Thread(LoadExpMestMetyReq);
            Thread thread2 = new Thread(LoadExpMestMatyReq);
            Thread thread3 = new Thread(LoadExpMestBltyReq);
            Thread thread4 = new Thread(LoadServiceReqMaty);
            Thread thread5 = new Thread(LoadServiceReqMety);
            Thread thread6 = new Thread(LoadTestServiceReq);

            try
            {
                thread.Start();
                thread1.Start();
                thread2.Start();
                thread3.Start();
                thread4.Start();
                thread5.Start();
                thread6.Start();

                thread.Join();
                thread1.Join();
                thread2.Join();
                thread3.Join();
                thread4.Join();
                thread5.Join();
                thread6.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
                thread1.Abort();
                thread2.Abort();
                thread3.Abort();
                thread4.Abort();
                thread5.Abort();
            }
        }

        /// <summary>
        /// lấy về medistock với medistockId tuong ung
        /// </summary>
        private void LoadMedistock()
        {
            try
            {
                if (moduleData != null)
                {
                    CommonParam param = new CommonParam();
                    // lấy về medistock với medistockId tuong ung
                    HisMediStockViewFilter medistockFilter = new HisMediStockViewFilter();
                    var medistocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == moduleData.RoomId);
                    if (medistocks != null && medistocks.Count() > 0)
                    {
                        this.currentMedistockId = medistocks.Select(o => o.ID).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu thuốc yêu cầu
        /// </summary>
        private void LoadExpMestMetyReq()
        {
            try
            {
                _ExpMestMetyReqs_Print = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMedicines_Print = new List<V_HIS_EXP_MEST_MEDICINE>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMetyReqFilter filter = new HisExpMestMetyReqFilter();
                filter.EXP_MEST_ID = this._CurrentExpMest.ID;
                _ExpMestMetyReqs_Print = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, filter, param);
                _ExpMestMetyReqs = new List<V_HIS_EXP_MEST_MEDICINE_1>();
                if (_ExpMestMetyReqs_Print != null && _ExpMestMetyReqs_Print.Count > 0)
                {
                    var dataGroups = _ExpMestMetyReqs_Print.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        V_HIS_EXP_MEST_MEDICINE_1 ado = new V_HIS_EXP_MEST_MEDICINE_1();
                        AutoMapper.Mapper.CreateMap<HIS_EXP_MEST_METY_REQ, V_HIS_EXP_MEST_MEDICINE_1>();
                        ado = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MEDICINE_1>(item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);

                        var typeName = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item[0].MEDICINE_TYPE_ID);
                        if (typeName != null)
                        {
                            ado.MEDICINE_TYPE_NAME = typeName.MEDICINE_TYPE_NAME;
                            ado.MEDICINE_TYPE_CODE = typeName.MEDICINE_TYPE_CODE;
                            ado.SERVICE_UNIT_NAME = typeName.SERVICE_UNIT_NAME;
                            ado.MEDICINE_GROUP_ID = typeName.MEDICINE_GROUP_ID;
                            ado.CONVERT_RATIO = typeName.CONVERT_RATIO;
                            ado.CONVERT_UNIT_NAME = typeName.CONVERT_UNIT_NAME;
                            ado.CONCENTRA = typeName.CONCENTRA;
                            //ado.IS_ADDICTIVE = typeName.IS_ADDICTIVE;
                            //ado.IS_ANTIBIOTIC = typeName.IS_ANTIBIOTIC;
                            //ado.IS_NEUROLOGICAL = typeName.IS_NEUROLOGICAL;
                        }
                        _ExpMestMetyReqs.Add(ado);
                    }
                }
                MOS.Filter.HisExpMestMedicineView1Filter MediFilter = new HisExpMestMedicineView1Filter();
                MediFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE_1>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW1, ApiConsumers.MosConsumer, MediFilter, param);

                if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                {
                    // var dataGroups = _ExpMestMedicines.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in _ExpMestMedicines)
                    {
                        V_HIS_EXP_MEST_MEDICINE ado = new V_HIS_EXP_MEST_MEDICINE();
                        AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE_1, V_HIS_EXP_MEST_MEDICINE>();
                        ado = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MEDICINE>(item);
                        _ExpMestMedicines_Print.Add(ado);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu Vat Tu yêu cầu
        /// </summary>
        private void LoadExpMestMatyReq()
        {
            try
            {
                _ExpMestMatyReqs_Print = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMaterials_Print = new List<V_HIS_EXP_MEST_MATERIAL>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMatyReqFilter filter = new HisExpMestMatyReqFilter();
                filter.EXP_MEST_ID = this._CurrentExpMest.ID;
                _ExpMestMatyReqs_Print = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, filter, param);
                _ExpMestMatyReqs = new List<V_HIS_EXP_MEST_MATERIAL_1>();
                if (_ExpMestMatyReqs_Print != null && _ExpMestMatyReqs_Print.Count > 0)
                {
                    var dataGroups = _ExpMestMatyReqs_Print.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        V_HIS_EXP_MEST_MATERIAL_1 ado = new V_HIS_EXP_MEST_MATERIAL_1();
                        AutoMapper.Mapper.CreateMap<HIS_EXP_MEST_MATY_REQ, V_HIS_EXP_MEST_MATERIAL_1>();
                        ado = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MATERIAL_1>(item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);

                        var typeName = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item[0].MATERIAL_TYPE_ID);
                        if (typeName != null)
                        {
                            ado.MATERIAL_TYPE_NAME = typeName.MATERIAL_TYPE_NAME;
                            ado.MATERIAL_TYPE_CODE = typeName.MATERIAL_TYPE_CODE;
                            ado.SERVICE_UNIT_NAME = typeName.SERVICE_UNIT_NAME;
                            ado.IS_CHEMICAL_SUBSTANCE = typeName.IS_CHEMICAL_SUBSTANCE;
                            ado.CONVERT_RATIO = typeName.CONVERT_RATIO;
                            ado.CONVERT_UNIT_NAME = typeName.CONVERT_UNIT_NAME;
                        }
                        _ExpMestMatyReqs.Add(ado);
                    }
                }
                //_ExpMestMaterials_Print
                MOS.Filter.HisExpMestMaterialView1Filter mateFilter = new HisExpMestMaterialView1Filter();
                mateFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL_1>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW1, ApiConsumers.MosConsumer, mateFilter, param);
                if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                {
                    // var dataGroups = _ExpMestMaterials.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in _ExpMestMaterials)
                    {
                        V_HIS_EXP_MEST_MATERIAL ado = new V_HIS_EXP_MEST_MATERIAL();
                        AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL_1, V_HIS_EXP_MEST_MATERIAL>();
                        ado = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MATERIAL>(item);
                        _ExpMestMaterials_Print.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu Mau yêu cầu
        /// </summary>
        private void LoadExpMestBltyReq()
        {
            try
            {
                _ExpMestBltyReqs_Print = new List<V_HIS_EXP_MEST_BLTY_REQ_1>();
                _ExpMestBloods_Print = new List<V_HIS_EXP_MEST_BLOOD>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestBltyReqView1Filter filter = new HisExpMestBltyReqView1Filter();
                filter.EXP_MEST_ID = this._CurrentExpMest.ID;
                Inventec.Common.Logging.LogSystem.Warn("Bat dau goi api HisExpMestBltyReq/GetView");
                _ExpMestBltyReqs_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLTY_REQ_1>>(RequestUri.HIS_EXP_MEST_BLTY_REQ_GET_VIEW1, ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Warn("Ket thuc goi api HisExpMestBltyReq/GetView");
                _ExpMestBltyReqs = new List<ExpMestBloodADODetail>();
                if (_ExpMestBltyReqs_Print != null && _ExpMestBltyReqs_Print.Count > 0)
                {
                    List<V_HIS_EXP_MEST_BLTY_REQ_1> expMestBltyReq1Temps = new List<V_HIS_EXP_MEST_BLTY_REQ_1>();
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_BLTY_REQ_1, V_HIS_EXP_MEST_BLTY_REQ_1>();
                    expMestBltyReq1Temps = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_BLTY_REQ_1>>(_ExpMestBltyReqs_Print);

                    var dataGroups = expMestBltyReq1Temps.GroupBy(p => p.BLOOD_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var itemGroup in dataGroups)
                    {
                        var _bloodTypes = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>();
                        ExpMestBloodADODetail ado = new ExpMestBloodADODetail();
                        var firstItem = itemGroup.First();
                        ado.AMOUNT = itemGroup.Sum(o => o.AMOUNT);
                        var data = _bloodTypes.FirstOrDefault(p => p.ID == itemGroup[0].BLOOD_TYPE_ID);
                        if (data != null)
                        {
                            ado.BLOOD_TYPE_CODE = firstItem.BLOOD_TYPE_CODE;
                            ado.BLOOD_TYPE_ID = data.ID;
                            ado.BLOOD_RH_CODE = firstItem.BLOOD_RH_CODE;
                            ado.BLOOD_ABO_CODE = firstItem.BLOOD_ABO_CODE;
                            ado.BLOOD_TYPE_NAME = firstItem.BLOOD_TYPE_NAME;
                            ado.SERVICE_UNIT_CODE = firstItem.SERVICE_UNIT_CODE;
                            ado.SERVICE_UNIT_NAME = firstItem.SERVICE_UNIT_NAME;
                            ado.VOLUME = data.VOLUME;
                        }
                        _ExpMestBltyReqs.Add(ado);
                    }
                }

                _ExpMestBloods = new List<ExpMestBloodADODetail>();
                MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                bloodFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                _ExpMestBloods_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(RequestUri.HIS_EXP_MEST_BLOOD_GET_VIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                if (_ExpMestBloods_Print != null && _ExpMestBloods_Print.Count > 0)
                {
                    //List<V_HIS_EXP_MEST_BLOOD> expMestBloodTemps = new List<V_HIS_EXP_MEST_BLOOD>();
                    //AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_BLOOD, V_HIS_EXP_MEST_BLOOD>();
                    //expMestBloodTemps = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_BLOOD>>(_ExpMestBloods_Print);
                    //var dataGroups = expMestBloodTemps.GroupBy(p => new { p.BLOOD_TYPE_ID, p.PRICE, p.IMP_PRICE, p.VOLUME }).Select(p => p.ToList()).ToList();
                    //foreach (var itemGroup in dataGroups)
                    //{
                    //    ExpMestBloodADODetail ado = new ExpMestBloodADODetail(itemGroup[0]);
                    //    ado.AMOUNT = itemGroup.Count();
                    //    _ExpMestBloods.Add(ado);
                    //}

                    foreach (var item in _ExpMestBloods_Print)
                    {
                        ExpMestBloodADODetail ado = new ExpMestBloodADODetail(item);
                        ado.AMOUNT = 1;
                        _ExpMestBloods.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu vật tư ngoài kho
        /// </summary>
        private void LoadServiceReqMaty()
        {
            try
            {
                if (this._CurrentExpMest == null || this._CurrentExpMest.SERVICE_REQ_ID == null)
                    return;

                ServiceReqMatys = new List<HIS_SERVICE_REQ_MATY>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMatyFilter filter = new HisServiceReqMatyFilter();
                filter.SERVICE_REQ_ID = this._CurrentExpMest.SERVICE_REQ_ID;
                ServiceReqMatys = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>(RequestUri.HIS_SERVICE_REQ_MATY_GET, ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu thuốc ngoài kho
        /// </summary>
        private void LoadServiceReqMety()
        {
            try
            {
                if (this._CurrentExpMest == null || this._CurrentExpMest.SERVICE_REQ_ID == null)
                    return;

                ServiceReqMetys = new List<HIS_SERVICE_REQ_METY>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMetyFilter filter = new HisServiceReqMetyFilter();
                filter.SERVICE_REQ_ID = this._CurrentExpMest.SERVICE_REQ_ID;
                ServiceReqMetys = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>(RequestUri.HIS_SERVICE_REQ_METY_GET, ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTestServiceReq()
        {
            try
            {
                if (this._CurrentExpMest == null || this._CurrentExpMest.SERVICE_REQ_ID == null)
                    return;

                this.ServiceReqTests = new List<HIS_SERVICE_REQ>();

                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.PARENT_ID = this._CurrentExpMest.SERVICE_REQ_ID;
                this.ServiceReqTests = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUri.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param);

                if (this.ServiceReqTests == null || this.ServiceReqTests.Count() == 0)
                {
                    return;
                }

                MOS.Filter.HisSereServViewFilter filterSs = new HisSereServViewFilter();
                filterSs.ORDER_FIELD = "TDL_INTRUCTION_TIME";
                filterSs.ORDER_DIRECTION = "ASC";
                filterSs.SERVICE_REQ_IDs = this.ServiceReqTests.Select(o => o.ID).ToList();
                this.SereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filterSs, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
