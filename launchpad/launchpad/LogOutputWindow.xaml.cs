using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace launchpad
{
    public partial class LogOutputWindow
    {
        public enum LogType
        {
            LOG,
            ERROR
        };

        private readonly string _filePrefix;
        private string _content;

        public LogOutputWindow(string content, LogType logType)
        {
            _filePrefix = logType switch
            {
                LogType.LOG => "log",
                LogType.ERROR => "error",
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
            _content = content;
            InitializeComponent();

            Title = logType switch
            {
                LogType.LOG => "Log Output Viewer",
                LogType.ERROR => "Error Output Viewer",
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
            SetupFlowDocument();
        }

        private void SetupFlowDocument()
        {
            var paragraph = new Paragraph()
            {
                FontFamily = new FontFamily("Consolas"),
                FontSize = 14,
            };
            doc.Blocks.Add(paragraph);

            var textBlock = new TextBlock
            {
                Text = _content,
                FontFamily = paragraph.FontFamily,
                FontSize = paragraph.FontSize
            };
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            doc.PageWidth = textBlock.DesiredSize.Width + 50;
            while (textBlock.Inlines.Count > 0)
            {
                paragraph.Inlines.Add(textBlock.Inlines.FirstInline);
            }
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".log",
                FileName = $"{_filePrefix}-{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            var response = dialog.ShowDialog();
            if (!response.HasValue || !response.Value)
            {
                return;
            }
            File.WriteAllText(dialog.FileName, _content);
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
