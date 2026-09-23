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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using His.Bhyt.ExportXml.XML130;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Plugins.ExportXmlQD130.Base;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// vCong XXXXX - Doi chieu ho so XML voi he thong soat loi MDInsight tai man Xuat XML 130.
    ///
    /// CHI CANH BAO, KHONG CHAN: khong chen buoc kiem tra nao vao truoc nam chuc nang ket xuat,
    /// khong chan bat ky thao tac nao ke ca khi ho so co loi nghiem trong (quy tac QT-02).
    ///
    /// Hai nut, dung chung mot bo quy tac suy trang thai:
    ///  - "Kiem tra loi XML": ket xuat tep -> gui len he ngoai -> cho -> tra ket qua MOT vong.
    ///  - "Lay ket qua": khong gui tep, chi tra lai ket qua cua lan gui gan nhat.
    ///
    /// Tinh nang MAC DINH TAT. Nut va cot chi duoc tao khi vien da khai du dia chi va it nhat
    /// mot doan tai khoan (quy tac QT-01) - vien khong dung thi man hinh khong phat sinh control nao.
    ///
    /// Tham chieu: PTTK muc B.4.1.1.
    /// </summary>
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        #region Khai bao

        /// <summary>Nguong so ho so vuot qua thi hoi xac nhan - quy tac QT-04, khong cau hinh</summary>
        private const int MDINSIGHT_CONFIRM_THRESHOLD = 50;

        /// <summary>Han muc ca vong tra, tinh bang giay - quy tac QT-10, khong cau hinh</summary>
        private const int MDINSIGHT_ROUND_LIMIT_SECOND = 180;

        /// <summary>
        /// So thieu sot toi da ghep vao cot Ly do khi ho so khong ket xuat duoc.
        /// Ho so hong nang co the thieu hang chuc truong - ghep het se tran o luoi
        /// va an het cot XML_PRECHECK_DESC (gioi han 4000 byte).
        /// </summary>
        private const int MAX_EXPORT_REASON = 4;

        /// <summary>Cu tra xong bao nhieu ho so thi luu mot lan - quy tac QT-11</summary>
        private const int MDINSIGHT_SAVE_BATCH = 10;

        /// <summary>So giay uoc tinh cho moi ho so khi bao thoi gian du kien</summary>
        private const int MDINSIGHT_SECOND_PER_TREATMENT = 2;

        private MdInsightConfig mdInsightConfig;

        /// <summary>
        /// Ket qua trong phien lam viec, khoa theo ma dot dieu tri.
        /// Chi o day moi co phan DIEN GIAI DAY DU - truong do khong luu xuong CSDL (quy tac QT-28).
        /// </summary>
        private readonly Dictionary<long, MdInsightResultADO> mdInsightSessionResults
            = new Dictionary<long, MdInsightResultADO>();

        private SimpleButton btnMdInsightCheck;
        private GridColumn gridColMdInsightResult;
        private GridColumn gridColMdInsightErrNum;
        private GridColumn gridColMdInsightTime;

        /// <summary>Da khai du cau hinh de bat tinh nang chua - quy tac QT-01</summary>
        private bool IsMdInsightConfigured
        {
            get { return this.mdInsightConfig != null && this.mdInsightConfig.IsValidConfig; }
        }

        #endregion

        #region Khoi tao giao dien

        /// <summary>
        /// Doc cau hinh, va CHI KHI da khai du thi moi tao nut va cot.
        /// Goi trong su kien Load cua man hinh.
        ///
        /// Tao luc chay thay vi khai trong Designer la CO CHU Y: hang nut cua man hinh nay da kin
        /// chieu ngang, chen them vao Designer se xo lech thanh cong cu cua moi vien - ke ca vien
        /// khong dung tinh nang. Tao luc chay thi vien chua khai cau hinh khong phat sinh control nao.
        /// </summary>
        internal void InitMdInsight()
        {
            try
            {
                this.mdInsightConfig = new MdInsightConfig(
                    HisConfigCFG.MDINSIGHT__CONNECTION_INFO);

                if (!this.IsMdInsightConfigured)
                {
                    //Vien chua dau noi - man hinh giu nguyen y het hien tai
                    return;
                }

                CreateMdInsightButtons();
                CreateMdInsightColumns();

                LogSystem.Info("MdInsight - Da kich hoat tinh nang soat loi XML tai man Xuat XML 130.");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void CreateMdInsightButtons()
        {
            try
            {
                this.btnMdInsightCheck = new SimpleButton();
                this.btnMdInsightCheck.Name = "btnMdInsightCheck";
                this.btnMdInsightCheck.Text = Resources.ResourceMessageLang.MdInsightNutKiemTraLoi;
                //Nut hep nen chu co the bi cat - chu thich day du dat o goi y (ui_rules muc 3)
                this.btnMdInsightCheck.ToolTip = Resources.ResourceMessageLang.MdInsightNutKiemTraLoi;
                this.btnMdInsightCheck.Click += btnMdInsightCheck_Click;

                this.layoutControl1.SuspendLayout();
                try
                {
                    //Chen vao CUOI hang nut san co (ben phai nut Gui) chu KHONG them hang moi.
                    //Hang do xep kin 1088px nhung phan lon cac muc khong ghim kich thuoc, nen
                    //DevExpress se tu co chung lai de lay cho - dung y nguoi dung, chot 2026-09-15.
                    DevExpress.XtraLayout.LayoutControlItem lciCheck =
                        this.layoutControl1.AddItem(
                            string.Empty, this.btnMdInsightCheck, this.layoutControlItem26,
                            DevExpress.XtraLayout.Utils.InsertType.Right);
                    SetFixedSize(lciCheck, 130);
                }
                finally
                {
                    this.layoutControl1.ResumeLayout(true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ghim kich thuoc mot muc bo cuc: rong dung <paramref name="width"/>, cao 26.
        /// Chieu ngang cua MaxSize PHAI khac 0 - de 0 la DevExpress hieu "khong gioi han"
        /// va nut se gian kin ca hang.
        /// </summary>
        private static void SetFixedSize(DevExpress.XtraLayout.LayoutControlItem item, int width)
        {
            item.TextVisible = false;
            item.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            item.MinSize = new Size(width, 26);
            item.MaxSize = new Size(width, 26);
        }

        /// <summary>
        /// Them ba cot ket qua vao cuoi luoi. Luoi chi doc BON thong tin nhe
        /// (trang thai, tong so loi, so loi nghiem trong, thoi diem kiem tra) - PTTK muc B.2.4.
        /// Chi tiet loi KHONG nap cung luoi.
        /// </summary>
        private void CreateMdInsightColumns()
        {
            try
            {
                this.gridColMdInsightResult = this.gridViewTreatment.Columns.AddField("XML_PRECHECK_RESULT");
                this.gridColMdInsightResult.Caption = Resources.ResourceMessageLang.MdInsightCotKetQua;
                this.gridColMdInsightResult.Name = "gridColMdInsightResult";
                this.gridColMdInsightResult.OptionsColumn.AllowEdit = false;
                this.gridColMdInsightResult.Width = 140;
                this.gridColMdInsightResult.Visible = true;
                this.gridColMdInsightResult.VisibleIndex = this.gridViewTreatment.Columns.Count;

                this.gridColMdInsightErrNum = this.gridViewTreatment.Columns.AddField("XML_PRECHECK_ERR_NUM");
                this.gridColMdInsightErrNum.Caption = Resources.ResourceMessageLang.MdInsightCotSoLoi;
                this.gridColMdInsightErrNum.Name = "gridColMdInsightErrNum";
                this.gridColMdInsightErrNum.OptionsColumn.AllowEdit = false;
                this.gridColMdInsightErrNum.Width = 90;
                this.gridColMdInsightErrNum.Visible = true;
                this.gridColMdInsightErrNum.VisibleIndex = this.gridViewTreatment.Columns.Count;

                this.gridColMdInsightTime = this.gridViewTreatment.Columns.AddField("XML_PRECHECK_TIME");
                this.gridColMdInsightTime.Caption = Resources.ResourceMessageLang.MdInsightCotThoiDiem;
                this.gridColMdInsightTime.Name = "gridColMdInsightTime";
                this.gridColMdInsightTime.OptionsColumn.AllowEdit = false;
                this.gridColMdInsightTime.Width = 130;
                this.gridColMdInsightTime.Visible = true;
                this.gridColMdInsightTime.VisibleIndex = this.gridViewTreatment.Columns.Count;

                //Dang ky them - khong go bo trinh xu ly nao san co cua man hinh
                this.gridViewTreatment.CustomColumnDisplayText += GridViewTreatment_MdInsightDisplayText;
                this.gridViewTreatment.RowCellStyle += GridViewTreatment_MdInsightRowCellStyle;
                this.gridViewTreatment.RowCellClick += GridViewTreatment_MdInsightRowCellClick;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Doi ma trang thai thanh chu, va ghep so loi nghiem trong vao cot so loi</summary>
        private void GridViewTreatment_MdInsightDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column == null)
                {
                    return;
                }

                if (e.Column == this.gridColMdInsightResult)
                {
                    e.DisplayText = GetMdInsightStatusName(e.Value as short?);
                    return;
                }

                if (e.Column == this.gridColMdInsightTime)
                {
                    long? time = e.Value as long?;
                    e.DisplayText = time.HasValue && time.Value > 0
                        ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(time.Value)
                        : "";
                    return;
                }

                if (e.Column == this.gridColMdInsightErrNum)
                {
                    long? errNum = e.Value as long?;
                    if (!errNum.HasValue || errNum.Value <= 0)
                    {
                        //Khong co loi thi de trong, khong hien so 0
                        e.DisplayText = "";
                        return;
                    }

                    V_HIS_TREATMENT_1 row = this.gridViewTreatment.GetRow(e.ListSourceRowIndex) as V_HIS_TREATMENT_1;
                    long crtNum = row != null && row.XML_PRECHECK_CRT_NUM.HasValue ? row.XML_PRECHECK_CRT_NUM.Value : 0;

                    e.DisplayText = crtNum > 0
                        ? errNum.Value + " (" + crtNum + ")"
                        : errNum.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>To mau cot ket qua theo muc do - PTTK muc B.4.1.1</summary>
        private void GridViewTreatment_MdInsightRowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column != this.gridColMdInsightResult)
                {
                    return;
                }

                V_HIS_TREATMENT_1 row = this.gridViewTreatment.GetRow(e.RowHandle) as V_HIS_TREATMENT_1;
                if (row == null || !row.XML_PRECHECK_RESULT.HasValue)
                {
                    return;
                }

                switch ((EnumXmlPrecheckStatus)row.XML_PRECHECK_RESULT.Value)
                {
                    case EnumXmlPrecheckStatus.NoError:
                        e.Appearance.ForeColor = Color.Green;
                        break;
                    case EnumXmlPrecheckStatus.Warning:
                        e.Appearance.ForeColor = Color.DarkOrange;
                        break;
                    case EnumXmlPrecheckStatus.Critical:
                        e.Appearance.ForeColor = Color.Red;
                        break;
                    default:
                        e.Appearance.ForeColor = Color.Gray;
                        break;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Bam vao o ket qua thi mo cua so chi tiet loi cua dung ho so do</summary>
        private void GridViewTreatment_MdInsightRowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column != this.gridColMdInsightResult || e.RowHandle < 0)
                {
                    return;
                }

                V_HIS_TREATMENT_1 row = this.gridViewTreatment.GetRow(e.RowHandle) as V_HIS_TREATMENT_1;
                if (row == null || !row.XML_PRECHECK_RESULT.HasValue)
                {
                    return;
                }

                ShowMdInsightResultWindow(LoadResultsForDisplay(new List<V_HIS_TREATMENT_1> { row }));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        internal string GetMdInsightStatusName(short? status)
        {
            if (!status.HasValue)
            {
                return "";
            }

            switch ((EnumXmlPrecheckStatus)status.Value)
            {
                case EnumXmlPrecheckStatus.NoError:
                    return Resources.ResourceMessageLang.MdInsightTrangThaiKhongLoi;
                case EnumXmlPrecheckStatus.Warning:
                    return Resources.ResourceMessageLang.MdInsightTrangThaiCanhBao;
                case EnumXmlPrecheckStatus.Critical:
                    return Resources.ResourceMessageLang.MdInsightTrangThaiLoiNghiemTrong;
                case EnumXmlPrecheckStatus.Pending:
                    return Resources.ResourceMessageLang.MdInsightTrangThaiChuaCoKetQua;
                case EnumXmlPrecheckStatus.CheckFailed:
                    return Resources.ResourceMessageLang.MdInsightTrangThaiKhongKiemTraDuoc;
                default:
                    return "";
            }
        }

        #endregion

        #region Hai nut

        /// <summary>
        /// MOT nut duy nhat. Phan mem tu quyet dinh gui hay chi tra theo trang thai TUNG ho so -
        /// nguoi dung khong phai chon ho (chot ngay 2026-09-16).
        /// </summary>
        private async void btnMdInsightCheck_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_TREATMENT_1> selected = GetMdInsightSelection();
                if (selected == null)
                {
                    return;
                }

                List<MdInsightResultADO> results = await RunMdInsightAsync(selected);
                FinishMdInsightRun(results);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Lay danh sach ho so da tich chon. Tra ve null khi chua chon gi - quy tac QT-03</summary>
        private List<V_HIS_TREATMENT_1> GetMdInsightSelection()
        {
            if (this.listSelection == null || this.listSelection.Count == 0)
            {
                XtraMessageBox.Show(
                    Resources.ResourceMessageLang.MdInsightChuaChonHoSo,
                    Resources.ResourceMessageLang.ThongBao,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            return this.listSelection.ToList();
        }

        /// <summary>
        /// Hoi xac nhan khi lo lon - quy tac QT-04.
        /// Thoi gian du kien theo cong thuc tai PTTK muc B.2.5.4; vuot han muc ca vong tra thi
        /// hien dung han muc do kem cau nhac, khong hua mot con so khong bao gio xay ra.
        /// </summary>
        private bool ConfirmMdInsightBatch(int count, bool isSendFlow)
        {
            if (count <= MDINSIGHT_CONFIRM_THRESHOLD)
            {
                return true;
            }

            int estimateSecond = count * MDINSIGHT_SECOND_PER_TREATMENT;
            if (isSendFlow)
            {
                estimateSecond += this.mdInsightConfig.WaitAfterUploadSecond;
            }

            bool overLimit = estimateSecond > MDINSIGHT_ROUND_LIMIT_SECOND;
            int shownSecond = overLimit ? MDINSIGHT_ROUND_LIMIT_SECOND : estimateSecond;
            string shownTime = ((int)Math.Ceiling(shownSecond / 60.0)) + " phút";

            string message = string.Format(
                overLimit
                    ? Resources.ResourceMessageLang.MdInsightXacNhanLoLonVuotHan
                    : Resources.ResourceMessageLang.MdInsightXacNhanLoLon,
                count, shownTime);

            return XtraMessageBox.Show(message, Resources.ResourceMessageLang.ThongBao,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>Cap nhat luoi va mo cua so ket qua sau moi luot chay</summary>
        private void FinishMdInsightRun(List<MdInsightResultADO> results)
        {
            try
            {
                if (results == null || results.Count == 0)
                {
                    return;
                }

                ApplyResultsToGrid(results);
                ShowMdInsightResultWindow(results);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
