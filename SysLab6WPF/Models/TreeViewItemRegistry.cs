using Microsoft.Win32;
using System.Windows.Controls;

namespace SysLab6WPF.Models
{
    public class TreeViewItemRegistry : TreeViewItem
    {
        public string FullPath { get; set; }

        public RegistryKey RegistryKey { get; set; }

        public RegistryValueModel ValueModel { get; set; }
    }
}
