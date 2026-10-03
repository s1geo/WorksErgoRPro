using System;

namespace WorksErgoRPro.Biomechanics
{
    public enum DHMPercentile
    {
        Female5th,
        Female50th,
        Female95th,
        Male5th,
        Male50th,
        Male95th
    }

    public enum TaskType
    {
        LiftingLowering,
        PushingPulling,
        Carrying,
        StaticPosture
    }

    public enum CouplingQuality
    {
        Good,
        Fair,
        Poor
    }

    public enum HandGripType
    {
        PowerGripMedial,
        PowerGripLateral,
        PinchGrip,
        PalmPush
    }

    public enum LiftingTechnique
    {
        AutomaticSemiSquat,
        StoopStraightLegs,
        DeepSquat
    }

    public struct PostureInputs
    {
        public DHMPercentile Percentile;
        public TaskType Task;
        public LiftingTechnique Technique;    // Lifting style: SemiSquat (optimal), Stoop (straight legs), Squat (deep bend)
        public double LoadKg;                 // Total load mass at hands (kg)
        public double ReachMm;                // Horizontal distance from ankles to hand grip (mm)
        public double VerticalMm;             // Vertical height from floor to hand grip (mm)
        public double TravelMm;               // Vertical travel distance (mm)
        public double AsymmetryDeg;           // Asymmetry / twist angle (degrees)
        public double FrequencyLiftsPerMin;   // Frequency (lifts/min)
        public double DurationHours;          // Duration of continuous exposure (hours)
        public double ShiftDurationHours;     // Total shift duration (default 8.0 hours)
        public double FrequencyPerDay;        // Daily frequency (e.g. 630 /day)
        public double EffectiveDurationSec;   // Duration of single effort (s, default ~0.922 s)
        public CouplingQuality Coupling;
        public HandGripType Grip;
    }

    public struct ErgonomicOutputs
    {
        public double OverallDCR;
        public double LumbarCompressionN;     // Total L5/S1 bone-on-bone compression (N)
        public double LumbarShearN;           // L5/S1 anterior/posterior shear (N)
        public double LumbarDCR;              // Peak compression DCR vs Jäger 2023 TLV
        public double LumbarTLV_N;            // Peak TLV (3500-3575 N female / 4270 N male)
        public double CumulativeCompDCR;      // LCFCD fatigue damage DCR (Brinckmann Weibull)
        public double KneeFlexionDeg;         // Articulated knee flexion angle (degrees)
        public double TrunkFlexionDeg;        // Trunk forward lean angle (degrees)
        public double HipHeightMm;            // Pelvis vertical height above ground (mm)
        public double KneeMomentNm;           // Knee joint flexion moment (Nm)
        public double HipMomentNm;            // Mid-Hip joint resultant moment (Nm)
        public double CenterOfPressureMm;     // Net Center of Pressure (CofP) along anterior/posterior axis (mm)
        public bool IsBalanced;               // True if CofP is inside the foot Base of Support (BoS)
        public double NioshRWLKg;             // NIOSH Recommended Weight Limit (kg)
        public double NioshLI;                // NIOSH Lifting Index (Load / RWL)
        public double SnookMAWLKg;            // Liberty Mutual LM-MMH 2021 MAWL (kg)
        public double SnookDCR;               // LM-MMH DCR
        public double ArmStrengthMVC_N;       // Maximum manual arm strength MVC (N)
        public double ArmMAF_N;               // Maximum acceptable arm force MAF (N)
        public double ArmDCR;                 // Arm Force Field (AFF) DCR
        public double HandStrengthMVC_N;      // HandPak grip strength MVC (N)
        public double HandMAF_N;              // Maximum acceptable hand force (N)
        public double HandDCR;                // Hand/Wrist/Forearm DCR
        public double NeckMomentNm;           // Neck resultant moment (Nm)
        public double NeckMAT_Nm;             // Maximum acceptable neck torque (Nm)
        public double NeckDCR;                // Neck flexion DCR
        public double DutyCycle;              // Effective duty cycle fraction
        public double PotvinMAE;              // Potvin 2012 Maximum Acceptable Effort factor
        public int RulaScore;                 // RULA (1-7)
        public int RebaScore;                 // REBA (1-15)
        public string RiskCategory;           // "Low (Safe)", "Moderate (Warning)", "High (Hazard)"
        public string PrimaryLimitingFactor;
        public string Recommendation;
    }

