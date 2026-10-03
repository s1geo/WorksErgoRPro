# 01. Спецификация Биомеханического Ядра и Математические Модели (Works Ergo R-Pro)

> **Назначение документа:** Полное математическое описание, аналитические выводы, исходные формулы и научные первоисточники для всех 7 осей анализа эргономики в `Plugin.WorksErgo.dll`.

---

## 1. Сводная карта 7 осей эргономики Work(s) Ergo

| Ось | Критерий / Модель | Первоисточник | Входные параметры | Выходная метрика | Порог нормы ($DCR \le 0.85$) |
| :---: | :--- | :--- | :--- | :--- | :--- |
| **1** | Пиковая компрессия L5/S1 | Jäger (2023), Gelb et al. (1995) | Вылет $H$, высота $V$, масса $m$, антропометрия | Сила сжатия $F_{\text{comp}}$ (Н), DCR | $\le 3500\text{ Н}$ (Ж) / $\le 4270\text{ Н}$ (М) |
| **2** | Усталостное разрушение L5/S1 (LCFCD) | Brinckmann et al. (1988), Potvin & Agnew (2026) | $F_{\text{comp}}$, циклы/день $N$, предел прочности $U_s$ | Кумулятивный DCR | $DCR_{\text{cumul}} \le 1.0$ |
| **3** | Психофизический предел (LM-MMH 2021) | Potvin, Dixon, Snook et al. (2021) | $H, V$, частота $f$, дистанция $D$, пол | Допустимая масса (MAWL, кг), DCR | Нагрузка $\le$ MAWL ($DCR \le 0.85$) |
| **4** | Плечи и руки (AFF ANN) | La Delfa & Potvin (2017) | 3D координаты кисти, $H, V$, масса | Макс. сила рук (MAF, Н), DCR | Приложенная сила $\le$ MAF ($DCR \le 0.85$) |
| **5** | Дистальные мышцы кисти (HandPak) | Potvin (2012, 18 исследований) | Тип хвата (Power/Pinch), качество сцепки | Сила хвата (MAF, Н), DCR | Сила хвата $\le$ MAF ($DCR \le 0.85$) |
| **6** | Шейный отдел C7/T1 | Harms-Ringdahl & Schuldt (1988) | Наклон головы и шеи, масса головы | Момент в шее ($M_{\text{neck}}$, Нм), DCR | Момент $\le$ MAT ($DCR \le 0.85$) |
| **7** | Мышечная выносливость (Potvin MAE) | Potvin (2012) | Коэффициент загрузки (Duty Cycle, DC) | Коэффициент запаса MAE ($0\dots 1$) | Поправка к максимальной силе MVC |

---

## 2. Математический вывод формул

### 2.1. Пояснично-крестцовый отдел L5/S1 (Jäger 2023, Gelb 1995)

#### Расчетная схема (Free-Body Diagram):
Пояснично-крестцовый сустав $L5/S1$ рассматривается как шарнир с одной степенью свободы на сгибание/разгибание. Равновесие моментов относительно центра межпозвоночного диска:
$$\sum M_{L5/S1} = M_{\text{trunk}} + M_{\text{load}} - F_{\text{erector}} \cdot d_{\text{muscle}} = 0$$

Где:
* **Масса верхней части тела:** $m_{\text{upper}} = m_{\text{body}} \cdot 0.60$ (голова, руки, туловище).
* **Длина сегмента туловища:** $L_{\text{trunk}} = (\text{StatureCm} / 100) \cdot 0.28\text{ м}$.
* **Плечо центра масс туловища:**
  $$d_{\text{trunk}} = \max\left(0.04,\, L_{\text{trunk}} \cdot \sin(\theta_{\text{trunk}}) \cdot 0.68\right)$$
* **Момент от веса туловища:**
  $$M_{\text{trunk}} = m_{\text{upper}} \cdot g \cdot d_{\text{trunk}}$$
* **Момент от внешнего груза в руках:**
  $$M_{\text{load}} = m_{\text{load}} \cdot g \cdot H_{\text{reach}}$$
