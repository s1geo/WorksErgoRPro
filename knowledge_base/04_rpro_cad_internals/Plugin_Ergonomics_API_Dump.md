# Assembly Analysis: Plugin.Ergonomics
- **Full Name:** Plugin.Ergonomics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
- **Location:** D:\Apps\RProv222\Plugin.Ergonomics.dll
- **Target Framework:** v4.0.30319

## Types and Signatures

### <PrivateImplementationDetails>
- **Base Type:** System.Object
- **Attributes:** CompilerGeneratedAttribute

**Methods:**
  * `UInt32 ComputeStringHash(String s)`

---

### ErgonomicsAddon.Ergonomics.Angle
- **Base Type:** System.Object
- **Interfaces:** IDisposable, INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `Int32 Value { get; set; }`
  * `String ValueStr { get; set; }`

**Methods:**
  * `Void References(String signReference, String increaseReference)`
  * `Vector3 GetReferenceVector(String reference)`
  * `Matrix CreateNewBasis(Vector3 direction, Matrix basis)`
  * `Vector3 TransformVectorToNewBasis(Vector3 vector, Vector3 newX, Vector3 newY, Vector3 newZ)`
  * `Double GetSignedAngleWithProjection(Vector3 vector, Vector3 projection, Vector3 signReference, Vector3 increaseReference)`
  * `Void UpdateValue(Boolean debug)`
  * `Void Link_PropertyChanged(Object sender, PropertyChangedEventArgs e)`
  * `Void Subscribe()`
  * `Void Dispose()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.Animations.Animation
- **Base Type:** System.Object
- **Interfaces:** INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `String Name { get; set; }`
  * `IStatementScope StatementScope { get; set; }`
  * `UInt32 PTPCount { get; set; }`
  * `BindableCollection`1 Comments { get; set; }`

**Methods:**
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.Animations.AnimationsView
- **Base Type:** System.Windows.Controls.UserControl
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient, IAddChild, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsAddon.Ergonomics.Animations.AnimationsViewModel
- **Base Type:** Caliburn.Micro.Screen
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, IChild
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `BindableCollection`1 Animations { get; set; }`
  * `Boolean AnimationsIsExpanded { get; set; }`

**Methods:**
  * `Void ClearAnimations()`
  * `Void ParseAnimations()`
  * `Void AnimationsAddComment(Object source)`
  * `Void AnimationsDeleteComment(Object source)`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.Animations.Comment
- **Base Type:** System.Object
- **Interfaces:** INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `Int32 SupportIndex { get; set; }`
  * `UInt32 PTPIndex { get; set; }`
  * `Int32 ModeIndex { get; set; }`

**Methods:**
  * `Void CreateICommentStatement()`
  * `Void DeleteICommentStatement()`
  * `Void Routine_StatementRemoved(Object sender, StatementRemovedEventArgs e)`
  * `Void WhileStatement_StatementRemoved(Object sender, StatementRemovedEventArgs e)`
  * `Void DeleteComment()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.ErgonomicsOperators
