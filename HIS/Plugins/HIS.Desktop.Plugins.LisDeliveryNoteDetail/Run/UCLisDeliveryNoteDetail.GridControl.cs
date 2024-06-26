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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.LisDeliveryNoteDetail.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.LisDeliveryNoteDetail
{
    public partial class UCLisDeliveryNoteDetail
    {
        private void FillDataToGridControl_Sample()
        {
            try
            {
                WaitingManager.Show();
                this.rootListSample = new List<SampleADO>();
                CommonParam paramCommon = new CommonParam();
                Inventec.Core.ApiResultObject<List<LIS.EFMODEL.DataModels.V_LIS_SAMPLE>> apiResult = null;
                LisSampleViewFilter filter = new LisSampleViewFilter();
                filter.DELIVERY_NOTE_ID = this.deliveryNote.ID;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "ID";
                gridViewSample.BeginUpdate();
                gridControlSample.DataSource = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<V_LIS_SAMPLE>)apiResult.Data;
                    if (data != null)
                    {
                        
                        foreach (var item in data)
                        {
                            this.rootListSample.Add(new SampleADO(item));
                        }
                        this.currentListSample = this.rootListSample;
                        gridControlSample.DataSource = this.currentListSample;

                        statecheckColumn = StateCheckBox.Unchecked;
                        SetGridColumn_Select_Image(statecheckColumn);
                        GridCheckChange(false);
                    }
                }
                gridControlSample.RefreshDataSource();
                gridViewSample.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FilldataToGridControl_Sample_Searching()
        {
            try
            {
                gridViewSample.BeginUpdate();
                gridControlSample.DataSource = null;

                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());
                var query = this.rootListSample.AsQueryable();
                query = query.Where(o => (o.BARCODE != null ? o.BARCODE.ToLower().Contains(keyword) : false)
                        || (o.VIR_PATIENT_NAME != null ? Inventec.Common.String.Convert.UnSignVNese(o.VIR_PATIENT_NAME.ToLower()).Contains(keyword) : false)
                        || (o.DOB_ForDisplay != null ? Inventec.Common.String.Convert.UnSignVNese(o.DOB_ForDisplay.ToLower()).Contains(keyword) : false)
                        || (o.GENDER_NAME != null ? Inventec.Common.String.Convert.UnSignVNese(o.GENDER_NAME.ToLower()).Contains(keyword) : false)
                        || (o.PHONE_NUMBER != null ? o.PHONE_NUMBER.ToLower().Contains(keyword) : false)
                        || (o.PersonalIDNumber_ForDisplay != null ? o.PersonalIDNumber_ForDisplay.ToLower().Contains(keyword) : false)
                        || (o.REJECT_REASON != null ? Inventec.Common.String.Convert.UnSignVNese(o.REJECT_REASON.ToLower()).Contains(keyword) : false)
                        || (o.SAMPLE_TYPE_NAME != null ? Inventec.Common.String.Convert.UnSignVNese(o.SAMPLE_TYPE_NAME.ToLower()).Contains(keyword) : false)
                        || (o.SAMPLE_LOGINNAME_USERNAME_ForDisplay != null ? Inventec.Common.String.Convert.UnSignVNese(o.SAMPLE_LOGINNAME_USERNAME_ForDisplay.ToLower()).Contains(keyword) : false)
                        || (o.COMMUNE_NAME != null ? Inventec.Common.String.Convert.UnSignVNese(o.COMMUNE_NAME.ToLower()).Contains(keyword) : false)
                        || (o.DISTRICT_NAME != null ? Inventec.Common.String.Convert.UnSignVNese(o.DISTRICT_NAME.ToLower()).Contains(keyword) : false)
                        || (o.PROVINCE_NAME != null ? Inventec.Common.String.Convert.UnSignVNese(o.PROVINCE_NAME.ToLower()).Contains(keyword) : false)
                        || (o.SAMPLE_TIME_ForDisplay != null ? o.SAMPLE_TIME_ForDisplay.ToLower().Contains(keyword) : false)
                        || (o.SPECIMEN_ORDER != null ? o.SPECIMEN_ORDER.ToString().ToLower().Contains(keyword) : false));

                this.currentListSample = query.OrderBy(o => o.ID).ToList();
                gridControlSample.DataSource = this.currentListSample;
                gridControlSample.RefreshDataSource();
                gridViewSample.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
