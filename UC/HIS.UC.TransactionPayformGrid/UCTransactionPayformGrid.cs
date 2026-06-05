/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.UC.TransactionPayformGrid.ADO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace HIS.UC.TransactionPayformGrid
{
    public partial class UCTransactionPayformGrid : UserControl
    {
        #region Declare
        TransactionPayformGridInitADO initADO;

        // BindingList de NewItemRow (them dong truc tiep) + xoa dong hoat dong
        BindingList<PayformRowADO> listRow;

        // Bind qua BindingSource (giong grid Chiet khau / Quy ho tro) de vong doi NewItemRow
        // (AddNew/EndNew) duoc quan ly dung -> them/xoa dong (ke ca dong cuoi) hoat dong on dinh.
        System.Windows.Forms.BindingSource bindingSourcePayform;

        // Danh muc UC TU LAY (form cha khong truyen) - PayForm/Bank tu cache, Currency/BankFee tu API
        List<PayFormItemADO> listPayForm;
        List<BankItemADO> listBank;
        List<CurrencyItemADO> listCurrency;
        List<BankFeeConfigADO> listBankFeeConfig;

        Dictionary<long, PayFormItemADO> payFormDict;
        Dictionary<long, BankItemADO> bankDict;
        Dictionary<string, CurrencyItemADO> currencyDict;

        decimal requiredAmount;
        bool isCalculating = false;
        #endregion

        #region Constructor
        public UCTransactionPayformGrid(TransactionPayformGridInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado ?? new TransactionPayformGridInitADO();
                this.requiredAmount = this.initADO.RequiredAmount;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Load
        private void UCTransactionPayformGrid_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCatalogData();        // UC tu lay danh muc (form cha khong truyen list)
                ApplySizing();            // Ap kich thuoc form cha truyen (Width/Height/SizeText/MinSize)
                BuildLookupDictionary();
                InitComboData();
                SetCaptionByLanguageKey();
                InitGridData();
                RecalcAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// UC TU LAY danh muc: PayForm/Bank tu BackendDataWorker (cache); Currency/BankFee tu API.
        /// Form cha khong can truyen list nao - chi truyen sizing / RequiredAmount / callback.
        /// </summary>
        private void LoadCatalogData()
        {
            try
            {
                this.listPayForm = LoadPayFormItems();
                this.listBank = LoadBankItems();
                this.listCurrency = LoadCurrencyItems();
                this.listBankFeeConfig = LoadBankFeeConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Hinh thuc thanh toan tu cache (HIS_PAY_FORM). Logic IsShowBank theo quet the / the.</summary>
        private List<PayFormItemADO> LoadPayFormItems()
        {
            var result = new List<PayFormItemADO>();
            try
            {
                var raws = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                    .Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.PAY_FORM_CODE).ToList();

                foreach (var item in raws)
                {
                    bool showBank = item.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE
                                 || item.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE;
                    result.Add(new PayFormItemADO
                    {
                        PAY_FORM_ID = item.ID,
                        PAY_FORM_CODE = item.PAY_FORM_CODE,
                        PAY_FORM_NAME = item.PAY_FORM_NAME,
                        IsRequiredBank = item.IS_REQUIRED_BANK == 1,
                        IsShowBank = showBank || item.IS_REQUIRED_BANK == 1,
                        IsForeignCurrency = false
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Ngan hang tu cache (HIS_BANK).</summary>
        private List<BankItemADO> LoadBankItems()
        {
            var result = new List<BankItemADO>();
            try
            {
                var banks = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                    .Get<MOS.EFMODEL.DataModels.HIS_BANK>();
                if (banks != null)
                {
                    foreach (var b in banks)
                    {
                        result.Add(new BankItemADO
                        {
                            BANK_ID = b.ID,
                            BANK_CODE = b.BANK_CODE,
                            BANK_NAME = b.BANK_NAME
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Loai tien / ti gia tu API (HIS_CURRENCY) - khong co trong cache nen phai goi API.</summary>
        private List<CurrencyItemADO> LoadCurrencyItems()
        {
            var result = new List<CurrencyItemADO>();
            try
            {
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                var filter = new MOS.Filter.HisCurrencyFilter();
                var data = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_CURRENCY>>("api/HisCurrency/Get",
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (data != null)
                {
                    foreach (var o in data.Where(x => x.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                          .OrderBy(x => x.CURRENCY_CODE))
                    {
                        result.Add(new CurrencyItemADO
                        {
                            CURRENCY_ID = o.ID,
                            CURRENCY_CODE = o.CURRENCY_CODE,
                            CURRENCY_NAME = o.CURRENCY_NAME,
                            EXCHANGE_RATE = Convert.ToDecimal(o.EXCHANGE_RATE)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Cau hinh phu phi ngan hang tu API (HIS_PAY_FORM_BANK_FEE).</summary>
        private List<BankFeeConfigADO> LoadBankFeeConfig()
        {
            var result = new List<BankFeeConfigADO>();
            try
            {
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                var filter = new MOS.Filter.HisPayFormBankFeeFilter();
                var data = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PAY_FORM_BANK_FEE>>("api/HisPayFormBankFee/Get",
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (data != null)
                {
                    foreach (var o in data.Where(x => x.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                    {
                        result.Add(new BankFeeConfigADO
                        {
                            PAY_FORM_ID = o.PAY_FORM_ID,
                            BANK_ID = o.BANK_ID,
                            FEE_RATIO = Convert.ToDecimal(o.FEE_RATE),
                            FEE_NAME = o.FEE_NAME
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Ap kich thuoc form cha truyen qua InitADO (giong HIS.UC.Icd). Gia tri &lt;= 0 thi bo qua.</summary>
        private void ApplySizing()
        {
            try
            {
                if (this.initADO.Height > 0 && this.initADO.Width > 0)
                {
                    this.Size = new System.Drawing.Size(this.initADO.Width, this.initADO.Height);
                }
                if (this.initADO.MinSize > 0)
                {
                    this.MinimumSize = new System.Drawing.Size(this.initADO.MinSize, this.MinimumSize.Height);
                }
                if (this.initADO.SizeText > 0)
                {
                    System.Drawing.Font baseFont = gridViewPayform.Appearance.Row.Font
                        ?? this.Font ?? System.Windows.Forms.Control.DefaultFont;
                    System.Drawing.Font newFont = new System.Drawing.Font(baseFont.FontFamily, this.initADO.SizeText);
                    gridViewPayform.Appearance.Row.Font = newFont;
                    gridViewPayform.Appearance.HeaderPanel.Font = newFont;
                    gridViewPayform.Appearance.FooterPanel.Font = newFont;
                    gridViewPayform.Appearance.Row.Options.UseFont = true;
                    gridViewPayform.Appearance.HeaderPanel.Options.UseFont = true;
                    gridViewPayform.Appearance.FooterPanel.Options.UseFont = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BuildLookupDictionary()
        {
            try
            {
                payFormDict = (this.listPayForm ?? new List<PayFormItemADO>())
                    .GroupBy(o => o.PAY_FORM_ID).ToDictionary(g => g.Key, g => g.First());
                bankDict = (this.listBank ?? new List<BankItemADO>())
                    .GroupBy(o => o.BANK_ID).ToDictionary(g => g.Key, g => g.First());
                currencyDict = (this.listCurrency ?? new List<CurrencyItemADO>())
                    .Where(o => !string.IsNullOrEmpty(o.CURRENCY_CODE))
                    .GroupBy(o => o.CURRENCY_CODE).ToDictionary(g => g.Key, g => g.First());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboData()
        {
            try
            {
                repoLookUpPayForm.DataSource = this.listPayForm;
                repoLookUpPayForm.ValueMember = "PAY_FORM_ID";
                repoLookUpPayForm.DisplayMember = "PAY_FORM_NAME";
                repoLookUpPayForm.Columns.Clear();
                repoLookUpPayForm.Columns.Add(new LookUpColumnInfo("PAY_FORM_NAME", "Hình thức"));
                repoLookUpPayForm.ShowHeader = false;
                repoLookUpPayForm.ImmediatePopup = true;
                repoLookUpPayForm.PopupWidth = 200;

                repoLookUpBank.DataSource = this.listBank;
                repoLookUpBank.ValueMember = "BANK_ID";
                repoLookUpBank.DisplayMember = "BANK_NAME";
                repoLookUpBank.Columns.Clear();
                repoLookUpBank.Columns.Add(new LookUpColumnInfo("BANK_CODE", "Mã", 60));
                repoLookUpBank.Columns.Add(new LookUpColumnInfo("BANK_NAME", "Tên", 180));
                repoLookUpBank.ShowHeader = false;
                repoLookUpBank.ImmediatePopup = true;
                repoLookUpBank.PopupWidth = 250;

                repoLookUpCurrency.DataSource = this.listCurrency;
                repoLookUpCurrency.ValueMember = "CURRENCY_CODE";
                repoLookUpCurrency.DisplayMember = "CURRENCY_CODE";
                repoLookUpCurrency.Columns.Clear();
                repoLookUpCurrency.Columns.Add(new LookUpColumnInfo("CURRENCY_CODE", "Mã", 60));
                repoLookUpCurrency.Columns.Add(new LookUpColumnInfo("CURRENCY_NAME", "Tên", 150));
                repoLookUpCurrency.ShowHeader = false;
                repoLookUpCurrency.ImmediatePopup = true;
                repoLookUpCurrency.PopupWidth = 220;

                ApplyDeleteButtonImage();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Dung icon X do (form cha truyen) cho dong bo voi grid Chiet khau/Quy ho tro</summary>
        private void ApplyDeleteButtonImage()
        {
            try
            {
                if (this.initADO.DeleteButtonImage == null) return;
                repoBtnDelete.Buttons.Clear();
                repoBtnDelete.Buttons.AddRange(new EditorButton[] {
                    new EditorButton(ButtonPredefines.Glyph, "", -1, true, true, false,
                        DevExpress.XtraEditors.ImageLocation.MiddleCenter, this.initADO.DeleteButtonImage,
                        new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None),
                        null, null, null, null, "Xóa", null, null, true)
                });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitGridData()
        {
            try
            {
                this.listRow = new BindingList<PayformRowADO>();
                this.listRow.AllowNew = true;
                this.listRow.AllowRemove = true;

                if (this.initADO.InitRows != null)
                {
                    foreach (var row in this.initADO.InitRows)
                    {
                        FillDisplayNames(row);
                        this.listRow.Add(row);
                    }
                }

                if (this.bindingSourcePayform == null)
                    this.bindingSourcePayform = new System.Windows.Forms.BindingSource();
                this.bindingSourcePayform.DataSource = this.listRow;

                gridViewPayform.BeginUpdate();
                try
                {
                    gridControlPayform.DataSource = this.bindingSourcePayform;
                }
                finally
                {
                    gridViewPayform.EndUpdate();
                }
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
                Resources.ResourceLanguageManager.LanguageResource =
                    new ResourceManager("HIS.UC.TransactionPayformGrid.Resources.Lang",
                        typeof(UCTransactionPayformGrid).Assembly);

                colPayForm.Caption = GetLang("UCTransactionPayformGrid.colPayForm.Caption", colPayForm.Caption);
                colBank.Caption = GetLang("UCTransactionPayformGrid.colBank.Caption", colBank.Caption);
                colBankFee.Caption = GetLang("UCTransactionPayformGrid.colBankFee.Caption", colBankFee.Caption);
                colAmount.Caption = GetLang("UCTransactionPayformGrid.colAmount.Caption", colAmount.Caption);
                colCurrency.Caption = GetLang("UCTransactionPayformGrid.colCurrency.Caption", colCurrency.Caption);
                colExchangeRate.Caption = GetLang("UCTransactionPayformGrid.colExchangeRate.Caption", colExchangeRate.Caption);
                colTotalAmount.Caption = GetLang("UCTransactionPayformGrid.colTotalAmount.Caption", colTotalAmount.Caption);
                colPayForm.SummaryItem.DisplayFormat = GetLang("UCTransactionPayformGrid.lblTotalCaption.Text", "Tổng thành tiền:");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLang(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key,
                    Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }
        #endregion

        #region Grid in-place edit
        private void gridViewPayform_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            try
            {
                // Dong moi: chua chon loai tien -> ti gia de rong (coi nhu VND)
                gridViewPayform.SetRowCellValue(e.RowHandle, colExchangeRate, (decimal)0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPayform_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                var row = gridViewPayform.GetRow(e.RowHandle) as PayformRowADO;
                if (row == null) return;

                PayFormItemADO payForm = GetPayForm(row.PAY_FORM_ID);
                bool showBank = payForm != null && payForm.IsShowBank;

                // Chi gate cot Ngan hang theo hinh thuc (giong combo goc).
                // Phu phi / Loai tien / Ti gia luon cho phep nhap & sua.
                if (e.Column == colBank)
                {
                    e.RepositoryItem = showBank ? (DevExpress.XtraEditors.Repository.RepositoryItem)repoLookUpBank : repoTextDash;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPayform_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex < 0) return;
                if (this.listRow == null || e.ListSourceRowIndex >= this.listRow.Count) return;
                var row = this.listRow[e.ListSourceRowIndex];
                if (row == null) return;

                PayFormItemADO payForm = GetPayForm(row.PAY_FORM_ID);
                bool showBank = payForm != null && payForm.IsShowBank;

                if (e.Column == colBank && !showBank)
                {
                    e.DisplayText = "—";
                }
                else if (e.Column == colBankFee)
                {
                    // Hien "8.100 (2.7%)" khi co ti le phu phi
                    if (row.BANK_FEE_RATIO.HasValue && row.BANK_FEE_RATIO.Value > 0)
                    {
                        e.DisplayText = string.Format("{0:#,##0} ({1}%)", row.BANK_FEE_AMOUNT, FormatRatio(row.BANK_FEE_RATIO.Value));
                    }
                }
                else if (e.Column == colExchangeRate && string.IsNullOrEmpty(row.CURRENCY_CODE))
                {
                    // Khong chon loai tien -> coi nhu VND -> Ti gia de rong
                    e.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPayform_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (isCalculating) return;
                // KHONG goi PostEditor()/RefreshData() dong bo o day: editor LookUpEdit con dang active
                // (vua pick xong) -> RefreshData reset editor lam mat gia tri lan chon dau (loi "chon 2 lan").
                var row = gridViewPayform.GetRow(e.RowHandle) as PayformRowADO;
                if (row == null) return;

                if (e.Column == colPayForm)
                {
                    OnPayFormChanged(row);
                }
                else if (e.Column == colBank)
                {
                    OnBankChanged(row);
                }
                else if (e.Column == colCurrency)
                {
                    OnCurrencyChanged(row);
                }
                else if (e.Column == colAmount || e.Column == colExchangeRate)
                {
                    RecomputeBankFee(row);
                }

                RecalcAll();

                // Defer repaint dong ra ngoai edit path -> editor commit gia tri xong moi refresh
                // (cap nhat hien thi cot Ngan hang "—" / Phu phi / Thanh tien ma khong pha editor).
                int rowHandle = e.RowHandle;
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    try { gridViewPayform.RefreshRow(rowHandle); }
                    catch (Exception exRefresh) { Inventec.Common.Logging.LogSystem.Warn(exRefresh); }
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPayform_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repoBtnDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (this.listRow == null) return;

                // 1. Commit editor + commit dong NewItemRow (dong vua them) thanh dong that trong listRow.
                //    Dem so dong truoc/sau de biet dong vua them co thuc su duoc commit hay khong.
                gridViewPayform.PostEditor();
                int countBefore = this.listRow.Count;
                gridViewPayform.UpdateCurrentRow();
                int countAfter = this.listRow.Count;

                int rowHandle = gridViewPayform.FocusedRowHandle;
                PayformRowADO row = gridViewPayform.GetRow(rowHandle) as PayformRowADO;

                // Dong vua them van la NewItemRow (handle < 0) nhung da duoc commit o tren -> la phan tu cuoi.
                // Neu countAfter == countBefore nghia la bam X tren dong "them moi" rong -> khong xoa gi.
                if (row == null && countAfter > countBefore)
                {
                    row = this.listRow[this.listRow.Count - 1];
                }

                // 2. Xoa theo OBJECT (khong theo rowHandle) -> on dinh ca voi dong cuoi vua them.
                if (row != null && this.listRow.Contains(row))
                {
                    if (this.bindingSourcePayform != null)
                        this.bindingSourcePayform.Remove(row);
                    else
                        this.listRow.Remove(row);
                    RecalcAll();
                }
                else if (rowHandle >= 0)
                {
                    gridViewPayform.DeleteRow(rowHandle);
                    RecalcAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khi editor cot LookUp duoc hien -> tu mo dropdown ngay (giong grid Quy ho tro).
        /// Nho do click 1 lan la thay danh sach + chon duoc, khong phai click lan 2 vao mui ten.
        /// </summary>
        private void gridViewPayform_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Base.ColumnView view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
                if (view == null || view.FocusedColumn == null) return;

                if (view.FocusedColumn == colPayForm
                    || view.FocusedColumn == colBank
                    || view.FocusedColumn == colCurrency)
                {
                    DevExpress.XtraEditors.LookUpEdit lookUp = view.ActiveEditor as DevExpress.XtraEditors.LookUpEdit;
                    if (lookUp != null) lookUp.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Cac handler EditValueChanged: chi PostEditor de commit gia tri vua chon ngay lap tuc
        // (cot lien quan: Ten/Ngan hang/Ti gia cap nhat theo). TUYET DOI KHONG goi LayoutChanged
        // o day vi se pha editor dang active lam rot gia tri lan chon dau (loi "chon 2 lan").
        private void repoLookUpPayForm_EditValueChanged(object sender, EventArgs e)
        {
            try { gridViewPayform.PostEditor(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void repoLookUpBank_EditValueChanged(object sender, EventArgs e)
        {
            try { gridViewPayform.PostEditor(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void repoLookUpCurrency_EditValueChanged(object sender, EventArgs e)
        {
            try { gridViewPayform.PostEditor(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion

        #region Row change logic
        private void OnPayFormChanged(PayformRowADO row)
        {
            try
            {
                PayFormItemADO payForm = GetPayForm(row.PAY_FORM_ID);
                row.PAY_FORM_NAME = payForm != null ? payForm.PAY_FORM_NAME : "";

                bool showBank = payForm != null && payForm.IsShowBank;

                // Hinh thuc khong dung ngan hang -> xoa ngan hang + phu phi
                if (!showBank)
                {
                    row.BANK_ID = null;
                    row.BANK_NAME = "";
                    row.BANK_FEE_AMOUNT = 0;
                    row.BANK_FEE_RATIO = null;
                }

                RecomputeBankFee(row);
                // Khong goi LayoutChanged() o day: RefreshData() cuoi CellValueChanged da repaint
                // + re-trigger CustomRowCellEdit cho cot Ngan hang. LayoutChanged se pha editor dang active.
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OnBankChanged(PayformRowADO row)
        {
            try
            {
                if (row.BANK_ID.HasValue)
                {
                    BankItemADO bank;
                    if (bankDict != null && bankDict.TryGetValue(row.BANK_ID.Value, out bank))
                    {
                        row.BANK_NAME = bank.BANK_NAME;
                    }
                }
                else
                {
                    row.BANK_NAME = "";
                }
                RecomputeBankFee(row);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OnCurrencyChanged(PayformRowADO row)
        {
            try
            {
                if (!string.IsNullOrEmpty(row.CURRENCY_CODE))
                {
                    CurrencyItemADO currency;
                    if (currencyDict != null && currencyDict.TryGetValue(row.CURRENCY_CODE, out currency))
                    {
                        // Tu dien ti gia mac dinh tu danh muc ngoai te, nguoi dung van sua duoc
                        row.CURRENCY_ID = currency.CURRENCY_ID;
                        row.EXCHANGE_RATE = currency.EXCHANGE_RATE > 0 ? currency.EXCHANGE_RATE : 1;
                    }
                }
                else
                {
                    // Khong chon loai tien -> coi nhu VND -> ti gia rong
                    row.CURRENCY_ID = null;
                    row.EXCHANGE_RATE = 0;
                }
                // Ti gia doi -> phu phi (phu thuoc ti gia) tinh lai
                RecomputeBankFee(row);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tu dien phu phi tu cau hinh HIS_PAY_FORM_BANK_FEE theo (Hinh thuc + Ngan hang).
        /// Phu phi = (So tien x Ti gia) x %Phu phi / 100. Nguoi dung sua lai duoc sau.
        /// </summary>
        private void RecomputeBankFee(PayformRowADO row)
        {
            try
            {
                BankFeeConfigADO cfg = GetBankFeeConfig(row.PAY_FORM_ID, row.BANK_ID);
                if (cfg != null && cfg.FEE_RATIO > 0)
                {
                    decimal rate = GetEffectiveRate(row);
                    row.BANK_FEE_RATIO = cfg.FEE_RATIO;
                    row.BANK_FEE_NAME = cfg.FEE_NAME;
                    row.BANK_FEE_AMOUNT = Math.Round(row.AMOUNT * rate * cfg.FEE_RATIO / 100m, 0);
                }
                else
                {
                    row.BANK_FEE_RATIO = null;
                    row.BANK_FEE_NAME = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Ti gia hieu dung: khong chon loai tien -> VND (=1), nguoc lai lay ti gia dong (mac dinh 1)</summary>
        private decimal GetEffectiveRate(PayformRowADO row)
        {
            if (string.IsNullOrEmpty(row.CURRENCY_CODE)) return 1m;
            return row.EXCHANGE_RATE > 0 ? row.EXCHANGE_RATE : 1m;
        }

        /// <summary>
        /// Tim cau hinh phu phi khop (uu tien ngan hang cu the, sau do BANK_ID = null = tat ca ngan hang cua payform).
        /// </summary>
        private BankFeeConfigADO GetBankFeeConfig(long payFormId, long? bankId)
        {
            try
            {
                if (this.listBankFeeConfig == null) return null;
                var match = this.listBankFeeConfig
                    .FirstOrDefault(o => o.PAY_FORM_ID == payFormId && bankId.HasValue && o.BANK_ID == bankId.Value);
                if (match == null)
                {
                    match = this.listBankFeeConfig
                        .FirstOrDefault(o => o.PAY_FORM_ID == payFormId && !o.BANK_ID.HasValue);
                }
                return match;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
        #endregion

        #region Calculate
        private void RecalcAll()
        {
            try
            {
                if (this.listRow == null) return;
                isCalculating = true;
                try
                {
                    decimal total = 0;
                    foreach (var r in this.listRow)
                    {
                        r.TOTAL_AMOUNT_VND = ComputeRowTotal(r);
                        total += r.TOTAL_AMOUNT_VND;
                    }

                    decimal conThieu = this.requiredAmount - total;
                    if (conThieu < 0) conThieu = 0;

                    if (this.initADO.DelegateTotalAmountChanged != null)
                    {
                        this.initADO.DelegateTotalAmountChanged(total, conThieu);
                    }
                }
                finally
                {
                    isCalculating = false;
                }

                gridViewPayform.UpdateTotalSummary();
            }
            catch (Exception ex)
            {
                isCalculating = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Thanh tien (VND) = So tien x Ti gia + Phu phi (khong chon loai tien -> ti gia = 1)</summary>
        private decimal ComputeRowTotal(PayformRowADO row)
        {
            try
            {
                decimal rate = GetEffectiveRate(row);
                return Math.Round(row.AMOUNT * rate, 0) + row.BANK_FEE_AMOUNT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return 0;
        }
        #endregion

        #region Helpers
        private PayFormItemADO GetPayForm(long payFormId)
        {
            PayFormItemADO result = null;
            if (payFormDict != null) payFormDict.TryGetValue(payFormId, out result);
            return result;
        }

        private void FillDisplayNames(PayformRowADO row)
        {
            try
            {
                PayFormItemADO payForm = GetPayForm(row.PAY_FORM_ID);
                if (payForm != null) row.PAY_FORM_NAME = payForm.PAY_FORM_NAME;
                if (row.BANK_ID.HasValue && bankDict != null)
                {
                    BankItemADO bank;
                    if (bankDict.TryGetValue(row.BANK_ID.Value, out bank)) row.BANK_NAME = bank.BANK_NAME;
                }
                // Co loai tien nhung chua co ti gia -> mac dinh 1; khong co loai tien -> de rong (VND)
                if (!string.IsNullOrEmpty(row.CURRENCY_CODE) && row.EXCHANGE_RATE <= 0) row.EXCHANGE_RATE = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string FormatRatio(decimal ratio)
        {
            return ratio.ToString("0.######");
        }
        #endregion

        #region Public API (goi qua Processor)
        public List<PayformRowADO> GetData()
        {
            try
            {
                gridViewPayform.PostEditor();
                gridViewPayform.UpdateCurrentRow();
                return this.listRow != null ? this.listRow.ToList() : new List<PayformRowADO>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return new List<PayformRowADO>();
        }

        public void Reload(List<PayformRowADO> data)
        {
            try
            {
                this.listRow = new BindingList<PayformRowADO>();
                this.listRow.AllowNew = true;
                this.listRow.AllowRemove = true;
                if (data != null)
                {
                    foreach (var row in data)
                    {
                        FillDisplayNames(row);
                        this.listRow.Add(row);
                    }
                }

                if (this.bindingSourcePayform == null)
                    this.bindingSourcePayform = new System.Windows.Forms.BindingSource();
                this.bindingSourcePayform.DataSource = this.listRow;

                gridViewPayform.BeginUpdate();
                try
                {
                    gridControlPayform.DataSource = this.bindingSourcePayform;
                }
                finally
                {
                    gridViewPayform.EndUpdate();
                }
                RecalcAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetRequiredAmount(decimal amount)
        {
            try
            {
                this.requiredAmount = amount;
                if (this.initADO != null) this.initADO.RequiredAmount = amount;
                RecalcAll();
                gridViewPayform.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public decimal GetTotalAmount()
        {
            decimal total = 0;
            try
            {
                if (this.listRow != null)
                {
                    foreach (var r in this.listRow) total += ComputeRowTotal(r);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return total;
        }

        public bool ValidateData()
        {
            try
            {
                gridViewPayform.PostEditor();
                if (this.listRow == null) return false;

                // Chi xet cac dong da chon hinh thuc (bo qua dong NewItemRow trong)
                var validRows = this.listRow.Where(o => o.PAY_FORM_ID > 0).ToList();
                if (validRows.Count == 0) return false;

                foreach (var row in validRows)
                {
                    PayFormItemADO payForm = GetPayForm(row.PAY_FORM_ID);
                    if (payForm != null && payForm.IsRequiredBank && !row.BANK_ID.HasValue) return false;
                    if (row.AMOUNT <= 0) return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
        #endregion
    }
}
