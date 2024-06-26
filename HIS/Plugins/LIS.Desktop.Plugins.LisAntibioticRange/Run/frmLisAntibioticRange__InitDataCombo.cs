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
using DevExpress.XtraEditors.DXErrorProvider;
using LIS.Desktop.Plugins.LisAntibioticRange.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LIS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using Inventec.Common.Controls.EditorLoader;

namespace LIS.Desktop.Plugins.LisAntibioticRange.Run
{
	public partial class frmLisAntibioticRange
	{
		private void LoadListAntibiotic()
		{
			try
			{

				CommonParam paramCommon = new CommonParam();
				dynamic filter = new System.Dynamic.ExpandoObject();
				datasAntibiotic = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<LIS_ANTIBIOTIC>>("api/LisAntibiotic/Get", HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer, filter, paramCommon);


				datasAntibiotic = datasAntibiotic != null ? datasAntibiotic.Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadListBacterium()
		{
			try
			{
				//if (BackendDataWorker.IsExistsKey<LIS_BACTERIUM>())
				//{
				//	datasBacterium = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<LIS_BACTERIUM>();
				//}
				//else
				//{ 
				CommonParam paramCommon = new CommonParam();
				dynamic filter = new System.Dynamic.ExpandoObject();
				datasBacterium = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<LIS_BACTERIUM>>("api/LisBacterium/Get", HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer, filter, paramCommon);
				//	if (datasBacterium != null) BackendDataWorker.UpdateToRam(typeof(LIS_BACTERIUM), datasBacterium, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
				//}
				datasBacterium = datasBacterium != null ? datasBacterium.Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitCombo()
		{
			try
			{
				InitCboAntibiotic();
				InitCboBacterium();
				InitCboTechnique();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

        private void InitCboTechnique()
        {
			try
			{
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("TECHNIQUE_CODE", "Mã", 50, 1));
				columnInfos.Add(new ColumnInfo("TECHNIQUE_NAME", "Tên", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("TECHNIQUE_NAME", "ID", columnInfos, true, 300);
				ControlEditorLoader.Load(cboTechnique, BackendDataWorker.Get<LIS.EFMODEL.DataModels.LIS_TECHNIQUE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), controlEditorADO);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

        private void InitCboAntibiotic()
		{
			try
			{

				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("ANTIBIOTIC_CODE", "Mã", 150, 1));
				columnInfos.Add(new ColumnInfo("ANTIBIOTIC_NAME", "Tên", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("ANTIBIOTIC_NAME", "ID", columnInfos, true, 400);
				ControlEditorLoader.Load(cboAntibiotic, datasAntibiotic, controlEditorADO);
				cboAntibiotic.Properties.ImmediatePopup = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitCboBacterium()
		{
			try
			{

				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("BACTERIUM_CODE", "Mã", 150, 1));
				columnInfos.Add(new ColumnInfo("BACTERIUM_NAME", "Tên", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("BACTERIUM_NAME", "ID", columnInfos, true, 400);
				ControlEditorLoader.Load(cboBacterium, datasBacterium, controlEditorADO);
				cboBacterium.Properties.ImmediatePopup = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