    public static class ErgonomicMathEngine
    {
        // ISO 7250 / Work(s) Ergo reference anthropometry
        public static (double MassKg, double StatureCm, bool IsFemale) GetAnthropometry(DHMPercentile p)
        {
            switch (p)
            {
                case DHMPercentile.Female5th: return (50.0, 152.0, true);
                case DHMPercentile.Female50th: return (73.5, 160.0, true); // Ground truth Work(s) benchmark: 73.5 kg / 1.60 m
                case DHMPercentile.Female95th: return (89.0, 172.0, true);
                case DHMPercentile.Male5th: return (64.0, 165.0, false);
                case DHMPercentile.Male50th: return (78.0, 175.0, false);
                case DHMPercentile.Male95th: return (98.0, 187.0, false);
                default: return (78.0, 175.0, false);
            }
        }

        // 1. Potvin Maximum Acceptable Effort (MAE 2012)
        // Peer-reviewed formulation: MAE = 1 - (DC)^0.24
        public static double CalculatePotvinMAE(double dutyCycle)
        {
            double dc = Math.Max(0.0001, Math.Min(0.999, dutyCycle));
            double mae = 1.0 - Math.Pow(dc, 0.24);
            return Math.Round(Math.Max(0.05, Math.Min(1.0, mae)), 3);
        }

        // Calculate Duty Cycle from task parameters
        public static double CalculateDutyCycle(PostureInputs input)
        {
            double effortSec = input.EffectiveDurationSec > 0.0 ? input.EffectiveDurationSec : 0.922;

            if (input.FrequencyPerDay > 0.0)
            {
                double shiftHours = input.ShiftDurationHours > 0.0 ? input.ShiftDurationHours : 8.0;
                double totalShiftSec = shiftHours * 3600.0;
                double totalEffortSec = input.FrequencyPerDay * effortSec;
                return Math.Round(totalEffortSec / totalShiftSec, 4);
            }

            double freqPerMin = Math.Max(0.1, input.FrequencyLiftsPerMin);
            double dc = (freqPerMin * effortSec) / 60.0;
            return Math.Round(Math.Max(0.001, Math.Min(0.99, dc)), 4);
        }

    public struct LumbarAndLowerLimbResult
    {
        public double CompN;
        public double ShearN;
        public double DCR;
        public double TLV;
        public double KneeFlexionDeg;
        public double TrunkFlexionDeg;
        public double HipHeightMm;
        public double KneeMomentNm;
        public double HipMomentNm;
        public double CenterOfPressureMm;
        public bool IsBalanced;
    }

