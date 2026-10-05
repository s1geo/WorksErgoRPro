# -*- coding: utf-8 -*-
"""
Generate the Unified Master Knowledge Portal for WorksErgo R-Pro Edition.
Matches user screenshot media_1791193673325.png 1:1.
Integrates:
- 1002 CAD articles (Help_RP_ru, Help_RP, Python API, Ergonomics, WPP, MoCap)
- Work(s) Ergo User Guide (HTML & PDF)
- 28 Work(s) Ergo Video Analyses
- 70+ Scientific Sources, Monographs & Downloaded PDFs
- Datasets & MoCap (ANSUR II, MoCap 22.9k frames, OpenSim models)
- Binary Assemblies & MEF contracts
"""

import os
import json
import re

KNOWLEDGE_BASE_DIR = r"D:\Git\WorksErgoRPro\knowledge_base"
RPRO_ARTICLES_PATH = os.path.join(KNOWLEDGE_BASE_DIR, "03_cad_documentation_and_help", "rpro_articles.json")

def load_cad_articles():
    with open(RPRO_ARTICLES_PATH, "r", encoding="utf-8") as f:
        articles = json.load(f)
    
    # Normalize paths so they are relative to knowledge_base root
    cleaned = []
    for a in articles:
        url = a.get("url", "")
        # in rpro_articles.json, url is like "rpro_help_decompiled/Help_RP_ru/..."
        # From knowledge_base root, this is "03_cad_documentation_and_help/" + url
        rel_url = "03_cad_documentation_and_help/" + url
        
        cat = a.get("category", "")
        # Map category names for clean tabs
        tab = "CAD"
        badge = a.get("lang", "RU")
        
        if "Python API" in cat:
            tab = "Python API"
            badge = "PY"
        elif "R-Pro CAD Core Manual" in cat:
            tab = "Р-Про CAD RU"
            badge = "RU"
        elif "VC CAD Core Manual" in cat:
            tab = "CAD EN"
            badge = "EN"
        elif "Ergonomics Module" in cat:
            tab = "Эргономика Р-Про"
            badge = "ERGO"
        elif "Working Postures" in cat:
            tab = "Рабочие позы WPP"
            badge = "WPP"
        elif "Motion Capture" in cat:
            tab = "Захват движения MoCap"
            badge = "MOCAP"
            
        cleaned.append({
            "category": cat.upper(),
            "tab": tab,
            "title": a.get("title", ""),
            "url": rel_url,
            "badge": badge,
            "badge_type": "ru" if badge=="RU" else ("en" if badge=="EN" else "api"),
            "doc_type": "html",
            "snippet": a.get("snippet", "")
        })
    return cleaned

def get_worksergo_guide():
    return [
        {
            "category": "WORK(S) ERGO GUIDE",
            "tab": "Work(s) Ergo Guide",
            "title": "Интерактивное руководство пользователя Work(s) Ergo v0.1",
            "url": "03_cad_documentation_and_help/Temp_Help_Ergonomics_v0.1.html",
            "badge": "GUIDE",
            "badge_type": "guide",
            "doc_type": "html",
            "snippet": "Официальное интерактивное руководство к модулю Work(s) Ergo: принципы InteliPose, привязка кистей к объектам, фильтрация коллизий, расчет L5/S1, баланс центра масс CoM и интеграция в Visual Components."
        },
        {
            "category": "WORK(S) ERGO GUIDE",
            "tab": "Work(s) Ergo Guide",
            "title": "Work(s) User Manual v1.17 (Official 50-Page PDF)",
            "url": "05_textbooks_and_manuals/Works_User_Manual_v1.17.pdf",
            "badge": "PDF",
            "badge_type": "pdf",
            "doc_type": "pdf",
            "snippet": "Полное 50-страничное руководство пользователя Work(s) v1.17: антропометрия NHANES, 14 уравнений LM-MMH, метод Arm Force Field (AFF), HandPak 23 хватов, расчет компрессии L5/S1 и усталости Вейбулла."
        }
    ]

