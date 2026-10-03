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
                Margin = new Thickness(0, 0, 0, 8)
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
                Text = "Offline Biomechanics, InteliPose\u2122 & EAWS Suite",
                Foreground = new SolidColorBrush(Color.FromRgb(189, 195, 199)),
                FontSize = 11,
                Margin = new Thickness(0, 2, 0, 0)
            };
            headerStack.Children.Add(title);
            headerStack.Children.Add(subtitle);
            headerBorder.Child = headerStack;
            root.Children.Add(headerBorder);

            // 1.1 1-Click CAD Object Snapping Button & Status Bar
            var cadBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(235, 245, 251)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(169, 204, 227)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 0, 0, 10)
            };
            var cadStack = new StackPanel();
            var snapBtn = new Button
            {
                Content = "\U0001F3AF Snap Coordinates From Selected 3D Object",
                Height = 30,
                FontWeight = FontWeights.Bold,
                FontSize = 11,
                Background = new SolidColorBrush(Color.FromRgb(41, 128, 185)),
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 4)
            };
            snapBtn.Click += (s, e) => (DataContext as WorksErgoPaneViewModel)?.SnapSelected3DObject();
            cadStack.Children.Add(snapBtn);

            var cadStatus = new TextBlock
            {
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                TextWrapping = TextWrapping.Wrap
            };
            cadStatus.SetBinding(TextBlock.TextProperty, new Binding("CadStatusMessage"));
            cadStack.Children.Add(cadStatus);
            cadBorder.Child = cadStack;
            root.Children.Add(cadBorder);

            // 2. High-Impact DCR Dashboard Card
            var dcrCard = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 224, 230)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 8)
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

            // 2.1 EAWS Automotive Standard Dashboard Card
            var eawsCard = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 224, 230)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 10)
            };
            var eawsStack = new StackPanel();
            var eawsTitleGrid = new Grid();
            eawsTitleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            eawsTitleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var eawsTitle = new TextBlock
            {
                Text = "EUROPEAN ASSESSMENT WORKSHEET (EAWS)",
                FontWeight = FontWeights.SemiBold,
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141))
            };
            var eawsScoreBadge = new TextBlock
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };
            eawsScoreBadge.SetBinding(TextBlock.TextProperty, new Binding("EawsScore") { StringFormat = "{0:F1} pts" });
            eawsScoreBadge.SetBinding(TextBlock.ForegroundProperty, new Binding("EawsBrush"));

            Grid.SetColumn(eawsTitle, 0);
            Grid.SetColumn(eawsScoreBadge, 1);
            eawsTitleGrid.Children.Add(eawsTitle);
            eawsTitleGrid.Children.Add(eawsScoreBadge);
            eawsStack.Children.Add(eawsTitleGrid);

            var eawsLight = new TextBlock
            {
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 2, 0, 4)
            };
            eawsLight.SetBinding(TextBlock.TextProperty, new Binding("EawsTrafficLight") { StringFormat = "Status: {0}" });
            eawsLight.SetBinding(TextBlock.ForegroundProperty, new Binding("EawsBrush"));
            eawsStack.Children.Add(eawsLight);

            eawsStack.Children.Add(CreateMetricRow("Sec 1: Postures & Movements:", "EawsSec1", "{0:F1} pts"));
            eawsStack.Children.Add(CreateMetricRow("Sec 2: Action Forces:", "EawsSec2", "{0:F1} pts"));
            eawsStack.Children.Add(CreateMetricRow("Sec 3: Manual Handling (MMH):", "EawsSec3", "{0:F1} pts"));
            eawsStack.Children.Add(CreateMetricRow("Sec 4: Repetitive Tasks:", "EawsSec4", "{0:F1} pts"));

            eawsCard.Child = eawsStack;
            root.Children.Add(eawsCard);

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

            var gripCombo = new ComboBox
            {
                Margin = new Thickness(0, 4, 0, 4),
                ItemsSource = Enum.GetValues(typeof(HandGripType))
            };
            gripCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedGrip") { Mode = BindingMode.TwoWay });
            dhmStack.Children.Add(new TextBlock { Text = "HandPak Interface (23 Anatomical Grips):", FontSize = 11 });
            dhmStack.Children.Add(gripCombo);

            dhmGroup.Content = dhmStack;
            root.Children.Add(dhmGroup);

            // 4. Geometry & Load Controls GroupBox
            var taskGroup = CreateGroupBox("Task Parameters");
            var taskStack = new StackPanel();

            taskStack.Children.Add(CreateSliderRow("Load Mass (kg):", "LoadWeightKg", 0.5, 50.0, 0.5, "{0:F1} kg"));
            taskStack.Children.Add(CreateSliderRow("Reach Distance H (mm):", "ReachMm", 200.0, 850.0, 10.0, "{0:F0} mm"));
            taskStack.Children.Add(CreateSliderRow("Vertical Height V (mm):", "VerticalMm", 100.0, 1600.0, 10.0, "{0:F0} mm"));
            taskStack.Children.Add(CreateSliderRow("Asymmetry / Twist (\u00b0):", "AsymmetryDeg", 0.0, 90.0, 5.0, "{0:F0}\u00b0"));
            taskStack.Children.Add(CreateSliderRow("Lateral Spine Tilt (\u00b0):", "LateralTiltDeg", 0.0, 45.0, 1.0, "{0:F0}\u00b0"));
            taskStack.Children.Add(CreateSliderRow("Dynamic Acceleration a (m/s\u00b2):", "DynamicAccelerationMs2", 0.0, 3.0, 0.1, "{0:F1} m/s\u00b2 (Kingma 1996)"));
            taskStack.Children.Add(CreateSliderRow("Lift Frequency (lifts/min):", "FrequencyLiftsPerMin", 0.1, 15.0, 0.5, "{0:F1}/min"));
            taskStack.Children.Add(CreateSliderRow("Daily Cycles (/day):", "FrequencyPerDay", 10.0, 2000.0, 50.0, "{0:F0} cycles/day"));

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
            bioStack.Children.Add(CreateMetricRow("HandPak Grip DCR:", "HandDCR", "{0:F2}"));
            bioStack.Children.Add(CreateMetricRow("RULA / REBA Posture Score:", "RulaScore", "{0} / REBA Score"));

            bioGroup.Content = bioStack;
            root.Children.Add(bioGroup);

            // 6. Multi-Subtask Job Manager (Shift Ergonomics)
            var jobGroup = CreateGroupBox("Shift Job Manager (Multi-Subtask Analysis)");
            var jobStack = new StackPanel();

            var jobBtnGrid = new Grid { Margin = new Thickness(0, 0, 0, 6) };
            jobBtnGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.4, GridUnitType.Star) });
            jobBtnGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.4, GridUnitType.Star) });
            jobBtnGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) });

            var addSubtaskBtn = new Button
            {
                Content = "\u2795 Add to Job",
                Height = 26,
                FontSize = 10,
                FontWeight = FontWeights.SemiBold,
                Background = new SolidColorBrush(Color.FromRgb(46, 204, 113)),
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 4, 0)
            };
            addSubtaskBtn.Click += (s, e) => (DataContext as WorksErgoPaneViewModel)?.AddCurrentAsSubtask();

            var evalJobBtn = new Button
            {
                Content = "\u26A1 Evaluate Shift",
                Height = 26,
                FontSize = 10,
                FontWeight = FontWeights.SemiBold,
                Background = new SolidColorBrush(Color.FromRgb(155, 89, 182)),
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 4, 0)
            };
            evalJobBtn.Click += (s, e) => (DataContext as WorksErgoPaneViewModel)?.EvaluateShiftJob();

            var clearJobBtn = new Button
            {
                Content = "\U0001F5D1 Clear",
                Height = 26,
                FontSize = 10,
                Background = new SolidColorBrush(Color.FromRgb(149, 165, 166)),
                Foreground = Brushes.White
            };
            clearJobBtn.Click += (s, e) => (DataContext as WorksErgoPaneViewModel)?.ClearSubtasks();

            Grid.SetColumn(addSubtaskBtn, 0);
            Grid.SetColumn(evalJobBtn, 1);
            Grid.SetColumn(clearJobBtn, 2);
            jobBtnGrid.Children.Add(addSubtaskBtn);
            jobBtnGrid.Children.Add(evalJobBtn);
            jobBtnGrid.Children.Add(clearJobBtn);
            jobStack.Children.Add(jobBtnGrid);

            // Subtasks ItemsControl List
            var subtasksList = new ItemsControl { Margin = new Thickness(0, 4, 0, 6) };
            subtasksList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("Subtasks"));

            var itemTemplate = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(Border));
            factory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            factory.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(220, 224, 230)));
            factory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            factory.SetValue(Border.PaddingProperty, new Thickness(6, 4, 6, 4));
            factory.SetValue(Border.MarginProperty, new Thickness(0, 0, 0, 4));
            factory.SetValue(Border.BackgroundProperty, Brushes.White);

            var rowGrid = new FrameworkElementFactory(typeof(Grid));
            var col1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col1.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.8, GridUnitType.Star));
            var col2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star));
            var col3 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col3.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star));
            rowGrid.AppendChild(col1);
            rowGrid.AppendChild(col2);
            rowGrid.AppendChild(col3);

            var nameText = new FrameworkElementFactory(typeof(TextBlock));
            nameText.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            nameText.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
            nameText.SetValue(TextBlock.FontSizeProperty, 10.5);
            nameText.SetValue(Grid.ColumnProperty, 0);

            var compText = new FrameworkElementFactory(typeof(TextBlock));
            compText.SetBinding(TextBlock.TextProperty, new Binding("PeakCompressionN") { StringFormat = "{0:F0} N" });
            compText.SetValue(TextBlock.FontSizeProperty, 10.0);
            compText.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            compText.SetValue(Grid.ColumnProperty, 1);

            var dcrBadge = new FrameworkElementFactory(typeof(TextBlock));
            dcrBadge.SetBinding(TextBlock.TextProperty, new Binding("TaskDCR") { StringFormat = "{0:P0}" });
            dcrBadge.SetBinding(TextBlock.ForegroundProperty, new Binding("RiskBrush"));
            dcrBadge.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            dcrBadge.SetValue(TextBlock.FontSizeProperty, 10.5);
            dcrBadge.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            dcrBadge.SetValue(Grid.ColumnProperty, 2);

            rowGrid.AppendChild(nameText);
            rowGrid.AppendChild(compText);
            rowGrid.AppendChild(dcrBadge);
            factory.AppendChild(rowGrid);
            itemTemplate.VisualTree = factory;
            subtasksList.ItemTemplate = itemTemplate;
            jobStack.Children.Add(subtasksList);

            // Composite Shift Summary
            var compSummaryBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(244, 246, 247)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(213, 216, 220)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 4, 0, 0)
            };
            var compStack = new StackPanel();
            compStack.Children.Add(CreateMetricRow("Composite Shift LCF\u1D04\u1D30 (Fatigue):", "CompositeLcfcd", "{0:F3}"));
            compStack.Children.Add(CreateMetricRow("Shift Composite Overall DCR:", "CompositeOverallDCR", "{0:P0}"));
            compStack.Children.Add(CreateMetricRow("Shift Peak Compression:", "CompositePeakCompN", "{0:F0} N"));
            compStack.Children.Add(CreateMetricRow("Shift EAWS Score:", "CompositeEawsScore", "{0:F1} pts"));
            compSummaryBorder.Child = compStack;
            jobStack.Children.Add(compSummaryBorder);

            jobGroup.Content = jobStack;
            root.Children.Add(jobGroup);

            // 7. Actionable Engineering Recommendation
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

            // 8. Action Buttons
            var btnStack = new StackPanel { Margin = new Thickness(0, 8, 0, 0) };

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
