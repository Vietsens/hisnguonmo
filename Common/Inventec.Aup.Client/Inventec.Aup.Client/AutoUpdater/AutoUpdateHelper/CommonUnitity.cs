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
/*****************************************************************
 * Copyright (C) Knights Warrior Corporation. All rights reserved.
 * 
 * Author:   圣殿骑士（Knights Warrior） 
 * Email:    KnightsWarrior@msn.com
 * Website:  http://www.cnblogs.com/KnightsWarrior/       http://knightswarrior.blog.51cto.com/
 * Create Date:  5/8/2010 
 * Usage:
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Inventec.Aup.Client.AutoUpdater
{
    class CommonUnitity
    {
        public static string SystemBinUrl = AppDomain.CurrentDomain.BaseDirectory;

        public static void RestartApplication()
        {
            Process.Start(Application.ExecutablePath);
            Environment.Exit(0);
        }

        public static void CloseApplication()
        {
            OperProcess op = new OperProcess();
            //启动进程 Bắt đầu quá trình
            op.StartProcess();
        }

        #region 原有模式 Mẫu cũ
        //public static string GetFolderUrl(DownloadFileInfo file)
        //{
        //    string folderPathUrl = string.Empty;
        //    int folderPathPoint = file.DownloadUrl.IndexOf("/", 15) + 1;
        //    string filepathstring = file.DownloadUrl.Substring(folderPathPoint);
        //    int folderPathPoint1 = filepathstring.IndexOf("/");
        //    string filepathstring1 = filepathstring.Substring(folderPathPoint1 + 1);
        //    if (filepathstring1.IndexOf("/") != -1)
        //    {
        //        string[] ExeGroup = filepathstring1.Split('/');
        //        for (int i = 0; i < ExeGroup.Length - 1; i++)
        //        {
        //            folderPathUrl += "\\" + ExeGroup[i];
        //        }
        //        if (!Directory.Exists(SystemBinUrl + ConstFile.TEMPFOLDERNAME + folderPathUrl))
        //        {
        //            Directory.CreateDirectory(SystemBinUrl + ConstFile.TEMPFOLDERNAME + folderPathUrl);
        //        }
        //    }
        //    return folderPathUrl;
        //}
        #endregion

        public static string GetFolderUrl(DownloadFileInfo file, string tempFolderUrl = "", string postProcess = "")
        {
            string folderPathUrl = string.Empty;
            int folderPathPoint = file.DownloadUrl.IndexOf("/", 15) + 1;
            string filepathstring = file.DownloadUrl.Substring(folderPathPoint);
            //int folderPathPoint1 = filepathstring.IndexOf("/");
            //string filepathstring1 = filepathstring.Substring(folderPathPoint1 + 1);
            //if(filepathstring1.IndexOf("/") != -1)
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filepathstring), filepathstring));
            if (filepathstring.IndexOf("/") != -1)
            {
                //string[] ExeGroup = filepathstring1.Split('/');
                string[] ExeGroup = filepathstring.Split('/');

                int i = 0;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ExeGroup), ExeGroup));
                for (int j = 0; j < ExeGroup.Length - 1; j++)
                {
                    if (!String.IsNullOrEmpty(postProcess))
                    {
                        if (ExeGroup[j] == "x86" || ExeGroup[j] == "x64")
                        {
                            i = j + 1;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => i), i));
                            break;
                        }
                    }
                    else
                    {
                        if (ExeGroup[j] == "Aup" || ExeGroup[j] == "Integrate\\Aup" || ExeGroup[j] == "Integrate\\\\Aup")
                        {
                            i = j + 1;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => i), i));
                            break;
                        }
                    }
                }

                for (; i < ExeGroup.Length - 1; i++)
                {
                    folderPathUrl += "\\" + ExeGroup[i];
                }
                if (string.IsNullOrEmpty(tempFolderUrl))
                {
                    tempFolderUrl = ConstFile.TEMPFOLDERNAME;
                    if (!Directory.Exists(SystemBinUrl + "\\" + tempFolderUrl + folderPathUrl))
                    {
                        Directory.CreateDirectory(SystemBinUrl + "\\" + tempFolderUrl + folderPathUrl);
                    }
                }
                else
                {
                    if (!Directory.Exists(SystemBinUrl + "\\" + tempFolderUrl))
                    {
                        Directory.CreateDirectory(SystemBinUrl + "\\" + tempFolderUrl);
                    }
                }

            }
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => folderPathUrl), folderPathUrl));
            return folderPathUrl;
        }
    }
}
