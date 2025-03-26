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
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Logging;
using Inventec.UC.ComboTHX.Data;
using Inventec.UC.ComboTHX.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Design.Template1
{
    public partial class Template1
    {

        private void LoadDataCommuneFromDbToLocal()
        {
            try
            {
                //SdaCommuneViewFilter filter = new SdaCommuneViewFilter();
                //ICA.MANAGER.Sda.SdaCommune.SdaCommuneLogic mngUIConcrete = new ICA.MANAGER.Sda.SdaCommune.SdaCommuneLogic(new CommonParam());
                var sCommune = listCommune;//mngUIConcrete.Get(filter);
                if (sCommune != null && sCommune.Count > 0)
                {
                    var listCOMMUNE = sCommune.OrderBy(o => o.COMMUNE_NAME).ToList();
                    if (listDistrict != null)
                    {
                        var query = (
                            from d in listDistrict
                            join c in listCOMMUNE on d.ID equals c.DISTRICT_ID
                            select new ViewSdaCommuneModel
                            {
                                ID = c.ID,
                                CREATE_TIME = c.CREATE_TIME,
                                MODIFY_TIME = c.MODIFY_TIME,
                                CREATOR = c.CREATOR,
                                MODIFIER = c.MODIFIER,
                                IS_ACTIVE = c.IS_ACTIVE,
                                IS_DELETE = c.IS_DELETE,
                                GROUP_CODE = c.GROUP_CODE,
                                SEARCH_CODE = c.SEARCH_CODE,
                                PROVINCE_ID = d.PROVINCE_ID,
                                PROVINCE_CODE = d.PROVINCE_CODE,
                                PROVINCE_NAME = d.PROVINCE_NAME,
                                DISTRICT_INITIAL_NAME = c.DISTRICT_INITIAL_NAME,
                                DISTRICT_CODE = c.DISTRICT_CODE,
                                DISTRICT_ID = c.DISTRICT_ID,
                                DISTRICT_NAME = c.DISTRICT_NAME,
                                INITIAL_NAME = c.INITIAL_NAME,
                                COMMUNE_CODE = c.COMMUNE_CODE,
                                COMMUNE_NAME = c.COMMUNE_NAME,
                                VIR_COMMUNE_CODE = c.VIR_COMMUNE_CODE,
                            });
                        listData = query.ToList();
                        listDataTHX = query.OrderBy(o => o.PROVINCE_NAME).ToList();
                    }
                    else
                    {
                        var query = (
                          from c in listCommune
                          select new ViewSdaCommuneModel
                          {
                              ID = c.ID,
                              CREATE_TIME = c.CREATE_TIME,
                              MODIFY_TIME = c.MODIFY_TIME,
                              CREATOR = c.CREATOR,
                              MODIFIER = c.MODIFIER,
                              IS_ACTIVE = c.IS_ACTIVE,
                              IS_DELETE = c.IS_DELETE,
                              GROUP_CODE = c.GROUP_CODE,
                              SEARCH_CODE = c.SEARCH_CODE,
                              DISTRICT_CODE = c.DISTRICT_CODE,
                              DISTRICT_INITIAL_NAME = c.DISTRICT_INITIAL_NAME,
                              DISTRICT_ID = c.DISTRICT_ID,
                              DISTRICT_NAME = c.DISTRICT_NAME,
                              INITIAL_NAME = c.INITIAL_NAME,
                              COMMUNE_CODE = c.COMMUNE_CODE,
                              COMMUNE_NAME = c.COMMUNE_NAME,
                              VIR_COMMUNE_CODE = c.VIR_COMMUNE_CODE,
                          });
                        listData = query.ToList();
                    }
                }
                else
                {
                    listData = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadTHX(string _MaTHX)
        {
            try
            {
                List<ViewSdaCommuneModel> listResult = new List<ViewSdaCommuneModel>();
                listResult = listData.Where(o => (o.SEARCH_CODE != null && o.SEARCH_CODE.StartsWith(_MaTHX))).ToList();

                cboTHX.Properties.DataSource = listResult;
                cboTHX.Properties.DisplayMember = "RENDERER_PDC_NAME";
                cboTHX.Properties.ValueMember = "ID";
                cboTHX.Properties.ForceInitialize();
                cboTHX.Properties.Columns.Clear();
                cboTHX.Properties.Columns.Add(new LookUpColumnInfo("SEARCH_CODE", "", 100));
                cboTHX.Properties.Columns.Add(new LookUpColumnInfo("RENDERER_PDC_NAME", "", 400));
                cboTHX.Properties.ShowHeader = true;
                cboTHX.Properties.ImmediatePopup = true;
                cboTHX.Properties.DropDownRows = 20;
                cboTHX.Properties.PopupWidth = 500;
                if (listResult.Count == 1)
                {
                    cboTHX.EditValue = listResult[0].ID;
                    txtMaTHX.Text = listResult[0].SEARCH_CODE;
                    var districtDTO = listDistrict.Where(o => o.ID == listResult[0].DISTRICT_ID).FirstOrDefault();
                    if (districtDTO != null)
                    {
                        if (_LoadHuyen != null) _LoadHuyen(districtDTO.PROVINCE_CODE);
                        //LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                        if (_SetValueTinh != null) _SetValueTinh(districtDTO.PROVINCE_CODE);
                        //cboTinh.EditValue = districtDTO.PROVINCE_NAME;
                        //txtMaTinh.Text = districtDTO.PROVINCE_CODE;
                    }
                    if (_LoadPhuongXa != null) _LoadPhuongXa(listResult[0].DISTRICT_CODE);
                    //LoadXaCombo("", listResult[0].DISTRICT_CODE, false);
                    if (_SetValueHuyen != null) _SetValueHuyen(listResult[0].DISTRICT_CODE);
                    //cboHuyen.EditValue = listResult[0].DISTRICT_NAME;
                    //txtMaHuyen.Text = listResult[0].DISTRICT_CODE;
                    if (_SetValuePhuongXa != null) _SetValuePhuongXa(listResult[0].COMMUNE_CODE);
                    //cboXaPhuong.EditValue = listResult[0].COMMUNE_CODE;
                    //txtMaXaPhuong.Text = listResult[0].COMMUNE_CODE;
                    if (_FocusNext != null) _FocusNext();
                    //txtMaDanToc.Focus();
                    //txtMaDanToc.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboTHX.EditValue = null;
                    cboTHX.Focus();
                    cboTHX.ShowPopup();
                    PopupProcess.SelectFirstRowPopup(cboTHX);
                }
                else
                {
                    cboTHX.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadDataToComboTHX()
        {
            try
            {
                cboTHX.Properties.DataSource = listDataTHX;
                cboTHX.Properties.DisplayMember = "RENDERER_PDC_NAME";
                cboTHX.Properties.ValueMember = "ID";
                cboTHX.Properties.ForceInitialize();
                cboTHX.Properties.Columns.Clear();
                cboTHX.Properties.Columns.Add(new LookUpColumnInfo("SEARCH_CODE", "", 100));
                cboTHX.Properties.Columns.Add(new LookUpColumnInfo("RENDERER_PDC_NAME", "", 400));
                cboTHX.Properties.ShowHeader = true;
                cboTHX.Properties.ImmediatePopup = true;
                cboTHX.Properties.DropDownRows = 20;
                cboTHX.Properties.PopupWidth = 500;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
