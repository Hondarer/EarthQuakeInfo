using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HondarerSoft.Utils
{
    /// <summary>
    /// <see cref="BitmapImage"/> に関するユーティリティー機能を提供します。
    /// </summary>
    public class BitmapImageUtil
    {
        /// <summary>
        /// バイト配列を <see cref="BitmapImage"/> に変換します。
        /// この操作は UI スレッドで実行する必要があります。非 UI スレッドで実行すると、深刻なリソース リークを引き起こします。
        /// </summary>
        /// <remarks>
        /// http://pierre3.hatenablog.com/entry/2015/10/25/001207
        /// </remarks>
        /// <param name="bytes">バイト配列。</param>
        /// <param name="freezing">生成した <see cref="BitmapImage"/> を変更不可能にするかどうか。省略可能です。既定値は <c>true</c> です。</param>
        /// <returns>生成した <see cref="BitmapImage"/>。変換に失敗した場合は <c>null</c> を返します。</returns>
        public static BitmapImage CreateBitmap(byte[] bytes, bool freezing = true)
        {
            if (bytes == null)
            {
                return null;
            }

            // MemoryStream を生で使うと、BitmapImage 生成後も内部バッファを解放しないため、リソースの利用量が多くなる。
            // WrappingStream を用いて、Dispose 時に Stream のバッファを解放する。
            // この処理を UI スレッド以外で行うと、ATOM テーブルに登録が残ってしまい、リソースがリークする。
            using (WrappingStream stream = new WrappingStream(new MemoryStream(bytes)))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    if (freezing == bitmap.CanFreeze)
                    {
                        bitmap.Freeze();
                    }
                    return bitmap;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("{0}", ex.ToString());
                    return null;
                }
            }
        }

        /// <summary>
        /// バイト配列を指定された <see cref="Dispatcher"/> 上で <see cref="BitmapImage"/> に非同期変換します。
        /// </summary>
        /// <param name="bytes">バイト配列。</param>
        /// <param name="freezing">生成した <see cref="BitmapImage"/> を変更不可能にするかどうか。省略可能です。既定値は <c>true</c> です。</param>
        /// <param name="dispatcherForBitmapCreation"><see cref="BitmapImage"/> に変換する際に使用する <see cref="Dispatcher"/>。省略可能です。</param>
        /// <returns>生成した <see cref="BitmapImage"/>を返す非同期操作。変換に失敗した場合は <c>null</c> を返します。</returns>
        public static async Task<BitmapImage> CreateBitmapAsync(byte[] bytes, bool freezing = true, Dispatcher dispatcherForBitmapCreation = null)
        {
            if (dispatcherForBitmapCreation == null)
            {
                dispatcherForBitmapCreation = System.Windows.Application.Current.Dispatcher;
            }

            return await dispatcherForBitmapCreation.InvokeAsync(() => CreateBitmap(bytes, freezing));
        }

        /// <summary>
        /// 指定された URI からデータを読み取り、<see cref="BitmapImage"/> を返す非同期動作を返します。
        /// </summary>
        /// <param name="uri">画像の URI。</param>
        /// <param name="client">
        /// 要求に使用する <see cref="HttpClient"/> を指定します。省略可能です。
        /// 省略時は内部で <see cref="HttpClient"/> のインスタンスを生成します。
        /// ソケットに関する資源の問題から、本パラメータには呼び出し元で管理された静的な <see cref="HttpClient"/> のインスタンスを指定することを推奨します。
        /// </param>
        /// <param name="dispatcherForBitmapCreation"><see cref="BitmapImage"/> に変換する際に使用する <see cref="Dispatcher"/>。省略可能です。</param>
        /// <returns><see cref="BitmapImage"/> を返す非同期操作。失敗した場合は <c>null</c> を返します。</returns>
        public static async Task<BitmapImage> GetImageAsync(string uri, HttpClient client = null, Dispatcher dispatcherForBitmapCreation = null)
        {
            if (string.IsNullOrEmpty(uri) == true)
            {
                return null;
            }

            byte[] bytes = await HttpClientUtil.SafeGetByteArrayAsync(uri, client).ConfigureAwait(false);
            return await CreateBitmapAsync(bytes, true, dispatcherForBitmapCreation);
        }
    }
}
