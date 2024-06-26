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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using SCN.EFMODEL.DataModels;
using SCN.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ScnPersonalHealth
{
    public partial class frmPersonalHealth : HIS.Desktop.Utility.FormBase
    {
        string _PersonCode = "";
        SCN_BORN _Born { get; set; }
        List<SCN_HEALTH_RISK> _HealthRisks { get; set; }
        List<SCN_ALLERGIC> _Allergics { get; set; }
        List<SCN_DISABILITY> _Disabilitys { get; set; }
        List<SCN_DISEASE> _Diseases { get; set; }
        // SCN_SURGERY _Surgery { get; set; }
        SCN_REPRODUCTION _Reproduction { get; set; }

        List<ScnSurgeryADO> _ScnSurgeryADOs { get; set; }

        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_PATIENT _Patient { get; set; }

        int positionHandleControl = -1;

        public frmPersonalHealth()
        {
            InitializeComponent();
        }

        public frmPersonalHealth(Inventec.Desktop.Common.Modules.Module _currentModule, V_HIS_PATIENT _patient)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _currentModule;
                this._Patient = _patient;
                this._PersonCode = _patient.PERSON_CODE;// "001189000001";
                if (_patient != null && _patient.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                {
                    layoutControlGroupSucKhoeSinhSan.Expanded = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmPersonalHealth_Load(object sender, EventArgs e)
        {
            try
            {
                //ResourceMessage.languageMessage = new ResourceManager("HIS.Desktop.Plugins.ScnPersonalHealth.Resources.Lang", typeof(HIS.Desktop.Plugins.ScnPersonalHealth.frmPersonalHealth).Assembly);

                this.SetIcon();

                this.InitComboABORH();

                this.ValidationSingleControl(this.spinCanNang, this.dxValidationProvider1);

                this.ValidationSingleControl(this.spinChieuDai, this.dxValidationProvider1);

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                if (string.IsNullOrEmpty(this._PersonCode))
                {

                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl1.Items)
                    {
                        if (item is DevExpress.XtraLayout.LayoutControlGroup)
                        {
                            DevExpress.XtraLayout.LayoutControlGroup itemGroup = item as DevExpress.XtraLayout.LayoutControlGroup;
                            itemGroup.Enabled = false;

                        }
                    }
                    return;
                }
                WaitingManager.Show();
                getDataBorn();
                getDataHealthRisks();
                getDataAllergics();
                getDataDisabilitys();
                getDataDiseases();
                getDataSurgerys();
                getDataReproduction();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboABORH()
        {
            try
            {
                List<ColumnInfo> columnABOInfos = new List<ColumnInfo>();
                columnABOInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 1));
                ControlEditorADO controlABOEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "BLOOD_ABO_CODE", columnABOInfos, false, 250);
                ControlEditorLoader.Load(cboABO, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>(), controlABOEditorADO);

                List<ColumnInfo> columnRHInfos = new List<ColumnInfo>();
                columnRHInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 1));
                ControlEditorADO controlRHEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "BLOOD_RH_CODE", columnRHInfos, false, 250);
                ControlEditorLoader.Load(cboRH, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>(), controlRHEditorADO);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
        }

        #region ----- GetDatas -----
        private void getDataBorn()
        {
            try
            {
                this._Born = new SCN_BORN();
                this.spinCanNang.EditValue = null;
                this.spinChieuDai.EditValue = null;
                this.cboABO.EditValue = null;
                this.cboRH.EditValue = null;

                SCN.Filter.ScnBornFilter filter = new SCN.Filter.ScnBornFilter();
                filter.PERSON_CODE__EXACT = this._PersonCode;
                CommonParam param = new CommonParam();
                var resultData = ApiConsumers.ScnWrapConsumer.Get<List<SCN_BORN>>(true, "api/ScnBorn/Get", param, filter); //new BackendAdapter(param).Get<List<SCN_BORN>>("api/ScnBorn/Get", ApiConsumers.ScnConsumer, filter, param);
                if (resultData != null && resultData.Count > 0)
                {
                    this._Born = resultData.FirstOrDefault();

                    txtKhac.Text = this._Born.DESCRIPTION;
                    txtDiTatBamSinh.Text = this._Born.MALFORMATION;
                    spinCanNang.EditValue = this._Born.WEIGHT;
                    spinChieuDai.EditValue = this._Born.HEIGHT;

                    if (this._Born.IS_SURGERY == 1)
                        chkDeMo.Checked = true;
                    else
                        chkDethuong.Checked = true;

                    if (this._Born.IS_PREMATURELY == 1)
                    {
                        chkThieuThang.Checked = true;
                    }
                    if (this._Born.IS_SUFFOCATE == 1)
                    {
                        chkBiNgat.Checked = true;
                    }
                    if (!string.IsNullOrEmpty(this._Born.BLOOD_ABO_CODE))
                    {
                        //var dataABO = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(p => p.BLOOD_ABO_CODE == this._Born.BLOOD_ABO_CODE);
                        //if (dataABO != null)
                        cboABO.EditValue = this._Born.BLOOD_ABO_CODE;
                    }
                    if (!string.IsNullOrEmpty(this._Born.BLOOD_RH_CODE))
                    {
                        //var dataRH = BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(p => p.BLOOD_RH_CODE == this._Born.BLOOD_RH_CODE);
                        //if (dataRH != null)
                        cboRH.EditValue = this._Born.BLOOD_RH_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void getDataHealthRisks()
        {
            try
            {
                this._HealthRisks = new List<SCN_HEALTH_RISK>();
                SCN.Filter.ScnHealthRiskFilter filter = new SCN.Filter.ScnHealthRiskFilter();
                filter.PERSON_CODE__EXACT = this._PersonCode;
                CommonParam param = new CommonParam();
                this._HealthRisks = ApiConsumers.ScnWrapConsumer.Get<List<SCN_HEALTH_RISK>>(true, "api/ScnHealthRisk/Get", param, filter); // new BackendAdapter(param).Get<List<SCN_HEALTH_RISK>>("api/ScnHealthRisk/Get", ApiConsumers.ScnConsumer, filter, param);

                if (this._HealthRisks != null && this._HealthRisks.Count > 0)
                {
                    foreach (var item in this._HealthRisks)
                    {
                        if (item.HEALTH_RISK_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__THUOC_LA_LAO)
                        {
                            chkHutThuocLaCo.Checked = true;
                            if (item.IS_REGULARLY == 1)
                                chkHutThuocLaHutThuongXuyen.Checked = true;
                            if (item.IS_QUIT == 1)
                                chkHutThuocLaDaBo.Checked = true;
                        }
                        else if (item.HEALTH_RISK_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__RUOU_BIA)
                        {
                            chkRuouBiaCo.Checked = true;

                            spinRuouBiaSoLy.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(item.DESCRIPTION);

                            if (item.IS_QUIT == 1)
                                chkRuouBiaDaBo.Checked = true;
                        }
                        else if (item.HEALTH_RISK_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__MA_TUY)
                        {
                            chkMaTuyCo.Checked = true;
                            if (item.IS_REGULARLY == 1)
                                chkMaTuyThuongXuyen.Checked = true;
                            if (item.IS_QUIT == 1)
                                chkMaTuyDaBo.Checked = true;
                        }
                        else if (item.HEALTH_RISK_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__THE_LUC)
                        {
                            chkTheLucCo.Checked = true;
                            txtTheLucThuongXuyen.Text = item.DESCRIPTION;
                        }
                        else if (item.HEALTH_RISK_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__YEU_TO_TIEP_XUC)
                        {
                            txtYeuToTiepXuc.Text = item.DESCRIPTION;
                        }
                        else if (item.HEALTH_RISK_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__LOAI_HO_XI)
                        {
                            txtLoaiHoXiGiaDinh.Text = item.DESCRIPTION;
                        }
                        else if (item.HEALTH_RISK_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__KHAC)
                        {
                            txtB2Khac.Text = item.DESCRIPTION;
                        }
                    }
                }
                else
                {
                    chkHutThuocLaKhong.Checked = true;
                    chkHutThuocLaHutThuongXuyen.Checked = false;
                    chkHutThuocLaDaBo.Checked = false;
                    chkRuouBiaKhong.Checked = true;
                    spinRuouBiaSoLy.EditValue = null;
                    chkRuouBiaDaBo.Checked = false;
                    chkMaTuyKhong.Checked = true;
                    chkMaTuyThuongXuyen.Checked = false;
                    chkMaTuyDaBo.Checked = false;
                    chkTheLucKhong.Checked = true;
                    txtTheLucThuongXuyen.Text = "";
                    txtYeuToTiepXuc.Text = "";
                    txtLoaiHoXiGiaDinh.Text = "";
                    txtB2Khac.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void getDataAllergics()
        {
            try
            {
                this._Allergics = new List<SCN_ALLERGIC>();
                SCN.Filter.ScnAllergicFilter filter = new SCN.Filter.ScnAllergicFilter();
                filter.PERSON_CODE__EXACT = this._PersonCode;
                CommonParam param = new CommonParam();
                this._Allergics = ApiConsumers.ScnWrapConsumer.Get<List<SCN_ALLERGIC>>(true, "api/ScnAllergic/Get", param, filter); // new BackendAdapter(param).Get<List<SCN_ALLERGIC>>("api/ScnAllergic/Get", ApiConsumers.ScnConsumer, filter, param);
                if (this._Allergics != null && this._Allergics.Count > 0)
                {
                    foreach (var item in this._Allergics)
                    {
                        if (item.ALLERGIC_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_ALLERGIC_TYPE.ID__HCMP)
                        {
                            txtDiUngHoaChat.Text = item.DESCRIPTION;
                        }
                        else if (item.ALLERGIC_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_ALLERGIC_TYPE.ID__KHAC)
                        {
                            txtDiUngKhac.Text = item.DESCRIPTION;
                        }
                        else if (item.ALLERGIC_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_ALLERGIC_TYPE.ID__TP)
                        {
                            txtDiUngThucPham.Text = item.DESCRIPTION;
                        }
                        else if (item.ALLERGIC_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_ALLERGIC_TYPE.ID__TH)
                        {
                            txtDiUngThuoc.Text = item.DESCRIPTION;
                        }
                    }
                }
                else
                {
                    txtDiUngHoaChat.Text = "";
                    txtDiUngKhac.Text = "";
                    txtDiUngThucPham.Text = "";
                    txtDiUngThuoc.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void getDataDisabilitys()
        {
            try
            {
                this._Disabilitys = new List<SCN_DISABILITY>();
                SCN.Filter.ScnDisabilityFilter filter = new SCN.Filter.ScnDisabilityFilter();
                filter.PERSON_CODE__EXACT = this._PersonCode;
                CommonParam param = new CommonParam();
                this._Disabilitys = ApiConsumers.ScnWrapConsumer.Get<List<SCN_DISABILITY>>(true, "api/ScnDisability/Get", param, filter); //new BackendAdapter(param).Get<List<SCN_DISABILITY>>("api/ScnDisability/Get", ApiConsumers.ScnConsumer, filter, param);
                if (this._Disabilitys != null && this._Disabilitys.Count > 0)
                {
                    foreach (var item in this._Disabilitys)
                    {
                        if (item.DISABILITY_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__CONG_VEO_COT_SONG)
                        {
                            txtKhuyetTatCongVeoCotSong.Text = item.DESCRIPTION;
                        }
                        else if (item.DISABILITY_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__CHAN)
                        {
                            txtKhuyetTatChan.Text = item.DESCRIPTION;
                        }
                        else if (item.DISABILITY_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__KHAC)
                        {
                            txtKhuyetTatKhac.Text = item.DESCRIPTION;
                        }
                        else if (item.DISABILITY_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__KHE_HO_MOI_VOM_MIENG)
                        {
                            txtKhuyetTatKheHoVomMieng.Text = item.DESCRIPTION;
                        }
                        else if (item.DISABILITY_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__TAY)
                        {
                            txtKhuyetTatTay.Text = item.DESCRIPTION;
                        }
                        else if (item.DISABILITY_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__THI_LUC)
                        {
                            txtKhuyetTatThiLuc.Text = item.DESCRIPTION;
                        }
                        else if (item.DISABILITY_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__THINH_LUC)
                        {
                            txtKhuyetTatThinhLuc.Text = item.DESCRIPTION;
                        }
                    }
                }
                else
                {
                    txtKhuyetTatCongVeoCotSong.Text = "";
                    txtKhuyetTatChan.Text = "";
                    txtKhuyetTatKhac.Text = "";
                    txtKhuyetTatKheHoVomMieng.Text = "";
                    txtKhuyetTatTay.Text = "";
                    txtKhuyetTatThiLuc.Text = "";
                    txtKhuyetTatThinhLuc.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void getDataDiseases()
        {
            try
            {
                this._Diseases = new List<SCN_DISEASE>();
                SCN.Filter.ScnDiseaseFilter filter = new SCN.Filter.ScnDiseaseFilter();
                filter.PERSON_CODE__EXACT = this._PersonCode;
                CommonParam param = new CommonParam();
                this._Diseases = ApiConsumers.ScnWrapConsumer.Get<List<SCN_DISEASE>>(true, "api/ScnDisease/Get", param, filter); // new BackendAdapter(param).Get<List<SCN_DISEASE>>("api/ScnDisease/Get", ApiConsumers.ScnConsumer, filter, param);
                if (this._Diseases != null && this._Diseases.Count > 0)
                {
                    foreach (var item in this._Diseases)
                    {
                        if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__BUOU_CO)
                        {
                            chkBenhTatBuouCo.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__DA_DAY)
                        {
                            chkBenhTatDaDay.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__DAI_THAO_DUONG)
                        {
                            chkBenhTatDaiThaoDuong.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__DONG_KINH)
                        {
                            chkBenhTatDongKinh.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__HEN_SUYEN)
                        {
                            chkBenhTatHenSuyen.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__KHAC)
                        {
                            txtBenhTatKhac.Text = item.DESCRIPTION;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__LAO)
                        {
                            txtBenhTatLao.Text = item.DESCRIPTION;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__LAO)
                        {
                            txtBenhTatLao.Text = item.DESCRIPTION;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__PHOI_MAN_TINH)
                        {
                            chkBenhTatPhoiManTinh.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__TAM_THAN)
                        {
                            chkBenhTatTamThan.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__TANG_HUYET_AP)
                        {
                            chkBenhTatHuyetAp.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__TIM_BAM_SINH)
                        {
                            chkBenhTatTimBamSinh.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__TIM_MACH)
                        {
                            chkBenhTatTimMach.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__TU_KY)
                        {
                            chkBenhTatTuKy.Checked = true;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__UNG_THU)
                        {
                            txtBenhTatUngThu.Text = item.DESCRIPTION;
                        }
                        else if (item.DISEASE_TYPE_ID == IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__VIEM_GAN)
                        {
                            chkBenhTatViemGan.Checked = true;
                        }
                    }
                }
                else
                {
                    chkBenhTatBuouCo.Checked = false;
                    chkBenhTatDaDay.Checked = false;
                    chkBenhTatDaiThaoDuong.Checked = false;
                    chkBenhTatDongKinh.Checked = false;
                    chkBenhTatHenSuyen.Checked = false;
                    txtBenhTatKhac.Text = "";
                    txtBenhTatLao.Text = "";
                    chkBenhTatPhoiManTinh.Checked = false;
                    chkBenhTatTamThan.Checked = false;
                    chkBenhTatHuyetAp.Checked = false;
                    chkBenhTatTimBamSinh.Checked = false;
                    chkBenhTatTimMach.Checked = false;
                    chkBenhTatTuKy.Checked = false;
                    txtBenhTatUngThu.Text = "";
                    chkBenhTatViemGan.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void getDataSurgerys()
        {
            try
            {
                SCN.Filter.ScnSurgeryFilter filter = new SCN.Filter.ScnSurgeryFilter();
                filter.PERSON_CODE__EXACT = this._PersonCode;
                CommonParam param = new CommonParam();
                var datas = ApiConsumers.ScnWrapConsumer.Get<List<SCN_SURGERY>>(true, "api/ScnSurgery/Get", param, filter);

                if (datas != null && datas.Count > 0)
                {
                    this._ScnSurgeryADOs = new List<ScnSurgeryADO>();
                    int d = 1;
                    foreach (var itemSurgery in datas)
                    {
                        ScnSurgeryADO _Surgery = new ScnSurgeryADO(itemSurgery);
                        _Surgery.Action = d;
                        this._ScnSurgeryADOs.Add(_Surgery);
                        d++;
                    }
                }
                else
                {
                    this._ScnSurgeryADOs = new List<ScnSurgeryADO>();
                    ScnSurgeryADO _Surgery = new ScnSurgeryADO();
                    _Surgery.Action = 1;
                    this._ScnSurgeryADOs.Add(_Surgery);
                }
                gridControlSurgery.DataSource = null;
                gridControlSurgery.DataSource = this._ScnSurgeryADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void getDataReproduction()
        {
            try
            {
                if (layoutControlGroupSucKhoeSinhSan.Expanded)
                {
                    this.spinSoLanCoThai.EditValue = null;
                    this.spinSoLanSayThai.EditValue = null;
                    this.spinSoLanPhaThai.EditValue = null;
                    this.spinSoLanSinhDe.EditValue = null;
                    this.spinDeMo.EditValue = null;
                    this.spinDeKho.EditValue = null;
                    this.spinSoLanDeNon.EditValue = null;
                    this.spinSoConHienSong.EditValue = null;
                    this.spinDeThuong.EditValue = null;
                    this.spinSoLanDeDuThang.EditValue = null;

                    this._Reproduction = new SCN_REPRODUCTION();
                    SCN.Filter.ScnReproductionFilter filter = new SCN.Filter.ScnReproductionFilter();
                    filter.PERSON_CODE__EXACT = this._PersonCode;
                    CommonParam param = new CommonParam();
                    var datas = ApiConsumers.ScnWrapConsumer.Get<List<SCN_REPRODUCTION>>(true, "api/ScnReproduction/Get", param, filter); // new BackendAdapter(param).Get<List<SCN_REPRODUCTION>>("api/ScnReproduction/Get", ApiConsumers.ScnConsumer, filter, param);
                    if (datas != null && datas.Count > 0)
                    {
                        this._Reproduction = datas.FirstOrDefault();
                        txtBienPhapTranhThai.Text = this._Reproduction.CONTRACEPTION;
                        txtKyCoThaiCuoiCung.Text = this._Reproduction.PREGNANCY_LAST;
                        spinSoLanCoThai.EditValue = this._Reproduction.PREGNANCY_COUNT;
                        spinSoLanSayThai.EditValue = this._Reproduction.MISCARRIAGE_COUNT;
                        spinSoLanPhaThai.EditValue = this._Reproduction.ABORTION_COUNT;
                        spinSoLanSinhDe.EditValue = this._Reproduction.BORN_COUNT;
                        spinDeMo.EditValue = this._Reproduction.BORN_SURGERY_COUNT;
                        spinDeKho.EditValue = this._Reproduction.BORN_HARD_COUNT;
                        spinSoLanDeNon.EditValue = this._Reproduction.BORN_PREMATURELY_COUNT;
                        spinSoConHienSong.EditValue = this._Reproduction.BORN_LIVING_COUNT;
                        txtBenhPhuKhoa.Text = this._Reproduction.GYNECOLOGY_DISEASE;
                        if (this._Reproduction.PREGNANCY_COUNT.HasValue && this._Reproduction.PREGNANCY_COUNT > 0)
                        {
                            long? dem = this._Reproduction.PREGNANCY_COUNT;// -this._Reproduction.MISCARRIAGE_COUNT;
                            if (this._Reproduction.MISCARRIAGE_COUNT.HasValue)//SoLanSayThai
                            {
                                dem = dem - this._Reproduction.MISCARRIAGE_COUNT;
                            }
                            if (this._Reproduction.BORN_PREMATURELY_COUNT.HasValue)//SoLanDeNon
                            {
                                dem = dem - this._Reproduction.BORN_PREMATURELY_COUNT;
                            }
                            spinSoLanDeDuThang.EditValue = dem;
                            long? deThuong = dem;
                            if (this._Reproduction.BORN_SURGERY_COUNT.HasValue)//SoLanDeMo
                            {
                                deThuong = deThuong - this._Reproduction.BORN_SURGERY_COUNT;
                            }

                            spinDeThuong.EditValue = deThuong;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled)
                    return;

                btnSave.Focus();

                HID.EFMODEL.DataModels.HID_PERSON _HidPerson = new HID.EFMODEL.DataModels.HID_PERSON();
                CommonParam param = new CommonParam();
                _HidPerson = new Inventec.Common.Adapter.BackendAdapter(param).Get<HID.EFMODEL.DataModels.HID_PERSON>(
                    "/api/HidPerson/GetByPersonCode",
                HIS.Desktop.ApiConsumer.ApiConsumers.HidConsumer,
                this._PersonCode,
                param);

                HID.Filter.HidGenderFilter filterGender = new HID.Filter.HidGenderFilter();
                filterGender.ID = _HidPerson.GENDER_ID;
                var _HidGender = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HID.EFMODEL.DataModels.HID_GENDER>>(
                       "/api/HidGender/Get",
                   HIS.Desktop.ApiConsumer.ApiConsumers.HidConsumer,
                   filterGender,
                   param);
                string _GENDER_NAME = (_HidGender != null && _HidGender.Count > 0) ? _HidGender[0].GENDER_NAME : "";


                SCN.SDO.PersonalHealthSDO dataAdo = new SCN.SDO.PersonalHealthSDO();
                dataAdo.PERSON_CODE = this._PersonCode;

                #region --- TinhTrangLucDe
                if (chkDeMo.Checked || chkDethuong.Checked)
                {

                    SCN.EFMODEL.DataModels.SCN_BORN born = new SCN_BORN();

                    if (_HidPerson != null)
                    {
                        born.GENDER_NAME = _GENDER_NAME;
                        born.FIRST_NAME = _HidPerson.FIRST_NAME;
                        born.LAST_NAME = _HidPerson.LAST_NAME;
                        born.DOB = _HidPerson.DOB;
                        born.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        born.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        born.CAREER_NAME = _HidPerson.CAREER_NAME;
                        born.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    born.PERSON_CODE = this._PersonCode;
                    born.DESCRIPTION = txtKhac.Text;
                    born.MALFORMATION = txtDiTatBamSinh.Text;
                    born.HEIGHT = spinChieuDai.Value;
                    born.WEIGHT = spinCanNang.Value;

                    this.positionHandleControl = -1;
                    if (!this.dxValidationProvider1.Validate())
                        return;

                    WaitingManager.Show();
                    if (chkDeMo.Checked)
                    {
                        born.IS_SURGERY = 1;
                    }
                    if (chkThieuThang.Checked)
                    {
                        born.IS_PREMATURELY = 1;
                    }
                    if (chkBiNgat.Checked)
                    {
                        born.IS_SUFFOCATE = 1;
                    }
                    if (cboABO.EditValue != null)// && !string.IsNullOrEmpty(cboABO.Text))
                    {
                        born.BLOOD_ABO_CODE = cboABO.EditValue.ToString();// cboABO.Text;
                    }
                    if (cboRH.EditValue != null)// && !string.IsNullOrEmpty(cboRH.Text))
                    {
                        born.BLOOD_RH_CODE = cboRH.EditValue.ToString();//cboRH.Text;
                    }

                    dataAdo.Born = born;
                }
                #endregion

                #region --- DiUng
                dataAdo.Allergics = new List<SCN_ALLERGIC>();
                if (!string.IsNullOrEmpty(txtDiUngThuoc.Text))
                {
                    SCN_ALLERGIC adoThuoc = new SCN_ALLERGIC();
                    adoThuoc.ALLERGIC_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_ALLERGIC_TYPE.ID__TH;
                    adoThuoc.PERSON_CODE = this._PersonCode;
                    adoThuoc.DESCRIPTION = txtDiUngThuoc.Text;

                    if (_HidPerson != null)
                    {
                        adoThuoc.GENDER_NAME = _GENDER_NAME;
                        adoThuoc.FIRST_NAME = _HidPerson.FIRST_NAME;
                        adoThuoc.LAST_NAME = _HidPerson.LAST_NAME;
                        adoThuoc.DOB = _HidPerson.DOB;
                        adoThuoc.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        adoThuoc.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        adoThuoc.CAREER_NAME = _HidPerson.CAREER_NAME;
                        adoThuoc.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Allergics.Add(adoThuoc);
                }

                if (!string.IsNullOrEmpty(txtDiUngThucPham.Text))
                {
                    SCN_ALLERGIC adoThucPham = new SCN_ALLERGIC();
                    adoThucPham.ALLERGIC_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_ALLERGIC_TYPE.ID__TP;
                    adoThucPham.PERSON_CODE = this._PersonCode;
                    adoThucPham.DESCRIPTION = txtDiUngThucPham.Text;

                    if (_HidPerson != null)
                    {
                        adoThucPham.GENDER_NAME = _GENDER_NAME;
                        adoThucPham.FIRST_NAME = _HidPerson.FIRST_NAME;
                        adoThucPham.LAST_NAME = _HidPerson.LAST_NAME;
                        adoThucPham.DOB = _HidPerson.DOB;
                        adoThucPham.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        adoThucPham.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        adoThucPham.CAREER_NAME = _HidPerson.CAREER_NAME;
                        adoThucPham.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Allergics.Add(adoThucPham);
                }
                if (!string.IsNullOrEmpty(txtDiUngHoaChat.Text))
                {
                    SCN_ALLERGIC adoHoaChat = new SCN_ALLERGIC();
                    adoHoaChat.ALLERGIC_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_ALLERGIC_TYPE.ID__HCMP;
                    adoHoaChat.PERSON_CODE = this._PersonCode;
                    adoHoaChat.DESCRIPTION = txtDiUngHoaChat.Text;

                    if (_HidPerson != null)
                    {
                        adoHoaChat.GENDER_NAME = _GENDER_NAME;
                        adoHoaChat.FIRST_NAME = _HidPerson.FIRST_NAME;
                        adoHoaChat.LAST_NAME = _HidPerson.LAST_NAME;
                        adoHoaChat.DOB = _HidPerson.DOB;
                        adoHoaChat.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        adoHoaChat.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        adoHoaChat.CAREER_NAME = _HidPerson.CAREER_NAME;
                        adoHoaChat.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Allergics.Add(adoHoaChat);
                }

                if (!string.IsNullOrEmpty(txtDiUngKhac.Text))
                {
                    SCN_ALLERGIC adoKhac = new SCN_ALLERGIC();
                    adoKhac.ALLERGIC_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_ALLERGIC_TYPE.ID__KHAC;
                    adoKhac.PERSON_CODE = this._PersonCode;
                    adoKhac.DESCRIPTION = txtDiUngKhac.Text;

                    if (_HidPerson != null)
                    {
                        adoKhac.GENDER_NAME = _GENDER_NAME;
                        adoKhac.FIRST_NAME = _HidPerson.FIRST_NAME;
                        adoKhac.LAST_NAME = _HidPerson.LAST_NAME;
                        adoKhac.DOB = _HidPerson.DOB;
                        adoKhac.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        adoKhac.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        adoKhac.CAREER_NAME = _HidPerson.CAREER_NAME;
                        adoKhac.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Allergics.Add(adoKhac);
                }

                #endregion

                #region --- YeuToNguyCoVoiSucKhoeCaNhan
                dataAdo.HealthRisks = new List<SCN_HEALTH_RISK>();

                if (chkHutThuocLaCo.Checked)
                {
                    SCN_HEALTH_RISK healthThuocLa = new SCN_HEALTH_RISK();
                    healthThuocLa.HEALTH_RISK_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__THUOC_LA_LAO;
                    healthThuocLa.PERSON_CODE = this._PersonCode;
                    if (chkHutThuocLaHutThuongXuyen.Checked)
                    {
                        healthThuocLa.IS_REGULARLY = 1;
                    }
                    if (chkHutThuocLaDaBo.Checked)
                    {
                        healthThuocLa.IS_QUIT = 1;
                    }

                    if (_HidPerson != null)
                    {
                        healthThuocLa.GENDER_NAME = _GENDER_NAME;
                        healthThuocLa.FIRST_NAME = _HidPerson.FIRST_NAME;
                        healthThuocLa.LAST_NAME = _HidPerson.LAST_NAME;
                        healthThuocLa.DOB = _HidPerson.DOB;
                        healthThuocLa.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        healthThuocLa.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        healthThuocLa.CAREER_NAME = _HidPerson.CAREER_NAME;
                        healthThuocLa.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.HealthRisks.Add(healthThuocLa);
                }

                if (chkRuouBiaCo.Checked)
                {
                    SCN_HEALTH_RISK healthRuouBia = new SCN_HEALTH_RISK();
                    healthRuouBia.HEALTH_RISK_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__RUOU_BIA;
                    healthRuouBia.PERSON_CODE = this._PersonCode;
                    healthRuouBia.DESCRIPTION = spinRuouBiaSoLy.Text;
                    if (chkRuouBiaDaBo.Checked)
                    {
                        healthRuouBia.IS_QUIT = 1;
                    }
                    if (_HidPerson != null)
                    {
                        healthRuouBia.GENDER_NAME = _GENDER_NAME;
                        healthRuouBia.FIRST_NAME = _HidPerson.FIRST_NAME;
                        healthRuouBia.LAST_NAME = _HidPerson.LAST_NAME;
                        healthRuouBia.DOB = _HidPerson.DOB;
                        healthRuouBia.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        healthRuouBia.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        healthRuouBia.CAREER_NAME = _HidPerson.CAREER_NAME;
                        healthRuouBia.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.HealthRisks.Add(healthRuouBia);
                }

                if (chkMaTuyCo.Checked)
                {
                    SCN_HEALTH_RISK healthMatuy = new SCN_HEALTH_RISK();
                    healthMatuy.HEALTH_RISK_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__MA_TUY;
                    healthMatuy.PERSON_CODE = this._PersonCode;
                    if (chkMaTuyThuongXuyen.Checked)
                    {
                        healthMatuy.IS_REGULARLY = 1;
                    }
                    if (chkMaTuyDaBo.Checked)
                    {
                        healthMatuy.IS_QUIT = 1;
                    }
                    if (_HidPerson != null)
                    {
                        healthMatuy.GENDER_NAME = _GENDER_NAME;
                        healthMatuy.FIRST_NAME = _HidPerson.FIRST_NAME;
                        healthMatuy.LAST_NAME = _HidPerson.LAST_NAME;
                        healthMatuy.DOB = _HidPerson.DOB;
                        healthMatuy.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        healthMatuy.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        healthMatuy.CAREER_NAME = _HidPerson.CAREER_NAME;
                        healthMatuy.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.HealthRisks.Add(healthMatuy);
                }

                if (chkTheLucCo.Checked)
                {
                    SCN_HEALTH_RISK healthTheLuc = new SCN_HEALTH_RISK();
                    healthTheLuc.HEALTH_RISK_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__THE_LUC;
                    healthTheLuc.PERSON_CODE = this._PersonCode;
                    healthTheLuc.DESCRIPTION = txtTheLucThuongXuyen.Text;

                    if (_HidPerson != null)
                    {
                        healthTheLuc.GENDER_NAME = _GENDER_NAME;
                        healthTheLuc.FIRST_NAME = _HidPerson.FIRST_NAME;
                        healthTheLuc.LAST_NAME = _HidPerson.LAST_NAME;
                        healthTheLuc.DOB = _HidPerson.DOB;
                        healthTheLuc.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        healthTheLuc.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        healthTheLuc.CAREER_NAME = _HidPerson.CAREER_NAME;
                        healthTheLuc.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.HealthRisks.Add(healthTheLuc);
                }

                if (!string.IsNullOrEmpty(txtYeuToTiepXuc.Text))
                {
                    SCN_HEALTH_RISK healthYeuToTiepXucNN = new SCN_HEALTH_RISK();
                    healthYeuToTiepXucNN.HEALTH_RISK_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__YEU_TO_TIEP_XUC;
                    healthYeuToTiepXucNN.PERSON_CODE = this._PersonCode;
                    healthYeuToTiepXucNN.DESCRIPTION = txtYeuToTiepXuc.Text;

                    if (_HidPerson != null)
                    {
                        healthYeuToTiepXucNN.GENDER_NAME = _GENDER_NAME;
                        healthYeuToTiepXucNN.FIRST_NAME = _HidPerson.FIRST_NAME;
                        healthYeuToTiepXucNN.LAST_NAME = _HidPerson.LAST_NAME;
                        healthYeuToTiepXucNN.DOB = _HidPerson.DOB;
                        healthYeuToTiepXucNN.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        healthYeuToTiepXucNN.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        healthYeuToTiepXucNN.CAREER_NAME = _HidPerson.CAREER_NAME;
                        healthYeuToTiepXucNN.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.HealthRisks.Add(healthYeuToTiepXucNN);
                }
                if (!string.IsNullOrEmpty(txtLoaiHoXiGiaDinh.Text))
                {
                    SCN_HEALTH_RISK healthLoai = new SCN_HEALTH_RISK();
                    healthLoai.HEALTH_RISK_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__LOAI_HO_XI;
                    healthLoai.PERSON_CODE = this._PersonCode;
                    healthLoai.DESCRIPTION = txtLoaiHoXiGiaDinh.Text;

                    if (_HidPerson != null)
                    {
                        healthLoai.GENDER_NAME = _GENDER_NAME;
                        healthLoai.FIRST_NAME = _HidPerson.FIRST_NAME;
                        healthLoai.LAST_NAME = _HidPerson.LAST_NAME;
                        healthLoai.DOB = _HidPerson.DOB;
                        healthLoai.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        healthLoai.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        healthLoai.CAREER_NAME = _HidPerson.CAREER_NAME;
                        healthLoai.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.HealthRisks.Add(healthLoai);
                }
                if (!string.IsNullOrEmpty(txtB2Khac.Text))
                {
                    SCN_HEALTH_RISK healthKhac = new SCN_HEALTH_RISK();
                    healthKhac.HEALTH_RISK_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_HEALTH_RISK_TYPE.ID__KHAC;
                    healthKhac.PERSON_CODE = this._PersonCode;
                    healthKhac.DESCRIPTION = txtB2Khac.Text;

                    if (_HidPerson != null)
                    {
                        healthKhac.GENDER_NAME = _GENDER_NAME;
                        healthKhac.FIRST_NAME = _HidPerson.FIRST_NAME;
                        healthKhac.LAST_NAME = _HidPerson.LAST_NAME;
                        healthKhac.DOB = _HidPerson.DOB;
                        healthKhac.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        healthKhac.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        healthKhac.CAREER_NAME = _HidPerson.CAREER_NAME;
                        healthKhac.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.HealthRisks.Add(healthKhac);
                }
                #endregion

                #region --- KhuyetTat
                dataAdo.Disabilitys = new List<SCN_DISABILITY>();
                if (!string.IsNullOrEmpty(txtKhuyetTatTay.Text))
                {
                    SCN_DISABILITY disabilityTay = new SCN_DISABILITY();
                    disabilityTay.PERSON_CODE = this._PersonCode;
                    disabilityTay.DISABILITY_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__TAY;
                    disabilityTay.DESCRIPTION = txtKhuyetTatTay.Text;

                    if (_HidPerson != null)
                    {
                        disabilityTay.GENDER_NAME = _GENDER_NAME;
                        disabilityTay.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disabilityTay.LAST_NAME = _HidPerson.LAST_NAME;
                        disabilityTay.DOB = _HidPerson.DOB;
                        disabilityTay.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disabilityTay.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disabilityTay.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disabilityTay.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Disabilitys.Add(disabilityTay);
                }
                if (!string.IsNullOrEmpty(txtKhuyetTatChan.Text))
                {
                    SCN_DISABILITY disabilityChan = new SCN_DISABILITY();
                    disabilityChan.PERSON_CODE = this._PersonCode;
                    disabilityChan.DISABILITY_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__CHAN;
                    disabilityChan.DESCRIPTION = txtKhuyetTatChan.Text;

                    if (_HidPerson != null)
                    {
                        disabilityChan.GENDER_NAME = _GENDER_NAME;
                        disabilityChan.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disabilityChan.LAST_NAME = _HidPerson.LAST_NAME;
                        disabilityChan.DOB = _HidPerson.DOB;
                        disabilityChan.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disabilityChan.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disabilityChan.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disabilityChan.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Disabilitys.Add(disabilityChan);
                }
                if (!string.IsNullOrEmpty(txtKhuyetTatThiLuc.Text))
                {
                    SCN_DISABILITY disabilityThiLuc = new SCN_DISABILITY();
                    disabilityThiLuc.PERSON_CODE = this._PersonCode;
                    disabilityThiLuc.DISABILITY_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__THI_LUC;
                    disabilityThiLuc.DESCRIPTION = txtKhuyetTatThiLuc.Text;

                    if (_HidPerson != null)
                    {
                        disabilityThiLuc.GENDER_NAME = _GENDER_NAME;
                        disabilityThiLuc.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disabilityThiLuc.LAST_NAME = _HidPerson.LAST_NAME;
                        disabilityThiLuc.DOB = _HidPerson.DOB;
                        disabilityThiLuc.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disabilityThiLuc.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disabilityThiLuc.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disabilityThiLuc.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Disabilitys.Add(disabilityThiLuc);
                }
                if (!string.IsNullOrEmpty(txtKhuyetTatThinhLuc.Text))
                {
                    SCN_DISABILITY disabilityThinhLuc = new SCN_DISABILITY();
                    disabilityThinhLuc.PERSON_CODE = this._PersonCode;
                    disabilityThinhLuc.DISABILITY_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__THINH_LUC;
                    disabilityThinhLuc.DESCRIPTION = txtKhuyetTatThinhLuc.Text;

                    if (_HidPerson != null)
                    {
                        disabilityThinhLuc.GENDER_NAME = _GENDER_NAME;
                        disabilityThinhLuc.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disabilityThinhLuc.LAST_NAME = _HidPerson.LAST_NAME;
                        disabilityThinhLuc.DOB = _HidPerson.DOB;
                        disabilityThinhLuc.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disabilityThinhLuc.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disabilityThinhLuc.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disabilityThinhLuc.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Disabilitys.Add(disabilityThinhLuc);
                }
                if (!string.IsNullOrEmpty(txtKhuyetTatCongVeoCotSong.Text))
                {
                    SCN_DISABILITY disabilityCongVeoCS = new SCN_DISABILITY();
                    disabilityCongVeoCS.PERSON_CODE = this._PersonCode;
                    disabilityCongVeoCS.DISABILITY_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__CONG_VEO_COT_SONG;
                    disabilityCongVeoCS.DESCRIPTION = txtKhuyetTatCongVeoCotSong.Text;

                    if (_HidPerson != null)
                    {
                        disabilityCongVeoCS.GENDER_NAME = _GENDER_NAME;
                        disabilityCongVeoCS.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disabilityCongVeoCS.LAST_NAME = _HidPerson.LAST_NAME;
                        disabilityCongVeoCS.DOB = _HidPerson.DOB;
                        disabilityCongVeoCS.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disabilityCongVeoCS.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disabilityCongVeoCS.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disabilityCongVeoCS.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Disabilitys.Add(disabilityCongVeoCS);
                }
                if (!string.IsNullOrEmpty(txtKhuyetTatKheHoVomMieng.Text))
                {
                    SCN_DISABILITY disabilityKheHoMoi = new SCN_DISABILITY();
                    disabilityKheHoMoi.PERSON_CODE = this._PersonCode;
                    disabilityKheHoMoi.DISABILITY_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__KHE_HO_MOI_VOM_MIENG;
                    disabilityKheHoMoi.DESCRIPTION = txtKhuyetTatKheHoVomMieng.Text;

                    if (_HidPerson != null)
                    {
                        disabilityKheHoMoi.GENDER_NAME = _GENDER_NAME;
                        disabilityKheHoMoi.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disabilityKheHoMoi.LAST_NAME = _HidPerson.LAST_NAME;
                        disabilityKheHoMoi.DOB = _HidPerson.DOB;
                        disabilityKheHoMoi.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disabilityKheHoMoi.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disabilityKheHoMoi.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disabilityKheHoMoi.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Disabilitys.Add(disabilityKheHoMoi);
                }
                if (!string.IsNullOrEmpty(txtKhuyetTatKhac.Text))
                {
                    SCN_DISABILITY disabilityKhac = new SCN_DISABILITY();
                    disabilityKhac.PERSON_CODE = this._PersonCode;
                    disabilityKhac.DISABILITY_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISABILITY_TYPE.ID__KHAC;
                    disabilityKhac.DESCRIPTION = txtKhuyetTatKhac.Text;

                    if (_HidPerson != null)
                    {
                        disabilityKhac.GENDER_NAME = _GENDER_NAME;
                        disabilityKhac.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disabilityKhac.LAST_NAME = _HidPerson.LAST_NAME;
                        disabilityKhac.DOB = _HidPerson.DOB;
                        disabilityKhac.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disabilityKhac.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disabilityKhac.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disabilityKhac.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Disabilitys.Add(disabilityKhac);
                }
                #endregion

                #region --- BenhTat
                dataAdo.Diseases = new List<SCN_DISEASE>();
                if (chkBenhTatTimMach.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__TIM_MACH;
                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatViemGan.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__VIEM_GAN;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatTimBamSinh.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__TIM_BAM_SINH;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatHuyetAp.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__TANG_HUYET_AP;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatHenSuyen.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__HEN_SUYEN;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatTamThan.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__TAM_THAN;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatDaiThaoDuong.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__DAI_THAO_DUONG;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatBuouCo.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__BUOU_CO;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatTuKy.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__TU_KY;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatDaDay.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__DA_DAY;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatPhoiManTinh.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__PHOI_MAN_TINH;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (chkBenhTatDongKinh.Checked)
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__DONG_KINH;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (!string.IsNullOrEmpty(txtBenhTatUngThu.Text))
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__UNG_THU;
                    disease.DESCRIPTION = txtBenhTatUngThu.Text;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (!string.IsNullOrEmpty(txtBenhTatLao.Text))
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__LAO;
                    disease.DESCRIPTION = txtBenhTatLao.Text;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }
                if (!string.IsNullOrEmpty(txtBenhTatKhac.Text))
                {
                    SCN_DISEASE disease = new SCN_DISEASE();
                    disease.PERSON_CODE = this._PersonCode;
                    disease.DISEASE_TYPE_ID = IMSys.DbConfig.SCN_RS.SCN_DISEASE_TYPE.ID__KHAC;
                    disease.DESCRIPTION = txtBenhTatKhac.Text;

                    if (_HidPerson != null)
                    {
                        disease.GENDER_NAME = _GENDER_NAME;
                        disease.FIRST_NAME = _HidPerson.FIRST_NAME;
                        disease.LAST_NAME = _HidPerson.LAST_NAME;
                        disease.DOB = _HidPerson.DOB;
                        disease.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                        disease.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                        disease.CAREER_NAME = _HidPerson.CAREER_NAME;
                        disease.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                    }

                    dataAdo.Diseases.Add(disease);
                }

                #endregion

                #region --- TienSuPhauThuat
                dataAdo.Surgerys = new List<SCN_SURGERY>();
                if (this._ScnSurgeryADOs != null && this._ScnSurgeryADOs.Count > 0)
                {
                    foreach (var item in this._ScnSurgeryADOs)
                    {
                        if (!string.IsNullOrEmpty(item.SURGERY_PART))
                        {
                            SCN.EFMODEL.DataModels.SCN_SURGERY _Surgery = new SCN_SURGERY();
                            _Surgery.ID = item.ID;
                            _Surgery.PERSON_CODE = this._PersonCode;
                            _Surgery.SURGERY_PART = item.SURGERY_PART;
                            _Surgery.DESCRIPTION = item.DESCRIPTION;
                            if (item.Time != null)
                            {
                                _Surgery.SURGERY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.Time);
                            }

                            if (_HidPerson != null)
                            {
                                _Surgery.GENDER_NAME = _GENDER_NAME;
                                _Surgery.FIRST_NAME = _HidPerson.FIRST_NAME;
                                _Surgery.LAST_NAME = _HidPerson.LAST_NAME;
                                _Surgery.DOB = _HidPerson.DOB;
                                _Surgery.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                                _Surgery.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                                _Surgery.CAREER_NAME = _HidPerson.CAREER_NAME;
                                _Surgery.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                            }

                            dataAdo.Surgerys.Add(_Surgery);
                        }
                    }
                }

                #endregion

                #region SucKhoeSinhSanVaKeHoachHoaGiaDinh
                if (layoutControlGroupSucKhoeSinhSan.Expanded)
                {
                    if (!string.IsNullOrEmpty(txtBienPhapTranhThai.Text)
                        || !string.IsNullOrEmpty(txtKyCoThaiCuoiCung.Text)
                        || !string.IsNullOrEmpty(txtBenhPhuKhoa.Text)
                        || spinSoLanCoThai.EditValue != null
                        || spinSoLanSayThai.EditValue != null
                        || spinSoLanPhaThai.EditValue != null
                        || spinSoLanSinhDe.EditValue != null
                        || spinDeMo.EditValue != null
                        || spinDeKho.EditValue != null
                        || spinSoLanDeNon.EditValue != null
                        || spinSoConHienSong.EditValue != null)
                    {
                        SCN_REPRODUCTION reproduction = new SCN_REPRODUCTION();
                        reproduction.PERSON_CODE = this._PersonCode;
                        if (!string.IsNullOrEmpty(txtBienPhapTranhThai.Text))
                            reproduction.CONTRACEPTION = txtBienPhapTranhThai.Text;
                        if (!string.IsNullOrEmpty(txtKyCoThaiCuoiCung.Text))
                            reproduction.PREGNANCY_LAST = txtKyCoThaiCuoiCung.Text;
                        if (spinSoLanCoThai.EditValue != null)
                            reproduction.PREGNANCY_COUNT = (long)spinSoLanCoThai.Value;
                        if (spinSoLanSayThai.EditValue != null)
                            reproduction.MISCARRIAGE_COUNT = (long)spinSoLanSayThai.Value;
                        if (spinSoLanPhaThai.EditValue != null)
                            reproduction.ABORTION_COUNT = (long)spinSoLanPhaThai.Value;
                        if (spinSoLanSinhDe.EditValue != null)
                            reproduction.BORN_COUNT = (long)spinSoLanSinhDe.Value;
                        //reproduction.BORN_COUNT = (long)spinDeThuong.Value;//Chua co so lan de thuong
                        if (spinDeMo.EditValue != null)
                            reproduction.BORN_SURGERY_COUNT = (long)spinDeMo.Value;
                        if (spinDeKho.EditValue != null)
                            reproduction.BORN_HARD_COUNT = (long)spinDeKho.Value;
                        //reproduction.BORN_HARD_COUNT = (long)spinDeKho.Value;//Chua co so lan de du thang
                        if (spinSoLanDeNon.EditValue != null)
                            reproduction.BORN_PREMATURELY_COUNT = (long)spinSoLanDeNon.Value;
                        if (spinSoConHienSong.EditValue != null)
                            reproduction.BORN_LIVING_COUNT = (long)spinSoConHienSong.Value;
                        if (!string.IsNullOrEmpty(txtBenhPhuKhoa.Text))
                            reproduction.GYNECOLOGY_DISEASE = txtBenhPhuKhoa.Text;

                        if (_HidPerson != null)
                        {
                            reproduction.GENDER_NAME = _GENDER_NAME;
                            reproduction.FIRST_NAME = _HidPerson.FIRST_NAME;
                            reproduction.LAST_NAME = _HidPerson.LAST_NAME;
                            reproduction.DOB = _HidPerson.DOB;
                            reproduction.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                            reproduction.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                            reproduction.CAREER_NAME = _HidPerson.CAREER_NAME;
                            reproduction.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                        }
                        dataAdo.Reproduction = reproduction;
                    }
                }
                #endregion

                bool success = false;
                // var resultData = new BackendAdapter(param).Post<SCN.SDO.PersonalHealthSDO>("api/ScnPersonalHealth/Create", ApiConsumers.ScnConsumer, dataAdo, param);

                var resultData = ApiConsumers.ScnWrapConsumer.Post<PersonalHealthSDO>(true, "api/ScnPersonalHealth/Create", param, dataAdo);

                WaitingManager.Hide();
                if (resultData != null)
                {
                    success = true;
                }
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region ----- EventArgs -----
        private void chkHutThuocLaCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkHutThuocLaCo.Checked)
                {
                    chkHutThuocLaHutThuongXuyen.Enabled = true;
                    chkHutThuocLaDaBo.Enabled = true;
                }
                else
                {
                    chkHutThuocLaHutThuongXuyen.Enabled = false;
                    chkHutThuocLaDaBo.Enabled = false;
                    chkHutThuocLaDaBo.Checked = false;
                    chkHutThuocLaHutThuongXuyen.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkRuouBiaCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkRuouBiaCo.Checked)
                {
                    spinRuouBiaSoLy.Enabled = true;
                    chkRuouBiaDaBo.Enabled = true;
                }
                else
                {
                    spinRuouBiaSoLy.Enabled = false;
                    chkRuouBiaDaBo.Enabled = false;
                    spinRuouBiaSoLy.EditValue = null;
                    chkRuouBiaDaBo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMaTuyCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkMaTuyCo.Checked)
                {
                    chkMaTuyThuongXuyen.Enabled = true;
                    chkMaTuyDaBo.Enabled = true;
                }
                else
                {
                    chkMaTuyThuongXuyen.Enabled = false;
                    chkMaTuyDaBo.Enabled = false;
                    chkMaTuyThuongXuyen.Checked = false;
                    chkMaTuyDaBo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTheLucCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkTheLucCo.Checked)
                {
                    txtTheLucThuongXuyen.Enabled = true;
                }
                else
                {
                    txtTheLucThuongXuyen.Enabled = false;
                    txtTheLucThuongXuyen.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinCanNang_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinChieuDai.Focus();
                    spinChieuDai.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinChieuDai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboABO.Focus();
                    cboABO.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiTatBamSinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKhac.Focus();
                    txtKhac.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKhac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHutThuocLaKhong.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHutThuocLaKhong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHutThuocLaCo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHutThuocLaCo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHutThuocLaHutThuongXuyen.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHutThuocLaHutThuongXuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHutThuocLaDaBo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHutThuocLaDaBo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkRuouBiaKhong.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkRuouBiaKhong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkRuouBiaCo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkRuouBiaCo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinRuouBiaSoLy.Focus();
                    spinRuouBiaSoLy.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinRuouBiaSoLy_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkRuouBiaDaBo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkRuouBiaDaBo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkMaTuyKhong.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMaTuyKhong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkMaTuyCo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMaTuyCo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkMaTuyThuongXuyen.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMaTuyThuongXuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkMaTuyDaBo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMaTuyDaBo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkTheLucKhong.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTheLucKhong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkTheLucCo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTheLucCo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTheLucThuongXuyen.Focus();
                    txtTheLucThuongXuyen.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTheLucThuongXuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtYeuToTiepXuc.Focus();
                    txtYeuToTiepXuc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtYeuToTiepXuc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLoaiHoXiGiaDinh.Focus();
                    txtLoaiHoXiGiaDinh.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLoaiHoXiGiaDinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtB2Khac.Focus();
                    txtB2Khac.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtB2Khac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKhuyetTatTay.Focus();
                    txtKhuyetTatTay.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKhuyetTatTay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKhuyetTatChan.Focus();
                    txtKhuyetTatChan.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKhuyetTatChan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKhuyetTatThiLuc.Focus();
                    txtKhuyetTatThiLuc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKhuyetTatThiLuc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKhuyetTatThinhLuc.Focus();
                    txtKhuyetTatThinhLuc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKhuyetTatThinhLuc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKhuyetTatCongVeoCotSong.Focus();
                    txtKhuyetTatCongVeoCotSong.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKhuyetTatCongVeoCotSong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKhuyetTatKheHoVomMieng.Focus();
                    txtKhuyetTatKheHoVomMieng.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKhuyetTatKheHoVomMieng_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKhuyetTatKhac.Focus();
                    txtKhuyetTatKhac.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKhuyetTatKhac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiUngThuoc.Focus();
                    txtDiUngThuoc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiUngThuoc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiUngThucPham.Focus();
                    txtDiUngThucPham.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiUngThucPham_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiUngHoaChat.Focus();
                    txtDiUngHoaChat.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiUngHoaChat_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiUngKhac.Focus();
                    txtDiUngKhac.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiUngKhac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatTimMach.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatTimMach_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatHuyetAp.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatHuyetAp_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatDaiThaoDuong.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatDaiThaoDuong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatDaDay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatDaDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatViemGan.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatViemGan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatHenSuyen.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatHenSuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatBuouCo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatBuouCo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatPhoiManTinh.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatPhoiManTinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatTimBamSinh.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatTimBamSinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatTamThan.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatTamThan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatTuKy.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatTuKy_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBenhTatDongKinh.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBenhTatDongKinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBenhTatUngThu.Focus();
                    txtBenhTatUngThu.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBenhTatUngThu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBenhTatLao.Focus();
                    txtBenhTatLao.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBenhTatLao_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBenhTatKhac.Focus();
                    txtBenhTatKhac.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBenhTatKhac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBienPhapTranhThai.Focus();
                    txtBienPhapTranhThai.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBienPhapTranhThai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKyCoThaiCuoiCung.Focus();
                    txtKyCoThaiCuoiCung.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKyCoThaiCuoiCung_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSoLanCoThai.Focus();
                    spinSoLanCoThai.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSoLanCoThai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSoLanSayThai.Focus();
                    spinSoLanSayThai.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSoLanSayThai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSoLanPhaThai.Focus();
                    spinSoLanPhaThai.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSoLanPhaThai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSoLanSinhDe.Focus();
                    spinSoLanSinhDe.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSoLanSinhDe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDeThuong.Focus();
                    spinDeThuong.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDeThuong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDeMo.Focus();
                    spinDeMo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDeMo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDeKho.Focus();
                    spinDeKho.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDeKho_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSoLanDeDuThang.Focus();
                    spinSoLanDeDuThang.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSoLanDeDuThang_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSoLanDeNon.Focus();
                    spinSoLanDeNon.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSoLanDeNon_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSoConHienSong.Focus();
                    spinSoConHienSong.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSoConHienSong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBenhPhuKhoa.Focus();
                    txtBenhPhuKhoa.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBenhPhuKhoa_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridControlSurgery.Focus();
                    //txtTienSuPhauThuat.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTienSuPhauThuat_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    //txtVanDeKhac.Focus();
                    //txtVanDeKhac.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtVanDeKhac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnSave.Enabled)
                    return;
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridViewSurgery_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ScnSurgeryADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "BtnAddDelete")
                        {
                            if (data.Action == 1)
                            {
                                e.RepositoryItem = repositoryItem__Add;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItem__Delete;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSurgery_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                //if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                //{
                //    ScnSurgeryADO dataRow = (ScnSurgeryADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                //    if (dataRow != null && dataRow.SURGERY_TIME.HasValue)
                //    {
                //        if (e.Column.FieldName == "DATE_DISPLAY")
                //        {
                //            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataRow.SURGERY_TIME ?? 0);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItem__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var _Surgery = (ScnSurgeryADO)gridViewSurgery.GetFocusedRow();
                if (_Surgery != null)
                {
                    this._ScnSurgeryADOs.Remove(_Surgery);
                    gridControlSurgery.DataSource = null;
                    gridControlSurgery.DataSource = this._ScnSurgeryADOs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItem__Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ScnSurgeryADO _Surgery = new ScnSurgeryADO();
                _Surgery.Action = 2;
                this._ScnSurgeryADOs.Add(_Surgery);
                gridControlSurgery.DataSource = null;
                gridControlSurgery.DataSource = this._ScnSurgeryADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(SpinEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                SpinEdit__ValidationRule validRule = new SpinEdit__ValidationRule();
                validRule.spinEdit = control;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboABO_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal && cboABO.EditValue != null)
                {
                    cboRH.Focus();
                    cboRH.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRH_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal && cboRH.EditValue != null)
                {
                    txtDiTatBamSinh.Focus();
                    txtDiTatBamSinh.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
