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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.ConnectionTest.CG300
{
    /// <summary>
    /// Writes the label array to the CG300 exchange folder.
    /// Write to *.tmp first then rename to *.txt so CG never reads a half-written file.
    /// </summary>
    internal class Cg300FileWriter
    {
        private const string TEMP_EXTENSION = ".tmp";
        private const string DATA_EXTENSION = ".txt";

        /// <summary>Returns the written file path, or null when failed.</summary>
        internal static string Write(string folder, List<Cg300LabelADO> labels)
        {
            string tempPath = null;
            try
            {
                if (String.IsNullOrWhiteSpace(folder) || labels == null || !labels.Any())
                    return null;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(labels);
                Inventec.Common.Logging.LogSystem.Debug("CG300 data____" + json);

                string fileName = (labels[0].LabNum ?? "CG300") + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                tempPath = Path.Combine(folder, fileName + TEMP_EXTENSION);
                string dataPath = Path.Combine(folder, fileName + DATA_EXTENSION);

                // UTF-8 without BOM - Vietnamese names must be kept
                File.WriteAllText(tempPath, json, new UTF8Encoding(false));
                File.Move(tempPath, dataPath);
                return dataPath;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Ghi file CG300 that bai. Folder=" + folder, ex);
                TryDelete(tempPath);
            }
            return null;
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (!String.IsNullOrEmpty(path) && File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
