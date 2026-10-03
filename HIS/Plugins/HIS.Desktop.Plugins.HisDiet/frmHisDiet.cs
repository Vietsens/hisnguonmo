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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisDiet
{
    public partial class frmHisDiet : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        /// <summary>So ban ghi tra ve o trang hien tai</summary>
        int rowCount = 0;

        /// <summary>Tong so ban ghi khop dieu kien tim kiem (do backend tra ve)</summary>
        int dataTotal = 0;

        /// <summary>Chi so ban ghi dau tien cua trang hien tai - dung de danh so cot STT</summary>
        int startPage = 0;

        /// <summary>GlobalVariables.ActionAdd / GlobalVariables.ActionEdit</summary>
        int ActionType = -1;

        /// <summary>TabIndex cua control dang giu loi validate dau tien</summary>
        int positionHandle = -1;

        /// <summary>Ban ghi dang duoc chon tren luoi</summary>
        HIS_DIET currentData;

        /// <summary>
        /// Bat khi dang gan DataSource cho luoi. Gan DataSource lam GridView ban su kien
        /// FocusedRowChanged, neu khong chan thi form editor bi do lai du lieu dong dau tien
        /// va ActionType bi keo ve che do Sua ngay sau khi vua Them moi xong.
        /// </summary>
        bool isLoadingData = false;

        /// <summary>
        /// Bat trong luc goi ucPaging.Init. UcPaging.Init gan txtPageSize.EditValue TRUOC khi
        /// tao PagingGrid moi, lam ComboBoxEdit ban SelectedIndexChanged trong khi PagingGrid cu
        /// van con song -> thu vien goi lai LoadPaging voi Start cua trang CU. Hau qua: sau khi
        /// bam Tim kiem tu trang >= 2, luoi hien nham trang cua ket qua moi va API bi goi 2 lan.
        /// </summary>
        bool isInitPaging = false;

        /// <summary>
        /// So ban ghi keo ve mot lan khi dang tim kiem. Danh muc che do an chi vai chuc dong
        /// nen keo het ve roi loc tai cho la re va chac an hon phan trang.
        /// </summary>
        const int SEARCH_FETCH_LIMIT = 1000;

        Inventec.Desktop.Common.Modules.Module moduleData;

        #endregion

        #region Construct

        public frmHisDiet(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this.moduleData = moduleData;
                gridControlDiet.ToolTipController = toolTipControllerGrid;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Public method

        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Private method

        private void frmHisDiet_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                HIS.Desktop.Plugins.HisDiet.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisDiet.Resources.Lang", typeof(frmHisDiet).Assembly);
                ResourceManager res = HIS.Desktop.Plugins.HisDiet.Resources.ResourceLanguageManager.LanguageResource;

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisDiet.btnSearch.Text", res, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisDiet.btnAdd.Text", res, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisDiet.btnEdit.Text", res, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisDiet.btnCancel.Text", res, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisDiet.txtKeyword.Properties.NullValuePrompt", res, LanguageManager.GetCulture());

                this.lciDietCode.Text = Inventec.Common.Resource.Get.Value("frmHisDiet.lciDietCode.Text", res, LanguageManager.GetCulture());
                this.lciNutritionInfo.Text = Inventec.Common.Resource.Get.Value("frmHisDiet.lciNutritionInfo.Text", res, LanguageManager.GetCulture());
                this.lciProcessingForm.Text = Inventec.Common.Resource.Get.Value("frmHisDiet.lciProcessingForm.Text", res, LanguageManager.GetCulture());
                this.lciTreatmentApply.Text = Inventec.Common.Resource.Get.Value("frmHisDiet.lciTreatmentApply.Text", res, LanguageManager.GetCulture());

                this.grdColSTT.Caption = Inventec.Common.Resource.Get.Value("frmHisDiet.grdColSTT.Caption", res, LanguageManager.GetCulture());
                this.grdColDietCode.Caption = Inventec.Common.Resource.Get.Value("frmHisDiet.grdColDietCode.Caption", res, LanguageManager.GetCulture());
                this.grdColNutritionInfo.Caption = Inventec.Common.Resource.Get.Value("frmHisDiet.grdColNutritionInfo.Caption", res, LanguageManager.GetCulture());
                this.grdColProcessingForm.Caption = Inventec.Common.Resource.Get.Value("frmHisDiet.grdColProcessingForm.Caption", res, LanguageManager.GetCulture());
                this.grdColTreatmentApply.Caption = Inventec.Common.Resource.Get.Value("frmHisDiet.grdColTreatmentApply.Caption", res, LanguageManager.GetCulture());
                this.grdColIsActive.Caption = Inventec.Common.Resource.Get.Value("frmHisDiet.grdColIsActive.Caption", res, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisDiet.grdColCreateTime.Caption", res, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisDiet.grdColCreator.Caption", res, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisDiet.grdColModifyTime.Caption", res, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisDiet.grdColModifier.Caption", res, LanguageManager.GetCulture());

                this.bbtnSearch.Caption = this.btnSearch.Text;
                this.bbtnAdd.Caption = this.btnAdd.Text;
                this.bbtnEdit.Caption = this.btnEdit.Text;
                this.bbtnReset.Caption = this.btnCancel.Text;

                this.Text = Inventec.Common.Resource.Get.Value("frmHisDiet.Text", res, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.currentData = null;
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);
                // Che do Them moi -> cho phep nhap Ma
                txtDietCode.Properties.ReadOnly = false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control editor dau tien
        /// </summary>
        private void SetFocusEditor()
        {
            try
            {
                if (txtDietCode.Properties.ReadOnly)
                {
                    memoNutritionInfo.Focus();
                    memoNutritionInfo.SelectAll();
                }
                else
                {
                    txtDietCode.Focus();
                    txtDietCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Load data

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null && ucPaging.pagingGrid.PageSize > 0)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                if (numPageSize <= 0) numPageSize = 20;

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;

                //Chan UcPaging.Init tai nhap LoadPaging voi Start cua trang cu (xem chu thich cua isInitPaging).
                //Gan pagingGrid = null de guard "pagingGrid != null" ben trong thu vien tu ngat,
                //kem co isInitPaging de van an toan neu thu vien doi thu tu khoi tao.
                //Bat buoc dat SAU khi da doc numPageSize o tren, neu khong se mat so ban ghi/trang nguoi dung chon.
                isInitPaging = true;
                try
                {
                    ucPaging.pagingGrid = null;
                    ucPaging.Init(LoadPaging, param, numPageSize);
                }
                finally
                {
                    isInitPaging = false;
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang. Duoc UcPaging goi lai moi khi doi trang/doi so ban ghi mot trang.
        /// </summary>
        /// <param name="param">CommonParam chua Start/Limit cua trang can lay</param>
        private void LoadPaging(object param)
        {
            try
            {
                //Bo qua lan goi lai do ucPaging.Init sinh ra (Start con la cua trang cu) 
                if (isInitPaging) return;

                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;

                string keyword = txtKeyword.Text.Trim();
                bool searching = keyword.Length > 0;

                // Dang tim kiem thi keo ca danh muc ve mot lan roi loc tai cho, khong phan trang.
                // Ly do: van gui KEY_WORD len server nhung khong chac server co loc hay khong;
                // neu no bo qua ma minh van phan trang thi chi loc duoc dung trang dau, cac ban ghi 
                // khop nam o trang sau se mat. Danh muc che do an chi vai chuc dong nen keo het
                // ve la re. Danh muc phinh to sau nay thi phai xem lai cho nay.
                CommonParam paramCommon = searching
                    ? new CommonParam(0, SEARCH_FETCH_LIMIT)
                    : new CommonParam(startPage, limit);

                HisDietFilter filter = new HisDietFilter();
                SetFilter(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                List<HIS_DIET> data = null;

                isLoadingData = true;
                gridViewDiet.BeginUpdate();
                try
                {
                    Inventec.Core.ApiResultObject<List<HIS_DIET>> apiResult =
                        new BackendAdapter(paramCommon).GetRO<List<HIS_DIET>>(HisRequestUriStore.MOSHIS_DIET_GET, ApiConsumers.MosConsumer, filter, paramCommon);

                    if (apiResult != null)
                    {
                        data = (List<HIS_DIET>)apiResult.Data;
                    }

                    int fromServer = (data == null ? 0 : data.Count);

                    if (searching)
                    {
                        data = FilterByKeyword(data, keyword);

                        if (data.Count != fromServer)
                        {
                            LogSystem.Warn(string.Format(
                                "Server bo qua KEY_WORD=[{0}]: tra ve {1} dong, loc tai cho con {2}",
                                keyword, fromServer, data.Count));
                        }
                    }

                    gridControlDiet.DataSource = data;
                    rowCount = (data == null ? 0 : data.Count);

                    // Dang tim kiem thi khong con phan trang, tong chinh la so dong dang hien
                    dataTotal = searching
                        ? rowCount
                        : (apiResult == null || apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                    LogSystem.Info(string.Format(
                        "TimKiemDiet | KEY_WORD=[{0}] | Start={1} Limit={2} | server tra {3} dong | hien thi {4} dong, tong {5}",
                        keyword, startPage, limit, fromServer, rowCount, dataTotal));
                }
                finally
                {
                    gridViewDiet.EndUpdate();
                    isLoadingData = false;
                }

                SyncEditorWithGrid(data);

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Sau moi lan nap lai luoi, keo con tro luoi va form editor ve cung mot ban ghi.
        /// Neu bo qua buoc nay: luoi tu dua con tro ve dong 0 trong khi editor van giu ban ghi cu,
        /// nguoi dung bam vao dong 0 thi FocusedRowChanged khong ban (con tro von da o do)
        /// -> bam "Sua" se ghi de nham ban ghi cu.
        /// Buoc nay dong thoi lam moi trang thai nut Sua sau khi Khoa/Bo khoa.
        /// </summary>
        private void SyncEditorWithGrid(List<HIS_DIET> data)
        {
            try
            {
                if (this.currentData == null) return;

                HIS_DIET stillExist = (data == null ? null : data.FirstOrDefault(o => o.ID == this.currentData.ID));

                if (stillExist != null)
                {
                    //Ban ghi dang mo tren form van con trong ket qua -> keo con tro luoi ve dung dong do
                    gridViewDiet.FocusedRowHandle = gridViewDiet.GetRowHandle(data.IndexOf(stillExist));
                    ChangedDataRow(stillExist);
                }
                else
                {
                    //Khong con trong ket qua (bi xoa, hoac lot khoi dieu kien tim kiem) -> ve che do Them moi
                    this.currentData = null;
                    this.ActionType = GlobalVariables.ActionAdd;
                    EnableControlChanged(this.ActionType);
                    ResetFormData();
                    txtDietCode.Properties.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Loc danh sach theo tu khoa ngay tai may tram, doi chieu ma che do an va ap dung dieu tri
        /// (vd go "DN" hoac "roi loan nuot").
        ///
        /// Van gui KEY_WORD len server o SetFilter, day chi la luoi do: khong xac dinh duoc
        /// backend co loc theo KEY_WORD cho danh muc nay hay khong, ma trieu chung nguoi dung
        /// bao la "go tu khoa xong danh sach khong doi" - dung kieu server tra ve nguyen ca danh muc.
        /// Server co loc thi ham nay chay tren tap da loc san, khong anh huong gi.
        /// </summary>
        private List<HIS_DIET> FilterByKeyword(List<HIS_DIET> source, string keyword)
        {
            List<HIS_DIET> result = new List<HIS_DIET>();
            if (source == null) return result;

            string needle = NormalizeForSearch(keyword);
            if (needle.Length == 0) return source;

            foreach (HIS_DIET item in source)
            {
                if (item == null) continue;

                if (NormalizeForSearch(item.DIET_CODE).Contains(needle)
                    || NormalizeForSearch(item.TREATMENT_APPLY).Contains(needle))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Bo dau tieng Viet va chuyen ve chu thuong de so sanh.
        /// Nguoi dung quen go khong dau ("roi loan nuot") de tim "Rối loạn nuốt", khong bo dau thi tim khong ra.
        /// </summary>
        private string NormalizeForSearch(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            string formD = value.Trim().ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder sb = new System.Text.StringBuilder(formD.Length);

            foreach (char c in formD)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                    != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            // "đ" khong tach dau bang FormD nen phai doi tay
            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC).Replace('đ', 'd');
        }

        private void SetFilter(ref HisDietFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lay lai ban ghi moi nhat tu backend truoc khi update, tranh ghi de cac truong khong hien tren form
        /// </summary>
        private void LoadCurrent(long currentId, ref HIS_DIET currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDietFilter filter = new HisDietFilter();
                filter.ID = currentId;
                HIS_DIET data = new BackendAdapter(param).Get<List<HIS_DIET>>(HisRequestUriStore.MOSHIS_DIET_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (data != null)
                {
                    currentDTO = data;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Editor <-> data

        private void ChangedDataRow(HIS_DIET data)
        {
            try
            {
                if (data == null) return;

                this.currentData = data;
                FillDataToEditorControl(data);
                this.ActionType = GlobalVariables.ActionEdit;
                EnableControlChanged(this.ActionType);

                // Che do Sua -> khong cho doi Ma
                txtDietCode.Properties.ReadOnly = true;

                //Disable nut Sua neu du lieu da bi khoa
                btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(HIS_DIET data)
        {
            try
            {
                if (data == null) return;

                txtDietCode.Text = data.DIET_CODE;
                memoNutritionInfo.Text = data.NUTRITION_INFO;
                memoProcessingForm.Text = data.PROCESSING_FORM;
                memoTreatmentApply.Text = data.TREATMENT_APPLY;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_DIET currentDTO)
        {
            try
            {
                currentDTO.DIET_CODE = txtDietCode.Text.Trim();
                //Giu nguyen xuong dong ben trong, chi bo khoang trang dau/cuoi.
                //Xuong dong duoc gop thanh 1 dong luc chen vao to dieu tri, khong phai o day.
                currentDTO.NUTRITION_INFO = String.IsNullOrWhiteSpace(memoNutritionInfo.Text) ? null : memoNutritionInfo.Text.Trim();
                currentDTO.PROCESSING_FORM = String.IsNullOrWhiteSpace(memoProcessingForm.Text) ? null : memoProcessingForm.Text.Trim();
                currentDTO.TREATMENT_APPLY = String.IsNullOrWhiteSpace(memoTreatmentApply.Text) ? null : memoTreatmentApply.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!layoutControl2.IsInitialized) return;
                layoutControl2.BeginUpdate();
                try
                {
                    txtDietCode.Text = "";
                    memoNutritionInfo.Text = "";
                    memoProcessingForm.Text = "";
                    memoTreatmentApply.Text = "";
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl2.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Grid handler

        private void gridViewDiet_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound) return;

                IList source = (IList)((BaseView)sender).DataSource;
                if (source == null || e.ListSourceRowIndex < 0 || e.ListSourceRowIndex >= source.Count) return;

                HIS_DIET data = (HIS_DIET)source[e.ListSourceRowIndex];
                if (data == null) return;

                if (e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                }
                else if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                }
                else if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                }
                else if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    e.Value = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        ? HIS.Desktop.Plugins.HisDiet.Resources.ResourceMessage.TrangThaiHoatDong
                        : HIS.Desktop.Plugins.HisDiet.Resources.ResourceMessage.TrangThaiTamKhoa;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Doi editor cua cot Khoa/Xoa theo trang thai ban ghi:
        /// - Dang hoat dong  -> nut "Khóa" + nut "Xóa" (bam duoc)
        /// - Dang bi khoa    -> nut "Bỏ khóa" + nut "Xóa" mo (khong bam duoc)
        /// </summary>
        private void gridViewDiet_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                //Dung GetRow (khong index thang vao DataSource) vi e.RowHandle la chi so theo
                //thu tu HIEN THI - nguoi dung sap xep cot thi no lech voi chi so cua list nguon
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null || e.RowHandle < 0) return;

                HIS_DIET data = view.GetRow(e.RowHandle) as HIS_DIET;
                if (data == null) return;

                bool isActive = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                if (e.Column.FieldName == "IS_LOCK")
                {
                    e.RepositoryItem = (isActive ? repositoryItemLock : repositoryItemUnLock);
                }
                else if (e.Column.FieldName == "IS_DELETE_ROW")
                {
                    e.RepositoryItem = (isActive ? repositoryItemDelete : repositoryItemDisDelete);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewDiet_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                //Xem chu thich o gridViewDiet_CustomRowCellEdit ve ly do dung GetRow
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null || e.RowHandle < 0) return;

                HIS_DIET data = view.GetRow(e.RowHandle) as HIS_DIET;
                if (data == null) return;

                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    e.Appearance.ForeColor = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE) ? Color.Red : Color.Green;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewDiet_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            try
            {
                //Dang nap lai luoi -> khong do du lieu sang form editor
                if (isLoadingData) return;

                HIS_DIET rowData = gridViewDiet.GetFocusedRow() as HIS_DIET;
                if (rowData != null)
                {
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bam mot lan tren luoi luon dong bo ban ghi dang chon sang form editor.
        /// Can thiet vi khi con tro da nam san o dong do (vd vua nap lai luoi) thi
        /// FocusedRowChanged khong ban. Khong goi SetFocusEditor de giu focus tren luoi,
        /// cho phep tiep tuc di chuyen bang phim mui ten.
        /// </summary>
        private void gridViewDiet_Click(object sender, EventArgs e)
        {
            try
            {
                if (isLoadingData) return;

                HIS_DIET rowData = gridViewDiet.GetFocusedRow() as HIS_DIET;
                if (rowData != null && (this.currentData == null || this.currentData.ID != rowData.ID))
                {
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridControlDiet_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                HIS_DIET rowData = gridViewDiet.GetFocusedRow() as HIS_DIET;
                if (rowData != null)
                {
                    ChangedDataRow(rowData);
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewDiet_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    HIS_DIET rowData = gridViewDiet.GetFocusedRow() as HIS_DIET;
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Button handler

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nut "Làm lại": xoa trang du lieu dang nhap, tro ve che do Them moi
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = null;
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                // Lam lai -> ve che do Them moi, cho phep nhap Ma
                txtDietCode.Properties.ReadOnly = false;
                SetFocusEditor();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();

                HIS_DIET updateDTO = new HIS_DIET();

                if (this.ActionType == GlobalVariables.ActionEdit && this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);

                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    HIS_DIET resultData = new BackendAdapter(param).Post<HIS_DIET>(HisRequestUriStore.MOSHIS_DIET_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<HIS_DIET>();

                        //Them moi xong -> tra form ve trang thai san sang them ban ghi tiep theo.
                        //Dat truoc khi nap lai luoi de SyncEditorWithGrid khong keo ban ghi cu len form.
                        this.currentData = null;
                        this.ActionType = GlobalVariables.ActionAdd;
                        EnableControlChanged(this.ActionType);
                        ResetFormData();
                        txtDietCode.Properties.ReadOnly = false;

                        FillDataToGridControl();
                    }
                }
                else
                {
                    HIS_DIET resultData = new BackendAdapter(param).Post<HIS_DIET>(HisRequestUriStore.MOSHIS_DIET_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<HIS_DIET>();

                        //Sua xong -> giu nguyen ban ghi vua sua tren form editor.
                        //SyncEditorWithGrid trong LoadPaging se do lai du lieu moi nhat va
                        //cap nhat trang thai nut Sua theo IS_ACTIVE.
                        this.currentData = resultData;
                        FillDataToGridControl();
                    }
                }

                WaitingManager.Hide();

                if (success)
                {
                    SetFocusEditor();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Grid button: Lock / Unlock / Delete

        /// <summary>
        /// Ban ghi dang hoat dong -> khoa lai (IS_ACTIVE = 0)
        /// </summary>
        private void repositoryItemLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                HIS_DIET data = gridViewDiet.GetFocusedRow() as HIS_DIET;
                if (data == null) return;

                if (MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                HIS_DIET result = new BackendAdapter(param).Post<HIS_DIET>(HisRequestUriStore.MOSHIS_DIET_LOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    BackendDataWorker.Reset<HIS_DIET>();
                    //SyncEditorWithGrid cap nhat lai trang thai nut Sua theo IS_ACTIVE moi
                    FillDataToGridControl();
                }

                MessageManager.Show(this, param, result != null);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ban ghi dang bi khoa -> bo khoa (IS_ACTIVE = 1)
        /// </summary>
        private void repositoryItemUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                HIS_DIET data = gridViewDiet.GetFocusedRow() as HIS_DIET;
                if (data == null) return;

                if (MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                HIS_DIET result = new BackendAdapter(param).Post<HIS_DIET>(HisRequestUriStore.MOSHIS_DIET_UNLOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    BackendDataWorker.Reset<HIS_DIET>();
                    //SyncEditorWithGrid cap nhat lai trang thai nut Sua theo IS_ACTIVE moi
                    FillDataToGridControl();
                }

                MessageManager.Show(this, param, result != null);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void repositoryItemDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                HIS_DIET data = gridViewDiet.GetFocusedRow() as HIS_DIET;
                if (data == null) return;

                if (MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                bool success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_DIET_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();

                if (success)
                {
                    BackendDataWorker.Reset<HIS_DIET>();
                    //SyncEditorWithGrid tu tra form ve che do Them moi neu ban ghi vua xoa dang mo tren form
                    FillDataToGridControl();
                }

                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region Validate

        private void ValidateForm()
        {
            try
            {
                //Chi Ma che do an la bat buoc, 3 truong con lai duoc de trong
                ValidationSingleControl(txtDietCode);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1 || positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Keyboard

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewDiet.Focus();
                    gridViewDiet.FocusedRowHandle = 0;
                    HIS_DIET rowData = gridViewDiet.GetFocusedRow() as HIS_DIET;
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtDietCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //3 o MemoEdit phia sau dung Enter de xuong dong nen khong bat Enter de nhay o,
                //nguoi dung di chuyen giua cac o bang Tab
                if (e.KeyCode == Keys.Enter)
                {
                    memoNutritionInfo.Focus();
                    memoNutritionInfo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Shortcut

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType != GlobalVariables.ActionEdit) return;

                if (!btnEdit.Enabled)
                {
                    //Ban ghi dang bi khoa -> khong cho sua
                    MessageBox.Show(HIS.Desktop.Plugins.HisDiet.Resources.ResourceMessage.BanGhiDangBiKhoaKhongTheSua, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Tooltip

        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
