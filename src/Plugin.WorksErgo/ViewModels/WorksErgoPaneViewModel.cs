using System;
using System.Windows.Media;
using Caliburn.Micro;
using RProSoftDigital1.UX.Shared;
using WorksErgoRPro.Biomechanics;

namespace WorksErgoRPro.ViewModels
{
    public class WorksErgoPaneViewModel : DockableScreen
    {
        private DHMPercentile _selectedPercentile = DHMPercentile.Male50th;
        private TaskType _selectedTask = TaskType.LiftingLowering;
        private double _loadWeightKg = 12.0;
        private double _reachMm = 400.0;
        private double _verticalMm = 750.0;
        private double _travelMm = 350.0;
        private double _asymmetryDeg = 0.0;
        private double _frequencyLiftsPerMin = 2.0;
        private double _durationHours = 2.0;
        private CouplingQuality _selectedCoupling = CouplingQuality.Good;

        // Evaluated Results
        private double _overallDcr = 0.0;
        private string _riskCategory = "Safe";
        private SolidColorBrush _riskBrush = new SolidColorBrush(Color.FromRgb(46, 204, 113));
        private double _lumbarCompN = 0.0;
        private double _lumbarDcr = 0.0;
        private double _nioshRwlKg = 0.0;
        private double _nioshLi = 0.0;
        private double _snookMawlKg = 0.0;
        private double _snookDcr = 0.0;
        private double _armDcr = 0.0;
        private int _rulaScore = 1;
        private int _rebaScore = 1;
        private string _limitingFactor = "None";
        private string _recommendation = "All parameters safe.";

        public WorksErgoPaneViewModel()
        {
            DisplayName = "Works Ergo";
            PanelId = "WorksErgoPane";
            DesiredPanePosition = (int)DesiredPaneLocation.DockedRight;
            Width = 440;
            Height = 800;
            IsPinned = true;
            IsVisible = true;

            Recalculate();
        }

        #region Properties

        public DHMPercentile SelectedPercentile
        {
            get => _selectedPercentile;
            set { if (_selectedPercentile != value) { _selectedPercentile = value; NotifyOfPropertyChange(nameof(SelectedPercentile)); Recalculate(); } }
        }

        public TaskType SelectedTask
        {
            get => _selectedTask;
            set { if (_selectedTask != value) { _selectedTask = value; NotifyOfPropertyChange(nameof(SelectedTask)); Recalculate(); } }
        }

        public double LoadWeightKg
        {
            get => _loadWeightKg;
            set { if (Math.Abs(_loadWeightKg - value) > 0.01) { _loadWeightKg = Math.Max(0.5, Math.Min(60.0, value)); NotifyOfPropertyChange(nameof(LoadWeightKg)); Recalculate(); } }
        }

        public double ReachMm
        {
            get => _reachMm;
            set { if (Math.Abs(_reachMm - value) > 0.1) { _reachMm = Math.Max(200.0, Math.Min(900.0, value)); NotifyOfPropertyChange(nameof(ReachMm)); Recalculate(); } }
        }

        public double VerticalMm
        {
            get => _verticalMm;
            set { if (Math.Abs(_verticalMm - value) > 0.1) { _verticalMm = Math.Max(50.0, Math.Min(1800.0, value)); NotifyOfPropertyChange(nameof(VerticalMm)); Recalculate(); } }
        }

        public double FrequencyLiftsPerMin
        {
            get => _frequencyLiftsPerMin;
            set { if (Math.Abs(_frequencyLiftsPerMin - value) > 0.01) { _frequencyLiftsPerMin = Math.Max(0.1, Math.Min(15.0, value)); NotifyOfPropertyChange(nameof(FrequencyLiftsPerMin)); Recalculate(); } }
        }

        public double OverallDCR
        {
            get => _overallDcr;
            private set { _overallDcr = value; NotifyOfPropertyChange(nameof(OverallDCR)); NotifyOfPropertyChange(nameof(OverallDcrText)); }
        }

