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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.ControlProcess
{
    public class TranPatiReasonProcess
    {
        public static async Task LoadDataToComboLyDoChuyen(DevExpress.XtraEditors.LookUpEdit cboLyDoChuyen, List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> data)
        {
            try
            {
                if (data == null)
                {
                    if (HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>())
                    {
                        data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>();
                    }
                    else
                    {
                        CommonParam paramCommon = new CommonParam();
                        dynamic filter = new System.Dynamic.ExpandoObject();
                        data = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>>("api/HisTranpatiReason/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                        if (data != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON), data, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    }
                }

                cboLyDoChuyen.Properties.DataSource = data;
                cboLyDoChuyen.Properties.DisplayMember = "TRAN_PATI_REASON_NAME";
                cboLyDoChuyen.Properties.ValueMember = "ID";
                cboLyDoChuyen.Properties.ForceInitialize();
                cboLyDoChuyen.Properties.Columns.Clear();
                cboLyDoChuyen.Properties.Columns.Add(new LookUpColumnInfo("TRAN_PATI_REASON_CODE", "", 50));
                cboLyDoChuyen.Properties.Columns.Add(new LookUpColumnInfo("TRAN_PATI_REASON_NAME", "", 400));
                cboLyDoChuyen.Properties.ShowHeader = false;
                cboLyDoChuyen.Properties.ImmediatePopup = true;
                cboLyDoChuyen.Properties.DropDownRows = 20;
                cboLyDoChuyen.Properties.PopupWidth = 450;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
