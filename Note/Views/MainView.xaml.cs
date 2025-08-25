using Microsoft.Win32;
using Note.Helper;
using Note.Models;
using Note.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Note.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        private MainViewModel vm;
        public MainView()
        {
            InitializeComponent();
            vm = new MainViewModel();
            DataContext = vm;
            int margin = 10;
            this.Left = SystemParameters.WorkArea.Width - this.Width - margin;
            this.Top = margin;
            InitData();
            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            int result = IniHelper.ReadSetting();
            AutoStart.IsChecked = result == 1 ? true : false;
            vm.InitRichEdit(RichEditBox);
        }

        private void InitData()
        {
            SqlSugarHelper.Db.DbMaintenance.CreateDatabase();
            SqlSugarHelper.Db.CodeFirst.InitTables<WorkEntity>();
        }

        #region 菜单栏事件
        // 加粗
        private void BtnBold_Click(object sender, RoutedEventArgs e)
        {
            var range = GetSelectedTextRange();
            if (range != null)
            {
                // 切换加粗状态
                if (range.GetPropertyValue(TextElement.FontWeightProperty).Equals(FontWeights.Bold))
                    range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                else
                    range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
        }

        // 下划线
        private void BtnUnderline_Click(object sender, RoutedEventArgs e)
        {
            var range = GetSelectedTextRange();
            if (range != null)
            {
                // 获取当前 TextDecorations
                var decorations = range.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
                if (decorations == null || !decorations.Contains(TextDecorations.Underline[0]))
                {
                    // 添加下划线
                    range.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
                }
                else
                {
                    // 移除下划线
                    range.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                }
            }
        }

        // 删除线（横线）
        private void BtnStrikethrough_Click(object sender, RoutedEventArgs e)
        {
            var range = GetSelectedTextRange();
            if (range != null)
            {
                var decorations = range.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;

                // 自定义删除线装饰
                var strikethrough = new TextDecoration
                {
                    Location = TextDecorationLocation.Strikethrough,
                    Pen = new Pen(Brushes.Black, 1) { DashStyle = new DashStyle() } // 实线删除线
                };

                if (decorations == null || !decorations.Any(d => d.Location == TextDecorationLocation.Strikethrough))
                {
                    // 添加删除线
                    var newDecorations = new TextDecorationCollection(decorations ?? new TextDecorationCollection());
                    newDecorations.Add(strikethrough);
                    range.ApplyPropertyValue(Inline.TextDecorationsProperty, newDecorations);
                }
                else
                {
                    // 获取当前的 TextDecorations（可能是 null）
                    var decorations2 = range.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;

                    // 创建一个新的集合，用于存放处理后的装饰
                    TextDecorationCollection newDecorations;

                    if (decorations2 != null)
                    {
                        // 手动创建一个新的集合，只添加 非删除线 的装饰
                        newDecorations = new TextDecorationCollection();

                        foreach (TextDecoration decoration in decorations2)
                        {
                            // 只保留不是删除线的装饰
                            if (decoration.Location != TextDecorationLocation.Strikethrough)
                            {
                                newDecorations.Add(decoration);
                            }
                            // 如果是删除线，则跳过不添加
                        }
                    }
                    else
                    {
                        // 如果原来没有装饰，就新建一个空集合
                        newDecorations = new TextDecorationCollection();
                    }

                    // 应用新的装饰集合到文本范围
                    range.ApplyPropertyValue(Inline.TextDecorationsProperty, newDecorations);
                }
            }
        }

        /// <summary>
        /// 斜体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnItalic_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前选中的文本范围
            TextRange range = GetSelectedTextRange();
            if (range != null)
            {
                // 获取当前字体样式
                var currentStyle = range.GetPropertyValue(TextElement.FontStyleProperty) as FontStyle?;

                if (currentStyle == FontStyles.Italic)
                {
                    // 如果已经是斜体，设置为正常
                    range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                }
                else
                {
                    // 否则设置为斜体
                    range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                }
            }
        }

        // 获取当前选中的文本范围
        private TextRange GetSelectedTextRange()
        {
            if (RichEditBox.Selection != null && !RichEditBox.Selection.IsEmpty)
            {
                return new TextRange(RichEditBox.Selection.Start, RichEditBox.Selection.End);
            }
            return null; // 没有选中任何内容
        }
        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            vm.IsCloseInfo = true;
            base.OnClosing(e);
            vm.OnSaveWork(vm.Work);
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            grid.Width = (grid.Width == 0) ? 200 : 0;
        }

        private void AutoStart_Click(object sender, RoutedEventArgs e)
        {
            RegistryHelper.RegistryAutoStart(AutoStart.IsChecked== true?true:false);
        }
    }
}
