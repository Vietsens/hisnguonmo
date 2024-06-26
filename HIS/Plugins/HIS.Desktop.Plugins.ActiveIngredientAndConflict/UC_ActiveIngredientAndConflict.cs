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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using DevExpress.XtraGrid.Columns;
using HIS.UC.ConflictActiveIngredient;
using HIS.UC.ConflictActiveIngredient.ADO;
using HIS.UC.ActiveIngredent;
using HIS.UC.ActiveIngredent.ADO;
using HIS.Desktop.Plugins.ActiveIngredientAndConflict.entity;
using Inventec.Common.Controls.EditorLoader;
using AutoMapper;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Plugins.ActiveIngredientAndConflict.Properties;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.ActiveIngredientAndConflict
{
	public partial class UC_ActiveIngredientAndConflict : HIS.Desktop.Utility.UserControlBase
	{

		#region Declare
		List<ConflictActiveIngredientADO> listExecuteRoleDeleteADO { get; set; }
		List<ConflictActiveIngredientADO> listExecuteRoleInsertADO { get; set; }
		ConflictActiveIngredientADO conflictADO;
		ActiveIngredentADO activeADO;
		List<HIS_ACIN_INTERACTIVE> CAI { get; set; }
		internal Inventec.Desktop.Common.Modules.Module currentModule;
		ConflictActiveIngredientProcessor CAIProcessor;
		ActiveIngredentProcessor ActiveIngredentProcessor;
		UserControl ucGridControlCAI;
		UserControl ucGridControlAI;
		int start = 0;
		int limit = 0;
		int rowCount = 0;
		int dataTotal = 0;
		int start1 = 0;
		int limit1 = 0;
		int rowCount1 = 0;
		int dataTotal1 = 0;
		bool isCheckAll;
		internal List<HIS.UC.ActiveIngredent.ActiveIngredentADO> lstAIADOs { get; set; }
		internal List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO> lstCAIADOs { get; set; }
		List<HIS_ACTIVE_INGREDIENT> listAI;
		List<HIS_ACTIVE_INGREDIENT> listCAI;
		List<HIS_ACTIVE_INGREDIENT> listCAIAll = new List<HIS_ACTIVE_INGREDIENT>();
		List<HIS_ACTIVE_INGREDIENT> listAIAll = new List<HIS_ACTIVE_INGREDIENT>();
		long CAIIdCheckByCAI = 0;
		long isChoseCAI;
		long isChoseAI;
		long AIIdCheckByAI = 0;
		ConflictActiveIngredientADO currentADO;
		HIS_ACTIVE_INGREDIENT currentIngredient;
		List<HIS_ACIN_INTERACTIVE> activeIngredientAndConflict { get; set; }
		List<HIS_ACIN_INTERACTIVE> activeIngredientAndConflict1 { get; set; }
		public bool IsActivebtnOkeMec { get; private set; }
		public bool IsActivebtnOkeCon { get; private set; }

		Inventec.Desktop.Common.Modules.Module moduleData;
		List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO> dataNew = new List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO>();
		bool checkRa = false;

		#endregion


		#region Constructor

		public UC_ActiveIngredientAndConflict(Inventec.Desktop.Common.Modules.Module _moduleData)
			: base(_moduleData)
		{
			InitializeComponent();
			this.moduleData = _moduleData;
		}

		private void UC_ActiveIngredientAndConflict_Load(object sender, EventArgs e)
		{
			try
			{
				SetCaptionByLanguageKey();
				WaitingManager.Show();
				emptySpaceItem1.Size = new Size(915, 26);
				txtSearchCAI.Focus();
				txtSearchCAI.SelectAll();
				LoadDataToCombo();
				InitUcgrid1();
				InitUcgrid2();
				FillDataToGrid1(this);
				FillDataToGrid2(this);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}


		#endregion

		private void SetCaptionByLanguageKey()
		{
			try
			{

				////Khoi tao doi tuong resource
				Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ActiveIngredientAndConflict.Resources.Lang", typeof(HIS.Desktop.Plugins.ActiveIngredientAndConflict.UC_ActiveIngredientAndConflict).Assembly);

				////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
				this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.cboChooseBy.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.cboChooseBy.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnSearchA.Text = Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.btnSearchA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.txtSearchAI.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.txtSearchAI.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.btnSeachER.Text = Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.btnSeachER.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.txtSearchCAI.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.txtSearchCAI.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
				this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}


		#region Load column

		private void InitUcgrid1()
		{
			try
			{
				CAIProcessor = new ConflictActiveIngredientProcessor();
				ConflictActiveIngredientInitADO ado = new ConflictActiveIngredientInitADO();
				ado.ListConflictActiveIngredientColumn = new List<ConflictActiveIngredientColumn>();
				ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
				ado.ConflictActiveIngredientGrid_CellValueChanged = ConflictActiveIngredientGrid_CellValueChanged;
				ado.ConflictActiveIngredientGrid_MouseDown = ConflictActiveIngredient_MouseDown;
				ado.repositoryItemButtonEdit_HuongXuLy_ButtonClick1 = repositoryItemButtonEdit_HuongXuLy_ButtonClick1;
				ado.repositoryItemButtonEdit_CoChe_ButtonClick1 = repositoryItemButtonEdit_CoChe_ButtonClick1;
				ado.repositoryItemButtonEdit_HauQua_ButtonClick1 = repositoryItemButtonEdit_HauQua_ButtonClick1;
				//ado.repositoryItemButtonEdit_CoChe_EditValueChanged1 = repositoryItemButtonEdit_CoChe_EditValueChanged1;
				//ado.repositoryItemButtonEdit_HauQua_EditValueChanged1 = repositoryItemButtonEdit_HauQua_EditValueChanged1;
				object image = Properties.Resources.ResourceManager.GetObject("check1");

				ConflictActiveIngredientColumn colRadio1 = new ConflictActiveIngredientColumn("   ", "radio2", 30, true);
				colRadio1.VisibleIndex = 0;
				colRadio1.Visible = false;
				colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
				ado.ListConflictActiveIngredientColumn.Add(colRadio1);

				ConflictActiveIngredientColumn colCheck1 = new ConflictActiveIngredientColumn("   ", "check2", 40, true);
				colCheck1.VisibleIndex = 1;
				colCheck1.Visible = false;
				colCheck1.image = imageCollection.Images[0];
				colCheck1.ToolTip = "Chọn tất cả";
				colCheck1.imageAlignment = StringAlignment.Center;
				colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
				ado.ListConflictActiveIngredientColumn.Add(colCheck1);

				ConflictActiveIngredientColumn colMaHCXD = new ConflictActiveIngredientColumn(Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.MaHCXungDot", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "ACTIVE_INGREDIENT_CODE", 200, false);
				colMaHCXD.VisibleIndex = 2;
				ado.ListConflictActiveIngredientColumn.Add(colMaHCXD);

				ConflictActiveIngredientColumn colTenHCXD = new ConflictActiveIngredientColumn(Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.TenHCXungDot", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "ACTIVE_INGREDIENT_NAME", 400, false);
				colTenHCXD.VisibleIndex = 3;
				ado.ListConflictActiveIngredientColumn.Add(colTenHCXD);

				ConflictActiveIngredientColumn colMoTa = new ConflictActiveIngredientColumn(Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.MoTa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "DESCRIPTION", 360, true);
				colMoTa.VisibleIndex = 4;
				ado.ListConflictActiveIngredientColumn.Add(colMoTa);


				ConflictActiveIngredientColumn colCoChe = new ConflictActiveIngredientColumn();
				colCoChe.Caption = "Cơ chế";
				colCoChe.VisibleIndex = 5;
				colCoChe.Visible = true;
				colCoChe.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
				colCoChe.FieldName = "MECHANISM";
				colCoChe.AllowEdit = true;
				colCoChe.ColumnWidth = 100;
				ado.ListConflictActiveIngredientColumn.Add(colCoChe);

				ConflictActiveIngredientColumn colHauQua = new ConflictActiveIngredientColumn();
				colHauQua.Caption = "Hậu quả";
				colHauQua.VisibleIndex = 6;
				colHauQua.Visible = true;
				colHauQua.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
				colHauQua.FieldName = "CONSEQUENCE";
				colHauQua.AllowEdit = true;
				colHauQua.ColumnWidth = 100;
				ado.ListConflictActiveIngredientColumn.Add(colHauQua);

				ConflictActiveIngredientColumn colHuongXuLy = new ConflictActiveIngredientColumn();
				colHuongXuLy.Caption = "Hướng xử lý";
				colHuongXuLy.VisibleIndex = 7;
				colHuongXuLy.Visible = true;
				colHuongXuLy.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
				colHuongXuLy.FieldName = "INSTRUCTION";
				colHuongXuLy.AllowEdit = true;
				colHuongXuLy.ColumnWidth = 100;

				ado.ListConflictActiveIngredientColumn.Add(colHuongXuLy);

				ConflictActiveIngredientColumn colMucDo = new ConflictActiveIngredientColumn(Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.MucDo", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "INTERACTIVE_GRADE_ID", 100, true);
				colMucDo.VisibleIndex = 8;
				ado.ListConflictActiveIngredientColumn.Add(colMucDo);

				this.ucGridControlCAI = (UserControl)CAIProcessor.Run(ado);
				if (ucGridControlCAI != null)
				{
					this.panelControl2.Controls.Add(this.ucGridControlCAI);
					this.ucGridControlCAI.Dock = DockStyle.Fill;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repositoryItemButtonEdit_CoChe_EditValueChanged1(object sender, HIS_ACTIVE_INGREDIENT data, ConflictActiveIngredientADO ado)
		{
			try
			{
				if (IsActivebtnOkeMec)
				{
					IsActivebtnOkeMec = false;
					return;
				}
				this.currentIngredient = data;
				this.currentADO = ado;
				ButtonEdit editor = sender as ButtonEdit;

				WaitingManager.Show();
				if (dataNew == null || dataNew.Count() == 0)
				{
					this.dataNew = (from r in listCAI select new HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO(r)).ToList();
				}
				if (dataNew != null && dataNew.Count() > 0)
				{
					var check = dataNew.FirstOrDefault(o => o.ID == this.currentADO.ID);
					if (check != null)
					{
						
							check.check2 = true;
							check.MECHANISM = editor.Text;
							Valid(check);
							popupContainerControl2.HidePopup();
						
					}

					dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
					if (ucGridControlCAI != null)
					{
						CAIProcessor.Reload(ucGridControlCAI, dataNew);
					}

					else
					{
						FillDataToGrid1(this);
					}
				}

				WaitingManager.Hide();
				checkRa = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			
		}

		private void repositoryItemButtonEdit_HauQua_EditValueChanged1(object sender, HIS_ACTIVE_INGREDIENT data, ConflictActiveIngredientADO ado)
		{

			try
			{
				if (IsActivebtnOkeCon)
				{
					IsActivebtnOkeCon = false;
					return;
				}
				this.currentIngredient = data;
				this.currentADO = ado;
				ButtonEdit editor = sender as ButtonEdit;

				WaitingManager.Show();
				if (dataNew == null || dataNew.Count() == 0)
				{
					this.dataNew = (from r in listCAI select new HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO(r)).ToList();
				}
				if (dataNew != null && dataNew.Count() > 0)
				{
					var check = dataNew.FirstOrDefault(o => o.ID == this.currentADO.ID);
					if (check != null)
					{
						
							check.check2 = true;
							check.CONSEQUENCE = editor.Text;
							Valid(check);
							popupContainerControl2.HidePopup();
						
					}

					dataNew = dataNew.OrderByDescending(p => p.check2).ToList();					
						if (ucGridControlCAI != null)
						{
							CAIProcessor.Reload(ucGridControlCAI, dataNew);
						}

						else
						{
							FillDataToGrid1(this);
						}
				}

				WaitingManager.Hide();
				checkRa = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repositoryItemButtonEdit_CoChe_ButtonClick1(object sender, HIS_ACTIVE_INGREDIENT data, ConflictActiveIngredientADO ado)
		{
			this.currentIngredient = data;
			this.currentADO = ado;
			ButtonEdit editor = sender as ButtonEdit;
			Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
			popupContainerControl2.ShowPopup(new Point(buttonPosition.X + 700, buttonPosition.Bottom + 200));
			mmMec.Text = ado.MECHANISM;
		}
		private void repositoryItemButtonEdit_HauQua_ButtonClick1(object sender, HIS_ACTIVE_INGREDIENT data, ConflictActiveIngredientADO ado)
		{
			this.currentIngredient = data;
			this.currentADO = ado;
			ButtonEdit editor = sender as ButtonEdit;
			Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
			popupContainerControl1.ShowPopup(new Point(buttonPosition.X + 700, buttonPosition.Bottom + 200));
			mmCon.Text = ado.CONSEQUENCE;
		}
		private void repositoryItemButtonEdit_HuongXuLy_ButtonClick1(object sender, HIS_ACTIVE_INGREDIENT data, ConflictActiveIngredientADO ado)
		{
			this.currentIngredient = data;
			this.currentADO = ado;
			ButtonEdit editor = sender as ButtonEdit;
			Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
			popupControlContainerHuongXuLy.ShowPopup(new Point(buttonPosition.X + 700, buttonPosition.Bottom + 200));
			txtHuongXuLy.Text = ado.INSTRUCTION;
		}

		private void InitUcgrid2()
		{
			try
			{
				ActiveIngredentProcessor = new ActiveIngredentProcessor();
				ActiveIngredentInitADO ado = new ActiveIngredentInitADO();
				ado.ListActiveIngredentColumn = new List<UC.ActiveIngredent.ActiveIngredentColumn>();
				ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
				ado.ActiveIngredentGrid_MouseDown = ActiveIngredent_MouseDown;
				object image = Properties.Resources.ResourceManager.GetObject("check1");

				ActiveIngredentColumn colRadio2 = new ActiveIngredentColumn("   ", "radio1", 20, true);
				colRadio2.VisibleIndex = 0;
				colRadio2.Visible = false;
				colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
				ado.ListActiveIngredentColumn.Add(colRadio2);

				ActiveIngredentColumn colCheck2 = new ActiveIngredentColumn("   ", "check1", 30, true);
				colCheck2.VisibleIndex = 1;
				colCheck2.Visible = false;
				colCheck2.image = imageCollection.Images[0];
				//colCheck2.image = StringAlignment.Center;
				colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
				ado.ListActiveIngredentColumn.Add(colCheck2);

				ActiveIngredentColumn colMaHC = new ActiveIngredentColumn(Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.MaHC", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "ACTIVE_INGREDIENT_CODE", 300, false);
				colMaHC.VisibleIndex = 2;
				ado.ListActiveIngredentColumn.Add(colMaHC);

				ActiveIngredentColumn colTenHC = new ActiveIngredentColumn(Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.TenHC", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "ACTIVE_INGREDIENT_NAME", 600, false);
				colTenHC.VisibleIndex = 3;
				ado.ListActiveIngredentColumn.Add(colTenHC);

				this.ucGridControlAI = (UserControl)ActiveIngredentProcessor.Run(ado);
				if (ucGridControlAI != null)
				{
					this.panelControl1.Controls.Add(this.ucGridControlAI);
					this.ucGridControlAI.Dock = DockStyle.Fill;
				}
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		#endregion


		#region Load Combo

		private void LoadDataToCombo()
		{
			try
			{
				LoadComboStatus();
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadComboStatus()
		{
			try
			{
				List<Status> status = new List<Status>();
				status.Add(new Status(1, Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.HCXD", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
				status.Add(new Status(2, Inventec.Common.Resource.Get.Value("UC_ActiveIngredientAndConflict.HC", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));

				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
				ControlEditorLoader.Load(cboChooseBy, status, controlEditorADO);
				cboChooseBy.EditValue = status[1].id;
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		#endregion


		#region Method


		private void FillDataToGrid2(UC_ActiveIngredientAndConflict uCActiveIngredientAndConflict)
		{
			try
			{
				int numPageSize;
				if (ucPaging1.pagingGrid != null)
				{
					numPageSize = ucPaging1.pagingGrid.PageSize;
				}
				else
				{
					numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
				}
				FillDataToGridActiveIngredient(new CommonParam(0, numPageSize));
				CommonParam param = new CommonParam();
				param.Limit = rowCount1;
				param.Count = dataTotal1;
				ucPaging1.Init(FillDataToGridActiveIngredient, param, numPageSize, (GridControl)ActiveIngredentProcessor.GetGridControl(ucGridControlAI));
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void FillDataToGridActiveIngredient(object data)
		{
			try
			{
				WaitingManager.Show();
				listAI = new List<HIS_ACTIVE_INGREDIENT>();
				int start1 = ((CommonParam)data).Start ?? 0;
				int limit1 = ((CommonParam)data).Limit ?? 0;
				CommonParam param = new CommonParam(start1, limit1);
				MOS.Filter.HisAcinInteractiveFilter ActiveIngredientFilter = new HisAcinInteractiveFilter();
				ActiveIngredientFilter.ORDER_DIRECTION = "DESC";
				ActiveIngredientFilter.ORDER_FIELD = "MODIFY_TIME";
				ActiveIngredientFilter.KEY_WORD = txtSearchAI.Text;

				if ((long)cboChooseBy.EditValue == 2)
				{
					isChoseAI = (long)cboChooseBy.EditValue;
				}

				var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<HIS_ACTIVE_INGREDIENT>>(
					HisRequestUriStore.HIS_ACTIVE_INGREDIENT_GET,
					HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
					ActiveIngredientFilter,
					param);

				listAIAll = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_ACTIVE_INGREDIENT>>(
					HisRequestUriStore.HIS_ACTIVE_INGREDIENT_GET,
					HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
					ActiveIngredientFilter,
					new CommonParam());

				lstAIADOs = new List<HIS.UC.ActiveIngredent.ActiveIngredentADO>();

				if (sar != null && sar.Data.Count > 0)
				{
					listAI = sar.Data;
					foreach (var item in listAI)
					{
						HIS.UC.ActiveIngredent.ActiveIngredentADO activeIngredentADO = new HIS.UC.ActiveIngredent.ActiveIngredentADO(item);
						if (isChoseAI == 2)
						{
							activeIngredentADO.isKeyChoose = true;
						}

						lstAIADOs.Add(activeIngredentADO);
					}
				}

				if (activeIngredientAndConflict != null && activeIngredientAndConflict.Count > 0)
				{
					foreach (var item in activeIngredientAndConflict)
					{
						var check = lstAIADOs.FirstOrDefault(o => o.ID == item.ACTIVE_INGREDIENT_ID);
						if (check != null)
						{
							check.check1 = true;
						}
					}
				}

				lstAIADOs = lstAIADOs.OrderByDescending(p => p.check1).ToList();

				if (activeADO != null)
				{
					var active = lstAIADOs.Where(o => o.ID == activeADO.ID).FirstOrDefault();
					if (active != null)
					{
						active.radio1 = true;
						lstAIADOs = lstAIADOs.OrderByDescending(p => p.radio1).ToList();
					}
				}

				if (ucGridControlAI != null)
				{
					ActiveIngredentProcessor.Reload(ucGridControlAI, lstAIADOs);
				}
				rowCount1 = (data == null ? 0 : lstAIADOs.Count);
				dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void FillDataToGrid1(UC_ActiveIngredientAndConflict uCActiveIngredientAndConflict)
		{
			try
			{
				int numPageSize;
				if (ucPaging2.pagingGrid != null)
				{
					numPageSize = ucPaging2.pagingGrid.PageSize;
				}
				else
				{
					numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
				}
				FillDataToGridConflictActiveIngredient(new CommonParam(0, numPageSize));

				CommonParam param = new CommonParam();
				param.Limit = rowCount;
				param.Count = dataTotal;
				ucPaging2.Init(FillDataToGridConflictActiveIngredient, param, numPageSize, (GridControl)CAIProcessor.GetGridControl(ucGridControlCAI));
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void FillDataToGridConflictActiveIngredient(object data)
		{
			try
			{
				WaitingManager.Show();
				listCAI = new List<HIS_ACTIVE_INGREDIENT>();
				int start = ((CommonParam)data).Start ?? 0;
				int limit = ((CommonParam)data).Limit ?? 0;
				CommonParam param = new CommonParam(start, limit);
				MOS.Filter.HisActiveIngredientFilter CAIFillter = new HisActiveIngredientFilter();
				CAIFillter.ORDER_FIELD = "MODIFY_TIME";
				CAIFillter.ORDER_DIRECTION = "DESC";
				CAIFillter.KEY_WORD = txtSearchCAI.Text;

				if ((long)cboChooseBy.EditValue == 1)
				{
					isChoseCAI = (long)cboChooseBy.EditValue;
				}

				var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT>>(
					 HisRequestUriStore.HIS_ACTIVE_INGREDIENT_GET,
					 HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
					 CAIFillter,
					 param);

				listCAIAll = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT>>(
					 HisRequestUriStore.HIS_ACTIVE_INGREDIENT_GET,
					 HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
					 CAIFillter,
					 new CommonParam());



				lstCAIADOs = new List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO>();

				if (rs != null && rs.Data.Count > 0)
				{
					listCAI = rs.Data;
					foreach (var item in listCAI)
					{
						HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO ConflictActiveIngredientADO = new HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO(item);
						if (isChoseCAI == 1)
						{
							ConflictActiveIngredientADO.isKeyChoose1 = true;
						}
						lstCAIADOs.Add(ConflictActiveIngredientADO);
					}
				}



				if (activeIngredientAndConflict1 != null && activeIngredientAndConflict1.Count > 0)
				{
					foreach (var item in activeIngredientAndConflict1)
					{
						var check = lstCAIADOs.FirstOrDefault(o => o.ID == item.CONFLICT_ID);
						if (check != null)
						{
							check.check2 = true;
							check.INTERACTIVE_GRADE_ID = item.INTERACTIVE_GRADE_ID;
							check.DESCRIPTION = item.DESCRIPTION;
							check.INSTRUCTION = item.INSTRUCTION;
							check.MECHANISM = item.MECHANISM;
							check.CONSEQUENCE= item.CONSEQUENCE;
						}
					}
				}

				lstCAIADOs = lstCAIADOs.OrderByDescending(p => p.check2).ToList();

				if (conflictADO != null)
				{
					var conflict = lstCAIADOs.Where(o => o.ID == conflictADO.ID).FirstOrDefault();
					if (conflict != null)
					{
						conflict.radio2 = true;
						lstCAIADOs = lstCAIADOs.OrderByDescending(p => p.radio2).ToList();
					}
				}


				if (ucGridControlCAI != null)
				{
					CAIProcessor.Reload(ucGridControlCAI, lstCAIADOs);
				}
				rowCount = (data == null ? 0 : lstCAIADOs.Count);
				dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		public void FindShortcut1()
		{
			try
			{
				btnFind1_Click(null, null);
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		public void FindShortcut2()
		{
			try
			{
				btnFind2_Click(null, null);
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		public void SaveShortcut()
		{
			try
			{
				btnSave.Focus();
				btnSave_Click(null, null);
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		#endregion


		#region Event

		private void btnFind1_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				FillDataToGrid2(this);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnFind2_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				FillDataToGrid1(this);
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
		{
			try
			{
				checkRa = false;
				isChoseAI = 0;
				isChoseCAI = 0;
				CAIIdCheckByCAI = 0;
				AIIdCheckByAI = 0;
				conflictADO = null;
				activeADO = null;
				activeIngredientAndConflict = null;
				activeIngredientAndConflict1 = null;
				FillDataToGrid1(this);
				FillDataToGrid2(this);
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
				bool resultSuccess = false;
				bool validate = true;
				CommonParam param = new CommonParam();
				if (!CheckValidationGrid((List<ConflictActiveIngredientADO>)CAIProcessor.GetDataGridView(ucGridControlCAI)))
					return;
				WaitingManager.Show();
				if (ucGridControlAI != null && ucGridControlCAI != null)
				{
					object ai = ActiveIngredentProcessor.GetDataGridView(ucGridControlAI);
					object cai = CAIProcessor.GetDataGridView(ucGridControlCAI);
					if (isChoseCAI == 1)
					{

						if (ai is List<HIS.UC.ActiveIngredent.ActiveIngredentADO>)
						{
							var data = (List<HIS.UC.ActiveIngredent.ActiveIngredentADO>)ai;
							if (data != null && data.Count > 0)
							{
								if (checkRa == true)
								{
									//Danh sach cac user duoc check
									MOS.Filter.HisAcinInteractiveFilter filter = new HisAcinInteractiveFilter();
									filter.ACTIVE_INGREDIENT_CONFLICT_ID = CAIIdCheckByCAI;

									var aIAC = new BackendAdapter(param).Get<List<HIS_ACIN_INTERACTIVE>>(
									   HisRequestUriStore.HIS_ACIN_INTERACTIVE_GET,
									   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
									   filter,
									   param);

									List<long> listAI = aIAC.Select(p => p.ACTIVE_INGREDIENT_ID).ToList();

									var dataCheckeds = data.Where(p => p.check1 == true).ToList();

									var dataUpdate = dataCheckeds;
									//List xoa

									var dataDeletes = data.Where(o => aIAC.Select(p => p.ACTIVE_INGREDIENT_ID)
										.Contains(o.ID) && o.check1 == false).ToList();

									//list them
									var dataCreates = dataCheckeds.Where(o => !aIAC.Select(p => p.ACTIVE_INGREDIENT_ID)
										.Contains(o.ID)).ToList();
									//if (dataCheckeds.Count != aIAC.Select(p => p.ACTIVE_INGREDIENT_ID).Count())
									//{

									foreach (var item in dataCreates)
									{
										dataUpdate.Remove(item);
									}

									bool checkDelete = false;
									bool checkCreate = false;
									bool checkUpdate = false;


									//if (dataChecked.Count != aIAC.Select(p => p.CONFLICT_ID).Count())
									//{

									conflictADO = new ConflictActiveIngredientADO();
									conflictADO = ((List<ConflictActiveIngredientADO>)CAIProcessor.GetDataGridView(ucGridControlCAI)).FirstOrDefault(o => o.radio2 == true);

									if (dataUpdate != null && dataUpdate.Count > 0)
									{
										List<HIS_ACIN_INTERACTIVE> haiUpdate = new List<HIS_ACIN_INTERACTIVE>();
										foreach (var item in dataUpdate)
										{
											HIS_ACIN_INTERACTIVE haiID = new HIS_ACIN_INTERACTIVE();
											haiID.ID = aIAC.Where(o => o.ACTIVE_INGREDIENT_ID == item.ID && o.CONFLICT_ID == CAIIdCheckByCAI).FirstOrDefault().ID;
											haiID.CONFLICT_ID = CAIIdCheckByCAI;
											haiID.ACTIVE_INGREDIENT_ID = item.ID;
											if (conflictADO != null)
											{
												if (conflictADO.INTERACTIVE_GRADE_ID < 0)
												{
													validate = false;
												}
												haiID.INTERACTIVE_GRADE_ID = conflictADO.INTERACTIVE_GRADE_ID;
												haiID.DESCRIPTION = conflictADO.DESCRIPTION;
												haiID.INSTRUCTION = conflictADO.INSTRUCTION;
												haiID.MECHANISM = conflictADO.MECHANISM;
												haiID.CONSEQUENCE = conflictADO.CONSEQUENCE;
											}
											haiUpdate.Add(haiID);
										}

										var updateResult = new BackendAdapter(param).Post<List<HIS_ACIN_INTERACTIVE>>(
												   "/api/HisAcinInteractive/UpdateList",
												   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
												   haiUpdate,
												   param);
										if (updateResult != null && updateResult.Count > 0)
										{
											checkUpdate = true;
											resultSuccess = true;
											foreach (var item in updateResult)
											{
												activeIngredientAndConflict.FirstOrDefault(o => o.ID == item.ID).INTERACTIVE_GRADE_ID = item.INTERACTIVE_GRADE_ID;
												activeIngredientAndConflict.FirstOrDefault(o => o.ID == item.ID).DESCRIPTION = item.DESCRIPTION;
												activeIngredientAndConflict.FirstOrDefault(o => o.ID == item.ID).INSTRUCTION = item.INSTRUCTION;
												activeIngredientAndConflict.FirstOrDefault(o => o.ID == item.ID).MECHANISM = item.MECHANISM;
												activeIngredientAndConflict.FirstOrDefault(o => o.ID == item.ID).CONSEQUENCE = item.CONSEQUENCE;
											}
										}
									}


									if (dataDeletes != null && dataDeletes.Count > 0)
									{
										List<long> deleteIds = aIAC.Where(o => dataDeletes.Select(p => p.ID)
											.Contains(o.ACTIVE_INGREDIENT_ID)).Select(o => o.ID).ToList();
										bool deleteResult = new BackendAdapter(param).Post<bool>(
												  "/api/HisAcinInteractive/DeleteList",
												  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
												  deleteIds,
												  param);
										if (deleteResult)
										{
											resultSuccess = true;
											checkDelete = true;
											activeIngredientAndConflict = activeIngredientAndConflict.Where(o =>
												!deleteIds.Contains(o.ID)).ToList();
										}

									}

									if (dataCreates != null && dataCreates.Count > 0)
									{
										List<HIS_ACIN_INTERACTIVE> haiCreates = new List<HIS_ACIN_INTERACTIVE>();
										foreach (var item in dataCreates)
										{
											HIS_ACIN_INTERACTIVE haiCreate = new HIS_ACIN_INTERACTIVE();
											haiCreate.CONFLICT_ID = CAIIdCheckByCAI;
											haiCreate.ACTIVE_INGREDIENT_ID = item.ID;
											if (conflictADO != null)
											{
												haiCreate.INTERACTIVE_GRADE_ID = conflictADO.INTERACTIVE_GRADE_ID;
												haiCreate.DESCRIPTION = conflictADO.DESCRIPTION;
												haiCreate.INSTRUCTION = conflictADO.INSTRUCTION; 
												haiCreate.CONSEQUENCE = conflictADO.CONSEQUENCE;
												haiCreate.MECHANISM = conflictADO.MECHANISM;
											}
											if (conflictADO.INTERACTIVE_GRADE_ID < 0)
												validate = false;
											haiCreates.Add(haiCreate);
										}

										if (validate)
										{
											var createResult = new BackendAdapter(param).Post<List<HIS_ACIN_INTERACTIVE>>(
													   "/api/HisAcinInteractive/CreateList",
													   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
													   haiCreates,
													   param);
											if (createResult != null && createResult.Count > 0)
											{
												checkCreate = true;
												resultSuccess = true;
												activeIngredientAndConflict.AddRange(createResult);
											}
										}
									}

									WaitingManager.Hide();
									if (validate)
									{
										if (checkCreate == true || checkDelete == true || checkUpdate == true)
										{
											#region Show message
											MessageManager.Show(this.ParentForm, param, resultSuccess);
											#endregion

											#region Process has exception
											SessionManager.ProcessTokenLost(param);
											#endregion
											//}

											data = data.OrderByDescending(p => p.check1).ToList();
											if (ucGridControlAI != null)
											{
												ActiveIngredentProcessor.Reload(ucGridControlAI, data);
											}
										}
									}
									else
									{
										DevExpress.XtraEditors.XtraMessageBox.Show("Mức độ không được âm", "Thông báo");
									}
									WaitingManager.Hide();
								}
								else
								{
									DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn hoạt chất tương tác");
									WaitingManager.Hide();
								}
							}
						}
					}
					if (isChoseAI == 2)
					{
						var hisActive = new List<HIS_ACTIVE_INGREDIENT>();
						if (cai is List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO>)
						{
							var data = (List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO>)cai;

							if (data != null && data.Count > 0)
							{
								if (checkRa == true)
								{
									//Danh sach cac user duoc check
									MOS.Filter.HisAcinInteractiveFilter filter = new HisAcinInteractiveFilter();
									filter.ACTIVE_INGREDIENT_ID = AIIdCheckByAI;
									var aIAC = new BackendAdapter(param).Get<List<HIS_ACIN_INTERACTIVE>>(
									   HisRequestUriStore.HIS_ACIN_INTERACTIVE_GET,
									   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
									   filter,
									   param);

									var listCAI = aIAC.Select(p => p.CONFLICT_ID).ToList();

									var dataChecked = data.Where(p => p.check2 == true).ToList();

									var dataUpdate = dataChecked;
									//List xoa

									var dataDelete = data.Where(o => aIAC.Select(p => p.CONFLICT_ID)
										.Contains(o.ID) && o.check2 == false).ToList();

									//list them
									var dataCreate = dataChecked.Where(o => !aIAC.Select(p => p.CONFLICT_ID)
										.Contains(o.ID)).ToList();
									//var update = (HIS_ACIN_INTERACTIVE)data;                               
									// Inventec.Common.Mapper.DataObjectMapper.Map<List<HIS_ACTIVE_INGREDIENT>>(hisActive, data);
									foreach (var item in dataCreate)
									{
										dataUpdate.Remove(item);
									}

									bool checkDelete = false;
									bool checkCreate = false;
									bool checkUpdate = false;

									activeADO = new ActiveIngredentADO();
									activeADO = ((List<ActiveIngredentADO>)ActiveIngredentProcessor.GetDataGridView(ucGridControlAI)).FirstOrDefault(o => o.radio1 == true);
									//if (dataChecked.Count != aIAC.Select(p => p.CONFLICT_ID).Count())
									//{
									if (dataUpdate != null && dataUpdate.Count > 0)
									{
										List<HIS_ACIN_INTERACTIVE> haiUpdate = new List<HIS_ACIN_INTERACTIVE>();
										foreach (var item in dataUpdate)
										{
											HIS_ACIN_INTERACTIVE haiID = new HIS_ACIN_INTERACTIVE();
											haiID.ID = aIAC.Where(o => o.CONFLICT_ID == item.ID && o.ACTIVE_INGREDIENT_ID == AIIdCheckByAI).FirstOrDefault().ID;
											haiID.ACTIVE_INGREDIENT_ID = AIIdCheckByAI;
											haiID.CONFLICT_ID = item.ID;
											if (item.INTERACTIVE_GRADE_ID < 0)
											{
												validate = false;
											}

											haiID.INTERACTIVE_GRADE_ID = item.INTERACTIVE_GRADE_ID;
											haiID.DESCRIPTION = item.DESCRIPTION;
											haiID.INSTRUCTION = item.INSTRUCTION;
											haiID.CONSEQUENCE = item.CONSEQUENCE;
											haiID.MECHANISM = item.MECHANISM;
											haiUpdate.Add(haiID);
										}

										if (validate)
										{
											var updateResult = new BackendAdapter(param).Post<List<HIS_ACIN_INTERACTIVE>>(
													   "/api/HisAcinInteractive/UpdateList",
													   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
													   haiUpdate,
													   param);
											if (updateResult != null && updateResult.Count > 0)
											{
												checkUpdate = true;
												resultSuccess = true;
												foreach (var item in updateResult)
												{
													activeIngredientAndConflict1.FirstOrDefault(o => o.ID == item.ID).INTERACTIVE_GRADE_ID = item.INTERACTIVE_GRADE_ID;
													activeIngredientAndConflict1.FirstOrDefault(o => o.ID == item.ID).DESCRIPTION = item.DESCRIPTION;
													activeIngredientAndConflict1.FirstOrDefault(o => o.ID == item.ID).INSTRUCTION = item.INSTRUCTION;
													activeIngredientAndConflict1.FirstOrDefault(o => o.ID == item.ID).CONSEQUENCE = item.CONSEQUENCE;
													activeIngredientAndConflict1.FirstOrDefault(o => o.ID == item.ID).MECHANISM = item.MECHANISM;
												}
											}
										}
									}

									if (dataDelete != null && dataDelete.Count > 0)
									{
										List<long> deleteId = aIAC.Where(o => dataDelete.Select(p => p.ID)

											.Contains(o.CONFLICT_ID)).Select(o => o.ID).ToList();
										bool deleteResult = new BackendAdapter(param).Post<bool>(
												  "/api/HisAcinInteractive/DeleteList",
												  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
												  deleteId,
												  param);
										if (deleteResult)
										{
											checkDelete = true;
											resultSuccess = true;
											activeIngredientAndConflict1 = activeIngredientAndConflict1.Where(o =>
												!deleteId.Contains(o.ID)).ToList();
										}

									}

									if (dataCreate != null && dataCreate.Count > 0)
									{
										List<HIS_ACIN_INTERACTIVE> haiCreate = new List<HIS_ACIN_INTERACTIVE>();
										foreach (var item in dataCreate)
										{
											HIS_ACIN_INTERACTIVE haiID = new HIS_ACIN_INTERACTIVE();
											haiID.ACTIVE_INGREDIENT_ID = AIIdCheckByAI;
											haiID.CONFLICT_ID = item.ID;
											if (item.INTERACTIVE_GRADE_ID < 0)
											{
												validate = false;
											}
											haiID.INTERACTIVE_GRADE_ID = item.INTERACTIVE_GRADE_ID;
											haiID.DESCRIPTION = item.DESCRIPTION;
											haiID.INSTRUCTION = item.INSTRUCTION;
											haiID.MECHANISM = item.MECHANISM;
											haiID.CONSEQUENCE = item.CONSEQUENCE;
											haiCreate.Add(haiID);
										}

										if (validate)
										{
											var createResult = new BackendAdapter(param).Post<List<HIS_ACIN_INTERACTIVE>>(
													   "/api/HisAcinInteractive/CreateList",
													   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
													   haiCreate,
													   param);


											if (createResult != null && createResult.Count > 0)
											{
												Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => createResult), createResult));
												checkCreate = true;
												resultSuccess = true;
												activeIngredientAndConflict1.AddRange(createResult);
											}
										}
									}
									WaitingManager.Hide();

									if (validate)
									{
										if (checkCreate == true || checkDelete == true || checkUpdate == true)
										{
											#region Show message
											MessageManager.Show(this.ParentForm, param, resultSuccess);
											#endregion

											#region Process has exception
											SessionManager.ProcessTokenLost(param);
											#endregion
											//}



											data = data.OrderByDescending(p => p.check2).ToList();
											if (ucGridControlCAI != null)
											{
												CAIProcessor.Reload(ucGridControlCAI, data);
											}
										}
									}
									else
									{
										DevExpress.XtraEditors.XtraMessageBox.Show("Mức độ không được âm", "Thông báo");
									}
									WaitingManager.Hide();
								}
								else
								{
									DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn hoạt chất");
									WaitingManager.Hide();
								}

							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ConflictActiveIngredientGrid_CellValueChanged(ConflictActiveIngredientADO data, CellValueChangedEventArgs e)
		{
			try
			{
				//if (data != null)
				//{
				//    if (data.check1 == true)
				//    {

				//        listExecuteRoleInsertADO.Add(data);
				//    }
				//    else
				//    {

				//        listExecuteRoleDeleteADO.Add(data);
				//    }
				//}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ConflictActiveIngredient_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if (isChoseCAI == 1)
				{
					return;
				}

				WaitingManager.Show();
				if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
				{
					GridView view = sender as GridView;
					GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
					GridHitInfo hi = view.CalcHitInfo(e.Location);

					if (hi.HitTest == GridHitTest.Column)
					{
						if (hi.Column.FieldName == "check2")
						{
							var lstCheckAll = lstCAIADOs;
							List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO> lstChecks = new List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO>();

							if (lstCheckAll != null && lstCheckAll.Count > 0)
							{
								if (isCheckAll)
								{
									foreach (var item in lstCheckAll)
									{
										if (item.ID != null)
										{
											item.check2 = true;
											lstChecks.Add(item);
										}
										else
										{
											lstChecks.Add(item);
										}
									}
									isCheckAll = false;
									hi.Column.Image = imageCollection.Images[1];
								}
								else
								{
									foreach (var item in lstCheckAll)
									{
										if (item.ID != null)
										{
											item.check2 = false;
											lstChecks.Add(item);
										}
										else
										{
											lstChecks.Add(item);
										}
									}
									isCheckAll = true;
									hi.Column.Image = imageCollection.Images[0];
								}

								CAIProcessor.Reload(ucGridControlCAI, lstChecks);
								//??

							}
						}
					}
				}
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ActiveIngredent_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if (isChoseAI == 2)
				{
					return;
				}
				WaitingManager.Show();
				if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
				{
					GridView view = sender as GridView;
					GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
					GridHitInfo hi = view.CalcHitInfo(e.Location);

					if (hi.HitTest == GridHitTest.Column)
					{
						if (hi.Column.FieldName == "check1")
						{
							var lstCheckAll = lstAIADOs;
							List<HIS.UC.ActiveIngredent.ActiveIngredentADO> lstChecks = new List<HIS.UC.ActiveIngredent.ActiveIngredentADO>();

							if (lstCheckAll != null && lstCheckAll.Count > 0)
							{
								if (isCheckAll)
								{
									foreach (var item in lstCheckAll)
									{
										if (item.ID != null)
										{
											item.check1 = true;
											lstChecks.Add(item);
										}
										else
										{
											lstChecks.Add(item);
										}
									}
									isCheckAll = false;
									hi.Column.Image = imageCollection.Images[1];
								}
								else
								{
									foreach (var item in lstCheckAll)
									{
										if (item.ID != null)
										{
											item.check1 = false;
											lstChecks.Add(item);
										}
										else
										{
											lstChecks.Add(item);
										}
									}
									isCheckAll = true;
									hi.Column.Image = imageCollection.Images[0];
								}

								//ReloadData
								ActiveIngredentProcessor.Reload(ucGridControlAI, lstChecks);
								//??

							}
						}
					}
				}
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btn_Radio_Enable_Click1(HIS_ACTIVE_INGREDIENT data, ConflictActiveIngredientADO CAIado)
		{
			try
			{
				WaitingManager.Show();
				CommonParam param = new CommonParam();
				conflictADO = new ConflictActiveIngredientADO();
				conflictADO = CAIado;
				MOS.Filter.HisAcinInteractiveFilter filter = new HisAcinInteractiveFilter();
				filter.ACTIVE_INGREDIENT_CONFLICT_ID = data.ID;
				CAIIdCheckByCAI = data.ID;
				activeIngredientAndConflict = new BackendAdapter(param).Get<List<HIS_ACIN_INTERACTIVE>>(
								HisRequestUriStore.HIS_ACIN_INTERACTIVE_GET,
								HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
								filter,
								param);
				List<HIS.UC.ActiveIngredent.ActiveIngredentADO> dataNew = new List<HIS.UC.ActiveIngredent.ActiveIngredentADO>();
				dataNew = (from r in listAI select new HIS.UC.ActiveIngredent.ActiveIngredentADO(r)).ToList();
				if (activeIngredientAndConflict != null && activeIngredientAndConflict.Count > 0)
				{
					foreach (var itemUsername in activeIngredientAndConflict)
					{
						var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.ACTIVE_INGREDIENT_ID);
						if (check != null)
						{
							check.check1 = true;
						}

					}
				}

				dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
				if (ucGridControlAI != null)
				{
					ActiveIngredentProcessor.Reload(ucGridControlAI, dataNew);
				}
				WaitingManager.Hide();
				checkRa = true;
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btn_Radio_Enable_Click(HIS_ACTIVE_INGREDIENT data, ActiveIngredentADO ActiveAdo)
		{
			try
			{
				activeADO = new ActiveIngredentADO();
				activeADO = ActiveAdo;
				WaitingManager.Show();
				CommonParam param = new CommonParam();
				MOS.Filter.HisAcinInteractiveFilter filter = new HisAcinInteractiveFilter();
				filter.ACTIVE_INGREDIENT_ID = data.ID;
				AIIdCheckByAI = data.ID;
				activeIngredientAndConflict1 = new BackendAdapter(param).Get<List<HIS_ACIN_INTERACTIVE>>(
								HisRequestUriStore.HIS_ACIN_INTERACTIVE_GET,
								HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
								filter,
								param);
				List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO> dataNew = new List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO>();
				this.dataNew = dataNew = (from r in listCAI select new HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO(r)).ToList();
				if (activeIngredientAndConflict1 != null && activeIngredientAndConflict1.Count > 0)
				{

					foreach (var itemUsername in activeIngredientAndConflict1)
					{
						var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.CONFLICT_ID);
						if (check != null)
						{
							check.check2 = true;
							check.INTERACTIVE_GRADE_ID = itemUsername.INTERACTIVE_GRADE_ID;
							check.DESCRIPTION = itemUsername.DESCRIPTION;
							check.INSTRUCTION = itemUsername.INSTRUCTION;
							check.MECHANISM = itemUsername.MECHANISM;
							check.CONSEQUENCE = itemUsername.CONSEQUENCE;
						}
					}

					this.dataNew = dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
					if (ucGridControlCAI != null)
					{
						CAIProcessor.Reload(ucGridControlCAI, dataNew);
					}
				}
				else
				{
					FillDataToGrid1(this);
				}
				WaitingManager.Hide();
				checkRa = true;
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtSearchCAI_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				WaitingManager.Show();
				if (e.KeyCode == Keys.Enter)
				{
					FillDataToGrid1(this);
				}
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void txtSearchAI_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				WaitingManager.Show();
				if (e.KeyCode == Keys.Enter)
				{
					FillDataToGrid2(this);
				}
				WaitingManager.Hide();
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		#endregion

		private void simpleButton2_Click(object sender, EventArgs e)
		{
			BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE)).ToString(), false);
			BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_PATY)).ToString(), false);
			BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_MEDICINE_TYPE)).ToString(), false);
			BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_ROOM)).ToString(), false);
			BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_MEDICINE_PATY)).ToString(), false);
			CommonParam param = new CommonParam();
			#region Show message
			MessageManager.Show(this.ParentForm, param, true);
			#endregion
		}

		private void simpleButton1_Click(object sender, EventArgs e)
		{
			BackendDataWorker.Reset<V_HIS_SERVICE>();
			BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
			BackendDataWorker.Reset<V_HIS_MEDICINE_TYPE>();
			BackendDataWorker.Reset<V_HIS_SERVICE_ROOM>();
			BackendDataWorker.Reset<V_HIS_MEDICINE_PATY>();
			BackendDataWorker.Reset<MedicineMaterialTypeComboADO>();
			CommonParam param = new CommonParam();
			#region Show message
			MessageManager.Show(this.ParentForm, param, true);
			#endregion
		}

		private void btnSkip_Click(object sender, EventArgs e)
		{
			try
			{
				txtHuongXuLy.Text = "";
				popupControlContainerHuongXuLy.HidePopup();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnAgree_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				if (dataNew == null || dataNew.Count() == 0)
				{
					this.dataNew = (from r in listCAI select new HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO(r)).ToList();
				}
				if (dataNew != null && dataNew.Count() > 0)
				{
					var check = dataNew.FirstOrDefault(o => o.ID == this.currentADO.ID);
					if (check != null)
					{
						
							check.check2 = true;
						check.INSTRUCTION = txtHuongXuLy.Text;
							txtHuongXuLy.Text = "";
							popupControlContainerHuongXuLy.HidePopup();
						
					}

					dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
					if (ucGridControlCAI != null)
					{
						CAIProcessor.Reload(ucGridControlCAI, dataNew);
					}

					else
					{
						FillDataToGrid1(this);
					}
				}

				WaitingManager.Hide();
				checkRa = true;
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			try
			{
				
				if ((listAIAll == null || listAIAll.Count() == 0) && (listCAIAll == null || listCAIAll.Count() == 0))
				{
					DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu!");
					return;
				}
				WaitingManager.Show();

				List<ActiveIngredientADO> lstADO = new List<ActiveIngredientADO>();

				foreach (var parent in listAIAll)
				{
					CommonParam param = new CommonParam();
					MOS.Filter.HisAcinInteractiveFilter filter = new HisAcinInteractiveFilter();
					filter.ACTIVE_INGREDIENT_ID = parent.ID;
					var dt = new BackendAdapter(param).Get<List<HIS_ACIN_INTERACTIVE>>(
									 HisRequestUriStore.HIS_ACIN_INTERACTIVE_GET,
									 HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
									 filter,
									 param);
					List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO> dataNew = new List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO>();
					this.dataNew = dataNew = (from r in listCAIAll select new HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO(r)).ToList();
					if (dt != null && dt.Count > 0)
					{
						foreach (var itemUsername in dt)
						{
							ActiveIngredientADO ado = new ActiveIngredientADO();
							ado.ACTIVE_INGREDIENT_CODE = parent.ACTIVE_INGREDIENT_CODE;
							ado.ACTIVE_INGREDIENT_NAME = parent.ACTIVE_INGREDIENT_NAME;
							var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.CONFLICT_ID);
							if (check != null)
							{						
								ado.ACTIVE_INGREDIENT_CODE_XD = check.ACTIVE_INGREDIENT_CODE;
								ado.ACTIVE_INGREDIENT_NAME_XD = check.ACTIVE_INGREDIENT_NAME;
								ado.INTERACTIVE_GRADE_ID = itemUsername.INTERACTIVE_GRADE_ID;
								ado.DESCRIPTION = itemUsername.DESCRIPTION;
								ado.INSTRUCTION = itemUsername.INSTRUCTION;
								ado.MECHANISM = itemUsername.MECHANISM;
								ado.CONSEQUENCE = itemUsername.CONSEQUENCE;
								lstADO.Add(ado);
							}
						}

					}

				}

				WaitingManager.Hide();


				Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

				string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "EXPORT_ACIN_INTERACTIVE.xls");

				//chọn đường dẫn
				SaveFileDialog saveFileDialog1 = new SaveFileDialog();
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

					//var errorList = this.ListDataImport.Where(o => !String.IsNullOrWhiteSpace(o.ERROR)).ToList();
					ProcessData(lstADO, ref store);
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
		private void ProcessData(List<ActiveIngredientADO> lstADO, ref Inventec.Common.FlexCellExport.Store store)
		{
			try
			{
				if ((lstADO != null && lstADO.Count() > 0))
				{
					Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
					Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
				
					store.SetCommonFunctions();
					objectTag.AddObjectData(store, "ExportData", lstADO);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnOkCon_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				if (dataNew == null || dataNew.Count() == 0)
				{
					this.dataNew = (from r in listCAI select new HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO(r)).ToList();
				}
				if (dataNew != null && dataNew.Count() > 0)
				{
					var check = dataNew.FirstOrDefault(o => o.ID == this.currentADO.ID);
					if (check != null)
					{
						IsActivebtnOkeCon = false;
						check.check2 = true;
							check.CONSEQUENCE = mmCon.Text;
							Valid(check);
							mmCon.Text = "";
							popupContainerControl1.HidePopup();
						if (string.IsNullOrEmpty(mmCon.Text))
							IsActivebtnOkeCon = true;

					}

					dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
					if (ucGridControlCAI != null)
					{
						CAIProcessor.Reload(ucGridControlCAI, dataNew);
					}

					else
					{
						FillDataToGrid1(this);
					}
				}

				WaitingManager.Hide();
				checkRa = true;
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void Valid(ConflictActiveIngredientADO conflictActiveIngredientADO)
		{
			try
			{
				if (conflictActiveIngredientADO != null)
				{
					if (!String.IsNullOrEmpty(conflictActiveIngredientADO.CONSEQUENCE) && Encoding.UTF8.GetByteCount((conflictActiveIngredientADO.CONSEQUENCE)) > 1000)
					{

						conflictActiveIngredientADO.ErrorMessageConsequence = "Vượt quá độ dài cho phép 1000 ký tự";
						conflictActiveIngredientADO.ErrorTypeConsequence = ErrorType.Warning;
					}
					else
					{
						conflictActiveIngredientADO.ErrorMessageConsequence = "";
						conflictActiveIngredientADO.ErrorTypeConsequence = ErrorType.None;
					}
					if (!String.IsNullOrEmpty(conflictActiveIngredientADO.MECHANISM) && Encoding.UTF8.GetByteCount(conflictActiveIngredientADO.MECHANISM) > 1000)
					{

						conflictActiveIngredientADO.ErrorMessageMechanism = "Vượt quá độ dài cho phép 1000 ký tự";
						conflictActiveIngredientADO.ErrorTypeMechanism = ErrorType.Warning;
					}
					else
					{
						conflictActiveIngredientADO.ErrorMessageMechanism = "";
						conflictActiveIngredientADO.ErrorTypeMechanism = ErrorType.None;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnOkMec_Click(object sender, EventArgs e)
		{
			try
			{
				WaitingManager.Show();
				if (dataNew == null || dataNew.Count() == 0)
				{
					this.dataNew = (from r in listCAI select new HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO(r)).ToList();
				}
				if (dataNew != null && dataNew.Count() > 0)
				{
					var check = dataNew.FirstOrDefault(o => o.ID == this.currentADO.ID);
					if (check != null)
					{
						IsActivebtnOkeMec = false;
							check.check2 = true;
							check.MECHANISM = mmMec.Text;
							Valid(check);
							mmMec.Text = "";
							popupContainerControl2.HidePopup();
						if (string.IsNullOrEmpty(mmMec.Text))
							IsActivebtnOkeMec = true;
					}

					dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
					if (ucGridControlCAI != null)
					{
						CAIProcessor.Reload(ucGridControlCAI, dataNew);
					}

					else
					{
						FillDataToGrid1(this);
					}
				}

				WaitingManager.Hide();
				checkRa = true;
			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnCancelCon_Click(object sender, EventArgs e)
		{
			try
			{
				mmCon.Text = "";
				popupContainerControl1.HidePopup();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			try
			{
				mmMec.Text = "";
				popupContainerControl2.HidePopup();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private bool CheckValidationGrid(List<ConflictActiveIngredientADO> lst)
		{
			bool valid = false;
			try
			{
				var checkGridMicrobi = lst.Where(o => o.ErrorTypeMechanism == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning || o.ErrorTypeConsequence == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning).ToList();
				if (checkGridMicrobi != null && checkGridMicrobi.Count > 0)
					return valid;
			
				valid = true;
			}
			catch (Exception ex)
			{
				valid = false;
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return valid;
		}
	}

}
