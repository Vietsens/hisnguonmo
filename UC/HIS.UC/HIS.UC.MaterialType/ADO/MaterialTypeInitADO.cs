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
using HIS.UC.MaterialType.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.MaterialType
{
    public class MaterialTypeInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<MaterialTypeColumn> MaterialTypeColumns { get; set; }
        public List<V_HIS_BID> listBids { get; set; }
        public List<HIS_MEDICAL_CONTRACT> listContracts { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> MaterialTypes { get; set; }
        public List<MaterialTypeADO> MaterialTypeADOs { get; set; }
        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public bool? IsShowExportExcel { get; set; }
        public bool? IsShowImport { get; set; }
        public bool? IsShowBid { get; set; }
        public bool? IsShowContract { get; set; }
        public bool? IsShowChkLock { get; set; }
        public bool? IsHightLightFilter { get; set; }
        public bool? IsCheckGroupByMap { get; set; }
        public MaterialType_NodeCellStyle MaterialTypeNodeCellStyle { get; set; }
        public MaterialTypeHandler MaterialTypeClick { get; set; }
        public MaterialTypeHandler MaterialTypeDoubleClick { get; set; }
        public MaterialTypeHandler MaterialTypeRowEnter { get; set; }
        public MaterialType_GetStateImage MaterialType_GetStateImage { get; set; }
        public MaterialType_GetSelectImage MaterialType_GetSelectImage { get; set; }
        public MaterialTypeHandler MaterialType_StateImageClick { get; set; }
        public MaterialTypeHandler MaterialType_SelectImageClick { get; set; }
        public MaterialType_CustomUnboundColumnData MaterialType_CustomUnboundColumnData { get; set; }
        public MaterialType_AfterCheck MaterialType_AfterCheck { get; set; }
        public MaterialType_BeforeCheck MaterialType_BeforeCheck { get; set; }
        public MaterialType_CheckAllNode MaterialType_CheckAllNode { get; set; }
        public MaterialType_CustomDrawNodeCell MaterialType_CustomDrawNodeCell { get; set; }
        public MaterialType_NewClick MaterialType_NewClick { get; set; }
        public MaterialType_RefeshData MaterialType_RefeshData { get; set; }
        public MaterialType_ExportExcel MaterialType_ExportExcel { get; set; }
        public MaterialType_Import MaterialType_Import { get; set; }
        public MaterialType_Save MaterialType_Save { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }
        public cboBid_EditValueChanged cboBid_EditValueChanged { get; set; }

        public cboContract_EditValueChanged cboContract_EditValueChanged { get; set; }

        public ChkLock_CheckChange chkLock_CheckChange { get; set; }
        public MaterialType_PrintPriceList MaterialType_PrintPriceList { get; set; }

        public MaterialTypeHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public ChkLock_CheckChange chkGroupByMap_CheckChanged { get; set; }

        public bool? IsCreateParentNodeWithMaterialTypeExpend { get; set; }

        public string LayoutMaterialTypeExpend { get; set; }
        public string Keyword_NullValuePrompt { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public cboIsReusable_EditValueChanged cboIsReusable_EditValueChanged { get; set; }
    }
}
