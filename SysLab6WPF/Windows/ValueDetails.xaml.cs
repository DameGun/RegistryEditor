using Microsoft.Win32;
using SysLab6WPF.Models;
using SysLab6WPF.Windows;
using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace SysLab6WPF
{
    /// <summary>
    /// Логика взаимодействия для ValueDetails.xaml
    /// </summary>
    public partial class ValueDetails : Window
	{
		private readonly string parameterType;

		public ValueDetails(RegistryValueModel? item)
		{
			InitializeComponent();
			if(item != null )
			{
				this.DataContext = item;
				parameterNameBox.Text = item.Name;
				parameterValueBox.Text = item.Value.ToString();
			}
		}

		public ValueDetails(string opType)
		{
			InitializeComponent();
			this.Title = "Создание параметра";
			parameterType = opType;
		}

		private void Accept_Click(object sender, RoutedEventArgs e)
		{
			var mainWindow = ((MainWindow)Application.Current.MainWindow);
			try
			{
				switch (parameterType)
				{
					case "createString":
						mainWindow.SelectedItem.RegistryKey.SetValue(parameterNameBox.Text, parameterValueBox.Text, RegistryValueKind.String);
						break;
					case "createBinary":
						var value = parameterValueBox.Text.Split(' ')
							.Select(x => Convert.ToByte(x, 16))
							.ToArray();

						mainWindow.SelectedItem.RegistryKey.SetValue(parameterNameBox.Text, value, RegistryValueKind.Binary);
						break;
					case "createInt":
						mainWindow.SelectedItem.RegistryKey.SetValue(parameterNameBox.Text, int.Parse(parameterValueBox.Text));
						break;
				}

				mainWindow.ValuesExplorer.Items.Add(new RegistryValueModel(mainWindow.SelectedItem.RegistryKey, parameterNameBox.Text));
				this.DialogResult = true;
			}
			catch
			{
				WarningWindow warningWindow = new WarningWindow("Ошибка создания:\nВыбранный тип параметра не совпадает с введенными данными");
				warningWindow.ShowDialog();
			}			
		}
	}
}