- **Base Type:** System.Object
- **Interfaces:** INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `ErgonomicsPaneViewModel Parent { get; set; }`
  * `BindableCollection`1 Operators { get; set; }`
  * `ISimComponent SelectedOperator { get; set; }`
  * `BindableCollection`1 SelectedOperatorLinks { get; set; }`
  * `List`1 SelectedOperatorAngles { get; set; }`

**Methods:**
  * `Void SetOperators(BindableCollection`1 simComponents)`
  * `Void SelectedOperatorDisposeLinks()`
  * `Void SelectedOperatorCreateLinks()`
  * `Void SelectedOperatorDisposeAngles()`
  * `Void SelectedOperatorSubscribeAngles()`
  * `Void SelectedOperatorCreateAngles()`
  * `Link GetLink(String name)`
  * `Void SelectedOperatorSubscribe()`
  * `Void SelectedOperatorUnsubscribe()`
  * `Void Routine_StatementAdded(Object sender, StatementAddedEventArgs e)`
  * `Void Routine_StatementRemoved(Object sender, StatementRemovedEventArgs e)`
  * `Void Program_RoutineAdded(Object sender, RoutineAddedEventArgs e)`
  * `Void Program_RoutineRemoving(Object sender, RoutineRemovingEventArgs e)`
  * `Void HumanExecutor_StatementExecuted_Comments(Object sender, StatementExecutedEventArgs e)`
  * `Void HumanExecutor_ActiveProgramChanged_Comments(Object sender, EventArgs e)`
  * `Void Simulation_SimulationReset_Comments(Object sender, EventArgs e)`
  * `Void SelectedOperatorNull()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.ErgonomicsPaneView
- **Base Type:** System.Windows.Controls.UserControl
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient, IAddChild, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsAddon.Ergonomics.ErgonomicsPaneViewModel
- **Base Type:** RProSoftDigital1.UX.Shared.DockableScreen
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, IChild, IDockableScreen, ICustomScreenVisibility
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `ErgonomicsOperators Operators { get; set; }`
  * `AnimationsViewModel AnimationsViewModel { get; set; }`
  * `RULAViewModel RULAViewModel { get; set; }`
  * `REBAViewModel REBAViewModel { get; set; }`
  * `ISOViewModel ISOViewModel { get; set; }`
  * `Double ScrollViewerMaxHeight { get; set; }`
  * `UInt32 SimulationRunTime { get; set; }`
  * `Boolean HumanIsSitting { get; set; }`
  * `Boolean TrunkSupport { get; set; }`
  * `Boolean RightArmSupport { get; set; }`
  * `Boolean LeftArmSupport { get; set; }`
  * `Boolean IsHideable { get; set; }`

**Methods:**
  * `Void OnViewLoaded(Object view)`
  * `Void OnActivate()`
  * `Void OnDeactivate(Boolean close)`
  * `Void LocalizationService_LanguageChanged(Object sender, LanguageChangedEventArgs e)`
  * `Void World_ComponentAdded(Object sender, ComponentAddedEventArgs e)`
  * `Void World_ComponentRemoving(Object sender, ComponentRemovingEventArgs e)`
  * `Void UserControl_SizeChanged(Object source)`
  * `Void PickFrom3DWorld_Click()`
  * `Void RunSimulation(Double simulationRunTime)`
  * `Void GetSimulationReport_Simulation_SimulationRun(Object sender, EventArgs e)`
  * `Void HumanExecutor_StatementExecuted_Scores(Object sender, StatementExecutedEventArgs e)`
  * `Void GetSimulationReport_Simulation_SimulationStopped(Object sender, EventArgs e)`
  * `Void DrawSimulationReportChart(DateTime dateTime)`
  * `Void GetSimulationReport_Simulation_SimulationReset(Object sender, EventArgs e)`
  * `Void GetSimulationReport(String str)`
  * `Void DrawReportChart(DateTime dateTime)`
  * `Void GetReport()`
  * `Void UpdateLimbs()`
  * `Void NotifyPropertyChanged(String propertyName)`
  * `Void <PickFrom3DWorld_Click>g__completed|54_0(Object sender, ResultCompletionEventArgs e)`

---

### ErgonomicsAddon.Ergonomics.ISO.ISOAngle
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `String NameKey { get; set; }`
  * `List`1 Results { get; set; }`

**Methods:**
  * `Void AddValue(Boolean humanIsSitting, Boolean trunkSupport, Boolean rightArmSupport, Boolean leftArmSupport)`
  * `Void FindStaticIntervals()`
  * `Void FindAverageAnglesInIntervals(List`1 intervals)`
  * `Boolean IsAcceptable(Double avgAngle, Double time, Boolean humanIsSitting, Boolean trunkSupport, Boolean rightArmSupport, Boolean leftArmSupport)`
  * `String IsAcceptableReasonRUS(Double avgAngle, Double time, Boolean humanIsSitting, Boolean trunkSupport, Boolean rightArmSupport, Boolean leftArmSupport)`
  * `String IsAcceptableReason(Double avgAngle, Double time, Boolean humanIsSitting, Boolean trunkSupport, Boolean rightArmSupport, Boolean leftArmSupport)`
  * `Double MaxTime(Double avgAngle)`
  * `Double CalculateAnkleAngle()`
  * `Void Clear()`

---

### ErgonomicsAddon.Ergonomics.ISO.ISOResult
- **Base Type:** System.Object
- **Interfaces:** INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `String Output { get; set; }`
  * `Double StartTime { get; set; }`

**Methods:**
  * `String ConvertSecondsToString(Double totalSeconds)`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.ISO.ISOView
- **Base Type:** System.Windows.Controls.UserControl
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient, IAddChild, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsAddon.Ergonomics.ISO.ISOViewModel
- **Base Type:** Caliburn.Micro.Screen
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, IChild
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `List`1 ISOAngles { get; set; }`
  * `BindableCollection`1 ISOResults { get; set; }`
  * `Visibility ISOResultShowAcceptable { get; set; }`
  * `Visibility ISOResultShowNotAcceptable { get; set; }`

**Methods:**
  * `Void ClearListISOAngles()`
  * `Void CreateISOAngles()`
  * `Void UpdateISOAngles()`
  * `Void FindStaticIntervalsISOAngles()`
  * `Void ClearISOAngles()`
  * `Void StartISOResults()`
  * `Void ClearISOResults()`
  * `Void CreateISOResults()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.Link
