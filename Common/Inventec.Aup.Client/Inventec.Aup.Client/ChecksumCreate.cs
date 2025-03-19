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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Inventec.Aup.Client
{
    internal class ChecksumCreate
    {
        internal const string versionFileName = "versionlog.txt";

        public ChecksumCreate() { }

        internal List<string> GetAllFileInpath(string path)
        {
            List<string> files = new List<string>();

            if (!string.IsNullOrWhiteSpace(path))
            {
                files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
                files = files.Where(o => !o.Contains("\\Logs\\") && !o.Contains("\\.svn\\")).ToList();
            }

            Inventec.Common.Logging.LogSystem.Debug("files count in Get All Files Inpath" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => path), path) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => files.Count), files.Count));
            return files;
        }

        string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        internal void Create(string containingFolder)
        {
            LogSystem.Debug("ChecksumCreate => bat dau khoi tao du lieu cho file versionlog");
            List<string> fileInPaths = GetAllFileInpath(containingFolder);
            string content = String.Join(Environment.NewLine, fileInPaths.Select(o => o.Substring(containingFolder.Length + 1) + "||" + CalculateMD5(o)).ToArray());
            System.IO.File.WriteAllText(String.Format("{0}/{1}", containingFolder, versionFileName), content, Encoding.UTF8);
            Inventec.Common.Logging.LogSystem.Debug("*CheckSumCreate*  Content" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => content), content));
            LogSystem.Debug("ChecksumCreate => Ket thuc khoi tao du lieu cho file versionlog");
        }
    }
}
