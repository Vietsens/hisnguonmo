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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LocalStorage.Location;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class frmInputDetail : Form
    {
        //
        HIS_EYE_SURGRY_DESC eyeSurgDesc;
        Action<HIS_EYE_SURGRY_DESC> GetEyeSurgryDescLast;
        SkinSurgeryDesADO SkinSurgeryDes;
        Action<SkinSurgeryDesADO> ActionSkinSurgeryDes;
        V_HIS_SERE_SERV_PTTT SereServPTTT;
        List<HIS_STENT_CONCLUDE> StenConclude;
        Action<V_HIS_SERE_SERV_PTTT> ActionSereServPTTT;
        Action<List<HIS_STENT_CONCLUDE>> ActionStentConclude;
        V_HIS_SERE_SERV_5 sereServ;
        long LOAI_PT_MAT = 0;//: LOAI_PT_MAT: 1: PT glocom, 2: PT mộng; 3: PT duc thuy tinh the, 4: PT tai tao le quan, 5: PT sup mi, 6: TT laser yag, 7: TT mong mat
        bool isFirstLoad = true;
        List<DmvADO> lstDataDmv { get; set; }
        bool isReset { get; set; }
        public frmInputDetail(V_HIS_SERE_SERV_5 sereServ, HIS_EYE_SURGRY_DESC eyeSurgDesc, List<HIS_STENT_CONCLUDE> StenConclude, Action<HIS_EYE_SURGRY_DESC> getEyeSurgryDescLast, SkinSurgeryDesADO skinSurgeryDes, Action<SkinSurgeryDesADO> actionSkinSurgeryDes, V_HIS_SERE_SERV_PTTT sereServPTTT, Action<V_HIS_SERE_SERV_PTTT> actionSereServPTTT, Action<List<HIS_STENT_CONCLUDE>> ActionStentConclude)
        {
            InitializeComponent();
            try
            {
                this.sereServ = sereServ;
                this.StenConclude = StenConclude;
                this.eyeSurgDesc = eyeSurgDesc;
                this.GetEyeSurgryDescLast = getEyeSurgryDescLast;
                this.SkinSurgeryDes = skinSurgeryDes;
                this.ActionSkinSurgeryDes = actionSkinSurgeryDes;
                this.SereServPTTT = sereServPTTT;
                this.ActionSereServPTTT = actionSereServPTTT;
                this.ActionStentConclude = ActionStentConclude;
                SetIcon();
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

        private void frmInputDetail_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                xtraTabControl1.ShowTabHeader = DefaultBoolean.False;
                if (eyeSurgDesc == null)
                {
                    eyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                }
                LOAI_PT_MAT = (eyeSurgDesc.LOAI_PT_MAT);
                switch (LOAI_PT_MAT)
                {
                    case IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__GLOCOM:
                        raPTGlocom.Checked = true;
                        LoadPTGlocom();
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_MONG:
                        raPTMong.Checked = true;
                        LoadTTMong();
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_TTT:
                        raPTDucThuyTInhThe.Checked = true;
                        LoadPTDucThuyTinhThe();
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_TAI_TAO_LE_QUAN:
                        raPTTaiTaoLeQuan.Checked = true;
                        LoadPTTaiTaoLeQuan();
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_SUP_MI:
                        raPTSupMi.Checked = true;
                        LoadPTSupMi();
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__TT_LASER_YAG:
                        raTTLaser.Checked = true;
                        LoadTTLaser();
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__TT_MONG_MAT:
                        raTTMongMat.Checked = true;
                        LoadTTMongMat();
                        break;
                    default:
                        raPTMong.Checked = true;
                        LoadTTMong();
                        break;
                }

                LoadTTSkin();
                LoadDMV();
                LoadSereServPTTT_Other();
                isFirstLoad = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDMV()
        {
            try
            {
                lstDataDmv = new List<DmvADO>();
                lstDataDmv.Add(new DmvADO() { Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd });
                if (this.SereServPTTT != null)
                {
                    txtPci.Text = this.SereServPTTT.PCI;
                    txtStenting.Text = this.SereServPTTT.STENTING;
                    txtLocation.Text = this.SereServPTTT.LOCATION_INTERVENTION;
                    txtReason.Text = this.SereServPTTT.REASON_INTERVENTION;
                    txtIntroducer.Text = this.SereServPTTT.INTRODUCER;
                    txtGuidingCatheter.Text = this.SereServPTTT.GUIDING_CATHETER;
                    txtGuiteWire.Text = this.SereServPTTT.GUITE_WIRE;
                    txtBallon.Text = this.SereServPTTT.BALLON;
                    txtStent.Text = this.SereServPTTT.STENT;
                    txtContrastAgent.Text = this.SereServPTTT.CONTRAST_AGENT;
                    txtInstrumentsOther.Text = this.SereServPTTT.INSTRUMENTS_OTHER;
                    memStentNote.Text = this.SereServPTTT.STENT_NOTE;
                    if (StenConclude != null && StenConclude.Count > 0)
                    {
                        lstDataDmv = new List<DmvADO>();
                        StenConclude.ForEach(o =>
                        {
                            lstDataDmv.Add(new DmvADO(o) { Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit });
                        });
                        lstDataDmv.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    }
                }
                gridControl1.DataSource = null;
                gridControl1.DataSource = lstDataDmv;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSereServPTTT_Other()
        {
            try
            {
                if (this.SereServPTTT != null)
                {
                    txtDrainage.Text = this.SereServPTTT.DRAINAGE;
                    txtWick.Text = this.SereServPTTT.WICK;
                    if (this.SereServPTTT.DRAW_DATE != null)
                    {
                        dtDrawDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.SereServPTTT.DRAW_DATE ?? 0) ?? new DateTime();
                    }
                    if (this.SereServPTTT.CUT_SEWING_DATE != null)
                    {
                        dtSewingDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.SereServPTTT.CUT_SEWING_DATE ?? 0) ?? new DateTime();
                    }
                    txtOther.Text = this.SereServPTTT.OTHER;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTTSkin()
        {
            try
            {
                if (SkinSurgeryDes != null)
                {
                    xtraTabAddInfo.SelectedTabPage = xtraTabSkin;
                    if (SkinSurgeryDes != null)
                    {
                        if (SkinSurgeryDes.SURGERY_POSITION_ID.HasValue)
                        {
                            if (SkinSurgeryDes.SURGERY_POSITION_ID.Value == 1)
                            {
                                chkPostureUp.Checked = true;
                            }
                            else if (SkinSurgeryDes.SURGERY_POSITION_ID.Value == 2)
                            {
                                chkPostureSide.Checked = true;
                            }
                            else if (SkinSurgeryDes.SURGERY_POSITION_ID.Value == 3)
                            {
                                chkPostureTummy.Checked = true;
                            }
                        }
                    }

                    if (SkinSurgeryDes != null)
                    {
                        txtSkinDamage.Text = SkinSurgeryDes.DAMAGE;
                        txtSkinDamagePosition.Text = SkinSurgeryDes.DAMAGE_POSITION;
                        spSkinDamageAmount.EditValue = SkinSurgeryDes.DAMAGE_AMOUNT;

                        if (SkinSurgeryDes.DAMAGE_TREAT_CUTTING_TYPE.HasValue)
                        {
                            if (SkinSurgeryDes.DAMAGE_TREAT_CUTTING_TYPE.Value == 1)
                            {
                                chkDamageTypeAll.Checked = true;
                            }
                            else if (SkinSurgeryDes.DAMAGE_TREAT_CUTTING_TYPE.Value == 2)
                            {
                                chkDamageTypePart.Checked = true;
                            }
                        }

                        if (SkinSurgeryDes.DAMAGE_TREAT_SHAPING_TYPE.HasValue)
                        {
                            if (SkinSurgeryDes.DAMAGE_TREAT_SHAPING_TYPE.Value == 1)
                            {
                                chkShapingSkin.Checked = true;
                            }
                            else if (SkinSurgeryDes.DAMAGE_TREAT_SHAPING_TYPE.Value == 2)
                            {
                                chkShapingSkinOther.Checked = true;
                            }
                        }

                        if (SkinSurgeryDes.DAMAGE_TREAT_CLOSING_TYPE.HasValue)
                        {
                            if (SkinSurgeryDes.DAMAGE_TREAT_CLOSING_TYPE.Value == 1)
                            {
                                chkCloseSkin.Checked = true;
                            }
                            else if (SkinSurgeryDes.DAMAGE_TREAT_CLOSING_TYPE.Value == 2)
                            {
                                chkDiscloseSkin.Checked = true;
                            }
                        }

                        txtTreatSkinOther.Text = SkinSurgeryDes.DAMAGE_TREAT_OTHER;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadPTTaiTaoLeQuan()
        {
            try
            {
                if (isReset) return;

                DefaultChosen();
                TextDefault();
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //4. Phẫu thuật tái tạo lệ quản:
                //- CHAN_DOAN_DUT_LE_QUAN: 1: Dut le quan duoi; 2: Dut le quan tren; 3: Dut le quan tren duoi
                if (eyeSurgDesc.CHAN_DOAN_DUT_LE_QUAN == 1)
                {
                    checkEdit205.Checked = true;
                }
                else if (eyeSurgDesc.CHAN_DOAN_DUT_LE_QUAN == 2)
                {
                    checkEdit201.Checked = true;
                }
                else if (eyeSurgDesc.CHAN_DOAN_DUT_LE_QUAN == 3)
                {
                    checkEdit204.Checked = true;
                }

                spinEdit51.EditValue = eyeSurgDesc.DUT_LE_QUAN_GIO_THU;
                //- DUT_LE_QUAN_GIO_THU: Dut le quan gio thu

                if (eyeSurgDesc.PP_PHAU_THUAT_LE_QUAN == 1)
                {
                    checkEdit200.Checked = true;
                }
                else if (eyeSurgDesc.PP_PHAU_THUAT_LE_QUAN == 2)
                {
                    checkEdit197.Checked = true;
                }

                spinEdit52.EditValue = eyeSurgDesc.PHAU_THUAT_LAN_THU;
                //- PP_PHAU_THUAT_LE_QUAN: 1: Dat ong silicon 1 le quan; 2: Dat ong silicon hinh nhan
                //- PHAU_THUAT_LAN_THU:

                if (eyeSurgDesc.PP_VO_CAM == 1)
                {
                    checkEdit194.Checked = true;
                }
                else if (eyeSurgDesc.PP_VO_CAM == 2)
                {
                    checkEdit193.Checked = true;
                }

                //- PP_VO_CAM_DUOI_HOC: Vo cam duoi hoc. 0: Khong; 1: co
                if (eyeSurgDesc.PP_VO_CAM_DUOI_HOC == null || eyeSurgDesc.PP_VO_CAM_DUOI_HOC == 1)
                {
                    checkEdit192.Checked = true;
                }

                if (eyeSurgDesc.PP_VO_CAM_MUI == null || eyeSurgDesc.PP_VO_CAM_MUI == 1)
                {
                    checkEdit191.Checked = true;
                }
                //- PP_VO_CAM_MUI: Vo cam vung mui. 0: Khong; 1: co

                if (eyeSurgDesc.PP_VO_CAM_VUNG_TUI_LE == null || eyeSurgDesc.PP_VO_CAM_VUNG_TUI_LE == 1)
                {
                    checkEdit183.Checked = true;
                }
                //- PP_VO_CAM_VUNG_TUI_LE: Vo cam vung tui le. 0: Khong; 1: co

                if (eyeSurgDesc.PP_VO_CAM_THE0_VET_RACH_MI == null || eyeSurgDesc.PP_VO_CAM_THE0_VET_RACH_MI == 1)
                {
                    checkEdit185.Checked = true;
                }
                textEdit49.Text = eyeSurgDesc.THUOC_TE;//TODO
                //- PP_VO_CAM_THE0_VET_RACH_MI: Vo cam doc theo vet rach mi. 0: Khong; 1: co

                if (eyeSurgDesc.LAY_DI_VAT == 1)
                {
                    checkEdit181.Checked = true;
                }
                else if (eyeSurgDesc.LAY_DI_VAT == 0)
                {
                    checkEdit184.Checked = true;
                }
                //- LAY_DI_VAT: Kiem tra vet thuong, lay di vat. 0: Khong; 1: Co

                if (eyeSurgDesc.LE_QUAN_LANH_DUT == 1)
                {
                    checkEdit175.Checked = true;
                }
                else if (eyeSurgDesc.LE_QUAN_LANH_DUT == 0)
                {
                    checkEdit182.Checked = true;
                }
                //- LE_QUAN_LANH_DUT: Kiem tra le quan lanh. 0: Khong dut; 1: Co dut

                if (eyeSurgDesc.TIM_DAU_DUT_NGOAI == null || eyeSurgDesc.TIM_DAU_DUT_NGOAI == 1)
                {
                    checkEdit180.Checked = true;
                }
                //- TIM_DAU_DUT_NGOAI: 0: Khong; 1: Co

                if (eyeSurgDesc.TIM_DAU_DUT_TRONG == null || eyeSurgDesc.TIM_DAU_DUT_TRONG == 1)
                {
                    checkEdit206.Checked = true;
                }
                //- TIM_DAU_DUT_TRONG: 0: Khong; 1: Co
                //- DAU_DUT_TRONG_VITRI: Vi tri dau dut trong. 1: 1/3 ngoai; 2: 1/3 giua; 3: 1/3 trong
                if (eyeSurgDesc.DAU_DUT_TRONG_VITRI == 1)
                {
                    checkEdit190.Checked = true;
                }
                else if (eyeSurgDesc.DAU_DUT_TRONG_VITRI == 2)
                {
                    checkEdit187.Checked = true;
                }
                else if (eyeSurgDesc.DAU_DUT_TRONG_VITRI == 3)
                {
                    checkEdit189.Checked = true;
                }

                //- DAT_LE_QUAN: 1: 1 le quan; 2: hinh nhan; 3: Mini monoka
                if (eyeSurgDesc.DAT_LE_QUAN == 1)
                {
                    checkEdit188.Checked = true;
                }
                else if (eyeSurgDesc.DAT_LE_QUAN == 2)
                {
                    checkEdit178.Checked = true;
                }
                else if (eyeSurgDesc.DAT_LE_QUAN == 3)
                {
                    checkEdit186.Checked = true;
                }

                textEdit50.Text = eyeSurgDesc.CHI_NOI_2_DAU_LE_QUAN;
                //- CHI_NOI_2_DAU_LE_QUAN:

                //- TAI_TAO_MI_KET_MAC: 0: Khong; 1: co
                if (eyeSurgDesc.TAI_TAO_MI_KET_MAC == 1)
                {
                    checkEdit173.Checked = true;
                }
                else if (eyeSurgDesc.TAI_TAO_MI_KET_MAC == 0)
                    checkEdit173.Checked = false;

                //- TAI_TAO_MI_MO_DUOI_DA: 0: Khong; 1: co
                if (eyeSurgDesc.TAI_TAO_MI_MO_DUOI_DA == 1)
                {
                    checkEdit176.Checked = true;
                }
                else if (eyeSurgDesc.TAI_TAO_MI_MO_DUOI_DA == 0)
                    checkEdit176.Checked = false;

                if (eyeSurgDesc.TAI_TAO_MI_DA == 1)
                {
                    checkEdit177.Checked = true;
                }
                else if (eyeSurgDesc.TAI_TAO_MI_DA == 0)
                    checkEdit177.Checked = false;
                //- TAI_TAO_MI_DA: 0: Khong; 1: co

                if (eyeSurgDesc.CO_DINH_ONG_SILICON == 1)
                {
                    checkEdit179.Checked = true;
                }
                else if (eyeSurgDesc.CO_DINH_ONG_SILICON == 2)
                {
                    checkEdit171.Checked = true;
                }
                else if (eyeSurgDesc.CO_DINH_ONG_SILICON == 3)
                {
                    checkEdit174.Checked = true;
                }
                textEdit44.Text = eyeSurgDesc.NYLON_CO_DINH_ONG_SILICON;
                //- CO_DINH_ONG_SILICON: 1: Hinh nhan; 2: Da mi; 3: Nut diem le; 4: Bang chi nylong
                //- NYLON_CO_DINH_ONG_SILICON: Chi nylon co dinh ong silicon

                textEdit48.Text = eyeSurgDesc.DIEN_BIEN_KHAC;
                //- DIEN_BIEN_KHAC

                textEdit41.Text = eyeSurgDesc.TRA_MAT_THUOC;
                //- TRA_MAT_THUOC:

                if (eyeSurgDesc.TRA_MAT_BANG_EP == 1)
                {
                    checkEdit144.Checked = true;
                }
                else if (eyeSurgDesc.TRA_MAT_BANG_EP == 0)
                {
                    checkEdit144.Checked = false;
                }
                //- TRA_MAT_BANG_EP: 0: khong, 1: co
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadPTSupMi()
        {
            try
            {
                if (isReset) return;
                TextDefaultPTSupMi();
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //5. Phẫu thuật sụp mí:
                if (eyeSurgDesc.CHAN_DOAN_SUP_MI_MP == null || eyeSurgDesc.CHAN_DOAN_SUP_MI_MP == 1)
                    checkEdit141.Checked = true;
                //- CHAN_DOAN_SUP_MI_MT: Sup mi mat trai hay khong. 0: Khong; 1: Co

                if (checkEdit141.Checked)
                {
                    if (eyeSurgDesc.CHAN_DOAN_SUP_MI_MP_DO == 1)
                    {
                        checkEdit137.Checked = true;
                    }
                    else if (eyeSurgDesc.CHAN_DOAN_SUP_MI_MP_DO == 2)
                    {
                        checkEdit140.Checked = true;
                    }
                    else if (eyeSurgDesc.CHAN_DOAN_SUP_MI_MP_DO == 3)
                    {
                        checkEdit138.Checked = true;
                    }
                    else if (eyeSurgDesc.CHAN_DOAN_SUP_MI_MP_DO == 4)
                    {
                        checkEdit139.Checked = true;
                    }
                    else
                        checkEdit138.Checked = true;
                }
                else
                    checkEdit137.Checked = checkEdit140.Checked = checkEdit138.Checked = checkEdit139.Checked = false;
                //- CHAN_DOAN_SUP_MI_MT_DO: 1: Sup do I; 2: Sup do II; 3: Sup do III; 4: Sup do IV

                if (eyeSurgDesc.CHAN_DOAN_SUP_MI_MT == null || eyeSurgDesc.CHAN_DOAN_SUP_MI_MT == 1)
                    checkEdit136.Checked = true;
                //- CHAN_DOAN_SUP_MI_MP: Sup mi mat phai hay khong. 0: Khong; 1: Co

                if (checkEdit136.Checked)
                {
                    if (eyeSurgDesc.CHAN_DOAN_SUP_MI_MT_DO == 1)
                    {
                        checkEdit133.Checked = true;
                    }
                    else if (eyeSurgDesc.CHAN_DOAN_SUP_MI_MT_DO == 2)
                    {
                        checkEdit132.Checked = true;
                    }
                    else if (eyeSurgDesc.CHAN_DOAN_SUP_MI_MT_DO == 3)
                    {
                        checkEdit135.Checked = true;
                    }
                    else if (eyeSurgDesc.CHAN_DOAN_SUP_MI_MT_DO == 4)
                    {
                        checkEdit131.Checked = true;
                    }
                    else
                        checkEdit132.Checked = true;
                }
                else
                    checkEdit133.Checked = checkEdit132.Checked = checkEdit135.Checked = checkEdit131.Checked = false;
                //- CHAN_DOAN_SUP_MI_MP_DO: 1: Sup do I; 2: Sup do II; 3: Sup do III; 4: Sup do IV

                if (eyeSurgDesc.PP_PT_CO_NANG_MI == null || eyeSurgDesc.PP_PT_CO_NANG_MI == 1)
                {
                    checkEdit128.Checked = true;
                }
                else if (eyeSurgDesc.PP_PT_CO_NANG_MI == 2)
                {
                    checkEdit127.Checked = true;
                }
                else if (eyeSurgDesc.PP_PT_CO_NANG_MI == 3)
                {
                    checkEdit126.Checked = true;
                }
                else if (eyeSurgDesc.PP_PT_CO_NANG_MI == 4)
                {
                    checkEdit125.Checked = true;
                }
                else if (eyeSurgDesc.PP_PT_CO_NANG_MI == 5)
                {
                    checkEdit123.Checked = true;
                }
                //- PP_PT_CO_NANG_MI: 1: Cat ngan co nang mi; 2: Gap can co nang mi

                //- PP_PT_TREO_MI_CO_TRAN: 1: treo mi co tran bang can co dui; 2: Treo mi co tran bang chi; 3: Treo mi co tran bang silicon

                //if (eyeSurgDesc.PP_VO_CAM == 1)
                //{
                //    checkEdit130.Checked = true;
                //    textEdit39.Enabled = false;
                //    textEdit39.Text = "";
                //}
                //else //if (eyeSurgDesc.PP_VO_CAM == 1)
                //{
                //    checkEdit129.Checked = true;
                //    textEdit39.Enabled = true;
                //    textEdit39.Text = eyeSurgDesc.THUOC_TE;
                //}
                //- PP_VO_CAM: 1: me, 5: Te tai cho
                //- THUOC_TE:

                if (eyeSurgDesc.VI_TRI_DUONG_RACH == null || eyeSurgDesc.VI_TRI_DUONG_RACH == 5)
                {
                    checkEdit117.Checked = true;
                }
                else if (eyeSurgDesc.VI_TRI_DUONG_RACH == 3)
                {
                    checkEdit142.Checked = true;
                }
                else if (eyeSurgDesc.VI_TRI_DUONG_RACH == 6)
                {
                    checkEdit124.Checked = true;
                }
                else if (eyeSurgDesc.VI_TRI_DUONG_RACH == 7)
                {
                    checkEdit120.Checked = true;
                }
                else if (eyeSurgDesc.VI_TRI_DUONG_RACH == 8)
                {
                    checkEdit122.Checked = true;
                }
                //- VI_TRI_DUONG_RACH: Do va ve vi tri duong rach cach bo tu do. 3, 5, 6, 7, 8 mm

                //textEdit38.Text = eyeSurgDesc.THUOC_TE_TAI_CHO;
                //- THUOC_TE_TAI_CHO: Ten loai thuoc te tai cho

                if (eyeSurgDesc.PHAU_TICH_DA_DU == null || eyeSurgDesc.PHAU_TICH_DA_DU == 1)
                {
                    checkEdit118.Checked = true;
                }
                else if (eyeSurgDesc.PHAU_TICH_DA_DU == 0)
                {
                    checkEdit119.Checked = true;
                }
                //- PHAU_TICH_DA_DU: Phau tich phan da du. 1: Co; 0: Khong

                if (eyeSurgDesc.CAT_CO_VONG_MI == null || eyeSurgDesc.CAT_CO_VONG_MI == 1)
                {
                    checkEdit111.Checked = true;
                }
                else if (eyeSurgDesc.CAT_CO_VONG_MI == 0)
                {
                    checkEdit114.Checked = true;
                }
                //- CAT_CO_VONG_MI: 1: Co; 0: Khong

                if (eyeSurgDesc.TACH_CO_NANG_MI_KHOI_KM == null || eyeSurgDesc.TACH_CO_NANG_MI_KHOI_KM == 1)
                {
                    checkEdit99.Checked = true;
                }
                else if (eyeSurgDesc.TACH_CO_NANG_MI_KHOI_KM == 0)
                {
                    checkEdit102.Checked = true;
                }
                //- TACH_CO_NANG_MI_KHOI_KM: Co dinh va tach co nang mi ra khoi ket mac va co duoi da den day chang whitnall. 1: Co; 0: Khong.

                if (eyeSurgDesc.CAT_NGAN_CO_NANG_MI == null || eyeSurgDesc.CAT_NGAN_CO_NANG_MI == 1)
                {
                    checkEdit105.Checked = true;
                }
                else if (eyeSurgDesc.CAT_NGAN_CO_NANG_MI == 2)
                {
                    checkEdit91.Checked = true;
                }
                else if (eyeSurgDesc.CAT_NGAN_CO_NANG_MI == 3)
                {
                    checkEdit98.Checked = true;
                }
                //- CAT_NGAN_CO_NANG_MI: 1: 10-12MM; 2: 18-24MM; 3: >24mm

                if (eyeSurgDesc.GAP_CO_NANG_MI == null || eyeSurgDesc.GAP_CO_NANG_MI == 1)
                {
                    //checkEdit90.Checked = true;
                    textEdit35.EditValue = eyeSurgDesc.GAP_CO_NANG_MI_KHOANG;
                    textEdit35.Enabled = true;
                }
                else if (eyeSurgDesc.GAP_CO_NANG_MI == 0)
                {
                    textEdit35.Text = "";
                    textEdit35.Enabled = false;
                    //checkEdit92.Checked = true;
                }
                //- GAP_CO_NANG_MI: 1: Co; 0: Khong
                //- GAP_CO_NANG_MI_KHOANG: (mm)

                //- TREO_MI_CO_TRAN: 1: Co; 0: Khong
                if (eyeSurgDesc.TREO_MI_CO_TRAN == null || eyeSurgDesc.TREO_MI_CO_TRAN == 1)
                {
                    //checkEdit89.Checked = true;
                    if (eyeSurgDesc.TREO_MI_CO_TRAN_LOAI == 1)
                    {
                        checkEdit87.Checked = true;
                    }
                    else if (eyeSurgDesc.TREO_MI_CO_TRAN_LOAI == 2)
                    {
                        checkEdit84.Checked = true;
                    }
                    else if (eyeSurgDesc.TREO_MI_CO_TRAN_LOAI == 3)
                    {
                        checkEdit88.Checked = true;
                    }
                    //else
                    //{
                    //    checkEdit84.Checked = true;
                    //}
                }
                else if (eyeSurgDesc.TREO_MI_CO_TRAN == 0)
                {
                    checkEdit87.Checked = checkEdit84.Checked = checkEdit88.Checked = false;
                    checkEdit95.Checked = true;
                }
                //- TREO_MI_CO_TRAN_LOAI: 1: Can co dui; 2: Bang chi; 3: Bang silicon

                //textEdit28.Text = "";//TODO
                if (eyeSurgDesc.KHAU_CO_DINH_CO_NANG_MI == null || eyeSurgDesc.KHAU_CO_DINH_CO_NANG_MI == 1)
                {
                    checkEdit85.Checked = true;
                }
                else if (eyeSurgDesc.KHAU_CO_DINH_CO_NANG_MI == 0)
                {
                    checkEdit83.Checked = true;
                }
                //- KHAU_CO_DINH_CO_NANG_MI: Khau co dinh co nang mi vao 1/3 bo tren sun mi tren 3 not chu U bang chi vicryl. 0: Khong; 1: Co

                if (eyeSurgDesc.LUON_CHI_HINH_NGU_GIAC == null || eyeSurgDesc.LUON_CHI_HINH_NGU_GIAC == 0)
                {
                    checkEdit86.Checked = true;
                }
                else if (eyeSurgDesc.LUON_CHI_HINH_NGU_GIAC == 1)
                {
                    checkEdit80.Checked = true;
                }
                //- LUON_CHI_HINH_NGU_GIAC: Luon chi hoac silicon theo hinh ngu giac. 0: Khong; 1: Co

                //textEdit24.Text = eyeSurgDesc.KHAU_DA_MI_TAO_MI_CHI;
                //- KHAU_DA_MI_TAO_MI_CHI: Chi dung de khau da mi va tแบกo mi bang chi

                //textEdit25.Text = eyeSurgDesc.TRA_MAT_THUOC;
                //- TRA_MAT_THUOC:
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadPTGlocom()
        {
            try
            {
                if (isReset) return;
                DefaultChosenGlocom();
                TextDefaultGlocom();
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();                

                //- CHAN_DOAN: 6: glocom goc dong, 7: glocom goc mo, 8: glocom bam sinh, 9: glocom thu phat
                if (eyeSurgDesc.CHAN_DOAN == 6)
                    Glocom__raGloocomGocDong.Checked = true;
                else if (eyeSurgDesc.CHAN_DOAN == 7)
                    Glocom__raGloocomGocMo.Checked = true;
                else if (eyeSurgDesc.CHAN_DOAN == 8)
                    Glocom__raGloocomBamSinh.Checked = true;
                else if (eyeSurgDesc.CHAN_DOAN == 9)
                    Glocom__raGloocomThuPhat.Checked = true;

                //- NGUYEN_NHAN:
                Glocom__txtDo.Text = eyeSurgDesc.NGUYEN_NHAN;

                //- GIAI_DOAN_BENH: 1: tiem tang, 2: so phat, 3: tien trien, 4: tram trong, 5: gan tuyet doi, 6: tuyet doi
                if (eyeSurgDesc.GIAI_DOAN_BENH == 1)
                    Glocom__raGiaDoanBenhTiemTang.Checked = true;
                else if (eyeSurgDesc.GIAI_DOAN_BENH == 2)
                    Glocom__raGiaDoanBenhSoPhat.Checked = true;
                else if (eyeSurgDesc.GIAI_DOAN_BENH == 3)
                    Glocom__raGiaDoanBenhTienTrien.Checked = true;
                else if (eyeSurgDesc.GIAI_DOAN_BENH == 4)
                    Glocom__raGiaDoanBenhTranTrong.Checked = true;
                else if (eyeSurgDesc.GIAI_DOAN_BENH == 5)
                    Glocom__raGiaDoanBenhGanTuyet.Checked = true;
                else if (eyeSurgDesc.GIAI_DOAN_BENH == 6)
                    Glocom__raGiaDoanBenhTuyetDoi.Checked = true;

                //- PP_PT_CAT_MONG_MAT: 0: khong, 1: co
                checkEdit9.Checked = (eyeSurgDesc.PP_PT_CAT_MONG_MAT == null || eyeSurgDesc.PP_PT_CAT_MONG_MAT == 1);

                //- PP_PHAU_THUAT: 5: cat be, 6: rach be, 7: mo be, 8: khac
                if (eyeSurgDesc.PP_PHAU_THUAT == 5)
                    checkEdit1.Checked = true;
                if (eyeSurgDesc.PP_PHAU_THUAT == 6)
                    checkEdit2.Checked = true;
                if (eyeSurgDesc.PP_PHAU_THUAT == 7)
                    checkEdit7.Checked = true;
                if (eyeSurgDesc.PP_PHAU_THUAT == 8)
                    checkEdit8.Checked = true;

                //- PP_VO_CAM: 1: me, 2: te tai mat, 3: duoi bao tenon, 4: Canh nhan cau
                if (eyeSurgDesc.PP_VO_CAM == 1)
                    Glocom__raPPVoCamGayMe.Checked = true;
                else if (eyeSurgDesc.PP_VO_CAM == 2)
                    Glocom__raPPVoCamTeTaiMat.Checked = true;
                else if (eyeSurgDesc.PP_VO_CAM == 3)
                    Glocom__raPPVoCamViTriBaoTenon.Checked = true;
                else if (eyeSurgDesc.PP_VO_CAM == 4)
                    Glocom__raPPVoCamCNC.Checked = true;

                //- THUOC_TE:
                Glocom__txtPPVoCamLoaiThuoc.Text = eyeSurgDesc.THUOC_TE;

                //- CO_DINH_NHAN_CAU: 1: Vanh mi, 2: Chi co truc, 3: chi giac mac
                if (eyeSurgDesc.CO_DINH_NHAN_CAU == null || eyeSurgDesc.CO_DINH_NHAN_CAU == 1)
                    Glocom__raCoDinhNhanCauVanhMi.Checked = true;
                else if (eyeSurgDesc.CO_DINH_NHAN_CAU == 2)
                    Glocom__raCoDinhNhanCauCoTruc.Checked = true;
                else if (eyeSurgDesc.CO_DINH_NHAN_CAU == 3)
                    Glocom__raCoDinhNhanCauChiGiacMac.Checked = true;

                //- TAO_VAT_KM_KINH_TUYEN:
                Glocom__txtCoDinhNhanCauKinhTuyen.Text = eyeSurgDesc.TAO_VAT_KM_KINH_TUYEN;

                //- TAO_VAT_KM_VI_TRI: 1: day cung do, 2: day vung ria
                if (eyeSurgDesc.TAO_VAT_KM_VI_TRI == 1)
                    Glocom__raCoDinhNhanCauDayCungDo.Checked = true;
                else if (eyeSurgDesc.CO_DINH_NHAN_CAU == 2)
                    Glocom__raCoDinhNhanCauDayVungRia.Checked = true;

                //- TINH_TRANG_BAO_TENON: 1: Binh thuong, 2: day, 3: xo
                if (eyeSurgDesc.TINH_TRANG_BAO_TENON == 1)
                    Glocom__raTinhTrangBaoTenonBT.Checked = true;
                else if (eyeSurgDesc.TINH_TRANG_BAO_TENON == 2)
                    Glocom__raTinhTrangBaoTenonGay.Checked = true;
                else if (eyeSurgDesc.TINH_TRANG_BAO_TENON == 3)
                    Glocom__raTinhTrangBaoTenonSo.Checked = true;

                //- UC_CHE_TAO_XO: 0: Khong, 1: co
                //- UC_CHE_TAO_XO_TT_BS: 1: 5FU; 2: MMC; 3: Olcogen; 4: Màng ối; 5: Khac
                //- UC_CHE_TAO_XO_TT_BS_KHAC:
                //- UC_CHE_TAO_XO_VITRI: 1: Tren nap CM; 2: Duoi nap CM; 3: Ca hai;
                //- UC_CHE_TAO_XO_THOIGIAN:

                //if (eyeSurgDesc.UC_CHE_TAO_XO == null || eyeSurgDesc.UC_CHE_TAO_XO == 1)
                //    checkEdit43.Checked = true;
                //else if (eyeSurgDesc.UC_CHE_TAO_XO == 0)
                //    checkEdit44.Checked = true;

                if (eyeSurgDesc.UC_CHE_TAO_XO_TT_BS == 1)
                    checkEdit42.Checked = true;
                else if (eyeSurgDesc.UC_CHE_TAO_XO_TT_BS == 2)
                    checkEdit38.Checked = true;
                else if (eyeSurgDesc.UC_CHE_TAO_XO_TT_BS == 3)
                    checkEdit37.Checked = true;
                else if (eyeSurgDesc.UC_CHE_TAO_XO_TT_BS == 4)
                    checkEdit36.Checked = true;
                else if (eyeSurgDesc.UC_CHE_TAO_XO_TT_BS == 5)
                    checkEdit11.Checked = true;
                textEdit4.Text = eyeSurgDesc.UC_CHE_TAO_XO_TT_BS_KHAC;

                if (eyeSurgDesc.UC_CHE_TAO_XO_VITRI == 1)
                    checkEdit31.Checked = true;
                else if (eyeSurgDesc.UC_CHE_TAO_XO_VITRI == 2)
                    checkEdit30.Checked = true;
                else if (eyeSurgDesc.UC_CHE_TAO_XO_VITRI == 3)
                    checkEdit29.Checked = true;
                textEdit16.Text = eyeSurgDesc.UC_CHE_TAO_XO_THOIGIAN;

                //- LANG_BOT_BAO_TENON: 0: Khong, 1: co
                if (eyeSurgDesc.LANG_BOT_BAO_TENON == null || eyeSurgDesc.LANG_BOT_BAO_TENON == 1)
                    checkEdit39.Checked = true;
                else
                    checkEdit40.Checked = true;
                textEdit14.Text = eyeSurgDesc.VAT_CM_HINHDANG;
                spinEdit15.EditValue = eyeSurgDesc.VAT_CM_KICHTHUOC;

                //- CHOC_TP: 0: Khong; 1: Co
                if (eyeSurgDesc.CHOC_TP == null || eyeSurgDesc.CHOC_TP == 1)
                    checkEdit35.Checked = true;
                else
                    checkEdit32.Checked = true;

                //- CAT_MAU_BE: 1: Vi tri vung be; 2: Truoc be; 3: Trach be
                if (eyeSurgDesc.CAT_MAU_BE == 1)
                    checkEdit33.Checked = true;
                else if (eyeSurgDesc.CAT_MAU_BE == 2)
                    checkEdit41.Checked = true;
                else //if (eyeSurgDesc.CAT_MAU_BE == 3)
                    checkEdit34.Checked = true;

                //- CAT_MONG_MAT: 0: Khong; 1: Co
                if (eyeSurgDesc.CAT_MONG_MAT == 1)
                    checkEdit28.Checked = true;
                else if (eyeSurgDesc.CAT_MONG_MAT == 0)
                    checkEdit27.Checked = true;

                spinEdit12.EditValue = eyeSurgDesc.KHAU_NAP_CM_SO_MUI;

                //- KHAU_NAP_CM_CHIRUT: 0: Khong; 1: Co
                if (eyeSurgDesc.KHAU_NAP_CM_CHIRUT == 1)
                {
                    checkEdit20.Checked = true;
                    //textEdit13.Enabled = true;
                }
                else if (eyeSurgDesc.KHAU_NAP_CM_CHIRUT == 0)
                {
                    checkEdit21.Checked = true;
                    //textEdit13.Text = "";
                    //textEdit13.Enabled = false;
                }

                textEdit13.Text = eyeSurgDesc.KHAU_NAP_CM_LOAICHI;

                //- TAI_TAO_TP: 1: Nuoc; 2: Hoi
                if (eyeSurgDesc.TAI_TAO_TP == 1)
                    checkEdit26.Checked = true;
                else if (eyeSurgDesc.TAI_TAO_TP == 2)
                    checkEdit23.Checked = true;

                //- KHAU_KM: 1: Khau vat; 2: Khau mui roi
                if (eyeSurgDesc.KHAU_KM == 1)
                {
                    checkEdit18.Checked = true;
                }
                else if (eyeSurgDesc.KHAU_KM == 2 || eyeSurgDesc.KHAU_KM == null)
                {
                    checkEdit15.Checked = true;
                }
                else
                {
                    spinEdit11.EditValue = null;
                }
                spinEdit11.EditValue = eyeSurgDesc.KHAU_KM_SOMUI;

                //- KHAU_KM_LOAICHI: 1: Nylon, 2: Vicryl
                if (eyeSurgDesc.KHAU_KM_LOAICHI == 1)
                {
                    checkEdit24.Checked = true;
                    textEdit10.Enabled = false;
                }
                else if (eyeSurgDesc.KHAU_KM_LOAICHI == 2)
                {
                    checkEdit22.Checked = true;
                    textEdit10.Enabled = true;
                }
                if (eyeSurgDesc.KHAU_KM_LOAICHI_BS == null)
                    textEdit10.Text = "10/0";
                else
                    textEdit10.Text = eyeSurgDesc.KHAU_KM_LOAICHI_BS;

                textEdit9.Text = eyeSurgDesc.DIEN_BIEN_KHAC;

                //- TIEM_MAT: 0: khong, 1: co
                //- TIEM_MAT_TT_BO_SUNG: 2: duoi km, 3: CNC
                if (eyeSurgDesc.TIEM_MAT == 1)
                {
                    checkEdit25.Checked = true;
                    //checkEdit5.Enabled = checkEdit4.Enabled = textEdit8.Enabled = true;
                    if (eyeSurgDesc.TIEM_MAT_TT_BO_SUNG == 2)
                        checkEdit5.Checked = true;
                    else if (eyeSurgDesc.TIEM_MAT_TT_BO_SUNG == 3)
                        checkEdit4.Checked = true;
                    textEdit8.Text = eyeSurgDesc.TIEM_MAT_THUOC;
                }
                else if (eyeSurgDesc.TIEM_MAT == 0)
                {
                    checkEdit13.Checked = true;
                    checkEdit5.Checked = checkEdit4.Checked = false;
                    //checkEdit5.Enabled = checkEdit4.Enabled = textEdit8.Enabled = false;
                    textEdit8.Text = "";
                }

                textEdit6.Text = eyeSurgDesc.TRA_MAT_THUOC;
                textEdit2.Text = eyeSurgDesc.TRA_MAT_BANG_TT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadPTDucThuyTinhThe()
        {
            try
            {
                if (isReset) return;
                DefaultChosenDucTTT();
                TextDefaultDucTTT();
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //3: Phẫu thuật đục thủy tinh thể:
                if (eyeSurgDesc.CHAN_DOAN == 1)
                    raDucT3Gia.Checked = true;
                else if (eyeSurgDesc.CHAN_DOAN == 2)
                    raDucT3BenhLy.Checked = true;
                else if (eyeSurgDesc.CHAN_DOAN == 3)
                    raLechT3.Checked = true;
                else if (eyeSurgDesc.CHAN_DOAN == 4)
                    raDucT3BamSinh.Checked = true;
                else if (eyeSurgDesc.CHAN_DOAN == 5)
                    raKhongCoT3.Checked = true;
                else
                    raDucT3Gia.Checked = true;
                //- chan_doan: 1: duc T3 gia, 2: duc T3 benh ly, 3: lech t3, 4: Duc t3 bam sinh, 5: khong co T3
                if (eyeSurgDesc.MAT_PHAU_THUAT == 1)
                    raMP.Checked = true;
                else if (eyeSurgDesc.MAT_PHAU_THUAT == 2)
                    raMT.Checked = true;
                else
                    raMP.Checked = true;
                //- chi_dinh_pt_mat: 1: mat phai, 2: mat trai
                if (eyeSurgDesc.PP_PHAU_THUAT == 1)
                    raPhaco.Checked = true;
                else if (eyeSurgDesc.PP_PHAU_THUAT == 2)
                    raNgoaiBao.Checked = true;
                else if (eyeSurgDesc.PP_PHAU_THUAT == 3)
                    raTrongBao.Checked = true;
                else if (eyeSurgDesc.PP_PHAU_THUAT == 4)
                    raTreoCungMac.Checked = true;
                else
                    raPhaco.Checked = true;
                //- chi_dinh_pt_loai: 1: phaco, 2: ngoai bao, 3: trong bao, 4: treo cung mac
                if (eyeSurgDesc.PP_VO_CAM == 1)
                {
                    raMe.Checked = true;
                    //txtLoaiThuocTe.Text = "";
                    //txtLoaiThuocTe.Enabled = false;
                }
                else// if (eyeSurgDesc.PP_VO_CAM == 2)
                {
                    raTeTaiMat.Checked = true;
                    //txtLoaiThuocTe.Enabled = true;
                }
                txtLoaiThuocTe.Text = eyeSurgDesc.THUOC_TE;

                //- pp_vo_cam: 1: me, 2: te tai mat

                //- thuoc_te:
                if (eyeSurgDesc.CO_DINH_NHAN_CAU == 1)
                    raVanhMi.Checked = true;
                else if (eyeSurgDesc.CO_DINH_NHAN_CAU == 2)
                    raChiCoTruc.Checked = true;
                else
                    raVanhMi.Checked = true;
                //- Co_dinh_nhan_cau: 1: Vanh mi, 2: Chi co truc
                if (eyeSurgDesc.TINH_TRANG_T3 == 1)
                    raDoI.Checked = true;
                else if (eyeSurgDesc.TINH_TRANG_T3 == 2)
                    raDoII.Checked = true;
                else if (eyeSurgDesc.TINH_TRANG_T3 == 3)
                    raDoIII.Checked = true;
                else if (eyeSurgDesc.TINH_TRANG_T3 == 4)
                    raDoIV.Checked = true;
                else if (eyeSurgDesc.TINH_TRANG_T3 == 5)
                    raDoV.Checked = true;
                else //if (eyeSurgDesc.TINH_TRANG_T3 == 6)
                    raLechTTT.Checked = true;
                //- Tinh_trang_t3: 1: do 1, 2: do II, 3: do III, 4: do IV, 5: do V, 6: lech TTT

                if (eyeSurgDesc.MO_KM_RIA == null || eyeSurgDesc.MO_KM_RIA == 1)
                {
                    raMoKMRiaKhong.Checked = true;
                    txtMoKMRiaKinhTuyen.Enabled = false;
                    txtMoKMRiaKinhTuyen.Text = "";
                }
                else if (eyeSurgDesc.MO_KM_RIA == 2)
                {
                    raMoKMRiaCo.Checked = true;
                    txtMoKMRiaKinhTuyen.Enabled = true;
                    txtMoKMRiaKinhTuyen.Text = eyeSurgDesc.MO_KM_RIA_KINH_TUYEN;
                }
                //- Mo_km_ria: 1: khong, 2: co                                
                //- Mo_km_ria_kinh_tuyen: (gio)

                if (eyeSurgDesc.MO_VAO_TP == null || eyeSurgDesc.MO_VAO_TP == 1)
                    raMVTPGiacMac.Checked = true;
                else if (eyeSurgDesc.MO_VAO_TP == 2)
                    raMVTPRia.Checked = true;
                else if (eyeSurgDesc.MO_VAO_TP == 3)
                    raMVTPCungMac.Checked = true;
                //- Mo_vao_tp: 1: giac mac; 2: vung ria; 3: cung mac
                textEdit1.Text = eyeSurgDesc.MO_VAO_TP_KINH_TUYEN;
                //- Mo_vao_tp_kinh_tuyen: (gio)
                if (eyeSurgDesc.MO_VAO_TP_KICH_THUOC != null)
                    txtkichthuoc.EditValue = eyeSurgDesc.MO_VAO_TP_KICH_THUOC;
                else
                    txtkichthuoc.EditValue = "";///////////////////////////////////////// ==> vừa sửa ở đây
                //- Mo_vao_tp_kich_thuoc: (mm)

                checkEdit12.Checked = (eyeSurgDesc.MO_VAO_TP_RACH_PHU == null || eyeSurgDesc.MO_VAO_TP_RACH_PHU == 1);
                //- Mo_vao_tp_rach_phu: 0: Khong, 1: co

                if (eyeSurgDesc.NHUOM_BAO == null || eyeSurgDesc.NHUOM_BAO == 0)
                    raNhuomBaoKhong.Checked = true;
                else if (eyeSurgDesc.NHUOM_BAO == 1)
                    raNhuomBaoCo.Checked = true;
                //- Nhuom_bao: 0: khong, 1: co

                if (eyeSurgDesc.LOAI_MO_BAO == null || eyeSurgDesc.LOAI_MO_BAO == 1)
                    raXeBaoTruocT3.Checked = true;
                else if (eyeSurgDesc.LOAI_MO_BAO == 2)
                    raXMoBaoHinhTemThu.Checked = true;
                else if (eyeSurgDesc.LOAI_MO_BAO == 3)
                    raCatBao.Checked = true;
                //- Loai_mo_bao: 1: Xe bao truoc T3, 2: Mo bao hinh tem thu, 3: Cat bao
                checkEdit6.Checked = (eyeSurgDesc.TACH_NHAN == null || eyeSurgDesc.TACH_NHAN == 1);
                //- Tach_nhan: 0: khong, 1: co
                if (eyeSurgDesc.XOAY_NHAN == 1)
                    raXoayNhanKhoKhan.Checked = true;
                else //if (eyeSurgDesc.XOAY_NHAN == 2)
                    raXoayNhanDeDang.Checked = true;
                //- Xoay_nhan: 1: kho khan, 2: de dang
                if (eyeSurgDesc.DAY_NHAN == null || eyeSurgDesc.DAY_NHAN == 0)
                    raDayNhanKhong.Checked = true;
                else if (eyeSurgDesc.DAY_NHAN == 1)
                    raDayNhanCo.Checked = true;
                //- Day_nhan: 0: khong, 1: co
                if (eyeSurgDesc.CACH_DAY_NHAN == 1)
                    raDayNhanBangNuoc.Checked = true;
                else if (eyeSurgDesc.CACH_DAY_NHAN == 2)
                    raDayNhanBangChatNhay.Checked = true;
                //- Cach_day_nhan: 1: bang nuoc, 2: bang chat nhay

                if (eyeSurgDesc.TAN_NHAN_NANG_LUONG != null)
                    txtNangLuong.EditValue = eyeSurgDesc.TAN_NHAN_NANG_LUONG;
                else
                    txtNangLuong.EditValue = "";///////////////////////////////// ==> vừa sửa ở đây
                //- Tan_nhan_nang_luong: (%)

                if (eyeSurgDesc.TAN_NHAN_LUC_HUT != null)
                    txtLucHut.EditValue = eyeSurgDesc.TAN_NHAN_LUC_HUT;
                else
                    txtLucHut.EditValue = "";////////////////////////////// ==> vừa sửa ở đây
                //- Tan_nhan_luc_hut: (mmHg)
                if (eyeSurgDesc.TAN_NHAN_TOC_DO_DC != null)
                    textEdit7.EditValue = eyeSurgDesc.TAN_NHAN_TOC_DO_DC;
                else
                    textEdit7.EditValue = "";///////////////////////////// ==> vừa sửa ở đây
                //- Tan_nhan_toc_do_dc:

                if (eyeSurgDesc.HUT_CHAT_T3 == null || eyeSurgDesc.HUT_CHAT_T3 == 1)
                    raHutChatT3IA.Checked = true;
                else if (eyeSurgDesc.HUT_CHAT_T3 == 2)
                    raHutChatT3Kim2Nong.Checked = true;
                //- Hut_chat_t3: 1: IA, 2: Kim hai nong

                if (eyeSurgDesc.DAT_IOL_LOAI == null || eyeSurgDesc.DAT_IOL_LOAI == 1)
                    raDatIOLMem.Checked = true;
                else if (eyeSurgDesc.DAT_IOL_LOAI == 2)
                    raDatIOLCung.Checked = true;
                //- Dat_iol_loai: 1: mem, 2: cung

                if (eyeSurgDesc.DAT_IOL_CACH_THUC == 1)
                    raDatIOLBangPince.Checked = true;
                else if (eyeSurgDesc.DAT_IOL_CACH_THUC == 2)
                    raDatIOLBangSung.Checked = true;
                else// if (eyeSurgDesc.DAT_IOL_CACH_THUC == 3)
                    raDatIOLCoDinhCM.Checked = true;
                //- Dat_iol_cach_thuc: 1: bang pince, 2: bang sung, 3: co dinh CM

                if (eyeSurgDesc.RACH_BAO_SAU == null || eyeSurgDesc.RACH_BAO_SAU == 0)
                {
                    raRachBaoSauKhong.Checked = true;
                    txtViTriRachForraRachBaoSau.Enabled = spinKichThuocRachForraRachBaoSau.Enabled = false;
                    txtViTriRachForraRachBaoSau.Text = "";
                    //- Rach_bao_sau_vi_tri:
                    spinKichThuocRachForraRachBaoSau.EditValue = null;
                }
                else if (eyeSurgDesc.RACH_BAO_SAU == 1)
                {
                    raRachBaoSauCo.Checked = true;
                    txtViTriRachForraRachBaoSau.Enabled = spinKichThuocRachForraRachBaoSau.Enabled = true;
                    txtViTriRachForraRachBaoSau.Text = eyeSurgDesc.RACH_BAO_SAU_VI_TRI;
                    //- Rach_bao_sau_vi_tri:
                    spinKichThuocRachForraRachBaoSau.EditValue = eyeSurgDesc.RACH_BAO_SAU_KICH_THUOC;
                    //- Rach_bao_sau_kich_thuoc:
                }
                //- Rach_bao_sau: 0: khong; 1: co

                if (eyeSurgDesc.CAT_BAO_SAU == null || eyeSurgDesc.CAT_BAO_SAU == 0)
                {
                    raRBSCBSKhong.Checked = true;
                    eyeSurgDesc.CAT_BAO_SAU_CACH_THUC = 0;//////////////// ==> sửa ở đây
                }
                else if (eyeSurgDesc.CAT_BAO_SAU == 1)
                    raRBSCBSCo.Checked = true;
                //- Cat_bao_sau: 0: khong, 1: co

                if (eyeSurgDesc.CAT_BAO_SAU_CACH_THUC == 1)
                    raRBSCBSCatBangKeo.Checked = true;
                else if (eyeSurgDesc.CAT_BAO_SAU_CACH_THUC == 2)
                    raRBSCBSMayCatDK.Checked = true;
                //- Cat_bao_sau_cach_thuc: 1: bang keo, 2: may cat DK

                if (eyeSurgDesc.CAT_MONG_MAT_NGOAI_VI == 1)
                {
                    raRBSCMMNVKhong.Checked = true;
                    txtViTriRachForraRBSCMMNV.Enabled = false;
                    txtViTriRachForraRBSCMMNV.EditValue = "";
                }
                else if (eyeSurgDesc.CAT_MONG_MAT_NGOAI_VI == 2)
                {
                    raRBSCMMNVCo.Checked = true;
                    txtViTriRachForraRBSCMMNV.Enabled = true;
                    txtViTriRachForraRBSCMMNV.EditValue = eyeSurgDesc.CAT_MONG_MAT_NGOAI_VI_VITRI;
                }
                else
                {
                    txtViTriRachForraRBSCMMNV.Enabled = false;
                    txtViTriRachForraRBSCMMNV.EditValue = "";
                }
                //- Cat_mong_mat_ngoai_vi: 0: khong, 1: co                
                //- Cat_mong_mat_ngoai_vi_vitri:

                if (eyeSurgDesc.PHUC_HOI_VET_MO == null || eyeSurgDesc.PHUC_HOI_VET_MO == 1)
                    raPhucHoiVetMoBomPhu.Checked = true;
                else if (eyeSurgDesc.PHUC_HOI_VET_MO == 2)
                    raPhucHoiVetMoKhauVat.Checked = true;
                //- Phu_hoi_vet_mo: 1: bom phu, 2: khau vat

                if (eyeSurgDesc.TIEM_MAT == null || eyeSurgDesc.TIEM_MAT == 1)
                    raTiemMatCo.Checked = true;
                else if (eyeSurgDesc.TIEM_MAT == 0)
                    raTiemMatKhong.Checked = true;
                //- Tiem_mat: 0: khong, 1: co

                if (eyeSurgDesc.TIEM_MAT_TT_BO_SUNG == null || eyeSurgDesc.TIEM_MAT_TT_BO_SUNG == 1)
                    raTiemMatTienPhong.Checked = true;
                else if (eyeSurgDesc.TIEM_MAT_TT_BO_SUNG == 2)
                    raTiemMatDuoiKM.Checked = true;
                else if (eyeSurgDesc.TIEM_MAT_TT_BO_SUNG == 3)
                    raTiemMatCNC.Checked = true;
                //- Tiem_mat_tt_bo_sung: 1: tien phong, 2: duoi km, 3: CNC
                if (eyeSurgDesc.TIEM_MAT_THUOC == null)
                    txtLoaiThuocForraTiemMat.Text = "";////////////////////////// ==> vừa sửa ở đây
                else
                    txtLoaiThuocForraTiemMat.Text = eyeSurgDesc.TIEM_MAT_THUOC;
                //- Tiem_mat_thuoc:

                if (eyeSurgDesc.TRA_MAT == null || eyeSurgDesc.TRA_MAT == 1)
                    raTraMatDD.Checked = true;
                else if (eyeSurgDesc.TRA_MAT == 2)
                    raTraMatMo.Checked = true;
                //- Tra_mat: 1: dung dich, 2: mong
                if (eyeSurgDesc.TRA_MAT_THUOC == null)
                    txtLoaiThuocForraTraMat.Text = "";////////////////////////// ==> vừa sửa ở đây
                else
                    txtLoaiThuocForraTraMat.Text = eyeSurgDesc.TRA_MAT_THUOC;
                //- Tra_mat_thuoc:
                if (eyeSurgDesc.TRA_MAT_BANG_EP == 0)
                    checkEdit10.Checked = false;
                else //if (eyeSurgDesc.TRA_MAT_BANG_EP == 1)
                    checkEdit10.Checked = true;
                //- Tra_mat_bang_ep: 0: khong, 1: co
                txtDienBienKhac.Text = eyeSurgDesc.DIEN_BIEN_KHAC;
                //- Dien_bien_khac
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadTTMongMat()
        {
            try
            {
                if (isReset) return;
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //7. Thủ thuật mống mắt:
                if (eyeSurgDesc.CHAN_DOAN_NGHI_NGO_GLOCOM == 1)
                    checkEdit269.Checked = true;
                else //if (eyeSurgDesc.CHAN_DOAN_NGHI_NGO_GLOCOM == 2)
                    checkEdit267.Checked = true;
                //- CHAN_DOAN_NGHI_NGO_GLOCOM: 1: Mat phai nghi ngo glocom, tien phong goc hep. 2: Mat trai nghi ngo glocom, tien phong goc hep.
                if (eyeSurgDesc.CHAN_DOAN_GLOCOM == 1)
                    checkEdit266.Checked = true;
                else //if (eyeSurgDesc.CHAN_DOAN_GLOCOM == 2)
                    checkEdit268.Checked = true;
                //- CHAN_DOAN_GLOCOM: 1: Mat phai glocom, tien phong goc hep. 2: Mat trai glocom, tien phong goc hep.
                if (eyeSurgDesc.LASER_YAG_NANG_LUONG == null || eyeSurgDesc.LASER_YAG_NANG_LUONG == 1)
                {
                    checkEdit264.Checked = true;
                    //textEdit64.Enabled = false;
                    textEdit64.Text = "";
                }
                else if (eyeSurgDesc.LASER_YAG_NANG_LUONG == 2)
                {
                    checkEdit261.Checked = true;
                    //textEdit64.Enabled = false;
                    textEdit64.Text = "";
                }
                else
                {
                    checkEdit264.Checked = checkEdit261.Checked = false;
                    //textEdit64.Enabled = true;
                    textEdit64.Text = eyeSurgDesc.LASER_YAG_NANG_LUONG_KHAC;
                }
                //- LASER_YAG_NANG_LUONG: 1: 4mj; 2: 5mj; 3: Khac
                //- LASER_YAG_NANG_LUONG_KHAC:

                if (eyeSurgDesc.LASER_YAG_DIEM_NO == 1)
                    checkEdit258.Checked = true;
                else //if (eyeSurgDesc.LASER_YAG_DIEM_NO == 2)
                    checkEdit257.Checked = true;
                //- LASER_YAG_DIEM_NO: 1: Truoc tieu diem; 2: Sau tieu diem

                if (eyeSurgDesc.VI_TRI_CAT_MONG_CHU_BIEN == 11)
                {
                    checkEdit256.Checked = true;
                    spinEdit63.EditValue = null;
                }
                else if (eyeSurgDesc.VI_TRI_CAT_MONG_CHU_BIEN == null || eyeSurgDesc.VI_TRI_CAT_MONG_CHU_BIEN == 13)
                {
                    checkEdit255.Checked = true;
                    spinEdit63.EditValue = null;
                }
                else
                {
                    checkEdit256.Checked = checkEdit255.Checked = false;
                    spinEdit63.EditValue = eyeSurgDesc.VI_TRI_CAT_MONG_CHU_BIEN;
                }
                //- VI_TRI_CAT_MONG_CHU_BIEN: (cho điền số giờ)
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadTTMong()
        {
            try
            {
                if (isReset) return;
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //2. Phẫu thuật mộng:
                //- MAT_PHAU_THUAT: 1: mat phai, 2: mat trai
                if (eyeSurgDesc.MAT_PHAU_THUAT == null || eyeSurgDesc.MAT_PHAU_THUAT == 1)
                    checkEdit64.Checked = true;
                else if (eyeSurgDesc.MAT_PHAU_THUAT == 2)
                    checkEdit71.Checked = true;

                //- PP_VO_CAM: 1: me, 2: te tai mat
                if (eyeSurgDesc.PP_VO_CAM == 1)
                {
                    checkEdit104.Checked = true;
                    //textEdit33.Enabled = false;
                    //textEdit33.Text = "";
                }
                else //if (eyeSurgDesc.PP_VO_CAM == 2)
                {
                    checkEdit103.Checked = true;
                    //textEdit33.Enabled = true;                    
                }
                //textEdit33.Text = eyeSurgDesc.THUOC_TE;
                SetDefaultValueLoadTTMong();
                checkEdit112.Checked = (eyeSurgDesc.DAT_VANH_MI == null || eyeSurgDesc.DAT_VANH_MI == 1);
                //- DAT_VANH_MI: 0: Khong; 1: co
                checkEdit108.Checked = (eyeSurgDesc.TIEM_LIDOCANIE_THAN_MONG == null || eyeSurgDesc.TIEM_LIDOCANIE_THAN_MONG == 1);
                //- TIEM_LIDOCANIE_THAN_MONG: 0: Khong; 1: Co
                checkEdit97.Checked = (eyeSurgDesc.CAT_DAU_MONG == null || eyeSurgDesc.CAT_DAU_MONG == 1);
                //- CAT_DAU_MONG: 0: Khong; 1: Co
                checkEdit100.Checked = (eyeSurgDesc.PHAN_TICH_THAN_MONG == null || eyeSurgDesc.PHAN_TICH_THAN_MONG == 1);
                //- PHAN_TICH_THAN_MONG: 0: Khong; 1: Co
                checkEdit101.Checked = (eyeSurgDesc.DOT_CAM_MAU == null || eyeSurgDesc.DOT_CAM_MAU == 1);
                //- DOT_CAM_MAU: 0: Khong; 1: Co
                checkEdit68.Checked = (eyeSurgDesc.GOT_GIAC_MAC_DAU_MONG == null || eyeSurgDesc.GOT_GIAC_MAC_DAU_MONG == 1);
                //- GOT_GIAC_MAC_DAU_MONG: 0: Khong; 1: Co
                if (eyeSurgDesc.LAY_MANH_KM_SAT_RIA == 1)
                    checkEdit69.Checked = true;
                else if (eyeSurgDesc.LAY_MANH_KM_SAT_RIA == 2)
                    checkEdit70.Checked = true;
                //- LAY_MANH_KM_SAT_RIA: 1: Tren, 2: duoi

                if (eyeSurgDesc.LAY_MANH_KM_SAT_RIA_KT == null)
                    txtEdit32.EditValue = "5 x 8 mm";
                else
                    txtEdit32.EditValue = eyeSurgDesc.LAY_MANH_KM_SAT_RIA_KT;
                //- LAY_MANH_KM_SAT_RIA_KT: (kich thuoc)

                checkEdit106.Checked = (eyeSurgDesc.LAY_MANH_MANG_OI == 1);
                //- LAY_MANH_MANG_OI: 0: Khong; 1: co
                if (checkEdit106.Checked)
                {
                    txtEdit21.Enabled = true;
                    txtEdit21.EditValue = eyeSurgDesc.LAY_MANH_MANG_OI_KT;
                }
                else
                {
                    txtEdit21.Enabled = false;
                    txtEdit21.EditValue = null;
                }
                //- LAY_MANH_MANG_OI_KT: (Kich thuoc)

                if (eyeSurgDesc.KHAU_MANH_GHEP_CHI == null || eyeSurgDesc.KHAU_MANH_GHEP_CHI == 1)
                    checkEdit115.Checked = true;
                else if (eyeSurgDesc.KHAU_MANH_GHEP_CHI == 2)
                    checkEdit96.Checked = true;
                //- KHAU_MANH_GHEP_CHI: 1: Nylon 10-O; 2: Vycryl 7-O

                spinEdit19.EditValue = (eyeSurgDesc.KHAU_MANH_GHEP_CHI_SO_MUI ?? 8);
                //- KHAU_MANH_GHEP_CHI_SO_MUI:
                checkEdit72.Checked = (eyeSurgDesc.KHAU_KM_CHE_PHAN_CAT_KM == null || eyeSurgDesc.KHAU_KM_CHE_PHAN_CAT_KM == 1);
                //- KHAU_KM_CHE_PHAN_CAT_KM: 0: Khong; 1: co
                if (checkEdit72.Checked)
                {
                    spinEdit31.Enabled = true;
                    spinEdit31.EditValue = (eyeSurgDesc.KHAU_KM_CHE_PHAN_CAT_KM_SO_MUI ?? 2);
                }
                else
                {
                    spinEdit31.Enabled = false;
                    spinEdit31.EditValue = null;
                }
                //- KHAU_KM_CHE_PHAN_CAT_KM_SO_MUI:
                if (eyeSurgDesc.BIEN_CHUNG == 1)
                    checkEdit107.Checked = true;
                else if (eyeSurgDesc.BIEN_CHUNG == 2)
                    checkEdit113.Checked = true;
                else if (eyeSurgDesc.BIEN_CHUNG == 3)
                    checkEdit109.Checked = true;
                //- BIEN_CHUNG: 1: Thung cung mac; 2: Giac mac; 3: Cat vao co truc

                textEdit29.Text = eyeSurgDesc.XU_TRI_BIEN_CHUNG;
                //- XU_TRI_BIEN_CHUNG:

                textEdit30.Text = eyeSurgDesc.DIEN_BIEN_KHAC;

                if (eyeSurgDesc.TRA_MAT == 1)
                    checkEdit110.Checked = true;
                else //if (eyeSurgDesc.TRA_MAT == 2)
                    checkEdit3.Checked = true;
                //- TRA_MAT: 1: dung dich, 2: mong

                if (eyeSurgDesc.TRA_MAT_THUOC == null)
                    textEdit27.Text = "Oflovid 0,3% 5ml";
                else
                    textEdit27.Text = eyeSurgDesc.TRA_MAT_THUOC;
                //- TRA_MAT_THUOC:
                checkEdit93.Checked = (eyeSurgDesc.TRA_MAT_BANG_EP == null || eyeSurgDesc.TRA_MAT_BANG_EP == 1);
                //- TRA_MAT_BANG_EP: 0: khong, 1: co
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueLoadTTMong()
        {
            textEdit33.Text = "Lidocain";
        }

        void LoadTTLaser()
        {
            try
            {
                if (isReset) return;
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //- CD_DUC_BAO_SAU_SAU_MO_TTT: Chan doan duc sau mo TTT. 1: Mat phai duc bao sau sau mo PTTT; 2: Mat trai duc bao sau sau mo PTTT
                //- LASER_YAG_NANG_LUONG: 4: 1.5mj; 5: 2mj; 3: Khac
                //- LASER_YAG_NANG_LUONG_KHAC:
                //- LASER_YAG_DIEM_NO: 1: Truoc tieu diem; 2: Sau tieu diem
                //- HINH_DANG_MO_BAO_SAU: 1: Hinh tron; 2: Hinh chu thap

                //6. Thủ thuật laser yag:
                if (eyeSurgDesc.CD_DUC_BAO_SAU_SAU_MO_TTT == 1)
                    checkEdit333.Checked = true;
                else if (eyeSurgDesc.CD_DUC_BAO_SAU_SAU_MO_TTT == 2)
                    checkEdit329.Checked = true;
                else
                    checkEdit333.Checked = true;
                //- CD_DUC_BAO_SAU_SAU_MO_TTT: Chan doan duc sau mo TTT. 1: Mat phai duc bao sau sau mo PTTT; 2: Mat trai duc bao sau sau mo PTTT
                if (!eyeSurgDesc.LASER_YAG_NANG_LUONG.HasValue || (eyeSurgDesc.LASER_YAG_NANG_LUONG.HasValue && eyeSurgDesc.LASER_YAG_NANG_LUONG == 4))
                {
                    checkEdit328.Checked = true;
                    textEdit75.Text = "";
                }
                else if (eyeSurgDesc.LASER_YAG_NANG_LUONG == 5)
                {
                    checkEdit325.Checked = true;
                    textEdit75.Text = "";
                }
                else if (eyeSurgDesc.LASER_YAG_NANG_LUONG == 3)
                {
                    checkEdit328.Checked = checkEdit325.Checked = false;
                    textEdit75.Text = eyeSurgDesc.LASER_YAG_NANG_LUONG_KHAC;
                }
                //- LASER_YAG_NANG_LUONG: 4: 1.5mj; 5: 2mj; 3: Khac
                //- LASER_YAG_NANG_LUONG_KHAC:

                if (eyeSurgDesc.LASER_YAG_DIEM_NO == 1)
                    checkEdit322.Checked = true;
                else if (eyeSurgDesc.LASER_YAG_DIEM_NO == 2)
                    checkEdit321.Checked = true;
                else
                    checkEdit321.Checked = true;
                //- LASER_YAG_DIEM_NO: 1: Truoc tieu diem; 2: Sau tieu diem
                if (eyeSurgDesc.HINH_DANG_MO_BAO_SAU == 1)
                    checkEdit320.Checked = true;
                else if (eyeSurgDesc.HINH_DANG_MO_BAO_SAU == 2)
                    checkEdit319.Checked = true;
                else
                    checkEdit320.Checked = true;
                //- HINH_DANG_MO_BAO_SAU: 1: Hinh tron; 2: Hinh chu thap
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetDataPTTaiTaoLeQuan()
        {
            try
            {
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //4. Phẫu thuật tái tạo lệ quản:
                //- CHAN_DOAN_DUT_LE_QUAN: 1: Dut le quan duoi; 2: Dut le quan tren; 3: Dut le quan tren duoi
                if (checkEdit205.Checked)
                {
                    eyeSurgDesc.CHAN_DOAN_DUT_LE_QUAN = 1;
                }
                else if (checkEdit201.Checked)
                {
                    eyeSurgDesc.CHAN_DOAN_DUT_LE_QUAN = 2;
                }
                else if (checkEdit204.Checked)
                {
                    eyeSurgDesc.CHAN_DOAN_DUT_LE_QUAN = 3;
                }
                else
                    eyeSurgDesc.CHAN_DOAN_DUT_LE_QUAN = null;

                eyeSurgDesc.DUT_LE_QUAN_GIO_THU = spinEdit51.EditValue != null ? (long?)spinEdit51.Value : null;
                //- DUT_LE_QUAN_GIO_THU: Dut le quan gio thu

                if (checkEdit200.Checked)
                {
                    eyeSurgDesc.PP_PHAU_THUAT_LE_QUAN = 1;
                }
                else if (checkEdit197.Checked)
                {
                    eyeSurgDesc.PP_PHAU_THUAT_LE_QUAN = 2;
                }
                eyeSurgDesc.PHAU_THUAT_LAN_THU = spinEdit52.EditValue != null ? (long?)spinEdit52.Value : null;
                //- PP_PHAU_THUAT_LE_QUAN: 1: Dat ong silicon 1 le quan; 2: Dat ong silicon hinh nhan
                //- PHAU_THUAT_LAN_THU:

                if (checkEdit194.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM = 1;
                }
                else if (checkEdit193.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM = 2;
                }
                else
                    eyeSurgDesc.PP_VO_CAM = null;

                //- PP_VO_CAM_DUOI_HOC: Vo cam duoi hoc. 0: Khong; 1: co
                if (checkEdit192.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM_DUOI_HOC = 1;
                }
                else
                    eyeSurgDesc.PP_VO_CAM_DUOI_HOC = 0;

                if (checkEdit191.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM_MUI = 1;
                }
                else
                    eyeSurgDesc.PP_VO_CAM_MUI = 0;
                //- PP_VO_CAM_MUI: Vo cam vung mui. 0: Khong; 1: co

                if (checkEdit183.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM_VUNG_TUI_LE = 1;
                }
                else
                    eyeSurgDesc.PP_VO_CAM_VUNG_TUI_LE = 0;
                //- PP_VO_CAM_VUNG_TUI_LE: Vo cam vung tui le. 0: Khong; 1: co

                if (checkEdit185.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM_THE0_VET_RACH_MI = 1;
                }
                else
                    eyeSurgDesc.PP_VO_CAM_THE0_VET_RACH_MI = 0;
                eyeSurgDesc.THUOC_TE = textEdit49.Text;
                //- PP_VO_CAM_THE0_VET_RACH_MI: Vo cam doc theo vet rach mi. 0: Khong; 1: co

                if (checkEdit181.Checked)
                {
                    eyeSurgDesc.LAY_DI_VAT = 1;
                }
                else if (checkEdit184.Checked)
                {
                    eyeSurgDesc.LAY_DI_VAT = 0;
                }
                //- LAY_DI_VAT: Kiem tra vet thuong, lay di vat. 0: Khong; 1: Co

                if (checkEdit175.Checked)
                {
                    eyeSurgDesc.LE_QUAN_LANH_DUT = 1;
                }
                else if (checkEdit182.Checked)
                {
                    eyeSurgDesc.LE_QUAN_LANH_DUT = 0;
                }
                //- LE_QUAN_LANH_DUT: Kiem tra le quan lanh. 0: Khong dut; 1: Co dut

                if (checkEdit180.Checked)
                {
                    eyeSurgDesc.TIM_DAU_DUT_NGOAI = 1;
                }
                else
                {
                    eyeSurgDesc.TIM_DAU_DUT_NGOAI = 0;
                }
                //- TIM_DAU_DUT_NGOAI: 0: Khong; 1: Co

                if (checkEdit206.Checked)
                {
                    eyeSurgDesc.TIM_DAU_DUT_TRONG = 1;
                }
                else
                    eyeSurgDesc.TIM_DAU_DUT_TRONG = 0;
                //- TIM_DAU_DUT_TRONG: 0: Khong; 1: Co

                //- DAU_DUT_TRONG_VITRI: Vi tri dau dut trong. 1: 1/3 ngoai; 2: 1/3 giua; 3: 1/3 trong
                if (checkEdit190.Checked)
                {
                    eyeSurgDesc.DAU_DUT_TRONG_VITRI = 1;
                }
                else if (checkEdit187.Checked)
                {
                    eyeSurgDesc.DAU_DUT_TRONG_VITRI = 2;
                }
                else if (checkEdit189.Checked)
                {
                    eyeSurgDesc.DAU_DUT_TRONG_VITRI = 3;
                }
                else
                {
                    eyeSurgDesc.DAU_DUT_TRONG_VITRI = null;
                }

                //- DAT_LE_QUAN: 1: 1 le quan; 2: hinh nhan; 3: Mini monoka
                if (checkEdit188.Checked)
                {
                    eyeSurgDesc.DAT_LE_QUAN = 1;
                }
                else if (checkEdit178.Checked)
                {
                    eyeSurgDesc.DAT_LE_QUAN = 2;
                }
                else if (checkEdit186.Checked)
                {
                    eyeSurgDesc.DAT_LE_QUAN = 3;
                }
                else
                {
                    eyeSurgDesc.DAT_LE_QUAN = null;
                }

                eyeSurgDesc.CHI_NOI_2_DAU_LE_QUAN = textEdit50.Text;
                //- CHI_NOI_2_DAU_LE_QUAN:

                //- TAI_TAO_MI_KET_MAC: 0: Khong; 1: co
                if (checkEdit173.Checked)
                {
                    eyeSurgDesc.TAI_TAO_MI_KET_MAC = 1;
                }
                else
                    eyeSurgDesc.TAI_TAO_MI_KET_MAC = 0;

                //- TAI_TAO_MI_MO_DUOI_DA: 0: Khong; 1: co
                if (checkEdit176.Checked)
                {
                    eyeSurgDesc.TAI_TAO_MI_MO_DUOI_DA = 1;
                }
                else
                    eyeSurgDesc.TAI_TAO_MI_MO_DUOI_DA = 0;
                if (checkEdit177.Checked)
                {
                    eyeSurgDesc.TAI_TAO_MI_DA = 1;
                }
                else
                    eyeSurgDesc.TAI_TAO_MI_DA = 0;
                //- TAI_TAO_MI_DA: 0: Khong; 1: co

                if (checkEdit179.Checked)
                {
                    eyeSurgDesc.CO_DINH_ONG_SILICON = 1;
                }
                else if (checkEdit171.Checked)
                {
                    eyeSurgDesc.CO_DINH_ONG_SILICON = 2;
                }
                else if (checkEdit174.Checked)
                {
                    eyeSurgDesc.CO_DINH_ONG_SILICON = 3;
                }
                else
                    eyeSurgDesc.CO_DINH_ONG_SILICON = null;
                eyeSurgDesc.NYLON_CO_DINH_ONG_SILICON = textEdit44.Text;
                //- CO_DINH_ONG_SILICON: 1: Hinh nhan; 2: Da mi; 3: Nut diem le; 4: Bang chi nylong
                //- NYLON_CO_DINH_ONG_SILICON: Chi nylon co dinh ong silicon

                eyeSurgDesc.DIEN_BIEN_KHAC = textEdit48.Text;
                //- DIEN_BIEN_KHAC

                eyeSurgDesc.TRA_MAT_THUOC = textEdit41.Text;
                //- TRA_MAT_THUOC:

                if (checkEdit144.Checked)
                {
                    eyeSurgDesc.TRA_MAT_BANG_EP = 1;
                }
                else
                    eyeSurgDesc.TRA_MAT_BANG_EP = 0;
                //- TRA_MAT_BANG_EP: 0: khong, 1: co
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetDataPTSupMi()
        {
            try
            {
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //5. Phẫu thuật sụp mí:
                if (checkEdit141.Checked)
                    eyeSurgDesc.CHAN_DOAN_SUP_MI_MP = 1;
                else
                    eyeSurgDesc.CHAN_DOAN_SUP_MI_MP = 0;
                //- CHAN_DOAN_SUP_MI_MT: Sup mi mat trai hay khong. 0: Khong; 1: Co

                if (checkEdit141.Checked)
                {
                    if (checkEdit137.Checked)
                    {
                        eyeSurgDesc.CHAN_DOAN_SUP_MI_MP_DO = 1;
                    }
                    else if (checkEdit140.Checked)
                    {
                        eyeSurgDesc.CHAN_DOAN_SUP_MI_MP_DO = 2;
                    }
                    else if (checkEdit138.Checked)
                    {
                        eyeSurgDesc.CHAN_DOAN_SUP_MI_MP_DO = 3;
                    }
                    else if (checkEdit139.Checked)
                    {
                        eyeSurgDesc.CHAN_DOAN_SUP_MI_MP_DO = 4;
                    }
                    else
                        eyeSurgDesc.CHAN_DOAN_SUP_MI_MP_DO = null;
                }
                else
                    eyeSurgDesc.CHAN_DOAN_SUP_MI_MP_DO = null;
                //- CHAN_DOAN_SUP_MI_MT_DO: 1: Sup do I; 2: Sup do II; 3: Sup do III; 4: Sup do IV

                if (checkEdit136.Checked)
                    eyeSurgDesc.CHAN_DOAN_SUP_MI_MT = 1;
                else
                    eyeSurgDesc.CHAN_DOAN_SUP_MI_MT = 0;
                //- CHAN_DOAN_SUP_MI_MP: Sup mi mat phai hay khong. 0: Khong; 1: Co

                if (checkEdit136.Checked)
                {
                    if (checkEdit133.Checked)
                    {
                        eyeSurgDesc.CHAN_DOAN_SUP_MI_MT_DO = 1;
                    }
                    else if (checkEdit132.Checked)
                    {
                        eyeSurgDesc.CHAN_DOAN_SUP_MI_MT_DO = 2;
                    }
                    else if (checkEdit135.Checked)
                    {
                        eyeSurgDesc.CHAN_DOAN_SUP_MI_MT_DO = 3;
                    }
                    else if (checkEdit131.Checked)
                    {
                        eyeSurgDesc.CHAN_DOAN_SUP_MI_MT_DO = 4;
                    }
                    else
                        eyeSurgDesc.CHAN_DOAN_SUP_MI_MT_DO = null;
                }
                else
                    eyeSurgDesc.CHAN_DOAN_SUP_MI_MT_DO = null;
                //- CHAN_DOAN_SUP_MI_MP_DO: 1: Sup do I; 2: Sup do II; 3: Sup do III; 4: Sup do IV

                if (checkEdit128.Checked)
                {
                    eyeSurgDesc.PP_PT_CO_NANG_MI = 1;
                }
                else if (checkEdit127.Checked)
                {
                    eyeSurgDesc.PP_PT_CO_NANG_MI = 2;
                }
                else if (checkEdit126.Checked)
                {
                    eyeSurgDesc.PP_PT_CO_NANG_MI = 3;
                }
                else if (checkEdit125.Checked)
                {
                    eyeSurgDesc.PP_PT_CO_NANG_MI = 4;
                }
                else if (checkEdit123.Checked)
                {
                    eyeSurgDesc.PP_PT_CO_NANG_MI = 5;
                }
                else
                    eyeSurgDesc.PP_PT_CO_NANG_MI = null;
                //- PP_PT_CO_NANG_MI: 1: Cat ngan co nang mi; 2: Gap can co nang mi

                //else
                //    eyeSurgDesc.PP_PT_TREO_MI_CO_TRAN = null;
                //- PP_PT_TREO_MI_CO_TRAN: 1: treo mi co tran bang can co dui; 2: Treo mi co tran bang chi; 3: Treo mi co tran bang silicon

                if (checkEdit130.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM = 1;
                    eyeSurgDesc.THUOC_TE = "";
                }
                else if (checkEdit129.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM = 5;
                    eyeSurgDesc.THUOC_TE = textEdit39.Text;
                }
                else
                {
                    eyeSurgDesc.PP_VO_CAM = null;
                    eyeSurgDesc.THUOC_TE = "";
                }
                //- PP_VO_CAM: 1: me, 5: Te tai cho
                //- THUOC_TE:

                if (checkEdit117.Checked)
                {
                    eyeSurgDesc.VI_TRI_DUONG_RACH = 5;
                }
                else if (checkEdit142.Checked)
                {
                    eyeSurgDesc.VI_TRI_DUONG_RACH = 3;
                }
                else if (checkEdit124.Checked)
                {
                    eyeSurgDesc.VI_TRI_DUONG_RACH = 6;
                }
                else if (checkEdit120.Checked)
                {
                    eyeSurgDesc.VI_TRI_DUONG_RACH = 7;
                }
                else if (checkEdit122.Checked)
                {
                    eyeSurgDesc.VI_TRI_DUONG_RACH = 8;
                }
                else
                {
                    eyeSurgDesc.VI_TRI_DUONG_RACH = null;
                }
                //- VI_TRI_DUONG_RACH: Do va ve vi tri duong rach cach bo tu do. 3, 5, 6, 7, 8 mm

                eyeSurgDesc.THUOC_TE_TAI_CHO = textEdit38.Text;
                //- THUOC_TE_TAI_CHO: Ten loai thuoc te tai cho

                if (checkEdit118.Checked)
                {
                    eyeSurgDesc.PHAU_TICH_DA_DU = 1;
                }
                else if (checkEdit119.Checked)
                {
                    eyeSurgDesc.PHAU_TICH_DA_DU = 0;
                }
                else
                    eyeSurgDesc.PHAU_TICH_DA_DU = null;
                //- PHAU_TICH_DA_DU: Phau tich phan da du. 1: Co; 0: Khong

                if (checkEdit111.Checked)
                {
                    eyeSurgDesc.CAT_CO_VONG_MI = 1;
                }
                else if (checkEdit114.Checked)
                {
                    eyeSurgDesc.CAT_CO_VONG_MI = 0;
                }
                else
                    eyeSurgDesc.CAT_CO_VONG_MI = null;
                //- CAT_CO_VONG_MI: 1: Co; 0: Khong

                if (checkEdit99.Checked)
                {
                    eyeSurgDesc.TACH_CO_NANG_MI_KHOI_KM = 1;
                }
                else if (checkEdit102.Checked)
                {
                    eyeSurgDesc.TACH_CO_NANG_MI_KHOI_KM = 0;
                }
                else
                    eyeSurgDesc.TACH_CO_NANG_MI_KHOI_KM = null;
                //- TACH_CO_NANG_MI_KHOI_KM: Co dinh va tach co nang mi ra khoi ket mac va co duoi da den day chang whitnall. 1: Co; 0: Khong.

                if (checkEdit105.Checked)
                {
                    eyeSurgDesc.CAT_NGAN_CO_NANG_MI = 1;
                }
                else if (checkEdit91.Checked)
                {
                    eyeSurgDesc.CAT_NGAN_CO_NANG_MI = 2;
                }
                else if (checkEdit98.Checked)
                {
                    eyeSurgDesc.CAT_NGAN_CO_NANG_MI = 3;
                }
                else
                    eyeSurgDesc.CAT_NGAN_CO_NANG_MI = null;
                //- CAT_NGAN_CO_NANG_MI: 1: 10-12MM; 2: 18-24MM; 3: >24mm

                if (checkEdit90.Checked)
                {
                    eyeSurgDesc.GAP_CO_NANG_MI = 1;
                    eyeSurgDesc.GAP_CO_NANG_MI_KHOANG = textEdit35.EditValue != null ? (long?)textEdit35.Value : null;
                }
                else //if (checkEdit92.Checked)
                {
                    eyeSurgDesc.GAP_CO_NANG_MI_KHOANG = null;
                    eyeSurgDesc.GAP_CO_NANG_MI = 0;
                }
                //- GAP_CO_NANG_MI: 1: Co; 0: Khong
                //- GAP_CO_NANG_MI_KHOANG: (mm)

                //- TREO_MI_CO_TRAN: 1: Co; 0: Khong
                if (checkEdit89.Checked)
                {
                    eyeSurgDesc.TREO_MI_CO_TRAN = 1;
                    if (checkEdit87.Checked)
                    {
                        eyeSurgDesc.TREO_MI_CO_TRAN_LOAI = 1;
                    }
                    else if (checkEdit84.Checked)
                    {
                        eyeSurgDesc.TREO_MI_CO_TRAN_LOAI = 2;
                    }
                    else if (checkEdit88.Checked)
                    {
                        eyeSurgDesc.TREO_MI_CO_TRAN_LOAI = 3;
                    }
                    else
                    {
                        eyeSurgDesc.TREO_MI_CO_TRAN_LOAI = null;
                    }
                }
                else if (checkEdit95.Checked)
                {
                    eyeSurgDesc.TREO_MI_CO_TRAN_LOAI = null;
                    eyeSurgDesc.TREO_MI_CO_TRAN = 0;
                }
                //- TREO_MI_CO_TRAN_LOAI: 1: Can co dui; 2: Bang chi; 3: Bang silicon

                //textEdit28.Text = "";//TODO Khâu cố định cơ nâng mi vào 1/3 bờ trên sụn mi trên 3 nốt chữ U bằng chỉ vicryl
                if (checkEdit85.Checked)
                {
                    eyeSurgDesc.KHAU_CO_DINH_CO_NANG_MI = 1;
                }
                else if (checkEdit83.Checked)
                {
                    eyeSurgDesc.KHAU_CO_DINH_CO_NANG_MI = 0;
                }
                else
                    eyeSurgDesc.KHAU_CO_DINH_CO_NANG_MI = null;
                //- KHAU_CO_DINH_CO_NANG_MI: Khau co dinh co nang mi vao 1/3 bo tren sun mi tren 3 not chu U bang chi vicryl. 0: Khong; 1: Co

                if (checkEdit86.Checked)
                {
                    eyeSurgDesc.LUON_CHI_HINH_NGU_GIAC = 0;
                }
                else if (checkEdit80.Checked)
                {
                    eyeSurgDesc.LUON_CHI_HINH_NGU_GIAC = 1;
                }
                else
                    eyeSurgDesc.LUON_CHI_HINH_NGU_GIAC = null;

                //- LUON_CHI_HINH_NGU_GIAC: Luon chi hoac silicon theo hinh ngu giac. 0: Khong; 1: Co

                eyeSurgDesc.KHAU_DA_MI_TAO_MI_CHI = textEdit24.Text;
                //- KHAU_DA_MI_TAO_MI_CHI: Chi dung de khau da mi va tแบกo mi bang chi

                eyeSurgDesc.TRA_MAT_THUOC = textEdit25.Text;
                //- TRA_MAT_THUOC:
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetDataPTGlocom()
        {
            try
            {
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();                

                //- CHAN_DOAN: 6: glocom goc dong, 7: glocom goc mo, 8: glocom bam sinh, 9: glocom thu phat
                if (Glocom__raGloocomGocDong.Checked)
                    eyeSurgDesc.CHAN_DOAN = 6;
                else if (Glocom__raGloocomGocMo.Checked)
                    eyeSurgDesc.CHAN_DOAN = 7;
                else if (Glocom__raGloocomBamSinh.Checked)
                    eyeSurgDesc.CHAN_DOAN = 8;
                else if (Glocom__raGloocomThuPhat.Checked)
                    eyeSurgDesc.CHAN_DOAN = 9;
                else
                    eyeSurgDesc.CHAN_DOAN = null;

                //- NGUYEN_NHAN:
                eyeSurgDesc.NGUYEN_NHAN = Glocom__txtDo.Text;

                //- GIAI_DOAN_BENH: 1: tiem tang, 2: so phat, 3: tien trien, 4: tram trong, 5: gan tuyet doi, 6: tuyet doi
                if (Glocom__raGiaDoanBenhTiemTang.Checked)
                    eyeSurgDesc.GIAI_DOAN_BENH = 1;
                else if (Glocom__raGiaDoanBenhSoPhat.Checked)
                    eyeSurgDesc.GIAI_DOAN_BENH = 2;
                else if (Glocom__raGiaDoanBenhTienTrien.Checked)
                    eyeSurgDesc.GIAI_DOAN_BENH = 3;
                else if (Glocom__raGiaDoanBenhTranTrong.Checked)
                    eyeSurgDesc.GIAI_DOAN_BENH = 4;
                else if (Glocom__raGiaDoanBenhGanTuyet.Checked)
                    eyeSurgDesc.GIAI_DOAN_BENH = 5;
                else if (Glocom__raGiaDoanBenhTuyetDoi.Checked)
                    eyeSurgDesc.GIAI_DOAN_BENH = 6;
                else
                    eyeSurgDesc.GIAI_DOAN_BENH = null;

                //- PP_PT_CAT_MONG_MAT: 0: khong, 1: co
                eyeSurgDesc.PP_PT_CAT_MONG_MAT = checkEdit9.Checked ? (short)1 : (short)0;

                //- PP_PHAU_THUAT: 5: cat be, 6: rach be, 7: mo be, 8: khac
                if (checkEdit1.Checked)
                    eyeSurgDesc.PP_PHAU_THUAT = 5;
                else if (checkEdit2.Checked)
                    eyeSurgDesc.PP_PHAU_THUAT = 6;
                else if (checkEdit7.Checked)
                    eyeSurgDesc.PP_PHAU_THUAT = 7;
                else if (checkEdit8.Checked)
                    eyeSurgDesc.PP_PHAU_THUAT = 8;
                else
                    eyeSurgDesc.PP_PHAU_THUAT = null;

                //- PP_VO_CAM: 1: me, 2: te tai mat, 3: duoi bao tenon, 4: Canh nhan cau
                if (Glocom__raPPVoCamGayMe.Checked)
                    eyeSurgDesc.PP_VO_CAM = 1;
                else if (Glocom__raPPVoCamTeTaiMat.Checked)
                    eyeSurgDesc.PP_VO_CAM = 2;
                else if (Glocom__raPPVoCamViTriBaoTenon.Checked)
                    eyeSurgDesc.PP_VO_CAM = 3;
                else if (Glocom__raPPVoCamCNC.Checked)
                    eyeSurgDesc.PP_VO_CAM = 4;
                else
                    eyeSurgDesc.PP_VO_CAM = null;
                //...........................
                //- THUOC_TE:
                eyeSurgDesc.THUOC_TE = Glocom__txtPPVoCamLoaiThuoc.Text;

                //- CO_DINH_NHAN_CAU: 1: Vanh mi, 2: Chi co truc, 3: chi giac mac
                if (Glocom__raCoDinhNhanCauVanhMi.Checked)
                    eyeSurgDesc.CO_DINH_NHAN_CAU = 1;
                else if (Glocom__raCoDinhNhanCauCoTruc.Checked)
                    eyeSurgDesc.CO_DINH_NHAN_CAU = 2;
                else if (Glocom__raCoDinhNhanCauChiGiacMac.Checked)
                    eyeSurgDesc.CO_DINH_NHAN_CAU = 3;
                else
                    eyeSurgDesc.CO_DINH_NHAN_CAU = null;
                //- TAO_VAT_KM_KINH_TUYEN:
                eyeSurgDesc.TAO_VAT_KM_KINH_TUYEN = Glocom__txtCoDinhNhanCauKinhTuyen.Text;

                //- TAO_VAT_KM_VI_TRI: 1: day cung do, 2: day vung ria
                if (Glocom__raCoDinhNhanCauDayCungDo.Checked)
                    eyeSurgDesc.TAO_VAT_KM_VI_TRI = 1;
                else if (Glocom__raCoDinhNhanCauDayVungRia.Checked)
                    eyeSurgDesc.CO_DINH_NHAN_CAU = 2;
                else
                    eyeSurgDesc.CO_DINH_NHAN_CAU = null;

                //- TINH_TRANG_BAO_TENON: 1: Binh thuong, 2: day, 3: xo
                if (Glocom__raTinhTrangBaoTenonBT.Checked)
                    eyeSurgDesc.TINH_TRANG_BAO_TENON = 1;
                else if (Glocom__raTinhTrangBaoTenonGay.Checked)
                    eyeSurgDesc.TINH_TRANG_BAO_TENON = 2;
                else if (Glocom__raTinhTrangBaoTenonSo.Checked)
                    eyeSurgDesc.TINH_TRANG_BAO_TENON = 3;
                else
                    eyeSurgDesc.TINH_TRANG_BAO_TENON = null;

                //- UC_CHE_TAO_XO: 0: Khong, 1: co
                //- UC_CHE_TAO_XO_TT_BS: 1: 5FU; 2: MMC; 3: Olcogen; 4: Màng ối; 5: Khac
                //- UC_CHE_TAO_XO_TT_BS_KHAC:
                //- UC_CHE_TAO_XO_VITRI: 1: Tren nap CM; 2: Duoi nap CM; 3: Ca hai;
                //- UC_CHE_TAO_XO_THOIGIAN:
                if (checkEdit43.Checked)
                    eyeSurgDesc.UC_CHE_TAO_XO = 1;
                else if (checkEdit44.Checked)
                    eyeSurgDesc.UC_CHE_TAO_XO = 0;
                else
                    eyeSurgDesc.UC_CHE_TAO_XO = null;

                if (checkEdit42.Checked)
                    eyeSurgDesc.UC_CHE_TAO_XO_TT_BS = 1;
                else if (checkEdit38.Checked)
                    eyeSurgDesc.UC_CHE_TAO_XO_TT_BS = 2;
                else if (checkEdit37.Checked)
                    eyeSurgDesc.UC_CHE_TAO_XO_TT_BS = 3;
                else if (checkEdit36.Checked)
                    eyeSurgDesc.UC_CHE_TAO_XO_TT_BS = 4;
                else if (checkEdit11.Checked)
                    eyeSurgDesc.UC_CHE_TAO_XO_TT_BS = 5;
                else
                    eyeSurgDesc.UC_CHE_TAO_XO_TT_BS = null;
                eyeSurgDesc.UC_CHE_TAO_XO_TT_BS_KHAC = textEdit4.Text;

                if (checkEdit31.Checked)
                    eyeSurgDesc.UC_CHE_TAO_XO_VITRI = 1;
                else if (checkEdit30.Checked)
                    eyeSurgDesc.UC_CHE_TAO_XO_VITRI = 2;
                else if (checkEdit29.Checked)
                    eyeSurgDesc.UC_CHE_TAO_XO_VITRI = 3;
                else
                    eyeSurgDesc.UC_CHE_TAO_XO_VITRI = null;
                eyeSurgDesc.UC_CHE_TAO_XO_THOIGIAN = textEdit16.Text;

                //- LANG_BOT_BAO_TENON: 0: Khong, 1: co
                if (checkEdit39.Checked)
                    eyeSurgDesc.LANG_BOT_BAO_TENON = 1;
                else
                    eyeSurgDesc.LANG_BOT_BAO_TENON = 0;
                eyeSurgDesc.VAT_CM_HINHDANG = textEdit14.Text;
                eyeSurgDesc.VAT_CM_KICHTHUOC = spinEdit15.EditValue != null ? (decimal?)spinEdit15.Value : null;

                //- CHOC_TP: 0: Khong; 1: Co
                if (checkEdit35.Checked)
                    eyeSurgDesc.CHOC_TP = 1;
                else
                    eyeSurgDesc.CHOC_TP = 0;

                //- CAT_MAU_BE: 1: Vi tri vung be; 2: Truoc be; 3: Trach be
                if (checkEdit33.Checked)
                    eyeSurgDesc.CAT_MAU_BE = 1;
                else if (checkEdit41.Checked)
                    eyeSurgDesc.CAT_MAU_BE = 2;
                else if (checkEdit34.Checked)
                    eyeSurgDesc.CAT_MAU_BE = 3;
                else
                    eyeSurgDesc.CAT_MAU_BE = null;

                //- CAT_MONG_MAT: 0: Khong; 1: Co
                if (checkEdit28.Checked)
                    eyeSurgDesc.CAT_MONG_MAT = 1;
                else if (checkEdit27.Checked)
                    eyeSurgDesc.CAT_MONG_MAT = 0;
                else
                    eyeSurgDesc.CAT_MONG_MAT = null;

                eyeSurgDesc.KHAU_NAP_CM_SO_MUI = spinEdit12.EditValue != null ? (decimal?)spinEdit12.Value : null;

                //- KHAU_NAP_CM_CHIRUT: 0: Khong; 1: Co
                if (checkEdit20.Checked)
                {
                    eyeSurgDesc.KHAU_NAP_CM_CHIRUT = 1;
                }
                else if (checkEdit21.Checked)
                {
                    eyeSurgDesc.KHAU_NAP_CM_CHIRUT = 0;
                }
                else
                {
                    eyeSurgDesc.KHAU_NAP_CM_CHIRUT = null;
                }
                eyeSurgDesc.KHAU_NAP_CM_LOAICHI = textEdit13.Text;

                //- TAI_TAO_TP: 1: Nuoc; 2: Hoi
                if (checkEdit26.Checked)
                    eyeSurgDesc.TAI_TAO_TP = 1;
                else if (checkEdit23.Checked)
                    eyeSurgDesc.TAI_TAO_TP = 2;
                else
                    eyeSurgDesc.TAI_TAO_TP = null;

                //- KHAU_KM: 1: Khau vat; 2: Khau mui roi
                if (checkEdit18.Checked)
                {
                    eyeSurgDesc.KHAU_KM = 1;
                }
                else if (checkEdit15.Checked)
                {
                    eyeSurgDesc.KHAU_KM = 2;
                }
                //else
                //{
                //    eyeSurgDesc.KHAU_KM = null;
                //}
                eyeSurgDesc.KHAU_KM_SOMUI = spinEdit11.EditValue != null ? (decimal?)spinEdit11.Value : null;

                //- KHAU_KM_LOAICHI: 1: Nylon, 2: Vicryl
                if (checkEdit24.Checked)
                {
                    eyeSurgDesc.KHAU_KM_LOAICHI = 1;
                }
                else if (checkEdit22.Checked)
                {
                    eyeSurgDesc.KHAU_KM_LOAICHI = 2;
                }
                eyeSurgDesc.KHAU_KM_LOAICHI_BS = textEdit10.Text;
                eyeSurgDesc.DIEN_BIEN_KHAC = textEdit9.Text;

                //- TIEM_MAT: 0: khong, 1: co
                //- TIEM_MAT_TT_BO_SUNG: 2: duoi km, 3: CNC
                if (checkEdit25.Checked)
                {
                    eyeSurgDesc.TIEM_MAT = 1;

                    checkEdit5.Enabled = checkEdit4.Enabled = textEdit8.Enabled = true;
                    if (checkEdit5.Checked)
                        eyeSurgDesc.TIEM_MAT_TT_BO_SUNG = 2;
                    else if (checkEdit4.Checked)
                        eyeSurgDesc.TIEM_MAT_TT_BO_SUNG = 3;
                    else
                        eyeSurgDesc.TIEM_MAT_TT_BO_SUNG = null;
                    eyeSurgDesc.TIEM_MAT_THUOC = textEdit8.Text;
                }
                else if (checkEdit13.Checked)
                {
                    eyeSurgDesc.TIEM_MAT = 0;
                    eyeSurgDesc.TIEM_MAT_TT_BO_SUNG = null;
                    eyeSurgDesc.TIEM_MAT_THUOC = "";
                }

                eyeSurgDesc.TRA_MAT_THUOC = textEdit6.Text;
                eyeSurgDesc.TRA_MAT_BANG_TT = textEdit2.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetDataPTDucThuyTinhThe()
        {
            try
            {
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //3: Phẫu thuật đục thủy tinh thể:
                if (raDucT3Gia.Checked)
                    eyeSurgDesc.CHAN_DOAN = 1;
                else if (raDucT3BenhLy.Checked)
                    eyeSurgDesc.CHAN_DOAN = 2;
                else if (raLechT3.Checked)
                    eyeSurgDesc.CHAN_DOAN = 3;
                else if (raDucT3BamSinh.Checked)
                    eyeSurgDesc.CHAN_DOAN = 4;
                else if (raKhongCoT3.Checked)
                    eyeSurgDesc.CHAN_DOAN = 5;
                else
                    eyeSurgDesc.CHAN_DOAN = null;
                //- chan_doan: 1: duc T3 gia, 2: duc T3 benh ly, 3: lech t3, 4: Duc t3 bam sinh, 5: khong co T3
                if (raMP.Checked)
                    eyeSurgDesc.MAT_PHAU_THUAT = 1;
                else if (raMT.Checked)
                    eyeSurgDesc.MAT_PHAU_THUAT = 2;
                else
                    eyeSurgDesc.MAT_PHAU_THUAT = null;
                //- chi_dinh_pt_mat: 1: mat phai, 2: mat trai
                if (raPhaco.Checked)
                    eyeSurgDesc.PP_PHAU_THUAT = 1;
                else if (raNgoaiBao.Checked)
                    eyeSurgDesc.PP_PHAU_THUAT = 2;
                else if (raTrongBao.Checked)
                    eyeSurgDesc.PP_PHAU_THUAT = 3;
                else if (raTreoCungMac.Checked)
                    eyeSurgDesc.PP_PHAU_THUAT = 4;
                else
                    eyeSurgDesc.PP_PHAU_THUAT = null;
                //- chi_dinh_pt_loai: 1: phaco, 2: ngoai bao, 3: trong bao, 4: treo cung mac
                if (raMe.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM = 1;
                }
                else if (raTeTaiMat.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM = 2;
                }
                else
                {
                    eyeSurgDesc.PP_VO_CAM = null;
                }
                eyeSurgDesc.THUOC_TE = txtLoaiThuocTe.Text;
                //- pp_vo_cam: 1: me, 2: te tai mat

                //- thuoc_te:
                if (raVanhMi.Checked)
                    eyeSurgDesc.CO_DINH_NHAN_CAU = 1;
                else if (raChiCoTruc.Checked)
                    eyeSurgDesc.CO_DINH_NHAN_CAU = 2;
                else
                    eyeSurgDesc.CO_DINH_NHAN_CAU = null;
                //- Co_dinh_nhan_cau: 1: Vanh mi, 2: Chi co truc

                if (raDoI.Checked)
                    eyeSurgDesc.TINH_TRANG_T3 = 1;
                else if (raDoII.Checked)
                    eyeSurgDesc.TINH_TRANG_T3 = 2;
                else if (raDoIII.Checked)
                    eyeSurgDesc.TINH_TRANG_T3 = 3;
                else if (raDoIV.Checked)
                    eyeSurgDesc.TINH_TRANG_T3 = 4;
                else if (raDoV.Checked)
                    eyeSurgDesc.TINH_TRANG_T3 = 5;
                else if (raLechTTT.Checked)
                    eyeSurgDesc.TINH_TRANG_T3 = 6;
                else
                    eyeSurgDesc.TINH_TRANG_T3 = null;
                //- Tinh_trang_t3: 1: do 1, 2: do II, 3: do III, 4: do IV, 5: do V, 6: lech TTT

                if (raMoKMRiaKhong.Checked)
                {
                    eyeSurgDesc.MO_KM_RIA = 1;
                    eyeSurgDesc.MO_KM_RIA_KINH_TUYEN = "";
                }
                else if (raMoKMRiaCo.Checked)
                {
                    eyeSurgDesc.MO_KM_RIA = 2;
                    eyeSurgDesc.MO_KM_RIA_KINH_TUYEN = txtMoKMRiaKinhTuyen.Text;
                }
                else
                {
                    eyeSurgDesc.MO_KM_RIA = null;
                    eyeSurgDesc.MO_KM_RIA_KINH_TUYEN = "";
                }
                //- Mo_km_ria: 1: khong, 2: co                                
                //- Mo_km_ria_kinh_tuyen: (gio)

                if (raMVTPGiacMac.Checked)
                    eyeSurgDesc.MO_VAO_TP = 1;
                else if (raMVTPRia.Checked)
                    eyeSurgDesc.MO_VAO_TP = 2;
                else if (raMVTPCungMac.Checked)
                    eyeSurgDesc.MO_VAO_TP = 3;
                else
                    eyeSurgDesc.MO_VAO_TP = null;
                //- Mo_vao_tp: 1: giac mac; 2: vung ria; 3: cung mac

                eyeSurgDesc.MO_VAO_TP_KINH_TUYEN = textEdit1.Text;
                //- Mo_vao_tp_kinh_tuyen: (gio)
                eyeSurgDesc.MO_VAO_TP_KICH_THUOC = txtkichthuoc.EditValue != null ? (decimal?)txtkichthuoc.EditValue : null;
                //- Mo_vao_tp_kich_thuoc: (mm)

                eyeSurgDesc.MO_VAO_TP_RACH_PHU = checkEdit12.Checked ? (short?)1 : (short?)0;
                //- Mo_vao_tp_rach_phu: 0: Khong, 1: co

                if (raNhuomBaoKhong.Checked)
                    eyeSurgDesc.NHUOM_BAO = 0;
                else if (raNhuomBaoCo.Checked)
                    eyeSurgDesc.NHUOM_BAO = 1;
                else
                    eyeSurgDesc.NHUOM_BAO = null;
                //- Nhuom_bao: 0: khong, 1: co

                if (raXeBaoTruocT3.Checked)
                    eyeSurgDesc.LOAI_MO_BAO = 1;
                else if (raXMoBaoHinhTemThu.Checked)
                    eyeSurgDesc.LOAI_MO_BAO = 2;
                else if (raCatBao.Checked)
                    eyeSurgDesc.LOAI_MO_BAO = 3;
                //- Loai_mo_bao: 1: Xe bao truoc T3, 2: Mo bao hinh tem thu, 3: Cat bao

                eyeSurgDesc.TACH_NHAN = checkEdit6.Checked ? (short?)1 : (short?)0;
                //- Tach_nhan: 0: khong, 1: co

                if (raXoayNhanKhoKhan.Checked)
                    eyeSurgDesc.XOAY_NHAN = 1;
                else if (raXoayNhanDeDang.Checked)
                    eyeSurgDesc.XOAY_NHAN = 2;
                else
                    eyeSurgDesc.XOAY_NHAN = null;
                //- Xoay_nhan: 1: kho khan, 2: de dang

                if (raDayNhanKhong.Checked)
                    eyeSurgDesc.DAY_NHAN = 0;
                else if (raDayNhanCo.Checked)
                    eyeSurgDesc.DAY_NHAN = 1;
                else
                    eyeSurgDesc.DAY_NHAN = null;
                //- Day_nhan: 0: khong, 1: co

                if (raDayNhanBangNuoc.Checked)
                    eyeSurgDesc.CACH_DAY_NHAN = 1;
                else if (raDayNhanBangChatNhay.Checked)
                    eyeSurgDesc.CACH_DAY_NHAN = 2;
                else
                    eyeSurgDesc.CACH_DAY_NHAN = null;
                //- Cach_day_nhan: 1: bang nuoc, 2: bang chat nhay

                eyeSurgDesc.TAN_NHAN_NANG_LUONG = txtNangLuong.EditValue != null ? (decimal?)txtNangLuong.EditValue : null;

                eyeSurgDesc.TAN_NHAN_LUC_HUT = txtLucHut.EditValue != null ? (decimal?)txtLucHut.EditValue : null;
                //- Tan_nhan_luc_hut: (mmHg)

                eyeSurgDesc.TAN_NHAN_TOC_DO_DC = textEdit7.Text;
                //- Tan_nhan_toc_do_dc:

                if (raHutChatT3IA.Checked)
                    eyeSurgDesc.HUT_CHAT_T3 = 1;
                else if (raHutChatT3Kim2Nong.Checked)
                    eyeSurgDesc.HUT_CHAT_T3 = 2;
                else
                    eyeSurgDesc.HUT_CHAT_T3 = null;
                //- Hut_chat_t3: 1: IA, 2: Kim hai nong

                if (raDatIOLMem.Checked)
                    eyeSurgDesc.DAT_IOL_LOAI = 1;
                else if (raDatIOLCung.Checked)
                    eyeSurgDesc.DAT_IOL_LOAI = 2;
                else
                    eyeSurgDesc.DAT_IOL_LOAI = null;
                //- Dat_iol_loai: 1: mem, 2: cung

                if (raDatIOLBangPince.Checked)
                    eyeSurgDesc.DAT_IOL_CACH_THUC = 1;
                else if (raDatIOLBangSung.Checked)
                    eyeSurgDesc.DAT_IOL_CACH_THUC = 2;
                else if (raDatIOLCoDinhCM.Checked)
                    eyeSurgDesc.DAT_IOL_CACH_THUC = 3;
                else
                    eyeSurgDesc.DAT_IOL_CACH_THUC = null;
                //- Dat_iol_cach_thuc: 1: bang pince, 2: bang sung, 3: co dinh CM

                if (raRachBaoSauKhong.Checked)
                {
                    eyeSurgDesc.RACH_BAO_SAU = 0;
                    txtViTriRachForraRachBaoSau.Enabled = spinKichThuocRachForraRachBaoSau.Enabled = false;
                    eyeSurgDesc.RACH_BAO_SAU_VI_TRI = "";
                    //- Rach_bao_sau_vi_tri:
                    eyeSurgDesc.RACH_BAO_SAU_KICH_THUOC = null;
                }
                else if (raRachBaoSauCo.Checked)
                {
                    eyeSurgDesc.RACH_BAO_SAU = 1;
                    eyeSurgDesc.RACH_BAO_SAU_VI_TRI = txtViTriRachForraRachBaoSau.Text;
                    //- Rach_bao_sau_vi_tri:
                    eyeSurgDesc.RACH_BAO_SAU_KICH_THUOC = spinKichThuocRachForraRachBaoSau.EditValue != null ? (decimal?)spinKichThuocRachForraRachBaoSau.Value : null;
                    //- Rach_bao_sau_kich_thuoc:
                }
                //- Rach_bao_sau: 0: khong; 1: co

                if (raRBSCBSKhong.Checked)
                {
                    eyeSurgDesc.CAT_BAO_SAU = 0;
                    eyeSurgDesc.CAT_BAO_SAU_CACH_THUC = 0;
                }
                    
                else if (raRBSCBSCo.Checked)
                    eyeSurgDesc.CAT_BAO_SAU = 1;
                else
                    eyeSurgDesc.CAT_BAO_SAU = null;
                //- Cat_bao_sau: 0: khong, 1: co

                if (raRBSCBSCatBangKeo.Checked)
                    eyeSurgDesc.CAT_BAO_SAU_CACH_THUC = 1;
                else if (raRBSCBSMayCatDK.Checked)
                    eyeSurgDesc.CAT_BAO_SAU_CACH_THUC = 2;
                else
                    eyeSurgDesc.CAT_BAO_SAU_CACH_THUC = 0;////////////////////////////// ==> mới sửa chỗ này
                //- Cat_bao_sau_cach_thuc: 1: bang keo, 2: may cat DK

                if (raRBSCMMNVKhong.Checked)
                {
                    eyeSurgDesc.CAT_MONG_MAT_NGOAI_VI = 1;
                    eyeSurgDesc.CAT_MONG_MAT_NGOAI_VI_VITRI = "";
                }
                else if (raRBSCMMNVCo.Checked)
                {
                    eyeSurgDesc.CAT_MONG_MAT_NGOAI_VI = 2;
                    eyeSurgDesc.CAT_MONG_MAT_NGOAI_VI_VITRI = txtViTriRachForraRBSCMMNV.Text;
                }
                else
                {
                    eyeSurgDesc.CAT_MONG_MAT_NGOAI_VI = null;
                    eyeSurgDesc.CAT_MONG_MAT_NGOAI_VI_VITRI = "";
                }
                //- Cat_mong_mat_ngoai_vi: 0: khong, 1: co                
                //- Cat_mong_mat_ngoai_vi_vitri:

                if (raPhucHoiVetMoBomPhu.Checked)
                    eyeSurgDesc.PHUC_HOI_VET_MO = 1;
                else if (raPhucHoiVetMoKhauVat.Checked)
                    eyeSurgDesc.PHUC_HOI_VET_MO = 2;
                else
                    eyeSurgDesc.PHUC_HOI_VET_MO = null;
                //- Phu_hoi_vet_mo: 1: bom phu, 2: khau vat

                if (raTiemMatKhong.Checked)
                    eyeSurgDesc.TIEM_MAT = 0;
                else if (raTiemMatCo.Checked)
                    eyeSurgDesc.TIEM_MAT = 1;
                else
                    eyeSurgDesc.TIEM_MAT = null;
                //- Tiem_mat: 0: khong, 1: co

                if (raTiemMatTienPhong.Checked)
                    eyeSurgDesc.TIEM_MAT_TT_BO_SUNG = 1;
                else if (raTiemMatDuoiKM.Checked)
                    eyeSurgDesc.TIEM_MAT_TT_BO_SUNG = 2;
                else if (raTiemMatCNC.Checked)
                    eyeSurgDesc.TIEM_MAT_TT_BO_SUNG = 3;
                else
                    eyeSurgDesc.TIEM_MAT_TT_BO_SUNG = null;
                //- Tiem_mat_tt_bo_sung: 1: tien phong, 2: duoi km, 3: CNC

                eyeSurgDesc.TIEM_MAT_THUOC = txtLoaiThuocForraTiemMat.Text;
                //- Tiem_mat_thuoc:

                if (raTraMatDD.Checked)
                    eyeSurgDesc.TRA_MAT = 1;
                else if (raTraMatMo.Checked)
                    eyeSurgDesc.TRA_MAT = 2;
                else
                    eyeSurgDesc.TRA_MAT = null;
                //- Tra_mat: 1: dung dich, 2: mong

                eyeSurgDesc.TRA_MAT_THUOC = txtLoaiThuocForraTraMat.Text;
                //- Tra_mat_thuoc:

                if (checkEdit10.Checked)
                    eyeSurgDesc.TRA_MAT_BANG_EP = 0;
                else if (checkEdit10.Checked)
                    eyeSurgDesc.TRA_MAT_BANG_EP = 1;
                else
                    eyeSurgDesc.TRA_MAT_BANG_EP = null;
                //- Tra_mat_bang_ep: 0: khong, 1: co
                eyeSurgDesc.DIEN_BIEN_KHAC = txtDienBienKhac.Text;
                //- Dien_bien_khac
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetDataTTMongMat()
        {
            try
            {
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //7. Thủ thuật mống mắt:
                if (checkEdit269.Checked)
                    eyeSurgDesc.CHAN_DOAN_NGHI_NGO_GLOCOM = 1;
                else if (checkEdit267.Checked)
                    eyeSurgDesc.CHAN_DOAN_NGHI_NGO_GLOCOM = 2;
                else
                    eyeSurgDesc.CHAN_DOAN_NGHI_NGO_GLOCOM = null;
                //- CHAN_DOAN_NGHI_NGO_GLOCOM: 1: Mat phai nghi ngo glocom, tien phong goc hep. 2: Mat trai nghi ngo glocom, tien phong goc hep.
                if (checkEdit266.Checked)
                    eyeSurgDesc.CHAN_DOAN_GLOCOM = 1;
                else if (checkEdit268.Checked)
                    eyeSurgDesc.CHAN_DOAN_GLOCOM = 2;
                else
                    eyeSurgDesc.CHAN_DOAN_GLOCOM = null;
                //- CHAN_DOAN_GLOCOM: 1: Mat phai glocom, tien phong goc hep. 2: Mat trai glocom, tien phong goc hep.
                if (checkEdit264.Checked)
                {
                    eyeSurgDesc.LASER_YAG_NANG_LUONG = 1;
                    eyeSurgDesc.LASER_YAG_NANG_LUONG_KHAC = "";
                }
                else if (checkEdit261.Checked)
                {
                    eyeSurgDesc.LASER_YAG_NANG_LUONG = 2;
                    eyeSurgDesc.LASER_YAG_NANG_LUONG_KHAC = "";
                }
                else if (!String.IsNullOrEmpty(textEdit64.Text))
                {
                    eyeSurgDesc.LASER_YAG_NANG_LUONG = 3;
                    eyeSurgDesc.LASER_YAG_NANG_LUONG_KHAC = textEdit64.Text;
                }
                else
                {
                    eyeSurgDesc.LASER_YAG_NANG_LUONG = null;
                    eyeSurgDesc.LASER_YAG_NANG_LUONG_KHAC = "";
                }
                //- LASER_YAG_NANG_LUONG: 1: 4mj; 2: 5mj; 3: Khac
                //- LASER_YAG_NANG_LUONG_KHAC:

                if (checkEdit258.Checked)
                    eyeSurgDesc.LASER_YAG_DIEM_NO = 1;
                else if (checkEdit257.Checked)
                    eyeSurgDesc.LASER_YAG_DIEM_NO = 2;
                else
                    eyeSurgDesc.LASER_YAG_DIEM_NO = null;
                //- LASER_YAG_DIEM_NO: 1: Truoc tieu diem; 2: Sau tieu diem

                if (checkEdit256.Checked)
                {
                    eyeSurgDesc.VI_TRI_CAT_MONG_CHU_BIEN = 11;
                }
                else if (checkEdit255.Checked)
                {
                    eyeSurgDesc.VI_TRI_CAT_MONG_CHU_BIEN = 13;
                }
                else if (!String.IsNullOrEmpty(spinEdit63.Text))
                {
                    eyeSurgDesc.VI_TRI_CAT_MONG_CHU_BIEN = Inventec.Common.TypeConvert.Parse.ToDecimal(spinEdit63.Text);
                }
                else
                {
                    eyeSurgDesc.VI_TRI_CAT_MONG_CHU_BIEN = null;
                }
                //- VI_TRI_CAT_MONG_CHU_BIEN: (cho điền số giờ)
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetDataTTMong()
        {
            try
            {
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //2. Phẫu thuật mộng:
                //- MAT_PHAU_THUAT: 1: mat phai, 2: mat trai
                if (checkEdit64.Checked)
                    eyeSurgDesc.MAT_PHAU_THUAT = 1;
                else if (checkEdit71.Checked)
                    eyeSurgDesc.MAT_PHAU_THUAT = 2;
                else
                    eyeSurgDesc.MAT_PHAU_THUAT = null;

                //- PP_VO_CAM: 1: me, 2: te tai mat
                if (checkEdit104.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM = 1;
                }
                else if (checkEdit103.Checked)
                {
                    eyeSurgDesc.PP_VO_CAM = 2;
                }
                else
                {
                    eyeSurgDesc.PP_VO_CAM = null;
                }
                eyeSurgDesc.THUOC_TE = textEdit33.Text;

                eyeSurgDesc.DAT_VANH_MI = checkEdit112.Checked ? (short?)1 : (short?)0;
                //- DAT_VANH_MI: 0: Khong; 1: co

                eyeSurgDesc.TIEM_LIDOCANIE_THAN_MONG = checkEdit108.Checked ? (short?)1 : (short?)0;
                //- TIEM_LIDOCANIE_THAN_MONG: 0: Khong; 1: Co

                eyeSurgDesc.CAT_DAU_MONG = checkEdit97.Checked ? (short?)1 : (short?)0;
                //- CAT_DAU_MONG: 0: Khong; 1: Co

                eyeSurgDesc.PHAN_TICH_THAN_MONG = checkEdit100.Checked ? (short?)1 : (short?)0;
                //- PHAN_TICH_THAN_MONG: 0: Khong; 1: Co

                eyeSurgDesc.DOT_CAM_MAU = checkEdit101.Checked ? (short?)1 : (short?)0;
                //- DOT_CAM_MAU: 0: Khong; 1: Co

                eyeSurgDesc.GOT_GIAC_MAC_DAU_MONG = checkEdit68.Checked ? (short?)1 : (short?)0;
                //- GOT_GIAC_MAC_DAU_MONG: 0: Khong; 1: Co
                if (checkEdit69.Checked)
                    eyeSurgDesc.LAY_MANH_KM_SAT_RIA = 1;
                else if (checkEdit70.Checked)
                    eyeSurgDesc.LAY_MANH_KM_SAT_RIA = 2;
                else
                    eyeSurgDesc.LAY_MANH_KM_SAT_RIA = null;
                //- LAY_MANH_KM_SAT_RIA: 1: Tren, 2: duoi

                eyeSurgDesc.LAY_MANH_KM_SAT_RIA_KT = txtEdit32.Text;
                //- LAY_MANH_KM_SAT_RIA_KT: (kich thuoc)

                eyeSurgDesc.LAY_MANH_MANG_OI = checkEdit106.Checked ? (short?)1 : (short?)0;
                //- LAY_MANH_MANG_OI: 0: Khong; 1: co

                if (checkEdit106.Checked)
                {
                    eyeSurgDesc.LAY_MANH_MANG_OI_KT = txtEdit21.Text;
                }
                else
                {
                    eyeSurgDesc.LAY_MANH_MANG_OI_KT = null;
                }
                //- LAY_MANH_MANG_OI_KT: (Kich thuoc)

                if (checkEdit115.Checked)
                    eyeSurgDesc.KHAU_MANH_GHEP_CHI = 1;
                else if (checkEdit96.Checked)
                    eyeSurgDesc.KHAU_MANH_GHEP_CHI = 2;
                else
                    eyeSurgDesc.KHAU_MANH_GHEP_CHI = null;
                //- KHAU_MANH_GHEP_CHI: 1: Nylon 10-O; 2: Vycryl 7-O

                eyeSurgDesc.KHAU_MANH_GHEP_CHI_SO_MUI = spinEdit19.EditValue != null ? (decimal?)spinEdit19.Value : null;
                //- KHAU_MANH_GHEP_CHI_SO_MUI:

                eyeSurgDesc.KHAU_KM_CHE_PHAN_CAT_KM = checkEdit72.Checked ? (short?)1 : (short?)0;
                //- KHAU_KM_CHE_PHAN_CAT_KM: 0: Khong; 1: co

                if (checkEdit72.Checked)
                {
                    eyeSurgDesc.KHAU_KM_CHE_PHAN_CAT_KM_SO_MUI = spinEdit31.EditValue != null ? (decimal?)spinEdit31.Value : null;
                }
                else
                {
                    eyeSurgDesc.KHAU_KM_CHE_PHAN_CAT_KM_SO_MUI = null;
                }
                //- KHAU_KM_CHE_PHAN_CAT_KM_SO_MUI:
                if (checkEdit107.Checked)
                    eyeSurgDesc.BIEN_CHUNG = 1;
                else if (checkEdit113.Checked)
                    eyeSurgDesc.BIEN_CHUNG = 2;
                else if (checkEdit109.Checked)
                    eyeSurgDesc.BIEN_CHUNG = 3;
                else
                    eyeSurgDesc.BIEN_CHUNG = null;
                //- BIEN_CHUNG: 1: Thung cung mac; 2: Giac mac; 3: Cat vao co truc

                eyeSurgDesc.XU_TRI_BIEN_CHUNG = textEdit29.Text;
                //- XU_TRI_BIEN_CHUNG:

                eyeSurgDesc.DIEN_BIEN_KHAC = textEdit30.Text;

                if (checkEdit110.Checked)
                    eyeSurgDesc.TRA_MAT = 1;
                else if (checkEdit3.Checked)
                    eyeSurgDesc.TRA_MAT = 2;
                else
                    eyeSurgDesc.TRA_MAT = null;
                //- TRA_MAT: 1: dung dich, 2: mong

                eyeSurgDesc.TRA_MAT_THUOC = textEdit27.Text;
                //- TRA_MAT_THUOC:

                eyeSurgDesc.TRA_MAT_BANG_EP = checkEdit93.Checked ? (short?)1 : (short?)0;
                //- TRA_MAT_BANG_EP: 0: khong, 1: co
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetDataTTLaser()
        {
            try
            {
                //dynamic eyeSurgDesc = new System.Dynamic.ExpandoObject();
                //- CD_DUC_BAO_SAU_SAU_MO_TTT: Chan doan duc sau mo TTT. 1: Mat phai duc bao sau sau mo PTTT; 2: Mat trai duc bao sau sau mo PTTT
                //- LASER_YAG_NANG_LUONG: 4: 1.5mj; 5: 2mj; 3: Khac
                //- LASER_YAG_NANG_LUONG_KHAC:
                //- LASER_YAG_DIEM_NO: 1: Truoc tieu diem; 2: Sau tieu diem
                //- HINH_DANG_MO_BAO_SAU: 1: Hinh tron; 2: Hinh chu thap

                //6. Thủ thuật laser yag:
                if (checkEdit333.Checked)
                    eyeSurgDesc.CD_DUC_BAO_SAU_SAU_MO_TTT = 1;
                else if (checkEdit329.Checked)
                    eyeSurgDesc.CD_DUC_BAO_SAU_SAU_MO_TTT = 2;
                else
                    eyeSurgDesc.CD_DUC_BAO_SAU_SAU_MO_TTT = null;
                //- CD_DUC_BAO_SAU_SAU_MO_TTT: Chan doan duc sau mo TTT. 1: Mat phai duc bao sau sau mo PTTT; 2: Mat trai duc bao sau sau mo PTTT

                if (checkEdit328.Checked)
                {
                    eyeSurgDesc.LASER_YAG_NANG_LUONG = 4;
                    eyeSurgDesc.LASER_YAG_NANG_LUONG_KHAC = "";
                }
                else if (checkEdit325.Checked)
                {
                    eyeSurgDesc.LASER_YAG_NANG_LUONG = 5;
                    eyeSurgDesc.LASER_YAG_NANG_LUONG_KHAC = "";
                }
                else if (!String.IsNullOrEmpty(textEdit75.Text))
                {
                    eyeSurgDesc.LASER_YAG_NANG_LUONG = 3;
                    eyeSurgDesc.LASER_YAG_NANG_LUONG_KHAC = textEdit75.Text;
                }
                else
                {
                    eyeSurgDesc.LASER_YAG_NANG_LUONG = null;
                    eyeSurgDesc.LASER_YAG_NANG_LUONG_KHAC = "";
                }
                //- LASER_YAG_NANG_LUONG: 4: 1.5mj; 5: 2mj; 3: Khac
                //- LASER_YAG_NANG_LUONG_KHAC:

                if (checkEdit322.Checked)
                    eyeSurgDesc.LASER_YAG_DIEM_NO = 1;
                else if (checkEdit321.Checked)
                    eyeSurgDesc.LASER_YAG_DIEM_NO = 2;
                else
                    eyeSurgDesc.LASER_YAG_DIEM_NO = null;
                //- LASER_YAG_DIEM_NO: 1: Truoc tieu diem; 2: Sau tieu diem

                if (checkEdit320.Checked)
                    eyeSurgDesc.HINH_DANG_MO_BAO_SAU = 1;
                else if (checkEdit319.Checked)
                    eyeSurgDesc.HINH_DANG_MO_BAO_SAU = 2;
                else
                    eyeSurgDesc.HINH_DANG_MO_BAO_SAU = null;
                //- HINH_DANG_MO_BAO_SAU: 1: Hinh tron; 2: Hinh chu thap
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raPTMong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raPTMong.Checked)
                {
                    xtraTabControl1.SelectedTabPage = tabPagePTMong;
                    if (!isFirstLoad)
                        eyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                    LoadTTMong();
                    //checkEdit64.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raPTDucThuyTInhThe_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raPTDucThuyTInhThe.Checked)
                {
                    xtraTabControl1.SelectedTabPage = tabPagePTDucTTT;
                    if (!isFirstLoad)
                        eyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                    LoadPTDucThuyTinhThe();
                    //raDucT3Gia.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raPTGlocom_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raPTGlocom.Checked)
                {
                    xtraTabControl1.SelectedTabPage = tabPageGlocom;
                    if (!isFirstLoad)
                        eyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                    LoadPTGlocom();
                    //Glocom__raGloocomGocDong.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raPTTaiTaoLeQuan_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raPTTaiTaoLeQuan.Checked)
                {
                    xtraTabControl1.SelectedTabPage = TabPageTaiTaoLeQuan;
                    if (!isFirstLoad)
                        eyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                    LoadPTTaiTaoLeQuan();
                    //checkEdit205.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raPTSupMi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raPTSupMi.Checked)
                {
                    xtraTabControl1.SelectedTabPage = TabPagePTSupMi;
                    if (!isFirstLoad)
                        eyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                    LoadPTSupMi();
                    //checkEdit141.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTTLaser_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTTLaser.Checked)
                {
                    xtraTabControl1.SelectedTabPage = TabPageTTTLaser;
                    if (!isFirstLoad)
                        eyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                    LoadTTLaser();
                    //checkEdit333.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTTMongMat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTTMongMat.Checked)
                {
                    xtraTabControl1.SelectedTabPage = TabPageTTMongMat;
                    if (!isFirstLoad)
                        eyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                    LoadTTMongMat();
                    //checkEdit269.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void textEdit64_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void textEdit63_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(spinEdit63.Text))
                {
                    checkEdit256.Checked = checkEdit255.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (xtraTabAddInfo.SelectedTabPage == xtraTabEye)
                {
                    HIS_EYE_SURGRY_DESC eyeSurgDescTmp = new HIS_EYE_SURGRY_DESC();
                    if (eyeSurgDesc.ID > 0)
                    {
                        eyeSurgDescTmp.ID = eyeSurgDesc.ID;
                        eyeSurgDescTmp.CREATE_TIME = eyeSurgDesc.CREATE_TIME;
                        eyeSurgDescTmp.CREATOR = eyeSurgDesc.CREATOR;
                        eyeSurgDescTmp.MODIFIER = eyeSurgDesc.MODIFIER;
                        eyeSurgDescTmp.MODIFY_TIME = eyeSurgDesc.MODIFY_TIME;
                        eyeSurgDescTmp.GROUP_CODE = eyeSurgDesc.GROUP_CODE;
                        eyeSurgDescTmp.APP_CREATOR = eyeSurgDesc.APP_CREATOR;
                        eyeSurgDescTmp.APP_MODIFIER = eyeSurgDesc.APP_MODIFIER;
                        eyeSurgDescTmp.IS_ACTIVE = eyeSurgDesc.IS_ACTIVE;
                        eyeSurgDescTmp.IS_DELETE = eyeSurgDesc.IS_DELETE;
                        eyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                        eyeSurgDesc.ID = eyeSurgDescTmp.ID;
                        eyeSurgDesc.CREATE_TIME = eyeSurgDescTmp.CREATE_TIME;
                        eyeSurgDesc.CREATOR = eyeSurgDescTmp.CREATOR;
                        eyeSurgDesc.MODIFIER = eyeSurgDescTmp.MODIFIER;
                        eyeSurgDesc.MODIFY_TIME = eyeSurgDescTmp.MODIFY_TIME;
                        eyeSurgDesc.GROUP_CODE = eyeSurgDescTmp.GROUP_CODE;
                        eyeSurgDesc.APP_CREATOR = eyeSurgDescTmp.APP_CREATOR;
                        eyeSurgDesc.APP_MODIFIER = eyeSurgDescTmp.APP_MODIFIER;
                        eyeSurgDesc.IS_ACTIVE = eyeSurgDescTmp.IS_ACTIVE;
                        eyeSurgDesc.IS_DELETE = eyeSurgDescTmp.IS_DELETE;
                    }
                    else
                        eyeSurgDesc = new HIS_EYE_SURGRY_DESC();

                    if (raPTGlocom.Checked)
                    {
                        eyeSurgDesc.LOAI_PT_MAT = IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__GLOCOM;
                        GetDataPTGlocom();
                    }
                    else if (raPTMong.Checked)
                    {
                        eyeSurgDesc.LOAI_PT_MAT = IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_MONG;
                        GetDataTTMong();
                    }
                    else if (raPTDucThuyTInhThe.Checked)
                    {
                        eyeSurgDesc.LOAI_PT_MAT = IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_TTT;
                        GetDataPTDucThuyTinhThe();
                    }
                    else if (raPTTaiTaoLeQuan.Checked)
                    {
                        eyeSurgDesc.LOAI_PT_MAT = IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_TAI_TAO_LE_QUAN;
                        GetDataPTTaiTaoLeQuan();
                    }
                    else if (raPTSupMi.Checked)
                    {
                        eyeSurgDesc.LOAI_PT_MAT = IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__PT_SUP_MI;
                        GetDataPTSupMi();
                    }
                    else if (raTTLaser.Checked)
                    {
                        eyeSurgDesc.LOAI_PT_MAT = IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__TT_LASER_YAG;
                        GetDataTTLaser();
                    }
                    else if (raTTMongMat.Checked)
                    {
                        eyeSurgDesc.LOAI_PT_MAT = IMSys.DbConfig.HIS_RS.HIS_EYE_SURGRY_DESC.LOAI_PT_MAT__TT_MONG_MAT;
                        GetDataTTMongMat();
                    }

                    if (this.GetEyeSurgryDescLast != null)
                        this.GetEyeSurgryDescLast(eyeSurgDesc);
                    this.Close();
                }
                else if (xtraTabAddInfo.SelectedTabPage == xtraTabSkin)
                {
                    GetDataTTDalieu();

                    if (this.ActionSkinSurgeryDes != null)
                        this.ActionSkinSurgeryDes(SkinSurgeryDes);
                    this.Close();
                }
                else if (xtraTabAddInfo.SelectedTabPage == xtraTabOther)
                {
                    GetDataSereServPTTT_Other();

                    if (this.ActionSereServPTTT != null && this.SereServPTTT != null)
                        this.ActionSereServPTTT(this.SereServPTTT);
                    this.Close();
                }
                else if (xtraTabAddInfo.SelectedTabPage == xtraTabPage1)
                {
                    GetDataDmv();

                    if (this.ActionSereServPTTT != null && this.SereServPTTT != null)
                        this.ActionSereServPTTT(this.SereServPTTT);
                    if (this.ActionStentConclude != null && this.ActionStentConclude != null)
                        this.ActionStentConclude(this.StenConclude);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataDmv()
        {
            try
            {
                if (this.SereServPTTT == null)
                    this.SereServPTTT = new V_HIS_SERE_SERV_PTTT();
                this.StenConclude = new List<HIS_STENT_CONCLUDE>();
                this.SereServPTTT.PCI = txtPci.Text.Trim();
                this.SereServPTTT.STENTING = txtStenting.Text.Trim();
                this.SereServPTTT.LOCATION_INTERVENTION = txtLocation.Text.Trim();
                this.SereServPTTT.REASON_INTERVENTION = txtReason.Text.Trim();
                this.SereServPTTT.INTRODUCER = txtIntroducer.Text.Trim();
                this.SereServPTTT.GUIDING_CATHETER = txtGuidingCatheter.Text.Trim();
                this.SereServPTTT.GUITE_WIRE = txtGuiteWire.Text.Trim();
                this.SereServPTTT.BALLON = txtBallon.Text.Trim();
                this.SereServPTTT.STENT = txtStent.Text.Trim();
                this.SereServPTTT.CONTRAST_AGENT = txtContrastAgent.Text.Trim();
                this.SereServPTTT.INSTRUMENTS_OTHER = txtInstrumentsOther.Text.Trim();
                this.SereServPTTT.STENT_NOTE = memStentNote.Text.Trim();
                foreach (var o in lstDataDmv)
                {
                    if (string.IsNullOrEmpty(o.SURGERY_NAME)
                       && string.IsNullOrEmpty(o.DEVICE_NAME)
                       && string.IsNullOrEmpty(o.BALLON_PRESSURE_TIME)
                       && o.RESULT_BEFORE == null
                       && o.RESULT_AFTER == null)
                        continue;
                    HIS_STENT_CONCLUDE data = new HIS_STENT_CONCLUDE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_STENT_CONCLUDE>(data, o);
                    data.SERE_SERV_ID = sereServ.ID;
                    StenConclude.Add(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataSereServPTTT_Other()
        {
            try
            {
                if (this.SereServPTTT == null)
                    this.SereServPTTT = new V_HIS_SERE_SERV_PTTT();
                this.SereServPTTT.WICK = txtWick.Text;
                this.SereServPTTT.DRAINAGE = txtDrainage.Text;
                if (dtDrawDate.EditValue != null && dtDrawDate.DateTime != DateTime.MinValue)
                {
                    this.SereServPTTT.DRAW_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDrawDate.DateTime);
                }
                if (dtSewingDate.EditValue != null && dtSewingDate.DateTime != DateTime.MinValue)
                {
                    this.SereServPTTT.CUT_SEWING_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtSewingDate.DateTime);
                }
                this.SereServPTTT.OTHER = txtOther.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataTTDalieu()
        {
            try
            {
                if (SkinSurgeryDes == null)
                {
                    SkinSurgeryDes = new SkinSurgeryDesADO();
                }

                if (chkPostureUp.Checked)
                    SkinSurgeryDes.SURGERY_POSITION_ID = 1;
                else if (chkPostureSide.Checked)
                    SkinSurgeryDes.SURGERY_POSITION_ID = 2;
                else if (chkPostureTummy.Checked)
                    SkinSurgeryDes.SURGERY_POSITION_ID = 3;
                else
                    SkinSurgeryDes.SURGERY_POSITION_ID = null;

                SkinSurgeryDes.DAMAGE = txtSkinDamage.Text;
                SkinSurgeryDes.DAMAGE_POSITION = txtSkinDamagePosition.Text;

                if (spSkinDamageAmount.EditValue != null && spSkinDamageAmount.Value > 0)
                    SkinSurgeryDes.DAMAGE_AMOUNT = (long)spSkinDamageAmount.Value;
                else
                    SkinSurgeryDes.DAMAGE_AMOUNT = null;

                if (chkDamageTypeAll.Checked)
                    SkinSurgeryDes.DAMAGE_TREAT_CUTTING_TYPE = 1;
                else if (chkDamageTypePart.Checked)
                    SkinSurgeryDes.DAMAGE_TREAT_CUTTING_TYPE = 2;
                else
                    SkinSurgeryDes.DAMAGE_TREAT_CUTTING_TYPE = null;

                if (chkShapingSkin.Checked)
                    SkinSurgeryDes.DAMAGE_TREAT_SHAPING_TYPE = 1;
                else if (chkShapingSkinOther.Checked)
                    SkinSurgeryDes.DAMAGE_TREAT_SHAPING_TYPE = 2;
                else
                    SkinSurgeryDes.DAMAGE_TREAT_SHAPING_TYPE = null;

                if (chkCloseSkin.Checked)
                    SkinSurgeryDes.DAMAGE_TREAT_CLOSING_TYPE = 1;
                else if (chkDiscloseSkin.Checked)
                    SkinSurgeryDes.DAMAGE_TREAT_CLOSING_TYPE = 2;
                else
                    SkinSurgeryDes.DAMAGE_TREAT_CLOSING_TYPE = null;

                SkinSurgeryDes.DAMAGE_TREAT_OTHER = txtTreatSkinOther.Text;

                SkinSurgeryDes.HasValue = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit141_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit141.Checked == false)
                    checkEdit137.Checked = checkEdit140.Checked = checkEdit138.Checked = checkEdit139.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit136_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit136.Checked == false)
                    checkEdit133.Checked = checkEdit132.Checked = checkEdit135.Checked = checkEdit131.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (xtraTabAddInfo.SelectedTabPage == xtraTabEye)
                {
                    eyeSurgDesc = new HIS_EYE_SURGRY_DESC();
                    if (this.GetEyeSurgryDescLast != null)
                        this.GetEyeSurgryDescLast(eyeSurgDesc);
                    this.Close();
                }
                else if (xtraTabAddInfo.SelectedTabPage == xtraTabSkin)
                {
                    SkinSurgeryDes = new SkinSurgeryDesADO();
                    if (this.ActionSkinSurgeryDes != null)
                        this.ActionSkinSurgeryDes(SkinSurgeryDes);
                    this.Close();
                }
                else if (xtraTabAddInfo.SelectedTabPage == xtraTabOther)
                {
                    if (this.SereServPTTT == null)
                        this.SereServPTTT = new V_HIS_SERE_SERV_PTTT();
                    this.SereServPTTT.DRAINAGE = null;
                    this.SereServPTTT.WICK = null;
                    this.SereServPTTT.OTHER = null;
                    this.SereServPTTT.DRAW_DATE = null;
                    this.SereServPTTT.CUT_SEWING_DATE = null;
                    if (this.ActionSereServPTTT != null)
                        this.ActionSereServPTTT(this.SereServPTTT);
                    this.Close();
                }
                else if (xtraTabAddInfo.SelectedTabPage == xtraTabPage1)
                {
                    if (this.SereServPTTT == null)
                        this.SereServPTTT = new V_HIS_SERE_SERV_PTTT();
                    if (this.StenConclude == null)
                        this.StenConclude = new List<HIS_STENT_CONCLUDE>();
                    this.SereServPTTT.PCI = null;
                    this.SereServPTTT.STENTING = null;
                    this.SereServPTTT.LOCATION_INTERVENTION = null;
                    this.SereServPTTT.REASON_INTERVENTION = null;
                    this.SereServPTTT.INTRODUCER = null;
                    this.SereServPTTT.GUIDING_CATHETER = null;
                    this.SereServPTTT.GUITE_WIRE = null;
                    this.SereServPTTT.BALLON = null;
                    this.SereServPTTT.STENT = null;
                    this.SereServPTTT.CONTRAST_AGENT = null;
                    this.SereServPTTT.INSTRUMENTS_OTHER = null;
                    this.SereServPTTT.STENT_NOTE = null;
                    if (this.ActionSereServPTTT != null && this.SereServPTTT != null)
                        this.ActionSereServPTTT(this.SereServPTTT);
                    if (this.ActionStentConclude != null && this.ActionStentConclude != null)
                        this.ActionStentConclude(null);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit106_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtEdit21.Enabled = checkEdit106.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raMoKMRiaCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtMoKMRiaKinhTuyen.Enabled = raMoKMRiaCo.Checked;
                if (raMoKMRiaCo.Checked)
                {
                    raMoKMRiaKhong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raRachBaoSauCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtViTriRachForraRachBaoSau.Enabled = spinKichThuocRachForraRachBaoSau.Enabled = raRachBaoSauCo.Checked;

                if (raRachBaoSauCo.Checked)
                {
                    raRachBaoSauKhong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raRBSCMMNVCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtViTriRachForraRBSCMMNV.Enabled = raRBSCMMNVCo.Checked;

                if (raRBSCMMNVCo.Checked)
                {
                    raRBSCMMNVKhong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void textEdit75_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void spinEdit63_TextChanged(object sender, EventArgs e)
        {
            try
            {
                checkEdit256.Checked = checkEdit255.Checked = String.IsNullOrEmpty(spinEdit63.Text);
                if (String.IsNullOrEmpty(spinEdit63.Text))
                {
                    checkEdit255.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SpinKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.') && (e.KeyChar != ','))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinEdit63_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void textEdit64_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(textEdit64.Text))
                {
                    checkEdit264.Checked = checkEdit261.Checked = false;
                }
                if (String.IsNullOrEmpty(textEdit64.Text))
                {
                    checkEdit264.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void textEdit75_TextChanged(object sender, EventArgs e)
        {
            try
            {
                checkEdit328.Checked = checkEdit325.Checked = String.IsNullOrEmpty(textEdit75.Text);
                if (String.IsNullOrEmpty(textEdit75.Text))
                {
                    checkEdit328.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit25_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                checkEdit5.Enabled = checkEdit4.Enabled = textEdit8.Enabled = checkEdit25.Checked;

                if (checkEdit25.Checked)
                {
                    checkEdit13.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DefaultChosen()
        {
            checkEdit200.Checked = true;
            checkEdit193.Checked = true;
            checkEdit181.Checked = true;
            checkEdit182.Checked = true;
            checkEdit188.Checked = true;
            checkEdit174.Checked = true;
        }

        private void TextDefault()
        {
            textEdit49.Text = "Lidocain";
            textEdit50.Text = "6/0";
            textEdit44.Text = "8/0";
            textEdit41.Text = "Pd.Oflovid";
        }

        private void DefaultChosenGlocom()
        {
            Glocom__raGloocomGocDong.Checked = true;
            Glocom__raGiaDoanBenhTienTrien.Checked = true;
            checkEdit1.Checked = true;
            Glocom__raPPVoCamCNC.Checked = true;
            Glocom__raCoDinhNhanCauCoTruc.Checked = true;
            Glocom__raCoDinhNhanCauDayVungRia.Checked = true;
            Glocom__raTinhTrangBaoTenonSo.Checked = true;
            checkEdit44.Checked = true;
            checkEdit31.Checked = true;
            checkEdit33.Checked = true;
            checkEdit28.Checked = true;
            checkEdit21.Checked = true;
            checkEdit23.Checked = true;
            checkEdit15.Checked = true;
            checkEdit24.Checked = true;
            checkEdit25.Checked = true;
            checkEdit4.Checked = true;
        }

        private void TextDefaultGlocom()
        {
            Glocom__txtPPVoCamLoaiThuoc.Text = "Lidocain";
            Glocom__txtCoDinhNhanCauKinhTuyen.Text = "12";
            textEdit14.Text = "Chữ nhật";
            textEdit3.Text = "4 x 6";
            spinEdit12.Text = "3";
            textEdit13.Text = "Nylon iop";
            spinEdit11.Text = "5";
            textEdit8.Text = "Dexamethasome";
            textEdit6.Text = "Pd.Oflovid";
            textEdit2.Text = "Mắt";
        }

        private void DefaultChosenDucTTT()
        {
            raDoIII.Checked = true;
            raMVTPGiacMac.Checked = true;
            raDatIOLBangSung.Checked = true;
            raTraMatMo.Checked = true;
        }

        private void TextDefaultDucTTT()
        {
            txtLoaiThuocTe.Text = "Lidocain";
            textEdit1.Text = "9";
            txtLoaiThuocForraTraMat.Text = "Pd.Oflovid";
        }

        private void TextDefaultPTSupMi()
        {
            textEdit39.Text = "Lidocain";
            textEdit38.Text = "Lidocain";
            textEdit28.Text = "6/0";
            textEdit24.Text = "Nylon 7/0";
            textEdit25.Text = "Pd.Oflovid";
            checkEdit129.Checked = true;
        }

        private void chkDamageTypeAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDamageTypeAll.Checked)
                {
                    chkDamageTypePart.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDamageTypePart_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDamageTypePart.Checked)
                {
                    chkDamageTypeAll.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkShapingSkin_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkShapingSkin.Checked)
                {
                    chkShapingSkinOther.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkShapingSkinOther_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkShapingSkinOther.Checked)
                {
                    chkShapingSkin.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCloseSkin_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCloseSkin.Checked)
                {
                    chkDiscloseSkin.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDiscloseSkin_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDiscloseSkin.Checked)
                {
                    chkCloseSkin.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPostureUp_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPostureUp.Checked)
                {
                    chkPostureSide.Checked = false;
                    chkPostureTummy.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPostureSide_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPostureSide.Checked)
                {
                    chkPostureUp.Checked = false;
                    chkPostureTummy.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPostureTummy_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPostureTummy.Checked)
                {
                    chkPostureSide.Checked = false;
                    chkPostureUp.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Đục thủy tinh thể
        private void raTiemMatTienPhong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTiemMatTienPhong.Checked)
                {
                    raTiemMatDuoiKM.Checked = false;
                    raTiemMatCNC.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTiemMatDuoiKM_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTiemMatDuoiKM.Checked)
                {
                    raTiemMatTienPhong.Checked = false;
                    raTiemMatCNC.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTiemMatCNC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTiemMatCNC.Checked)
                {
                    raTiemMatDuoiKM.Checked = false;
                    raTiemMatTienPhong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDucT3Gia_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDucT3Gia.Checked)
                {
                    raDucT3BenhLy.Checked = false;
                    raLechT3.Checked = false;
                    raDucT3BamSinh.Checked = false;
                    raKhongCoT3.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDucT3BenhLy_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDucT3BenhLy.Checked)
                {
                    raDucT3Gia.Checked = false;
                    raLechT3.Checked = false;
                    raDucT3BamSinh.Checked = false;
                    raKhongCoT3.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raLechT3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raLechT3.Checked)
                {
                    raDucT3Gia.Checked = false;
                    raDucT3BenhLy.Checked = false;
                    raDucT3BamSinh.Checked = false;
                    raKhongCoT3.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDucT3BamSinh_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDucT3BamSinh.Checked)
                {
                    raDucT3Gia.Checked = false;
                    raDucT3BenhLy.Checked = false;
                    raLechT3.Checked = false;
                    raKhongCoT3.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raKhongCoT3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raKhongCoT3.Checked)
                {
                    raDucT3Gia.Checked = false;
                    raDucT3BenhLy.Checked = false;
                    raLechT3.Checked = false;
                    raDucT3BamSinh.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raMP_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raMP.Checked)
                {
                    raMT.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raMT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raMT.Checked)
                {
                    raMP.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raPhaco_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raPhaco.Checked)
                {
                    raNgoaiBao.Checked = false;
                    raTrongBao.Checked = false;
                    raTreoCungMac.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raNgoaiBao_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raNgoaiBao.Checked)
                {
                    raPhaco.Checked = false;
                    raTrongBao.Checked = false;
                    raTreoCungMac.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTrongBao_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTrongBao.Checked)
                {
                    raPhaco.Checked = false;
                    raNgoaiBao.Checked = false;
                    raTreoCungMac.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTreoCungMac_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTreoCungMac.Checked)
                {
                    raPhaco.Checked = false;
                    raNgoaiBao.Checked = false;
                    raTrongBao.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raMe_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raMe.Checked)
                {
                    raTeTaiMat.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTeTaiMat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTeTaiMat.Checked)
                {
                    raMe.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raVanhMi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raVanhMi.Checked)
                {
                    raChiCoTruc.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raChiCoTruc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raChiCoTruc.Checked)
                {
                    raVanhMi.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDoI_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDoI.Checked)
                {
                    raDoII.Checked = false;
                    raDoIII.Checked = false;
                    raDoIV.Checked = false;
                    raDoV.Checked = false;
                    raLechTTT.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDoII_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDoII.Checked)
                {
                    raDoI.Checked = false;
                    raDoIII.Checked = false;
                    raDoIV.Checked = false;
                    raDoV.Checked = false;
                    raLechTTT.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDoIII_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDoIII.Checked)
                {
                    raDoI.Checked = false;
                    raDoII.Checked = false;
                    raDoIV.Checked = false;
                    raDoV.Checked = false;
                    raLechTTT.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDoIV_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDoIV.Checked)
                {
                    raDoII.Checked = false;
                    raDoI.Checked = false;
                    raDoIII.Checked = false;
                    raDoV.Checked = false;
                    raLechTTT.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDoV_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDoV.Checked)
                {
                    raDoI.Checked = false;
                    raDoII.Checked = false;
                    raDoIII.Checked = false;
                    raDoIV.Checked = false;
                    raLechTTT.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raLechTTT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raLechTTT.Checked)
                {
                    raDoII.Checked = false;
                    raDoIII.Checked = false;
                    raDoIV.Checked = false;
                    raDoV.Checked = false;
                    raDoI.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raMoKMRiaKhong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raMoKMRiaKhong.Checked)
                {
                    raMoKMRiaCo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raMVTPGiacMac_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raMVTPGiacMac.Checked)
                {
                    raMVTPRia.Checked = false;
                    raMVTPCungMac.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raMVTPRia_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raMVTPRia.Checked)
                {
                    raMVTPGiacMac.Checked = false;
                    raMVTPCungMac.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raMVTPCungMac_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raMVTPCungMac.Checked)
                {
                    raMVTPGiacMac.Checked = false;
                    raMVTPRia.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raNhuomBaoKhong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raNhuomBaoKhong.Checked)
                {
                    raNhuomBaoCo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raNhuomBaoCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raNhuomBaoCo.Checked)
                {
                    raNhuomBaoKhong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raXeBaoTruocT3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raXeBaoTruocT3.Checked)
                {
                    raXMoBaoHinhTemThu.Checked = false;
                    raCatBao.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raXMoBaoHinhTemThu_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raXMoBaoHinhTemThu.Checked)
                {
                    raXeBaoTruocT3.Checked = false;
                    raCatBao.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raCatBao_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raCatBao.Checked)
                {
                    raXeBaoTruocT3.Checked = false;
                    raXMoBaoHinhTemThu.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raXoayNhanKhoKhan_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raXoayNhanKhoKhan.Checked)
                {
                    raXoayNhanDeDang.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raXoayNhanDeDang_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raXoayNhanDeDang.Checked)
                {
                    raXoayNhanKhoKhan.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDayNhanKhong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDayNhanKhong.Checked)
                {
                    raDayNhanCo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDayNhanCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDayNhanCo.Checked)
                {
                    raDayNhanKhong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDayNhanBangNuoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDayNhanBangNuoc.Checked)
                {
                    raDayNhanBangChatNhay.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDayNhanBangChatNhay_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDayNhanBangChatNhay.Checked)
                {
                    raDayNhanBangNuoc.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raHutChatT3IA_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raHutChatT3IA.Checked)
                {
                    raHutChatT3Kim2Nong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raHutChatT3Kim2Nong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raHutChatT3Kim2Nong.Checked)
                {
                    raHutChatT3IA.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDatIOLMem_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDatIOLMem.Checked)
                {
                    raDatIOLCung.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDatIOLCung_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDatIOLCung.Checked)
                {
                    raDatIOLMem.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDatIOLBangPince_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDatIOLBangPince.Checked)
                {
                    raDatIOLBangSung.Checked = false;
                    raDatIOLCoDinhCM.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDatIOLBangSung_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDatIOLBangSung.Checked)
                {
                    raDatIOLBangPince.Checked = false;
                    raDatIOLCoDinhCM.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raDatIOLCoDinhCM_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raDatIOLCoDinhCM.Checked)
                {
                    raDatIOLBangPince.Checked = false;
                    raDatIOLBangSung.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raRachBaoSauKhong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raRachBaoSauKhong.Checked)
                {
                    raRachBaoSauCo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raRBSCBSKhong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raRBSCBSKhong.Checked)
                {
                    raRBSCBSCo.Checked = false;
                    raRBSCBSCatBangKeo.Checked = false;
                    raRBSCBSMayCatDK.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raRBSCBSCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raRBSCBSCo.Checked)
                {
                    raRBSCBSKhong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raRBSCBSCatBangKeo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raRBSCBSCatBangKeo.Checked)
                {
                    raRBSCBSMayCatDK.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raRBSCBSMayCatDK_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raRBSCBSMayCatDK.Checked)
                {
                    raRBSCBSCatBangKeo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raRBSCMMNVKhong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raRBSCMMNVKhong.Checked)
                {
                    raRBSCMMNVCo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raPhucHoiVetMoBomPhu_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raPhucHoiVetMoBomPhu.Checked)
                {
                    raPhucHoiVetMoKhauVat.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raPhucHoiVetMoKhauVat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raPhucHoiVetMoKhauVat.Checked)
                {
                    raPhucHoiVetMoBomPhu.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTiemMatKhong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTiemMatKhong.Checked)
                {
                    raTiemMatCo.Checked = false;
                    raTiemMatTienPhong.Checked = false;
                    raTiemMatDuoiKM.Checked = false;
                    raTiemMatCNC.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTiemMatCo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTiemMatCo.Checked)
                {
                    raTiemMatKhong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTraMatDD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTraMatDD.Checked)
                {
                    raTraMatMo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void raTraMatMo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (raTraMatMo.Checked)
                {
                    raTraMatDD.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Glocom
        private void Glocom__raGloocomGocDong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raGloocomGocDong.Checked)
                {
                    Glocom__raGloocomGocMo.Checked = false;
                    Glocom__raGloocomBamSinh.Checked = false;
                    Glocom__raGloocomThuPhat.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raGloocomGocMo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raGloocomGocMo.Checked)
                {
                    Glocom__raGloocomGocDong.Checked = false;
                    Glocom__raGloocomBamSinh.Checked = false;
                    Glocom__raGloocomThuPhat.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raGloocomBamSinh_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raGloocomBamSinh.Checked)
                {
                    Glocom__raGloocomGocDong.Checked = false;
                    Glocom__raGloocomGocMo.Checked = false;
                    Glocom__raGloocomThuPhat.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raGloocomThuPhat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raGloocomThuPhat.Checked)
                {
                    Glocom__raGloocomGocDong.Checked = false;
                    Glocom__raGloocomGocMo.Checked = false;
                    Glocom__raGloocomBamSinh.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raGiaDoanBenhTiemTang_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raGiaDoanBenhTiemTang.Checked)
                {
                    Glocom__raGiaDoanBenhSoPhat.Checked = false;
                    Glocom__raGiaDoanBenhTienTrien.Checked = false;
                    Glocom__raGiaDoanBenhTranTrong.Checked = false;
                    Glocom__raGiaDoanBenhGanTuyet.Checked = false;
                    Glocom__raGiaDoanBenhTuyetDoi.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raGiaDoanBenhSoPhat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raGiaDoanBenhSoPhat.Checked)
                {
                    Glocom__raGiaDoanBenhTiemTang.Checked = false;
                    Glocom__raGiaDoanBenhTienTrien.Checked = false;
                    Glocom__raGiaDoanBenhTranTrong.Checked = false;
                    Glocom__raGiaDoanBenhGanTuyet.Checked = false;
                    Glocom__raGiaDoanBenhTuyetDoi.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raGiaDoanBenhTienTrien_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raGiaDoanBenhTienTrien.Checked)
                {
                    Glocom__raGiaDoanBenhTiemTang.Checked = false;
                    Glocom__raGiaDoanBenhSoPhat.Checked = false;
                    Glocom__raGiaDoanBenhTranTrong.Checked = false;
                    Glocom__raGiaDoanBenhGanTuyet.Checked = false;
                    Glocom__raGiaDoanBenhTuyetDoi.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raGiaDoanBenhTranTrong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raGiaDoanBenhTranTrong.Checked)
                {
                    Glocom__raGiaDoanBenhTiemTang.Checked = false;
                    Glocom__raGiaDoanBenhSoPhat.Checked = false;
                    Glocom__raGiaDoanBenhTienTrien.Checked = false;
                    Glocom__raGiaDoanBenhGanTuyet.Checked = false;
                    Glocom__raGiaDoanBenhTuyetDoi.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raGiaDoanBenhGanTuyet_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raGiaDoanBenhGanTuyet.Checked)
                {
                    Glocom__raGiaDoanBenhTiemTang.Checked = false;
                    Glocom__raGiaDoanBenhSoPhat.Checked = false;
                    Glocom__raGiaDoanBenhTienTrien.Checked = false;
                    Glocom__raGiaDoanBenhTranTrong.Checked = false;
                    Glocom__raGiaDoanBenhTuyetDoi.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raGiaDoanBenhTuyetDoi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raGiaDoanBenhTuyetDoi.Checked)
                {
                    Glocom__raGiaDoanBenhTiemTang.Checked = false;
                    Glocom__raGiaDoanBenhSoPhat.Checked = false;
                    Glocom__raGiaDoanBenhTienTrien.Checked = false;
                    Glocom__raGiaDoanBenhTranTrong.Checked = false;
                    Glocom__raGiaDoanBenhGanTuyet.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit1.Checked)
                {
                    checkEdit2.Checked = false;
                    checkEdit7.Checked = false;
                    checkEdit8.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit2.Checked)
                {
                    checkEdit1.Checked = false;
                    checkEdit7.Checked = false;
                    checkEdit8.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit7_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit7.Checked)
                {
                    checkEdit1.Checked = false;
                    checkEdit2.Checked = false;
                    checkEdit8.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit8_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit8.Checked)
                {
                    checkEdit1.Checked = false;
                    checkEdit2.Checked = false;
                    checkEdit7.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raPPVoCamGayMe_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raPPVoCamGayMe.Checked)
                {
                    Glocom__raPPVoCamTeTaiMat.Checked = false;
                    Glocom__raPPVoCamViTriBaoTenon.Checked = false;
                    Glocom__raPPVoCamCNC.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raPPVoCamTeTaiMat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raPPVoCamTeTaiMat.Checked)
                {
                    Glocom__raPPVoCamGayMe.Checked = false;
                    Glocom__raPPVoCamViTriBaoTenon.Checked = false;
                    Glocom__raPPVoCamCNC.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raPPVoCamViTriBaoTenon_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raPPVoCamViTriBaoTenon.Checked)
                {
                    Glocom__raPPVoCamGayMe.Checked = false;
                    Glocom__raPPVoCamTeTaiMat.Checked = false;
                    Glocom__raPPVoCamCNC.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raPPVoCamCNC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raPPVoCamCNC.Checked)
                {
                    Glocom__raPPVoCamGayMe.Checked = false;
                    Glocom__raPPVoCamTeTaiMat.Checked = false;
                    Glocom__raPPVoCamViTriBaoTenon.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raCoDinhNhanCauVanhMi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raCoDinhNhanCauVanhMi.Checked)
                {
                    Glocom__raCoDinhNhanCauCoTruc.Checked = false;
                    Glocom__raCoDinhNhanCauChiGiacMac.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raCoDinhNhanCauCoTruc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raCoDinhNhanCauCoTruc.Checked)
                {
                    Glocom__raCoDinhNhanCauVanhMi.Checked = false;
                    Glocom__raCoDinhNhanCauChiGiacMac.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raCoDinhNhanCauChiGiacMac_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raCoDinhNhanCauChiGiacMac.Checked)
                {
                    Glocom__raCoDinhNhanCauVanhMi.Checked = false;
                    Glocom__raCoDinhNhanCauCoTruc.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raCoDinhNhanCauDayCungDo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raCoDinhNhanCauDayCungDo.Checked)
                {
                    Glocom__raCoDinhNhanCauDayVungRia.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raCoDinhNhanCauDayVungRia_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raCoDinhNhanCauDayVungRia.Checked)
                {
                    Glocom__raCoDinhNhanCauDayCungDo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raTinhTrangBaoTenonBT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raTinhTrangBaoTenonBT.Checked)
                {
                    Glocom__raTinhTrangBaoTenonGay.Checked = false;
                    Glocom__raTinhTrangBaoTenonSo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raTinhTrangBaoTenonGay_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raTinhTrangBaoTenonGay.Checked)
                {
                    Glocom__raTinhTrangBaoTenonBT.Checked = false;
                    Glocom__raTinhTrangBaoTenonSo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Glocom__raTinhTrangBaoTenonSo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Glocom__raTinhTrangBaoTenonSo.Checked)
                {
                    Glocom__raTinhTrangBaoTenonBT.Checked = false;
                    Glocom__raTinhTrangBaoTenonGay.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit44_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit44.Checked)
                {
                    checkEdit43.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit43_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit43.Checked)
                {
                    checkEdit44.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit42_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit42.Checked)
                {
                    checkEdit38.Checked = false;
                    checkEdit37.Checked = false;
                    checkEdit36.Checked = false;
                    checkEdit11.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit38_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit38.Checked)
                {
                    checkEdit42.Checked = false;
                    checkEdit37.Checked = false;
                    checkEdit36.Checked = false;
                    checkEdit11.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit37_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit37.Checked)
                {
                    checkEdit42.Checked = false;
                    checkEdit38.Checked = false;
                    checkEdit36.Checked = false;
                    checkEdit11.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit36_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit36.Checked)
                {
                    checkEdit42.Checked = false;
                    checkEdit38.Checked = false;
                    checkEdit37.Checked = false;
                    checkEdit11.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit11_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit11.Checked)
                {
                    checkEdit42.Checked = false;
                    checkEdit38.Checked = false;
                    checkEdit37.Checked = false;
                    checkEdit36.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit31_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit31.Checked)
                {
                    checkEdit30.Checked = false;
                    checkEdit29.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit30_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit30.Checked)
                {
                    checkEdit31.Checked = false;
                    checkEdit29.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit29_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit29.Checked)
                {
                    checkEdit31.Checked = false;
                    checkEdit30.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit40_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit40.Checked)
                {
                    checkEdit39.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit39_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit39.Checked)
                {
                    checkEdit40.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit32_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit32.Checked)
                {
                    checkEdit35.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit35_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit35.Checked)
                {
                    checkEdit32.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit33_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit33.Checked)
                {
                    checkEdit41.Checked = false;
                    checkEdit34.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit41_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit41.Checked)
                {
                    checkEdit33.Checked = false;
                    checkEdit34.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit34_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit34.Checked)
                {
                    checkEdit33.Checked = false;
                    checkEdit41.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit27_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit27.Checked)
                {
                    checkEdit28.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit28_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit28.Checked)
                {
                    checkEdit27.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit21_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit21.Checked)
                {
                    checkEdit20.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit20_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit20.Checked)
                {
                    checkEdit21.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit26_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit26.Checked)
                {
                    checkEdit23.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit23_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit23.Checked)
                {
                    checkEdit26.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit18_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit18.Checked)
                {
                    checkEdit15.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit15_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit15.Checked)
                {
                    checkEdit18.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkEdit24_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit24.Checked)
                {
                    checkEdit22.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit22_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit22.Checked)
                {
                    checkEdit24.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit13_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit13.Checked)
                {
                    checkEdit25.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit5_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit5.Checked)
                {
                    checkEdit4.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit4_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit4.Checked)
                {
                    checkEdit5.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region PT mộng
        private void checkEdit64_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit64.Checked)
                {
                    checkEdit71.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit71_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit71.Checked)
                {
                    checkEdit64.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit104_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit104.Checked)
                {
                    checkEdit103.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit103_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit103.Checked)
                {
                    checkEdit104.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit69_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit69.Checked)
                {
                    checkEdit70.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit70_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit70.Checked)
                {
                    checkEdit69.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit115_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit115.Checked)
                {
                    checkEdit96.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit96_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit96.Checked)
                {
                    checkEdit115.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit107_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit107.Checked)
                {
                    checkEdit113.Checked = false;
                    checkEdit109.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit113_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit113.Checked)
                {
                    checkEdit107.Checked = false;
                    checkEdit109.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit109_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit109.Checked)
                {
                    checkEdit107.Checked = false;
                    checkEdit113.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit110_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit110.Checked)
                {
                    checkEdit3.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region PT sụp mi
        private void checkEdit3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit3.Checked)
                {
                    checkEdit110.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit137_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit137.Checked)
                {
                    checkEdit140.Checked = false;
                    checkEdit138.Checked = false;
                    checkEdit139.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit140_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit140.Checked)
                {
                    checkEdit137.Checked = false;
                    checkEdit138.Checked = false;
                    checkEdit139.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit138_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit138.Checked)
                {
                    checkEdit137.Checked = false;
                    checkEdit140.Checked = false;
                    checkEdit139.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit139_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit139.Checked)
                {
                    checkEdit137.Checked = false;
                    checkEdit140.Checked = false;
                    checkEdit138.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit133_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit133.Checked)
                {
                    checkEdit132.Checked = false;
                    checkEdit135.Checked = false;
                    checkEdit131.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit132_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit132.Checked)
                {
                    checkEdit133.Checked = false;
                    checkEdit135.Checked = false;
                    checkEdit131.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit135_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit135.Checked)
                {
                    checkEdit133.Checked = false;
                    checkEdit132.Checked = false;
                    checkEdit131.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit131_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit131.Checked)
                {
                    checkEdit133.Checked = false;
                    checkEdit132.Checked = false;
                    checkEdit135.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit128_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit128.Checked)
                {
                    checkEdit127.Checked = false;
                    checkEdit126.Checked = false;
                    checkEdit125.Checked = false;
                    checkEdit123.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit127_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit127.Checked)
                {
                    checkEdit128.Checked = false;
                    checkEdit126.Checked = false;
                    checkEdit125.Checked = false;
                    checkEdit123.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit126_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit126.Checked)
                {
                    checkEdit128.Checked = false;
                    checkEdit127.Checked = false;
                    checkEdit125.Checked = false;
                    checkEdit123.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit125_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit125.Checked)
                {
                    checkEdit128.Checked = false;
                    checkEdit127.Checked = false;
                    checkEdit126.Checked = false;
                    checkEdit123.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit123_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit123.Checked)
                {
                    checkEdit128.Checked = false;
                    checkEdit127.Checked = false;
                    checkEdit126.Checked = false;
                    checkEdit125.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit130_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit130.Checked)
                {
                    checkEdit129.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit129_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit129.Checked)
                {
                    checkEdit130.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit142_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit142.Checked)
                {
                    checkEdit117.Checked = false;
                    checkEdit124.Checked = false;
                    checkEdit120.Checked = false;
                    checkEdit122.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit117_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit117.Checked)
                {
                    checkEdit142.Checked = false;
                    checkEdit124.Checked = false;
                    checkEdit120.Checked = false;
                    checkEdit122.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit124_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit124.Checked)
                {
                    checkEdit142.Checked = false;
                    checkEdit117.Checked = false;
                    checkEdit120.Checked = false;
                    checkEdit122.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit120_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit120.Checked)
                {
                    checkEdit142.Checked = false;
                    checkEdit117.Checked = false;
                    checkEdit124.Checked = false;
                    checkEdit122.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit122_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit122.Checked)
                {
                    checkEdit142.Checked = false;
                    checkEdit117.Checked = false;
                    checkEdit124.Checked = false;
                    checkEdit120.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit119_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit119.Checked)
                {
                    checkEdit118.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit118_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit118.Checked)
                {
                    checkEdit119.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit114_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit114.Checked)
                {
                    checkEdit111.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit111_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit111.Checked)
                {
                    checkEdit114.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit102_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit102.Checked)
                {
                    checkEdit99.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit99_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit99.Checked)
                {
                    checkEdit102.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit105_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit105.Checked)
                {
                    checkEdit91.Checked = false;
                    checkEdit98.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit91_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit91.Checked)
                {
                    checkEdit105.Checked = false;
                    checkEdit98.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit98_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit98.Checked)
                {
                    checkEdit105.Checked = false;
                    checkEdit91.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit92_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit92.Checked)
                {
                    checkEdit90.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit90_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit90.Checked)
                {
                    checkEdit92.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit95_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit95.Checked)
                {
                    checkEdit89.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit89_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit89.Checked)
                {
                    checkEdit95.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit87_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit87.Checked)
                {
                    checkEdit84.Checked = false;
                    checkEdit88.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit84_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit84.Checked)
                {
                    checkEdit87.Checked = false;
                    checkEdit88.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit88_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit88.Checked)
                {
                    checkEdit87.Checked = false;
                    checkEdit84.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit83_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit83.Checked)
                {
                    checkEdit85.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit85_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit85.Checked)
                {
                    checkEdit83.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit86_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit86.Checked)
                {
                    checkEdit80.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit80_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit80.Checked)
                {
                    checkEdit86.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region tái tạo lệ quản
        private void checkEdit205_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit205.Checked)
                {
                    checkEdit201.Checked = false;
                    checkEdit204.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit201_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit201.Checked)
                {
                    checkEdit205.Checked = false;
                    checkEdit204.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit204_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit204.Checked)
                {
                    checkEdit205.Checked = false;
                    checkEdit201.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit200_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit200.Checked)
                {
                    checkEdit197.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit197_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit197.Checked)
                {
                    checkEdit200.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit194_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit194.Checked)
                {
                    checkEdit193.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit193_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit193.Checked)
                {
                    checkEdit194.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit184_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit184.Checked)
                {
                    checkEdit181.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit181_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit181.Checked)
                {
                    checkEdit184.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit182_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit182.Checked)
                {
                    checkEdit175.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit175_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit175.Checked)
                {
                    checkEdit182.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit190_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit190.Checked)
                {
                    checkEdit187.Checked = false;
                    checkEdit189.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit187_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit187.Checked)
                {
                    checkEdit190.Checked = false;
                    checkEdit189.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit189_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit189.Checked)
                {
                    checkEdit190.Checked = false;
                    checkEdit187.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit188_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit188.Checked)
                {
                    checkEdit178.Checked = false;
                    checkEdit186.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit178_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit178.Checked)
                {
                    checkEdit188.Checked = false;
                    checkEdit186.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit186_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit186.Checked)
                {
                    checkEdit188.Checked = false;
                    checkEdit178.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit179_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit179.Checked)
                {
                    checkEdit171.Checked = false;
                    checkEdit174.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit171_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit171.Checked)
                {
                    checkEdit179.Checked = false;
                    checkEdit174.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit174_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit174.Checked)
                {
                    checkEdit179.Checked = false;
                    checkEdit171.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion


        #region TT mống mắt
        private void checkEdit269_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit269.Checked)
                {
                    checkEdit267.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit267_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit267.Checked)
                {
                    checkEdit269.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit266_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit266.Checked)
                {
                    checkEdit268.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit268_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit268.Checked)
                {
                    checkEdit266.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit264_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit264.Checked)
                {
                    checkEdit261.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit261_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit261.Checked)
                {
                    checkEdit264.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit258_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit258.Checked)
                {
                    checkEdit257.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit257_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit257.Checked)
                {
                    checkEdit258.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit256_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit256.Checked)
                {
                    checkEdit255.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit255_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit255.Checked)
                {
                    checkEdit256.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit333_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit333.Checked)
                {
                    checkEdit329.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit329_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit329.Checked)
                {
                    checkEdit333.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit328_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit328.Checked)
                {
                    checkEdit325.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit325_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit325.Checked)
                {
                    checkEdit328.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit322_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit322.Checked)
                {
                    checkEdit321.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit321_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit321.Checked)
                {
                    checkEdit322.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit320_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit320.Checked)
                {
                    checkEdit319.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit319_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit319.Checked)
                {
                    checkEdit320.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void dtDrawDate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dtDrawDate.Properties.Buttons[1].Visible = false;
                    dtDrawDate.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtSewingDate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dtSewingDate.Properties.Buttons[1].Visible = false;
                    dtSewingDate.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDrawDate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtDrawDate.EditValue != null)
                {
                    dtDrawDate.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    dtDrawDate.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtSewingDate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtSewingDate.EditValue != null)
                {
                    dtSewingDate.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    dtSewingDate.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmInputDetail
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmInputDetail = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(frmInputDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcPtDucTTT.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTraMatMo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTraMatMo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit10.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit10.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTraMatDD.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTraMatDD.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTiemMatCNC.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTiemMatCNC.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTiemMatDuoiKM.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTiemMatDuoiKM.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTiemMatTienPhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTiemMatTienPhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTiemMatCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTiemMatCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTiemMatKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTiemMatKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raPhucHoiVetMoKhauVat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPhucHoiVetMoKhauVat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raPhucHoiVetMoBomPhu.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPhucHoiVetMoBomPhu.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl8.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl8.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl7.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl7.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl6.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raRBSCMMNVCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCMMNVCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raRBSCBSCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCBSCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raRBSCMMNVKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCMMNVKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raRBSCBSMayCatDK.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCBSMayCatDK.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raRBSCBSKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCBSKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raRBSCBSCatBangKeo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCBSCatBangKeo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raRachBaoSauCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRachBaoSauCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raRachBaoSauKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRachBaoSauKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDatIOLBangSung.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDatIOLBangSung.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDatIOLCung.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDatIOLCung.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDatIOLCoDinhCM.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDatIOLCoDinhCM.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDatIOLBangPince.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDatIOLBangPince.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDatIOLMem.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDatIOLMem.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raHutChatT3IA.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raHutChatT3IA.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raHutChatT3Kim2Nong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raHutChatT3Kim2Nong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDayNhanBangChatNhay.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDayNhanBangChatNhay.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDayNhanBangNuoc.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDayNhanBangNuoc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDayNhanCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDayNhanCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raXoayNhanKhoKhan.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raXoayNhanKhoKhan.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDayNhanKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDayNhanKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit6.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit6.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raXoayNhanDeDang.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raXoayNhanDeDang.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raCatBao.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raCatBao.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raXMoBaoHinhTemThu.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raXMoBaoHinhTemThu.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raXeBaoTruocT3.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raXeBaoTruocT3.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raNhuomBaoCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raNhuomBaoCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl5.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl4.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl4.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raNhuomBaoKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raNhuomBaoKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit12.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit12.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raMVTPCungMac.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMVTPCungMac.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raMVTPRia.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMVTPRia.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raMVTPGiacMac.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMVTPGiacMac.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raMoKMRiaCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMoKMRiaCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raMoKMRiaKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMoKMRiaKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raLechTTT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raLechTTT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDoIII.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDoIII.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDoV.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDoV.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDoIV.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDoIV.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDoII.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDoII.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raChiCoTruc.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raChiCoTruc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raVanhMi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raVanhMi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTeTaiMat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTeTaiMat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raMe.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMe.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTrongBao.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTrongBao.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raPhaco.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPhaco.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raMT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTreoCungMac.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTreoCungMac.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raNgoaiBao.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raNgoaiBao.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raMP.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMP.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDucT3BenhLy.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDucT3BenhLy.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDucT3BamSinh.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDucT3BamSinh.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raKhongCoT3.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raKhongCoT3.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raLechT3.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raLechT3.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDucT3Gia.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDucT3Gia.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raDoI.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDoI.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl3.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem38.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem38.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem39.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem39.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem41.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem41.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem42.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem44.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem47.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem47.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem52.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem52.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem58.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem58.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem59.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem59.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem64.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem64.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem69.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem69.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem51.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem51.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem53.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem53.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem72.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem72.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem74.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem74.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem79.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem79.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem73.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem73.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem75.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem75.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem67.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem67.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem80.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem80.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem82.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem82.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem84.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem84.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem81.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem81.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem89.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem89.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem92.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem92.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem93.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem93.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTTMongMat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTTMongMat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl24.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl24.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl10.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl10.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raPTMong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPTMong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.tabPagePTDucTTT.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.tabPagePTDucTTT.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.tabPageGlocom.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.tabPageGlocom.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lcGlocom.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl33.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl33.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit1.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit1.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit2.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit2.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit7.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit7.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit8.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit8.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit9.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit9.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl32.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl32.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit4.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit4.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit5.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit5.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit11.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit11.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit13.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit13.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl11.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl11.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl12.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl12.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit15.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit15.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit18.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit18.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit20.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit20.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit21.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit21.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit22.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit22.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit23.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit23.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit24.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit24.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit25.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit25.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit26.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit26.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit27.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit27.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit28.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit28.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit29.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit29.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit30.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit30.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit31.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit31.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit32.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit32.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit33.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit33.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit34.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit34.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit35.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit35.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit36.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit36.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit37.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit37.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit38.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit38.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit39.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit39.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl14.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl14.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl15.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl15.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl16.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl16.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit40.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit40.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit41.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit41.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit42.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit42.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit43.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit43.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit44.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit44.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raCoDinhNhanCauDayVungRia.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raCoDinhNhanCauDayVungRia.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raCoDinhNhanCauDayCungDo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raCoDinhNhanCauDayCungDo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raTinhTrangBaoTenonGay.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raTinhTrangBaoTenonGay.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raPPVoCamCNC.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raPPVoCamCNC.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raCoDinhNhanCauChiGiacMac.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raCoDinhNhanCauChiGiacMac.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raTinhTrangBaoTenonSo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raTinhTrangBaoTenonSo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raPPVoCamViTriBaoTenon.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raPPVoCamViTriBaoTenon.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raCoDinhNhanCauCoTruc.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raCoDinhNhanCauCoTruc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raCoDinhNhanCauVanhMi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raCoDinhNhanCauVanhMi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl17.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl17.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raPPVoCamTeTaiMat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raPPVoCamTeTaiMat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raPPVoCamGayMe.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raPPVoCamGayMe.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhGanTuyet.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhGanTuyet.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhTienTrien.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhTienTrien.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhSoPhat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhSoPhat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhTuyetDoi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhTuyetDoi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhTranTrong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhTranTrong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhTiemTang.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhTiemTang.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raGloocomGocMo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGloocomGocMo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raGloocomThuPhat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGloocomThuPhat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raGloocomBamSinh.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGloocomBamSinh.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raGloocomGocDong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGloocomGocDong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Glocom__raTinhTrangBaoTenonBT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raTinhTrangBaoTenonBT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl18.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl18.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem104.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem104.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem109.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem109.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem115.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem115.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem117.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem117.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem119.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem119.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem122.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem122.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem128.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem128.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem132.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem132.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem135.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem135.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem136.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem136.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem143.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem143.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem144.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem144.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem145.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem145.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem153.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem153.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem169.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem169.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem187.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem187.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem121.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem121.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem130.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem130.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem141.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem141.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem157.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem157.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem138.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem138.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem152.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem152.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem158.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem158.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem146.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem146.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem168.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem168.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem170.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem170.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem162.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem162.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem174.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem174.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem175.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem175.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem182.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem182.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem185.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem185.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem281.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem281.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem96.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem96.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem285.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem285.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.tabPagePTMong.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.tabPagePTMong.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl9.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lcPtMong.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl10.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit72.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit72.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit108.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit108.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl20.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl20.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit71.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit71.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit70.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit70.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit69.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit69.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit3.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit3.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit93.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit93.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit96.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit96.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit97.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit97.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit100.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit100.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit101.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit101.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl26.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl26.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit103.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit103.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit104.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit104.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit106.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit106.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit107.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit107.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit109.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit109.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit110.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit110.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit112.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit112.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit113.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit113.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit115.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit115.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit64.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit64.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit68.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit68.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem184.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem184.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem209.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem209.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem106.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem106.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem230.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem230.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem228.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem228.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem202.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem202.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem159.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem159.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem176.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem176.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem225.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem225.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem147.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem147.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem148.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem148.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem233.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem233.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem177.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem177.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem226.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem226.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem199.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem199.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem173.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem173.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem212.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem212.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem244.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem244.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem245.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem245.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem248.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem248.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl11.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl11.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.TabPagePTSupMi.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.TabPagePTSupMi.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl12.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl12.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lcPtSupMi.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl13.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl53.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl53.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl19.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl19.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl21.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl21.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit80.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit80.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit83.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit83.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit84.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit84.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit85.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit85.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit86.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit86.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit87.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit87.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit88.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit88.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit89.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit89.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit90.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit90.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit91.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit91.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit92.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit92.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit95.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit95.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit98.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit98.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit99.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit99.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit102.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit102.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit105.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit105.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit111.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit111.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl22.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl22.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl23.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl23.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl24.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl24.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit114.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit114.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit117.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit117.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit118.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit118.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit119.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit119.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit120.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit120.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit122.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit122.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit123.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit123.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit124.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit124.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit125.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit125.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit126.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit126.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit127.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit127.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit128.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit128.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl25.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl25.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit129.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit129.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit130.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit130.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit131.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit131.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit132.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit132.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit133.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit133.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit135.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit135.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit136.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit136.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit137.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit137.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit138.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit138.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit139.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit139.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit140.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit140.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit141.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit141.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit142.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit142.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl27.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl27.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl52.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl52.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem180.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem180.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem201.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem201.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem211.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem211.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem214.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem214.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem218.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem218.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem219.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem219.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem237.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem237.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem250.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem250.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem254.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem254.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem262.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem262.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem573.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem573.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem216.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem216.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem235.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem235.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem242.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem242.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem247.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem247.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem260.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem260.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem261.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem261.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem270.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem270.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem253.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem253.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem271.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem271.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem275.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem275.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem287.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem287.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl14.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl14.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.TabPageTaiTaoLeQuan.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.TabPageTaiTaoLeQuan.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl15.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl15.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lcTaiTaoLeQuan.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl16.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit144.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit144.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit171.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit171.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit173.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit173.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit174.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit174.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit175.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit175.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit176.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit176.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit177.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit177.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit178.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit178.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit179.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit179.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit180.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit180.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit181.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit181.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit182.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit182.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit183.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit183.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit184.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit184.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit185.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit185.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit186.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit186.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit187.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit187.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit188.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit188.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit189.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit189.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit190.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit190.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit191.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit191.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit192.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit192.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl34.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl34.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit193.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit193.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit194.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit194.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit197.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit197.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit200.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit200.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit201.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit201.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit204.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit204.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit205.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit205.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit206.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit206.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem306.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem306.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem311.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem311.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem317.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem317.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem321.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem321.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem323.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem323.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem325.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem325.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem332.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem332.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem319.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem319.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem340.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem340.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem330.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem330.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem334.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem334.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem331.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem331.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem335.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem335.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem342.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem342.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem327.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem327.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem337.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem337.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem343.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem343.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem354.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem354.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem324.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem324.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem355.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem355.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem364.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem364.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem359.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem359.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem387.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem387.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl17.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl17.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.TabPageTTMongMat.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.TabPageTTMongMat.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl18.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl18.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lcTTMongMat.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl19.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl28.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl28.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl37.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl37.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl38.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl38.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl39.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl39.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl40.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl40.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl41.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl41.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit255.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit255.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit256.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit256.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl42.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl42.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit257.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit257.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit258.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit258.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit261.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit261.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit264.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit264.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit266.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit266.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit267.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit267.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit268.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit268.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit269.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit269.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem395.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem395.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem400.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem400.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem406.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem406.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem410.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem410.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem421.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem421.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem426.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem426.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl20.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl20.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.TabPageTTTLaser.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.TabPageTTTLaser.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl21.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl21.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lcTTTLaser.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl22.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl31.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl31.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl29.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl29.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl30.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl30.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl13.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl13.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit319.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit319.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit320.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit320.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl50.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl50.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit321.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit321.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit322.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit322.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit325.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit325.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit328.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit328.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit329.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit329.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.checkEdit333.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit333.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem484.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem484.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem489.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem489.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem495.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem495.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem499.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem499.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem518.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem518.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl23.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl23.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raPTDucThuyTInhThe.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPTDucThuyTInhThe.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raTTLaser.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTTLaser.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raPTGlocom.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPTGlocom.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raPTSupMi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPTSupMi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.raPTTaiTaoLeQuan.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPTTaiTaoLeQuan.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.xtraTabEye.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.xtraTabEye.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.xtraTabSkin.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.xtraTabSkin.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lcDaLieu.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl25.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl44.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl44.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl43.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl43.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.chkDiscloseSkin.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkDiscloseSkin.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.chkCloseSkin.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkCloseSkin.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.chkShapingSkinOther.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkShapingSkinOther.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.chkShapingSkin.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkShapingSkin.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.chkDamageTypePart.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkDamageTypePart.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.chkDamageTypeAll.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkDamageTypeAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl36.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl36.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl35.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl35.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.labelControl9.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl9.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.chkPostureTummy.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkPostureTummy.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.chkPostureSide.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkPostureSide.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.chkPostureUp.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkPostureUp.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lciPosture.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciPosture.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lciSkinDamage.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciSkinDamage.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lciSkinDamagePosition.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciSkinDamagePosition.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lciSkinDamageAmount.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciSkinDamageAmount.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lciDamageType.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciDamageType.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lciShapingSkin.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciShapingSkin.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lciCloseSkin.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciCloseSkin.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.lciTreatSkinOther.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciTreatSkinOther.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.btnDelete.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.simpleButton2.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.simpleButton2.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.xtraTabOther.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.xtraTabOther.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControl_TabOther.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl_TabOther.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem286.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem286.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem289.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem289.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem291.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem291.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem293.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem293.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem295.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem295.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.layoutControlItem295.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem295.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.Text", Resources.ResourceLanguageManager.LanguageResource__frmInputDetail, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repPlus_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                lstDataDmv.Add(new DmvADO());
                lstDataDmv.ForEach(o =>
                {
                    o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                });
                lstDataDmv.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                gridControl1.DataSource = null;
                gridControl1.DataSource = lstDataDmv;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repMinus_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var item = (DmvADO)gridView1.GetFocusedRow();
                lstDataDmv.Remove(item);
                lstDataDmv.ForEach(o =>
                {
                    o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                });
                lstDataDmv.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                gridControl1.DataSource = null;
                gridControl1.DataSource = lstDataDmv;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public enum ContentType { INTRODUCER, GUIDING_CATHETER, GUITE_WIRE, BALLON, STENT, CONTRAST_AGENT }
        public ContentType typeFind { get; set; }
        private void ShorForm()
        {
            try
            {
                frmMediMate frm = new frmMediMate(this.sereServ.ID, GetResult);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetResult(string obj)
        {
            try
            {
                switch (typeFind)
                {
                    case ContentType.INTRODUCER:
                        txtIntroducer.Text = obj;
                        break;
                    case ContentType.GUIDING_CATHETER:
                        txtGuidingCatheter.Text = obj;
                        break;
                    case ContentType.GUITE_WIRE:
                        txtGuiteWire.Text = obj;
                        break;
                    case ContentType.BALLON:
                        txtBallon.Text = obj;
                        break;
                    case ContentType.STENT:
                        txtStent.Text = obj;
                        break;
                    case ContentType.CONTRAST_AGENT:
                        txtContrastAgent.Text = obj;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnIntroducer_Click(object sender, EventArgs e)
        {
            try
            {
                typeFind = ContentType.INTRODUCER;
                ShorForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGuiteWire_Click(object sender, EventArgs e)
        {
            try
            {
                typeFind = ContentType.GUITE_WIRE;
                ShorForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnStent_Click(object sender, EventArgs e)
        {
            try
            {
                typeFind = ContentType.STENT;
                ShorForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGuidingCatheter_Click(object sender, EventArgs e)
        {
            try
            {
                typeFind = ContentType.GUIDING_CATHETER;
                ShorForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBallon_Click(object sender, EventArgs e)
        {
            try
            {
                typeFind = ContentType.BALLON;
                ShorForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnContrastAgent_Click(object sender, EventArgs e)
        {
            try
            {
                typeFind = ContentType.CONTRAST_AGENT;
                ShorForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Action")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridView1.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repPlus;
                    }
                    else
                    {
                        e.RepositoryItem = repMinus;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                isReset = true;
                List<DevExpress.XtraLayout.LayoutControl> layoutControls = new List<DevExpress.XtraLayout.LayoutControl>
                    {
                        lcPtDucTTT, lcGlocom, lcPtMong, lcPtSupMi, lcTaiTaoLeQuan, lcTTMongMat, lcTTTLaser, lcDaLieu, lcDieuTriCanThiepDMV, layoutControl_TabOther
                    };
                foreach (DevExpress.XtraLayout.LayoutControl layoutControl in layoutControls)
                {
                    layoutControl.BeginUpdate();
                    try
                    {
                        foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl.Items)
                        {
                            DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                            if (lci != null && lci.Control != null)
                            {
                                if ((lci.Control is BaseEdit) && !(lci.Control is DevExpress.XtraEditors.CheckEdit))
                                {
                                    DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                                    fomatFrm.EditValue = null;
                                    fomatFrm.Text = null;
                                }
                                else if (lci.Control is DevExpress.XtraEditors.CheckEdit)
                                {
                                    DevExpress.XtraEditors.CheckEdit checkEdit = lci.Control as DevExpress.XtraEditors.CheckEdit;

                                    checkEdit.Checked = false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                    finally
                    {
                        layoutControl.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinKichThuocRachForraRachBaoSau_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ngăn nhập ký tự không phải số
            }
        }

        private void txtkichthuoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ngăn nhập ký tự không phải số
            }
        }

        private void txtNangLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ngăn nhập ký tự không phải số
            }
        }

        private void txtLucHut_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ngăn nhập ký tự không phải số
            }
        }
    }
}
