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
using HIS.Desktop.Plugins.CreatePatientList;
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

namespace HIS.Desktop.Plugins.CreatePatientList
{
    public partial class frmCreatePatientList : Form
    {
        bool check = false;
        private void InitComboProvince(object control, List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PROVINCE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PROVINCE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PROVINCE_NAME", "PROVINCE_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDistrict(object control, List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DISTRICT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("RENDERER_DISTRICT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_DISTRICT_NAME", "DISTRICT_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommune(object control, List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("COMMUNE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("RENDERER_COMMUNE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_COMMUNE_NAME", "COMMUNE_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
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
                    cboProvince.Focus();
                    cboProvince.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(cboProvince);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE.ToUpper().Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                        txtProvince.Text = listResult[0].PROVINCE_CODE;
                        LoadDistrictsCombo("", listResult[0].PROVINCE_CODE, false);
                        if (isExpand)
                        {
                            txtDistricts.Focus();
                            txtDistricts.SelectAll();
                        }
                    }
                    else if (listResult.Count > 1)
                    //else
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
                            //cboProvince.Properties.DataSource = listResult;
                            cboProvince.Focus();
                            cboProvince.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(cboProvince);
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
                listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.DISTRICT_CODE.ToUpper().Contains(searchCode) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();

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
                    cboDistricts.Focus();
                    cboDistricts.ShowPopup();
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
                            txtCommune.Focus();
                            txtCommune.SelectAll();
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
                            cboDistricts.Focus();
                            cboDistricts.ShowPopup();
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
                listResult = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.COMMUNE_CODE.ToUpper().Contains(searchCode) && (districtCode == "" || o.DISTRICT_CODE == districtCode)).ToList();
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
                    cboCommune.Focus();
                    cboCommune.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(cboCommune);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                        txtCommune.Text = listResult[0].COMMUNE_CODE;
                        //cboTHX.EditValue = listResult[0].SEARCH_CODE;
                        //txtTHX.Text = listResult[0].SEARCH_CODE;
                        if (isExpand)
                        {
                            txtNation.Focus();
                            txtNation.SelectAll();
                        }
                    }
                    else if (isExpand && listResult.Count > 1)
                    {
                        cboCommune.EditValue = null;
                        cboCommune.Focus();
                        cboCommune.ShowPopup();
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
                if (GlobalVariables.CheDoHienThiNoiLamViecManHinhDangKyTiepDon == 1)
                {
                    workPlaceTemplate = WorkPlaceProcessor.Template.Textbox;
                }
                else
                {
                    workPlaceTemplate = WorkPlaceProcessor.Template.Combo;
                }
                List<HIS_WORK_PLACE> defaultworwplace = new List<HIS_WORK_PLACE>();
                HisWorkPlaceFilter filterWorkPlace = new HisWorkPlaceFilter();
                defaultworwplace = BackendDataWorker.Get<HIS_WORK_PLACE>(filterWorkPlace);

                WorkPlaceInitADO data = new WorkPlaceInitADO();
                data.Template = workPlaceTemplate;
                data.FocusMoveout = FocusMoveout;
                data.WorlPlaces = defaultworwplace;

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
        public static void LoadGioiTinhCombo(string searchCode, DevExpress.XtraEditors.LookUpEdit cboGioiTinh, DevExpress.XtraEditors.TextEdit txtMaGioiTinh, DevExpress.XtraEditors.ButtonEdit txtNgaySinh)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboGioiTinh.EditValue = null;
                    cboGioiTinh.Focus();
                    cboGioiTinh.ShowPopup();
                    PopupProcess.SelectFirstRowPopup(cboGioiTinh);
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().Where(o => o.GENDER_CODE.ToUpper().Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboGioiTinh.EditValue = data[0].ID;
                            txtMaGioiTinh.Text = data[0].GENDER_CODE;

                            txtNgaySinh.Focus();
                            txtNgaySinh.SelectAll();
                        }
                        else
                        {
                            cboGioiTinh.EditValue = null;
                            cboGioiTinh.Focus();
                            cboGioiTinh.ShowPopup();
                            PopupProcess.SelectFirstRowPopup(cboGioiTinh);
                        }
                    }
                    else
                    {
                        cboGioiTinh.EditValue = null;
                        cboGioiTinh.Focus();
                        cboGioiTinh.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public static void LoadDanTocCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboDanToc, DevExpress.XtraEditors.TextEdit txtMaDanToc, DevExpress.XtraEditors.TextEdit txtMaQuocTich)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDanToc.EditValue = null;
                    cboDanToc.Focus();
                    cboDanToc.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboDanToc);
                }
                else
                {
                    var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(o => o.ETHNIC_CODE.ToUpper().Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDanToc.EditValue = data[0].ETHNIC_NAME;
                            txtMaDanToc.Text = data[0].ETHNIC_CODE;
                            txtMaQuocTich.Focus();
                            txtMaQuocTich.SelectAll();
                        }
                        else
                        {
                            cboDanToc.EditValue = null;
                            cboDanToc.Focus();
                            cboDanToc.ShowPopup();
                        }
                    }
                    else
                    {
                        cboDanToc.EditValue = null;
                        cboDanToc.Focus();
                        cboDanToc.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
       
        public static void LoadNoiCapCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboNoiCap, DevExpress.XtraEditors.TextEdit txtMaNoiCap, DevExpress.XtraEditors.PanelControl txtNoiCap)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboNoiCap.EditValue = null;
                    cboNoiCap.Focus();
                    cboNoiCap.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboQuocTich);
                }
                else
                {
                    var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboNoiCap.EditValue = data[0].NATIONAL_NAME;
                            txtMaNoiCap.Text = data[0].NATIONAL_CODE;
                            txtNoiCap.Focus();
                        }
                        else
                        {
                            cboNoiCap.EditValue = null;
                            cboNoiCap.Focus();
                            cboNoiCap.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboQuocTich);
                        }
                    }
                    else
                    {
                        cboNoiCap.EditValue = null;
                        cboNoiCap.Focus();
                        cboNoiCap.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboQuocTich);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public static void LoadNgheNghiepCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboNgheNghiep, DevExpress.XtraEditors.TextEdit txtMaNgheNghiep, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboNgheNghiep.EditValue = null;
                    cboNgheNghiep.Focus();
                    cboNgheNghiep.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                }
                else
                {
                    //var data = HisDataLocalStore.HisCareers.Where(o => o.CAREER_CODE.Contains(searchCode)).ToList();
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.CAREER_CODE.ToUpper().Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboNgheNghiep.EditValue = data[0].ID;
                            txtMaNgheNghiep.Text = data[0].CAREER_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                        }
                        else
                        {
                            cboNgheNghiep.EditValue = null;
                            cboNgheNghiep.Focus();
                            cboNgheNghiep.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                        }
                    }
                    else
                    {
                        cboNgheNghiep.EditValue = null;
                        cboNgheNghiep.Focus();
                        cboNgheNghiep.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadRankCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboRank, DevExpress.XtraEditors.TextEdit txtRank, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboRank.EditValue = null;
                    cboRank.Focus();
                    cboRank.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                }
                else
                {
                    //var data = HisDataLocalStore.HisCareers.Where(o => o.CAREER_CODE.Contains(searchCode)).ToList();
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().Where(o => o.MILITARY_RANK_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboRank.EditValue = data[0].ID;
                            txtRank.Text = data[0].MILITARY_RANK_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                        }
                        else
                        {
                            cboRank.EditValue = null;
                            cboRank.Focus();
                            cboRank.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                        }
                    }
                    else
                    {
                        cboRank.EditValue = null;
                        cboRank.Focus();
                        cboRank.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public static void LoadTinhCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboTinh, DevExpress.XtraEditors.TextEdit txtTinh, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboTinh.EditValue = null;
                    cboTinh.Focus();
                    cboTinh.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                }
                else
                {
                    //var data = HisDataLocalStore.HisCareers.Where(o => o.CAREER_CODE.Contains(searchCode)).ToList();
                    var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboTinh.EditValue = data[0].ID;
                            txtTinh.Text = data[0].PROVINCE_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                        }
                        else
                        {
                            cboTinh.EditValue = null;
                            cboTinh.Focus();
                            cboTinh.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                        }
                    }
                    else
                    {
                        cboTinh.EditValue = null;
                        cboTinh.Focus();
                        cboTinh.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public static void LoadHuyenCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboHuyen, DevExpress.XtraEditors.TextEdit txtHuyen, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboHuyen.EditValue = null;
                    cboHuyen.Focus();
                    cboHuyen.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                }
                else
                {
                    //var data = HisDataLocalStore.HisCareers.Where(o => o.CAREER_CODE.Contains(searchCode)).ToList();
                    var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.DISTRICT_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboHuyen.EditValue = data[0].ID;
                            txtHuyen.Text = data[0].DISTRICT_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                        }
                        else
                        {
                            cboHuyen.EditValue = null;
                            cboHuyen.Focus();
                            cboHuyen.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                        }
                    }
                    else
                    {
                        cboHuyen.EditValue = null;
                        cboHuyen.Focus();
                        cboHuyen.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public static void LoadXaCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboXa, DevExpress.XtraEditors.TextEdit txtXa, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboXa.EditValue = null;
                    cboXa.Focus();
                    cboXa.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                }
                else
                {
                    //var data = HisDataLocalStore.HisCareers.Where(o => o.CAREER_CODE.Contains(searchCode)).ToList();
                    var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.COMMUNE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboXa.EditValue = data[0].ID;
                            txtXa.Text = data[0].COMMUNE_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                        }
                        else
                        {
                            cboXa.EditValue = null;
                            cboXa.Focus();
                            cboXa.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                        }
                    }
                    else
                    {
                        cboXa.EditValue = null;
                        cboXa.Focus();
                        cboXa.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadMilitaryRankCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMilitaryRank.EditValue = null;
                    cboMilitaryRank.Focus();
                    cboMilitaryRank.ShowPopup();
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().Where(o => o.MILITARY_RANK_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMilitaryRank.EditValue = data[0].ID;
                            txtMilitaryRankCode.Text = data[0].MILITARY_RANK_CODE;
                            txtPhone.Focus();
                            txtPhone.SelectAll();
                        }
                        else
                        {
                            cboMilitaryRank.EditValue = null;
                            cboMilitaryRank.Focus();
                            cboMilitaryRank.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //private void LoadNgayCapCombo(string searchCode)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(searchCode))
        //        {
        //            txtDR.EditValue = null;
        //            txtDR.Focus();
        //            txtDR.Show();
        //        }
        //        else
        //        {
        //            var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DATA_STORE>().Where(o => o.DATA_STORE_CODE.Contains(searchCode)).ToList();
        //            if (data != null)
        //            {
        //                if (data.Count == 1)
        //                {
        //                    txtDR.EditValue = data[0].ID;
        //                    txtDR.Text = data[0].DATA_STORE_CODE;
        //                    txtIB.Focus();
        //                    txtIB.SelectAll();
        //                }
        //                else
        //                {
        //                    txtDR.EditValue = null;
        //                    txtDR.Focus();
        //                    txtDR.ShowPopup();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        public static void LoadABOCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboAbo, DevExpress.XtraEditors.TextEdit txtAbo, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboAbo.EditValue = null;
                    cboAbo.Focus();
                    cboAbo.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                }
                else
                {
                    //var data = HisDataLocalStore.HisCareers.Where(o => o.CAREER_CODE.Contains(searchCode)).ToList();
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>().Where(o => o.BLOOD_ABO_CODE.ToUpper().Equals(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboAbo.EditValue = data[0].ID;
                            txtAbo.Text = data[0].BLOOD_ABO_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                        }
                        else
                        {
                            cboAbo.EditValue = null;
                            cboAbo.Focus();
                            cboAbo.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                        }
                    }
                    else
                    {
                        cboAbo.EditValue = null;
                        cboAbo.Focus();
                        cboAbo.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadQuocTichCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboQuocTich, DevExpress.XtraEditors.TextEdit txtMaQuocTich, ref bool check)
        {
            try
            {
                
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboQuocTich.EditValue = null;
                    cboQuocTich.Focus();
                    cboQuocTich.ShowPopup();
                    check = false;
                    //PopupProcess.SelectFirstRowPopup(control.cboQuocTich);
                }
                else
                {
                    var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_CODE.ToUpper().Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboQuocTich.EditValue = data[0].NATIONAL_NAME;
                            txtMaQuocTich.Text = data[0].NATIONAL_CODE;
                            check = true;
                            
                        }
                        else
                        {
                            cboQuocTich.EditValue = null;
                            cboQuocTich.Focus();
                            cboQuocTich.ShowPopup();
                            check = false;
                            //PopupProcess.SelectFirstRowPopup(control.cboQuocTich);
                        }
                    }
                    else
                    {
                        cboQuocTich.EditValue = null;
                        cboQuocTich.Focus();
                        cboQuocTich.ShowPopup();
                        check = false;
                        //PopupProcess.SelectFirstRowPopup(control.cboQuocTich);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadRHCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboAboRh, DevExpress.XtraEditors.TextEdit txtAboRh, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboAboRh.EditValue = null;
                    cboAboRh.Focus();
                    cboAboRh.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                }
                else
                {
                    //var data = HisDataLocalStore.HisCareers.Where(o => o.CAREER_CODE.Contains(searchCode)).ToList();
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>().Where(o => o.BLOOD_RH_CODE.ToUpper().Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboAboRh.EditValue = data[0].ID;
                            txtAboRh.Text = data[0].BLOOD_RH_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                        }
                        else
                        {
                            cboAboRh.EditValue = null;
                            cboAboRh.Focus();
                            cboAboRh.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                        }
                    }
                    else
                    {
                        cboAboRh.EditValue = null;
                        cboAboRh.Focus();
                        cboAboRh.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadBloodABOCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboBloodAbo.EditValue = null;
                    cboBloodAbo.Focus();
                    cboBloodAbo.ShowPopup();
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>().Where(o => o.BLOOD_ABO_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboBloodAbo.EditValue = data[0].ID;
                            txtBloodAbo.Text = data[0].BLOOD_ABO_CODE;
                            txtRh.Focus();
                            txtRh.SelectAll();
                        }
                        else
                        {
                            cboBloodAbo.EditValue = null;
                            cboBloodAbo.Focus();
                            cboBloodAbo.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    //    private void LoadRHCombo(string searchCode)
    //    {
    //        try
    //        {
    //            if (String.IsNullOrEmpty(searchCode))
    //            {
    //                cboRh.EditValue = null;
    //                cboRh.Focus();
    //                cboRh.ShowPopup();
    //            }
    //            else
    //            {
    //                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>().Where(o => o.BLOOD_RH_CODE.Contains(searchCode)).ToList();
    //                if (data != null)
    //                {
    //                    if (data.Count == 1)
    //                    {
    //                        cboRh.EditValue = data[0].ID;
    //                        txtPersonFamily.Focus();
    //                        txtPersonFamily.SelectAll();
    //                    }
    //                    else
    //                    {
    //                        cboRh.EditValue = null;
    //                        cboRh.Focus();
    //                        cboRh.ShowPopup();
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Inventec.Common.Logging.LogSystem.Warn(ex);
    //        }
    //    }
}
    public class PopupProcess
    {
        public static void SelectFirstRowPopup(LookUpEdit cbo)
        {
            try
            {
                if (cbo != null && cbo.IsPopupOpen)
                {
                    DevExpress.Utils.Win.IPopupControl popupEdit = cbo as DevExpress.Utils.Win.IPopupControl;
                    DevExpress.XtraEditors.Popup.PopupLookUpEditForm popupWindow = popupEdit.PopupWindow as DevExpress.XtraEditors.Popup.PopupLookUpEditForm;
                    if (popupWindow != null)
                    {
                        popupWindow.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void SelectFirstRowPopup(GridLookUpEdit cbo)
        {
            try
            {
                if (cbo != null && cbo.IsPopupOpen)
                {
                    DevExpress.Utils.Win.IPopupControl popupEdit = cbo as DevExpress.Utils.Win.IPopupControl;
                    DevExpress.XtraEditors.Popup.PopupLookUpEditForm popupWindow = popupEdit.PopupWindow as DevExpress.XtraEditors.Popup.PopupLookUpEditForm;
                    if (popupWindow != null)
                    {
                        popupWindow.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
    public class EventProcessor
    {
        public static void GetNotInListValue(object sender, GetNotInListValueEventArgs e, LookUpEdit cbo)
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
