// ...existing code...
using System.Collections.Generic;
using System.Text;
using HIS.Desktop.MIMS.Integration.Models;

namespace HIS.Desktop.MIMS.Integration.Core
{
	public class MimsRequestBuilder
	{
        private static string BuildDrugTag(DrugItem drug)
        {
            if (drug == null || string.IsNullOrEmpty(drug.MimsGuid))
                return string.Empty;

			switch (drug.DrugType)
			{
				case MimsType.Product:
					return string.Format("<Product reference=\"{{{0}}}\" />", drug.MimsGuid);
				case MimsType.GGPI:
					return string.Format("<GGPI reference=\"{{{0}}}\" />", drug.MimsGuid);
				default:
					return string.Format("<GenericItem reference=\"{{{0}}}\" />", drug.MimsGuid);
			}
        }

        public static string BuildDrugInformationRequest(DrugItem drug)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<Request>");
            sb.AppendLine("<Content>");

            sb.AppendLine(BuildDrugTag(drug));

            sb.AppendLine("<References/>");
            sb.AppendLine("</Content>");
            sb.Append("</Request>");

            return sb.ToString();
        }

        public static string BuildDrugDrugInteractionRequest(
            List<DrugItem> currentDrugs,
            List<DrugItem> previousDrugs, bool checkDuplicateDrug = false)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<Request>");
            sb.AppendLine("<Interaction>");
            sb.AppendLine("<Prescribing>");

            foreach (var drug in currentDrugs)
                sb.AppendLine(BuildDrugTag(drug));

            sb.AppendLine("</Prescribing>");

            if (previousDrugs != null && previousDrugs.Count > 0)
            {
                sb.AppendLine("<Prescribed>");
                foreach (var drug in previousDrugs)
                    sb.AppendLine(BuildDrugTag(drug));
                sb.AppendLine("</Prescribed>");
            }
            sb.AppendLine("<References/>");
            if (checkDuplicateDrug)
            {
                sb.AppendLine("<DuplicateTherapy checkSameDrug=\"true\"/>");
                sb.AppendLine("<DuplicateIngredient checkSameDrug=\"true\"/>");
            }
            sb.AppendLine("</Interaction>");
            sb.Append("</Request>");
            return sb.ToString();
        }

        /// <summary>
        /// Yêu cầu kiểm tra trùng lặp thuốc.
        /// Ở đây tái sử dụng cấu trúc tương tự Drug-Drug Interaction nhưng
        /// chỉ truyền 1 danh sách thuốc hiện tại, không có thuốc lịch sử.
        /// </summary>
        public static string BuildDrugDrugInteractionRequest(List<DrugItem> drugs, bool checkDuplicateDrug = false)
        {
            return BuildDrugDrugInteractionRequest(drugs, new List<DrugItem>(), checkDuplicateDrug);
        }

        public static string BuildDrugAllergyRequest(
            List<DrugItem> drugs,
            List<AllergyItem> allergies)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<Request>");
            sb.AppendLine("<Interaction>");
            sb.AppendLine("<Prescribing>");

            drugs.ForEach(d => sb.AppendLine(BuildDrugTag(d)));

            sb.AppendLine("</Prescribing>");

            AddAllergies(allergies, ref sb);

            sb.AppendLine("<References/>");
            sb.AppendLine("</Interaction>");
            sb.Append("</Request>");

            return sb.ToString();
        }

        public static string BuildDrugHealthAlertRequest(
            List<DrugItem> drugs, List<AllergyItem> allergies,
            List<string> icd10Codes, bool checkDuplicateDrug = false, bool checkAllergy = false)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<Request>");
            sb.AppendLine("<Interaction>");
            sb.AppendLine("<Prescribing>");

            if (drugs != null)
            {
                foreach (var d in drugs)
                {
                    sb.AppendLine(BuildDrugTag(d));
                }
            }

            sb.AppendLine("</Prescribing>");

            if (icd10Codes != null && icd10Codes.Count > 0)
            {
                sb.AppendLine("<HealthIssueCodes>");
                foreach (var code in icd10Codes)
                {
                    if (string.IsNullOrWhiteSpace(code))
                        continue;

                    sb.AppendLine(string.Format("<HealthIssueCode code=\"{0}\" codeType=\"ICD10\" />", code.Trim()));
                }
                sb.AppendLine("</HealthIssueCodes>");
            }
            if (checkAllergy) AddAllergies(allergies, ref sb);

            sb.AppendLine("<References/>");
            if (checkDuplicateDrug)
            {
                sb.AppendLine("<DuplicateTherapy checkSameDrug=\"true\"/>");
                sb.AppendLine("<DuplicateIngredient checkSameDrug=\"true\"/>");
            }
            sb.AppendLine("</Interaction>");
            sb.Append("</Request>");

            return sb.ToString();
        }

        private static void AddAllergies(List<AllergyItem> allergies,ref StringBuilder sb)
        {
            if (allergies == null || allergies.Count == 0)
            {
                sb.AppendLine("<Allergies/>");
                return;
            }
            sb.AppendLine("<Allergies>");
            foreach (var al in allergies)
            {
                switch (al.Type)
                {
                    case MimsType.GGPI:
                        sb.AppendLine(string.Format("<GGPI reference=\"{{{0}}}\" />", al.MimsGuid));
                        break;
                    case MimsType.Product:
                        sb.AppendLine(string.Format("<Product reference=\"{{{0}}}\" />", al.MimsGuid));
                        break;
                    case MimsType.GenericItem:
                        sb.AppendLine(string.Format("<GenericItem reference=\"{{{0}}}\" />", al.MimsGuid));
                        break;
                    case MimsType.Molecule:
                        sb.AppendLine(string.Format("<Molecule reference=\"{{{0}}}\" />", al.MimsGuid));
                        break;
                    case MimsType.SubstanceClass:
                        sb.AppendLine(string.Format("<SubstanceClass reference=\"{{{0}}}\" />", al.MimsGuid));
                        break;
                    default:
                        break;
                }
            }
            sb.AppendLine("</Allergies>");
        }

        public static string BuildVnContraindicationRequest(
            List<string> hisDrugCodes)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<Request>");
            sb.AppendLine("<Interaction>");
            sb.AppendLine("<Prescribing>");

            foreach (var code in hisDrugCodes)
                sb.AppendLine(string.Format("<ItemCode=\"{0}\" />", code));

            sb.AppendLine("</Prescribing>");
            sb.AppendLine("</Interaction>");
            sb.Append("</Request>");

            return sb.ToString();
        }
	}
}
