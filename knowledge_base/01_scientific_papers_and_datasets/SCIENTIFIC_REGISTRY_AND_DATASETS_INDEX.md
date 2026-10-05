# Реестр Научных Первоисточников, Датасетов и Биомеханических Стандартов

> **Назначение каталога:** Полный академический свод 36 первоисточников, открытых и локальных датасетов антропометрии/MoCap, математических моделей кинематики и стандартов эргономического аудита, заложенных в ядро WorksErgo R-Pro Edition.

---

## 1. Локальные и мировые датасеты (Datasets Index)

| Датасет | Объем / Формат | Локальный путь / Ссылка | Роль в проекте |
| :--- | :---: | :--- | :--- |
| **Axis Studio MoCap (Пользовательский)** | **12 BVH файлов, 22 907 кадров @ 96 Гц** | `D:\Задания, Уроки\Запись с датчиков\` | **Основная обучающая выборка:** реальные приседания, наклоны, перенос грузов с датчиков Perception Neuron для обучения ИИ-суррогата. |
| **ANSUR II (US Army Anthropometry)** | 6 068 3D-замеров (4082 M, 1986 F), 93 параметра | `knowledge_base/02_datasets/ANSUR_II_*.csv` | Параметризация 5-го, 50-го и 95-го перцентилей манекенов (длины сегментов, массы, обхваты). |
| **CAESAR (Civilian Anthropometry)** | 4 400 3D-сканов поверхности тела | В открытом доступе SAE International | Сетки тела и вариативность распределения жировой/мышечной массы. |
| **AMASS (SMPL-X MoCap Archive)** | 11 000+ движений в формате SMPL-X | Открытый академический архив Max Planck | Предварительное обучение кинематики суставов пальцев и позвоночника. |
| **OpenSim Rajagopal 2016 / 2023** | 80 степеней свободы, 138 мышц нижних конечностей (`.osim`) | `knowledge_base/02_datasets/Rajagopal*.osim` | Валидация моментов в коленных суставах при глубоком приседе (Deep Squat). |

---

## 2. Академический реестр 36 первоисточников по дисциплинам

### Дисциплина I: Вычислительная кинематика и оптимизация позы
1. **Aristidou & Lasenby (2011)** — *FABRIK: A fast, iterative solver for the Inverse Kinematics problem.* Оптимизация рук и ног без матричных сингулярностей.
2. **Buss (2004)** — *Introduction to Inverse Kinematics with Damped Least Squares (DLS).* Устойчивость кинематической цепи при сингулярных вылетах кистей.
3. **Schulman et al. (2014)** — *Motion Planning with Sequential Convex Optimization and TrajOpt.* Планирование траекторий движения без самопересечений.

### Дисциплина II: Биомеханика позвоночника и сжатие L5/S1
4. **Chaffin, Andersson & Martin (2006)** — *Occupational Biomechanics (4th Ed.).* 2D/3D SSPP модели пояснично-крестцового отдела позвоночника ($F_{\text{comp}} \le 3400\text{ Н}$).
5. **Potvin (2012)** — *Predicting Maximum Acceptable Efforts with a general fatigue model (MAE).* Пределы мышечной выносливости в зависимости от длительности удержания.
6. **Jäger, Luttmann et al. (2023)** — *The Dortmund Lumbar Load Atlas (The "Dortmund Approach").* Возрастные пределы прочности межпозвонковых дисков ($F_{\text{crit}}(age)$).
7. **Brinckmann et al. (1988)** — *Fatigue fracture of human lumbar vertebrae.* Усталостная прочность замыкательных пластинок позвонков при циклической нагрузке.

### Дисциплина III: Равновесие, центр масс (CoM) и площадь опоры (BoS)
8. **Dempster (1955)** — *Space requirements of the seated operator.* Масс-инерционные параметры сегментов тела человека.
9. **Winter (2009)** — *Biomechanics and Motor Control of Human Movement (4th Ed.).* Расчет проекции $X_{\text{CoM}}$ по формуле взвешенной суммы:
   $$X_{\text{CoM}} = \frac{\sum m_i X_i + m_{\text{load}} X_{\text{load}}}{\sum m_i + m_{\text{load}}}$$
   Критерий устойчивости: Центр давления $X_{\text{CofP}} \in [-70, +180]\text{ мм}$ относительно лодыжки.
10. **Kingma et al. (1996)** — *Estimation of lumbar sacral moments using spatial dynamics ($F=ma$).* Динамический всплеск компрессии на 40–60% выше статики при ускорении подъема груза.

### Дисциплина IV: Психофизика ручного перемещения тяжестей (MMH)
11. **Snook & Ciriello (1991)** / **Potvin (2021)** — *Liberty Mutual Manual Materials Handling (LM-MMH) Equations.* Предельные допустимые массы подъема (MAWL) и усилия толкания/тяги для 75% и 90% популяции.
12. **Waters, Putz-Anderson, Garg (1993)** — *Revised NIOSH Lifting Equation.* Рекомендуемый предел массы $RWL = LC \times HM \times VM \times DM \times AM \times FM \times CM$, индекс подъема $LI = \frac{L}{RWL}$.
13. **Potvin (2017) / HandPak** — *23 Hand Grip and Pinch Interfaces.* Классификация и силовые пределы 23 хватов кисти.
14. **LaDelfa & Potvin (2017)** — *Arm Force Field (AFF).* 3D-поверхности максимальной силы руки во всей полусфере досягаемости.

### Дисциплина V: Международные и промышленные стандарты эргономики
15. **ISO 11228-1:2021** — *Ergonomics: Manual handling — Part 1: Lifting and carrying.*
16. **ISO 11228-2:2007** — *Part 2: Pushing and pulling.*
17. **ISO 11228-3:2007** — *Part 3: Handling of low loads at high frequency.*
18. **ISO 11226:2000** — *Evaluation of static working postures.*
19. **EAWS (European Assessment Worksheet, Schaub 2012)** — Стандарт оценки физической нагрузки автоконцернов (BMW, VW, Stellantis, Daimler): шкала 0–25 (Зеленая), 26–50 (Желтая), >50 (Красная).
20. **ГОСТ Р 56644-2015** — *Эргономика ручной обработки грузов.*
21. **McAtamney & Corlett (1993)** — *RULA: Rapid Upper Limb Assessment.*
22. **Hignett & McAtamney (2000)** — *REBA: Rapid Entire Body Assessment.*

---

## 3. Математическое сведение индекса DCR (Demand/Capacity Ratio)

Каждый эргономический расчет нормализуется в единый безразмерный показатель нагрузки:
$$DCR = \frac{\text{Demand (Требуемая нагрузка)}}{\text{Capacity (Допустимая физиологическая емкость)}}$$

* **$DCR \le 0.85$ (ЗЕЛЕНЫЙ):** Нагрузка полностью безопасна для 75–90% рабочей популяции.
* **$0.85 < DCR \le 1.00$ (ЖЕЛТЫЙ):** Пограничная зона (рекомендуется оптимизация или чередование труда).
* **$DCR > 1.00$ (КРАСНЫЙ):** Недопустимая нагрузка (высокий риск травматизма L5/S1 или мышечного перенапряжения).