def get_videos():
    videos_data = [
        ("01", "1TK7Z2mKARo", "Floating Hands and MultiPose", "32s", "Интерактивное перетаскивание кистей рук (Floating Hands) с автоматическим решением обратной кинематики (IK). Запись ключевых рабочих поз в стек MultiPose для циклического эргономического анализа."),
        ("02", "0bh-dkYO8WU", "Copy and Paste", "28s", "Мгновенное копирование и дублирование настроенных манекенов DHM с сохранением антропометрии, суставных ограничений и привязанных нагрузок в разные рабочие ячейки."),
        ("03", "m6IJ2ifP5dE", "Obstructed Eye Gaze and Eye View", "35s", "Моделирование конуса видимости (Eye Gaze) оператора. Проверка прямой видимости дисплеев станков и скрытых зон с расчетом угла наклона шейного отдела позвоночника."),
        ("04", "bL7HhOwo4TE", "Task Types (Fast Switch)", "24s", "Быстрое переключение типов операций в 1 клик: подъем (Lift), опускание (Lower), толкание (Push), тяга (Pull), удержание (Hold) и перенос (Carry) с мгновенным пересчетом DCR."),
        ("05", "84i_Tnt5JCw", "Hand Tasks, Force and Direction", "33s", "Задание 3D-вектора внешних усилий на каждую кисть (величина силы в Н, направление X/Y/Z). Автоматический расчет пределов AFF (Arm Force Field) и HandPak."),
        ("06", "-5CGQ5sS9YQ", "Body Bracing", "29s", "Опора тела о технологическое оборудование (Body Bracing). Разгрузка поясничного отдела L5/S1 за счет дополнительной точки кинематического контакта (грудь, бедро, левая рука)."),
        ("07", "_vBcBMkdWd4", "Import Environment", "40s", "Импорт полигонального CAD-окружения (станки, столы, конвейеры) с автоматической генерацией бесколлизионных объемов и привязкой манекена."),
        ("08", "GnIaK98FOYQ", "Straight Legs", "26s", "Стиль подъема с прямыми ногами (Stoop). Фиксация коленей и наклон торса с демонстрацией пикового всплеска компрессии L5/S1 (>4000 Н) и выхода центра давления CofP."),
        ("09", "tiO1ZDzxMzM", "Batch Analysis", "21s", "Пакетный расчет эргономики сотен рабочих циклов в фоновом режиме с генерацией сводных отчетов по нормативам NIOSH, LM-MMH и ISO 11228."),
        ("10", "S9co7fxjUnI", "51 - Multiple DHM (Packing Cell)", "30s", "Синхронная работа группы манекенов в упаковочной ячейке. Анализ эргономической усталости бригады рабочих за 8-часовую смену."),
        ("11", "P6qJU_Np7VQ", "52 - Multiple DHM (Assembly Line)", "34s", "Анализ поточно-конвейерной линии сборки с множеством рабочих мест. Балансировка эргономической нагрузки между станциями."),
        ("12", "F1vVvvfaWwM", "53 - Task Types (Interactive Controller)", "25s", "Напольный 3D-контроллер (Floor Gizmo) для интерактивного перемещения позиции рабочего и высоты захвата груза."),
        ("13", "5AMS2BFSOBk", "54 - MultiPose (Door Opening)", "22s", "Анализ динамики открывания тяжелой створки шкафа: непрерывная смена поз от первого касания до полного распахивания двери."),
        ("14", "2g5-ASHl3l4", "55 - Embed Images and Videos", "30s", "Интеграция мультимедийных инструкций, эталонных фото рабочих поз и видеозаписей реальных операций прямо в 3D-окно Visual Components."),
        ("15", "owQOJ87rvuY", "56 - VC x Work(s) x item_v1", "20s", "Проектирование рабочего стола из алюминиевого профиля item. Валидация высоты рабочей поверхности под 5-й и 95-й перцентили рабочих."),
        ("16", "T7w0oYlxgG4", "57 - VC x Work(s) x item_v2", "22s", "Оптимизация зоны досягаемости полок и лотков на верстаке item с исключением опасных наклонов спины."),
        ("17", "H1i-9kuh4iY", "58 - Office Ergonomics", "23s", "Офисная эргономика: регулировка высоты стула, монитора и клавиатуры для предотвращения шейно-плечевого синдрома."),
        ("18", "bLwyQbVnxRo", "59 - Eliminate Candy-Wrapper Tests", "27s", "Отказ от субъективных чек-листов («фантиков от конфет») в пользу строгой биомеханики L5/S1, AFF и MAE."),
        ("19", "w1LHXrCcPt0", "60 - Teamwork in Office", "27s", "Командная работа в офисном пространстве: анализ коммуникационных зон и комфорта сотрудников."),
        ("20", "QPSrNR54sRE", "61 - VC x Work(s) x Bosch Rexroth", "16s", "Интеграция с каталогом модульных рабочих мест Bosch Rexroth: быстрая подгонка высоты подвеса пневмоинструмента."),
        ("21", "d1lEIRjtvj0", "62 - Detroit Industry Murals", "19s", "Историческая реконструкция труда рабочих на фресках Диего Риверы в Детройте: биомеханический аудит тяжелого ручного труда 1930-х."),
        ("22", "sRSOwTO9EYM", "63 - Experience Web", "24s", "Экспорт интерактивной 3D-модели манекена и эргономических графиков в веб-браузер для демонстрации заказчикам."),
        ("23", "Gi0wf8AqvCI", "64 - NVIDIA Omniverse-powered", "30s", "Фотореалистичный рендеринг эргономической симуляции через коннектор NVIDIA Omniverse с трассировкой лучей RTX."),
        ("24", "8v6QIcK6Gx8", "65 - Pose-to-Pose Animation", "10s", "Плавная интерполяция между ключевыми позами по закону минимального рывка (Minimum Jerk) без рывков конечностей."),
        ("25", "wuyDohBZ6tQ", "66 - Above Shoulder Correction", "21s", "Алгоритм коррекции работы выше уровня плеч (Rempel & Potvin 2022): резкое снижение допустимого усилия руки для защиты ротаторной манжеты."),
        ("26", "XkNFZ6PfLpo", "67 - LM-MMH", "21s", "Психофизический расчет 14 уравнений Liberty Mutual (Potvin et al. 2021): расчет допустимой массы для 75% и 90% популяции женщин и мужчин."),
        ("27", "5Y8_QzWxIRg", "68 - VR testing", "36s", "Тестирование эргономики в виртуальной реальности (VR): оператор в шлеме HTC Vive / Oculus выполняет операции, манекен повторяет движения в реальном времени."),
        ("28", "nleLRl6NM44", "Visual Components - Ergonomics Toolchain", "37s", "Сквозной инструментальный конвейер эргономики: от импорта планировки цеха до генерации сводного паспорта безопасности рабочего места.")
    ]
    
    entries = []
    for num, ytid, title, dur, desc in videos_data:
        entries.append({
            "category": "WORK(S) ERGO VIDEO ANALYSIS",
            "tab": "28 Демо-Видео",
            "title": f"Видео {num}: {title} ({dur})",
            "url": f"https://www.youtube.com/watch?v={ytid}",
            "badge": "VIDEO",
            "badge_type": "video",
            "doc_type": "video",
            "ytid": ytid,
            "snippet": f"[YouTube @idkfa3] {desc} Детальный анализ 3D-механики зафиксирован в MASTER_28_VIDEOS_ANALYSIS.md."
        })
    return entries

