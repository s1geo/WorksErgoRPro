using System;
using System.ComponentModel.Composition;
using RProSoftDigital1.UX.Shared;
using RProSoftDigital1.UX.Ribbon;
using RProSoftDigital1.Create3D;

namespace WorksErgoRPro.Core
{
    [Export(typeof(IPlugin))]
    public class WorksErgoPlugin : IPlugin
    {
        public void Initialize()
        {
            // Initialized when R-Pro starts and MEF scans Plugin.*.dll
        }

        public void Exit()
        {
            // Cleanup on R-Pro exit
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

        [ImportingConstructor]
        public WorksErgoActionItem(ILocalizationService localizationService) 
            : base("Works Ergo", "Works Ergo Offline Biomechanics Suite", null)
        {
            _localizationService = localizationService;
            RibbonId = "WorksErgoRibbonGroup";
            SiteOrder = 10.0;
        }

        public override void Execute()
        {
            System.Windows.MessageBox.Show(
                "Works Ergo R-Pro Edition (Offline Biomechanics Engine)\nInitialized successfully!",
                "Works Ergo R-Pro",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information
            );
        }
    }
}
