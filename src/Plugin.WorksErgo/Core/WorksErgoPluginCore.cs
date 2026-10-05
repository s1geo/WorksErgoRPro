using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using RProSoftDigital1.Create3D;
using RProSoftDigital1.UX.Ribbon;
using RProSoftDigital1.UX.Shared;
using WorksErgoRPro.ViewModels;
using WorksErgoRPro.Views;

namespace WorksErgoRPro.Core
{
    [Export(typeof(IPlugin))]
    public class WorksErgoPlugin : IPlugin
    {
        public void Initialize()
        {
            var msger = IoC.Get<IMessageService>();
            try
            {
                // 1. Register assembly with Caliburn.Micro AssemblySource
                if (!AssemblySource.Instance.Contains(typeof(WorksErgoPaneViewModel).Assembly))
                {
                    AssemblySource.Instance.Add(typeof(WorksErgoPaneViewModel).Assembly);
                }

                // 2. Register ViewLocator rule to guarantee locating WorksErgoPaneView
                var defaultLocator = ViewLocator.LocateForModelType;
                ViewLocator.LocateForModelType = (modelType, displayLocation, context) =>
                {
                    if (modelType == typeof(WorksErgoPaneViewModel))
                    {
                        return new WorksErgoPaneView { DataContext = context };
                    }
                    return defaultLocator(modelType, displayLocation, context);
                };

                var uxConfig = IoC.Get<IUXConfiguration>();
                var commandRegistry = IoC.Get<ICommandRegistry>();
                var locService = IoC.Get<ILocalizationService>();

                if (uxConfig != null && commandRegistry != null)
                {
                    // Find or instantiate action item
                    var actionItem = commandRegistry.FindItem("WorksErgoActionItemId");
                    if (actionItem == null)
                    {
                        actionItem = new WorksErgoActionItem(locService);
                    }

                    // 3. Register site on VcTabHome (Home Tab)
                    var homeTab = uxConfig.Sites.Cast<UXSiteConfigElement>().FirstOrDefault(s => s.ItemId == "VcTabHome");
                    if (homeTab != null)
                    {
                        var homeSites = homeTab.UXSites;
                        var group = homeSites.Cast<UXSiteConfigElement>().FirstOrDefault(s => s.ItemId == "RibbonGroupWorksErgoId");
                        if (group == null)
                        {
                            uxConfig.RegisterSite("VcTabHome/RibbonGroupWorksErgoId", "Work(s) Ergo", "Ergonomics/ErgonomicsIcon", -1);
                        }
                        var setupHome = new UXSiteSetup
                        {
                            UXSiteIdPath = "VcTabHome/RibbonGroupWorksErgoId",
                            UXSiteType = UXSiteType.Menu,
                            EntryId = actionItem.Id
                        };
                        commandRegistry.RegisterActionItem(actionItem, setupHome, 0);
                    }

                    // 4. Register site on VcTabTeach (Teach Tab)
                    var teachTab = uxConfig.Sites.Cast<UXSiteConfigElement>().FirstOrDefault(s => s.ItemId == "VcTabTeach");
                    if (teachTab != null)
                    {
                        var teachSites = teachTab.UXSites;
                        var groupTeach = teachSites.Cast<UXSiteConfigElement>().FirstOrDefault(s => s.ItemId == "RibbonGroupWorksErgoId");
                        if (groupTeach == null)
                        {
                            uxConfig.RegisterSite("VcTabTeach/RibbonGroupWorksErgoId", "Work(s) Ergo", "Ergonomics/ErgonomicsIcon", -1);
                        }
                        var setupTeach = new UXSiteSetup
                        {
                            UXSiteIdPath = "VcTabTeach/RibbonGroupWorksErgoId",
                            UXSiteType = UXSiteType.Menu,
                            EntryId = actionItem.Id
                        };
                        commandRegistry.RegisterActionItem(actionItem, setupTeach, 0);
                    }

                    msger?.AppendMessage("[Work(s) Ergo] Модуль успешно зарегистрирован в ленте Р-Про (Вкладки 'Главная' и 'Обучение').", MessageLevel.Info);
                }
            }
            catch (Exception ex)
            {
                msger?.AppendMessage("[Work(s) Ergo] Ошибка инициализации плагина: " + ex.Message, MessageLevel.Error);
            }
        }

        public void Exit()
        {
        }
    }

    [Export(typeof(IRibbonGroup))]
    public class WorksErgoRibbonGroup : RibbonGroupBase
    {
        public override string Header => "Work(s) Ergo";
        public override string Id => "RibbonGroupWorksErgoId";
        public override string Icon => "Ergonomics/ErgonomicsIcon";

