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
    /// Nut "Dinh kem don" CHI enable khi phieu xuat DA LUU (moduleAction = EDIT va co phieu dang hien thi:
    /// sau luu / mo sua tu danh sach / tim theo don) -> mo danh sach don dinh kem cua phieu
    /// (xem/in, bo sung, xoa theo trang thai phieu). Phieu chua luu (ADD) -> nut disable.
    /// Nut chi tao khi bat config (khong bat -> giao dien giu nguyen nhu cu).
    /// </summary>
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        private const string IMAGE__ATTACH = "images/mail/attach_16x16.png";

        private SimpleButton btnAttachPrescription;
        private LayoutControlItem lciAttachPrescription;

        /// <summary>Phieu xuat dang hien thi o che do EDIT (sau luu / mo sua / tim theo don)</summary>
        private List<V_HIS_EXP_MEST> attachExpMests;

        /// <summary>Lan luu vua roi la ban cho nhieu benh nhan (SaleCreateBillList) -> khong dinh kem chung 1 don</summary>
        private bool isAttachExpMestsMultiPatient;

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
                    this.btnAttachPrescription.Text = ExpMestAttachFileProcessor.GetButtonCaption(0);
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
                RefreshAttachPrescriptionButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Enable chi khi phieu da luu. Goi tu setter moduleAction + SetAttachExpMests nen moi luong
        /// (Moi, Don moi, tim don, mo sua, sau luu) deu cap nhat dung.
        /// </summary>
        private void RefreshAttachPrescriptionButtonState()
        {
            try
            {
                if (this.btnAttachPrescription == null) return;
                this.btnAttachPrescription.Enabled = GetAttachExpMestInfos().Any();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Ghi nhan phieu dang hien thi khi chuyen sang che do EDIT</summary>
        private void SetAttachExpMests(IEnumerable<V_HIS_EXP_MEST> expMests, bool isMultiPatient = false)
        {
            try
            {
                this.attachExpMests = expMests != null ? expMests.Where(o => o != null).ToList() : null;
                this.isAttachExpMestsMultiPatient = isMultiPatient;
                RefreshAttachPrescriptionButtonState();
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
                var expMests = GetAttachExpMestInfos();
                // Phieu chua luu: nut da disable — chan them o day phong khi goi qua phim tat/code
                if (!expMests.Any())
                    return;

                if (this.isAttachExpMestsMultiPatient)
                {
                    // Ban nhieu benh nhan: moi benh nhan 1 don rieng -> dinh kem tung phieu tai Danh sach xuat
                    ExpMestAttachFileProcessor.ShowMultiPatientWarning();
                    return;
                }

                this.btnAttachPrescription.Enabled = false;
                try
                {
                    if (expMests.Count == 1)
                    {
                        ExpMestAttachFileProcessor.ShowAttachList(expMests[0], this.roomId, null);
                    }
                    else
                    {
                        // 1 don -> nhieu phieu xuat (cung benh nhan): dinh kem cung bo tep cho tung phieu,
                        // ten van ban mac dinh dung ma cua TUNG phieu
                        var chosen = ExpMestAttachFileProcessor.ChooseFiles(null);
                        if (chosen != null && chosen.Count > 0)
                        {
                            ExpMestAttachFileProcessor.AttachPendingFiles(expMests, chosen);
                            ExpMestAttachFileProcessor.Release(chosen);
                        }
                    }
                }
                finally
                {
                    RefreshAttachPrescriptionButtonState();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Goi sau khi luu phieu (ProcessSave): luu thanh cong -> ghi nhan (cac) phieu vua luu, enable nut</summary>
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
                SetAttachExpMests(savedExpMests, this.isTwoPatient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
