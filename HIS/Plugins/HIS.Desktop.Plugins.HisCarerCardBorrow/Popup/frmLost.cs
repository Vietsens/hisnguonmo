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

namespace HIS.Desktop.Plugins.HisCarerCardBorrow.Popup
{
	public partial class frmLost : Form
	{
		private int positionHandleControl = -1;
		private RefeshReference delegateFrm { get; set; }
		private V_HIS_CARER_CARD_BORROW currentObj { get; set; }
		Inventec.Desktop.Common.Modules.Module moduleData { get; set; }
		public frmLost(RefeshReference refesh, V_HIS_CARER_CARD_BORROW obj, Inventec.Desktop.Common.Modules.Module moduleData)
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

		private void frmLost_Load(object sender, EventArgs e)
		{
			try
			{
				ValidateTime();
				SetDefaultValue();
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
				valid.dte = dteLost;
				valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
				dxValidationProvider1.SetValidationRule(dteLost, valid);
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
				dteLost.DateTime = DateTime.Now;
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
				if (currentObj.BORROW_TIME > 0 && (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteLost.DateTime) ?? 0) < currentObj.BORROW_TIME)
				{
					DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian mất thẻ không được nhỏ hơn thời gian mượn thẻ: " + Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentObj.BORROW_TIME), "Thông báo");
					return;
				}
				bool success = false;
				CommonParam param = new CommonParam();
				HisCarerCardBorrowLostSDO sdo = new HisCarerCardBorrowLostSDO();
				sdo.GiveBackTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteLost.DateTime) ?? 0;
				sdo.CarerCardBorrowId = currentObj.ID;
				sdo.RequestRoomId = moduleData.RoomId;

				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
				var rs = new BackendAdapter(param).Post<HIS_CARER_CARD_BORROW>("api/HisCarerCardBorrow/Lost", ApiConsumers.MosConsumer, sdo, param);
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
				if (rs != null)
				{
					success = true;
					if (delegateFrm != null)
						delegateFrm();
					this.Close();
					WaitingManager.Hide();
				}
				WaitingManager.Hide();
				MessageManager.Show(this.ParentForm, param, success);
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
	}
}
