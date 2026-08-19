using System.Globalization;
using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Utilities
{
    public class BoolToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b && parameter is string options)
            {
                var parts = options.Split('|');
                if (parts.Length == 2)
                    return b ? parts[0] : parts[1];
            }
            return value?.ToString();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class IntToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int intVal)
                return intVal > 0;
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class RoleFilterActiveConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string current = value?.ToString() ?? string.Empty;
            string target = parameter?.ToString() ?? string.Empty;

            bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);

            if (isSelected)
            {
                return target switch
                {
                    UserRoles.Admin => Color.FromArgb("#FF3366"),
                    UserRoles.Manager => Color.FromArgb("#3068DF"),
                    UserRoles.Student => Color.FromArgb("#107C41"),
                    _ => Color.FromArgb("#512BD4") // Primary
                };
            }

            return Application.Current?.RequestedTheme == AppTheme.Dark 
                ? Color.FromArgb("#404040") 
                : Color.FromArgb("#ACACAC");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class RoleSelectedStrokeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string current = value?.ToString() ?? string.Empty;
            string target = parameter?.ToString() ?? string.Empty;

            bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
            if (isSelected)
            {
                return target switch
                {
                    UserRoles.Admin => Color.FromArgb("#FF3366"),
                    UserRoles.Manager => Color.FromArgb("#3068DF"),
                    UserRoles.Student => Color.FromArgb("#107C41"),
                    _ => Color.FromArgb("#512BD4")
                };
            }

            return Colors.Transparent;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class RoleEqualConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string current = value?.ToString() ?? string.Empty;
            string target = parameter?.ToString() ?? string.Empty;

            return string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter is string role)
            {
                return role;
            }
            return Binding.DoNothing;
        }
    }
}
