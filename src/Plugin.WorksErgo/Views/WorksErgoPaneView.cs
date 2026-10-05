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
            Background = new SolidColorBrush(Color.FromRgb(244, 246, 249));

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
                Background = new SolidColorBrush(Color.FromRgb(41, 128, 185)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(0, 0, 0, 8)
            };
            var headerStack = new StackPanel();
            var title = new TextBlock
            {
                Text = "WORK(S) ERGO R-PRO",
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 14
            };
            var subtitle = new TextBlock
            {
                Text = "Task Analysis, DHM Biomechanics & InteliPose Suite",
                Foreground = new SolidColorBrush(Color.FromRgb(235, 245, 251)),
                FontSize = 10.5,
                Margin = new Thickness(0, 2, 0, 0)
            };
            headerStack.Children.Add(title);
            headerStack.Children.Add(subtitle);
            headerBorder.Child = headerStack;
            root.Children.Add(headerBorder);

            // 2. Active DHM / Operator Card
            var opCard = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 224, 230)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 8)
            };
            var opStack = new StackPanel();
            var opLabel = new TextBlock
            {
                Text = "ЦИФРОВОЙ МАНЕКЕН (DHM / ОПЕРАТОР):",
                FontWeight = FontWeights.Bold,
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                Margin = new Thickness(0, 0, 0, 4)
            };
            opStack.Children.Add(opLabel);

            var opGrid = new Grid();
            opGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            opGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            opGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var opCombo = new ComboBox
            {
                DisplayMemberPath = "Name",
                Height = 26,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0)
            };
            opCombo.SetBinding(ComboBox.ItemsSourceProperty, new Binding("Operators"));
            opCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedOperator") { Mode = BindingMode.TwoWay });
            Grid.SetColumn(opCombo, 0);
            opGrid.Children.Add(opCombo);

            var pickBtn = new Button
            {
                Content = "3D Pick",
                Height = 26,
                Padding = new Thickness(8, 2, 8, 2),
                FontWeight = FontWeights.SemiBold,
                FontSize = 10.5,
                Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 4, 0)
            };
            pickBtn.Click += (s, e) => (DataContext as WorksErgoPaneViewModel)?.PickFrom3DWorld_Click();
            Grid.SetColumn(pickBtn, 1);
            opGrid.Children.Add(pickBtn);

            var refBtn = new Button
            {
                Content = "Обновить",
                Height = 26,
                Padding = new Thickness(6, 2, 6, 2),
                FontSize = 10.5,
                Background = new SolidColorBrush(Color.FromRgb(149, 165, 166)),
                Foreground = Brushes.White
            };
            refBtn.Click += (s, e) => (DataContext as WorksErgoPaneViewModel)?.RefreshOperators();
            Grid.SetColumn(refBtn, 2);
            opGrid.Children.Add(refBtn);

            opStack.Children.Add(opGrid);
            opCard.Child = opStack;
            root.Children.Add(opCard);

            // 3. High-Impact DCR Dashboard Card
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
                Text = "ИНДЕКС НАГРУЗКИ / DCR (DEMAND-CAPACITY RATIO)",
                FontWeight = FontWeights.Bold,
                FontSize = 9.5,
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
                FontSize = 12.5,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 4)
            };
            riskBadge.SetBinding(TextBlock.TextProperty, new Binding("RiskCategory"));
            riskBadge.SetBinding(TextBlock.ForegroundProperty, new Binding("RiskBrush"));
            dcrStack.Children.Add(riskBadge);

            var limitText = new TextBlock
            {
                FontSize = 10.5,
                Foreground = new SolidColorBrush(Color.FromRgb(85, 85, 85))
            };
            var limitBind = new Binding("LimitingFactor") { StringFormat = "Ограничивающий фактор: {0}" };
            limitText.SetBinding(TextBlock.TextProperty, limitBind);
            dcrStack.Children.Add(limitText);

            dcrCard.Child = dcrStack;
            root.Children.Add(dcrCard);

            // 3.1 EAWS Dashboard Card
            var eawsCard = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 224, 230)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 8)
            };
            var eawsStack = new StackPanel();
            var eawsTitleGrid = new Grid();
            eawsTitleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            eawsTitleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var eawsTitle = new TextBlock
            {
                Text = "ЕВРОПЕЙСКИЙ СТАНДАРТ EAWS",
                FontWeight = FontWeights.Bold,
                FontSize = 9.5,
                Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141))
            };
            var eawsScoreBadge = new TextBlock
            {
                FontSize = 13,
                FontWeight = FontWeights.Bold
            };
            eawsScoreBadge.SetBinding(TextBlock.TextProperty, new Binding("EawsScore") { StringFormat = "{0:F1} баллов" });
            eawsScoreBadge.SetBinding(TextBlock.ForegroundProperty, new Binding("EawsBrush"));

            Grid.SetColumn(eawsTitle, 0);
            Grid.SetColumn(eawsScoreBadge, 1);
            eawsTitleGrid.Children.Add(eawsTitle);
            eawsTitleGrid.Children.Add(eawsScoreBadge);
            eawsStack.Children.Add(eawsTitleGrid);

            var eawsLight = new TextBlock
            {
                FontSize = 10.5,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 2, 0, 4)
            };
            eawsLight.SetBinding(TextBlock.TextProperty, new Binding("EawsTrafficLight") { StringFormat = "Статус: {0}" });
            eawsLight.SetBinding(TextBlock.ForegroundProperty, new Binding("EawsBrush"));
            eawsStack.Children.Add(eawsLight);

            eawsStack.Children.Add(CreateMetricRow("Раздел 1 (Рабочие позы):", "EawsSec1", "{0:F1}"));
            eawsStack.Children.Add(CreateMetricRow("Раздел 2 (Усилия действия):", "EawsSec2", "{0:F1}"));
            eawsStack.Children.Add(CreateMetricRow("Раздел 3 (Тяжесть грузов MMH):", "EawsSec3", "{0:F1}"));
            eawsStack.Children.Add(CreateMetricRow("Раздел 4 (Повторяемость):", "EawsSec4", "{0:F1}"));

            eawsCard.Child = eawsStack;
            root.Children.Add(eawsCard);

            // 4. Step 1: Target Population
            var step1Group = CreateGroupBox("Шаг 1: Целевая популяция (Target Population)");
            var step1Stack = new StackPanel();

            var dhmCombo = new ComboBox
            {
                Margin = new Thickness(0, 2, 0, 4),
                ItemsSource = Enum.GetValues(typeof(DHMPercentile))
            };
            dhmCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedPercentile") { Mode = BindingMode.TwoWay });
            step1Stack.Children.Add(new TextBlock { Text = "Перцентиль и пол оператора:", FontSize = 10.5 });
            step1Stack.Children.Add(dhmCombo);

            step1Group.Content = step1Stack;
            root.Children.Add(step1Group);

            // 5. Step 2: Task Characteristics
            var step2Group = CreateGroupBox("Шаг 2: Параметры операции (Task Characteristics)");
            var step2Stack = new StackPanel();

            var taskCombo = new ComboBox
            {
                Margin = new Thickness(0, 2, 0, 4),
                ItemsSource = Enum.GetValues(typeof(TaskType))
            };
            taskCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedTask") { Mode = BindingMode.TwoWay });
            step2Stack.Children.Add(new TextBlock { Text = "Тип операции:", FontSize = 10.5 });
            step2Stack.Children.Add(taskCombo);

            var techCombo = new ComboBox
            {
                Margin = new Thickness(0, 2, 0, 4),
                ItemsSource = Enum.GetValues(typeof(LiftingTechnique))
            };
            techCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedTechnique") { Mode = BindingMode.TwoWay });
            step2Stack.Children.Add(new TextBlock { Text = "Техника подъема (кинематика ног):", FontSize = 10.5 });
            step2Stack.Children.Add(techCombo);

            step2Stack.Children.Add(CreateSliderRow("Частота подъемов (в минуту):", "FrequencyLiftsPerMin", 0.1, 15.0, 0.5, "{0:F1}/мин"));
            step2Stack.Children.Add(CreateSliderRow("Циклов за смену (в день):", "FrequencyPerDay", 10.0, 2000.0, 50.0, "{0:F0} циклов"));

            step2Group.Content = step2Stack;
            root.Children.Add(step2Group);

            // 6. Step 3: Hand Contact Locations & 3D Snapping
            var step3Group = CreateGroupBox("Шаг 3: Позиционирование кистей (Floating Hands)");
            var step3Stack = new StackPanel();

            var attachBox = new CheckBox
            {
                Content = "Привязать кисти к телу (Attach Hands?)",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 2, 0, 6)
            };
            attachBox.SetBinding(CheckBox.IsCheckedProperty, new Binding("AttachHands") { Mode = BindingMode.TwoWay });
            step3Stack.Children.Add(attachBox);

            var snapBtn = new Button
            {
                Content = "🎯 1-Click CAD Snap (Нормаль ладони '-X')",
                Height = 28,
                FontWeight = FontWeights.Bold,
                FontSize = 11,
                Background = new SolidColorBrush(Color.FromRgb(41, 128, 185)),
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 4)
            };
            snapBtn.Click += (s, e) => (DataContext as WorksErgoPaneViewModel)?.SnapSelected3DObject();
            step3Stack.Children.Add(snapBtn);

            var cadStatus = new TextBlock
            {
                FontSize = 9.5,
                FontStyle = FontStyles.Italic,
                Foreground = new SolidColorBrush(Color.FromRgb(70, 80, 90)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 4)
            };
            cadStatus.SetBinding(TextBlock.TextProperty, new Binding("CadStatusMessage"));
            step3Stack.Children.Add(cadStatus);

            step3Stack.Children.Add(CreateSliderRow("Вылет по горизонтали H (мм):", "ReachMm", 200.0, 850.0, 10.0, "{0:F0} мм"));
            step3Stack.Children.Add(CreateSliderRow("Высота хвата V (мм):", "VerticalMm", 100.0, 1600.0, 10.0, "{0:F0} мм"));

            step3Group.Content = step3Stack;
            root.Children.Add(step3Group);

            // 7. Step 4: Force Parameters
            var step4Group = CreateGroupBox("Шаг 4: Силовые нагрузки (Force Parameters)");
            var step4Stack = new StackPanel();

            step4Stack.Children.Add(CreateSliderRow("Масса перемещаемого груза (кг):", "LoadWeightKg", 0.5, 50.0, 0.5, "{0:F1} кг"));
            step4Stack.Children.Add(CreateSliderRow("Асимметрия / Скручивание (°):", "AsymmetryDeg", 0.0, 90.0, 5.0, "{0:F0}°"));
            step4Stack.Children.Add(CreateSliderRow("Боковой наклон позвоночника (°):", "LateralTiltDeg", 0.0, 45.0, 1.0, "{0:F0}°"));
            step4Stack.Children.Add(CreateSliderRow("Динамическое ускорение (м/с²):", "DynamicAccelerationMs2", 0.0, 3.0, 0.1, "{0:F1} м/с²"));

            step4Group.Content = step4Stack;
            root.Children.Add(step4Group);

            // 8. Step 5: Hand Grip & Interface (HandPak)
            var step5Group = CreateGroupBox("Шаг 5: Захваты кисти (HandPak Grips)");
            var step5Stack = new StackPanel();

            var gripCombo = new ComboBox
            {
                Margin = new Thickness(0, 2, 0, 4),
                ItemsSource = Enum.GetValues(typeof(HandGripType))
            };
            gripCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedGrip") { Mode = BindingMode.TwoWay });
            step5Stack.Children.Add(new TextBlock { Text = "Анатомический тип захвата (23 HandPak):", FontSize = 10.5 });
            step5Stack.Children.Add(gripCombo);

            var coupCombo = new ComboBox
            {
                Margin = new Thickness(0, 2, 0, 4),
                ItemsSource = Enum.GetValues(typeof(CouplingQuality))
            };
            coupCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedCoupling") { Mode = BindingMode.TwoWay });
            step5Stack.Children.Add(new TextBlock { Text = "Качество сопряжения (рукоятка / тара):", FontSize = 10.5 });
            step5Stack.Children.Add(coupCombo);

            step5Group.Content = step5Stack;
            root.Children.Add(step5Group);

            // 9. Step 6: Barriers & Bracing
            var step6Group = CreateGroupBox("Шаг 6: Упоры телом и барьеры (Body Bracing)");
            var step6Stack = new StackPanel();

            var braceBox = new CheckBox
            {
                Content = "Упор телом / поддержка туловища (Body Bracing)",
                FontSize = 10.5,
                Margin = new Thickness(0, 2, 0, 4)
            };
            braceBox.SetBinding(CheckBox.IsCheckedProperty, new Binding("BodyBracing") { Mode = BindingMode.TwoWay });
            step6Stack.Children.Add(braceBox);

            var straightBox = new CheckBox
            {
                Content = "Прямые ноги / подъем наклоном (Stoop Lift)",
                FontSize = 10.5,
                Margin = new Thickness(0, 2, 0, 4)
            };
            straightBox.SetBinding(CheckBox.IsCheckedProperty, new Binding("StraightLegs") { Mode = BindingMode.TwoWay });
            step6Stack.Children.Add(straightBox);

            step6Group.Content = step6Stack;
            root.Children.Add(step6Group);

            // 10. 7-Axis Biomechanical Breakdown
            var bioGroup = CreateGroupBox("7-Осевой Биомеханический Анализ");
            var bioStack = new StackPanel();

            bioStack.Children.Add(CreateMetricRow("Компрессия L5/S1 (позвоночник):", "LumbarCompN", "{0:F0} Н (Предел 3400 Н)"));
            bioStack.Children.Add(CreateMetricRow("DCR L5/S1:", "LumbarDCR", "{0:F2}"));
            bioStack.Children.Add(CreateMetricRow("Угол сгибания коленей:", "KneeFlexionDeg", "{0:F1}°"));
            bioStack.Children.Add(CreateMetricRow("Момент коленного сустава:", "KneeMomentNm", "{0:F1} Нм"));
            bioStack.Children.Add(CreateMetricRow("Центр давления (CofP от лодыжки):", "CenterOfPressureMm", "{0:F1} мм"));
            bioStack.Children.Add(CreateMetricRow("Предел по NIOSH (RWL):", "NioshRWLKg", "{0:F1} кг"));
            bioStack.Children.Add(CreateMetricRow("Индекс подъема NIOSH (LI):", "NioshLI", "{0:F2}"));
            bioStack.Children.Add(CreateMetricRow("Предел по Snook MAWL (Liberty Mutual):", "SnookMAWLKg", "{0:F1} кг"));
            bioStack.Children.Add(CreateMetricRow("DCR руки (AFF нейросеть):", "ArmDCR", "{0:F2}"));
            bioStack.Children.Add(CreateMetricRow("DCR пальцев и кисти (HandPak):", "HandDCR", "{0:F2}"));
            bioStack.Children.Add(CreateMetricRow("Балл RULA / REBA:", "RulaScore", "{0} баллов"));

            bioGroup.Content = bioStack;
            root.Children.Add(bioGroup);

            // 11. Multi-Subtask Job Manager (Shift Ergonomics)
            var jobGroup = CreateGroupBox("Менеджер смены (Многоэтапный расчет)");
            var jobStack = new StackPanel();

            var jobBtnGrid = new Grid { Margin = new Thickness(0, 0, 0, 6) };
            jobBtnGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.4, GridUnitType.Star) });
            jobBtnGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.4, GridUnitType.Star) });
            jobBtnGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) });

            var addSubtaskBtn = new Button
            {
                Content = "➕ В смену",
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
                Content = "⚡ Итог смены",
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
                Content = "🗑 Сброс",
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
            var subtasksList = new ItemsControl { Margin = new Thickness(0, 2, 0, 4) };
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
            nameText.SetValue(TextBlock.FontSizeProperty, 10.0);
            nameText.SetValue(Grid.ColumnProperty, 0);

            var compText = new FrameworkElementFactory(typeof(TextBlock));
            compText.SetBinding(TextBlock.TextProperty, new Binding("PeakCompressionN") { StringFormat = "{0:F0} Н" });
            compText.SetValue(TextBlock.FontSizeProperty, 9.5);
            compText.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            compText.SetValue(Grid.ColumnProperty, 1);

            var dcrBadge = new FrameworkElementFactory(typeof(TextBlock));
            dcrBadge.SetBinding(TextBlock.TextProperty, new Binding("TaskDCR") { StringFormat = "{0:P0}" });
            dcrBadge.SetBinding(TextBlock.ForegroundProperty, new Binding("RiskBrush"));
            dcrBadge.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            dcrBadge.SetValue(TextBlock.FontSizeProperty, 10.0);
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
                Padding = new Thickness(6),
                Margin = new Thickness(0, 2, 0, 0)
            };
            var compStack = new StackPanel();
            compStack.Children.Add(CreateMetricRow("Усталость LCFCD:", "CompositeLcfcd", "{0:F3}"));
            compStack.Children.Add(CreateMetricRow("Итоговый DCR смены:", "CompositeOverallDCR", "{0:P0}"));
            compStack.Children.Add(CreateMetricRow("Пиковая нагрузка L5/S1:", "CompositePeakCompN", "{0:F0} Н"));
            compStack.Children.Add(CreateMetricRow("Итоговый EAWS смены:", "CompositeEawsScore", "{0:F1} баллов"));
            compSummaryBorder.Child = compStack;
            jobStack.Children.Add(compSummaryBorder);

            jobGroup.Content = jobStack;
            root.Children.Add(jobGroup);

            // 12. Engineering Guidance
            var recGroup = CreateGroupBox("Инженерные Рекомендации");
            var recBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(254, 249, 231)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(249, 231, 159)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 2, 0, 4)
            };
            var recText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                FontSize = 10.5,
                Foreground = new SolidColorBrush(Color.FromRgb(125, 102, 8)),
                FontWeight = FontWeights.Medium
            };
            recText.SetBinding(TextBlock.TextProperty, new Binding("Recommendation"));
            recBorder.Child = recText;
            recGroup.Content = recBorder;
            root.Children.Add(recGroup);

            // 13. Action Buttons
            var btnStack = new StackPanel { Margin = new Thickness(0, 8, 0, 10) };

            var recalcBtn = new Button
            {
                Content = "⚡ РАССЧИТАТЬ НАГРУЗКУ (ANALYSE)",
                Height = 34,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Background = new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 6)
            };
            recalcBtn.Click += (s, e) => (DataContext as WorksErgoPaneViewModel)?.Recalculate();
            btnStack.Children.Add(recalcBtn);

            var reportBtn = new Button
            {
                Content = "📁 Открыть папку с отчетами Excel",
                Height = 28,
                FontWeight = FontWeights.SemiBold,
                FontSize = 11,
                Background = new SolidColorBrush(Color.FromRgb(52, 73, 94)),
                Foreground = Brushes.White
            };
            reportBtn.Click += (s, e) => (DataContext as WorksErgoPaneViewModel)?.OpenReportFolder();
            btnStack.Children.Add(reportBtn);

            root.Children.Add(btnStack);
        }

        private GroupBox CreateGroupBox(string header)
        {
            return new GroupBox
            {
                Header = header,
                FontWeight = FontWeights.SemiBold,
                FontSize = 10.5,
                Margin = new Thickness(0, 0, 0, 6),
                Padding = new Thickness(6)
            };
        }

        private UIElement CreateSliderRow(string label, string propName, double min, double max, double tick, string format)
        {
            var sp = new StackPanel { Margin = new Thickness(0, 2, 0, 4) };
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var lbl = new TextBlock { Text = label, FontSize = 10.5, Foreground = new SolidColorBrush(Color.FromRgb(50, 50, 50)) };
            var valLbl = new TextBlock { FontSize = 10.5, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(41, 128, 185)) };
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
            var grid = new Grid { Margin = new Thickness(0, 1.5, 0, 1.5) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.3, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });

            var lbl = new TextBlock { Text = label, FontSize = 10.5, Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80)) };
            var val = new TextBlock { FontSize = 10.5, FontWeight = FontWeights.SemiBold, HorizontalAlignment = HorizontalAlignment.Right };
            val.SetBinding(TextBlock.TextProperty, new Binding(propName) { StringFormat = format });

            Grid.SetColumn(lbl, 0);
            Grid.SetColumn(val, 1);
            grid.Children.Add(lbl);
            grid.Children.Add(val);

            return grid;
        }
    }
}
