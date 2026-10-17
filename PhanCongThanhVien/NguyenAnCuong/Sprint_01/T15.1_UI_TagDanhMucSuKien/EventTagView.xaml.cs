using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CampusActivitiesManager.Pages.Controls
{
    public partial class EventTagView : ContentView
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(EventTagView), string.Empty, propertyChanged: (b, o, n) => ((EventTagView)b).UpdateDisplay());

        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create(nameof(Color), typeof(Color), typeof(EventTagView), Color.FromArgb("#512BD4"), propertyChanged: (b, o, n) => ((EventTagView)b).UpdateDisplay());

        public static readonly BindableProperty TagBackgroundProperty =
            BindableProperty.Create(nameof(DisplayColor), typeof(Brush), typeof(EventTagView), new SolidColorBrush(Color.FromArgb("#DFD8F7")));

        public static readonly BindableProperty TooltipTextProperty =
            BindableProperty.Create(nameof(TooltipText), typeof(string), typeof(EventTagView), string.Empty);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EventTagView), null);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(EventTagView), null);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public Brush DisplayColor
        {
            get => (Brush)GetValue(TagBackgroundProperty);
            set => SetValue(TagBackgroundProperty, value);
        }

        public string TooltipText
        {
            get => (string)GetValue(TooltipTextProperty);
            set => SetValue(TooltipTextProperty, value);
        }

        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public string FormattedTitle => Title?.Length > 15 ? Title[..13] + "..." : (Title ?? string.Empty);
        public Color TextColor => Color ?? Color.FromArgb("#512BD4");

        public EventTagView()
        {
            InitializeComponent();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            OnPropertyChanged(nameof(FormattedTitle));
            OnPropertyChanged(nameof(TextColor));
            if (string.IsNullOrEmpty(TooltipText))
            {
                TooltipText = $"Danh mục / Trạng thái: {Title}";
            }
        }

        private void OnTagTapped(object sender, TappedEventArgs e)
        {
            if (Command?.CanExecute(CommandParameter ?? Title) == true)
            {
                Command.Execute(CommandParameter ?? Title);
            }
        }
    }
}
