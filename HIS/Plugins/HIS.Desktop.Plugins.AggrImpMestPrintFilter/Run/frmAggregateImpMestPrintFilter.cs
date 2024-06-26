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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using MPS.Processor.Mps000078.PDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrImpMestPrintFilter
{
    internal partial class frmAggregateImpMestPrintFilter : HIS.Desktop.Utility.FormBase
    {
        #region varialbe declare
        List<V_HIS_ROOM> RoomDTO2s = new List<V_HIS_ROOM>();
        List<V_HIS_ROOM> RoomDTO3s = new List<V_HIS_ROOM>();
        List<HIS_SERVICE_UNIT> ServiceUnits = new List<HIS_SERVICE_UNIT>();
        List<HIS_MEDICINE_USE_FORM> MedicineUseForms = new List<HIS_MEDICINE_USE_FORM>();
        V_HIS_IMP_MEST aggrImpMest;
        List<MPS.ADO.ImpMestMedicinePrintADO> ImpMestManuMedicines = new List<MPS.ADO.ImpMestMedicinePrintADO>();
        HIS_DEPARTMENT department;
        internal List<long> serviceUnitIds = new List<long>();
        internal List<long> reqRoomIds = new List<long>();
        internal List<long> _useFormIds = new List<long>();

        Inventec.Desktop.Common.Modules.Module currentModule;
        long printType;
        List<HIS_IMP_MEST> _MobaImpMests = new List<HIS_IMP_MEST>();
        List<HIS_EXP_MEST> _MobaExpMests = new List<HIS_EXP_MEST>();

        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_GN_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_GNs = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_Ts = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_TDs = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_PXs = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_Others = new List<V_HIS_IMP_MEST_MEDICINE>();

        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines { get; set; }
        List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterials { get; set; }

        #endregion

        #region contructor
        internal frmAggregateImpMestPrintFilter(Inventec.Desktop.Common.Modules.Module currentModule, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST aggrImpMest, long printType)
            : base(currentModule)
        {
            try
            {
                InitializeComponent();
                this.currentModule = currentModule;
                this.aggrImpMest = aggrImpMest;
                this.printType = printType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Load
        private void frmRequestPatientReport_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                SetCaptionByLanguageKey();
                InitControl();
                FillDataToForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToForm()
        {
            try
            {
                _ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                _ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                RoomDTO2s.Clear();
                grdRoom.DataSource = null;
                gridViewServiceUnit.GridControl.DataSource = null;
                WaitingManager.Show();
                //Review
                LoadDataMedicineAndMaterial(this.aggrImpMest);

                if (reqRoomIds != null && reqRoomIds.Count > 0)
                {
                    RoomDTO2s = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.DEPARTMENT_ID == WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId && reqRoomIds.Contains(o.ID)).ToList();
                    grdRoom.DataSource = RoomDTO2s;
                }
                if (serviceUnitIds != null && serviceUnitIds.Count > 0)
                {
                    serviceUnitIds = serviceUnitIds.Distinct().ToList();
                    var serviceUnits = BackendDataWorker.Get<HIS_SERVICE_UNIT>().Where(o => serviceUnitIds.Contains(o.ID)).ToList();
                    gridViewServiceUnit.BeginUpdate();
                    gridViewServiceUnit.GridControl.DataSource = serviceUnits;
                    gridViewServiceUnit.SelectAll();
                    gridViewServiceUnit.EndUpdate();
                }
                if (this._useFormIds != null && this._useFormIds.Count > 0)
                {
                    this._useFormIds = this._useFormIds.Distinct().ToList();
                    LoadDataToGridControlUseForm(this._useFormIds);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridControlServiceUnit(List<long> serviceUnitIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                //MOS.Filter.HisServiceUnitFilter serviceUnitFilter = new HisServiceUnitFilter();
                //serviceUnitFilter.IDs = serviceUnitIds;
                //var serviceUnits = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>>(HisRequestUriStore.HIS_SERVICE_UNIT_GET, ApiConsumers.MosConsumer, serviceUnitFilter, param);
                serviceUnitIds = serviceUnitIds.Distinct().ToList();
                var serviceUnits = BackendDataWorker.Get<HIS_SERVICE_UNIT>().Where(o => serviceUnitIds.Contains(o.ID)).ToList();
                gridViewServiceUnit.BeginUpdate();
                gridViewServiceUnit.GridControl.DataSource = serviceUnits;
                gridViewServiceUnit.SelectAll();
                gridViewServiceUnit.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridControlUseForm(List<long> useFormIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineUseFormFilter medicineUseFormFilter = new HisMedicineUseFormFilter();
                medicineUseFormFilter.IDs = useFormIds;
                var medicineUseForms = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>>(HisRequestUriStore.HIS_MEDICINE_USE_FORM_GET, ApiConsumers.MosConsumer, medicineUseFormFilter, param);
                gridViewUseForm.BeginUpdate();
                gridViewUseForm.GridControl.DataSource = medicineUseForms;
                gridViewUseForm.SelectAll();
                gridViewUseForm.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Events control
        private void InitControl()
        {
            try
            {
                chkMedicine.Checked = true;
                chkMaterial.Checked = true;

                switch (this.printType)
                {
                    case 1:
                        this.Text = "Điều kiện lọc - In phiếu tra đổi";

                        lciChkMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciChkMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;
                    case 2:
                        this.Text = "Điều kiện lọc - In phiếu tổng hợp";

                        lciChkMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciChkMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;
                    case 3:
                        this.Text = "Phiếu trả thuốc gây nghiện, hướng TT";

                        lciChkMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciChkMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        break;
                    case 4:
                        this.Text = "Phiếu trả thuốc chi tiết";

                        lciChkMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciChkMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSendRequest_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSendRequest_Click(object sender, EventArgs e)
        {
            try
            {
                //CallThreadPool();
                ProcessPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView3_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                RoomDTO3s.Clear();
                int[] rows = gridViewRoom.GetSelectedRows();
                for (int i = 0; i < rows.Length; i++)
                {
                    RoomDTO3s.Add((MOS.EFMODEL.DataModels.V_HIS_ROOM)gridViewRoom.GetRow(rows[i]));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridViewServiceUnit_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                ServiceUnits.Clear();
                int[] rows = gridViewServiceUnit.GetSelectedRows();
                for (int i = 0; i < rows.Length; i++)
                {
                    ServiceUnits.Add((MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT)gridViewServiceUnit.GetRow(rows[i]));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewUseForm_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

            try
            {
                MedicineUseForms.Clear();
                int[] rows = gridViewUseForm.GetSelectedRows();
                for (int i = 0; i < rows.Length; i++)
                {
                    MedicineUseForms.Add((MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM)gridViewUseForm.GetRow(rows[i]));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddListMedicineAndMaterialCommon(ref List<MPS.ADO.ImpMestMedicinePrintADO> lstImpMestManuMedicine, long aggrImpMestId)
        {
            try
            {
                //Thuoc
                CommonParam param = new CommonParam();
                HisImpMestMedicineViewFilter impMestMedicineViewFilter = new HisImpMestMedicineViewFilter();
                impMestMedicineViewFilter.AGGR_IMP_MEST_ID = aggrImpMestId;

                //Review
                // impMestMedicineViewFilter.IMP_MEST_TYPE_ID = HisImpMestTypeCFG.HisImpMestTypeId__Moba;
                List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE> lstImpMestMedicine = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);
                if (lstImpMestMedicine != null && lstImpMestMedicine.Count > 0)
                {
                    lstImpMestMedicine = lstImpMestMedicine.OrderByDescending(o => o.NUM_ORDER).ToList();
                    foreach (var item in lstImpMestMedicine)
                    {
                        MPS.ADO.ImpMestMedicinePrintADO impMestMedicineSdo = new MPS.ADO.ImpMestMedicinePrintADO();
                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE, MPS.ADO.ImpMestMedicinePrintADO>();
                        impMestMedicineSdo = AutoMapper.Mapper.Map<MPS.ADO.ImpMestMedicinePrintADO>(item);

                        impMestMedicineSdo.IS_MEDICINE = true;

                        #region Lấy ID phòng chỉ định
                        //Review
                        //HisMobaImpMestFilter mobaImpMestViewfilter = new HisMobaImpMestFilter();
                        //mobaImpMestViewfilter.IMP_MEST_ID = item.IMP_MEST_ID;
                        //var mobaImpMest = new BackendAdapter(param).Get<List<HIS_MOBA_IMP_MEST>>("api/HisMoBaImpMest/Get", ApiConsumers.MosConsumer, mobaImpMestViewfilter, param).FirstOrDefault();
                        //if (mobaImpMest != null)
                        //{
                        //    HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                        //    expMestFilter.ID = mobaImpMest.EXP_MEST_ID;
                        //    var currentExpMest = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expMestFilter, param).FirstOrDefault();
                        //    if (currentExpMest != null)
                        //    {
                        //        HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                        //        serviceReqFilter.ID = currentExpMest.SERVICE_REQ_ID;
                        //        var currentServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();
                        //        //TODO
                        //        if (currentServiceReq != null)
                        //        {
                        //            impMestMedicineSdo.ROOM_ASSIGN_ID = currentServiceReq.REQUEST_ROOM_ID;
                        //            impMestMedicineSdo.INTRUCTION_TIME = currentServiceReq.INTRUCTION_TIME;
                        //        }
                        //    }
                        //}
                        #endregion

                        lstImpMestManuMedicine.Add(impMestMedicineSdo);

                    }
                }

                //Vat tu
                HisImpMestMaterialViewFilter impMestMaterialViewFilter = new HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.AGGR_IMP_MEST_ID = aggrImpMestId;
                List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MATERIAL> lstImpMestMaterial = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);
                if (lstImpMestMaterial != null && lstImpMestMaterial.Count > 0)
                {
                    lstImpMestMaterial = lstImpMestMaterial.OrderByDescending(o => o.NUM_ORDER).ToList();

                    foreach (var item_impMaterial in lstImpMestMaterial)
                    {
                        MPS.ADO.ImpMestMedicinePrintADO impMestMaterial = new MPS.ADO.ImpMestMedicinePrintADO();
                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MATERIAL, MPS.ADO.ImpMestMedicinePrintADO>();
                        impMestMaterial = AutoMapper.Mapper.Map<MPS.ADO.ImpMestMedicinePrintADO>(item_impMaterial);

                        impMestMaterial.MEDICINE_TYPE_ID = item_impMaterial.MATERIAL_TYPE_ID;
                        impMestMaterial.MEDICINE_TYPE_CODE = item_impMaterial.MATERIAL_TYPE_CODE;
                        impMestMaterial.MEDICINE_TYPE_NAME = item_impMaterial.MATERIAL_TYPE_NAME;
                        impMestMaterial.MEDICINE_ID = item_impMaterial.MATERIAL_ID;
                        // impMestMaterial.MEDICINE_BEAN_ID = item_impMaterial.MATERIAL_BEAN_ID;
                        impMestMaterial.IMP_MEST_ID = item_impMaterial.IMP_MEST_ID;
                        impMestMaterial.ID = item_impMaterial.ID;

                        impMestMaterial.IS_MEDICINE = false;

                        #region Lấy ID phòng chỉ định
                        //Review
                        //HisMobaImpMestFilter mobaImpMestViewfilter = new HisMobaImpMestFilter();
                        //mobaImpMestViewfilter.IMP_MEST_ID = item_impMaterial.IMP_MEST_ID;
                        //var mobaImpMest = new BackendAdapter(param).Get<List<HIS_MOBA_IMP_MEST>>("api/HisMoBaImpMest/Get", ApiConsumers.MosConsumer, mobaImpMestViewfilter, param).FirstOrDefault();
                        //if (mobaImpMest != null)
                        //{
                        //    HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                        //    expMestFilter.ID = mobaImpMest.EXP_MEST_ID;
                        //    var currentExpMest = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expMestFilter, param).FirstOrDefault();
                        //    if (currentExpMest != null)
                        //    {
                        //        HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                        //        serviceReqFilter.ID = currentExpMest.SERVICE_REQ_ID;
                        //        var currentServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();
                        //        //TODO
                        //        if (currentServiceReq != null)
                        //        {
                        //            impMestMaterial.ROOM_ASSIGN_ID = currentServiceReq.REQUEST_ROOM_ID;
                        //            impMestMaterial.INTRUCTION_TIME = currentServiceReq.INTRUCTION_TIME;
                        //        }
                        //    }
                        //}
                        #endregion

                        lstImpMestManuMedicine.Add(impMestMaterial);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSendRequest_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicineAndMaterial(V_HIS_IMP_MEST currentAggImpMest)
        {
            try
            {
                if (currentAggImpMest == null)
                    throw new Exception("Du lieu rong currentAggImpMest");

                this._MobaExpMests = new List<HIS_EXP_MEST>();
                CommonParam param = new CommonParam();

                this._MobaImpMests = new List<HIS_IMP_MEST>();
                MOS.Filter.HisImpMestFilter impMestFilter = new HisImpMestFilter();
                impMestFilter.AGGR_IMP_MEST_ID = currentAggImpMest.ID;
                this._MobaImpMests = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GET, ApiConsumers.MosConsumer, impMestFilter, param);
                if (this._MobaImpMests != null && this._MobaImpMests.Count > 0)
                {
                    int start = 0;
                    int count = this._MobaImpMests.Count;
                    while (count > 0)
                    {
                        int limit = (count <= 100) ? count : 100;
                        var listSub = this._MobaImpMests.Skip(start).Take(limit).ToList();
                        List<long> _impMestIds = new List<long>();
                        _impMestIds = listSub.Select(p => p.ID).Distinct().ToList();

                        List<long> _MobaExpMestIds = listSub.Select(p => p.MOBA_EXP_MEST_ID ?? 0).Distinct().ToList();
                        MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                        expMestFilter.IDs = _MobaExpMestIds;
                        var dataExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                        if (dataExpMests != null && dataExpMests.Count > 0)
                        {
                            this._MobaExpMests.AddRange(dataExpMests);
                        }

                        CreateThread(_impMestIds);

                        start += 100;
                        count -= 100;
                    }
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThread(object param)
        {
            Thread threadMedi = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMedicineNewThread));
            Thread threadMate = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMaterialNewThread));

            threadMedi.Priority = ThreadPriority.Normal;
            try
            {
                threadMate.Start(param);
                threadMedi.Start(param);

                threadMedi.Join();
                threadMate.Join();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedi.Abort();
                threadMate.Abort();
            }
        }

        private void LoadDataMaterialNewThread(object obj)
        {
            try
            {
                LoadDataMaterial((List<long>)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicineNewThread(object obj)
        {
            try
            {
                LoadDataMedicine((List<long>)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicine(List<long> impMestIds)
        {
            try
            {
                //Thuoc
                CommonParam param = new CommonParam();
                HisImpMestMedicineViewFilter impMestMedicineViewFilter = new HisImpMestMedicineViewFilter();
                impMestMedicineViewFilter.IMP_MEST_IDs = impMestIds;
                var datas = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);

                if (datas != null && datas.Count > 0)
                {
                    this._useFormIds.AddRange(BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(p => datas.Select(o => o.MEDICINE_TYPE_ID).Contains(p.ID)).Select(p => p.MEDICINE_USE_FORM_ID ?? 0).ToList());
                    this.serviceUnitIds.AddRange(datas.Select(p => p.SERVICE_UNIT_ID).ToList());
                    this.reqRoomIds.AddRange(datas.Select(p => p.REQ_ROOM_ID ?? 0).ToList());
                    this._ImpMestMedicines.AddRange(datas);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterial(List<long> impMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                //Vat tu
                HisImpMestMaterialViewFilter impMestMaterialViewFilter = new HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.IMP_MEST_IDs = impMestIds;
                var datas = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);

                if (datas != null && datas.Count > 0)
                {
                    serviceUnitIds.AddRange(datas.Select(p => p.SERVICE_UNIT_ID).ToList());
                    reqRoomIds.AddRange(datas.Select(p => p.REQ_ROOM_ID ?? 0).ToList());
                    _ImpMestMaterials.AddRange(datas);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallThreadPool()
        {
            // Tạo một đối tượng ủy nhiệm, cho phép chúng ta
            // truyền phương thức DisplayMessage cho thread-pool.
            WaitCallback workMethod =
                new WaitCallback(ThreadPoolExample.DisplayMessage);

            // Thực thi DisplayMessage bằng thread-pool (không có đối số).
            ThreadPool.QueueUserWorkItem(workMethod);

            // Thực thi DisplayMessage bằng thread-pool (truyền một
            // đối tượng MessageInfo cho phương thức DisplayMessage).
            MessageInfo info =
                new MessageInfo(5, "A thread-pool example with arguments.");

            ThreadPool.QueueUserWorkItem(workMethod, info);

            // Nhấn Enter để kết thúc.
            Inventec.Common.Logging.LogSystem.Error("KT -------------------------< Finish");
        }

    }
    public class MessageInfo
    {

        private int iterations;
        private string message;

        // Phương thức khởi dựng nhận các thiết lập cấu hình cho tiểu trình.
        public MessageInfo(int iterations, string message)
        {

            this.iterations = iterations;
            this.message = message;
        }

        // Các thuộc tính dùng để lấy các thiết lập cấu hình.
        public int Iterations { get { return iterations; } }
        public string Message { get { return message; } }
    }

    public class ThreadPoolExample
    {

        // Hiển thị thông tin ra cửa sổ Console.
        public static void DisplayMessage(object state)
        {

            // Ép đối số state sang MessageInfo.
            MessageInfo config = state as MessageInfo;

            // Nếu đối số config là null, không có đối số nào được
            // truyền cho phương thức ThreadPool.QueueUserWorkItem;
            // sử dụng các giá trị mặc định.
            if (config == null)
            {

                // Hiển thị một thông báo ra cửa sổ Console ba lần.
                for (int count = 0; count < 3; count++)
                {

                    Inventec.Common.Logging.LogSystem.Error("A thread-pool example." + count);

                    // Vào trạng thái chờ, dùng cho mục đích minh họa.
                    // Tránh đưa các tiểu trình của thread-pool
                    // vào trạng thái chờ trong các ứng dụng thực tế.
                    Thread.Sleep(1000);
                }

            }
            else
            {

                // Hiển thị một thông báo được chỉ định trước
                // với số lần cũng được chỉ định trước.
                for (int count = 0; count < config.Iterations; count++)
                {

                    Inventec.Common.Logging.LogSystem.Error(config.Message + count);

                    // Vào trạng thái chờ, dùng cho mục đích minh họa.
                    // Tránh đưa các tiểu trình của thread-pool
                    // vào trạng thái chờ trong các ứng dụng thực tế.
                    Thread.Sleep(1000);
                }
            }
        }
    }

}
