/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Khoi phuc tu ban deploy MPS.Processor.Mps000158.dll (file nguon khong co trong git).
 */
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Common.Repository;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000158.ADO
{
	public class HeinServiceTypeADO
	{
		public long? ID { get; set; }

		public int ROW_POS { get; set; }

		public string HEIN_SERVICE_TYPE_CODE { get; set; }

		public string HEIN_SERVICE_TYPE_NAME { get; set; }

		public decimal? TOTAL_PRICE_HEIN_SERVICE_TYPE { get; set; }

		public decimal? TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE { get; set; }

		public decimal? TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE { get; set; }

		public decimal? TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE { get; set; }
	}
	public class PatientADO
	{
		public string VIR_ADDRESS { get; set; }

		public string VIR_PATIENT_NAME { get; set; }

		public long DOB { get; set; }

		public string AGE { get; set; }

		public string DOB_STR { get; set; }

		public string CMND_DATE_STR { get; set; }

		public string DOB_YEAR { get; set; }

		public string GENDER_MALE { get; set; }

		public string GENDER_FEMALE { get; set; }

		public string GENDER_NAME { get; set; }
	}
	public class PatyAlterBhytADO : HIS_PATIENT_TYPE_ALTER
	{
		public string PATIENT_TYPE_NAME { get; set; }

		public string HEIN_CARD_NUMBER_SEPARATE { get; set; }

		public string IS_HEIN { get; set; }

		public string IS_VIENPHI { get; set; }

		public string STR_HEIN_CARD_FROM_TIME { get; set; }

		public string STR_HEIN_CARD_TO_TIME { get; set; }

		public string RATIO { get; set; }

		public string HEIN_CARD_NUMBER_1 { get; set; }

		public string HEIN_CARD_NUMBER_2 { get; set; }

		public string HEIN_CARD_NUMBER_3 { get; set; }

		public string HEIN_CARD_NUMBER_4 { get; set; }

		public string HEIN_CARD_NUMBER_5 { get; set; }

		public string HEIN_CARD_NUMBER_6 { get; set; }

		public long TIME_IN_TREATMENT { get; set; }

		public string RATIO_STR { get; set; }
	}
	public class SereServADO : SereServKey
	{
		public SereServADO(HIS_SERE_SERV data, List<HIS_SERE_SERV_EXT> sereServExts, List<HIS_HEIN_SERVICE_TYPE> heinServiceTypes, List<V_HIS_SERVICE> services, List<V_HIS_ROOM> rooms, List<HIS_MATERIAL_TYPE> materialTypes, List<HIS_PATIENT_TYPE_ALTER> patyAlters)
		{
			try
			{
				PropertyInfo[] array = Properties.Get<HIS_SERE_SERV>();
				PropertyInfo[] array2 = array;
				foreach (PropertyInfo propertyInfo in array2)
				{
					propertyInfo.SetValue(this, propertyInfo.GetValue(data));
				}
				if (heinServiceTypes != null && heinServiceTypes.Count > 0 && services != null && services.Count > 0)
				{
					V_HIS_SERVICE service = services.FirstOrDefault((V_HIS_SERVICE o) => o.ID == data.SERVICE_ID);
					if (service != null)
					{
						HIS_HEIN_SERVICE_TYPE hIS_HEIN_SERVICE_TYPE = heinServiceTypes.FirstOrDefault((HIS_HEIN_SERVICE_TYPE o) => o.ID == service.HEIN_SERVICE_TYPE_ID);
						base.SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
						base.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
						base.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
						base.SERVICE_NAME = service.SERVICE_NAME;
						base.SERVICE_CODE = service.SERVICE_CODE;
						base.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
						base.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
						base.ACTIVE_INGR_BHYT_CODE = service.ACTIVE_INGR_BHYT_CODE;
						base.ACTIVE_INGR_BHYT_NAME = service.ACTIVE_INGR_BHYT_NAME;
						base.HEIN_SERVICE_BHYT_CODE = service.HEIN_SERVICE_BHYT_CODE;
						base.HEIN_SERVICE_BHYT_NAME = service.HEIN_SERVICE_BHYT_NAME;
						base.CONCENTRA = service.CONCENTRA;
						HIS_SERE_SERV_EXT hIS_SERE_SERV_EXT = (from o in sereServExts?.Where((HIS_SERE_SERV_EXT o) => o.SERE_SERV_ID == data.ID)
							orderby o.CREATE_TIME descending
							select o).FirstOrDefault();
						if (hIS_SERE_SERV_EXT != null && hIS_SERE_SERV_EXT.NUMBER_OF_FILM.GetValueOrDefault() > 0)
						{
							base.NUMBER_OF_FILM = hIS_SERE_SERV_EXT.NUMBER_OF_FILM;
						}
						else
						{
							base.NUMBER_OF_FILM = service.NUMBER_OF_FILM;
						}
						if (hIS_HEIN_SERVICE_TYPE != null)
						{
							base.HEIN_SERVICE_TYPE_ID = hIS_HEIN_SERVICE_TYPE.ID;
							base.HEIN_SERVICE_TYPE_NUM_ORDER = hIS_HEIN_SERVICE_TYPE.NUM_ORDER;
							base.HEIN_SERVICE_TYPE_CODE = hIS_HEIN_SERVICE_TYPE.HEIN_SERVICE_TYPE_CODE;
							base.HEIN_SERVICE_TYPE_NAME = hIS_HEIN_SERVICE_TYPE.HEIN_SERVICE_TYPE_NAME;
						}
					}
				}
				if (rooms != null && rooms.Count > 0)
				{
					V_HIS_ROOM v_HIS_ROOM = rooms.FirstOrDefault((V_HIS_ROOM o) => o.ID == data.TDL_EXECUTE_ROOM_ID);
					if (v_HIS_ROOM != null)
					{
						base.EXECUTE_ROOM_CODE = v_HIS_ROOM.ROOM_CODE;
						base.EXECUTE_ROOM_NAME = v_HIS_ROOM.ROOM_NAME;
					}
				}
				base.PRICE_BHYT = PriceBHYTProcess(data, patyAlters, materialTypes);
				base.TOTAL_PRICE_BHYT = base.PRICE_BHYT * base.AMOUNT;
				base.TOTAL_PRICE_PATIENT_SELF = base.TOTAL_PRICE_BHYT - base.VIR_TOTAL_PATIENT_PRICE_BHYT.GetValueOrDefault() - base.VIR_TOTAL_HEIN_PRICE.GetValueOrDefault();
				if (base.VIR_TOTAL_HEIN_PRICE.HasValue)
				{
					base.TOTAL_HEIN_PRICE_ONE_AMOUNT = base.VIR_TOTAL_HEIN_PRICE.Value / base.AMOUNT;
				}
				base.RADIO_SERIVCE = ((!(base.ORIGINAL_PRICE > 0m)) ? 0m : (base.HEIN_LIMIT_PRICE.HasValue ? (base.HEIN_LIMIT_PRICE.Value / base.ORIGINAL_PRICE * 100m) : (base.PRICE / base.ORIGINAL_PRICE * 100m)));
				if (base.HEIN_LIMIT_PRICE.HasValue && base.HEIN_LIMIT_PRICE < base.VIR_PRICE)
				{
					base.PRICE_CO_PAYMENT = base.VIR_PRICE - (decimal?)base.HEIN_LIMIT_PRICE.Value;
				}
			}
			catch (Exception ex)
			{
				LogSystem.Warn(ex);
			}
		}

		public decimal PriceBHYTProcess(HIS_SERE_SERV sereServ, List<HIS_PATIENT_TYPE_ALTER> patyAlters, List<HIS_MATERIAL_TYPE> materialTypes)
		{
			decimal result = default(decimal);
			try
			{
				if (patyAlters != null && patyAlters.Count > 0 && !string.IsNullOrEmpty(sereServ.HEIN_CARD_NUMBER))
				{
					HIS_PATIENT_TYPE_ALTER hIS_PATIENT_TYPE_ALTER = patyAlters.FirstOrDefault((HIS_PATIENT_TYPE_ALTER o) => !string.IsNullOrEmpty(o.HEIN_CARD_NUMBER) && o.HEIN_CARD_NUMBER.Equals(sereServ.HEIN_CARD_NUMBER));
					if (hIS_PATIENT_TYPE_ALTER != null)
					{
						decimal? vIR_TOTAL_HEIN_PRICE = base.VIR_TOTAL_HEIN_PRICE;
						if ((vIR_TOTAL_HEIN_PRICE.GetValueOrDefault() > default(decimal)) & vIR_TOTAL_HEIN_PRICE.HasValue)
						{
							if (sereServ.HEIN_LIMIT_PRICE.HasValue)
							{
								return sereServ.HEIN_LIMIT_PRICE.Value;
							}
							HIS_MATERIAL_TYPE hIS_MATERIAL_TYPE = materialTypes.FirstOrDefault((HIS_MATERIAL_TYPE o) => o.SERVICE_ID == sereServ.SERVICE_ID && o.IS_STENT == 1);
							return (hIS_MATERIAL_TYPE != null) ? (sereServ.ORIGINAL_PRICE * (1m + sereServ.VAT_RATIO)) : sereServ.VIR_PRICE_NO_ADD_PRICE.Value;
						}
						result = default(decimal);
					}
				}
			}
			catch (Exception ex)
			{
				result = default(decimal);
				LogSystem.Warn(ex);
			}
			return result;
		}
	}
	public class SereServKey : HIS_SERE_SERV
	{
		public long SERVICE_TYPE_ID { get; set; }

		public string SERVICE_TYPE_CODE { get; set; }

		public string SERVICE_TYPE_NAME { get; set; }

		public string SERVICE_CODE { get; set; }

		public string SERVICE_NAME { get; set; }

		public string SERVICE_UNIT_CODE { get; set; }

		public string SERVICE_UNIT_NAME { get; set; }

		public long? HEIN_SERVICE_TYPE_ID { get; set; }

		public string HEIN_SERVICE_TYPE_NAME { get; set; }

		public string HEIN_SERVICE_TYPE_CODE { get; set; }

		public long? HEIN_SERVICE_TYPE_NUM_ORDER { get; set; }

		public string EXECUTE_ROOM_NAME { get; set; }

		public string EXECUTE_ROOM_CODE { get; set; }

		public string HEIN_SERVICE_BHYT_CODE { get; set; }

		public string HEIN_SERVICE_BHYT_NAME { get; set; }

		public string ACTIVE_INGR_BHYT_CODE { get; set; }

		public string ACTIVE_INGR_BHYT_NAME { get; set; }

		public string CONCENTRA { get; set; }

		public long? NUMBER_OF_FILM { get; set; }

		public int ROW_POS { get; set; }

		public decimal PRICE_BHYT { get; set; }

		public decimal TOTAL_PRICE_BHYT { get; set; }

		public decimal TOTAL_PRICE_PATIENT_SELF { get; set; }

		public decimal RADIO_SERIVCE { get; set; }

		public decimal? TOTAL_PRICE_DEPARTMENT { get; set; }

		public decimal? TOTAL_PATIENT_PRICE_DEPARTMENT { get; set; }

		public decimal? TOTAL_HEIN_PRICE_DEPARTMENT { get; set; }

		public decimal? TOTAL_PRICE_ROOM { get; set; }

		public decimal? TOTAL_PATIENT_PRICE_ROOM { get; set; }

		public decimal? TOTAL_HEIN_PRICE_ROOM { get; set; }

		public decimal? TOTAL_HEIN_PRICE_ONE_AMOUNT { get; set; }

		public decimal? PRICE_CO_PAYMENT { get; set; }
	}
}