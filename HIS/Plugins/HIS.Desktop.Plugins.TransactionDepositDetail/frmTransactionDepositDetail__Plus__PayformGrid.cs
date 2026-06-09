/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * 4.2.7 Chi tiết tạm ứng — Tương tự 4.2.6: nhúng UC HIS.UC.TransactionPayformGrid để hiển thị + sửa
 * hình thức thanh toán của giao dịch tạm ứng, + nút "Lưu hình thức thanh toán".
 *   - Load: GET api/HisTransactionPayform/Get (TRANSACTION_ID) -> nạp vào UC.
 *   - Lưu : POST api/HisTransaction/UpdatePayformDetails (HisTransactionUpdatePayformDetailsSDO).
 * LƯU Ý: form nhận billId = DEPOSIT_ID (phiếu tạm ứng) — dùng làm TRANSACTION_ID của giao dịch tạm ứng.
 * Form này KHÔNG có LayoutControl nên nhúng bằng docking (panel đáy + đưa cây dịch vụ BringToFront).
 */
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

namespace HIS.Desktop.Plugins.TransactionDepositDetail
{
    public partial class frmTransactionDepositDetail
    {
        #region Declare Payform
        UcPayform.UCTransactionPayformGridProcessor payformProcessor;
        UserControl ucPayform;
        DevExpress.XtraEditors.SimpleButton btnSavePayform;
        DevExpress.XtraEditors.PanelControl pnlPayformHost;
        #endregion

        /// <summary>Tạo UC lưới hình thức thanh toán + nút Lưu, nhúng vào form. Gọi trong constructor sau InitSereServTree().</summary>
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
                    Inventec.Common.Logging.LogSystem.Error("[DepositDetail] Khởi tạo UC TransactionPayformGrid trả về NULL.");
                    return;
                }
                HostPayformGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nhúng UC + nút vào form bằng docking (form không có LayoutControl):
        /// panel đáy chứa [footer nút] + [UC fill]; cây dịch vụ BringToFront để fill phần còn lại phía trên.
        /// </summary>
        private void HostPayformGrid()
        {
            try
            {
                this.SuspendLayout();
                try
                {
                    pnlPayformHost = new DevExpress.XtraEditors.PanelControl();
                    pnlPayformHost.Name = "pnlPayformHost";
                    pnlPayformHost.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                    pnlPayformHost.Dock = DockStyle.Bottom;
                    pnlPayformHost.Height = 150;

                    // Footer chứa nút Lưu (căn phải)
                    var pnlFooter = new DevExpress.XtraEditors.PanelControl();
                    pnlFooter.Name = "pnlPayformFooter";
                    pnlFooter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                    pnlFooter.Dock = DockStyle.Bottom;
                    pnlFooter.Height = 36;

                    btnSavePayform = new DevExpress.XtraEditors.SimpleButton();
                    btnSavePayform.Name = "btnSavePayform";
                    btnSavePayform.Text = "Lưu hình thức thanh toán";
                    btnSavePayform.Width = 200;
                    btnSavePayform.Dock = DockStyle.Right;
                    btnSavePayform.Click += btnSavePayform_Click;
                    pnlFooter.Controls.Add(btnSavePayform);

                    pnlPayformHost.Controls.Add(this.ucPayform);
                    this.ucPayform.Dock = DockStyle.Fill;
                    pnlPayformHost.Controls.Add(pnlFooter);
                    this.ucPayform.BringToFront(); // UC fill phần trên footer

                    this.Controls.Add(pnlPayformHost);
                    this.panelControlSereServTree.BringToFront(); // cây dịch vụ fill phần còn lại phía trên
                }
                finally
                {
                    this.ResumeLayout(false);
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
                    LoadPayformData();
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