* **Плечо эквивалентной мышцы-разгибателя спины (*Erector Spinae*):**
  $$d_{\text{muscle}} = 0.0585\text{ м}\dots 0.060\text{ м} \quad (\approx 5.85\dots 6.0\text{ см})$$
  *(Верифицировано по эталону Work(s) Ergo v1.17, стр. 39: $M_{\text{result}} = 144.39\text{ Нм},\, F_{\text{comp}} = 2409\text{ Н} \implies d = 144.39 / 2409 = 0.0599\text{ м}$)*.

#### Сила сокращения мышц спины:
$$F_{\text{erector}} = \frac{M_{\text{trunk}} + M_{\text{load}}}{d_{\text{muscle}}}$$

#### Полная осевая сила сжатия диска $L5/S1$:
$$F_{\text{comp}} = F_{\text{erector}} + \left(m_{\text{upper}} \cdot g + m_{\text{load}} \cdot g\right) \cdot \cos(\theta_{\text{trunk}}) \cdot 0.25$$

#### Пороговые значения предела прочности диска (TLV, Jäger 2023):
* Женщины: $TLV = 3500\text{ Н}$ (для 42 лет: $3575\text{ Н}$).
* Мужчины: $TLV = 4270\text{ Н}$.
$$DCR_{\text{lumbar}} = \frac{F_{\text{comp}}}{TLV}$$

---

### 2.2. Кумулятивная усталость позвоночника (LCFCD Brinckmann 1988)

Модель основана на усталостных испытаниях 70 изолированных поясничных позвоночно-двигательных сегментов человека до разрушения при циклическом нагружении:
$$U_s = \frac{TLV}{0.82} \quad (\text{Предел статической прочности})$$
$$\text{Stress Ratio} = \frac{F_{\text{comp}}}{U_s}$$

* **Характеристическое число циклов до усталостного перелома замыкательной пластинки ($\text{CtF}$, 63.2% вероятность отказа):**
  $$\text{CtF} = 5000 \cdot \exp\left(-5.35 \cdot \left(\text{Stress Ratio} - 0.46\right)\right)$$
* **Дневной индекс кумулятивного повреждения:**
  $$DCR_{\text{cumul}} = \frac{N_{\text{cycles\_per\_day}}}{\text{CtF}}$$

---

### 2.3. Допустимое мышечное усилие (Potvin MAE 2012)

Коэффициент снижения силы мышц при циклической работе во времени:
$$\text{Duty Cycle (DC)} = \frac{t_{\text{effort}}}{t_{\text{shift}}} = \frac{N_{\text{cycles}} \cdot t_{\text{effective\_sec}}}{T_{\text{shift\_hours}} \cdot 3600}$$

#### Прямое уравнение Potvin (2012):
$$\text{MAE} = 1.0 - (\text{Duty Cycle})^{0.24}$$

* **Проверка по эталону Work(s) Ergo:**  
  При $DC = 0.0231$ (630 подъемов по 0.922 с за 7 часов):
  $$\text{MAE} = 1.0 - (0.0231)^{0.24} = \mathbf{0.596} \quad (\text{100\% точное совпадение со стр. 39})$$

---

### 2.4. Нижние конечности, кинематика и равновесие (Stoop / Squat / CofP)

#### Кинематика ноги (2 сегмента):
* Голень ($L_{\text{shin}} = 0.25 \cdot \text{StatureCm}$): соединяет лодыжку $(x_{\text{ankle}}, y_{\text{ankle}})$ и коленный сустав $(x_{\text{knee}}, y_{\text{knee}})$.
* Бедро ($L_{\text{thigh}} = 0.24 \cdot \text{StatureCm}$): соединяет колено и тазобедренный сустав $(x_{\text{hip}}, y_{\text{hip}})$.

