using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace SysLab6WPF
{
	/// <summary>
	/// Логика взаимодействия для CreateKeyWindow.xaml
	/// </summary>
	public partial class CreateKeyWindow : Window
	{
		public CreateKeyWindow()
		{
			InitializeComponent();
		}

		private void Accept_Click(object sender, RoutedEventArgs e)
		{
			var mainWindow = ((MainWindow)Application.Current.MainWindow);

			try
			{
				RegistryKey? createdKey = mainWindow.SelectedItem.RegistryKey.CreateSubKey(createKeyNameBox.Text);
				if (createdKey != null)
				{
					//mainWindow.TreeViewItemRegistryBuff.RegistryKey.Close();

					var createdKeyView = mainWindow.AddTreeViewKey(createKeyNameBox.Text,
						Path.Combine(mainWindow.SelectedItem.FullPath, createdKey.Name),
						ref createdKey);

					mainWindow.SelectedItem.Items.Add(createdKeyView);
				}
				this.DialogResult = true;
			}
			catch
			{
				this.DialogResult = false;
			}
			
		}
	}
}
