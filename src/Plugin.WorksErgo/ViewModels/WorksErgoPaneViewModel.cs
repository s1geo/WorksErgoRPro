using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Media;
using MediaColor = System.Windows.Media.Color;
using Caliburn.Micro;
using RProSoftDigital1.Create3D;
using RProSoftDigital1.UX.Shared;
using WorksErgoRPro.Biomechanics;

namespace WorksErgoRPro.ViewModels
{
    public class SubtaskDisplayItem : PropertyChangedBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TaskType Task { get; set; }
        public double LoadKg { get; set; }
        public double VerticalMm { get; set; }
        public double ReachMm { get; set; }
        public double CyclesPerDay { get; set; }
        public double PeakCompressionN { get; set; }
        public double LCFCD { get; set; }
        public double TaskDCR { get; set; }
        public double EawsScore { get; set; }
        public string RiskCategory { get; set; }
        public SolidColorBrush RiskBrush { get; set; }
        public PostureInputs OriginalInputs { get; set; }
    }

    [Export(typeof(IDockableScreen))]
    [Export(typeof(WorksErgoPaneViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class WorksErgoPaneViewModel : DockableScreen, ICustomScreenVisibility
    {
        private readonly IApplication _application;
        private readonly IMessageService _messageService;
        private PickAction _pickAction;

        private DHMPercentile _selectedPercentile = DHMPercentile.Male50th;
        private TaskType _selectedTask = TaskType.LiftingLowering;
        private LiftingTechnique _selectedTechnique = LiftingTechnique.AutomaticSemiSquat;
        private HandGripType _selectedGrip = HandGripType.PowerGripMedial;
        private CouplingQuality _selectedCoupling = CouplingQuality.Good;

        private double _loadWeightKg = 12.0;
        private double _reachMm = 400.0;
        private double _verticalMm = 750.0;
        private double _travelMm = 350.0;
        private double _asymmetryDeg = 0.0;
        private double _lateralTiltDeg = 0.0;
        private double _dynamicAccelerationMs2 = 0.0;
        private double _frequencyLiftsPerMin = 2.0;
        private double _frequencyPerDay = 300.0;
        private double _durationHours = 2.0;

        private bool _attachHands = true;
        private bool _bodyBracing = false;
        private bool _straightLegs = false;

        // Evaluated Results
        private double _overallDcr = 0.0;
        private string _riskCategory = "Safe";
        private SolidColorBrush _riskBrush = new SolidColorBrush(MediaColor.FromRgb(46, 204, 113));
        private double _lumbarCompN = 0.0;
        private double _lumbarDcr = 0.0;
        private double _nioshRwlKg = 0.0;
        private double _nioshLi = 0.0;
        private double _kneeFlexionDeg = 0.0;
        private double _trunkFlexionDeg = 0.0;
        private double _hipHeightMm = 0.0;
        private double _kneeMomentNm = 0.0;
        private double _centerOfPressureMm = 0.0;
        private bool _isBalanced = true;
        private double _snookMawlKg = 0.0;
        private double _snookDcr = 0.0;
        private double _armDcr = 0.0;
        private double _handDcr = 0.0;
        private int _rulaScore = 1;
        private int _rebaScore = 1;
        private double _eawsScore = 0.0;
        private string _eawsTrafficLight = "Green (Low Risk)";
        private SolidColorBrush _eawsBrush = new SolidColorBrush(MediaColor.FromRgb(46, 204, 113));
        private double _eawsSec1 = 0.0;
        private double _eawsSec2 = 0.0;
        private double _eawsSec3 = 0.0;
        private double _eawsSec4 = 0.0;
        private string _limitingFactor = "None";
        private string _recommendation = "All parameters safe.";
        private string _cadStatusMessage = "3D CAD Snapping: Готов к привязке из 3D-сцены.";

        // Multi-Subtask Job Properties
        private BindableCollection<SubtaskDisplayItem> _subtasks = new BindableCollection<SubtaskDisplayItem>();
        private double _compositeLcfcd = 0.0;
        private double _compositeOverallDcr = 0.0;
        private double _compositePeakCompN = 0.0;
        private double _compositeDutyCycle = 0.0;
        private double _compositeEawsScore = 0.0;
        private string _compositeRiskCategory = "Not evaluated";
        private SolidColorBrush _compositeRiskBrush = new SolidColorBrush(MediaColor.FromRgb(127, 140, 141));
        private bool _isJobEvaluated = false;

        // Operator Management
        public BindableCollection<ISimComponent> Operators { get; } = new BindableCollection<ISimComponent>();
        private ISimComponent _selectedOperator;

        public bool IsHideable => false;

        public WorksErgoPaneViewModel()
        {
            DisplayName = "Work(s) Ergo";
            PanelId = "WorksErgoPaneId";
            DesiredPanePosition = (int)DesiredPaneLocation.DockedRight;
            PaneLocation = DesiredPaneLocation.DockedRight;
            TabGroupId = "Vc_R_TabGroup";
            Width = 460;
            Height = 850;
            IsPinned = true;
            IsVisible = true;

            try
            {
                _application = IoC.Get<IApplication>();
                _messageService = IoC.Get<IMessageService>();
            }
            catch { }

            RefreshOperators();
            Recalculate();
        }

        #region Operator Management & 3D Interaction

        public ISimComponent SelectedOperator
        {
            get => _selectedOperator;
            set
            {
                if (_selectedOperator != value)
                {
                    _selectedOperator = value;
                    NotifyOfPropertyChange(nameof(SelectedOperator));
                    if (_selectedOperator != null)
                    {
                        _messageService?.AppendMessage($"[Work(s) Ergo] Выбран активный оператор: {_selectedOperator.Name}", MessageLevel.Info);
                    }
                }
            }
        }

        public void RefreshOperators()
        {
            try
            {
                Operators.Clear();
                if (_application?.World?.Components != null)
                {
                    foreach (var comp in _application.World.Components)
                    {
                        Operators.Add(comp);
                    }
                }

                if (SelectedOperator == null && Operators.Count > 0)
                {
                    SelectedOperator = Operators.FirstOrDefault(c =>
                        c.Name.IndexOf("DHM", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        c.Name.IndexOf("Worker", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        c.Name.IndexOf("Human", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        c.Name.IndexOf("Оператор", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        c.Name.IndexOf("Operator", StringComparison.OrdinalIgnoreCase) >= 0)
                        ?? Operators[0];
                }
            }
            catch (Exception ex)
            {
                _messageService?.AppendMessage("[Work(s) Ergo] Ошибка сканирования компонентов: " + ex.Message, MessageLevel.Warning);
            }
        }

        public void PickFrom3DWorld_Click()
        {
            try
            {
                if (_pickAction == null)
                {
                    _pickAction = new PickAction
                    {
                        Filter = SelectionTypes.Component,
                        HighlightResult = true
                    };
                    _pickAction.StartAction((sender, e) =>
                    {
                        try
                        {
                            if (_pickAction != null && _pickAction.PickResult.PickedObject != null)
                            {
                                var picked = _pickAction.PickResult.PickedObject;
                                var simComp = picked.GetValue("Component") as ISimComponent;
                                if (simComp != null)
                                {
                                    if (!Operators.Contains(simComp))
                                    {
                                        Operators.Add(simComp);
                                    }
                                    SelectedOperator = simComp;
                                    _messageService?.AppendMessage($"[Work(s) Ergo] Из 3D-сцены выбран: {simComp.Name}", MessageLevel.Info);
                                }
                            }
                        }
                        catch (Exception pickErr)
                        {
                            _messageService?.AppendMessage("[Work(s) Ergo] Ошибка обработки 3D-выбора: " + pickErr.Message, MessageLevel.Warning);
                        }
                        finally
                        {
                            _pickAction = null;
                        }
                    });
                    _messageService?.AppendMessage("[Work(s) Ergo] Режим 3D-выбора активирован: кликните по объекту в сцене.", MessageLevel.Info);
                }
            }
            catch (Exception ex)
            {
                _messageService?.AppendMessage("[Work(s) Ergo] Не удалось запустить 3D Pick: " + ex.Message, MessageLevel.Warning);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            RefreshOperators();
            if (_application?.World != null)
            {
                _application.World.ComponentAdded += OnWorldComponentAdded;
                _application.World.ComponentRemoving += OnWorldComponentRemoving;
            }
        }

        protected override void OnDeactivate(bool close)
        {
            if (_application?.World != null)
            {
                _application.World.ComponentAdded -= OnWorldComponentAdded;
                _application.World.ComponentRemoving -= OnWorldComponentRemoving;
            }
            base.OnDeactivate(close);
        }

        private void OnWorldComponentAdded(object sender, ComponentAddedEventArgs e)
        {
            if (e.Component != null && !Operators.Contains(e.Component))
            {
                Operators.Add(e.Component);
                if (SelectedOperator == null) SelectedOperator = e.Component;
            }
        }

        private void OnWorldComponentRemoving(object sender, ComponentRemovingEventArgs e)
        {
            if (e.Component != null)
            {
                Operators.Remove(e.Component);
                if (SelectedOperator == e.Component)
                {
                    SelectedOperator = Operators.FirstOrDefault();
                }
            }
        }

        #endregion

        #region Properties - Inputs

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

        public LiftingTechnique SelectedTechnique
        {
            get => _selectedTechnique;
            set { if (_selectedTechnique != value) { _selectedTechnique = value; NotifyOfPropertyChange(nameof(SelectedTechnique)); Recalculate(); } }
        }

        public HandGripType SelectedGrip
        {
            get => _selectedGrip;
            set { if (_selectedGrip != value) { _selectedGrip = value; NotifyOfPropertyChange(nameof(SelectedGrip)); Recalculate(); } }
        }

        public CouplingQuality SelectedCoupling
        {
            get => _selectedCoupling;
            set { if (_selectedCoupling != value) { _selectedCoupling = value; NotifyOfPropertyChange(nameof(SelectedCoupling)); Recalculate(); } }
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

        public double AsymmetryDeg
        {
            get => _asymmetryDeg;
            set { if (Math.Abs(_asymmetryDeg - value) > 0.1) { _asymmetryDeg = Math.Max(0.0, Math.Min(90.0, value)); NotifyOfPropertyChange(nameof(AsymmetryDeg)); Recalculate(); } }
        }

        public double LateralTiltDeg
        {
            get => _lateralTiltDeg;
            set { if (Math.Abs(_lateralTiltDeg - value) > 0.1) { _lateralTiltDeg = Math.Max(0.0, Math.Min(45.0, value)); NotifyOfPropertyChange(nameof(LateralTiltDeg)); Recalculate(); } }
        }

        public double DynamicAccelerationMs2
        {
            get => _dynamicAccelerationMs2;
            set { if (Math.Abs(_dynamicAccelerationMs2 - value) > 0.05) { _dynamicAccelerationMs2 = Math.Max(0.0, Math.Min(3.0, value)); NotifyOfPropertyChange(nameof(DynamicAccelerationMs2)); Recalculate(); } }
        }

        public double FrequencyLiftsPerMin
        {
            get => _frequencyLiftsPerMin;
            set { if (Math.Abs(_frequencyLiftsPerMin - value) > 0.05) { _frequencyLiftsPerMin = Math.Max(0.1, Math.Min(15.0, value)); NotifyOfPropertyChange(nameof(FrequencyLiftsPerMin)); Recalculate(); } }
        }

        public double FrequencyPerDay
        {
            get => _frequencyPerDay;
            set { if (Math.Abs(_frequencyPerDay - value) > 1.0) { _frequencyPerDay = Math.Max(1.0, Math.Min(2000.0, value)); NotifyOfPropertyChange(nameof(FrequencyPerDay)); Recalculate(); } }
        }

        public bool AttachHands
        {
            get => _attachHands;
            set { if (_attachHands != value) { _attachHands = value; NotifyOfPropertyChange(nameof(AttachHands)); } }
        }

        public bool BodyBracing
        {
            get => _bodyBracing;
            set { if (_bodyBracing != value) { _bodyBracing = value; NotifyOfPropertyChange(nameof(BodyBracing)); Recalculate(); } }
        }

        public bool StraightLegs
        {
            get => _straightLegs;
            set
            {
                if (_straightLegs != value)
                {
                    _straightLegs = value;
                    NotifyOfPropertyChange(nameof(StraightLegs));
                    if (_straightLegs)
                    {
                        SelectedTechnique = LiftingTechnique.StoopStraightLegs;
                    }
                    else
                    {
                        SelectedTechnique = LiftingTechnique.AutomaticSemiSquat;
                    }
                }
            }
        }

        public string CadStatusMessage
        {
            get => _cadStatusMessage;
            private set { _cadStatusMessage = value; NotifyOfPropertyChange(nameof(CadStatusMessage)); }
        }

        #endregion

        #region Properties - Evaluated Outputs

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

        public double KneeFlexionDeg
        {
            get => _kneeFlexionDeg;
            private set { _kneeFlexionDeg = value; NotifyOfPropertyChange(nameof(KneeFlexionDeg)); }
        }

        public double TrunkFlexionDeg
        {
            get => _trunkFlexionDeg;
            private set { _trunkFlexionDeg = value; NotifyOfPropertyChange(nameof(TrunkFlexionDeg)); }
        }

        public double HipHeightMm
        {
            get => _hipHeightMm;
            private set { _hipHeightMm = value; NotifyOfPropertyChange(nameof(HipHeightMm)); }
        }

        public double KneeMomentNm
        {
            get => _kneeMomentNm;
            private set { _kneeMomentNm = value; NotifyOfPropertyChange(nameof(KneeMomentNm)); }
        }

        public double CenterOfPressureMm
        {
            get => _centerOfPressureMm;
            private set { _centerOfPressureMm = value; NotifyOfPropertyChange(nameof(CenterOfPressureMm)); }
        }

        public bool IsBalanced
        {
            get => _isBalanced;
            private set { _isBalanced = value; NotifyOfPropertyChange(nameof(IsBalanced)); }
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

        public double HandDCR
        {
            get => _handDcr;
            private set { _handDcr = value; NotifyOfPropertyChange(nameof(HandDCR)); }
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

        public double EawsScore
        {
            get => _eawsScore;
            private set { _eawsScore = value; NotifyOfPropertyChange(nameof(EawsScore)); }
        }

        public string EawsTrafficLight
        {
            get => _eawsTrafficLight;
            private set { _eawsTrafficLight = value; NotifyOfPropertyChange(nameof(EawsTrafficLight)); }
        }

        public SolidColorBrush EawsBrush
        {
            get => _eawsBrush;
            private set { _eawsBrush = value; NotifyOfPropertyChange(nameof(EawsBrush)); }
        }

        public double EawsSec1
        {
            get => _eawsSec1;
            private set { _eawsSec1 = value; NotifyOfPropertyChange(nameof(EawsSec1)); }
        }

        public double EawsSec2
        {
            get => _eawsSec2;
            private set { _eawsSec2 = value; NotifyOfPropertyChange(nameof(EawsSec2)); }
        }

        public double EawsSec3
        {
            get => _eawsSec3;
            private set { _eawsSec3 = value; NotifyOfPropertyChange(nameof(EawsSec3)); }
        }

        public double EawsSec4
        {
            get => _eawsSec4;
            private set { _eawsSec4 = value; NotifyOfPropertyChange(nameof(EawsSec4)); }
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

        #region Properties - Multi-Subtask Job Manager

        public BindableCollection<SubtaskDisplayItem> Subtasks => _subtasks;

        public double CompositeLcfcd
        {
            get => _compositeLcfcd;
            private set { _compositeLcfcd = value; NotifyOfPropertyChange(nameof(CompositeLcfcd)); }
        }

        public double CompositeOverallDCR
        {
            get => _compositeOverallDcr;
            private set { _compositeOverallDcr = value; NotifyOfPropertyChange(nameof(CompositeOverallDCR)); NotifyOfPropertyChange(nameof(CompositeOverallDcrText)); }
        }

        public string CompositeOverallDcrText => $"{Math.Round(_compositeOverallDcr * 100.0, 0)}%";

        public double CompositePeakCompN
        {
            get => _compositePeakCompN;
            private set { _compositePeakCompN = value; NotifyOfPropertyChange(nameof(CompositePeakCompN)); }
        }

        public double CompositeDutyCycle
        {
            get => _compositeDutyCycle;
            private set { _compositeDutyCycle = value; NotifyOfPropertyChange(nameof(CompositeDutyCycle)); }
        }

        public double CompositeEawsScore
        {
            get => _compositeEawsScore;
            private set { _compositeEawsScore = value; NotifyOfPropertyChange(nameof(CompositeEawsScore)); }
        }

        public string CompositeRiskCategory
        {
            get => _compositeRiskCategory;
            private set { _compositeRiskCategory = value; NotifyOfPropertyChange(nameof(CompositeRiskCategory)); }
        }

        public SolidColorBrush CompositeRiskBrush
        {
            get => _compositeRiskBrush;
            private set { _compositeRiskBrush = value; NotifyOfPropertyChange(nameof(CompositeRiskBrush)); }
        }

        public bool IsJobEvaluated
        {
            get => _isJobEvaluated;
            private set { _isJobEvaluated = value; NotifyOfPropertyChange(nameof(IsJobEvaluated)); }
        }

        #endregion

        #region Actions & Commands

        public void SnapSelected3DObject()
        {
            try
            {
                bool snapped = false;
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.FullName != null && asm.FullName.StartsWith("UX.Shared"))
                    {
                        var contextType = asm.GetType("RProSoftDigital1.UX.Shared.IUXContext");
                        if (contextType != null)
                        {
                            var method = typeof(IoC).GetMethod("Get", Type.EmptyTypes)?.MakeGenericMethod(contextType);
                            var uxCtx = method?.Invoke(null, null);
                            if (uxCtx != null)
                            {
                                var selObjProp = uxCtx.GetType().GetProperty("SelectedObject");
                                var selObj = selObjProp?.GetValue(uxCtx);
                                if (selObj != null)
                                {
                                    var boundBoxProp = selObj.GetType().GetProperty("BoundingBox");
                                    var matrixProp = selObj.GetType().GetProperty("Transformation");
                                    var nameProp = selObj.GetType().GetProperty("Name");
                                    string objName = nameProp?.GetValue(selObj)?.ToString() ?? "Selected CAD Part";

                                    double centerZ = 750.0;
                                    double reach = 400.0;
                                    double mass = 12.0;

                                    if (matrixProp != null)
                                    {
                                        var mat = matrixProp.GetValue(selObj);
                                        var pz = mat?.GetType().GetProperty("Z")?.GetValue(mat);
                                        var px = mat?.GetType().GetProperty("X")?.GetValue(mat);
                                        var py = mat?.GetType().GetProperty("Y")?.GetValue(mat);
                                        if (pz is double dz) centerZ = Math.Max(100.0, Math.Min(1700.0, dz));
                                        if (px is double dx && py is double dy)
                                        {
                                            reach = Math.Max(200.0, Math.Min(850.0, Math.Sqrt(dx * dx + dy * dy)));
                                        }
                                    }

                                    var propCont = selObj as dynamic;
                                    try
                                    {
                                        var massProp = propCont?.GetProperty("Mass") ?? propCont?.GetProperty("Weight");
                                        if (massProp != null)
                                        {
                                            object vObj = massProp.Value;
                                            if (vObj is double && (double)vObj > 0.0)
                                            {
                                                mass = Math.Min(50.0, (double)vObj);
                                            }
                                        }
                                    }
                                    catch { }

                                    VerticalMm = centerZ;
                                    ReachMm = reach;
                                    LoadWeightKg = mass;

                                    CadStatusMessage = $"3D Snap: '{objName}' (Z={centerZ:F0} мм, Reach={reach:F0} мм, Load={mass:F1} кг, -X normal)";
                                    _messageService?.AppendMessage($"[Work(s) Ergo] 3D Привязка нормали ладони (-X) к объекту '{objName}': Z={centerZ:F0} мм, Reach={reach:F0} мм, Груз={mass:F1} кг", MessageLevel.Info);
                                    snapped = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!snapped)
                {
                    CadStatusMessage = "3D Snap: Выбран тестовый CAD-узел (Z=800 мм, Reach=420 мм, Load=15.0 кг, -X normal)";
                    VerticalMm = 800.0;
                    ReachMm = 420.0;
                    LoadWeightKg = 15.0;
                    _messageService?.AppendMessage("[Work(s) Ergo] Привязка: объект по умолчанию (Z=800 мм, Reach=420 мм, Load=15 кг)", MessageLevel.Info);
                }
            }
            catch (Exception ex)
            {
                CadStatusMessage = "3D Snap notice: " + ex.Message;
                _messageService?.AppendMessage("[Work(s) Ergo] Предупреждение 3D Snap: " + ex.Message, MessageLevel.Warning);
            }
            Recalculate();
        }

        public void AddCurrentAsSubtask()
        {
            var curInputs = new PostureInputs
            {
                Percentile = _selectedPercentile,
                Task = _selectedTask,
                Technique = _selectedTechnique,
                Grip = _selectedGrip,
                Coupling = _selectedCoupling,
                LoadKg = _loadWeightKg,
                ReachMm = _reachMm,
                VerticalMm = _verticalMm,
                TravelMm = _travelMm,
                AsymmetryDeg = _asymmetryDeg,
                LateralTiltDeg = _lateralTiltDeg,
                AccelerationMs2 = _dynamicAccelerationMs2,
                FrequencyLiftsPerMin = _frequencyLiftsPerMin,
                FrequencyPerDay = _frequencyPerDay,
                DurationHours = _durationHours
            };

            var res = ErgonomicMathEngine.Evaluate(curInputs);
            int nextId = _subtasks.Count + 1;

            var brush = res.OverallDCR <= 0.85 ? new SolidColorBrush(MediaColor.FromRgb(46, 204, 113))
                      : res.OverallDCR <= 1.00 ? new SolidColorBrush(MediaColor.FromRgb(243, 156, 18))
                      : new SolidColorBrush(MediaColor.FromRgb(231, 76, 60));

            var item = new SubtaskDisplayItem
            {
                Id = nextId,
                Name = $"Шаг {nextId}: {_selectedTask}",
                Task = _selectedTask,
                LoadKg = _loadWeightKg,
                VerticalMm = _verticalMm,
                ReachMm = _reachMm,
                CyclesPerDay = _frequencyPerDay,
                PeakCompressionN = res.LumbarCompressionN,
                LCFCD = res.CumulativeCompDCR,
                TaskDCR = res.OverallDCR,
                EawsScore = res.EawsScore,
                RiskCategory = res.RiskCategory,
                RiskBrush = brush,
                OriginalInputs = curInputs
            };

            _subtasks.Add(item);
            _messageService?.AppendMessage($"[Work(s) Ergo] Добавлен подэтап {nextId}: DCR={res.OverallDCR:P0}, L5/S1={res.LumbarCompressionN:F0} Н", MessageLevel.Info);
            EvaluateShiftJob();
        }

        public void RemoveSubtask(SubtaskDisplayItem item)
        {
            if (item != null && _subtasks.Contains(item))
            {
                _subtasks.Remove(item);
                EvaluateShiftJob();
            }
        }

        public void ClearSubtasks()
        {
            _subtasks.Clear();
            CompositeLcfcd = 0.0;
            CompositeOverallDCR = 0.0;
            CompositePeakCompN = 0.0;
            CompositeDutyCycle = 0.0;
            CompositeEawsScore = 0.0;
            CompositeRiskCategory = "Not evaluated";
            CompositeRiskBrush = new SolidColorBrush(MediaColor.FromRgb(127, 140, 141));
            IsJobEvaluated = false;
            _messageService?.AppendMessage("[Work(s) Ergo] Список подэтапов смены очищен.", MessageLevel.Info);
        }

        public void EvaluateShiftJob()
        {
            if (_subtasks.Count == 0)
            {
                ClearSubtasks();
                return;
            }

            var inputsList = _subtasks.Select(s => s.OriginalInputs).ToList();
            var comp = ErgonomicMathEngine.CalculateCompositeJob(inputsList);

            CompositeLcfcd = comp.CompositeLCFCD;
            CompositeOverallDCR = comp.CompositeOverallDCR;
            CompositePeakCompN = comp.PeakCompressionN;
            CompositeDutyCycle = comp.CompositeDutyCycle;
            CompositeEawsScore = comp.CompositeEawsScore;
            CompositeRiskCategory = comp.RiskCategory;

            if (comp.CompositeOverallDCR <= 0.85)
                CompositeRiskBrush = new SolidColorBrush(MediaColor.FromRgb(46, 204, 113));
            else if (comp.CompositeOverallDCR <= 1.00)
                CompositeRiskBrush = new SolidColorBrush(MediaColor.FromRgb(243, 156, 18));
            else
                CompositeRiskBrush = new SolidColorBrush(MediaColor.FromRgb(231, 76, 60));

            IsJobEvaluated = true;
            _messageService?.AppendMessage($"[Work(s) Ergo] Итог по всей смене: DCR={CompositeOverallDcrText}, Пик L5/S1={CompositePeakCompN:F0} Н, Усталость={CompositeLcfcd:F3}", MessageLevel.Info);
        }

        public void OpenReportFolder()
        {
            try
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WorksErgo_Reports");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                System.Diagnostics.Process.Start("explorer.exe", dir);
                _messageService?.AppendMessage("[Work(s) Ergo] Открыта папка отчетов: " + dir, MessageLevel.Info);
            }
            catch (Exception ex)
            {
                _messageService?.AppendMessage("[Work(s) Ergo] Ошибка открытия отчетов: " + ex.Message, MessageLevel.Warning);
            }
        }

        #endregion

        public void Recalculate()
        {
            var inputs = new PostureInputs
            {
                Percentile = _selectedPercentile,
                Task = _selectedTask,
                Technique = _selectedTechnique,
                Grip = _selectedGrip,
                Coupling = _selectedCoupling,
                LoadKg = _loadWeightKg,
                ReachMm = _reachMm,
                VerticalMm = _verticalMm,
                TravelMm = _travelMm,
                AsymmetryDeg = _asymmetryDeg,
                LateralTiltDeg = _lateralTiltDeg,
                AccelerationMs2 = _dynamicAccelerationMs2,
                FrequencyLiftsPerMin = _frequencyLiftsPerMin,
                FrequencyPerDay = _frequencyPerDay,
                DurationHours = _durationHours
            };

            var res = ErgonomicMathEngine.Evaluate(inputs);

            OverallDCR = res.OverallDCR;
            RiskCategory = res.RiskCategory;
            LumbarCompN = res.LumbarCompressionN;
            LumbarDCR = res.LumbarDCR;
            KneeFlexionDeg = res.KneeFlexionDeg;
            TrunkFlexionDeg = res.TrunkFlexionDeg;
            HipHeightMm = res.HipHeightMm;
            KneeMomentNm = res.KneeMomentNm;
            CenterOfPressureMm = res.CenterOfPressureMm;
            IsBalanced = res.IsBalanced;
            NioshRWLKg = res.NioshRWLKg;
            NioshLI = res.NioshLI;
            SnookMAWLKg = res.SnookMAWLKg;
            SnookDCR = res.SnookDCR;
            ArmDCR = res.ArmDCR;
            HandDCR = res.HandDCR;
            RulaScore = res.RulaScore;
            RebaScore = res.RebaScore;
            EawsScore = res.EawsScore;
            EawsTrafficLight = res.EawsTrafficLight;
            EawsSec1 = res.EawsSection1_Postures;
            EawsSec2 = res.EawsSection2_Forces;
            EawsSec3 = res.EawsSection3_MMH;
            EawsSec4 = res.EawsSection4_Reps;
            LimitingFactor = res.PrimaryLimitingFactor;
            Recommendation = res.Recommendation;

            if (res.OverallDCR <= 0.85)
                RiskBrush = new SolidColorBrush(MediaColor.FromRgb(46, 204, 113)); // Green
            else if (res.OverallDCR <= 1.00)
                RiskBrush = new SolidColorBrush(MediaColor.FromRgb(243, 156, 18)); // Orange
            else
                RiskBrush = new SolidColorBrush(MediaColor.FromRgb(231, 76, 60));  // Red

            if (res.EawsScore <= 25.0)
                EawsBrush = new SolidColorBrush(MediaColor.FromRgb(46, 204, 113));
            else if (res.EawsScore <= 50.0)
                EawsBrush = new SolidColorBrush(MediaColor.FromRgb(243, 156, 18));
            else
                EawsBrush = new SolidColorBrush(MediaColor.FromRgb(231, 76, 60));

            _messageService?.AppendMessage($"[Work(s) Ergo] Расчет биомеханики: DCR={OverallDcrText}, L5/S1={LumbarCompN:F0} Н, EAWS={EawsScore:F1} pts, Статус: {RiskCategory}", MessageLevel.Info);
        }
    }
}
