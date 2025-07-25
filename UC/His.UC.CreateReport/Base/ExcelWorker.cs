using His.UC.CreateReport.Data;
using Inventec.Common.Logging;
using Inventec.Fss.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.CreateReport.Base
{
    internal class ExcelWorker
    {
        FlexCel.XlsAdapter.XlsFile xls;
        int sheetIndexForDynamicFilterConfig = 0;
        public ExcelWorker()
        {
        }

        public void InitData(string reportTemplateUrl)
        {
            try
            {
                ReportTemplateFile templateFile = JsonConvert.DeserializeObject<ReportTemplateFile>(reportTemplateUrl);
                var template = FileDownload.GetFile(templateFile.URL);
                MemoryStream TemplateStream = new MemoryStream();
                template.CopyTo(TemplateStream);
                TemplateStream.Position = 0;
                xls = new FlexCel.XlsAdapter.XlsFile(true);
                xls.Open(TemplateStream);
                //xls.Save(TemplateStream);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<DynamicFilterConfigADO> InitDynamicFilterConfig()
        {
            List<DynamicFilterConfigADO> results = new List<DynamicFilterConfigADO>();
            try
            {
                sheetIndexForDynamicFilterConfig = xls.GetSheetIndex("DynamicFilterConfig");
                LogSystem.Info("sheetIndexForDynamicFilterConfig: " + sheetIndexForDynamicFilterConfig);
                for (int j = 2; j < xls.RowCount + 1; j++)
                {
                    DynamicFilterConfigADO dynamicFilterConfig = new DynamicFilterConfigADO();
                    for (int i = 1; i < xls.ColCount; i++)
                    {
                        string cellValue = GetCellValue(sheetIndexForDynamicFilterConfig, j, i);
                        LogSystem.Info($"Row: {j}, Column: {i}, Value: {cellValue}");
                        if (string.IsNullOrEmpty(cellValue)) continue;
                        switch (i)
                        {
                            case 1:
                                dynamicFilterConfig.ID = int.TryParse(cellValue, out int id) ? id : (int?)null; ;
                                break;
                            case 2:
                                dynamicFilterConfig.SQL = cellValue;
                                break;
                            case 3:
                                dynamicFilterConfig.FormType = cellValue;
                                break;
                            case 4:
                                dynamicFilterConfig.DelegateForChangeValueConfig = cellValue;
                                break;
                            case 5:
                                dynamicFilterConfig.PropetiesConfig = cellValue;
                                break;
                            case 6:
                                dynamicFilterConfig.Title = cellValue;
                                break;
                            case 7:
                                dynamicFilterConfig.JSON_OUTPUT = cellValue;
                                break;
                            case 8:
                                dynamicFilterConfig.IS_REQUIRE = (cellValue == "1" || cellValue.ToLower() == "true") ? true : false;
                                break;
                            case 9:
                                dynamicFilterConfig.NUM_ORDER = long.TryParse(cellValue, out long numOrder) ? numOrder : (long?)null;
                                break;
                            case 10:
                                dynamicFilterConfig.HEIGHT = long.TryParse(cellValue, out long height) ? height : (long?)null;
                                break;
                            case 11:
                                dynamicFilterConfig.WIDTH_RATIO = short.TryParse(cellValue, out short widthRatio) ? widthRatio : (short?)null;
                                break;
                            case 12:
                                dynamicFilterConfig.ROW_COUNT = long.TryParse(cellValue, out long rowCount) ? rowCount : (long?)null;
                                break;
                            case 13:
                                dynamicFilterConfig.COLUMN_COUNT = long.TryParse(cellValue, out long columnCount) ? columnCount : (long?)null;
                                break;
                            case 14:
                                dynamicFilterConfig.ROW_INDEX = long.TryParse(cellValue, out long rowIndex) ? rowIndex : (long?)null;
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(dynamicFilterConfig.JSON_OUTPUT) && !string.IsNullOrEmpty(dynamicFilterConfig.FormType))
                    {
                        results.Add(dynamicFilterConfig);
                    }
                }


                //xls.Save(TemplateStream);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return results;
        }

        string GetCellValue(int sheetIndex, int row, int column)
        {
            string result = "";
            try
            {
                int xf = 0;
                var st = xls.GetCellValue(sheetIndex, row, column, ref xf);
                if (st != null)
                {
                    result = st.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
