using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Models
{
    public partial class WorkEntity : ObservableObject
    {

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        [ObservableProperty]
        public string title = string.Empty;

        //[ObservableProperty]
        //public string content = string.Empty;

        [ObservableProperty]
        public int complete = 0;

        [ObservableProperty]
        public DateTime? insertDate = null;

        // 深拷贝方法
        public WorkEntity DeepClone()
        {
            // 序列化当前对象为JSON字符串，再反序列化为新对象
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<WorkEntity>(json);
        }
    }
}
