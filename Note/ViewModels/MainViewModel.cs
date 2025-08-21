using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Note.Helper;
using Note.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MessageBox = HandyControl.Controls.MessageBox;

namespace Note.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        public List<WorkEntity> works;

        [ObservableProperty]
        public WorkEntity work;

        public static string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        [ObservableProperty]
        string saveFilePath = Path.Combine(myDocumentsPath, "NoteApp");

        private RichTextBox _richEditBox;

        public bool IsCloseInfo { get; set; } = false;

        public void InitRichEdit(RichTextBox richEditBox)
        {
            _richEditBox = richEditBox;
        }

        [RelayCommand]
        private void OnLoaded()
        {
            FileHelper.TryCreateParentDir(SaveFilePath);
            FileHelper.CopyFile(SaveFilePath);
            LoadWorks();
        }


        // 当 Work 属性发生变化时，自动调用此方法
        partial void OnWorkChanged(WorkEntity? oldValue, WorkEntity? newValue)
        {
            //if (oldValue != null)
            //    SaveRtf($"{SaveFilePath}\\{oldValue.Title}.rtf");
            if (newValue != null)
            {
                // 当选中一个新的 Work 时，尝试加载它对应的 RTF 文件
                var path = $"{SaveFilePath}\\{newValue.Title}.rtf";
                if (File.Exists(path))
                {
                    LoadRtf(path);
                }
                else
                {
                    // 如果文件不存在，可以加载一个空的 RTF 或提示
                    LoadRtf($"{SaveFilePath}\\请在此输入标题.rtf");
                }
            }
            else
            {
                // 如果没有选中任何项，清空 RichTextBox
                LoadRtf($"{SaveFilePath}\\请在此输入标题.rtf");
            }
        }

        [RelayCommand]
        private void OnCreateWork()
        {
            CreateWork();
        }

        private void CreateWork()
        {
            var entity = new WorkEntity();
            entity.InsertDate = DateTime.Now;
            entity.Title = $"{DateTime.Now:MMddHHmmss}";
            //entity.Content = "请在此输入内容...";
            LoadRtf($"{SaveFilePath}\\请在此输入标题.rtf");
            SqlSugarHelper.Db.Insertable(entity).ExecuteCommand();
            LoadWorks();
        }

        private void LoadWorks()
        {
            Works = SqlSugarHelper.Db.Queryable<WorkEntity>().OrderByDescending(x => x.id).ToList();
            if (Works != null && Works.Any())
            {
                Work = Works.First();
            }
        }

        [RelayCommand]
        public void OnSaveWork()
        {
            if (Work == null) return;
            var result = SqlSugarHelper.Db.Queryable<WorkEntity>().Where(x => x.Title == Work.Title).ToList();
            SaveRtf($"{SaveFilePath}\\{Work.Title}.rtf");
            if (result.Count > 1)
            {
                MessageBox.Show("标题重复，请修改标题！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            SqlSugarHelper.Db.Updateable(Work).ExecuteCommand();
            if (!IsCloseInfo)
                MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 保存为 RTF 文件
        private void SaveRtf(string path)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    TextRange range = new TextRange(_richEditBox.Document.ContentStart, _richEditBox.Document.ContentEnd);
                    range.Save(stream, DataFormats.Rtf);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        // 从 RTF 文件加载
        private void LoadRtf(string path)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    TextRange range = new TextRange(_richEditBox.Document.ContentStart, _richEditBox.Document.ContentEnd);
                    range.Load(stream, DataFormats.Rtf);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        [RelayCommand]
        private void OnDeleteWork(WorkEntity entity)
        {
            if (entity == null)
                return;
            var result = MessageBox.Show("确定要删除吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SqlSugarHelper.Db.Deleteable(entity).ExecuteCommand();
                LoadWorks();
                if (System.IO.File.Exists($"{SaveFilePath}\\{entity.Title}.rtf"))
                {
                    System.IO.File.Delete($"{SaveFilePath}\\{entity.Title}.rtf");
                }
                MessageBox.Show("删除成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
