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
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.HisCarerCardBorrow.Validation;
using HIS.Desktop.Plugins.HisCarerCardBorrow.ADO;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.HisCarerCardBorrow.Popup
{
	public partial class frmGiveBack : Form
	{
		private int positionHandleControl = -1;
		private List<AcsUserADO> lstReAcsUserADO;
		private RefeshReference delegateFrm { get; set; }
		private V_HIS_CARER_CARD_BORROW currentObj { get; set; }
		Inventec.Desktop.Common.Modules.Module moduleData { get; set; }
		public frmGiveBack(RefeshReference refesh, V_HIS_CARER_CARD_BORROW obj, Inventec.Desktop.Common.Modules.Module moduleData)
		{
			InitializeComponent();
			try
			{
				this.moduleData = moduleData;
				this.currentObj = obj;
				this.delegateFrm = refesh;
				string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
				this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void frmGiveBack_Load(object sender, EventArgs e)
		{
			try
			{
				ValidateTime();
				ValidateUser();
				LoadUser();
				SetDefaultValue();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SetDefaultValue()
		{
			try
			{
				dteGiveBack.DateTime = DateTime.Now;
				cboUser.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadUser()
		{
			try
			{
				this.lstReAcsUserADO = new List<AcsUserADO>();

				foreach (var item in Base.GlobalStore.HisAcsUser)
				{
					AcsUserADO ado = new AcsUserADO(item);

					var VhisEmployee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
					if (VhisEmployee != null)
					{
						this.lstReAcsUserADO.Add(ado);
					}
				}
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
				columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 400);
				ControlEditorLoader.Load(cboUser, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
				cboUser.Properties.ImmediatePopup = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidateUser()
		{
			try
			{
				ValidationGridTextControls valid = new ValidationGridTextControls();
				valid.txt = txtLoginName;
				valid.cbo = cboUser;
				valid.ErrorText = "Trường dữ liệu bắt buộc";
				valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
				dxValidationProvider1.SetValidationRule(txtLoginName, valid);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ValidateTime()
		{
			try
			{
				ValidationDate valid = new ValidationDate();
				valid.dte = dteGiveBack;
				valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
				dxValidationProvider1.SetValidationRule(dteGiveBack, valid);
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
				positionHandleControl = -1;
				if (!dxValidationProvider1.Validate()) return;
				if (currentObj.BORROW_TIME > 0 && (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteGiveBack.DateTime) ?? 0) < currentObj.BORROW_TIME)
				{
					DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian trả thẻ không được nhỏ hơn thời gian mượn thẻ: " + Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentObj.BORROW_TIME), "Thông báo");
					return;
				}
				CommonParam param = new CommonParam();
				HisCarerCardBorrowGiveBackSDO sdo = new HisCarerCardBorrowGiveBackSDO();
				sdo.CarerCardBorrowId = currentObj.ID;
				sdo.GiveBackTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteGiveBack.DateTime) ?? 0;
				sdo.ReceivingLoginName = cboUser.EditValue.ToString();
				sdo.ReceivingUserName = cboUser.Text.ToString();
				sdo.RequestRoomId = moduleData.RoomId;
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
				var rs = new BackendAdapter(param).Post<HIS_CARER_CARD_BORROW>("api/HisCarerCardBorrow/GiveBack", ApiConsumers.MosConsumer, sdo, param);
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
				if (rs != null)
				{
					if (delegateFrm != null)
						delegateFrm();
					this.Close();
					WaitingManager.Hide();
				}
				WaitingManager.Hide();
				MessageManager.Show(this.ParentForm, param, rs != null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			btnSave_Click(null, null);
		}

		private void cboUser_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (cboUser.EditValue != null)
				{
					var checkUser = this.lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == cboUser.EditValue.ToString());
					if (checkUser != null)
					{
						txtLoginName.Text = checkUser.LOGINNAME;
						dteGiveBack.Focus();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{

			try
			{
				if (e.KeyData == Keys.Enter)
					if (!string.IsNullOrEmpty(txtLoginName.Text))
					{
						var checkUser = this.lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == txtLoginName.Text.Trim());
						if (checkUser != null)
						{
							cboUser.EditValue = checkUser.LOGINNAME;
							dteGiveBack.Focus();
						}
						else
						{
							cboUser.ShowPopup();
						}
					}
					else
					{
						cboUser.Focus();
						cboUser.ShowPopup();
					}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboUser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyData == Keys.Enter)
					if (cboUser.EditValue == null)
					{
						cboUser.ShowPopup();
					}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
