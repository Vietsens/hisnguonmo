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
                    List<V_SDA_PROVINCE> provinces = null;
                    List<V_SDA_DISTRICT> district = null;
                    V_SDA_COMMUNE commune = null;
                    string[] splitA = result.Address.Split(',', '-');
                    string[] joinsAdd = new string[splitA.Length];
                    for (int i = splitA.Length - 1; i >= 0; i--)
                    {
                        string path = splitA[i];
                        string checkData = string.Join(" ", path.Split(' ').Where(o => !String.IsNullOrWhiteSpace(o)));
                        if (provinces == null)
                        {
                            //tỉnh không thêm dấu phẩy ở 2 đầu để so sánh dữ liệu
                            string lowerPath = checkData.ToLower();
                            provinces = VSdaProvince.Where(o => lowerPath.Contains(o.PROVINCE_NAME.ToLower())).ToList();

                            if (provinces != null && provinces.Count > 0)
                            {
                                continue;
                            }
                        }

                        if (district == null)
                        {
                            string lowerPath = string.Format(formatCheck, checkData.ToLower());
                            List<V_SDA_DISTRICT> districts = new List<V_SDA_DISTRICT>();
                            districts.AddRange(VSdaDistrict);
                            if (provinces != null)
                            {
                                List<long> provinceIds = provinces.Select(s => s.ID).ToList();
                                districts = districts.Where(o => provinceIds.Contains(o.PROVINCE_ID)).ToList();
                            }

                            //Lấy ra tất cả các trường hợp
                            List<V_SDA_DISTRICT> existDistrict = districts.Where(o => lowerPath.Contains(string.Format(formatCheck, o.DISTRICT_NAME.ToLower())) || lowerPath.Contains(string.Format(formatCheck, (o.INITIAL_NAME ?? "").ToLower() + " " + o.DISTRICT_NAME.ToLower()))).ToList();

                            //khác đơn vị hành chính sẽ không lấy được.
                            if (existDistrict == null || existDistrict.Count <= 0)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Có sai khác đơn vị hành chính: " + lowerPath);
                                existDistrict = districts.Where(o => lowerPath.Contains(o.DISTRICT_NAME.ToLower())).ToList();
                            }

                            if (existDistrict.Count > 0)
                            {
                                district = existDistrict;
                                //continue;
                            }
                        }

                        if (commune == null)
                        {
                            string lowerPath = string.Format(formatCheck, checkData.ToLower());
                            List<V_SDA_COMMUNE> communes = new List<V_SDA_COMMUNE>();

                            if (district != null)
                            {
                                List<long> districtIds = district.Select(s => s.ID).ToList();
                                var dCommunes = VSdaCommune.Where(o => districtIds.Contains(o.DISTRICT_ID ?? 0)).ToList();
                                if (dCommunes.Count > 0)
                                {
                                    communes.AddRange(dCommunes);
                                }
                            }

                            if (provinces != null)
                            {
                                List<long> provinceIds = provinces.Select(s => s.ID).ToList();
                                List<long> districtIds = VSdaDistrict.Where(o => provinceIds.Contains(o.PROVINCE_ID)).Select(s => s.ID).ToList();
                                List<long> notInIds = communes.Select(s => s.ID).ToList();
                                var dCommunes = VSdaCommune.Where(o => (districtIds.Contains(o.DISTRICT_ID ?? 0) || provinceIds.Contains(o.PROVINCE_ID ?? 0)) && !notInIds.Contains(o.ID)).ToList();
                                if (dCommunes.Count > 0)
                                {
                                    communes.AddRange(dCommunes);
                                }
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
                                    //ghép thông tin T/H/X đầy đủ so sánh với địa chỉ gốc để đảm bảo không lấy nhầm xã cũ
                                    string checkaddress = string.Join(" - ", splitA);
                                    foreach (var item in existCommunes)
                                    {
                                        List<string> addr = new List<string>();
                                        addr.Add(string.Format(formatAddress, item.INITIAL_NAME, item.COMMUNE_NAME).Trim());
                                        addr.Add(string.Format(formatAddress, item.DISTRICT_INITIAL_NAME, item.DISTRICT_NAME).Trim());
                                        addr.Add(item.PROVINCE_NAME);

                                        string address = string.Join(" - ", addr);
                                        if (checkaddress.Contains(address))
                                        {
                                            commune = item;
                                            break;
                                        }
                                    }

                                    if (commune == null)
                                    {
                                        //Nếu có nhiều hơn 1 huyện thỏa mãn thì vẫn lấy tạm ra 1 huyện bất kỳ.
                                        commune = existCommunes.OrderByDescending(o => o.ID).First();
                                    }
                                }
                            }

                            if (commune != null)
                            {
                                result.CommuneCode = commune.COMMUNE_CODE;
                                result.CommuneName = string.Format(formatAddress, commune.INITIAL_NAME, commune.COMMUNE_NAME).Trim();
                                result.IsNoDistrict = commune.IS_NO_DISTRICT == 1;

                                if (commune.DISTRICT_ID.HasValue)
                                {
                                    //gán lại thông tin tỉnh nếu chưa có
                                    district = VSdaDistrict.Where(o => o.ID == commune.DISTRICT_ID).ToList();
                                    result.DistrictCode = commune.DISTRICT_CODE;
                                    result.DistrictName = string.Format(formatAddress, commune.DISTRICT_INITIAL_NAME, commune.DISTRICT_NAME).Trim();
                                }

                                //gán lại thông tin tỉnh nếu chưa có
                                provinces = VSdaProvince.Where(o => o.ID == commune.PROVINCE_ID).ToList();
                                result.ProvinceCode = commune.PROVINCE_CODE;
                                result.ProvinceName = commune.PROVINCE_NAME;

                                continue;
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(path))
                        {
                            joinsAdd[i] = path.Trim(',', '-', ' ');
                        }
                    }

                    for (int i = 0; i < joinsAdd.Length; i++)
                    {
                        if (joinsAdd[i].ToLower() == result.CommuneName.ToLower()
                            || (!String.IsNullOrWhiteSpace(result.DistrictName) && joinsAdd[i].ToLower() == result.DistrictName.ToLower())
                            || joinsAdd[i].ToLower() == result.ProvinceName.ToLower())
                        {
                            joinsAdd[i] = "";
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
