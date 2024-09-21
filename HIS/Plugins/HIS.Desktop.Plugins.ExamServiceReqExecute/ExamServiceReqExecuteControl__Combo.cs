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
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        public async Task InitComboKsk()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<HIS_HEALTH_EXAM_RANK>())
                {
                    lstHealthExamRank = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    filter.IS_ACTIVE = 1;
                    lstHealthExamRank = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_HEALTH_EXAM_RANK>>("api/HisHealthExamRank/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (lstHealthExamRank != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_HEALTH_EXAM_RANK), lstHealthExamRank, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEALTH_EXAM_RANK_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("HEALTH_EXAM_RANK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEALTH_EXAM_RANK_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboKskCode, lstHealthExamRank, controlEditorADO);

                this.cboKskCode.EditValue = (this.HisServiceReqView != null ? this.HisServiceReqView.HEALTH_EXAM_RANK_ID : null);
                if (this.treatment != null && !string.IsNullOrEmpty(this.treatment.HRM_KSK_CODE))
                {
                    lciKskCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    ValidationSingleControl(cboKskCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public async Task InitComboPatientCase()
        {
            try
            {
                List<HIS_PATIENT_CASE> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_PATIENT_CASE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_CASE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    filter.IS_ACTIVE = 1;
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PATIENT_CASE>>("api/HisPatientCase/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_CASE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : datas;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_CASE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_CASE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_CASE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPatientCase, datas, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
     

    }
}
