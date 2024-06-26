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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExportBlood.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportBlood
{
    public partial class frmExpMestBlood
    {
        private void frmExpMestBlood_Plus_GridLookup()
        {
            try
            {
                initGridLookupVolume();
                initGridLookupAbo();
                initGridLookupRh();
                initGridLookupBloodType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupVolume()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodVolumeFilter filter = new MOS.Filter.HisBloodVolumeFilter();
                filter.KEY_WORD = gridLookUpVolume.Text;

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>>
                  ("api/HisBloodVolume/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                obj = obj.OrderByDescending(o => o.ID).ToList();
                bloodVolume = new List<BloodVolumeADO>();
                if (obj != null && obj.Count > 0)
                {
                    foreach (var item in obj)
                    {
                        BloodVolumeADO blood = new BloodVolumeADO(item);
                        bloodVolume.Add(blood);
                    }
                }
                BloodVolumeADO addAll = new BloodVolumeADO();
                addAll.ID = obj.FirstOrDefault().ID + 1;
                addAll.Blood_Volume_Str = "Tất cả";
                bloodVolume.Add(addAll);

                bloodVolume = bloodVolume.OrderByDescending(o => o.ID).ToList();

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("Blood_Volume_Str", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("Blood_Volume_Str", "ID", columnInfo, false);
                ControlEditorLoader.Load(gridLookUpVolume, bloodVolume, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupAbo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodAboFilter filter = new MOS.Filter.HisBloodAboFilter();

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>>
                  ("api/HisBloodAbo/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfo, false);
                ControlEditorLoader.Load(gridLookUpBloodAboCode, obj, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupRh()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodRhFilter filter = new MOS.Filter.HisBloodRhFilter();

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>>
                  ("api/HisBloodRh/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("BLOOD_RH_CODE", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfo, false);
                ControlEditorLoader.Load(gridLookUpBloodRhCode, obj, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupBloodType()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodTypeFilter filter = new MOS.Filter.HisBloodTypeFilter();

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>>
                  ("api/HisBloodType/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("BLOOD_TYPE_CODE", "", 0, 1));
                columnInfo.Add(new ColumnInfo("BLOOD_TYPE_NAME", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("BLOOD_TYPE_NAME", "ID", columnInfo, false);
                ControlEditorLoader.Load(cboBloodType, obj, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setGridLookUpClick()
        {
            try
            {
                gridLookUpVolume.EditValue = this.currentBlty.BLOOD_VOLUME_ID;
                gridLookUpBloodAboCode.EditValue = this.currentBlty.BLOOD_ABO_ID;
                gridLookUpBloodRhCode.EditValue = this.currentBlty.BLOOD_RH_ID;
                cboBloodType.EditValue = this.currentBlty.BLOOD_TYPE_ID;
                checkBtnRefresh = false;
                fillDataGridViewBlood();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void fillDataGridViewBlood()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodViewFilter filter = new MOS.Filter.HisBloodViewFilter();

               
                long maxId = 0;

                if (bloodVolume != null && bloodVolume.Count > 0)
                {
                    var listID = bloodVolume.Select(o => o.ID).ToList();
                    maxId = listID.Max();
                }

                if (cboBloodType.EditValue != null)
                {
                    filter.BLOOD_TYPE_ID = (long?)cboBloodType.EditValue;
                }

                if (gridLookUpVolume.EditValue != null)
                {
                    if ((long)gridLookUpVolume.EditValue == maxId)
                        filter.BLOOD_VOLUME_ID = null;
                    else
                        filter.BLOOD_VOLUME_ID = (long)gridLookUpVolume.EditValue;
                }
                if (gridLookUpBloodAboCode.EditValue != null)
                    filter.BLOOD_ABO_ID = (long)gridLookUpBloodAboCode.EditValue;
                if (gridLookUpBloodRhCode.EditValue != null)
                    filter.BLOOD_RH_ID = (long)gridLookUpBloodRhCode.EditValue;

                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                var listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                filter.MEDI_STOCK_ID = listMediStock.FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId).ID;

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_BLOOD>>
                  ("api/HisBlood/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                if (obj != null)
                    gridViewBlood.GridControl.DataSource = obj;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void resetGridLookup()
        {
            try
            {
                gridLookUpVolume.EditValue = null;
                gridLookUpBloodAboCode.EditValue = null;
                gridLookUpBloodRhCode.EditValue = null;
                cboBloodType.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
