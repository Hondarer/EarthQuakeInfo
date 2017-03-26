using EarthQuakeInfo.ViewModels;
using System;
using System.Windows;
using System.Windows.Threading;

namespace EarthQuakeInfo.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 表示更新用のタイマーを保持します。
        /// </summary>
        private DispatcherTimer timer = null;

        /// <summary>
        /// <see cref="MainWindow"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 要素のレイアウトやレンダリングが完了し、操作を受け入れる準備が整ったときに発生します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">ルーティング イベントに関連付けられている状態情報とイベント データ。</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            timer.Tick += Timer_Tick;

            // first fire
            Timer_Tick(this, new EventArgs());

            timer.Start();
        }

        /// <summary>
        /// 表示更新用のタイマーの間隔が経過したときに発生します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データを格納していないオブジェクト。</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            ((MainWindowViewModel)DataContext).Refresh();
        }

        /// <summary>
        /// 読み込まれた要素の要素ツリーから要素が削除されたときに発生します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">ルーティング イベントに関連付けられている状態情報とイベント データ。</param>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timer = null;
        }
    }
}
