# Анализ Бинарных Сборок и Контрактов CAD (.NET / MEF / DLL)

> **Назначение каталога:** Полный репозиторий бинарных сборок (DLL), декомпилированных контрактов и архитектурных срезов для интеграции в Р-Про v2.2.2 и реверс-инжиниринга Work(s) Ergo.

---

## 1. Реестр бинарных файлов в `02_rpro_and_vc_binaries/`

| Файл | Размер | Источник | Роль в экосистеме |
| :--- | :---: | :--- | :--- |
| **`Plugin.WorksErgoAPI.dll`** | 6 066 480 байт | Оригинал Work(s) Ergo | Облачный клиент Visual Components (WebView2 + HTTP API) |
| **`Plugin.WorksErgoAPI_.dll`** | 46 384 байта | Оригинал Work(s) Ergo (backup) | Легковесный загрузчик интерфейса и учетных данных |
| **`Plugin.Ergonomics.dll`** | 551 936 байт | `D:\Apps\RProv222\` | Заводской модуль «Эргономика» Р-Про (RULA, REBA, ISO 11226) |
| **`Plugin.ErgonomicsWPP.dll`** | 644 096 байт | `D:\Apps\RProv222\` | Заводской модуль «Рабочие позы» Р-Про (базовая кинематика) |
| **`UX.Shared.dll`** | 5 999 408 байт | `D:\Apps\RProv222\` | Базовая системная сборка Р-Про (v4.5.0.0, unsigned, MEF, Ribbon, DockManager) |
| **`Plugin.WorksErgo.dll`** | 60 928 байт | Наша разработка | Наш 100% автономный офлайн-плагин для Р-Про v2.2.2 |

---

## 2. Результаты декомпиляции `Plugin.WorksErgoAPI.dll`

### 2.1. Ключевые классы и пространства имен:
* `WorksErgo.VisualComponents.Plugin`:
  - `WorksErgoAPI` : `IPlugin` — точка входа MEF плагина.
  - `WorksErgoAPICommand` : `ActionItem` — кнопка в ленте Visual Components.
  - `HelpErgonomicsView` : `System.Windows.Controls.UserControl` — WPF-представление окна справки на базе WebView2 (`Microsoft.Web.WebView2.Wpf`).
  - `HelpErgonomicsViewModel` — модель представления с сохранением куков и токенов сессии.
  - `HttpClientHandler`, `HttpMessageHandler` — HTTP-клиент связи с веб-сервером `https://worksergo.com/api/v1/`.

### 2.2. Вшитые структуры скелета (JSON Schema):
В бинарнике найдены константы иерархии суставов кистей рук:
```json
{
  "hand_l": ["index_01_l", "middle_01_l", "pinky_01_l", "ring_01_l", "thumb_01_l"],
  "hand_r": ["index_01_r", "middle_01_r", "pinky_01_r", "ring_01_r", "thumb_01_r"]
}
```
А также 4x4 матрицы трансформаций пальцев в нейтральном положении.

### 2.3. Фундаментальный вывод:
Оригинальный плагин Visual Components являлся **тонким клиентом к облаку**. Никаких расчетов L5/S1, MAE или RULA локально на процессоре пользователя он не производил. Наше ядро полностью автономно и производит расчеты локально.

---

## 3. Архитектура интеграции Р-Про v2.2.2 (`UX.Shared.dll`)

### 3.1. MEF (Managed Extensibility Framework) Каталог:
* При запуске `D:\Apps\RProv222\RPro.exe` создает `DirectoryCatalog` по корневой папке приложения и сканирует все `Plugin.*.dll`.
* Экспортные интерфейсы:
  * `[Export(typeof(IPlugin))]` — инициализация плагина (`OnAppInitialized`).
  * `[Export(typeof(IRibbonGroup))]` — наследуя `RibbonGroupBase`, регистрирует группу на ленте WPF.
  * `[Export(typeof(IActionItem))]` — наследуя `ActionItem`, создает кнопки с иконками SVG/PNG.
  * `[Export(typeof(IDockableScreen))]` — наследуя `DockableScreen`, регистрирует док-панели для Caliburn.Micro / Infragistics DockManager.

### 3.2. Требования к бинарной совместимости:
1. **Строгая подпись (Strong Name):** `UX.Shared.dll` в Р-Про v2.2.2 **НЕ подписана** (`PublicKeyToken=null`, версия сборки 4.5.0.0 / 1.0.1). Попытка сослаться на подписанную сборку из Visual Components 4.8+ вызывает фатальный сбой CLR `0x80131044`.
2. **Целевой фреймворк:** строго `.NET Framework 4.8`.
3. **Компиляция:** сборки компилируются напрямую через Roslyn C# / `csc.exe` против локального файла `D:\Apps\RProv222\UX.Shared.dll`.
