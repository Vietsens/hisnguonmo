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
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;

namespace HIS.Desktop.Plugins.HisCarerCardBorrow.Popup
{
	public partial class frmBorrow : Form
	{
		private int positionHandleControl = -1;
		private List<AcsUserADO> lstReAcsUserADO { get; set; }
		private HIS_TREATMENT currentTreatment { get; set; }
		private List<V_HIS_CARER_CARD> listCarerCard { get; set; }
		private List<V_HIS_CARER_CARD> listCarerCardAll { get; set; }
		private List<BorrowADO> lstBorrowADO = new List<BorrowADO>();
		private long idRow { get; set; }
		private RefeshReference delegateFrm { get; set; }
		Inventec.Desktop.Common.Modules.Module moduleData { get; set; }
		public frmBorrow(RefeshReference refeshReference, Inventec.Desktop.Common.Modules.Module moduleData)
		{
			InitializeComponent();
			try
			{
				this.moduleData = moduleData;
				this.delegateFrm = refeshReference;
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

		private void frmBorrow_Load(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				ValidateTime();
				ValidateUser();
				ValidatePatientName();
				ValidateCarerCard();
				LoadUser();
				LoadCarerCard();
				SetDefaultValue();
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void FillDataToControl()
		{
			try
			{
				CommonParam paramCommon = new CommonParam();
				HisTreatmentFilter filter = new HisTreatmentFilter();
				if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
				{
					string code = txtTreatmentCode.Text.Trim();
					if (code.Length < 12)
					{
						code = string.Format("{0:000000000000}", Convert.ToInt64(code));
						txtTreatmentCode.Text = code;
					}
					filter = new HisTreatmentFilter();
					filter.TREATMENT_CODE__EXACT = code;
				}
				else if (!string.IsNullOrEmpty(txtInCode.Text))
				{
					filter.IN_CODE__EXACT = txtInCode.Text.Trim();
				}
				else
					return;
				var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, paramCommon);
				if (result != null && result.Count > 0)
				{
					currentTreatment = result[0];
					lblPatientName.Text = currentTreatment.TDL_PATIENT_NAME;
					lblPatientGender.Text = currentTreatment.TDL_PATIENT_GENDER_NAME;
					if (currentTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
						lblPatientDob.Text = currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
					else
						lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentTreatment.TDL_PATIENT_DOB);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ValidatePatientName()
		{
			try
			{
				ValidationLabel valid = new ValidationLabel();
				valid.lbl = lblPatientName;
				valid.ErrorText = "Trường dữ liệu bắt buộc";
				valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
				//dxValidationProvider1.SetValidationRule(lblPatientName, valid);
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
		private void LoadCarerCard()
		{
			try
			{
				this.listCarerCard = new List<V_HIS_CARER_CARD>();
				this.listCarerCardAll = new List<V_HIS_CARER_CARD>();
				CommonParam paramCommon = new CommonParam();
				HisCarerCardViewFilter filter = new HisCarerCardViewFilter();
				filter.IS_BORROWED = false;
				filter.IS_LOST = false;
				filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
				 var listCarerCardTemp = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_CARER_CARD>>("api/HisCarerCard/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
				if(listCarerCardTemp != null && listCarerCardTemp.Count>0)
				{
					listCarerCardAll = listCarerCardTemp;
					listCarerCard = listCarerCardTemp;
				}
				InitCombo(listCarerCard);


			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitCombo(List<V_HIS_CARER_CARD> lst)
		{
			try
			{
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("CARER_CARD_NUMBER", "Số thẻ", 150, 1));
				ControlEditorADO controlEditorADO = new ControlEditorADO("CARER_CARD_NUMBER", "ID", columnInfos, false, 150);
				ControlEditorLoader.Load(cboCarerCard, lst, controlEditorADO);
				cboCarerCard.Properties.ImmediatePopup = true;
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

		private void ValidateCarerCard()
		{
			try
			{
				ValidationGridControls valid = new ValidationGridControls();
				valid.cbo = cboCarerCard;
				valid.ErrorText = "Trường dữ liệu bắt buộc";
				valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
				dxValidationProvider1.SetValidationRule(cboCarerCard, valid);
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
				valid.dte = dteBorrow;
				valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
				dxValidationProvider1.SetValidationRule(dteBorrow, valid);
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
				dteBorrow.DateTime = DateTime.Now;
				cboUser.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
				gridControl1.DataSource = null;
				gridControl1.DataSource = lstBorrowADO;
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
				if (string.IsNullOrEmpty(lblPatientName.Text))
				{
					DevExpress.XtraEditors.XtraMessageBox.Show("Thiếu thông tin họ tên bệnh nhân", "Thông báo");
					return;
				}
				if (lstBorrowADO == null || lstBorrowADO.Count == 0)
				{
					DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa nhập thông tin thẻ", "Thông báo");
					return;
				}
				CommonParam param = new CommonParam();
				HisCarerCardBorrowSDO sdo = new HisCarerCardBorrowSDO();
				sdo.TreatmentId = currentTreatment.ID;
				sdo.CarerCardInfos = new List<HisCarerCardSDOInfo>();
				foreach (var item in lstBorrowADO)
				{
					HisCarerCardSDOInfo info = new HisCarerCardSDOInfo();
					info.CarerCardId = item.CARER_CARD_ID;
					info.BorrowTime = item.TIME_FROM;
					sdo.CarerCardInfos.Add(info);
				}				
				sdo.GivingLoginName = cboUser.EditValue.ToString();
				sdo.GivingUserName = cboUser.Text.ToString();
				sdo.RequestRoomId = moduleData.RoomId;
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
				var rs = new BackendAdapter(param).Post<bool>("api/HisCarerCardBorrow/Borrow", ApiConsumers.MosConsumer, sdo, param);
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
				if (rs)
				{
					if (delegateFrm != null)
						delegateFrm();
					this.Close();
					WaitingManager.Hide();
				}
				WaitingManager.Hide();
				MessageManager.Show(this.ParentForm, param, rs);

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

		private void btnFind_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				FillDataToControl();
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboCarerCard_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (cboCarerCard.EditValue != null)
				{
					var dt = listCarerCard.FirstOrDefault(o => o.ID == Int64.Parse(cboCarerCard.EditValue.ToString()));
					if (dt != null)
					{
						lblServiceName.Text = dt.SERVICE_NAME;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			try
			{
				if (!dxValidationProvider1.Validate()) return;
				idRow++;
				BorrowADO ado = new BorrowADO();
				ado.ID_ROW = idRow;
				ado.LOGINNAME = cboUser.EditValue.ToString();
				ado.CARER_CARD_ID = Int64.Parse(cboCarerCard.EditValue.ToString());
				ado.CARER_CARD_NUMBER = listCarerCard.First(o => o.ID == ado.CARER_CARD_ID).CARER_CARD_NUMBER.ToString();
				ado.TIME_FROM = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteBorrow.DateTime) ?? 0;
				ado.SERVICE_NAME = lblServiceName.Text;
				ado.HIS_CARER_CARD = listCarerCard.First(o=>o.ID == ado.CARER_CARD_ID);
				lstBorrowADO.Add(ado);
				lstBorrowADO = lstBorrowADO.OrderBy(x => x.ID_ROW).ToList();
				gridControl1.DataSource = null;
				gridControl1.DataSource = lstBorrowADO;

				listCarerCard.Remove(listCarerCard.FirstOrDefault(o=>o.ID == Int64.Parse(cboCarerCard.EditValue.ToString())));
				lblServiceName.Text = String.Empty;
				cboCarerCard.EditValue = null;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
					var data = (BorrowADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
					if (e.Column.FieldName == "TIME_FROM_str")
					{
						e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.TIME_FROM);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repbtnDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
				var row = (BorrowADO)gridView1.GetFocusedRow();
				if (row != null)
				{				
					listCarerCard.Add(row.HIS_CARER_CARD);
					InitCombo(listCarerCard);
					lstBorrowADO.Remove(row);
					gridControl1.DataSource = null;
					gridControl1.DataSource = lstBorrowADO;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
		{
			try
			{

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboUser_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (cboUser.EditValue != null)
				{
					var dt = lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == cboUser.EditValue.ToString());
					if (dt != null)
					{
						txtLoginName.Text = dt.LOGINNAME;
						cboCarerCard.Focus();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if(e.KeyData == Keys.Enter)
					FillDataToControl();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtInCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (e.KeyData == Keys.Enter)
					FillDataToControl();
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
							cboCarerCard.Focus();
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

		private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				btnAdd_Click(null,null);
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
				btnFind_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
