using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.UC
{
    /// <summary>
    /// Helper: gen dong nhieu ucDiseaseDetailGroup cho 1 PARENT_TYPE,
    /// add vao XtraScrollableControl target.
    /// Group theo DISEASE_TYPE_ID, sap xep theo NUM_ORDER_TYPE.
    /// </summary>
    public static class ucDiseaseDetailContainer
    {
        /// <summary>
        /// Gen dong cac nhom benh cho 1 PARENT_TYPE va add vao container.
        /// </summary>
        /// <param name="container">XtraScrollableControl target (VD: xtraScrollableControl2)</param>
        /// <param name="allDetails">Toan bo V_HIS_DISEASE_DETAIL da load tu API</param>
        /// <param name="parentType">PARENT_TYPE can gen (1, 2, 3, 4, 5)</param>
        /// <param name="checkMapping">Output: mapping DISEASE_DETAIL_ID -> CheckEdit</param>
        /// <param name="textMapping">Output: mapping DISEASE_DETAIL_ID -> TextEdit</param>
        public static void Generate(
            XtraScrollableControl container,
            List<V_HIS_DISEASE_DETAIL> allDetails,
            long parentType,
            Dictionary<long, CheckEdit> checkMapping,
            Dictionary<long, Control> textMapping)
        {
            try
            {
                if (container == null || allDetails == null) return;

                container.SuspendLayout();

                // Clear existing dynamic controls
                ClearDynamicControls(container);

                // Filter + group by DISEASE_TYPE_ID, sort by NUM_ORDER_TYPE
                var typeGroups = allDetails
                    .Where(d => d.PARENT_TYPE == parentType)
                    .GroupBy(d => d.DISEASE_TYPE_ID)
                    .OrderByDescending(g => g.Min(d => d.NUM_ORDER_TYPE ?? 0))
                    .ToList();

                foreach (var group in typeGroups)
                {
                    string typeName = group.First().DISEASE_TYPE_NAME ?? "";
                    var groupDetails = group.OrderBy(d => d.NUM_ORDER_DETAIL).ToList();

                    var uc = new ucDiseaseDetailGroup(typeName, groupDetails);
                    uc.Dock = DockStyle.Top;
                    uc.Tag = "DiseaseGroup_" + group.Key;
                    container.Controls.Add(uc);

                    // Merge mappings
                    foreach (var kv in uc.CheckMapping)
                    {
                        checkMapping[kv.Key] = kv.Value;
                    }
                    foreach (var kv in uc.TextMapping)
                    {
                        textMapping[kv.Key] = kv.Value;
                    }
                }

                container.ResumeLayout(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load ket qua da luu vao cac UC.
        /// </summary>
        public static void LoadResults(
            XtraScrollableControl container,
            List<HIS_DISEASE_DETAIL_RESULT> results)
        {
            try
            {
                if (container == null || results == null) return;
                foreach (Control ctrl in container.Controls)
                {
                    var uc = ctrl as ucDiseaseDetailGroup;
                    if (uc != null)
                        uc.LoadResults(results);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thu thap ket qua tu UI de luu.
        /// KSK_GENERAL_ID truyen tu form cha de dam bao du FK khi INSERT.
        /// </summary>
        public static List<ADO.DiseaseDetailResultADO> CollectResults(
            XtraScrollableControl container,
            long? kskGeneralId = null)
        {
            var rows = new List<ADO.DiseaseDetailResultADO>();
            try
            {
                if (container == null) return rows;
                foreach (Control ctrl in container.Controls)
                {
                    var uc = ctrl as ucDiseaseDetailGroup;
                    if (uc != null)
                        rows.AddRange(uc.CollectResults(kskGeneralId));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rows;
        }

        /// <summary>
        /// Reset toan bo checkboxes va textboxes.
        /// </summary>
        public static void ResetAll(XtraScrollableControl container)
        {
            try
            {
                if (container == null) return;
                foreach (Control ctrl in container.Controls)
                {
                    var uc = ctrl as ucDiseaseDetailGroup;
                    if (uc != null)
                        uc.ResetAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void ClearDynamicControls(XtraScrollableControl container)
        {
            try
            {
                var toRemove = new List<Control>();
                foreach (Control ctrl in container.Controls)
                {
                    if (ctrl is ucDiseaseDetailGroup)
                        toRemove.Add(ctrl);
                }
                foreach (var ctrl in toRemove)
                {
                    container.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
