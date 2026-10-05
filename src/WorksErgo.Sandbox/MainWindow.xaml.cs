using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WorksErgoRPro.Biomechanics;

namespace WorksErgo.Sandbox
{
    public partial class MainWindow : Window
    {
        private Point _lastMousePos;
        private bool _isOrbiting;
        private bool _isPanning;
        private double _cameraTheta = 1.0;
        private double _cameraPhi = 0.4;
        private double _cameraRadius = 2600.0;
        private Point3D _cameraTarget = new Point3D(200, 0, 850);

        private LiftingTechnique _technique = LiftingTechnique.AutomaticSemiSquat;
        private TaskType _currentTask = TaskType.LiftingLowering;
        private DHMPercentile _percentile = DHMPercentile.Male50th;

        private Model3DGroup _skeletonGroup = new Model3DGroup();
        private GeometryModel3D _yellowCylinderModel;
        private GeometryModel3D _blueCylinderModel;
        private GeometryModel3D _pinkCylinderModel;

        public MainWindow()
        {
            InitializeComponent();
            SceneGroup.Children.Add(_skeletonGroup);
            BuildStaticEnvironment();
            UpdateCamera();
            RecalculateAndRender();
        }

        private void BuildStaticEnvironment()
        {
            // Floor Grid
            var gridGroup = new Model3DGroup();
            var gridMat = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(80, 70, 80, 95)));
            for (int x = -1000; x <= 1500; x += 250)
            {
                gridGroup.Children.Add(CreateLine(new Point3D(x, -1000, 0), new Point3D(x, 1000, 0), 3, gridMat));
            }
            for (int y = -1000; y <= 1000; y += 250)
            {
                gridGroup.Children.Add(CreateLine(new Point3D(-1000, y, 0), new Point3D(1500, y, 0), 3, gridMat));
            }

