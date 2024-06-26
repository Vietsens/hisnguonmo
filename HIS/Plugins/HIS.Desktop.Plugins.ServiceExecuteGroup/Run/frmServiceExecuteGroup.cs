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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ServiceExecuteGroup.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;

namespace HIS.Desktop.Plugins.ServiceExecuteGroup.Run
{
	public partial class frmServiceExecuteGroup : FormBase
	{
		private Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
		private Common.RefeshReference delegateRefresh { get; set; }
		private List<L_HIS_SERVICE_REQ> lstServiceReqSend { get; set; }
		private List<ResultADO> lstResultError { get; set; }
		private int positionHandleControl = -1;
		private int totalServiceReq = 0;
		private int indexServiceReq = 0;
		private List<V_HIS_SERVICE_REQ> lstServiceReq { get; set; }
		private L_HIS_SERVICE_REQ currentServiceReq { get; set; }
		private List<HIS_SERE_SERV> lstSereServ { get; set; }
		private List<HIS_SERE_SERV_EXT> lstSereServExt { get; set; }
		private enum Choose
		{
			SAVE,
			CANCEL
		}

		bool? paramSucess { get; set; }
		bool? paramSucessTemp { get; set; }
		private Choose? currentChoose { get; set; }
		public frmServiceExecuteGroup(Inventec.Desktop.Common.Modules.Module currentModule, Common.RefeshReference delegateRefresh, List<L_HIS_SERVICE_REQ> lst) : base(currentModule)
		{
			InitializeComponent();
			try
			{

				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lst), lst));
				this.currentModule = currentModule;
				this.delegateRefresh = delegateRefresh;
				this.lstServiceReqSend = lst;
				string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
				this.Icon = Icon.ExtractAssociatedIcon(iconPath);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void frmServiceExecuteGroup_Load(object sender, EventArgs e)
		{
			try
			{
				SetCaptionByLanguageKey();
				SetDefaultValueControl();
				SetValidateForm();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SetCaptionByLanguageKey()
		{
			try
			{
				Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ServiceExecuteGroup.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceExecuteGroup.Run.frmServiceExecuteGroup).Assembly);

				this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmServiceExecuteGroup.dteStart.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmServiceExecuteGroup.dteEnd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmServiceExecuteGroup.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmServiceExecuteGroup.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmServiceExecuteGroup.btnEnd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmServiceExecuteGroup.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmServiceExecuteGroup.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmServiceExecuteGroup.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SetDefaultValueControl()
		{
			try
			{
				memNote.Text = string.Empty;
				memDescription.Text = string.Empty;
				memConclude.Text = string.Empty;
				dteStart.DateTime = DateTime.Now;
				dteEnd.DateTime = DateTime.Now;
				gridControl1.DataSource = null;
				lblProcess.Text = "Đã xử lý: 0/" + ((lstServiceReqSend != null && lstServiceReqSend.Count > 0) ? lstServiceReqSend.Count.ToString() : "0");
				pbProcess.Maximum = (lstServiceReqSend != null && lstServiceReqSend.Count > 0) ? lstServiceReqSend.Count : 0;
				pbProcess.Value = 0;
				lstResultError = new List<ResultADO>();
				currentChoose = null;
				EnablePrint();
				LoadData();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadData()
		{
			try
			{
				if (lstServiceReqSend == null || lstServiceReqSend.Count == 0) return;
				var lstServiceReqSendNotHT = lstServiceReqSend.Where(o => o.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT);
				if (lstServiceReqSendNotHT != null && lstServiceReqSendNotHT.Count() > 0)
				{
					CommonParam param = new CommonParam();

					HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
					sereServFilter.SERVICE_REQ_IDs = lstServiceReqSendNotHT.Select(o => o.ID).ToList();
					lstSereServ = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, param);

					if (lstSereServ != null && lstSereServ.Count > 0)
					{
						HisSereServExtFilter sereServExtFilter = new HisSereServExtFilter();
						sereServExtFilter.SERE_SERV_IDs = lstSereServ.Select(o => o.ID).ToList();
						lstSereServExt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServExtFilter, param);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				if (lstServiceReqSend == null || lstServiceReqSend.Count == 0)
					return;
				positionHandleControl = -1;
				if (!dxValidationProvider1.Validate()) return;
				if (Convert.ToInt64(dteStart.DateTime.ToString("yyyyMMddHHmm")) >= Convert.ToInt64(dteEnd.DateTime.ToString("yyyyMMddHHmm")))
				{
					DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc", "Thông báo");
					return;
				}
				pbProcess.Value = 0;
				indexServiceReq = 0;
				totalServiceReq = lstServiceReqSend.Count;
				pbProcess.Maximum = totalServiceReq;
				currentChoose = Choose.SAVE;
				lstResultError = new List<ResultADO>();
				gridControl1.DataSource = lstResultError;
				lblProcess.Text = "Đã xử lý: 0/" + totalServiceReq;
				backgroundWorker1.RunWorkerAsync();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
		{
			try
			{
				BaseEdit edit = e.InvalidControl as BaseEdit;
				if (edit == null)
					return;

				BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
				if (viewInfo == null)
					return;

				if (positionHandleControl == -1)
				{
					positionHandleControl = edit.TabIndex;
					if (edit.Visible)
					{
						edit.SelectAll();
						edit.Focus();
					}
				}
				if (positionHandleControl > edit.TabIndex)
				{
					positionHandleControl = edit.TabIndex;
					if (edit.Visible)
					{
						edit.SelectAll();
						edit.Focus();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				switch (currentChoose)
				{
					case Choose.SAVE:
						SetDataToSave();
						break;
					case Choose.CANCEL:
						SetDataToCancel();
						break;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			try
			{
				btnSave.Enabled = false;
				btnCancel.Enabled = false;
				pbProcess.Value = indexServiceReq;
				lblProcess.Text = "Đã xử lý: " + indexServiceReq + "/" + totalServiceReq;
				if (lstResultError != null && lstResultError.Count > 0)
				{
					gridControl1.DataSource = null;
					gridControl1.DataSource = lstResultError;
					gridView1.FocusedRowHandle = lstResultError.Count - 1;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			try
			{
				EnablePrint();
				indexServiceReq = totalServiceReq;
				lblProcess.Text = "Đã xử lý: " + indexServiceReq + "/" + totalServiceReq;
				pbProcess.Value = indexServiceReq;
				if (lstResultError != null && lstResultError.Count > 0)
				{
					gridControl1.DataSource = null;
					gridControl1.DataSource = lstResultError;
					gridView1.FocusedRowHandle = lstResultError.Count - 1;
				}
				btnSave.Enabled = true;
				btnCancel.Enabled = true;
				bool success = false;
				if (paramSucess != null && paramSucessTemp != null)
				{
					success = (paramSucess ?? false) && (paramSucessTemp ?? false);
				}
				else if (paramSucess == null && paramSucessTemp != null)
				{
					success = paramSucessTemp ?? false;
				}
				else if (paramSucess != null && paramSucessTemp == null)
				{
					success = paramSucess ?? false;
				}
				if (success)
					MessageManager.Show(this.ParentForm, new CommonParam(), success);
				else
					DevExpress.XtraEditors.XtraMessageBox.Show("Xảy ra lỗi trong quá trình trả kết quả", "Thông báo");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SetDataToSave()
		{
			try
			{
				foreach (var item in lstServiceReqSend)
				{
					if (item.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
					{
						paramSucessTemp = true;
						continue;
					}
					if (item.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
					{
						L_HIS_SERVICE_REQ serviceReqResult = new BackendAdapter(new CommonParam())
			  .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_START, ApiConsumers.MosConsumer, item.ID, null);
						if (serviceReqResult != null)
							item.SERVICE_REQ_STT_ID = serviceReqResult.SERVICE_REQ_STT_ID;
						if (this.delegateRefresh != null)
						{
							delegateRefresh();
						}
					}
					string api = "";
					CommonParam param = new CommonParam();
					HisSereServExtSDO data = new HisSereServExtSDO();
					data.HisSereServExt = new HIS_SERE_SERV_EXT();
					data.HisSereServExt.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteStart.DateTime);
					data.HisSereServExt.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteEnd.DateTime);
					data.HisSereServExt.DESCRIPTION = memDescription.Text;
					data.HisSereServExt.CONCLUDE = memConclude.Text;
					data.HisSereServExt.NOTE = memNote.Text;

					var checkSereServ = lstSereServ.Where(o => o.SERVICE_REQ_ID == item.ID).ToList();

					if (checkSereServ != null && checkSereServ.Count > 0)
					{
						foreach (var ssItem in checkSereServ)
						{
							var checkSereServExt = lstSereServExt.Where(o => o.SERE_SERV_ID == ssItem.ID).ToList();
							if (checkSereServExt != null && checkSereServExt.Count > 0)
							{
								api = "api/HisSereServExt/UpdateSdo";
								foreach (var seItem in checkSereServExt)
								{
									seItem.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteStart.DateTime);
									seItem.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteEnd.DateTime);
									seItem.DESCRIPTION = memDescription.Text;
									seItem.CONCLUDE = memConclude.Text;
									seItem.NOTE = memNote.Text;
									data.HisSereServExt = seItem;
									backgroundWorker1.ReportProgress(indexServiceReq);
									MOS.SDO.HisSereServExtWithFileSDO apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
			  <MOS.SDO.HisSereServExtWithFileSDO>
			  (api, ApiConsumer.ApiConsumers.MosConsumer, data, param);
									if (apiResult != null)
									{
										paramSucess = true;
									}
									else
									{
										paramSucess = false;
										ResultADO ado = new ResultADO();
										ado.SERVICE_REQ_CODE = item.SERVICE_REQ_CODE;
										ado.SERVICE_NAME = ssItem.TDL_SERVICE_NAME;
										string mesError = String.Empty;
										if (!string.IsNullOrEmpty(param.GetBugCode()))
											mesError = param.GetBugCode();
										if (!string.IsNullOrEmpty(param.GetMessage()))
											mesError += !string.IsNullOrEmpty(mesError) ? " - " + param.GetMessage() : param.GetMessage();
										if (string.IsNullOrEmpty(mesError.Trim()))
										{
											ado.DescriptionError = "Xử lý thất bại.";
										}
										else
										{
											ado.DescriptionError = mesError;
										}
										lstResultError.Add(ado);
									}
								}
							}
							else
							{
								backgroundWorker1.ReportProgress(indexServiceReq);
								data.HisSereServExt.SERE_SERV_ID = ssItem.ID;
								api = "api/HisSereServExt/CreateSdo";
								MOS.SDO.HisSereServExtWithFileSDO apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
		  <MOS.SDO.HisSereServExtWithFileSDO>
		  (api, ApiConsumer.ApiConsumers.MosConsumer, data, param);
								if (apiResult != null)
								{
									paramSucess = true;
								}
								else
								{
									paramSucess = false;
									ResultADO ado = new ResultADO();
									ado.SERVICE_REQ_CODE = item.SERVICE_REQ_CODE;
									ado.SERVICE_NAME = ssItem.TDL_SERVICE_NAME;
									string mesError = String.Empty;
									if (!string.IsNullOrEmpty(param.GetBugCode()))
										mesError = param.GetBugCode();
									if (!string.IsNullOrEmpty(param.GetMessage()))
										mesError += !string.IsNullOrEmpty(mesError) ? " - " + param.GetMessage() : param.GetMessage();
									if (string.IsNullOrEmpty(mesError.Trim()))
									{
										ado.DescriptionError = "Xử lý thất bại.";
									}
									else
									{
										ado.DescriptionError = mesError;
									}
									lstResultError.Add(ado);
								}
							}
						}
					}
					indexServiceReq++;
				}
			}
			catch (Exception ex)
			{
				btnSave.Enabled = true;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SetDataToCancel()
		{
			try
			{
				foreach (var item in lstServiceReqSend)
				{
					if (item.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
					{
						paramSucessTemp = true;
						backgroundWorker1.ReportProgress(indexServiceReq);
						indexServiceReq++;
						continue;
					}

					CommonParam param = new CommonParam();
					backgroundWorker1.ReportProgress(indexServiceReq);
					var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_FINISH, ApiConsumer.ApiConsumers.MosConsumer, item.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
					indexServiceReq++;
					if (result != null)
					{
						paramSucess = true;
						item.SERVICE_REQ_STT_ID = result.SERVICE_REQ_STT_ID;
						if (this.delegateRefresh != null)
						{
							delegateRefresh();
						}
					}
					else
					{
						paramSucess = false;
						ResultADO ado = new ResultADO();
						ado.SERVICE_REQ_CODE = item.SERVICE_REQ_CODE;
						ado.SERVICE_NAME = String.Empty;
						string mesError = String.Empty;
						if (!string.IsNullOrEmpty(param.GetBugCode()))
							mesError = param.GetBugCode();
						if (!string.IsNullOrEmpty(param.GetMessage()))
							mesError += !string.IsNullOrEmpty(mesError) ? " - " + param.GetMessage() : param.GetMessage();

						if (string.IsNullOrEmpty(mesError.Trim()))
						{
							ado.DescriptionError = "Xử lý thất bại.";
						}
						else
						{
							ado.DescriptionError = mesError;
						}
						lstResultError.Add(ado);
					}


				}
			}
			catch (Exception ex)
			{
				btnSave.Enabled = true;
				btnCancel.Enabled = true;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{

			try
			{
				pbProcess.Value = 0;
				indexServiceReq = 0;
				totalServiceReq = lstServiceReqSend.Count;
				pbProcess.Maximum = totalServiceReq;
				currentChoose = Choose.CANCEL;
				lstResultError = new List<ResultADO>();
				gridControl1.DataSource = lstResultError;
				lblProcess.Text = "Đã xử lý: 0/" + totalServiceReq;
				backgroundWorker1.RunWorkerAsync();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void EnablePrint()
		{
			try
			{
				if (lstServiceReqSend != null && lstServiceReqSend.Count > 0)
				{
					var dtCheckList = lstServiceReqSend.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL || o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT).ToList();
					if (dtCheckList != null && dtCheckList.Count > 0)
						btnPrint.Enabled = true;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnPrint_Click(object sender, EventArgs e)
		{
			try
			{
				foreach (var item in lstServiceReqSend.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL || o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT).ToList())
				{
					currentServiceReq = item;
					Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
					richStore.RunPrintTemplate("Mps000471", this.DelegateRunPrinter);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (btnPrint.Enabled)
					btnPrint_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (btnSave.Enabled)
					btnSave_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (btnCancel.Enabled)
					btnCancel_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
