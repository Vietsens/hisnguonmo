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

        public void MappingMIMS(DrugItem drug)
        {
            MappingMIMS(new List<DrugItem> { drug });
        }

        public void MappingMIMS(List<DrugItem> drugs)
        {
            try
            {
                if (drugs == null || drugs.Count == 0) return;
                foreach (var drug in drugs)
                {
                    if (drug == null || drug.HisDrugCode == null) continue;
                    var med = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.MEDICINE_TYPE_CODE == drug.HisDrugCode);
                    if (med == null) continue;
                    drug.MimsGuid = med.MIMS_GUID;
                    drug.Name = med.MEDICINE_TYPE_NAME;
                    switch (med.MIMS_TYPE)
                    {
                        case 1:
                            drug.DrugType = MimsType.GGPI;
                            break;
                        case 2:
                            drug.DrugType = MimsType.Product;
                            break;
                        case 3:
                            drug.DrugType = MimsType.GenericItem;
                            break;
                        case 4:
                            drug.DrugType = MimsType.Molecule;
                            break;
                        case 5:
                            drug.DrugType = MimsType.SubstanceClass;
                            break;
                        default:
                            drug.DrugType = MimsType.GGPI;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
