using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HondarerSoft.Utils
{
    /// <summary>
    /// <see cref="Dispatcher"/> の支援機能を提供します。
    /// </summary>
    public class DispatcherHelper
    {
        /// <summary>
        /// バックグラウンドスレッドにて、引数に与えられた <see cref="Action"/> を実行します。
        /// <see para="action"/> 内にて <see cref="Dispatcher"/> が生成された場合、処理を返す際にその <see cref="Dispatcher"/> はシャットダウンされます。
        /// </summary>
        /// <param name="action">処理したい <see cref="Action"/>。</param>
        /// <remarks>
        /// 本メソッドを経由しない方法で、バックグラウンドスレッドで <see cref="DispatcherObject"/> を生成する操作は行わないでください。
        /// バックグラウンドスレッドで <see cref="Dispatcher"/> を生成し処置を行わない場合、深刻なメモリー リークを引き起こします。
        /// </remarks>
        public static void InvokeBackground(Action action)
        {
            ThreadStart threadStart = () =>
            {
                try
                {
                    action();
                }
                finally
                {
                    Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);
                    Dispatcher.Run();
                }
            };

            Thread thread = new Thread(threadStart)
            {
                Name = string.Format("DispatcherSafeAction from {0}", Thread.CurrentThread.ManagedThreadId),
                IsBackground = true
            };

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        /// <summary>
        /// バックグラウンドスレッドにて、引数に与えられた <see cref="Action"/> を実行する <see cref="Task"/> を返します。
        /// <see para="action"/> 内にて <see cref="Dispatcher"/> が生成された場合、処理を返す際にその <see cref="Dispatcher"/> はシャットダウンされます。
        /// </summary>
        /// <param name="action">処理したい <see cref="Action"/>。</param>
        /// <returns>処理したい <see cref="Action"/> の完了を待ち合わせる <see cref="Task"/>。</returns>
        /// <remarks>
        /// 本メソッドを経由しない方法で、バックグラウンドスレッドで <see cref="DispatcherObject"/> を生成する操作は行わないでください。
        /// バックグラウンドスレッドで <see cref="Dispatcher"/> を生成し処置を行わない場合、深刻なメモリー リークを引き起こします。
        /// </remarks>
        public static Task BeginInvokeBackground(Action action)
        {
            return Task.Run(() =>
            {
                InvokeBackground(action);
            });
        }
    }
}