- **Base Type:** System.Object
- **Interfaces:** IDisposable, INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `String Name { get; set; }`
  * `ISimNode SimNode { get; set; }`
  * `Matrix Matrix { get; set; }`
  * `Boolean IsSubscribed { get; set; }`

**Methods:**
  * `Void SimNode_TransformationChanged(Object sender, EventArgs e)`
  * `Void Subscribe()`
  * `Void Dispose()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.Output.Block
- **Base Type:** System.Object
- **Interfaces:** IDisposable
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `Limb Parent { get; set; }`
  * `List`1 Lines { get; set; }`
  * `Conclusion Conclusion { get; set; }`

**Methods:**
  * `Void RULAUpdateConclusion()`
  * `Void REBAUpdateConclusion()`
  * `Void Line_PropertyChanged(Object sender, PropertyChangedEventArgs e)`
  * `Void ChangeAngles(List`1 angles)`
  * `Void Dispose()`

---

### ErgonomicsAddon.Ergonomics.Output.Conclusion
- **Base Type:** System.Object
- **Interfaces:** IDisposable, INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `String Name { get; set; }`
  * `String Result { get; set; }`

**Methods:**
  * `Void LocalizationService_LanguageChanged(Object sender, LanguageChangedEventArgs e)`
  * `Void Dispose()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.Output.Limb
