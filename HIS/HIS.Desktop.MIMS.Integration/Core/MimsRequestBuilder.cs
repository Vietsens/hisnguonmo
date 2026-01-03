// ...existing code...
using System.Collections.Generic;
using System.Text;
using HIS.Desktop.MIMS.Integration.Models;

namespace HIS.Desktop.MIMS.Integration.Core
{
	public class MimsRequestBuilder
	{
        public static string BuildDrugDrugInteractionRequest(
            List<DrugItem> currentDrugs,
            List<DrugItem> previousDrugs)
        {
            var sb = new StringBuilder();

            sb.Append("<Request><Interaction><Prescribing>");

            foreach (var drug in currentDrugs)
                sb.Append(BuildDrugTag(drug));

            sb.Append("</Prescribing>");

            if (previousDrugs != null && previousDrugs.Count > 0)
            {
                sb.Append("<Prescribed>");
                foreach (var drug in previousDrugs)
                    sb.Append(BuildDrugTag(drug));
                sb.Append("</Prescribed>");
            }
            sb.Append("<References/></Interaction></Request>");

            return sb.ToString();
        }

        /// <summary>
        /// Yêu cầu kiểm tra trùng lặp thuốc.
        /// Ở đây tái sử dụng cấu trúc tương tự Drug-Drug Interaction nhưng
        /// chỉ truyền 1 danh sách thuốc hiện tại, không có thuốc lịch sử.
        /// </summary>
        public static string BuildDrugInteractionRequest(List<DrugItem> drugs)
        {
            return BuildDrugDrugInteractionRequest(drugs, new List<DrugItem>());
        }

        private static string BuildDrugTag(DrugItem drug)
        {
			switch (drug.DrugType)
			{
				case MimsDrugType.Product:
					return string.Format("<Product reference=\"{{{0}}}\" />", drug.MimsGuid);
				case MimsDrugType.GGPI:
					return string.Format("<GGPI reference=\"{{{0}}}\" />", drug.MimsGuid);
				default:
					return string.Format("<GenericItem reference=\"{{{0}}}\" />", drug.MimsGuid);
			}
        }

        public static string BuildDrugAllergyRequest(
            List<DrugItem> drugs,
            List<AllergyItem> allergies)
        {
            var sb = new StringBuilder();

            sb.Append("<Request><Interaction><Prescribing>");

            drugs.ForEach(d => sb.Append(BuildDrugTag(d)));

            sb.Append("</Prescribing><Allergies>");

			foreach (var al in allergies)
			{
				if (al.Type == AllergyType.Molecule)
					sb.Append(string.Format("<Molecule reference=\"{{{0}}}\" />", al.MimsGuid));
                else if (al.Type == AllergyType.SubstanceClass)
					sb.Append(string.Format("<SubstanceClass reference=\"{{{0}}}\" />", al.MimsGuid));
			}

            sb.Append("</Allergies><References/></Interaction></Request>");

            return sb.ToString();
        }

        public static string BuildDrugHealthAlertRequest(
            List<DrugItem> drugs,
            List<string> icd10Codes)
        {
            var sb = new StringBuilder();

            sb.Append("<Request><Interaction><Prescribing>");

            if (drugs != null)
            {
                foreach (var d in drugs)
                {
                    sb.Append(BuildDrugTag(d));
                }
            }

            sb.Append("</Prescribing>");

            if (icd10Codes != null && icd10Codes.Count > 0)
            {
                sb.Append("<HealthIssueCodes>");
                foreach (var code in icd10Codes)
                {
                    if (string.IsNullOrWhiteSpace(code))
                        continue;

                    sb.Append(string.Format("<HealthIssueCode code=\"{0}\" codeType=\"ICD10\" />", code.Trim()));
                }
                sb.Append("</HealthIssueCodes>");
            }

            sb.Append("<References/></Interaction></Request>");

            return sb.ToString();
        }

        public static string BuildVnContraindicationRequest(
            List<string> hisDrugCodes)
        {
            var sb = new StringBuilder();

            sb.Append("<Request><Interaction><Prescribing>");

			foreach (var code in hisDrugCodes)
				sb.Append(string.Format("<ItemCode=\"{0}\" />", code));

            sb.Append("</Prescribing></Interaction></Request>");

            return sb.ToString();
        }
	}
}
