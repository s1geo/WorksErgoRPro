# Assembly Analysis: Plugin.ErgonomicsWPP
- **Full Name:** Plugin.ErgonomicsWPP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
- **Location:** D:\Apps\RProv222\Plugin.ErgonomicsWPP.dll
- **Target Framework:** v4.0.30319

## Types and Signatures

### ErgonomicsWPPAddon.ErgonomicsHelp
- **Base Type:** RProSoftDigital1.UX.Shared.ActionItem
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IActionItem, IToolTip, IPartImportsSatisfiedNotification
- **Attributes:** NullableContextAttribute, NullableAttribute, ExportAttribute

**Methods:**
  * `Void Execute()`

---

### ErgonomicsWPPAddon.ErgonomicsLicenseChecker
- **Base Type:** System.Object

**Methods:**
  * `Boolean HasFeat()`

---

### ErgonomicsWPPAddon.ErgonomicsUtils
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Methods:**
  * `Double Clamp(Double v, Double min, Double max)`
  * `Void AppendMessage(String message, MessageLevel messageLevel)`
  * `Void AppendMessageWarning(String message)`
  * `Void AppendMessageWarningByKey(String key)`
  * `Void UpdateOperatorComponent(ISimComponent component)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.ErgonomicsWPPOperators
- **Base Type:** System.Object
- **Interfaces:** INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `ErgonomicsWPPPaneViewModel Parent { get; set; }`
  * `BindableCollection`1 Operators { get; set; }`
  * `ISimComponent SelectedOperator { get; set; }`
  * `BindableCollection`1 SelectedOperatorLinks { get; set; }`
  * `Link SelectedLink { get; set; }`
  * `Link PreviousSelectedLink { get; set; }`

**Methods:**
  * `Void Simulation_SimulationStopped(Object sender, EventArgs e)`
  * `Void Simulation_SimulationReset(Object sender, EventArgs e)`
  * `Void SetOperators(BindableCollection`1 simComponents)`
  * `Void SelectedOperatorCreateLinks()`
  * `Void CalculateAllLinks()`
  * `Void SelectedOperatorSubscribeLinks()`
  * `Void SelectedOperatorDisposeLinks()`
  * `Void Link_PropertyCurrentAngles_PropertyChanged(Object sender, EventArgs e)`
  * `Int32 ColorToIndex(Color color)`
  * `Color IndexToColor(Int32 index)`
  * `IMaterial ColorToMaterial(Color color)`
  * `Void AssignMaterialToLinkGeometryFeatures(Link link, IMaterial material)`
  * `Void AssignMaterialToGeometryFeature(IGeometryFeature geometryFeature, IMaterial material)`
  * `Void SelectedOperatorUpdateLinks()`
  * `Void SelectedOperatorNull()`
  * `Void NotifyPropertyChanged(String propertyName)`
  * `Void <SelectedOperatorSubscribeLinks>b__36_0(Link link)`
  * `Void <SelectedOperatorDisposeLinks>b__37_0(Link link)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.ErgonomicsWPPPaneView
- **Base Type:** System.Windows.Controls.UserControl
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient, IAddChild, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.ErgonomicsWPPPaneViewModel
- **Base Type:** RProSoftDigital1.UX.Shared.DockableScreen
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, IChild, IDockableScreen, ICustomScreenVisibility
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `PointOfViewViewModel PointOfViewViewModel { get; set; }`
  * `PostureEditorViewModel PostureEditorViewModel { get; set; }`
  * `StandardPosturesViewModel StandardPosturesViewModel { get; set; }`
  * `PostureAssessmentViewModel PostureAssessmentViewModel { get; set; }`
  * `PostureAssessmentEvaluationResult PostureAssessmentEvaluationResult { get; set; }`
  * `ErgonomicsWPPOperators Operators { get; set; }`
  * `Boolean IsHideable { get; set; }`

**Methods:**
  * `Void OnViewLoaded(Object view)`
  * `Void OnActivate()`
  * `Void OnDeactivate(Boolean close)`
  * `Void LocalizationService_LanguageChanged(Object sender, LanguageChangedEventArgs e)`
  * `Void PostureAssessmentViewModel_PropertyChanged(Object sender, PropertyChangedEventArgs e)`
  * `Void UpdatePostureAssessmentEvaluationResult()`
  * `Void World_ComponentAdded(Object sender, ComponentAddedEventArgs e)`
  * `Void World_ComponentRemoving(Object sender, ComponentRemovingEventArgs e)`
  * `Void PickFrom3DWorld_Click()`
  * `Void OffsetDefinition()`
  * `Void SimNodesKeyPressed(Object sender, String key)`
  * `Void PickNodeFrom3DWorld_Click()`
  * `Void NotifyPropertyChanged(String propertyName)`
  * `Void <UpdatePostureAssessmentEvaluationResult>b__29_1(PostureAssessmentCellEvaluation d)`
  * `Void <PickFrom3DWorld_Click>g__completed|32_0(Object sender, ResultCompletionEventArgs e)`
  * `Void <PickNodeFrom3DWorld_Click>g__completed|35_0(Object sender, ResultCompletionEventArgs e)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.LegsIK
- **Base Type:** System.Object

**Methods:**
  * `Void Solve(Double& hipAngleDeg, Double& kneeAngleDeg)`
  * `Void UpdateByHipZ(Double newHipZ, Double& newFootX, Double& hipAngleDeg, Double& kneeAngleDeg)`
  * `Void UpdateByFootX(Double newFootX, Double& newHipZ, Double& hipAngleDeg, Double& kneeAngleDeg)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.Link
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute, DebuggerDisplayAttribute

**Properties:**
  * `String Name { get; set; }`
  * `ISimNode SimNode { get; set; }`
  * `List`1 SubLinks { get; set; }`
  * `IProperty PropertyCurrentAngles { get; set; }`
  * `ISimNode SimNodeGeometry { get; set; }`
  * `Vector3D SimNodeVector { get; set; }`

**Methods:**
  * `Void CalculateCurrentDirections()`
  * `Void CalculateDirections()`
  * `Void CalculateZeroAngleDirections()`
  * `Void CalculateAngles()`
  * `Void SetDofValues(Double dofValueX, Double dofValueY, Double dofValueZ)`
  * `Void CalculateAll()`
  * `Void CalculateWPR()`
  * `Vector3D CalculateTransformedDirection(Vector3D direction)`
  * `Matrix GetManipulatorOrigin()`
  * `Void Dispose()`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.Manipulate.CenterLinkManipulatorPart
- **Base Type:** RProSoftDigital1.UX.Viewport.ManipulatorPart
- **Interfaces:** IResource, IVisual3DContainer, IAnimatable, IInputElement
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `Double GeometryScale { get; set; }`
  * `Nullable`1 CameraForward { get; set; }`
  * `Nullable`1 CameraUp { get; set; }`

**Methods:**
  * `Void UpdateGeometry()`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.Manipulate.EulerHelper
- **Base Type:** System.Object

**Methods:**
  * `Double CopySign(Double value, Double sign)`
  * `Quaternion EulerToQuaternion(Double xDeg, Double yDeg, Double zDeg)`
  * `Vector3 QuaternionToEuler(Quaternion q)`
  * `ValueTuple`3 AddAndRecompute(Double angleX, Double angleY, Double angleZ, Vector3D direction, Double deltaDeg)`
  * `Vector3 ProjectOnPlane(Double angleX, Double angleY, Double angleZ, Char axis, Vector3D v, Boolean useProj)`
  * `Vector3 RotateVector(Double angleX, Double angleY, Double angleZ, Vector3 v)`
  * `Vector3 RotateVector(Double angleX, Double angleY, Double angleZ, Vector3D v)`
  * `Vector3D RotateVector3D(Double angleX, Double angleY, Double angleZ, Vector3D v)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.Manipulate.InvisibleLinkManipulatorPart
- **Base Type:** RProSoftDigital1.UX.Viewport.ManipulatorPart
- **Interfaces:** IResource, IVisual3DContainer, IAnimatable, IInputElement
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `RotateLinkManipulatorPart AssociatedPart { get; set; }`
  * `Double GeometryScale { get; set; }`

**Methods:**
  * `Void UpdateGeometry()`
  * `Void OnMouseEnter(MouseEventArgs e)`
  * `Void OnMouseLeave(MouseEventArgs e)`
  * `Void PassMouseEvent(MouseEventArgs e)`
  * `Void OnMouseLeftButtonUp(MouseButtonEventArgs e)`
  * `Void OnMouseLeftButtonDown(MouseButtonEventArgs e)`
  * `Void PassMouseLeftButtonEvent(MouseButtonEventArgs e)`
  * `Void OnMouseMove(MouseEventArgs e)`
  * `Void OnLostMouseCapture(MouseEventArgs e)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.Manipulate.LinkManipulator
- **Base Type:** RProSoftDigital1.UX.Viewport.ManipulatorBase
- **Interfaces:** IResource, IVisual3DContainer, IAnimatable, IAddChild, IManipulator, IHandle`1, IHandle
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `BindableCollection`1 Links { get; set; }`
  * `Link Link { get; set; }`
  * `ISimNode SimNode { get; set; }`
  * `IManipulationTarget Target { get; set; }`
  * `RotateLinkManipulatorPart RotateXLinkManipulator { get; set; }`
  * `RotateLinkManipulatorPart RotateYLinkManipulator { get; set; }`
  * `RotateLinkManipulatorPart RotateZLinkManipulator { get; set; }`
  * `CenterLinkManipulatorPart CenterManipulator { get; set; }`

**Methods:**
  * `Void Target_TransformationChanged(Object sender, EventArgs e)`
  * `Void Target_ObjectInvalidated(Object sender, EventArgs e)`
  * `Void Simulation_SimulationStarted(Object sender, EventArgs e)`
  * `Void Simulation_SimulationStopped(Object sender, EventArgs e)`
  * `Void ApplicationContext_ContextChanged(Object sender, EventArgs e)`
  * `Void UndoService_UndoDone(Object sender, UndoEventArgs e)`
  * `Void UndoService_RedoDone(Object sender, UndoEventArgs e)`
  * `Void UndoRedoDone()`
  * `Void OnCreate()`
  * `Void HelperViewport_CameraChanged(Object sender, EventArgs e)`
  * `Void UpdateGeometryScale()`
  * `Void Manipulator_GotMouseFocus(Object sender, EventArgs e)`
  * `Void Manipulator_LostMouseFocus(Object sender, EventArgs e)`
  * `Void FocusAll()`
  * `Void UnfocusAll()`
  * `Void PositionHandle()`
  * `Void MakeUniform(Matrix& mat)`
  * `Void AddChildren()`
  * `Void Handle(ToggleLinkManipulatorMessage message)`
  * `Void UpdateVisibility()`
  * `Boolean HasTargetAndEnabled()`
  * `Void ShowAllParts()`
  * `Void HideAllParts()`
  * `Void RotationStart(RotateLinkManipulatorPart part)`
  * `Void RotateRequest(Double pendingRotation, RotateLinkManipulatorPart part)`
  * `Void RotationEnd(RotateLinkManipulatorPart part)`
  * `Void <set_Link>b__37_0(RotateLinkManipulatorPart r)`
  * `Void <Target_TransformationChanged>b__47_0()`
  * `Void <OnCreate>b__72_0(RotateLinkManipulatorPart r)`
  * `Void <OnCreate>b__72_1(CenterLinkManipulatorPart c)`
  * `Void <AddChildren>b__81_0(RotateLinkManipulatorPart r)`
  * `Void <AddChildren>b__81_1(CenterLinkManipulatorPart c)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.Manipulate.LinkManipulatorDictionaries
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Methods:**
  * `ValueTuple`2 GetStartAndEndAngles(String simNodeName, Char axis)`
  * `Vector3D GetZeroAngleDirection(String simNodeName, Char axis)`
  * `Boolean GetIsFixedAxis(String simNodeName, Char axis)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.Manipulate.RotateLinkManipulatorPart
- **Base Type:** RProSoftDigital1.UX.Viewport.ManipulatorPart
- **Interfaces:** IResource, IVisual3DContainer, IAnimatable, IInputElement
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `SubLink SubLink { get; set; }`
  * `ISimNode SimNode { get; set; }`
  * `IDoubleProperty DofDoubleProperty { get; set; }`
  * `Boolean Fixed { get; set; }`
  * `Double StartAngle { get; set; }`
  * `Double EndAngle { get; set; }`
  * `Visibility SectorVisibility { get; set; }`
  * `Visibility RulerVisibility { get; set; }`
  * `Color Color { get; set; }`
  * `Double GeometryScale { get; set; }`
  * `Double ArcScale { get; set; }`
  * `Nullable`1 CameraForward { get; set; }`
  * `Nullable`1 CameraUp { get; set; }`
  * `Vector3D TransformedDirection { get; set; }`
  * `Vector3D TransformedZeroAngleDirection { get; set; }`
  * `Double CurrentAngle { get; set; }`

**Methods:**
  * `Void SubLink_PropertyChanged(Object sender, PropertyChangedEventArgs e)`
  * `Void SetVisualFocus(Boolean isFocused)`
  * `Void Update()`
  * `Void UpdateGeometry()`
  * `Void OnMouseEnter(MouseEventArgs e)`
  * `Void OnMouseLeave(MouseEventArgs e)`
  * `Void ProcessMouseLeftButtonEvent(MouseButtonEventArgs e)`
  * `Void OnMouseLeftButtonUp(MouseButtonEventArgs e)`
  * `Void OnMouseLeftButtonDown(MouseButtonEventArgs e)`
  * `Void ProcessMouseMoveEvent(MouseEventArgs e)`
  * `Void OnMouseMove(MouseEventArgs e)`
  * `Void ProcessLostMouseCaptureEvent(MouseEventArgs e)`
  * `Void OnLostMouseCapture(MouseEventArgs e)`
  * `Vector3D ProjectToPlane(Point mouseScreenPosition)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.Manipulate.RulerLinkManipulatorPart
- **Base Type:** RProSoftDigital1.UX.Viewport.RulerManipulator
- **Interfaces:** IResource, IVisual3DContainer, IAnimatable, IInputElement
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `RotateLinkManipulatorPart AssociatedPart { get; set; }`
  * `Double Diameter { get; set; }`
  * `Double Thickness { get; set; }`
  * `Vector3D CurrentDirection { get; set; }`
  * `List`1 SnappingPoints { get; set; }`
  * `Vector3D CurrentDirectionSnapped { get; set; }`
  * `Nullable`1 CameraForward { get; set; }`
  * `Nullable`1 CameraUp { get; set; }`

**Methods:**
  * `Void Update()`
  * `Void UpdateGeometry()`
  * `Void UpdateRulerGeometry()`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.Manipulate.SectorLinkManipulatorPart
- **Base Type:** RProSoftDigital1.UX.Viewport.ManipulatorPart
- **Interfaces:** IResource, IVisual3DContainer, IAnimatable, IInputElement
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `RotateLinkManipulatorPart AssociatedPart { get; set; }`
  * `Visibility SectorVisibility { get; set; }`
  * `Color Color { get; set; }`
  * `Double GeometryScale { get; set; }`
  * `Double CurrentAngle { get; set; }`
  * `Double Diameter { get; set; }`

**Methods:**
  * `Void UpdateGeometry()`
  * `Void AddPizzaSlice(MeshBuilder meshBuilder, Point3D center, Double innerRadius, Double outerRadius, Double startAngle, Double endAngle, Int32 thetaDiv)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.Manipulate.ToggleLinkManipulatorMessage
- **Base Type:** System.Object

**Properties:**
  * `Boolean ShowTool { get; set; }`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PointOfView.PointOfViewView
- **Base Type:** System.Windows.Controls.UserControl
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient, IAddChild, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Delegate _CreateDelegate(Type delegateType, String handler)`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PointOfView.PointOfViewViewModel
- **Base Type:** Caliburn.Micro.Screen
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, IChild
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `BindableCollection`1 TextBoxWithArrowsLines { get; set; }`
  * `Visibility OperatorIsSelected { get; set; }`
  * `Visibility OperatorNotSelected { get; set; }`
  * `BindableCollection`1 POVTypes { get; set; }`
  * `Int32 POVTypeSelectedIndex { get; set; }`

**Methods:**
  * `Void OnViewLoaded(Object view)`
  * `Void OnActivate()`
  * `Void OnDeactivate(Boolean close)`
  * `Void LocalizationService_LanguageChanged(Object sender, LanguageChangedEventArgs e)`
  * `Void ComboBoxGetText()`
  * `Void ShowPOVWindow()`
  * `Void CreatePOVWindow(String name)`
  * `Void ErgonomicsPaneViewModel_OnRequestRender(Object sender, EventArgs e)`
  * `Void RenderSecondCamera()`
  * `Void POV_Application_LayoutSaving(Object sender, LayoutSavingEventArgs e)`
  * `Void POV_Application_LayoutClosing(Object sender, LayoutClosingEventArgs e)`
  * `Void ClosePOVSplitPane()`
  * `/* unresolved method: FileNotFoundException */ OperatorPOVContentPane_Closed()`
  * `Void DrawPaths()`
  * `Void DrawBinocularPath(Grid grid)`
  * `Void DrawAmbinocularPath(Grid grid)`
  * `Void DrawSideMonocularPath(Grid grid, String eye)`
  * `Void DrawVisualConePath(Grid grid)`
  * `Void ResetLimits()`
  * `Void ResetDistance()`
  * `Void UpdateLinesOperatorSelected()`
  * `Void UpdateMinimumMaximum()`
  * `Void ValueChangedEvent(String index, RoutedPropertyChangedEventArgs`1 eventArgs)`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureAssessment.PostureAssessmentCell
- **Base Type:** System.Object
- **Interfaces:** INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute, DebuggerDisplayAttribute

**Properties:**
  * `String Id { get; set; }`
  * `Int32 BaseLevel { get; set; }`
  * `String Description { get; set; }`
  * `Brush Brush { get; set; }`

**Methods:**
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureAssessment.PostureAssessmentCellEvaluation
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute, DebuggerDisplayAttribute

**Properties:**
  * `String Id { get; set; }`
  * `Int32 Level { get; set; }`
  * `String Description { get; set; }`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureAssessment.PostureAssessmentCoefficient
- **Base Type:** System.Object
- **Interfaces:** INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `Int32 Index { get; set; }`
  * `String Description { get; set; }`
  * `Double Value { get; set; }`
  * `Visibility IsCustom { get; set; }`
  * `Visibility IsNotCustom { get; set; }`

**Methods:**
  * `Void ChangeDescription()`
  * `Void ChangeValue()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureAssessment.PostureAssessmentContext
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `Double TrunkBendAngleDeg { get; set; }`
  * `Double TrunkSideBendAngleDeg { get; set; }`
  * `Double TrunkRotationAngleDeg { get; set; }`
  * `Double HeadBendAngleDeg { get; set; }`
  * `Double HeadSideBendAngleDeg { get; set; }`
  * `Double HeadRotationAngleDeg { get; set; }`
  * `Double RightShoulderElevationAngleDeg { get; set; }`
  * `Double RightArmElevationAngleDeg { get; set; }`
  * `Double RightArmAbductionAngleDeg { get; set; }`
  * `Double LeftShoulderElevationAngleDeg { get; set; }`
  * `Double LeftArmElevationAngleDeg { get; set; }`
  * `Double LeftArmAbductionAngleDeg { get; set; }`
  * `Double HeartLevel { get; set; }`
  * `Double RightShoulderLevel { get; set; }`
  * `Double RightWristLevel { get; set; }`
  * `Double LeftShoulderLevel { get; set; }`
  * `Double LeftWristLevel { get; set; }`
  * `Double HeadLevel { get; set; }`
  * `Double RightWristElevationAngleDeg { get; set; }`
  * `Double LeftWristElevationAngleDeg { get; set; }`
  * `Double LegLevelDifference { get; set; }`
  * `Boolean HasSupport { get; set; }`
  * `Double DurationSec { get; set; }`
  * `Double WeightEffortKg { get; set; }`
  * `Boolean BadContactArea { get; set; }`
  * `Boolean SimpleAttachment { get; set; }`
  * `Boolean SimpleFiling { get; set; }`
  * `Boolean BasicHoldHandStrike { get; set; }`
  * `Boolean WearingProtectiveGloves { get; set; }`
  * `Boolean HammerOrMalletSimpleUse { get; set; }`
  * `Boolean RepetitiveRotationsOfTheHandForearmElbow { get; set; }`
  * `Boolean StrongScrewing { get; set; }`
  * `Boolean ComplexHoldHandStrike { get; set; }`
  * `Boolean HammerOrMalletRepetitiveUse { get; set; }`
  * `Boolean RiskOfJointDiseases { get; set; }`
  * `Boolean AscentDescentObstacle { get; set; }`
  * `Boolean HandsAreFull { get; set; }`
  * `Boolean HandleForClimbing { get; set; }`
  * `Boolean BulkyLoad { get; set; }`
  * `Boolean Squatting { get; set; }`
  * `Boolean MovingToTheSide { get; set; }`
  * `Boolean MovingBack { get; set; }`
  * `Boolean MovingBackLessAndEqual { get; set; }`
  * `Boolean MovingBackGreater { get; set; }`
  * `Boolean KneelingOrSittingOnTheFloorOfTheCargoBed { get; set; }`
  * `Boolean MovementIsGreaterThen20mmin { get; set; }`

