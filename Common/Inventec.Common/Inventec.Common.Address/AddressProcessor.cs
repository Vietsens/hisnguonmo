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
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Address
{
    public class AddressProcessor
    {
        private static string formatCheck = ",{0},";
        private static string formatAddress = "{0} {1}";
        List<V_SDA_PROVINCE> VSdaProvince = new List<V_SDA_PROVINCE>();
        List<V_SDA_DISTRICT> VSdaDistrict = new List<V_SDA_DISTRICT>();
        List<V_SDA_COMMUNE> VSdaCommune = new List<V_SDA_COMMUNE>();

        public AddressProcessor(List<V_SDA_PROVINCE> vSdaProvince, List<V_SDA_DISTRICT> vSdaDistrict, List<V_SDA_COMMUNE> vSdaCommune)
        {
            if (vSdaProvince != null) VSdaProvince = vSdaProvince.Where(o => o.IS_ACTIVE == 1).ToList();
            if (vSdaDistrict != null) VSdaDistrict = vSdaDistrict.Where(o => o.IS_ACTIVE == 1).ToList();
            if (vSdaCommune != null) VSdaCommune = vSdaCommune.Where(o => o.IS_ACTIVE == 1).ToList();
        }

        /// <summary>
        /// xử lý tách chuỗi địa chỉ thành mã, tên tương ứng trên hệ thống
        /// 
        /// </summary>
        /// <param name="fullAddress">số 12a thôn bắc tiến 1 , xã phú lạc , huyện cẩm khê, tỉnh phú thọ</param>
        /// <returns>
        /// số 12a thôn bắc tiến 1
        /// xã phú lạc
        /// huyện cẩm khê
        /// tỉnh phú thọ
        /// </returns>
        public AddressADO SplitFromFullAddress(string fullAddress)
        {
            AddressADO result = new AddressADO();
            result.Address = fullAddress;
            try
            {
                if (!String.IsNullOrWhiteSpace(fullAddress))
                {
                    result.Address = fullAddress.Trim(',', '-', ' ');
                    V_SDA_PROVINCE province = null;
                    V_SDA_DISTRICT district = null;
                    V_SDA_COMMUNE commune = null;
                    string[] splitA = result.Address.Split(',', '-');
                    string[] joinsAdd = new string[splitA.Length];
                    for (int i = splitA.Length - 1; i >= 0; i--)
                    {
                        string path = splitA[i];
                        string checkData = string.Join(" ", path.Split(' ').Where(o => !String.IsNullOrWhiteSpace(o)));
                        if (province == null)
                        {
                            //tỉnh không thêm dấu phẩy ở 2 đầu để so sánh dữ liệu
                            string lowerPath = checkData.ToLower();
                            province = VSdaProvince.FirstOrDefault(o => lowerPath.Contains(o.PROVINCE_NAME.ToLower()));

                            if (province != null)
                            {
                                result.ProvinceCode = province.PROVINCE_CODE;
                                result.ProvinceName = province.PROVINCE_NAME;
                                continue;
                            }
                        }

                        if (district == null)
                        {
                            string lowerPath = string.Format(formatCheck, checkData.ToLower());
                            List<V_SDA_DISTRICT> districts = new List<V_SDA_DISTRICT>();
                            districts.AddRange(VSdaDistrict);
                            if (province != null)
                            {
                                districts = districts.Where(o => o.PROVINCE_ID == province.ID).ToList();
                            }

                            //Lấy ra tất cả các trường hợp
                            List<V_SDA_DISTRICT> existDistrict = districts.Where(o => lowerPath.Contains(string.Format(formatCheck, o.DISTRICT_NAME.ToLower())) || lowerPath.Contains(string.Format(formatCheck, (o.INITIAL_NAME ?? "").ToLower() + " " + o.DISTRICT_NAME.ToLower()))).ToList();

                            //khác đơn vị hành chính sẽ không lấy được.
                            if (existDistrict == null || existDistrict.Count <= 0)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Có sai khác đơn vị hành chính: "+ lowerPath);
                                existDistrict = districts.Where(o => lowerPath.Contains(o.DISTRICT_NAME.ToLower())).ToList();
                            }

                            if (existDistrict.Count > 0)
                            {
                                //Nếu có 1 huyện thỏa mãn thì lấy luôn huyện tương ứng
                                if (existDistrict.Count == 1)
                                {
                                    district = existDistrict.First();
                                }
                                else
                                {
                                    //Nếu có nhiều hơn 1 huyện thỏa mãn thì vẫn lấy tạm ra 1 huyện bất kỳ.
                                    district = existDistrict.OrderByDescending(o => o.ID).First();
                                }
                            }

                            if (district != null)
                            {
                                result.DistrictCode = district.DISTRICT_CODE;
                                result.DistrictName = string.Format(formatAddress, district.INITIAL_NAME, district.DISTRICT_NAME).Trim();
                                if (province == null)
                                {
                                    //gán lại thông tin tỉnh nếu chưa có
                                    province = VSdaProvince.FirstOrDefault(o => o.ID == district.PROVINCE_ID);
                                    result.ProvinceCode = district.PROVINCE_CODE;
                                    result.ProvinceName = district.PROVINCE_NAME;
                                }
                                continue;
                            }
                        }

                        if (commune == null)
                        {
                            string lowerPath = string.Format(formatCheck, checkData.ToLower());
                            List<V_SDA_COMMUNE> communes = new List<V_SDA_COMMUNE>();
                            communes.AddRange(VSdaCommune);

                            if (district != null)
                            {
                                communes = communes.Where(o => o.DISTRICT_ID == district.ID).ToList();
                            }
                            else if (province != null)
                            {
                                List<V_SDA_DISTRICT> districts = VSdaDistrict.Where(o => o.PROVINCE_ID == province.ID).ToList();
                                communes = communes.Where(o => districts.Exists(e => e.ID == o.DISTRICT_ID)).ToList();
                            }

                            //Lấy ra tất cả các trường hợp
                            List<V_SDA_COMMUNE> existCommunes = communes.Where(o => lowerPath.Contains(string.Format(formatCheck, o.COMMUNE_NAME.ToLower())) || lowerPath.Contains(string.Format(formatCheck, (o.INITIAL_NAME ?? "").ToLower() + " " + o.COMMUNE_NAME.ToLower()))).ToList();

                            //khác đơn vị hành chính sẽ không lấy được.
                            if (existCommunes == null || existCommunes.Count <= 0)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Có sai khác đơn vị hành chính: " + lowerPath);
                                existCommunes = communes.Where(o => lowerPath.Contains(o.COMMUNE_NAME.ToLower())).ToList();
                            }

                            if (existCommunes.Count > 0)
                            {
                                //Nếu có 1 huyện thỏa mãn thì lấy luôn huyện tương ứng
                                if (existCommunes.Count == 1)
                                {
                                    commune = existCommunes.First();
                                }
                                else
                                {
                                    //Nếu có nhiều hơn 1 huyện thỏa mãn thì vẫn lấy tạm ra 1 huyện bất kỳ.
                                    commune = existCommunes.OrderByDescending(o => o.ID).First();
                                }
                            }

                            if (commune != null)
                            {
                                result.CommuneCode = commune.COMMUNE_CODE;
                                result.CommuneName = string.Format(formatAddress, commune.INITIAL_NAME, commune.COMMUNE_NAME).Trim(); ;
                                continue;
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(path))
                        {
                            joinsAdd[i] = path.Trim(',', '-', ' ');
                        }
                    }

                    result.Address = string.Join(", ", joinsAdd.Where(o => !String.IsNullOrWhiteSpace(o)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result.Address = fullAddress;
            }

            return result;
        }
    }
}
