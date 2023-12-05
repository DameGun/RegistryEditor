using Microsoft.Win32;
using System.Linq;

namespace SysLab6WPF.Models
{
    public class RegistryValueModel
    {
        public string Name { get; set; }
        public string ViewName { get; set; }
        public RegistryValueKind Type { get; private set; }

        public object? Value { get; set; }
        public string ViewValue { get; set; }

        public RegistryValueModel(RegistryKey rk, string name)
        {
            Name = name;
            ViewName = name != "" ? name : "(Имя не присвоено)";

            Type = rk.GetValueKind(name);

            var data = rk.GetValue(name);

            switch (Type)
            {
                case RegistryValueKind.Binary:
                    Value = string.Join(" ", (data as byte[]).Select(x => x.ToString("X2")).ToArray());
                    ViewValue = Value.ToString();
                    break;
                case RegistryValueKind.MultiString:
                    Value = string.Join('\n', data as string[]);
                    ViewValue = string.Join(' ', data as string[]);
                    break;
                default:
                    Value = data.ToString();
                    ViewValue = Value.ToString();
                    break;
            }
        }
    }
}
