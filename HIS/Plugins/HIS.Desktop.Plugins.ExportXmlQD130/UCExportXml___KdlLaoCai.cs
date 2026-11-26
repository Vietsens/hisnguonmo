/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Plugins.ExportXmlQD130.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Viec 62218/62313 - Gui du lieu kham benh, chua benh (22 truong XML8 QD 130) len
    /// Kho du lieu dung chung tinh Lao Cai.
    ///
    /// NUT RIENG, luong RIENG (chot 26/11/2026): khong di qua menu "Gui" / ProcessSyncTreatment,
    /// khong dung bo du lieu theo lo cua man hinh -> khong anh huong gui cong BHYT, CSDL 4750,
    /// Gui tu dong. Tu nap 3 nhom du lieu XML8 can (ho so, dien doi tuong, tre so sinh).
    /// Chi tao nut khi da khai khoa HIS.KDL_LAOCAI.CONNECTION_INFO (vien khac khong thay doi gi).
    /// </summary>
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        private SimpleButton btnSendKdlLaoCai;
        private bool isSendingKdlLaoCai = false;

        /// <summary>Goi trong Load cua man hinh. Chua khai khoa -> khong tao control nao.</summary>
        internal void InitKdlLaoCai()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(HisConfigCFG.KDL_LAOCAI__CONNECTION_INFO))
                {
                    return;
                }

                this.btnSendKdlLaoCai = new SimpleButton();
                this.btnSendKdlLaoCai.Name = "btnSendKdlLaoCai";
                this.btnSendKdlLaoCai.Text = "Gửi Kho DL Lào Cai";
                this.btnSendKdlLaoCai.ToolTip = "Gửi dữ liệu khám bệnh, chữa bệnh của các hồ sơ đang chọn lên Kho dữ liệu dùng chung tỉnh Lào Cai";
                this.btnSendKdlLaoCai.Click += btnSendKdlLaoCai_Click;

                this.layoutControl1.SuspendLayout();
                try
                {
                    //Chen vao cuoi hang nut (ben phai nut Gui) - cung cach nut MdInsight, khong them hang moi
                    DevExpress.XtraLayout.LayoutControlItem lci =
                        this.layoutControl1.AddItem(
                            string.Empty, this.btnSendKdlLaoCai, this.layoutControlItem26,
                            DevExpress.XtraLayout.Utils.InsertType.Right);
                    SetFixedSize(lci, 115);
                }
                finally
                {
                    this.layoutControl1.ResumeLayout(true);
                }
                LogSystem.Info("KdlLaoCai - Da tao nut Gui Kho DL Lao Cai tai man Xuat XML 130.");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private async void btnSendKdlLaoCai_Click(object sender, EventArgs e)
        {
            if (this.isSendingKdlLaoCai) return;
            try
            {
                KdlLaoCaiConfig config = new KdlLaoCaiConfig(HisConfigCFG.KDL_LAOCAI__CONNECTION_INFO);
                if (!config.IsValid)
                {
                    XtraMessageBox.Show("Cấu hình " + HisConfigCFG.HIS_KDL_LAOCAI__CONNECTION_INFO + " không hợp lệ: " + config.Error + "." + Environment.NewLine
                        + "Định dạng: ClientID;ClientSecret|MaCoSo;IdNguonDuLieu;ApiKey|MaCoSo;IdNguonDuLieu;ApiKey...",
                        Resources.ResourceMessageLang.ThongBao);
                    return;
                }

                List<V_HIS_TREATMENT_1> selected = this.listSelection != null ? this.listSelection.ToList() : new List<V_HIS_TREATMENT_1>();
                if (selected.Count == 0)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.BanChuaChonHoSoDeDongBo, Resources.ResourceMessageLang.ThongBao);
                    return;
                }

                string confirm = string.Format("Gửi {0} hồ sơ đã chọn lên Kho dữ liệu dùng chung tỉnh Lào Cai?", selected.Count)
                    + Environment.NewLine + "(Cơ sở đã khai báo: " + string.Join(", ", config.Facilities.Keys) + ")";
                if (XtraMessageBox.Show(confirm, Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }

                this.isSendingKdlLaoCai = true;
                this.btnSendKdlLaoCai.Enabled = false;
                List<KdlLaoCaiResultADO> results = null;
                try
                {
                    WaitingManager.Show();
                    results = await SendKdlLaoCaiAsync(config, selected);
                }
                finally
                {
                    WaitingManager.Hide();
                    this.isSendingKdlLaoCai = false;
                    this.btnSendKdlLaoCai.Enabled = true;
                }

                if (results != null)
                {
                    using (frmKdlLaoCaiResult frm = new frmKdlLaoCaiResult(results))
                    {
                        frm.ShowDialog(this.ParentForm);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                this.isSendingKdlLaoCai = false;
                if (this.btnSendKdlLaoCai != null) this.btnSendKdlLaoCai.Enabled = true;
                LogSystem.Error(ex);
            }
        }

        /// <summary>Nap du lieu, dung XML8 va gui tuan tu tung ho so. Chay ngoai luong giao dien.</summary>
        private Task<List<KdlLaoCaiResultADO>> SendKdlLaoCaiAsync(KdlLaoCaiConfig config, List<V_HIS_TREATMENT_1> selected)
        {
            bool is3176 = ReadXml3176CheckedSafe();
            return Task.Run(async () =>
            {
                List<KdlLaoCaiResultADO> results = new List<KdlLaoCaiResultADO>();
                List<long> ids = selected.Select(o => o.ID).Distinct().ToList();

                List<V_HIS_TREATMENT_12> treatments = new List<V_HIS_TREATMENT_12>();
                List<V_HIS_PATIENT_TYPE_ALTER> patientTypeAlters = new List<V_HIS_PATIENT_TYPE_ALTER>();
                List<V_HIS_BABY> babies = new List<V_HIS_BABY>();
                for (int skip = 0; skip < ids.Count; skip += GlobalVariables.MAX_REQUEST_LENGTH_PARAM)
                {
                    List<long> limit = ids.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();

                    CommonParam param = new CommonParam();
                    HisTreatmentView12Filter treatmentFilter = new HisTreatmentView12Filter();
                    treatmentFilter.IDs = limit;
                    var t = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_12>>("api/HisTreatment/GetView12", ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (t != null) treatments.AddRange(t);

                    param = new CommonParam();
                    HisPatientTypeAlterViewFilter ptaFilter = new HisPatientTypeAlterViewFilter();
                    ptaFilter.TREATMENT_IDs = limit;
                    var p = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, ptaFilter, param);
                    if (p != null) patientTypeAlters.AddRange(p);

                    param = new CommonParam();
                    HisBabyViewFilter babyFilter = new HisBabyViewFilter();
                    babyFilter.TREATMENT_IDs = limit;
                    var b = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, babyFilter, param);
                    if (b != null) babies.AddRange(b);
                }

                List<HIS_CONFIG> hisConfigs = GetNewConfig();
                List<HIS_EMPLOYEE> employees = BackendDataWorker.Get<HIS_EMPLOYEE>();
                Dictionary<long, V_HIS_TREATMENT_12> dicTreatment = treatments.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());
                KdlLaoCaiWorker worker = new KdlLaoCaiWorker(config);

                foreach (V_HIS_TREATMENT_1 sel in selected)
                {
                    KdlLaoCaiResultADO row = new KdlLaoCaiResultADO();
                    row.TreatmentCode = sel.TREATMENT_CODE;
                    row.PatientName = sel.TDL_PATIENT_NAME;
                    results.Add(row);
                    try
                    {
                        V_HIS_TREATMENT_12 treatment;
                        if (!dicTreatment.TryGetValue(sel.ID, out treatment) || treatment == null)
                        {
                            row.Status = KdlLaoCaiResultADO.STATUS__FAILED;
                            row.Message = "Không tải được thông tin hồ sơ điều trị";
                            continue;
                        }
                        if (!treatment.OUT_TIME.HasValue)
                        {
                            row.Status = KdlLaoCaiResultADO.STATUS__SKIPPED;
                            row.Message = "Hồ sơ chưa ra viện (chưa có ngày ra viện)";
                            continue;
                        }

                        //Dung input8 y nhu thu vien XML130 dung (His.Bhyt.ExportXml.XML130/HoSoProcessor.cs - XML8)
                        His.Bhyt.ExportXml.XML130.XML8.Base.InputADO input8 = new His.Bhyt.ExportXml.XML130.XML8.Base.InputADO();
                        input8.HisConfig = hisConfigs;
                        input8.vTreatment12 = treatment;
                        input8.HisBabys = babies.Where(o => o.TREATMENT_ID == treatment.ID).ToList();
                        input8.HisPatientTypeAlters = patientTypeAlters.Where(o => o.TREATMENT_ID == treatment.ID).ToList();
                        input8.Employees = employees;
                        input8.IS_3176 = is3176;
                        His.Bhyt.ExportXml.XML130.XML8.QD130.XML.XML8Data xml8 = new His.Bhyt.ExportXml.XML130.XML8.CreateXmlMain(input8).RunXml8Ado();
                        if (xml8 == null)
                        {
                            row.Status = KdlLaoCaiResultADO.STATUS__SKIPPED;
                            row.Message = "Không thuộc diện dữ liệu tóm tắt hồ sơ bệnh án (XML8): chỉ áp dụng điều trị nội trú, nội trú ban ngày, ngoại trú (không gồm bệnh mạn tính)";
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(treatment.HEIN_MEDI_ORG_CODE))
                        {
                            row.Status = KdlLaoCaiResultADO.STATUS__FAILED;
                            row.Message = "Hồ sơ chưa có mã cơ sở khám chữa bệnh";
                            continue;
                        }
                        KdlLaoCaiFacility facility = config.GetFacility(treatment.HEIN_MEDI_ORG_CODE);
                        if (facility == null)
                        {
                            row.Status = KdlLaoCaiResultADO.STATUS__FAILED;
                            row.Message = "Chưa khai báo kết nối Kho cho cơ sở " + treatment.HEIN_MEDI_ORG_CODE.Trim();
                            continue;
                        }

                        KdlLaoCaiPushResult push = await worker.PushAsync(facility, xml8, treatment.MODIFY_TIME, treatment.TREATMENT_CODE);
                        row.IdBanGhi = push.IdBanGhi;
                        row.Status = push.Success ? KdlLaoCaiResultADO.STATUS__SUCCESS : KdlLaoCaiResultADO.STATUS__FAILED;
                        row.Message = push.Message;
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Error(ex);
                        row.Status = KdlLaoCaiResultADO.STATUS__FAILED;
                        row.Message = ex.Message;
                    }
                }

                LogSystem.Info(string.Format("KdlLaoCai - Ket thuc gui. Tong: {0}; thanh cong: {1}; loi: {2}; bo qua: {3}",
                    results.Count,
                    results.Count(o => o.Status == KdlLaoCaiResultADO.STATUS__SUCCESS),
                    results.Count(o => o.Status == KdlLaoCaiResultADO.STATUS__FAILED),
                    results.Count(o => o.Status == KdlLaoCaiResultADO.STATUS__SKIPPED)));
                return results;
            });
        }
    }
}
