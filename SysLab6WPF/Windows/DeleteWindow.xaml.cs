using System.Media;
using System.Windows;

namespace SysLab6WPF.Windows
{
	/// <summary>
	/// Логика взаимодействия для DeleteWindow.xaml
	/// </summary>
	public partial class DeleteWindow : Window
	{
		public DeleteWindow(string title, string message)
		{
			InitializeComponent();
			this.Title = title;
			MessageBlock.Text = message;
			SystemSounds.Beep.Play();
		}

		private void Accept_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
	}
}
