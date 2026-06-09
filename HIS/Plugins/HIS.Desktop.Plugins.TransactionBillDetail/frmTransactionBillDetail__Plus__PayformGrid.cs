/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * 4.2.6 Chi tiết thanh toán — Nhúng UC HIS.UC.TransactionPayformGrid để hiển thị + sửa
 * hình thức thanh toán của 1 giao dịch (TRANSACTION_ID = billId), + nút "Lưu hình thức thanh toán".
 *   - Load: GET api/HisTransactionPayform/Get (TRANSACTION_ID) -> nạp vào UC.
 *   - Lưu : POST api/HisTransaction/UpdatePayformDetails (HisTransactionUpdatePayformDetailsSDO).
 */
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UcPayform = HIS.UC.TransactionPayformGrid;

namespace HIS.Desktop.Plugins.TransactionBillDetail
{
    public partial class frmTransactionBillDetail
    {
        #region Declare Payform
        UcPayform.UCTransactionPayformGridProcessor payformProcessor;
        UserControl ucPayform;
        DevExpress.XtraEditors.SimpleButton btnSavePayform;
        #endregion

        /// <summary>Tạo UC lưới hình thức thanh toán + nút Lưu, nhúng vào layout. Gọi trong constructor sau InitSereServTree().</summary>
        private void InitPayformGrid()
        {
            try
            {
                var initADO = new UcPayform.ADO.TransactionPayformGridInitADO();
                initADO.RequiredAmount = 0;
                initADO.IsShowRemainingColumn = true;

                payformProcessor = new UcPayform.UCTransactionPayformGridProcessor(new CommonParam());
                ucPayform = (UserControl)payformProcessor.Run(initADO);
                if (ucPayform == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("[BillDetail] Khởi tạo UC TransactionPayformGrid trả về NULL.");
                    return;
                }

                btnSavePayform = new DevExpress.XtraEditors.SimpleButton();
                btnSavePayform.Name = "btnSavePayform";
                btnSavePayform.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__BTN_SAVE_PAYFORM",
                    Base.ResourceLangManager.LanguageFrmTransactionBillDetail,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                btnSavePayform.Click += btnSavePayform_Click;

                HostPayformGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Nhúng UC + nút vào layoutControlGroup1 (dưới cây dịch vụ — cây giữ vai trò fill phía trên).</summary>
        private void HostPayformGrid()
        {
            try
            {
                this.layoutControl1.BeginUpdate();
                try
                {
                    // 1. UC lưới hình thức
                    this.layoutControl1.Controls.Add(this.ucPayform);
                    var lciGrid = new LayoutControlItem();
                    lciGrid.Name = "lciPayformGrid";
                    lciGrid.Text = Inventec.Common.Resource.Get.Value(
                        "IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__LBL_PAYFORM",
                        Base.ResourceLangManager.LanguageFrmTransactionBillDetail,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciGrid.TextSize = new System.Drawing.Size(90, 20);
                    lciGrid.TextToControlDistance = 5;
                    lciGrid.AppearanceItemCaption.Options.UseTextOptions = true;
                    lciGrid.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    lciGrid.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
                    lciGrid.SizeConstraintsType = SizeConstraintsType.Custom;
                    lciGrid.MinSize = new System.Drawing.Size(0, 110);
                    lciGrid.MaxSize = new System.Drawing.Size(0, 110);
                    lciGrid.Control = this.ucPayform;
                    this.layoutControlGroup1.AddItem(lciGrid);

                    // 2. Nút "Lưu hình thức thanh toán" — căn phải, có khoảng trống bên trái
                    this.layoutControl1.Controls.Add(this.btnSavePayform);
                    var lciBtn = new LayoutControlItem();
                    lciBtn.Name = "lciBtnSavePayform";
                    lciBtn.Control = this.btnSavePayform;
                    lciBtn.TextVisible = false;
                    lciBtn.SizeConstraintsType = SizeConstraintsType.Custom;
                    lciBtn.MinSize = new System.Drawing.Size(200, 30);
                    lciBtn.MaxSize = new System.Drawing.Size(200, 30);
                    this.layoutControlGroup1.AddItem(lciBtn);

                    var emptyLeft = new EmptySpaceItem();
                    emptyLeft.Name = "emptyLeftOfSavePayform";
                    this.layoutControlGroup1.AddItem(emptyLeft, lciBtn, InsertType.Left);
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

        /// <summary>Tải hình thức thanh toán hiện có của giao dịch -> điền vào UC. Gọi trong Load (sau khi đã có billId).</summary>
        private void LoadPayformData()
        {
            try
            {
                if (payformProcessor == null || ucPayform == null || this.billId <= 0) return;

                CommonParam param = new CommonParam();
                var filter = new HisTransactionPayformFilter();
                filter.TRANSACTION_ID = this.billId;
                var data = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<HIS_TRANSACTION_PAYFORM>>("api/HisTransactionPayform/Get",
                        ApiConsumers.MosConsumer, filter, param)
                    ?? new List<HIS_TRANSACTION_PAYFORM>();

                var rows = data.OrderBy(o => o.SORT_ORDER ?? 0).Select(MapToPayformRow).ToList();
                // Số tiền phải thu = tổng thành tiền các dòng hiện có (để cột "Còn lại" cân bằng)
                payformProcessor.SetRequiredAmount(ucPayform, data.Sum(o => o.TOTAL_AMOUNT));
                payformProcessor.Reload(ucPayform, rows);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Map entity HIS_TRANSACTION_PAYFORM -> dòng hiển thị PayformRowADO. UC tự hiển thị tên theo ID.</summary>
        private UcPayform.ADO.PayformRowADO MapToPayformRow(HIS_TRANSACTION_PAYFORM o)
        {
            return new UcPayform.ADO.PayformRowADO
            {
                PAY_FORM_ID = o.PAY_FORM_ID,
                BANK_ID = o.BANK_ID,
                AMOUNT = o.AMOUNT,
                BANK_FEE_AMOUNT = o.SURCHARGE_AMOUNT ?? 0,
                BANK_FEE_NAME = o.SURCHARGE_NAME,
                TOTAL_AMOUNT_VND = o.TOTAL_AMOUNT,
                CURRENCY_ID = o.CURRENCY_ID,
                CURRENCY_CODE = o.CURRENCY_CODE,
                EXCHANGE_RATE = o.EXCHANGE_RATE ?? 1,
                IS_REMAINING = (o.IS_REMAINDER ?? 0) == 1
            };
        }

        private void btnSavePayform_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (payformProcessor == null || ucPayform == null || this.billId <= 0) return;

                var rows = (payformProcessor.GetData(ucPayform) as List<UcPayform.ADO.PayformRowADO>
                            ?? new List<UcPayform.ADO.PayformRowADO>())
                    .Where(o => o.PAY_FORM_ID > 0)
                    .ToList();

                var sdo = new MOS.SDO.HisTransactionUpdatePayformDetailsSDO();
                sdo.TransactionId = this.billId;
                sdo.RequestRoomId = this.currentModule != null ? this.currentModule.RoomId : 0;
                sdo.PayformDetails = new List<MOS.SDO.PayformDetailSDO>();
                short sortOrder = 1;
                foreach (var r in rows)
                {
                    bool hasCurrency = !string.IsNullOrEmpty(r.CURRENCY_CODE);
                    sdo.PayformDetails.Add(new MOS.SDO.PayformDetailSDO
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
                        IsRemainder = r.IS_REMAINING,
                        SortOrder = sortOrder++
                    });
                }

                btnSavePayform.Enabled = false;
                WaitingManager.Show();
                var result = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<bool>("api/HisTransaction/UpdatePayformDetails", ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();

                bool success = result && !param.HasException;
                MessageManager.Show(this, param, success);
                if (success)
                {
                    LoadPayformData(); // refresh lại đúng dữ liệu đã lưu
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                if (btnSavePayform != null) btnSavePayform.Enabled = true;
            }
        }
    }
}