        public string OverallDcrText => $"{Math.Round(_overallDcr * 100.0, 0)}%";

        public string RiskCategory
        {
            get => _riskCategory;
            private set { _riskCategory = value; NotifyOfPropertyChange(nameof(RiskCategory)); }
        }

        public SolidColorBrush RiskBrush
        {
            get => _riskBrush;
            private set { _riskBrush = value; NotifyOfPropertyChange(nameof(RiskBrush)); }
        }

        public double LumbarCompN
        {
            get => _lumbarCompN;
            private set { _lumbarCompN = value; NotifyOfPropertyChange(nameof(LumbarCompN)); }
        }

        public double LumbarDCR
        {
            get => _lumbarDcr;
            private set { _lumbarDcr = value; NotifyOfPropertyChange(nameof(LumbarDCR)); }
        }

        public double NioshRWLKg
        {
            get => _nioshRwlKg;
            private set { _nioshRwlKg = value; NotifyOfPropertyChange(nameof(NioshRWLKg)); }
        }

        public double NioshLI
        {
            get => _nioshLi;
            private set { _nioshLi = value; NotifyOfPropertyChange(nameof(NioshLI)); }
        }

        public double SnookMAWLKg
        {
            get => _snookMawlKg;
            private set { _snookMawlKg = value; NotifyOfPropertyChange(nameof(SnookMAWLKg)); }
        }

        public double SnookDCR
        {
            get => _snookDcr;
            private set { _snookDcr = value; NotifyOfPropertyChange(nameof(SnookDCR)); }
        }

        public double ArmDCR
        {
            get => _armDcr;
            private set { _armDcr = value; NotifyOfPropertyChange(nameof(ArmDCR)); }
        }

        public int RulaScore
        {
            get => _rulaScore;
            private set { _rulaScore = value; NotifyOfPropertyChange(nameof(RulaScore)); }
        }

        public int RebaScore
        {
            get => _rebaScore;
            private set { _rebaScore = value; NotifyOfPropertyChange(nameof(RebaScore)); }
        }

        public string LimitingFactor
        {
            get => _limitingFactor;
            private set { _limitingFactor = value; NotifyOfPropertyChange(nameof(LimitingFactor)); }
        }

        public string Recommendation
        {
            get => _recommendation;
            private set { _recommendation = value; NotifyOfPropertyChange(nameof(Recommendation)); }
        }

        #endregion

        public void Recalculate()
        {
            var inputs = new PostureInputs
            {
                Percentile = _selectedPercentile,
                Task = _selectedTask,
                LoadKg = _loadWeightKg,
                ReachMm = _reachMm,
                VerticalMm = _verticalMm,
                TravelMm = _travelMm,
                AsymmetryDeg = _asymmetryDeg,
                FrequencyLiftsPerMin = _frequencyLiftsPerMin,
                DurationHours = _durationHours,
                Coupling = _selectedCoupling
            };

            var res = ErgonomicMathEngine.Evaluate(inputs);

            OverallDCR = res.OverallDCR;
            RiskCategory = res.RiskCategory;
            LumbarCompN = res.LumbarCompressionN;
            LumbarDCR = res.LumbarDCR;
            NioshRWLKg = res.NioshRWLKg;
            NioshLI = res.NioshLI;
            SnookMAWLKg = res.SnookMAWLKg;
            SnookDCR = res.SnookDCR;
            ArmDCR = res.ArmDCR;
            RulaScore = res.RulaScore;
            RebaScore = res.RebaScore;
            LimitingFactor = res.PrimaryLimitingFactor;
            Recommendation = res.Recommendation;

            if (res.OverallDCR <= 0.85)
            {
                RiskBrush = new SolidColorBrush(Color.FromRgb(46, 204, 113)); // Green
            }
            else if (res.OverallDCR <= 1.00)
            {
                RiskBrush = new SolidColorBrush(Color.FromRgb(243, 156, 18)); // Orange
            }
            else
            {
                RiskBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));  // Red
            }
        }
    }
}
