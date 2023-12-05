using System.Media;
using System.Windows;

namespace SysLab6WPF.Windows
{
	/// <summary>
	/// Логика взаимодействия для WarningWindow.xaml
	/// </summary>
	public partial class WarningWindow : Window
	{
		public WarningWindow(string message)
		{
			InitializeComponent();
			MessageBlock.Text = message;
			SystemSounds.Beep.Play();
		}
	}
}
