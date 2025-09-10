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

        [ObservableProperty]
        public WorkEntity oldWork;

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
            var result = SqlSugarHelper.Db.Queryable<WorkEntity>().ToList();
            if (result.Count == 0)
            {
                CreateWork();
            }
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
            Work = null;
            Work = new WorkEntity();
            Work.Title = $"{DateTime.Now:MMddHHmmss}";
            LoadRtf($"{SaveFilePath}\\请在此输入标题.rtf");
        }

        private void LoadWorks()
        {
            Works = SqlSugarHelper.Db.Queryable<WorkEntity>().OrderByDescending(x => x.id).ToList();
            if (Works != null && Works.Any())
            {
                Work = Works.First();
                OldWork = Work?.DeepClone();
        }
        }

        [RelayCommand]
        public void OnSaveWork(WorkEntity entity)
        {
            if (Work == null) return;
            if (string.IsNullOrEmpty(Work.Title))
            {
                MessageBox.Show("标题不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Work.id == 0)
            {
                if (SqlSugarHelper.Db.Queryable<WorkEntity>().Any(x => x.Title == Work.Title))
                {
                    MessageBox.Show("标题已存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                entity.InsertDate = DateTime.Now;
                entity.Title = Work.Title;
                //entity.Content = "请在此输入内容...";          
                SqlSugarHelper.Db.Insertable(entity).ExecuteCommand();
                SaveRtf($"{SaveFilePath}\\{Work.Title}.rtf");
                LoadWorks();
            }
            else
            {
                SqlSugarHelper.Db.Updateable(Work).ExecuteCommand();
                if (System.IO.File.Exists($"{SaveFilePath}\\{OldWork.Title}.rtf")&&Work.id==OldWork.id)
                {
                    System.IO.File.Delete($"{SaveFilePath}\\{OldWork.Title}.rtf");
                }
                SaveRtf($"{SaveFilePath}\\{Work.Title}.rtf");
                LoadWorks();
            }
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

        [RelayCommand]
        void DoneWork()
        {
            if (Work == null) return;
            Work.Complete = Work.Complete == 0 ? 1 : 0;
            SqlSugarHelper.Db.Updateable(Work).ExecuteCommand();
        }
    }
}
