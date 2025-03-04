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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.InfantInformation.ADO;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.InfantInformation
{
    public partial class frmInfantInformation : HIS.Desktop.Utility.FormBase
    {
        public List<V_HIS_BIRTH_CERT_BOOK> lstBirthCertBook { get; set; }
        private void LoadComboBornType()
        {
            try
            {
                var dataCbo = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BORN_TYPE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("EXECUTE_GROUP_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("BORN_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BORN_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboInfantTybe, dataCbo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboBornPosition()
        {
            try
            {
                var dataCbo = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BORN_POSITION>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("EXECUTE_GROUP_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("BORN_POSITION_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BORN_POSITION_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboInfantPosition, dataCbo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboUserGCS()
        {
            try
            {
                var dataCboGCS = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                dataCboGCS = dataCboGCS.Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 200);
                ControlEditorLoader.Load(cboUserGCS, dataCboGCS, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboDirectorUser()
        {
            
            try
            {
                var data = BackendDataWorker.Get<V_HIS_EMPLOYEE>();
                data = data.Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 200);
                ControlEditorLoader.Load(cboDirectorUsername, data, controlEditorADO);
                cboDirectorUsername.Properties.ImmediatePopup = true;
                if(data.Exists(o=>o.LOGINNAME == hisBranch.DIRECTOR_LOGINNAME))
                {
                    cboDirectorUsername.EditValue = hisBranch.DIRECTOR_LOGINNAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboBornResult()
        {
            try
            {
                var dataCbo = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BORN_RESULT>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("EXECUTE_GROUP_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("BORN_RESULT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BORN_RESULT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboInfantResult, dataCbo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboHisBirthCertBook()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BIRTH_CERT_BOOK_CODE", "", 10, 1, true));
                columnInfos.Add(new ColumnInfo("BIRTH_CERT_BOOK_NAME", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BIRTH_CERT_BOOK_NAME", "ID", columnInfos, false, 110);
                ControlEditorLoader.Load(cboHisBirthSertBook, lstBirthCertBook, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetDataHisBirthCertBook()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBirthCertBookViewFilter filter = new HisBirthCertBookViewFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                lstBirthCertBook = new BackendAdapter(param).Get<List<V_HIS_BIRTH_CERT_BOOK>>("api/HisBirthCertBook/GetView", ApiConsumers.MosConsumer, filter, null).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboEthnic()
        {
            try
            {
                var ethnic = BackendDataWorker.Get<SDA_ETHNIC>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ETHNIC_CODE", "", 50, 1, true));
                columnInfos.Add(new ColumnInfo("ETHNIC_NAME", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ETHNIC_NAME", "ETHNIC_CODE", columnInfos, false, 150);
                ControlEditorLoader.Load(cboEthnic, ethnic, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadComboGender()
        {
            try
            {
                var dataCbo = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboInfantGendercode, dataCbo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        //Lọc và Load dữ liệu khi nhập từ bàn phím 
        public void LoadGioiTinhCombo(string searchGender)
        {
            try
            {
                if (String.IsNullOrEmpty(searchGender))
                {
                    cboInfantGendercode.EditValue = null;
                    cboInfantGendercode.Focus();
                    cboInfantGendercode.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>().Where(o => o.GENDER_CODE.Contains(searchGender)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboInfantGendercode.EditValue = data[0].ID;
                            //txtInfantGendercode.Text = data[0].GENDER_CODE;
                            cboInfantResult.Focus();
                        }
                        else
                        {
                            //txtInfantGendercode.EditValue = null;
                            //txtInfantGendercode.Focus();
                            cboInfantGendercode.ShowPopup();

                        }
                        //cboGender.Focus();
                        //cboGender.SelectAll();
                    }
                    else
                    {
                        cboInfantResult.Focus();
                        cboInfantResult.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadComboBirthHospital()
        {
            try
            {
                List<HIS_MEDI_ORG> listDataCombo = new List<HIS_MEDI_ORG>();
                if (this.isBornedAtHospital)
                {
                    HIS_MEDI_ORG mediOrg = new HIS_MEDI_ORG();
                    mediOrg.MEDI_ORG_CODE = hisBranch.BRANCH_CODE;
                    mediOrg.MEDI_ORG_NAME = hisBranch.BRANCH_NAME;
                    listDataCombo.Add(mediOrg);
                }
                else
                {
                    listDataCombo = listHisMediOrg;
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_ORG_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_ORG_NAME", "MEDI_ORG_CODE", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBirthHospital, listDataCombo, controlEditorADO);
                cboBirthHospital.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_BABY data)
        {
            try
            {

                if (data != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("data__________", data));
                    babyid = data.ID;
                    txtInfantName.Text = data.BABY_NAME;
                    //txtInfantGendercode.Text = data.GENDER_CODE;
                    cboInfantGendercode.EditValue = data.GENDER_ID;
                    cboInfantResult.EditValue = data.BORN_RESULT_ID;
                    cboInfantTybe.EditValue = data.BORN_TYPE_ID;
                    cboInfantPosition.EditValue = data.BORN_POSITION_ID;
                    lblHisBirthCertNum.Text = data.BIRTH_CERT_NUM.ToString();
                    cboHisBirthSertBook.EditValue = data.BIRTH_CERT_BOOK_ID;

                    if (data.BORN_TIME.HasValue)
                    {
                        if ((long)(data.BORN_TIME.Value / 1000000) > 0)
                        {
                            dtdInfantdate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.BORN_TIME.Value) ?? DateTime.Now;
                        }
                        else
                        {
                            dtdInfantdate.EditValue = null;
                        }
                        if ((long)(data.BORN_TIME.Value % 1000000) > 0)
                        {
                            txtInfantBorntime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.BORN_TIME.Value) ?? DateTime.Now;// data.BORN_TIME.Value.ToString().Substring(8, 6);
                        }
                        else
                        {
                            txtInfantBorntime.EditValue = null;
                        }
                    }
                    else
                    {
                        dtdInfantdate.EditValue = null;
                        txtInfantBorntime.EditValue = null;
                    }

                    if (data.DEATH_DATE != null)
                    {
                        lciDeathDate.Enabled = true;
                        dtDeathDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.DEATH_DATE ?? 0);
                    }
                    else
                    {
                        dtDeathDate.EditValue = null;
                        lciDeathDate.Enabled = false;
                    }

                    spnInfantMonth.EditValue = data.BABY_ORDER;
                    spnChildLive.EditValue = data.CURRENT_ALIVE;
                    if (data.MONTH_COUNT.HasValue && data.MONTH_COUNT == 9 && data.WEEK_COUNT.HasValue && data.WEEK_COUNT == 40)
                    {
                        chkInfantcheck.Checked = true;
                    }
                    else
                        chkInfantcheck.Checked = false;

                    chkIsSurgery.Checked = data.IS_SURGERY == 1 ? true : false;
                    txtHeinCardTmp.Text = data.HEIN_CARD_NUMBER_TMP.ToUpper();
                    txtInfantMonth.EditValue = data.MONTH_COUNT;
                    txtInfantWeek.EditValue = data.WEEK_COUNT;
                    spnInfantHeight.EditValue = data.HEIGHT;
                    spnInfantWeight.EditValue = data.WEIGHT;
                    spnInfanthead.EditValue = data.HEAD;
                    cboEthnic.EditValue = data.ETHNIC_CODE;

                    if (!String.IsNullOrEmpty(data.MIDWIFE))
                    {
                        string[] midwife = data.MIDWIFE.Split(';');
                        if (midwife.ToList().Count > 0)
                        {
                            txtInfantMidwife1.Text = midwife[0];
                        }
                        else
                        {
                            txtInfantMidwife1.Text = null;
                        }
                        if (midwife.ToList().Count > 1)
                        {
                            txtInfantMidwife2.Text = midwife[1];
                        }
                        else
                        {
                            txtInfantMidwife2.Text = null;
                        }
                        if (midwife.ToList().Count > 2)
                        {
                            txtInfantMidwife3.Text = midwife[2];
                        }
                        else
                        {
                            txtInfantMidwife3.Text = null;
                        }
                    }
                    else
                    {
                        txtInfantMidwife1.Text = null;
                        txtInfantMidwife2.Text = null;
                        txtInfantMidwife3.Text = null;
                    }

                    if (!String.IsNullOrEmpty(data.FATHER_NAME))
                    {
                        txtFather.Text = data.FATHER_NAME;
                    }
                    else
                    {
                        txtFather.Text = null;
                    }
                    btnCancel.Enabled = true;
                    if (!String.IsNullOrEmpty(data.ISSUER_LOGINNAME))
                    {
                        txtUserGCS.Text = data.ISSUER_LOGINNAME;
                    }
                    if (!String.IsNullOrEmpty(data.ISSUER_USERNAME))
                    {
                        //cboUserGCS.EditValue = data.ISSUER_LOGINNAME;
                        txtUserGCS.Text = data.ISSUER_LOGINNAME;
                        cboUserGCS.EditValue = data.ISSUER_LOGINNAME;
                    }
                    if (!String.IsNullOrEmpty(data.DIRECTOR_LOGINNAME))
                    {
                        txtDirectorLoginname.Text = data.DIRECTOR_LOGINNAME;
                        cboDirectorUsername.EditValue = data.DIRECTOR_LOGINNAME;
                    }
                    else
                    {
                        txtDirectorLoginname.Text = "";
                        cboDirectorUsername.EditValue = null;
                    }
                    if (data.ISSUED_DATE != null)
                        dteIssue.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.ISSUED_DATE ?? 0) ?? DateTime.Now;
                    else
                        dteIssue.DateTime = DateTime.Now;
                    if (data.NUMBER_CHILDREN_BIRTH != null)
                    {
                        txtNumberChildrenBirth.EditValue = data.NUMBER_CHILDREN_BIRTH;
                    }
                    else
                    {
                        txtNumberChildrenBirth.EditValue = null;
                    }
                    if (data.NUMBER_OF_PREGNANCIES != null)
                    {
                        txtNumberOfPregnancies.Text = data.NUMBER_OF_PREGNANCIES.ToString();
                    }
                    else
                    {
                        txtNumberOfPregnancies.Text = null;
                    }
                    if (data.NUMBER_OF_BIRTH != null)
                    {
                        txtNumberOfBirth.EditValue = data.NUMBER_OF_BIRTH;
                    }
                    else
                    {
                        txtNumberOfBirth.EditValue = null;
                    }
                    cboBirthPlaceType.EditValue = data.BIRTHPLACE_TYPE ?? null;
                    cboBirthHospital.EditValue = data.BIRTH_HOSPITAL_CODE;
                    txtProvinceCodeHospital.Text = data.BIRTH_PROVINCE_CODE;
                    cboProvinceNameHospital.EditValue = data.BIRTH_PROVINCE_CODE;
                    txtDistrictCodeHospital.Text = data.BIRTH_DISTRICT_CODE;
                    cboDistrictNameHospital.EditValue = data.BIRTH_DISTRICT_CODE;
                    txtCommuneCodeHospital.Text = data.BIRTH_COMMUNE_CODE;
                    cboCommuneNameHospital.EditValue = data.BIRTH_COMMUNE_CODE;
                    txtBirthPlace.Text = data.BIRTHPLACE;

                    if (!string.IsNullOrEmpty(data.METHOD_STYLE))
                    {
                        txtMethodStyle.Text = data.METHOD_STYLE;
                    }
                    else
                    {
                        txtMethodStyle.Text = null;
                    }
                    // checkbox
                    if (data.IS_DIFFICULT_BIRTH == 1)
                    {
                        chkIsDifficultBirth.Checked = true;
                    }
                    else
                    {
                        chkIsDifficultBirth.Checked = false;
                    }
                    if (data.IS_HAEMORRHAGE == 1)
                    {
                        ChkIsHaemorrhage.Checked = true;
                    }
                    else
                    {
                        ChkIsHaemorrhage.Checked = false;
                    }
                    if (data.IS_UTERINE_RUPTURE == 1)
                    {
                        chkIsUterineRupture.Checked = true;
                    }
                    else
                    {
                        chkIsUterineRupture.Checked = false;
                    }
                    if (data.IS_PUERPERAL == 1)
                    {
                        chkIsPuerperal.Checked = true;
                    }
                    else
                    {
                        chkIsPuerperal.Checked = false;
                    }
                    if (data.IS_BACTERIAL_CONTAMINATION == 1)
                    {
                        chkIsBacterialContamination.Checked = true;
                    }
                    else
                    {
                        chkIsBacterialContamination.Checked = false;
                    }
                    if (data.IS_TETANUS == 1)
                    {
                        chkIsTetanus.Checked = true;
                    }
                    else
                    {
                        chkIsTetanus.Checked = false;
                    }
                    if (data.IS_MOTHER_DEATH == 1)
                    {
                        chkIsMotherDeath.Checked = true;
                    }
                    else
                    {
                        chkIsMotherDeath.Checked = false;
                    }
                    if (data.IS_FETAL_DEATH_22_WEEKS == 1)
                    {
                        chkIsFetalDeath22Weeks.Checked = true;
                    }
                    else
                    {
                        chkIsFetalDeath22Weeks.Checked = false;
                    }
                    if (data.IS_INJECT_K1 == 1)
                    {
                        chkIsInjeckK1.Checked = true;
                    }
                    else
                    {
                        chkIsInjeckK1.Checked = false;
                    }
                    if (data.IS_INJECT_B == 1)
                    {
                        chkIsInjeckB.Checked = true;
                    }
                    else
                    {
                        chkIsInjeckB.Checked = false;
                    }
                    if (data.POSTPARTUM_CARE == 2)
                    {
                        chkPostpartumCare2.Checked = true;
                    }
                    else
                    {
                        chkPostpartumCare2.Checked = false;
                    }
                    if (data.POSTPARTUM_CARE == 6)
                    {
                        chkPostpartumCare6.Checked = true;
                    }
                    else
                    {
                        chkPostpartumCare6.Checked = false;
                    }
                    if (!String.IsNullOrEmpty(data.NOTE))
                    {
                        txtNote.Text = data.NOTE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetControl()
        {
            try
            {
                txtInfantName.Text = "";
                //txtInfantGendercode.Text = "";
                cboInfantGendercode.EditValue = null;
                cboInfantResult.EditValue = null;
                cboInfantTybe.EditValue = null;
                cboInfantPosition.EditValue = null;
                //txtCMT.Text = "" ;
                //txtNgaycap.EditValue = null ;
                //txtNoicap.Text = "";
                dtdInfantdate.Text = "";
                txtInfantBorntime.Text = "";
                spnInfantMonth.EditValue = null;
                txtInfantMonth.Text = "";
                txtInfantWeek.Text = "";

                spnInfantHeight.EditValue = null;
                spnInfantWeight.EditValue = null;
                spnInfanthead.EditValue = null;
                txtFather.Text = "";
                txtInfantMidwife1.Text = "";
                txtInfantMidwife2.Text = "";
                txtInfantMidwife3.Text = "";
                chkInfantcheck.Checked = false;
                cboEthnic.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultDataToControl()
        {
            try
            {
                foreach (var item in listDistrict)
                {
                    DistrictADO ado = new DistrictADO(item);
                    this.lstDistrictADO.Add(ado);
                }
                foreach (var item in listCommune)
                {
                    CommuneADO ado = new CommuneADO(item);
                    this.lstCommuneADO.Add(ado);
                }
                this.InitComboCommon(this.cboProvinceName, listProvince, "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboDistrictName, lstDistrictADO, "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboCommuneName, lstCommuneADO, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");

                this.InitComboCommon(this.cboHTProvinceName, listProvince, "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboHTDistrictName, lstDistrictADO, "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboHTCommuneName, lstCommuneADO, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");

                this.InitComboCommon(this.cboProvinceNameHospital, listProvince, "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboDistrictNameHospital, lstDistrictADO, "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboCommuneNameHospital, lstCommuneADO, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboTinhThanh(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    this.cboCommuneName.Properties.DataSource = null;
                    this.cboCommuneName.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.cboDistrictName.Properties.DataSource = null;
                    this.cboDistrictName.EditValue = null;
                    this.txtDistrictCode.Text = "";
                    this.cboProvinceName.EditValue = null;
                    this.FocusShowpopup(this.cboProvinceName, false);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = listProvince.Where(o => o.SEARCH_CODE.Contains(searchCode) || o.PROVINCE_CODE == searchCode).ToList();
                    if (listResult.Count == 1)
                    {
                        bool isReLoadRef = false;
                        if (listResult[0].PROVINCE_CODE != (this.cboProvinceName.EditValue ?? "").ToString())
                        {
                            isReLoadRef = true;
                        }
                        if (isReLoadRef)
                        {
                            this.cboProvinceName.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtProvinceCode.Text = listResult[0].SEARCH_CODE;
                            this.LoadComboHuyen("", listResult[0].PROVINCE_CODE, false);
                        }
                        if (isExpand)
                        {
                            this.txtDistrictCode.Focus();
                            this.txtDistrictCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommuneName.Properties.DataSource = null;
                        this.cboCommuneName.EditValue = null;
                        this.txtCommuneCode.Text = "";
                        this.cboDistrictName.Properties.DataSource = null;
                        this.cboDistrictName.EditValue = null;
                        this.txtDistrictCode.Text = "";
                        this.cboProvinceName.EditValue = null;
                        if (isExpand)
                        {
                            this.cboProvinceName.Properties.DataSource = listResult;
                            this.FocusShowpopup(this.cboProvinceName, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadComboTinhThanh_HT(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    this.cboHTCommuneName.Properties.DataSource = null;
                    this.cboHTCommuneName.EditValue = null;
                    this.txtHTCommuneCode.Text = "";
                    this.cboHTDistrictName.Properties.DataSource = null;
                    this.cboHTDistrictName.EditValue = null;
                    this.txtHTDistrictCode.Text = "";
                    this.cboHTProvinceName.EditValue = null;
                    this.FocusShowpopup(this.cboHTProvinceName, false);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = listProvince.Where(o => o.SEARCH_CODE.Contains(searchCode) || o.PROVINCE_CODE == searchCode).ToList();
                    if (listResult.Count == 1)
                    {
                        bool isReLoadRef = false;
                        if (listResult[0].PROVINCE_CODE != (this.cboHTProvinceName.EditValue ?? "").ToString())
                        {
                            isReLoadRef = true;
                        }
                        if (isReLoadRef)
                        {
                            this.cboHTProvinceName.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtHTProvinceCode.Text = listResult[0].SEARCH_CODE;
                            this.LoadComboHuyen_HT("", listResult[0].PROVINCE_CODE, false);
                        }
                        if (isExpand)
                        {
                            this.txtHTDistrictCode.Focus();
                            this.txtHTDistrictCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboHTCommuneName.Properties.DataSource = null;
                        this.cboHTCommuneName.EditValue = null;
                        this.txtHTCommuneCode.Text = "";
                        this.cboHTDistrictName.Properties.DataSource = null;
                        this.cboHTDistrictName.EditValue = null;
                        this.txtHTDistrictCode.Text = "";
                        this.cboHTProvinceName.EditValue = null;
                        if (isExpand)
                        {
                            this.cboHTProvinceName.Properties.DataSource = listResult;
                            this.FocusShowpopup(this.cboHTProvinceName, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void LoadComboTinhThanh_BV(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    this.cboCommuneNameHospital.Properties.DataSource = null;
                    this.cboCommuneNameHospital.EditValue = null;
                    this.txtCommuneCodeHospital.Text = "";
                    this.cboDistrictNameHospital.Properties.DataSource = null;
                    this.cboDistrictNameHospital.EditValue = null;
                    this.txtDistrictCodeHospital.Text = "";
                    this.cboProvinceNameHospital.EditValue = null;
                    this.FocusShowpopup(this.cboProvinceNameHospital, false);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = listProvince.Where(o => o.SEARCH_CODE.Contains(searchCode) || o.PROVINCE_CODE == searchCode).ToList();
                    if (listResult.Count == 1)
                    {
                        bool isReLoadRef = false;
                        if (listResult[0].PROVINCE_CODE != (this.cboProvinceNameHospital.EditValue ?? "").ToString())
                        {
                            isReLoadRef = true;
                        }
                        if (isReLoadRef)
                        {
                            this.cboProvinceNameHospital.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtProvinceCodeHospital.Text = listResult[0].SEARCH_CODE;
                            this.LoadComboHuyen_BV("", listResult[0].PROVINCE_CODE, false);
                        }
                        if (isExpand)
                        {
                            this.txtDistrictCodeHospital.Focus();
                            this.txtDistrictCodeHospital.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommuneNameHospital.Properties.DataSource = null;
                        this.cboCommuneNameHospital.EditValue = null;
                        this.txtCommuneCodeHospital.Text = "";
                        this.cboDistrictNameHospital.Properties.DataSource = null;
                        this.cboDistrictNameHospital.EditValue = null;
                        this.txtDistrictCodeHospital.Text = "";
                        this.cboProvinceNameHospital.EditValue = null;
                        if (isExpand)
                        {
                            this.cboProvinceNameHospital.Properties.DataSource = listResult;
                            this.FocusShowpopup(this.cboProvinceNameHospital, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadComboHuyen(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = listDistrict.Where(o => (o.SEARCH_CODE.ToUpper().Contains(searchCode.ToUpper()) || o.DISTRICT_CODE == searchCode) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();

                bool isReLoadRef = false;
                if (listResult[0].DISTRICT_CODE != (this.cboDistrictName.EditValue ?? "").ToString())
                {
                    isReLoadRef = true;
                }

                if (!isReLoadRef)
                {
                    if (isExpand)
                    {
                        this.txtCommuneCode.Focus();
                        this.txtCommuneCode.SelectAll();
                    }
                    return;
                }
                List<DistrictADO> lstADO = new List<DistrictADO>();
                foreach (var item in listResult)
                {
                    DistrictADO ado = new DistrictADO(item);
                    lstADO.Add(ado);
                }
                this.InitComboCommon(this.cboDistrictName, lstADO, "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    this.cboCommuneName.Properties.DataSource = null;
                    this.cboCommuneName.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.txtDistrictCode.Text = "";
                    this.cboDistrictName.EditValue = null;
                    this.FocusShowpopup(this.cboDistrictName, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboDistrictName.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictCode.Text = listResult[0].SEARCH_CODE;
                        if (String.IsNullOrEmpty(this.cboProvinceName.Text))
                        {
                            this.cboProvinceName.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtProvinceCode.Text = listResult[0].PROVINCE_CODE;
                        }
                        this.LoadComboXa("", listResult[0].DISTRICT_CODE, false);

                        if (isExpand)
                        {
                            this.txtCommuneCode.Focus();
                            this.txtCommuneCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommuneName.Properties.DataSource = null;
                        this.cboCommuneName.EditValue = null;
                        this.txtCommuneCode.Text = "";
                        this.cboDistrictName.EditValue = null;
                        if (isExpand)
                        {
                            this.FocusShowpopup(this.cboDistrictName, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboHuyen_HT(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = listDistrict.Where(o => (o.SEARCH_CODE.ToUpper().Contains(searchCode.ToUpper()) || o.DISTRICT_CODE == searchCode) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();

                bool isReLoadRef = false;
                if (listResult[0].DISTRICT_CODE != (this.cboHTDistrictName.EditValue ?? "").ToString())
                {
                    isReLoadRef = true;
                }

                if (!isReLoadRef)
                {
                    if (isExpand)
                    {
                        this.txtHTCommuneCode.Focus();
                        this.txtHTCommuneCode.SelectAll();
                    }
                    return;
                }
                List<DistrictADO> lstADO = new List<DistrictADO>();
                foreach (var item in listResult)
                {
                    DistrictADO ado = new DistrictADO(item);
                    lstADO.Add(ado);
                }
                this.InitComboCommon(this.cboHTDistrictName, lstADO, "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    this.cboHTCommuneName.Properties.DataSource = null;
                    this.cboHTCommuneName.EditValue = null;
                    this.txtHTCommuneCode.Text = "";
                    this.txtHTDistrictCode.Text = "";
                    this.cboHTDistrictName.EditValue = null;
                    this.FocusShowpopup(this.cboHTDistrictName, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboHTDistrictName.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtHTDistrictCode.Text = listResult[0].SEARCH_CODE;
                        if (String.IsNullOrEmpty(this.cboHTProvinceName.Text))
                        {
                            this.cboHTProvinceName.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtHTProvinceCode.Text = listResult[0].PROVINCE_CODE;
                        }
                        this.LoadComboXa_HT("", listResult[0].DISTRICT_CODE, false);

                        if (isExpand)
                        {
                            this.txtHTCommuneCode.Focus();
                            this.txtHTCommuneCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboHTCommuneName.Properties.DataSource = null;
                        this.cboHTCommuneName.EditValue = null;
                        this.txtHTCommuneCode.Text = "";
                        this.cboHTDistrictName.EditValue = null;
                        if (isExpand)
                        {
                            this.FocusShowpopup(this.cboHTDistrictName, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadComboHuyen_BV(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = listDistrict.Where(o => (o.SEARCH_CODE.ToUpper().Contains(searchCode.ToUpper()) || o.DISTRICT_CODE == searchCode) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();

                bool isReLoadRef = false;
                if (listResult[0].DISTRICT_CODE != (this.cboDistrictNameHospital.EditValue ?? "").ToString())
                {
                    isReLoadRef = true;
                }

                if (!isReLoadRef)
                {
                    if (isExpand)
                    {
                        this.txtCommuneCodeHospital.Focus();
                        this.txtCommuneCodeHospital.SelectAll();
                    }
                    return;
                }
                List<DistrictADO> lstADO = new List<DistrictADO>();
                foreach (var item in listResult)
                {
                    DistrictADO ado = new DistrictADO(item);
                    lstADO.Add(ado);
                }
                this.InitComboCommon(this.cboDistrictNameHospital, lstADO, "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    this.cboCommuneNameHospital.Properties.DataSource = null;
                    this.cboCommuneNameHospital.EditValue = null;
                    this.txtCommuneCodeHospital.Text = "";
                    this.txtDistrictCodeHospital.Text = "";
                    this.cboDistrictNameHospital.EditValue = null;
                    this.FocusShowpopup(this.cboDistrictNameHospital, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboDistrictNameHospital.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictCodeHospital.Text = listResult[0].SEARCH_CODE;
                        if (String.IsNullOrEmpty(this.cboProvinceNameHospital.Text))
                        {
                            this.cboProvinceNameHospital.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtProvinceCodeHospital.Text = listResult[0].PROVINCE_CODE;
                        }
                        this.LoadComboXa_BV("", listResult[0].DISTRICT_CODE, false);

                        if (isExpand)
                        {
                            this.txtCommuneCodeHospital.Focus();
                            this.txtCommuneCodeHospital.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommuneNameHospital.Properties.DataSource = null;
                        this.cboCommuneNameHospital.EditValue = null;
                        this.txtCommuneCodeHospital.Text = "";
                        this.cboDistrictNameHospital.EditValue = null;
                        if (isExpand)
                        {
                            this.FocusShowpopup(this.cboDistrictNameHospital, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboXa(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = listCommune.Where(o => ((o.SEARCH_CODE ?? "").Contains(searchCode ?? "") || o.COMMUNE_CODE == searchCode)
                        && (String.IsNullOrEmpty(districtCode) || o.DISTRICT_CODE == districtCode)).ToList();
                List<CommuneADO> lstCommuneADO = new List<CommuneADO>();
                foreach (var item in listResult)
                {
                    CommuneADO ado = new CommuneADO(item);
                    lstCommuneADO.Add(ado);
                }
                this.InitComboCommon(this.cboCommuneName, lstCommuneADO, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    this.cboCommuneName.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.FocusShowpopup(this.cboCommuneName, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboCommuneName.EditValue = listResult[0].COMMUNE_CODE;
                        this.txtCommuneCode.Text = listResult[0].SEARCH_CODE;
                        this.cboDistrictName.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictCode.Text = listResult[0].SEARCH_CODE;
                        if (String.IsNullOrEmpty(this.cboProvinceName.Text))
                        {
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.Where(o => o.ID == listResult[0].DISTRICT_ID).FirstOrDefault();
                            if (district != null)
                            {
                                this.cboProvinceName.EditValue = district.PROVINCE_CODE;
                                this.txtProvinceCode.Text = district.PROVINCE_CODE;
                            }
                        }

                        if (isExpand)
                        {
                            this.txtAddress.Focus();
                            this.txtAddress.SelectAll();
                        }
                    }
                    else if (isExpand)
                    {
                        this.cboCommuneName.EditValue = null;
                        this.FocusShowpopup(this.cboCommuneName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboXa_HT(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = listCommune.Where(o => ((o.SEARCH_CODE ?? "").Contains(searchCode ?? "") || o.COMMUNE_CODE == searchCode)
                        && (String.IsNullOrEmpty(districtCode) || o.DISTRICT_CODE == districtCode)).ToList();
                List<CommuneADO> lstCommuneADO = new List<CommuneADO>();
                foreach (var item in listResult)
                {
                    CommuneADO ado = new CommuneADO(item);
                    lstCommuneADO.Add(ado);
                }
                this.InitComboCommon(this.cboHTCommuneName, lstCommuneADO, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    this.cboHTCommuneName.EditValue = null;
                    this.txtHTCommuneCode.Text = "";
                    this.FocusShowpopup(this.cboHTCommuneName, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboHTCommuneName.EditValue = listResult[0].COMMUNE_CODE;
                        this.txtHTCommuneCode.Text = listResult[0].SEARCH_CODE;
                        this.cboHTDistrictName.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtHTDistrictCode.Text = listResult[0].SEARCH_CODE;
                        if (String.IsNullOrEmpty(this.cboHTProvinceName.Text))
                        {
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.Where(o => o.ID == listResult[0].DISTRICT_ID).FirstOrDefault();
                            if (district != null)
                            {
                                this.cboHTProvinceName.EditValue = district.PROVINCE_CODE;
                                this.txtHTProvinceCode.Text = district.PROVINCE_CODE;
                            }
                        }

                        if (isExpand)
                        {
                            this.txtHTAddress.Focus();
                            this.txtHTAddress.SelectAll();
                        }
                    }
                    else if (isExpand)
                    {
                        this.cboHTCommuneName.EditValue = null;
                        this.FocusShowpopup(this.cboHTCommuneName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadComboXa_BV(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = listCommune.Where(o => ((o.SEARCH_CODE ?? "").Contains(searchCode ?? "") || o.COMMUNE_CODE == searchCode)
                        && (String.IsNullOrEmpty(districtCode) || o.DISTRICT_CODE == districtCode)).ToList();
                List<CommuneADO> lstCommuneADO = new List<CommuneADO>();
                foreach (var item in listResult)
                {
                    CommuneADO ado = new CommuneADO(item);
                    lstCommuneADO.Add(ado);
                }
                this.InitComboCommon(this.cboCommuneNameHospital, lstCommuneADO, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    this.cboCommuneNameHospital.EditValue = null;
                    this.txtCommuneCodeHospital.Text = "";
                    this.FocusShowpopup(this.cboCommuneNameHospital, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboCommuneNameHospital.EditValue = listResult[0].COMMUNE_CODE;
                        this.txtCommuneCodeHospital.Text = listResult[0].SEARCH_CODE;
                        this.cboDistrictNameHospital.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictCodeHospital.Text = listResult[0].SEARCH_CODE;
                        if (String.IsNullOrEmpty(this.cboProvinceNameHospital.Text))
                        {
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.Where(o => o.ID == listResult[0].DISTRICT_ID).FirstOrDefault();
                            if (district != null)
                            {
                                this.cboProvinceNameHospital.EditValue = district.PROVINCE_CODE;
                                this.txtProvinceCodeHospital.Text = district.PROVINCE_CODE;
                            }
                        }

                        if (isExpand)
                        {
                            this.txtBirthPlace.Focus();
                            this.txtBirthPlace.SelectAll();
                        }
                    }
                    else if (isExpand)
                    {
                        this.cboCommuneNameHospital.EditValue = null;
                        this.FocusShowpopup(this.cboCommuneNameHospital, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(GridLookUpEdit cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommonUtil(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommonUtil(GridLookUpEdit cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1, true));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2, true));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
                cboEditor.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMotherAddress(HIS_PATIENT patient)
        {
            try
            {
                if (!String.IsNullOrEmpty(patient.PROVINCE_CODE))
                {
                    SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == patient.PROVINCE_CODE);
                    if (province != null)
                    {
                        this.cboProvinceName.EditValue = province.PROVINCE_CODE;
                        this.LoadComboHuyen("", province.PROVINCE_CODE, false);
                        this.txtProvinceCode.Text = province.SEARCH_CODE;
                    }
                }
                if (!String.IsNullOrEmpty(patient.DISTRICT_CODE))
                {
                    SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.SingleOrDefault(o => o.DISTRICT_CODE == patient.DISTRICT_CODE
                               && (String.IsNullOrEmpty((patient.PROVINCE_CODE ?? "").ToString()) || o.PROVINCE_CODE == (patient.PROVINCE_CODE ?? "").ToString()));
                    if (district != null)
                    {
                        this.cboDistrictName.EditValue = district.DISTRICT_CODE;
                        if (String.IsNullOrEmpty((this.cboProvinceName.EditValue ?? "").ToString()))
                        {
                            this.cboProvinceName.EditValue = district.SEARCH_CODE;
                        }
                        this.LoadComboXa("", district.DISTRICT_CODE, false);
                        this.txtDistrictCode.Text = district.SEARCH_CODE;
                    }
                }
                if (!String.IsNullOrEmpty(patient.COMMUNE_CODE))
                {
                    SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = listCommune.SingleOrDefault(o => o.COMMUNE_CODE == patient.COMMUNE_CODE.ToString()
                                && (String.IsNullOrEmpty((patient.DISTRICT_CODE ?? "").ToString()) || o.DISTRICT_CODE == (patient.DISTRICT_CODE ?? "").ToString()));
                    if (commune != null)
                    {
                        this.cboCommuneName.EditValue = commune.COMMUNE_CODE;
                        this.txtCommuneCode.Text = commune.SEARCH_CODE;
                    }
                }
                if (!String.IsNullOrEmpty(patient.ADDRESS))
                {
                    this.txtAddress.Text = patient.ADDRESS;
                }

                //Địa chỉ hiện tại
                SDA.EFMODEL.DataModels.V_SDA_PROVINCE provinceHT = null;
                if (!String.IsNullOrWhiteSpace(patient.HT_PROVINCE_NAME))
                {
                    provinceHT = listProvince.FirstOrDefault(o => o.PROVINCE_NAME != null && o.PROVINCE_NAME.ToLower().Trim() == patient.HT_PROVINCE_NAME.ToLower().Trim());
                    if (provinceHT != null)
                    {
                        this.cboHTProvinceName.EditValue = provinceHT.PROVINCE_CODE;
                        this.LoadComboHuyen_HT("", provinceHT.PROVINCE_CODE, false);
                        this.txtHTProvinceCode.Text = provinceHT.SEARCH_CODE;
                    }
                }
                SDA.EFMODEL.DataModels.V_SDA_DISTRICT districtHT = null;
                if (provinceHT != null && !String.IsNullOrEmpty(patient.HT_DISTRICT_NAME))
                {
                    districtHT = listDistrict.FirstOrDefault(o => o.DISTRICT_NAME == patient.HT_DISTRICT_NAME && o.PROVINCE_ID == provinceHT.ID);
                    if (districtHT != null)
                    {
                        this.cboHTDistrictName.EditValue = districtHT.DISTRICT_CODE;
                        this.LoadComboXa_HT("", districtHT.DISTRICT_CODE, false);
                        this.txtHTDistrictCode.Text = districtHT.SEARCH_CODE;
                    }
                }
                if (districtHT != null && !String.IsNullOrEmpty(patient.HT_COMMUNE_NAME))
                {
                    var commune = listCommune.FirstOrDefault(o => o.COMMUNE_NAME == patient.HT_COMMUNE_NAME && o.DISTRICT_ID == districtHT.ID);
                    if (commune != null)
                    {
                        this.cboHTCommuneName.EditValue = commune.COMMUNE_CODE;
                        this.txtHTCommuneCode.Text = commune.SEARCH_CODE;
                    }
                }
                if (!String.IsNullOrEmpty(patient.HT_ADDRESS))
                {
                    this.txtHTAddress.Text = patient.HT_ADDRESS;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(DevExpress.XtraEditors.GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    Inventec.Common.Controls.PopupLoader.PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FocusToDistrict()
        {
            try
            {
                this.txtDistrictCode.Focus();
                this.txtDistrictCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToCommune()
        {
            try
            {
                this.txtCommuneCode.Focus();
                this.txtCommuneCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToAddress()
        {
            try
            {

                this.txtAddress.Focus();
                this.txtAddress.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToDistrict_HT()
        {
            try
            {
                this.txtHTDistrictCode.Focus();
                this.txtHTDistrictCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToCommune_HT()
        {
            try
            {
                this.txtHTCommuneCode.Focus();
                this.txtHTCommuneCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToAddress_HT()
        {
            try
            {
                this.txtHTAddress.Focus();
                this.txtHTAddress.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}


