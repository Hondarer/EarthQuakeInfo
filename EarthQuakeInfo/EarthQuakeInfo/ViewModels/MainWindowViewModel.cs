using HondarerSoft.Utils;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Media.Imaging;

namespace EarthQuakeInfo.ViewModels
{
    /// <summary>
    /// 地震情報に関する ViewModel を提供します。
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// HTTP のタイムアウト時間[秒]を表します。
        /// </summary>
        private const double HTTP_TIMEOUT = 10.0D;

        /// <summary>
        /// 地震情報を取得する <see cref="HttpClient"/> を保持します。
        /// </summary>
        private static HttpClient client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT)
        };

        /// <summary>
        /// タイトルを保持します。
        /// </summary>
        private string viewModelTitle = "EarthQuakeInfo";

        /// <summary>
        /// 地震情報の <see cref="BitmapImage"/> を保持します。
        /// </summary>
        private BitmapImage reportMapImage = null;

        /// <summary>
        /// 前回の ID を保持します。
        /// </summary>
        private string previousID = null;

        /// <summary>
        /// タイトルを取得または設定します。
        /// </summary>
        public string ViewModelTitle
        {
            get
            {
                return viewModelTitle;
            }
            set
            {
                SetProperty(ref viewModelTitle, value);
            }
        }

        /// <summary>
        /// 地震情報の <see cref="BitmapImage"/> を取得または設定します。
        /// </summary>
        public BitmapImage ReportMapImage
        {
            get
            {
                return reportMapImage;
            }
            set
            {
                SetProperty(ref reportMapImage, value);
            }
        }

#if false
        /// <summary>
        /// <see cref="MainWindowViewModel"/> クラスの静的な初期化をします。
        /// </summary>
        static MainWindowViewModel()
        {
            // proxy 認証データのセット
            // プログラムの開始時に設定が必要
            IWebProxy proxy = WebRequest.DefaultWebProxy;
            if (proxy != null)
            {
                proxy.Credentials = new NetworkCredential("USER", "PASSWORD");
            }
        }
#endif

        /// <summary>
        /// 内容の更新を行います。
        /// </summary>
        public async void Refresh()
        {
            ZishinInfomation zishinInfomation = await JsonUtil.JsonUriToObjectAsync<ZishinInfomation>(ZishinInfomation.BindingUri, client);
            if (zishinInfomation == null)
            {
                return;
            }

            ReportMapImage = await BitmapImageUtil.GetImageAsync(zishinInfomation.ReportMapUri, client);

            ViewModelTitle = string.Format("EarthQuakeInfo(Latest) - {0}", DateTime.Now);

            if (previousID != zishinInfomation.ID)
            {
                SpeechMSP speechMSP = new SpeechMSP();
                string speakContents = string.Format("{0:d日H時m分}頃、{1}、最大震度{2}、マグニチュード{3}、{4}、{5}",
                    zishinInfomation.QuakeDateTime,
                    zishinInfomation.Type,
                    zishinInfomation.MaxIntensity,
                    zishinInfomation.MagnitudeFloat,
                    zishinInfomation.Place,
                    string.Join("、", zishinInfomation.OtherInfo.ToArray()));
                Debug.WriteLine(speakContents);
                await speechMSP.SpeakAsync(speakContents);

                if (zishinInfomation.ObservationDetail != null)
                {
                    StringBuilder intensityDetail = new StringBuilder();

                    foreach (string pref in zishinInfomation.ObservationDetail.Keys)
                    {
                        if (pref == "茨城県")
                        {
                            foreach (string intensity in zishinInfomation.ObservationDetail[pref].Keys)
                            {
                                bool hitplace = false;

                                foreach (string place in zishinInfomation.ObservationDetail[pref][intensity])
                                {
                                    if (place.Contains("日立市") == true)
                                    {
                                        if (hitplace == false)
                                        {
                                            intensityDetail.AppendFormat("震度{0}、{1}", intensity, place);
                                            hitplace = true;
                                        }
                                        else
                                        {
                                            intensityDetail.AppendFormat("、{0}", place);
                                        }
                                    }
                                }

                                if (hitplace == true)
                                {
                                    intensityDetail.Append("。");
                                }
                            }
                        }
                    }

                    string intensityDetailString = intensityDetail.ToString();

                    Debug.Print(intensityDetailString);
                    await speechMSP.SpeakAsync(intensityDetailString);
                }
            }

            previousID = zishinInfomation.ID;
        }
    }
}
