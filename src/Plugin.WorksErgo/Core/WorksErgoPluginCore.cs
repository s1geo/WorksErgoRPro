using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using RProSoftDigital1.UX.Shared;
using RProSoftDigital1.UX.Ribbon;
using RProSoftDigital1.Create3D;
using WorksErgoRPro.ViewModels;
using WorksErgoRPro.Views;

namespace WorksErgoRPro.Core
{
    [Export(typeof(IPlugin))]
    public class WorksErgoPlugin : IPlugin
    {
        public void Initialize()
        {
            try
            {
                var uxConfig = IoC.Get<IUXConfiguration>();
                var commandRegistry = IoC.Get<ICommandRegistry>();
                var locService = IoC.Get<ILocalizationService>();

                if (uxConfig != null && commandRegistry != null)
                {
                    // 1. Register Dedicated Ribbon Site on Home Tab
                    uxConfig.RegisterSite("VcTabHome/WorksErgoGroup", "Work(s) Ergo", null, -1);

                    // 2. Register Action Item in Ribbon
                    var actionItem = commandRegistry.FindItem("WorksErgoActionItem");
                    if (actionItem == null)
                    {
                        actionItem = new WorksErgoActionItem(locService);
                    }

                    var setup = new UXSiteSetup
                    {
                        UXSiteIdPath = "VcTabHome/WorksErgoGroup",
                        UXSiteType = UXSiteType.RibbonGroup,
                        EntryId = actionItem.Id
                    };
                    commandRegistry.RegisterActionItem(actionItem, setup);

                    // 3. Register on Teach Tab as well
                    var setupTeach = new UXSiteSetup
                    {
                        UXSiteIdPath = "VcTabTeach/WorksErgoGroup",
                        UXSiteType = UXSiteType.RibbonGroup,
                        EntryId = actionItem.Id
                    };
                    uxConfig.RegisterSite("VcTabTeach/WorksErgoGroup", "Work(s) Ergo", null, -1);
                    commandRegistry.RegisterActionItem(actionItem, setupTeach);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WorksErgo] SiteSetup error: {ex.Message}");
            }
        }

        public void Exit()
        {
        }
    }

    [Export(typeof(IRibbonGroup))]
    public class WorksErgoRibbonGroup : RibbonGroupBase
    {
        public override string Header => "Works Ergo";
        public override string Id => "WorksErgoRibbonGroup";
        public override string Icon => null;

        [ImportingConstructor]
        public WorksErgoRibbonGroup(ILocalizationService localizationService) : base(localizationService)
        {
        }
    }

    [Export(typeof(IActionItem))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class WorksErgoActionItem : ActionItem
    {
        private readonly ILocalizationService _localizationService;
        private WorksErgoPaneViewModel _viewModel;
        private Window _floatingWindowFallback;

        [ImportingConstructor]
        public WorksErgoActionItem(ILocalizationService localizationService)
            : base("Works Ergo", "Works Ergo Offline Biomechanics & InteliPose Suite", null)
        {
            _localizationService = localizationService;
            RibbonId = "WorksErgoRibbonGroup";
            SiteOrder = 10.0;
        }

        public override void Execute()
        {
            try
            {
                if (_viewModel == null)
                {
                    _viewModel = new WorksErgoPaneViewModel();
                }

                // Attempt to dock into R-Pro's native Infragistics DockManager via Caliburn.Micro IoC
                IDockAwareWindowManager winManager = null;
                try
                {
                    winManager = IoC.Get<IDockAwareWindowManager>();
                }
                catch
                {
                    winManager = null;
                }

                if (winManager != null)
                {
                    winManager.ShowDockedWindow(
                        "WorksErgoPane",
                        _viewModel,
                        null,
                        true,
                        DesiredPaneLocation.DockedRight,
                        true,
                        Orientation.Vertical,
                        null
                    );
                }
                else
                {
                    // Fallback to native WPF floating tool window
                    if (_floatingWindowFallback == null || !_floatingWindowFallback.IsLoaded)
                    {
                        var view = new WorksErgoPaneView { DataContext = _viewModel };
                        _floatingWindowFallback = new Window
                        {
                            Title = "Works Ergo R-Pro Edition",
                            Content = view,
                            Width = 460,
                            Height = 780,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            ResizeMode = ResizeMode.CanResizeWithGrip
                        };
                        _floatingWindowFallback.Closed += (s, e) => _floatingWindowFallback = null;
                        _floatingWindowFallback.Show();
                    }
                    else
                    {
                        _floatingWindowFallback.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to launch Works Ergo panel:\n{ex.Message}\n\nStack:\n{ex.StackTrace}",
                    "Works Ergo - Initialization Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }
    }
}
