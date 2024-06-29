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
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.BedMapView.Config;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedMapView
{
	public partial class FormBedMapView : HIS.Desktop.Utility.FormBase
	{
		private Inventec.Desktop.Common.Modules.Module moduleData;
		private System.Globalization.CultureInfo cultureLang;
		private MOS.EFMODEL.DataModels.V_HIS_ROOM CurrentRoom;
		private List<V_HIS_BED> ListBed;
		private long TimeNow;
		private List<V_HIS_BED_ROOM_1> ListBedRoom;
		private V_HIS_BED_ROOM_1 BedRoom;

		private const int Dai = 10;
		private const int Rong = 20;
		private static int do_rong = 200;
		private static int do_cao = 130;

		public FormBedMapView()
		{
			InitializeComponent();
		}

		public FormBedMapView(Inventec.Desktop.Common.Modules.Module moduleData)
			: base(moduleData)
		{
			InitializeComponent();
			try
			{
				this.moduleData = moduleData;
				this.Text = moduleData.text;
				this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
				cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void FormBedMapView_Load(object sender, EventArgs e)
		{
			try
			{
				HisConfigCFG.LoadConfig();

				LoadKeysFromlanguage();

				LoadCurrentDepartmentData();

				SetDefaultValueControl();

				InitDataCombo();

				FillDataToControl(BedRoom.ID);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadCurrentDepartmentData()
		{
			try
			{
				var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
				if (room != null)
				{
					this.CurrentRoom = room;
					HisBedRoomView1Filter filter = new HisBedRoomView1Filter();
					filter.DEPARTMENT_ID = room.DEPARTMENT_ID;
                    filter.IS_ACTIVE = 1;
					 ListBedRoom = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_BED_ROOM_1>>("api/HisBedRoom/GetView1", ApiConsumer.ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, null);

					if (ListBedRoom != null && ListBedRoom.Count > 0)
					{
						ListBedRoom = ListBedRoom.OrderBy(o => o.BED_ROOM_CODE).ToList();
						ListBed = BackendDataWorker.Get<V_HIS_BED>().Where(o => ListBedRoom.Select(s => s.ID).Contains(o.BED_ROOM_ID) && o.IS_ACTIVE == 1).ToList();
						BedRoom = ListBedRoom.FirstOrDefault(o => o.ROOM_ID == this.moduleData.RoomId);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void InitDataCombo()
		{
			try
			{
				//int rong = panelControlRoom.Width;
				int rong = xtraScrollableControl1.Width;
				int dai = 60;
				for (int i = 0; i < ListBedRoom.Count; i++)
				{
					var seat = new Base.Bed();

					seat.Text = "  " + ListBedRoom[i].BED_ROOM_NAME + "(" + Convert.ToInt64(ListBedRoom[i].PATIENT_COUNT) + "/" + Convert.ToInt64(ListBedRoom[i].BED_COUNT) + ")";
					seat.Font = new Font(seat.Font.Name, 16);
					seat.ForeColor = Color.White;
					if (ListBedRoom[i].PATIENT_COUNT >= ListBedRoom[i].BED_COUNT)
					{
						seat.BackColor = Color.Red;
					}
					else
					{
						seat.BackColor = Color.YellowGreen;
					}
					seat.Tag = ListBedRoom[i].ID;
					//seat.BackgroundImage = imageCollection.Images[1];
					seat.TextAlign = ContentAlignment.MiddleCenter;
					seat.BackgroundImageLayout = ImageLayout.Stretch;
					seat.GridX = 0;
					seat.GridY = i;
					seat.Location = new System.Drawing.Point(seat.GridX * rong, seat.GridY * dai + (seat.GridY > 0 ? (seat.GridY) * 5 : 0));
					seat.Size = new System.Drawing.Size(rong, dai);
					seat.TabIndex = 0;
					seat.TabStop = false;
					seat.Click += new EventHandler(Seat_Click);

					this.xtraScrollableControl1.Controls.Add(seat);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void Seat_Click(object sender, EventArgs e)
		{
			try
			{
				Base.Bed ic = (Base.Bed)sender;
				if (ic != null) FillDataToControl((long)ic.Tag);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadKeysFromlanguage()
		{
			try
			{
				//this.LciRoom.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_MAP_VIEW__LCI_ROOM");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private string GetLanguageControl(string key)
		{
			return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
		}

		private void SetDefaultValueControl()
		{
			try
			{
				this.TimeNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void FillDataToControl(long roomId)
		{
			try
			{
				long bedRoomId = 0;
				bedRoomId = roomId;
				ChangeColorChooseRoom(roomId);
				this.xtraScrollablePanel.Controls.Clear();
				if (bedRoomId > 0)
				{
                    var lstBed = ListBed.Where(o => o.BED_ROOM_ID == bedRoomId).ToList();
                    //var lstBed = lstBedData.Where(o => o.IS_ACTIVE == 1).ToList();
					if (lstBed != null && lstBed.Count > 0)
					{
						List<V_HIS_BED_LOG_4> listbedcurr = new List<V_HIS_BED_LOG_4>();
						MOS.Filter.HisBedLogView4Filter filter = new MOS.Filter.HisBedLogView4Filter();
						filter.FINISH_TIME_FROM__OR__NULL = this.TimeNow;
						filter.START_TIME_TO = this.TimeNow;
						filter.BED_ROOM_ID = roomId;
						var apiresult = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_BED_LOG_4>>("api/HisBedLog/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, null);
						if (apiresult != null && apiresult.Count > 0)
						{
							listbedcurr = apiresult.Where(o => lstBed.Select(s => s.ID).Contains(o.BED_ID)).ToList();
						}

						InitBedRoom(lstBed, listbedcurr);
					}
					else
					{
						MessageBox.Show("Buồng chưa được thiết lập giường");
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ChangeColorChooseRoom(long roomId)
		{
			try
			{
				if (this.xtraScrollableControl1.Controls != null)
				{
					foreach (var item in this.xtraScrollableControl1.Controls)
					{
						if (item is Base.Bed)
						{
							var seat = item as Base.Bed;
							if (roomId == (long)seat.Tag)
							{
								//seat.BackColor = Color.YellowGreen;								
								seat.Font = new Font(seat.Font.Name, 16, FontStyle.Italic);
							}
							else
							{
								seat.Font = new Font(seat.Font.Name, 16, FontStyle.Regular);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void InitBedRoom(List<V_HIS_BED> lstBed, List<V_HIS_BED_LOG_4> listbedcurr)
		{
			try
			{
				if (lstBed != null && lstBed.Count > 0)
				{
					lstBed = lstBed.OrderBy(o => o.BED_CODE).ToList();

					var LstBedHasXY = lstBed.Where(o => !String.IsNullOrWhiteSpace(o.X) || !String.IsNullOrWhiteSpace(o.Y)).ToList();
					var LstBedNew = lstBed.Where(o => String.IsNullOrWhiteSpace(o.X) && String.IsNullOrWhiteSpace(o.Y)).ToList();

					if (LstBedHasXY != null && LstBedHasXY.Count > 0)
					{
						AddItemToScrollControl(LstBedHasXY, listbedcurr);
						int maxX = LstBedHasXY.Max(o => Inventec.Common.TypeConvert.Parse.ToInt32(o.X));
						int maxY = LstBedHasXY.Max(o => Inventec.Common.TypeConvert.Parse.ToInt32(o.Y));

						if (LstBedNew != null && LstBedNew.Count() > 0)
						{
							int start = 0;
							int count = LstBedNew.Count;
							int newY = maxY + 1;

							while (count > 0)
							{
								int dem = 0;
								int newX = maxX + 1;
								int limit = (count <= newX) ? count : newX;
								var listSub = LstBedNew.Skip(start).Take(limit).ToList();
								foreach (var item in listSub)
								{
									item.X = (dem++) + "";
									item.Y = newY + "";
								}
								AddItemToScrollControl(listSub, listbedcurr);
								newY++;
								start += maxX + 1;
								count -= maxX + 1;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void AddItemToScrollControl(List<V_HIS_BED> lstBed, List<V_HIS_BED_LOG_4> listbedcurr)
		{
			try
			{
				if (lstBed == null || lstBed.Count == 0)
				{
					return;
				}
				foreach (var bed in lstBed)
				{
					var seat = new Base.Bed();

					seat.Font = new Font(seat.Font.Name, 12);
					seat.TextAlign = ContentAlignment.MiddleLeft;
					seat.BackgroundImage = imageCollection.Images[6];
					seat.BackgroundImageLayout = ImageLayout.Center;
					//seat.BackColor = Color.RosyBrown;

					seat.ForeColor = Color.White;

					List<V_HIS_BED_LOG_4> bedLog = new List<V_HIS_BED_LOG_4>();
					if (listbedcurr != null && listbedcurr.Count > 0)
					{
						bedLog = listbedcurr.Where(o => o.BED_ID == bed.ID).ToList();
					}

					if (HisConfigCFG.SHOW_INFO_OPTION == "1")
					{
						seat.Text = bed.BED_CODE + " - " + bed.BED_NAME + "\n\r";

						long namDon = 0;
						long? namGhepCount = 0;
						if (bedLog != null && bedLog.Count > 0)
						{
							namDon = bedLog.Where(o => o.SHARE_COUNT == null || o.SHARE_COUNT == 0).Count();

							var namGhep = bedLog.Where(o => o.SHARE_COUNT > 0).OrderByDescending(p => p.SHARE_COUNT);

							if (namGhep != null && namGhep.Count() > 0)
							{
								namGhepCount = namGhep.FirstOrDefault().SHARE_COUNT;
							}

							//seat.BackColor = Color.Red;
							seat.BackgroundImage = imageCollection.Images[2];
						}
						seat.Text += "Nằm đơn: " + namDon + ", Nằm ghép: " + namGhepCount;
					}
					else
					{
						seat.Text = bed.BED_CODE + " - " + bed.BED_NAME + "\n\r";

						if (bedLog != null && bedLog.Count > 0)
						{
							List<string> names = new List<string>();
							var groupByPatient = bedLog.GroupBy(o => o.TDL_PATIENT_CODE).ToList();
							foreach (var item in groupByPatient)
							{
								names.Add(string.Format("  {0}", item.FirstOrDefault().TDL_PATIENT_NAME));
							}
							seat.Text += String.Join("\n\r", names);
							//seat.BackColor = Color.Red;
							seat.BackgroundImage = imageCollection.Images[2];
						}
					}


					int x = 0;
					int y = 0;
					x = Inventec.Common.TypeConvert.Parse.ToInt32(bed.X);
					y = Inventec.Common.TypeConvert.Parse.ToInt32(bed.Y);

					//if (x <= 0 && y <= 0)// không có tọa độ được thiết lập
					//{
					//    GetMinXY(ref x, ref y);
					//}
					//else if (x > 0 && y > 0)// có tọa độ thiết lập
					//{
					//    //x--; y--;
					//}
					//else if (x <= 0 && y > 0)// có 1 tọa độ;
					//{
					//    //x = 0; y--;
					//    x = 0;
					//}
					//else if (x > 0 && y <= 0)// có 1 tọa độ;
					//{
					//    //x--; y = 0;
					//    y = 0;
					//}

					seat.GridX = x;
					seat.GridY = y;
					seat.Location = new System.Drawing.Point(seat.GridX * do_rong + (seat.GridX > 0 ? (seat.GridX) * 10 : 0), seat.GridY * do_cao);
					seat.Size = new System.Drawing.Size(do_rong, do_cao);
					seat.TabIndex = 0;
					seat.TabStop = false;
					xtraScrollablePanel.Controls.Add(seat);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void GetMinXY(ref int minX, ref int minY)
		{
			try
			{
				Base.Bed[,] ChoNgoi = new Base.Bed[Dai + 1, Rong + 1];
				int maxX = 0;
				int maxY = 0;
				foreach (var item in this.xtraScrollablePanel.Controls)
				{
					if (item is Base.Bed)
					{
						var a = item as Base.Bed;
						ChoNgoi[a.GridX, a.GridY] = a;
						if (a.GridY > maxY)
						{
							maxY = a.GridY;
						}

						if (a.GridX > maxX)
						{
							maxX = a.GridX;
						}
					}
				}

				for (int j = 0; j <= maxY; j++)
				{
					minY = j;
					for (int i = 0; i <= maxX; i++)
					{
						minX = i;

						if (ChoNgoi[minX, minY] == null) break;
						else minX += 1;
						if (minX > Dai) break;
						if (ChoNgoi[minX, minY] == null) break;
					}

					if (minX > Dai)
					{
						minX = 1;
						minY += 1;
					}

					if (minY > Rong)
					{
						if (minX > Dai)
						{
							minX = (int)Dai + 1;
							minY = (int)Rong + 1;
						}
					}

					if (ChoNgoi[minX, minY] == null) break;
				}
			}
			catch (Exception ex)
			{
				minX = 0;
				minY = 0;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
