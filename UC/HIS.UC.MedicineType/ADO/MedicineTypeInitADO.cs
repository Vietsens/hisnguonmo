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
using HIS.UC.MedicineType.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.MedicineType
{
    public class MedicineTypeInitADO 
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<MedicineTypeColumn> MedicineTypeColumns { get; set; }
        public List<V_HIS_BID> listBids { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> MedicineTypes { get; set; }
        public List<MedicineTypeADO> MedicineTypeADOs { get; set; }

        public List<HIS_MEDICAL_CONTRACT> listContracts { get; set; }

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public bool? IsShowRadioThieuThongTinBHYT { get; set; }
        public bool? IsExportExcel { get; set; }
        public bool? IsShowImport { get; set; }
        public bool? IsShowBid { get; set; }
        public bool? IsShowChkLock { get; set; }
        public bool? IsHightLightFilter { get; set; }
        public bool? IsShowContract { get; set; }
        public MedicineType_NodeCellStyle MedicineTypeNodeCellStyle { get; set; }
        public MedicineTypeHandler MedicineTypeClick { get; set; }
        public MedicineTypeHandler MedicineTypeDoubleClick { get; set; }
        public MedicineTypeHandler MedicineTypeRowEnter { get; set; }
        public MedicineType_GetStateImage MedicineType_GetStateImage { get; set; }
        public MedicineType_GetSelectImage MedicineType_GetSelectImage { get; set; }
        public MedicineTypeHandler MedicineType_StateImageClick { get; set; }
        public MedicineTypeHandler MedicineType_SelectImageClick { get; set; }
        public MedicineType_CustomUnboundColumnData MedicineType_CustomUnboundColumnData { get; set; }
        public MedicineType_AfterCheck MedicineType_AfterCheck { get; set; }
        public MedicineType_BeforeCheck MedicineType_BeforeCheck { get; set; }
        public MedicineType_CheckAllNode MedicineType_CheckAllNode { get; set; }
        public MedicineType_CustomDrawNodeCell MedicineType_CustomDrawNodeCell { get; set; }
        public CheckThieuThongTinBHYT_CheckChange checkThieuThongTinBHYT_CheckChange { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }
        public MedicineType_ExportExcel MedicineType_ExportExcel { get; set; }
        public MedicineType_Import MedicineType_Import { get; set; }
        public MedicineType_Save MedicineType_Save { get; set; }
        public CboBid_EditValueChanged cboBid_EditValueChanged { get; set; }
        public MedicineType_PrintPriceList MedicineType_PrintPriceList { get; set; }
        public CboContract_EditValueChanged cboContract_EditValueChanged { get; set; }
        


        public MedicineTypeHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithMedicineTypeExpend { get; set; }

        public string LayoutMedicineTypeExpend { get; set; }
        public string Keyword_NullValuePrompt { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public ChkLock_CheckChange chkLock_CheckChange { get; set; }
    }
}
