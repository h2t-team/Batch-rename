using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename.DataTypes
{
    public class FileUI : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Preview { get; set; }
        public string Status { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
