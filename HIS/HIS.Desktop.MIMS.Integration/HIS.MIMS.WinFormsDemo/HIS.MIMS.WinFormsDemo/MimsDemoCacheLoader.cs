using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;

namespace HIS.MIMS.WinFormsDemo
{
    /// <summary>
    /// Load HIS data from CSV into BackendDataWorker cache so the demo can run
    /// standalone (without HIS Desktop main app pre-initializing the cache).
    ///
    /// Tables loaded:
    /// - HIS_ATC                  (ATC_CODE -> MIMS_GUID)
    /// - V_HIS_MEDICINE_TYPE      (MEDICINE_TYPE_CODE -> ATC_CODES)
    /// - HIS_MEDICINE_TYPE_ACIN   (empty — active ingredient path not needed)
    /// - HIS_ACTIVE_INGREDIENT    (empty)
    /// </summary>
    public static class MimsDemoCacheLoader
    {
        public static void LoadAll()
        {
            string basePath = ConfigurationSettings.AppSettings["MIMS.Demo.CsvBasePath"];
            if (string.IsNullOrEmpty(basePath))
                throw new Exception("Thiếu config MIMS.Demo.CsvBasePath trong App.config");

            string atcPath = Path.Combine(basePath, "HIS_ATC", "HIS_ATC_DATA_TABLE.csv");
            string medTypePath = Path.Combine(basePath, "HIS_MEDICINE_TYPE", "HIS_MEDICINE_TYPE_DATA_TABLE.csv");

            if (!File.Exists(atcPath))
                throw new FileNotFoundException("Không tìm thấy file HIS_ATC CSV: " + atcPath);
            if (!File.Exists(medTypePath))
                throw new FileNotFoundException("Không tìm thấy file HIS_MEDICINE_TYPE CSV: " + medTypePath);

            var atcs = LoadHisAtc(atcPath);
            BackendDataWorker.UpdateToRam(typeof(HIS_ATC), atcs, 0);

            var medTypes = LoadVHisMedicineType(medTypePath);
            BackendDataWorker.UpdateToRam(typeof(V_HIS_MEDICINE_TYPE), medTypes, 0);

            BackendDataWorker.UpdateToRam(typeof(HIS_MEDICINE_TYPE_ACIN), new List<HIS_MEDICINE_TYPE_ACIN>(), 0);
            BackendDataWorker.UpdateToRam(typeof(HIS_ACTIVE_INGREDIENT), new List<HIS_ACTIVE_INGREDIENT>(), 0);
        }

        public static int CountAtc()
        {
            var list = BackendDataWorker.Get<HIS_ATC>();
            return list == null ? 0 : list.Count;
        }

        public static int CountMedicineType()
        {
            var list = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
            return list == null ? 0 : list.Count;
        }

        private static List<HIS_ATC> LoadHisAtc(string path)
        {
            var rows = ParseCsv(path);
            if (rows.Count == 0) return new List<HIS_ATC>();
            string[] header = rows[0];

            int idxId = FindColumnIndex(header, "ID");
            int idxIsActive = FindColumnIndex(header, "IS_ACTIVE");
            int idxIsDelete = FindColumnIndex(header, "IS_DELETE");
            int idxAtcCode = FindColumnIndex(header, "ATC_CODE");
            int idxAtcName = FindColumnIndex(header, "ATC_NAME");
            int idxMimsGuid = FindColumnIndex(header, "MIMS_GUID");
            int idxMimsName = FindColumnIndex(header, "MIMS_NAME");
            int idxMimsType = FindColumnIndex(header, "MIMS_TYPE");
            int idxIsMimsMapped = FindColumnIndex(header, "IS_MIMS_MAPPED");

            var list = new List<HIS_ATC>(rows.Count);
            for (int i = 1; i < rows.Count; i++)
            {
                string[] row = rows[i];
                if (row.Length <= idxAtcCode) continue;
                var atc = new HIS_ATC
                {
                    ID = ParseLong(SafeGet(row, idxId)),
                    IS_ACTIVE = ParseShortNull(SafeGet(row, idxIsActive)),
                    IS_DELETE = ParseShortNull(SafeGet(row, idxIsDelete)),
                    ATC_CODE = SafeGet(row, idxAtcCode),
                    ATC_NAME = SafeGet(row, idxAtcName),
                    MIMS_GUID = SafeGet(row, idxMimsGuid),
                    MIMS_NAME = SafeGet(row, idxMimsName),
                    MIMS_TYPE = ParseShortNull(SafeGet(row, idxMimsType)),
                    IS_MIMS_MAPPED = ParseShortNull(SafeGet(row, idxIsMimsMapped))
                };
                list.Add(atc);
            }
            return list;
        }

        private static List<V_HIS_MEDICINE_TYPE> LoadVHisMedicineType(string path)
        {
            var rows = ParseCsv(path);
            if (rows.Count == 0) return new List<V_HIS_MEDICINE_TYPE>();
            string[] header = rows[0];

            int idxId = FindColumnIndex(header, "ID");
            int idxCode = FindColumnIndex(header, "MEDICINE_TYPE_CODE");
            int idxName = FindColumnIndex(header, "MEDICINE_TYPE_NAME");
            int idxAtcCodes = FindColumnIndex(header, "ATC_CODES");

            var list = new List<V_HIS_MEDICINE_TYPE>(rows.Count);
            for (int i = 1; i < rows.Count; i++)
            {
                string[] row = rows[i];
                if (row.Length <= idxCode) continue;
                var med = new V_HIS_MEDICINE_TYPE
                {
                    ID = ParseLong(SafeGet(row, idxId)),
                    MEDICINE_TYPE_CODE = SafeGet(row, idxCode),
                    MEDICINE_TYPE_NAME = SafeGet(row, idxName),
                    ATC_CODES = SafeGet(row, idxAtcCodes)
                };
                list.Add(med);
            }
            return list;
        }

        private static int FindColumnIndex(string[] header, string name)
        {
            for (int i = 0; i < header.Length; i++)
            {
                if (string.Equals(header[i], name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            throw new Exception("Không tìm thấy cột " + name + " trong CSV header.");
        }

        private static string SafeGet(string[] row, int index)
        {
            return index >= 0 && index < row.Length ? row[index] : string.Empty;
        }

        private static List<string[]> ParseCsv(string path)
        {
            var rows = new List<string[]>();
            using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(path, Encoding.UTF8))
            {
                parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = false;
                while (!parser.EndOfData)
                {
                    rows.Add(parser.ReadFields());
                }
            }
            return rows;
        }

        private static long ParseLong(string s)
        {
            long v;
            return long.TryParse(s, out v) ? v : 0L;
        }

        private static short? ParseShortNull(string s)
        {
            short v;
            return short.TryParse(s, out v) ? (short?)v : null;
        }
    }
}
