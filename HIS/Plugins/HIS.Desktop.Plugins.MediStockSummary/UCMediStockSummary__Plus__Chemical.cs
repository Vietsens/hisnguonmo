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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.MediStockSummary
{
    /// <summary>
    /// vCong60286 - Lua chon "Hoa chat" tai man Ton kho (canh Thuoc / Vat tu / Mau).
    /// - Chi hien khi key HIS.Desktop.Plugins.MediStockSummary.IsShowChemicalOption = 1 (mac dinh tat).
    /// - Hoa chat = loai vat tu co danh dau "La hoa chat" (HIS_MATERIAL_TYPE.IS_CHEMICAL_SUBSTANCE = 1).
    /// - Khi chon Hoa chat: tai ton vat tu nhu nhanh Vat tu, sau do chi giu hoa chat; moi bo loc / to mau /
    ///   xuat Excel / in di chung nhanh Vat tu. Lua chon Vat tu giu nguyen (van gom ca hoa chat).
    /// </summary>
    public partial class UCMediStockSummary : HIS.Desktop.Utility.UserControlBase
    {
        private const string CHEMICAL_LOG_PREFIX = "vCong60286 ";
        /// <summary>1 = hien lua chon "Hoa chat"; khac 1 hoac de trong = tat (mac dinh)</summary>
        private const string CONFIG_KEY__IS_SHOW_CHEMICAL_OPTION = "HIS.Desktop.Plugins.MediStockSummary.IsShowChemicalOption";

        /// <summary>Gia tri key IsShowChemicalOption, doc 1 lan khi Load</summary>
        private bool isShowChemicalOption;

        /// <summary>
        /// Doc key cau hinh va an/hien o "Hoa chat". Goi khi Load, truoc khi khoi phuc trang thai control.
        /// </summary>
        private void InitChemicalOption()
        {
            try
            {
                this.isShowChemicalOption = (HisConfigs.Get<string>(CONFIG_KEY__IS_SHOW_CHEMICAL_OPTION) ?? "").Trim() == "1";
                this.lciChemical.Visibility = this.isShowChemicalOption
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Dang xem rieng hoa chat (key bat + dang chon o "Hoa chat")</summary>
        private bool IsChemicalView()
        {
            return this.isShowChemicalOption && this.chkChemical != null && this.chkChemical.Checked;
        }

        /// <summary>Dang o nhanh vat tu: chon "Vat tu" hoac "Hoa chat" (hoa chat dung chung xu ly cua vat tu)</summary>
        private bool IsMaterialOrChemicalView()
        {
            return this.chkMaterial.Checked || IsChemicalView();
        }

        /// <summary>
        /// Khoi phuc lua chon "Hoa chat" da nho. Key tat ma lan truoc chon Hoa chat -> ve "Vat tu".
        /// Goi khi IsInitForm = true (khong ghi lai trang thai).
        /// </summary>
        private void RestoreChemicalControlState()
        {
            try
            {
                if (this.currentControlStateRDO == null)
                    return;
                var cs = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == chkChemical.Name && o.MODULE_LINK == this.ModuleLink);
                if (cs == null || cs.VALUE != "1")
                    return;
                if (this.isShowChemicalOption)
                    chkChemical.Checked = true;
                else
                    chkMaterial.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkChemical_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!IsInitForm)
                {
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                        ? this.currentControlStateRDO.Where(o => o.KEY == chkChemical.Name && o.MODULE_LINK == this.ModuleLink).FirstOrDefault()
                        : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (chkChemical.Checked ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = chkChemical.Name;
                        csAddOrUpdate.VALUE = (chkChemical.Checked ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = this.ModuleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                }

                if (chkChemical.Checked)
                {
                    ShowUCControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Loai vat tu cua mot dong tren cay. Dong loai (type node) duoc map tu danh muc nen ID = ID loai;
        /// MATERIAL_TYPE_ID co the = 0 voi dong loai.
        /// </summary>
        private static long GetMaterialTypeIdOfNode(HisMaterialInStockSDO node)
        {
            if (node.isTypeNode && node.MATERIAL_TYPE_ID <= 0)
                return node.ID;
            return node.MATERIAL_TYPE_ID;
        }

        /// <summary>
        /// Chi giu cac dong thuoc loai vat tu danh dau "La hoa chat" (dong loai + dong lo),
        /// kem cac dong nhom cha co it nhat 1 hoa chat con; tinh lai so luong dong nhom theo con con lai.
        /// Giu nguyen thu tu danh sach goc.
        /// </summary>
        private List<HisMaterialInStockSDO> FilterChemicalOnly(List<HisMaterialInStockSDO> source)
        {
            if (source == null || source.Count == 0)
                return source;
            try
            {
                var materialTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                HashSet<long> chemicalTypeIds = new HashSet<long>(materialTypes != null
                    ? materialTypes.Where(o => o.IS_CHEMICAL_SUBSTANCE == 1).Select(o => o.ID)
                    : new List<long>());
                if (chemicalTypeIds.Count == 0)
                    return new List<HisMaterialInStockSDO>();

                Dictionary<string, HisMaterialInStockSDO> dicNode = source
                    .Where(o => o != null && o.isTypeNode && !String.IsNullOrEmpty(o.NodeId))
                    .GroupBy(o => o.NodeId)
                    .ToDictionary(g => g.Key, g => g.First());

                HashSet<HisMaterialInStockSDO> keep = new HashSet<HisMaterialInStockSDO>();
                foreach (var item in source)
                {
                    if (item == null)
                        continue;
                    bool isGroupNode = item.isTypeNode && item.IS_LEAF != 1;
                    if (isGroupNode)
                        continue;
                    if (!chemicalTypeIds.Contains(GetMaterialTypeIdOfNode(item)))
                        continue;
                    keep.Add(item);

                    // Them cac dong cha (dong loai cua lo, cac dong nhom) - dung khi gap dong da them
                    string parentNodeId = item.ParentNodeId;
                    int guard = 0;
                    HisMaterialInStockSDO parent;
                    while (!String.IsNullOrEmpty(parentNodeId) && guard++ < 100
                        && dicNode.TryGetValue(parentNodeId, out parent) && keep.Add(parent))
                    {
                        parentNodeId = parent.ParentNodeId;
                    }
                }

                List<HisMaterialInStockSDO> result = source.Where(o => o != null && keep.Contains(o)).ToList();

                // Tinh lai so luong dong nhom theo cac dong con con lai (nhom sau nhat truoc)
                var groupNodes = result.Where(o => o.isTypeNode && o.IS_LEAF != 1 && !String.IsNullOrEmpty(o.NodeId)).ToList();
                if (groupNodes.Count > 0)
                {
                    foreach (var group in groupNodes.OrderByDescending(o => GetNodeDepth(o, dicNode)))
                    {
                        var children = result.Where(o => o.isTypeNode && o.ParentNodeId == group.NodeId).ToList();
                        group.TotalAmount = children.Sum(o => o.TotalAmount);
                        group.AvailableAmount = children.Sum(o => o.AvailableAmount);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(CHEMICAL_LOG_PREFIX + "FilterChemicalOnly: " + source.Count + " -> " + result.Count);
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<HisMaterialInStockSDO>();
            }
        }

        private static int GetNodeDepth(HisMaterialInStockSDO node, Dictionary<string, HisMaterialInStockSDO> dicNode)
        {
            int depth = 0;
            string parentNodeId = node.ParentNodeId;
            HisMaterialInStockSDO parent;
            while (!String.IsNullOrEmpty(parentNodeId) && depth < 100 && dicNode.TryGetValue(parentNodeId, out parent))
            {
                depth++;
                parentNodeId = parent.ParentNodeId;
            }
            return depth;
        }
    }
}
