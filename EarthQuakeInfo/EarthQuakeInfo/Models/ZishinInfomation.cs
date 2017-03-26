using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EarthQuakeInfo
{
    /// <summary>
    /// 地震情報を提供します。
    /// </summary>
    [DataContract]
    public class ZishinInfomation
    {
        //{
        //    "AnnounceDateTime": "2017年3月11日 21:21", 
        //    "AnnounceUnixTime": 1489234860, 
        //    "Correction": false, 
        //    "Depth": "約10km", 
        //    "EEW": "", 
        //    "EEWFlag": false, 
        //    "EventID": "20170311211843", 
        //    "ID": "58c3ebe20bd9394d3600007d", 
        //    "Magnitude": "M2.2", 
        //    "MagnitudeFloat": 2.2, 
        //    "MaxIntensity": "1", 
        //    "Number": 1, 
        //    "ObservationCityList": {
        //        "鹿児島県": {
        //            "1": [
        //                "鹿児島市"
        //            ]
        //        }
        //    }, 
        //    "ObservationDetailList": {
        //        "鹿児島県": {
        //            "1": [
        //                "鹿児島市喜入町"
        //            ]
        //        }
        //    }, 
        //    "ObservationList": {
        //        "鹿児島県": {
        //            "1": [
        //                "鹿児島県薩摩"
        //            ]
        //        }
        //    }, 
        //    "OtherInfo": [
        //        "この地震による津波の心配はありません。"
        //    ], 
        //    "Place": "鹿児島湾", 
        //    "PlaceDetail": null, 
        //    "Point": {
        //        "Lat": "北緯31.4", 
        //        "LatInt": 31.4, 
        //        "Long": "東経130.6", 
        //        "LongInt": 130.6
        //    }, 
        //    "QuakeDateTime": "2017年3月11日 21:18", 
        //    "QuakeUnixTime": 1489234680, 
        //    "ReportDetailMap": true, 
        //    "ReportMap": true, 
        //    "Tsunami": "この地震による津波の心配はありません。", 
        //    "TsunamiFlag": false, 
        //    "Type": "震源・震度に関する情報", 
        //    "Version": 4, 
        //    "body_md5": "aa7b587d4b549731bbc011e09e0bd484"
        //}

        /// <summary>
        /// このクラスが対応する URI を表します。
        /// </summary>
        private const string BINDING_URI = "http://zish.in/api/quake.json";

        /// <summary>
        /// ReportMapUri のフォーマットを表します。
        /// </summary>
        private const string REPORTMAP_URI_FORMAT = "http://images.zish.in/{0}.png";

        /// <summary>
        /// 対応する URI を取得します。
        /// </summary>
        public static string BindingUri
        {
            get
            {
                return BINDING_URI;
            }
        }

        /// <summary>
        /// EventID を取得または設定します。
        /// </summary>
        [DataMember]
        public string EventID { get; set; }

        /// <summary>
        /// ID を取得または設定します。
        /// </summary>
        [DataMember]
        public string ID { get; set; }

        /// <summary>
        /// MagnitudeFloat を取得または設定します。
        /// </summary>
        [DataMember]
        public float MagnitudeFloat { get; set; }

        /// <summary>
        /// MaxIntensity を取得または設定します。
        /// </summary>
        [DataMember]
        public float MaxIntensity { get; set; }

        /// <summary>
        /// ObservationCity
        /// </summary>
        [DataMember(Name = "ObservationCityList")]
        public Dictionary<string, Dictionary<string, List<string>>> ObservationCity { get; set; }

        /// <summary>
        /// ObservationDetail を取得または設定します。
        /// </summary>
        [DataMember(Name = "ObservationDetailList")]
        public Dictionary<string, Dictionary<string, List<string>>> ObservationDetail { get; set; }

        /// <summary>
        /// OtherInfo を取得または設定します。
        /// </summary>
        [DataMember]
        public List<string> OtherInfo { get; set; }

        /// <summary>
        /// Place を取得または設定します。
        /// </summary>
        [DataMember]
        public string Place { get; set; }

        /// <summary>
        /// QuakeDateTime を取得または設定します。
        /// </summary>
        public DateTime QuakeDateTime { get; set; }

        /// <summary>
        /// 整合用の QuakeDateTime を取得または設定します。
        /// </summary>
        [DataMember(Name = "QuakeDateTime")]
        private string _QuakeDateTime
        {
            get
            {
                return QuakeDateTime.ToString();
            }
            set
            {
                DateTime quakeDateTime = default(DateTime);
                if (DateTime.TryParse(value, out quakeDateTime) == true)
                {
                    QuakeDateTime = quakeDateTime;
                }
            }
        }

        /// <summary>
        /// ReportMap を取得または設定します。
        /// </summary>
        [DataMember]
        public bool ReportMap { get; set; }

        /// <summary>
        /// ReportMapUri を取得します。
        /// </summary>
        public string ReportMapUri
        {
            get
            {
                if (ReportMap == false)
                {
                    return null;
                }

                return string.Format(string.Format(REPORTMAP_URI_FORMAT, EventID));
            }
        }

        /// <summary>
        /// Type を取得または設定します。
        /// </summary>
        [DataMember]
        public string Type { get; set; }
    }
}
