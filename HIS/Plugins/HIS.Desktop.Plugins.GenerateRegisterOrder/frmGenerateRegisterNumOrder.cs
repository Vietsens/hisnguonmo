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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.GenerateRegisterOrder.ADO;
using HIS.Desktop.Plugins.GenerateRegisterOrder.Config;
using HIS.Desktop.Plugins.GenerateRegisterOrder.Popup;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.GenerateRegisterOrder
{
    public partial class frmGenerateRegisterNumOrder : FormBase
    {
        private V_HIS_REGISTER_REQ resultRegister;
        private List<HisRegisterGateADO> lstRegister;
        private SettingADO ConfigSettings;
        private Delegates.FormClosedDelegate formClosedDelegate;

        /// <summary>
        /// Thong tin dinh danh nguoi benh dang giu, doc tu popup chon hinh thuc lay so.
        /// Rong khi chua dinh danh hoac khi nguoi benh chon khong co giay to.
        /// </summary>
        private IdentityInfoADO currentIdentity;

        /// <summary>Nguoi benh da chon duong lay so khong can giay to</summary>
        private bool isNoPaperChosen;

        public frmGenerateRegisterNumOrder(Inventec.Desktop.Common.Modules.Module module, List<HisRegisterGateADO> lstAdo, SettingADO setting)
            : base(module)
        {
            InitializeComponent();
            try
            {
                ConfigSettings = setting;
                lstRegister = lstAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmGenerateRegisterNumOrder(Inventec.Desktop.Common.Modules.Module module, List<HisRegisterGateADO> lstAdo, SettingADO setting, Delegates.FormClosedDelegate formClosed)
            : this(module, lstAdo, setting)
        {
            try
            {
                this.formClosedDelegate = formClosed;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmGenerateRegisterNumOrder_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                this.SetControlValue();
                this.GenerateControlByRegisterGate();
                this.SetIdentityBar();
                WaitingManager.Hide();

                // Bat popup sau khi Load xong de khong mo hop thoai trong luc form dang dung len
                if (HisConfigCFG.IsIssueWithIdentity)
                {
                    this.BeginInvoke(new MethodInvoker(delegate() { this.ShowIdentityPopup(); }));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlValue()
        {
            try
            {
                string branchName = WorkPlace.GetBranchName() ?? "";
                lblTitlePage.Text = branchName.ToUpper();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GenerateControlByRegisterGate()
        {
            try
            {
                WaitingManager.Show();
                HisRegisterGateFilter filter = new HisRegisterGateFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                List<HIS_REGISTER_GATE> registerGates = new BackendAdapter(new CommonParam()).Get<List<HIS_REGISTER_GATE>>("api/HisRegisterGate/Get", ApiConsumers.MosConsumer, filter, null);

                if (registerGates != null && registerGates.Count > 0)
                {
                    registerGates = registerGates.OrderBy(o => o.REGISTER_GATE_CODE).ToList();
                    registerGates = registerGates.Where(o => lstRegister.Exists(p => p.REGISTER_GATE_CODE == o.REGISTER_GATE_CODE)).ToList();
                    //long WidthControl = 0;
                    //long HeightControl = 0;
                    //if (ConfigSettings != null)
                    //{
                    //    if (ConfigSettings.Columns <= lstRegister.Count)
                    //    {
                    //        WidthControl = ConfigSettings.Columns * ConfigSettings.SizeItem + 50;
                    //    }
                    //    else
                    //    {
                    //        WidthControl = lstRegister.Count * ConfigSettings.SizeItem + 50;
                    //    }
                    //    var wC = registerGates.Count / ConfigSettings.Columns;
                    //    if (wC.GetType() == typeof(Int32))
                    //    {
                    //        HeightControl = wC * ConfigSettings.SizeItem;
                    //    }
                    //    else
                    //    {
                    //        HeightControl = (int)(wC + 1) * ConfigSettings.SizeItem + 50;
                    //    }
                    //}

                    //flowLayoutPanel1.Size = new System.Drawing.Size((int)WidthControl, (int)HeightControl);
                    //flowLayoutPanel1.AutoScrollPosition = new Point(0);
                    //int x = (panel1.Size.Width - flowLayoutPanel1.Size.Width) / 2;
                    //int y = (panel1.Size.Height - flowLayoutPanel1.Size.Height) / 2;
                    //flowLayoutPanel1.Location = new Point(x, y);

                    //foreach (var gate in registerGates)
                    //{
                    //    var num = lstRegister.Where(o => o.REGISTER_GATE_CODE == gate.REGISTER_GATE_CODE).First().BEGIN_NUM_ORDER;
                    //    UCItem item = new UCItem(registerGates, lstRegister);
                    //    item.TextStt = (num == 0 ? num : num - 1) + "";
                    //    item.TextTitle = gate.REGISTER_GATE_NAME;
                    //    item.SizeStt = ConfigSettings.SizeStt;
                    //    item.SizeTitle = ConfigSettings.SizeTitle;
                    //    item.Size = new System.Drawing.Size((int)ConfigSettings.SizeItem, (int)ConfigSettings.SizeItem);
                    //    item.Tag = gate;
                    //    item._Click += item__Click;
                    //    flowLayoutPanel1.Controls.Add(item);
                    //}
                    //WaitingManager.Hide();

                    var group = new TileGroup();
                    group.Text = "";
                    foreach (HIS_REGISTER_GATE gate in registerGates)
                    {


                        var num = lstRegister.Where(o => o.REGISTER_GATE_CODE == gate.REGISTER_GATE_CODE).First().BEGIN_NUM_ORDER;
                        TileItem tileNew = new TileItem();
                        tileNew.Elements = new TileItemElementCollection(tileNew);
                        TileItemElement title = new TileItemElement();
                        title.Text = "\n" + gate.REGISTER_GATE_NAME;
                        title.Appearance.Normal.Font = new System.Drawing.Font(title.Appearance.Normal.Font.FontFamily, ConfigSettings.SizeTitle, FontStyle.Bold);
                        title.TextAlignment = TileItemContentAlignment.TopCenter;
                        tileNew.Elements.Add(title);

                        TileItemElement content = new TileItemElement();
                        content.Text = "STT hiện tại:";
                        content.Appearance.Normal.Font = new System.Drawing.Font(title.Appearance.Normal.Font.FontFamily, ConfigSettings.SizeStt, FontStyle.Regular);
                        content.TextAlignment = TileItemContentAlignment.MiddleCenter;
                        tileNew.Elements.Add(content);

                        TileItemElement cnum = new TileItemElement();
                        cnum.Text = (num == 0 ? num : num - 1) + "\n\n";
                        cnum.Appearance.Normal.Font = new System.Drawing.Font(title.Appearance.Normal.Font.FontFamily, ConfigSettings.SizeStt, FontStyle.Regular);
                        cnum.TextAlignment = TileItemContentAlignment.BottomCenter;
                        tileNew.Elements.Add(cnum);
                        //tileNew.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
                        //tileNew.Text = "<h1>" + gate.REGISTER_GATE_NAME + "</h1>" + "\n\nSTT hiện tại:\n" +
                        //    (num == 0 ? num : num - 1);
                        //tileNew.AppearanceItem.Normal.FontSizeDelta = 4;
                        //tileNew.AppearanceItem.Normal.ForeColor = Color.White;
                        //System.Drawing.Font f = tileNew.AppearanceItem.Normal.Font;
                        //tileNew.AppearanceItem.Normal.Font = new System.Drawing.Font(tileNew.AppearanceItem.Normal.Font.FontFamily, tileNew.AppearanceItem.Normal.Font.Size, FontStyle.Bold);

                        //tileNew.TextAlignment = TileItemContentAlignment.MiddleCenter;
                        tileNew.ItemSize = TileItemSize.Medium;
                        tileNew.Tag = gate;

                        //System.Threading.Thread.Sleep(10);
                        //tileNew.AppearanceItem.Normal.BorderColor = Color.FromArgb(0, 162, 232);
                        tileNew.Checked = false;
                        tileNew.Visible = true;
                        tileNew.ItemClick += ItemClick;

                        //tileNew.AppearanceItem.Normal.BackColor = Color.FromArgb(0, 162, 232);
                        group.Items.Add(tileNew);
                    }
                    tileControlRegisterGate.ColumnCount = (int)ConfigSettings.Columns;
                    tileControlRegisterGate.ItemSize = (int)ConfigSettings.SizeItem;
                    tileControlRegisterGate.Groups.Add(group);
                }
            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        void item__Click(object sender, EventArgs e)
        {
            try
            {
                Label item = sender as Label;
                if (item != null && item.Tag != null)
                {

                    HIS_REGISTER_GATE dt = item.Tag as HIS_REGISTER_GATE;
                    WaitingManager.Show();
                    this.resultRegister = null;
                    CommonParam param = new CommonParam();
                    bool success = false;

                    HisRegisterReqSDO req = new HisRegisterReqSDO();
                    req.RegisterGateId = dt.ID;

                    V_HIS_REGISTER_REQ rs = new BackendAdapter(param).Post<V_HIS_REGISTER_REQ>("api/HisRegisterReq/CreateSdo", ApiConsumers.MosConsumer, req, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                    if (rs != null)
                    {
                        success = true;
                        this.resultRegister = rs;
                        item.Text = rs.NUM_ORDER.ToString();
                        //                       e.Item.Text = e.Item.Text.Substring(0, e.Item.Text.LastIndexOf(":")) + ":\n" + rs.NUM_ORDER;
                    }
                    WaitingManager.Hide();
                    if (!success)
                    {
                        MessageManager.Show(this, param, success);
                    }
                    else
                    {
                        this.PrintMps138();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                HIS_REGISTER_GATE gate = (HIS_REGISTER_GATE)e.Item.Tag;

                // Vien khong bat tinh nang thi giu nguyen hoan toan luong cap so hien tai
                if (!HisConfigCFG.IsIssueWithIdentity)
                {
                    this.IssueNumOrderDefault(gate, e.Item);
                    return;
                }

                // Chua dinh danh va cung chua chon duong khong giay to thi bat chon truoc
                if (!this.isNoPaperChosen && (this.currentIdentity == null || !this.currentIdentity.HasIdentity()))
                {
                    this.ShowIdentityPopup();
                    return;
                }

                if (this.isNoPaperChosen)
                {
                    this.IssueNumOrderDefault(gate, e.Item);
                    this.ShowIdentityPopup();
                    return;
                }

                this.IssueNumOrderWithIdentity(gate, e.Item);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Cap so theo dung cach hien tai, khong kem dinh danh.
        /// Dung cho vien khong bat tinh nang va cho duong khong co giay to.
        /// </summary>
        private void IssueNumOrderDefault(HIS_REGISTER_GATE gate, TileItem tileItem)
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                this.resultRegister = null;
                bool success = false;

                HisRegisterReqSDO req = new HisRegisterReqSDO();
                req.RegisterGateId = gate.ID;

                V_HIS_REGISTER_REQ rs = new BackendAdapter(param).Post<V_HIS_REGISTER_REQ>("api/HisRegisterReq/CreateSdo", ApiConsumers.MosConsumer, req, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                if (rs != null)
                {
                    success = true;
                    this.resultRegister = rs;
                    tileItem.Elements[2].Text = rs.NUM_ORDER + "\n\n";
                }
                WaitingManager.Hide();
                if (!success)
                {
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    this.PrintMps138();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Cap so kem dinh danh nguoi benh. May chu se quy dinh danh ve mot nguoi,
        /// dem so luot da cap trong ngay tren toan vien roi quyet dinh co cap tiep hay khong.
        /// Thiet ke: PTTK_54254 muc B.3.1.1.
        /// </summary>
        private void IssueNumOrderWithIdentity(HIS_REGISTER_GATE gate, TileItem tileItem)
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                this.resultRegister = null;

                HisRegisterReqWithIdentitySDO req = new HisRegisterReqWithIdentitySDO();
                req.RegisterGateId = gate.ID;
                req.IdentityType = this.currentIdentity.IdentityType;
                req.IdentityNumber = this.currentIdentity.IdentityNumber;
                req.IdentityJson = this.currentIdentity.ToJson();

                HisRegisterReqWithIdentityResultSDO rs = new BackendAdapter(param)
                    .Post<HisRegisterReqWithIdentityResultSDO>("api/HisRegisterReq/CreateWithIdentity", ApiConsumers.MosConsumer, req, param);

                WaitingManager.Hide();

                if (rs == null)
                {
                    MessageManager.Show(this, param, false);
                    return;
                }

                if (rs.IsLimitReached)
                {
                    // Da du luot trong ngay: khong cap so moi, hien lai so cu va cho in lai phieu
                    this.resultRegister = rs.RegisterReq;
                    this.ShowLimitReachedMessage(rs);
                    return;
                }

                if (rs.RegisterReq == null)
                {
                    MessageManager.Show(this, param, false);
                    return;
                }

                this.resultRegister = rs.RegisterReq;
                tileItem.Elements[2].Text = rs.RegisterReq.NUM_ORDER + "\n\n";
                this.PrintMps138();

                // Xoa thong tin dang giu roi hien lai popup cho nguoi ke tiep
                this.ShowIdentityPopup();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Dinh danh nguoi benh

        /// <summary>
        /// Mo popup chon hinh thuc lay so. Goi khi mo man, sau moi luot cap so,
        /// va khi nguoi benh nhan huy de chon lai.
        /// </summary>
        private void ShowIdentityPopup()
        {
            try
            {
                this.ClearLimitMessage();
                this.currentIdentity = null;
                this.isNoPaperChosen = false;

                using (frmChooseIdentity frm = new frmChooseIdentity())
                {
                    frm.ShowDialog(this);
                    if (frm.DialogResult == DialogResult.OK)
                    {
                        this.currentIdentity = frm.Identity;
                        this.isNoPaperChosen = frm.IsNoPaper;
                    }
                }

                this.SetIdentityBar();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Cap nhat dai thong tin phia duoi tieu de man lay so.
        /// Vien khong bat tinh nang thi an han dai nay di.
        /// </summary>
        private void SetIdentityBar()
        {
            try
            {
                if (!HisConfigCFG.IsIssueWithIdentity)
                {
                    this.lciIdentityBar.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    return;
                }

                this.lciIdentityBar.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.lblIdentityInfo.Appearance.ForeColor = Color.FromArgb(0, 100, 150);
                this.lblIdentityInfo.Appearance.Options.UseForeColor = true;
                this.btnReprint.Visible = false;

                if (this.isNoPaperChosen)
                {
                    this.lblIdentityInfo.Text = Resources.ResourceMessage.LaySoKhongDinhDanh;
                    this.btnCancelIdentity.Visible = true;
                    return;
                }

                if (this.currentIdentity != null && this.currentIdentity.HasIdentity())
                {
                    string name = String.IsNullOrWhiteSpace(this.currentIdentity.PatientName)
                        ? ""
                        : this.currentIdentity.PatientName + " - ";
                    this.lblIdentityInfo.Text = Resources.ResourceMessage.DangLaySoCho
                        + ": " + name + this.currentIdentity.GetMaskedNumber();
                    this.btnCancelIdentity.Visible = true;
                    return;
                }

                this.lblIdentityInfo.Text = Resources.ResourceMessage.VuiLongChonHinhThucLaySo;
                this.btnCancelIdentity.Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hien thong bao da lay du so trong ngay kem so thu tu va ten day da lay truoc do.
        /// Sau mot khoang cho ngan tu quay ve popup cho nguoi ke tiep.
        /// </summary>
        private void ShowLimitReachedMessage(HisRegisterReqWithIdentityResultSDO rs)
        {
            try
            {
                string numOrder = "";
                string gateName = rs.PreviousGateName ?? "";
                if (rs.RegisterReq != null)
                {
                    numOrder = rs.RegisterReq.NUM_ORDER.ToString();
                    if (String.IsNullOrWhiteSpace(gateName))
                    {
                        gateName = rs.RegisterReq.REGISTER_GATE_NAME ?? "";
                    }
                }

                this.lblIdentityInfo.Appearance.ForeColor = Color.Red;
                this.lblIdentityInfo.Appearance.Options.UseForeColor = true;
                this.lblIdentityInfo.Text = String.Format(
                    Resources.ResourceMessage.BanDaLayDuSoTrongNgay, numOrder, gateName);

                this.btnCancelIdentity.Visible = true;
                this.btnReprint.Visible = (this.resultRegister != null);
                this.tmrAutoReset.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClearLimitMessage()
        {
            try
            {
                this.tmrAutoReset.Enabled = false;
                this.btnReprint.Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancelIdentity_Click(object sender, EventArgs e)
        {
            try
            {
                this.ShowIdentityPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.resultRegister == null)
                {
                    return;
                }
                this.PrintMps138();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmrAutoReset_Tick(object sender, EventArgs e)
        {
            try
            {
                this.tmrAutoReset.Enabled = false;
                this.ShowIdentityPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void PrintMps138()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate("Mps000138", DelegateRunPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultRegister == null)
                {
                    return false;
                }

                HisRegisterReqViewFilter filter = new HisRegisterReqViewFilter();
                filter.CALL_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd000000"));
                filter.REGISTER_GATE_ID = this.resultRegister.REGISTER_GATE_ID;
                var dataPrint = new BackendAdapter(new CommonParam()).Get<List<V_HIS_REGISTER_REQ>>("api/HisRegisterReq/GetView", ApiConsumers.MosConsumer, filter, null);

                MPS.Processor.Mps000138.PDO.Mps000138PDO rdo = new MPS.Processor.Mps000138.PDO.Mps000138PDO(this.resultRegister, dataPrint != null ? dataPrint.OrderByDescending(o => o.NUM_ORDER).ThenByDescending(o => o.REGISTER_TIME).FirstOrDefault() : null);

                result = MPS.MpsPrinter.Run(new PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void frmGenerateRegisterNumOrder_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (this.formClosedDelegate != null)
                {
                    this.formClosedDelegate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
