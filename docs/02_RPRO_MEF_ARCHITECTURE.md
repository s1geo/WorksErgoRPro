# 02. Архитектура Нативного Плагина Р-Про v2.2.2 (.NET Framework 4.8 / MEF)

> **Назначение документа:** Инженерное руководство по внутреннему устройству хост-оболочки Р-Про v2.2.2, контрактам Managed Extensibility Framework (MEF), привязке представлений Caliburn.Micro и правилам компиляции C#.

---

## 1. Контекст среды исполнения Р-Про v2.2.2

* **Оболочка:** Infragistics NetAdvantage WPF + Caliburn.Micro v1.4/v1.5.
* **CLR / Фреймворк:** .NET Framework 4.8 (x64).
* **Каталог ядра приложения:** `D:\Apps\RProv222\`
* **Критический нюанс сборок (`UX.Shared.dll`):**
  - В Р-Про v2.2.2 библиотека `UX.Shared.dll` является **неподписанной** (`PublicKeyToken=null`, версия 4.5.0.0 / 1.0.1).
  - Если плагин скомпилирован против Visual Components 4.8+ (`UX.Shared.dll` с сильной подписью `PublicKeyToken=31bc53bc7503b77a`), загрузчик CLR при запуске выбрасывает исключение:
    ```
    System.IO.FileLoadException: Could not load file or assembly 'UX.Shared, Version=4.10.2.0, Culture=neutral, PublicKeyToken=31bc53bc7503b77a'
    HRESULT: 0x80131044
    ```
  - **Решение:** Плагин `Plugin.WorksErgo.dll` компилируется строго против неподписанной `D:\Apps\RProv222\UX.Shared.dll` без сильной подписи (`/keyfile` не используется).

---

## 2. Точки расширения MEF (Managed Extensibility Framework)

При старте Р-Про выполняет сканирование всех файлов `Plugin.*.dll` и `UX.*.dll` в корне `D:\Apps\RProv222\` через `DirectoryCatalog`. Плагин экспортирует следующие 3 фундаментальных типа:

### 2.1. Инициализатор плагина: `IPlugin`
```csharp
[Export(typeof(IPlugin))]
public class WorksErgoPlugin : IPlugin
{
    public void Initialize() { ... }
    public void Exit() { ... }
}
```

### 2.2. Встраивание группы в ленту Ribbon: `IRibbonGroup`
Наследуется от `RibbonGroupBase`:
```csharp
[Export(typeof(IRibbonGroup))]
public class WorksErgoRibbonGroup : RibbonGroupBase
{
    public WorksErgoRibbonGroup()
    {
        Id = "WorksErgoRibbonGroup";
        Header = "Works Ergo";
        TabId = "VcTabHome"; // Вкладка 'Главная'
        GroupPosition = RibbonGroupPosition.Normal;
    }
}
```

### 2.3. Интерактивная кнопка ленты: `IActionItem`
Наследуется от `ActionItem`:
```csharp
[Export(typeof(IActionItem))]
public class WorksErgoActionItem : ActionItem
{
    public WorksErgoActionItem()
    {
        Id = "cmdOpenWorksErgoPane";
        Label = "Works Ergo";
        GroupId = "WorksErgoRibbonGroup";
    }

    public override void Execute()
    {
        // Докование панели через IoC контейнер Р-Про
        var winMgr = IoC.Get<IDockAwareWindowManager>();
        var paneVm = IoC.Get<WorksErgoPaneViewModel>();
        winMgr.ShowDockableScreen(paneVm);
    }
}
```

---

## 3. Архитектура док-панели (Caliburn.Micro DockableScreen)

1. **`WorksErgoPaneViewModel`:**
   - Наследуется от `DockableScreen` (`Caliburn.Micro`).
   - Назначает `DesiredPanePosition = (int)DesiredPaneLocation.DockedRight` (докование справа от 3D-сцены).
   - Содержит свойства для всех 7 осей биомеханики, углов ног, CofP, порогов и рекомендаций.
   - Метод `Recalculate()` запускает ядро `ErgonomicMathEngine.Evaluate()`.

2. **`WorksErgoPaneView`:**
   - Нативный `UserControl` WPF.
   - Построен с темной CAD-палитрой Slate (`#0f172a`, `#1e293b`), гармонирующей со стилем Р-Про.
   - Автоматически связывается с DataContext через соглашения об именовании Caliburn.Micro (`WorksErgoPaneViewModel` $\to$ `WorksErgoPaneView`).

---

## 4. Конвейер сборки и развертывания

```powershell
# build.ps1
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe" `
  /target:library `
  /out:"D:\Git\WorksErgoRPro\src\Plugin.WorksErgo\bin\Plugin.WorksErgo.dll" `
  /reference:"D:\Apps\RProv222\UX.Shared.dll" `
  /reference:"D:\Apps\RProv222\Caliburn.Micro.dll" `
  /reference:"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationCore.dll" `
  /reference:"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationFramework.dll" `
  /reference:"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\WindowsBase.dll" `
  /reference:"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Xaml.dll" `
  /recurse:"D:\Git\WorksErgoRPro\src\Plugin.WorksErgo\*.cs"
```

Команда `deploy.ps1` безопасно копирует скомпилированный файл в `D:\Apps\RProv222\Plugin.WorksErgo.dll`.
