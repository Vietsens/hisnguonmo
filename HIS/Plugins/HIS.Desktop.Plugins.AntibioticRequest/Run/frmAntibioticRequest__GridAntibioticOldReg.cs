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
using DevExpress.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.AntibioticRequest.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.AntibioticRequest.Run
{
	public partial class frmAntibioticRequest
	{
		private void LoadDefaultGridAntibioticOldReg()
		{
			try
			{
				for (int i = 0; i < 2; i++)
				{
					AntibioticOldRegADO ado = new AntibioticOldRegADO();
					ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
					if (i == 1)
					{
						ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
					}
					lstOldRegADO.Add(ado);
				}
				gridControlAntibioticOldReg.DataSource = null;
				gridControlAntibioticOldReg.DataSource = lstOldRegADO;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void FillDataToGridAntibioticOldReg()
		{
			try
			{
				if (this.currentAntibioticRequest != null && this.currentAntibioticRequest.AntibioticRequest != null)
				{
					CommonParam param = new CommonParam();
					HisAntibioticOldRegFilter filter = new HisAntibioticOldRegFilter();
					filter.ANTIBIOTIC_REQUEST_ID = this.currentAntibioticRequest.AntibioticRequest.ID;
					filter.ORDER_FIELD = "MODIFY_TIME";
					filter.ORDER_DIRECTION = "ASC";
					var dataDf = new BackendAdapter(param)
		.Get<List<HIS_ANTIBIOTIC_OLD_REG>>("api/HisAntibioticOldReg/Get", ApiConsumers.MosConsumer, filter, param);
					if (dataDf != null && dataDf.Count > 0)
					{
						lstOldRegADO = new List<AntibioticOldRegADO>();
						for (int i = 0; i < dataDf.Count; i++)
						{
							AntibioticOldRegADO ado = new AntibioticOldRegADO();
							Inventec.Common.Mapper.DataObjectMapper.Map<AntibioticOldRegADO>(ado, dataDf[i]);
							ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
							if (i == dataDf.Count - 1)
							{
								ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
							}
							lstOldRegADO.Add(ado);
						}
					
						gridControlAntibioticOldReg.DataSource = null;
						gridControlAntibioticOldReg.DataSource = lstOldRegADO;
						if (lstOldRegADO != null && lstOldRegADO.Count > 0)
						{
							lstOldRegADOTemp = lstOldRegADO.Where(o => !string.IsNullOrEmpty(o.ANTIBIOTIC_NAME)
																).ToList();
						}
						foreach (var item in lstOldRegADOTemp)
						{
							HIS_ANTIBIOTIC_OLD_REG obj = new HIS_ANTIBIOTIC_OLD_REG();
							obj.ANTIBIOTIC_REQUEST_ID = item.ANTIBIOTIC_REQUEST_ID;
							obj.ANTIBIOTIC_NAME = item.ANTIBIOTIC_NAME;
							this.currentAntibioticOldReg.Add(obj);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void gridViewAntibioticOldReg_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
		{
			try
			{
				if (e.Column.FieldName == "BtnDeleteAntibioticOldReg")
				{
					int rowSelected = Convert.ToInt32(e.RowHandle);
					int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewAntibioticOldReg.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
					if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
					{
						e.RepositoryItem = btnAddAntibioticOldReg;
					}
					else
					{
						e.RepositoryItem = btnDeleteAntibioticOldReg;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void gridViewAntibioticOldReg_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
			{
				AntibioticOldRegADO dataRow = (AntibioticOldRegADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				if (e.Column.FieldName == "STT")
				{
					e.Value = e.ListSourceRowIndex + 1;
				}
			}
		}
		private void btnAddAntibioticOldReg_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				List<AntibioticOldRegADO> AntibioticOldRegADOs = new List<AntibioticOldRegADO>();
				var AntibioticOldRegADO = gridControlAntibioticOldReg.DataSource as List<AntibioticOldRegADO>;
				if (AntibioticOldRegADO == null || AntibioticOldRegADO.Count < 1)
				{
					AntibioticOldRegADO ekipUserAdoTemp = new AntibioticOldRegADO();
					AntibioticOldRegADOs.Add(ekipUserAdoTemp);
					AntibioticOldRegADOs.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
					AntibioticOldRegADOs.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
					gridControlAntibioticOldReg.DataSource = null;
					gridControlAntibioticOldReg.DataSource = AntibioticOldRegADOs;
				}
				else
				{
					AntibioticOldRegADO participant = new AntibioticOldRegADO();
					AntibioticOldRegADO.Add(participant);
					AntibioticOldRegADO.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
					AntibioticOldRegADO.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
					gridControlAntibioticOldReg.DataSource = null;
					gridControlAntibioticOldReg.DataSource = AntibioticOldRegADO;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void btnDeleteAntibioticOldReg_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				CommonParam param = new CommonParam();
				var AntibioticOldRegADOs = gridControlAntibioticOldReg.DataSource as List<AntibioticOldRegADO>;
				var AntibioticOldRegADO = (AntibioticOldRegADO)gridViewAntibioticOldReg.GetFocusedRow();
				if (AntibioticOldRegADO != null)
				{
					if (AntibioticOldRegADOs.Count > 0)
					{
						AntibioticOldRegADOs.Remove(AntibioticOldRegADO);
						AntibioticOldRegADOs.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
						AntibioticOldRegADOs.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
						gridControlAntibioticOldReg.DataSource = null;
						gridControlAntibioticOldReg.DataSource = AntibioticOldRegADOs;
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
