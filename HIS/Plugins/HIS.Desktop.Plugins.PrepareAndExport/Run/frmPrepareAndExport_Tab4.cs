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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using DevExpress.XtraGrid.Views.Base;
using MOS.EFMODEL.DataModels;
using DevExpress.Data;
using System.Collections;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;

namespace HIS.Desktop.Plugins.PrepareAndExport.Run
{
	public partial class frmPrepareAndExport
	{
		private async Task LoadTab4()
		{
			try
			{
				Action myaction = () =>
				{
					lstTab4 = lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && o.IS_ABSENT == 1).OrderBy(o => o.LAST_APPROVAL_TIME).ToList();
				};
				Task task = new Task(myaction);
				task.Start();
				await task;
				gcAbssentN.DataSource = null;
				if (lstTab4 != null && lstTab4.Count > 0)
				{
					gcAbssentN.DataSource = lstTab4;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}


		private void repPlusN_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			CommonParam param = new CommonParam();
			MOS.EFMODEL.DataModels.HIS_EXP_MEST rs;
			bool success = false;
			try
			{

				HIS_EXP_MEST data = (HIS_EXP_MEST)gvAbssentN.GetFocusedRow();
				if (data!=null)
				{
					HisExpMestSDO sdo = new HisExpMestSDO();
					sdo.ExpMestId = data.ID;
					sdo.ReqRoomId = this.currentModule.RoomId;
					WaitingManager.Show();
					string api = String.Empty;
					switch (data.EXP_MEST_TYPE_ID)
					{
						case IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK:
							api = "api/HisExpMest/AggrExamExport";
							break;
						case IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL:
							api = "api/HisExpMest/AggrExport";
							break;
					}
					rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(api, ApiConsumers.MosConsumer, sdo, param);
					WaitingManager.Hide();
					if (rs!=null)
					{
						foreach (var item in lstAll)
						{
							if (item.ID == rs.ID)
							{
								item.EXP_MEST_STT_ID = rs.EXP_MEST_STT_ID;
								if (lstTab5 == null || lstTab5.Count == 0)
									lstTab5 = new List<HIS_EXP_MEST>();
								lstTab5.Add(item);
								gcPassMedicine.DataSource = null;
								gcPassMedicine.DataSource = lstTab5;
								if (!string.IsNullOrEmpty(txtGateCodeString) && dteStt.DateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
								{
									CreateThreadCallPatientRefresh();
								}
								break;
							}
						}
						success = true;
						LoadTab4();
					}
					MessageManager.Show(this.ParentForm, param, success);
				}

			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}


		private void btnScreenWaiting_Click(object sender, EventArgs e)
		{
			try
			{
				
					Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PrescriptionAbsentList").FirstOrDefault();
					if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.PrescriptionAbsentList");
					if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
					{
						List<object> listArgs = new List<object>();
						
						var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
						if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
						((Form)extenceInstance).ShowDialog();
					}
				
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}


		private void gvAbssentN_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{

				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					HIS_EXP_MEST pData = (HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
					if (e.Column.FieldName == "STT")
					{
						e.Value = e.ListSourceRowIndex + 1;
					}
					else if (e.Column.FieldName == "DOB_str")
					{
						if (pData.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
						{
							e.Value = pData.TDL_PATIENT_DOB.ToString().Substring(0, 4);
						}
						else
						{
							e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.TDL_PATIENT_DOB ?? 0);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}


		private void gvAbssentN_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				if (e.RowHandle >= 0)
				{
					long? priority = (long?)view.GetRowCellValue(e.RowHandle, "PRIORITY");
					if (priority != null & priority == 1)
						e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
