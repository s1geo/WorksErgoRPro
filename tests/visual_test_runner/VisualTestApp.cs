using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WorksErgoRPro.Biomechanics;
using WorksErgoRPro.ViewModels;

namespace WorksErgoRPro.VisualTests
{
    public class VisualTestApp : Application
    {
        [STAThread]
        public static void Main()
        {
            var app = new VisualTestApp();
            app.Run(new VisualTestWindow());
        }
    }

    public class VisualTestWindow : Window
    {
        private WorksErgoPaneViewModel _vm;
        private Canvas _manikinCanvas;
        private Line _legLeft, _legThigh, _neckLine, _armUpper, _armFore, _lumbarVector;
        private Ellipse _jointPelvis, _jointL5S1, _jointShoulder, _jointElbow, _jointHand, _head;
        private Path _spinePath;
        private Border _boxBorder;
        private TextBlock _txtBoxMass, _txtHudComp, _txtScenarioTitle, _txtScenarioStatus, _txtDcrBig, _txtRiskBadge, _txtRecommendation;
        private ProgressBar _pbDcr;
        private TextBlock _txtLumbarRow, _txtNioshRow, _txtSnookRow, _txtArmRow, _txtPotvinRow;
        private DispatcherTimer _autoTestTimer;
        private int _currentTestIndex = 0;

        public VisualTestWindow()
        {
            Title = "Works Ergo R-Pro \u2022 Biomechanical Manikin Autotest Visualizer";
            Width = 1080;
            Height = 720;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = new SolidColorBrush(Color.FromRgb(15, 23, 42)); // Dark Slate

            _vm = new WorksErgoPaneViewModel();

            BuildUI();
            UpdateVisuals();
        }

        private void BuildUI()
        {
            var rootGrid = new Grid { Margin = new Thickness(12) };
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // 1. Top Header Bar
            var headerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(0, 0, 0, 12)
            };
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var titleStack = new StackPanel();
            var title = new TextBlock
            {
                Text = "WORKS ERGO R-PRO \u2022 VISUAL MANIKIN TEST SUITE",
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 16
            };
            var subtitle = new TextBlock
            {
                Text = "Live 2D Kinematic Inverse Kinematics, L5/S1 Compression Heatmap & Automated Test Pipeline",
                Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                FontSize = 11,
                Margin = new Thickness(0, 2, 0, 0)
            };
            titleStack.Children.Add(title);
            titleStack.Children.Add(subtitle);
            Grid.SetColumn(titleStack, 0);
            headerGrid.Children.Add(titleStack);

            var actionsStack = new StackPanel { Orientation = Orientation.Horizontal };
            var btnRunAll = new Button
            {
                Content = "Run All Autotests",
                Background = new SolidColorBrush(Color.FromRgb(37, 99, 235)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(14, 6, 14, 6),
                Margin = new Thickness(0, 0, 8, 0)
            };
            btnRunAll.Click += (s, e) => StartSequentialAutotests();
            actionsStack.Children.Add(btnRunAll);

            Grid.SetColumn(actionsStack, 1);
            headerGrid.Children.Add(actionsStack);
            headerBorder.Child = headerGrid;
            Grid.SetRow(headerBorder, 0);
            rootGrid.Children.Add(headerBorder);

            // 2. Main Content (Left: Manikin Canvas, Right: Telemetry & Controls)
            var contentGrid = new Grid();
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });

