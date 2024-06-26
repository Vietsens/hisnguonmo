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
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
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

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.MaternityLeave
{
    public partial class frmMaternityLeave : FormBase
    {
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboGender()
        {
            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemGridLookUpEditGender, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitMaternityLeaveGrid()
        {
            try
            {
                //Khoi tao mui tiem
                gridControlMaternityLeave.DataSource = null;
                List<MaternityLeaveData> maternityLeaveDatas = new List<MaternityLeaveData>();
                MaternityLeaveData maternityLeaveData = new MaternityLeaveData();
                maternityLeaveDatas.Add(maternityLeaveData);
                gridControlMaternityLeave.DataSource = maternityLeaveDatas;
                //
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataDefault()
        {
            try
            {

                if (this.MaternityLeaveDatas != null && this.MaternityLeaveDatas.Count > 0)
                {
                    gridControlMaternityLeave.DataSource = this.MaternityLeaveDatas;
                }
                else if (this.treatmentId > 0)
                {
                    List<MaternityLeaveData> maternityLeaveDatas = new List<MaternityLeaveData>();

                    CommonParam param = new CommonParam();
                    HisBabyFilter filter = new HisBabyFilter();
                    filter.TREATMENT_ID = this.treatmentId;
                    List<HIS_BABY> babes = new BackendAdapter(param)
                    .Get<List<HIS_BABY>>("api/HisBaby/Get", ApiConsumers.MosConsumer, filter, param);
                    if (babes != null && babes.Count > 0)
                    {
                        foreach (var item in babes)
                        {
                            MaternityLeaveData maternityLeaveData = new MaternityLeaveData();
                            maternityLeaveData.id = item.ID;
                            if (item.BORN_TIME.HasValue)
                                maternityLeaveData.BornTimeDt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.BORN_TIME.Value);
                            maternityLeaveData.FatherName = item.FATHER_NAME;
                            maternityLeaveData.GenderId = item.GENDER_ID;
                            maternityLeaveData.Weight = item.WEIGHT;
                            maternityLeaveDatas.Add(maternityLeaveData);
                        }

                        if (maternityLeaveDatas != null && maternityLeaveDatas.Count > 0)
                        {
                            gridControlMaternityLeave.DataSource = maternityLeaveDatas;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