        [ImportingConstructor]
        public WorksErgoRibbonGroup(ILocalizationService localizationService) : base(localizationService)
        {
        }
    }

    [Export(typeof(IActionItem))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class WorksErgoActionItem : ActionItem
    {
        private readonly IDockAwareWindowManager _windowManager;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageService _messageService;
        private dynamic _paneContentPane = null;
        private WorksErgoPaneViewModel _viewModel = null;

        [ImportingConstructor]
        public WorksErgoActionItem(ILocalizationService localizationService)
            : base("WorksErgoActionItemId", "Work(s) Ergo", "Ergonomics/ErgonomicsIcon", null, "ButtonTool", false, false)
        {
            canExecute = () => true;
            ContextFilter = "None";
            EventMng?.Subscribe(this);
            ToolTipVisibility = Visibility.Visible;
            ToolTipHeader = "Work(s) Ergo";
            ToolTipContent = "Цифровая эргономика, InteliPose и биомеханика рабочих мест";
            _windowManager = IoC.Get<IDockAwareWindowManager>();
            _localizationService = localizationService;
            _messageService = IoC.Get<IMessageService>();
            RibbonId = "RibbonGroupWorksErgoId";
            SiteOrder = 1.0;
        }

        public override void Execute()
        {
            try
            {
                if (_viewModel == null)
                {
                    _viewModel = new WorksErgoPaneViewModel();
                }

                if (_paneContentPane == null)
                {
                    try
                    {
                        // 1. Показываем панель в DockManager
                        _windowManager.ShowDockedWindow(
                            "WorksErgoPaneId",
                            _viewModel,
                            null,
                            true,
                            DesiredPaneLocation.DockedRight,
                            true,
                            Orientation.Horizontal,
                            "1"
                        );

                        // 2. Извлекаем созданную ContentPane через DLR dynamic
                        dynamic winMgr = _windowManager;
                        dynamic dockMgr = winMgr.DockManager;
                        dynamic tempSplit = null;
                        foreach (dynamic p in dockMgr.Panes)
                        {
                            string pName = (string)p.Name;
                            if (pName != null && pName.Contains("WorksErgoPaneId"))
                            {
                                tempSplit = p;
                                break;
                            }
                        }

                        if (tempSplit != null && tempSplit.Panes.Count > 0)
                        {
                            _paneContentPane = tempSplit.Panes[0];
                            // CloseAction: 0 = PaneCloseAction.Hide (скрывать при клике на 'X', а не уничтожать!)
                            _paneContentPane.CloseAction = 0;
                            tempSplit.Panes.Remove(_paneContentPane);
                            dockMgr.Panes.Remove(tempSplit);

                            // 3. Встраиваем напрямую в нативный правый контейнер вкладок Р-Про
                            dynamic rightSplit = null;
                            foreach (dynamic p in dockMgr.Panes)
                            {
                                string pName = (string)p.Name;
                                if (pName == "VcDockedRightSplitPane")
                                {
                                    rightSplit = p;
                                    break;
                                }
                            }

                            if (rightSplit != null && rightSplit.Panes.Count > 0)
                            {
                                dynamic tabGroup = rightSplit.Panes[0];
                                tabGroup.Items.Add(_paneContentPane);
                                tabGroup.SelectedItem = _paneContentPane;
                            }
                        }

                        _messageService?.AppendMessage("[Work(s) Ergo] Панель успешно инициализирована и встроена в правую док-панель Р-Про.", MessageLevel.Info);
                        return;
                    }
                    catch (Exception dockEx)
                    {
                        _messageService?.AppendMessage("[Work(s) Ergo] Ошибка докинга в панель Р-Про: " + dockEx.Message, MessageLevel.Error);
                        return;
                    }
                }

                // 4. Повторный клик: плавный toggle видимости без падений
                if (((UIElement)_paneContentPane).Visibility == Visibility.Collapsed)
                {
                    ((UIElement)_paneContentPane).Visibility = Visibility.Visible;
                    if (_paneContentPane.Parent != null)
                    {
                        dynamic parent = _paneContentPane.Parent;
                        parent.SelectedItem = _paneContentPane;
                    }
                    _messageService?.AppendMessage("[Work(s) Ergo] Панель открыта.", MessageLevel.Info);
                }
                else
                {
                    ((UIElement)_paneContentPane).Visibility = Visibility.Collapsed;
                    _messageService?.AppendMessage("[Work(s) Ergo] Панель скрыта.", MessageLevel.Info);
                }
            }
            catch (Exception ex)
            {
                _messageService?.AppendMessage("[Work(s) Ergo] Ошибка при выполнении команды: " + ex.Message, MessageLevel.Error);
            }
        }
    }
}