        // 2. Lumbar Spine L5/S1 & Articulated Lower Limb Kinematics (Jäger 2023, Gelb 1995, Work(s) Ergo)
        public static LumbarAndLowerLimbResult CalculateLumbarL5S1(PostureInputs input)
        {
            var anthro = GetAnthropometry(input.Percentile);
            double bodyMass = anthro.MassKg;
            double upperMass = bodyMass * 0.60; // 60% of total mass above L5/S1
            double g = 9.81;

            // Geometry and Moment Arms (meters)
            double hMeters = Math.Max(0.15, Math.Min(0.90, input.ReachMm / 1000.0));

            // Lower limb kinematics and hip height based on lifting technique
            double standingHipHeightM = (anthro.StatureCm / 100.0) * 0.53;
            double vM = input.VerticalMm / 1000.0;
            double vDiff = Math.Max(0.0, standingHipHeightM - vM);

            double kneeFlexionDeg = 0.0;
            double hipHeightM = standingHipHeightM;
            double forwardLeanDeg = 0.0;

            switch (input.Technique)
            {
                case LiftingTechnique.StoopStraightLegs:
                    kneeFlexionDeg = 5.0; // Straight legs
                    hipHeightM = standingHipHeightM;
                    forwardLeanDeg = Math.Max(0.0, Math.Min(82.0, (vDiff / 0.08) * 8.5));
                    break;

                case LiftingTechnique.DeepSquat:
                    // Knees bend deeply, pelvis drops down, spine stays upright
                    kneeFlexionDeg = Math.Max(10.0, Math.Min(105.0, (vDiff / 0.007)));
                    hipHeightM = Math.Max(0.35, standingHipHeightM - (kneeFlexionDeg / 105.0) * 0.45);
                    forwardLeanDeg = Math.Max(5.0, Math.Min(35.0, (vDiff / 0.025)));
                    break;

                case LiftingTechnique.AutomaticSemiSquat:
                default:
                    // Natural physiological compromise
                    kneeFlexionDeg = Math.Max(5.0, Math.Min(65.0, (vDiff / 0.012)));
                    hipHeightM = Math.Max(0.45, standingHipHeightM - (kneeFlexionDeg / 65.0) * 0.25);
                    forwardLeanDeg = Math.Max(0.0, Math.Min(74.0, (vDiff / 0.08) * 7.8));
                    break;
            }

            double thetaRad = forwardLeanDeg * (Math.PI / 180.0);

            // Trunk segment length and center-of-mass moment arm
            double trunkLengthM = (anthro.StatureCm / 100.0) * 0.28;
            double trunkMomentArm = trunkLengthM * Math.Sin(thetaRad) * 0.68;
            trunkMomentArm = Math.Max(0.04, trunkMomentArm);

            // External Resultant Flexion Moment (Nm) at L5/S1
            double trunkTorque = upperMass * g * trunkMomentArm;
            double loadTorque = (input.LoadKg * g) * hMeters;
            double resultantMoment = trunkTorque + loadTorque;

            // Single-equivalent erector moment arm estimate (Jäger / Gelb / Potvin ~ 0.0585-0.060 m)
            double momentArmMuscle = 0.0585;

            // Erector spinae muscle force
            double erectorForce = resultantMoment / momentArmMuscle;

            // Reaction forces
            double upperPlusLoadForce = (upperMass * g) + (input.LoadKg * g);
            double reactionComp = upperPlusLoadForce * Math.Cos(thetaRad) * 0.25;
            double reactionShear = upperPlusLoadForce * Math.Sin(thetaRad) * 0.65;

            // Total bone-on-bone compression force
            double totalCompressionN = erectorForce + reactionComp;
            double totalShearN = Math.Max(120.0, reactionShear);

            // Knee and Mid-Hip joint moments
            double kneeAngleRad = kneeFlexionDeg * (Math.PI / 180.0);
            double thighLengthM = (anthro.StatureCm / 100.0) * 0.24;
            double kneeMomentNm = (bodyMass * g + input.LoadKg * g) * (thighLengthM * Math.Sin(kneeAngleRad) * 0.45);
            double midHipMomentNm = resultantMoment + (bodyMass * 0.14 * g * 0.10);

            // Center of Pressure (CofP) along anterior/posterior axis from ankle center
            double totalWeightN = (bodyMass + input.LoadKg) * g;
            double netHorizontalMoment = trunkTorque + loadTorque - (kneeMomentNm * 0.25);
            double cofpM = netHorizontalMoment / totalWeightN;
            double cofpMm = Math.Round(cofpM * 1000.0, 1);
            // Base of Support (BoS): heels at -70 mm, toes at +180 mm relative to ankle center
            bool isBalanced = (cofpMm >= -70.0 && cofpMm <= 180.0);

            // Jäger (2023) TLV: 3500 N / 3575 N female, 4270 N male
            double tlv = anthro.IsFemale ? 3500.0 : 4270.0;
            double dcr = totalCompressionN / tlv;

            return new LumbarAndLowerLimbResult
            {
                CompN = Math.Round(totalCompressionN, 1),
                ShearN = Math.Round(totalShearN, 1),
                DCR = Math.Round(dcr, 3),
                TLV = tlv,
                KneeFlexionDeg = Math.Round(kneeFlexionDeg, 1),
                TrunkFlexionDeg = Math.Round(forwardLeanDeg, 1),
                HipHeightMm = Math.Round(hipHeightM * 1000.0, 1),
                KneeMomentNm = Math.Round(kneeMomentNm, 1),
                HipMomentNm = Math.Round(midHipMomentNm, 1),
                CenterOfPressureMm = cofpMm,
                IsBalanced = isBalanced
            };
        }

        // 3. Brinckmann / Potvin & Agnew (2026) Cumulative Compression Fatigue DCR
        public static double CalculateCumulativeCompression(double compressionN, double tlvN, double cyclesPerDay)
        {
            double us = tlvN / 0.82; // Ultimate strength
            double ratio = compressionN / us;

            if (ratio < 0.45) return Math.Round(Math.Min(0.20, (cyclesPerDay / 10000.0) * 0.1), 3);

            // Weibull characteristic fatigue life (63.2% failure probability)
            // fitted to Brinckmann et al. (1988) 70 lumbar joints
            double ctf = 5000.0 * Math.Exp(-5.35 * (ratio - 0.46));
            ctf = Math.Max(50.0, ctf);

            double cumulDcr = cyclesPerDay / ctf;
            return Math.Round(Math.Max(0.05, Math.Min(5.0, cumulDcr)), 3);
        }