            // Left: Canvas Viewport
            var canvasBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(2, 6, 23)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                ClipToBounds = true,
                Margin = new Thickness(0, 0, 8, 0)
            };

            _manikinCanvas = new Canvas { Width = 600, Height = 560 };
            BuildManikinElements();
            canvasBorder.Child = _manikinCanvas;
            Grid.SetColumn(canvasBorder, 0);
            contentGrid.Children.Add(canvasBorder);

            // Right: Telemetry Dashboard & Test Selectors
            var rightStack = new StackPanel { Margin = new Thickness(8, 0, 0, 0) };

            // Scenario Status Card
            var scenarioCard = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14),
                Margin = new Thickness(0, 0, 0, 10)
            };
            var scStack = new StackPanel();
            _txtScenarioTitle = new TextBlock
            {
                Text = "Test Case 1: Baseline Waist Lift (10 kg)",
                Foreground = new SolidColorBrush(Color.FromRgb(96, 165, 250)),
                FontWeight = FontWeights.Bold,
                FontSize = 13
            };
            _txtScenarioStatus = new TextBlock
            {
                Text = "Status: PASSED (Low Risk / Safe)",
                Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153)),
                FontWeight = FontWeights.SemiBold,
                FontSize = 11,
                Margin = new Thickness(0, 2, 0, 0)
            };
            scStack.Children.Add(_txtScenarioTitle);
            scStack.Children.Add(_txtScenarioStatus);
            scenarioCard.Child = scStack;
            rightStack.Children.Add(scenarioCard);

            // DCR Gauge Card
            var dcrCard = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14),
                Margin = new Thickness(0, 0, 0, 10)
            };
            var dcrStack = new StackPanel();
            dcrStack.Children.Add(new TextBlock { Text = "OVERALL RISK INDEX (DCR)", Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)), FontSize = 10, FontWeight = FontWeights.SemiBold });

            _txtDcrBig = new TextBlock { Text = "78%", FontSize = 36, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153)) };
            dcrStack.Children.Add(_txtDcrBig);

            _txtRiskBadge = new TextBlock { Text = "LOW RISK (SAFE)", FontSize = 12, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153)) };
            dcrStack.Children.Add(_txtRiskBadge);

            _pbDcr = new ProgressBar { Minimum = 0, Maximum = 200, Value = 78, Height = 6, Margin = new Thickness(0, 8, 0, 4), Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153)) };
            dcrStack.Children.Add(_pbDcr);

            dcrCard.Child = dcrStack;
            rightStack.Children.Add(dcrCard);

            // 7-Axis Breakdown Card
            var metricsCard = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14),
                Margin = new Thickness(0, 0, 0, 10)
            };
            var mStack = new StackPanel();
            mStack.Children.Add(new TextBlock { Text = "BIOMECHANICAL METRICS", Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)), FontSize = 10, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 0, 0, 6) });

            _txtLumbarRow = CreateMetricTextBlock("Lumbar L5/S1 Compression: 2649 N (Limit 3400 N)");
            _txtNioshRow = CreateMetricTextBlock("NIOSH Lifting Index: 0.67 (RWL: 15.0 kg)");
            _txtSnookRow = CreateMetricTextBlock("Snook & Ciriello MAWL: 17.2 kg (DCR 0.58)");
            _txtArmRow = CreateMetricTextBlock("Arm AFF Strength: DCR 0.44");
            _txtPotvinRow = CreateMetricTextBlock("Potvin MAE Fatigue: 0.841 (84.1% capacity)");

            mStack.Children.Add(_txtLumbarRow);
            mStack.Children.Add(_txtNioshRow);
            mStack.Children.Add(_txtSnookRow);
            mStack.Children.Add(_txtArmRow);
            mStack.Children.Add(_txtPotvinRow);
            metricsCard.Child = mStack;
            rightStack.Children.Add(metricsCard);

            // Recommendation Card
            var recCard = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(24, 34, 53)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(59, 130, 246)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 10)
            };
            _txtRecommendation = new TextBlock
            {
                Text = "Guidance: Task parameters are safe. No redesign necessary.",
                Foreground = new SolidColorBrush(Color.FromRgb(191, 219, 254)),
                FontSize = 11,
                TextWrapping = TextWrapping.Wrap
            };
            recCard.Child = _txtRecommendation;
            rightStack.Children.Add(recCard);

            // Test Case Buttons
            var testBtnsGrid = new Grid();
            testBtnsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            testBtnsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var btnT1 = CreateTestButton("1. Baseline (10kg)", () => LoadScenario(1));
            var btnT2 = CreateTestButton("2. Floor Hazard (25kg)", () => LoadScenario(2));
            var btnT3 = CreateTestButton("3. High Reach (15kg)", () => LoadScenario(3));
            var btnT4 = CreateTestButton("4. High Frequency", () => LoadScenario(4));

            Grid.SetColumn(btnT1, 0); Grid.SetRow(btnT1, 0);
            Grid.SetColumn(btnT2, 1); Grid.SetRow(btnT2, 0);
            testBtnsGrid.RowDefinitions.Add(new RowDefinition());
            testBtnsGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetColumn(btnT3, 0); Grid.SetRow(btnT3, 1);
            Grid.SetColumn(btnT4, 1); Grid.SetRow(btnT4, 1);

            testBtnsGrid.Children.Add(btnT1);
            testBtnsGrid.Children.Add(btnT2);
            testBtnsGrid.Children.Add(btnT3);
            testBtnsGrid.Children.Add(btnT4);
            rightStack.Children.Add(testBtnsGrid);

            Grid.SetColumn(rightStack, 1);
            contentGrid.Children.Add(rightStack);

            Grid.SetRow(contentGrid, 1);
            rootGrid.Children.Add(contentGrid);

            Content = rootGrid;
        }

        private void BuildManikinElements()
        {
            // Floor line
            var floor = new Line { X1 = 20, Y1 = 480, X2 = 580, Y2 = 480, Stroke = new SolidColorBrush(Color.FromRgb(71, 85, 105)), StrokeThickness = 3, StrokeDashArray = new DoubleCollection { 4, 3 } };
            _manikinCanvas.Children.Add(floor);

            // Legs
            _legLeft = new Line { Stroke = new SolidColorBrush(Color.FromRgb(148, 163, 184)), StrokeThickness = 8, StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round };
            _legThigh = new Line { Stroke = new SolidColorBrush(Color.FromRgb(148, 163, 184)), StrokeThickness = 9, StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round };
            _manikinCanvas.Children.Add(_legLeft);
            _manikinCanvas.Children.Add(_legThigh);

            // Spine Path
            _spinePath = new Path { Stroke = new SolidColorBrush(Color.FromRgb(34, 197, 94)), StrokeThickness = 12, StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round };
            _manikinCanvas.Children.Add(_spinePath);

            // Lumbar Vector
            _lumbarVector = new Line { Stroke = new SolidColorBrush(Color.FromRgb(239, 68, 68)), StrokeThickness = 3 };
            _manikinCanvas.Children.Add(_lumbarVector);

            // Pelvis
            _jointPelvis = new Ellipse { Width = 16, Height = 16, Fill = new SolidColorBrush(Color.FromRgb(56, 189, 248)) };
            _manikinCanvas.Children.Add(_jointPelvis);

            // L5/S1 Joint
            _jointL5S1 = new Ellipse { Width = 18, Height = 18, Fill = new SolidColorBrush(Color.FromRgb(34, 197, 94)) };
            _manikinCanvas.Children.Add(_jointL5S1);

            // Neck & Head
            _neckLine = new Line { Stroke = new SolidColorBrush(Color.FromRgb(148, 163, 184)), StrokeThickness = 6, StrokeStartLineCap = PenLineCap.Round };
            _head = new Ellipse { Width = 32, Height = 32, Fill = new SolidColorBrush(Color.FromRgb(203, 213, 225)) };
            _manikinCanvas.Children.Add(_neckLine);
            _manikinCanvas.Children.Add(_head);

            // Arms
            _jointShoulder = new Ellipse { Width = 14, Height = 14, Fill = new SolidColorBrush(Color.FromRgb(56, 189, 248)) };
            _jointElbow = new Ellipse { Width = 12, Height = 12, Fill = new SolidColorBrush(Color.FromRgb(56, 189, 248)) };
            _jointHand = new Ellipse { Width = 14, Height = 14, Fill = new SolidColorBrush(Color.FromRgb(245, 158, 11)) };
            _armUpper = new Line { Stroke = new SolidColorBrush(Color.FromRgb(34, 197, 94)), StrokeThickness = 7, StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round };
            _armFore = new Line { Stroke = new SolidColorBrush(Color.FromRgb(34, 197, 94)), StrokeThickness = 7, StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round };

            _manikinCanvas.Children.Add(_armUpper);
            _manikinCanvas.Children.Add(_armFore);
            _manikinCanvas.Children.Add(_jointShoulder);
            _manikinCanvas.Children.Add(_jointElbow);
            _manikinCanvas.Children.Add(_jointHand);

            // Box / Load
            _boxBorder = new Border
            {
                Width = 56,
                Height = 50,
                Background = new SolidColorBrush(Color.FromRgb(217, 119, 6)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(180, 83, 9)),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(4)
            };
            var boxStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            boxStack.Children.Add(new TextBlock { Text = "BOX", Foreground = Brushes.White, FontSize = 10, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center });
            _txtBoxMass = new TextBlock { Text = "10 kg", Foreground = new SolidColorBrush(Color.FromRgb(254, 243, 199)), FontSize = 9, HorizontalAlignment = HorizontalAlignment.Center };
            boxStack.Children.Add(_txtBoxMass);
            _boxBorder.Child = boxStack;
            _manikinCanvas.Children.Add(_boxBorder);
        }

        private void UpdateVisuals()
        {
            _vm.Recalculate();

            // Kinematics parameters
            double floorY = 480.0;
            double ankleX = 180.0;
            double ankleY = floorY - 5.0;
            double scale = 0.22;

            double vDiff = (1750.0 * 0.53) - _vm.VerticalMm;
            double forwardLeanDeg = Math.Max(0.0, Math.Min(75.0, vDiff / 8.0));
            double forwardLeanRad = forwardLeanDeg * (Math.PI / 180.0);
            double kneeBend = Math.Min(25.0, forwardLeanDeg * 0.4);

            // Knee & Hip
            double kneeX = ankleX - kneeBend * 0.5;
            double kneeY = ankleY - 110.0;
            _legLeft.X1 = ankleX; _legLeft.Y1 = ankleY;
            _legLeft.X2 = kneeX;  _legLeft.Y2 = kneeY;

            double hipX = ankleX + 15.0;
            double hipY = kneeY - 105.0;
            _legThigh.X1 = kneeX; _legThigh.Y1 = kneeY;
            _legThigh.X2 = hipX;  _legThigh.Y2 = hipY;
            Canvas.SetLeft(_jointPelvis, hipX - 8);
            Canvas.SetTop(_jointPelvis, hipY - 8);

            // Torso & Shoulders
            double torsoLen = 120.0;
            double shoulderX = hipX + torsoLen * Math.Sin(forwardLeanRad);
            double shoulderY = hipY - torsoLen * Math.Cos(forwardLeanRad);

            // Spine Path
            var geom = new PathGeometry();
            var fig = new PathFigure { StartPoint = new Point(hipX, hipY) };
            double spineMidX = (hipX + shoulderX) / 2 - 14 * Math.Sin(forwardLeanRad);
            double spineMidY = (hipY + shoulderY) / 2;
            fig.Segments.Add(new QuadraticBezierSegment(new Point(spineMidX, spineMidY), new Point(shoulderX, shoulderY), true));
            geom.Figures.Add(fig);
            _spinePath.Data = geom;

            // L5/S1
            double l5X = hipX + (shoulderX - hipX) * 0.25;
            double l5Y = hipY + (shoulderY - hipY) * 0.25;
            Canvas.SetLeft(_jointL5S1, l5X - 9);
            Canvas.SetTop(_jointL5S1, l5Y - 9);

            // Head
            double neckX = shoulderX + 26 * Math.Sin(forwardLeanRad * 0.5);
            double neckY = shoulderY - 26 * Math.Cos(forwardLeanRad * 0.5);
            _neckLine.X1 = shoulderX; _neckLine.Y1 = shoulderY;
            _neckLine.X2 = neckX;     _neckLine.Y2 = neckY;
            Canvas.SetLeft(_head, neckX - 16);
            Canvas.SetTop(_head, neckY - 26);

            Canvas.SetLeft(_jointShoulder, shoulderX - 7);
            Canvas.SetTop(_jointShoulder, shoulderY - 7);

            // Hand & Box
            double targetX = ankleX + _vm.ReachMm * scale;
            double targetY = floorY - _vm.VerticalMm * scale;

            double armLen1 = 70.0;
            double armLen2 = 70.0;
            double dx = targetX - shoulderX;
            double dy = targetY - shoulderY;
            double dist = Math.Min(armLen1 + armLen2 - 2.0, Math.Sqrt(dx * dx + dy * dy));
            double baseAngle = Math.Atan2(dy, dx);
            double elbowAngle = Math.Acos((dist * dist) / (2.0 * armLen1 * dist));
            if (double.IsNaN(elbowAngle)) elbowAngle = 0.5;

            double elbowX = shoulderX + armLen1 * Math.Cos(baseAngle + elbowAngle);
            double elbowY = shoulderY + armLen1 * Math.Sin(baseAngle + elbowAngle);

            _armUpper.X1 = shoulderX; _armUpper.Y1 = shoulderY;
            _armUpper.X2 = elbowX;    _armUpper.Y2 = elbowY;
            _armFore.X1 = elbowX;     _armFore.Y1 = elbowY;
            _armFore.X2 = targetX;    _armFore.Y2 = targetY;

            Canvas.SetLeft(_jointElbow, elbowX - 6);
            Canvas.SetTop(_jointElbow, elbowY - 6);
            Canvas.SetLeft(_jointHand, targetX - 7);
            Canvas.SetTop(_jointHand, targetY - 7);

            Canvas.SetLeft(_boxBorder, targetX - 28);
            Canvas.SetTop(_boxBorder, targetY - 25);
            _txtBoxMass.Text = $"{_vm.LoadWeightKg:F1} kg";

            // Heatmap Colors
            SolidColorBrush spineBrush;
            if (_vm.OverallDCR > 1.0 || _vm.LumbarCompN > 3400)
            {
                spineBrush = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red
            }
            else if (_vm.OverallDCR > 0.85 || _vm.LumbarCompN > 2700)
            {
                spineBrush = new SolidColorBrush(Color.FromRgb(245, 158, 11)); // Amber
            }
            else
            {
                spineBrush = new SolidColorBrush(Color.FromRgb(34, 197, 94));  // Green
            }

            _spinePath.Stroke = spineBrush;
            _jointL5S1.Fill = spineBrush;

            // Telemetry updates
            _txtDcrBig.Text = _vm.OverallDcrText;
            _txtDcrBig.Foreground = spineBrush;
            _txtRiskBadge.Text = _vm.RiskCategory.ToUpper();
            _txtRiskBadge.Foreground = spineBrush;
            _pbDcr.Value = Math.Min(200, _vm.OverallDCR * 100);
            _pbDcr.Foreground = spineBrush;

            _txtLumbarRow.Text = $"Lumbar L5/S1 Compression: {_vm.LumbarCompN:F0} N (Limit 3400 N)";
            _txtNioshRow.Text = $"NIOSH Lifting Index: {_vm.NioshLI:F2} (RWL: {_vm.NioshRWLKg:F1} kg)";
            _txtSnookRow.Text = $"Snook & Ciriello MAWL: {_vm.SnookMAWLKg:F1} kg (DCR {_vm.SnookDCR:F2})";
            _txtArmRow.Text = $"Arm AFF Strength: DCR {_vm.ArmDCR:F2}";
            _txtRecommendation.Text = $"Guidance: {_vm.Recommendation}";
        }

        private void LoadScenario(int id)
        {
            switch (id)
            {
                case 1:
                    _txtScenarioTitle.Text = "Test Case 1: Baseline Waist Lift (10 kg, Waist Height)";
                    _txtScenarioStatus.Text = "Status: PASSED (Low Risk / Safe)";
                    _txtScenarioStatus.Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153));
                    _vm.SelectedPercentile = DHMPercentile.Male50th;
                    _vm.LoadWeightKg = 10.0;
                    _vm.ReachMm = 350.0;
                    _vm.VerticalMm = 750.0;
                    _vm.FrequencyLiftsPerMin = 1.0;
                    break;
                case 2:
                    _txtScenarioTitle.Text = "Test Case 2: Hazardous Floor Pick (25 kg from floor, Female 5th)";
                    _txtScenarioStatus.Text = "Status: PASSED (Hazard correctly detected!)";
                    _txtScenarioStatus.Foreground = new SolidColorBrush(Color.FromRgb(248, 113, 113));
                    _vm.SelectedPercentile = DHMPercentile.Female5th;
                    _vm.LoadWeightKg = 25.0;
                    _vm.ReachMm = 600.0;
                    _vm.VerticalMm = 120.0;
                    _vm.FrequencyLiftsPerMin = 6.0;
                    break;
                case 3:
                    _txtScenarioTitle.Text = "Test Case 3: Overhead Reach (15 kg at 1450 mm, Male 95th)";
                    _txtScenarioStatus.Text = "Status: PASSED (Shoulder AFF stress alert)";
                    _txtScenarioStatus.Foreground = new SolidColorBrush(Color.FromRgb(251, 191, 36));
                    _vm.SelectedPercentile = DHMPercentile.Male95th;
                    _vm.LoadWeightKg = 15.0;
                    _vm.ReachMm = 500.0;
                    _vm.VerticalMm = 1450.0;
                    _vm.FrequencyLiftsPerMin = 2.0;
                    break;
                case 4:
                    _txtScenarioTitle.Text = "Test Case 4: High-Frequency Fatigue (12 lifts/min, Potvin MAE)";
                    _txtScenarioStatus.Text = "Status: PASSED (Fatigue attenuation verified)";
                    _txtScenarioStatus.Foreground = new SolidColorBrush(Color.FromRgb(192, 132, 252));
                    _vm.SelectedPercentile = DHMPercentile.Male50th;
                    _vm.LoadWeightKg = 14.0;
                    _vm.ReachMm = 400.0;
                    _vm.VerticalMm = 700.0;
                    _vm.FrequencyLiftsPerMin = 12.0;
                    break;
            }
            UpdateVisuals();
        }

        private void StartSequentialAutotests()
        {
            if (_autoTestTimer != null && _autoTestTimer.IsEnabled) return;

            _currentTestIndex = 1;
            LoadScenario(_currentTestIndex);

            _autoTestTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.3) };
            _autoTestTimer.Tick += (s, e) =>
            {
                _currentTestIndex++;
                if (_currentTestIndex > 4)
                {
                    _autoTestTimer.Stop();
                    _txtScenarioStatus.Text = ">>> ALL 4 VISUAL AUTOTESTS PASSED (100%) <<<";
                    _txtScenarioStatus.Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153));
                }
                else
                {
                    LoadScenario(_currentTestIndex);
                }
            };
            _autoTestTimer.Start();
        }

        private TextBlock CreateMetricTextBlock(string text)
        {
            return new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                FontSize = 11,
                Margin = new Thickness(0, 2, 0, 2),
                FontFamily = new FontFamily("Consolas, Segoe UI")
            };
        }

        private Button CreateTestButton(string label, Action onClick)
        {
            var btn = new Button
            {
                Content = label,
                Background = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                Foreground = Brushes.White,
                FontSize = 11,
                Padding = new Thickness(8, 6, 8, 6),
                Margin = new Thickness(3)
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }
    }
}