def get_scientific_sources():
    # Registry of key academic papers, textbooks, and downloaded PDFs
    sources = [
        # Downloaded PDFs
        {
            "author": "Waters, Putz-Anderson, Garg (1994)",
            "title": "Applications Manual for the Revised NIOSH Lifting Equation (PB94-110307)",
            "url": "01_scientific_papers/Waters_1994_Applications_Manual_Revised_NIOSH_Lifting_Equation.pdf",
            "badge": "PDF 4.0MB",
            "desc": "Официальное полное руководство NIOSH по расчету уравнения подъема: RWL = LC × HM × VM × DM × AM × FM × CM, расчет Lifting Index (LI), мультипликаторы для одноручного и составного подъема."
        },
        {
            "author": "NIOSH (1981)",
            "title": "Work Practices Guide for Manual Lifting (DHHS NIOSH 81-122)",
            "url": "01_scientific_papers/NIOSH_1981_Work_Practices_Guide_Manual_Lifting.pdf",
            "badge": "PDF 15.4MB",
            "desc": "Исторический нормативный отчет NIOSH 1981 года, установивший базовый порог компрессии на L5/S1 (Action Limit = 3400 Н, Maximum Permissible Limit = 6400 Н)."
        },
        {
            "author": "Dempster, W. T. (1955)",
            "title": "Space Requirements of the Seated Operator (WADC Technical Report 55-159)",
            "url": "01_scientific_papers/Dempster_1955_Space_Requirements_Seated_Operator.pdf",
            "badge": "PDF 17.8MB",
            "desc": "Фундаментальный кадаверный труд по масс-инерционным характеристикам звеньев тела человека: центры масс сегментов, суставные центры вращения, площади проекций и кинематика сидящего оператора."
        },
        {
            "author": "Grenier, T. M. (1991)",
            "title": "Hand Anthropometry of U.S. Army Personnel (Natick/TR-92/011)",
            "url": "01_scientific_papers/Grenier_1991_Hand_Anthropometry_US_Army.pdf",
            "badge": "PDF 44.9MB",
            "desc": "Исчерпывающее антропометрическое исследование параметров кисти и пальцев руки (длины фаланг, толщина суставов, ширина ладони, сила хватов) для военных и промышленных интерфейсов."
        },
        {
            "author": "Robinette, K. M. et al. (2002)",
            "title": "Civilian American and European Surface Anthropometry Resource (CAESAR Final Report)",
            "url": "01_scientific_papers/Robinette_2002_CAESAR_Final_Report.pdf",
            "badge": "PDF 5.5MB",
            "desc": "Итоговый отчет проекта 3D-сканирования тела CAESAR (4400 сканов, США и Европа): анатомические ориентиры, распределение объемов и параметризация 3D-мешей манекенов."
        },
        {
            "author": "de Leva, P. (1996)",
            "title": "Adjustments to Zatsiorsky-Seluyanov's Segment Inertia Parameters",
            "url": "01_scientific_papers/de_Leva_1996_Segment_Inertia_Parameters.pdf",
            "badge": "PDF 1.0MB",
            "desc": "Скорректированные масс-инерционные параметры сегментов тела человека относительно костных ориентиров (длины звеньев, радиусы инерции, центры масс для мужчин и женщин)."
        },
        {
            "author": "Potvin, J. R. et al. (2021)",
            "title": "The Liberty Mutual Manual Materials Handling (LM-MMH) Equations",
            "url": "01_scientific_papers/Potvin_2021_LM_MMH_Equations.pdf",
            "badge": "PDF 2.1MB",
            "desc": "14 непрерывных психофизических уравнений Liberty Mutual для операций подъема, опускания, толкания, тяги и переноски с расчетом допустимых усилий для 75% и 90% популяции рабочих."
        },
        {
            "author": "Potvin, J. R. (2012)",
            "title": "Predicting Maximum Acceptable Efforts (MAE) with a General Fatigue Model",
            "url": "01_scientific_papers/Potvin_2012_Maximum_Acceptable_Effort_MAE.pdf",
            "badge": "PDF 0.8MB",
            "desc": "Общая нелинейная модель мышечной выносливости MAE на основе коэффициента занятости (Duty Cycle): расчет падения максимальной силы при длительном статическом удержании нагрузки."
        },
        {
            "author": "Aristidou, A. & Lasenby, J. (2011)",
            "title": "FABRIK: A Fast, Iterative Solver for the Inverse Kinematics Problem",
            "url": "01_scientific_papers/Aristidou_2011_FABRIK_IK.pdf",
            "badge": "PDF 1.5MB",
            "desc": "Алгоритм прямой и обратной итеративной кинематики без матричных инверсий. Идеален для мгновенного решения позы конечностей DHM-манекена с суставными ограничениями."
        },
        {
            "author": "Buss, S. R. (2004)",
            "title": "Introduction to Inverse Kinematics with Damped Least Squares (DLS)",
            "url": "01_scientific_papers/Buss_2004_Inverse_Kinematics_DLS.pdf",
            "badge": "PDF 0.2MB",
            "desc": "Теория демпфированного метода наименьших квадратов (DLS) для кинематических цепей роботов и манекенов, предотвращающая бесконечные скорости и взрывы при приближении к сингулярностям."
        },
        {
            "author": "Delp, S. L. et al. (2007)",
            "title": "OpenSim: Open-Source Software to Create and Analyze Dynamic Simulations",
            "url": "01_scientific_papers/Delp_2007_OpenSim_Biomechanical_Software.pdf",
            "badge": "PDF 1.1MB",
            "desc": "Архитектура среды биомеханического моделирования OpenSim: многозвенные модели скелета, мышечно-сухожильные элементы Хилла и динамический расчет суставных моментов."
        },
        {
            "author": "Dvoretzky, A., Kiefer, J., Wolfowitz, J. (1956)",
            "title": "Asymptotic Minimax Character of the Sample Distribution Function (DKW Inequality)",
            "url": "01_scientific_papers/Dvoretzky_Kiefer_Wolfowitz_1956_DKW_Inequality.pdf",
            "badge": "PDF 2.7MB",
            "desc": "Фундаментальное математическое неравенство ДКВ, используемое в Work(s) для доказательства статистической достаточности подвыборки сгенерированных поз рабочего из генеральной совокупности."
        },
        {
            "author": "Flash, T. & Hogan, N. (1985)",
            "title": "The Coordination of Arm Movements: A Mathematically Confirmed Minimum Jerk Model",
            "url": "01_scientific_papers/Flash_Hogan_1985_Coordination_Arm_Movements_Min_Jerk.pdf",
            "badge": "PDF 1.6MB",
            "desc": "Модель минимального рывка ЦНС человека (минимизация третьей производной координаты d³x/dt³): формирование естественных плавных траекторий движения руки между точками."
        },
        {
            "author": "Loper, M. et al. (2015)",
            "title": "SMPL: A Skinned Multi-Person Linear Model",
            "url": "01_scientific_papers/Loper_2015_SMPL_Body_Model.pdf",
            "badge": "PDF 39.8MB",
            "desc": "Стандарт параметрического представления 3D-поверхности тела человека: линейное пространство формы (Shape Blend Shapes) и деформации позы (Pose Blend Shapes)."
        },
        {
            "author": "Pavlakos, G. et al. (2019)",
            "title": "Expressive Body Capture: 3D Hands, Face, and Body from a Single Image (SMPL-X)",
            "url": "01_scientific_papers/Pavlakos_2019_SMPL-X.pdf",
            "badge": "PDF 9.9MB",
            "desc": "Расширенная анатомическая 3D-модель SMPL-X: полная кинематическая артикуляция всех фаланг кистей рук, суставов шеи и лицевых ориентиров."
        },
        {
            "author": "Pan, J., Chitta, S., Manocha, D. (2012)",
            "title": "FCL: A General Purpose Library for Collision and Proximity Queries",
            "url": "01_scientific_papers/Pan_2012_FCL_Collision_Queries.pdf",
            "badge": "PDF 1.4MB",
            "desc": "Библиотека быстрого расчета коллизий и дистанций между полигональными сетками произвольной геометрии на основе иерархий ограничивающих объемов (BVH OBB/RSS)."
        },
        {
            "author": "Schulman, J. et al. (2014)",
            "title": "Motion Planning with Sequential Convex Optimization (TrajOpt)",
            "url": "01_scientific_papers/Schulman_2014_TrajOpt_Motion_Planning.pdf",
            "badge": "PDF 3.5MB",
            "desc": "Непрерывная оптимизация траекторий движения в кинематическом пространстве с гарантированным избеганием самопересечений и коллизий с оборудованием."
        },
        {
            "author": "Snook, S. H. (1978)",
            "title": "The Design of Manual Handling Tasks",
            "url": "01_scientific_papers/Snook_1978_Design_Manual_Handling_Tasks.pdf",
            "badge": "PDF 0.1MB",
            "desc": "Классическая работа Снука, заложившая психофизические таблицы допустимой массы груза (Maximum Acceptable Weight of Lift, MAWL) для промышленности."
        },
        {
            "author": "Todorov, E. & Jordan, M. I. (2002)",
            "title": "Optimal Feedback Control as a Theory of Motor Coordination",
            "url": "01_scientific_papers/Todorov_2002_Optimal_Feedback_Control.pdf",
            "badge": "PDF 0.1MB",
            "desc": "Теория оптимального управления с обратной связью моторной координации человека: вариативность суставов допускается в направлениях, не влияющих на выполнение задачи."
        },
        {
            "author": "USAF (1964)",
            "title": "Human Mechanics: Four Monographs Abridged (AMRL-TDR-64-102)",
            "url": "01_scientific_papers/USAF_1964_Human_Mechanics_Four_Monographs.pdf",
            "badge": "PDF 17.0MB",
            "desc": "Исторический свод четырех классических монографий по биомеханике человека: Брауне и Фишер (центры тяжести), Демпстер (кинематика конечностей), Контини."
        },
        {
            "author": "Gordon, C. C. et al. (2012)",
            "title": "2012 Anthropometric Survey of U.S. Army Personnel: Methods and Statistics (NATICK/TR-15/007)",
            "url": "05_textbooks_and_manuals/Gordon_2012_ANSUR_II_Anthropometric_Survey.pdf",
            "badge": "PDF 7.3MB",
            "desc": "Официальный том методологии и статистических таблиц ANSUR II: 93 биометрических измерения на 6068 военнослужащих с перцентильными таблицами."
        },
        {
            "author": "Hotzman, J. et al. (2011)",
            "title": "Measurer's Handbook: U.S. Army Anthropometric Survey (NATICK/TR-11/017)",
            "url": "05_textbooks_and_manuals/Hotzman_2011_ANSUR_Measurement_Methods.pdf",
            "badge": "PDF 5.2MB",
            "desc": "Иллюстрированное руководство по анатомическим точкам и методикам измерений 93 параметров тела в проекте ANSUR II."
        },
        {
            "author": "Fryar, C. D. et al. (2021)",
            "title": "Anthropometric Reference Data for Children and Adults: United States 2015–2018 (NHANES)",
            "url": "05_textbooks_and_manuals/Fryar_2021_NHANES_Anthropometric_Reference_Data.pdf",
            "badge": "PDF 1.3MB",
            "desc": "Гражданский нормативный справочник CDC/NCHS: распределение роста и массы тела взрослого населения США в возрасте 20-69 лет (базис антропометрии Work(s) Ergo)."
        },
        {
            "author": "Schimpl, M. et al. (2011)",
            "title": "Association between Walking Speed and Age in Healthy Individuals (PLoS ONE)",
            "url": "01_scientific_papers/Schimpl_2011_Walking_Speed_Age.pdf",
            "badge": "PDF 0.2MB",
            "desc": "Экспериментальное исследование скорости ходьбы в свободных условиях с помощью акселерометрии (эталонная скорость переноски грузов 1.25 м/с)."
        },
        {
            "author": "NIOSH (1993)",
            "title": "Implications of the Revised NIOSH Lifting Guide of 1991: A Field Study (DTIC ADA267036)",
            "url": "01_scientific_papers/NIOSH_1993_Implications_Revised_Lifting_Guide_Field_Study.pdf",
            "badge": "PDF 4.6MB",
            "desc": "Полевая валидация применимости уравнения подъема NIOSH в производственных условиях: корреляция Lifting Index с обращениями к врачу по поводу болей в спине."
        },

        # Detailed Monographs for Remaining Literature in Exhaustive Register
        {
            "author": "Chaffin, Andersson, Martin (2006)",
            "title": "Occupational Biomechanics (4th Edition) — Comprehensive Monograph",
            "url": "01_scientific_papers_and_datasets/EXHAUSTIVE_70_SOURCES_ANALYSIS_AND_EXTRACTION.md#15-chaffin-andersson-martin-2006",
            "badge": "MONO",
            "desc": "Библия производственной биомеханики: 2D/3D SSPP модели многозвенного тела, вычисление реакций L5/S1, анатомические моменты суставов, критерии предельной нагрузки 3400 Н."
        },
        {
            "author": "Jäger, Luttmann et al. (2023)",
            "title": "The Dortmund Lumbar Load Atlas (The 'Dortmund Approach') — Monograph",
            "url": "01_scientific_papers/Jager_2023_Dortmund_Lumbar_Load_Atlas.md",
            "badge": "MONO",
            "desc": "Мета-анализ 1192 кадаверных препаратов позвоночника: нормативные пределы прочности поясничных дисков US75% (4360 Н для женщин, 5210 Н для мужчин в возрасте 42 лет)."
        },
        {
            "author": "Brinckmann et al. (1988) / Potvin (2026)",
            "title": "Spinal Fatigue & Cumulative Damage Weibull Model — Monograph",
            "url": "01_scientific_papers/Brinckmann_1988_Potvin_2026_Spinal_Fatigue_Weibull.md",
            "badge": "MONO",
            "desc": "Математическая модель кумулятивного усталостного разрушения замыкательных пластинок позвонков LCF_CD: распределение Вейбулла, циклы до разрушения CtF = f(LCF/US)."
        },
        {
            "author": "LaDelfa & Potvin (2017)",
            "title": "The Arm Force Field (AFF) Method — 3D Upper Limb Strength Monograph",
            "url": "01_scientific_papers/LaDelfa_Potvin_2017_Arm_Force_Field_AFF.md",
            "badge": "MONO",
            "desc": "3D-поверхности максимальной силы руки во всей полусфере досягаемости на базе 13 460 измерений: расчет допустимой силы по положению кисти и направлению усилия."
        },
        {
            "author": "Potvin / HandPak (2017)",
            "title": "HandPak: 23 Hand Grip and Pinch Interfaces Monograph",
            "url": "01_scientific_papers/HandPak_Potvin_23_Grip_Interfaces.md",
            "badge": "MONO",
            "desc": "Биомеханика 23 типов силовых и щипковых захватов (Power Grip, Chuck Pinch, Lateral Pinch, Pulp Pinch) с поправками на перчатки и трение материалов."
        },
        {
            "author": "Featherstone (2008) / Kingma (1996)",
            "title": "Spatial Dynamics (F = ma) & Recursive Newton-Euler Algorithm (RNEA) Monograph",
            "url": "01_scientific_papers/Featherstone_2008_Kingma_1996_Spatial_Dynamics_RNEA.md",
            "badge": "MONO",
            "desc": "Пространственная динамика многозвенного тела: учет инерционных сил и ускорений подъема груза, дающих всплеск компрессии L5/S1 на 40-60% выше квазистатики."
        },
        {
            "author": "Dempster (1955) / Winter (2009)",
            "title": "Dempster-Winter Center of Mass (CoM) & Base of Support (BoS) Monograph",
            "url": "01_scientific_papers/Dempster_1955_Winter_2009_CoM_BoS_Squat_Kinematics.md",
            "badge": "MONO",
            "desc": "Истинный расчет равновесия: взвешенная сумма координат звеньев тела, 3 стиля подъема (Stoop, Semi-Squat, Deep Squat) и проекция CofP в базу стоп [-70, +180] мм."
        },
        {
            "author": "Snook & Ciriello (1991) / Potvin (2021)",
            "title": "Liberty Mutual MMH 14 Equations Mathematical Specification",
            "url": "01_scientific_papers/Snook_Ciriello_1991_Potvin_2021_LM_MMH_14_Equations.md",
            "badge": "MONO",
            "desc": "Полная сводка формул и коэффициентов 14 уравнений LM-MMH: расчет MAWL для подъема/опускания и начальных/поддерживающих сил толкания/тяги."
        },
        {
            "author": "Schaub et al. (2012)",
            "title": "European Assessment Worksheet (EAWS) Automotive Standard Specification",
            "url": "01_scientific_papers_and_datasets/EXHAUSTIVE_70_SOURCES_ANALYSIS_AND_EXTRACTION.md#41-schaub-caragnano-britzke-bruder-2012",
            "badge": "STD",
            "desc": "Отраслевой регламент автоконцернов (BMW, VW, Stellantis): шкала 0-25 (Зеленая), 26-50 (Желтая), >50 (Красная) для оценки физической нагрузки за рабочий такт."
        },
        {
            "author": "Occhipinti (1998)",
            "title": "OCRA (Occupational Repetitive Actions) Index & Checklist",
            "url": "01_scientific_papers_and_datasets/EXHAUSTIVE_70_SOURCES_ANALYSIS_AND_EXTRACTION.md#42-occhipinti-1998",
            "badge": "STD",
            "desc": "Международный стандарт ISO 11228-3 по оценке монотонных высокочастотных движений верхних конечностей: подсчет технический действий и факторов риска."
        },
        {
            "author": "McAtamney & Corlett (1993) / Hignett (2000)",
            "title": "RULA & REBA Ergonomic Assessment Matrices Specification",
            "url": "01_scientific_papers_and_datasets/EXHAUSTIVE_70_SOURCES_ANALYSIS_AND_EXTRACTION.md#43-mcatamney-corlett-1993",
            "badge": "STD",
            "desc": "Матричные шкалы быстрой оценки позы: RULA (руки, шея, торс) и REBA (все тело с учетом захвата груза и динамики) со шкалами уровней срочности вмешательства 1-4."
        },
        {
            "author": "ISO Standards (11228-1/2/3, 11226, 7250)",
            "title": "ISO Ergonomics Standards Portfolio (ISO 11228 / ISO 11226 / ГОСТ Р 56644)",
            "url": "01_scientific_papers_and_datasets/EXHAUSTIVE_70_SOURCES_ANALYSIS_AND_EXTRACTION.md#4-section-4-international-and-industry-standards",
            "badge": "STD",
            "desc": "Международные стандарты ручной обработки грузов, оценки статических поз и антропометрии для проектирования промышленного оборудования."
        },
        {
            "author": "Rempel & Potvin (2022)",
            "title": "Maximum Acceptable Arm Forces for Above-Shoulder Work",
            "url": "01_scientific_papers_and_datasets/EXHAUSTIVE_70_SOURCES_ANALYSIS_AND_EXTRACTION.md#27-rempel-potvin-2022",
            "badge": "MONO",
            "desc": "Критерии защиты ротаторной манжеты и плечевого сустава при работе руками выше уровня плеч (subacromial impingement prevention)."
        },
        {
            "author": "Crowninshield & Brand (1981) / Zajac (1989)",
            "title": "Musculoskeletal Optimization & Hill Muscle Mechanics Monograph",
            "url": "01_scientific_papers_and_datasets/EXHAUSTIVE_70_SOURCES_ANALYSIS_AND_EXTRACTION.md#31-crowninshield-brand-1981",
            "badge": "MONO",
            "desc": "Статическая оптимизация распределения суставных моментов по мышцам min sum(Fi/PCSA)² и трехэлементная феноменологическая модель мышцы Хилла."
        },
        {
            "author": "Gilbert, Johnson, Keerthi (1988) / van den Bergen (2001)",
            "title": "GJK & EPA Algorithms for Mesh Proximity & CAD Collision Detection",
            "url": "01_scientific_papers_and_datasets/EXHAUSTIVE_70_SOURCES_ANALYSIS_AND_EXTRACTION.md#51-gilbert-johnson-keerthi-1988",
            "badge": "MONO",
            "desc": "Алгоритмы вычисления расстояния между выпуклыми многогранниками (GJK) и глубины проникновения (EPA) для бесколлизионного позиционирования в CAD."
        }
    ]

    entries = []
    for s in sources:
        badge = s.get("badge", "PDF")
        badge_type = "pdf" if "PDF" in badge else ("mono" if "MONO" in badge else "std")
        entries.append({
            "category": "SCIENTIFIC LITERATURE & PAPERS",
            "tab": "70+ Научных Первоисточников",
            "title": f"{s['author']} — {s['title']}",
            "url": s["url"],
            "badge": badge,
            "badge_type": badge_type,
            "doc_type": "pdf" if s["url"].endswith(".pdf") else "md",
            "snippet": s["desc"]
        })
    return entries

