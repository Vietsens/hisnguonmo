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
 * Author:   Ê¥µîÆïÊ¿£¨Knights Warrior£© 
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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;

namespace Inventec.Aup.Client.AutoUpdater
{
    public class Config
    {
        #region The private fields
        private bool enabled = true;
        private string serverUrl = string.Empty;
        private string userName = string.Empty;
        private string passWord = string.Empty;
        private string tryTimes = string.Empty;
        private UpdateFileList updateFileList = new UpdateFileList();
        #endregion

        #region The public property
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        public string ServerUrl
        {
            get { return serverUrl; }
            set { serverUrl = value; }
        }
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        public string PassWord
        {
            get { return passWord; }
            set { passWord = value; }
        }
        public string TryTimes
        {
            get { return string.IsNullOrEmpty(tryTimes) ? "1" : tryTimes; }
            set { tryTimes = value; }
        }

        public UpdateFileList UpdateFileList
        {
            get { return updateFileList; }
            set { updateFileList = value; }
        }
        #endregion

        #region The public method
        public static Config LoadConfig(string file)
        {
            Config config = new Config();
            if (File.Exists(file))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                StreamReader sr = new StreamReader(file);
                config = xs.Deserialize(sr) as Config;
                sr.Close();
                return config;
            }
            else
            {
                config.Enabled = true;
            }
            return config;
        }

        public void SaveConfig(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Config));
            StreamWriter sw = new StreamWriter(file);
            xs.Serialize(sw, this);
            sw.Close();
        }

        public static string GetFileHash(string filePath)
        {
            HashAlgorithm hash = HashAlgorithm.Create();
            FileStream file = new FileStream(filePath, FileMode.Open);
            byte[] hashByte = hash.ComputeHash(file);//哈希算法根据文本得到哈希码的字节数组
            string str1 = BitConverter.ToString(hashByte);//将字节数组装换为字符串
            return str1;
        }
        #endregion
    }

}
