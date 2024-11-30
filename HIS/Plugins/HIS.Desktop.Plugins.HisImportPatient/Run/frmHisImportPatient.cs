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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisImportPatient.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisImportPatient.PopUp;
namespace HIS.Desktop.Plugins.HisImportPatient.Run
{
	public partial class frmHisImportPatient : HIS.Desktop.Utility.FormBase
	{
        
		List<ACS_USER> listAcsUser;
		int positionHandleControl = -1;
		Inventec.Desktop.Common.Modules.Module currentModule;
		List<ImportPatientADO> currentAdos;
		List<ImportPatientADO> importAdos;
		List<HIS_CAREER> lstCareer;
		List<HIS_PATIENT_TYPE> lstPatientType;
		List<SDA_ETHNIC> lstEthnic;
		List<SDA_NATIONAL> lstNational;

		List<V_SDA_PROVINCE> lstProvince;
		List<V_SDA_DISTRICT> lstDistrict;
		List<V_SDA_COMMUNE> lstCommune;
		List<HIS_SERVICE> lstSerivce;
		List<V_HIS_ROOM> lstRoom;
		List<HIS_PRIORITY_TYPE> lstPriority;
		List<L_HIS_ROOM_COUNTER> dataExecuteRooms;
		bool checkClick;
		int index = 0;
		int total = 0;
		bool checkSuccess = false;
		CommonParam paramSuccess = new CommonParam();
		frmWaiting frmWaiting;
		Dictionary<long, string> DicMessage = new Dictionary<long, string>();
		public frmHisImportPatient(Inventec.Desktop.Common.Modules.Module module) : base(module)
		{
			InitializeComponent();
			try
			{
				this.currentModule = module;
				string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
				this.Icon = Icon.ExtractAssociatedIcon(iconPath);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void frmHisImportPatient_Load(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				ValidateForm();
				InitComboLogin();
				if (this.currentModule != null)
				{
					this.Text = this.currentModule.text;
				}
				btnSave.Enabled = false;
				btnShowLineError.Enabled = false;
				GetDataLocal();
				dataExecuteRooms = GetLCounter1();
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		public List<long> GetPatientTypeRoom1(long _patientTypeId)
		{
			List<long> _roomIdByPatientTypeRooms = new List<long>();
			try
			{
				MOS.Filter.HisPatientTypeRoomFilter _Filter = new MOS.Filter.HisPatientTypeRoomFilter();
				_Filter.PATIENT_TYPE_ID = _patientTypeId;
				_Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
				var datas = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ROOM>>("api/HisPatientTypeRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, _Filter, null);
				if (datas != null && datas.Count > 0)
				{
					_roomIdByPatientTypeRooms = datas.Select(p => p.ROOM_ID).Distinct().ToList();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return _roomIdByPatientTypeRooms;
		}


		private void InitComboLogin()
		{
			try
			{
				listAcsUser = BackendDataWorker.Get<ACS_USER>();
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("LOGINNAME", "Tài khoản", 100, 1));
				columnInfos.Add(new ColumnInfo("USERNAME", "Tên tài khoản", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 350);
				ControlEditorLoader.Load(cboLogin, listAcsUser, controlEditorADO);
				cboLogin.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
				txtLogin.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void ValidateForm()
		{
			try
			{
				ValidateLookupWithTextEdit(cboLogin, txtLogin);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

		}
		private void ValidateLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
		{
			try
			{
				GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
				validRule.txtTextEdit = textEdit;
				validRule.cbo = cbo;
				validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
				validRule.ErrorType = ErrorType.Warning;
				dxValidationProvider1.SetValidationRule(textEdit, validRule);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
		{
			try
			{
				BaseEdit edit = e.InvalidControl as BaseEdit;
				if (edit == null)
					return;

				BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
				if (viewInfo == null)
					return;

				if (this.positionHandleControl == -1)
				{
					this.positionHandleControl = edit.TabIndex;
					if (edit.Visible)
					{
						edit.SelectAll();
						edit.Focus();
					}
				}
				if (this.positionHandleControl > edit.TabIndex)
				{
					this.positionHandleControl = edit.TabIndex;
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

		private List<L_HIS_ROOM_COUNTER> GetLCounter1()
		{
			try
			{
				HisRoomCounterLViewFilter exetuteFilter = new HisRoomCounterLViewFilter();
				exetuteFilter.IS_EXAM = true;
				exetuteFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
				exetuteFilter.BRANCH_ID = WorkPlace.GetBranchId();
				return new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<L_HIS_ROOM_COUNTER>>("api/HisRoom/GetCounterLView", ApiConsumers.MosConsumer, exetuteFilter, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return null;
		}
		private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				if (e.RowHandle >= 0)
				{
					ImportPatientADO data = (ImportPatientADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
					if (e.Column.FieldName == "ErrorLine")
					{
						if (!string.IsNullOrEmpty(data.ERROR))
						{
							e.RepositoryItem = bbtnInfor;
						}
					}
					else if (e.Column.FieldName == "Delete")
					{
						e.RepositoryItem = bbtnDelete;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnDownload_Click(object sender, EventArgs e)
		{
			try
			{
				var source = System.IO.Path.Combine(Application.StartupPath
				+ "\\Tmp\\Imp", "IMPORT_PATIENT_SERVICE.xlsx");

				if (File.Exists(source))
				{
					SaveFileDialog saveFileDialog1 = new SaveFileDialog();

					saveFileDialog1.Title = "Save File";
					saveFileDialog1.FileName = "IMPORT_KHÁM";
					saveFileDialog1.DefaultExt = "xlsx";
					saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
					saveFileDialog1.FilterIndex = 2;
					saveFileDialog1.RestoreDirectory = true;

					if (saveFileDialog1.ShowDialog() == DialogResult.OK)
					{
						File.Copy(source, saveFileDialog1.FileName, true);
						DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
						if (MessageBox.Show("Bạn có muốn mở file?",
								"Thông báo", MessageBoxButtons.YesNo,
								MessageBoxIcon.Question) == DialogResult.Yes)
							System.Diagnostics.Process.Start(saveFileDialog1.FileName);
					}
				}
				else
				{
					DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
				}
			}
			catch (Exception ex)
			{
				DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thất bại");
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			try
			{
				//WaitingManager.Show();
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Multiselect = false;
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					WaitingManager.Show();

					var import = new Inventec.Common.ExcelImport.Import();
					if (import.ReadFileExcel(ofd.FileName))
					{
						var patientImport = import.GetWithCheck<ImportPatientADO>(0);
						if (patientImport != null && patientImport.Count > 0)
						{
							List<ImportPatientADO> listAfterRemove = new List<ImportPatientADO>();
							foreach (var item in patientImport)
							{
								listAfterRemove.Add(item);
							}

							foreach (var item in patientImport)
							{
								bool checkNull = string.IsNullOrEmpty(item.VIR_PATIENT_NAME)
									&& string.IsNullOrEmpty(item.GENDER_NAME)
									&& string.IsNullOrEmpty(item.DOB_DATE)
									&& string.IsNullOrEmpty(item.DOB_YEAR)
									&& string.IsNullOrEmpty(item.CAREER_CODE)
									&& string.IsNullOrEmpty(item.PATIENT_TYPE_CODE)
									&& string.IsNullOrEmpty(item.PHONE)
									&& string.IsNullOrEmpty(item.PROVINCE_CODE)
									&& string.IsNullOrEmpty(item.DISTRICT_CODE)
									&& string.IsNullOrEmpty(item.COMMUNE_CODE)
									&& string.IsNullOrEmpty(item.ADDRESS)
									&& string.IsNullOrEmpty(item.FATHER_NAME)
									&& string.IsNullOrEmpty(item.MOTHER_NAME)
									&& string.IsNullOrEmpty(item.RELATIVE_NAME)
									&& string.IsNullOrEmpty(item.RELATIVE_TYPE)
									&& string.IsNullOrEmpty(item.RELATIVE_CMND_NUMBER)
									&& string.IsNullOrEmpty(item.RELATIVE_PHONE)
									&& string.IsNullOrEmpty(item.RELATIVE_ADDRESS)
									&& string.IsNullOrEmpty(item.WORK_PLACE)
									&& string.IsNullOrEmpty(item.ETHNIC_CODE)
									&& string.IsNullOrEmpty(item.NATIONAL_CODE)
									&& string.IsNullOrEmpty(item.CMND)
									&& string.IsNullOrEmpty(item.RELEASE_CMCCHC_DATE)
									&& string.IsNullOrEmpty(item.CMCCHC_PLACE)
									&& string.IsNullOrEmpty(item.SERVICE_CODE)
									&& string.IsNullOrEmpty(item.ROOM_CODE)
									&& string.IsNullOrEmpty(item.SERVICE_CLS_CODE)
									&& string.IsNullOrEmpty(item.PATIENT_TYPE_CODE_DTTT)
									&& string.IsNullOrEmpty(item.DATE_INSTRUCTION_STR)
									&& string.IsNullOrEmpty(item.IS_PRIORITY)
									&& string.IsNullOrEmpty(item.PRIORITY_TYPE_CODE)
									&& string.IsNullOrEmpty(item.IS_EMERGENCY)
									&& string.IsNullOrEmpty(item.IS_NOT_REQUIRE_FEE)
									&& string.IsNullOrEmpty(item.IS_CHRONIC)
									&& string.IsNullOrEmpty(item.IS_TUBERCULOSIS)
									&& string.IsNullOrEmpty(item.PRIORITY_TYPE_NAME)
									;



								if (checkNull)
								{
									listAfterRemove.Remove(item);
								}
							}


							this.currentAdos = listAfterRemove;

							if (this.currentAdos != null && this.currentAdos.Count > 0)
							{
								checkClick = false;
								btnSave.Enabled = true;
								btnShowLineError.Enabled = true;
								importAdos = new List<ImportPatientADO>();
								addPatientToProcessList(currentAdos, ref importAdos);
								SetDataSource(importAdos);
							}
							WaitingManager.Hide();

							//btnSave.Enabled = true;
						}
						else
						{
							WaitingManager.Hide();
							DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
						}
					}
					else
					{
						WaitingManager.Hide();
						DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
					}
				}
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void GetDataLocal()
		{
			try
			{
				lstCareer = BackendDataWorker.Get<HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				lstEthnic = BackendDataWorker.Get<SDA_ETHNIC>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				lstNational = BackendDataWorker.Get<SDA_NATIONAL>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				lstSerivce = BackendDataWorker.Get<HIS_SERVICE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
				lstProvince = BackendDataWorker.Get<V_SDA_PROVINCE>();
				lstDistrict = BackendDataWorker.Get<V_SDA_DISTRICT>();
				lstCommune = BackendDataWorker.Get<V_SDA_COMMUNE>();
				lstRoom = BackendDataWorker.Get<V_HIS_ROOM>();
				lstPriority = BackendDataWorker.Get<HIS_PRIORITY_TYPE>();
				lstPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void SetDataSource(List<ImportPatientADO> dataSource)
		{
			try
			{
				gridControl1.BeginUpdate();
				gridControl1.DataSource = null;
				gridControl1.DataSource = dataSource;
				gridControl1.EndUpdate();
				CheckErrorLine(null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

		}
		private void CheckErrorLine(List<ImportPatientADO> dataSource)
		{
			try
			{
				if (importAdos != null && importAdos.Count > 0)
				{
					var checkError = importAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
					if (!checkError)
					{
						btnSave.Enabled = true;
					}
					else
					{
						btnSave.Enabled = false;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

		}
		private void addPatientToProcessList(List<ImportPatientADO> patient, ref List<ImportPatientADO> _patient)
		{
			int exceptionPatient = 0;
			try
			{
				_patient = new List<ImportPatientADO>();
				long i = 0;
				foreach (var item in patient)
				{
					exceptionPatient++;
					i++;
					string error = "";
					var kskAdo = new ImportPatientADO();
					Inventec.Common.Mapper.DataObjectMapper.Map<ImportPatientADO>(kskAdo, item);
					V_SDA_PROVINCE currentProvince = new V_SDA_PROVINCE();
					V_SDA_DISTRICT currentDistrict = new V_SDA_DISTRICT();
					V_SDA_COMMUNE currentCommune = new V_SDA_COMMUNE();
					#region Tên bệnh nhân
					if (!string.IsNullOrEmpty(item.VIR_PATIENT_NAME))
					{
						if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.VIR_PATIENT_NAME, 200))
						{
							error += string.Format(Message.MessageImport.Maxlength, "Họ và tên");
						}

						try
						{
							int idx = item.VIR_PATIENT_NAME.Trim().LastIndexOf(" ");
							if (idx > -1)
							{
								kskAdo.FIRST_NAME = item.VIR_PATIENT_NAME.Trim().Substring(idx).Trim();
								kskAdo.LAST_NAME = item.VIR_PATIENT_NAME.Trim().Substring(0, idx).Trim();
							}
							else
							{
								kskAdo.FIRST_NAME = item.VIR_PATIENT_NAME.Trim();
								kskAdo.LAST_NAME = "";
							}
						}
						catch (Exception ex)
						{
							Inventec.Common.Logging.LogSystem.Warn("Loi xu ly cat chuoi ho ten benh nhan: ", ex);
						}
					}
					else
					{
						error += string.Format(Message.MessageImport.ThieuTruongDL, "Họ và tên");
					}
					#endregion

					#region Giới tính
					if (!string.IsNullOrEmpty(item.GENDER_NAME))
					{
						var gender = BackendDataWorker.Get<HIS_GENDER>();
						if (gender != null && gender.Count > 0)
						{
							var genderGet = gender.FirstOrDefault(o => o.GENDER_NAME == item.GENDER_NAME.Trim());
							if (genderGet != null)
							{
								kskAdo.GENDER_ID = genderGet.ID;
								kskAdo.GENDER_CODE = genderGet.GENDER_CODE;
							}
							else
							{
								error += string.Format(Message.MessageImport.KhongHopLe, "Giới tính");
							}
						}
					}
					else
					{
						error += string.Format(Message.MessageImport.ThieuTruongDL, "Giới tính");
					}
					#endregion

					#region Ngày sinh / Năm sinh
					if (!string.IsNullOrEmpty(item.DOB_DATE))
					{
						try
						{
							if (item.DOB_DATE.Trim().Length < 10)
							{
								throw new Exception();
							}
							var dob = Convert.ToDateTime(item.DOB_DATE.Trim());
							kskAdo.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dob) ?? 0;
						}
						catch (Exception)
						{
							kskAdo.IsDobFalse = true;
							error += string.Format(Message.MessageImport.KhongHopLe, "Ngày sinh");
						}
					}
					else if (!string.IsNullOrEmpty(item.DOB_YEAR))
					{
						if (Inventec.Common.Number.Check.IsNumber(item.DOB_YEAR.Trim()))
						{
							kskAdo.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(item.DOB_YEAR.Trim() + "0101000000");
							kskAdo.IS_HAS_NOT_DAY_DOB = true;
							if (kskAdo.DOB.ToString().Trim().Length > 14 || kskAdo.DOB < 0)
							{
								error += string.Format(Message.MessageImport.KhongHopLe, "Năm sinh");
							}
						}
						else
						{
							error += string.Format(Message.MessageImport.KhongHopLe, "Năm sinh");
						}
					}
					else
					{
						error += string.Format(Message.MessageImport.ThieuTruongDL, "Ngày sinh hoặc năm sinh");
					}
					#endregion

					#region Nghề nghiệp
					if (!string.IsNullOrEmpty(item.CAREER_CODE))
					{
						if (lstCareer != null && lstCareer.Count > 0 && lstCareer.FirstOrDefault(o => o.CAREER_CODE == item.CAREER_CODE) != null)
						{
							kskAdo.CAREER_NAME = lstCareer.FirstOrDefault(o => o.CAREER_CODE == item.CAREER_CODE).CAREER_NAME;
						}
						else
						{
							kskAdo.CAREER_NAME = item.CAREER_CODE;
							error += string.Format(Message.MessageImport.KhongHopLe, "Nghề nghiệp");
						}
					}
					else
					{
						error += string.Format(Message.MessageImport.ThieuTruongDL, "Nghề nghiệp");
					}
					#endregion

					#region Đối tượng
					if (!string.IsNullOrEmpty(item.PATIENT_TYPE_CODE))
					{
						if (item.PATIENT_TYPE_CODE != HisConfigs.Get<string>("HIS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT") && lstPatientType != null && lstPatientType.Count > 0 && lstPatientType.FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE) != null)
						{
							kskAdo.PATIENT_TYPE_ID = lstPatientType.FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE).ID;
							kskAdo.PATIENT_TYPE_NAME = lstPatientType.FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE).PATIENT_TYPE_NAME;
						}
						else
						{
							kskAdo.PATIENT_TYPE_NAME = item.PATIENT_TYPE_CODE;
							error += string.Format(Message.MessageImport.KhongHopLe, "Đối tượng");
						}
					}
					else
					{
						error += string.Format(Message.MessageImport.ThieuTruongDL, "Đối tượng");
					}
					#endregion

					#region Số điện thoại
					if (!string.IsNullOrEmpty(item.PHONE))
					{
						if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.PHONE, 12))
						{
							error += string.Format(Message.MessageImport.Maxlength, "Số điện thoại");
						}
					}
					#endregion

					#region Lỗi nhập thông tin THX
					if (string.IsNullOrEmpty(item.PROVINCE_CODE) && (!string.IsNullOrEmpty(item.DISTRICT_CODE) || !string.IsNullOrEmpty(item.COMMUNE_CODE))
						|| (string.IsNullOrEmpty(item.PROVINCE_CODE) || string.IsNullOrEmpty(item.DISTRICT_CODE)) && !string.IsNullOrEmpty(item.COMMUNE_CODE)
						)
					{
						error += string.Format(Message.MessageImport.KhongHopLe, "Tỉnh / Huyện/ Xã");
					}
					#endregion

					#region Tỉnh
					if (!string.IsNullOrEmpty(item.PROVINCE_CODE))
					{
						if (lstProvince != null && lstProvince.Count > 0 && lstProvince.FirstOrDefault(o => o.PROVINCE_CODE == item.PROVINCE_CODE) != null)
						{
							kskAdo.PROVINCE_NAME = lstProvince.FirstOrDefault(o => o.PROVINCE_CODE == item.PROVINCE_CODE).PROVINCE_NAME;
							currentProvince = lstProvince.FirstOrDefault(o => o.PROVINCE_CODE == item.PROVINCE_CODE);
						}
						else
						{
							kskAdo.PROVINCE_NAME = item.PROVINCE_CODE;
							error += string.Format(Message.MessageImport.KhongHopLe, "Tỉnh");
						}
					}
					#endregion

					#region Huyện
					if (!string.IsNullOrEmpty(item.DISTRICT_CODE))
					{
						string provinceCode = "";
						if (currentProvince != null)
							provinceCode = currentProvince.PROVINCE_CODE;
						var lstDisTemp = lstDistrict.Where(o => o.SEARCH_CODE.ToUpper().Contains("") && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();
						if (lstDisTemp != null && lstDisTemp.Count > 0 && lstDisTemp.FirstOrDefault(o => o.DISTRICT_CODE == item.DISTRICT_CODE) != null)
						{
							kskAdo.DISTRICT_NAME = lstDisTemp.FirstOrDefault(o => o.DISTRICT_CODE == item.DISTRICT_CODE).DISTRICT_NAME;
							currentDistrict = lstDisTemp.FirstOrDefault(o => o.DISTRICT_CODE == item.DISTRICT_CODE);
						}
						else
						{
							kskAdo.DISTRICT_NAME = item.DISTRICT_CODE;
							error += string.Format(Message.MessageImport.KhongHopLe, "Huyện");
						}
					}
					#endregion

					#region Xã
					if (!string.IsNullOrEmpty(item.COMMUNE_CODE))
					{
						string districCode = "";
						if (currentDistrict != null)
							districCode = currentDistrict.DISTRICT_CODE;
						var lstComTemp = lstCommune.Where(o => o.SEARCH_CODE.ToUpper().Contains("") && (districCode == "" || o.DISTRICT_CODE == districCode)).ToList();
						if (lstComTemp != null && lstComTemp.Count > 0 && lstComTemp.FirstOrDefault(o => o.COMMUNE_CODE == item.COMMUNE_CODE) != null)
						{
							kskAdo.COMMUNE_NAME = lstComTemp.FirstOrDefault(o => o.COMMUNE_CODE == item.COMMUNE_CODE).COMMUNE_NAME;
							currentCommune = lstComTemp.FirstOrDefault(o => o.COMMUNE_CODE == item.COMMUNE_CODE);
						}
						else
						{
							kskAdo.COMMUNE_NAME = item.COMMUNE_CODE;
							error += string.Format(Message.MessageImport.KhongHopLe, "Xã");
						}
					}
					#endregion

					#region Địa chỉ
					if (!string.IsNullOrEmpty(item.ADDRESS))
					{
						if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ADDRESS, 200))
						{
							error += string.Format(Message.MessageImport.Maxlength, "Địa chỉ");
						}
					}
					#endregion

					#region Dân tộc
					if (!string.IsNullOrEmpty(item.ETHNIC_CODE))
					{
						if (lstEthnic != null && lstEthnic.Count > 0 && lstEthnic.FirstOrDefault(o => o.ETHNIC_CODE == item.ETHNIC_CODE) != null)
						{
							kskAdo.ETHNIC_NAME = lstEthnic.FirstOrDefault(o => o.ETHNIC_CODE == item.ETHNIC_CODE).ETHNIC_NAME;
						}
						else
						{

							kskAdo.ETHNIC_NAME = item.ETHNIC_CODE;
							error += string.Format(Message.MessageImport.KhongHopLe, "Dân tộc");
						}
					}
					else
					{
						error += string.Format(Message.MessageImport.ThieuTruongDL, "Dân tộc");
					}
					#endregion

					#region Quốc tịch
					if (!string.IsNullOrEmpty(item.NATIONAL_CODE))
					{
						if (lstNational != null && lstNational.Count > 0 && lstNational.FirstOrDefault(o => o.NATIONAL_CODE == item.NATIONAL_CODE) != null)
						{
							kskAdo.NATIONAL_NAME = lstNational.FirstOrDefault(o => o.NATIONAL_CODE == item.NATIONAL_CODE).NATIONAL_NAME;
						}
						else
						{
							kskAdo.NATIONAL_NAME = item.NATIONAL_CODE;
							error += string.Format(Message.MessageImport.KhongHopLe, "Dân tộc");
						}
					}
					#endregion



					#region CMND/CCCD/HC
					if (!string.IsNullOrEmpty(item.CMND))
					{						
						if (Encoding.UTF8.GetBytes(item.CMND.Trim()).Count() == 9)
						{
							try
							{
								var dt = Int64.Parse(item.CMND.Trim());
								kskAdo.IsCmnd = true;
							}
							catch (Exception ex)
							{
								kskAdo.IsHC = true;
							}
						}
						else if (Encoding.UTF8.GetBytes(item.CMND.Trim()).Count() == 12)
						{
							try
							{
								var dt = Int64.Parse(item.CMND.Trim());
								kskAdo.IsCccd = true;
							}
							catch (Exception ex)
							{
								error += string.Format(Message.MessageImport.KhongHopLe, "CMND/CCCD/HC");
							}
						}
						else
						{
							error += string.Format(Message.MessageImport.KhongHopLe, "CMND/CCCD/HC");
						}

					}
					#endregion

					#region Ngày cấp
					if (!string.IsNullOrEmpty(item.RELEASE_CMCCHC_DATE))
					{
						try
						{
							if (item.RELEASE_CMCCHC_DATE.Trim().Length < 10)
							{
								throw new Exception();
							}
							var dob = Convert.ToDateTime(item.RELEASE_CMCCHC_DATE.Trim());
							kskAdo.RELEASE_CMCCHC_DATE_VALUE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dob) ?? 0;
						}
						catch (Exception)
						{
							kskAdo.IsDateCmndFalse = true;
							error += string.Format(Message.MessageImport.KhongHopLe, "Ngày cấp CMND/CCCD/HC");
						}
					}
					#endregion

					#region Dịch vụ khám
					List<long> lstServiceId = new List<long>();
					if (!string.IsNullOrEmpty(item.SERVICE_CODE))
					{
						var dtCheckCode = lstSerivce.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
						if (dtCheckCode != null && dtCheckCode.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
						{
							lstServiceId.Add(dtCheckCode.ID);
							kskAdo.SetListService(lstServiceId);
							kskAdo.SERVICE_ID = dtCheckCode.ID;
							kskAdo.SERVICE_NAME = dtCheckCode.SERVICE_NAME;
							if (string.IsNullOrEmpty(item.ROOM_CODE))
							{
								error += string.Format(Message.MessageImport.ChuaCoPhongKham, "Mã dịch vụ khám");
							}

						}
						else
						{
							kskAdo.SERVICE_NAME = item.SERVICE_CODE;
							error += string.Format(Message.MessageImport.KhongHopLe, "Mã dịch vụ khám");
						}
					}

					#endregion

					#region Phòng khám

					if (!string.IsNullOrEmpty(item.ROOM_CODE))
					{
						var dtCheckCode = lstRoom.FirstOrDefault(o => o.ROOM_CODE == item.ROOM_CODE);
						if (dtCheckCode != null && dtCheckCode.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
						{
							kskAdo.ROOM_ID = dtCheckCode.ID;
							kskAdo.ROOM_NAME = dtCheckCode.ROOM_NAME;
							if (string.IsNullOrEmpty(item.SERVICE_CODE))
								error += string.Format(Message.MessageImport.ChuaCoDichVu, "Mã phòng khám");
						}
						else
						{
							kskAdo.ROOM_NAME = item.ROOM_CODE;
							error += string.Format(Message.MessageImport.KhongHopLe, "Mã phòng khám");
						}
					}
					#endregion

					#region Mã dịch vụ CLS
					if (!string.IsNullOrEmpty(item.SERVICE_CLS_CODE))
					{

						List<long> lstServiceClsId = new List<long>();
						if (item.SERVICE_CLS_CODE.Trim().Contains(","))
						{
							int count = 0;
							List<long> lstCount = new List<long>();
							foreach (var ser in item.SERVICE_CLS_CODE.Split(','))
							{
								count++;
								var dtCheckCode = lstSerivce.FirstOrDefault(o => o.SERVICE_CODE == ser);
								if (dtCheckCode != null && dtCheckCode.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
								{
									lstServiceClsId.Add(dtCheckCode.ID);
									kskAdo.SERVICE_CLS_NAME = dtCheckCode.SERVICE_NAME;
								}
								else
								{
									lstCount.Add(count);
								}
							}
							if (lstCount != null && lstCount.Count > 0)
							{
								kskAdo.SERVICE_CLS_CODE = item.SERVICE_CLS_CODE;
								error += string.Format(Message.MessageImport.KhongHopLe, "Mã dịch vụ cận lâm sàn " + string.Join(", ", lstCount));

							}
						}
						else
						{
							var dtCheckCode = lstSerivce.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CLS_CODE);
							if (dtCheckCode != null && dtCheckCode.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
							{
								lstServiceClsId.Add(dtCheckCode.ID);
								kskAdo.SERVICE_CLS_NAME = dtCheckCode.SERVICE_NAME;
							}
							else
							{
								kskAdo.SERVICE_CLS_CODE = item.SERVICE_CLS_CODE;
								error += string.Format(Message.MessageImport.KhongHopLe, "Mã dịch vụ cận lâm sàn");
							}
						}
						kskAdo.SetListServiceCLS(lstServiceClsId);
					}
					#endregion

					#region ĐTTT
					if ((!string.IsNullOrEmpty(item.SERVICE_CODE) || !string.IsNullOrEmpty(item.SERVICE_CLS_CODE)) && string.IsNullOrEmpty(item.PATIENT_TYPE_CODE_DTTT))
					{
						error += string.Format(Message.MessageImport.ThieuTruongDL, "Đối tượng thanh toán");
					}
					else
					{
						if (!string.IsNullOrEmpty(item.PATIENT_TYPE_CODE_DTTT))
						{
							if (lstPatientType != null && lstPatientType.Count > 0 && lstPatientType.FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_DTTT) != null)
							{
								kskAdo.PATIENT_TYPE_NAME_DTTT = lstPatientType.FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_DTTT).PATIENT_TYPE_NAME;
								kskAdo.PATIENT_TYPE_DTTT_ID = lstPatientType.FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE_DTTT).ID;
							}
							else
							{
								kskAdo.PATIENT_TYPE_NAME_DTTT = item.PATIENT_TYPE_CODE_DTTT;
								error += string.Format(Message.MessageImport.KhongHopLe, "Đối tượng thanh toán");
							}
						}
					}
					#endregion

					if ((kskAdo.SERVICE_ID > 0 || (kskAdo.GetListServiceCLS() != null && kskAdo.GetListServiceCLS().Count > 0)) && kskAdo.ROOM_ID != null && kskAdo.ROOM_ID > 0)
					{
						string log = null;
						string usr = null;
						var check = dataExecuteRooms.FirstOrDefault(o => o.ROOM_ID == kskAdo.ROOM_ID);
						if (check != null)
						{
							log = dataExecuteRooms.FirstOrDefault(o => o.ROOM_ID == kskAdo.ROOM_ID) != null ? dataExecuteRooms.FirstOrDefault(o => o.ROOM_ID == kskAdo.ROOM_ID).RESPONSIBLE_LOGINNAME : null;
							usr = dataExecuteRooms.FirstOrDefault(o => o.ROOM_ID == kskAdo.ROOM_ID) != null ? dataExecuteRooms.FirstOrDefault(o => o.ROOM_ID == kskAdo.ROOM_ID).RESPONSIBLE_USERNAME : null;
						}
						List<ServiceReqDetailSDO> lst = new List<ServiceReqDetailSDO>();
						if (kskAdo.SERVICE_ID > 0)
						{
							ServiceReqDetailSDO sdoSer = new ServiceReqDetailSDO();
							sdoSer.PatientTypeId = kskAdo.PATIENT_TYPE_DTTT_ID;
							sdoSer.RoomId = kskAdo.ROOM_ID;
							sdoSer.ServiceId = kskAdo.SERVICE_ID;
							sdoSer.Amount = 1;
							sdoSer.AssignedExecuteLoginName = log;
							sdoSer.AssignedExecuteUserName = usr;
							lst.Add(sdoSer);
						}
						foreach (var serCls in kskAdo.GetListServiceCLS())
						{
							ServiceReqDetailSDO sdoSerCls = new ServiceReqDetailSDO();
							sdoSerCls.PatientTypeId = kskAdo.PATIENT_TYPE_DTTT_ID;
							//sdoSerCls.RoomId = kskAdo.ROOM_ID;
							sdoSerCls.ServiceId = serCls;
							sdoSerCls.Amount = 1;
							sdoSerCls.AssignedExecuteLoginName = log;
							sdoSerCls.AssignedExecuteUserName = usr;
							lst.Add(sdoSerCls);
						}
						kskAdo.SetListServiceDetailSDO(lst);
						kskAdo.IsExamnation = true;
					}
					else
					{
						kskAdo.IsExamnation = false;
					}

					#region Ngày khám

					if (!string.IsNullOrEmpty(item.DATE_INSTRUCTION_STR))
					{
						try
						{
							if (item.DATE_INSTRUCTION_STR.Trim().Length < 10)
							{
								throw new Exception();
							}
							var dob = Convert.ToDateTime(item.DATE_INSTRUCTION_STR.Trim());
							kskAdo.DATE_INSTRUCTION = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dob) ?? 0;
						}
						catch (Exception)
						{
							kskAdo.IsDateInstructionFalse = true;
							error += string.Format(Message.MessageImport.KhongHopLe, "Ngày khám");
						}
					}
					else
					{
						error += string.Format(Message.MessageImport.ThieuTruongDL, "Ngày khám");
					}
					#endregion

					#region Ưu tiên
					if (!string.IsNullOrEmpty(item.IS_PRIORITY) && item.IS_PRIORITY.Trim().ToLower() == "x")
					{
						kskAdo.IS_PRIORITY = "1";
					}
					#endregion

					#region Trường hợp Ưu tiên
					if (item.IS_PRIORITY.Trim().ToLower() == "x")
					{
						if (!string.IsNullOrEmpty(item.PRIORITY_TYPE_CODE))
						{
							if (lstPriority.FirstOrDefault(o => o.PRIORITY_TYPE_CODE == item.PRIORITY_TYPE_CODE) != null)
							{
								kskAdo.PRIORITY_TYPE_ID = lstPriority.FirstOrDefault(o => o.PRIORITY_TYPE_CODE == item.PRIORITY_TYPE_CODE).ID;
								kskAdo.PRIORITY_TYPE_NAME = lstPriority.FirstOrDefault(o => o.PRIORITY_TYPE_CODE == item.PRIORITY_TYPE_CODE).PRIORITY_TYPE_NAME;
							}
							else
							{
								kskAdo.PRIORITY_TYPE_NAME = item.PRIORITY_TYPE_CODE;
								error += string.Format(Message.MessageImport.KhongHopLe, "Trường hợp ưu tiên");
							}
						}
						else
						{
							error += string.Format(Message.MessageImport.ThieuTruongDL, "Trường hợp ưu tiên");
						}
					}
					#endregion

					#region Cấp cứu
					if (!string.IsNullOrEmpty(item.IS_EMERGENCY) && item.IS_EMERGENCY.Trim().ToLower() == "x")
					{
						kskAdo.IS_EMERGENCY = "1";
					}
					#endregion

					#region Thu sau
					if (!string.IsNullOrEmpty(item.IS_NOT_REQUIRE_FEE) && item.IS_NOT_REQUIRE_FEE.Trim().ToLower() == "x")
					{
						kskAdo.IS_NOT_REQUIRE_FEE = "1";
					}
					#endregion

					#region Mãn tính
					if (!string.IsNullOrEmpty(item.IS_TUBERCULOSIS) && item.IS_TUBERCULOSIS.Trim().ToLower() == "X")
					{
						kskAdo.IS_TUBERCULOSIS = "1";
					}
					#endregion

					kskAdo.ERROR = error;
					kskAdo.RowId = i;

					_patient.Add(kskAdo);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(exceptionPatient + "___________________" + ex);
			}

		}

		private void btnShowLineError_Click(object sender, EventArgs e)
		{
			try
			{
				btnSave.Enabled = false;
				checkClick = true;
				if (btnShowLineError.Text == "Dòng lỗi")
				{
					btnShowLineError.Text = "Dòng không lỗi";
					var errorLine = importAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
					SetDataSource(errorLine);

				}
				else
				{
					btnShowLineError.Text = "Dòng lỗi";
					var errorLine = importAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
					SetDataSource(errorLine);
					if (errorLine != null && errorLine.Count > 0)
					{
						btnSave.Enabled = true;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			try
			{
				Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CreateExport));
				thread.Priority = ThreadPriority.Normal;
				thread.IsBackground = true;
				thread.SetApartmentState(System.Threading.ApartmentState.STA);
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
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void CreateExport()
		{
			try
			{
				List<string> expCode = new List<string>();

				Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

				string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "EXPORT_KHÁM.xlsx");

				//chọn đường dẫn
				saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
				if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{

					//getdata
					WaitingManager.Show();

					if (String.IsNullOrEmpty(templateFile))
					{
						store = null;
						DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không tìm thấy file", templateFile));
						return;
					}

					store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
					if (store.TemplatePath == "")
					{
						DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
						return;
					}

					List<ImportPatientADO> export = new List<ImportPatientADO>();

					if (this.importAdos != null && this.importAdos.Count > 0)
					{
						var errorList = this.importAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
						if (errorList != null && errorList.Count > 0)
						{
							export = errorList;
						}
					}

					ProcessData(export, ref store);
					WaitingManager.Hide();

					if (store != null)
					{
						try
						{
							if (store.OutFile(saveFileDialog1.FileName))
							{
								DevExpress.XtraEditors.XtraMessageBox.Show("Xuất file thành công");

								if (MessageBox.Show("Bạn có muốn mở file?",
									"Thông báo", MessageBoxButtons.YesNo,
									MessageBoxIcon.Question) == DialogResult.Yes)
									System.Diagnostics.Process.Start(saveFileDialog1.FileName);
							}
						}
						catch (Exception ex)
						{
							Inventec.Common.Logging.LogSystem.Warn(ex);
						}
					}
					else
					{
						DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void ProcessData(List<ImportPatientADO> data, ref Inventec.Common.FlexCellExport.Store store)
		{
			try
			{
				Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
				Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

				store.SetCommonFunctions();
				objectTag.AddObjectData(store, "ErrorList", data);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				store = null;
			}
		}
		private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{
				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					ImportPatientADO pData = (ImportPatientADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
					if (e.Column.FieldName == "STT")
					{
						e.Value = e.ListSourceRowIndex + 1;
					}
					else if (e.Column.FieldName == "DOB_DATE_STR_x")
					{
						if (!string.IsNullOrEmpty(pData.DOB_DATE) && !pData.IsDobFalse)
						{
							e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.DOB ?? 0);
						}
						else
						{
							e.Value = pData.DOB_DATE;
						}
					}
					else if (e.Column.FieldName == "RELEASE_CMCCHC_DATE_STR_x")
					{
						if (!string.IsNullOrEmpty(pData.RELEASE_CMCCHC_DATE) && !pData.IsDateCmndFalse)
							e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.RELEASE_CMCCHC_DATE_VALUE);
						else
							e.Value = pData.RELEASE_CMCCHC_DATE;
					}
					else if (e.Column.FieldName == "DATE_INSTRUCTION_STR_x")
					{
						if (!pData.IsDateInstructionFalse)
							e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.DATE_INSTRUCTION ?? 0);
						else
							e.Value = pData.DATE_INSTRUCTION_STR;
					}
					else if (e.Column.FieldName == "IS_PRIORITY_STR")
					{
						if (!string.IsNullOrEmpty(pData.IS_PRIORITY))
							e.Value = "X";
					}
					else if (e.Column.FieldName == "IS_EMERGENCY_STR")
					{
						if (!string.IsNullOrEmpty(pData.IS_EMERGENCY))
							e.Value = "X";
					}
					else if (e.Column.FieldName == "IS_NOT_REQUIRE_FEE_STR")
					{
						if (!string.IsNullOrEmpty(pData.IS_NOT_REQUIRE_FEE))
							e.Value = "X";
					}
					else if (e.Column.FieldName == "IS_CHRONIC_STR")
					{
						if (!string.IsNullOrEmpty(pData.IS_CHRONIC))
							e.Value = "X";
					}
					else if (e.Column.FieldName == "IS_TUBERCULOSIS_STR")
					{
						if (!string.IsNullOrEmpty(pData.IS_TUBERCULOSIS))
							e.Value = "X";
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private HIS_PATIENT InitHisPatient(ImportPatientADO ado)
		{
			HIS_PATIENT hisPatient = null;
			try
			{
				hisPatient = new HIS_PATIENT();
				hisPatient.FIRST_NAME = ado.FIRST_NAME;
				hisPatient.LAST_NAME = ado.LAST_NAME;
				hisPatient.VIR_PATIENT_NAME = ado.VIR_PATIENT_NAME;
				hisPatient.GENDER_ID = ado.GENDER_ID;
				hisPatient.DOB = ado.DOB ?? 0;
				hisPatient.IS_HAS_NOT_DAY_DOB = ado.IS_HAS_NOT_DAY_DOB ? (short?)1 : 0;
				hisPatient.CAREER_CODE = ado.CAREER_CODE;
				hisPatient.CAREER_NAME = ado.CAREER_NAME;
				hisPatient.PHONE = ado.PHONE;
				hisPatient.PROVINCE_CODE = ado.PROVINCE_CODE;
				hisPatient.PROVINCE_NAME = ado.PROVINCE_NAME;
				hisPatient.DISTRICT_CODE = ado.DISTRICT_CODE;
				hisPatient.DISTRICT_NAME = ado.DISTRICT_NAME;
				hisPatient.COMMUNE_CODE = ado.COMMUNE_CODE;
				hisPatient.COMMUNE_NAME = ado.COMMUNE_NAME;
				hisPatient.ADDRESS = ado.ADDRESS;
				hisPatient.FATHER_NAME = ado.FATHER_NAME;
				hisPatient.MOTHER_NAME = ado.MOTHER_NAME;
				hisPatient.RELATIVE_NAME = ado.RELATIVE_NAME;
				hisPatient.RELATIVE_TYPE = ado.RELATIVE_TYPE;
				hisPatient.RELATIVE_CMND_NUMBER = ado.RELATIVE_CMND_NUMBER;
				hisPatient.RELATIVE_PHONE = ado.RELATIVE_PHONE;
				hisPatient.RELATIVE_ADDRESS = ado.RELATIVE_ADDRESS;
				hisPatient.WORK_PLACE = ado.WORK_PLACE;
				hisPatient.ETHNIC_CODE = ado.ETHNIC_CODE;
				hisPatient.ETHNIC_NAME = ado.ETHNIC_NAME;
				hisPatient.NATIONAL_CODE = ado.NATIONAL_CODE;
				hisPatient.NATIONAL_NAME = ado.NATIONAL_NAME;
				if (ado.IsCmnd)
				{
					hisPatient.CMND_NUMBER = ado.CMND;
					hisPatient.CMND_DATE = ado.RELEASE_CMCCHC_DATE_VALUE;
					hisPatient.CMND_PLACE = ado.CMCCHC_PLACE;
				}
				if (ado.IsCccd)
				{
					hisPatient.CCCD_NUMBER = ado.CMND;
					hisPatient.CCCD_DATE = ado.RELEASE_CMCCHC_DATE_VALUE;
					hisPatient.CCCD_PLACE = ado.CMCCHC_PLACE;
				}
				if (ado.IsHC)
				{
					hisPatient.PASSPORT_NUMBER = ado.CMND;
					hisPatient.PASSPORT_DATE = ado.RELEASE_CMCCHC_DATE_VALUE;
					hisPatient.PASSPORT_PLACE = ado.CMCCHC_PLACE;
				}
				hisPatient.IS_CHRONIC = ado.IS_CHRONIC == "1" ? (short?)1 : 0;
				hisPatient.IS_TUBERCULOSIS = ado.IS_TUBERCULOSIS == "1" ? (short?)1 : 0;


            }
			catch (Exception ex)
			{
				hisPatient = null;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return hisPatient;
		}

		private HIS_PATIENT_TYPE_ALTER InitHisPatientTypeAlter(ImportPatientADO ado)
		{
			HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter = null;
			try
			{
				if (ado.PATIENT_TYPE_ID > 0)
				{
					hisPatientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
					hisPatientTypeAlter.PATIENT_TYPE_ID = ado.PATIENT_TYPE_ID;
					hisPatientTypeAlter.TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
				}
			}
			catch (Exception ex)
			{
				hisPatientTypeAlter = null;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return hisPatientTypeAlter;
		}

		private void ProcessExamServiceRequestData(ref HisServiceReqExamRegisterSDO ServiceReqData, ref List<long> serviceIds, ref List<long> _roomIds, ImportPatientADO ado)
		{
			try
			{
				if (ServiceReqData.ServiceReqDetails == null)
					ServiceReqData.ServiceReqDetails = new List<ServiceReqDetailSDO>();
				if (ado.GetListServiceDetailSDO() != null && ado.GetListServiceDetailSDO().Count > 0)
				{
					ServiceReqData.ServiceReqDetails.AddRange(ado.GetListServiceDetailSDO());
				}
				foreach (var item in ServiceReqData.ServiceReqDetails)
				{
					serviceIds.Add(item.ServiceId);
					_roomIds.Add(item.RoomId ?? 0);
				}

				ServiceReqData.Priority = ado.IS_PRIORITY == "1" ? (short?)1 : 0;
				ServiceReqData.InstructionTime = ado.DATE_INSTRUCTION ?? 0;
				ServiceReqData.IsNotRequireFee = ado.IS_NOT_REQUIRE_FEE == "1" ? (short?)1 : 0;
				ServiceReqData.PriorityTypeId = ado.PRIORITY_TYPE_ID;
				ServiceReqData.RequestLoginName = cboLogin.EditValue != null ? cboLogin.EditValue.ToString() : null;
				ServiceReqData.RequestUserName = cboLogin.EditValue != null ? cboLogin.Text.ToString() : null;

            }
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				this.positionHandleControl = -1;
				if (!dxValidationProvider1.Validate())
					return;
				if (importAdos == null || importAdos.Count == 0)
					return;

				UpdateIndex.currentIndex = 0;
				total = importAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList().Count();
				backgroundWorker1.RunWorkerAsync();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

		}

		private void bbtnDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				var row = (ImportPatientADO)gridView1.GetFocusedRow();
				if (row != null)
				{
					if (importAdos != null && importAdos.Count > 0)
					{
						importAdos.Remove(row);
						gridControl1.DataSource = null;
						gridControl1.DataSource = importAdos;
						CheckErrorLine(null);
						//if (checkClick)
						//{
						//	if (btnShowLineError.Text == "Dòng lỗi")
						//	{
						//		btnShowLineError.Text = "Dòng không lỗi";
						//	}
						//	else
						//	{
						//		btnShowLineError.Text = "Dòng lỗi";
						//	}
						//	btnShowLineError_Click(null, null);
						//}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

		}

		private void bbtnInfor_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
				var row = (ImportPatientADO)gridView1.GetFocusedRow();
				if (row != null)
				{
					DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			try
			{
				List<ImportPatientADO> ListServiceReqExam = null;
				List<ImportPatientADO> ListPatientProfile = null;
				List<ImportPatientADO> importAdosTemp = null;
				if (importAdos != null && importAdos.Count > 0)
				{
					importAdosTemp = importAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
					ListServiceReqExam = importAdosTemp.Where(o => o.IsExamnation).ToList();
					ListPatientProfile = importAdosTemp.Where(o => !o.IsExamnation).ToList();
				}
				DicMessage = new Dictionary<long, string>();

				foreach (var item in ListPatientProfile)
				{
					CommonParam param = new CommonParam();
					HisPatientProfileSDO PatientProfile = new HisPatientProfileSDO();
					PatientProfile.HisPatient = InitHisPatient(item);
					PatientProfile.HisPatientTypeAlter = InitHisPatientTypeAlter(item);
					PatientProfile.HisTreatment = new HIS_TREATMENT();
					if (item.IS_EMERGENCY == "1")
					{
						if (PatientProfile.HisTreatment == null)
							PatientProfile.HisTreatment = new HIS_TREATMENT();
						PatientProfile.HisTreatment.IS_EMERGENCY = 1;
					}
					PatientProfile.RequestRoomId = currentModule.RoomId;
					PatientProfile.ProvinceCode = item.PROVINCE_CODE;
					PatientProfile.DistrictCode = item.DISTRICT_CODE;
					PatientProfile.DepartmentId = lstRoom.FirstOrDefault(o => o.ID == currentModule.RoomId).DEPARTMENT_ID;
					if (item.DATE_INSTRUCTION != null)
						PatientProfile.TreatmentTime = item.DATE_INSTRUCTION ?? 0;
					backgroundWorker1.ReportProgress(UpdateIndex.currentIndex);
					var rsult = new BackendAdapter(param).Post<HisPatientProfileSDO>(HisRequestUriStore.HIS_PATIENT_REGISTER_PROFILE, ApiConsumers.MosConsumer, PatientProfile, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
					UpdateIndex.currentIndex++;
					if (rsult != null)
					{
						if (rsult.HisPatient != null)
						{
							checkSuccess = true;
						}
					}
					else
					{
						checkSuccess = false;
						DicMessage[item.RowId] = param.GetBugCode() + " " + param.GetMessage();
						if (string.IsNullOrEmpty(DicMessage[item.RowId].Trim()))
						{
							DicMessage[item.RowId] = "Xử lý thất bại.";
						}
					}
				}

				foreach (var item in ListServiceReqExam)
				{
					CommonParam param = new CommonParam();
					HisServiceReqExamRegisterSDO serviceReqExamRegister = new HisServiceReqExamRegisterSDO();
					serviceReqExamRegister.HisPatientProfile = new HisPatientProfileSDO();
					serviceReqExamRegister.HisPatientProfile.RequestRoomId = currentModule.RoomId;
					serviceReqExamRegister.HisPatientProfile.DepartmentId = lstRoom.FirstOrDefault(o => o.ID == currentModule.RoomId).DEPARTMENT_ID;
					serviceReqExamRegister.HisPatientProfile.HisPatient = InitHisPatient(item);
					serviceReqExamRegister.HisPatientProfile.HisPatientTypeAlter = InitHisPatientTypeAlter(item);
					serviceReqExamRegister.HisPatientProfile.HisTreatment = new HIS_TREATMENT();
					if (item.IS_EMERGENCY == "1")
					{
						if (serviceReqExamRegister.HisPatientProfile.HisTreatment == null)
							serviceReqExamRegister.HisPatientProfile.HisTreatment = new HIS_TREATMENT();
						serviceReqExamRegister.HisPatientProfile.HisTreatment.IS_EMERGENCY = 1;
					}
					serviceReqExamRegister.RequestRoomId = currentModule.RoomId;
					serviceReqExamRegister.HisPatientProfile.ProvinceCode = item.PROVINCE_CODE;
					serviceReqExamRegister.HisPatientProfile.DistrictCode = item.DISTRICT_CODE;
					if (item.DATE_INSTRUCTION != null)
						serviceReqExamRegister.HisPatientProfile.TreatmentTime = item.DATE_INSTRUCTION ?? 0;
					List<long> serviceIds = new List<long>();
					List<long> _RoomIds = new List<long>();
					ProcessExamServiceRequestData(ref serviceReqExamRegister, ref serviceIds, ref _RoomIds, item);
					backgroundWorker1.ReportProgress(UpdateIndex.currentIndex);
					var rsult = new BackendAdapter(param).Post<HisServiceReqExamRegisterResultSDO>(HisRequestUriStore.HIS_SERVICE_REQ_EXAMREGISTER, ApiConsumers.MosConsumer, serviceReqExamRegister, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
					UpdateIndex.currentIndex++;
					if (rsult != null)
					{
						if (rsult.HisPatientProfile.HisPatient != null)
						{
							checkSuccess = true;
						}
					}
					else
					{
						checkSuccess = false;
						DicMessage[item.RowId] = param.GetBugCode() + " " + param.GetMessage();
						if (string.IsNullOrEmpty(DicMessage[item.RowId].Trim()))
						{
							DicMessage[item.RowId] = "Xử lý thất bại.";
						}
					}
				}
			}
			catch (Exception ex)
			{
				frmWaiting = null;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

		}

		private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
		{
			try
			{
				if (frmWaiting == null)
				{
					frmWaiting = new frmWaiting(total);
					frmWaiting.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				frmWaiting = null;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			try
			{
				if (UpdateIndex.currentIndex < total)
					UpdateIndex.currentIndex = total;
				if (!checkSuccess)
				{
					#region Hien thi message thong bao					
					if (DicMessage != null && DicMessage.Count > 0)
					{
						foreach (var item in importAdos)
						{
							if (DicMessage.ContainsKey(item.RowId))
							{
								item.IsFalseAfterSave = true;
								item.ERROR = DicMessage[item.RowId];
							}
						}
						SetDataSource(importAdos);
						DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Có {0} bệnh nhân nhập khẩu lỗi. Xem các dòng để biết thêm chi tiết", DicMessage.Count));

					}

				}
				else
				{
					MessageManager.Show(this.ParentForm, paramSuccess, checkSuccess);
				}
				#endregion
				frmWaiting = null;
				#region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
				SessionManager.ProcessTokenLost(paramSuccess);
				#endregion
			}
			catch (Exception ex)
			{
				frmWaiting = null;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
		{
			try
			{
				if (btnSave.Enabled)
					btnSave_Click(null, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void txtLogin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			try
			{
				if (listAcsUser != null && listAcsUser.Count > 0)
				{
					if (!string.IsNullOrEmpty(txtLogin.Text.Trim()) && listAcsUser.FirstOrDefault(o => o.LOGINNAME == txtLogin.Text.Trim()) != null)
						cboLogin.EditValue = listAcsUser.FirstOrDefault(o => o.LOGINNAME == txtLogin.Text.Trim()).LOGINNAME;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void cboLogin_EditValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (listAcsUser != null && listAcsUser.Count > 0 && cboLogin.EditValue != null)
				{
					if (listAcsUser.FirstOrDefault(o => o.LOGINNAME == cboLogin.EditValue.ToString()) != null)
						txtLogin.Text = listAcsUser.FirstOrDefault(o => o.LOGINNAME == cboLogin.EditValue.ToString()).LOGINNAME;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);

				if (e.RowHandle >= 0)
				{
					//string DISPLAY_COLOR = (vw.GetRowCellValue(e.RowHandle, "IsFalseAfterSave") ?? "").ToString();
					ImportPatientADO data = (ImportPatientADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];

					if (data.IsFalseAfterSave)
					{
						e.Appearance.ForeColor = Color.Red;
					}
					else
						e.Appearance.ForeColor = Color.Black;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