        // 4. NIOSH Revised Lifting Equation (1991/2021)
        public static (double RWL, double LI) CalculateNIOSH(PostureInputs input)
        {
            double lc = 23.0; // Load Constant (kg)

            double hCm = Math.Max(25.0, Math.Min(63.0, input.ReachMm / 10.0));
            double hm = 25.0 / hCm;

            double vCm = Math.Max(0.0, Math.Min(175.0, input.VerticalMm / 10.0));
            double vm = Math.Max(0.0, 1.0 - 0.003 * Math.Abs(vCm - 75.0));

            double dCm = Math.Max(25.0, Math.Min(200.0, input.TravelMm / 10.0));
            double dm = Math.Max(0.0, Math.Min(1.0, 0.82 + (4.5 / dCm)));

            double aDeg = Math.Max(0.0, Math.Min(135.0, input.AsymmetryDeg));
            double am = Math.Max(0.0, 1.0 - 0.0032 * aDeg);

            double freq = input.FrequencyLiftsPerMin;
            if (freq <= 0.0 && input.FrequencyPerDay > 0.0)
            {
                double shiftHours = input.ShiftDurationHours > 0.0 ? input.ShiftDurationHours : 8.0;
                freq = input.FrequencyPerDay / (shiftHours * 60.0);
            }

            double fm = 1.0;
            if (freq <= 0.2) fm = 1.0;
            else if (freq <= 1.0) fm = 0.94;
            else if (freq <= 2.0) fm = 0.91;
            else if (freq <= 4.0) fm = 0.84;
            else if (freq <= 6.0) fm = 0.75;
            else if (freq <= 9.0) fm = 0.52;
            else if (freq <= 12.0) fm = 0.37;
            else fm = 0.20;

            if (input.DurationHours > 2.0 || input.ShiftDurationHours > 2.0) fm *= 0.85;

            double cm = 1.0;
            switch (input.Coupling)
            {
                case CouplingQuality.Good: cm = 1.00; break;
                case CouplingQuality.Fair: cm = 0.95; break;
                case CouplingQuality.Poor: cm = 0.90; break;
            }

            double rwl = lc * hm * vm * dm * am * fm * cm;
            rwl = Math.Max(0.1, rwl);
            double li = input.LoadKg / rwl;

            return (Math.Round(rwl, 2), Math.Round(li, 3));
        }

        // 5. Liberty Mutual MMH Equations (Potvin et al. 2021)
        public static (double MAL_N, double MAWL_Kg, double DCR) CalculateLMMMH(PostureInputs input)
        {
            var anthro = GetAnthropometry(input.Percentile);
            bool isFemale = anthro.IsFemale;

            // Lift MAL for 75% female / 90% male population (Work(s) benchmark MAL = 127.04 N)
            double baseMalN = isFemale ? 148.0 : 225.0;

            double hM = input.ReachMm / 1000.0;
            double reachFactor = Math.Max(0.55, 1.0 - 0.70 * (hM - 0.25));

            double freq = input.FrequencyLiftsPerMin;
            if (freq <= 0.0 && input.FrequencyPerDay > 0.0)
            {
                double shiftHours = input.ShiftDurationHours > 0.0 ? input.ShiftDurationHours : 8.0;
                freq = input.FrequencyPerDay / (shiftHours * 60.0);
            }
            double freqFactor = Math.Max(0.40, 1.0 - 0.045 * freq);

            double vM = input.VerticalMm / 1000.0;
            double vFactor = Math.Max(0.70, 1.0 - 0.25 * Math.Abs(vM - 0.75));

            double malN = baseMalN * reachFactor * freqFactor * vFactor;
            if (input.DurationHours > 4.0 || input.ShiftDurationHours > 4.0) malN *= 0.90;

            malN = Math.Max(20.0, malN);
            double mawlKg = malN / 9.81;

            double appliedForceN = input.LoadKg * 9.81;
            double dcr = appliedForceN / malN;

            return (Math.Round(malN, 1), Math.Round(mawlKg, 2), Math.Round(dcr, 3));
        }