def get_datasets():
    datasets = [
        {
            "name": "ANSUR II Male Anthropometric Public Database",
            "file": "02_datasets/ANSUR_II_MALE_Public.csv",
            "badge": "CSV 2.0MB",
            "desc": "Официальная база данных биометрических замеров военнослужащих армии США: 4 082 субъекта мужского пола, 93 стандартизированных антропометрических параметра (рост, масса, длины звеньев, обхваты)."
        },
        {
            "name": "ANSUR II Female Anthropometric Public Database",
            "file": "02_datasets/ANSUR_II_FEMALE_Public.csv",
            "badge": "CSV 1.0MB",
            "desc": "Официальная база данных биометрических замеров военнослужащих армии США: 1 986 субъектов женского пола, 93 стандартизированных антропометрических параметра."
        },
        {
            "name": "Axis Studio MoCap Perception Neuron Dataset",
            "file": "02_datasets/mocap_axis_studio",
            "badge": "BVH 22.9k",
            "desc": "Пользовательский массив захвата движения (MoCap) костюма Perception Neuron: 12 BVH-файлов, 22 907 кадров @ 96 Гц с реальными приседаниями, наклонами и подъемом грузов."
        },
        {
            "name": "OpenSim Rajagopal 2016 Musculoskeletal Model",
            "file": "02_datasets/Rajagopal2016.osim",
            "badge": "OSIM 875KB",
            "desc": "Эталонная скелетно-мышечная модель полного тела человека OpenSim: 80 степеней свободы, 138 мышечно-сухожильных пучков нижних конечностей для валидации моментов в суставах."
        },
        {
            "name": "OpenSim Rajagopal-Lai-Uhlrich 2023 Multi-Body Model",
            "file": "02_datasets/RajagopalLaiUhlrich2023.osim",
            "badge": "OSIM 923KB",
            "desc": "Модифицированная полноразмерная динамическая модель OpenSim 2023 года с улучшенной кинематикой поясничного отдела и коленных суставов."
        }
    ]
    entries = []
    for d in datasets:
        entries.append({
            "category": "DATASETS & MOCAP",
            "tab": "Датасеты & MoCap",
            "title": d["name"],
            "url": d["file"],
            "badge": d["badge"],
            "badge_type": "data",
            "doc_type": "data",
            "snippet": d["desc"]
        })
    return entries

