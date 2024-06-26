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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {

        internal void RebuildNhaThuocMediMatyWithInControlContainer()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStockNhaThuocSelecteds), currentMediStockNhaThuocSelecteds));
                if ((this.currentMediStockNhaThuocSelecteds != null && this.currentMediStockNhaThuocSelecteds.Count > 0))
                {
                    List<V_HIS_MEST_ROOM> mediStocks = new List<V_HIS_MEST_ROOM>();
                    foreach (var item in currentMediStockNhaThuocSelecteds)
                    {
                        mediStocks.Add(new V_HIS_MEST_ROOM()
                        {
                            MEDI_STOCK_ID = item.ID,
                            MEDI_STOCK_CODE = item.MEDI_STOCK_CODE,
                            MEDI_STOCK_NAME = item.MEDI_STOCK_NAME,
                        });
                    }
                    InitDataMetyMatyTypeInStockD1(mediStocks);
                }

                this.RebuildPopupContainerNhaThuocShowMediMatyForSelect(this.mediStockD1ADOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void RebuildPopupContainerNhaThuocShowMediMatyForSelect(List<DMediStock1ADO> dMediStock1ADOs)
        {
            try
            {
                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                popupControlContainerMediMaty.Width = theRequiredWidth;
                popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_NAME";
                col2.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col2.Width = 250;
                col2.VisibleIndex = 1;
                gridViewMediMaty.Columns.Add(col2);


                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "SERVICE_UNIT_NAME_DISPLAY";
                col3.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col3.Width = 60;
                col3.VisibleIndex = 2;
                col3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col3);


                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "CONCENTRA";
                col8.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col8.Width = 100;
                col8.VisibleIndex = 3;
                gridViewMediMaty.Columns.Add(col8);

                DevExpress.XtraGrid.Columns.GridColumn col14 = new DevExpress.XtraGrid.Columns.GridColumn();
                col14.FieldName = "PARENT_NAME";
                col14.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_PARENT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col14.Width = 100;
                col14.VisibleIndex = 4;
                gridViewMediMaty.Columns.Add(col14);

                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col7.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col7.Width = 160;
                col7.VisibleIndex = 5;
                gridViewMediMaty.Columns.Add(col7);

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "MANUFACTURER_NAME";
                col9.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col9.Width = 150;
                col9.VisibleIndex = 10;
                gridViewMediMaty.Columns.Add(col9);


                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "AMOUNT_DISPLAY";
                col4.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_AVAILABLE_AMOUNT",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col4.Width = 70;
                col4.VisibleIndex = 6;
                col4.DisplayFormat.FormatString = "#,##0.000000";
                col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                col4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col4);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "IMP_PRICE_DISPLAY";
                col5.Caption = "Giá bán";
                col5.Width = 100;
                col5.VisibleIndex = 7;
                col5.DisplayFormat.FormatString = "#,##0.0000";
                col5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                col5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col6 = new DevExpress.XtraGrid.Columns.GridColumn();
                col6.FieldName = "IMP_VAT_RATIO_DISPLAY";
                col6.Caption = "VAT(%)";
                col6.Width = 100;
                col6.VisibleIndex = 8;
                col6.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col6);



                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "NATIONAL_NAME";
                col10.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col10.Width = 80;
                col10.VisibleIndex = 9;
                gridViewMediMaty.Columns.Add(col10);



                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_CODE";
                col1.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col1.Width = 60;
                col1.VisibleIndex = 10;
                gridViewMediMaty.Columns.Add(col1);


                //Phuc vu cho tim kiem khong dau

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col11.Width = 80;
                col11.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col11);

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col12.Width = 80;
                col12.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col12.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col12);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col13.VisibleIndex = -1;
                col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewMediMaty.Columns.Add(col13);

                gridViewMediMaty.GridControl.DataSource = dMediStock1ADOs;
                gridViewMediMaty.EndUpdate();
                LogSystem.Debug("RebuildPopupContainerNhaThuocShowMediMatyForSelect__Du lieu thuoc/vat tu____ " + (dMediStock1ADOs != null ? dMediStock1ADOs.Count : 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<DMediStock1ADO> ConvertToDMediStockForNhaThuoc(List<D_HIS_MEDI_STOCK_2> listMediStock)
        {
            List<DMediStock1ADO> result = new List<DMediStock1ADO>();
            try
            {
                var currentMedicineTypeTemps = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var currentMaterialTypeTemps = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (listMediStock != null && listMediStock.Count > 0)
                {
                    //- Khi chọn nhà thuốc, hiển thị các thuốc/vật tư thỏa mãn 1 trong 2 điều kiện:
                    //+ Các thuốc/vật tư có tồn > 0 và các thuốc
                    //+ Các thuốc/vật tư là thuốc/vật tư ngoại viện (HIS_MATERIAL_TYPE/HIS_MEDICINE_TYPE có IS_OUT_HOSPITAL = 1)


                    foreach (var item in listMediStock)
                    {
                        bool isUse = false;
                        V_HIS_MEDICINE_TYPE mety = null;
                        V_HIS_MATERIAL_TYPE maty = null;

                        if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            mety = currentMedicineTypeTemps.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault();
                        }
                        if (this.oldServiceReq != null && this.oldServiceReq.IS_EXECUTE_KIDNEY_PRES == 1 && item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            isUse = (mety != null && mety.IS_KIDNEY == 1);
                        }
                        else
                        {
                            isUse = true;
                        }


                        if (isUse)
                        {
                            if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                            {
                                maty = currentMaterialTypeTemps != null && currentMaterialTypeTemps.Any(o => o.SERVICE_ID == item.SERVICE_ID) ? currentMaterialTypeTemps.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault() : null;
                            }

                            DMediStock1ADO m1 = null;
                            //if (!HisConfigCFG.IsAutoCreateSaleExpMest)
                            //{
                            //    m1 = result.Where(o => o.ID == item.ID && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID && o.EXP_PRICE_DISPLAY == (o.LAST_EXP_PRICE * (1 + o.LAST_EXP_VAT_RATIO))).FirstOrDefault();
                            //    if (m1 != null)
                            //    {
                            //        decimal? am = ((m1.IsUseOrginalUnitForPres ?? false) == false && item.CONVERT_RATIO.HasValue && item.CONVERT_RATIO > 0) ? item.AMOUNT * item.CONVERT_RATIO : item.AMOUNT;
                            //        m1.AMOUNT += am;
                            //        SetAmountOddByKeyShowRoundAvailableAmount(ref m1, mety, currentMedicineTypeTemps, currentMaterialTypeTemps);
                            //    }
                            //}

                            if (m1 == null)
                            {
                                DMediStock1ADO dMediStock1ADO = new DMediStock1ADO();
                                dMediStock1ADO.ACTIVE_INGR_BHYT_CODE = item.ACTIVE_INGR_BHYT_CODE;
                                dMediStock1ADO.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                                dMediStock1ADO.ALERT_MAX_IN_PRESCRIPTION = item.ALERT_MAX_IN_PRESCRIPTION;
                                dMediStock1ADO.ALERT_MAX_IN_TREATMENT = item.ALERT_MAX_IN_TREATMENT;
                                dMediStock1ADO.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK;
                                dMediStock1ADO.CONCENTRA = item.CONCENTRA;
                                dMediStock1ADO.GENDER_ID = item.GENDER_ID;
                                dMediStock1ADO.HEIN_SERVICE_TYPE_ID = item.HEIN_SERVICE_TYPE_ID;
                                dMediStock1ADO.ID = item.ID;
                                dMediStock1ADO.IMP_PRICE = item.IMP_PRICE;
                                dMediStock1ADO.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                                dMediStock1ADO.IS_ACTIVE = item.IS_ACTIVE;
                                dMediStock1ADO.IS_AUTO_EXPEND = item.IS_AUTO_EXPEND;
                                dMediStock1ADO.IS_CHEMICAL_SUBSTANCE = item.IS_CHEMICAL_SUBSTANCE;
                                dMediStock1ADO.IS_OUT_PARENT_FEE = item.IS_OUT_PARENT_FEE;
                                dMediStock1ADO.IS_STAR_MARK = item.IS_STAR_MARK;
                                dMediStock1ADO.IS_STENT = item.IS_STENT;
                                dMediStock1ADO.IS_VACCINE = item.IS_VACCINE;
                                dMediStock1ADO.MANUFACTURER_CODE = item.MANUFACTURER_CODE;
                                dMediStock1ADO.MANUFACTURER_ID = item.MANUFACTURER_ID;
                                dMediStock1ADO.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                                dMediStock1ADO.MEDI_STOCK_CODE = item.MEDI_STOCK_CODE;
                                dMediStock1ADO.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                                dMediStock1ADO.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
                                dMediStock1ADO.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                                dMediStock1ADO.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                dMediStock1ADO.MEDICINE_USE_FORM_ID = item.MEDICINE_USE_FORM_ID;
                                dMediStock1ADO.NATIONAL_NAME = item.NATIONAL_NAME;
                                dMediStock1ADO.RANK = item.RANK;
                                dMediStock1ADO.SERVICE_ID = item.SERVICE_ID;
                                dMediStock1ADO.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                                dMediStock1ADO.SERVICE_UNIT_CODE = item.SERVICE_UNIT_CODE;
                                dMediStock1ADO.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                                dMediStock1ADO.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                dMediStock1ADO.TUTORIAL = item.TUTORIAL;
                                dMediStock1ADO.USE_ON_DAY = item.USE_ON_DAY;
                                dMediStock1ADO.CONVERT_RATIO = item.CONVERT_RATIO;
                                dMediStock1ADO.CONVERT_UNIT_CODE = item.CONVERT_UNIT_CODE;
                                dMediStock1ADO.CONVERT_UNIT_NAME = item.CONVERT_UNIT_NAME;
                                dMediStock1ADO.LAST_EXP_PRICE = item.LAST_EXP_PRICE;
                                dMediStock1ADO.LAST_EXP_VAT_RATIO = item.LAST_EXP_VAT_RATIO;
                                dMediStock1ADO.EXP_PRICE_DISPLAY = (item.LAST_EXP_PRICE * (1 + item.LAST_EXP_VAT_RATIO));
                                dMediStock1ADO.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_CODE) + item.MEDICINE_TYPE_CODE;
                                dMediStock1ADO.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_NAME) + item.MEDICINE_TYPE_NAME;
                                dMediStock1ADO.ACTIVE_INGR_BHYT_NAME__UNSIGN = StringUtil.convertToUnSign3(item.ACTIVE_INGR_BHYT_NAME) + item.ACTIVE_INGR_BHYT_NAME;
                                dMediStock1ADO.DO_NOT_REQUIRED_USE_FORM = item.DO_NOT_REQUIRED_USE_FORM;
                                //MestMetyUnitWorker.UpdateUnit(dMediStock1ADO, GlobalStore.HisMestMetyUnit);
                                dMediStock1ADO.AMOUNT = item.AMOUNT;

                                SetAmountOddByKeyShowRoundAvailableAmount(ref dMediStock1ADO, mety, currentMedicineTypeTemps, currentMaterialTypeTemps);

                                if (mety != null)
                                {
                                    dMediStock1ADO.SERVICE_ID = mety.SERVICE_ID;
                                    dMediStock1ADO.SERVICE_TYPE_ID = mety.SERVICE_TYPE_ID;
                                    dMediStock1ADO.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                                    dMediStock1ADO.HEIN_SERVICE_TYPE_ID = mety.HEIN_SERVICE_TYPE_ID;
                                    dMediStock1ADO.DO_NOT_REQUIRED_USE_FORM = mety.DO_NOT_REQUIRED_USE_FORM;
                                    //dMediStock1ADO.HEIN_SERVICE_TYPE_CODE = mety.HEIN_SERVICE_TYPE_CODE;
                                    //dMediStock1ADO.HEIN_SERVICE_BHYT_CODE = mety.HEIN_SERVICE_BHYT_CODE;
                                    //dMediStock1ADO.HEIN_SERVICE_BHYT_NAME = mety.HEIN_SERVICE_BHYT_NAME;
                                    //dMediStock1ADO.IS_BLOCK_MAX_IN_PRESCRIPTION = mety.IS_BLOCK_MAX_IN_PRESCRIPTION;
                                    //dMediStock1ADO.IS_SPLIT_COMPENSATION = mety.IS_SPLIT_COMPENSATION;
                                    //dMediStock1ADO.IS_OUT_HOSPITAL = mety.IS_OUT_HOSPITAL;
                                    //dMediStock1ADO.IsAllowOdd = (mety.IS_ALLOW_ODD == 1) ? true : false;
                                    //dMediStock1ADO.IsAllowOddAndExportOdd = (mety.IS_ALLOW_ODD == 1 && mety.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                                    //dMediStock1ADO.DESCRIPTION = mety.DESCRIPTION;

                                }

                                if (maty != null)
                                {
                                    dMediStock1ADO.MATERIAL_TYPE_MAP_ID = maty.MATERIAL_TYPE_MAP_ID;
                                    //dMediStock1ADO.MATERIAL_TYPE_MAP_CODE = maty.MATERIAL_TYPE_MAP_CODE;
                                    //dMediStock1ADO.MATERIAL_TYPE_MAP_NAME = maty.MATERIAL_TYPE_MAP_NAME;
                                    //dMediStock1ADO.IS_OUT_HOSPITAL = maty.IS_OUT_HOSPITAL;
                                    //dMediStock1ADO.IsAllowOdd = (maty.IS_ALLOW_ODD == 1) ? true : false;
                                    //dMediStock1ADO.IsAllowOddAndExportOdd = (maty.IS_ALLOW_ODD == 1 && maty.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                                    //dMediStock1ADO.DESCRIPTION = maty.DESCRIPTION;

                                    //dMediStock1ADO.IS_SPLIT_COMPENSATION = maty.IS_SPLIT_COMPENSATION;
                                }
                                //if ((dMediStock1ADO.IS_OUT_HOSPITAL.HasValue && dMediStock1ADO.IS_OUT_HOSPITAL == GlobalVariables.CommonNumberTrue))
                                //{
                                //    Inventec.Common.Logging.LogSystem.Debug("MEDICINE_TYPE_CODE: " + dMediStock1ADO.MEDICINE_TYPE_CODE + ", MEDICINE_TYPE_NAME: " + dMediStock1ADO.MEDICINE_TYPE_NAME + ", IS_OUT_HOSPITAL: " + dMediStock1ADO.IS_OUT_HOSPITAL);
                                //}
                                //if (dMediStock1ADO.MEDICINE_TYPE_CODE == "VTNT_195")
                                //{
                                //    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maty), maty));
                                //    Inventec.Common.Logging.LogSystem.Debug("MEDICINE_TYPE_CODE: " + dMediStock1ADO.MEDICINE_TYPE_CODE + ", MEDICINE_TYPE_NAME: " + dMediStock1ADO.MEDICINE_TYPE_NAME + ", IS_OUT_HOSPITAL: " + dMediStock1ADO.IS_OUT_HOSPITAL);
                                //}
                                //if ((dMediStock1ADO.AMOUNT.HasValue && dMediStock1ADO.AMOUNT.Value > 0) || (dMediStock1ADO.IS_OUT_HOSPITAL.HasValue && dMediStock1ADO.IS_OUT_HOSPITAL == GlobalVariables.CommonNumberTrue))
                                result.Add(dMediStock1ADO);
                            }
                        }
                    }
                }

                var currentMedicineTypeTempsHasOutHospital = currentMedicineTypeTemps != null ? currentMedicineTypeTemps.Where(o => o.IS_OUT_HOSPITAL.HasValue && o.IS_OUT_HOSPITAL == 1 && (result.Count == 0 || !result.Exists(k => k.SERVICE_ID == o.SERVICE_ID))).ToList() : null;
                if (currentMedicineTypeTempsHasOutHospital != null && currentMedicineTypeTempsHasOutHospital.Count > 0)
                {
                    foreach (var item in currentMedicineTypeTempsHasOutHospital)
                    {
                        DMediStock1ADO dMediStock1ADO = new DMediStock1ADO();
                        dMediStock1ADO.ACTIVE_INGR_BHYT_CODE = item.ACTIVE_INGR_BHYT_CODE;
                        dMediStock1ADO.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                        dMediStock1ADO.ALERT_MAX_IN_PRESCRIPTION = item.ALERT_MAX_IN_PRESCRIPTION;
                        dMediStock1ADO.ALERT_MAX_IN_TREATMENT = item.ALERT_MAX_IN_TREATMENT;
                        dMediStock1ADO.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK;
                        dMediStock1ADO.CONCENTRA = item.CONCENTRA;
                        dMediStock1ADO.GENDER_ID = item.TDL_GENDER_ID;
                        dMediStock1ADO.HEIN_SERVICE_TYPE_ID = item.HEIN_SERVICE_TYPE_ID;
                        dMediStock1ADO.ID = item.ID;
                        dMediStock1ADO.IMP_PRICE = item.IMP_PRICE;
                        dMediStock1ADO.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        dMediStock1ADO.IS_ACTIVE = item.IS_ACTIVE;
                        dMediStock1ADO.IS_AUTO_EXPEND = item.IS_AUTO_EXPEND;
                        dMediStock1ADO.IS_CHEMICAL_SUBSTANCE = item.IS_CHEMICAL_SUBSTANCE;
                        dMediStock1ADO.IS_OUT_PARENT_FEE = item.IS_OUT_PARENT_FEE;
                        dMediStock1ADO.IS_STAR_MARK = item.IS_STAR_MARK;
                        //dMediStock1ADO.IS_STENT = item.IS_STENT;
                        dMediStock1ADO.IS_VACCINE = item.IS_VACCINE;
                        dMediStock1ADO.MANUFACTURER_CODE = item.MANUFACTURER_CODE;
                        dMediStock1ADO.MANUFACTURER_ID = item.MANUFACTURER_ID;
                        dMediStock1ADO.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                        //dMediStock1ADO.MEDI_STOCK_CODE = item.MEDI_STOCK_CODE;
                        //dMediStock1ADO.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                        //dMediStock1ADO.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
                        dMediStock1ADO.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                        dMediStock1ADO.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                        dMediStock1ADO.MEDICINE_USE_FORM_ID = item.MEDICINE_USE_FORM_ID;
                        dMediStock1ADO.NATIONAL_NAME = item.NATIONAL_NAME;
                        dMediStock1ADO.RANK = item.RANK;
                        dMediStock1ADO.SERVICE_ID = item.SERVICE_ID;
                        dMediStock1ADO.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                        dMediStock1ADO.SERVICE_UNIT_CODE = item.SERVICE_UNIT_CODE;
                        dMediStock1ADO.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                        dMediStock1ADO.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        dMediStock1ADO.TUTORIAL = item.TUTORIAL;
                        dMediStock1ADO.USE_ON_DAY = item.USE_ON_DAY;
                        dMediStock1ADO.CONVERT_RATIO = item.CONVERT_RATIO;
                        dMediStock1ADO.CONVERT_UNIT_CODE = item.CONVERT_UNIT_CODE;
                        dMediStock1ADO.CONVERT_UNIT_NAME = item.CONVERT_UNIT_NAME;
                        dMediStock1ADO.LAST_EXP_PRICE = item.LAST_EXP_PRICE;
                        dMediStock1ADO.LAST_EXP_VAT_RATIO = item.LAST_EXP_VAT_RATIO;
                        dMediStock1ADO.EXP_PRICE_DISPLAY = (item.LAST_EXP_PRICE * (1 + item.LAST_EXP_VAT_RATIO));
                        dMediStock1ADO.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_CODE) + item.MEDICINE_TYPE_CODE;
                        dMediStock1ADO.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_NAME) + item.MEDICINE_TYPE_NAME;
                        dMediStock1ADO.ACTIVE_INGR_BHYT_NAME__UNSIGN = StringUtil.convertToUnSign3(item.ACTIVE_INGR_BHYT_NAME) + item.ACTIVE_INGR_BHYT_NAME;

                        dMediStock1ADO.SERVICE_ID = item.SERVICE_ID;
                        dMediStock1ADO.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                        dMediStock1ADO.ACTIVE_INGR_BHYT_CODE = item.ACTIVE_INGR_BHYT_CODE;
                        dMediStock1ADO.HEIN_SERVICE_TYPE_ID = item.HEIN_SERVICE_TYPE_ID;
                        dMediStock1ADO.DO_NOT_REQUIRED_USE_FORM = item.DO_NOT_REQUIRED_USE_FORM;
                        //dMediStock1ADO.HEIN_SERVICE_TYPE_CODE = item.HEIN_SERVICE_TYPE_CODE;
                        //dMediStock1ADO.HEIN_SERVICE_BHYT_CODE = item.HEIN_SERVICE_BHYT_CODE;
                        //dMediStock1ADO.HEIN_SERVICE_BHYT_NAME = item.HEIN_SERVICE_BHYT_NAME;
                        //dMediStock1ADO.IS_BLOCK_MAX_IN_PRESCRIPTION = item.IS_BLOCK_MAX_IN_PRESCRIPTION;
                        //dMediStock1ADO.IS_SPLIT_COMPENSATION = item.IS_SPLIT_COMPENSATION;
                        //dMediStock1ADO.IS_OUT_HOSPITAL = item.IS_OUT_HOSPITAL;
                        //dMediStock1ADO.IsAllowOdd = (item.IS_ALLOW_ODD == 1) ? true : false;
                        //dMediStock1ADO.IsAllowOddAndExportOdd = (item.IS_ALLOW_ODD == 1 && item.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                        //dMediStock1ADO.DESCRIPTION = item.DESCRIPTION;

                        result.Add(dMediStock1ADO);
                    }
                }
                var currentMaterialTypeTempsHasOutHospital = currentMaterialTypeTemps != null ? currentMaterialTypeTemps.Where(o => o.IS_OUT_HOSPITAL.HasValue && o.IS_OUT_HOSPITAL == 1 && (result.Count == 0 || !result.Exists(k => k.SERVICE_ID == o.SERVICE_ID))).ToList() : null;
                if (currentMaterialTypeTempsHasOutHospital != null && currentMaterialTypeTempsHasOutHospital.Count > 0)
                {
                    foreach (var item in currentMaterialTypeTempsHasOutHospital)
                    {
                        DMediStock1ADO dMediStock1ADO = new DMediStock1ADO();
                        dMediStock1ADO.ALERT_MAX_IN_PRESCRIPTION = item.ALERT_MAX_IN_PRESCRIPTION;
                        dMediStock1ADO.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK;
                        dMediStock1ADO.CONCENTRA = item.CONCENTRA;
                        dMediStock1ADO.GENDER_ID = item.TDL_GENDER_ID;
                        dMediStock1ADO.HEIN_SERVICE_TYPE_ID = item.HEIN_SERVICE_TYPE_ID;
                        dMediStock1ADO.ID = item.ID;
                        dMediStock1ADO.IMP_PRICE = item.IMP_PRICE;
                        dMediStock1ADO.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        dMediStock1ADO.IS_ACTIVE = item.IS_ACTIVE;
                        dMediStock1ADO.IS_AUTO_EXPEND = item.IS_AUTO_EXPEND;
                        dMediStock1ADO.IS_CHEMICAL_SUBSTANCE = item.IS_CHEMICAL_SUBSTANCE;
                        dMediStock1ADO.IS_OUT_PARENT_FEE = item.IS_OUT_PARENT_FEE;
                        dMediStock1ADO.IS_STENT = item.IS_STENT;
                        dMediStock1ADO.MANUFACTURER_CODE = item.MANUFACTURER_CODE;
                        dMediStock1ADO.MANUFACTURER_ID = item.MANUFACTURER_ID;
                        dMediStock1ADO.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                        //dMediStock1ADO.MEDI_STOCK_CODE = item.MEDI_STOCK_CODE;
                        //dMediStock1ADO.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                        //dMediStock1ADO.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
                        dMediStock1ADO.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                        dMediStock1ADO.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        dMediStock1ADO.NATIONAL_NAME = item.NATIONAL_NAME;
                        dMediStock1ADO.SERVICE_ID = item.SERVICE_ID;
                        dMediStock1ADO.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                        dMediStock1ADO.SERVICE_UNIT_CODE = item.SERVICE_UNIT_CODE;
                        dMediStock1ADO.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                        dMediStock1ADO.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        dMediStock1ADO.CONVERT_RATIO = item.CONVERT_RATIO;
                        dMediStock1ADO.CONVERT_UNIT_CODE = item.CONVERT_UNIT_CODE;
                        dMediStock1ADO.CONVERT_UNIT_NAME = item.CONVERT_UNIT_NAME;
                        dMediStock1ADO.LAST_EXP_PRICE = item.LAST_EXP_PRICE;
                        dMediStock1ADO.LAST_EXP_VAT_RATIO = item.LAST_EXP_VAT_RATIO;
                        dMediStock1ADO.EXP_PRICE_DISPLAY = (item.LAST_EXP_PRICE * (1 + item.LAST_EXP_VAT_RATIO));
                        dMediStock1ADO.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(item.MATERIAL_TYPE_CODE) + item.MATERIAL_TYPE_CODE;
                        dMediStock1ADO.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(item.MATERIAL_TYPE_NAME) + item.MATERIAL_TYPE_NAME;

                        //dMediStock1ADO.HEIN_SERVICE_TYPE_CODE = item.HEIN_SERVICE_TYPE_CODE;
                        //dMediStock1ADO.HEIN_SERVICE_BHYT_CODE = item.HEIN_SERVICE_BHYT_CODE;
                        //dMediStock1ADO.HEIN_SERVICE_BHYT_NAME = item.HEIN_SERVICE_BHYT_NAME;
                        //dMediStock1ADO.MATERIAL_TYPE_MAP_ID = item.MATERIAL_TYPE_MAP_ID;
                        //dMediStock1ADO.MATERIAL_TYPE_MAP_CODE = item.MATERIAL_TYPE_MAP_CODE;
                        //dMediStock1ADO.MATERIAL_TYPE_MAP_NAME = item.MATERIAL_TYPE_MAP_NAME;
                        //dMediStock1ADO.IS_OUT_HOSPITAL = item.IS_OUT_HOSPITAL;
                        //dMediStock1ADO.IsAllowOdd = (item.IS_ALLOW_ODD == 1) ? true : false;
                        //dMediStock1ADO.IsAllowOddAndExportOdd = (item.IS_ALLOW_ODD == 1 && item.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                        //dMediStock1ADO.DESCRIPTION = item.DESCRIPTION;

                        result.Add(dMediStock1ADO);
                    }
                }
                //Inventec.Common.Logging.LogSystem.Debug("ConvertToDMediStockForNhaThuoc___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SetAmountOddByKeyShowRoundAvailableAmount(ref DMediStock1ADO dMediStock1ADO, V_HIS_MEDICINE_TYPE mety, List<V_HIS_MEDICINE_TYPE> medicineTypes, List<V_HIS_MATERIAL_TYPE> materialTypes)
        {
            try
            {
                if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet) return;//|| GlobalStore.IsExecutePTTT

                long? serviceId = dMediStock1ADO.SERVICE_ID;
                long? id = dMediStock1ADO.ID;
                decimal? serviceTypeId = dMediStock1ADO.SERVICE_TYPE_ID;
                decimal? amount = dMediStock1ADO.AMOUNT;

                if (amount == (long)amount) return;

                if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    if (mety == null)
                        mety = medicineTypes.Where(o => o.SERVICE_ID == serviceId).FirstOrDefault();
                    if (mety != null && (mety.IS_ALLOW_EXPORT_ODD ?? -1) != 1)
                    {
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MEDICINE_TYPE_NAME", dMediStock1ADO.MEDICINE_TYPE_NAME) + "____" + Inventec.Common.Logging.LogUtil.TraceData("AMOUNT", dMediStock1ADO.AMOUNT));
                        dMediStock1ADO.AMOUNT = (long)dMediStock1ADO.AMOUNT;
                    }
                }
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                {
                    var maty = materialTypes.Where(o => o.SERVICE_ID == serviceId).FirstOrDefault();
                    if (maty != null && (maty.IS_ALLOW_EXPORT_ODD ?? -1) != 1)
                    {
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MEDICINE_TYPE_NAME", dMediStock1ADO.MEDICINE_TYPE_NAME) + "____" + Inventec.Common.Logging.LogUtil.TraceData("AMOUNT", dMediStock1ADO.AMOUNT));
                        dMediStock1ADO.AMOUNT = (long)dMediStock1ADO.AMOUNT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void RebuildMediMatyWithInControlContainer(object data)
        {
            try
            {
                //Tại màn hình kê đơn, nếu phòng mà người dùng đang làm việc có "Giới hạn thuốc được phép sử dụng" (IS_RESTRICT_MEDICINE_TYPE trong HIS_ROOM bằng true) thì danh sách thuốc khi kê thuốc trong kho chỉ hiển thị các thuốc được khai cấu hình tương ứng với phòng đấy (dữ liệu lưu trong bảng HIS_MEDICINE_TYPE_ROOM)
                List<DMediStock1ADO> dMediStock1s = data as List<DMediStock1ADO>;
                long roomId = GetRoomId();
                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                if (room != null && (room.IS_RESTRICT_MEDICINE_TYPE ?? 0) == 1)
                {
                    List<HIS_MEDICINE_TYPE_ROOM> medicineTypeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE_ROOM>()
                        .Where(o => o.ROOM_ID == roomId).ToList();
                    List<long> medicineTypeIdRooms = medicineTypeRooms != null ? medicineTypeRooms.Select(o => o.MEDICINE_TYPE_ID).ToList() : new List<long>();
                    dMediStock1s = dMediStock1s.Where(o => medicineTypeIdRooms.Contains(o.ID ?? 0)
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                }

                //Loc du lieu theo Rank
                if (cboUser.EditValue != null)
                {
                    string loginname = cboUser.EditValue.ToString();
                    List<HIS_EMPLOYEE> employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>();
                    if (employees == null) employees = new List<HIS_EMPLOYEE>();
                    HIS_EMPLOYEE employee = employees.FirstOrDefault(o => o.LOGINNAME == loginname);
                    if (employee == null || !employee.MEDICINE_TYPE_RANK.HasValue)
                    {
                        dMediStock1s = dMediStock1s.Where(o => o.RANK == null).ToList();
                    }
                    else
                    {
                        dMediStock1s = dMediStock1s.Where(o => o.RANK == null || o.RANK <= employee.MEDICINE_TYPE_RANK).ToList();
                    }
                }

                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                popupControlContainerMediMaty.Width = theRequiredWidth;
                popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_CODE";
                col2.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col2.Width = 60;
                col2.VisibleIndex = 1;
                gridViewMediMaty.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_NAME";
                col1.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col1.Width = 250;
                col1.VisibleIndex = 2;
                gridViewMediMaty.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "SERVICE_UNIT_NAME";
                col7.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col7.Width = 60;
                col7.VisibleIndex = 3;
                gridViewMediMaty.Columns.Add(col7);

                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "AMOUNT";
                col3.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_AVAILABLE_AMOUNT",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col3.Width = 70;
                col3.VisibleIndex = 4;
                col3.DisplayFormat.FormatString = "#,##0.00";
                col3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                gridViewMediMaty.Columns.Add(col3);

                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col4.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col4.Width = 160;
                col4.VisibleIndex = 5;
                gridViewMediMaty.Columns.Add(col4);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "CONCENTRA";
                col5.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col5.Width = 100;
                col5.VisibleIndex = 6;
                gridViewMediMaty.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "PARENT_NAME";
                col13.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_PARENT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col13.Width = 100;
                col13.VisibleIndex = 7;
                gridViewMediMaty.Columns.Add(col13);

                DevExpress.XtraGrid.Columns.GridColumn col6 = new DevExpress.XtraGrid.Columns.GridColumn();
                col6.FieldName = "MEDI_STOCK_NAME";
                col6.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MEDI_STOCK",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col6.Width = 100;
                col6.VisibleIndex = 8;
                gridViewMediMaty.Columns.Add(col6);

                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "MANUFACTURER_NAME";
                col8.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col8.Width = 150;
                col8.VisibleIndex = 9;
                gridViewMediMaty.Columns.Add(col8);

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "NATIONAL_NAME";
                col9.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col9.Width = 80;
                col9.VisibleIndex = 10;
                gridViewMediMaty.Columns.Add(col9);


                //Phuc vu cho tim kiem khong dau

                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col10.Width = 80;
                col10.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col11.Width = 80;
                col11.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col11.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col11);

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col12.VisibleIndex = -1;
                col12.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewMediMaty.Columns.Add(col12);

                gridViewMediMaty.GridControl.DataSource = dMediStock1s;
                gridViewMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MetyMatyTypeInStock_RowClick(object data)
        {
            try
            {
                this.currentMedicineTypeADOForEdit = new MediMatyTypeADO();
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this.currentMedicineTypeADOForEdit, data);
                    if (this.currentMedicineTypeADOForEdit == null) throw new ArgumentNullException("currentMedicineTypeADOForEdit is null");
                    if (data.GetType() == typeof(DMediStock1ADO))
                    {
                        DMediStock1ADO dMediStock = data as DMediStock1ADO;
                        this.currentMedicineTypeADOForEdit.IsStent = ((dMediStock.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                        this.currentMedicineTypeADOForEdit.IsAllowOdd = this.GetIsAllowOdd(dMediStock);
                        this.currentMedicineTypeADOForEdit.DO_NOT_REQUIRED_USE_FORM = GetDoNotRequiredUseForm(dMediStock);
                        this.SetControlSoLuongNgayNhapChanLe(this.currentMedicineTypeADOForEdit);
                    }

                    //this.ChangeControlSoLuongNgayNhapChanLe(this.currentMedicineTypeADOForEdit.MEDI_STOCK_ID ?? 0);
                    //this.UpdateMedicineUseFormInDataRow(this.currentMedicineTypeADOForEdit);
                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.VisibleButton(this.actionBosung);
                    this.ReSetDataInputAfterAdd__MedicinePage();
                    this.btnAdd.Enabled = true;
                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        this.FillDataIntoMedicineUseForm(currentMedicineTypeADOForEdit.ID);

                        //Neu la vat tu thi mặc định focus vào ô số lượng
                        this.spinAmount.Focus();
                        this.spinAmount.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool GetIsAllowOdd(DMediStock1ADO dMediStock)
        {
            bool result = false;
            try
            {
                if (dMediStock != null)
                {
                    V_HIS_MEDICINE_TYPE medicineType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == dMediStock.ID);
                    if (medicineType != null && medicineType.IS_ALLOW_ODD.HasValue)
                    {
                        result = medicineType.IS_ALLOW_ODD == 1 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private short? GetDoNotRequiredUseForm(DMediStock1ADO dMediStock)
        {
            short? result = null;
            try
            {
                if (dMediStock != null)
                {
                    V_HIS_MEDICINE_TYPE medicineType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == dMediStock.ID);
                    if (medicineType != null)
                    {
                        result = medicineType.DO_NOT_REQUIRED_USE_FORM;
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void FillDataIntoMedicineUseForm(long medicineTypeId)
        {
            try
            {
                //Lấy dữ liệu cấu hình hướng dẫn sử dụng của thuốc (HIS_MEDICINE_TYPE_TUT) theo tài khoản đăng nhập và loại thuốc
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var medicineTypeTuts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                if (medicineTypeTuts != null && medicineTypeTuts.Count > 0)
                {
                    List<HIS_MEDICINE_TYPE_TUT> medicineTypeTutFilters = medicineTypeTuts.OrderByDescending(o => o.MODIFY_TIME).Where(o => o.MEDICINE_TYPE_ID == medicineTypeId && o.LOGINNAME == loginName).ToList();

                    this.medicineTypeTutSelected = medicineTypeTutFilters.FirstOrDefault();
                    if (this.medicineTypeTutSelected != null)
                    {
                        //Nếu hướng dẫn sử dụng mẫu có đường dùng thì lấy ra
                        if (this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID > 0)
                        {
                            this.cboMedicineUseForm.EditValue = this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID;
                        }
                        //Nếu không có đường dùng thì lấy đường dùng từ danh mục loại thuốc
                        else
                        {
                            var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeId);
                            if (medicineType != null)
                            {
                                if ((medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                                {
                                    this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;
                                }
                                currentMedicineTypeADOForEdit.DO_NOT_REQUIRED_USE_FORM = medicineType.DO_NOT_REQUIRED_USE_FORM;
                            }
                        }

                        if (this.spinSoNgay.Value < (this.medicineTypeTutSelected.DAY_COUNT ?? 0))
                            this.spinSoNgay.EditValue = this.medicineTypeTutSelected.DAY_COUNT;

                    }
                }
                //Trường hợp thuốc không có cấu hình hướng dẫn sử dụng thì lấy hướng dẫn sử dụng ở danh mục loại thuốc fill vào
                else
                    this.medicineTypeTutSelected = null;

                if (this.medicineTypeTutSelected == null)
                {
                    var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeId);
                    if (medicineType != null)
                    {
                        if ((medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                        {
                            this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;
                        }
                        currentMedicineTypeADOForEdit.DO_NOT_REQUIRED_USE_FORM = medicineType.DO_NOT_REQUIRED_USE_FORM;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long? CalulateUseTimeTo()
        {
            long? result = null;
            try
            {
                if (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.OrderByDescending(o => o).First() > 0 && !string.IsNullOrEmpty(this.spinSoNgay.Text))
                {
                    long useTime = this.intructionTimeSelecteds.OrderByDescending(o => o).First();
                    DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(useTime) ?? DateTime.MinValue;
                    DateTime dtUseTimeTo = dtUseTime.AddDays((double)this.spinSoNgay.Value - 1);
                    long useTimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(dtUseTimeTo.ToString("yyyyMMddHHmm") + "00");
                    result = useTimeTo;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool ExistsAssianInDay(long serviceId)
        {
            bool result = false;
            try
            {
                if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count > 0)
                {
                    result = this.sereServWithTreatment.Where(o => o.SERVICE_ID == serviceId
                        && o.INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.OrderByDescending(t => t).First().ToString().Substring(0, 8)).Any();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitDataMetyMatyTypeInStockD()
        {
            try
            {
                InitDataMetyMatyTypeInStockD1(this.currentMediStock);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gọi api lấy dữ liệu thuốc và vật tư gộp trong 1 danh sách, lọc theo các điều kiện và cấu hình khác => sử dụng trong phòng khám
        /// </summary>
        private void InitDataMetyMatyTypeInStockD1(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStock)
        {
            try
            {
                if (currentMediStock != null && currentMediStock.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
                    List<long> mediStockIds = new List<long>();

                    mediStockIds = currentMediStock.Select(o => o.MEDI_STOCK_ID).ToList();
                    filter.MEDI_STOCK_IDs = mediStockIds;
                    this.ProcessFilterDontPresExpiredTime(ref filter);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                    this.mediStockD1ADOs = new BackendAdapter(param).Get<List<DMediStock1ADO>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);

                    LogSystem.Debug("Load du lieu kho theo dieu kien loc 1____ " + String.Join(",", mediStockIds) + "____ket qua tim thay " + (this.mediStockD1ADOs != null ? this.mediStockD1ADOs.Count : 0));

                    ConvertToDMediStock1(this.mediStockD1ADOs);

                    if (this.mediStockD1ADOs != null && this.mediStockD1ADOs.Count > 0)
                    {
                        this.mediStockD1ADOs = this.mediStockD1ADOs.Where(o => (o.AMOUNT ?? 0) > 0).ToList();                      
                        MediStockWorker.FilterByMediStockMeti(mediStockIds, ref this.mediStockD1ADOs);
                        MediStockWorker.FilterByMestMetyDepa(mediStockIds, this.currentWorkPlace, ref this.mediStockD1ADOs);
                    }
                }
                else
                {
                    this.mediStockD1ADOs = new List<DMediStock1ADO>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessGroupByForCFGDontPresExpiredTime(ref List<DMediStock1ADO> mediStockD1s)
        {
            try
            {
                if (mediStockD1s == null || mediStockD1s.Count == 0) return;

                List<DMediStock1ADO> mediStockD1News = new List<DMediStock1ADO>();
                var mediGroups = mediStockD1s.GroupBy(o => new { o.MEDI_STOCK_ID, o.ID, o.SERVICE_TYPE_ID });
                foreach (var item in mediGroups)
                {
                    DMediStock1ADO mediAdd = item.ToList().First();
                    mediAdd.AMOUNT = item.ToList().Sum(o => o.AMOUNT);

                    mediStockD1News.Add(mediAdd);
                }
                mediStockD1s = new List<DMediStock1ADO>();
                mediStockD1s.AddRange(mediStockD1News);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hiện tại: Hệ thống chưa có luồng xử lý đối với các thuốc, vật tư hết hạn sử dụng khi kê đơn.
        ///Mong muốn:
        ///Đối với các thuốc, vật tư có hạn sử dụng nhỏ hơn ngày y lệnh thì không hiển thị lên tại màn hình kê đơn.
        ///#17940
        /// </summary>
        private void ProcessFilterDontPresExpiredTime(ref MOS.Filter.DHisMediStock1Filter filter)
        {
            try
            {
                //- Bổ sung cấu hình hệ thống: "MOS.HIS_MEDI_STOCK.DONT_PRES_EXPIRED_ITEM": "1: Không cho phép kê thuốc/vật tư hết hạn sử dụng."
                //- Sửa chức năng "Kê đơn dược", "Kê đơn YHCT", "Tủ trực":
                //Khi bật cấu hình trên (giá trị = 1), thì khi load thông tin tồn kho để người dùng chọn, sẽ lấy thông tin tồn kho của các thuốc ko có hạn sử dụng hoặc hạn sử dụng thỏa mãn: 
                //+ Nếu là kê đơn từng ngày, thì hạn sử dụng phải lớn hơn hoặc bằng thời gian y lệnh.
                //+ Nếu là kê đơn nhiều ngày, và cấu hình chọn nhiều ngày theo cả đơn, thì hạn sử dụng phải lớn hơn hoặc bằng thời gian y lệnh của ngày lớn nhất được chọn
                //+ Nếu là kê đơn nhiều ngày, và cấu hình chọn nhiều ngày theo từng thuốc, thì HSD phải lớn hơn hoặc bằng thời gian chỉ định của đơn 

                //(server bổ sung filter để cho phép lấy thông tin tồn theo ngày)
                if (HisConfigCFG.IsDontPresExpiredTime)
                {
                    filter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = this.intructionTimeSelecteds.OrderByDescending(o => o).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ConvertToDMediStock1(List<DMediStock1ADO> listMediStock)
        {
            try
            {
                if (listMediStock != null && listMediStock.Count > 0)
                {
                    var currentMedicineTypeTemps = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                  
                    foreach (var item in listMediStock)
                    {
                        V_HIS_MEDICINE_TYPE mety = null;
                        if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            mety = currentMedicineTypeTemps != null && currentMedicineTypeTemps.Any(o => o.SERVICE_ID == item.SERVICE_ID) ? currentMedicineTypeTemps.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault() : null;
                        }
                        item.ATC_CODES = mety != null ? mety.ATC_CODES : null;
                        item.ATC_GROUP_CODES = mety != null ? mety.ATC_GROUP_CODES : null;
                        item.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_CODE);
                        item.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_NAME);
                        item.CONTRAINDICATION = item.CONTRAINDICATION;
                        item.ACTIVE_INGR_BHYT_NAME__UNSIGN = StringUtil.convertToUnSign3(item.ACTIVE_INGR_BHYT_NAME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateMedicineUseFormInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                bool hasUseForm = false;
                if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    V_HIS_MEDICINE_TYPE mety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.SERVICE_ID == medicineTypeSDO.SERVICE_ID);
                    if (mety != null)
                    {
                        medicineTypeSDO.MEDICINE_USE_FORM_ID = mety.MEDICINE_USE_FORM_ID;
                        medicineTypeSDO.MEDICINE_USE_FORM_CODE = mety.MEDICINE_USE_FORM_CODE;
                        medicineTypeSDO.MEDICINE_USE_FORM_NAME = mety.MEDICINE_USE_FORM_NAME;
                        hasUseForm = true;
                    }
                }
                if (!hasUseForm)
                {
                    medicineTypeSDO.MEDICINE_USE_FORM_ID = null;
                    medicineTypeSDO.MEDICINE_USE_FORM_CODE = "";
                    medicineTypeSDO.MEDICINE_USE_FORM_NAME = "";
                    medicineTypeSDO.ErrorMessageMedicineUseForm = "";
                    medicineTypeSDO.ErrorTypeMedicineUseForm = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
