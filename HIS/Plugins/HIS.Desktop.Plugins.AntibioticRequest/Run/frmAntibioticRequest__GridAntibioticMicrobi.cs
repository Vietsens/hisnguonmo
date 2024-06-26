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
using Inventec.Desktop.CustomControl;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors;
using MOS.Filter;

namespace HIS.Desktop.Plugins.AntibioticRequest.Run
{
	public partial class frmAntibioticRequest
	{
		private void LoadDefaultGridAntibioticMicrobi()
		{
			try
			{
				for (int i = 0; i < 2; i++)
				{
					AntibioticMicrobiADO ado = new AntibioticMicrobiADO();
					ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
					if (i == 1)
					{
						ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
					}
					lstMicrobiADO.Add(ado);
				}
				gridControlAntibioticMicrobi.DataSource = null;
				gridControlAntibioticMicrobi.DataSource = lstMicrobiADO;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void FillDataToGridAntibioticMicrobi()
		{
			try
			{
				if (this.currentAntibioticRequest != null && this.currentAntibioticRequest.AntibioticRequest != null)
				{
					CommonParam param = new CommonParam();
					HisAntibioticMicrobiFilter filter = new HisAntibioticMicrobiFilter();
					filter.ANTIBIOTIC_REQUEST_ID = this.currentAntibioticRequest.AntibioticRequest.ID;
					var dataDf = new BackendAdapter(param)
		.Get<List<HIS_ANTIBIOTIC_MICROBI>>("api/HisAntibioticMicrobi/Get", ApiConsumers.MosConsumer, filter, param);
					if (dataDf != null && dataDf.Count > 0)
					{
						lstMicrobiADO = new List<AntibioticMicrobiADO>();
						for (int i = 0; i < dataDf.Count; i++)
						{
							AntibioticMicrobiADO ado = new AntibioticMicrobiADO();
							Inventec.Common.Mapper.DataObjectMapper.Map<AntibioticMicrobiADO>(ado, dataDf[i]);
							ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
							if (i == dataDf.Count - 1)
							{
								ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
							}
							lstMicrobiADO.Add(ado);
						}
						gridControlAntibioticMicrobi.DataSource = null;
						gridControlAntibioticMicrobi.DataSource = lstMicrobiADO;
						if (lstMicrobiADO != null && lstMicrobiADO.Count > 0)
						{
							lstMicrobiADOTemp = lstMicrobiADO.Where(o => !string.IsNullOrEmpty(o.SPECIMENS)
																|| (o.IMPLANTION_TIME != null && o.IMPLANTION_TIME > 0)
																|| (o.RESULT_TIME != null && o.RESULT_TIME > 0)
																|| !string.IsNullOrEmpty(o.RESULT)
																).ToList();
						}
						foreach (var item in lstMicrobiADOTemp)
						{
							HIS_ANTIBIOTIC_MICROBI obj = new HIS_ANTIBIOTIC_MICROBI();
							obj.SPECIMENS = item.SPECIMENS;
							if (item.IMPLANTION_TIME != null && item.IMPLANTION_TIME.ToString().Length < 9)
								obj.IMPLANTION_TIME = Int64.Parse(item.IMPLANTION_TIME.ToString() + "000000");
							else
								obj.IMPLANTION_TIME = item.IMPLANTION_TIME;
							if (item.RESULT_TIME != null && item.RESULT_TIME.ToString().Length < 9)
								obj.RESULT_TIME = Int64.Parse(item.RESULT_TIME.ToString() + "000000");
							else
								obj.RESULT_TIME = item.RESULT_TIME;
							obj.RESULT = item.RESULT;
							this.currentAntibioticMicrobi.Add(obj);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewAntibioticMicrobi_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
		{
			try
			{
				if (e.Column.FieldName == "BtnDeleteAntibioticMicrobi")
				{
					int rowSelected = Convert.ToInt32(e.RowHandle);
					int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewAntibioticMicrobi.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
					if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
					{
						e.RepositoryItem = btnAddAntibioticMicrobi;
					}
					else
					{
						e.RepositoryItem = btnDeleteAntibioticMicrobi;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewAntibioticMicrobi_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
			{
				AntibioticMicrobiADO dataRow = (AntibioticMicrobiADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				if (e.Column.FieldName == "IMPLANTION_TIME_STR")
				{
					e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.IMPLANTION_TIME ?? 0);
				}
				else if (e.Column.FieldName == "RESULT_TIME_STR")
				{
					e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.RESULT_TIME ?? 0);
				}
			}
		}

		private void gridViewAntibioticMicrobi_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
		{
			try
			{
				if (e.ColumnName == "SPECIMENS" || e.ColumnName == "RESULT")
				{
					this.gridViewAntibioticMicrobi_CustomRowError(sender, e);
				}

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewAntibioticMicrobi_CustomRowError(object sender, RowColumnErrorEventArgs e)
		{
			try
			{
				var index = this.gridViewAntibioticMicrobi.GetDataSourceRowIndex(e.RowHandle);
				if (index < 0)
				{
					e.Info.ErrorType = ErrorType.None;
					e.Info.ErrorText = "";
					return;
				}
				var listDatas = this.gridControlAntibioticMicrobi.DataSource as List<AntibioticMicrobiADO>;
				var row = listDatas[index];
				if (e.ColumnName == "SPECIMENS")
				{
					if (row.ErrorTypeSpecimens == ErrorType.Warning)
					{
						e.Info.ErrorType = (ErrorType)(row.ErrorTypeSpecimens);
						e.Info.ErrorText = (string)(row.ErrorMessageSpecimens);
					}
					else
					{
						e.Info.ErrorType = (ErrorType)(ErrorType.None);
						e.Info.ErrorText = "";
					}
				}
				else if (e.ColumnName == "RESULT")
				{
					if (row.ErrorTypeResult == ErrorType.Warning)
					{
						e.Info.ErrorType = (ErrorType)(row.ErrorTypeResult);
						e.Info.ErrorText = (string)(row.ErrorMessageResult);
					}
					else
					{
						e.Info.ErrorType = (ErrorType)(ErrorType.None);
						e.Info.ErrorText = "";
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridViewAntibioticMicrobi_CellValueChanged(object sender, CellValueChangedEventArgs e)
		{
			try
			{
				var AntibioticMicrobiADO = (AntibioticMicrobiADO)this.gridViewAntibioticMicrobi.GetFocusedRow();
				if (AntibioticMicrobiADO != null)
				{
					if (e.Column.FieldName == "SPECIMENS"
						|| e.Column.FieldName == "RESULT"
						)
					{
						ValidMicrobiProcessing(AntibioticMicrobiADO);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidMicrobiProcessing(AntibioticMicrobiADO antibioticMicrobiADO)
		{
			try
			{
				if (antibioticMicrobiADO != null)
				{
					if (!String.IsNullOrEmpty(antibioticMicrobiADO.SPECIMENS) && Inventec.Common.String.CountVi.Count(antibioticMicrobiADO.SPECIMENS) > 500)
					{

						antibioticMicrobiADO.ErrorMessageSpecimens = "Vượt quá độ dài cho phép 500 ký tự";
						antibioticMicrobiADO.ErrorTypeSpecimens = ErrorType.Warning;
					}
					else
					{
						antibioticMicrobiADO.ErrorMessageSpecimens = "";
						antibioticMicrobiADO.ErrorTypeSpecimens = ErrorType.None;
					}
					if (!String.IsNullOrEmpty(antibioticMicrobiADO.RESULT) && Inventec.Common.String.CountVi.Count(antibioticMicrobiADO.RESULT) > 1000)
					{

						antibioticMicrobiADO.ErrorMessageResult = "Vượt quá độ dài cho phép 1000 ký tự";
						antibioticMicrobiADO.ErrorTypeResult = ErrorType.Warning;
					}
					else
					{
						antibioticMicrobiADO.ErrorMessageResult = "";
						antibioticMicrobiADO.ErrorTypeResult = ErrorType.None;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnAddAntibioticMicrobi_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				List<AntibioticMicrobiADO> MicrobiADOs = new List<AntibioticMicrobiADO>();
				var MicrobiADO = gridControlAntibioticMicrobi.DataSource as List<AntibioticMicrobiADO>;
				if (MicrobiADO == null || MicrobiADO.Count < 1)
				{
					AntibioticMicrobiADO ekipUserAdoTemp = new AntibioticMicrobiADO();
					MicrobiADOs.Add(ekipUserAdoTemp);
					MicrobiADOs.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
					MicrobiADOs.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
					gridControlAntibioticMicrobi.DataSource = null;
					gridControlAntibioticMicrobi.DataSource = MicrobiADOs;
				}
				else
				{
					AntibioticMicrobiADO participant = new AntibioticMicrobiADO();
					MicrobiADO.Add(participant);
					MicrobiADO.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
					MicrobiADO.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
					gridControlAntibioticMicrobi.DataSource = null;
					gridControlAntibioticMicrobi.DataSource = MicrobiADO;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnDeleteAntibioticMicrobi_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				CommonParam param = new CommonParam();
				var MicrobiADOs = gridControlAntibioticMicrobi.DataSource as List<AntibioticMicrobiADO>;
				var MicrobiADO = (AntibioticMicrobiADO)gridViewAntibioticMicrobi.GetFocusedRow();
				if (MicrobiADO != null)
				{
					if (MicrobiADOs.Count > 0)
					{
						MicrobiADOs.Remove(MicrobiADO);
						MicrobiADOs.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
						MicrobiADOs.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
						gridControlAntibioticMicrobi.DataSource = null;
						gridControlAntibioticMicrobi.DataSource = MicrobiADOs;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtImplaneTime_EditValueChanged(object sender, EventArgs e)
		{

			try
			{
				AntibioticMicrobiADO data = (AntibioticMicrobiADO)gridViewAntibioticMicrobi.GetFocusedRow();
				if (data != null)
				{

					TextEdit txt = sender as TextEdit;

					if (txt.Text != null && txt.Text != "")
					{

						if (txt.Text.Contains("/"))
						{
							var dt = txt.Text.Replace("_", "").Split('/');
							data.IMPLANTION_TIME = Int64.Parse(dt[2].Trim() + dt[1].Trim() + dt[0].Trim());
						}
					}
					else
					{
						data.IMPLANTION_TIME = null;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}


		}

		private void txtResultTime_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				AntibioticMicrobiADO data = (AntibioticMicrobiADO)gridViewAntibioticMicrobi.GetFocusedRow();
				if (data != null)
				{

					TextEdit txt = sender as TextEdit;

					if (txt.Text != null && txt.Text != "")
					{

						if (txt.Text.Contains("/"))
						{
							var dt = txt.Text.Replace("_", "").Split('/');
							data.RESULT_TIME = Int64.Parse(dt[2].Trim() + dt[1].Trim() + dt[0].Trim());
						}
					}
					else
					{
						data.RESULT_TIME = null;
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
