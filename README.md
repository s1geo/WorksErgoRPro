# WorksErgo R-Pro Edition

> **Автономный нативный .NET CAD плагин промышленной эргономики для Р-Про v2.2.2**  
> 100% локальное исполнение (Air-Gapped / Zero-Cloud), эквивалент Work(s) Ergo (Visual Components).

---

## Архитектура проекта

```text
WorksErgoRPro/
├── src/
│   ├── Plugin.WorksErgo/      # Нативная сборка C# .NET Framework 4.8 / WPF
│   │   ├── Core/              # MEF экспорты (IPlugin, IRibbonGroup, ActionItem)
│   │   ├── Views/             # WPF XAML разметка панелей
│   │   ├── ViewModels/        # Caliburn.Micro контроллеры
│   │   ├── Biomechanics/      # Математические классы C# (L5/S1, MAE, LM-MMH)
│   │   └── Resources/         # Оригинальные 3D сетки манекенов (6 перцентилей)
│   ├── PythonEngine/          # Скриптовые мосты и Action Panel Р-Про
│   └── AISurrogate/           # Обучение суррогатных нейросетей (PyTorch/ONNX/SQLite)
├── tests/
│   ├── unit_math_tests/       # Юнит-тесты формул (Jäger, Potvin, LM-MMH)
│   ├── assembly_mef_tests/    # Тесты сборки DLL, CLR и MEF экспортов
│   ├── cad_integration_tests/ # Тесты 3D захвата сцены в Р-Про
│   └── ai_latency_tests/      # Стресс-тесты задержки ONNX (<0.2 мс / 60 FPS)
├── build.ps1                  # Скрипт сборки через csc.exe в 1 клик
└── deploy.ps1                 # Автоматическая доставка в D:\Apps\RProv222\
```

## Сборка и развертывание

```powershell
# 1. Запуск автоматической сборки
.\build.ps1

# 2. Прогон всех 4 уровней автотестов
.\tests\run_all_tests.ps1

# 3. Деплой плагина в Р-Про
.\deploy.ps1
```
