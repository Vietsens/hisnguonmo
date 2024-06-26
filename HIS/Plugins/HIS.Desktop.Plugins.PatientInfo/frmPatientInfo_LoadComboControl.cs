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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.PatientInfo;
using HIS.UC.WorkPlace;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.LocalData;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;

namespace HIS.Desktop.Plugins.PatientInfo
{
    public partial class frmPatientInfo : HIS.Desktop.Utility.FormBase
    {
        private void LoadProvinceCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboCommune.Properties.DataSource = null;
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    cboDistricts.Properties.DataSource = null;
                    cboDistricts.EditValue = null;
                    txtDistricts.Text = "";
                    cboProvince.EditValue = null;
                    FocusShowPopup(cboProvince);
                    //PopupLoader.SelectFirstRowPopup(cboProvince);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                        txtProvince.Text = listResult[0].PROVINCE_CODE;
                        LoadDistrictsCombo("", listResult[0].PROVINCE_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtDistricts);
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboCommune.Properties.DataSource = null;
                        cboCommune.EditValue = null;
                        txtCommune.Text = "";
                        cboDistricts.Properties.DataSource = null;
                        cboDistricts.EditValue = null;
                        txtDistricts.Text = "";
                        cboProvince.EditValue = null;
                        if (isExpand)
                        {
                            FocusShowPopup(cboProvince);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private SDA.EFMODEL.DataModels.V_SDA_PROVINCE LoadProvinceComboByDistrict(string districtCode, bool isExpand)
        {
            SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = null ;
            try
            { 
                if (!String.IsNullOrEmpty(districtCode))
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                    listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.DISTRICT_CODE.Contains(districtCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        var result = listResult.FirstOrDefault().PROVINCE_CODE;
                        var resultProvince = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE.Contains(result)).ToList();
                        province = resultProvince.FirstOrDefault();
                    }
                }
                return province;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return province;
            }
        }

