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
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using System.Threading;
using MOS.SDO;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.Data;

namespace HIS.Desktop.Plugins.PrepareAndExport.Run
{
	public partial class frmPrepareAndExport
	{
		private async Task LoadTab3()
		{
			try
			{
				Action myaction = () =>
				{
					lstTab3 = lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && o.IS_ABSENT != 1).OrderBy(o => o.LAST_APPROVAL_TIME).ToList();
				};
				Task task = new Task(myaction);
				task.Start();
				await task;
				gcPrepareMedicine.DataSource = null;
				if (lstTab3 != null && lstTab3.Count > 0)
				{
					gcPrepareMedicine.DataSource = lstTab3;
				}
				Inventec.Common.Logging.LogSystem.Debug("QUẦY __" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => txtGateCodeString), txtGateCodeString));
				Inventec.Common.Logging.LogSystem.Debug("IP __" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => txtIpCPA), txtIpCPA));
				if (!string.IsNullOrEmpty(txtGateCodeString) && dteStt.DateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
				{
					CreateThreadCallPatientRefresh();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void simpleButton1_Click(object sender, EventArgs e)
		{
			try
			{
				frmConfig frm = new frmConfig(IsOpen,GateConfig,IpConfig);
				frm.ShowDialog();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void IpConfig(string obj)
		{
			try
			{
				txtIpCPA = obj;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void GateConfig(string obj)
		{
			try
			{
				txtGateCodeString = obj;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void IsOpen(bool obj)
		{
			if(obj)
			{
				//CreateThreadCallPatientRefresh();				
			}	
		}

		private void btnAbsent_Click(object sender, EventArgs e)
		{
			CommonParam param = new CommonParam();
			MOS.EFMODEL.DataModels.HIS_EXP_MEST rs;
			bool success = false;
			try
			{
				HIS_EXP_MEST data = currentCall;
				if (data != null)
				{
					HisExpMestSDO sdo = new HisExpMestSDO();
					sdo.ExpMestId = data.ID;
					sdo.ReqRoomId = this.currentModule.RoomId;
					WaitingManager.Show();
					success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisExpMest/Absent", ApiConsumers.MosConsumer, sdo, param);
					WaitingManager.Hide();
					if (success)
					{
						foreach (var item in lstAll)
						{
							if (item.ID == data.ID)
							{
								item.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
								item.IS_ABSENT = 1;
								if (lstTab4 == null || lstTab4.Count == 0)
									lstTab4 = new List<HIS_EXP_MEST>();
								lstTab4.Add(item);
								gcAbssentN.DataSource = null;
								gcAbssentN.DataSource = lstTab4;
								break;
							}
						}
						LoadTab3();
						currentCall = null;
						txtCurrentCall.Text = "";
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

		private void btnGaveMedicine_Click(object sender, EventArgs e)
		{
			CommonParam param = new CommonParam();
			MOS.EFMODEL.DataModels.HIS_EXP_MEST rs;
			bool success = false;
			try
			{
				HIS_EXP_MEST data = currentCall;
				if (data != null)
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
					if (rs != null)
					{
						foreach (var item in lstAll)
						{
							if (item.ID == rs.ID)
							{
								item.EXP_MEST_STT_ID = rs.EXP_MEST_STT_ID;
								item.IS_ABSENT = null;
								if (lstTab5 == null || lstTab5.Count == 0)
									lstTab5 = new List<HIS_EXP_MEST>();
								lstTab5.Add(item);
								gcPassMedicine.DataSource = null;
								gcPassMedicine.DataSource = lstTab5;
								break;
							}
						}
						success = true;
						LoadTab3();
						currentCall = null;
						txtCurrentCall.Text = "";
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

		private void btnCall_Click(object sender, EventArgs e)
		{
			try
			{
				if(string.IsNullOrEmpty(txtGateCodeString))
				{
					frmConfig frm = new frmConfig(IsOpen,GateConfig,IpConfig);
					frm.ShowDialog();
					return;
				}	
				if (currentCall != null)
				{
					if (this.clienttManager == null)
						this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager(txtIpCPA);
					bool rs = this.clienttManager.RecallOrderDataClientBool(currentCall.NUM_ORDER.ToString());
					Inventec.Common.Logging.LogSystem.Error("GỌI LẠI ___ " + rs);
				}
				else
				{
					Inventec.Common.Logging.LogSystem.Error("KẾT NỐI ___ ");
					//CreateThreadCallPatientCPA();
					CallPatientCPA();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void CreateThreadCallPatientCPA()
		{
			Thread thread = new System.Threading.Thread(CallPatientCPA);			
			try
			{
				thread.Start();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				thread.Abort();
			}
		}

		private void CallPatientCPA()
		{
			try
			{

					if (this.clienttManager == null)
						this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager(txtIpCPA);
					string rs = this.clienttManager.CallOrderDataString(txtGateCodeString,chkCallAll.Checked);
					Inventec.Common.Logging.LogSystem.Error("GỌI ___" + rs);
				if (rs == null)
					return;
				if (lstAll.FirstOrDefault(o => o.NUM_ORDER == Int64.Parse(rs)) == null || !chkAutoLoadTab.Checked)
				{
					LoadListDataSource();
					LoadAllTab();
				}
				currentCall = lstAll.FirstOrDefault(o => o.NUM_ORDER == Int64.Parse(rs));
                txtCurrentCall.Text = currentCall.NUM_ORDER + " - " + currentCall.TDL_PATIENT_NAME + " - " + currentCall.TDL_TREATMENT_CODE;					
					if (rs != currentCall.GATE_CODE)
					{
						CommonParam param = new CommonParam();
						bool success;
						if (currentCall != null)
						{
							ExpMestCallSDO sdo = new ExpMestCallSDO();
							sdo.ExpMestId = currentCall.ID;
							sdo.GateCode = txtGateCodeString;
							WaitingManager.Show();
							success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisExpMest/Call", ApiConsumers.MosConsumer, sdo, param);
							WaitingManager.Hide();
							if (success)
							{
								foreach (var item in lstAll)
								{
									if (item.ID == currentCall.ID)
									{
										item.GATE_CODE = txtGateCodeString;
										break;
									}
								}
								LoadTab3();
							}
							MessageManager.Show(this.ParentForm, param, success);
						}
					
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void chkCallAll_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkCallAll.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.PrepareAndExport").FirstOrDefault() : null;
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
				if (csAddOrUpdate != null)
				{
					csAddOrUpdate.VALUE = chkCallAll.Checked ? "1" : "0";
				}
				else
				{
					csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
					csAddOrUpdate.KEY = chkCallAll.Name;
					csAddOrUpdate.VALUE = chkCallAll.Checked ? "1" : "0";
					csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.PrepareAndExport";
					if (currentControlStateRDO == null)
						currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
					currentControlStateRDO.Add(csAddOrUpdate);
				}
				controlStateWorker.SetData(currentControlStateRDO);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		private void gvPrepareMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
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

	
        private void txtCurrentCall_TextChanged(object sender, EventArgs e)
        {
            try
            {
                btnAbsent.Enabled = !string.IsNullOrEmpty(txtCurrentCall.Text);
                btnGaveMedicine.Enabled = !string.IsNullOrEmpty(txtCurrentCall.Text);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

		private void gvPrepareMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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

		private void gvPrepareMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
		{
			try
			{
				if (e.RowHandle < 0)
					return;
				string gateCode = (gvPrepareMedicine.GetRowCellValue(e.RowHandle, "GATE_CODE") ?? "").ToString();
				if (e.Column.FieldName == "ReCall")
				{
					if (!string.IsNullOrEmpty(gateCode))
					{
						e.RepositoryItem = repReCall;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repReCall_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{

				HIS_EXP_MEST data = (HIS_EXP_MEST)gvPrepareMedicine.GetFocusedRow();
				if (this.clienttManager == null)
					this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager(txtIpCPA);
				bool rs = this.clienttManager.RecallOrderDataClientBool(data.NUM_ORDER.ToString());
				Inventec.Common.Logging.LogSystem.Error("GỌI LẠI TRÊN LƯỚI ___ " + rs);
				currentCall = data;
                txtCurrentCall.Text = currentCall.NUM_ORDER + " - " + currentCall.TDL_PATIENT_NAME + " - " + currentCall.TDL_TREATMENT_CODE;

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