        // 6. Arm Force Field (AFF) Strength & Capacity (La Delfa & Potvin 2017)
        public static (double ArmMVC_N, double ArmMAF_N, double ArmDCR) CalculateAFF(PostureInputs input, double mae)
        {
            var anthro = GetAnthropometry(input.Percentile);
            bool isFemale = anthro.IsFemale;

            // Baseline maximum manual arm strength (160.5 N female from Page 39 benchmark)
            double baseArmMvcFemale = 160.5;
            double armMvcN = isFemale ? baseArmMvcFemale : (baseArmMvcFemale * 1.60);

            // Reach factor: 300 mm vs 650 mm max reach
            double hM = input.ReachMm / 1000.0;
            if (hM > 0.45)
            {
                armMvcN /= (hM / 0.45);
            }

            double armMafN = armMvcN * mae;
            armMafN = Math.Max(10.0, armMafN);

            double forcePerArmN = (input.LoadKg * 9.81) / 2.0;
            double armDcr = forcePerArmN / armMafN;

            return (Math.Round(armMvcN, 1), Math.Round(armMafN, 1), Math.Round(armDcr, 3));
        }

        // 7. HandPak Distal Upper Extremity Grip & Pinch Capacity
        public static (double HandMVC_N, double HandMAF_N, double HandDCR) CalculateHandPak(PostureInputs input, double mae)
        {
            var anthro = GetAnthropometry(input.Percentile);
            bool isFemale = anthro.IsFemale;

            // Power Grip Medial Grasp MVC (Work(s) benchmark: 135.2 N female)
            double baseHandMvc = isFemale ? 135.2 : 216.0;

            if (input.Grip == HandGripType.PinchGrip) baseHandMvc *= 0.35;
            if (input.Coupling == CouplingQuality.Poor) baseHandMvc *= 0.85;

            double handMafN = baseHandMvc * mae;
            handMafN = Math.Max(10.0, handMafN);

            double forcePerHandN = (input.LoadKg * 9.81) / 2.0;
            double handDcr = forcePerHandN / handMafN;

            return (Math.Round(baseHandMvc, 1), Math.Round(handMafN, 1), Math.Round(handDcr, 3));
        }

        // 8. Neck Demands (Harms-Ringdahl & Schuldt 1988, Potvin 2012)
        public static (double NeckMomentNm, double NeckMAT_Nm, double NeckDCR) CalculateNeck(PostureInputs input, double mae)
        {
            var anthro = GetAnthropometry(input.Percentile);
            bool isFemale = anthro.IsFemale;

            double headMassKg = anthro.MassKg * 0.07;
            double g = 9.81;

            double forwardLeanDeg = Math.Max(0.0, Math.Min(75.0, (1750.0 * 0.53 - input.VerticalMm) / 8.0));
            double neckAngleRad = (forwardLeanDeg * 0.5) * (Math.PI / 180.0);
            double momentArmM = 0.06 + 0.12 * Math.Sin(neckAngleRad);

            double neckMomentNm = (headMassKg * g) * momentArmM;

            // Maximum Acceptable Torque: 26.1 Nm female (Page 39 benchmark)
            double baseNeckMvc = isFemale ? 26.1 : 41.8;
            double neckMatNm = baseNeckMvc * mae;
            neckMatNm = Math.Max(5.0, neckMatNm);

            double neckDcr = neckMomentNm / neckMatNm;
            return (Math.Round(neckMomentNm, 2), Math.Round(neckMatNm, 2), Math.Round(neckDcr, 3));
        }

