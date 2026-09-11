/* IVT
 * @Project : hisnguonmo
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPS.Processor.Mps000007.ADO
{
    /// <summary>
    /// Mot dong = mot phong thuoc khoa dang in.
    /// Dung cho vung lap ClsByRooms tren mau MPS000007.
    /// </summary>
    public class ClsByRoomADO
    {
        public long ROOM_ID { get; set; }
        public string ROOM_NAME { get; set; }
        //Ma cac dich vu CLS do phong nay chi dinh - CUNG THU TU, CUNG SO LUONG voi CLS_NAMES
        public string CLS_CODES { get; set; }
        //Ten cac dich vu CLS do phong nay chi dinh, noi bang dau ; va da loai trung
        public string CLS_NAMES { get; set; }
        //Tom tat ket qua CLS cua lan kham tai phong nay
        public string SUBCLINICAL { get; set; }
    }
}