            // Foot Base of Support (BoS: -70 to +180 mm)
            var bosMat = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(120, 152, 195, 121)));
            gridGroup.Children.Add(CreateBox(new Point3D(55, 120, 10), 250, 100, 20, bosMat));
            gridGroup.Children.Add(CreateBox(new Point3D(55, -120, 10), 250, 100, 20, bosMat));

            SceneGroup.Children.Add(gridGroup);
        }

        private void RecalculateAndRender()
        {
            if (SliderHandZ == null || SliderHandX == null || SliderLoadMass == null) return;

            double handZ = SliderHandZ.Value;
            double handX = SliderHandX.Value;
            double loadMass = SliderLoadMass.Value;

            TxtHandZ.Text = $"{(int)handZ} mm";
            TxtHandX.Text = $"{(int)handX} mm";
            TxtLoadMass.Text = $"{loadMass:F1} kg";

            // 1. Solve Kinematics based on InteliPose technique & hand targets
            // Ankle is at (0, ±120, 80).
            // Dempster-Winter closed kinematics: Pelvis moves back (T), Knee moves forward (K)
            double pelvisX, pelvisZ, kneeX, kneeZ;
            double shankL = 420.0;
            double thighL = 430.0;
            double trunkL = 500.0;

            switch (_technique)
            {
                case LiftingTechnique.StoopStraightLegs:
                    pelvisX = -50.0;
                    pelvisZ = 850.0;
                    kneeX = 40.0;
                    kneeZ = 450.0;
                    break;
                case LiftingTechnique.DeepSquat:
                    pelvisX = -120.0;
                    pelvisZ = 420.0;
                    kneeX = 180.0;
                    kneeZ = 380.0;
                    break;
                case LiftingTechnique.AutomaticSemiSquat:
                default:
                    pelvisX = -180.0;
                    pelvisZ = 680.0;
                    kneeX = 110.0;
                    kneeZ = 430.0;
                    break;
            }

            // Shoulder position derived from Pelvis & Hand reach
            Point3D pelvis = new Point3D(pelvisX, 0, pelvisZ);
            Point3D rAnkle = new Point3D(0, -120, 80);
            Point3D lAnkle = new Point3D(0, 120, 80);
            Point3D rKnee = new Point3D(kneeX, -120, kneeZ);
            Point3D lKnee = new Point3D(kneeX, 120, kneeZ);

            // Torso angles forward towards hands
            double dx = handX - pelvisX;
            double dz = handZ - pelvisZ;
            double trunkAngleRad = Math.Atan2(dx, -dz);
            if (trunkAngleRad < 0.1) trunkAngleRad = 0.1;
            if (trunkAngleRad > 1.4) trunkAngleRad = 1.4;

            Point3D neck = new Point3D(pelvisX + trunkL * Math.Sin(trunkAngleRad), 0, pelvisZ + trunkL * Math.Cos(trunkAngleRad));
            Point3D head = new Point3D(neck.X + 80, 0, neck.Z + 180);

            Point3D rShoulder = new Point3D(neck.X, -180, neck.Z - 30);
            Point3D lShoulder = new Point3D(neck.X, 180, neck.Z - 30);

            Point3D rHand = new Point3D(handX, -150, handZ);
            Point3D lHand = new Point3D(handX, 150, handZ);

            // 2-link analytical IK for elbows (Shoulder -> Elbow -> Hand)
            Point3D rElbow = SolveElbowIK(rShoulder, rHand, 310.0, 270.0, -1);
            Point3D lElbow = SolveElbowIK(lShoulder, lHand, 310.0, 270.0, 1);

            // 2. Evaluate Biomechanics
            var inputs = new PostureInputs
            {
                Percentile = _percentile,
                Technique = _technique,
                Task = _currentTask,
                Coupling = CouplingQuality.Fair,
                Grip = HandGripType.CylindricalPowerGrip,
                VerticalMm = handZ,
                LoadKg = loadMass,
                ReachMm = handX,
                FrequencyLiftsPerMin = 1.0,
                DurationHours = 2.0
            };

            var ergoOutputs = ErgonomicMathEngine.Evaluate(inputs);

            // Update UI Gauges
            TxtDcrValue.Text = ergoOutputs.OverallDCR.ToString("F2");
            string trafficLight = ergoOutputs.OverallDCR < 0.85 ? "GREEN" : (ergoOutputs.OverallDCR <= 1.0 ? "YELLOW" : "RED");
            TxtDcrCategory.Text = $"{trafficLight} [{(ergoOutputs.OverallDCR < 0.85 ? "ACCEPTABLE" : (ergoOutputs.OverallDCR <= 1.0 ? "ACTION RECOMMENDED" : "CRITICAL RISK"))}]";
            TxtL5S1.Text = $"{ergoOutputs.LumbarCompressionN:F0} N (Limit 3400 N)";
            TxtCoM.Text = $"{ergoOutputs.CenterOfPressureMm:+0.0;-0.0;0.0} mm ({(ergoOutputs.IsBalanced ? "STABLE" : "UNBALANCED")})";
            TxtRula.Text = $"EAWS: {ergoOutputs.EawsScore:F1} ({ergoOutputs.EawsTrafficLight})";
            TxtReba.Text = $"NIOSH LI: {ergoOutputs.NioshLI:F2} | Snook: {ergoOutputs.SnookDCR:F2}";

            // Traffic light color update
            Color statusColor;
            switch (trafficLight)
            {
                case "RED":
                    statusColor = Color.FromRgb(224, 108, 117);
                    break;
                case "YELLOW":
                    statusColor = Color.FromRgb(229, 192, 123);
                    break;
                case "GREEN":
                default:
                    statusColor = Color.FromRgb(152, 195, 121);
                    break;
            }
            DcrCard.BorderBrush = new SolidColorBrush(statusColor);
            TxtDcrValue.Foreground = new SolidColorBrush(statusColor);
            TxtDcrCategory.Foreground = new SolidColorBrush(statusColor);

            // 3. Render Skeleton into 3D Viewport
            _skeletonGroup.Children.Clear();

            var boneMat = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(171, 178, 191)));
            var jointMat = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(97, 175, 239)));
            var handMat = new DiffuseMaterial(new SolidColorBrush(statusColor));

            // Legs
            RenderLimb(rAnkle, rKnee, boneMat, jointMat);
            RenderLimb(rKnee, new Point3D(pelvisX, -100, pelvisZ), boneMat, jointMat);
            RenderLimb(lAnkle, lKnee, boneMat, jointMat);
            RenderLimb(lKnee, new Point3D(pelvisX, 100, pelvisZ), boneMat, jointMat);

            // Spine & Head
            RenderLimb(pelvis, neck, boneMat, jointMat);
            _skeletonGroup.Children.Add(CreateSphere(head, 90, jointMat));

            // Arms
            RenderLimb(rShoulder, rElbow, boneMat, jointMat);
            RenderLimb(rElbow, rHand, boneMat, jointMat);
            RenderLimb(lShoulder, lElbow, boneMat, jointMat);
            RenderLimb(lElbow, lHand, boneMat, jointMat);

            // Floating Hand Targets
            _skeletonGroup.Children.Add(CreateSphere(rHand, 45, handMat));
            _skeletonGroup.Children.Add(CreateSphere(lHand, 45, handMat));

            // Load Object between hands
            if (loadMass > 0.5)
            {
                var boxMat = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(180, 209, 154, 102)));
                _skeletonGroup.Children.Add(CreateBox(new Point3D(handX, 0, handZ), 260, 320, 220, boxMat));
            }

            // 4. Render 3D Work(s) Controller Cylinders
            // Yellow Cylinder: InteliPose switch
            var yellowMat = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(229, 192, 123)));
            _yellowCylinderModel = CreateCylinder(new Point3D(pelvisX, 0, pelvisZ + 140), 65, 80, yellowMat);
            _skeletonGroup.Children.Add(_yellowCylinderModel);

            // Blue Cylinder: Task Type switch
            var blueMat = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(97, 175, 239)));
            _blueCylinderModel = CreateCylinder(new Point3D(pelvisX, 0, pelvisZ + 260), 45, 120, blueMat);
            _skeletonGroup.Children.Add(_blueCylinderModel);

            // Pink Cylinder: Reset to Ready
            var pinkMat = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(224, 108, 117)));
            _pinkCylinderModel = CreateCylinder(new Point3D(pelvisX, 0, pelvisZ + 370), 35, 60, pinkMat);
            _skeletonGroup.Children.Add(_pinkCylinderModel);
        }

        private Point3D SolveElbowIK(Point3D shoulder, Point3D hand, double upperArmL, double forearmL, int outwardSign)
        {
            Vector3D d = hand - shoulder;
            double dist = d.Length;
            if (dist > upperArmL + forearmL - 10) dist = upperArmL + forearmL - 10;
            if (dist < Math.Abs(upperArmL - forearmL) + 10) dist = Math.Abs(upperArmL - forearmL) + 10;

            double alpha = Math.Acos((upperArmL * upperArmL + dist * dist - forearmL * forearmL) / (2.0 * upperArmL * dist));
            if (double.IsNaN(alpha)) alpha = 0.5;

            d.Normalize();
            Vector3D normal = Vector3D.CrossProduct(d, new Vector3D(0, outwardSign, 0));
            if (normal.LengthSquared < 0.001) normal = new Vector3D(0, outwardSign, 0);
            normal.Normalize();

            Point3D mid = shoulder + d * (upperArmL * Math.Cos(alpha));
            return mid + normal * (upperArmL * Math.Sin(alpha));
        }

        private void RenderLimb(Point3D p1, Point3D p2, Material boneMat, Material jointMat)
        {
            _skeletonGroup.Children.Add(CreateSphere(p1, 24, jointMat));
            _skeletonGroup.Children.Add(CreateSphere(p2, 24, jointMat));
            _skeletonGroup.Children.Add(CreateLine(p1, p2, 16, boneMat));
        }

        // --- 3D Geometry Utilities ---
        private GeometryModel3D CreateSphere(Point3D center, double radius, Material mat)
        {
            var mesh = new MeshGeometry3D();
            int slices = 12, stacks = 8;
            for (int i = 0; i <= stacks; i++)
            {
                double phi = Math.PI * i / stacks;
                for (int j = 0; j <= slices; j++)
                {
                    double theta = 2.0 * Math.PI * j / slices;
                    double x = center.X + radius * Math.Sin(phi) * Math.Cos(theta);
                    double y = center.Y + radius * Math.Sin(phi) * Math.Sin(theta);
                    double z = center.Z + radius * Math.Cos(phi);
                    mesh.Positions.Add(new Point3D(x, y, z));
                }
            }
            for (int i = 0; i < stacks; i++)
            {
                for (int j = 0; j < slices; j++)
                {
                    int p0 = i * (slices + 1) + j;
                    int p1 = p0 + 1;
                    int p2 = p0 + (slices + 1);
                    int p3 = p2 + 1;
                    mesh.TriangleIndices.Add(p0); mesh.TriangleIndices.Add(p2); mesh.TriangleIndices.Add(p1);
                    mesh.TriangleIndices.Add(p1); mesh.TriangleIndices.Add(p2); mesh.TriangleIndices.Add(p3);
                }
            }
            return new GeometryModel3D(mesh, mat);
        }

        private GeometryModel3D CreateLine(Point3D p1, Point3D p2, double width, Material mat)
        {
            return CreateBox(new Point3D((p1.X + p2.X) / 2.0, (p1.Y + p2.Y) / 2.0, (p1.Z + p2.Z) / 2.0),
                             Math.Max(Math.Abs(p2.X - p1.X), width),
                             Math.Max(Math.Abs(p2.Y - p1.Y), width),
                             Math.Max(Math.Abs(p2.Z - p1.Z), width),
                             mat);
        }

        private GeometryModel3D CreateBox(Point3D center, double sx, double sy, double sz, Material mat)
        {
            var mesh = new MeshGeometry3D();
            double hx = sx / 2.0, hy = sy / 2.0, hz = sz / 2.0;
            Point3D[] pts = {
                new Point3D(center.X - hx, center.Y - hy, center.Z - hz),
                new Point3D(center.X + hx, center.Y - hy, center.Z - hz),
                new Point3D(center.X + hx, center.Y + hy, center.Z - hz),
                new Point3D(center.X - hx, center.Y + hy, center.Z - hz),
                new Point3D(center.X - hx, center.Y - hy, center.Z + hz),
                new Point3D(center.X + hx, center.Y - hy, center.Z + hz),
                new Point3D(center.X + hx, center.Y + hy, center.Z + hz),
                new Point3D(center.X - hx, center.Y + hy, center.Z + hz),
            };
            foreach (var p in pts) mesh.Positions.Add(p);
            int[] indices = {
                0,2,1, 0,3,2, 4,5,6, 4,6,7, 0,1,5, 0,5,4, 1,2,6, 1,6,5, 2,3,7, 2,7,6, 3,0,4, 3,4,7
            };
            foreach (var idx in indices) mesh.TriangleIndices.Add(idx);
            return new GeometryModel3D(mesh, mat);
        }

        private GeometryModel3D CreateCylinder(Point3D center, double radius, double height, Material mat)
        {
            var mesh = new MeshGeometry3D();
            int segs = 16;
            for (int i = 0; i < segs; i++)
            {
                double a = 2.0 * Math.PI * i / segs;
                mesh.Positions.Add(new Point3D(center.X + radius * Math.Cos(a), center.Y + radius * Math.Sin(a), center.Z - height / 2.0));
                mesh.Positions.Add(new Point3D(center.X + radius * Math.Cos(a), center.Y + radius * Math.Sin(a), center.Z + height / 2.0));
            }
            for (int i = 0; i < segs; i++)
            {
                int next = (i + 1) % segs;
                int b0 = i * 2, t0 = b0 + 1;
                int b1 = next * 2, t1 = b1 + 1;
                mesh.TriangleIndices.Add(b0); mesh.TriangleIndices.Add(t0); mesh.TriangleIndices.Add(b1);
                mesh.TriangleIndices.Add(b1); mesh.TriangleIndices.Add(t0); mesh.TriangleIndices.Add(t1);
            }
            return new GeometryModel3D(mesh, mat);
        }

        // --- Camera Orbit & Pan ---
        private void UpdateCamera()
        {
            double x = _cameraTarget.X + _cameraRadius * Math.Cos(_cameraPhi) * Math.Cos(_cameraTheta);
            double y = _cameraTarget.Y + _cameraRadius * Math.Cos(_cameraPhi) * Math.Sin(_cameraTheta);
            double z = _cameraTarget.Z + _cameraRadius * Math.Sin(_cameraPhi);

            MainCamera.Position = new Point3D(x, y, z);
            MainCamera.LookDirection = new Vector3D(_cameraTarget.X - x, _cameraTarget.Y - y, _cameraTarget.Z - z);
        }

        private void Viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _lastMousePos = e.GetPosition(MainViewport);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Perform 3D CAD Raycast Hit Test on Yellow, Blue, Pink Cylinders
                Point hitPt = e.GetPosition(MainViewport);
                var hitParams = new PointHitTestParameters(hitPt);
                VisualTreeHelper.HitTest(MainViewport, null, HitTestCallback, hitParams);

                _isOrbiting = true;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                _isPanning = true;
            }
        }

        private HitTestResultBehavior HitTestCallback(HitTestResult result)
        {
            if (result is RayMeshGeometry3DHitTestResult meshHit)
            {
                if (meshHit.ModelHit == _yellowCylinderModel)
                {
                    CycleInteliPose();
                    return HitTestResultBehavior.Stop;
                }
                if (meshHit.ModelHit == _blueCylinderModel)
                {
                    CycleTaskType();
                    return HitTestResultBehavior.Stop;
                }
                if (meshHit.ModelHit == _pinkCylinderModel)
                {
                    BtnResetPose_Click(null, null);
                    return HitTestResultBehavior.Stop;
                }
            }
            return HitTestResultBehavior.Continue;
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MainViewport);
            double dx = pos.X - _lastMousePos.X;
            double dy = pos.Y - _lastMousePos.Y;
            _lastMousePos = pos;

            if (_isOrbiting)
            {
                _cameraTheta -= dx * 0.01;
                _cameraPhi += dy * 0.01;
                if (_cameraPhi > 1.4) _cameraPhi = 1.4;
                if (_cameraPhi < -0.2) _cameraPhi = -0.2;
                UpdateCamera();
            }
            else if (_isPanning)
            {
                _cameraTarget.Y -= dx * 1.5;
                _cameraTarget.Z += dy * 1.5;
                UpdateCamera();
            }
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isOrbiting = false;
            _isPanning = false;
        }

        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _cameraRadius -= e.Delta * 1.2;
            if (_cameraRadius < 600) _cameraRadius = 600;
            if (_cameraRadius > 5000) _cameraRadius = 5000;
            UpdateCamera();
        }

        // --- Event Handlers & Controllers ---
        private void CycleInteliPose()
        {
            switch (_technique)
            {
                case LiftingTechnique.StoopStraightLegs:
                    _technique = LiftingTechnique.AutomaticSemiSquat;
                    break;
                case LiftingTechnique.AutomaticSemiSquat:
                    _technique = LiftingTechnique.DeepSquat;
                    break;
                case LiftingTechnique.DeepSquat:
                default:
                    _technique = LiftingTechnique.StoopStraightLegs;
                    break;
            }
            RecalculateAndRender();
        }

        private void CycleTaskType()
        {
            switch (_currentTask)
            {
                case TaskType.LiftingLowering:
                    _currentTask = TaskType.PushingPulling;
                    break;
                case TaskType.PushingPulling:
                    _currentTask = TaskType.Carrying;
                    break;
                case TaskType.Carrying:
                default:
                    _currentTask = TaskType.LiftingLowering;
                    break;
            }
            if (ComboTaskType != null) ComboTaskType.SelectedIndex = (int)_currentTask;
            RecalculateAndRender();
        }

        private void BtnStoop_Click(object sender, RoutedEventArgs e) { _technique = LiftingTechnique.StoopStraightLegs; RecalculateAndRender(); }
        private void BtnSemiSquat_Click(object sender, RoutedEventArgs e) { _technique = LiftingTechnique.AutomaticSemiSquat; RecalculateAndRender(); }
        private void BtnDeepSquat_Click(object sender, RoutedEventArgs e) { _technique = LiftingTechnique.DeepSquat; RecalculateAndRender(); }
        private void BtnResetPose_Click(object sender, RoutedEventArgs e)
        {
            _technique = LiftingTechnique.AutomaticSemiSquat;
            SliderHandZ.Value = 750;
            SliderHandX.Value = 450;
            SliderLoadMass.Value = 10;
            RecalculateAndRender();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => RecalculateAndRender();
        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboPercentile != null) _percentile = (DHMPercentile)ComboPercentile.SelectedIndex;
            if (ComboTaskType != null) _currentTask = (TaskType)ComboTaskType.SelectedIndex;
            RecalculateAndRender();
        }
        private void ChkAttachHands_Changed(object sender, RoutedEventArgs e) => RecalculateAndRender();
    }
}
