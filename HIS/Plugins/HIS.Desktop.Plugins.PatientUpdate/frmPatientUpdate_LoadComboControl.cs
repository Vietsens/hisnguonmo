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
using HIS.Desktop.Plugins.PatientUpdate;
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
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    public partial class frmPatientUpdate : HIS.Desktop.Utility.FormBase
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
                    FocusShowPopup(cboProvince, gridLookUpEdit1View);
                    //PopupLoader.SelectFirstRowPopup(cboProvince);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.SEARCH_CODE.Contains(searchCode) || o.PROVINCE_NAME.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                        txtProvince.Text = listResult[0].SEARCH_CODE;
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
                            cboProvince.Properties.DataSource = listResult;
                            FocusShowPopup(cboProvince, gridLookUpEdit1View);
                        }
                    }
                    else
                    {
                        FocusShowPopup(cboProvince, gridLookUpEdit1View);
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
            SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = null;
            try
            {
                if (!String.IsNullOrEmpty(districtCode))
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                    listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.DISTRICT_CODE.Contains(districtCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        var result = listResult.FirstOrDefault().PROVINCE_CODE;
                        var resultProvince = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.PROVINCE_CODE.Contains(result)).ToList();
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
        private void LoadHTProvinceCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboHTCommuneName.Properties.DataSource = null;
                    cboHTCommuneName.EditValue = null;
                    txtHTCommuneCode.Text = "";
                    cboHTDistrictName.Properties.DataSource = null;
                    cboHTDistrictName.EditValue = null;
                    txtHTDistrictCode.Text = "";
                    cboHTProvinceName.EditValue = null;
                    FocusShowPopup(cboHTProvinceName, gridView3);
                    //PopupLoader.SelectFirstRowPopup(cboProvince);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.SEARCH_CODE != null && o.SEARCH_CODE.Contains(searchCode) || (o.PROVINCE_NAME != null && o.PROVINCE_NAME.Contains(searchCode))).ToList();
                    if (listResult.Count == 1)
                    {
                        cboHTProvinceName.EditValue = listResult[0].PROVINCE_CODE;
                        txtHTProvinceCode.Text = listResult[0].SEARCH_CODE;
                        LoadHTDistrictsCombo("", listResult[0].PROVINCE_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtHTDistrictCode);
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboHTCommuneName.Properties.DataSource = null;
                        cboHTCommuneName.EditValue = null;
                        cboHTCommuneName.Text = "";
                        cboHTDistrictName.Properties.DataSource = null;
                        cboHTDistrictName.EditValue = null;
                        txtHTDistrictCode.Text = "";
                        cboHTProvinceName.EditValue = null;
                        if (isExpand)
                        {
                            cboHTProvinceName.Properties.DataSource = listResult;
                            FocusShowPopup(cboHTProvinceName, gridView3);
                        }
                    }
                    else
                    {
                        FocusShowPopup(cboHTProvinceName, gridView3);
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
                listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => ((o.SEARCH_CODE != null && o.SEARCH_CODE.Contains(searchCode)) || (o.DISTRICT_NAME != null && o.DISTRICT_NAME.Contains(searchCode))) && (provinceCode == "" || (o.PROVINCE_CODE != null && o.PROVINCE_CODE == provinceCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SEARCH_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DISTRICT_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DISTRICT_NAME", "DISTRICT_CODE", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboDistricts, listResult, controlEditorADO);

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    cboCommune.Properties.DataSource = null;
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    cboDistricts.EditValue = null;
                    FocusShowPopup(cboDistricts, gridView1);
                    //PopupProcess.SelectFirstRowPopup(cboDistricts);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboDistricts.EditValue = listResult[0].DISTRICT_CODE;
                        txtDistricts.Text = listResult[0].SEARCH_CODE;
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
                        cboDistricts.EditValue = null;
                        if (isExpand)
                        {
                            FocusShowPopup(cboDistricts, gridView1);
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

        public void LoadHTDistrictsCombo(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => ((o.SEARCH_CODE != null && o.SEARCH_CODE.Contains(searchCode)) || (o.DISTRICT_NAME != null && o.DISTRICT_NAME.Contains(searchCode))) && (provinceCode == "" || (o.PROVINCE_CODE != null && o.PROVINCE_CODE == provinceCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SEARCH_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DISTRICT_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DISTRICT_NAME", "DISTRICT_CODE", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboHTDistrictName, listResult, controlEditorADO);

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    cboHTCommuneName.Properties.DataSource = null;
                    cboHTCommuneName.EditValue = null;
                    txtHTCommuneCode.Text = "";
                    txtHTDistrictCode.Text = "";
                    cboHTDistrictName.EditValue = null;
                    FocusShowPopup(cboHTDistrictName, gridView4);
                    //PopupProcess.SelectFirstRowPopup(cboDistricts);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboHTDistrictName.EditValue = listResult[0].DISTRICT_CODE;
                        txtHTDistrictCode.Text = listResult[0].SEARCH_CODE;
                        LoadHTCommuneCombo("", listResult[0].DISTRICT_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtHTCommuneCode);
                        }
                    }

                    else if (listResult.Count > 1)
                    {
                        cboHTCommuneName.Properties.DataSource = null;
                        cboHTCommuneName.EditValue = null;
                        txtHTCommuneCode.Text = "";
                        cboHTDistrictName.EditValue = null;
                        if (isExpand)
                        {
                            cboHTDistrictName.Properties.DataSource = listResult;
                            FocusShowPopup(cboHTDistrictName, gridView4);
                            //PopupProcess.SelectFirstRowPopup(cboDistricts);
                        }
                    }
                    else
                    {
                        FocusShowPopup(cboHTDistrictName, gridView4);
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

                Inventec.Common.Logging.LogSystem.Debug("searchCode_:" + searchCode);
                Inventec.Common.Logging.LogSystem.Debug("districtCode_:" + districtCode);
                Inventec.Common.Logging.LogSystem.Debug("isExpand_:" + isExpand);

                listResult = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => ((o.SEARCH_CODE?? "").Contains(searchCode) || o.COMMUNE_NAME.Contains(searchCode)) && o.DISTRICT_CODE == districtCode).ToList();

                if (listResult != null)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("SEARCH_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("COMMUNE_NAME", "", 200, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("COMMUNE_NAME", "COMMUNE_CODE", columnInfos, false, 300);
                    controlEditorADO.ImmediatePopup = true;
                    ControlEditorLoader.Load(cboCommune, listResult, controlEditorADO);

                    if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                    {
                        cboCommune.EditValue = null;
                        txtCommune.Text = "";
                        FocusShowPopup(cboCommune, gridView2);
                        //PopupProcess.SelectFirstRowPopup(cboCommune);
                    }
                    else
                    {
                        if (listResult.Count == 1)
                        {
                            cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                            txtCommune.Text = listResult[0].SEARCH_CODE;
                            if (isExpand)
                            {
                                FocusMoveText(txtAddress);
                            }
                        }
                        else if (isExpand && listResult.Count > 1)
                        {
                            cboCommune.Properties.DataSource = listResult;
                            cboCommune.EditValue = null;
                            FocusShowPopup(cboCommune, gridView2);
                            // PopupProcess.SelectFirstRowPopup(cboCommune);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadHTCommuneCombo(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                listResult = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => (o.SEARCH_CODE.Contains(searchCode) || o.COMMUNE_NAME.Contains(searchCode)) && o.DISTRICT_CODE == districtCode).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SEARCH_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("COMMUNE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("COMMUNE_NAME", "COMMUNE_CODE", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboHTCommuneName, listResult, controlEditorADO);

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    cboHTCommuneName.EditValue = null;
                    txtHTCommuneCode.Text = "";
                    FocusShowPopup(cboHTCommuneName, gridView5);
                    //PopupProcess.SelectFirstRowPopup(cboCommune);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboHTCommuneName.EditValue = listResult[0].COMMUNE_CODE;
                        txtHTCommuneCode.Text = listResult[0].SEARCH_CODE;
                        if (isExpand)
                        {
                            FocusMoveText(txtHTAddress);
                        }
                    }
                    else if (isExpand && listResult.Count > 1)
                    {
                        cboHTCommuneName.Properties.DataSource = listResult;
                        cboHTCommuneName.EditValue = null;
                        FocusShowPopup(cboHTCommuneName, gridView5);
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
                filterWorkPlace.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //defaultworwplace = BackendDataWorker.Get<HIS_WORK_PLACE>();
                defaultworwplace = new BackendAdapter(param).Get<List<HIS_WORK_PLACE>>("api/HisWorkPlace/Get", ApiConsumers.MosConsumer, filterWorkPlace, param);

                WorkPlaceInitADO data = new WorkPlaceInitADO();
                data.Template = workPlaceTemplate;
                data.FocusMoveout = FocusMoveout;
                data.WorlPlaces = defaultworwplace;
                //data.SetValidateControl = validate;

                workPlaceProcessor.FocusControl(workPlaceTemplate);

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
                txtOtherAddress.Focus();
                txtOtherAddress.SelectAll();
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
                        if (txtPatientDob.Enabled)
                        {
                            txtPatientDob.Focus();
                            txtPatientDob.SelectAll();
                        }
                        else
                        {
                            txtNation.Focus();
                            txtNation.SelectAll();
                        }
                    }
                    else
                    {
                        cboGioiTinh.EditValue = null;
                        if (txtPatientDob.Enabled)
                        {
                            txtPatientDob.Focus();
                            txtPatientDob.SelectAll();
                        }
                        else
                        {
                            txtNation.Focus();
                            txtNation.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDanTocCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.GridLookUpEdit cboDanToc, DevExpress.XtraEditors.TextEdit txtMaDanToc, DevExpress.XtraEditors.TextEdit txtMaQuocTich)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDanToc.EditValue = null;
                    FocusShowPopup(cboDanToc, gridView7);
                    PopupLoader.SelectFirstRowPopup(cboDanToc);
                }
                else
                {
                    var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(o => o.ETHNIC_CODE.Contains(searchCode) || o.ETHNIC_NAME.Contains(searchCode)).ToList();
                    List<SDA.EFMODEL.DataModels.SDA_ETHNIC> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.ETHNIC_CODE == searchCode || o.ETHNIC_NAME == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboDanToc.EditValue = result[0].ETHNIC_CODE;
                        txtMaDanToc.Text = result[0].ETHNIC_CODE;
                        FocusMoveText(cboTonGiao);
                    }
                    else
                    {
                        cboDanToc.EditValue = null;
                        FocusShowPopup(cboDanToc, gridView7);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadQuocTichCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.GridLookUpEdit cboQuocTich, DevExpress.XtraEditors.TextEdit txtMaQuocTich, DevExpress.XtraEditors.PanelControl txtNoiLamViec)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboQuocTich.EditValue = null;
                    FocusShowPopup(cboQuocTich, gridView8);
                    PopupLoader.SelectFirstRowPopup(cboQuocTich);
                }
                else
                {
                    var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_CODE.ToUpper().Contains(searchCode) || o.NATIONAL_NAME.ToUpper().Contains(searchCode)).ToList();
                    List<SDA_NATIONAL> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.NATIONAL_CODE.ToUpper() == searchCode || o.NATIONAL_NAME.ToUpper() == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboQuocTich.EditValue = result[0].NATIONAL_CODE;
                        txtMaQuocTich.Text = result[0].NATIONAL_CODE;
                        txtTheBHYT.Focus();
                        txtTheBHYT.SelectAll();
                    }
                    else
                    {
                        cboQuocTich.EditValue = null;
                        FocusShowPopup(cboQuocTich, gridView8);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadNgheNghiepCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.GridLookUpEdit cboNgheNghiep, DevExpress.XtraEditors.TextEdit txtMaNgheNghiep)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboNgheNghiep.EditValue = null;
                    FocusShowPopup(cboNgheNghiep, gridView6);
                    PopupLoader.SelectFirstRowPopup(cboNgheNghiep);
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && ((o.CAREER_CODE != null ? o.CAREER_CODE.ToUpper() : "").Contains(searchCode) || (o.CAREER_NAME != null ? o.CAREER_NAME.ToUpper() : "").Contains(searchCode))).ToList();
                    List<HIS_CAREER> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => (o.CAREER_CODE != null ? o.CAREER_CODE.ToUpper() : "") == searchCode || (o.CAREER_NAME != null ? o.CAREER_NAME.ToUpper() : "").Contains(searchCode)).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboNgheNghiep.EditValue = result[0].ID;
                        txtMaNgheNghiep.Text = result[0].CAREER_CODE;
                        //if (workPlaceProcessor != null && workPlaceTemplate != null)
                        //    workPlaceProcessor.FocusControl(workPlaceTemplate);
                    }
                    else
                    {
                        cboNgheNghiep.EditValue = null;
                        FocusShowPopup(cboNgheNghiep, gridView6);
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

        void GetNotInListValue(object sender, GetNotInListValueEventArgs e, GridLookUpEdit cbo)
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
