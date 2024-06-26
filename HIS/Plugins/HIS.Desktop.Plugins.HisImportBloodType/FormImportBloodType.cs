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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportBloodType
{
    public partial class FormImportBloodType : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private List<ADO.BloodTypeAdo> ListDataImport;

        private bool checkClick;
        private List<HIS_BLOOD_GROUP> ListBloodGroup;
        private List<HIS_BLOOD_VOLUME> ListBloodVolume;
        private HIS_HEIN_SERVICE_TYPE heinServiceType;
        RefeshReference delegateRefresh;
        public FormImportBloodType()
        {
            InitializeComponent();
        }

        public FormImportBloodType(Inventec.Desktop.Common.Modules.Module moduleData)
        //: base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.Text = moduleData.text;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormImportBloodType_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                LoadDataDeafult();

                BtnSave.Enabled = false;
                BtnLineError.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDeafult()
        {
            try
            {
                MOS.Filter.HisBloodGroupFilter bloodGroupFilter = new MOS.Filter.HisBloodGroupFilter();
                this.ListBloodGroup = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_BLOOD_GROUP>>("api/HisBloodGroup/Get", ApiConsumer.ApiConsumers.MosConsumer, bloodGroupFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (this.ListBloodGroup == null) this.ListBloodGroup = new List<HIS_BLOOD_GROUP>();

                MOS.Filter.HisBloodVolumeFilter bloodVolumeFilter = new MOS.Filter.HisBloodVolumeFilter();
                this.ListBloodVolume = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_BLOOD_VOLUME>>("api/HisBloodVolume/Get", ApiConsumer.ApiConsumers.MosConsumer, bloodVolumeFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (this.ListBloodVolume == null) this.ListBloodVolume = new List<HIS_BLOOD_VOLUME>();

                heinServiceType = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.BtnDownloadTemplate.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_DOWNLOAD_TEMPLATE");
                this.BtnImport.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_IMPORT");
                this.BtnLineError.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_LINE_ERROR__ERROR");
                //this.BtnRefresh.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_REFRESH");
                this.BtnSave.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_SAVE");
                this.Gc_AlterExpiredDate.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_ALERT_EXPIRED_DATE");
                this.Gc_BloodGroupName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_BLOOD_GROUP_NAME");
                this.Gc_BloodTypeCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_BLOOD_TYPE_CODE");
                this.Gc_BloodTypeName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_BLOOD_TYPE_NAME");
                this.Gc_Delete.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_DELETE");
                this.Gc_HeinLimitPrice.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE");
                this.Gc_HeinLimitPriceInTime.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE_IN_TIME");
                this.Gc_HeinLimitPriceIntrTime.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE_INTR_TIME");
                this.Gc_HeinLimitPriceOld.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE_OLD");
                this.Gc_HeinLimitRatio.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_RATIO");
                this.Gc_HeinLimitRatioOld.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_RATIO_OLD");
                this.Gc_HeinOrder.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__HEIN_ORDER");
                this.Gc_HeinServiceBhytCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_SERVICE_BHYT_CODE");
                this.Gc_HeinServiceBhytName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_SERVICE_BHYT_NAME");
                this.Gc_HeinServiceTypeName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_SERVICE_TYPE_NAME");
                this.Gc_ImpPrice.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_IMP_PRICE");
                this.Gc_ImpVatRatio.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_IMP_VAT_RATIO");
                this.Gc_InternalPrice.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_INTERNAL_PRICE");
                this.Gc_LineError.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_LINE_ERROR");
                this.Gc_PackingTypeName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_PACKING_TYPE_NAME");
                this.Gc_ParentCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_PARENT_CODE");
                this.Gc_ServiceUnitName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_SERVICE_UNIT_NAME");
                this.Gc_Stt.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_STT");
                this.Gc_Volume.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_VOLUME");
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__RP_BTN_DELETE");
                this.repositoryItemBtnLineError.Buttons[0].ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__RP_BTN_LINE_ERROR");

                this.Gc_CPNG.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_CPNG");
                this.Gc_NumOrder.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_NUM_ORDER");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void BtnDownloadTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath + "/Tmp/Imp", "IMPORT_BLOOD_TYPE.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_BLOOD_TYPE";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thất bại");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var ImpListProcessor = import.Get<ADO.BloodTypeAdo>(0);
                        if (ImpListProcessor != null && ImpListProcessor.Count > 0)
                        {
                            this.ListDataImport = new List<ADO.BloodTypeAdo>();
                            AddListBloodTypeToProcessList(ImpListProcessor);

                            if (this.ListDataImport != null && this.ListDataImport.Count > 0)
                            {
                                SetDataSource(ListDataImport);

                                checkClick = false;
                                //BtnSave.Enabled = true;
                                BtnLineError.Enabled = true;
                            }
                            WaitingManager.Hide();
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceLanguageManager.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceLanguageManager.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSource(List<ADO.BloodTypeAdo> ListDataImport)
        {
            try
            {
                gridControl.BeginUpdate();
                gridControl.DataSource = null;
                gridControl.DataSource = ListDataImport;
                gridControl.EndUpdate();
                CheckErrorLine(ListDataImport);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<ADO.BloodTypeAdo> dataSource)
        {
            try
            {
                if (dataSource != null && dataSource.Count > 0)
                {
                    var checkError = dataSource.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                    if (!checkError)
                    {
                        BtnSave.Enabled = true;
                    }
                    else
                    {
                        BtnSave.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddListBloodTypeToProcessList(List<ADO.BloodTypeAdo> ImpListProcessor)
        {
            try
            {
                if (ImpListProcessor != null && ImpListProcessor.Count > 0)
                {
                    long i = 0;
                    foreach (var item in ImpListProcessor)
                    {
                        i++;
                        List<string> errors = new List<string>();
                        var ado = new ADO.BloodTypeAdo();
                        ado.IdRow = i;
                        ado.IS_LEAF = 1;
                        ado.ALERT_EXPIRED_DATE_STR = item.ALERT_EXPIRED_DATE_STR;
                        if (!string.IsNullOrEmpty(item.ALERT_EXPIRED_DATE_STR))
                        {
                            if (Inventec.Common.Number.Check.IsNumber(item.ALERT_EXPIRED_DATE_STR))
                            {
                                ado.ALERT_EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(item.ALERT_EXPIRED_DATE_STR);
                                //ado.ALERT_EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.ALERT_EXPIRED_DATE_STR);

                                if ((ado.ALERT_EXPIRED_DATE ?? 0) > 999999999999999999 || (ado.ALERT_EXPIRED_DATE ?? 0) < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_ALERT_EXPIRED_DATE")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_ALERT_EXPIRED_DATE")));
                            }

                        }

                        if (!String.IsNullOrWhiteSpace(item.BLOOD_GROUP_CODE))
                        {
                            var bloodGroup = ListBloodGroup.FirstOrDefault(o => o.BLOOD_GROUP_CODE == item.BLOOD_GROUP_CODE);
                            if (bloodGroup != null)
                            {
                                ado.BLOOD_GROUP_CODE = bloodGroup.BLOOD_GROUP_CODE;
                                ado.BLOOD_GROUP_ID = bloodGroup.ID;
                                ado.BLOOD_GROUP_NAME = bloodGroup.BLOOD_GROUP_NAME;
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, Resources.ResourceLanguageManager.NhomMau));
                            }
                        }

                        ado.BLOOD_TYPE_CODE = item.BLOOD_TYPE_CODE;
                        if (string.IsNullOrWhiteSpace(item.BLOOD_TYPE_CODE))
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_BLOOD_TYPE_CODE")));
                        }
                        else
                        {
                            if (item.BLOOD_TYPE_CODE.Length > 25)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_BLOOD_TYPE_CODE")));
                            }

                            var check = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().Exists(o => o.BLOOD_TYPE_CODE == item.BLOOD_TYPE_CODE);
                            if (check)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.DaTonTai, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_BLOOD_TYPE_CODE")));
                            }

                            var checkExel = ListDataImport.FirstOrDefault(o => o.BLOOD_TYPE_CODE == item.BLOOD_TYPE_CODE);
                            if (checkExel != null)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.BiTrung, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_BLOOD_TYPE_CODE")));
                            }
                        }

                        ado.BLOOD_TYPE_NAME = item.BLOOD_TYPE_NAME;
                        if (string.IsNullOrWhiteSpace(item.BLOOD_TYPE_NAME))
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_BLOOD_TYPE_NAME")));
                        }
                        else
                        {
                            if (Inventec.Common.String.CountVi.Count(ado.BLOOD_TYPE_NAME) > 500)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_BLOOD_TYPE_NAME")));
                            }
                        }

                        ado.ELEMENT = item.ELEMENT;
                        ado.HEIN_ORDER = item.HEIN_ORDER;
                        if (!string.IsNullOrEmpty(item.HEIN_ORDER))
                        {
                            if (item.HEIN_ORDER.Length > 20)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__HEIN_ORDER")));
                            }
                        }

                        ado.HEIN_SERVICE_BHYT_CODE = item.HEIN_SERVICE_BHYT_CODE;
                        ado.HEIN_SERVICE_BHYT_NAME = item.HEIN_SERVICE_BHYT_NAME;
                        if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE))
                        {
                            if (item.HEIN_SERVICE_BHYT_CODE.Length > 100)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_SERVICE_BHYT_CODE")));
                            }
                            if (string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.CoThiPhaiNhap, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_SERVICE_BHYT_CODE"), GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_SERVICE_BHYT_NAME")));
                            }
                        }

                        ado.HEIN_SERVICE_BHYT_NAME = item.HEIN_SERVICE_BHYT_NAME;
                        if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                        {
                            if (Inventec.Common.String.CountVi.Count(item.HEIN_SERVICE_BHYT_NAME) > 500)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_SERVICE_BHYT_NAME")));
                            }
                            if (string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_CODE))
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.CoThiPhaiNhap, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_SERVICE_BHYT_NAME"), GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_SERVICE_BHYT_CODE")));
                            }
                        }

                        ado.HEIN_LIMIT_PRICE_IN_TIME_STR = item.HEIN_LIMIT_PRICE_IN_TIME_STR;
                        if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_IN_TIME_STR))
                        {
                            long? dateTime = null;
                            string check = "";
                            convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_IN_TIME_STR, ref dateTime, ref check);
                            if (dateTime != null && string.IsNullOrEmpty(check))
                            {
                                ado.HEIN_LIMIT_PRICE_IN_TIME = dateTime;
                                ado.HEIN_LIMIT_PRICE_IN_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dateTime ?? 0);
                            }
                            else
                            {
                                errors.Add(string.Format(check, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE_IN_TIME")));
                            }
                        }

                        ado.HEIN_LIMIT_PRICE_INTR_TIME_STR = item.HEIN_LIMIT_PRICE_INTR_TIME_STR;
                        if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_INTR_TIME_STR))
                        {
                            long? dateTime = null;
                            string check = "";
                            convertDateStringToTimeNumber(item.HEIN_LIMIT_PRICE_INTR_TIME_STR, ref dateTime, ref check);
                            if (dateTime != null && string.IsNullOrEmpty(check))
                            {
                                ado.HEIN_LIMIT_PRICE_INTR_TIME = dateTime;
                                ado.HEIN_LIMIT_PRICE_INTR_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dateTime ?? 0);
                            }
                            else
                            {
                                errors.Add(string.Format(check, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE_INTR_TIME")));
                            }
                        }

                        if (ado.HEIN_LIMIT_PRICE_INTR_TIME.HasValue && ado.HEIN_LIMIT_PRICE_IN_TIME.HasValue)
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.KhongDongThoiCoCacThongTin, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE_INTR_TIME"), GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE_IN_TIME")));
                        }

                        ado.HEIN_LIMIT_PRICE_STR = item.HEIN_LIMIT_PRICE_STR;
                        if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_STR))
                        {
                            if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_PRICE_STR))
                            {
                                ado.HEIN_LIMIT_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_PRICE_STR);
                                if (ado.HEIN_LIMIT_PRICE.Value > 99999999999999 || ado.HEIN_LIMIT_PRICE < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE")));
                            }
                        }

                        ado.HEIN_LIMIT_PRICE_OLD_STR = item.HEIN_LIMIT_PRICE_OLD_STR;
                        if (!string.IsNullOrEmpty(item.HEIN_LIMIT_PRICE_OLD_STR))
                        {
                            if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_PRICE_OLD_STR))
                            {
                                ado.HEIN_LIMIT_PRICE_OLD = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_PRICE_OLD_STR);
                                if (ado.HEIN_LIMIT_PRICE_OLD.Value > 99999999999999 || ado.HEIN_LIMIT_PRICE_OLD < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE_OLD")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE_OLD")));
                            }
                        }

                        ado.HEIN_LIMIT_RATIO_STR = item.HEIN_LIMIT_RATIO_STR;
                        if (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_STR))
                        {
                            if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_RATIO_STR))
                            {
                                ado.HEIN_LIMIT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_RATIO_STR);
                                if (ado.HEIN_LIMIT_RATIO.Value > 99999999999999 || ado.HEIN_LIMIT_RATIO < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_RATIO")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_RATIO")));
                            }
                        }

                        if (!string.IsNullOrEmpty(item.HEIN_LIMIT_RATIO_OLD_STR))
                        {
                            ado.HEIN_LIMIT_RATIO_OLD_STR = item.HEIN_LIMIT_RATIO_OLD_STR;
                            if (Inventec.Common.Number.Check.IsDecimal(item.HEIN_LIMIT_RATIO_OLD_STR))
                            {
                                ado.HEIN_LIMIT_RATIO_OLD = Inventec.Common.TypeConvert.Parse.ToDecimal(item.HEIN_LIMIT_RATIO_OLD_STR);
                                if (ado.HEIN_LIMIT_RATIO_OLD.Value > 99999999999999 || ado.HEIN_LIMIT_RATIO_OLD < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_RATIO_OLD")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_RATIO_OLD")));
                            }
                        }

                        if ((ado.HEIN_LIMIT_PRICE.HasValue || ado.HEIN_LIMIT_PRICE_OLD.HasValue) && (ado.HEIN_LIMIT_RATIO.HasValue || ado.HEIN_LIMIT_RATIO_OLD.HasValue))
                        {
                            string heinLimit = ado.HEIN_LIMIT_PRICE.HasValue ? GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE") : GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_PRICE_OLD");
                            string heinlimitRatio = ado.HEIN_LIMIT_RATIO.HasValue ? GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_RATIO") : GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_HEIN_LIMIT_RATIO_OLD");
                            errors.Add(string.Format(Resources.ResourceLanguageManager.KhongDongThoiCoCacThongTin, heinLimit, heinlimitRatio));
                        }

                        if (this.heinServiceType != null)
                        {
                            ado.HEIN_SERVICE_TYPE_CODE = this.heinServiceType.HEIN_SERVICE_TYPE_CODE;
                            ado.HEIN_SERVICE_TYPE_ID = this.heinServiceType.ID;
                            ado.HEIN_SERVICE_TYPE_NAME = this.heinServiceType.HEIN_SERVICE_TYPE_NAME;
                        }

                        ado.IMP_PRICE_STR = item.IMP_PRICE_STR;
                        if (!string.IsNullOrEmpty(item.IMP_PRICE_STR))
                        {
                            if (Inventec.Common.Number.Check.IsDecimal(item.IMP_PRICE_STR))
                            {
                                ado.IMP_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.IMP_PRICE_STR);
                                if (ado.IMP_PRICE.Value > 99999999999999 || ado.IMP_PRICE < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_IMP_PRICE")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_IMP_PRICE")));
                            }
                        }

                        ado.IMP_VAT_RATIO_STR = item.IMP_VAT_RATIO_STR;
                        if (!string.IsNullOrEmpty(item.IMP_VAT_RATIO_STR))
                        {
                            if (Inventec.Common.Number.Check.IsDecimal(item.IMP_VAT_RATIO_STR))
                            {
                                ado.IMP_VAT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(item.IMP_VAT_RATIO_STR);
                                if (ado.IMP_VAT_RATIO.Value > 1 || ado.IMP_VAT_RATIO < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_IMP_VAT_RATIO")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_IMP_VAT_RATIO")));
                            }
                        }

                        ado.INTERNAL_PRICE_STR = item.INTERNAL_PRICE_STR;
                        if (!string.IsNullOrEmpty(item.INTERNAL_PRICE_STR))
                        {
                            if (Inventec.Common.Number.Check.IsDecimal(item.INTERNAL_PRICE_STR))
                            {
                                ado.INTERNAL_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.INTERNAL_PRICE_STR);
                                if (ado.INTERNAL_PRICE.Value > 99999999999999 || ado.INTERNAL_PRICE < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_INTERNAL_PRICE")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_INTERNAL_PRICE")));
                            }
                        }

                        ado.PACKING_TYPE_CODE = item.PACKING_TYPE_CODE;
                        if (!String.IsNullOrWhiteSpace(item.PACKING_TYPE_CODE))
                        {
                            var packingType = BackendDataWorker.Get<HIS_PACKING_TYPE>().FirstOrDefault(o => o.PACKING_TYPE_CODE == item.PACKING_TYPE_CODE);
                            if (packingType != null)
                            {
                                ado.PACKING_TYPE_CODE = packingType.PACKING_TYPE_CODE;
                                ado.PACKING_TYPE_ID = packingType.ID;
                                ado.PACKING_TYPE_NAME = packingType.PACKING_TYPE_NAME;
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_PACKING_TYPE_NAME")));
                            }
                        }

                        ado.PARENT_CODE = item.PARENT_CODE;
                        if (!string.IsNullOrEmpty(item.PARENT_CODE))
                        {
                            if (item.PARENT_CODE.Length > 25)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_PARENT_CODE")));
                            }
                            var getData = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(o => o.BLOOD_TYPE_CODE == item.PARENT_CODE);
                            if (getData != null)
                            {
                                ado.PARENT_ID = getData.ID;
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_PARENT_CODE")));
                            }
                        }

                        if (!string.IsNullOrEmpty(item.SERVICE_UNIT_CODE))
                        {
                            if (item.SERVICE_UNIT_CODE.Length > 3)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_SERVICE_UNIT_NAME")));
                            }
                            var getData = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.SERVICE_UNIT_CODE == item.SERVICE_UNIT_CODE);
                            if (getData != null)
                            {
                                ado.SERVICE_UNIT_CODE = getData.SERVICE_UNIT_CODE;
                                ado.SERVICE_UNIT_ID = getData.ID;
                                ado.SERVICE_UNIT_NAME = getData.SERVICE_UNIT_NAME;
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_SERVICE_UNIT_NAME")));
                            }

                        }
                        else
                        {
                            errors.Add(Resources.ResourceLanguageManager.ThieuTruongDL + " " + GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_SERVICE_UNIT_NAME"));
                        }

                        ado.VOLUME_STR = item.VOLUME_STR;
                        if (!string.IsNullOrEmpty(item.VOLUME_STR))
                        {
                            if (Inventec.Common.Number.Check.IsDecimal(item.VOLUME_STR))
                            {
                                var volume = Inventec.Common.TypeConvert.Parse.ToDecimal(item.VOLUME_STR);
                                if (volume > 99999999999999 || volume < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_VOLUME")));
                                }
                                else
                                {
                                    var BloodVolume = ListBloodVolume.FirstOrDefault(o => o.VOLUME == volume);
                                    if (BloodVolume != null)
                                    {
                                        ado.BLOOD_VOLUME_ID = BloodVolume.ID;
                                        ado.VOLUME = BloodVolume.VOLUME;
                                    }
                                    else
                                    {
                                        errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_VOLUME")));
                                    }
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_VOLUME")));
                            }
                        }
                        else
                        {
                            errors.Add(Resources.ResourceLanguageManager.ThieuTruongDL + " " + GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_VOLUME"));
                        }

                        ado.NUM_ORDER_STR = item.NUM_ORDER_STR;
                        if (!string.IsNullOrEmpty(item.NUM_ORDER_STR))
                        {
                            if (Inventec.Common.Number.Check.IsNumber(item.NUM_ORDER_STR))
                            {
                                ado.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.NUM_ORDER_STR);
                                if (ado.NUM_ORDER.ToString().Length > 19 || ado.NUM_ORDER < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_NUM_ORDER")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_NUM_ORDER")));
                            }
                        }

                        if (!string.IsNullOrEmpty(item.OUT_PARENT_FEE))
                        {
                            if (item.OUT_PARENT_FEE.ToLower() == "x")
                            {
                                ado.IS_OUT_PARENT_FEE = 1;
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__GC_CPNG")));
                            }
                        }

                        ado.ERROR = string.Join(";", errors);
                        ListDataImport.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void convertDateStringToTimeNumber(string date, ref long? dateTime, ref string check)
        {
            try
            {
                if (date.Length > 14)
                {
                    check = Resources.ResourceLanguageManager.Maxlength;
                    return;
                }

                if (date.Length < 14)
                {
                    check = Resources.ResourceLanguageManager.KhongHopLe;
                    return;
                }

                if (date.Length > 0)
                {
                    if (!Inventec.Common.DateTime.Check.IsValidTime(date))
                    {
                        check = Resources.ResourceLanguageManager.KhongHopLe;
                        return;
                    }
                    dateTime = Inventec.Common.TypeConvert.Parse.ToInt64(date);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnLineError.Enabled) return;

                checkClick = true;
                if (BtnLineError.Text == GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_LINE_ERROR__ERROR"))
                {
                    BtnLineError.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_LINE_ERROR__OK");
                    var data = ListDataImport.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(data);
                }
                else
                {
                    BtnLineError.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_LINE_ERROR__ERROR");
                    var data = ListDataImport.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnSave.Enabled) return;
                BtnSave.Focus();
                var listData = (List<ADO.BloodTypeAdo>)gridControl.DataSource;

                if (listData == null || listData.Count <= 0) return;

                bool success = false;
                WaitingManager.Show();

                List<HIS_BLOOD_TYPE> listblood = new List<HIS_BLOOD_TYPE>();

                foreach (var item in listData)
                {
                    HIS_BLOOD_TYPE medi = new HIS_BLOOD_TYPE();
                    HIS_SERVICE ser = new HIS_SERVICE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_BLOOD_TYPE>(medi, item);
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(ser, item);
                    ser.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    ser.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
                    ser.ID = 0;
                    ser.PARENT_ID = null;
                    medi.TDL_SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    medi.HIS_SERVICE = ser;
                    medi.ID = 0;
                    listblood.Add(medi);
                }
                CommonParam param = new CommonParam();

                if (listblood != null && listblood.Count > 0)
                {
                    var rs = new BackendAdapter(param).Post<List<HIS_BLOOD_TYPE>>("api/HisBloodType/CreateList", ApiConsumer.ApiConsumers.MosConsumer, listblood, SessionManager.ActionLostToken, param);
                    if (rs != null)
                    {
                        success = true;
                        BtnSave.Enabled = false;
                        BackendDataWorker.Reset<V_HIS_BLOOD_TYPE>();
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                        //if (this.delegateRefresh != null)
                        //{
                        //    this.delegateRefresh();
                        //}
                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.BloodTypeAdo data = (ADO.BloodTypeAdo)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "CPNG")
                    {
                        try
                        {
                            e.Value = data.IS_OUT_PARENT_FEE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnLineError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.BloodTypeAdo)gridView.GetFocusedRow();
                if (row != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.BloodTypeAdo)gridView.GetFocusedRow();
                if (row != null)
                {
                    if (ListDataImport != null && ListDataImport.Count > 0)
                    {
                        ListDataImport.Remove(row);

                        SetDataSource(ListDataImport);

                        if (checkClick)
                        {
                            if (BtnLineError.Text == GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_LINE_ERROR__ERROR"))
                            {
                                BtnLineError.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_LINE_ERROR__OK");
                            }
                            else
                            {
                                BtnLineError.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BLOOD_TPYE__BTN_LINE_ERROR__ERROR");
                            }
                            BtnLineError_Click(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string ERROR = (view.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString().Trim();
                    if (e.Column.FieldName == "ErrorLine")
                    {
                        if (!string.IsNullOrEmpty(ERROR))
                        {
                            e.RepositoryItem = repositoryItemBtnLineError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
