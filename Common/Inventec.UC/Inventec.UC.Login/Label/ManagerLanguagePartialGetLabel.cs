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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Label
{
    internal partial class ManagerLanguage
    {
        public string GetTextLabel(Enum enumBC)
        {
            string text = "";
            if (Language == LanguageEnum.Vietnamese)
            {
                switch (enumBC)
                {
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_CONFIG: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_CONFIG;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_EXIT: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_EXIT;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_LOGIN: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_LOGIN;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_CHK_AUTO_LOGIN: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_CHK_AUTO_LOGIN; 
                       break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_FRM_LOGIN: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_FRM_LOGIN;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_ACCOUNT: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_ACCOUNT;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_FORGOT_PASSWORD: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_FORGOT_PASSWORD;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_LANGUAGE: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_LANGUAGE;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_PASSWORD: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_PASSWORD;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_PROGRESS_LOADING: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_PROGRESS_LOADING;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_BRANCH: text = TextLabelViResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_BRANCH;
                        break;
                    default: text = defaultViMessage;
                        break;
                }
            }
            else if (Language == LanguageEnum.English)
            {
                switch (enumBC)
                {
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_CONFIG: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_CONFIG;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_EXIT: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_EXIT;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_LOGIN: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_LOGIN;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_CHK_AUTO_LOGIN: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_CHK_AUTO_LOGIN;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_FRM_LOGIN: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_FRM_LOGIN;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_ACCOUNT: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_ACCOUNT;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_FORGOT_PASSWORD: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_FORGOT_PASSWORD;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_LANGUAGE: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_LANGUAGE;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_PASSWORD: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_PASSWORD;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_PROGRESS_LOADING: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_PROGRESS_LOADING;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_BRANCH: text = TextLabelEnResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_BRANCH;
                        break;
                    default: text = defaultEnMessage;
                        break;
                }
            }
            else if (Language == LanguageEnum.Myanmar)
            {
                switch (enumBC)
                {
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_CONFIG: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_CONFIG;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_EXIT: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_EXIT;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_LOGIN: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_LOGIN;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_CHK_AUTO_LOGIN: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_CHK_AUTO_LOGIN;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_FRM_LOGIN: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_FRM_LOGIN;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_ACCOUNT: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_ACCOUNT;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_FORGOT_PASSWORD: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_FORGOT_PASSWORD;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_LANGUAGE: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_LANGUAGE;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_PASSWORD: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_PASSWORD;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_PROGRESS_LOADING: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_PROGRESS_LOADING;
                        break;
                    case Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_BRANCH: text = TextLabelMyResource.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_BRANCH;
                        break;
                    default: text = defaultEnMessage;
                        break;
                }
            }

            return text;
        }
    }
}