def get_binaries_and_contracts():
    binaries = [
        {
            "name": "UX.Shared.dll — R-Pro v2.2.2 MEF UI Contracts",
            "file": "03_cad_documentation_and_help/RPRO_VS_VISUAL_COMPONENTS_DIFFERENCES_AND_DOCS.md#3-архитектура-плагинов-net-mef",
            "badge": "C# DLL",
            "desc": "Сборка базовых интерфейсов взаимодействия Р-Про v2.2.2: [Export(typeof(IPlugin))], [Export(typeof(IRibbonGroup))], ActionItem, DockableScreen. PublicKeyToken=null (без строгой подписи)."
        },
        {
            "name": "Plugin.Ergonomics.dll — Native R-Pro Ergonomics Assembly",
            "file": "03_cad_documentation_and_help/RPRO_VS_VISUAL_COMPONENTS_DIFFERENCES_AND_DOCS.md#6-декомпиляция-нативных-модулей-р-про",
            "badge": "C# DLL",
            "desc": "Декомпилированный базовый заводской модуль «Эргономика» Р-Про: дискретные таблицы RULA/REBA, примитивная кинематика ног. Образец инфраструктурных контрактов MEF."
        },
        {
            "name": "Plugin.ErgonomicsWPP.dll — Native Working Postures Assembly",
            "file": "03_cad_documentation_and_help/RPRO_VS_VISUAL_COMPONENTS_DIFFERENCES_AND_DOCS.md#6-декомпиляция-нативных-модулей-р-про",
            "badge": "C# DLL",
            "desc": "Декомпилированный модуль «Рабочие позы» Р-Про: оценка статических поз по ISO 11226, интеграция с таймлайном симуляции."
        },
        {
            "name": "VisualComponents.Create3D.dll & ISimComponent",
            "file": "03_cad_documentation_and_help/RPRO_VS_VISUAL_COMPONENTS_DIFFERENCES_AND_DOCS.md#7-модель-компонентов-ecat-rpro-vs-vcmx",
            "badge": "CAD CORE",
            "desc": "Контракты взаимодействия с геометрическим ядром Р-Про: управление деревом геометрии, суставами (ISimJoint), поведением скриптов Python и сериализация компонентов .rpro."
        }
    ]
    entries = []
    for b in binaries:
        entries.append({
            "category": "BINARY ASSEMBLIES & MEF API",
            "tab": "Бинарные Сборки & API",
            "title": b["name"],
            "url": b["file"],
            "badge": b["badge"],
            "badge_type": "api",
            "doc_type": "api",
            "snippet": b["desc"]
        })
    return entries

