using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace HondarerSoft.Utils
{
    /// <summary>
    /// <see cref="System.Runtime.Serialization.Json"/> に関するユーティリティー機能を提供します。
    /// </summary>
    public class JsonUtil
    {
        /// <summary>
        /// json をオブジェクトにデシリアライズします。
        /// </summary>
        /// <typeparam name="T">デシリアライズしたいオブジェクトの型。</typeparam>
        /// <param name="json">デシリアライズしたい json。</param>
        /// <param name="deserializedObject">デシリアライズしたオブジェクト。デシリアライズに失敗した場合は <c>null</c> を返します。</param>
        /// <param name="settings">デシリアライズに使用する <see cref="DataContractJsonSerializerSettings"/> を指定します。省略可能です。</param>
        /// <returns>デシリアライズに成功した場合は <c>true</c>、失敗した場合は <c>false</c> を返します。</returns>
        public static bool JsonToObject<T>(string json, out T deserializedObject, DataContractJsonSerializerSettings settings = null) where T : class, new()
        {
            deserializedObject = null;

            if (string.IsNullOrEmpty(json) != true)
            {
                try
                {
                    deserializedObject = new T();
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        if (settings == null)
                        {
                            settings = DefaultSettings();
                        }
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), settings);
                        deserializedObject = serializer.ReadObject(ms) as T;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("{0}", ex.ToString());
                    deserializedObject = null;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 指定 URI から json を GET し、オブジェクトにデシリアライズします。
        /// </summary>
        /// <typeparam name="T">デシリアライズしたいオブジェクトの型。</typeparam>
        /// <param name="uri">デシリアライズしたい json を示す URI。</param>
        /// <param name="client">
        /// 要求に使用する <see cref="HttpClient"/> を指定します。省略可能です。
        /// 省略時は内部で <see cref="HttpClient"/> のインスタンスを生成します。
        /// ソケットに関する資源の問題から、本パラメータには呼び出し元で管理された static な <see cref="HttpClient"/> のインスタンスを指定することを推奨します。
        /// </param>
        /// <param name="settings">デシリアライズに使用する <see cref="DataContractJsonSerializerSettings"/> を指定します。省略可能です。</param>
        /// <returns>デシリアライズに成功した場合はオブジェクト、失敗した場合は <c>null</c> を返します。</returns>
        public static Task<T> JsonUriToObjectAsync<T>(string uri, HttpClient client = null, DataContractJsonSerializerSettings settings = null) where T : class, new()
        {
            return Task.Run(() => 
            {
                string json = HttpClientUtil.SafeGetStringAsync(uri, client).Result;
                Debug.WriteLine(json);

                if (string.IsNullOrEmpty(json) != true)
                {
                    T deserializedObject = null;

                    try
                    {
                        deserializedObject = new T();
                        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                        {
                            if (settings == null)
                            {
                                settings = DefaultSettings();
                            }
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), settings);
                            deserializedObject = serializer.ReadObject(ms) as T;

                            return deserializedObject;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("{0}", ex.ToString());
                        return null;
                    }
                }

                return null;
            });
        }

        /// <summary>
        /// オブジェクトを json にシリアライズします。
        /// </summary>
        /// <typeparam name="T">シリアライズしたいオブジェクトの型。</typeparam>
        /// <param name="serializeObject">シリアライズしたいオブジェクト。</param>
        /// <param name="settings">シリアライズに使用する <see cref="DataContractJsonSerializerSettings"/> を指定します。省略可能です。</param>
        /// <returns>シリアライズされた json。変換に失敗した場合は <c>null</c> を返します。</returns>
        public static string ObjectToJson<T>(T serializeObject, DataContractJsonSerializerSettings settings = null) where T : class
        {
            string json = null;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (settings == null)
                    {
                        settings = DefaultSettings();
                    }
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), settings);
                    serializer.WriteObject(ms, serializeObject);
                    json = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0}", ex.ToString());
                return null;
            }

            return json;
        }

        /// <summary>
        /// このクラスで利用する既定の <see cref="DataContractJsonSerializerSettings"/> を返します。
        /// </summary>
        /// <returns>このクラスで利用する既定の <see cref="DataContractJsonSerializerSettings"/>。</returns>
        private static DataContractJsonSerializerSettings DefaultSettings()
        {
            return new DataContractJsonSerializerSettings()
            {
                // Dictionary の扱いを有効にする
                UseSimpleDictionaryFormat = true
            };
        }
    }
}
