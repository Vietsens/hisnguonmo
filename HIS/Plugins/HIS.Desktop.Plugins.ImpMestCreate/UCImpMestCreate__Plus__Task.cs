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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using HIS.Desktop.Plugins.ImpMestCreate.Base;
using HIS.Desktop.Plugins.ImpMestCreate.Save;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.MaterialType.ADO;
using HIS.UC.MedicineType.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    public partial class UCImpMestCreate : UserControlBase
    {
		Task taskUcMedicine { get; set; }
		Task taskUcMaterial { get; set; }
		Task taskImpMestTypeAllow { get; set; }
		private async void TaskAll()
		{
			try
			{
				taskImpMestTypeAllow = GetImpMestTypeAllow();
				
				await taskImpMestTypeAllow;
				backgroundWorker1.RunWorkerAsync();
				backgroundWorker2.RunWorkerAsync();

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void GetBid()
		{
			try
			{
				GetBids(ref listBids);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadDataByBidTask()
		{
			try
			{
				Action myaction = () =>
				{
					LoadDataByBid();
				};
				Task task = new Task(myaction);
				task.Start();

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void GetMedicine(long mediId)
        {
            try
            {
				// thread load lo thuoc
				CommonParam param = new CommonParam();
				HisMedicineFilter filter = new HisMedicineFilter();
				filter.MEDICINE_TYPE_ID = mediId;
				filter.ORDER_FIELD = "IMP_TIME";
				filter.ORDER_DIRECTION = "DESC";
				var rs = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer,filter, param);



				//var rs = BackendDataWorker.Get<V_HIS_MEDICINE>().Where(s => s.MEDICINE_TYPE_ID == mediId).OrderByDescending(o => o.IMP_TIME).FirstOrDefault();
				if (rs != null)
				{

					rs = rs.OrderByDescending(s => s.IMP_TIME).ToList();

					this.currentMedicine = rs.FirstOrDefault();				
				}
				else this.currentMedicine = null;
				Inventec.Common.Logging.LogSystem.Debug("Loai thuoc ID: " + mediId + " co danh  sach lo thuoc la: " + rs);
			}
            catch (Exception ex)
            {

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
        }
		private void GetMaterial(long mediId)
        {
            try
            {
				// thread load lo thuoc
				CommonParam param = new CommonParam();
				HisMaterialFilter filter = new HisMaterialFilter();
				filter.MATERIAL_TYPE_ID = mediId;
				filter.ORDER_FIELD = "IMP_TIME";
				filter.ORDER_DIRECTION = "DESC";
				var rs  = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, filter, param);
				//var rs = BackendDataWorker.Get<V_HIS_MATERIAL>().Where(s => s.MATERIAL_TYPE_ID == mediId).OrderByDescending(o => o.IMP_TIME).FirstOrDefault();
				if (rs != null)
                {
					rs = rs.OrderByDescending(s => s.IMP_TIME).ToList();
					this.currentMaterial = rs.FirstOrDefault();
				}
				else this.currentMaterial = null;
				Inventec.Common.Logging.LogSystem.Debug("Loai vat tu ID: " + mediId + " co danh  sach lo vat tu la: " + rs);
			}
            catch (Exception ex)
            {

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
        }
		private void GetContract()
		{
			try
			{
				GetContracts(ref listContracts);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task GetImpMestTypeAllow()
		{
			try
			{
				Action myaction = () => {
					LoadImpMestTypeAllow();
					this.currentImpMestType = null;
					this.currentImpMestType = listImpMestType.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC);
				};
				Task task = new Task(myaction);
				task.Start();

				await task;
				LoadDataToComboImpMestType();
				var task1 = SetDefaultImpMestTypeTask();
				await task1;
				if (this.currentImpMestType != null)
				{
					cboImpMestType.EditValue = this.currentImpMestType.ID;
				}
				else if (listImpMestType != null && listImpMestType.Count == 1)
				{
					this.currentImpMestType = listImpMestType.First();
					cboImpMestType.EditValue = listImpMestType.FirstOrDefault().ID;
				}
				if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
				{   
					if(medistock.IS_ALLOW_IMP_SUPPLIER == 0)
					{
						DevExpress.XtraEditors.XtraMessageBox.Show("Kho không cho phép nhập từ nhà cung cấp", "Thông báo");
						return;
					}
					txtNhaCC_Closed(null,null);
				}
				SetDefaultCheckNoBid();
				SetDataByExpMestUp();			

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void GetSupplier()
		{
			try
			{
				listSupplier = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1).ToList();				
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task GetImpSource()
		{
			try
			{
				Action myaction = () => {
					lstImpSource = BackendDataWorker.Get<HIS_IMP_SOURCE>().Where(p => p.IS_ACTIVE == 1).ToList();
				};
				Task task = new Task(myaction);
				task.Start();

				await task;
				LoadDataToCboImpSource();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task GetMedicineUseForm()
		{
			try
			{
				Action myaction = () => {
					lstMedicineUseForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().Where(p => p.IS_ACTIVE == 1).ToList();
				};
				Task task = new Task(myaction);
				task.Start();

				await task;
				LoadDataToComboMedicineUseForm();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task SetDefaultImpMestTypeTask()
		{
			try
			{
				Action myaction = () => {
					listMediStock = new List<V_HIS_MEDI_STOCK>();
					listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();					
				};
				Task task = new Task(myaction);
				task.Start();

				await task;


				cboMediStock.Properties.DataSource = listMediStock;
				SetDefaultValueMediStock();
				SetControlEnableImMestTypeManu();
				cboMediStock.Enabled = false;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadMedicineType()
		{
			try
			{
                var activeBidMedicineTypeIds = BackendDataWorker.Get<HIS_BID_MEDICINE_TYPE>()
												.Where(bid => bid.IS_ACTIVE == 1)
												.Select(bid => bid.MEDICINE_TYPE_ID)
												.Distinct()
												.ToList();
                var activeBidMedicineTypeIds1 = BackendDataWorker.Get<HIS_BID_MEDICINE_TYPE>()
                                               .Where(bid => bid.IS_ACTIVE != 1)
                                               .Select(bid => bid.MEDICINE_TYPE_ID)
                                               .Distinct()
                                               .ToList();
               
                listMedicineTypeTemp = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o =>
					  o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
					  && o.IS_LEAF == 1
					  && o.IS_STOP_IMP == null
                      && (activeBidMedicineTypeIds.Contains(o.ID) || !activeBidMedicineTypeIds1.Contains(o.ID))).ToList();

            }
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
		private void LoadMaterialType()
		{
			try
			{
                var activeBidMaterialTypeIds = BackendDataWorker.Get<HIS_BID_MATERIAL_TYPE>()
                                                .Where(bid => bid.IS_ACTIVE == 1)
                                                .Select(bid => bid.MATERIAL_TYPE_ID)
												.Distinct()
                                                .ToList();
                var activeBidMaterialTypeIds1 = BackendDataWorker.Get<HIS_BID_MATERIAL_TYPE>()
                                                .Where(bid => bid.IS_ACTIVE != 1)
                                                .Select(bid => bid.MATERIAL_TYPE_ID)
                                                .Distinct()
                                                .ToList();
                listMaterialTypeTemp = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o =>
						o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
						&& o.IS_LEAF == 1
						&& o.IS_STOP_IMP == null
                        && (activeBidMaterialTypeIds.Contains(o.ID) || !activeBidMaterialTypeIds1.Contains(o.ID))).ToList();
                

            }
            catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task LoadReceiver()
		{
			try
			{
				Action myaction = () => {
					lstAcsUser = BackendDataWorker.Get<ACS_USER>().Where(p => p.IS_ACTIVE == 1).ToList();
				};
				Task task = new Task(myaction);
				task.Start();

				await task;
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("LOGINNAME", "", 250, 1));
				columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "ID", columnInfos, false, 250);
				ControlEditorLoader.Load(cboRecieve, lstAcsUser, controlEditorADO);
				var loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
				var defaultLogginNames = lstAcsUser.Where(o => o.LOGINNAME == loginname).ToList();
				if (defaultLogginNames != null && defaultLogginNames.Count > 0)
				{
					var defaultLogginName = defaultLogginNames.FirstOrDefault();
					cboRecieve.EditValue = defaultLogginName.ID;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task LoadSaleProfits()
		{
			try
			{
				Action myaction = () => {
					GetSaleProfits();
				};
				Task task = new Task(myaction);
				task.Start();

				await task;				
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task LoadManufacturer()
		{
			try
			{
				List<ManufacturerADO> listADO = new List<ManufacturerADO>();
				Action myaction = () => {
					var dataManus = BackendDataWorker.Get<HIS_MANUFACTURER>().Where(p => p.IS_ACTIVE == 1).ToList();
					foreach (var item in dataManus)
					{
						ManufacturerADO manuf = new ManufacturerADO();
						manuf.ID = item.ID;
						manuf.MANUFACTURER_CODE = item.MANUFACTURER_CODE;
						manuf.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
						manuf.MANUFACTURER_NAME_UNSIGN = convertToUnSign3(item.MANUFACTURER_NAME);
						listADO.Add(manuf);
					}
				};
				Task task = new Task(myaction);
				task.Start();

				await task;
				DataToComboManufacturer(cboHangSX,listADO);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

        private void LoadDosageForm()
        {
            try
            {
                List<DosageFormADO> listADO = new List<DosageFormADO>();
                //Action myaction = () => {
                    var dataDosageForm = BackendDataWorker.Get<HIS_DOSAGE_FORM>().Where(p => p.IS_ACTIVE == 1).ToList();
                    foreach (var item in dataDosageForm)
                    {
                        DosageFormADO Dosage = new DosageFormADO();
                        Dosage.ID = item.ID;
                        Dosage.DOSAGE_FORM_CODE = item.DOSAGE_FORM_CODE;
                        Dosage.DOSAGE_FORM_NAME = item.DOSAGE_FORM_NAME;
                        Dosage.DOSAGEFORM_NAME_UNSIGN = convertToUnSign3(item.DOSAGE_FORM_NAME);
                        listADO.Add(Dosage);
                    }
				//};
				//Task task = new Task(myaction);
				// task.Start();

				//await task;
				cboDosageForm.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                cboDosageForm.EditValue = null;
                cboDosageForm.Text = string.Empty;

                DataToComboDosageForm(cboDosageForm, listADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadNation()
		{
			try
			{
				List<NationalADO> listADO = new List<NationalADO>();
				Action myaction = () => {
					var dataNationals = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => p.IS_ACTIVE == 1).ToList();
					foreach (var item in dataNationals)
					{
						NationalADO national = new NationalADO();
						national.ID = item.ID;
						national.NATIONAL_CODE = item.NATIONAL_CODE;
						national.NATIONAL_NAME = item.NATIONAL_NAME;
						national.NATIONAL_NAME_UNSIGN = convertToUnSign3(item.NATIONAL_NAME);
						listADO.Add(national);
					}
				};
				Task task = new Task(myaction);
				task.Start();

				await task;
				DataToComboNation(cboNationals, listADO);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task LoadDataUCMedicineTemp()
		{
			try
			{
				Action myaction = () => {
					listMedicineTypeTemp = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o =>
					  o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
					  && o.IS_LEAF == 1
					  && o.IS_STOP_IMP == null).ToList();
				};
				Task task = new Task(myaction);
				task.Start();
				
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task LoadDataUCMaterialTemp()
		{
			try
			{
				Action myaction = () => {
					listMaterialTypeTemp = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o =>

					o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
					&& o.IS_LEAF == 1
					&& o.IS_STOP_IMP == null).ToList();
				};
				Task task = new Task(myaction);
				task.Start();

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}


		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				if (xtraTabPageMedicine.PageVisible)
					InitSourceMediData();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.medicineProcessor.Reload(this.ucMedicineTypeTree, listMedicineType);
			Inventec.Common.Logging.LogSystem.Warn("INIT XONG DỮ LIỆU THUỐC");
		}

		private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
		{
            if (xtraTabPageMaterial.PageVisible)
                InitSourceMatiData();
		}

		private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.materialProcessor.Reload(this.ucMaterialTypeTree, listMaterialType);
			Inventec.Common.Logging.LogSystem.Warn("INIT XONG DỮ LIỆU VẬT TƯ");
		}


		private void InitSourceMediData()
		{
			try
			{
				//int start = 0;
				//int count = this.listMedicineTypeTemp.Count;
				//while (count > 0)
				//{
				//	int limit = (count <= 100) ? count : 100;
				var listSub = this.listMedicineTypeTemp;
					//.Skip(start).Take(limit).ToList();
					

					if (medistock.IS_BUSINESS == 1)
					{
						if (medistock.IS_NEW_MEDICINE == 1 && medistock.IS_TRADITIONAL_MEDICINE == 1)
						{
							listSub = listSub.Where(o => o.IS_BUSINESS == 1).ToList();
						}
						else if (medistock.IS_NEW_MEDICINE == 1)
						{
							listSub = listSub.Where(o =>
								o.IS_BUSINESS == 1
								&& (o.MEDICINE_LINE_ID == null
								|| o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__TTD)
								).ToList();
						}
						else if (medistock.IS_TRADITIONAL_MEDICINE == 1)
						{
							listSub = listSub.Where(o =>
								o.IS_BUSINESS == 1
								&& (o.MEDICINE_LINE_ID == null
								|| o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__CP_YHCT
								|| o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT)
								).ToList();
						}
						else
							listSub = listSub.Where(o => o.IS_BUSINESS == 1).ToList();

						if (listSub != null && listSub.Count > 0)
						{
							if (medistock.IS_SHOW_DRUG_STORE == 1)
							{
								listSub = listSub.Where(o => o.IS_DRUG_STORE == 1).ToList();
							}
							else
							{
								listSub = listSub.Where(o => o.IS_DRUG_STORE == null).ToList();
							}
						}
					}
					else
					{
						if (medistock.IS_NEW_MEDICINE == 1 && medistock.IS_TRADITIONAL_MEDICINE == 1)
						{
							listSub = listSub.Where(o => o.IS_BUSINESS != 1).ToList();
						}
						else if (medistock.IS_NEW_MEDICINE == 1)
						{
							listSub = listSub.Where(o =>
								o.IS_BUSINESS != 1
								&& (o.MEDICINE_LINE_ID == null
								|| o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__TTD)
								).ToList();
						}
						else if (medistock.IS_TRADITIONAL_MEDICINE == 1)
						{
							listSub = listSub.Where(o =>
								o.IS_BUSINESS != 1
								&& (o.MEDICINE_LINE_ID == null
								|| o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__CP_YHCT
								|| o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT)
								).ToList();
						}
						else
							listSub = listSub.Where(o => o.IS_BUSINESS != 1).ToList();
					}
					if (this.currentBid != null)
					{
						if (dicBidMedicine.Count > 0)
						{
							foreach (var item in dicBidMedicine)
							{
								if (item.Value == null)
								{
									continue;
								}

								var medicineType = listSub.FirstOrDefault(o => o.ID == item.Value.MEDICINE_TYPE_ID);
								if (medicineType == null)
									continue;
								MedicineTypeADO medicineTypeADO = new MedicineTypeADO(medicineType);
								medicineTypeADO.AMOUNT_IN_BID = item.Value.AMOUNT;
								medicineTypeADO.IMP_PRICE_IN_BID = item.Value.IMP_PRICE;
								medicineTypeADO.IMP_VAT_RATIO_IN_BID = item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null;
								medicineTypeADO.BidGroupCode = item.Value.BID_GROUP_CODE;
								medicineTypeADO.KeyField = Base.StaticMethod.GetTypeKey(item.Value.MEDICINE_TYPE_ID, item.Value.BID_GROUP_CODE);
								if (medicineType.IMP_UNIT_ID.HasValue)
								{
									medicineTypeADO.SERVICE_UNIT_NAME = medicineType.IMP_UNIT_NAME;
								}
								listMedicineType.Add(medicineTypeADO);
							}
						}
					}
					if (this.currentContract != null)
					{
						if (dicContractMety.Count > 0)
						{
							foreach (var item in dicContractMety)
							{
								var medicineType = listSub.FirstOrDefault(o => o.ID == item.Value.MEDICINE_TYPE_ID);
								if (medicineType == null)
									continue;
								MedicineTypeADO medicineTypeADO = new MedicineTypeADO(medicineType);
								medicineTypeADO.AMOUNT_IN_CONTRACT = item.Value.AMOUNT;
								medicineTypeADO.IMP_PRICE_IN_CONTRACT = item.Value.CONTRACT_PRICE;
								medicineTypeADO.IMP_VAT_RATIO_IN_CONTRACT = item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null;
								medicineTypeADO.MEDI_CONTRACT_METY_ID = item.Value.ID;
								if (medicineType.IMP_UNIT_ID.HasValue)
								{
									medicineTypeADO.SERVICE_UNIT_NAME = medicineType.IMP_UNIT_NAME;
								}
								medicineTypeADO.DISCOUNT_FROM_DATE = item.Value.DISCOUNT_FROM_DATE;
								medicineTypeADO.DISCOUNT_TO_DATE = item.Value.DISCOUNT_TO_DATE;
								medicineTypeADO.DISCOUNT_FROM_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_FROM_DATE ?? 0);
								medicineTypeADO.DISCOUNT_TO_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_TO_DATE ?? 0);
								listMedicineType.Add(medicineTypeADO);
							}
						}
					}
					if (this.currentBid == null && this.currentContract == null)
					{
						listMedicineType.AddRange((from r in listSub select new MedicineTypeADO(r)).ToList());
					}
					if (listMedicineType != null && listMedicineType.Count > 0)
					{
						listMedicineType.ForEach(o =>
						{
							if (o.IMP_UNIT_ID.HasValue) o.SERVICE_UNIT_NAME = o.IMP_UNIT_NAME;
						});
					}
				//	backgroundWorker1.ReportProgress(start);
				//	start += 100;
				//	count -= 100;
				//}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InitSourceMatiData()
		{
			try
			{
				//int start = 0;
				//int count = this.listMaterialTypeTemp.Count;
				//while (count > 0)
				//{
				//int limit = (count <= 100) ? count : 100;
				var listSub = this.listMaterialTypeTemp;
				//.Skip(start).Take(limit).ToList();


					if (medistock.IS_BUSINESS == 1)
					{
						listSub = listSub.Where(o => o.IS_BUSINESS == 1).ToList();
						if (listSub != null && listSub.Count > 0)
						{
							if (medistock.IS_SHOW_DRUG_STORE == 1)
							{
								listSub = listSub.Where(o => o.IS_DRUG_STORE == 1).ToList();
							}
							else
							{
								listSub = listSub.Where(o => o.IS_DRUG_STORE == null).ToList();
							}
						}
					}
					else
					{
						listSub = listSub.Where(o => o.IS_BUSINESS != 1).ToList();
					}

					if (this.currentBid != null)
					{
						if (dicBidMaterial.Count > 0)
						{
							foreach (var item in dicBidMaterial)
							{
								if (item.Value == null)
								{
									continue;
								}

								var materialType = listSub.FirstOrDefault(o => o.ID == item.Value.MATERIAL_TYPE_ID);
								if (materialType == null)
									continue;
								MaterialTypeADO materialTypeADO = new MaterialTypeADO(materialType);
								materialTypeADO.AMOUNT_IN_BID = item.Value.AMOUNT;
								materialTypeADO.IMP_PRICE_IN_BID = item.Value.IMP_PRICE;
								materialTypeADO.IMP_VAT_RATIO_IN_BID = item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null;
								materialTypeADO.BidGroupCode = item.Value.BID_GROUP_CODE;
								materialTypeADO.KeyField = Base.StaticMethod.GetTypeKey(item.Value.MATERIAL_TYPE_ID ?? 0, item.Value.BID_GROUP_CODE);
								if (materialType.IMP_UNIT_ID.HasValue)
								{
									materialTypeADO.SERVICE_UNIT_NAME = materialType.IMP_UNIT_NAME;
								}
								listMaterialType.Add(materialTypeADO);
							}
						}
					}
					if (this.currentContract != null)
					{
						if (dicContractMaty.Count > 0)
						{
							foreach (var item in dicContractMaty)
							{
								var materialType = listSub.FirstOrDefault(o => o.ID == item.Value.MATERIAL_TYPE_ID);
								if (materialType == null)
									continue;
								MaterialTypeADO materialTypeADO = new MaterialTypeADO(materialType);
								materialTypeADO.AMOUNT_IN_CONTRACT = item.Value.AMOUNT;
								materialTypeADO.IMP_PRICE_IN_CONTRACT = item.Value.CONTRACT_PRICE;
								materialTypeADO.IMP_VAT_RATIO_IN_CONTRACT = item.Value.IMP_VAT_RATIO.HasValue ? item.Value.IMP_VAT_RATIO * 100 : null;
								materialTypeADO.MEDI_CONTRACT_MATY_ID = item.Value.ID;
								if (materialType.IMP_UNIT_ID.HasValue)
								{
									materialTypeADO.SERVICE_UNIT_NAME = materialType.IMP_UNIT_NAME;
								}

								materialTypeADO.DISCOUNT_FROM_DATE = item.Value.DISCOUNT_FROM_DATE;
								materialTypeADO.DISCOUNT_TO_DATE = item.Value.DISCOUNT_TO_DATE;
								materialTypeADO.DISCOUNT_FROM_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_FROM_DATE ?? 0);
								materialTypeADO.DISCOUNT_TO_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.Value.DISCOUNT_TO_DATE ?? 0);
								listMaterialType.Add(materialTypeADO);
							}
						}
					}
					if (this.currentBid == null && this.currentContract == null)
					{
						listMaterialType.AddRange((from r in listSub select new MaterialTypeADO(r)).ToList());
					}
					if (listMaterialType != null && listMaterialType.Count > 0)
					{
						listMaterialType.ForEach(o =>
						{
							if (o.IMP_UNIT_ID.HasValue) o.SERVICE_UNIT_NAME = o.IMP_UNIT_NAME;
						});
					}
				//	backgroundWorker1.ReportProgress(start);
				//	start += 100;
				//	count -= 100;
				//}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

	}
}
