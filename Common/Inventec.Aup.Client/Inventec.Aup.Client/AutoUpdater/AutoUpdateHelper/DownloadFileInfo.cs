﻿/*****************************************************************
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
using System.Text;
using System.IO;

namespace Inventec.Aup.Client.AutoUpdater
{
    public class DownloadFileInfo
    {
        #region The private fields
        string downloadUrl = string.Empty;
        string fileName = string.Empty;
        string lastver = string.Empty;
        int size = 0;
        int tryTimes = 0;//Số lần thử tải xuống
        string version = string.Empty;
        #endregion

        #region The public property
        public string DownloadUrl { get { return downloadUrl; } }
        public string FileFullName { get { return fileName; } }
        public string FileName { get { return Path.GetFileName(FileFullName); } }
        public string LastVer { get { return lastver; } set { lastver = value; } }
        public int Size { get { return size; } }
        public int TryTimes { get { return tryTimes; } set { tryTimes = value; } }
        public string Version { get { return version; } set { version = value; } }
        #endregion

        #region The constructor of DownloadFileInfo
        public DownloadFileInfo(string url, string name, string ver, int size, string versionid)
        {
            this.downloadUrl = url;
            this.fileName = name;
            this.lastver = ver;
            this.size = size;
            this.version = versionid;
        }
        #endregion
    }
}
