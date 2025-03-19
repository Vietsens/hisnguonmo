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
using Inventec.Aup.Utility;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Client
{
    public class VersionV2
    {
        public VersionV2() { }

        public FileDataResult Update(string clientVersionName, string clientVersionLog, string clientCode, string containingFolder)
        {
            try
            {
                LogSystem.Debug("Update.1");
                if (!Directory.Exists(containingFolder))
                {
                    throw new FolderContainerException();
                }

                LogSystem.Debug("Update.2____clientVersionName=" + clientVersionName + "____clientVersionLog=" + clientVersionLog + "____clientCode=" + clientCode + "____containingFolder=" + containingFolder + "____AupConstant.BASE_URI=" + AupConstant.BASE_URI);
                DirectoryInfo dInfo = new DirectoryInfo(containingFolder);
                FileInfo versionFileName = dInfo.GetFiles(clientVersionName, SearchOption.TopDirectoryOnly).FirstOrDefault();
                //if (versionFileName == null) throw new FileNotFoundException("Khong tim thay file clientVersion: " + clientVersionName);
                FileInfo versionFileLog = dInfo.GetFiles(clientVersionLog, SearchOption.TopDirectoryOnly).FirstOrDefault();
                LogSystem.Debug("Update.3");
                CommonParam param = new CommonParam();
                UpdateInfoTransfer updateInfoTransfer = new Utility.UpdateInfoTransfer();
                updateInfoTransfer.AppCode = clientCode;
                updateInfoTransfer.OSVersion = OSVersionWorker.CurrentOSVersion;
                if (versionFileName != null)
                    updateInfoTransfer.VersionContent = System.IO.File.ReadAllText(versionFileName.FullName);
                else
                    updateInfoTransfer.VersionContent = clientVersionName;
                if (versionFileLog == null)
                {
                    //Nếu không tồn tại file VersionLog.txt tại client thì thực hiện tạo dưới client, mục đích để tránh trường hợp client tải quá nhiều file từ server dẫn đến nghẽn băng thông trên server, dẫn đến client bị treo
                    new ChecksumCreate().Create(containingFolder);
                    versionFileLog = dInfo.GetFiles(clientVersionLog, SearchOption.TopDirectoryOnly).FirstOrDefault();
                }
                updateInfoTransfer.VersionLogContent = System.IO.File.ReadAllText(versionFileLog.FullName);
                updateInfoTransfer.FileAppInfos = GetAllFileInfo(containingFolder);
                LogSystem.Debug("Update.4");
                var rs = Inventec.Aup.Client.Base.ApiConsumerStore.AupConsumer.Post<Inventec.Core.ApiResultObject<FileDataResult>>(AupConstant.api__AupVersion__Update, param, updateInfoTransfer, 360);
                LogSystem.Debug("**Inventec.Aup.Client Update(string clientVersionName, string clientVersionLog, string clientCode, string containingFolder) ** rs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                if (rs != null && rs.Data != null)
                {
                    return rs.Data;
                }
            }
            catch (FolderContainerException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FolderContainerException("Exception when CheckForUpdate product version", ex);
            }
            return null;
        }

        List<FileAppInfo> GetAllFileInfo(string containingFolder)
        {
            List<FileAppInfo> fileAppInfos = new List<FileAppInfo>();
            LogSystem.Debug("GetAllFileInfo.1");
            List<string> fileInPaths = new ChecksumCreate().GetAllFileInpath(containingFolder);
            fileInPaths = fileInPaths.Where(o => o.EndsWith(".dll") || o.EndsWith(".doc") || o.EndsWith(".docx") || o.EndsWith(".xls") || o.EndsWith(".xlsx") || o.EndsWith(".repx") || o.EndsWith(".exe") || o.EndsWith(".config")).Select(o => o.Substring(containingFolder.Length + 1)).ToList();
            if (fileInPaths != null && fileInPaths.Count > 0)
            {
                foreach (var item in fileInPaths)
                {
                    fileAppInfos.Add(new FileAppInfo()
                    {
                        FullName = item
                    });
                }
                LogSystem.Debug("GetAllFileInfo.2 => fileAppInfos.count=" + fileAppInfos.Count);
            }
            LogSystem.Debug("GetAllFileInfo.3");
            return fileAppInfos;
        }

        public FileDataResult UpdateSpecify(string clientVersionName, string clientVersionLog, string clientCode, string containingFolder)
        {
            try
            {
                if (!Directory.Exists(containingFolder))
                {
                    throw new FolderContainerException();
                }

                DirectoryInfo dInfo = new DirectoryInfo(containingFolder);
                FileInfo versionFileName = dInfo.GetFiles(clientVersionName, SearchOption.TopDirectoryOnly).FirstOrDefault();
                FileInfo versionFileLog = dInfo.GetFiles(clientVersionLog, SearchOption.TopDirectoryOnly).FirstOrDefault();

                CommonParam param = new CommonParam();
                UpdateInfoTransfer updateInfoTransfer = new Utility.UpdateInfoTransfer();
                updateInfoTransfer.AppCode = clientCode;
                updateInfoTransfer.OSVersion = OSVersionWorker.CurrentOSVersion;
                if (versionFileName != null)
                    updateInfoTransfer.VersionContent = System.IO.File.ReadAllText(versionFileName.FullName);
                if (versionFileLog != null)
                    updateInfoTransfer.VersionLogContent = System.IO.File.ReadAllText(versionFileLog.FullName);
                updateInfoTransfer.FileAppInfos = GetAllFileInfo(containingFolder);

                var rs = Inventec.Aup.Client.Base.ApiConsumerStore.AupConsumer.Post<Inventec.Core.ApiResultObject<FileDataResult>>(AupConstant.api__AupVersion__Update_Specify, param, updateInfoTransfer);
                LogSystem.Debug("**Inventec.Aup.Client UpdateSpecify(string clientVersionName, string clientVersionLog, string clientCode, string containingFolder) ** rs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                if (rs != null && rs.Data != null)
                {
                    return rs.Data;
                }
            }
            catch (FolderContainerException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FolderContainerException("Exception when CheckForUpdate product version", ex);
            }
            return null;
        }

        public FileDataResult UpdateFixFile(string clientNote, string clientCode, string containingFolder)
        {
            try
            {
                if (!File.Exists(string.Format("{0}//{1}//{2}", containingFolder, "Integrate//Aup", clientNote)))
                {
                    CommonParam param = new CommonParam();
                    UpdateInfoTransfer updateInfoTransfer = new Utility.UpdateInfoTransfer();
                    updateInfoTransfer.AppCode = clientCode;
                    updateInfoTransfer.OSVersion = OSVersionWorker.CurrentOSVersion;
                    //updateInfoTransfer.VersionContent = System.IO.File.ReadAllText(string.Format("{0}//{1}", containingFolder, "Integrate//Aup//note.txt"));

                    var rs = Inventec.Aup.Client.Base.ApiConsumerStore.AupConsumer.Post<Inventec.Core.ApiResultObject<FileDataResult>>(AupConstant.api__AupVersion__UpdateFixFile, param, updateInfoTransfer);
                    LogSystem.Debug("**UpdateFixFile ** rs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                    if (rs != null && rs.Data != null)
                    {
                        return rs.Data;
                    }
                }
            }
            catch (FolderContainerException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FolderContainerException("Exception when CheckForUpdate product version", ex);
            }
            return null;
        }

        public bool CleanZipFile(FileDataResult fileDataResult)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rs = Inventec.Aup.Client.Base.ApiConsumerStore.AupConsumer.Post<Inventec.Core.ApiResultObject<bool>>(AupConstant.api__AupVersion__CleanZipFile, param, fileDataResult);
                if (rs != null)
                {
                    return rs.Data;
                }
            }
            catch (FolderContainerException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FolderContainerException("Exception when CheckForUpdate product version", ex);
            }
            return false;
        }
    }
}
