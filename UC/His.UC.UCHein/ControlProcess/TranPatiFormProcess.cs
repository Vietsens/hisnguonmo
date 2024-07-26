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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;


namespace His.UC.UCHein.ControlProcess
{
    public class TranPatiFormProcess
    {
        public static async Task LoadDataToCombo(DevExpress.XtraEditors.LookUpEdit cboHinhThucChuyen, List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> data)
        {
            try
            {
                if (data == null)
                {
                    if (HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>())
                    {
                        data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>();
                    }
                    else
                    {
                        CommonParam paramCommon = new CommonParam();
                        dynamic filter = new System.Dynamic.ExpandoObject();
                        data = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>>("api/HisTranpatiForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                        if (data != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM), data, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    }
                }
                cboHinhThucChuyen.Properties.DataSource = data;
                cboHinhThucChuyen.Properties.DisplayMember = "TRAN_PATI_FORM_NAME";
                cboHinhThucChuyen.Properties.ValueMember = "ID";
                cboHinhThucChuyen.Properties.ForceInitialize();
                cboHinhThucChuyen.Properties.Columns.Clear();
                cboHinhThucChuyen.Properties.Columns.Add(new LookUpColumnInfo("TRAN_PATI_FORM_CODE", "", 50));
                cboHinhThucChuyen.Properties.Columns.Add(new LookUpColumnInfo("TRAN_PATI_FORM_NAME", "", 400));
                cboHinhThucChuyen.Properties.ShowHeader = false;
                cboHinhThucChuyen.Properties.ImmediatePopup = true;
                cboHinhThucChuyen.Properties.DropDownRows = 20;
                cboHinhThucChuyen.Properties.PopupWidth = 450;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
