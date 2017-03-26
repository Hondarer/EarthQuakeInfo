using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace HondarerSoft.Utils
{
    /// <summary>
    /// <see cref="HttpClient"/> に関するユーティリティー機能を提供します。
    /// </summary>
    public class HttpClientUtil
    {
        /// <summary>
        /// 指定 URI に GET 要求を送信し、非同期操作で応答本体を文字列として返します。
        /// </summary>
        /// <param name="uri">要求の送信先 URI。</param>
        /// <param name="client">
        /// 要求に使用する <see cref="HttpClient"/> を指定します。省略可能です。
        /// 省略時は内部で <see cref="HttpClient"/> のインスタンスを生成します。
        /// ソケットに関する資源の問題から、本パラメータには呼び出し元で管理された静的な <see cref="HttpClient"/> のインスタンスを指定することを推奨します。
        /// </param>
        /// <returns><see cref="Task"/> を返します。非同期操作を表すタスク オブジェクト。</returns>
        public static async Task<string> SafeGetStringAsync(string uri, HttpClient client = null)
        {
            return await Task.Run(async () =>
            {
                HttpClient _client = client;
                string getString = null;

                if (client == null)
                {
                    // ここで生成された HttpClient オブジェクトは、メソッド終了時に Dispose される。
                    // その都度、ソケットは閉じて TIME_WAIT となるため、基本的には推奨されない。
                    _client = new HttpClient();
                }

                try
                {
                    getString = await _client.GetStringAsync(uri);
                }
                catch (ArgumentNullException ex) // uri is null
                {
                    Debug.WriteLine("{0}", ex.ToString());
                }
                catch (HttpRequestException ex) // Error
                {
                    Debug.WriteLine("{0}", ex.ToString());
                }
                catch (TaskCanceledException ex) // Timeout
                {
                    Debug.WriteLine("{0}", ex.ToString());
                }

                if (client == null)
                {
                    _client.Dispose();
                }

                return getString;
            });
        }

        /// <summary>
        /// 指定 URI に GET 要求を送信し、非同期操作で応答本体をバイト配列として返します。
        /// </summary>
        /// <param name="uri">要求の送信先 URI。</param>
        /// <param name="client">
        /// 要求に使用する <see cref="HttpClient"/> を指定します。省略可能です。
        /// 省略時は内部で <see cref="HttpClient"/> のインスタンスを生成します。
        /// ソケットに関する資源の問題から、本パラメータには呼び出し元で管理された静的な <see cref="HttpClient"/> のインスタンスを指定することを推奨します。
        /// </param>
        /// <returns><see cref="Task"/> を返します。非同期操作を表すタスク オブジェクト。</returns>
        public static async Task<byte[]> SafeGetByteArrayAsync(string uri, HttpClient client = null)
        {
            return await Task.Run(async () => 
            {
                HttpClient _client = client;
                byte[] getBytes = null;

                if (client == null)
                {
                    // ここで生成された HttpClient オブジェクトは、メソッド終了時に Dispose される。
                    // その都度、ソケットは閉じて TIME_WAIT となるため、基本的には推奨されない。
                    _client = new HttpClient();
                }

                try
                {
                    getBytes = await _client.GetByteArrayAsync(uri);
                }
                catch (ArgumentNullException ex) // uri is null
                {
                    Debug.WriteLine("{0}", ex.ToString());
                }
                catch (HttpRequestException ex) // Error
                {
                    Debug.WriteLine("{0}", ex.ToString());
                }
                catch (TaskCanceledException ex) // Timeout
                {
                    Debug.WriteLine("{0}", ex.ToString());
                }

                if (client == null)
                {
                    _client.Dispose();
                }

                return getBytes;
            });
        }
    }
}
