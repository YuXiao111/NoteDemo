using CommunityToolkit.Mvvm.ComponentModel;
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
        public DateTime? insertDate = null;
    }
}
