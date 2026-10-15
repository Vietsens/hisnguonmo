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
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ConnectionTest.CG300
{
    /// <summary>Output of Cg300LabelBuilder.Build.</summary>
    internal class Cg300BuildResult
    {
        internal List<Cg300LabelADO> Labels = new List<Cg300LabelADO>();
        /// <summary>Samples that produced at least one label</summary>
        internal List<LisSampleADO> SentSamples = new List<LisSampleADO>();
        /// <summary>Services without any test sample type (tube) mapping</summary>
        internal List<string> UnmappedServiceNames = new List<string>();
    }

    /// <summary>
    /// Builds CG300 tube records: 1 record = 1 barcode x 1 ContainerCode.
    /// ContainerCode = HIS_TEST_SAMPLE_TYPE.TEST_SAMPLE_TYPE_CODE, mapped to services via HIS_SERVICE_TESA.
    /// </summary>
    internal class Cg300LabelBuilder
    {
        private const string DATE_FORMAT = "yyyy-MM-ddTHH:mm:ss";
        private const string TEST_ITEM_SEPARATOR = "; ";
        /// <summary>Test sample type name convention "Color - Source", e.g. "Purple - EDTA"</summary>
        private const string NAME_SEPARATOR = " - ";
        private const string PRIORITY_URGENT = "Urgent";
        private const string PRIORITY_NORMAL = "Normal";
        private const string YES = "Yes";
        private const string NO = "No";
        private const string BLOOD_CONFIRMED = "1";
        private const short IS_EMERGENCY_TRUE = 1;

        internal static Cg300BuildResult Build(
            List<LisSampleADO> samples,
            List<V_LIS_SAMPLE_SERVICE> sampleServices,
            Dictionary<string, V_HIS_SERVICE> serviceByCode,
            ILookup<long, long> testSampleTypeIdsByServiceId,
            Dictionary<long, HIS_TEST_SAMPLE_TYPE> testSampleTypeById,
            HashSet<string> printedBarcodes,
            string printUser,
            string printIp)
        {
            Cg300BuildResult result = new Cg300BuildResult();
            try
            {
                if (samples == null || !samples.Any()) return result;
                if (serviceByCode == null) serviceByCode = new Dictionary<string, V_HIS_SERVICE>();
                if (testSampleTypeById == null) testSampleTypeById = new Dictionary<long, HIS_TEST_SAMPLE_TYPE>();
                if (testSampleTypeIdsByServiceId == null) testSampleTypeIdsByServiceId = new List<long>().ToLookup(o => o, o => o);

                var servicesBySampleId = (sampleServices ?? new List<V_LIS_SAMPLE_SERVICE>())
                    .Where(o => o != null)
                    .ToLookup(o => o.SAMPLE_ID);
                HashSet<string> unmapped = new HashSet<string>();
                string printTime = DateTime.Now.ToString(DATE_FORMAT);

                foreach (var sample in samples)
                {
                    // ContainerCode -> (tube type, service names)
                    Dictionary<long, List<string>> serviceNamesByTypeId = new Dictionary<long, List<string>>();
                    foreach (var ss in servicesBySampleId[sample.ID])
                    {
                        V_HIS_SERVICE service = null;
                        if (!String.IsNullOrEmpty(ss.SERVICE_CODE))
                            serviceByCode.TryGetValue(ss.SERVICE_CODE, out service);

                        var typeIds = service != null
                            ? testSampleTypeIdsByServiceId[service.ID].Where(id => testSampleTypeById.ContainsKey(id)).ToList()
                            : new List<long>();
                        if (!typeIds.Any())
                        {
                            unmapped.Add(ss.SERVICE_NAME ?? ss.SERVICE_CODE);
                            continue;
                        }

                        foreach (var typeId in typeIds)
                        {
                            List<string> names;
                            if (!serviceNamesByTypeId.TryGetValue(typeId, out names))
                            {
                                names = new List<string>();
                                serviceNamesByTypeId.Add(typeId, names);
                            }
                            if (!names.Contains(ss.SERVICE_NAME))
                                names.Add(ss.SERVICE_NAME);
                        }
                    }

                    if (!serviceNamesByTypeId.Any()) continue;

                    bool isReprint = (sample.PRINT_COUNT ?? 0) > 0
                        || (printedBarcodes != null && printedBarcodes.Contains(sample.BARCODE));
                    foreach (var item in serviceNamesByTypeId.OrderBy(o => testSampleTypeById[o.Key].TEST_SAMPLE_TYPE_CODE))
                    {
                        result.Labels.Add(MakeLabel(sample, testSampleTypeById[item.Key], item.Value, isReprint, printTime, printUser, printIp));
                    }
                    result.SentSamples.Add(sample);
                }

                result.UnmappedServiceNames = unmapped.ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private static Cg300LabelADO MakeLabel(LisSampleADO sample, HIS_TEST_SAMPLE_TYPE tubeType, List<string> serviceNames,
            bool isReprint, string printTime, string printUser, string printIp)
        {
            string color, source;
            SplitTubeName(tubeType.TEST_SAMPLE_TYPE_NAME, out color, out source);

            Cg300LabelADO label = new Cg300LabelADO();
            label.LabNum = sample.BARCODE ?? "";
            label.HospNum = sample.PATIENT_CODE ?? "";
            label.PatientType = sample.PATIENT_TYPE_NAME ?? "";
            label.CnName = GetPatientName(sample);
            label.Sex = sample.GENDER_NAME ?? "";
            label.AgeDesc = sample.DOB.HasValue ? MPS.AgeUtil.CalculateFullAge(sample.DOB.Value) : "";
            label.Location = sample.REQUEST_ROOM_NAME ?? "";
            label.OrderDate = FormatTime(sample.INTRUCTION_TIME);
            label.Priority = sample.IS_EMERGENCY == IS_EMERGENCY_TRUE ? PRIORITY_URGENT : PRIORITY_NORMAL;
            label.Source = source;
            label.ContainerCode = tubeType.TEST_SAMPLE_TYPE_CODE ?? "";
            label.Container = color;
            label.RePrint = isReprint ? YES : NO;
            label.TestItemDesc = String.Join(TEST_ITEM_SEPARATOR, serviceNames.Where(o => !String.IsNullOrEmpty(o)));
            label.DeptDesc = sample.REQUEST_DEPARTMENT_NAME ?? "";
            label.ExamineRequestID = sample.SERVICE_REQ_CODE ?? "";
            label.PaymentStatus = "";
            label.PrintTime = printTime;
            label.PrintIP = printIp ?? "";
            label.TestType = sample.SAMPLE_TYPE_NAME ?? "";
            label.TestItemType = "";
            label.PrintUser = printUser ?? "";
            label.TakeReportDesc = "";
            label.TakeReportAddress = "";
            label.BedNum = "";
            label.BloodConfirmState = BLOOD_CONFIRMED;
            return label;
        }

        /// <summary>"Purple - EDTA" -> color "Purple", source "EDTA"; no separator -> both = full name.</summary>
        private static void SplitTubeName(string name, out string color, out string source)
        {
            name = name ?? "";
            int idx = name.IndexOf(NAME_SEPARATOR, StringComparison.Ordinal);
            if (idx > 0)
            {
                color = name.Substring(0, idx).Trim();
                source = name.Substring(idx + NAME_SEPARATOR.Length).Trim();
            }
            else
            {
                color = name.Trim();
                source = name.Trim();
            }
        }

        private static string GetPatientName(LisSampleADO sample)
        {
            if (!String.IsNullOrWhiteSpace(sample.VIR_PATIENT_NAME))
                return sample.VIR_PATIENT_NAME.Trim();
            return String.Join(" ", new[] { sample.LAST_NAME, sample.FIRST_NAME }.Where(o => !String.IsNullOrWhiteSpace(o)));
        }

        private static string FormatTime(long? time)
        {
            if (!time.HasValue || time.Value <= 0) return "";
            DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(time.Value);
            return dt.HasValue ? dt.Value.ToString(DATE_FORMAT) : "";
        }
    }
}