def build_portal():
    print("Loading CAD articles...")
    cad_articles = load_cad_articles()
    print(f"Loaded {len(cad_articles)} CAD articles.")
    
    print("Loading Work(s) Ergo Guides...")
    guides = get_worksergo_guide()
    
    print("Loading 28 Videos...")
    videos = get_videos()
    
    print("Loading Scientific Sources...")
    sources = get_scientific_sources()
    
    print("Loading Datasets...")
    datasets = get_datasets()
    
    print("Loading Binary Assemblies...")
    binaries = get_binaries_and_contracts()
    
    all_items = cad_articles + guides + videos + sources + datasets + binaries
    total_count = len(all_items)
    print(f"TOTAL ITEMS IN UNIFIED INDEX: {total_count}")
    
    # Save master JSON
    json_path = os.path.join(KNOWLEDGE_BASE_DIR, "master_knowledge_index.json")
    with open(json_path, "w", encoding="utf-8") as f:
        json.dump(all_items, f, ensure_ascii=False, indent=2)
    print(f"Saved master index to {json_path}")
    
    # Generate HTML content matching screenshot media_1791193673325.png
    html_template = """<!DOCTYPE html>
<html lang="ru">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>База Знаний и Справочник Р-Про, Visual Components & Work(s) Ergo</title>
<style>
:root {
  --bg-primary: #0f172a;
  --bg-secondary: #1e293b;
  --bg-card: #1e293b;
  --text-main: #f8fafc;
  --text-muted: #94a3b8;
  --accent: #38bdf8;
  --accent-hover: #0284c7;
  --border: #334155;
  --tag-bg: #0284c7;
  --tag-lang-ru: #059669;
  --tag-lang-en: #2563eb;
  --tag-video: #7c3aed;
  --tag-pdf: #d97706;
  --tag-data: #0891b2;
  --tag-api: #e11d48;
}
* { box-sizing: border-box; margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; }
body { background: var(--bg-primary); color: var(--text-main); padding: 24px; min-height: 100vh; }
.header { max-width: 1400px; margin: 0 auto 24px; text-align: center; }
.header h1 { font-size: 28px; font-weight: 700; margin-bottom: 8px; color: #fff; }
.header p { color: var(--text-muted); font-size: 15px; }

.search-container { max-width: 1400px; margin: 0 auto 20px; }
#searchInput {
  width: 100%; padding: 14px 20px; font-size: 16px; border-radius: 8px;
  border: 1px solid var(--border); background: var(--bg-secondary); color: #fff; outline: none;
  transition: all 0.2s;
}
#searchInput:focus { border-color: var(--accent); box-shadow: 0 0 0 3px rgba(56,189,248,0.25); }

.filter-tabs { max-width: 1400px; margin: 0 auto 20px; display: flex; gap: 8px; flex-wrap: wrap; }
.tab-btn {
  background: var(--bg-secondary); border: 1px solid var(--border); color: var(--text-muted);
  padding: 8px 14px; border-radius: 6px; cursor: pointer; font-size: 13px; font-weight: 500; transition: all 0.2s;
}
.tab-btn:hover { background: #273549; color: #fff; border-color: var(--accent); }
.tab-btn.active { background: var(--tag-bg); color: #fff; border-color: var(--accent); font-weight: 600; }

.stats { max-width: 1400px; margin: 0 auto 12px; color: var(--text-muted); font-size: 14px; display: flex; justify-content: space-between; }

.results-grid {
  max-width: 1400px; margin: 0 auto; display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr)); gap: 16px;
}
.card {
  background: var(--bg-card); border: 1px solid var(--border); border-radius: 8px;
  padding: 16px; display: flex; flex-direction: column; justify-content: space-between;
  transition: transform 0.15s, border-color 0.15s, box-shadow 0.15s;
}
.card:hover { transform: translateY(-2px); border-color: var(--accent); box-shadow: 0 4px 12px rgba(0,0,0,0.3); }
.card-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 8px; }
.card-category {
  font-size: 11px; text-transform: uppercase; letter-spacing: 0.5px;
  color: var(--accent); font-weight: 700;
}
.badge {
  font-size: 10px; font-weight: 700; padding: 2px 7px; border-radius: 4px;
  color: #fff; text-transform: uppercase; letter-spacing: 0.5px;
}
.badge-ru { background: var(--tag-lang-ru); }
.badge-en { background: var(--tag-lang-en); }
.badge-video { background: var(--tag-video); }
.badge-pdf { background: var(--tag-pdf); }
.badge-data { background: var(--tag-data); }
.badge-api { background: var(--tag-api); }
.badge-guide { background: #0284c7; }
.badge-mono { background: #8b5cf6; }
.badge-std { background: #10b981; }

.card-title { font-size: 16px; font-weight: 600; margin-bottom: 8px; color: #fff; line-height: 1.35; }
.card-snippet {
  font-size: 13px; color: var(--text-muted); line-height: 1.5; margin-bottom: 14px; flex: 1;
  display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; overflow: hidden;
}
.card-link {
  display: block; width: 100%; background: var(--tag-bg); color: #fff; text-decoration: none;
  padding: 9px 14px; border-radius: 6px; font-size: 13px; text-align: center; font-weight: 500;
  border: none; cursor: pointer; transition: background 0.2s;
}
.card-link:hover { background: var(--accent-hover); }

/* Document Modal / Reader */
.modal-overlay {
  position: fixed; top: 0; left: 0; width: 100vw; height: 100vh;
  background: rgba(15, 23, 42, 0.85); backdrop-filter: blur(4px);
  display: none; justify-content: center; align-items: center; z-index: 1000;
}
.modal-content {
  background: var(--bg-secondary); border: 1px solid var(--border); border-radius: 10px;
  width: 92vw; height: 90vh; max-width: 1400px; display: flex; flex-direction: column;
  overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.5);
}
.modal-header {
  padding: 14px 20px; background: #0f172a; border-bottom: 1px solid var(--border);
  display: flex; justify-content: space-between; align-items: center; gap: 12px;
}
.modal-title { font-size: 16px; font-weight: 600; color: #fff; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.modal-actions { display: flex; gap: 10px; align-items: center; }
.modal-btn {
  background: #334155; border: 1px solid var(--border); color: #fff;
  padding: 6px 12px; border-radius: 5px; font-size: 12px; cursor: pointer; text-decoration: none;
}
.modal-btn:hover { background: #475569; }
.modal-btn.primary { background: var(--tag-bg); border-color: var(--accent); }
.modal-btn.close-btn { background: #e11d48; border-color: #f43f5e; font-weight: bold; }
.modal-body { flex: 1; overflow: hidden; background: #1e293b; position: relative; }
.modal-iframe { width: 100%; height: 100%; border: none; background: #fff; }
.modal-viewer-text { padding: 24px; overflow-y: auto; height: 100%; font-size: 14px; line-height: 1.6; color: #cbd5e1; white-space: pre-wrap; font-family: Consolas, monospace; }
</style>
</head>
<body>

<div class="header">
  <h1>База Знаний и Справочник Р-Про, Visual Components & Work(s) Ergo</h1>
  <p>Локальный поисковый портал по всем 1000+ темам официальной документации (CAD, Python API, Эргономика, WPP, MoCap, Видео и Первоисточники)</p>
</div>

<div class="search-container">
  <input type="text" id="searchInput" placeholder="Поиск по API, классам, операциям, стандартам, видео и статьям (vcComponent, Snapping, L5/S1, RULA, REBA, WPP, Servo, Kinematics, AFF, Snook)..." autofocus>
</div>

<div class="filter-tabs" id="filterTabs">
  <button class="tab-btn active" data-cat="all">Все разделы (__TOTAL__)</button>
  <button class="tab-btn" data-cat="Python API">Python API (222)</button>
  <button class="tab-btn" data-cat="Р-Про CAD RU">Р-Про CAD RU (387)</button>
  <button class="tab-btn" data-cat="CAD EN">CAD EN (363)</button>
  <button class="tab-btn" data-cat="Эргономика Р-Про">Эргономика Р-Про (14)</button>
  <button class="tab-btn" data-cat="Рабочие позы WPP">Рабочие позы WPP (8)</button>
  <button class="tab-btn" data-cat="Захват движения MoCap">Захват движения MoCap (8)</button>
  <button class="tab-btn" data-cat="Work(s) Ergo Guide">Work(s) Ergo Guide (2)</button>
  <button class="tab-btn" data-cat="28 Демо-Видео">28 Демо-Видео (28)</button>
  <button class="tab-btn" data-cat="70+ Научных Первоисточников">70+ Научных Первоисточников (__SOURCES_COUNT__)</button>
  <button class="tab-btn" data-cat="Датасеты & MoCap">Датасеты & MoCap (5)</button>
  <button class="tab-btn" data-cat="Бинарные Сборки & API">Бинарные Сборки & API (4)</button>
</div>

<div class="stats" id="stats">
  <span id="matchCount">Загрузка индекса...</span>
  <span>Офлайн-индекс готов</span>
</div>

<div class="results-grid" id="resultsGrid"></div>

<!-- Document Reader Modal -->
<div class="modal-overlay" id="docModal">
  <div class="modal-content">
    <div class="modal-header">
      <div class="modal-title" id="modalDocTitle">Просмотр документа</div>
      <div class="modal-actions">
        <a id="modalExternalLink" href="#" target="_blank" class="modal-btn primary">Открыть в новой вкладке ↗</a>
        <button class="modal-btn close-btn" onclick="closeDoc()">✕ Закрыть</button>
      </div>
    </div>
    <div class="modal-body" id="modalBody">
      <iframe class="modal-iframe" id="modalIframe" src="about:blank"></iframe>
      <div class="modal-viewer-text" id="modalViewerText" style="display:none;"></div>
    </div>
  </div>
</div>

<script>
const articles = __ARTICLES_JSON__;

const searchInput = document.getElementById('searchInput');
const resultsGrid = document.getElementById('resultsGrid');
const matchCount = document.getElementById('matchCount');
const tabs = document.querySelectorAll('.tab-btn');
const docModal = document.getElementById('docModal');
const modalIframe = document.getElementById('modalIframe');
const modalViewerText = document.getElementById('modalViewerText');
const modalDocTitle = document.getElementById('modalDocTitle');
const modalExternalLink = document.getElementById('modalExternalLink');

let currentTab = 'all';

function render(items) {
  resultsGrid.innerHTML = '';
  matchCount.textContent = 'Найдено документов: ' + items.length + ' из ' + articles.length;
  
  const fragment = document.createDocumentFragment();
  const limit = Math.min(items.length, 120); // Render top 120 matches smoothly
  
  for (let i = 0; i < limit; i++) {
    const a = items[i];
    const card = document.createElement('div');
    card.className = 'card';
    
    let badgeClass = 'badge-' + (a.badge_type || 'ru');
    
    card.innerHTML = `
      <div>
        <div class="card-header">
          <span class="card-category">${a.category}</span>
          <span class="badge ${badgeClass}">${a.badge}</span>
        </div>
        <div class="card-title">${a.title}</div>
        <div class="card-snippet">${a.snippet}</div>
      </div>
      <button class="card-link" onclick="openDoc(${articles.indexOf(a)})">Открыть документ →</button>
    `;
    fragment.appendChild(card);
  }
  
  resultsGrid.appendChild(fragment);
  if (items.length > limit) {
    const moreNotice = document.createElement('div');
    moreNotice.style.gridColumn = '1 / -1';
    moreNotice.style.textAlign = 'center';
    moreNotice.style.padding = '20px';
    moreNotice.style.color = '#94a3b8';
    moreNotice.textContent = 'Показаны первые ' + limit + ' результатов. Уточните поисковый запрос для более точной фильтрации.';
    resultsGrid.appendChild(moreNotice);
  }
}

function filterDocs() {
  const q = searchInput.value.toLowerCase().trim();
  const filtered = articles.filter(a => {
    const matchCat = (currentTab === 'all') || (a.tab === currentTab);
    if (!matchCat) return false;
    if (!q) return true;
    return a.title.toLowerCase().includes(q) ||
           a.snippet.toLowerCase().includes(q) ||
           a.category.toLowerCase().includes(q) ||
           (a.url && a.url.toLowerCase().includes(q));
  });
  render(filtered);
}

function openDoc(idx) {
  const doc = articles[idx];
  if (!doc) return;
  
  modalDocTitle.textContent = doc.title;
  modalExternalLink.href = doc.url;
  
  if (doc.doc_type === 'video' && doc.ytid) {
    modalViewerText.style.display = 'none';
    modalIframe.style.display = 'block';
    modalIframe.src = 'https://www.youtube.com/embed/' + doc.ytid + '?autoplay=1';
    docModal.style.display = 'flex';
  } else if (doc.url.endsWith('.html') || doc.url.endsWith('.htm')) {
    modalViewerText.style.display = 'none';
    modalIframe.style.display = 'block';
    modalIframe.src = doc.url;
    docModal.style.display = 'flex';
  } else if (doc.url.endsWith('.pdf')) {
    modalViewerText.style.display = 'none';
    modalIframe.style.display = 'block';
    modalIframe.src = doc.url;
    docModal.style.display = 'flex';
  } else if (doc.url.endsWith('.md') || doc.url.endsWith('.csv') || doc.url.endsWith('.osim') || doc.doc_type === 'api') {
    modalIframe.style.display = 'none';
    modalViewerText.style.display = 'block';
    modalViewerText.textContent = 'Загрузка: ' + doc.url + '...\\n\\n' + doc.snippet;
    
    // Fetch and display content
    fetch(doc.url)
      .then(r => r.text())
      .then(t => {
        modalViewerText.textContent = t.slice(0, 100000); // Display up to 100KB
      })
      .catch(e => {
        modalViewerText.textContent = doc.title + '\\n\\n' + doc.snippet + '\\n\\nЛокальный путь к файлу: ' + doc.url;
      });
    docModal.style.display = 'flex';
  } else {
    // External link or general
    window.open(doc.url, '_blank');
  }
}

function closeDoc() {
  docModal.style.display = 'none';
  modalIframe.src = 'about:blank';
}

// Close on ESC
document.addEventListener('keydown', (e) => {
  if (e.key === 'Escape') closeDoc();
});

// Close when clicking modal backdrop
docModal.addEventListener('click', (e) => {
  if (e.target === docModal) closeDoc();
});

tabs.forEach(btn => {
  btn.addEventListener('click', () => {
    tabs.forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
    currentTab = btn.getAttribute('data-cat');
    filterDocs();
  });
});

searchInput.addEventListener('input', filterDocs);

// Initial render
filterDocs();
</script>
</body>
</html>
"""

    rendered_html = html_template.replace("__TOTAL__", str(total_count))
    rendered_html = rendered_html.replace("__SOURCES_COUNT__", str(len(sources)))
    rendered_html = rendered_html.replace("__ARTICLES_JSON__", json.dumps(all_items, ensure_ascii=False))

    # Write knowledge_base/index.html
    kb_index_path = os.path.join(KNOWLEDGE_BASE_DIR, "index.html")
    with open(kb_index_path, "w", encoding="utf-8") as f:
        f.write(rendered_html)
    print(f"Generated {kb_index_path} ({len(rendered_html)} bytes)")

    # Also update documentation_portal.html
    # In documentation_portal.html (inside 03_cad_documentation_and_help/), relative URLs need to be adjusted
    # by adding "../" to urls that are not in 03_cad_documentation_and_help/
    doc_portal_items = []
    for it in all_items:
        u = it["url"]
        new_u = u
        if u.startswith("03_cad_documentation_and_help/"):
            new_u = u.replace("03_cad_documentation_and_help/", "", 1)
        elif not u.startswith("http"):
            new_u = "../" + u
        it_copy = dict(it)
        it_copy["url"] = new_u
        doc_portal_items.append(it_copy)

    doc_portal_html = html_template.replace("__TOTAL__", str(total_count))
    doc_portal_html = doc_portal_html.replace("__SOURCES_COUNT__", str(len(sources)))
    doc_portal_html = doc_portal_html.replace("__ARTICLES_JSON__", json.dumps(doc_portal_items, ensure_ascii=False))

    doc_portal_path = os.path.join(KNOWLEDGE_BASE_DIR, "03_cad_documentation_and_help", "documentation_portal.html")
    with open(doc_portal_path, "w", encoding="utf-8") as f:
        f.write(doc_portal_html)
    print(f"Updated {doc_portal_path} ({len(doc_portal_html)} bytes)")

if __name__ == "__main__":
    build_portal()
