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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Base;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.ADO;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    /// <summary>
    /// Viec 57853: dinh kem don thuoc (tep/anh chup) cho phieu xuat ban — luu sang EMR qua
    /// HIS.Desktop.Plugins.Library.ExpMestAttachFile.
    /// - Phieu CHUA luu (ADD): tep giu tam trong RAM, luu phieu thanh cong -> tu dong dinh kem vao phieu vua tao.
    /// - Phieu DA co (EDIT): mo danh sach don dinh kem cua phieu (xem/in, bo sung, xoa theo trang thai phieu).
    /// Nut "Dinh kem don" chi tao khi bat config (khong bat -> giao dien giu nguyen nhu cu).
    /// </summary>
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        private const string IMAGE__ATTACH = "images/mail/attach_16x16.png";

        private SimpleButton btnAttachPrescription;
        private LayoutControlItem lciAttachPrescription;

        /// <summary>Tep da chon khi phieu chua luu (chua co EXP_MEST_CODE)</summary>
        private PendingAttachADO pendingAttachPrescription;

        /// <summary>Phieu xuat dang hien thi o che do EDIT (sau luu / mo sua / tim theo don)</summary>
        private List<V_HIS_EXP_MEST> attachExpMests;

        /// <summary>Tao nut "Dinh kem don" canh nut "Huy xuat" (goi trong Load)</summary>
        private void InitAttachPrescriptionButton()
        {
            try
            {
                if (!ExpMestAttachFileProcessor.IsEnable() || this.btnAttachPrescription != null)
                    return;

                LayoutGroup group = this.layoutControlItem1.Parent;
                if (group == null)
                    return;

                this.layoutControl1.BeginUpdate();
                try
                {
                    this.btnAttachPrescription = new SimpleButton();
                    this.btnAttachPrescription.Name = "btnAttachPrescription";
                    this.btnAttachPrescription.StyleController = this.layoutControl1;
                    this.btnAttachPrescription.ToolTip = ExpMestAttachFileProcessor.GetButtonToolTip();
                    this.btnAttachPrescription.Image = DevExpress.Images.ImageResourceCache.Default.GetImage(IMAGE__ATTACH);
                    this.btnAttachPrescription.Click += new EventHandler(this.btnAttachPrescription_Click);

                    // Chen ben trai nut "Huy xuat" (layoutControlItem1) bang item-move (DevExpress 15.2),
                    // co dinh do rong de khong chia doi nut Huy xuat
                    this.lciAttachPrescription = new LayoutControlItem();
                    this.lciAttachPrescription.Control = this.btnAttachPrescription;
                    group.AddItem(this.lciAttachPrescription);
                    this.lciAttachPrescription.Move(this.layoutControlItem1, InsertType.Left);
                    this.lciAttachPrescription.Name = "lciAttachPrescription";
                    this.lciAttachPrescription.TextVisible = false;
                    this.lciAttachPrescription.SizeConstraintsType = SizeConstraintsType.Custom;
                    this.lciAttachPrescription.MinSize = new System.Drawing.Size(120, 26);
                    this.lciAttachPrescription.MaxSize = new System.Drawing.Size(120, 26);
                }
                finally
                {
                    this.layoutControl1.EndUpdate();
                }
                RefreshAttachPrescriptionCaption();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefreshAttachPrescriptionCaption()
        {
            try
            {
                if (this.btnAttachPrescription == null) return;
                int pendingCount = this.pendingAttachPrescription != null ? this.pendingAttachPrescription.Count : 0;
                this.btnAttachPrescription.Text = ExpMestAttachFileProcessor.GetButtonCaption(pendingCount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Ghi nhan phieu dang hien thi khi chuyen sang che do EDIT</summary>
        private void SetAttachExpMests(IEnumerable<V_HIS_EXP_MEST> expMests)
        {
            try
            {
                this.attachExpMests = expMests != null ? expMests.Where(o => o != null).ToList() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<ExpMestAttachInfoADO> GetAttachExpMestInfos()
        {
            List<ExpMestAttachInfoADO> result = new List<ExpMestAttachInfoADO>();
            if (this.moduleAction == GlobalDataStore.ModuleAction.EDIT && this.attachExpMests != null)
                result = this.attachExpMests.Select(o => new ExpMestAttachInfoADO(o)).ToList();
            return result;
        }

        private void btnAttachPrescription_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnAttachPrescription.Enabled = false;
                try
                {
                    var expMests = GetAttachExpMestInfos();
                    if (!expMests.Any())
                    {
                        // Phieu chua luu -> chon tep, giu tam cho toi khi luu phieu
                        var result = ExpMestAttachFileProcessor.ChooseFiles(this.pendingAttachPrescription);
                        if (result != null)
                            this.pendingAttachPrescription = result;
                    }
                    else
                    {
                        // Con tep tam cua lan dinh kem truoc bi loi -> dinh kem lai truoc
                        if (this.pendingAttachPrescription != null && this.pendingAttachPrescription.Count > 0
                            && ExpMestAttachFileProcessor.AttachPendingFiles(expMests, this.pendingAttachPrescription))
                        {
                            ReleasePendingAttachPrescription();
                        }

                        if (expMests.Count == 1)
                        {
                            ExpMestAttachFileProcessor.ShowAttachList(expMests[0], this.roomId, null);
                        }
                        else
                        {
                            // Nhieu phieu cung luc (1 don -> nhieu phieu xuat): dinh kem cung bo tep cho tung phieu
                            var chosen = ExpMestAttachFileProcessor.ChooseFiles(null);
                            if (chosen != null && chosen.Count > 0)
                            {
                                ExpMestAttachFileProcessor.AttachPendingFiles(expMests, chosen);
                                ExpMestAttachFileProcessor.Release(chosen);
                            }
                        }
                    }
                }
                finally
                {
                    this.btnAttachPrescription.Enabled = true;
                    RefreshAttachPrescriptionCaption();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Goi sau khi luu phieu (ProcessSave): luu thanh cong -> dinh kem tep tam vao (cac) phieu vua luu.
        /// Ban nhieu benh nhan (SaleCreateBillList) -> khong tu dong dinh kem, bao nguoi dung dinh kem tai Danh sach xuat.
        /// </summary>
        private void ProcessAttachPrescriptionAfterSave(bool success)
        {
            try
            {
                if (!success) return;

                List<V_HIS_EXP_MEST> savedExpMests = null;
                if (this.isTwoPatient)
                {
                    if (this.ListResultSDO != null)
                        savedExpMests = this.ListResultSDO.Where(o => o != null && o.ExpMestSdos != null)
                            .SelectMany(o => o.ExpMestSdos).Select(o => o.ExpMest).ToList();
                }
                else if (this.resultSDO != null && this.resultSDO.ExpMestSdos != null)
                {
                    savedExpMests = this.resultSDO.ExpMestSdos.Select(o => o.ExpMest).ToList();
                }
                SetAttachExpMests(savedExpMests);

                if (this.btnAttachPrescription == null || this.pendingAttachPrescription == null || this.pendingAttachPrescription.Count == 0)
                    return;

                if (this.isTwoPatient)
                {
                    ExpMestAttachFileProcessor.ShowMultiPatientWarning();
                    ReleasePendingAttachPrescription();
                    return;
                }

                // Loi dinh kem -> giu tep tam, bam "Dinh kem don" de dinh kem lai
                if (ExpMestAttachFileProcessor.AttachPendingFiles(GetAttachExpMestInfos(), this.pendingAttachPrescription))
                    ReleasePendingAttachPrescription();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                RefreshAttachPrescriptionCaption();
            }
        }

        /// <summary>Nut Moi / Don moi: con tep tam chua luu -> hoi truoc khi bo. false = nguoi dung muon giu lai</summary>
        private bool ConfirmDiscardPendingAttachPrescription()
        {
            try
            {
                if (this.pendingAttachPrescription == null || this.pendingAttachPrescription.Count == 0)
                    return true;
                if (!ExpMestAttachFileProcessor.ConfirmDiscard(this.pendingAttachPrescription))
                    return false;
                ReleasePendingAttachPrescription();
                RefreshAttachPrescriptionCaption();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
        }

        private void ReleasePendingAttachPrescription()
        {
            try
            {
                ExpMestAttachFileProcessor.Release(this.pendingAttachPrescription);
                this.pendingAttachPrescription = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