        private void LoadProvinceComboHT(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboXaHienTai.Properties.DataSource = null;
                    cboXaHienTai.EditValue = null;
                    txtXaHienTai.Text = "";
                    cboHuyenHienTai.Properties.DataSource = null;
                    cboHuyenHienTai.EditValue = null;
                    txtHuyenHienTai.Text = "";
                    cboTinhHienTai.EditValue = null;
                    FocusShowPopup(cboTinhHienTai);
                    //PopupLoader.SelectFirstRowPopup(cboProvince);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        cboTinhHienTai.EditValue = listResult[0].PROVINCE_NAME;
                        txtTinhHienTai.Text = listResult[0].PROVINCE_CODE;
                        LoadDistrictsComboHT("", listResult[0].PROVINCE_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtHuyenHienTai);
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboXaHienTai.Properties.DataSource = null;
                        cboXaHienTai.EditValue = null;
                        txtXaHienTai.Text = "";
                        cboHuyenHienTai.Properties.DataSource = null;
                        cboHuyenHienTai.EditValue = null;
                        txtHuyenHienTai.Text = "";
                        cboTinhHienTai.EditValue = null;
                        if (isExpand)
                        {
                            FocusShowPopup(cboTinhHienTai);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadProvinceComboKS(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboTinhKhaiSinh.EditValue = null;
                    FocusShowPopup(cboTinhKhaiSinh);
                    //PopupLoader.SelectFirstRowPopup(cboProvince);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        cboTinhKhaiSinh.EditValue = listResult[0].PROVINCE_CODE;
                        txtTinhKhaiSinh.Text = listResult[0].PROVINCE_CODE;
                        //LoadDistrictsCombo("", listResult[0].PROVINCE_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtChuongTrinh);
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboProvince.EditValue = null;
                        if (isExpand)
                        {
                            FocusShowPopup(cboTinhKhaiSinh);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadDistrictsCombo(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.DISTRICT_CODE.Contains(searchCode) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();         
                cboDistricts.Properties.DataSource = listResult;
                cboDistricts.Properties.DisplayMember = "RENDERER_DISTRICT_NAME";
                cboDistricts.Properties.ValueMember = "DISTRICT_CODE";
                cboDistricts.Properties.ForceInitialize();

                cboDistricts.Properties.Columns.Clear();
                cboDistricts.Properties.Columns.Add(new LookUpColumnInfo("DISTRICT_CODE", "", 100));
                cboDistricts.Properties.Columns.Add(new LookUpColumnInfo("RENDERER_DISTRICT_NAME", "", 200));

                cboDistricts.Properties.ShowHeader = false;
                cboDistricts.Properties.ImmediatePopup = true;
                cboDistricts.Properties.DropDownRows = 20;
                cboDistricts.Properties.PopupWidth = 300;

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    cboCommune.Properties.DataSource = null;
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    txtDistricts.Text = "";
                    cboDistricts.EditValue = null;
                    FocusShowPopup(cboDistricts);
                    //PopupProcess.SelectFirstRowPopup(cboDistricts);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboDistricts.EditValue = listResult[0].DISTRICT_CODE;
                        txtDistricts.Text = listResult[0].DISTRICT_CODE;
                        LoadCommuneCombo("", listResult[0].DISTRICT_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtCommune);
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboCommune.Properties.DataSource = null;
                        cboCommune.EditValue = null;
                        txtCommune.Text = "";
                        txtDistricts.Text = "";
                        cboDistricts.EditValue = null;
                        if (isExpand)
                        {
                            FocusShowPopup(cboDistricts);
                            //PopupProcess.SelectFirstRowPopup(cboDistricts);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadDistrictsComboHT(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.DISTRICT_CODE.Contains(searchCode) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();

                cboHuyenHienTai.Properties.DataSource = listResult;
                cboHuyenHienTai.Properties.DisplayMember = "DISTRICT_NAME";
                cboHuyenHienTai.Properties.ValueMember = "DISTRICT_NAME";
                cboHuyenHienTai.Properties.ForceInitialize();

                cboHuyenHienTai.Properties.Columns.Clear();
                cboHuyenHienTai.Properties.Columns.Add(new LookUpColumnInfo("DISTRICT_CODE", "", 100));
                cboHuyenHienTai.Properties.Columns.Add(new LookUpColumnInfo("DISTRICT_NAME", "", 200));

                cboHuyenHienTai.Properties.ShowHeader = false;
                cboHuyenHienTai.Properties.ImmediatePopup = true;
                cboHuyenHienTai.Properties.DropDownRows = 20;
                cboHuyenHienTai.Properties.PopupWidth = 300;

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    cboXaHienTai.Properties.DataSource = null;
                    cboXaHienTai.EditValue = null;
                    txtXaHienTai.Text = "";
                    txtHuyenHienTai.Text = "";
                    cboHuyenHienTai.EditValue = null;
                    FocusShowPopup(cboHuyenHienTai);
                    //PopupProcess.SelectFirstRowPopup(cboDistricts);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboHuyenHienTai.EditValue = listResult[0].DISTRICT_NAME;
                        txtHuyenHienTai.Text = listResult[0].DISTRICT_CODE;
                        LoadCommuneComboHT("", listResult[0].DISTRICT_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtXaHienTai);
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboXaHienTai.Properties.DataSource = null;
                        cboXaHienTai.EditValue = null;
                        txtXaHienTai.Text = "";
                        txtHuyenHienTai.Text = "";
                        cboHuyenHienTai.EditValue = null;
                        if (isExpand)
                        {
                            FocusShowPopup(cboHuyenHienTai);
                            //PopupProcess.SelectFirstRowPopup(cboDistricts);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCommuneCombo(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                listResult = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.COMMUNE_CODE.Contains(searchCode) && (districtCode == "" || o.DISTRICT_CODE == districtCode)).ToList();
                cboCommune.Properties.DataSource = listResult;
                cboCommune.Properties.DisplayMember = "RENDERER_COMMUNE_NAME";
                cboCommune.Properties.ValueMember = "COMMUNE_CODE";
                cboCommune.Properties.ForceInitialize();

                cboCommune.Properties.Columns.Clear();
                cboCommune.Properties.Columns.Add(new LookUpColumnInfo("COMMUNE_CODE", "", 100));
                cboCommune.Properties.Columns.Add(new LookUpColumnInfo("RENDERER_COMMUNE_NAME", "", 200));

                cboCommune.Properties.ShowHeader = false;
                cboCommune.Properties.ImmediatePopup = true;
                cboCommune.Properties.DropDownRows = 20;
                cboCommune.Properties.PopupWidth = 300;

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    FocusShowPopup(cboCommune);
                    //PopupProcess.SelectFirstRowPopup(cboCommune);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                        txtCommune.Text = listResult[0].COMMUNE_CODE;
                        if (isExpand)
                        {
                            FocusMoveText(txtNation);
                        }
                    }
                    else if (isExpand && listResult.Count > 1)
                    {
                        cboCommune.EditValue = null;
                        FocusShowPopup(cboCommune);
                        // PopupProcess.SelectFirstRowPopup(cboCommune);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCommuneComboHT(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                listResult = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.COMMUNE_CODE.Contains(searchCode) && (districtCode == "" || o.DISTRICT_CODE == districtCode)).ToList();
                cboXaHienTai.Properties.DataSource = listResult;
                cboXaHienTai.Properties.DisplayMember = "COMMUNE_NAME";
                cboXaHienTai.Properties.ValueMember = "COMMUNE_CODE";
                cboXaHienTai.Properties.ForceInitialize();

                cboXaHienTai.Properties.Columns.Clear();
                cboXaHienTai.Properties.Columns.Add(new LookUpColumnInfo("COMMUNE_CODE", "", 100));
                cboXaHienTai.Properties.Columns.Add(new LookUpColumnInfo("COMMUNE_NAME", "", 200));

                cboXaHienTai.Properties.ShowHeader = false;
                cboXaHienTai.Properties.ImmediatePopup = true;
                cboXaHienTai.Properties.DropDownRows = 20;
                cboXaHienTai.Properties.PopupWidth = 300;

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    cboXaHienTai.EditValue = null;
                    txtXaHienTai.Text = "";
                    FocusShowPopup(cboXaHienTai);
                    //PopupProcess.SelectFirstRowPopup(cboCommune);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboXaHienTai.EditValue = listResult[0].COMMUNE_CODE;
                        txtXaHienTai.Text = listResult[0].COMMUNE_CODE;
                        if (isExpand)
                        {
                            FocusMoveText(txtDiaChiHienTai);
                        }
                    }
                    else if (isExpand && listResult.Count > 1)
                    {
                        cboXaHienTai.EditValue = null;
                        FocusShowPopup(cboXaHienTai);
                        // PopupProcess.SelectFirstRowPopup(cboCommune);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        async Task InitWorkPlaceControl()
        {
            try
            {
                CommonParam param = new CommonParam();
                workPlaceProcessor = new WorkPlaceProcessor(param);
                if (this.CheDoHienThiNoiLamViecManHinhDangKyTiepDon == 1)
                    workPlaceTemplate = WorkPlaceProcessor.Template.Textbox;
                else
                    workPlaceTemplate = WorkPlaceProcessor.Template.Combo;
                List<HIS_WORK_PLACE> defaultworwplace = new List<HIS_WORK_PLACE>();
                HisWorkPlaceFilter filterWorkPlace = new HisWorkPlaceFilter();
                defaultworwplace = BackendDataWorker.Get<HIS_WORK_PLACE>();

                WorkPlaceInitADO data = new WorkPlaceInitADO();
                data.Template = workPlaceTemplate;
                data.FocusMoveout = FocusMoveout;
                data.WorlPlaces = defaultworwplace;
                //data.SetValidateControl = validate;

                ucWorkPlace = (await workPlaceProcessor.Generate(data).ConfigureAwait(false)) as UserControl;
                if (ucWorkPlace != null)
                {
                    ucWorkPlace.Dock = DockStyle.Fill;
                    pnlWorkPlace.Controls.Add(ucWorkPlace);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void FocusMoveout()
        {
            try
            {
                txtMilitaryRankCode.Focus();
                txtMilitaryRankCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void LoadGioiTinhCombo(string searchCode, DevExpress.XtraEditors.LookUpEdit cboGioiTinh, DevExpress.XtraEditors.TextEdit txtMaGioiTinh, DevExpress.XtraEditors.ButtonEdit txtNgaySinh)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboGioiTinh.EditValue = null;
                    FocusShowPopup(cboGioiTinh);
                    PopupLoader.SelectFirstRowPopup(cboGioiTinh);
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().Where(o => o.GENDER_CODE.Contains(searchCode)).ToList();
                    List<HIS_GENDER> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.GENDER_CODE == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboGioiTinh.EditValue = result[0].ID;
                        txtMaGioiTinh.Text = result[0].GENDER_CODE;
                        txtPatientDOB.Focus();
                        txtPatientDOB.SelectAll();
                    }
                    else
                    {
                        cboGioiTinh.EditValue = null;
                        txtPatientDOB.Focus();
                        txtPatientDOB.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void LoadDanTocCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboDanToc, DevExpress.XtraEditors.TextEdit txtMaDanToc, DevExpress.XtraEditors.TextEdit txtMaQuocTich)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDanToc.EditValue = null;
                    FocusShowPopup(cboDanToc);
                    PopupLoader.SelectFirstRowPopup(cboDanToc);
                }
                else
                {
                    var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(o => o.ETHNIC_CODE.Contains(searchCode)).ToList();
                    List<SDA.EFMODEL.DataModels.SDA_ETHNIC> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.ETHNIC_CODE == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboDanToc.EditValue = result[0].ETHNIC_CODE;
                        txtMaDanToc.Text = result[0].ETHNIC_CODE;
                        FocusMoveText(txtCareer);
                    }
                    else
                    {
                        cboDanToc.EditValue = null;
                        FocusShowPopup(cboDanToc);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadQuocTichCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboQuocTich, DevExpress.XtraEditors.TextEdit txtMaQuocTich, DevExpress.XtraEditors.PanelControl txtNoiLamViec)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboQuocTich.EditValue = null;
                    FocusShowPopup(cboQuocTich);
                    //PopupLoader.SelectFirstRowPopup(cboQuocTich);
                }
                else
                {
                    var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_CODE.ToUpper().Contains(searchCode)).ToList();
                    List<SDA_NATIONAL> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.NATIONAL_CODE.ToUpper() == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboQuocTich.EditValue = result[0].NATIONAL_CODE;
                        txtMaQuocTich.Text = result[0].NATIONAL_CODE;
                        if (workPlaceProcessor != null && workPlaceTemplate != null)
                            workPlaceProcessor.FocusControl(workPlaceTemplate);
                    }
                    else
                    {
                        cboQuocTich.EditValue = null;
                        FocusShowPopup(cboQuocTich);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadNgheNghiepCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboNgheNghiep, DevExpress.XtraEditors.TextEdit txtMaNgheNghiep, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboNgheNghiep.EditValue = null;
                    FocusShowPopup(cboNgheNghiep);
                    PopupLoader.SelectFirstRowPopup(cboNgheNghiep);
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.CAREER_CODE.ToUpper().Contains(searchCode)).ToList();
                    List<HIS_CAREER> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.CAREER_CODE.ToUpper() == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboNgheNghiep.EditValue = result[0].ID;
                        txtMaNgheNghiep.Text = result[0].CAREER_CODE;
                        FocusMoveText(focusControl);
                    }
                    else
                    {
                        cboNgheNghiep.EditValue = null;
                        FocusShowPopup(cboNgheNghiep);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadABOCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboAbo, DevExpress.XtraEditors.TextEdit txtAbo, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboAbo.EditValue = null;
                    FocusShowPopup(cboAbo);
                    //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>().Where(o => o.BLOOD_ABO_CODE.ToUpper().Equals(searchCode)).ToList();
                    List<HIS_BLOOD_ABO> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.BLOOD_ABO_CODE.ToUpper() == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboAbo.EditValue = result[0].ID;
                        txtAbo.Text = result[0].BLOOD_ABO_CODE;
                        FocusMoveText(focusControl);
                    }
                    else
                    {
                        cboAbo.EditValue = null;
                        FocusShowPopup(cboAbo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void LoadRHCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboAboRh, DevExpress.XtraEditors.TextEdit txtAboRh, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboAboRh.EditValue = null;
                    FocusShowPopup(cboAboRh);
                    //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>().Where(o => o.BLOOD_RH_CODE.ToUpper().Contains(searchCode)).ToList();
                    List<HIS_BLOOD_RH> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.BLOOD_RH_CODE.ToUpper() == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboAboRh.EditValue = result[0].ID;
                        txtAboRh.Text = result[0].BLOOD_RH_CODE;
                        FocusMoveText(focusControl);
                    }
                    else
                    {
                        cboAboRh.EditValue = null;
                        FocusShowPopup(cboAboRh);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetNotInListValue(object sender, GetNotInListValueEventArgs e, LookUpEdit cbo)
        {
            try
            {
                if (e.FieldName == "RENDERER_DISTRICT_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.DISTRICT_NAME);
                }

                if (e.FieldName == "RENDERER_COMMUNE_NAME")
                {
                    var item = ((List<V_SDA_COMMUNE>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.COMMUNE_NAME);
                }

                if (e.FieldName == "RENDERER_PROVINCE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", "", item.PROVINCE_NAME);
                }

                if (e.FieldName == "RENDERER_PDC_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} - {1} {2} - {3}", item.DISTRICT_INITIAL_NAME, item.DISTRICT_NAME, item.INITIAL_NAME, item.COMMUNE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
