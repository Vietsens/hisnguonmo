using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HIS.Desktop.MIMS.Integration.Models;

namespace HIS.Desktop.MIMS.Integration.Core
{
    /// <summary>
    /// Helper parse các XML trả về từ MIMS sang các model chi tiết phục vụ FE.
    /// </summary>
    public static class MimsResultDetailParser
    {
        /// <summary>
        /// Parse kết quả VN Contraindication Alert (Result/Interaction/DANH_SACH_TUONG_TAC/CAP_TUONG_TAC...).
        /// </summary>
        public static List<VnContraindicationInteraction> ParseVnContraindicationInteractions(string xml)
        {
            if (string.IsNullOrEmpty(xml) || xml.Trim().Length == 0)
                return null;

            try
            {
                var doc = XDocument.Parse(xml);
                var root = doc.Root;
                if (root == null) return null;
                var interaction = root.Element("Interaction");
                if (interaction == null) return null;
                var danhSach = interaction.Element("DANH_SACH_TUONG_TAC");
                if (danhSach == null) return null;
                var caps = danhSach.Elements("CAP_TUONG_TAC");
                if (caps == null) return null;

                var list = new List<VnContraindicationInteraction>();
                foreach (var x in caps)
                {
                    var item = new VnContraindicationInteraction
                    {
                        PairName = (string)x.Element("CapTuongTac"),
                        Drug1 = (string)x.Element("HoatChat_1"),
                        Drug2 = (string)x.Element("HoatChat_2"),
                        InteractionLevel = (string)x.Element("MucDoNghiemTrong"),
                        ClinicalConsequence = (string)x.Element("HauQuaCuaTuongTac"),
                        Mechanism = (string)x.Element("CoCheTuongTac"),
                        Management = (string)x.Element("XuTriTuongTac"),
                        Reference = (string)x.Element("TaiLieuThamKhao"),
                        Disclaimer = (string)x.Element("TuyenBoMienTruTrachNhiem")
                    };
                    if (!string.IsNullOrEmpty(item.PairName) && item.PairName.Trim().Length > 0)
                        list.Add(item);
                }
                return list.Count > 0 ? list : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Parse kết quả CDS DrugDrug Alert (MIMS DRUG-DRUG Alert.xml).
        /// Hỗ trợ nhiều ClassInteraction nếu có.
        /// </summary>
        public static List<DrugDrugAlertDetail> ParseDrugDrugAlerts(string xml)
        {
            if (string.IsNullOrEmpty(xml) || xml.Trim().Length == 0)
                return null;

            try
            {
                var doc = XDocument.Parse(xml);
                var root = doc.Root;
                if (root == null) return null;
                var interaction = root.Element("Interaction");
                if (interaction == null) return null;

                XElement primaryGgpi = null, secondaryGgpi = null;
                foreach (var e in interaction.Elements("GGPI"))
                {
                    var attr = e.Attribute("rejected");
                    if (attr == null && primaryGgpi == null)
                        primaryGgpi = e;
                    if (attr != null && secondaryGgpi == null)
                        secondaryGgpi = e;
                }

                string primaryDrugName = primaryGgpi != null ? (string)primaryGgpi.Attribute("name") : null;
                string primaryDrugRef = primaryGgpi != null ? (string)primaryGgpi.Attribute("reference") : null;
                string secondaryDrugRef = secondaryGgpi != null ? (string)secondaryGgpi.Attribute("reference") : null;

                var classInteractions = new List<XElement>();
                foreach (var ci in interaction.Descendants("ClassInteraction"))
                    classInteractions.Add(ci);
                if (classInteractions.Count == 0)
                    return null;

                var result = new List<DrugDrugAlertDetail>();

                foreach (var ci in classInteractions)
                {
                    var prescribingClass = ci.Element("PrescribingInteractionClass");
                    var interactionClass = ci.Element("InteractionClass");
                    var molecule = interactionClass != null ? interactionClass.Element("Molecule") : null;

                    var detail = new DrugDrugAlertDetail();
                    detail.PrimaryDrugName = primaryDrugName;
                    detail.PrimaryDrugReference = primaryDrugRef;
                    detail.InteractingDrugName = molecule != null ? (string)molecule.Attribute("name") : null;
                    detail.InteractingDrugReference = (molecule != null && molecule.Attribute("reference") != null) ? (string)molecule.Attribute("reference") : secondaryDrugRef;
                    detail.PrescribingClassName = prescribingClass != null ? (string)prescribingClass.Attribute("name") : null;
                    detail.InteractingClassName = interactionClass != null ? (string)interactionClass.Attribute("name") : null;

                    var severityText = (string)ci.Element("Severity");
                    detail.Severity = severityText;
                    detail.SeverityLevel = ParseDrugInteractionSeverity(severityText);

                    detail.Likelihood = (string)ci.Element("Likelihood");
                    detail.Documentation = (string)ci.Element("Documentation");
                    detail.ProfessionalText = (ci.Element("Interaction") != null && ci.Element("Interaction").Element("Professional") != null) ? (string)ci.Element("Interaction").Element("Professional") : null;

                    foreach (var p in ci.Elements("Precaution"))
                    {
                        var professionalElem = p.Element("Professional");
                        var text = professionalElem != null ? (string)professionalElem : null;
                        if (!string.IsNullOrEmpty(text) && text.Trim().Length > 0)
                        {
                            detail.Precautions.Add(text);
                        }
                    }

                    result.Add(detail);
                }

                return result.Count > 0 ? result : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Parse kết quả Drug-Allergy Alert (Result/Interaction/GGPI/Product/GenericItem/Allergy...).
        /// </summary>
        public static List<DrugAllergyAlertDetail> ParseDrugAllergyAlerts(string xml)
        {
            if (string.IsNullOrEmpty(xml) || xml.Trim().Length == 0)
                return null;

            try
            {
                var doc = XDocument.Parse(xml);
                var root = doc.Root;
                if (root == null) return null;
                var interaction = root.Element("Interaction");
                if (interaction == null) return null;

                var list = new List<DrugAllergyAlertDetail>();

                foreach (var drugNode in interaction.Elements())
                {
                    var nodeName = drugNode.Name.LocalName;
                    if (nodeName != "GGPI" && nodeName != "Product" && nodeName != "GenericItem")
                        continue;

                    var allergy = drugNode.Element("Allergy");
                    if (allergy == null)
                        continue;

                    foreach (var allergenNode in allergy.Elements())
                    {
                        var allergenType = allergenNode.Name.LocalName;
                        var allergenName = (string)allergenNode.Attribute("name");
                        var allergenRef = (string)allergenNode.Attribute("reference");

                        string allergyClassName = null;
                        string allergyClassRef = null;

                        XElement classElement = null;
                        if (allergenNode.Name.LocalName == "Molecule")
                        {
                            classElement = allergenNode.Element("SubstanceClass");
                        }
                        else if (allergenNode.Name.LocalName == "SubstanceClass")
                        {
                            classElement = allergenNode;
                        }

                        if (classElement != null)
                        {
                            allergyClassName = (string)classElement.Attribute("name");
                            allergyClassRef = (string)classElement.Attribute("reference");
                        }

                        var detail = new DrugAllergyAlertDetail
                        {
                            DrugName = (string)drugNode.Attribute("name"),
                            DrugReference = (string)drugNode.Attribute("reference"),
                            AllergenNodeType = allergenType,
                            AllergenName = allergenName,
                            AllergenReference = allergenRef,
                            AllergyClassName = allergyClassName,
                            AllergyClassReference = allergyClassRef
                        };

                        list.Add(detail);
                    }
                }

                return list.Count > 0 ? list : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Parse kết quả Drug-Health Alert (Result/Interaction/GGPI/.../Route/HealthIssueCode/ClassInteraction...).
        /// </summary>
        public static List<DrugHealthAlertDetail> ParseDrugHealthAlerts(string xml)
        {
            if (string.IsNullOrEmpty(xml) || xml.Trim().Length == 0)
                return null;

            try
            {
                var doc = XDocument.Parse(xml);
                var root = doc.Root;
                if (root == null) return null;
                var interaction = root.Element("Interaction");
                if (interaction == null) return null;

                var list = new List<DrugHealthAlertDetail>();

                foreach (var drugNode in interaction.Elements())
                {
                    var nodeName = drugNode.Name.LocalName;
                    if (nodeName != "GGPI" && nodeName != "Product" && nodeName != "GenericItem")
                        continue;

                    string drugName = (string)drugNode.Attribute("name");
                    string drugRef = (string)drugNode.Attribute("reference");

                    foreach (var route in drugNode.Elements("Route"))
                    {
                        string routeName = (string)route.Attribute("name");

                        foreach (var hic in route.Elements("HealthIssueCode"))
                        {
                            string code = (string)hic.Attribute("code");
                            string codeType = (string)hic.Attribute("codeType");
                            string issueName = (string)hic.Attribute("name");

                            foreach (var ci in hic.Elements("ClassInteraction"))
                            {
                                var prescribingClass = ci.Element("PrescribingInteractionClass");
                                var severityElem = ci.Element("Severity");
                                var likelihoodElem = ci.Element("Likelihood");
                                var documentationElem = ci.Element("Documentation");
                                var interactionElem = ci.Element("Interaction");

                                var detail = new DrugHealthAlertDetail();
                                detail.DrugName = drugName;
                                detail.DrugReference = drugRef;
                                detail.RouteName = routeName;
                                detail.HealthIssueCode = code;
                                detail.HealthIssueCodeType = codeType;
                                detail.HealthIssueName = issueName;
                                detail.PrescribingClassName = prescribingClass != null ? (string)prescribingClass.Attribute("name") : null;
                                detail.PrescribingClassDescription = prescribingClass != null ? (string)prescribingClass.Attribute("description") : null;
                                detail.PrescribingMoleculeName = (prescribingClass != null && prescribingClass.Element("PrescribingMolecule") != null) ? (string)prescribingClass.Element("PrescribingMolecule").Attribute("name") : null;

                                string severityText = null;
                                if (severityElem != null)
                                {
                                    var nameAttr = (string)severityElem.Attribute("name");
                                    severityText = !string.IsNullOrEmpty(nameAttr) ? nameAttr : severityElem.Value;
                                }
                                detail.Severity = severityText;
                                detail.SeverityLevel = ParseDrugHealthSeverity(severityText);

                                detail.Likelihood = likelihoodElem != null ? ((string)likelihoodElem.Attribute("name") != null ? (string)likelihoodElem.Attribute("name") : likelihoodElem.Value) : null;
                                detail.Documentation = documentationElem != null ? ((string)documentationElem.Attribute("name") != null ? (string)documentationElem.Attribute("name") : documentationElem.Value) : null;
                                detail.ProfessionalText = (interactionElem != null && interactionElem.Element("Professional") != null) ? (string)interactionElem.Element("Professional") : null;

                                list.Add(detail);
                            }
                        }
                    }
                }

                return list.Count > 0 ? list : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Parse Drug Information (GGPI) từ Result/Content/GGPI...
        /// </summary>
        public static DrugInformationGgpiDetail ParseDrugInformationGgpi(string xml)
        {
            if (string.IsNullOrEmpty(xml) || xml.Trim().Length == 0)
                return null;

            try
            {
                var doc = XDocument.Parse(xml);
                var root = doc.Root;
                if (root == null) return null;
                var content = root.Element("Content");
                if (content == null) return null;
                var ggpi = content.Element("GGPI");
                if (ggpi == null) return null;

                var monograph = ggpi.Element("MONOGRAPH");

                // Ưu tiên SPECIFICPIL language="English" nếu có, nếu không lấy bản đầu tiên
                XElement pilSpecific = null;
                foreach (var p in ggpi.Elements("PILS"))
                {
                    var s = p.Element("SPECIFICPIL");
                    if (s != null)
                    {
                        if (pilSpecific == null)
                            pilSpecific = s;
                        var langAttr = s.Attribute("Language");
                        if (langAttr != null && string.Equals((string)langAttr, "English", StringComparison.OrdinalIgnoreCase))
                        {
                            pilSpecific = s;
                            break;
                        }
                    }
                }

                var detail = new DrugInformationGgpiDetail
                {
                    DrugName = (string)ggpi.Attribute("name"),
                    Reference = (string)ggpi.Attribute("reference"),
                    GenericName = monograph != null ? (string)monograph.Element("GENMONO") : null,
                    TherapeuticClass = monograph != null ? (string)monograph.Element("GCLS") : null,
                    Category = monograph != null ? (string)monograph.Element("GPCAT") : null,
                    Contraindications = monograph != null ? (string)monograph.Element("GCI") : (pilSpecific != null ? (string)pilSpecific.Element("CONTRAINDICATIONS") : null),
                    SpecialPrecautions = monograph != null ? (string)monograph.Element("GSP") : (pilSpecific != null ? (string)pilSpecific.Element("SPECIALPRECAUTIONS") : null),
                    AdverseReactions = monograph != null ? (string)monograph.Element("GAR") : null,
                    DrugInteractions = monograph != null ? (string)monograph.Element("GDI") : null,
                    DosageAndAdministration = monograph != null ? (string)monograph.Element("GDOSE") : (pilSpecific != null ? (string)pilSpecific.Element("DOSAGE") : null),
                    Pharmacology = monograph != null ? (string)monograph.Element("GACTION") : null
                };

                return detail;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        private static DrugInteractionSeverity ParseDrugInteractionSeverity(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DrugInteractionSeverity.Unknown;

            var normalized = value.Trim().ToLowerInvariant();
            switch (normalized)
            {
                case "severe":
                    return DrugInteractionSeverity.Severe;
                case "moderate":
                    return DrugInteractionSeverity.Moderate;
                case "minor":
                    return DrugInteractionSeverity.Minor;
                case "caution":
                    return DrugInteractionSeverity.Caution;
                default:
                    return DrugInteractionSeverity.Unknown;
            }
        }

        private static DrugHealthSeverity ParseDrugHealthSeverity(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DrugHealthSeverity.Unknown;

            var normalized = value.Trim().ToLowerInvariant();
            switch (normalized)
            {
                case "contraindicated":
                    return DrugHealthSeverity.Contraindicated;
                case "extreme caution":
                    return DrugHealthSeverity.ExtremeCaution;
                default:
                    return DrugHealthSeverity.Unknown;
            }
        }
    }
}
