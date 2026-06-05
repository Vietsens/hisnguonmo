/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tich hop UC luoi hinh thuc thanh toan (HIS.UC.TransactionPayformGrid).
 * Chi hoat dong khi config MOS.HIS_TRANSACTION.MULTI_PAYFORM = 1.
 */
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TransactionBill.Config;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UcPayform = HIS.UC.TransactionPayformGrid;

namespace HIS.Desktop.Plugins.TransactionBill
{
    public partial class frmTransactionBill : HIS.Desktop.Utility.FormBase
    {
        #region Declare MultiPayform
        const string CONFIG_MULTI_PAYFORM = "MOS.HIS_TRANSACTION.MULTI_PAYFORM";
        const string CONFIG_PAYFORM_BANK_FEE = "HisPayFormBankFee";

        UcPayform.UCTransactionPayformGridProcessor payformGridProcessor;
        UserControl ucPayformGrid;
        bool isMultiPayform;
        #endregion

        /// <summary>
        /// Khoi tao UC luoi hinh thuc thanh toan khi config bat.
        /// Goi trong timerInitForm_Tick (sau CalcuCanThu).
        /// </summary>
        // Doi BUILD_TAG moi lan sua de nhan biet dll dang chay co phai ban moi khong (grep trong LogSystem.txt)
        private const string PAYFORM_BUILD_TAG = "MultiPayform-20260603-03";

        private void InitMultiPayformGrid()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("[MultiPayform] >>> InitMultiPayformGrid BEGIN. BUILD_TAG=" + PAYFORM_BUILD_TAG);

                isMultiPayform = ReadMultiPayformConfig();
                Inventec.Common.Logging.LogSystem.Info("[MultiPayform] config MULTI_PAYFORM = " + isMultiPayform + " (HisConfigCFG.MultiPayform=" + HisConfigCFG.MultiPayform + ")");
                if (!isMultiPayform)
                {
                    Inventec.Common.Logging.LogSystem.Info("[MultiPayform] CONFIG OFF -> giu giao dien cu, khong hien UC.");
                    return; // Config tat -> giu nguyen giao dien cu
                }

