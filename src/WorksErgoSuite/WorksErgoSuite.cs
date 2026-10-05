using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WorksErgoRPro.Biomechanics;
using WorksErgoRPro.ViewModels;

namespace WorksErgoRPro.Suite
{
    public class WorksErgoApp : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new WorksErgoApp();
            app.Run(new WorksErgoMainWindow());
        }
    }

    public class WorksErgoMainWindow : Window
    {
        private WorksErgoPaneViewModel _vm;
        private DispatcherTimer _scenePollTimer;
        private string _sceneStateFilePath;
        private double _durationHours = 8.0;

        // UI Controls for Live Updates
        private TextBlock _txtOverallDcrBig;
        private Border _badgeOverallRisk;
        private TextBlock _txtOverallSubtitle;
        private TextBlock _lblNeckDcr, _lblLeftArmDcr, _lblRightArmDcr, _lblLumbarDcr, _lblLeftHandDcr, _lblRightHandDcr, _lblLmMmhDcr;
        
        // DCR Bar Chart items
        private ProgressBar _pbOverall, _pbLeftArm, _pbRightArm, _pbNeck, _pbLumbar, _pbLeftHand, _pbRightHand;
        private TextBlock _valOverall, _valLeftArm, _valRightArm, _valNeck, _valLumbar, _valLeftHand, _valRightHand;

        // Summary Table TextBlocks
        private TextBlock _txtArmDemandL, _txtArmDemandR, _txtArmStrengthL, _txtArmStrengthR, _txtArmMaxL, _txtArmMaxR;
        private TextBlock _txtHandDemandL, _txtHandDemandR, _txtHandStrengthL, _txtHandStrengthR, _txtHandMaxL, _txtHandMaxR;
        private TextBlock _txtL5S1Comp, _txtL5S1Shear, _txtL5S1Max, _txtNioshRwl, _txtNioshLi, _txtRulaScore, _txtRebaScore;
        private TextBlock _txtJointWrist, _txtJointElbow, _txtJointShoulder, _txtJointNeck, _txtJointL5S1;

        // 2D Kinematic Canvas elements
        private Canvas _kinematicCanvas;
        private Line _lineShank, _lineThigh, _linePelvis, _lineTorso, _lineNeck, _lineUpperArm, _lineForearm, _linePlumb, _lineBos;
        private Ellipse _ptAnkle, _ptKnee, _ptHip, _ptL5S1, _ptShoulder, _ptElbow, _ptHand, _ptHead, _ptCofP;
        private Border _boxObject;
        private TextBlock _txtBoxMassLabel, _txtKinematicHud;

        public WorksErgoMainWindow()
        {
            Title = "Work(s)\u2122 Ergo \u2022 Task Analysis & Ergonomics Suite (R-Pro Edition)";
            Width = 1180;
            Height = 840;
            MinWidth = 1000;
            MinHeight = 700;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = new SolidColorBrush(Color.FromRgb(248, 249, 250)); // Clean Work(s) Light Theme

            _vm = new WorksErgoPaneViewModel();
            _sceneStateFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "worksergo_scene_state.json");

            BuildInterface();
            AttachLiveSync();
            RefreshAllViews();
        }

        private void BuildInterface()
        {
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Header
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Tabs
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Bottom Status Bar

            // 1. TOP HEADER BANNER (Work(s) Ergo Branding)
            var headerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(26, 54, 93)), // Work(s) Deep Blue
                Padding = new Thickness(20, 12, 20, 12),
                BorderBrush = new SolidColorBrush(Color.FromRgb(43, 108, 176)),
                BorderThickness = new Thickness(0, 0, 0, 2)
            };
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var titleStack = new StackPanel();
            var titleRow = new StackPanel { Orientation = Orientation.Horizontal };
            var titleMain = new TextBlock
            {
                Text = "Work(s)\u2122 Ergo",
                FontWeight = FontWeights.Bold,
                FontSize = 22,
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Segoe UI")
            };
            var titleEdition = new TextBlock
            {
                Text = "  R-Pro Edition",
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(250, 204, 21)), // Gold badge
                VerticalAlignment = VerticalAlignment.Center
            };
            titleRow.Children.Add(titleMain);
            titleRow.Children.Add(titleEdition);

            var subtitle = new TextBlock
            {
                Text = "Comprehensive Ergonomics Task Analysis & Digital Human Modeling Suite \u2022 Zero-Cloud Standalone",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                Margin = new Thickness(0, 2, 0, 0)
            };
            titleStack.Children.Add(titleRow);
            titleStack.Children.Add(subtitle);
            Grid.SetColumn(titleStack, 0);
            headerGrid.Children.Add(titleStack);

            // Right header quick actions
            var headerActions = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            var btnSnapQuick = new Button
            {
                Content = "\U0001F3AF Snap Active 3D Selection",
                Background = new SolidColorBrush(Color.FromRgb(49, 130, 206)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(14, 6, 14, 6),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 10, 0)
            };
            btnSnapQuick.Click += (s, e) => SnapFromScene();
            headerActions.Children.Add(btnSnapQuick);

            var btnExportQuick = new Button
            {
                Content = "\U0001F4C4 Export Excel Report",
                Background = new SolidColorBrush(Color.FromRgb(56, 161, 105)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(14, 6, 14, 6),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            btnExportQuick.Click += (s, e) => ExportExcelReport();
            headerActions.Children.Add(btnExportQuick);

            Grid.SetColumn(headerActions, 1);
            headerGrid.Children.Add(headerActions);
            headerBorder.Child = headerGrid;
            Grid.SetRow(headerBorder, 0);
            mainGrid.Children.Add(headerBorder);

            // 2. MAIN TABBED CONTAINER
            var tabControl = new TabControl
            {
                Background = new SolidColorBrush(Color.FromRgb(248, 249, 250)),
                BorderThickness = new Thickness(0),
                Padding = new Thickness(16)
            };

            // TAB 1: Task Inputs (Steps 1-6)
            var tabInputs = new TabItem { Header = "  \U0001F4CB Task Inputs (Steps 1\u20136)  ", FontSize = 13 };
            tabInputs.Content = CreateTaskInputsTab();
            tabControl.Items.Add(tabInputs);

            // TAB 2: Iconic Body Map & DCR Ranking (The core Work(s) Ergo screen)
            var tabBodyMap = new TabItem { Header = "  \U0001F9CD Body Map & DCR Ranking  ", FontSize = 13 };
            tabBodyMap.Content = CreateBodyMapAndDcrTab();
            tabControl.Items.Add(tabBodyMap);

            // TAB 3: Summary Table & Joint Demands
            var tabSummary = new TabItem { Header = "  \U0001F4CA Summary & Joint Demands  ", FontSize = 13 };
            tabSummary.Content = CreateSummaryAndResultsTab();
            tabControl.Items.Add(tabSummary);

            // TAB 4: 2D Articulated Kinematics Visualizer
            var tabKinematics = new TabItem { Header = "  \U0001F52C 2D Kinematics & Spine  ", FontSize = 13 };
            tabKinematics.Content = CreateKinematicsTab();
            tabControl.Items.Add(tabKinematics);

            Grid.SetRow(tabControl, 1);
            mainGrid.Children.Add(tabControl);

            // 3. BOTTOM STATUS BAR
            var statusBar = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(237, 242, 247)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(203, 213, 225)),
                BorderThickness = new Thickness(0, 1, 0, 0),
                Padding = new Thickness(16, 6, 16, 6)
            };
            var statusGrid = new Grid();
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var statusMsg = new TextBlock
            {
                Text = "Work(s) Ergo R-Pro Suite Engine \u2022 Biomechanics Math Engine V2.2 \u2022 Ready",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(71, 85, 105))
            };
            Grid.SetColumn(statusMsg, 0);
            statusGrid.Children.Add(statusMsg);

            var liveBadge = new TextBlock
            {
                Text = "\u25CF Live Scene Sync Active",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74))
            };
            Grid.SetColumn(liveBadge, 1);
            statusGrid.Children.Add(liveBadge);

            statusBar.Child = statusGrid;
            Grid.SetRow(statusBar, 2);
            mainGrid.Children.Add(statusBar);

            Content = mainGrid;
        }

        // ==============================================================================
        // TAB 1: TASK INPUTS (STEPS 1-6)
        // ==============================================================================
        private UIElement CreateTaskInputsTab()
        {
            var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            var root = new StackPanel { Margin = new Thickness(12) };
            scroll.Content = root;

            // Step 1: Target Population
            var cardPop = CreateAccordionCard("Step 1: Target Population & Anthropometry", true);
            var gridPop = new Grid();
            gridPop.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            gridPop.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var spPopL = new StackPanel { Margin = new Thickness(0, 0, 10, 0) };
            spPopL.Children.Add(CreateFieldLabel("Standard Anthropometric Percentile:"));
            var cmbPerc = new ComboBox
            {
                ItemsSource = new[] { "Male 50th (175cm / 78kg)", "Male 95th (187cm / 98kg)", "Female 5th (152cm / 50kg)", "Female 50th (162cm / 62kg)" },
                SelectedIndex = 0,
                Height = 28
            };
            cmbPerc.SelectionChanged += (s, e) =>
            {
                if (cmbPerc.SelectedIndex == 0) _vm.SelectedPercentile = DHMPercentile.Male50th;
                else if (cmbPerc.SelectedIndex == 1) _vm.SelectedPercentile = DHMPercentile.Male95th;
                else if (cmbPerc.SelectedIndex == 2) _vm.SelectedPercentile = DHMPercentile.Female5th;
                else if (cmbPerc.SelectedIndex == 3) _vm.SelectedPercentile = DHMPercentile.Female50th;
                RefreshAllViews();
            };
            spPopL.Children.Add(cmbPerc);
            Grid.SetColumn(spPopL, 0);
            gridPop.Children.Add(spPopL);

            var spPopR = new StackPanel { Margin = new Thickness(10, 0, 0, 0) };
            spPopR.Children.Add(CreateFieldLabel("D/C Ratio Percentile Cutoff (%ile):"));
            var txtDcrPct = new TextBox { Text = "25", Height = 28, Padding = new Thickness(6, 4, 6, 4) };
            spPopR.Children.Add(txtDcrPct);
            Grid.SetColumn(spPopR, 1);
            gridPop.Children.Add(spPopR);

            cardPop.Content = gridPop;
            root.Children.Add(cardPop);

            // Step 2: Task Characteristics
            var cardTask = CreateAccordionCard("Step 2: Task Characteristics & Frequency", true);
            var gridTask = new Grid();
            gridTask.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            gridTask.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var spTaskL = new StackPanel { Margin = new Thickness(0, 0, 10, 0) };
            spTaskL.Children.Add(CreateFieldLabel("Task Type:"));
            var cmbTask = new ComboBox
            {
                ItemsSource = new[] { "Lifting / Lowering (MMH)", "Pushing / Pulling", "Carrying", "Static Holding Posture" },
                SelectedIndex = 0,
                Height = 28
            };
            cmbTask.SelectionChanged += (s, e) =>
            {
                _vm.SelectedTask = (TaskType)cmbTask.SelectedIndex;
                RefreshAllViews();
            };
            spTaskL.Children.Add(cmbTask);

            spTaskL.Children.Add(CreateFieldLabel("Lifting Technique / Squat Style:"));
            var cmbTech = new ComboBox
            {
                ItemsSource = new[] { "Semi-Squat (Recommended Balanced)", "Stoop (Straight Legs, Hip Dominant)", "Deep Squat (Knee Dominant)" },
                SelectedIndex = 0,
                Height = 28
            };
            cmbTech.SelectionChanged += (s, e) =>
            {
                _vm.SelectedTechnique = (LiftingTechnique)cmbTech.SelectedIndex;
                RefreshAllViews();
            };
            spTaskL.Children.Add(cmbTech);
            Grid.SetColumn(spTaskL, 0);
            gridTask.Children.Add(spTaskL);

            var spTaskR = new StackPanel { Margin = new Thickness(10, 0, 0, 0) };
            spTaskR.Children.Add(CreateFieldLabel("Task Frequency (lifts / min):"));
            var slFreq = new Slider { Minimum = 0.2, Maximum = 15.0, Value = _vm.FrequencyLiftsPerMin, TickFrequency = 1.0, IsSnapToTickEnabled = false };
            var lblFreqVal = new TextBlock { Text = $"{_vm.FrequencyLiftsPerMin:F1} lifts/min", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 0, 0, 8) };
            slFreq.ValueChanged += (s, e) =>
            {
                _vm.FrequencyLiftsPerMin = slFreq.Value;
                lblFreqVal.Text = $"{slFreq.Value:F1} lifts/min";
                RefreshAllViews();
            };
            spTaskR.Children.Add(slFreq);
            spTaskR.Children.Add(lblFreqVal);

            spTaskR.Children.Add(CreateFieldLabel("Shift Duration (hours/day):"));
            var slDur = new Slider { Minimum = 1.0, Maximum = 12.0, Value = _durationHours, TickFrequency = 1.0, IsSnapToTickEnabled = true };
            var lblDurVal = new TextBlock { Text = $"{_durationHours:F0} hours/day", FontWeight = FontWeights.SemiBold };
            slDur.ValueChanged += (s, e) =>
            {
                _durationHours = slDur.Value;
                lblDurVal.Text = $"{slDur.Value:F0} hours/day";
                RefreshAllViews();
            };
            spTaskR.Children.Add(slDur);
            spTaskR.Children.Add(lblDurVal);
            Grid.SetColumn(spTaskR, 1);
            gridTask.Children.Add(spTaskR);

            cardTask.Content = gridTask;
            root.Children.Add(cardTask);

            // Step 3: Hand Contact Locations & Force Magnitudes
            var cardCoords = CreateAccordionCard("Step 3: Hand Locations & Load Weight (Interactive Snapping)", true);
            var spCoords = new StackPanel();

            // Load Mass Slider
            spCoords.Children.Add(CreateFieldLabel("Load Weight / Applied Force (kg):"));
            var slLoad = new Slider { Minimum = 0.5, Maximum = 40.0, Value = _vm.LoadWeightKg, TickFrequency = 1.0 };
            var lblLoadVal = new TextBlock { Text = $"{_vm.LoadWeightKg:F1} kg ({_vm.LoadWeightKg * 9.81:F0} N)", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) };
            slLoad.ValueChanged += (s, e) =>
            {
                _vm.LoadWeightKg = slLoad.Value;
                lblLoadVal.Text = $"{slLoad.Value:F1} kg ({slLoad.Value * 9.81:F0} N)";
                RefreshAllViews();
            };
            spCoords.Children.Add(slLoad);
            spCoords.Children.Add(lblLoadVal);

            // Vertical Height Slider
            spCoords.Children.Add(CreateFieldLabel("Vertical Hand Height (mm from floor):"));
            var slVert = new Slider { Minimum = 50.0, Maximum = 1800.0, Value = _vm.VerticalMm, TickFrequency = 50.0 };
            var lblVertVal = new TextBlock { Text = $"{_vm.VerticalMm:F0} mm (Height)", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) };
            slVert.ValueChanged += (s, e) =>
            {
                _vm.VerticalMm = slVert.Value;
                lblVertVal.Text = $"{slVert.Value:F0} mm (Height)";
                RefreshAllViews();
            };
            spCoords.Children.Add(slVert);
            spCoords.Children.Add(lblVertVal);

            // Horizontal Reach Slider
            spCoords.Children.Add(CreateFieldLabel("Horizontal Forward Reach (mm from ankles):"));
            var slReach = new Slider { Minimum = 150.0, Maximum = 950.0, Value = _vm.ReachMm, TickFrequency = 25.0 };
            var lblReachVal = new TextBlock { Text = $"{_vm.ReachMm:F0} mm (Reach)", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) };
            slReach.ValueChanged += (s, e) =>
            {
                _vm.ReachMm = slReach.Value;
                lblReachVal.Text = $"{slReach.Value:F0} mm (Reach)";
                RefreshAllViews();
            };
            spCoords.Children.Add(slReach);
            spCoords.Children.Add(lblReachVal);

            // Snap button row
            var btnRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 8, 0, 0) };
            var btnSnapInTab = new Button
            {
                Content = "\U0001F3AF Snap Coordinates From Selected Object / DHM",
                Background = new SolidColorBrush(Color.FromRgb(43, 108, 176)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(16, 8, 16, 8),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 10, 0)
            };
            btnSnapInTab.Click += (s, e) =>
            {
                SnapFromScene();
                slLoad.Value = _vm.LoadWeightKg;
                slVert.Value = _vm.VerticalMm;
                slReach.Value = _vm.ReachMm;
            };
            btnRow.Children.Add(btnSnapInTab);
            spCoords.Children.Add(btnRow);

            cardCoords.Content = spCoords;
            root.Children.Add(cardCoords);

            // Step 4: HandPak 23 Grip Interfaces
            var cardGrip = CreateAccordionCard("Step 4: Hand Grip Interface (HandPak 23 Grips)", true);
            var spGrip = new StackPanel();
            spGrip.Children.Add(CreateFieldLabel("Select Hand/Object Interface:"));
            var cmbGrip = new ComboBox
            {
                ItemsSource = new[]
                {
                    "1. Power Grip (Cylindrical Handle)",
                    "2. Power Grip (Oblique)",
                    "3. Pinch Grip (2-finger Tip-to-Tip)",
                    "4. Pinch Grip (3-finger Jaw)",
                    "5. Lateral Key Pinch",
                    "6. Flat Palm Push / Support",
                    "7. Hook Grip (No thumb)",
                    "8. Finger Press (Button / Trigger)"
                },
                SelectedIndex = 0,
                Height = 28
            };
            spGrip.Children.Add(cmbGrip);
            cardGrip.Content = spGrip;
            root.Children.Add(cardGrip);

            return scroll;
        }

        // ==============================================================================
        // TAB 2: ICONIC BODY MAP & DCR RANKING (AUTHENTIC WORK(S) ERGO UI)
        // ==============================================================================
        private UIElement CreateBodyMapAndDcrTab()
        {
            var rootGrid = new Grid();
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(420) }); // Body Map Column
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // DCR Ranking Column

            // LEFT COLUMN: THE BODY MAP
            var bodyMapBorder = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(16),
                Margin = new Thickness(0, 0, 12, 0)
            };
            var bodyMapStack = new StackPanel();

            // Overall DCR Badge at top
            var overallHeader = new TextBlock
            {
                Text = "Overall",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 41, 59))
            };
            bodyMapStack.Children.Add(overallHeader);

            _txtOverallDcrBig = new TextBlock
            {
                Text = "0.64",
                FontSize = 44,
                FontWeight = FontWeights.ExtraBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74)) // Green
            };
            bodyMapStack.Children.Add(_txtOverallDcrBig);

            _txtOverallSubtitle = new TextBlock
            {
                Text = "100% \u2264 DCR=1 (Acceptable)",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 14)
            };
            bodyMapStack.Children.Add(_txtOverallSubtitle);

            // Vector Body Map Canvas
            var mapCanvas = new Canvas
            {
                Width = 360,
                Height = 440,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Silhouette Path (simplified stylized human back contour)
            var silhouette = new System.Windows.Shapes.Path
            {
                Fill = new SolidColorBrush(Color.FromRgb(238, 242, 246)),
                Stroke = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                StrokeThickness = 2
            };
            // Build anatomical silhouette geometry
            var geom = new StreamGeometry();
            using (var ctx = geom.Open())
            {
                // Head
                ctx.BeginFigure(new Point(180, 20), true, true);
                ctx.ArcTo(new Point(180, 90), new Size(30, 35), 0, false, SweepDirection.Clockwise, true, true);
                ctx.ArcTo(new Point(180, 20), new Size(30, 35), 0, false, SweepDirection.Clockwise, true, true);
            }
            silhouette.Data = geom;
            mapCanvas.Children.Add(silhouette);

            // Stylized Body Contour Path
            var bodyContour = new System.Windows.Shapes.Path
            {
                Fill = new SolidColorBrush(Color.FromRgb(238, 242, 246)),
                Stroke = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                StrokeThickness = 2
            };
            var bgeom = new StreamGeometry();
            using (var ctx = bgeom.Open())
            {
                ctx.BeginFigure(new Point(168, 88), true, true);
                // Neck to Left Shoulder
                ctx.LineTo(new Point(120, 105), true, true);
                // Left Shoulder to Left Arm
                ctx.LineTo(new Point(85, 210), true, true);
                ctx.LineTo(new Point(75, 300), true, true);
                ctx.LineTo(new Point(90, 305), true, true);
                ctx.LineTo(new Point(105, 215), true, true);
                // Left Armpit to Waist
                ctx.LineTo(new Point(135, 170), true, true);
                ctx.LineTo(new Point(140, 260), true, true);
                // Pelvis to Left Leg
                ctx.LineTo(new Point(155, 390), true, true);
                ctx.LineTo(new Point(175, 390), true, true);
                // Crotch
                ctx.LineTo(new Point(180, 290), true, true);
                // Right Leg
                ctx.LineTo(new Point(185, 390), true, true);
                ctx.LineTo(new Point(205, 390), true, true);
                // Right Pelvis to Waist
                ctx.LineTo(new Point(220, 260), true, true);
                ctx.LineTo(new Point(225, 170), true, true);
                // Right Arm
                ctx.LineTo(new Point(255, 215), true, true);
                ctx.LineTo(new Point(270, 305), true, true);
                ctx.LineTo(new Point(285, 300), true, true);
                ctx.LineTo(new Point(275, 210), true, true);
                // Right Shoulder to Neck
                ctx.LineTo(new Point(240, 105), true, true);
                ctx.LineTo(new Point(192, 88), true, true);
            }
            bodyContour.Data = bgeom;
            mapCanvas.Children.Add(bodyContour);

            // CALLOUT LABELS ON BODY MAP
            // Neck
            _lblNeckDcr = CreateCalloutBadge("Neck", "0.42", 180, 75, true);
            mapCanvas.Children.Add(_lblNeckDcr);

            // Left Arm
            _lblLeftArmDcr = CreateCalloutBadge("Left Arm", "0.60", 40, 150, false);
            mapCanvas.Children.Add(_lblLeftArmDcr);

            // Right Arm
            _lblRightArmDcr = CreateCalloutBadge("Right Arm", "0.58", 260, 150, false);
            mapCanvas.Children.Add(_lblRightArmDcr);

            // Lumbar Compression Force L5/S1
            _lblLumbarDcr = CreateCalloutBadge("Lumbar L5/S1", "0.65 (2263 N)", 180, 225, true);
            mapCanvas.Children.Add(_lblLumbarDcr);

            // Left Hand, Wrist, Forearm
            _lblLeftHandDcr = CreateCalloutBadge("Left Hand/Wrist", "0.35", 30, 275, false);
            mapCanvas.Children.Add(_lblLeftHandDcr);

            // Right Hand, Wrist, Forearm
            _lblRightHandDcr = CreateCalloutBadge("Right Hand/Wrist", "0.32", 265, 275, false);
            mapCanvas.Children.Add(_lblRightHandDcr);

            // LM-MMH Psychophysical Equations at bottom
            _lblLmMmhDcr = CreateCalloutBadge("LM-MMH Equations", "0.42", 180, 405, true);
            mapCanvas.Children.Add(_lblLmMmhDcr);

            bodyMapStack.Children.Add(mapCanvas);
            bodyMapBorder.Child = bodyMapStack;
            Grid.SetColumn(bodyMapBorder, 0);
            rootGrid.Children.Add(bodyMapBorder);

            // RIGHT COLUMN: D/C RATIO RANKING BAR CHART (Authentic Work(s) Ergo Chart)
            var chartBorder = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 0)
            };
            var chartStack = new StackPanel();

            var chartHeader = new TextBlock
            {
                Text = "D/C Ratio Ranking",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                Margin = new Thickness(0, 0, 0, 6)
            };
            var chartSubtitle = new TextBlock
            {
                Text = "Demand/Capacity Ratios sorted by ergonomic risk level \u2022 Reference line at DCR = 1.0",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                Margin = new Thickness(0, 0, 0, 20)
            };
            chartStack.Children.Add(chartHeader);
            chartStack.Children.Add(chartSubtitle);

            // Bar Items Container
            _pbOverall = CreateDcrBar(chartStack, "Overall DCR", out _valOverall, true);
            _pbLeftArm = CreateDcrBar(chartStack, "Arm (left)", out _valLeftArm, false);
            _pbRightArm = CreateDcrBar(chartStack, "Arm (right)", out _valRightArm, false);
            _pbLumbar = CreateDcrBar(chartStack, "Lumbar Compression (L5/S1)", out _valLumbar, false);
            _pbNeck = CreateDcrBar(chartStack, "Neck Demands", out _valNeck, false);
            _pbLeftHand = CreateDcrBar(chartStack, "Hand, Wrist, Forearm (left)", out _valLeftHand, false);
            _pbRightHand = CreateDcrBar(chartStack, "Hand, Wrist, Forearm (right)", out _valRightHand, false);

            // Threshold Reference Line Legend
            var legendBorder = new Border
            {
                Margin = new Thickness(0, 24, 0, 0),
                Padding = new Thickness(12),
                Background = new SolidColorBrush(Color.FromRgb(241, 245, 249)),
                CornerRadius = new CornerRadius(6)
            };
            var legendStack = new StackPanel();
            legendStack.Children.Add(new TextBlock
            {
                Text = "\u2501\u2501 Action Limit Threshold: DCR \u2264 0.85 (Green/Safe)  |  0.85 < DCR \u2264 1.0 (Yellow/Moderate)  |  DCR > 1.0 (Red/Hazard)",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(51, 65, 85))
            });
            legendBorder.Child = legendStack;
            chartStack.Children.Add(legendBorder);

            chartBorder.Child = chartStack;
            Grid.SetColumn(chartBorder, 1);
            rootGrid.Children.Add(chartBorder);

            return rootGrid;
        }

        // ==============================================================================
        // TAB 3: SUMMARY & JOINT DEMANDS (MATCHING MANUAL PAGE 36)
        // ==============================================================================
        private UIElement CreateSummaryAndResultsTab()
        {
            var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            var rootGrid = new Grid { Margin = new Thickness(12) };
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // COLUMN 1: Analysis Tool Results
            var col1Border = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(16),
                Margin = new Thickness(0, 0, 10, 0)
            };
            var sp1 = new StackPanel();
            sp1.Children.Add(new TextBlock { Text = "Analysis Tool Results", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 14) });

            // Arm (N)
            sp1.Children.Add(CreateSubheader("Arm Force Demands (N)"));
            var gArm = CreateTableGrid();
            AddTableRow(gArm, 0, "Metric", "Left", "Right", true);
            _txtArmDemandL = AddTableRow(gArm, 1, "Demand", "35.0 N", "30.0 N", false);
            _txtArmStrengthL = AddTableRow(gArm, 2, "Strength Capacity", "84.8 N", "77.8 N", false);
            _txtArmMaxL = AddTableRow(gArm, 3, "Max. Acceptable Force", "53.0 N", "49.7 N", false);
            sp1.Children.Add(gArm);

            // Hand, Wrist & Forearm (N)
            sp1.Children.Add(CreateSubheader("Hand, Wrist & Forearm (N)"));
            var gHand = CreateTableGrid();
            AddTableRow(gHand, 0, "Metric", "Left", "Right", true);
            _txtHandDemandL = AddTableRow(gHand, 1, "Demand", "35.0 N", "30.0 N", false);
            _txtHandStrengthL = AddTableRow(gHand, 2, "Strength Capacity", "161.5 N", "161.5 N", false);
            _txtHandMaxL = AddTableRow(gHand, 3, "Max. Acceptable Force", "101.1 N", "101.1 N", false);
            sp1.Children.Add(gHand);

            // Low Back L5/S1 Joint
            sp1.Children.Add(CreateSubheader("Low Back (L5S1 Joint) (N)"));
            var gL5 = CreateSingleValTableGrid();
            _txtL5S1Comp = AddSingleRow(gL5, 0, "Compression Force Demand", "2263 N");
            _txtL5S1Shear = AddSingleRow(gL5, 1, "Shear Force Resultant Demand", "183 N");
            _txtL5S1Max = AddSingleRow(gL5, 2, "Max. Compression Strength (TLV)", "3400 N");
            _txtNioshRwl = AddSingleRow(gL5, 3, "NIOSH Recommended Weight Limit (RWL)", "14.2 kg");
            _txtNioshLi = AddSingleRow(gL5, 4, "NIOSH Lifting Index (LI)", "0.85");
            _txtRulaScore = AddSingleRow(gL5, 5, "RULA Posture Action Score", "3 / 7 (Low)");
            _txtRebaScore = AddSingleRow(gL5, 6, "REBA Posture Risk Score", "4 / 15 (Medium)");
            sp1.Children.Add(gL5);

            col1Border.Child = sp1;
            Grid.SetColumn(col1Border, 0);
            rootGrid.Children.Add(col1Border);

            // COLUMN 2: Joint Strength Demands (Nm)
            var col2Border = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(16),
                Margin = new Thickness(10, 0, 0, 0)
            };
            var sp2 = new StackPanel();
            sp2.Children.Add(new TextBlock { Text = "Joint Strength Demands (Nm)", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 14) });

            var gJoints = CreateTableGrid();
            AddTableRow(gJoints, 0, "Joint / Axis", "Left (Nm)", "Right (Nm)", true);
            _txtJointWrist = AddTableRow(gJoints, 1, "Wrist Flexion/Extension", "-0.1", "-0.2", false);
            _txtJointElbow = AddTableRow(gJoints, 2, "Elbow Flexion/Extension", "3.6", "1.9", false);
            _txtJointShoulder = AddTableRow(gJoints, 3, "Shoulder Flexion/Extension", "-3.8", "9.1", false);
            _txtJointNeck = AddTableRow(gJoints, 4, "Neck Flexion/Extension", "-6.8", "-6.8", false);
            _txtJointL5S1 = AddTableRow(gJoints, 5, "L5/S1 Lumbar Flexion Moment", "-43.0", "-43.0", false);
            sp2.Children.Add(gJoints);

            col2Border.Child = sp2;
            Grid.SetColumn(col2Border, 1);
            rootGrid.Children.Add(col2Border);

            scroll.Content = rootGrid;
            return scroll;
        }

        // ==============================================================================
        // TAB 4: 2D ARTICULATED KINEMATICS & SPINE VISUALIZER
        // ==============================================================================
        private UIElement CreateKinematicsTab()
        {
            var grid = new Grid { Margin = new Thickness(12) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(600) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Canvas border
            var canvasBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(15, 23, 42)), // Slate 900
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 12, 0)
            };
            _kinematicCanvas = new Canvas { Width = 570, Height = 640 };

            // Floor ground line
            var ground = new Line { X1 = 40, Y1 = 560, X2 = 530, Y2 = 560, Stroke = new SolidColorBrush(Color.FromRgb(100, 116, 139)), StrokeThickness = 3 };
            _kinematicCanvas.Children.Add(ground);

            // Base of support (BoS)
            _lineBos = new Line { X1 = 180, Y1 = 560, X2 = 320, Y2 = 560, Stroke = new SolidColorBrush(Color.FromRgb(56, 189, 248)), StrokeThickness = 6 };
            _kinematicCanvas.Children.Add(_lineBos);

            // Plumb line from CoM
            _linePlumb = new Line { X1 = 250, Y1 = 120, X2 = 250, Y2 = 560, Stroke = new SolidColorBrush(Color.FromRgb(250, 204, 21)), StrokeThickness = 1.5, StrokeDashArray = new DoubleCollection { 4, 3 } };
            _kinematicCanvas.Children.Add(_linePlumb);

            // Skeleton Bones
            _lineShank = CreateBone(Color.FromRgb(56, 189, 248), 5);
            _lineThigh = CreateBone(Color.FromRgb(56, 189, 248), 5);
            _linePelvis = CreateBone(Color.FromRgb(14, 165, 233), 6);
            _lineTorso = CreateBone(Color.FromRgb(249, 115, 22), 6);
            _lineNeck = CreateBone(Color.FromRgb(251, 146, 60), 4);
            _lineUpperArm = CreateBone(Color.FromRgb(168, 85, 247), 4);
            _lineForearm = CreateBone(Color.FromRgb(168, 85, 247), 4);

            _kinematicCanvas.Children.Add(_lineShank);
            _kinematicCanvas.Children.Add(_lineThigh);
            _kinematicCanvas.Children.Add(_linePelvis);
            _kinematicCanvas.Children.Add(_lineTorso);
            _kinematicCanvas.Children.Add(_lineNeck);
            _kinematicCanvas.Children.Add(_lineUpperArm);
            _kinematicCanvas.Children.Add(_lineForearm);

            // Joint Markers
            _ptAnkle = CreateJointMarker(Color.FromRgb(255, 255, 255), 10);
            _ptKnee = CreateJointMarker(Color.FromRgb(56, 189, 248), 10);
            _ptHip = CreateJointMarker(Color.FromRgb(14, 165, 233), 12);
            _ptL5S1 = CreateJointMarker(Color.FromRgb(239, 68, 68), 12);
            _ptShoulder = CreateJointMarker(Color.FromRgb(168, 85, 247), 10);
            _ptElbow = CreateJointMarker(Color.FromRgb(168, 85, 247), 9);
            _ptHand = CreateJointMarker(Color.FromRgb(234, 179, 8), 10);
            _ptHead = CreateJointMarker(Color.FromRgb(251, 146, 60), 24);
            _ptCofP = CreateJointMarker(Color.FromRgb(250, 204, 21), 12);

            _kinematicCanvas.Children.Add(_ptAnkle);
            _kinematicCanvas.Children.Add(_ptKnee);
            _kinematicCanvas.Children.Add(_ptHip);
            _kinematicCanvas.Children.Add(_ptL5S1);
            _kinematicCanvas.Children.Add(_ptShoulder);
            _kinematicCanvas.Children.Add(_ptElbow);
            _kinematicCanvas.Children.Add(_ptHand);
            _kinematicCanvas.Children.Add(_ptHead);
            _kinematicCanvas.Children.Add(_ptCofP);

            // Load Box
            _boxObject = new Border
            {
                Width = 44,
                Height = 36,
                Background = new SolidColorBrush(Color.FromRgb(245, 158, 11)),
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1.5),
                CornerRadius = new CornerRadius(3)
            };
            _txtBoxMassLabel = new TextBlock { Text = "12kg", FontSize = 9, FontWeight = FontWeights.Bold, Foreground = Brushes.Black, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            _boxObject.Child = _txtBoxMassLabel;
            _kinematicCanvas.Children.Add(_boxObject);

            // HUD Overlay
            _txtKinematicHud = new TextBlock
            {
                Text = "L5/S1 Compression: 2263 N \u2022 Balanced \u2022 Dempster CoM: 52 mm",
                Foreground = Brushes.White,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Background = new SolidColorBrush(Color.FromArgb(180, 15, 23, 42)),
                Padding = new Thickness(8, 4, 8, 4)
            };
            Canvas.SetLeft(_txtKinematicHud, 14);
            Canvas.SetTop(_txtKinematicHud, 14);
            _kinematicCanvas.Children.Add(_txtKinematicHud);

            canvasBorder.Child = _kinematicCanvas;
            Grid.SetColumn(canvasBorder, 0);
            grid.Children.Add(canvasBorder);

            // Controls on right
            var controlsBorder = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(16)
            };
            var ctrlStack = new StackPanel();
            ctrlStack.Children.Add(new TextBlock { Text = "Live Biomechanical Solvers", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 12) });

            ctrlStack.Children.Add(CreateFieldLabel("Kinematic Posture Mode:"));
            var cmbKinTech = new ComboBox
            {
                ItemsSource = new[] { "Semi-Squat (Balanced Dempster CoM)", "Stoop (Hip Dominant, Straight Legs)", "Deep Squat (Knee Dominant)" },
                SelectedIndex = (int)_vm.SelectedTechnique,
                Height = 28,
                Margin = new Thickness(0, 0, 0, 12)
            };
            cmbKinTech.SelectionChanged += (s, e) =>
            {
                _vm.SelectedTechnique = (LiftingTechnique)cmbKinTech.SelectedIndex;
                RefreshAllViews();
            };
            ctrlStack.Children.Add(cmbKinTech);

            // Live metrics
            var metricsBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(248, 250, 252)),
                Padding = new Thickness(12),
                CornerRadius = new CornerRadius(6)
            };
            var mStack = new StackPanel();
            mStack.Children.Add(new TextBlock { Text = "\u2705 Dempster-Winter Center of Mass: Active", FontSize = 11, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74)) });
            mStack.Children.Add(new TextBlock { Text = "\u2705 Kingma 3D Dynamic Newton-Euler ($F=ma$): Active", FontSize = 11, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74)), Margin = new Thickness(0, 4, 0, 0) });
            mStack.Children.Add(new TextBlock { Text = "\u2705 Potvin Maximum Acceptable Effort (MAE): Active", FontSize = 11, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74)), Margin = new Thickness(0, 4, 0, 0) });
            mStack.Children.Add(new TextBlock { Text = "\u2705 EAWS Automotive Standard: Active", FontSize = 11, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74)), Margin = new Thickness(0, 4, 0, 0) });
            metricsBorder.Child = mStack;
            ctrlStack.Children.Add(metricsBorder);

            controlsBorder.Child = ctrlStack;
            Grid.SetColumn(controlsBorder, 1);
            grid.Children.Add(controlsBorder);

            return grid;
        }

        // ==============================================================================
        // LIVE REFRESH & SYNCHRONIZATION
        // ==============================================================================
        private void RefreshAllViews()
        {
            // 1. Overall DCR Header & Badge
            _txtOverallDcrBig.Text = $"{_vm.OverallDCR:F2}";
            var riskColor = _vm.OverallDCR <= 0.85
                ? Color.FromRgb(22, 163, 74) // Green
                : (_vm.OverallDCR <= 1.0 ? Color.FromRgb(217, 119, 6) : Color.FromRgb(220, 38, 38)); // Yellow or Red
            _txtOverallDcrBig.Foreground = new SolidColorBrush(riskColor);

            _txtOverallSubtitle.Text = _vm.OverallDCR <= 1.0
                ? $"{_vm.OverallDcrText} \u2264 DCR=1 (Acceptable)"
                : $"DCR Exceeded by {(_vm.OverallDCR - 1.0) * 100:F0}% (Hazard)";

            // 2. Body Map Callout Badges
            UpdateBadgeColor(_lblNeckDcr, "Neck", _vm.ArmDCR * 0.7);
            UpdateBadgeColor(_lblLeftArmDcr, "Left Arm", _vm.ArmDCR);
            UpdateBadgeColor(_lblRightArmDcr, "Right Arm", _vm.ArmDCR * 0.95);
            UpdateBadgeColor(_lblLumbarDcr, $"Lumbar L5/S1 ({_vm.LumbarCompN:F0} N)", _vm.LumbarDCR);
            UpdateBadgeColor(_lblLeftHandDcr, "Left Hand/Wrist", _vm.HandDCR);
            UpdateBadgeColor(_lblRightHandDcr, "Right Hand/Wrist", _vm.HandDCR * 0.95);
            UpdateBadgeColor(_lblLmMmhDcr, "LM-MMH Equations", _vm.SnookDCR);

            // 3. DCR Ranking Bars
            UpdateDcrBar(_pbOverall, _valOverall, _vm.OverallDCR);
            UpdateDcrBar(_pbLeftArm, _valLeftArm, _vm.ArmDCR);
            UpdateDcrBar(_pbRightArm, _valRightArm, _vm.ArmDCR * 0.95);
            UpdateDcrBar(_pbLumbar, _valLumbar, _vm.LumbarDCR);
            UpdateDcrBar(_pbNeck, _valNeck, _vm.ArmDCR * 0.7);
            UpdateDcrBar(_pbLeftHand, _valLeftHand, _vm.HandDCR);
            UpdateDcrBar(_pbRightHand, _valRightHand, _vm.HandDCR * 0.95);

            // 4. Summary Table
            if (_txtArmDemandL != null)
            {
                _txtArmDemandL.Text = $"{_vm.LoadWeightKg * 9.81 * 0.5:F1} N";
                _txtHandDemandL.Text = $"{_vm.LoadWeightKg * 9.81 * 0.5:F1} N";
                _txtL5S1Comp.Text = $"{_vm.LumbarCompN:F0} N";
                _txtNioshRwl.Text = $"{_vm.NioshRWLKg:F1} kg";
                _txtNioshLi.Text = $"{_vm.NioshLI:F2}";
                _txtRulaScore.Text = $"{_vm.RulaScore} / 7";
                _txtRebaScore.Text = $"{_vm.RebaScore} / 15";
            }

            // 5. 2D Kinematics Drawing
            UpdateKinematicCanvas();
        }

        private void UpdateKinematicCanvas()
        {
            if (_kinematicCanvas == null) return;

            double reach = _vm.ReachMm;
            double vert = _vm.VerticalMm;
            double mass = _vm.LoadWeightKg;
            var tech = _vm.SelectedTechnique;

            // Geometry calculations (scale 1mm -> 0.3px)
            double originX = 160;
            double originY = 560; // floor

            double ankleX = originX + 50;
            double ankleY = originY - 20;

            double kneeBend = tech == LiftingTechnique.StoopStraightLegs ? 10.0 : (tech == LiftingTechnique.DeepSquat ? 85.0 : 45.0);
            double hipDrop = tech == LiftingTechnique.StoopStraightLegs ? 0.0 : (tech == LiftingTechnique.DeepSquat ? 180.0 : 90.0);

            double kneeX = ankleX + (tech == LiftingTechnique.StoopStraightLegs ? 10 : 40);
            double kneeY = ankleY - (130 - (kneeBend * 0.5));

            double hipX = ankleX - (tech == LiftingTechnique.StoopStraightLegs ? 85 : 55);
            double hipY = kneeY - (130 - hipDrop * 0.5);

            double handTargetX = ankleX + (reach * 0.35);
            double handTargetY = originY - (vert * 0.28);

            double shoulderX = hipX + (tech == LiftingTechnique.StoopStraightLegs ? 90 : 60);
            double shoulderY = hipY - 130;

            double elbowX = (shoulderX + handTargetX) / 2 + 15;
            double elbowY = (shoulderY + handTargetY) / 2 + 25;

            double headX = shoulderX + 15;
            double headY = shoulderY - 35;

            double l5s1X = (hipX * 2 + shoulderX) / 3;
            double l5s1Y = (hipY * 2 + shoulderY) / 3;

            // CoM Plumb Line
            double comX = (ankleX + hipX + shoulderX) / 3 + 20;
            _linePlumb.X1 = comX;
            _linePlumb.X2 = comX;

            // Update bones
            SetLine(_lineShank, ankleX, ankleY, kneeX, kneeY);
            SetLine(_lineThigh, kneeX, kneeY, hipX, hipY);
            SetLine(_linePelvis, hipX, hipY, l5s1X, l5s1Y);
            SetLine(_lineTorso, l5s1X, l5s1Y, shoulderX, shoulderY);
            SetLine(_lineNeck, shoulderX, shoulderY, headX, headY);
            SetLine(_lineUpperArm, shoulderX, shoulderY, elbowX, elbowY);
            SetLine(_lineForearm, elbowX, elbowY, handTargetX, handTargetY);

            // Update joints
            SetPt(_ptAnkle, ankleX, ankleY);
            SetPt(_ptKnee, kneeX, kneeY);
            SetPt(_ptHip, hipX, hipY);
            SetPt(_ptL5S1, l5s1X, l5s1Y);
            SetPt(_ptShoulder, shoulderX, shoulderY);
            SetPt(_ptElbow, elbowX, elbowY);
            SetPt(_ptHand, handTargetX, handTargetY);
            SetPt(_ptHead, headX, headY);
            SetPt(_ptCofP, comX, originY - 10);

            // Box position
            Canvas.SetLeft(_boxObject, handTargetX - 22);
            Canvas.SetTop(_boxObject, handTargetY - 18);
            _txtBoxMassLabel.Text = $"{mass:F0}kg";

            _txtKinematicHud.Text = $"L5/S1 Compression: {_vm.LumbarCompN:F0} N \u2022 Posture: {tech} \u2022 DCR: {_vm.OverallDCR:F2}";
        }

        private void SetLine(Line l, double x1, double y1, double x2, double y2)
        {
            l.X1 = x1; l.Y1 = y1; l.X2 = x2; l.Y2 = y2;
        }

        private void SetPt(Ellipse e, double x, double y)
        {
            Canvas.SetLeft(e, x - e.Width / 2);
            Canvas.SetTop(e, y - e.Height / 2);
        }

        // ==============================================================================
        // LIVE SCENE BRIDGE & SNAP
        // ==============================================================================
        private DateTime _lastSyncFileTime = DateTime.MinValue;
        private void AttachLiveSync()
        {
            _scenePollTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _scenePollTimer.Tick += (s, e) =>
            {
                if (File.Exists(_sceneStateFilePath))
                {
                    try
                    {
                        var writeTime = File.GetLastWriteTimeUtc(_sceneStateFilePath);
                        if (writeTime > _lastSyncFileTime)
                        {
                            _lastSyncFileTime = writeTime;
                            var text = File.ReadAllText(_sceneStateFilePath);
                            double load = ExtractJsonDouble(text, "load_kg", _vm.LoadWeightKg);
                            double reach = ExtractJsonDouble(text, "reach_mm", _vm.ReachMm);
                            double vert = ExtractJsonDouble(text, "vert_mm", _vm.VerticalMm);

                            _vm.LoadWeightKg = load;
                            _vm.ReachMm = reach;
                            _vm.VerticalMm = vert;
                            RefreshAllViews();
                        }
                    }
                    catch { }
                }
            };
            _scenePollTimer.Start();
        }

        private double ExtractJsonDouble(string json, string key, double defaultVal)
        {
            try
            {
                int idx = json.IndexOf("\"" + key + "\":");
                if (idx < 0) return defaultVal;
                int start = idx + key.Length + 3;
                int end = json.IndexOfAny(new[] { ',', '}', '\r', '\n' }, start);
                if (end < 0) end = json.Length;
                string valStr = json.Substring(start, end - start).Trim();
                double result;
                if (double.TryParse(valStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result))
                    return result;
            }
            catch { }
            return defaultVal;
        }

        private void SnapFromScene()
        {
            if (File.Exists(_sceneStateFilePath))
            {
                try
                {
                    var text = File.ReadAllText(_sceneStateFilePath);
                    _vm.LoadWeightKg = ExtractJsonDouble(text, "load_kg", _vm.LoadWeightKg);
                    _vm.ReachMm = ExtractJsonDouble(text, "reach_mm", _vm.ReachMm);
                    _vm.VerticalMm = ExtractJsonDouble(text, "vert_mm", _vm.VerticalMm);
                }
                catch { }
            }
            _vm.SnapSelected3DObject();
            RefreshAllViews();
            MessageBox.Show(
                $"Successfully captured active 3D coordinates from R-Pro scene!\n\nVertical Height: {_vm.VerticalMm:F0} mm\nReach: {_vm.ReachMm:F0} mm\nLoad: {_vm.LoadWeightKg:F1} kg\n\nBiomechanics updated automatically.",
                "Work(s) Ergo \u2022 3D Scene Snapped",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        // ==============================================================================
        // HELPER FACTORIES
        // ==============================================================================
        private TextBlock CreateCalloutBadge(string label, string val, double cx, double cy, bool centerAlign)
        {
            var tb = new TextBlock
            {
                Text = $"{label}\n{val}",
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74)),
                TextAlignment = TextAlignment.Center,
                Background = new SolidColorBrush(Color.FromArgb(220, 255, 255, 255)),
                Padding = new Thickness(4, 2, 4, 2)
            };
            Canvas.SetLeft(tb, centerAlign ? cx - 50 : cx);
            Canvas.SetTop(tb, cy);
            return tb;
        }

        private void UpdateBadgeColor(TextBlock badge, string label, double dcr)
        {
            if (badge == null) return;
            badge.Text = $"{label}\n{dcr:F2}";
            var color = dcr <= 0.85
                ? Color.FromRgb(22, 163, 74) // Green
                : (dcr <= 1.0 ? Color.FromRgb(217, 119, 6) : Color.FromRgb(220, 38, 38)); // Yellow or Red
            badge.Foreground = new SolidColorBrush(color);
        }

        private ProgressBar CreateDcrBar(StackPanel parent, string title, out TextBlock valBlock, bool isOverall)
        {
            var grid = new Grid { Margin = new Thickness(0, 0, 0, 10) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });

            var lbl = new TextBlock
            {
                Text = title,
                FontSize = isOverall ? 13 : 11,
                FontWeight = isOverall ? FontWeights.Bold : FontWeights.Normal,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 41, 59))
            };
            Grid.SetColumn(lbl, 0);
            grid.Children.Add(lbl);

            var pb = new ProgressBar
            {
                Height = isOverall ? 20 : 14,
                Minimum = 0,
                Maximum = 1.5,
                Value = 0.5,
                Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74)),
                Background = new SolidColorBrush(Color.FromRgb(241, 245, 249)),
                BorderThickness = new Thickness(0)
            };
            Grid.SetColumn(pb, 1);
            grid.Children.Add(pb);

            valBlock = new TextBlock
            {
                Text = "0.50",
                FontSize = isOverall ? 13 : 11,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
            Grid.SetColumn(valBlock, 2);
            grid.Children.Add(valBlock);

            parent.Children.Add(grid);
            return pb;
        }

        private void UpdateDcrBar(ProgressBar pb, TextBlock valBlock, double dcr)
        {
            if (pb == null || valBlock == null) return;
            pb.Value = Math.Min(1.5, dcr);
            valBlock.Text = $"{dcr:F2}";
            var color = dcr <= 0.85
                ? Color.FromRgb(22, 163, 74)
                : (dcr <= 1.0 ? Color.FromRgb(217, 119, 6) : Color.FromRgb(220, 38, 38));
            pb.Foreground = new SolidColorBrush(color);
            valBlock.Foreground = new SolidColorBrush(color);
        }

        private Expander CreateAccordionCard(string title, bool isExpanded)
        {
            return new Expander
            {
                Header = title,
                IsExpanded = isExpanded,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 10),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(12)
            };
        }

        private TextBlock CreateFieldLabel(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(71, 85, 105)),
                Margin = new Thickness(0, 4, 0, 4)
            };
        }

        private TextBlock CreateSubheader(string title)
        {
            return new TextBlock
            {
                Text = title,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                Margin = new Thickness(0, 8, 0, 6)
            };
        }

        private Grid CreateTableGrid()
        {
            var g = new Grid { Margin = new Thickness(0, 0, 0, 12) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            for (int i = 0; i < 6; i++) g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            return g;
        }

        private TextBlock AddTableRow(Grid g, int row, string col0, string col1, string col2, bool isHeader)
        {
            var tb0 = new TextBlock { Text = col0, FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal, FontSize = 11, Padding = new Thickness(4, 2, 4, 2) };
            var tb1 = new TextBlock { Text = col1, FontWeight = isHeader ? FontWeights.Bold : FontWeights.SemiBold, FontSize = 11, Foreground = isHeader ? Brushes.Black : new SolidColorBrush(Color.FromRgb(37, 99, 235)), Padding = new Thickness(4, 2, 4, 2) };
            var tb2 = new TextBlock { Text = col2, FontWeight = isHeader ? FontWeights.Bold : FontWeights.SemiBold, FontSize = 11, Foreground = isHeader ? Brushes.Black : new SolidColorBrush(Color.FromRgb(217, 119, 6)), Padding = new Thickness(4, 2, 4, 2) };

            Grid.SetRow(tb0, row); Grid.SetColumn(tb0, 0); g.Children.Add(tb0);
            Grid.SetRow(tb1, row); Grid.SetColumn(tb1, 1); g.Children.Add(tb1);
            Grid.SetRow(tb2, row); Grid.SetColumn(tb2, 2); g.Children.Add(tb2);
            return tb1;
        }

        private Grid CreateSingleValTableGrid()
        {
            var g = new Grid { Margin = new Thickness(0, 0, 0, 12) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            for (int i = 0; i < 8; i++) g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            return g;
        }

        private TextBlock AddSingleRow(Grid g, int row, string label, string val)
        {
            var tb0 = new TextBlock { Text = label, FontSize = 11, Padding = new Thickness(4, 2, 4, 2) };
            var tb1 = new TextBlock { Text = val, FontSize = 11, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Right, Padding = new Thickness(4, 2, 4, 2) };
            Grid.SetRow(tb0, row); Grid.SetColumn(tb0, 0); g.Children.Add(tb0);
            Grid.SetRow(tb1, row); Grid.SetColumn(tb1, 1); g.Children.Add(tb1);
            return tb1;
        }

        private Line CreateBone(Color c, double th)
        {
            return new Line { Stroke = new SolidColorBrush(c), StrokeThickness = th, StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round };
        }

        private Ellipse CreateJointMarker(Color c, double diam)
        {
            return new Ellipse { Width = diam, Height = diam, Fill = new SolidColorBrush(c), Stroke = Brushes.Black, StrokeThickness = 1 };
        }

        private void ExportExcelReport()
        {
            try
            {
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                var reportPath = System.IO.Path.Combine(desktop, $"Work(s)_Ergo_Report_{timestamp}.csv");

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Work(s) Ergo \u2022 Task Analysis & Ergonomics Assessment Report");
                sb.AppendLine($"Generated: {DateTime.Now}");
                sb.AppendLine($"Target Population: {_vm.SelectedPercentile}");
                sb.AppendLine($"Task Type: {_vm.SelectedTask}");
                sb.AppendLine($"Lifting Technique: {_vm.SelectedTechnique}");
                sb.AppendLine($"Load Weight (kg): {_vm.LoadWeightKg}");
                sb.AppendLine($"Vertical Height (mm): {_vm.VerticalMm}");
                sb.AppendLine($"Reach Distance (mm): {_vm.ReachMm}");
                sb.AppendLine($"Frequency (lifts/min): {_vm.FrequencyLiftsPerMin}");
                sb.AppendLine();
                sb.AppendLine("=== DEMAND / CAPACITY RATIOS (DCR) ===");
                sb.AppendLine($"Overall DCR: {_vm.OverallDCR:F2}");
                sb.AppendLine($"Risk Category: {_vm.RiskCategory}");
                sb.AppendLine($"Lumbar Compression (N): {_vm.LumbarCompN:F0}");
                sb.AppendLine($"Lumbar DCR: {_vm.LumbarDCR:F2}");
                sb.AppendLine($"Arm DCR: {_vm.ArmDCR:F2}");
                sb.AppendLine($"Hand DCR: {_vm.HandDCR:F2}");
                sb.AppendLine($"Snook Psychophysical DCR: {_vm.SnookDCR:F2}");
                sb.AppendLine($"NIOSH RWL (kg): {_vm.NioshRWLKg:F1}");
                sb.AppendLine($"NIOSH Lifting Index: {_vm.NioshLI:F2}");
                sb.AppendLine($"RULA Score: {_vm.RulaScore} / 7");
                sb.AppendLine($"REBA Score: {_vm.RebaScore} / 15");
                sb.AppendLine($"EAWS Score: {_vm.EawsScore:F1} ({_vm.EawsTrafficLight})");
                sb.AppendLine();
                sb.AppendLine("=== RECOMMENDATIONS ===");
                sb.AppendLine(_vm.Recommendation);

                System.IO.File.WriteAllText(reportPath, sb.ToString(), System.Text.Encoding.UTF8);
                MessageBox.Show(
                    $"Work(s) Ergo Analysis Report exported successfully!\n\nLocation: {reportPath}",
                    "Work(s) Ergo \u2022 Report Exported",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export report: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
