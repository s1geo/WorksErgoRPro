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

#### Центр давления (Center of Pressure — CofP):
$$x_{\text{CofP}} = \frac{M_{\text{trunk}} + M_{\text{load}} - 0.25 \cdot M_{\text{knee}}}{(m_{\text{body}} + m_{\text{load}}) \cdot g}$$
* **Условие устойчивости (Base of Support — BoS):**
  $$-70\text{ мм (пятка)} \le x_{\text{CofP}} \le +180\text{ мм (носок)}$$
  При $x_{\text{CofP}} > +180\text{ мм}$ происходит потеря равновесия и падение вперед.
