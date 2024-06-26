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
using DevExpress.XtraGrid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisHoldReturn.ADO;
using HIS.Desktop.Plugins.HisHoldReturn.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHoldReturn.HoldReturn
{
    public partial class UCHoldReturn : UserControlBase
    {
        private void InitComboBanGiao(DevExpress.XtraEditors.GridLookUpEdit cboBanGiao)
        {
            try
            {
                var data = new BanGiaoADO().BanGiaoADOs;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Text", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Text", "Value", columnInfos, false, 150);
                ControlEditorLoader.Load(cboBanGiao, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitGridHoldDhyt()
        {
            var docHoldTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DOC_HOLD_TYPE>();
            docHoldTypes = (docHoldTypes != null && docHoldTypes.Count > 0) ?
                       docHoldTypes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                           )
                           .ToList()
                   : docHoldTypes;
            gridControlDocType.DataSource = docHoldTypes;
        }

        void FillDataToControlHR(HIS_TREATMENT treatment, List<HIS_DOC_HOLD_TYPE> docHoldTypeSelecteds)
        {
            if (treatment != null && docHoldTypeSelecteds != null && docHoldTypeSelecteds.Count > 0)
            {
                this.txtPatientCodeForAdd.Text = treatment.TDL_PATIENT_CODE;
                this.lblPatientName.Text = treatment.TDL_PATIENT_NAME;
                this.lblGenderName.Text = treatment.TDL_PATIENT_GENDER_NAME;
                this.lblPatientAddress.Text = treatment.TDL_PATIENT_ADDRESS;
                this.lblHeinCardNumber.Text = treatment.TDL_HEIN_CARD_NUMBER;
                this.lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB);

                gridControlDocType.BeginUpdate();
                if (docHoldTypeSelecteds != null && docHoldTypeSelecteds.Count > 0)
                {
                    foreach (var item in docHoldTypeSelecteds)
                    {
                        int rowHandle = gridViewDocType.LocateByValue("ID", item.ID);
                        if (rowHandle != GridControl.InvalidRowHandle)
                            gridViewDocType.SelectRow(rowHandle);
                    }
                }
                gridControlDocType.EndUpdate();
            }            
        }

        private void InitComboHoldDhty(DevExpress.XtraEditors.GridLookUpEdit cboDocHoldType)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_DOC_HOLD_TYPE> docHoldType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DOC_HOLD_TYPE>();

                docHoldType = (docHoldType != null && docHoldType.Count > 0) ?
                    docHoldType.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        )
                        .ToList()
                : docHoldType;

                // order tăng dần theo num_order
                if (docHoldType != null && docHoldType.Count > 0)
                {
                    cboDocHoldType.Properties.DataSource = docHoldType;
                }
                else
                {
                    cboDocHoldType.Properties.DataSource = null;
                }

                cboDocHoldType.Properties.DisplayMember = "DOC_HOLD_TYPE_NAME";
                cboDocHoldType.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboDocHoldType.Properties.View.Columns.AddField("DOC_HOLD_TYPE_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboDocHoldType.Properties.View.Columns.AddField("DOC_HOLD_TYPE_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "Tên";
                cboDocHoldType.Properties.PopupFormWidth = 320;
                cboDocHoldType.Properties.View.OptionsView.ShowColumnHeaders = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
