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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using His.Bhyt.InsuranceExpertise.LDO;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.UCCheckTT
{
    public partial class UCCheckTT : UserControl
    {
        Dictionary<string, HIS_MEDI_ORG> dicMediOrg;
        ResultHistoryLDO _resultHistoryLDO;

        public UCCheckTT()
        {
            InitializeComponent();
            SetCaptionByLanguageKey();
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCCheckTT
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCCheckTT.Resources.Lang", typeof(UCCheckTT).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCCheckTT.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnPrintf.Text = Inventec.Common.Resource.Get.Value("UCCheckTT.btnPrintf.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintf.ToolTip = Inventec.Common.Resource.Get.Value("UCCheckTT.btnPrintf.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCCheckTT.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCCheckTT.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCCheckTT.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCCheckTT.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCCheckTT.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCCheckTT.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCCheckTT.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("UCCheckTT.layoutControlGroup1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCCheckTT.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCCheckTT.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDiaChi.Text = Inventec.Common.Resource.Get.Value("UCCheckTT.lciDiaChi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void DisposeControl()
        {
            try
            {
                _resultHistoryLDO = null;
                dicMediOrg = null;
                this.btnPrintf.Click -= new System.EventHandler(this.btnPrintf_Click);
                this.gridViewHistory.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewHistory_CustomUnboundColumnData);
                this.repositoryItemButton__View.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton__View_ButtonClick);
                this.Load -= new System.EventHandler(this.UCCheckTT_Load);
                gridViewHistory.GridControl.DataSource = null;
                gridControlHistory.DataSource = null;
                emptySpaceItem2 = null;
                layoutControlItem4 = null;
                btnPrintf = null;
                repositoryItemButton__View = null;
                gridColumn1 = null;
                layoutControlItem3 = null;
                gridColumn8 = null;
                gridColumn6 = null;
                gridColumn7 = null;
                gridColumn5 = null;
                gridColumn4 = null;
                gridColumn3 = null;
                gridViewHistory = null;
                gridControlHistory = null;
                lciDiaChi = null;
                lblNgayDu5Nam = null;
                layoutControlItem2 = null;
                lblThoiHanThe = null;
                layoutControlItem1 = null;
                lblKetQua = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void ResetData()
        {
            lblNgayDu5Nam.Text = "";
            lblKetQua.Text = "";
            lblThoiHanThe.Text = "";
            gridControlHistory.DataSource = null;
        }

        public void FillDataIntoUCCheckTT(ResultHistoryLDO resultHistoryLDO)
        {
            try
            {
                this._resultHistoryLDO = resultHistoryLDO;
                lblNgayDu5Nam.Text = resultHistoryLDO.ngayDu5Nam;
                lblKetQua.Text = "Thẻ hợp lệ";
                lblThoiHanThe.Text = resultHistoryLDO.gtTheTu + " - " + resultHistoryLDO.gtTheDen;
                LoadDataGridControl(resultHistoryLDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ResetDataControl(ResultHistoryLDO resultHistoryLDO)
        {
            lblNgayDu5Nam.Text = resultHistoryLDO.ngayDu5Nam;
            lblKetQua.Text = resultHistoryLDO.message;
            lblThoiHanThe.Text = resultHistoryLDO.gtTheTu + " - " + resultHistoryLDO.gtTheDen;
            LoadDataGridControl(resultHistoryLDO);
        }

        private void LoadDataGridControl(ResultHistoryLDO _resultHistoryLDO)
        {
            try
            {
                gridControlHistory.DataSource = null;
                if (_resultHistoryLDO.dsLichSuKCB2018 != null)
                {
                    var query = _resultHistoryLDO.dsLichSuKCB2018.OrderByDescending(o => o.ngayRa).AsQueryable();
                    if (_resultHistoryLDO.dsLichSuKCB2018.Count > 5)
                    {
                        query = query.Skip(0).Take(5);
                    }

                    gridControlHistory.DataSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHistory_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExamHistoryLDO data = (ExamHistoryLDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "tinhTrang_str")
                        {
                            if (data.tinhTrang == "1")
                                e.Value = "Ra viện";
                            else if (data.tinhTrang == "2")
                                e.Value = "Chuyển viện";
                            else if (data.tinhTrang == "3")
                                e.Value = "Trốn viện";
                            else if (data.tinhTrang == "4")
                                e.Value = "Xin ra viện";
                        }
                        else if (e.Column.FieldName == "kqDieuTri")
                        {
                            if (data.kqDieuTri == "1")
                                e.Value = "Khỏi";
                            else if (data.kqDieuTri == "2")
                                e.Value = "Đỡ";
                            else if (data.kqDieuTri == "3")
                                e.Value = "Không thay đổi";
                            else if (data.kqDieuTri == "4")
                                e.Value = "Nặng hơn";
                            else if (data.kqDieuTri == "5")
                                e.Value = "Tử vong";
                        }
                        else if (e.Column.FieldName == "ngayVao_str" && !String.IsNullOrEmpty(data.ngayVao))
                        {
                            e.Value = TimeNumberToTimeStringWithoutSecond(Int64.Parse(data.ngayVao));
                        }
                        else if (e.Column.FieldName == "ngayRa_str" && !String.IsNullOrEmpty(data.ngayRa))
                        {
                            e.Value = TimeNumberToTimeStringWithoutSecond(Int64.Parse(data.ngayRa));
                        }
                        else if (e.Column.FieldName == "cskcbbd_name")
                        {
                            if (this.dicMediOrg != null && this.dicMediOrg.ContainsKey(data.maCSKCB))
                            {
                                e.Value = this.dicMediOrg[data.maCSKCB].MEDI_ORG_NAME;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCCheckTT_Load(object sender, EventArgs e)
        {
            try
            {
                this.dicMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().ToDictionary(o => o.MEDI_ORG_CODE, o => o);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__View_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (ExamHistoryLDO)gridViewHistory.GetFocusedRow();
                if (data != null && !string.IsNullOrEmpty(data.maHoSo))
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTreatmentDetailGOV").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTreatmentDetailGOV");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();

                        listArgs.Add(data.maHoSo);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintf_Click(object sender, EventArgs e)
        {
            if (this._resultHistoryLDO != null)
            {
                PrintProcess("Mps000270");
            }
        }

        void PrintProcess(string printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case "Mps000270":
                        richEditorMain.RunPrintTemplate("Mps000270", DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000270":
                        Mps000270(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000270(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                MPS.Processor.Mps000270.PDO.Mps000270PDO mps000270RDO = new MPS.Processor.Mps000270.PDO.Mps000270PDO(
                this._resultHistoryLDO,
                BackendDataWorker.Get<HIS_MEDI_ORG>()
                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000270RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000270RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public static string TimeNumberToTimeStringWithoutSecond(long time)
        {
            string result = null;
            try
            {
                string text = time.ToString();
                if (text != null)
                {
                    if (text.Length >= 12)
                    {
                        return new StringBuilder().Append(text.Substring(6, 2)).Append("/").Append(text.Substring(4, 2))
                            .Append("/")
                            .Append(text.Substring(0, 4))
                            .Append(" ")
                            .Append(text.Substring(8, 2))
                            .Append(":")
                            .Append(text.Substring(10, 2))
                            .ToString();
                    }

                    return result;
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
