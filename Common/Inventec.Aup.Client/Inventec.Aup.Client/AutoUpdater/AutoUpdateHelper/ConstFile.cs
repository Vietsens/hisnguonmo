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

namespace Inventec.Aup.Client.AutoUpdater
{
    public class ConstFile
    {
        public const string TEMPFOLDERNAME = "TempFolderBak";
        public const string CONFIGFILEKEY = "config_";
        public const string CONFIGFILE = "config";
        public const string FILENAME = "AutoUpdater.config";
        public const string AUP_FILENAME = "AUPAutoUpdater.config";
        public const string ROOLBACKFILE = "WEBSiteUpdate.exe";
        public const string MESSAGETITLE = "Cập nhật tự động";
        public const string CANCELORNOT = "Bạn có muốn hủy cập nhật hay không";
        public const string APPLYTHEUPDATE = "Chương trình cần được khởi động lại, vui lòng nhấn OK để tiếp tục";
        public const string NOTNETWORK = "Cập nhật không thành công, vui lòng thử lại";
    }
}