**Methods:**
  * `Void UpdateProperty(Object value, String propertyName)`
  * `Void UpdateContext(IEnumerable`1 links)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureAssessment.PostureAssessmentEvaluationResult
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `Int32 FinalLevel { get; set; }`
  * `IReadOnlyList`1 Details { get; set; }`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureAssessment.PostureAssessmentEvaluator
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Methods:**
  * `PostureAssessmentEvaluationResult Evaluate(PostureAssessmentContext context)`
  * `String DetermineTrunkBendCell(PostureAssessmentContext context)`
  * `String DetermineTrunkSideBendCell(PostureAssessmentContext context)`
  * `String DetermineTrunkTwistCell(PostureAssessmentContext context)`
  * `String DetermineHeadCell(PostureAssessmentContext context)`
  * `String DetermineArmCell(PostureAssessmentContext context, Char arm)`
  * `String DetermineWristCell(PostureAssessmentContext context, Char arm)`
  * `String DetermineLowerLimbsCell(PostureAssessmentContext context)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureAssessment.PostureAssessmentReport
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Methods:**
  * `ValueTuple`4 ReportAddPage(PdfDocument document)`
  * `Void AddTextBox(PdfDocument pdf, PdfPage page, Double x, Double y, Double width, Double height, String text)`
  * `Void DrawStringH1(PdfPage page, XTextFormatter tf, String text, Double& y)`
  * `Void DrawStringH2(PdfPage page, XTextFormatter tf, String text, Double& y)`
  * `Void DrawStringP(PdfPage page, XTextFormatter tf, String text, Double& y)`
  * `String WrapText(String text, Int32 maxLength)`
  * `Void DrawStringPageNumber(PdfPage page, XTextFormatter tf, Int32& pageNumber)`
  * `Void Header(PdfDocument document, DateTime dateTime, String operatorName)`
  * `Void AddPostureAssessmentTable(XGraphics gfx, BindableCollection`1 postureAssessmentCells, Double x, Double& y, Double width, Double height)`
  * `String PostureAssessmentCoefficientToString(PostureAssessmentCoefficient coefficient)`
  * `String BoolToMarker(Boolean value)`
  * `String PostureAssessmentContextPropertyToString(Boolean propertyValue, String propertyTextKey, Boolean tabulation)`
  * `Void Report(PdfDocument document, Int32 pageNumber, PostureAssessmentContext postureContext, BindableCollection`1 postureAssessmentCells, BindableCollection`1 postureAssessmentCoefficients, Int32 finalLevel)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureAssessment.PostureAssessmentView
- **Base Type:** System.Windows.Controls.UserControl
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient, IAddChild, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureAssessment.PostureAssessmentViewModel
- **Base Type:** Caliburn.Micro.Screen
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, IChild
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `BindableCollection`1 PostureAssessmentCells { get; set; }`
  * `BindableCollection`1 PostureAssessmentCoefficients { get; set; }`
  * `Int32 FinalLevel { get; set; }`
  * `Boolean HasSupport { get; set; }`
  * `Double DurationSec { get; set; }`
  * `Double EnteredWeightEffortKg { get; set; }`
  * `Double WeightEffortKg { get; set; }`
  * `Boolean BadContactArea { get; set; }`
  * `Boolean SimpleAttachment { get; set; }`
  * `Boolean SimpleFiling { get; set; }`
  * `Boolean BasicHoldHandStrike { get; set; }`
  * `Boolean WearingProtectiveGloves { get; set; }`
  * `Boolean HammerOrMalletSimpleUse { get; set; }`
  * `Boolean RepetitiveRotationsOfTheHandForearmElbow { get; set; }`
  * `Boolean StrongScrewing { get; set; }`
  * `Boolean ComplexHoldHandStrike { get; set; }`
  * `Boolean HammerOrMalletRepetitiveUse { get; set; }`
  * `Boolean RiskOfJointDiseases { get; set; }`
  * `Boolean AscentDescentObstacle { get; set; }`
  * `Boolean HandsAreFull { get; set; }`
  * `Boolean HandleForClimbing { get; set; }`
  * `Boolean BulkyLoad { get; set; }`
  * `Boolean Squatting { get; set; }`
  * `Boolean MovingToTheSide { get; set; }`
  * `Boolean MovingBack { get; set; }`
  * `Boolean MovingBackLessAndEqual { get; set; }`
  * `Boolean MovingBackGreater { get; set; }`
  * `Boolean KneelingOrSittingOnTheFloorOfTheCargoBed { get; set; }`
  * `Boolean MovementIsGreaterThen20mmin { get; set; }`

**Methods:**
  * `Void CreatePostureAssessmentCells()`
  * `String GetJSON(String jsonName)`
  * `Void GetPostureAssessmentReport()`
  * `Void AddCoefficient()`
  * `Void DeleteCoefficient(Object source)`
  * `Void PostureAssessmentCoefficient_PropertyChanged(Object sender, PropertyChangedEventArgs e)`
  * `Void CalculateWeightEffort()`
  * `PostureAssessmentEvaluationResult GetPostureAssessmentEvaluationResult()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureDictionaries
- **Base Type:** System.Object
- **Attributes:** NullableContextAttribute, NullableAttribute

**Methods:**
  * `List`1 GetPredefinedPosture(Int32 key)`
  * `List`1 GetStandardPosture(Int32 key)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureEditor.PostureEditorView
- **Base Type:** System.Windows.Controls.UserControl
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient, IAddChild, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.PostureEditor.PostureEditorViewModel
- **Base Type:** Caliburn.Micro.Screen
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, IChild
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `LinkManipulator LinkManipulator { get; set; }`
  * `Boolean IsLinkManipulatorVisible { get; set; }`
  * `Boolean ApplyColor { get; set; }`
  * `BindableCollection`1 PredefinedPostures { get; set; }`
  * `Int32 PredefinedPosturesSelectedIndex { get; set; }`
  * `String SelectedLinkName { get; set; }`
  * `IProperty SelectedLinkVectorProperty { get; set; }`

**Methods:**
  * `Void OnViewLoaded(Object view)`
  * `Void OnActivate()`
  * `Void OnDeactivate(Boolean close)`
  * `Void LocalizationService_LanguageChanged(Object sender, LanguageChangedEventArgs e)`
  * `Void ComboBoxGetText()`
  * `Void CreateLinkManipulator()`
  * `Void ToggleLinkManipulator(Object sender)`
  * `Void PredefinedPostures_SelectionChanged()`
  * `Void UpdatePredefinedPosture()`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.StandardPostures.StandardPosturesView
- **Base Type:** System.Windows.Controls.UserControl
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient, IAddChild, IComponentConnector

**Methods:**
  * `Void InitializeComponent()`
  * `Delegate _CreateDelegate(Type delegateType, String handler)`
  * `Void System.Windows.Markup.IComponentConnector.Connect(Int32 connectionId, Object target)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.StandardPostures.StandardPosturesViewModel
- **Base Type:** Caliburn.Micro.Screen
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, IChild
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `BindableCollection`1 StandardPostures { get; set; }`
  * `Int32 StandardPosturesSelectedIndex { get; set; }`
  * `Boolean KeepEyeDirection { get; set; }`
  * `Visibility SitStandardPostureVisibility { get; set; }`
  * `Visibility SquatStandardPostureVisibility { get; set; }`
  * `Visibility StoopStandardPostureVisibility { get; set; }`
  * `Visibility TwistStandardPostureVisibility { get; set; }`
  * `Visibility LeanStandardPostureVisibility { get; set; }`
  * `Visibility AdjustElbowStandardPostureVisibility { get; set; }`
  * `BindableCollection`1 TextBoxWithArrowsLines { get; set; }`

**Methods:**
  * `Void OnViewLoaded(Object view)`
  * `Void OnActivate()`
  * `Void OnDeactivate(Boolean close)`
  * `Void LocalizationService_LanguageChanged(Object sender, LanguageChangedEventArgs e)`
  * `Void ComboBoxGetText()`
  * `Void CollapseGrids()`
  * `Void ResetTextBoxWithArrowsLines()`
  * `Void UpdateStandardPosture_Click()`
  * `Void ValueChangedEvent(String index, RoutedPropertyChangedEventArgs`1 eventArgs)`
  * `Void SetStandardPosture()`
  * `Void UpdateStandardPosture(Int32 index, RoutedPropertyChangedEventArgs`1 eventArgs)`
  * `Void SetJoints(Double hipAngleDeg, Double kneeAngleDeg, Double trunkStoopAngleDeg, Double trunkTwistAngleDeg, Double trunkLeanAngleDeg, Double adjustElbowAngleDeg)`
  * `Double FullSpineCalc(Double pendingRotation, Char axis)`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsWPPAddon.ErgonomicsWPP.SubLink
- **Base Type:** System.Object
- **Interfaces:** INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute, DebuggerDisplayAttribute

**Properties:**
  * `Link Parent { get; set; }`
  * `Char Axis { get; set; }`
  * `ISimNode SimNode { get; set; }`
  * `IDoubleProperty DofDoubleProperty { get; set; }`
  * `Vector3D Direction { get; set; }`
  * `Vector3D ZeroAngleDirection { get; set; }`
  * `Boolean Fixed { get; set; }`
  * `Vector3D TransformedDirection { get; set; }`
  * `Vector3D TransformedZeroAngleDirection { get; set; }`
  * `Vector3D CurrentDirection { get; set; }`
  * `Double StartAngle { get; set; }`
  * `Double EndAngle { get; set; }`
  * `Double CurrentAngle { get; set; }`
  * `Color Color { get; set; }`

**Methods:**
  * `Void CalculateCurrentDirection(Double dofValueX, Double dofValueY, Double dofValueZ)`
  * `Void CalculateDirection(Double dofValueX, Double dofValueY, Double dofValueZ)`
  * `Void CalculateZeroAngleDirection(Double dofValueX, Double dofValueY, Double dofValueZ)`
  * `Void CalculateAngle(Double dofValueX, Double dofValueY, Double dofValueZ, Boolean rotateRequest)`
  * `Vector3D CalculateEulerProjectOnPlane(Double dofValueX, Double dofValueY, Double dofValueZ, Vector3D vector, Vector3D vectorUp, Boolean useProj)`
  * `ValueTuple`3 ParentSubLinksDofValues()`
  * `SubLink GetSibling(Char axis)`
  * `Double SignedAngleBetween(Vector3D zeroTransformed, Vector3D current, Func`2 signSelector)`
  * `Vector3D RotateAndNorm(Double rx, Double ry, Double rz, Vector3D v)`
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsWPPAddon.Ribbon.ActionItemErgonomicsWPP
- **Base Type:** RProSoftDigital1.UX.Shared.ActionItem
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IActionItem, IToolTip, IPartImportsSatisfiedNotification
- **Attributes:** NullableContextAttribute, NullableAttribute, ExportAttribute

**Methods:**
  * `Void Execute()`

---

### ErgonomicsWPPAddon.Ribbon.RibbonGroupErgonomics
- **Base Type:** RProSoftDigital1.UX.Ribbon.RibbonGroupBase
- **Interfaces:** INotifyPropertyChangedEx, INotifyPropertyChanged, IRibbonGroup
- **Attributes:** NullableContextAttribute, NullableAttribute, ExportAttribute

**Properties:**
  * `String Header { get; set; }`
  * `String Icon { get; set; }`
  * `String Id { get; set; }`

---

### ErgonomicsWPPAddon.Ribbon.SiteSetupErgonomicsWPP
- **Base Type:** System.Object
- **Interfaces:** IPlugin
- **Attributes:** ExportAttribute

**Methods:**
  * `Void Exit()`
  * `Void Initialize()`

---

### ErgonomicsWPPAddon.TextBoxWithArrows
- **Base Type:** System.Windows.Controls.Primitives.RangeBase
- **Interfaces:** IResource, IAnimatable, IInputElement, IFrameworkInputElement, ISupportInitialize, IHaveResources, IQueryAmbient
- **Attributes:** NullableContextAttribute, NullableAttribute, TemplatePartAttribute, TemplatePartAttribute

**Properties:**
  * `String Label { get; set; }`

**Methods:**
  * `Void OnApplyTemplate()`
  * `Void IncreaseButton_Click(Object sender, RoutedEventArgs e)`
  * `Void DecreaseButton_Click(Object sender, RoutedEventArgs e)`
  * `Object CoerceValue(DependencyObject d, Object baseValue)`
  * `Void OnMinMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)`

---

### ErgonomicsWPPAddon.TextBoxWithArrowsLine
- **Base Type:** System.Object
- **Interfaces:** INotifyPropertyChanged
- **Attributes:** NullableContextAttribute, NullableAttribute

**Properties:**
  * `String Label { get; set; }`
  * `Double Minimum { get; set; }`
  * `Double Maximum { get; set; }`
  * `Double Value { get; set; }`

**Methods:**
  * `Void NotifyPropertyChanged(String propertyName)`

---

### ErgonomicsWPPAddon.Vector3Extensions
- **Base Type:** System.Object
- **Attributes:** ExtensionAttribute

**Methods:**
  * `Vector3D ZeroSmall(Vector3D v, Double eps)`
  * `Vector3 ZeroSmall(Vector3 v, Double eps)`
  * `Vector3 ZeroSmall(Vector3 v, Double eps)`
  * `Vector3 Normalized(Vector3 v)`
  * `Vector3D Normalized(Vector3D v)`
  * `Vector3 ToLocal(Vector3 v, Vector3 n, Vector3 o, Vector3 a)`
  * `Double ScalarProjectionOnAxis(Vector3 v, Vector3 axis)`

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

### XamlGeneratedNamespace.GeneratedInternalTypeHelper
- **Base Type:** System.Windows.Markup.InternalTypeHelper
- **Attributes:** DebuggerNonUserCodeAttribute, GeneratedCodeAttribute, EditorBrowsableAttribute

**Methods:**
  * `Object CreateInstance(Type type, CultureInfo culture)`
  * `Object GetPropertyValue(PropertyInfo propertyInfo, Object target, CultureInfo culture)`
  * `Void SetPropertyValue(PropertyInfo propertyInfo, Object target, Object value, CultureInfo culture)`
  * `Delegate CreateDelegate(Type delegateType, Object target, String handler)`
  * `Void AddEventHandler(EventInfo eventInfo, Object target, Delegate handler)`

---

