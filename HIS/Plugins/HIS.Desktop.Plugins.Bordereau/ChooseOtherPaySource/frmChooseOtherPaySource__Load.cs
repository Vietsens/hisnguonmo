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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Bordereau.ADO;
using HIS.Desktop.Plugins.Bordereau.Config;
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

namespace HIS.Desktop.Plugins.Bordereau.ChooseOtherPaySource
{
    public partial class frmChooseOtherPaySource : Form
    {
        internal async Task LoadAndFillDataToReposiOtherPaySource()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE> datas = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>>("api/HisOtherPaySource/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                //List<long> otherPaySourceIdList = new List<long>();
                //if (this.sereServADOSelecteds != null && this.sereServADOSelecteds.Count > 0)
                //{
                //    List<long> otherPaySoureList = this.sereServADOSelecteds.Select(o => o.OTHER_PAY_SOURCE_ID ?? -1).ToList();
                //    if (otherPaySoureList != null && otherPaySoureList.Count > 0)
                //    {
                //        var groupOtherPaySource = otherPaySoureList.GroupBy(o => o);
                //        foreach (var item in groupOtherPaySource)
                //        {
                //            if (item.Count() > 0)
                //            {
                //                otherPaySourceIdList.Add(item.FirstOrDefault());
                //            }
                //        }

                //    }
                //}
                //var dataSource = datas != null && datas.Count > 0 && otherPaySourceIdList != null && otherPaySourceIdList.Count > 0 
                //    ? datas.Where(o => otherPaySourceIdList.Contains(o.ID)).ToList() 
                //    : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("OTHER_PAY_SOURCE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("OTHER_PAY_SOURCE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboOtherPaySource, datas, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
