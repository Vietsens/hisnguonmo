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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.Prepare.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Prepare
{
    public partial class frmPrepare : FormBase
    {
        /// <summary>
        /// Load lai day du thong du lieu 
        /// vi du lieu truyen vao doi tuong chi co ID
        /// </summary>
        private void InitDataInput()
        {
            CommonParam param = new CommonParam();
            if (actionType == PEnum.ACTION_TYPE.CREATE && treatment != null)
            {
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatment.ID;
                var treatments = new BackendAdapter(param)
                    .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param);
                if (treatments == null && treatments.Count == 0)
                {
                    throw new Exception("Khong lay duoc thong tin treatment hoac du lieu khong dung" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatments), treatments));
                }
                treatment = treatments[0];
            }

            if (actionType == PEnum.ACTION_TYPE.UPDATE && prepare != null)
            {
                HisPrepareFilter prepareFilter = new HisPrepareFilter();
                prepareFilter.ID = prepare.ID;
                var prepares = new BackendAdapter(param)
                    .Get<List<HIS_PREPARE>>("api/HisPrepare/Get", ApiConsumers.MosConsumer, prepareFilter, param);
                if (prepares == null && prepares.Count == 0)
                {
                    throw new Exception("Khong lay duoc thong tin treatment hoac du lieu khong dung" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prepare), prepare));
                }
                prepare = prepares[0];
            }
        }

        private void LoadMediMatyToCbo()
        {
            try
            {
                List<MedicineMaterialTypeComboADO> mediMateTypeComboADOs = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true, false, false);
                var mediMateTypeIsPrepares = mediMateTypeComboADOs != null ? mediMateTypeComboADOs.Where(o => o.IS_MUST_PREPARE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("METY_MATY_TYPE_CODE", "Mã thuốc/vật tư", 100, 1, true));
                columnInfos.Add(new ColumnInfo("METY_MATY_TYPE_NAME", "Tên thuốc/ vật tư", 250, 2, true));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "ĐVT", 100, 3, true));
                columnInfos.Add(new ColumnInfo("ACTIVE_INGR_BHYT_NAME", "Hoạt chất", 150, 4, true));
                columnInfos.Add(new ColumnInfo("CONCENTRA", "Hàm lượng", 150, 5, true));
                columnInfos.Add(new ColumnInfo("MANUFACTURER_NAME", "Nơi sản xuất", 100, 6, true));
                columnInfos.Add(new ColumnInfo("NATIONAL_NAME", "Quốc gia", 100, 7, true));
                columnInfos.Add(new ColumnInfo("METY_MATY_TYPE_NAME__UNSIGN", "", 100, -1, true));
                columnInfos.Add(new ColumnInfo("ACTIVE_INGR_BHYT_NAME__UNSIGN", "", 100, -1, true));
                columnInfos.Add(new ColumnInfo("NATIONAL_NAME__UNSIGN", "", 100, -1, true));
                columnInfos.Add(new ColumnInfo("MANUFACTURER_NAME__UNSIGN", "", 100, -1, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("METY_MATY_TYPE_NAME", "METY_MATY_TYPE_ID", columnInfos, true, 950);
                ControlEditorLoader.Load(repositoryItemCustomGridLookUpEditMetyMaty, ConvertToPrepareMediMatyADO(mediMateTypeIsPrepares), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<PrepareMetyMatyADO> ConvertToPrepareMediMatyADO(List<MedicineMaterialTypeComboADO> medicineMaterialTypeComboADOs)
        {
            List<PrepareMetyMatyADO> result = new List<PrepareMetyMatyADO>();
            try
            {
                if (medicineMaterialTypeComboADOs != null && medicineMaterialTypeComboADOs.Count > 0)
                {
                    foreach (var item in medicineMaterialTypeComboADOs)
                    {
                        PrepareMetyMatyADO prepareMedyMatyADO = new PrepareMetyMatyADO();
                        prepareMedyMatyADO.METY_MATY_TYPE_ID = item.ID;
                        prepareMedyMatyADO.METY_MATY_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                        prepareMedyMatyADO.METY_MATY_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                        prepareMedyMatyADO.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                        prepareMedyMatyADO.CONCENTRA = item.CONCENTRA;
                        prepareMedyMatyADO.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                        prepareMedyMatyADO.NATIONAL_NAME = item.NATIONAL_NAME;
                        prepareMedyMatyADO.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        prepareMedyMatyADO.ACTIVE_INGR_BHYT_NAME__UNSIGN = Inventec.Common.String.Convert.UnSignVNese(item.ACTIVE_INGR_BHYT_NAME);
                        prepareMedyMatyADO.METY_MATY_TYPE_NAME__UNSIGN = Inventec.Common.String.Convert.UnSignVNese(item.MEDICINE_TYPE_NAME);
                        prepareMedyMatyADO.MANUFACTURER_NAME__UNSIGN = Inventec.Common.String.Convert.UnSignVNese(item.MANUFACTURER_NAME);
                        prepareMedyMatyADO.NATIONAL_NAME__UNSIGN = Inventec.Common.String.Convert.UnSignVNese(item.NATIONAL_NAME);
                        if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                        {
                            prepareMedyMatyADO.TYPE = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                        }
                        if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                        {
                            prepareMedyMatyADO.TYPE = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                        }

                        result.Add(prepareMedyMatyADO);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitPrepareMetyMatyGrid()
        {
            try
            {
                //Khoi tao mui tiem
                gridControlPrepareMety.DataSource = null;
                List<PrepareMetyMatyADO> prepareMetyMatyADOs = new List<PrepareMetyMatyADO>();
                PrepareMetyMatyADO prepareMetyMatyADO = new PrepareMetyMatyADO();
                prepareMetyMatyADO.ACTION = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                prepareMetyMatyADOs.Add(prepareMetyMatyADO);
                gridControlPrepareMety.DataSource = prepareMetyMatyADOs;
                //
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadPrepareEdit()
        {
            try
            {
                if (actionType == PEnum.ACTION_TYPE.UPDATE && prepare != null)
                {
                    if (prepare.FROM_TIME.HasValue)
                        dtFromTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(prepare.FROM_TIME.Value).Value;
                    if (prepare.TO_TIME.HasValue)
                        dtToTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(prepare.TO_TIME.Value).Value;
                    txtDescription.Text = prepare.DESCRIPTION;

                    List<PrepareMetyMatyADO> prepareMetyMatyADOs = new List<PrepareMetyMatyADO>();
                    this.GetPrepareMetyEdit(ref prepareMetyMatyADOs);
                    this.GetPrepareMatyEdit(ref prepareMetyMatyADOs);
                    if (prepareMetyMatyADOs != null && prepareMetyMatyADOs.Count > 0)
                    {
                        foreach (var item in prepareMetyMatyADOs)
                        {
                            if (prepareMetyMatyADOs.IndexOf(item) == 0)
                                item.ACTION = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            else
                                item.ACTION = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }
                        gridControlPrepareMety.DataSource = prepareMetyMatyADOs;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetPrepareMetyEdit(ref List<PrepareMetyMatyADO> prepareMetyMatyADOs)
        {
            try
            {
                //Load prepare mety
                List<V_HIS_PREPARE_METY> prepareMetyByPrepares = prepareMetybyTreatments != null ?
                    prepareMetybyTreatments.Where(o => o.PREPARE_ID == prepare.ID).ToList() : null;
                if (prepareMetyByPrepares != null && prepareMetyByPrepares.Count > 0)
                {
                    foreach (var item in prepareMetyByPrepares)
                    {
                        PrepareMetyMatyADO ado = new PrepareMetyMatyADO();
                        ado.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                        ado.APPROVAL_AMOUNT = item.APPROVAL_AMOUNT;
                        ado.CONCENTRA = item.CONCENTRA;
                        ado.ID = item.ID;
                        ado.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                        ado.METY_MATY_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                        ado.METY_MATY_TYPE_ID = item.MEDICINE_TYPE_ID;
                        ado.METY_MATY_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                        ado.NATIONAL_NAME = item.NATIONAL_NAME;
                        ado.REQ_AMOUNT = item.REQ_AMOUNT;
                        ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        ado.TYPE = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                        prepareMetyMatyADOs.Add(ado);
                    }
                    gridControlPrepareMety.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetPrepareMatyEdit(ref List<PrepareMetyMatyADO> prepareMetyMatyADOs)
        {
            try
            {
               
                //Load prepare maty
                List<V_HIS_PREPARE_MATY> prepareMatyByPrepares = prepareMatybyTreatments != null ?
                    prepareMatybyTreatments.Where(o => o.PREPARE_ID == prepare.ID).ToList() : null;

                if (prepareMatyByPrepares != null && prepareMatyByPrepares.Count > 0)
                {
                    foreach (var item in prepareMatyByPrepares)
                    {
                        PrepareMetyMatyADO ado = new PrepareMetyMatyADO();
                        ado.APPROVAL_AMOUNT = item.APPROVAL_AMOUNT;
                        ado.ID = item.ID;
                        ado.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                        ado.METY_MATY_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                        ado.METY_MATY_TYPE_ID = item.MATERIAL_TYPE_ID;
                        ado.METY_MATY_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        ado.NATIONAL_NAME = item.NATIONAL_NAME;
                        ado.REQ_AMOUNT = item.REQ_AMOUNT;
                        ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        ado.TYPE = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                        prepareMetyMatyADOs.Add(ado);
                    }
                    gridControlPrepareMety.RefreshDataSource();
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPrepareMetyByTreatment()
        {
            try
            {
                //get treatmentid
                long treatmentId = treatment != null ? treatment.ID : prepare != null ? prepare.TREATMENT_ID : 0;
                if (treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisPrepareMetyViewFilter filter = new HisPrepareMetyViewFilter();
                    filter.TDL_TREATMENT_ID = treatmentId;
                    prepareMetybyTreatments = new BackendAdapter(param)
                        .Get<List<V_HIS_PREPARE_METY>>("api/HisPrepareMety/GetView", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPrepareMatyByTreatment()
        {
            try
            {
                //get treatmentid
                long treatmentId = treatment != null ? treatment.ID : prepare != null ? prepare.TREATMENT_ID : 0;
                if (treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisPrepareMatyViewFilter filter = new HisPrepareMatyViewFilter();
                    filter.TDL_TREATMENT_ID = treatmentId;
                    prepareMatybyTreatments = new BackendAdapter(param)
                        .Get<List<V_HIS_PREPARE_MATY>>("api/HisPrepareMaty/GetView", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPrepareMetyMatyLog()
        {
            try
            {
                List<PrepareMetyMatyADO> prepareMetyMatyADOs = new List<PrepareMetyMatyADO>();
                //Load prepare mety
                List<V_HIS_PREPARE_METY> prepareMetyByPrepares = null;
                List<V_HIS_PREPARE_MATY> prepareMatyByPrepares = null;
                if (prepare != null)
                {
                    prepareMetyByPrepares = prepareMetybyTreatments != null ?
                        prepareMetybyTreatments.Where(o => o.PREPARE_ID == prepare.ID).ToList() : null;
                    prepareMatyByPrepares = prepareMatybyTreatments != null ?
                   prepareMatybyTreatments.Where(o => o.PREPARE_ID == prepare.ID).ToList() : null;
                }
                else
                {
                    prepareMetyByPrepares = prepareMetybyTreatments;
                    prepareMatyByPrepares = prepareMatybyTreatments;
                }

                if (prepareMetyByPrepares != null && prepareMetyByPrepares.Count > 0)
                {
                    foreach (var item in prepareMetyByPrepares)
                    {
                        PrepareMetyMatyADO ado = new PrepareMetyMatyADO();
                        ado.ACTION = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        ado.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                        ado.APPROVAL_AMOUNT = item.APPROVAL_AMOUNT;
                        ado.CONCENTRA = item.CONCENTRA;
                        ado.ID = item.ID;
                        ado.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                        ado.METY_MATY_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                        ado.METY_MATY_TYPE_ID = item.MEDICINE_TYPE_ID;
                        ado.METY_MATY_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                        ado.NATIONAL_NAME = item.NATIONAL_NAME;
                        ado.REQ_AMOUNT = item.REQ_AMOUNT;
                        ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        ado.REQ_LOGINNAME = item.REQ_LOGINNAME;
                        ado.REQ_USERNAME = item.REQ_USERNAME;
                        ado.APPROVAL_LOGINNAME = item.APPROVAL_LOGINNAME;
                        ado.APPROVAL_USERNAME = item.APPROVAL_USERNAME;
                        ado.TYPE = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                        prepareMetyMatyADOs.Add(ado);
                    }
                }

                if (prepareMatyByPrepares != null && prepareMatyByPrepares.Count > 0)
                {
                    foreach (var item in prepareMatyByPrepares)
                    {
                        PrepareMetyMatyADO ado = new PrepareMetyMatyADO();
                        ado.ACTION = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        ado.APPROVAL_AMOUNT = item.APPROVAL_AMOUNT;
                        ado.ID = item.ID;
                        ado.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                        ado.METY_MATY_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                        ado.METY_MATY_TYPE_ID = item.MATERIAL_TYPE_ID;
                        ado.METY_MATY_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        ado.NATIONAL_NAME = item.NATIONAL_NAME;
                        ado.REQ_AMOUNT = item.REQ_AMOUNT;
                        ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        ado.REQ_LOGINNAME = item.REQ_LOGINNAME;
                        ado.REQ_USERNAME = item.REQ_USERNAME;
                        ado.APPROVAL_LOGINNAME = item.APPROVAL_LOGINNAME;
                        ado.APPROVAL_USERNAME = item.APPROVAL_USERNAME;
                        ado.TYPE = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                        prepareMetyMatyADOs.Add(ado);
                    }
                }
                gridControlPrepareMetyLog.DataSource = prepareMetyMatyADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }
    }
}
