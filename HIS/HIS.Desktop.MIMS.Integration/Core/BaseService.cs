using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.View;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.MIMS.Integration.Core
{
    public class BaseService
    {
        public string NameText { get; set; }

        public static string BuildSimpleHtml(string message)
        {
            string safe = System.Security.SecurityElement.Escape(message ?? string.Empty);
            return "<html><head><meta charset=\"utf-8\"/></head><body><h3>" + safe + "</h3></body></html>";
        }

        public List<DrugItem> MappingMIMS(DrugItem drug)
        {
            return MappingMIMS(new List<DrugItem> { drug });
        }

        public List<DrugItem> MappingMIMS(List<DrugItem> drugs)
        {
            var result = new List<DrugItem>();
            try
            {
                if (drugs == null || drugs.Count == 0) return result;

                var allMedType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                var allAtc = BackendDataWorker.Get<HIS_ATC>();
                var allAcin = BackendDataWorker.Get<HIS_MEDICINE_TYPE_ACIN>();
                var allAcIng = BackendDataWorker.Get<HIS_ACTIVE_INGREDIENT>();

                var seenGuids = new HashSet<string>();

                foreach (var drug in drugs)
                {
                    if (drug == null || drug.HisDrugCode == null) continue;

                    var med = allMedType.FirstOrDefault(o => o.MEDICINE_TYPE_CODE == drug.HisDrugCode);
                    if (med == null) continue;

                    // Expand via ATC codes
                    if (!string.IsNullOrEmpty(med.ATC_CODES))
                    {
                        var atcCodes = med.ATC_CODES.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var raw in atcCodes)
                        {
                            var atcCode = raw.Trim();
                            if (string.IsNullOrEmpty(atcCode)) continue;
                            var atcRow = allAtc.FirstOrDefault(o => o.ATC_CODE == atcCode && o.IS_MIMS_MAPPED == 1);
                            if (atcRow == null || string.IsNullOrEmpty(atcRow.MIMS_GUID)) continue;
                            if (!seenGuids.Add(atcRow.MIMS_GUID)) continue;
                            result.Add(new DrugItem
                            {
                                HisDrugCode = drug.HisDrugCode,
                                Name = atcRow.MIMS_NAME ?? med.MEDICINE_TYPE_NAME,
                                MimsGuid = atcRow.MIMS_GUID,
                                DrugType = ConvertToMimsType(atcRow.MIMS_TYPE)
                            });
                        }
                    }

                    // Expand via active ingredients
                    var acinRows = allAcin.Where(o => o.MEDICINE_TYPE_ID == med.ID).ToList();
                    foreach (var acin in acinRows)
                    {
                        var acIng = allAcIng.FirstOrDefault(o => o.ID == acin.ACTIVE_INGREDIENT_ID && o.IS_MIMS_MAPPED == 1);
                        if (acIng == null || string.IsNullOrEmpty(acIng.MIMS_GUID)) continue;
                        if (!seenGuids.Add(acIng.MIMS_GUID)) continue;
                        result.Add(new DrugItem
                        {
                            HisDrugCode = drug.HisDrugCode,
                            Name = acIng.MIMS_NAME ?? med.MEDICINE_TYPE_NAME,
                            MimsGuid = acIng.MIMS_GUID,
                            DrugType = ConvertToMimsType(acIng.MIMS_TYPE)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private MimsType ConvertToMimsType(short? mimsType)
        {
            switch (mimsType)
            {
                case 1: return MimsType.GGPI;
                case 2: return MimsType.Product;
                case 3: return MimsType.GenericItem;
                case 4: return MimsType.Molecule;
                case 5: return MimsType.SubstanceClass;
                default: return MimsType.GGPI;
            }
        }

        public void ShowResult(MimsResult result)
        {
            if (result != null && !string.IsNullOrEmpty(result.Html))
            {
                WebViewHelper.ShowHtml(result.Html, NameText);
            }
        }
    }
}
