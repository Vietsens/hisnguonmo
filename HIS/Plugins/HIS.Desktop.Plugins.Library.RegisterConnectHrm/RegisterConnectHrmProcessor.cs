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
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.RegisterConnectHrm
{
    public class RegisterConnectHrmProcessor
    {
        HrmTokenData token = null;
        private static List<IPAddress> GetIPInternet()
        {
            List<IPAddress> localIP = new List<IPAddress>();
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP.Add(IPAddress.Parse(endPoint.Address.ToString()));
            }
            return localIP;
        }

        public async Task<MOS.SDO.HisPatientSDO> GetDataHrm1(string _baseUri, string loginname, string password, string grant_type, string client_id, string client_secret, string _employeeCode)
        {
            MOS.SDO.HisPatientSDO data = new MOS.SDO.HisPatientSDO();
            try
            {
                await RegisToken(_baseUri, loginname, password, grant_type, client_id, client_secret);
                if (token == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Dang Nhap HRM That Bai");
                    return data;
                }
                var adrr = new RebindingHandler(GetIPInternet());
                using (var client = new HttpClient(adrr))
                {
                    client.BaseAddress = new Uri(_baseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token.access_token);

                    string dataUri = string.Format("employee_code={0}", _employeeCode);

                    HttpResponseMessage responseRs = await client.GetAsync("WebService/api/v1/employee?" + dataUri).ConfigureAwait(false);
                    if (responseRs.IsSuccessStatusCode)
                    {
                        string rsChiTiet = responseRs.Content.ReadAsStringAsync().Result;
                        try
                        {
                            var resultHrm = (Temperatures)JsonConvert.DeserializeObject<Temperatures>(rsChiTiet);
                            if (resultHrm != null && resultHrm.Content != null && resultHrm.Content.Length > 0)
                            {
                                Content ado = resultHrm.Content[0];
                                data.HRM_EMPLOYEE_CODE = ado.EmployeeCode;
                                data.VIR_PATIENT_NAME = ado.FullName;
                                data.LAST_NAME = (string)ado.LastName;
                                data.FIRST_NAME = (string)ado.FirstName;
                                data.VIR_ADDRESS = ado.CurrentAddress;
                                data.WORK_PLACE = ado.DepartmentName;
                                data.GENDER_ID = ado.Gender == 1 ? 2 : 1;
                                data.DOB = this.ConvertDob(ado.DateOfBirth);
                                data.ADDRESS = ado.CurrentAddress;

                                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("resultHrm >>>>>>>>>>> : ", ado));
                            }
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Warn(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return data;
        }

        private async Task RegisToken(string _baseUri, string loginname, string password, string grant_type, string client_id, string client_secret)
        {
            try
            {
                var adrr = new RebindingHandler(GetIPInternet());
                using (HttpClient client = new HttpClient(adrr))
                {
                    client.BaseAddress = new Uri(_baseUri);
                    var formContent = new FormUrlEncodedContent(new[]
                   {
                       new KeyValuePair<string, string>("grant_type",grant_type),
                       new KeyValuePair<string, string>("client_id", client_id) ,
                       new KeyValuePair<string, string>("client_secret", client_secret),
                       new KeyValuePair<string, string>("username", loginname),
                       new KeyValuePair<string, string>("password", password)
                   }); HttpResponseMessage resp = await client.PostAsync("/WebService/oauth/token", formContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        string respData = resp.Content.ReadAsStringAsync().Result;
                        token = Newtonsoft.Json.JsonConvert.DeserializeObject<HrmTokenData>(respData);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong Dang Nhap Duoc HRM >>>><<<<Address: " + _baseUri + " loginname: " + loginname + " password: " + password);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public MOS.SDO.HisPatientSDO GetDataHrm(string _baseUri, string loginname, string password, string grant_type, string client_id, string client_secret, string _employeeCode)
        {
            MOS.SDO.HisPatientSDO data = new MOS.SDO.HisPatientSDO();
            try
            {
                RegisToken(_baseUri, loginname, password, grant_type, client_id, client_secret);
                if (token == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Dang Nhap HRM That Bai");
                    return data;
                }
                var adrr = new RebindingHandler(GetIPInternet());
                using (var client = new HttpClient(adrr))
                {
                    client.BaseAddress = new Uri(_baseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token.access_token);

                    string dataUri = string.Format("employee_code={0}", _employeeCode);

                    HttpResponseMessage responseRs = client.GetAsync("WebService/api/v1/employee?" + dataUri).Result;
                    if (responseRs.IsSuccessStatusCode)
                    {
                        string rsChiTiet = responseRs.Content.ReadAsStringAsync().Result;
                        try
                        {
                            var resultHrm = (Temperatures)JsonConvert.DeserializeObject<Temperatures>(rsChiTiet);
                            if (resultHrm != null && resultHrm.Content != null && resultHrm.Content.Length > 0)
                            {
                                Content ado = resultHrm.Content[0];
                                data.HRM_EMPLOYEE_CODE = ado.EmployeeCode;
                                data.VIR_PATIENT_NAME = ado.FullName;
                                data.LAST_NAME = (string)ado.LastName;
                                data.FIRST_NAME = (string)ado.FirstName;
                                data.VIR_ADDRESS = ado.CurrentAddress;
                                data.WORK_PLACE = ado.OrganizationName.ToString();
                                data.GENDER_ID = ado.Gender == 1 ? 2 : 1;
                                data.DOB = this.ConvertDob(ado.DateOfBirth);
                                data.ADDRESS = ado.CurrentAddress;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Warn(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return data;
        }

        private long ConvertDob(long dob)
        {
            long rs = 0;
            try
            {
                DateTime t = new DateTime(1970, 1, 1).AddMilliseconds(dob);
                rs = Convert.ToInt64(t.ToLocalTime().ToString("yyyyMMdd") + "000000");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

    }

    public partial class Temperatures
    {
        [JsonProperty("content")]
        public Content[] Content { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }

    public partial class Content
    {
        [JsonProperty("organizationCode")]
        public object OrganizationCode { get; set; }

        [JsonProperty("employeeId")]
        public long EmployeeId { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("employeeCode")]
        public string EmployeeCode { get; set; }

        [JsonProperty("dateOfBirth")]
        public long DateOfBirth { get; set; }

        [JsonProperty("gender")]
        public long Gender { get; set; }

        [JsonProperty("placeOfBirth")]
        public string PlaceOfBirth { get; set; }

        [JsonProperty("permanentAddress")]
        public string PermanentAddress { get; set; }

        [JsonProperty("currentAddress")]
        public string CurrentAddress { get; set; }

        [JsonProperty("telephoneNumber")]
        public string TelephoneNumber { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("mobileNumber2")]
        public object MobileNumber2 { get; set; }

        [JsonProperty("mobileNumber3")]
        public object MobileNumber3 { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("trainingSpeciality")]
        public object TrainingSpeciality { get; set; }

        [JsonProperty("trainingLevel")]
        public object TrainingLevel { get; set; }

        [JsonProperty("language")]
        public object Language { get; set; }

        [JsonProperty("languageLevel")]
        public object LanguageLevel { get; set; }

        [JsonProperty("partyAdmissionDate")]
        public object PartyAdmissionDate { get; set; }

        [JsonProperty("partyAdmissionPlace")]
        public string PartyAdmissionPlace { get; set; }

        [JsonProperty("personalIdNumber")]
        public string PersonalIdNumber { get; set; }

        [JsonProperty("personalIdIssuedDate")]
        public long? PersonalIdIssuedDate { get; set; }

        [JsonProperty("personalIdIssuedPlace")]
        public string PersonalIdIssuedPlace { get; set; }

        [JsonProperty("enlistedDate")]
        public object EnlistedDate { get; set; }

        [JsonProperty("soldierLevel")]
        public object SoldierLevel { get; set; }

        [JsonProperty("empType")]
        public string EmpType { get; set; }

        [JsonProperty("joinCompanyDate")]
        public object JoinCompanyDate { get; set; }

        [JsonProperty("taxNumber")]
        public object TaxNumber { get; set; }

        [JsonProperty("positionId")]
        public long PositionId { get; set; }

        [JsonProperty("organizationId")]
        public long OrganizationId { get; set; }

        [JsonProperty("positionName")]
        public string PositionName { get; set; }

        [JsonProperty("organizationName")]
        public object OrganizationName { get; set; }

        [JsonProperty("departmentName")]
        public string DepartmentName { get; set; }

        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("positionGrade")]
        public object PositionGrade { get; set; }

        [JsonProperty("positionFactor")]
        public object PositionFactor { get; set; }

        [JsonProperty("signContractDate")]
        public object SignContractDate { get; set; }

        [JsonProperty("positionDate")]
        public object PositionDate { get; set; }

        [JsonProperty("partyOfficialAdmissionDate")]
        public object PartyOfficialAdmissionDate { get; set; }

        [JsonProperty("currentOrganizationId")]
        public object CurrentOrganizationId { get; set; }

        [JsonProperty("empTypeId")]
        public long EmpTypeId { get; set; }

        [JsonProperty("empTypeName")]
        public object EmpTypeName { get; set; }

        [JsonProperty("saleCode")]
        public object SaleCode { get; set; }

        [JsonProperty("collectCallCode")]
        public object CollectCallCode { get; set; }

        [JsonProperty("accountNumber")]
        public object AccountNumber { get; set; }

        [JsonProperty("bank")]
        public object Bank { get; set; }

        [JsonProperty("bankBranch")]
        public object BankBranch { get; set; }

        [JsonProperty("salaryTableName")]
        public object SalaryTableName { get; set; }

        [JsonProperty("salaryScaleName")]
        public object SalaryScaleName { get; set; }

        [JsonProperty("salaryGradeName")]
        public object SalaryGradeName { get; set; }

        [JsonProperty("factor")]
        public object Factor { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("isLongLeave")]
        public object IsLongLeave { get; set; }

        [JsonProperty("percent")]
        public object Percent { get; set; }

        [JsonProperty("minimumMoney")]
        public object MinimumMoney { get; set; }

        [JsonProperty("isForeignEmployee")]
        public object IsForeignEmployee { get; set; }

        [JsonProperty("insuranceFactor")]
        public object InsuranceFactor { get; set; }

        [JsonProperty("positionType")]
        public object PositionType { get; set; }

        [JsonProperty("firstName")]
        public object FirstName { get; set; }

        [JsonProperty("middleName")]
        public object MiddleName { get; set; }

        [JsonProperty("lastName")]
        public object LastName { get; set; }

        [JsonProperty("aliasName")]
        public object AliasName { get; set; }

        [JsonProperty("createdTime")]
        public object CreatedTime { get; set; }

        [JsonProperty("modifiedTime")]
        public object ModifiedTime { get; set; }

        [JsonProperty("effectiveStartDate")]
        public object EffectiveStartDate { get; set; }

        [JsonProperty("effectiveEndDate")]
        public object EffectiveEndDate { get; set; }

        [JsonProperty("signedDate")]
        public object SignedDate { get; set; }

        [JsonProperty("contractDecisionNumber")]
        public object ContractDecisionNumber { get; set; }

        [JsonProperty("educationSubjectName")]
        public object EducationSubjectName { get; set; }

        [JsonProperty("degreeName")]
        public object DegreeName { get; set; }

        [JsonProperty("orgAddress")]
        public object OrgAddress { get; set; }

        [JsonProperty("contractMonth")]
        public object ContractMonth { get; set; }

        [JsonProperty("labourContractTypeId")]
        public object LabourContractTypeId { get; set; }

        [JsonProperty("cultureLevelName")]
        public object CultureLevelName { get; set; }

        [JsonProperty("isGHREmp")]
        public object IsGhrEmp { get; set; }

        [JsonProperty("attachmentFileId")]
        public object AttachmentFileId { get; set; }

        [JsonProperty("religionId")]
        public object ReligionId { get; set; }

        [JsonProperty("ethnicId")]
        public object EthnicId { get; set; }

        [JsonProperty("passportIssueDate")]
        public object PassportIssueDate { get; set; }

        [JsonProperty("invalidedSoldierLevelId")]
        public object InvalidedSoldierLevelId { get; set; }

        [JsonProperty("passportNumber")]
        public object PassportNumber { get; set; }

        [JsonProperty("nationId")]
        public object NationId { get; set; }

        [JsonProperty("soldierNumber")]
        public object SoldierNumber { get; set; }

        [JsonProperty("educationTypeId")]
        public object EducationTypeId { get; set; }

        [JsonProperty("sickSoldierLevelId")]
        public object SickSoldierLevelId { get; set; }

        [JsonProperty("maritalStatus")]
        public object MaritalStatus { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("countryCode2")]
        public object CountryCode2 { get; set; }

        [JsonProperty("countryCode3")]
        public object CountryCode3 { get; set; }

        [JsonProperty("reasonLongLeaveName")]
        public object ReasonLongLeaveName { get; set; }

        [JsonProperty("reasonLongLeaveId")]
        public object ReasonLongLeaveId { get; set; }

        [JsonProperty("leaveStartDate")]
        public object LeaveStartDate { get; set; }

        [JsonProperty("leaveEndDate")]
        public object LeaveEndDate { get; set; }

        [JsonProperty("isSickSoldier")]
        public object IsSickSoldier { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("numberOfPageElements")]
        public long NumberOfPageElements { get; set; }

        [JsonProperty("perPage")]
        public long PerPage { get; set; }

        [JsonProperty("totalPages")]
        public long TotalPages { get; set; }

        [JsonProperty("links")]
        public Link[] Links { get; set; }

        [JsonProperty("totalElements")]
        public long TotalElements { get; set; }
    }

    public partial class Link
    {
        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
