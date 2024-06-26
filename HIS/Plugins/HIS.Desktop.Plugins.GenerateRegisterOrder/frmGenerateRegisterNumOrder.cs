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
                this.SetControlValue();
                this.GenerateControlByRegisterGate();
                WaitingManager.Hide();
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
                //Nếu phòng không có dịch vụ khám thì đưa ra thông báo khoong có dịch vụ nào.
                //Nếu có 1 dịch vụ khám thì hiển thị thông báo Bạn có chắc chắn muốn đăng ký khám
                //Nếu có nhiều dịch vụ thì hiển thị ra popup đẻ người dùng chọn dịch vụ để đăng kí
                WaitingManager.Show();
                this.resultRegister = null;
                HIS_REGISTER_GATE gate = (HIS_REGISTER_GATE)e.Item.Tag;
                CommonParam param = new CommonParam();
                bool success = false;

                HisRegisterReqSDO req = new HisRegisterReqSDO();
                req.RegisterGateId = gate.ID;

                V_HIS_REGISTER_REQ rs = new BackendAdapter(param).Post<V_HIS_REGISTER_REQ>("api/HisRegisterReq/CreateSdo", ApiConsumers.MosConsumer, req, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                if (rs != null)
                {
                    success = true;
                    this.resultRegister = rs;
                    e.Item.Elements[2].Text = rs.NUM_ORDER + "\n\n";
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