#### Зависимость угла сгибания коленей и высоты таза от стиля:
1. **Stoop (Нагиб на прямых ногах):**
   $$\theta_{\text{knee}} = 5^\circ, \quad Z_{\text{hip}} = Z_{\text{standing}}, \quad \theta_{\text{trunk}} \approx \frac{\Delta V}{0.08} \cdot 8.5^\circ \le 82^\circ$$
2. **Semi-Squat (Полуприсед — естественный оптимум):**
   $$\theta_{\text{knee}} = \min(65^\circ, 5^\circ + \frac{\Delta V}{0.012}), \quad Z_{\text{hip}} = Z_{\text{standing}} - \frac{\theta_{\text{knee}}}{65^\circ} \cdot 0.25\text{ м}$$
3. **Deep Squat (Глубокий эргономичный присед):**
   $$\theta_{\text{knee}} = \min(105^\circ, 10^\circ + \frac{\Delta V}{0.007}), \quad Z_{\text{hip}} = Z_{\text{standing}} - \frac{\theta_{\text{knee}}}{105^\circ} \cdot 0.45\text{ м}, \quad \theta_{\text{trunk}} \le 30^\circ$$

#### Момент в коленном суставе:
$$M_{\text{knee}} = (m_{\text{body}} + m_{\text{load}}) \cdot g \cdot \left(L_{\text{thigh}} \cdot \sin(\theta_{\text{knee}}) \cdot 0.45\right)$$

#### Центр давления (Center of Pressure — CofP) и противовес таза:
При наклоне вперед таз рефлекторно отводится назад на величину $\Delta X_{\text{pelvis}} = 0.08 + L_{\text{thigh}} \sin(0.5 \theta_{\text{knee}}) + 0.35 d_{\text{trunk}}$, создавая удерживающий контр-момент массы нижней части тела:
$$M_{\text{pelvis\_counter}} = (0.35 \cdot m_{\text{body}} \cdot g) \cdot \Delta X_{\text{pelvis}}$$
$$x_{\text{CofP}} = \frac{M_{\text{trunk}} + M_{\text{load}} - M_{\text{pelvis\_counter}} - 0.20 \cdot M_{\text{knee}}}{(m_{\text{body}} + m_{\text{load}}) \cdot g}$$
* **Условие устойчивости (Base of Support — BoS):**
  $$-70\text{ мм (пятка)} \le x_{\text{CofP}} \le +180\text{ мм (носок)}$$
  При $x_{\text{CofP}} > +180\text{ мм}$ происходит потеря равновесия и падение вперед.

---

### 2.5. Толкание и тяга (Pushing & Pulling — Snook & Ciriello 1991 / LM-MMH 2021)

Для операций горизонтального перемещения тележек и контейнеров оцениваются две критические фазы:
1. **Начальное усилие (Initial Force):** преодоление инерции покоя и трения страгивания:
   * Базовый предел для 75% женской популяции: $\approx 200\text{ Н}$, мужской: $\approx 300\text{ Н}$.
   * Поправочные коэффициенты:
     $$F_{\text{init\_limit}} = F_{\text{base\_init}} \cdot \left(1.0 - 0.25 |V_{\text{handle}} - 1.0|\right) \cdot \left(1.0 - 0.035 \cdot \text{Freq}\right)$$
2. **Усилие поддержания движения (Sustained Force):** равномерное качение:
   * Базовый предел для 75% женской популяции: $\approx 110\text{ Н}$, мужской: $\approx 170\text{ Н}$.
   * Поправка на дистанцию ($D = 2.1\text{ м} \to 1.15, 7.5\text{ м} \to 1.00, 15\text{ м} \to 0.88, 30\text{ м} \to 0.78$):
     $$F_{\text{sust\_limit}} = F_{\text{base\_sust}} \cdot \left(1.0 - 0.25 |V_{\text{handle}} - 1.0|\right) \cdot \left(1.0 - 0.0075 (D - 7.5)\right) \cdot \left(1.0 - 0.035 \cdot \text{Freq}\right)$$
3. **Итоговый индекс:**
   $$DCR_{\text{push/pull}} = \max\left(\frac{F_{\text{init}}}{F_{\text{init\_limit}}}, \frac{F_{\text{sust}}}{F_{\text{sust\_limit}}}\right)$$

