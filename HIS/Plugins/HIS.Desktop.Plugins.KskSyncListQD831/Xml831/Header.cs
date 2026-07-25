/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Xml831
{
    /// <summary>&lt;HEADER&gt; — thông tin phiên gửi (cấu hình cổng/hệ thống).</summary>
    public class Header
    {
        [XmlElement("MESSAGE_VERSION")]
        public string MessageVersion { get; set; }

        [XmlElement("SENDER_CODE")]
        public string SenderCode { get; set; }

        [XmlElement("SENDER_NAME")]
        public string SenderName { get; set; }

        [XmlElement("TRANSACTION_TYPE")]
        public string TransactionType { get; set; }

        [XmlElement("TRANSACTION_NAME")]
        public string TransactionName { get; set; }

        [XmlElement("TRANSACTION_DATE")]
        public string TransactionDate { get; set; }

        [XmlElement("TRANSACTION_ID")]
        public string TransactionId { get; set; }

        [XmlElement("REQUEST_ID")]
        public string RequestId { get; set; }

        [XmlElement("ACTION_TYPE")]
        public string ActionType { get; set; }
    }
}
