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
using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Utilities
{
    public class WordProtectedProcess
    {
        public const string keyFull = "<#DocProtected>";
        public const string keyBegin = "<#DocProtectedB>";
        public const string keyEnd = "<#DocProtectedE>";

        public WordProtectedProcess()
        {

        }

        //khóa toàn bộ documet và tạo ra vùng được phép nhập từ vị trí key cuối cùng đến hết trang.
        public void InitialProtected(DevExpress.XtraRichEdit.RichEditControl txtDescription, ref int positionFinded)
        {
            try
            {
                #region #RegisterUserList
                txtDescription.ReplaceService<DevExpress.XtraRichEdit.Services.IUserListService>(new MyUserListService());
                #endregion #RegisterUserList

                #region #RegisterUserGroupList
                txtDescription.ReplaceService<DevExpress.XtraRichEdit.Services.IUserGroupListService>(new MyGroupListService());
                #endregion #RegisterUserGroupList

                RangePermissionCollection rangePermissions = txtDescription.Document.BeginUpdateRangePermissions();

                #region #Authentication
                //Define the user credentials:
                txtDescription.Options.Authentication.UserName = "phuongdt";
                txtDescription.Options.Authentication.Group = "Skywalkers";
                #endregion #Authentication

                #region #RangesColor
                //Customize the editable ranges appearance: 
                txtDescription.Options.RangePermissions.HighlightColor = Color.White;
                txtDescription.Options.RangePermissions.HighlightBracketsColor = Color.White;
                #endregion #RangesColor

                DocumentRange rangeAdmin = null;

                DevExpress.XtraRichEdit.API.Native.DocumentRange[] ranges = txtDescription.Document.FindAll(keyFull, SearchOptions.None);
                if (ranges != null && ranges.Length > 0)
                {
                    ranges = ranges.OrderByDescending(o => o.Start.ToInt()).ToArray();
                    positionFinded = ranges[0].Start.ToInt() + 1;
                    int lengthRange = txtDescription.Text.Length - positionFinded;
                    lengthRange = lengthRange > 0 ? lengthRange : 1;
                    rangeAdmin = txtDescription.Document.CreateRange(positionFinded, lengthRange);

                    txtDescription.RtfText = txtDescription.RtfText.Replace(keyFull, "");
                }
                else
                {
                    rangeAdmin = positionFinded > 0 ? txtDescription.Document.CreateRange(positionFinded, txtDescription.Text.Length - positionFinded) : null;
                }

                if (rangeAdmin != null)
                {
                    RangePermission permission = rangePermissions.CreateRangePermission(rangeAdmin);
                    permission.UserName = "phuongdt";
                    permission.Group = "Skywalkers";
                    rangePermissions.Add(permission);

                    txtDescription.Document.EndUpdateRangePermissions(rangePermissions);
                    // Enforce protection and set password. 
                    txtDescription.Document.Protect("abc123#@!");
                }
                else
                {
                    txtDescription.Document.Unprotect();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //khóa toàn bộ documet.
        //tạo ra vùng cho phép nhập ngoài khoảng key đầu và cuối
        public void InitialProtected(DevExpress.XtraRichEdit.RichEditControl txtDescription, ref string positionProtect)
        {
            try
            {
                //nếu là kiểu vị trí cũ thì gọi hàm cũ
                if (!string.IsNullOrWhiteSpace(positionProtect))
                {
                    int positionFinded = Inventec.Common.TypeConvert.Parse.ToInt32(positionProtect);
                    if (positionFinded > 0)
                    {
                        InitialProtected(txtDescription, ref positionFinded);
                        positionProtect = positionFinded.ToString();
                        return;
                    }
                }

                #region #RegisterUserList
                txtDescription.ReplaceService<DevExpress.XtraRichEdit.Services.IUserListService>(new MyUserListService());
                #endregion #RegisterUserList

                #region #RegisterUserGroupList
                txtDescription.ReplaceService<DevExpress.XtraRichEdit.Services.IUserGroupListService>(new MyGroupListService());
                #endregion #RegisterUserGroupList

                RangePermissionCollection rangePermissions = txtDescription.Document.BeginUpdateRangePermissions();

                #region #Authentication
                //Define the user credentials:
                txtDescription.Options.Authentication.UserName = "phuongdt";
                txtDescription.Options.Authentication.Group = "Skywalkers";
                #endregion #Authentication

                #region #RangesColor
                //Customize the editable ranges appearance: 
                txtDescription.Options.RangePermissions.HighlightColor = Color.White;
                txtDescription.Options.RangePermissions.HighlightBracketsColor = Color.White;
                #endregion #RangesColor

                List<DocumentRange> rangeAdmins = new List<DocumentRange>();

                DevExpress.XtraRichEdit.API.Native.DocumentRange[] rangesB = txtDescription.Document.FindAll(keyBegin, SearchOptions.None);
                DevExpress.XtraRichEdit.API.Native.DocumentRange[] rangesE = txtDescription.Document.FindAll(keyEnd, SearchOptions.None);
                if (rangesB != null && rangesB.Length > 0 && rangesE != null && rangesE.Length > 0)
                {
                    List<string> position = new List<string>();

                    int countEBeforeB = rangesE.Count(o => o.Start < rangesB[0].Start);

                    //từ vị trí 0 đến vị trí key begin đầu tiên sẽ cho phép nhập.
                    int start0 = 0;
                    int length0 = rangesB[0].Start.ToInt() - start0;
                    var rang0 = txtDescription.Document.CreateRange(start0, length0);
                    rangeAdmins.Add(rang0);
                    position.Add(string.Format("{0}:{1}", start0, length0));

                    if (rangesB.Count() == 1 && rangesE.Count() == 1) // có 1 cặp thì sẽ khóa trong khoảng
                    {
                        int start = rangesE.Last().Start.ToInt();
                        int length = txtDescription.Document.Length - start;
                        length = length > 0 ? length : 1;
                        var rang = txtDescription.Document.CreateRange(start, length);
                        rangeAdmins.Add(rang);
                        position.Add(string.Format("{0}:{1}", start0, length0));
                    }
                    else
                    {
                        //tạo vùng cho phép nhập từ vị trí end đến begin tiếp theo
                        //bắt đầu từ vị trí end có begin đầu tiên.
                        for (int i = countEBeforeB; i < rangesE.Length; i++)
                        {
                            //do key bị xóa nên vị trí bắt đầu sẽ thay đổi.

                            int start = rangesE[i].Start.ToInt() + 1;
                            int length = 0;

                            var ran = rangesB.FirstOrDefault(o => o.Start > rangesE[i].Start);

                            if (ran == null)//end ko có begin sẽ mở đến hết
                            {
                                length = txtDescription.Document.Length - start;
                                length = length > 0 ? length : 1;

                                var rang = txtDescription.Document.CreateRange(start, length);
                                rangeAdmins.Add(rang);
                                position.Add(string.Format("{0}:{1}", start, length));
                                break;
                            }
                            else
                            {
                                length = ran.Start.ToInt() - rangesE[i].Start.ToInt();
                                length = length > 0 ? length : 1;

                                var rang = txtDescription.Document.CreateRange(start, length);
                                rangeAdmins.Add(rang);
                                position.Add(string.Format("{0}:{1}", start, length));
                            }
                        }
                    }

                    positionProtect = string.Join("|", position);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(positionProtect))
                    {
                        var posi = positionProtect.Split('|');
                        foreach (var item in posi)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                var ps = item.Split(':');
                                if (ps.Length == 2)
                                {
                                    int start = Inventec.Common.TypeConvert.Parse.ToInt32(ps[0]);
                                    int length = Inventec.Common.TypeConvert.Parse.ToInt32(ps[1]);
                                    if (start > 0 && length > 0)
                                    {
                                        DocumentRange ran = txtDescription.Document.CreateRange(start, length);
                                        rangeAdmins.Add(ran);
                                    }
                                }
                            }
                        }
                    }
                }

                txtDescription.Document.ReplaceAll(keyBegin, " ", SearchOptions.CaseSensitive);
                txtDescription.Document.ReplaceAll(keyEnd, " ", SearchOptions.CaseSensitive);

                //lưu sẽ lấy vị trí đã có
                if (rangePermissions.Count > 0)
                {
                    List<string> position = new List<string>();
                    foreach (var item in rangePermissions)
                    {
                        position.Add(string.Format("{0}:{1}", item.Range.Start.ToInt(), item.Range.Length));
                    }

                    positionProtect = string.Join("|", position);
                }
                else if (rangeAdmins != null && rangeAdmins.Count > 0)
                {
                    foreach (var rangeAdmin in rangeAdmins)
                    {
                        RangePermission permission = rangePermissions.CreateRangePermission(rangeAdmin);
                        permission.UserName = "phuongdt";
                        permission.Group = "Skywalkers";
                        rangePermissions.Add(permission);
                    }

                    txtDescription.Document.EndUpdateRangePermissions(rangePermissions);
                    // Enforce protection and set password. 
                    txtDescription.Document.Protect("abc123#@!");
                }
                else
                {
                    txtDescription.Document.Unprotect();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    #region #NewUserGroupList
    public class MyGroupListService : DevExpress.XtraRichEdit.Services.IUserGroupListService
    {
        List<string> userGroups = CreateUserGroups();

        static List<string> CreateUserGroups()
        {
            List<string> result = new List<string>();
            result.Add(@"Everyone");
            result.Add(@"Administrators");
            result.Add(@"Contributors");
            result.Add(@"Owners");
            result.Add(@"Editors");
            result.Add(@"Current User");
            result.Add("Skywalkers");
            return result;
        }
        public IList<string> GetUserGroups()
        {
            return userGroups;
        }
    }
    #endregion #NewUserGroupList

    #region #NewUserList
    public class MyUserListService : DevExpress.XtraRichEdit.Services.IUserListService
    {
        List<string> users = CreateUsers();

        static List<string> CreateUsers()
        {
            List<string> result = new List<string>();
            result.Add("phuongdt");
            return result;
        }
        public IList<string> GetUsers()
        {
            return users;
        }
    }
    #endregion #NewUserList

}