---

### 2.6. Переноска груза (Carrying — Snook & Ciriello 1991 / LM-MMH 2021)

Определяет максимально допустимую массу переносимого в руках груза (Maximum Acceptable Weight of Carry — MAWC):
* Базовый предел на дистанции $2.1\text{ м}$ (75% женщин): $\approx 14.5\text{ кг}$, мужчины: $\approx 22.0\text{ кг}$.
* Дистанционное затухание ($2.1\text{ м} \to 1.00, 4.3\text{ м} \to 0.90, 8.5\text{ м} \to 0.82, 15\text{ м} \to 0.74$):
  $$DM = \max(0.65, 1.0 - 0.02 \cdot (D_{\text{carry}} - 2.1))$$
* Итоговая грузоподъемность переноски:
  $$\text{MAWC} = \text{MAWC}_{\text{base}} \cdot DM \cdot FM \cdot HM$$
  $$DCR_{\text{carry}} = \frac{M_{\text{load}}}{\text{MAWC}}$$

---

### 2.7. Нормативные дискретные матрицы RULA и REBA

* **RULA (Rapid Upper Limb Assessment — McAtamney & Corlett 1993):**
  * Таблица A: Upper Arm $[1..6] \times$ Lower Arm $[1..3] \times$ Wrist $[1..4] \times$ Wrist Twist $[1..2] \to$ Постуральный балл A.
  * Балл C = Постуральный балл A + мышечная статичность + нагрузка ($<2\text{ кг} \to 0, 2..10\text{ кг} \to 1/2, >10\text{ кг} \to 3$).
  * Таблица B: Neck $[1..6] \times$ Trunk $[1..6] \times$ Legs $[1..2] \to$ Постуральный балл B.
  * Балл D = Постуральный балл B + мышечная статичность + нагрузка.
  * Таблица C: Балл C $[1..8] \times$ Балл D $[1..7] \to$ Итоговый Grand Score $[1..7]$ (Action Levels 1–4).
* **REBA (Rapid Entire Body Assessment — Hignett & McAtamney 2000):**
  * Таблица A: Trunk $[1..5] \times$ Neck $[1..3] \times$ Legs $[1..4] \to$ Балл A. Балл A' = Балл A + Load Score.
  * Таблица B: Upper Arm $[1..6] \times$ Lower Arm $[1..2] \times$ Wrist $[1..3] \to$ Балл B. Балл B' = Балл B + Coupling Score.
  * Таблица C: Балл A' $[1..12] \times$ Балл B' $[1..12] \to$ Балл C.
  * Итоговый REBA Grand Score = Балл C + Activity Score $[1..15]$ (Risk Levels 0–4).

---

### 2.8. Многоэтапный композитный анализ техпроцесса (Composite Job Analysis — Gibson & Potvin 2016)

Оценка сменного цикла, состоящего из $N$ подзадач (Lift $\to$ Carry $\to$ Place):
1. **Кумулятивное разрушение замыкательной пластинки L5/S1 по смене:**
   $$\text{LCFCD}_{\text{composite}} = \sum_{i=1}^{N} \frac{\text{Cycles}_i}{\text{CtF}_i} \le 1.00$$
2. **Суммарный фактор мышечной усталости (Duty Cycle):**
   $$\text{DutyCycle}_{\text{composite}} = \frac{\sum_{i=1}^{N} \text{Cycles}_i \cdot t_{\text{effort}, i}}{T_{\text{shift}} \cdot 3600}$$
   $$\text{Potvin MAE}_{\text{composite}} = 1.0 - (\text{DutyCycle}_{\text{composite}})^{0.24}$$
3. **Итоговый сменный DCR (Weakest Link):**
   $$DCR_{\text{composite}} = \max\left(\frac{\max_i(F_{\text{compression}, i})}{\text{TLV}}, \text{LCFCD}_{\text{composite}}, \max_i(DCR_i)\right)$$