        // Comprehensive 7-Axis Evaluation Pipeline
        public static ErgonomicOutputs Evaluate(PostureInputs input)
        {
            double dutyCycle = CalculateDutyCycle(input);
            double mae = CalculatePotvinMAE(dutyCycle);

            var lumbar = CalculateLumbarL5S1(input);
            double dailyCycles = input.FrequencyPerDay > 0.0 ? input.FrequencyPerDay : (input.FrequencyLiftsPerMin * 60.0 * input.DurationHours);
            double cumulCompDcr = CalculateCumulativeCompression(lumbar.CompN, lumbar.TLV, dailyCycles);

            var niosh = CalculateNIOSH(input);
            var lmmmh = CalculateLMMMH(input);
            var aff = CalculateAFF(input, mae);
            var hand = CalculateHandPak(input, mae);
            var neck = CalculateNeck(input, mae);

            // Overall DCR represents the weakest link across all 7 axes
            double overallDcr = Math.Max(lumbar.DCR,
                                Math.Max(cumulCompDcr,
                                Math.Max(lmmmh.DCR,
                                Math.Max(aff.ArmDCR,
                                Math.Max(hand.HandDCR, neck.NeckDCR)))));
            overallDcr = Math.Round(overallDcr, 3);

            string riskCategory;
            if (overallDcr <= 0.85) riskCategory = "Low (Safe)";
            else if (overallDcr <= 1.00) riskCategory = "Moderate (Warning)";
            else riskCategory = "High (Hazard)";

            // Primary limiting factor
            string primaryFactor = "Lumbar Spine L5/S1 Compression";
            double maxVal = lumbar.DCR;

            if (cumulCompDcr > maxVal) { maxVal = cumulCompDcr; primaryFactor = "Lumbar Cumulative Damage (LCFCD)"; }
            if (lmmmh.DCR > maxVal) { maxVal = lmmmh.DCR; primaryFactor = "Liberty Mutual MMH Population Capacity"; }
            if (aff.ArmDCR > maxVal) { maxVal = aff.ArmDCR; primaryFactor = "Shoulder/Arm Strength (AFF ANN)"; }
            if (hand.HandDCR > maxVal) { maxVal = hand.HandDCR; primaryFactor = "HandPak Grip/Pinch Fatigue"; }
            if (neck.NeckDCR > maxVal) { maxVal = neck.NeckDCR; primaryFactor = "Cervical Spine Neck Moment"; }

            // Actionable engineering recommendations
            string recommendation;
            if (overallDcr <= 0.85)
            {
                recommendation = "Task parameters are within safe ergonomic limits. No engineering redesign required.";
            }
            else if (input.VerticalMm < 550.0)
            {
                recommendation = $"Raise pickup point by {Math.Round(800.0 - input.VerticalMm)} mm (using scissor lift or pallet riser) to eliminate deep trunk flexion.";
            }
            else if (input.ReachMm > 450.0)
            {
                recommendation = $"Bring load {Math.Round(input.ReachMm - 300.0)} mm closer to body center to reduce lumbar torque and arm strain.";
            }
            else if (input.LoadKg > (lmmmh.MAWL_Kg))
            {
                recommendation = $"Reduce unit package weight from {input.LoadKg:F1} kg to \u2264 {lmmmh.MAWL_Kg:F1} kg, or introduce a vacuum hoist/balancer.";
            }
            else
            {
                recommendation = "Reduce cycle frequency or introduce job rotation to lower duty-cycle fatigue.";
            }

            int rula = 1;
            int reba = 1;
            if (input.VerticalMm < 500.0) { rula += 3; reba += 3; }
            else if (input.VerticalMm < 800.0) { rula += 2; reba += 2; }
            else if (input.VerticalMm > 1400.0) { rula += 2; reba += 2; }
            if (input.ReachMm > 500.0) { rula += 2; reba += 2; }
            if (input.LoadKg > 10.0) { rula += 2; reba += 2; }

            return new ErgonomicOutputs
            {
                OverallDCR = overallDcr,
                LumbarCompressionN = lumbar.CompN,
                LumbarShearN = lumbar.ShearN,
                LumbarDCR = lumbar.DCR,
                LumbarTLV_N = lumbar.TLV,
                CumulativeCompDCR = cumulCompDcr,
                KneeFlexionDeg = lumbar.KneeFlexionDeg,
                TrunkFlexionDeg = lumbar.TrunkFlexionDeg,
                HipHeightMm = lumbar.HipHeightMm,
                KneeMomentNm = lumbar.KneeMomentNm,
                HipMomentNm = lumbar.HipMomentNm,
                CenterOfPressureMm = lumbar.CenterOfPressureMm,
                IsBalanced = lumbar.IsBalanced,
                NioshRWLKg = niosh.RWL,
                NioshLI = niosh.LI,
                SnookMAWLKg = lmmmh.MAWL_Kg,
                SnookDCR = lmmmh.DCR,
                ArmStrengthMVC_N = aff.ArmMVC_N,
                ArmMAF_N = aff.ArmMAF_N,
                ArmDCR = aff.ArmDCR,
                HandStrengthMVC_N = hand.HandMVC_N,
                HandMAF_N = hand.HandMAF_N,
                HandDCR = hand.HandDCR,
                NeckMomentNm = neck.NeckMomentNm,
                NeckMAT_Nm = neck.NeckMAT_Nm,
                NeckDCR = neck.NeckDCR,
                DutyCycle = Math.Round(dutyCycle, 4),
                PotvinMAE = mae,
                RulaScore = Math.Min(7, rula),
                RebaScore = Math.Min(12, reba),
                RiskCategory = riskCategory,
                PrimaryLimitingFactor = primaryFactor,
                Recommendation = recommendation
            };
        }
    }
}