                // UC TU LAY danh muc (PayForm/Bank/Currency/BankFee) - form cha CHI truyen sizing + can thu + callback.
                var initADO = new UcPayform.ADO.TransactionPayformGridInitADO();
                initADO.RequiredAmount = GetCurrentRequiredAmount();
                initADO.IsShowRemainingColumn = true;
                initADO.DelegateTotalAmountChanged = OnPayformGridTotalChanged;
                initADO.DeleteButtonImage = GetFundDeleteImage();
                // Sizing: truyen kich thuoc mong muon cho UC (UC tu ap trong ApplySizing)
                initADO.Width = (this.LciBillFund != null ? this.LciBillFund.Size.Width : 500);
                initADO.Height = 95;
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "[MultiPayform] RequiredAmount={0}, Size={1}x{2}",
                    initADO.RequiredAmount, initADO.Width, initADO.Height));

                payformGridProcessor = new UcPayform.UCTransactionPayformGridProcessor(new CommonParam());
                ucPayformGrid = (UserControl)payformGridProcessor.Run(initADO);
                if (ucPayformGrid == null)
                {
                    isMultiPayform = false;
                    Inventec.Common.Logging.LogSystem.Error("[MultiPayform] Run() tra ve NULL -> khong tao duoc UC (xem loi Factory/UC phia tren).");
                    return;
                }
                Inventec.Common.Logging.LogSystem.Info("[MultiPayform] Run() OK, UC da tao. Bat dau nhung vao layout...");

                HostPayformGridUc();
                HideOldPayformControls();
                MoveDescriptionToTop();
                Inventec.Common.Logging.LogSystem.Info("[MultiPayform] <<< InitMultiPayformGrid DONE - UC da hien.");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("[MultiPayform] InitMultiPayformGrid EXCEPTION", ex);
            }
        }

        /// <summary>Lay icon X (do) tu nut xoa cua grid Quy ho tro de dong bo visual.</summary>
        private System.Drawing.Image GetFundDeleteImage()
        {
            try
            {
                if (this.repositoryItemBtnDeleteFund != null && this.repositoryItemBtnDeleteFund.Buttons.Count > 0)
                {
                    return this.repositoryItemBtnDeleteFund.Buttons[0].Image;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>Khi dung UC: day "Ghi chu" len ngang voi "So tien" (giong layout goc).</summary>
        private void MoveDescriptionToTop()
        {
            try
            {
                if (this.layoutDescription != null && this.layoutTotalAmount != null
                    && this.layoutTotalAmount.Parent != null)
                {
                    this.layoutDescription.Move(this.layoutTotalAmount, DevExpress.XtraLayout.Utils.InsertType.Left);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ReadMultiPayformConfig()
        {
            try
            {
                // Log gia tri config tho de doi chieu (phong khi HisConfigCFG chua load kip)
                string raw = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_MULTI_PAYFORM);
                Inventec.Common.Logging.LogSystem.Info("[MultiPayform] raw HisConfigs(" + CONFIG_MULTI_PAYFORM + ")='" + (raw ?? "<null>") + "'");
                // Doc tu HisConfigCFG (da load luc form Load) cho dong bo voi cac config khac
                return HisConfigCFG.MultiPayform;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>
        /// Nhung UC vao layoutControl1 — can chinh GIONG grid Chiet khau / Quy ho tro:
        /// label "Hinh thuc:" ben trai (TextSize 90), cung chieu cao 70, dat phia tren Chiet khau.
        /// Thu tu hien thi: Hinh thuc -> Chiet khau -> Quy ho tro.
        /// </summary>
        private void HostPayformGridUc()
        {
            try
            {
                this.layoutControl1.BeginUpdate();
                try
                {
                    this.layoutControl1.Controls.Add(this.ucPayformGrid);

                    LayoutControlItem lci = new LayoutControlItem();
                    lci.Name = "lciPayformGrid";
                    lci.Control = this.ucPayformGrid;
                    lci.AppearanceItemCaption.Options.UseTextOptions = true;
                    lci.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    lci.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
                    lci.Text = "Hình thức:";
                    lci.TextAlignMode = TextAlignModeItem.CustomSize;
                    lci.TextSize = new System.Drawing.Size(90, 0);
                    lci.TextToControlDistance = 5;

                    // Cao hon Chiet khau/Quy ho tro mot chut de mo len thay du 2 dong (header + dong nhap + dong them)
                    int gridHeight = 95;
                    int gridWidth = this.LciBillFund != null ? this.LciBillFund.Size.Width : 500;

                    lci.SizeConstraintsType = SizeConstraintsType.Custom;
                    lci.MinSize = new System.Drawing.Size(gridWidth, gridHeight);
                    lci.MaxSize = new System.Drawing.Size(0, gridHeight);
                    lci.Size = new System.Drawing.Size(gridWidth, gridHeight);

                    this.layoutControlGroup1.AddItem(lci);

                    // Dat phia TREN grid Chiet khau (neu co), nguoc lai tren Quy ho tro
                    DevExpress.XtraLayout.BaseLayoutItem anchor = null;
                    if (this.lciDiscountGrid != null && this.lciDiscountGrid.Parent != null)
                        anchor = this.lciDiscountGrid;
                    else if (this.LciBillFund != null && this.LciBillFund.Parent != null)
                        anchor = this.LciBillFund;

                    if (anchor != null)
                    {
                        lci.Move(anchor, DevExpress.XtraLayout.Utils.InsertType.Top);
                    }
                }
                finally
                {
                    this.layoutControl1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>An cac control hinh thuc cu khi dung UC luoi.</summary>
        private void HideOldPayformControls()
        {
            try
            {
                HideLayoutItemOfControl(this.cboPayForm);       // Hình thức
                HideLayoutItemOfControl(this.cboBank);           // Ngân hàng
                HideLayoutItemOfControl(this.spinTransferAmount); // Số tiền (CK cũ)
                HideLayoutItemOfControl(this.spinTransferAmountNew); // Số tiền CK
                HideLayoutItemOfControl(this.spinSwipeAmountNew);    // Số tiền QT
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HideLayoutItemOfControl(Control control)
        {
            try
            {
                if (control == null) return;
                LayoutControlItem item = this.layoutControl1.GetItemByControl(control) as LayoutControlItem;
                if (item != null)
                {
                    item.Visibility = LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdatePayformGridRequiredAmount(decimal requiredAmount)
        {
            try
            {
                if (isMultiPayform && payformGridProcessor != null && ucPayformGrid != null)
                {
                    payformGridProcessor.SetRequiredAmount(ucPayformGrid, requiredAmount);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal GetCurrentRequiredAmount()
        {
            try
            {
                return Inventec.Common.TypeConvert.Parse.ToDecimal(
                    (lblReceiveAmount.Text ?? "0").Replace(".", "").Replace(",", ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        private void OnPayformGridTotalChanged(decimal totalAmount, decimal remainAmount)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    string.Format("PayformGrid total={0}, remain={1}", totalAmount, remainAmount));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<UcPayform.ADO.PayformRowADO> GetPayformGridRows()
        {
            try
            {
                if (isMultiPayform && payformGridProcessor != null && ucPayformGrid != null)
                {
                    return payformGridProcessor.GetData(ucPayformGrid) as List<UcPayform.ADO.PayformRowADO>;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// Ghi du lieu hinh thuc thanh toan tu UC vao SDO truoc khi goi CreateBill.
        /// Goi trong ProcessSave khi isMultiPayform = true.
        /// LUU Y: HisTransactionBillSDO hien chi ho tro 1 Transaction. De gui DANH SACH
        /// hinh thuc len API can backend bo sung field (vd HisTransactionBillSDO.PayForms).
        /// Tam thoi: set dong dau vao Transaction don + log canh bao neu co nhieu dong.
        /// </summary>
        private bool ApplyPayformGridToSave(MOS.SDO.HisTransactionBillSDO data)
        {
            try
            {
                if (!isMultiPayform) return true;

                var allRows = GetPayformGridRows();
                // Loai dong NewItemRow trong (chua chon hinh thuc) -> tranh gui PayFormId=0
                var rows = (allRows ?? new List<UcPayform.ADO.PayformRowADO>())
                    .Where(o => o.PAY_FORM_ID > 0)
                    .ToList();
                if (rows.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("MultiPayform bat nhung khong co dong hinh thuc hop le nao.");
                    return false;
                }

                // Do toan bo danh sach hinh thuc vao PayformDetails (backend da ho tro)
                data.PayformDetails = new List<MOS.SDO.PayformDetailSDO>();
                int sortOrder = 1;
                foreach (var r in rows)
                {
                    bool hasCurrency = !string.IsNullOrEmpty(r.CURRENCY_CODE);
                    data.PayformDetails.Add(new MOS.SDO.PayformDetailSDO
                    {
                        PayFormId = r.PAY_FORM_ID,
                        BankId = r.BANK_ID,
                        Amount = r.AMOUNT,
                        SurchargeAmount = r.BANK_FEE_AMOUNT,
                        SurchargeName = r.BANK_FEE_NAME,
                        TotalAmount = r.TOTAL_AMOUNT_VND,
                        ForeignAmount = hasCurrency ? (decimal?)r.AMOUNT : null,
                        ExchangeRate = hasCurrency ? (decimal?)r.EXCHANGE_RATE : null,
                        CurrencyId = r.CURRENCY_ID,
                        CurrencyCode = r.CURRENCY_CODE,
                        IsRemainder = false,
                        SortOrder = (short)(sortOrder++)
                    });
                }

                // Set dong dau + tong vao Transaction don de tuong thich logic cu
                var first = rows[0];
                data.Transaction.PAY_FORM_ID = first.PAY_FORM_ID;
                data.Transaction.BANK_ID = first.BANK_ID;
                data.Transaction.AMOUNT = rows.Sum(o => o.TOTAL_AMOUNT_VND);

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => data.PayformDetails), data.PayformDetails));

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
    }
}
