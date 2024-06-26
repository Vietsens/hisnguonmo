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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrExpMestPrintFilter
{
    internal partial class frmAggregateExpMestPrintFilter : HIS.Desktop.Utility.FormBase
    {
        List<HIS_EXP_MEST> _ExpMests_Print { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqList { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqList { get; set; }

        private void LoadDataMedicineAndMaterial(V_HIS_EXP_MEST currentAggExpMest)
        {
            try
            {
                if (currentAggExpMest == null)
                    throw new ArgumentNullException("Du lieu rong currentAggExpMest");
                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.AGGR_EXP_MEST_ID = currentAggExpMest.ID;
                _ExpMests_Print = new List<HIS_EXP_MEST>();
                _ExpMests_Print = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                if (_ExpMests_Print != null && _ExpMests_Print.Count > 0)
                {
                    reqRoomIds.AddRange(_ExpMests_Print.Select(p => p.REQ_ROOM_ID).ToList());
                }

                LoadDataExpMestMety();
                LoadDataExpMestMaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void LoadDataMedicineAndMaterial_(List<V_HIS_EXP_MEST> currentAggExpMest)
        {
            try
            {
                if (currentAggExpMest == null)
                    throw new ArgumentNullException("Du lieu rong currentAggExpMest");
                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.AGGR_EXP_MEST_IDs = currentAggExpMest.Select(o => o.ID).ToList();
                _ExpMests_Print = new List<HIS_EXP_MEST>();
                _ExpMests_Print = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                if (_ExpMests_Print != null && _ExpMests_Print.Count > 0)
                {
                    reqRoomIds.AddRange(_ExpMests_Print.Select(p => p.REQ_ROOM_ID).ToList());
                }

                //LoadDataExpMestMety();
                CommonParam param_ = new CommonParam();
                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.AGGR_EXP_MEST_IDs = currentAggExpMest.Select(o => o.ID).ToList();  // TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_ID
                var dataMedicines = new BackendAdapter(param_).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param_);

                CommonParam param_1 = new CommonParam();
                HisExpMestMedicineViewFilter medicineFilter_ = new HisExpMestMedicineViewFilter();
                medicineFilter_.EXP_MEST_IDs = currentAggExpMest.Select(o => o.ID).ToList();  // TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_ID
                var dataMedicines_ = new BackendAdapter(param_1).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter_, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param_1);
               
                if (dataMedicines_ != null && dataMedicines_.Count > 0) 
                {
                    dataMedicines.AddRange(dataMedicines_);
                }
                
                if (dataMedicines != null && dataMedicines.Count > 0)
                {
                    _ExpMestMedicines.AddRange(dataMedicines.Distinct());
                    serviceUnitIds.AddRange(dataMedicines.Distinct().Select(p => p.SERVICE_UNIT_ID).ToList());
                    useFormIds.AddRange(dataMedicines.Distinct().Select(p => p.MEDICINE_USE_FORM_ID ?? 0).ToList());
                }


                //LoadDataExpMestMaty();
                CommonParam _param = new CommonParam();
                HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter();
                materialFilter.AGGR_EXP_MEST_IDs = currentAggExpMest.Select(o => o.ID).ToList(); //TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_ID
                var dataMaterials = new BackendAdapter(_param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, _param);

                CommonParam _param1 = new CommonParam();
                HisExpMestMaterialViewFilter materialFilter_ = new HisExpMestMaterialViewFilter();
                materialFilter_.EXP_MEST_IDs = currentAggExpMest.Select(o => o.ID).ToList(); //TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_ID
                var dataMaterials_ = new BackendAdapter(_param1).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter_, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, _param1);


                if (dataMaterials_ != null && dataMaterials_.Count > 0)
                {
                    dataMaterials.AddRange(dataMaterials_);
                }
                if (dataMaterials != null && dataMaterials.Count > 0)
                {
                    _ExpMestMaterials.AddRange(dataMaterials.Distinct());
                    serviceUnitIds.AddRange(dataMaterials.Distinct().Select(p => p.SERVICE_UNIT_ID).ToList());
                }

               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMety()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_ID = this.aggrExpMest.ID;
                var dataMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (dataMedicines != null && dataMedicines.Count > 0)
                {
                    _ExpMestMedicines.AddRange(dataMedicines);
                    serviceUnitIds.AddRange(dataMedicines.Select(p => p.SERVICE_UNIT_ID).ToList());
                    useFormIds.AddRange(dataMedicines.Select(p => p.MEDICINE_USE_FORM_ID ?? 0).ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMaty()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter();
                materialFilter.TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_ID = this.aggrExpMest.ID;
                var dataMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (dataMaterials != null && dataMaterials.Count > 0)
                {
                    _ExpMestMaterials.AddRange(dataMaterials);
                    serviceUnitIds.AddRange(dataMaterials.Select(p => p.SERVICE_UNIT_ID).ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataReqMetyMaty(V_HIS_EXP_MEST currentAggExpMest)
        {
            try
            {
                if (currentAggExpMest == null)
                    throw new ArgumentNullException("Du lieu rong currentAggExpMest");
                _ExpMestMetyReqList = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReqList = new List<HIS_EXP_MEST_MATY_REQ>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.ID = currentAggExpMest.ID;
                _ExpMests_Print = new List<HIS_EXP_MEST>();
                _ExpMests_Print = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                if (_ExpMests_Print != null && _ExpMests_Print.Count > 0)
                {
                    reqRoomIds.AddRange(_ExpMests_Print.Select(p => p.REQ_ROOM_ID).ToList());

                    int start = 0;
                    int count = _ExpMests_Print.Count;
                    while (count > 0)
                    {
                        int limit = (count <= 100) ? count : 100;
                        var listSub = _ExpMests_Print.Skip(start).Take(limit).ToList();
                        List<long> _impMestIds = new List<long>();
                        _impMestIds = listSub.Select(p => p.ID).Distinct().ToList();

                        LoadDataExpMestMatyReqBCS(_impMestIds);
                        LoadDataExpMestMetyReqBCS(_impMestIds);
                        start += 100;
                        count -= 100;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMatyReqBCS(List<long> _expMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMatyReqFilter materialFilter = new HisExpMestMatyReqFilter();
                materialFilter.EXP_MEST_IDs = _expMestIds;
                var dataMaterials = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, materialFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (dataMaterials != null && dataMaterials.Count > 0)
                {
                    _ExpMestMatyReqList.AddRange(dataMaterials);

                    List<long> _mmaterialTypeIds = dataMaterials.Select(p => p.MATERIAL_TYPE_ID).Distinct().ToList();
                    var dataMaterialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(p => _mmaterialTypeIds.Contains(p.ID)).ToList();
                    if (dataMaterialTypes != null && dataMaterialTypes.Count > 0)
                    {
                        serviceUnitIds.AddRange(dataMaterialTypes.Select(p => p.SERVICE_UNIT_ID).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMetyReqBCS(List<long> _expMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMetyReqFilter medicineFilter = new HisExpMestMetyReqFilter();
                medicineFilter.EXP_MEST_IDs = _expMestIds;
                var dataMedicines = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (dataMedicines != null && dataMedicines.Count > 0)
                {
                    _ExpMestMetyReqList.AddRange(dataMedicines);
                    List<long> _medicineTypeIds = dataMedicines.Select(p => p.MEDICINE_TYPE_ID).Distinct().ToList();
                    var dataMedicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(p => _medicineTypeIds.Contains(p.ID)).ToList();
                    if (dataMedicineTypes != null && dataMedicineTypes.Count > 0)
                    {
                        serviceUnitIds.AddRange(dataMedicineTypes.Select(p => p.SERVICE_UNIT_ID).ToList());
                        useFormIds.AddRange(dataMedicineTypes.Select(p => p.MEDICINE_USE_FORM_ID ?? 0).ToList());
                    }
                }
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
                if (serviceUnitIds != null && serviceUnitIds.Count > 0)
                {
                    serviceUnitIds = serviceUnitIds.Distinct().ToList();
                }
                else
                {
                    serviceUnitIds = new List<long>();
                }

                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceUnitFilter serviceUnitFilter = new HisServiceUnitFilter();
                serviceUnitFilter.IDs = serviceUnitIds;
                var serviceUnits = new BackendAdapter(param).Get<List<HIS_SERVICE_UNIT>>(HisRequestUriStore.HIS_SERVICE_UNIT_GET_RAW, ApiConsumers.MosConsumer, serviceUnitFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (serviceUnits != null && serviceUnits.Count > 0)
                {
                    serviceUnits = serviceUnits.Where(o => serviceUnitIds.Contains(o.ID)).ToList();
                }
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
                if (useFormIds != null && useFormIds.Count > 0)
                {
                    useFormIds = useFormIds.Distinct().ToList();
                }
                else
                {
                    useFormIds = new List<long>();
                }

                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineUseFormFilter medicineUseFormFilter = new HisMedicineUseFormFilter();
                medicineUseFormFilter.IDs = useFormIds;
                var medicineUseForms = new BackendAdapter(param).Get<List<HIS_MEDICINE_USE_FORM>>(HisRequestUriStore.HIS_MEDICINE_USE_FORM_GET, ApiConsumers.MosConsumer, medicineUseFormFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
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
    }
}
