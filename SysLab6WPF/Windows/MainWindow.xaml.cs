using Microsoft.Win32;
using SysLab6WPF.Models;
using SysLab6WPF.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SysLab6WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		public TreeViewItemRegistry SelectedItem;
		public TreeViewItemRegistry SelectedParentItem;

		public RegistryValueModel SelectedParameter;

		public MainWindow()
		{
			InitializeComponent();
			Main();
		}

		public void Main()
		{
			foreach(var hiveName in Enum.GetNames(typeof(RegistryHive)).Where(t => t != "PerformanceData").ToArray()) {
				RegistryKey rkBase = RegistryKey.OpenBaseKey(Enum.Parse<RegistryHive>(hiveName), RegistryView.Default);

				var rootKey = AddTreeViewKey(rkBase.Name, rkBase.Name, ref rkBase);

				KeysExplorer.Items.Add(rootKey);
			}
		}

		public TreeViewItemRegistry AddTreeViewKey(string name, string path, ref RegistryKey rk)
		{
			TreeViewItemRegistry key = new TreeViewItemRegistry();
			key.Name = name;
			key.FullPath = path;
			key.Header = name;
			key.Expanded += TreeItem_Expanded;
			key.Selected += TreeItem_Selected;
			key.RegistryKey = rk;

			ShowTreeViewExpandButton(ref key, rk);

			return key;
		}

		public void ShowTreeViewExpandButton(ref TreeViewItemRegistry item, RegistryKey rk)
		{
			if (rk.SubKeyCount > 0)
			{
				item.Items.Add(new TreeViewItem());
			}
		}

		private ItemsControl GetSelectesTreeViewItemParent(TreeViewItemRegistry item)
		{
			DependencyObject parent = VisualTreeHelper.GetParent(item);

			while(!(parent is TreeViewItemRegistry || parent is TreeView))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}

			return parent as ItemsControl;
		}

		public bool CheckChildrenItemsValid(TreeViewItemRegistry item)
		{
			if (item.Items.Count > 0)
			{
				if (item.Items[0].GetType() != typeof(TreeViewItemRegistry))
				{
					item.Items.Clear();
					return true;
				}
			}
			return false;
		}

		public void TreeItem_Selected(object sender, RoutedEventArgs e)
		{
			var item = sender as TreeViewItemRegistry;

			ValuesExplorer.Items.Clear();

			if (item != null)
			{
				CreateKeyButton.IsEnabled = true;
				DeleteKeyButton.IsEnabled = true;
				SelectedItem = item;
				ItemsControl parent = GetSelectesTreeViewItemParent(item);

				searchBox.Text = item.FullPath;

				SelectedParentItem = parent as TreeViewItemRegistry;

				string[] values = item.RegistryKey.GetValueNames();

				if (values.Length > 0)
				{
					for (int i = 0; i < values.Length; i++)
					{
						ValuesExplorer.Items.Add(new RegistryValueModel(item.RegistryKey, values[i]));
					}
				}
			}

			e.Handled = true;
		}

		public void TreeItem_Expanded(object sender, RoutedEventArgs e)
		{
			var item = sender as TreeViewItemRegistry;

			if (item != null)
			{
				if (!CheckChildrenItemsValid(item)) return;

				string[] keys = item.RegistryKey.GetSubKeyNames();

				for (int i = 0; i < keys.Length; i++)
				{
					try
					{
						RegistryKey? buffSubKey = item.RegistryKey.OpenSubKey(keys[i], true);
						if (buffSubKey != null)
						{
							string path = Path.Combine(item.FullPath, keys[i]);
							var subKey = AddTreeViewKey(keys[i], path, ref buffSubKey);
							item.Items.Add(subKey);
						}
					}
					catch
					{
						Debug.WriteLine("Access Denied");
					}
				}
			}			
		}

		private void Value_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			//var item = sender as ListViewItem;

			////ValueDetails valueDetails = new ValueDetails(((List<RegistryValueModel>)item.Content).FirstOrDefault());
			//ValueDetails valueDetails = new ValueDetails((RegistryValueModel)item.Content);

			//valueDetails.ShowDialog();
		}

		private void CreateKeyButton_Click(object sender, RoutedEventArgs e)
		{
			CreateKeyWindow createKeyWindow = new CreateKeyWindow();

			createKeyWindow.ShowDialog();
		}

		private void DeleteKeyButton_Click(object sender, RoutedEventArgs e)
		{
			DeleteWindow deleteWindow = new DeleteWindow("Подтверждение удаление раздела", "Вы действительно хотите удалить этот раздел и все его подразделы?");

			deleteWindow.ShowDialog();

			if(deleteWindow.DialogResult == false)
			{
				e.Handled = true;
				return;
			}

			SelectedParentItem.RegistryKey.DeleteSubKeyTree(SelectedItem.Header.ToString());

			SelectedParentItem.Items.Remove(SelectedItem);
		}

		private void CreateParameterButton_Click(object sender, RoutedEventArgs e)
		{
			var item = sender as MenuItem;

			if(item != null && SelectedItem != null)
			{
				ValueDetails createParameterWindow = new ValueDetails(item.Name);

				createParameterWindow.ShowDialog();
			}
		}

		private void DeleteParameterButton_Click(object sender, RoutedEventArgs e)
		{
			DeleteWindow deleteWindow = new DeleteWindow("Удаление параметра", "Вы действительно хотите безвозвратно удалить этот параметр?");

			deleteWindow.ShowDialog();

			if (deleteWindow.DialogResult == false)
			{
				e.Handled = true;
				return;
			}

			SelectedItem.RegistryKey.DeleteValue(SelectedParameter.Name);

			ValuesExplorer.Items.Remove(SelectedParameter);
		}

		private void ListViewItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			var item = sender as ListViewItem;
			
			if(item != null)
			{
				var parameter = item.Content as RegistryValueModel;

				SelectedParameter = parameter;
			}
		}
	}
}
