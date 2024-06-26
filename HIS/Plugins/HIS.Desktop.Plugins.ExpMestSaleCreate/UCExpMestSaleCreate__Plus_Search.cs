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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Base;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using ACS.EFMODEL.DataModels;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        List<V_HIS_EXP_MEST> expMestDones = null;
        List<HIS_SERVICE_REQ> dataServiceReqs = null;
        private void ProcessorSearch(bool IsServiceReqCode, List<long> serviceReqIds = null)
        {
            try
            {
                // #22085
                Inventec.Common.Logging.LogSystem.Debug("****Bat dau goi ProcessorSearch ****");
                lblTotalPrice.Text = "";
                lblPayPrice.Text = "";
                ReleaseAll();
                SetControlByExpMest(null);
                treeListMediMate.DataSource = null;
                dicMediMateAdo = new Dictionary<long, List<MediMateTypeADO>>();
                LoadPatientInfoFromdataExpMest(null);
                moduleAction = GlobalDataStore.ModuleAction.ADD;
                CommonParam param = new CommonParam();
                HisExpMestForSaleFilter _serviceReqFilter = new HisExpMestForSaleFilter();
                string codeSer = "";
                if (serviceReqIds != null && serviceReqIds.Count > 0)
                {
                    _serviceReqFilter.SERVICE_REQ_IDs = serviceReqIds;
                }
                else if (IsServiceReqCode)
                {
                    codeSer = String.Format("{0:000000000000}", Convert.ToInt64(txtPrescriptionCode.Text.Trim()));
                    txtPrescriptionCode.Text = codeSer;
                    _serviceReqFilter.SERVICE_REQ_CODE__EXACT = codeSer;
                }
                else
                {
                    codeSer = String.Format("{0:000000000000}", Convert.ToInt64(txtTreatmentCode.Text.Trim()));
                    txtTreatmentCode.Text = codeSer;
                    _serviceReqFilter.TREATMENT_CODE__EXACT = codeSer;
                }

                var ExpMestSall = new BackendAdapter(param)
              .Get<HisExpMestForSaleSDO>("api/HisExpMest/GetForSale", ApiConsumers.MosConsumer, _serviceReqFilter, param);
                 dataServiceReqs = ExpMestSall != null ? ExpMestSall.ServiceReqs : null;

                if (dataServiceReqs != null && dataServiceReqs.Count > 0)
                {
                    List<HIS_SERVICE_REQ_METY> serviceReqMetys = ExpMestSall.ServiceReqMetys;

                    bool IsNK = false;
                    if (serviceReqMetys != null && serviceReqMetys.Count > 0)
                    {
                        IsNK = true;
                    }

                    List<HIS_SERVICE_REQ_MATY> serviceReqMatys = ExpMestSall.ServiceReqMatys;
                    if (serviceReqMatys != null && serviceReqMatys.Count > 0)
                    {
                        IsNK = true;
                    }
                    if (IsNK)
                    {
                        var dataEs = ExpMestSall.ExpMests;

                        if (dataEs != null && dataEs.Count > 0)
                        {

                            moduleAction = GlobalDataStore.ModuleAction.EDIT;
                            var expMestReqs = dataEs.Where(p => p.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && !p.BILL_ID.HasValue).ToList();
                            if (expMestReqs != null && expMestReqs.Count > 0)
                            {
                                LoadDataExpMestBySearch(expMestReqs, IsServiceReqCode, ExpMestSall);
                            }
                            else
                            {
                                //+ Khi người dùng thực hiện tìm kiếm, nếu có thông tin phiếu xuất đã thực xuất --> kiểm tra xem số lượng đã xuất và số lượng kê ngoài kho có bằng nhau ko, nếu số lượng kê ngoài kho lớn hơn số lượng đã xuất --> hiển thị thông tin kê thuốc ngoài kho còn lại --> cho phép người dùng tạo mới phiếu xuất
                                expMestDones = dataEs.Where(p => p.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                                    || p.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                                    || p.BILL_ID.HasValue).ToList();//xuandv them ca hoan thanh

                                //#27803 - Trong trường hợp khi quét mã, nếu tất cả các thuốc đều đã thuộc các phiếu xuất và các phiếu đều ở trạng thái duyệt/thực xuất thì:
                                //+ Hiển thị cảnh báo "Đơn thuốc đã được xuất bán hết. Mã xuất (xxxx, yyyy)"
                                //+ Đồng thời, hiển thị thông tin các phiếu xuất lên màn hình phiếu xuất bán.
                                //+ Các nút "lưu", "lưu in" sẽ disable, chỉ enable nút "hủy thực xuất"
                                //- Người dùng nhấn "hủy thực xuất" --> xử lý thành công thì enable các nút "lưu", "lưu in" lên để cho sửa
                                if (expMestDones != null && dataEs != null && expMestDones.Count == dataEs.Count)
                                {
                                    LoadDataExpMestByEditMulti(expMestDones, ExpMestSall, true);
                                    List<string> ccc = expMestDones.Select(p => p.EXP_MEST_CODE).ToList();
                                    string messs = string.Format("Đơn thuốc đã được xuất bán hết. Mã xuất ({0})", string.Join(",", ccc));
                                    DevExpress.XtraEditors.XtraMessageBox.Show(messs, "Thông báo");
                                }
                                else if (expMestDones != null && expMestDones.Count > 0)
                                {
                                    moduleAction = GlobalDataStore.ModuleAction.ADD;
                                    btnCancelExport.Enabled = false;
                                    LoadDataFromExpMest(expMestDones[0], IsServiceReqCode);

                                    List<long> _expMestIds = expMestDones.Select(p => p.ID).ToList();

                                    List<HIS_EXP_MEST_MEDICINE> dataExpMestMedicines = ExpMestSall.Medicines;

                                    List<HIS_EXP_MEST_MATERIAL> dataExpMestMaterials = ExpMestSall.Materials;

                                    #region Thuoc
                                    List<HIS_SERVICE_REQ_METY> _listNewMedis = new List<HIS_SERVICE_REQ_METY>();
                                    if (dataExpMestMedicines != null && dataExpMestMedicines.Count > 0)
                                    {
                                        var dataMetyGroups = serviceReqMetys.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.SERVICE_REQ_ID }).Select(p => p.ToList()).ToList();
                                        foreach (var item in dataMetyGroups)
                                        {
                                            HIS_SERVICE_REQ_METY adoNew = new HIS_SERVICE_REQ_METY();
                                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ_METY>(adoNew, item[0]);
                                            adoNew.AMOUNT = item.Sum(p => p.AMOUNT);

                                            var expPre = expMestDones.Where(p => p.PRESCRIPTION_ID == adoNew.SERVICE_REQ_ID).ToList();
                                            List<HIS_EXP_MEST_MEDICINE> dataMedis = new List<HIS_EXP_MEST_MEDICINE>();
                                            if (expPre != null && expPre.Count > 0)
                                            {
                                                List<long> _checks = expPre.Select(p => p.ID).ToList();
                                                dataMedis = dataExpMestMedicines.Where(p =>
                                                                                     p.TDL_MEDICINE_TYPE_ID == adoNew.MEDICINE_TYPE_ID
                                                                                     && _checks.Contains(p.EXP_MEST_ID ?? 0)).ToList();
                                            }

                                            if (dataMedis != null && dataMedis.Count > 0)
                                            {
                                                if (adoNew.AMOUNT > dataMedis.Sum(p => p.AMOUNT))
                                                {
                                                    //Lay thuoc và gan lại số lượng = sl yêu cầu - sl đã xuất
                                                    adoNew.AMOUNT = adoNew.AMOUNT - dataMedis.Sum(p => p.AMOUNT);
                                                    _listNewMedis.Add(adoNew);
                                                }
                                            }
                                            else
                                            {
                                                //Thuốc chưa xuất--- lấy ra tạo phiếu mới
                                                _listNewMedis.Add(adoNew);
                                            }
                                        }
                                    }
                                    else if (serviceReqMetys != null && serviceReqMetys.Count > 0)
                                    {
                                        var dataMetyGroups = serviceReqMetys.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.SERVICE_REQ_ID }).Select(p => p.ToList()).ToList();
                                        foreach (var item in dataMetyGroups)
                                        {
                                            HIS_SERVICE_REQ_METY adoNew = new HIS_SERVICE_REQ_METY();
                                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ_METY>(adoNew, item[0]);
                                            adoNew.AMOUNT = item.Sum(p => p.AMOUNT);

                                            //Thuốc chưa xuất--- lấy ra tạo phiếu mới
                                            _listNewMedis.Add(adoNew);
                                        }
                                    }
                                    #endregion

                                    #region VatTu
                                    List<HIS_SERVICE_REQ_MATY> _listNewMates = new List<HIS_SERVICE_REQ_MATY>();
                                    if (dataExpMestMaterials != null && dataExpMestMaterials.Count > 0)
                                    {
                                        var dataMatyGroups = serviceReqMatys.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.SERVICE_REQ_ID }).Select(p => p.ToList()).ToList();
                                        foreach (var item in dataMatyGroups)
                                        {
                                            HIS_SERVICE_REQ_MATY adoNew = new HIS_SERVICE_REQ_MATY();
                                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ_MATY>(adoNew, item[0]);
                                            adoNew.AMOUNT = item.Sum(p => p.AMOUNT);
                                            var expPre = expMestDones.Where(p => p.PRESCRIPTION_ID == adoNew.SERVICE_REQ_ID).ToList();
                                            List<HIS_EXP_MEST_MATERIAL> dataMedis = new List<HIS_EXP_MEST_MATERIAL>();
                                            if (expPre != null && expPre.Count > 0)
                                            {
                                                List<long> _checks = expPre.Select(p => p.ID).ToList();
                                                dataMedis = dataExpMestMaterials.Where(p =>
                                                                                     p.TDL_MATERIAL_TYPE_ID == adoNew.MATERIAL_TYPE_ID
                                                                                     && _checks.Contains(p.EXP_MEST_ID ?? 0)).ToList();
                                            }
                                            if (dataMedis != null && dataMedis.Count > 0)
                                            {
                                                if (adoNew.AMOUNT > dataMedis.Sum(p => p.AMOUNT))
                                                {
                                                    //Lay thuoc và gan lại số lượng = sl yêu cầu - sl đã xuất
                                                    adoNew.AMOUNT = adoNew.AMOUNT - dataMedis.Sum(p => p.AMOUNT);
                                                    _listNewMates.Add(adoNew);
                                                }
                                            }
                                            else
                                            {
                                                //Thuốc chưa xuất--- lấy ra tạo phiếu mới
                                                _listNewMates.Add(adoNew);
                                            }
                                        }
                                    }
                                    else if (serviceReqMatys != null && serviceReqMatys.Count > 0)
                                    {
                                        var dataMatyGroups = serviceReqMatys.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.SERVICE_REQ_ID }).Select(p => p.ToList()).ToList();
                                        foreach (var item in dataMatyGroups)
                                        {
                                            HIS_SERVICE_REQ_MATY adoNew = new HIS_SERVICE_REQ_MATY();
                                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ_MATY>(adoNew, item[0]);
                                            adoNew.AMOUNT = item.Sum(p => p.AMOUNT);

                                            //Thuốc chưa xuất--- lấy ra tạo phiếu mới
                                            _listNewMates.Add(adoNew);
                                        }
                                    }
                                    #endregion

                                    foreach (var itemService in dataServiceReqs)
                                    {
                                        Dictionary<long, MediMateTypeADO> dicMediMateTempAdo = new Dictionary<long, MediMateTypeADO>();
                                        var clientSessionKey = Guid.NewGuid().ToString();
                                        #region -- T
                                        if (_listNewMedis != null && _listNewMedis.Count > 0)
                                        {
                                            _listNewMedis = _listNewMedis.Where(o => o.MEDICINE_TYPE_ID.HasValue).ToList();

                                            var dataNews = _listNewMedis.Where(p => p.SERVICE_REQ_ID == itemService.ID).ToList();
                                            foreach (var serviceReqMety in dataNews)
                                            {
                                                long? intructionTime = null;
                                                if (dtIntructionTime.EditValue != null)
                                                    intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTime.DateTime) ?? 0;
                                                MediMateTypeADO ado = null;
                                                if (dicMediMateTempAdo.ContainsKey(serviceReqMety.MEDICINE_TYPE_ID.Value))
                                                {
                                                    ado = dicMediMateTempAdo[serviceReqMety.MEDICINE_TYPE_ID.Value];
                                                    ado.EXP_AMOUNT += serviceReqMety.AMOUNT;
                                                    ado.TOTAL_PRICE = ado.EXP_AMOUNT * (ado.EXP_PRICE ?? 0);
                                                }
                                                else
                                                {
                                                    ado = new MediMateTypeADO(serviceReqMety, intructionTime);
                                                    dicMediMateTempAdo[serviceReqMety.MEDICINE_TYPE_ID.Value] = ado;
                                                }
                                                ado.ClientSessionKey = clientSessionKey;
                                                ado.SERVICE_REQ_ID = itemService.ID;
                                                ado.SERVICE_REQ_CODE = itemService.SERVICE_REQ_CODE;
                                                ado.PARENT_ID__IN_SETY = ado.SERVICE_REQ_CODE;
                                                ado.CONCRETE_ID__IN_SETY = ado.SERVICE_REQ_CODE + "_" + serviceReqMety.MEDICINE_TYPE_ID;
                                                ado.TDL_PATIENT_NAME = itemService.TDL_PATIENT_NAME;
                                                ado.PATIENT_TYPE_ID = itemService.TDL_PATIENT_TYPE_ID;
                                                ado.TDL_PATIENT_CODE = itemService.TDL_PATIENT_CODE;
                                                ado.TDL_PATIENT_DOB = itemService.TDL_PATIENT_DOB;
                                                ado.TDL_PATIENT_GENDER_ID = itemService.TDL_PATIENT_GENDER_ID;
                                                ado.TDL_PATIENT_GENDER_NAME = itemService.TDL_PATIENT_GENDER_NAME;
                                                ado.TDL_PATIENT_ID = itemService.TDL_PATIENT_ID;
                                                ado.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = itemService.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                                                ado.REQUEST_LOGINNAME = itemService.REQUEST_LOGINNAME;
                                                ado.REQUEST_USERNAME = itemService.REQUEST_USERNAME;
                                                if (mediInStocks != null && mediInStocks.Count > 0)
                                                {
                                                    HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == serviceReqMety.MEDICINE_TYPE_ID);
                                                    if (mediInStockSDO != null)
                                                    {
                                                        ado.NATIONAL_NAME = mediInStockSDO.NationalName;
                                                        ado.MANUFACTURER_NAME = mediInStockSDO.ManufacturerName;
                                                        ado.REGISTER_NUMBER = mediInStockSDO.RegisterNumber;
                                                        ado.SERVICE_UNIT_NAME = mediInStockSDO.ServiceUnitName;
                                                        ado.AVAILABLE_AMOUNT = mediInStockSDO.AvailableAmount;
                                                        if (mediInStockSDO.AvailableAmount < ado.EXP_AMOUNT)
                                                        {
                                                            ado.IsExceedsAvailable = true;
                                                        }
                                                        ado.ACTIVE_INGR_BHYT_CODE = mediInStockSDO.ActiveIngrBhytCode;
                                                        ado.ACTIVE_INGR_BHYT_NAME = mediInStockSDO.ActiveIngrBhytName;
                                                        ado.CONCENTRA = mediInStockSDO.Concentra;
                                                    }
                                                    else
                                                    {
                                                        ado.IsNotInStock = true;
                                                    }
                                                }
                                                else
                                                {
                                                    ado.IsNotInStock = true;
                                                }
                                            }
                                        }
                                        #endregion

                                        #region ----VT
                                        if (_listNewMates != null && _listNewMates.Count > 0)
                                        {
                                            _listNewMates = _listNewMates.Where(o => o.MATERIAL_TYPE_ID.HasValue).ToList();
                                            var dataNews = _listNewMates.Where(p => p.SERVICE_REQ_ID == itemService.ID).ToList();
                                            foreach (var serviceReqMaty in dataNews)
                                            {
                                                MediMateTypeADO ado = null;
                                                if (dicMediMateTempAdo.ContainsKey(serviceReqMaty.MATERIAL_TYPE_ID.Value))
                                                {
                                                    ado = dicMediMateTempAdo[serviceReqMaty.MATERIAL_TYPE_ID.Value];
                                                    ado.EXP_AMOUNT += serviceReqMaty.AMOUNT;
                                                    ado.TOTAL_PRICE = ado.EXP_AMOUNT * (ado.EXP_PRICE ?? 0);
                                                }
                                                else
                                                {
                                                    ado = new MediMateTypeADO(serviceReqMaty);
                                                    dicMediMateTempAdo[serviceReqMaty.MATERIAL_TYPE_ID.Value] = ado;
                                                }
                                                ado.ClientSessionKey = clientSessionKey;
                                                ado.SERVICE_REQ_ID = itemService.ID;
                                                ado.SERVICE_REQ_CODE = itemService.SERVICE_REQ_CODE;
                                                ado.PARENT_ID__IN_SETY = ado.SERVICE_REQ_CODE;
                                                ado.CONCRETE_ID__IN_SETY = ado.SERVICE_REQ_CODE + "_" + serviceReqMaty.MATERIAL_TYPE_ID;
                                                ado.TDL_PATIENT_NAME = itemService.TDL_PATIENT_NAME;
                                                ado.PATIENT_TYPE_ID = itemService.TDL_PATIENT_TYPE_ID;
                                                ado.TDL_PATIENT_CODE = itemService.TDL_PATIENT_CODE;
                                                ado.TDL_PATIENT_DOB = itemService.TDL_PATIENT_DOB;
                                                ado.TDL_PATIENT_GENDER_ID = itemService.TDL_PATIENT_GENDER_ID;
                                                ado.TDL_PATIENT_GENDER_NAME = itemService.TDL_PATIENT_GENDER_NAME;
                                                ado.TDL_PATIENT_ID = itemService.TDL_PATIENT_ID;
                                                ado.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = itemService.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                                                ado.REQUEST_LOGINNAME = itemService.REQUEST_LOGINNAME;
                                                ado.REQUEST_USERNAME = itemService.REQUEST_USERNAME;
                                                if (mateInStocks != null && mateInStocks.Count > 0)
                                                {
                                                    HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == serviceReqMaty.MATERIAL_TYPE_ID.Value);
                                                    if (mateInStockSDO != null)
                                                    {
                                                        ado.NATIONAL_NAME = mateInStockSDO.NationalName;
                                                        ado.MANUFACTURER_NAME = mateInStockSDO.ManufacturerName;
                                                        ado.SERVICE_UNIT_NAME = mateInStockSDO.ServiceUnitName;
                                                        ado.AVAILABLE_AMOUNT = mateInStockSDO.AvailableAmount;
                                                        if (mateInStockSDO.AvailableAmount < serviceReqMaty.AMOUNT)
                                                        {
                                                            ado.IsExceedsAvailable = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ado.IsNotInStock = true;
                                                    }
                                                }
                                                else
                                                {
                                                    ado.IsNotInStock = true;
                                                }
                                            }
                                        }
                                        #endregion

                                        if (dicMediMateTempAdo != null && dicMediMateTempAdo.Count > 0)
                                        {
                                            //Take bean

                                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMediMateTempAdo), dicMediMateTempAdo));
                                            TakeBeanMedicineAll(dicMediMateTempAdo, clientSessionKey);
                                            TakeBeanMaterialAll(dicMediMateTempAdo, clientSessionKey);
                                        }
                                    }

                                    List<MediMateTypeADO> dataSources = dicMediMateAdo.Select(p => p.Value).SelectMany(p => p).Distinct().ToList();
                                    if (dataSources != null && dataSources.Count > 0)
                                    {
                                        SetTotalPriceExpMestDetail();
                                        ProcessSetDataTrees(dataSources);
                                    }
                                    else
                                    {
                                        List<string> ccc = expMestDones.Select(p => p.EXP_MEST_CODE).ToList();
                                        string messs = string.Format("Đơn thuốc đã được xuất bán hết. Mã xuất ({0})", string.Join(",", ccc));
                                        DevExpress.XtraEditors.XtraMessageBox.Show(messs, "Thông báo");
                                    }
                                }
                            }

                            if (dataEs.Exists(o => o.BILL_ID.HasValue))
                            {
                                GetTransactionById(dataEs.Select(s => s.BILL_ID ?? 0).ToList());
                            }
                        }
                        else
                        {
                            LoadPatientInfoFromdataServiceReq(dataServiceReqs[0]);

                            foreach (var itemService in dataServiceReqs)
                            {
                                Dictionary<long, MediMateTypeADO> dicMediMateTempAdo = new Dictionary<long, MediMateTypeADO>();
                                var clientSessionKey = Guid.NewGuid().ToString();
                                #region --- T
                                if (serviceReqMetys != null && serviceReqMetys.Count > 0)
                                {
                                    serviceReqMetys = serviceReqMetys.Where(o => o.MEDICINE_TYPE_ID.HasValue).ToList();
                                    var dataNews = serviceReqMetys.Where(p => p.SERVICE_REQ_ID == itemService.ID).ToList();
                                    foreach (var serviceReqMety in dataNews)
                                    {
                                        long? intructionTime = null;
                                        if (dtIntructionTime.EditValue != null)
                                            intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTime.DateTime) ?? 0;
                                        MediMateTypeADO ado = null;
                                        if (dicMediMateTempAdo.ContainsKey(serviceReqMety.MEDICINE_TYPE_ID.Value))
                                        {
                                            ado = dicMediMateTempAdo[serviceReqMety.MEDICINE_TYPE_ID.Value];
                                            ado.EXP_AMOUNT += serviceReqMety.AMOUNT;
                                            ado.TOTAL_PRICE = ado.EXP_AMOUNT * (ado.EXP_PRICE ?? 0);
                                        }
                                        else
                                        {
                                            ado = new MediMateTypeADO(serviceReqMety, intructionTime);
                                            dicMediMateTempAdo[serviceReqMety.MEDICINE_TYPE_ID.Value] = ado;
                                        }
                                        ado.ClientSessionKey = clientSessionKey;
                                        ado.SERVICE_REQ_ID = itemService.ID;
                                        ado.SERVICE_REQ_CODE = itemService.SERVICE_REQ_CODE;
                                        ado.PARENT_ID__IN_SETY = itemService.SERVICE_REQ_CODE;
                                        ado.CONCRETE_ID__IN_SETY = itemService.SERVICE_REQ_CODE + "_" + serviceReqMety.MEDICINE_TYPE_ID;
                                        ado.TDL_PATIENT_NAME = itemService.TDL_PATIENT_NAME;
                                        ado.PATIENT_TYPE_ID = itemService.TDL_PATIENT_TYPE_ID;
                                        ado.TDL_PATIENT_CODE = itemService.TDL_PATIENT_CODE;
                                        ado.TDL_PATIENT_DOB = itemService.TDL_PATIENT_DOB;
                                        ado.TDL_PATIENT_GENDER_ID = itemService.TDL_PATIENT_GENDER_ID;
                                        ado.TDL_PATIENT_GENDER_NAME = itemService.TDL_PATIENT_GENDER_NAME;
                                        ado.TDL_PATIENT_ID = itemService.TDL_PATIENT_ID;
                                        ado.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = itemService.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                                        ado.REQUEST_LOGINNAME = itemService.REQUEST_LOGINNAME;
                                        ado.REQUEST_USERNAME = itemService.REQUEST_USERNAME;
                                        ado.TDL_PATIENT_PHONE = itemService.TDL_PATIENT_PHONE;

                                        if (mediInStocks != null && mediInStocks.Count > 0)
                                        {
                                            HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == serviceReqMety.MEDICINE_TYPE_ID);
                                            if (mediInStockSDO != null)
                                            {
                                                ado.NATIONAL_NAME = mediInStockSDO.NationalName;
                                                ado.MANUFACTURER_NAME = mediInStockSDO.ManufacturerName;
                                                ado.REGISTER_NUMBER = mediInStockSDO.RegisterNumber;
                                                ado.SERVICE_UNIT_NAME = mediInStockSDO.ServiceUnitName;
                                                ado.AVAILABLE_AMOUNT = mediInStockSDO.AvailableAmount;
                                                if (mediInStockSDO.AvailableAmount < ado.EXP_AMOUNT)
                                                {
                                                    ado.IsExceedsAvailable = true;
                                                }
                                                ado.ACTIVE_INGR_BHYT_CODE = mediInStockSDO.ActiveIngrBhytCode;
                                                ado.ACTIVE_INGR_BHYT_NAME = mediInStockSDO.ActiveIngrBhytName;
                                                ado.CONCENTRA = mediInStockSDO.Concentra;
                                            }
                                            else
                                            {
                                                ado.IsNotInStock = true;
                                            }
                                        }
                                        else
                                        {
                                            ado.IsNotInStock = true;
                                        }
                                    }
                                }
                                #endregion

                                #region ---VT
                                if (serviceReqMatys != null && serviceReqMatys.Count > 0)
                                {
                                    serviceReqMatys = serviceReqMatys.Where(o => o.MATERIAL_TYPE_ID.HasValue).ToList();
                                    var dataNews = serviceReqMatys.Where(p => p.SERVICE_REQ_ID == itemService.ID).ToList();
                                    foreach (var serviceReqMaty in dataNews)
                                    {
                                        MediMateTypeADO ado = null;
                                        if (dicMediMateTempAdo.ContainsKey(serviceReqMaty.MATERIAL_TYPE_ID.Value))
                                        {
                                            ado = dicMediMateTempAdo[serviceReqMaty.MATERIAL_TYPE_ID.Value];
                                            ado.EXP_AMOUNT += serviceReqMaty.AMOUNT;
                                            ado.TOTAL_PRICE = ado.EXP_AMOUNT * (ado.EXP_PRICE ?? 0);
                                        }
                                        else
                                        {
                                            ado = new MediMateTypeADO(serviceReqMaty);
                                            dicMediMateTempAdo[serviceReqMaty.MATERIAL_TYPE_ID.Value] = ado;
                                        }
                                        ado.ClientSessionKey = clientSessionKey;
                                        ado.SERVICE_REQ_ID = itemService.ID;
                                        ado.SERVICE_REQ_CODE = itemService.SERVICE_REQ_CODE;
                                        ado.PARENT_ID__IN_SETY = itemService.SERVICE_REQ_CODE;
                                        ado.CONCRETE_ID__IN_SETY = itemService.SERVICE_REQ_CODE + "_" + serviceReqMaty.MATERIAL_TYPE_ID;
                                        ado.TDL_PATIENT_NAME = itemService.TDL_PATIENT_NAME;
                                        ado.PATIENT_TYPE_ID = itemService.TDL_PATIENT_TYPE_ID;
                                        ado.TDL_PATIENT_CODE = itemService.TDL_PATIENT_CODE;
                                        ado.TDL_PATIENT_DOB = itemService.TDL_PATIENT_DOB;
                                        ado.TDL_PATIENT_GENDER_ID = itemService.TDL_PATIENT_GENDER_ID;
                                        ado.TDL_PATIENT_GENDER_NAME = itemService.TDL_PATIENT_GENDER_NAME;
                                        ado.TDL_PATIENT_ID = itemService.TDL_PATIENT_ID;
                                        ado.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = itemService.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                                        ado.REQUEST_LOGINNAME = itemService.REQUEST_LOGINNAME;
                                        ado.REQUEST_USERNAME = itemService.REQUEST_USERNAME;
                                        ado.TDL_PATIENT_PHONE = itemService.TDL_PATIENT_PHONE; 
                                        if (mateInStocks != null && mateInStocks.Count > 0)
                                        {
                                            HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == serviceReqMaty.MATERIAL_TYPE_ID.Value);
                                            if (mateInStockSDO != null)
                                            {
                                                ado.NATIONAL_NAME = mateInStockSDO.NationalName;
                                                ado.MANUFACTURER_NAME = mateInStockSDO.ManufacturerName;
                                                ado.SERVICE_UNIT_NAME = mateInStockSDO.ServiceUnitName;
                                                ado.AVAILABLE_AMOUNT = mateInStockSDO.AvailableAmount;
                                                if (mateInStockSDO.AvailableAmount < serviceReqMaty.AMOUNT)
                                                {
                                                    ado.IsExceedsAvailable = true;
                                                }
                                            }
                                            else
                                            {
                                                ado.IsNotInStock = true;
                                            }
                                        }
                                        else
                                        {
                                            ado.IsNotInStock = true;
                                        }
                                    }
                                }
                                #endregion

                                if (dicMediMateTempAdo != null && dicMediMateTempAdo.Count > 0)
                                {
                                    //Take bean
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMediMateTempAdo), dicMediMateTempAdo));
                                    TakeBeanMedicineAll(dicMediMateTempAdo, clientSessionKey);
                                    TakeBeanMaterialAll(dicMediMateTempAdo, clientSessionKey);
                                }
                            }

                            List<MediMateTypeADO> dataSources = dicMediMateAdo.Select(p => p.Value).SelectMany(p => p).Distinct().ToList();

                            

                            ProcessSetDataTrees(dataSources);
                            SetTotalPriceExpMestDetail();
                        }
                    }
                    else
                    {
                        //FillDataMediMateGrid();
                    }
                    LoadPatientInfoFromdataServiceReq(dataServiceReqs.FirstOrDefault());
                    //TakeBeanFromServiceReqs(dataServiceReqs);
                }
                else
                {
                    //DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Không tim thấy thông tin với mã tương ứng {0}", codeSer), "Thông báo");
                    //FillDataMediMateGrid();
                }
                this.SetEnableButtonDebt(this.moduleAction == GlobalDataStore.ModuleAction.EDIT);
                if (chkAutoShow.Checked)
                {
                    repositoryItemBtnView_ButtonClick(null, null);
                }
                Inventec.Common.Logging.LogSystem.Debug("****Ket thuc goi ProcessorSearch ****");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessChooseDonCu(List<V_HIS_EXP_MEST> listData)
        {
            try
            {
                lblTotalPrice.Text = "";
                lblPayPrice.Text = "";
                ReleaseAll();
                SetControlByExpMest(null);
                treeListMediMate.DataSource = null;
                dicMediMateAdo = new Dictionary<long, List<MediMateTypeADO>>();
                LoadPatientInfoFromdataExpMest(null);
                moduleAction = GlobalDataStore.ModuleAction.ADD;
                CommonParam param = new CommonParam();
                if (listData != null && listData.Count > 0)
                {
                    HIS_SERVICE_REQ serviceReq = null;
                    WaitingManager.Show();
                    if (listData.Exists(o => o.SERVICE_REQ_ID.HasValue))
                    {
                        HisServiceReqFilter filter = new HisServiceReqFilter();
                        filter.ID = listData.FirstOrDefault(o => o.SERVICE_REQ_ID.HasValue).SERVICE_REQ_ID;
                        var apiResult = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            serviceReq = apiResult.FirstOrDefault();
                        }
                    }
                    Dictionary<long, MediMateTypeADO> dicMediMateTempAdo = new Dictionary<long, MediMateTypeADO>();

                    List<long> expMestIds = listData.Select(s => s.ID).Distinct().ToList();
                    string clientSessionKey = Guid.NewGuid().ToString();
                    int skip = 0;
                    while (expMestIds.Count - skip > 0)
                    {
                        var listIds = expMestIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        #region Thuoc
                        HisExpMestMedicineView2Filter exmedifilter = new HisExpMestMedicineView2Filter();
                        exmedifilter.EXP_MEST_IDs = listIds;
                        var expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE_2>>("api/HisExpMestMedicine/GetView2", ApiConsumers.MosConsumer, exmedifilter, param);
                        if (expMestMedicines != null && expMestMedicines.Count > 0)
                        {
                            foreach (var medicine in expMestMedicines)
                            {
                                var expMest = listData.FirstOrDefault(o => o.ID == medicine.EXP_MEST_ID);

                                if (!medicine.TDL_MEDICINE_TYPE_ID.HasValue || expMest == null)
                                    continue;

                                HIS_SERVICE_REQ_METY adoNew = new HIS_SERVICE_REQ_METY();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ_METY>(adoNew, medicine);
                                adoNew.MEDICINE_TYPE_ID = medicine.TDL_MEDICINE_TYPE_ID;
                                adoNew.AMOUNT = medicine.AMOUNT;

                                MediMateTypeADO ado = null;
                                if (dicMediMateTempAdo.ContainsKey(adoNew.MEDICINE_TYPE_ID.Value))
                                {
                                    ado = dicMediMateTempAdo[adoNew.MEDICINE_TYPE_ID.Value];
                                    ado.EXP_AMOUNT += adoNew.AMOUNT;
                                    ado.TOTAL_PRICE = ado.EXP_AMOUNT * (ado.EXP_PRICE ?? 0);
                                }
                                else
                                {
                                    ado = new MediMateTypeADO(adoNew, expMest.TDL_INTRUCTION_TIME);
                                    dicMediMateTempAdo[adoNew.MEDICINE_TYPE_ID.Value] = ado;
                                }
                                ado.ClientSessionKey = clientSessionKey;
                                ado.SERVICE_REQ_ID = expMest.PRESCRIPTION_ID;
                                ado.SERVICE_REQ_CODE = !string.IsNullOrEmpty(expMest.TDL_SERVICE_REQ_CODE) ? expMest.TDL_SERVICE_REQ_CODE : "000000000000";
                                ado.PARENT_ID__IN_SETY = ado.SERVICE_REQ_CODE;
                                ado.CONCRETE_ID__IN_SETY = ado.SERVICE_REQ_CODE + "_" + adoNew.MEDICINE_TYPE_ID;
                                ado.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
                                ado.PATIENT_TYPE_ID = expMest.TDL_PATIENT_TYPE_ID;                               
                                ado.TDL_PATIENT_CODE = expMest.TDL_PATIENT_CODE;
                                ado.TDL_PATIENT_DOB = expMest.TDL_PATIENT_DOB ?? 0;
                                ado.TDL_PATIENT_GENDER_ID = expMest.TDL_PATIENT_GENDER_ID;
                                ado.TDL_PATIENT_GENDER_NAME = expMest.TDL_PATIENT_GENDER_NAME;
                                ado.TDL_PATIENT_ID = expMest.TDL_PATIENT_ID ?? 0;
                                ado.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = expMest.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                                ado.REQUEST_LOGINNAME = expMest.REQ_LOGINNAME;
                                ado.REQUEST_USERNAME = expMest.REQ_USERNAME;
                                ado.TDL_PATIENT_PHONE = expMest.TDL_PATIENT_PHONE;
                                HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == adoNew.MEDICINE_TYPE_ID);
                                if (mediInStockSDO != null)
                                {
                                    ado.NATIONAL_NAME = mediInStockSDO.NationalName;
                                    ado.MANUFACTURER_NAME = mediInStockSDO.ManufacturerName;
                                    ado.REGISTER_NUMBER = mediInStockSDO.RegisterNumber;
                                    ado.SERVICE_UNIT_NAME = mediInStockSDO.ServiceUnitName;
                                    ado.AVAILABLE_AMOUNT = mediInStockSDO.AvailableAmount;
                                    if (mediInStockSDO.AvailableAmount < ado.EXP_AMOUNT)
                                    {
                                        ado.IsExceedsAvailable = true;
                                    }
                                    ado.ACTIVE_INGR_BHYT_CODE = mediInStockSDO.ActiveIngrBhytCode;
                                    ado.ACTIVE_INGR_BHYT_NAME = mediInStockSDO.ActiveIngrBhytName;
                                    ado.CONCENTRA = mediInStockSDO.Concentra;
                                }
                                else
                                {
                                    ado.IsNotInStock = true;
                                }
                            }
                        }
                        #endregion

                        #region vat tu
                        HisExpMestMaterialView2Filter materialFiler = new HisExpMestMaterialView2Filter();
                        materialFiler.EXP_MEST_IDs = listIds;
                        var expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL_2>>("api/HisExpMestMaterial/GetView2", ApiConsumers.MosConsumer, materialFiler, param);
                        if (expMestMaterials != null && expMestMaterials.Count > 0)
                        {
                            foreach (var material in expMestMaterials)
                            {
                                var expMest = listData.FirstOrDefault(o => o.ID == material.EXP_MEST_ID);

                                if (!material.TDL_MATERIAL_TYPE_ID.HasValue || expMest == null)
                                    continue;

                                HIS_SERVICE_REQ_MATY adoNew = new HIS_SERVICE_REQ_MATY();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ_MATY>(adoNew, material);
                                adoNew.MATERIAL_TYPE_ID = material.TDL_MATERIAL_TYPE_ID;
                                adoNew.AMOUNT = material.AMOUNT;

                                MediMateTypeADO ado = null;
                                if (dicMediMateTempAdo.ContainsKey(adoNew.MATERIAL_TYPE_ID.Value))
                                {
                                    ado = dicMediMateTempAdo[adoNew.MATERIAL_TYPE_ID.Value];
                                    ado.EXP_AMOUNT += adoNew.AMOUNT;
                                    ado.TOTAL_PRICE = ado.EXP_AMOUNT * (ado.EXP_PRICE ?? 0);
                                }
                                else
                                {
                                    ado = new MediMateTypeADO(adoNew);
                                    dicMediMateTempAdo[adoNew.MATERIAL_TYPE_ID.Value] = ado;
                                }
                                ado.ClientSessionKey = clientSessionKey;
                                ado.SERVICE_REQ_ID = expMest.PRESCRIPTION_ID;
                                ado.SERVICE_REQ_CODE = !string.IsNullOrEmpty(expMest.TDL_SERVICE_REQ_CODE) ? expMest.TDL_SERVICE_REQ_CODE : "000000000000";
                                ado.PARENT_ID__IN_SETY = ado.SERVICE_REQ_CODE;
                                ado.CONCRETE_ID__IN_SETY = ado.SERVICE_REQ_CODE + "_" + adoNew.MATERIAL_TYPE_ID;
                                ado.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
                                ado.PATIENT_TYPE_ID = expMest.TDL_PATIENT_TYPE_ID;   
                                ado.TDL_PATIENT_CODE = expMest.TDL_PATIENT_CODE;
                                ado.TDL_PATIENT_DOB = expMest.TDL_PATIENT_DOB ?? 0;
                                ado.TDL_PATIENT_GENDER_ID = expMest.TDL_PATIENT_GENDER_ID;
                                ado.TDL_PATIENT_GENDER_NAME = expMest.TDL_PATIENT_GENDER_NAME;
                                ado.TDL_PATIENT_ID = expMest.TDL_PATIENT_ID ?? 0;
                                ado.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = expMest.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                                ado.REQUEST_LOGINNAME = expMest.REQ_LOGINNAME;
                                ado.REQUEST_USERNAME = expMest.REQ_USERNAME;
                                ado.TDL_PATIENT_PHONE = expMest.TDL_PATIENT_PHONE;
                                HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == adoNew.MATERIAL_TYPE_ID.Value);
                                if (mateInStockSDO != null)
                                {
                                    ado.NATIONAL_NAME = mateInStockSDO.NationalName;
                                    ado.MANUFACTURER_NAME = mateInStockSDO.ManufacturerName;
                                    ado.SERVICE_UNIT_NAME = mateInStockSDO.ServiceUnitName;
                                    ado.AVAILABLE_AMOUNT = mateInStockSDO.AvailableAmount;
                                    if (mateInStockSDO.AvailableAmount < adoNew.AMOUNT)
                                    {
                                        ado.IsExceedsAvailable = true;
                                    }
                                }
                                else
                                {
                                    ado.IsNotInStock = true;
                                }
                            }
                        }
                        #endregion
                    }

                    if (dicMediMateTempAdo != null && dicMediMateTempAdo.Count > 0)
                    {
                        IsDonCu = true;
                        //Take bean
                        TakeBeanMedicineAll(dicMediMateTempAdo, clientSessionKey);
                        TakeBeanMaterialAll(dicMediMateTempAdo, clientSessionKey);
                    }

                    List<MediMateTypeADO> dataSources = dicMediMateAdo.Select(p => p.Value).SelectMany(p => p).Distinct().ToList();
                    if (dataSources != null && dataSources.Count > 0)
                    {
                        
                        ProcessSetDataTrees(dataSources);
                        SetTotalPriceExpMestDetail();
                    }

                    if (serviceReq != null)
                    {
                        LoadPatientInfoFromdataServiceReq(serviceReq);
                    }
                    else
                    {
                        LoadPatientInfoFromdataExpMest(listData[0]);
                    }

                    SetLabelSave(this.moduleAction);
                    WaitingManager.Hide();
                    this.SetEnableButtonDebt(this.moduleAction == GlobalDataStore.ModuleAction.ADD);
                    if (chkAutoShow.Checked)
                    {
                        repositoryItemBtnView_ButtonClick(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatientInfoFromdataExpMest(V_HIS_EXP_MEST _expMest)
        {
            try
            {
                if (_expMest != null)
                {
                    txtTreatmentCode.Text = _expMest.TDL_TREATMENT_CODE;
                    txtPatientCode.Text = _expMest.TDL_PATIENT_CODE;
                    txtPatientPhone.Text = _expMest.TDL_PATIENT_PHONE;
                    txtVirPatientName.Text = _expMest.TDL_PATIENT_NAME;
                    txtAddress.Text = _expMest.TDL_PATIENT_ADDRESS;
                    cboGender.EditValue = _expMest.TDL_PATIENT_GENDER_ID;
                    if (_expMest.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtPatientDob.Text = _expMest.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_expMest.TDL_PATIENT_DOB ?? 0);
                    }

                    if (_expMest.TDL_PATIENT_ID > 0 && this.Patient == null)
                    {
                        this.Patient = ProcessGetPatientById(_expMest.TDL_PATIENT_ID ?? 0);
                    }

                    txtLoginName.Text = _expMest.REQ_LOGINNAME;
                    txtPresUser.Text = _expMest.REQ_USERNAME;
                    CalulatePatientAge(this.txtPatientDob.Text);
                    if (_expMest.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtPatientDob.Text = _expMest.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_expMest.TDL_PATIENT_DOB ?? 0);
                    }
                    UpdateTHXControl(_expMest.TDL_PATIENT_PROVINCE_CODE, _expMest.TDL_PATIENT_DISTRICT_CODE, _expMest.TDL_PATIENT_COMMUNE_CODE);
                }
                else
                {
                    txtPatientCode.Text = "";
                    txtPatientPhone.Text = "";
                    txtVirPatientName.Text = "";
                    txtAddress.Text = "";
                    cboGender.EditValue = null;
                    txtPatientDob.EditValue = null;
                    txtLoginName.Text = "";
                    txtPresUser.Text = null;
                    this.txtAge.EditValue = "";
                    this.cboAge.EditValue = null;
                    txtMaTHX.Text = "";
                    cboTHX.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientInfoFromdataServiceReq(HIS_SERVICE_REQ _serviceReq)
        {
            try
            {
                if (_serviceReq != null)
                {
                    if (this.serviceReq == null)
                    {
                        this.serviceReq = new List<V_HIS_SERVICE_REQ_11>();
                        V_HIS_SERVICE_REQ_11 addData = new V_HIS_SERVICE_REQ_11();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ_11>(addData, _serviceReq);
                        this.serviceReq.Add(addData);
                    }

                    if (_serviceReq.TDL_PATIENT_ID > 0 && this.Patient == null)
                    {
                        this.Patient = ProcessGetPatientById(_serviceReq.TDL_PATIENT_ID);
                    }

                    //this.serviceReq = _serviceReq;
                    txtTreatmentCode.Text = _serviceReq.TDL_TREATMENT_CODE;
                    txtPatientCode.Text = _serviceReq.TDL_PATIENT_CODE;
                    txtPatientPhone.Text = _serviceReq.TDL_PATIENT_PHONE;
                    txtVirPatientName.Text = _serviceReq.TDL_PATIENT_NAME;
                    txtAddress.Text = _serviceReq.TDL_PATIENT_ADDRESS;
                    cboGender.EditValue = _serviceReq.TDL_PATIENT_GENDER_ID;
                    if (_serviceReq.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtPatientDob.Text = _serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_serviceReq.TDL_PATIENT_DOB);
                    }

                    CalulatePatientAge(this.txtPatientDob.Text);
                    if (_serviceReq.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtPatientDob.Text = _serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_serviceReq.TDL_PATIENT_DOB);
                    }
                    UpdateTHXControl(_serviceReq.TDL_PATIENT_PROVINCE_CODE, _serviceReq.TDL_PATIENT_DISTRICT_CODE, _serviceReq.TDL_PATIENT_COMMUNE_CODE);

                    txtLoginName.Text = _serviceReq.REQUEST_LOGINNAME;
                    txtPresUser.Text = _serviceReq.REQUEST_USERNAME;
                }
                else
                {
                    txtPatientCode.Text = "";
                    txtPatientPhone.Text = "";
                    this.txtAge.Text = "";
                    this.cboAge.EditValue = null;
                    txtVirPatientName.Text = "";
                    txtAddress.Text = "";
                    cboGender.EditValue = null;
                    txtPatientDob.EditValue = null;
                    txtLoginName.Text = "";
                    txtPresUser.Text = null;
                    txtMaTHX.Text = "";
                    cboTHX.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateTHXControl(string Province_Code, string District_Code, string Commune_Code)
        {
            try
            {
                var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == Province_Code);

                var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => o.PROVINCE_CODE == Province_Code && o.DISTRICT_CODE == District_Code);

                var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o =>
                o.COMMUNE_CODE == Commune_Code
                && o.DISTRICT_CODE == District_Code);
                if (commune != null)
                {
                    this.cboTHX.EditValue = commune.ID;
                    this.txtMaTHX.Text = commune.SEARCH_CODE + district.SEARCH_CODE + province.SEARCH_CODE;
                }
                else if (Province_Code != null && District_Code != null)
                {
                    if (String.IsNullOrEmpty(Commune_Code))
                    {
                        var dist = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o =>
                (o.DISTRICT_CODE == District_Code && o.PROVINCE_CODE == Province_Code));

                        if (dist != null)
                        {
                            var com = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().FirstOrDefault(o =>
                    o.ID == -dist.ID);
                            if (com != null)
                            {
                                this.cboTHX.EditValue = com.ID;
                                this.txtMaTHX.Text = com.SEARCH_CODE_COMMUNE;
                            }
                        }
                    }
                }
                else if (Commune_Code != null && District_Code != null)
                {
                    var communeTHX = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().FirstOrDefault(o =>
                    (o.SEARCH_CODE_COMMUNE) == (province.SEARCH_CODE + district.SEARCH_CODE)
                    && o.ID < 0);
                    if (communeTHX != null)
                    {
                        this.cboTHX.EditValue = communeTHX.ID;
                        this.txtMaTHX.Text = communeTHX.SEARCH_CODE_COMMUNE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSetDataTrees(List<MediMateTypeADO> dicMediMateAdosss)
        {
            try
            {
                if (dicMediMateAdosss != null && dicMediMateAdosss.Count > 0)
                {
                    var dataGroups = dicMediMateAdosss.GroupBy(p => p.SERVICE_REQ_CODE).Select(p => p.ToList()).ToList();

                    foreach (var items in dataGroups)
                    {
                        string strParent = "";
                        strParent += " - " + items[0].TDL_PATIENT_NAME;

                        if (items[0].SERVICE_REQ_ID > 0 && !string.IsNullOrEmpty(items[0].SERVICE_REQ_CODE) && items[0].SERVICE_REQ_CODE != "000000000000")
                        {
                            strParent += "( Mã y lệnh - " + items[0].SERVICE_REQ_CODE + ")";
                        }

                        if (!string.IsNullOrEmpty(items[0].EXP_MEST_CODE))
                            strParent = "Mã phiếu xuất - " + items[0].EXP_MEST_CODE;
                       

                       // strParent += " - " + items[0].TDL_PATIENT_NAME;

                        MediMateTypeADO ado = new MediMateTypeADO();
                        ado.ClientSessionKey = items[0].ClientSessionKey;
                        ado.CONCRETE_ID__IN_SETY = items[0].SERVICE_REQ_CODE;
                        ado.MEDI_MATE_TYPE_NAME = items[0].SERVICE_REQ_CODE == "000000000000" ? "Thêm mới" : strParent;
                        ado.SERVICE_REQ_CODE = items[0].SERVICE_REQ_CODE;
                        ado.EXP_MEST_ID = items[0].EXP_MEST_ID;
                        ado.TDL_PATIENT_NAME = items[0].TDL_PATIENT_NAME;
                        dicMediMateAdosss.Add(ado);
                    }
                }

                dicMediMateAdosss = dicMediMateAdosss.OrderByDescending(o => o.SERVICE_REQ_CODE).ThenBy(p => p.NUM_ORDER ?? 9999).ThenBy(o => o.MEDI_MATE_TYPE_NAME).ToList(); 
                if (this.currentMediMateFocus == null)
                {
                    this.currentMediMateFocus = dicMediMateAdosss[0];
                }
                BindingList<MediMateTypeADO> records = new BindingList<MediMateTypeADO>(dicMediMateAdosss);
                treeListMediMate.DataSource = records;
                treeListMediMate.ExpandAll();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => records), records));
                if (records != null && records.Count > 0 && records.Where(o=>o.KEY_PRICE_PARENT).ToList().Count > 1)
                {
                    layoutControlItem48.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    IsShowDetails = true;
                    
                }else{
                     layoutControlItem48.Visibility =   DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                     IsShowDetails = false;
                }
                layoutControlItem47.Visibility = (chkRoundPrice.Checked && chkCreateBill.Checked && IsShowDetails) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestBySearch(List<V_HIS_EXP_MEST> _expMest, bool _is, HisExpMestForSaleSDO expMestForSaleSDO)
        {
            try
            {
                if (_expMest != null && _expMest.Count > 0)
                {
                    List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = null;
                    List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = null;
                    List<long> expMestIds = _expMest.Select(p => p.ID).ToList();
                    //List<Action> methods = new List<Action>();
                    //methods.Add(() => { expMestMedicines = GetExpMestMedicineByExpMests(expMestIds); });
                    //methods.Add(() => { expMestMaterials = GetExpMestMaterialByExpMests(expMestIds); });
                    //ThreadCustomManager.MultipleThreadWithJoin(methods);
                    //expMestMedicines = GetExpMestMedicineByExpMests(expMestIds);
                    //expMestMaterials = GetExpMestMaterialByExpMests(expMestIds);
                    expMestMedicines = expMestForSaleSDO.ViewMedicines;
                    expMestMaterials = expMestForSaleSDO.ViewMaterials;

                    LoadMediMateBeanByExpMestList(_expMest, expMestMedicines, expMestMaterials, expMestForSaleSDO.MedicineBeans, expMestForSaleSDO.MaterialBeans);
                    //expMestDones_.ID = _expMest[0].ID;
                    LoadDataFromExpMest(_expMest[0], _is);

                    moduleAction = GlobalDataStore.ModuleAction.EDIT;

                    SetLabelSave(this.moduleAction);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_EXP_MEST_MEDICINE> GetExpMestMedicineByExpMests(List<long> _expMestIds)
        {
            List<V_HIS_EXP_MEST_MEDICINE> result = null;
            try
            {
                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_IDs = _expMestIds;
                result = new BackendAdapter(new CommonParam())
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<V_HIS_EXP_MEST_MATERIAL> GetExpMestMaterialByExpMests(List<long> _expMestIds)
        {
            List<V_HIS_EXP_MEST_MATERIAL> result = null;
            try
            {
                HisExpMestMaterialViewFilter medicineFilter = new HisExpMestMaterialViewFilter();
                medicineFilter.EXP_MEST_IDs = _expMestIds;
                result = new BackendAdapter(new CommonParam())
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadMediMateBeanByExpMestList(List<V_HIS_EXP_MEST> expMests, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials, List<HIS_MEDICINE_BEAN> medicineBeanAlls, List<HIS_MATERIAL_BEAN> materialBeanAlls)
        {
            try
            {
                List<MediMateTypeADO> mediMateTypeADOs = new List<MediMateTypeADO>();

                List<HIS_MEDICINE_BEAN> medicineBeans = new List<HIS_MEDICINE_BEAN>();
                if (expMestMedicines != null && expMestMedicines.Count > 0 && medicineBeanAlls != null && medicineBeanAlls.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    expMestMedicineIds = expMestMedicines.Select(o => o.ID).ToList();
                    HisMedicineBeanFilter medicineBeanFilter = new HisMedicineBeanFilter();
                    medicineBeanFilter.EXP_MEST_MEDICINE_IDs = expMestMedicineIds;
                    //    medicineBeans = new BackendAdapter(param)
                    //.Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_BEAN>>(RequestUriStore.HIS_MEDICINE_BEAN__GET, ApiConsumers.MosConsumer, medicineBeanFilter, param);

                    medicineBeans = medicineBeanAlls.Where(o => expMestMedicineIds.Contains(o.EXP_MEST_MEDICINE_ID ?? -1)).ToList();

                }

                List<HIS_MATERIAL_BEAN> materialBeans = new List<HIS_MATERIAL_BEAN>();
                if (expMestMaterials != null && expMestMaterials.Count > 0 && materialBeanAlls != null && materialBeanAlls.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    expMestMaterialIds = expMestMaterials.Select(o => o.ID).ToList();
                    //HisMaterialBeanFilter materialBeanFilter = new HisMaterialBeanFilter();
                    //materialBeanFilter.EXP_MEST_MATERIAL_IDs = expMestMaterialIds;
                    //    materialBeans = new BackendAdapter(param)
                    //.Get<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_BEAN>>(RequestUriStore.HIS_MATERIAL_BEAN__GET, ApiConsumers.MosConsumer, materialBeanFilter, param);
                    materialBeans = materialBeanAlls.Where(o => expMestMaterialIds.Contains(o.EXP_MEST_MATERIAL_ID ?? -1)).ToList();
                }

                foreach (var item in expMests)
                {
                    var dataMedis = expMestMedicines.Where(p => p.EXP_MEST_ID == item.ID).ToList();

                    var dataGroups = dataMedis.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.PRICE, p.VAT_RATIO }).Select(p => p.ToList()).ToList();
                    foreach (var itemGr in dataGroups)
                    {
                        MediMateTypeADO adoN = new MediMateTypeADO(itemGr[0], item, medicineBeans);
                        adoN.EXP_AMOUNT = itemGr.Sum(p => p.AMOUNT);
                        adoN.OLD_AMOUNT = itemGr.Sum(p => p.AMOUNT);
                        adoN.TOTAL_PRICE = itemGr.Sum(p => p.AMOUNT * p.PRICE * (1 + (p.VAT_RATIO ?? 0)) * (1 - (p.DISCOUNT ?? 0)));
                        adoN.BeanIds = medicineBeans != null && medicineBeans.Count > 0 ? medicineBeans.Where(o => itemGr.Select(s => s.ID).Contains(o.EXP_MEST_MEDICINE_ID ?? 0)).Select(s => s.ID).ToList() : null;
                        mediMateTypeADOs.Add(adoN);
                    }
                    // mediMateTypeADOs.AddRange(from r in dataMedis select new MediMateTypeADO(r, item, medicineBeans));

                    var dataMates = expMestMaterials.Where(p => p.EXP_MEST_ID == item.ID).ToList();
                    var dataGroupMates = dataMates.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.PRICE, p.VAT_RATIO }).Select(p => p.ToList()).ToList();
                    foreach (var itemGr in dataGroupMates)
                    {
                        MediMateTypeADO adoN = new MediMateTypeADO(itemGr[0], item, materialBeans);
                        adoN.EXP_AMOUNT = itemGr.Sum(p => p.AMOUNT);
                        adoN.OLD_AMOUNT = itemGr.Sum(p => p.AMOUNT);
                        adoN.TOTAL_PRICE = itemGr.Sum(p => p.AMOUNT * p.PRICE * (1 + (p.VAT_RATIO ?? 0)) * (1 - (p.DISCOUNT ?? 0)));
                        adoN.BeanIds = materialBeans != null && materialBeans.Count > 0 ? materialBeans.Where(o => itemGr.Select(s => s.ID).Contains(o.EXP_MEST_MATERIAL_ID ?? 0)).Select(s => s.ID).ToList() : null;
                        mediMateTypeADOs.Add(adoN);
                    }
                    // mediMateTypeADOs.AddRange(from r in dataMates select new MediMateTypeADO(r, item, materialBeans));
                }

                if (mediMateTypeADOs != null && mediMateTypeADOs.Count > 0)
                {
                    var dicMediMateAdoTmp = dicMediMateAdo;
                    dicMediMateAdo = new Dictionary<long, List<MediMateTypeADO>>();
                    foreach (var mediMateTypeADO in mediMateTypeADOs)
                    {
                        if (mediMateTypeADO.IsMedicine)
                        {
                            HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == mediMateTypeADO.MEDI_MATE_TYPE_ID);
                            if (mediInStockSDO != null)
                            {
                                mediMateTypeADO.AVAILABLE_AMOUNT = mediInStockSDO.AvailableAmount;
                                if (spinAmount.Value > mediInStockSDO.AvailableAmount)
                                {
                                    mediMateTypeADO.IsExceedsAvailable = true;
                                }
                            }
                            else
                            {
                                if (!mediMateTypeADO.OLD_AMOUNT.HasValue)
                                    mediMateTypeADO.IsNotInStock = true;
                            }
                        }
                        if (mediMateTypeADO.IsMaterial)
                        {
                            HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == mediMateTypeADO.MEDI_MATE_TYPE_ID && !mediMateTypeADO.OLD_AMOUNT.HasValue);
                            if (mateInStockSDO != null)
                            {
                                mediMateTypeADO.AVAILABLE_AMOUNT = mateInStockSDO.AvailableAmount;
                                if (spinAmount.Value > mateInStockSDO.AvailableAmount)
                                {
                                    mediMateTypeADO.IsExceedsAvailable = true;
                                }
                            }
                            else
                            {
                                if (!mediMateTypeADO.OLD_AMOUNT.HasValue)
                                    mediMateTypeADO.IsNotInStock = true;
                            }
                        }
                        if (dicMediMateAdo.ContainsKey(mediMateTypeADO.MEDI_MATE_TYPE_ID))
                            dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID].Add(mediMateTypeADO);
                        else
                        {
                            dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID] = new List<MediMateTypeADO>();
                            if (dicMediMateAdoTmp != null && dicMediMateAdoTmp.Count > 0 && dicMediMateAdoTmp.ContainsKey(mediMateTypeADO.MEDI_MATE_TYPE_ID))
                                mediMateTypeADO.ClientSessionKey = dicMediMateAdoTmp[mediMateTypeADO.MEDI_MATE_TYPE_ID][0].ClientSessionKey;
                            else
                                mediMateTypeADO.ClientSessionKey = clientSessionKey;
                            dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID].Add(mediMateTypeADO);
                        }
                    }
                }
                LoadDataToGridExpMestDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ReloadDataDicBeforSave()
        {
            try
            {
                if (this.resultSDO.ExpMestSdos != null && this.resultSDO.ExpMestSdos.Count > 0)
                {
                    List<MediMateTypeADO> mediMateTypeADOs = new List<MediMateTypeADO>();
                    foreach (var item in this.resultSDO.ExpMestSdos)
                    {
                        if (item.ExpMedicines != null && item.ExpMedicines.Count > 0)
                        {
                            CommonParam param = new CommonParam();
                            HisMedicineBeanFilter medicineBeanFilter = new HisMedicineBeanFilter();
                            medicineBeanFilter.EXP_MEST_MEDICINE_IDs = item.ExpMedicines.Select(o => o.ID).ToList();
                            var medicineBeans = new BackendAdapter(param)
                          .Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_BEAN>>(RequestUriStore.HIS_MEDICINE_BEAN__GET, ApiConsumers.MosConsumer, medicineBeanFilter, param);
                            mediMateTypeADOs.AddRange(from r in item.ExpMedicines select new MediMateTypeADO(r, item.ExpMest, medicineBeans, BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>()));
                        }
                        if (item.ExpMaterials != null && item.ExpMaterials.Count > 0)
                        {
                            CommonParam param = new CommonParam();
                            HisMaterialBeanFilter materialBeanFilter = new HisMaterialBeanFilter();
                            materialBeanFilter.EXP_MEST_MATERIAL_IDs = item.ExpMaterials.Select(o => o.ID).ToList();
                            var materialBeans = new BackendAdapter(param)
                          .Get<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_BEAN>>(RequestUriStore.HIS_MATERIAL_BEAN__GET, ApiConsumers.MosConsumer, materialBeanFilter, param);
                            mediMateTypeADOs.AddRange(from r in item.ExpMaterials select new MediMateTypeADO(r, item.ExpMest, materialBeans, BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>()));
                        }
                    }

                    if (mediMateTypeADOs != null && mediMateTypeADOs.Count > 0)
                    {
                        var dicMediMateAdoTmp = dicMediMateAdo;
                        dicMediMateAdo = new Dictionary<long, List<MediMateTypeADO>>();
                        foreach (var mediMateTypeADO in mediMateTypeADOs)
                        {
                            mediMateTypeADO.AVAILABLE_AMOUNT = (mediMateTypeADO.OLD_AMOUNT ?? 0);
                            if (mediMateTypeADO.IsMedicine)
                            {
                                HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == mediMateTypeADO.MEDI_MATE_TYPE_ID);
                                if (mediInStockSDO != null)
                                {
                                    mediMateTypeADO.AVAILABLE_AMOUNT += (mediInStockSDO.AvailableAmount ?? 0);
                                    if (mediMateTypeADO.EXP_AMOUNT > mediMateTypeADO.AVAILABLE_AMOUNT)// if (spinAmount.Value > mediInStockSDO.AvailableAmount)
                                    {
                                        mediMateTypeADO.IsExceedsAvailable = true;
                                    }
                                }
                                else
                                {
                                    if (!mediMateTypeADO.OLD_AMOUNT.HasValue)
                                        mediMateTypeADO.IsNotInStock = true;
                                }
                            }
                            if (mediMateTypeADO.IsMaterial)
                            {
                                HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == mediMateTypeADO.MEDI_MATE_TYPE_ID && !mediMateTypeADO.OLD_AMOUNT.HasValue);
                                if (mateInStockSDO != null)
                                {
                                    mediMateTypeADO.AVAILABLE_AMOUNT += (mateInStockSDO.AvailableAmount ?? 0);
                                    if (mediMateTypeADO.EXP_AMOUNT > mediMateTypeADO.AVAILABLE_AMOUNT)
                                    {
                                        mediMateTypeADO.IsExceedsAvailable = true;
                                    }
                                }
                                else
                                {
                                    if (!mediMateTypeADO.OLD_AMOUNT.HasValue)
                                        mediMateTypeADO.IsNotInStock = true;
                                }
                            }
                            if (dicMediMateAdo.ContainsKey(mediMateTypeADO.MEDI_MATE_TYPE_ID))
                                dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID].Add(mediMateTypeADO);
                            else
                            {
                                dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID] = new List<MediMateTypeADO>();
                                if (dicMediMateAdoTmp != null && dicMediMateAdoTmp.Count > 0 && dicMediMateAdoTmp.ContainsKey(mediMateTypeADO.MEDI_MATE_TYPE_ID))
                                    mediMateTypeADO.ClientSessionKey = dicMediMateAdoTmp[mediMateTypeADO.MEDI_MATE_TYPE_ID][0].ClientSessionKey;
                                else
                                    mediMateTypeADO.ClientSessionKey = clientSessionKey;
                                dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID].Add(mediMateTypeADO);
                            }
                        }

                    }
                    LoadDataToGridExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void GetTransactionById(List<long> transactionIds)
        {
            try
            {
                if (transactionIds != null && transactionIds.Count > 0)
                {
                    transactionIds = transactionIds.Distinct().ToList();
                    CommonParam param = new CommonParam();
                    HisTransactionFilter filter = new HisTransactionFilter();
                    filter.IDs = transactionIds;
                    filter.IS_CANCEL = false;

                    var transaction = new BackendAdapter(param).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, filter, param);
                    if (transaction != null && transaction.Count > 0)
                    {
                        lblTransactionCode.Text = string.Join(",", transaction.Select(s => s.TRANSACTION_CODE).OrderBy(o => o));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