- **Base Type:** System.Object
- **Interfaces:** IDisposable, INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `ErgonomicsPaneViewModel Parent { get; set; }`
  * `String Header { get; set; }`
  * `List`1 Blocks { get; set; }`
  * `Byte PostureScore { get; set; }`

**Methods:**
  * `Void LocalizationService_LanguageChanged(Object sender, LanguageChangedEventArgs e)`
  * `Void RULAUpdatePostureScore()`
  * `Void REBAUpdatePostureScore()`
  * `Void Conclusion_PropertyChanged(Object sender, PropertyChangedEventArgs e)`
  * `Void ChangeAngles(List`1 listAngles)`
  * `Void Dispose()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.Output.Line
- **Base Type:** System.Object
- **Interfaces:** IDisposable, INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `Block Parent { get; set; }`
  * `String Name { get; set; }`
  * `Angle Angle { get; set; }`
  * `String Result { get; set; }`

**Methods:**
  * `Void LocalizationService_LanguageChanged(Object sender, LanguageChangedEventArgs e)`
  * `Void RULAUpdateResult()`
  * `Void REBAUpdateResult()`
  * `Void ChangeAngle(Angle angle)`
  * `Void Angle_PropertyChanged(Object sender, PropertyChangedEventArgs e)`
  * `Void Unsubscribe()`
  * `Void Dispose()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.REBA.REBATables
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Methods:**
  * `Void VisualizeTableA(XGraphics gfx, Double x, Double y, Double width, Double height, Byte scoreTrunk, Byte scoreNeck, Byte scoreLegs)`
  * `Void VisualizeTableB(XGraphics gfx, Double x, Double y, Double width, Double height, Byte scoreShoulder, Byte scoreElbow, Byte scoreWrist)`
  * `Void VisualizeTableC(XGraphics gfx, Double x, Double y, Double width, Double height, Byte scoreNeckTrunkLegs, Byte scoreRightArm, Byte scoreLeftArm)`

---

### ErgonomicsAddon.Ergonomics.REBA.REBAView
- **Base Type:** System.Windows.Controls.UserControl
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient, IAddChild, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsAddon.Ergonomics.REBA.REBAViewModel
- **Base Type:** Caliburn.Micro.Screen
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, IChild
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `List`1 Scores { get; set; }`
  * `List`1 ScoresPercent { get; set; }`
  * `List`1 Limbs { get; set; }`
  * `Byte Score { get; set; }`
  * `Byte LoadSelectedIndex { get; set; }`
  * `BindableCollection`1 Load { get; set; }`
  * `Byte EffortSelectedIndex { get; set; }`
  * `BindableCollection`1 Effort { get; set; }`
  * `Byte GripSelectedIndex { get; set; }`
  * `BindableCollection`1 Grip { get; set; }`
  * `Boolean Activity0IsChecked { get; set; }`
  * `Boolean Activity1IsChecked { get; set; }`
  * `Boolean Activity2IsChecked { get; set; }`

**Methods:**
  * `Void OnViewLoaded(Object view)`
  * `Void LocalizationService_LanguageChanged(Object sender, LanguageChangedEventArgs e)`
  * `Void ComboBoxesGetText()`
  * `Void DisposeLimbs()`
  * `Void CreateLimbs()`
  * `Void UpdateLimbs()`
  * `Void Limb_PropertyChanged(Object sender, PropertyChangedEventArgs e)`
  * `Void Update()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.Ergonomics.Reports.Report
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Methods:**
  * `Void AddTextBox(PdfDocument pdf, PdfPage page, Double x, Double y, Double width, Double height, String text)`
  * `ValueTuple`4 ReportAddPage(PdfDocument document)`
  * `Void DrawStringH1(PdfPage page, XTextFormatter tf, String text, Double& y)`
  * `Void DrawStringH2(PdfPage page, XTextFormatter tf, String text, Double& y)`
  * `Void DrawStringP(PdfPage page, XTextFormatter tf, String text, Double& y)`
  * `Void DrawStringP(PdfPage page, XTextFormatter tf, String text, String text2, Double& y)`
  * `Void DrawStringPageNumber(PdfPage page, XTextFormatter tf, Byte& pageNumber)`
  * `Void Header(PdfDocument document, DateTime dateTime, String operatorName)`
  * `Void FinalResult(PdfDocument document, Byte pageNumber, Byte RULAScore, Byte REBAScore)`
  * `Void RULAReport(PdfDocument document, Byte pageNumber, List`1 RULALimbs, Byte RULALoadSelectedIndex, Byte RULAPhysicalWorkSelectedIndex)`
  * `Void REBAReport(PdfDocument document, Byte pageNumber, List`1 REBALimbs, Byte REBALoadSelectedIndex, Byte REBAEffortSelectedIndex, Byte REBAGripSelectedIndex, Boolean REBAActivity0IsChecked, Boolean REBAActivity1IsChecked, Boolean REBAActivity2IsChecked)`

---

### ErgonomicsAddon.Ergonomics.Reports.SimulationReport
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Methods:**
  * `Void AddTextBox(PdfDocument pdf, PdfPage page, Double x, Double y, Double width, Double height, String text)`
  * `ValueTuple`4 ReportAddPage(PdfDocument document)`
  * `Void DrawStringH1(PdfPage page, XTextFormatter tf, String text, Double& y)`
  * `Void DrawStringH2(PdfPage page, XTextFormatter tf, String text, Double& y)`
  * `Void DrawStringP(PdfPage page, XTextFormatter tf, String text, Double& y)`
  * `Void DrawStringISOP(PdfPage page, XTextFormatter tf, String text, Double& y)`
  * `Void DrawStringPageNumber(PdfPage page, XTextFormatter tf, Byte& pageNumber)`
  * `Void Header(PdfDocument document, DateTime dateTime, String operatorName)`
  * `Void RULADrawPieDiagram(XGraphics gfx, List`1 RULAScoresPercent, Double x, Double& y, Double width, Double height)`
  * `Void RULADrawOxyPlot(XGraphics gfx, List`1 RULAScores, Double x, Double& y, Double width, Double height)`
  * `Void RULAReport(PdfDocument document, Byte pageNumber, List`1 RULAScores, List`1 RULAScoresPercent)`
  * `Void REBADrawPieDiagram(XGraphics gfx, List`1 REBAScoresPercent, Double x, Double& y, Double width, Double height)`
  * `Void REBADrawOxyPlot(XGraphics gfx, List`1 REBAScores, Double x, Double& y, Double width, Double height)`
  * `Void REBAReport(PdfDocument document, Byte pageNumber, List`1 REBAScores, List`1 REBAScoresPercent)`
  * `Void ISOReport(PdfDocument document, Byte pageNumber, Visibility isAcceptable, BindableCollection`1 ISOResults)`

---

### ErgonomicsAddon.Ergonomics.RULA.RULATables
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Methods:**
  * `Void VisualizeTableA(XGraphics gfx, Double x, Double y, Double width, Double height, Byte scoreShoulder, Byte scoreForearm, Byte scoreWrist, Byte scoreTwistForearm)`
  * `Void VisualizeTableB(XGraphics gfx, Double x, Double y, Double width, Double height, Byte scoreNeck, Byte scoreTrunk, Byte scoreLegs)`
  * `Void VisualizeTableC(XGraphics gfx, Double x, Double y, Double width, Double height, Byte scoreRightArm, Byte scoreLeftArm, Byte scoreNeckTrunkLegs)`

---

### ErgonomicsAddon.Ergonomics.RULA.RULAView
- **Base Type:** System.Windows.Controls.UserControl
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient, IAddChild, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsAddon.Ergonomics.RULA.RULAViewModel
- **Base Type:** Caliburn.Micro.Screen
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, IChild
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `List`1 Scores { get; set; }`
  * `List`1 ScoresPercent { get; set; }`
  * `List`1 Limbs { get; set; }`
  * `Byte Score { get; set; }`
  * `Byte LoadSelectedIndex { get; set; }`
  * `BindableCollection`1 Load { get; set; }`
  * `Byte PhysicalWorkSelectedIndex { get; set; }`
  * `BindableCollection`1 PhysicalWork { get; set; }`

**Methods:**
  * `Void OnViewLoaded(Object view)`
  * `Void LocalizationService_LanguageChanged(Object sender, LanguageChangedEventArgs e)`
  * `Void ComboBoxesGetText()`
  * `Void DisposeLimbs()`
  * `Void CreateLimbs()`
  * `Void UpdateLimbs()`
  * `Void Limb_PropertyChanged(Object sender, PropertyChangedEventArgs e)`
  * `Void Update()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsAddon.ErgonomicsDataTemplates
- **Base Type:** System.Windows.ResourceDictionary
- **Interfaces:** IDictionary, ICollection, IEnumerable, ISupportInitialize, IUriContext, INameScope, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsAddon.ErgonomicsHelp
- **Base Type:** RProSoftDigital1.UX.Shared.ActionItem
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IActionItem, IToolTip, IPartImportsSatisfiedNotification
- **Attributes:** NullableContextAttribute, NullableAttribute, ExportAttribute

**Methods:**
  * `Void Execute()`

---

### ErgonomicsAddon.ErgonomicsLicenseChecker
- **Base Type:** System.Object

**Methods:**
  * `Boolean HasFeat()`

---

### ErgonomicsAddon.ErgonomicsUtils
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Methods:**
  * `Double Clamp(Double v, Double min, Double max)`
  * `Void AppendMessage(String message, MessageLevel messageLevel)`
  * `Void AppendMessageWarning(String message)`
  * `Void AppendMessageWarningByKey(String key)`
  * `Void UpdateOperatorComponent(ISimComponent component)`
  * `Vector3 TransformVectorToNewBasis(Vector3 vector, Vector3 newX, Vector3 newY, Vector3 newZ)`

---

### ErgonomicsAddon.Ribbon.ActionItemErgonomics
- **Base Type:** RProSoftDigital1.UX.Shared.ActionItem
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IActionItem, IToolTip, IPartImportsSatisfiedNotification
- **Attributes:** NullableContextAttribute, NullableAttribute, ExportAttribute, PartCreationPolicyAttribute

**Methods:**
  * `Void Execute()`

---

### ErgonomicsAddon.Ribbon.RibbonGroupErgonomics
- **Base Type:** RProSoftDigital1.UX.Ribbon.RibbonGroupBase
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IRibbonGroup
- **Attributes:** NullableContextAttribute, NullableAttribute, ExportAttribute

**Properties:**
  * `String Header { get; set; }`
  * `String Icon { get; set; }`
  * `String Id { get; set; }`

---

### ErgonomicsAddon.Ribbon.SiteSetupErgonomics
- **Base Type:** System.Object
- **Interfaces:** IPlugin
- **Attributes:** ExportAttribute

**Methods:**
  * `Void Exit()`
  * `Void Initialize()`

---

### Microsoft.CodeAnalysis.EmbeddedAttribute
- **Base Type:** System.Attribute
- **Interfaces:** _Attribute
- **Attributes:** CompilerGeneratedAttribute, EmbeddedAttribute

---

### System.Runtime.CompilerServices.NullableAttribute
- **Base Type:** System.Attribute
- **Interfaces:** _Attribute
- **Attributes:** CompilerGeneratedAttribute, EmbeddedAttribute, AttributeUsageAttribute

---

### System.Runtime.CompilerServices.NullableContextAttribute
- **Base Type:** System.Attribute
- **Interfaces:** _Attribute
- **Attributes:** CompilerGeneratedAttribute, EmbeddedAttribute, AttributeUsageAttribute

---

### System.Runtime.CompilerServices.RefSafetyRulesAttribute
- **Base Type:** System.Attribute
- **Interfaces:** _Attribute
- **Attributes:** CompilerGeneratedAttribute, EmbeddedAttribute, AttributeUsageAttribute

---

