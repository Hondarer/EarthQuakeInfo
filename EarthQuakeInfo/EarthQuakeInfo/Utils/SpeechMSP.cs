// Reference
// お前の血は何色だ!! 4 - C# で日本語合成音声・音声認識をやってみよう。
// http://d.hatena.ne.jp/rti7743/20111215/1323965483

using SpeechLib;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace HondarerSoft.Utils
{
    /// <summary>
    /// Microsoft Speech Object Library を利用した音声合成機能を提供します。
    /// </summary>
    public class SpeechMSP
    {
        /// <summary>
        /// 
        /// </summary>
        private static SpeechLib.SpVoice spVoice = null;

        /// <summary>
        /// 
        /// </summary>
        static SpeechMSP()
        {
            try
            {
                spVoice = new SpeechLib.SpVoice();
            }
            catch (Exception ex)
            {
                Console.WriteLine("音声合成エンジンの初期化に失敗しました。\r\n{0}", ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SpeechMSP()
        {
            // NOP
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public SpeechMSP(string description) : this()
        {
            SetAudioOutput(description);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public void SetAudioOutput(string description)
        {
            if (spVoice == null)
            {
                return;
            }

            foreach (SpObjectToken audioOutput in spVoice.GetAudioOutputs())
            {
                string targetDescription = audioOutput.GetDescription();

                // 先頭一致で再生デバイスの名称を特定する
                // 再生デバイスの名称は、コントロールパネルから適切なものにあらかじめ変更しておく。
                if ((targetDescription.Length >= description.Length) && (targetDescription.Substring(0, description.Length) == description))
                {
                    spVoice.AudioOutput = audioOutput;
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="culture"></param>
        public Task SpeakAsync(string str, string culture = null)
        {
            if (spVoice == null)
            {
                return null;
            }

            return Task.Run(() =>
            {
                while (spVoice.Status.RunningState == SpeechRunState.SRSEIsSpeaking)
                {
                    // 現在話し中..
                    Thread.Sleep(100);
                }

                string targetLocale; // 16 進表記のロケール ID
                if (culture == null)
                {
                    // 未指定時は、現在の UI カルチャー
                    targetLocale = string.Format("{0:x}", CultureInfo.CurrentUICulture.LCID);
                }
                else
                {
                    // 指定時は、指定されたカルチャーのロケール ID
                    targetLocale = string.Format("{0:x}", new CultureInfo(culture, false).LCID);
                }

                bool isHitEngine = false;

                // 合成音声エンジンで指定言語を話す人を探す。
                foreach (SpObjectToken voiceperson in spVoice.GetVoices())
                {
                    string language = voiceperson.GetAttribute("Language");
                    string gender = voiceperson.GetAttribute("Gender");
                    if (language == targetLocale)
                    {
                        // 指定言語を話す人だ!
                        spVoice.Voice = voiceperson;
                        isHitEngine = true;
                        break;
                    }
                }

                if (isHitEngine == true)
                {
                    spVoice.Speak(str, SpeechVoiceSpeakFlags.SVSFIsXML);
                }
                else
                {
                    // 誰も話してくれなかった。
                    Console.WriteLine("音声合成が利用できません。");
                }
            });
        }
    }
}
