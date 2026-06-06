/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tich hop UC luoi hinh thuc thanh toan (HIS.UC.TransactionPayformGrid) cho man THANH TOAN 2 SO.
 * Moi so (Hoa don vien phi = Reciept, Hoa don dich vu = Invoice) co 1 luoi rieng.
 * Chi hoat dong khi config MOS.HIS_TRANSACTION.MULTI_PAYFORM = 1 (HisConfig.EnableMultiPayform).
 * Lam tuong tu ban "1 so" (frmTransactionBill__Plus__PayformGrid.cs) + style luoi Chiet khau (__Plus__GridDiscount.cs).
 */
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Config;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UcPayform = HIS.UC.TransactionPayformGrid;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne
{
    public partial class frmTransactionBillTwoInOne
    {
        #region Declare MultiPayform
        /// <summary>BAT (=1): hien thi UC luoi hinh thuc thanh toan cho ca 2 so.</summary>
        bool isMultiPayform;

        // So Hoa don vien phi (Reciept)
        UcPayform.UCTransactionPayformGridProcessor payformProcessorReciept;
        UserControl ucPayformReciept;

        // So Hoa don dich vu (Invoice)
        UcPayform.UCTransactionPayformGridProcessor payformProcessorInvoice;
        UserControl ucPayformInvoice;

        // Danh muc dung chung 2 so (build 1 lan)
        List<UcPayform.ADO.PayFormItemADO> payformItemList;
        List<UcPayform.ADO.BankItemADO> payformBankList;
        List<UcPayform.ADO.CurrencyItemADO> payformCurrencyList;
        List<UcPayform.ADO.BankFeeConfigADO> payformBankFeeList;
        #endregion

        /// <summary>
        /// Khoi tao UC luoi hinh thuc thanh toan cho ca 2 so khi config bat.
        /// Goi trong Load NGAY SAU InitGridDiscountIfEnable().
        /// </summary>
        private void InitMultiPayformGridIfEnable()
        {
            try
            {
                isMultiPayform = HisConfig.EnableMultiPayform;
                if (!isMultiPayform) return; // Config tat -> giu nguyen giao dien cu

                // Danh muc dung chung
                payformItemList = BuildPayformItems();
                payformBankList = BuildBankItems();
                payformCurrencyList = BuildCurrencyItems();
                payformBankFeeList = BuildBankFeeConfig();

                InitRecieptPayformGrid();
                InitInvoicePayformGrid();

                // Trang thai enable ban dau theo "Khong TT" cua tung so
                SetEnableRecieptPayform(!checkNotReciept.Checked);
                SetEnableInvoicePayform(!checkNotInvoice.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Build danh muc (dung chung 2 so)
        /// <summary>Danh sach hinh thuc TT goc (tien mat, chuyen khoan, quet the...).</summary>
        private List<UcPayform.ADO.PayFormItemADO> BuildPayformItems()
        {
            var result = new List<UcPayform.ADO.PayFormItemADO>();
            try
            {
                var rawPayForms = BackendDataWorker.Get<HIS_PAY_FORM>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.PAY_FORM_CODE)
                    .ToList();

                foreach (var item in rawPayForms)
                {
                    bool showBank = item.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE
                                 || item.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE;
                    result.Add(new UcPayform.ADO.PayFormItemADO
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

        private List<UcPayform.ADO.BankItemADO> BuildBankItems()
        {
            var result = new List<UcPayform.ADO.BankItemADO>();
            try
            {
                var banks = BackendDataWorker.Get<HIS_BANK>();
                if (banks != null)
                {
                    foreach (var b in banks)
                    {
                        result.Add(new UcPayform.ADO.BankItemADO
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

        /// <summary>
        /// Danh muc loai tien / ti gia tu HIS_CURRENCY. Neu chua co du lieu -> tra ve rong (UC mac dinh VND).
        /// </summary>
        private List<UcPayform.ADO.CurrencyItemADO> BuildCurrencyItems()
        {
            var result = new List<UcPayform.ADO.CurrencyItemADO>();
            try
            {
                CommonParam param = new CommonParam();
                var filter = new MOS.Filter.HisCurrencyFilter();
                var data = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<HIS_CURRENCY>>("api/HisCurrency/Get",
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (data != null)
                {
                    foreach (var o in data.Where(x => x.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                          .OrderBy(x => x.CURRENCY_CODE))
                    {
                        result.Add(new UcPayform.ADO.CurrencyItemADO
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Cau hinh phu phi ngan hang tu HIS_PAY_FORM_BANK_FEE. BANK_ID = null -> ap dung moi ngan hang cua hinh thuc.
        /// </summary>
        private List<UcPayform.ADO.BankFeeConfigADO> BuildBankFeeConfig()
        {
            var result = new List<UcPayform.ADO.BankFeeConfigADO>();
            try
            {
                CommonParam param = new CommonParam();
                var filter = new MOS.Filter.HisPayFormBankFeeFilter();
                var data = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<HIS_PAY_FORM_BANK_FEE>>("api/HisPayFormBankFee/Get",
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (data != null)
                {
                    foreach (var o in data.Where(x => x.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                    {
                        result.Add(new UcPayform.ADO.BankFeeConfigADO
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        #endregion

        #region Khoi tao + nhung UC tung so
        private void InitRecieptPayformGrid()
        {
            try
            {
                var initADO = new UcPayform.ADO.TransactionPayformGridInitADO();
                initADO.ListPayForm = payformItemList;
                initADO.ListBank = payformBankList;
                initADO.ListCurrency = payformCurrencyList;
                initADO.ListBankFeeConfig = payformBankFeeList;
                initADO.RequiredAmount = GetCurrentRecieptRequiredAmount();
                initADO.IsShowRemainingColumn = true;
                initADO.DelegateTotalAmountChanged = OnRecieptPayformTotalChanged;

                payformProcessorReciept = new UcPayform.UCTransactionPayformGridProcessor(new CommonParam());
                ucPayformReciept = (UserControl)payformProcessorReciept.Run(initADO);
                if (ucPayformReciept == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("[MultiPayform] Run() Reciept tra ve NULL.");
                    return;
                }
                HostRecieptPayformUc();
                HideOldRecieptPayformControls();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitInvoicePayformGrid()
        {
            try
            {
                var initADO = new UcPayform.ADO.TransactionPayformGridInitADO();
                initADO.ListPayForm = payformItemList;
                initADO.ListBank = payformBankList;
                initADO.ListCurrency = payformCurrencyList;
                initADO.ListBankFeeConfig = payformBankFeeList;
                initADO.RequiredAmount = GetCurrentInvoiceRequiredAmount();
                initADO.IsShowRemainingColumn = true;
                initADO.DelegateTotalAmountChanged = OnInvoicePayformTotalChanged;

                payformProcessorInvoice = new UcPayform.UCTransactionPayformGridProcessor(new CommonParam());
                ucPayformInvoice = (UserControl)payformProcessorInvoice.Run(initADO);
                if (ucPayformInvoice == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("[MultiPayform] Run() Invoice tra ve NULL.");
                    return;
                }
                HostInvoicePayformUc();
                HideOldInvoicePayformControls();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Nhung UC vao group "Hoa don vien phi" (giong style luoi Chiet khau: AddItem 1 hang full ngang).</summary>
        private void HostRecieptPayformUc()
        {
            try
            {
                this.lciNotReciept.BeginUpdate();
                try
                {
                    this.lciNotReciept.Controls.Add(this.ucPayformReciept);

                    var lciGrid = new LayoutControlItem();
                    lciGrid.Name = "lciRecieptPayformGrid";
                    lciGrid.Text = "Hình thức:";
                    lciGrid.TextSize = new System.Drawing.Size(90, 20);
                    lciGrid.TextToControlDistance = 5;
                    lciGrid.AppearanceItemCaption.Options.UseTextOptions = true;
                    lciGrid.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    lciGrid.SizeConstraintsType = SizeConstraintsType.Custom;
                    lciGrid.MinSize = new System.Drawing.Size(0, 72);
                    lciGrid.MaxSize = new System.Drawing.Size(0, 72); // gon: header + dong nhap + footer (du them -> cuon doc)
                    lciGrid.Control = this.ucPayformReciept;

                    this.lcgReceiptGroup.AddItem(lciGrid);
                }
                finally
                {
                    this.lciNotReciept.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Nhung UC vao group "Hoa don dich vu".</summary>
        private void HostInvoicePayformUc()
        {
            try
            {
                this.layoutControl5.BeginUpdate();
                try
                {
                    this.layoutControl5.Controls.Add(this.ucPayformInvoice);

                    var lciGrid = new LayoutControlItem();
                    lciGrid.Name = "lciInvoicePayformGrid";
                    lciGrid.Text = "Hình thức:";
                    lciGrid.TextSize = new System.Drawing.Size(70, 20);
                    lciGrid.TextToControlDistance = 5;
                    lciGrid.AppearanceItemCaption.Options.UseTextOptions = true;
                    lciGrid.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    lciGrid.SizeConstraintsType = SizeConstraintsType.Custom;
                    lciGrid.MinSize = new System.Drawing.Size(0, 72);
                    lciGrid.MaxSize = new System.Drawing.Size(0, 72); // gon: header + dong nhap + footer (du them -> cuon doc)
                    lciGrid.Control = this.ucPayformInvoice;

                    this.lcgInvoiceGroup.AddItem(lciGrid);
                }
                finally
                {
                    this.layoutControl5.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// An cac control hinh thuc/so tien/CK/QT/ngan hang CU cua so Vien phi (nhap qua UC luoi).
        /// An theo TEN lci truc tiep (giong luoi Chiet khau) - GetItemByControl khong an duoc.
        /// </summary>
        private void HideOldRecieptPayformControls()
        {
            try
            {
                this.lciNotReciept.BeginUpdate();
                try
                {
                    this.layoutControlItem78.Visibility = LayoutVisibility.Never;  // Hình thức
                    this.layoutControlItem80.Visibility = LayoutVisibility.Never;  // Số tiền / CK
                    this.lciSoTienQTReceipt.Visibility = LayoutVisibility.Never;   // Số tiền QT
                    this.layoutControlItem81.Visibility = LayoutVisibility.Never;  // Ngân hàng
                }
                finally
                {
                    this.lciNotReciept.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>An cac control hinh thuc/so tien/CK/QT/ngan hang CU cua so Dich vu.</summary>
        private void HideOldInvoicePayformControls()
        {
            try
            {
                this.layoutControl5.BeginUpdate();
                try
                {
                    this.layoutControlItem79.Visibility = LayoutVisibility.Never;  // Hình thức
                    this.layoutControlItem77.Visibility = LayoutVisibility.Never;  // Số tiền / CK
                    this.lciInvoiceQT.Visibility = LayoutVisibility.Never;         // Số tiền QT
                    this.layoutControlItem82.Visibility = LayoutVisibility.Never;  // Ngân hàng
                }
                finally
                {
                    this.layoutControl5.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Dong bo so tien can thu + enable/disable
        /// <summary>Cap nhat "so tien phai thu" cua luoi so Vien phi (goi cuoi CalcuTotalPrice).</summary>
        private void UpdateRecieptPayformRequiredAmount()
        {
            try
            {
                if (isMultiPayform && payformProcessorReciept != null && ucPayformReciept != null)
                {
                    payformProcessorReciept.SetRequiredAmount(ucPayformReciept, GetCurrentRecieptRequiredAmount());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Cap nhat "so tien phai thu" cua luoi so Dich vu.</summary>
        private void UpdateInvoicePayformRequiredAmount()
        {
            try
            {
                if (isMultiPayform && payformProcessorInvoice != null && ucPayformInvoice != null)
                {
                    payformProcessorInvoice.SetRequiredAmount(ucPayformInvoice, GetCurrentInvoiceRequiredAmount());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal GetCurrentRecieptRequiredAmount()
        {
            try
            {
                if (checkNotReciept.Checked) return 0;
                return Inventec.Common.TypeConvert.Parse.ToDecimal(
                    (lblRecieptAmount.Text ?? "0").Replace(".", "").Replace(",", ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        private decimal GetCurrentInvoiceRequiredAmount()
        {
            try
            {
                if (checkNotInvoice.Checked) return 0;
                return Inventec.Common.TypeConvert.Parse.ToDecimal(
                    (lblInvoiceAmount.Text ?? "0").Replace(".", "").Replace(",", ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        /// <summary>Bat/tat luoi hinh thuc so Vien phi (goi khi tick "Khong TT").</summary>
        internal void SetEnableRecieptPayform(bool enable)
        {
            try
            {
                if (isMultiPayform && ucPayformReciept != null)
                {
                    ucPayformReciept.Enabled = enable;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Bat/tat luoi hinh thuc so Dich vu.</summary>
        internal void SetEnableInvoicePayform(bool enable)
        {
            try
            {
                if (isMultiPayform && ucPayformInvoice != null)
                {
                    ucPayformInvoice.Enabled = enable;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnRecieptPayformTotalChanged(decimal totalAmount, decimal remainAmount)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    string.Format("[MultiPayform] Reciept total={0}, remain={1}", totalAmount, remainAmount));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnInvoicePayformTotalChanged(decimal totalAmount, decimal remainAmount)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    string.Format("[MultiPayform] Invoice total={0}, remain={1}", totalAmount, remainAmount));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Lay du lieu + ghi vao SDO khi luu
        private List<UcPayform.ADO.PayformRowADO> GetRecieptPayformRows()
        {
            try
            {
                if (isMultiPayform && payformProcessorReciept != null && ucPayformReciept != null)
                {
                    return payformProcessorReciept.GetData(ucPayformReciept) as List<UcPayform.ADO.PayformRowADO>;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<UcPayform.ADO.PayformRowADO> GetInvoicePayformRows()
        {
            try
            {
                if (isMultiPayform && payformProcessorInvoice != null && ucPayformInvoice != null)
                {
                    return payformProcessorInvoice.GetData(ucPayformInvoice) as List<UcPayform.ADO.PayformRowADO>;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// Ghi danh sach hinh thuc TT so Vien phi vao RecieptTransaction.HIS_TRANSACTION_PAYFORM truoc khi POST CreateBillTwoBook.
        /// LUU Y: backend SDO 2 so hien chi co 1 field PayformDetails dung chung -> FE gan theo NAV cua tung transaction.
        /// Backend can doc HIS_TRANSACTION_PAYFORM cua tung transaction (hoac bo sung Reciept/InvoicePayformDetails vao SDO).
        /// </summary>
        private void ApplyRecieptPayformToSave(MOS.SDO.HisTransactionBillTwoBookSDO billTwoBookSDO)
        {
            try
            {
                if (!isMultiPayform || billTwoBookSDO == null || billTwoBookSDO.RecieptTransaction == null) return;
                if (checkNotReciept.Checked) return; // So khong thanh toan -> bo qua

                var rows = (GetRecieptPayformRows() ?? new List<UcPayform.ADO.PayformRowADO>())
                    .Where(o => o.PAY_FORM_ID > 0)
                    .ToList();
                if (rows.Count == 0) return;

                billTwoBookSDO.RecieptTransaction.HIS_TRANSACTION_PAYFORM = BuildTransactionPayforms(rows);

                // Set dong dau vao Transaction don de tuong thich logic cu (hien thi 1 hinh thuc).
                var first = rows[0];
                billTwoBookSDO.RecieptTransaction.PAY_FORM_ID = first.PAY_FORM_ID;
                billTwoBookSDO.RecieptTransaction.BANK_ID = first.BANK_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Ghi danh sach hinh thuc TT so Dich vu vao InvoiceTransaction.HIS_TRANSACTION_PAYFORM.</summary>
        private void ApplyInvoicePayformToSave(MOS.SDO.HisTransactionBillTwoBookSDO billTwoBookSDO)
        {
            try
            {
                if (!isMultiPayform || billTwoBookSDO == null || billTwoBookSDO.InvoiceTransaction == null) return;
                if (checkNotInvoice.Checked) return;

                var rows = (GetInvoicePayformRows() ?? new List<UcPayform.ADO.PayformRowADO>())
                    .Where(o => o.PAY_FORM_ID > 0)
                    .ToList();
                if (rows.Count == 0) return;

                billTwoBookSDO.InvoiceTransaction.HIS_TRANSACTION_PAYFORM = BuildTransactionPayforms(rows);

                var first = rows[0];
                billTwoBookSDO.InvoiceTransaction.PAY_FORM_ID = first.PAY_FORM_ID;
                billTwoBookSDO.InvoiceTransaction.BANK_ID = first.BANK_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Map cac dong luoi (PayformRowADO) sang entity HIS_TRANSACTION_PAYFORM. KHONG set nav back-reference (tranh vong lap serialize).</summary>
        private List<HIS_TRANSACTION_PAYFORM> BuildTransactionPayforms(List<UcPayform.ADO.PayformRowADO> rows)
        {
            var result = new List<HIS_TRANSACTION_PAYFORM>();
            short sortOrder = 1;
            foreach (var r in rows)
            {
                bool hasCurrency = !string.IsNullOrEmpty(r.CURRENCY_CODE);
                result.Add(new HIS_TRANSACTION_PAYFORM
                {
                    ID = 0,
                    TRANSACTION_ID = 0, // backend gan sau khi tao HIS_TRANSACTION
                    PAY_FORM_ID = r.PAY_FORM_ID,
                    BANK_ID = r.BANK_ID,
                    AMOUNT = r.AMOUNT,
                    SURCHARGE_AMOUNT = r.BANK_FEE_AMOUNT,
                    SURCHARGE_NAME = r.BANK_FEE_NAME,
                    TOTAL_AMOUNT = r.TOTAL_AMOUNT_VND,
                    FOREIGN_AMOUNT = hasCurrency ? (decimal?)r.AMOUNT : null,
                    EXCHANGE_RATE = hasCurrency ? (decimal?)r.EXCHANGE_RATE : null,
                    CURRENCY_ID = r.CURRENCY_ID,
                    CURRENCY_CODE = r.CURRENCY_CODE,
                    IS_REMAINDER = (short)(r.IS_REMAINING ? 1 : 0),
                    SORT_ORDER = sortOrder++
                });
            }
            return result;
        }
        #endregion
    }
}
