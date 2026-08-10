using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.Pharmacology
{
    public partial class frmPharmacologyAcin : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        private const string API_HIS_ACTIVE_INGREDIENT_GET = "/api/HisActiveIngredient/Get";
        private const string API_HIS_ACTIVE_INGREDIENT_UPDATE_LIST = "/api/HisActiveIngredient/UpdateList";

        private long pharmacologyId;
        private int rowCount;
        private int dataTotal;
        private int startIndex;
        private List<ActiveIngredientADO> listActiveIngredient;

        /// <summary>
        /// Cac hoat chat da tich chon, giu lai khi chuyen trang hoac tim kiem lai
        /// </summary>
        private readonly List<ActiveIngredientADO> listActiveIngredientChecked = new List<ActiveIngredientADO>();

        #endregion

        #region Constructor

        public frmPharmacologyAcin()
            : this(0)
        {
        }

        public frmPharmacologyAcin(long pharmacologyId)
        {
            InitializeComponent();
            this.pharmacologyId = pharmacologyId;
        }

        #endregion

        #region Process

        private void frmPharmacologyAcin_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pharmacologyId), pharmacologyId));
                LoadActiveIngredientChecked();
                FillDataToGrid();
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lay cac hoat chat da thuoc duoc ly dang mo de tich san tren luoi.
        /// Lay het 1 lan, khong phan trang, vi 1 duoc ly khong co qua nhieu hoat chat
        /// </summary>
        private void LoadActiveIngredientChecked()
        {
            try
            {
                this.listActiveIngredientChecked.Clear();
                if (this.pharmacologyId <= 0)
                {
                    return;
                }

                CommonParam param = new CommonParam();
                HisActiveIngredientFilter filter = new HisActiveIngredientFilter();
                filter.PHARMACOLOGY_ID = this.pharmacologyId;
                filter.ORDER_FIELD = "ACTIVE_INGREDIENT_CODE";
                filter.ORDER_DIRECTION = "ASC";

                var data = new BackendAdapter(param).Get<List<HIS_ACTIVE_INGREDIENT>>(
                    API_HIS_ACTIVE_INGREDIENT_GET,
                    ApiConsumers.MosConsumer,
                    filter,
                    param);

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        ActiveIngredientADO activeIngredientADO = new ActiveIngredientADO(item);
                        activeIngredientADO.check2 = true;
                        this.listActiveIngredientChecked.Add(activeIngredientADO);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("LoadActiveIngredientChecked: pharmacologyId = " + this.pharmacologyId
                    + ", so hoat chat da gan = " + this.listActiveIngredientChecked.Count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Cac hoat chat da tich khop voi tu khoa dang tim, de dua len dau luoi
        /// </summary>
        private List<ActiveIngredientADO> GetActiveIngredientCheckedByKeyWord(string keyWord)
        {
            List<ActiveIngredientADO> result = new List<ActiveIngredientADO>();
            try
            {
                if (this.listActiveIngredientChecked.Count == 0)
                {
                    return result;
                }

                keyWord = (keyWord ?? "").Trim();
                if (String.IsNullOrEmpty(keyWord))
                {
                    return this.listActiveIngredientChecked.ToList();
                }

                result = this.listActiveIngredientChecked.Where(o =>
                    (!String.IsNullOrEmpty(o.ACTIVE_INGREDIENT_CODE) && o.ACTIVE_INGREDIENT_CODE.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (!String.IsNullOrEmpty(o.ACTIVE_INGREDIENT_NAME) && o.ACTIVE_INGREDIENT_NAME.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Nap lai du lieu tu trang dau tien va khoi tao lai bo phan trang
        /// </summary>
        private void FillDataToGrid()
        {
            try
            {
                int numPageSize;
                if (ucPagingActiveIngredient.pagingGrid != null)
                {
                    numPageSize = ucPagingActiveIngredient.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridActiveIngredient(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = this.rowCount;
                param.Count = this.dataTotal;
                ucPagingActiveIngredient.Init(FillDataToGridActiveIngredient, param, numPageSize);
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

                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                this.startIndex = start;

                CommonParam param = new CommonParam(start, limit);
                HisActiveIngredientFilter filter = new HisActiveIngredientFilter();
                filter.KEY_WORD = txtSearch.Text;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                var activeIngredients = new BackendAdapter(param).GetRO<List<HIS_ACTIVE_INGREDIENT>>(
                    API_HIS_ACTIVE_INGREDIENT_GET,
                    ApiConsumers.MosConsumer,
                    filter,
                    param);

                this.listActiveIngredient = new List<ActiveIngredientADO>();

                //Cac hoat chat da tich dua len dau, chi dua o trang dau de khong lap lai o moi trang
                List<ActiveIngredientADO> checkeds = new List<ActiveIngredientADO>();
                if (start == 0)
                {
                    checkeds = GetActiveIngredientCheckedByKeyWord(txtSearch.Text);
                    this.listActiveIngredient.AddRange(checkeds);
                }

                if (activeIngredients != null && activeIngredients.Data != null)
                {
                    foreach (var item in activeIngredients.Data)
                    {
                        //Dong da nam o phan tich san ben tren thi khong lay lai
                        if (checkeds.Any(o => o.ID == item.ID))
                        {
                            continue;
                        }

                        ActiveIngredientADO activeIngredientADO = new ActiveIngredientADO(item);
                        activeIngredientADO.check2 = this.listActiveIngredientChecked.Any(o => o.ID == item.ID);
                        this.listActiveIngredient.Add(activeIngredientADO);
                    }
                }

                gridControlActiveIngredient.BeginUpdate();
                gridControlActiveIngredient.DataSource = this.listActiveIngredient;
                gridControlActiveIngredient.EndUpdate();

                this.rowCount = this.listActiveIngredient.Count;
                this.dataTotal = (activeIngredients != null && activeIngredients.Param != null)
                    ? (activeIngredients.Param.Count ?? 0)
                    : 0;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Danh sach hoat chat dang duoc tich chon tren luoi (bao gom ca cac trang da duyet qua)
        /// </summary>
        public List<HIS_ACTIVE_INGREDIENT> GetActiveIngredientChecked()
        {
            return this.listActiveIngredientChecked.Cast<HIS_ACTIVE_INGREDIENT>().ToList();
        }

        #endregion

        #region Event

        private void btnSearchActiveIngredient_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gan duoc ly dang mo cho cac hoat chat da tich chon  
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (this.pharmacologyId <= 0)
                {
                    MessageBox.Show("Không xác định được dược lý. Hãy chọn dòng dược lý rồi mở lại màn hình này.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (this.listActiveIngredientChecked.Count == 0)
                {
                    MessageBox.Show("Hãy tích chọn hoạt chất cần gán vào dược lý.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Gui lai ca dong, chi thay PHARMACOLOGY_ID de khong lam mat cac truong khac
                List<HIS_ACTIVE_INGREDIENT> listUpdate = new List<HIS_ACTIVE_INGREDIENT>();
                foreach (var activeIngredient in this.listActiveIngredientChecked)
                {
                    HIS_ACTIVE_INGREDIENT data = activeIngredient.ToActiveIngredient();
                    data.PHARMACOLOGY_ID = this.pharmacologyId;
                    listUpdate.Add(data);
                }

                Inventec.Common.Logging.LogSystem.Debug("btnSave_Click: pharmacologyId = " + this.pharmacologyId
                    + ", so hoat chat gan = " + listUpdate.Count);

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<List<HIS_ACTIVE_INGREDIENT>>(
                    API_HIS_ACTIVE_INGREDIENT_UPDATE_LIST,
                    ApiConsumers.MosConsumer,
                    listUpdate,
                    param);
                WaitingManager.Hide();

                if (result != null && result.Count > 0)
                {
                    success = true;
                    //Lay lai danh sach da gan de luoi tich dung theo du lieu vua luu
                    LoadActiveIngredientChecked();
                    FillDataToGrid();
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewActiveIngredient_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column == null || e.Column.FieldName != "check2")
                {
                    return;
                }

                var focusedRow = gridViewActiveIngredient.GetRow(e.RowHandle) as ActiveIngredientADO;
                if (focusedRow == null)
                {
                    return;
                }

                if (focusedRow.check2)
                {
                    if (!this.listActiveIngredientChecked.Any(o => o.ID == focusedRow.ID))
                    {
                        this.listActiveIngredientChecked.Add(focusedRow);
                    }
                }
                else
                {
                    this.listActiveIngredientChecked.RemoveAll(o => o.ID == focusedRow.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewActiveIngredient_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column != null && e.Column.FieldName == "STT")
                {
                    e.Value = this.startIndex + e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.D) || keyData == (Keys.Control | Keys.F))
                {
                    FillDataToGrid();
                    return true;
                }

                if (keyData == (Keys.Control | Keys.N))
                {
                    btnSave_Click(null, null);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion
    }
}
