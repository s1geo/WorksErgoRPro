using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WorksErgoRPro.Biomechanics;
using WorksErgoRPro.ViewModels;

namespace WorksErgoRPro.Views
{
    public class WorksErgoPaneView : UserControl
    {
        public WorksErgoPaneView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Background = new SolidColorBrush(Color.FromRgb(245, 247, 250));

            var scroll = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };

            var root = new StackPanel { Margin = new Thickness(10) };
            scroll.Content = root;
            Content = scroll;

            // 1. Header Banner
            var headerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(0, 0, 0, 10)
            };
            var headerStack = new StackPanel();
            var title = new TextBlock
            {
                Text = "WORKS ERGO R-PRO",
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 15
            };
            var subtitle = new TextBlock
            {
                Text = "Offline Biomechanics & InteliPose\u2122 Suite",
                Foreground = new SolidColorBrush(Color.FromRgb(189, 195, 199)),
                FontSize = 11,
                Margin = new Thickness(0, 2, 0, 0)
            };
            headerStack.Children.Add(title);
            headerStack.Children.Add(subtitle);
            headerBorder.Child = headerStack;
            root.Children.Add(headerBorder);

            // 2. High-Impact DCR Dashboard Card
            var dcrCard = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 224, 230)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 10)
            };
            var dcrStack = new StackPanel();

            var dcrHeader = new TextBlock
            {
                Text = "OVERALL DEMAND/CAPACITY RATIO (DCR)",
                FontWeight = FontWeights.SemiBold,
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141))
            };
            dcrStack.Children.Add(dcrHeader);

            var dcrVal = new TextBlock
            {
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 4, 0, 2)
            };
            dcrVal.SetBinding(TextBlock.TextProperty, new Binding("OverallDcrText"));
            dcrVal.SetBinding(TextBlock.ForegroundProperty, new Binding("RiskBrush"));
            dcrStack.Children.Add(dcrVal);

            var riskBadge = new TextBlock
            {
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 4)
            };
            riskBadge.SetBinding(TextBlock.TextProperty, new Binding("RiskCategory"));
            riskBadge.SetBinding(TextBlock.ForegroundProperty, new Binding("RiskBrush"));
            dcrStack.Children.Add(riskBadge);

            var limitText = new TextBlock
            {
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(85, 85, 85))
            };
            var limitBind = new Binding("LimitingFactor") { StringFormat = "Limiting Factor: {0}" };
            limitText.SetBinding(TextBlock.TextProperty, limitBind);
            dcrStack.Children.Add(limitText);

            dcrCard.Child = dcrStack;
            root.Children.Add(dcrCard);

            // 3. DHM Anthropometry GroupBox
            var dhmGroup = CreateGroupBox("Digital Human Model (DHM)");
            var dhmStack = new StackPanel();

            var dhmCombo = new ComboBox
            {
                Margin = new Thickness(0, 4, 0, 8),
                ItemsSource = Enum.GetValues(typeof(DHMPercentile))
            };
            dhmCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedPercentile") { Mode = BindingMode.TwoWay });
            dhmStack.Children.Add(new TextBlock { Text = "Worker Population Percentile:", FontSize = 11 });
            dhmStack.Children.Add(dhmCombo);

            var taskCombo = new ComboBox
            {
                Margin = new Thickness(0, 4, 0, 4),
                ItemsSource = Enum.GetValues(typeof(TaskType))
            };
            taskCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedTask") { Mode = BindingMode.TwoWay });
            dhmStack.Children.Add(new TextBlock { Text = "Operation Type:", FontSize = 11 });
            dhmStack.Children.Add(taskCombo);

            var techCombo = new ComboBox
            {
                Margin = new Thickness(0, 4, 0, 4),
                ItemsSource = Enum.GetValues(typeof(LiftingTechnique))
            };
            techCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedTechnique") { Mode = BindingMode.TwoWay });
            dhmStack.Children.Add(new TextBlock { Text = "Lifting Technique (Leg Mechanics):", FontSize = 11 });
            dhmStack.Children.Add(techCombo);

            dhmGroup.Content = dhmStack;
            root.Children.Add(dhmGroup);

            // 4. Geometry & Load Controls GroupBox
            var taskGroup = CreateGroupBox("Task Parameters");
            var taskStack = new StackPanel();

            taskStack.Children.Add(CreateSliderRow("Load Mass (kg):", "LoadWeightKg", 0.5, 50.0, 0.5, "{0:F1} kg"));
            taskStack.Children.Add(CreateSliderRow("Reach Distance H (mm):", "ReachMm", 200.0, 850.0, 10.0, "{0:F0} mm"));
            taskStack.Children.Add(CreateSliderRow("Vertical Height V (mm):", "VerticalMm", 100.0, 1600.0, 10.0, "{0:F0} mm"));
            taskStack.Children.Add(CreateSliderRow("Lift Frequency (lifts/min):", "FrequencyLiftsPerMin", 0.1, 15.0, 0.5, "{0:F1}/min"));

            taskGroup.Content = taskStack;
            root.Children.Add(taskGroup);

            // 5. Biomechanical Breakdown GroupBox
            var bioGroup = CreateGroupBox("7-Axis Biomechanical Breakdown");
            var bioStack = new StackPanel();

            bioStack.Children.Add(CreateMetricRow("Lumbar L5/S1 Compression:", "LumbarCompN", "{0:F0} N (Limit 3400 N)"));
            bioStack.Children.Add(CreateMetricRow("Lumbar DCR:", "LumbarDCR", "{0:F2}"));
            bioStack.Children.Add(CreateMetricRow("Knee Flexion Angle:", "KneeFlexionDeg", "{0:F1}\u00b0"));
            bioStack.Children.Add(CreateMetricRow("Knee Joint Moment:", "KneeMomentNm", "{0:F1} Nm"));
            bioStack.Children.Add(CreateMetricRow("Center of Pressure (CofP):", "CenterOfPressureMm", "{0:F1} mm from ankle"));
            bioStack.Children.Add(CreateMetricRow("NIOSH Rec. Weight (RWL):", "NioshRWLKg", "{0:F1} kg"));
            bioStack.Children.Add(CreateMetricRow("NIOSH Lifting Index (LI):", "NioshLI", "{0:F2}"));
            bioStack.Children.Add(CreateMetricRow("Snook MAWL (Liberty Mutual):", "SnookMAWLKg", "{0:F1} kg"));
            bioStack.Children.Add(CreateMetricRow("Arm AFF Capacity DCR:", "ArmDCR", "{0:F2}"));
            bioStack.Children.Add(CreateMetricRow("RULA / REBA Posture Score:", "RulaScore", "{0} / REBA Score"));

            bioGroup.Content = bioStack;
            root.Children.Add(bioGroup);

            // 6. Actionable Engineering Recommendation
            var recGroup = CreateGroupBox("Engineering Guidance");
            var recBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(254, 249, 231)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(249, 231, 159)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 4, 0, 4)
            };
            var recText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(125, 102, 8)),
                FontWeight = FontWeights.Medium
            };
            recText.SetBinding(TextBlock.TextProperty, new Binding("Recommendation"));
            recBorder.Child = recText;
            recGroup.Content = recBorder;
            root.Children.Add(recGroup);

            // 7. Action Buttons
            var btnStack = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };

            var recalcBtn = new Button
            {
                Content = "Recalculate Posture Profile",
                Height = 30,
                FontWeight = FontWeights.SemiBold,
                Background = new SolidColorBrush(Color.FromRgb(41, 128, 185)),
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 6)
            };
            recalcBtn.Click += (s, e) => (DataContext as WorksErgoPaneViewModel)?.Recalculate();
            btnStack.Children.Add(recalcBtn);

            var pickBtn = new Button
            {
                Content = "Pick Object from 3D Scene",
                Height = 28,
                Background = new SolidColorBrush(Color.FromRgb(52, 73, 94)),
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 6)
            };
            pickBtn.Click += (s, e) =>
            {
                MessageBox.Show(
                    "3D Raycast Picker active. Select any box, part or operator in the Р-Про 3D viewport to synchronize coordinates.",
                    "Works Ergo 3D Grabber",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            };
            btnStack.Children.Add(pickBtn);

            root.Children.Add(btnStack);
        }

        private GroupBox CreateGroupBox(string header)
        {
            return new GroupBox
            {
                Header = header,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 8),
                Padding = new Thickness(6)
            };
        }

        private UIElement CreateSliderRow(string label, string propName, double min, double max, double tick, string format)
        {
            var sp = new StackPanel { Margin = new Thickness(0, 3, 0, 5) };
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var lbl = new TextBlock { Text = label, FontSize = 11, Foreground = new SolidColorBrush(Color.FromRgb(50, 50, 50)) };
            var valLbl = new TextBlock { FontSize = 11, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(41, 128, 185)) };
            valLbl.SetBinding(TextBlock.TextProperty, new Binding(propName) { StringFormat = format });

            Grid.SetColumn(lbl, 0);
            Grid.SetColumn(valLbl, 1);
            headerGrid.Children.Add(lbl);
            headerGrid.Children.Add(valLbl);
            sp.Children.Add(headerGrid);

            var slider = new Slider
            {
                Minimum = min,
                Maximum = max,
                SmallChange = tick,
                LargeChange = tick * 5,
                Margin = new Thickness(0, 2, 0, 0)
            };
            slider.SetBinding(Slider.ValueProperty, new Binding(propName) { Mode = BindingMode.TwoWay });
            sp.Children.Add(slider);

            return sp;
        }

        private UIElement CreateMetricRow(string label, string propName, string format)
        {
            var grid = new Grid { Margin = new Thickness(0, 2, 0, 2) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.3, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });

            var lbl = new TextBlock { Text = label, FontSize = 11, Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80)) };
            var val = new TextBlock { FontSize = 11, FontWeight = FontWeights.SemiBold, HorizontalAlignment = HorizontalAlignment.Right };
            val.SetBinding(TextBlock.TextProperty, new Binding(propName) { StringFormat = format });

            Grid.SetColumn(lbl, 0);
            Grid.SetColumn(val, 1);
            grid.Children.Add(lbl);
            grid.Children.Add(val);

            return grid;
        }
    }
}
