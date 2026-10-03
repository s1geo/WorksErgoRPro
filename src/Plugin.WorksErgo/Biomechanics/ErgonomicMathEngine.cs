using System;
using System.Collections.Generic;

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
        public double FrequencyLiftsPerMin;   // Frequency (efforts/min)
        public double DurationHours;          // Duration of continuous exposure (hours)
        public double ShiftDurationHours;     // Total shift duration (default 8.0 hours)
        public double FrequencyPerDay;        // Daily frequency (e.g. 630 /day)
        public double EffectiveDurationSec;   // Duration of single effort (s, default ~0.922 s)
        public CouplingQuality Coupling;
        public HandGripType Grip;
        // Pushing & Pulling parameters (Snook & Ciriello 1991 / LM-MMH 2021)
        public double PushDistanceM;          // Push/Pull distance (m, default 7.5 m)
        public double PushInitialForceN;      // Optional override: measured initial push/pull force (N)
        public double PushSustainedForceN;    // Optional override: measured sustained push/pull force (N)
        // Carrying parameters (Snook & Ciriello 1991 / LM-MMH 2021)
        public double CarryDistanceM;         // Carry distance (m, default 4.0 m)
    }

    public struct SubtaskSummary
    {
        public string Name;
        public TaskType Task;
        public double CyclesPerDay;
        public double PeakCompressionN;
        public double CtF;
        public double LCFCD;
        public double TaskDCR;
        public string PrimaryLimitingFactor;
    }

    public struct CompositeJobResult
    {
        public double CompositeLCFCD;
        public double CompositeDutyCycle;
        public double CompositePotvinMAE;
        public double PeakCompressionN;
        public double PeakCompressionDCR;
        public double CompositeOverallDCR;
        public string RiskCategory;
        public string PrimaryLimitingFactor;
        public List<SubtaskSummary> Subtasks;
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
        // Pushing & Pulling outputs
        public double PushInitialForceN;
        public double PushSustainedForceN;
        public double PushInitialLimitN;
        public double PushSustainedLimitN;
        public double PushInitialDCR;
        public double PushSustainedDCR;
        public double PushPullDCR;
        // Carrying outputs
        public double CarryLimitKg;
        public double CarryDCR;
        // Discrete RULA (McAtamney & Corlett 1993)
        public int RulaScore;                 // RULA Grand Score (1-7)
        public int RulaTableAScore;
        public int RulaTableBScore;
        public int RulaScoreC;
        public int RulaScoreD;
        // Discrete REBA (Hignett & McAtamney 2000)
        public int RebaScore;                 // REBA Grand Score (1-15)
        public int RebaTableAScore;
        public int RebaTableBScore;
        public int RebaScoreC;
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

            // Pelvis and lower-body counter-balancing shift (natural human postural reflex)
            // As the torso leans forward, the pelvis shifts backward to keep CofP within the feet Base of Support
            double pelvisOffsetM = 0.08 + (thighLengthM * Math.Sin(kneeAngleRad * 0.5)) + (trunkMomentArm * 0.35);
            double pelvisCounterMoment = (bodyMass * 0.35 * g) * pelvisOffsetM;

            // Center of Pressure (CofP) along anterior/posterior axis from ankle center
            double totalWeightN = (bodyMass + input.LoadKg) * g;
            double netHorizontalMoment = trunkTorque + loadTorque - pelvisCounterMoment - (kneeMomentNm * 0.20);
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
        public static double CalculateCyclesToFailure(double compressionN, double tlvN)
        {
            double us = tlvN / 0.82; // Ultimate strength
            double ratio = compressionN / us;
            if (ratio < 0.45) return 50000.0;
            double ctf = 5000.0 * Math.Exp(-5.35 * (ratio - 0.46));
            return Math.Max(50.0, ctf);
        }

        public static double CalculateCumulativeCompression(double compressionN, double tlvN, double cyclesPerDay)
        {
            double ctf = CalculateCyclesToFailure(compressionN, tlvN);
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

        // 9. Liberty Mutual Push & Pull Capacity (Snook & Ciriello 1991, Potvin LM-MMH 2021)
        public struct PushPullResult
        {
            public double AppliedInitialN;
            public double AppliedSustainedN;
            public double InitialLimitN;
            public double SustainedLimitN;
            public double InitialDCR;
            public double SustainedDCR;
            public double PushPullDCR;
        }

        public static PushPullResult CalculateLMMMH_PushPull(PostureInputs input)
        {
            var anthro = GetAnthropometry(input.Percentile);
            bool isFemale = anthro.IsFemale;

            // Handle height (m)
            double hM = input.VerticalMm > 0 ? (input.VerticalMm / 1000.0) : 1.0;
            // Optimal handle height is around elbow height (0.95 - 1.05 m)
            double heightFactor = Math.Max(0.70, 1.0 - 0.25 * Math.Abs(hM - 1.00));

            // Distance (m)
            double distM = input.PushDistanceM > 0 ? input.PushDistanceM : 7.5;
            // Distance factor for sustained force: 2.1m -> 1.15, 7.5m -> 1.00, 15m -> 0.88, 30m -> 0.78
            double distFactor = Math.Max(0.70, 1.0 - 0.0075 * (distM - 7.5));

            // Frequency factor
            double freq = input.FrequencyLiftsPerMin;
            if (freq <= 0.0 && input.FrequencyPerDay > 0.0)
            {
                double shiftHours = input.ShiftDurationHours > 0.0 ? input.ShiftDurationHours : 8.0;
                freq = input.FrequencyPerDay / (shiftHours * 60.0);
            }
            double freqFactor = Math.Max(0.40, 1.0 - 0.035 * freq);

            // Baseline maximum acceptable forces for 75% capable population (Snook & Ciriello 1991, Potvin 2021)
            // Initial Force: Female ~200 N, Male ~300 N
            double baseInitial = isFemale ? 200.0 : 300.0;
            // Sustained Force: Female ~110 N, Male ~170 N
            double baseSustained = isFemale ? 110.0 : 170.0;

            double initialLimitN = Math.Round(baseInitial * heightFactor * freqFactor, 1);
            double sustainedLimitN = Math.Round(baseSustained * heightFactor * distFactor * freqFactor, 1);

            // Determine applied forces
            double appliedInitN = input.PushInitialForceN;
            double appliedSustN = input.PushSustainedForceN;

            // If not directly specified in Newtons, estimate from LoadKg and standard industrial rolling resistance
            if (appliedInitN <= 0.0 && input.LoadKg > 0.0)
            {
                // Rolling start coefficient mu_init ~ 0.20
                appliedInitN = input.LoadKg * 9.81 * 0.20;
            }
            if (appliedSustN <= 0.0 && input.LoadKg > 0.0)
            {
                // Rolling friction mu_sust ~ 0.08
                appliedSustN = input.LoadKg * 9.81 * 0.08;
            }

            appliedInitN = Math.Round(appliedInitN, 1);
            appliedSustN = Math.Round(appliedSustN, 1);

            double initDcr = Math.Round(appliedInitN / Math.Max(10.0, initialLimitN), 3);
            double sustDcr = Math.Round(appliedSustN / Math.Max(10.0, sustainedLimitN), 3);
            double pushPullDcr = Math.Max(initDcr, sustDcr);

            return new PushPullResult
            {
                AppliedInitialN = appliedInitN,
                AppliedSustainedN = appliedSustN,
                InitialLimitN = initialLimitN,
                SustainedLimitN = sustainedLimitN,
                InitialDCR = initDcr,
                SustainedDCR = sustDcr,
                PushPullDCR = pushPullDcr
            };
        }

        // 10. Liberty Mutual Carry Capacity (Snook & Ciriello 1991, Potvin LM-MMH 2021)
        public struct CarryResult
        {
            public double MAWC_Kg;
            public double CarryDCR;
        }

        public static CarryResult CalculateLMMMH_Carry(PostureInputs input)
        {
            var anthro = GetAnthropometry(input.Percentile);
            bool isFemale = anthro.IsFemale;

            // Carry distance (m)
            double distM = input.CarryDistanceM > 0 ? input.CarryDistanceM : 4.0;
            // Snook distance multiplier: 2.1m -> 1.0, 4.3m -> 0.90, 8.5m -> 0.82, 15m -> 0.74
            double distFactor = Math.Max(0.65, 1.0 - 0.02 * (distM - 2.1));

            // Frequency factor
            double freq = input.FrequencyLiftsPerMin;
            if (freq <= 0.0 && input.FrequencyPerDay > 0.0)
            {
                double shiftHours = input.ShiftDurationHours > 0.0 ? input.ShiftDurationHours : 8.0;
                freq = input.FrequencyPerDay / (shiftHours * 60.0);
            }
            double freqFactor = Math.Max(0.40, 1.0 - 0.04 * freq);

            // Knuckle / waist height factor (0.75m is standard carry height)
            double carryHeightM = input.VerticalMm > 0 ? (input.VerticalMm / 1000.0) : 0.75;
            double heightFactor = Math.Max(0.70, 1.0 - 0.20 * Math.Abs(carryHeightM - 0.75));

            // Baseline MAWC for 75% capable population at 2.1m (Snook & Ciriello 1991, Potvin 2021)
            // Female: ~14.5 kg, Male: ~22.0 kg
            double baseMawc = isFemale ? 14.5 : 22.0;
            double mawcKg = Math.Round(baseMawc * distFactor * freqFactor * heightFactor, 2);
            mawcKg = Math.Max(2.0, mawcKg);

            double carryDcr = Math.Round(input.LoadKg / mawcKg, 3);

            return new CarryResult
            {
                MAWC_Kg = mawcKg,
                CarryDCR = carryDcr
            };
        }

        // 11. Discrete RULA Assessment (McAtamney & Corlett 1993)
        public struct RulaResult
        {
            public int UpperArm;
            public int LowerArm;
            public int Wrist;
            public int WristTwist;
            public int TableAScore;
            public int ScoreC;
            public int Neck;
            public int Trunk;
            public int Legs;
            public int TableBScore;
            public int ScoreD;
            public int GrandScore;
            public string ActionLevelDescription;
        }

        // Standard RULA Table A [UpperArm 1..6, LowerArm 1..3, Wrist 1..4, WristTwist 1..2]
        private static readonly int[,,,] RulaTableA = new int[6, 3, 4, 2]
        {
            // Upper Arm 1
            {
                { {1, 2}, {2, 2}, {2, 3}, {3, 3} },
                { {2, 2}, {2, 2}, {3, 3}, {3, 4} },
                { {2, 3}, {3, 3}, {3, 4}, {4, 4} }
            },
            // Upper Arm 2
            {
                { {2, 3}, {3, 3}, {3, 4}, {4, 4} },
                { {3, 3}, {3, 3}, {3, 4}, {4, 4} },
                { {3, 4}, {4, 4}, {4, 5}, {5, 5} }
            },
            // Upper Arm 3
            {
                { {3, 3}, {4, 4}, {4, 4}, {5, 5} },
                { {3, 4}, {4, 4}, {4, 5}, {5, 5} },
                { {4, 4}, {4, 5}, {5, 5}, {5, 6} }
            },
            // Upper Arm 4
            {
                { {4, 4}, {4, 5}, {5, 5}, {5, 6} },
                { {4, 4}, {4, 5}, {5, 5}, {6, 6} },
                { {4, 5}, {5, 5}, {6, 6}, {6, 7} }
            },
            // Upper Arm 5
            {
                { {5, 5}, {5, 6}, {6, 7}, {7, 7} },
                { {5, 6}, {6, 6}, {6, 7}, {7, 7} },
                { {5, 6}, {6, 7}, {7, 7}, {7, 8} }
            },
            // Upper Arm 6
            {
                { {7, 7}, {7, 7}, {7, 8}, {8, 8} },
                { {8, 8}, {8, 8}, {8, 8}, {8, 9} },
                { {9, 9}, {9, 9}, {9, 9}, {9, 9} }
            }
        };

        // Standard RULA Table B [Neck 1..6, Trunk 1..6, Legs 1..2]
        private static readonly int[,,] RulaTableB = new int[6, 6, 2]
        {
            // Neck 1
            { {1, 3}, {2, 3}, {3, 4}, {5, 5}, {6, 6}, {7, 7} },
            // Neck 2
            { {2, 3}, {2, 3}, {4, 5}, {5, 5}, {6, 7}, {7, 7} },
            // Neck 3
            { {3, 3}, {3, 4}, {4, 5}, {5, 6}, {6, 7}, {7, 7} },
            // Neck 4
            { {5, 5}, {5, 6}, {6, 7}, {7, 7}, {7, 8}, {8, 8} },
            // Neck 5
            { {7, 7}, {7, 7}, {7, 8}, {8, 8}, {8, 8}, {8, 8} },
            // Neck 6
            { {8, 8}, {8, 8}, {8, 8}, {8, 9}, {9, 9}, {9, 9} }
        };

        // Standard RULA Table C [Score C 1..8, Score D 1..7]
        private static readonly int[,] RulaTableC = new int[8, 7]
        {
            { 1, 2, 3, 3, 4, 5, 5 },
            { 2, 2, 3, 4, 4, 5, 5 },
            { 3, 3, 3, 4, 4, 5, 6 },
            { 3, 3, 3, 4, 5, 6, 6 },
            { 4, 4, 4, 5, 6, 7, 7 },
            { 4, 4, 5, 6, 6, 7, 7 },
            { 5, 5, 6, 6, 7, 7, 7 },
            { 5, 5, 6, 7, 7, 7, 7 }
        };

        public static RulaResult CalculateRULA(PostureInputs input, double trunkDeg, double kneeDeg, bool isBalanced)
        {
            // Trunk score (1-4, +1 twist)
            int trunk = 1;
            if (trunkDeg > 60.0) trunk = 4;
            else if (trunkDeg > 20.0) trunk = 3;
            else if (trunkDeg > 5.0) trunk = 2;
            if (input.AsymmetryDeg > 10.0) trunk = Math.Min(6, trunk + 1);

            // Neck score (1-4, +1 twist)
            double neckDeg = trunkDeg * 0.5;
            int neck = 1;
            if (neckDeg > 20.0) neck = 3;
            else if (neckDeg > 10.0) neck = 2;
            if (input.AsymmetryDeg > 15.0) neck = Math.Min(6, neck + 1);

            // Legs score (1-2)
            int legs = isBalanced ? 1 : 2;

            // Upper Arm score (1-4, +1 reach)
            double reachM = input.ReachMm / 1000.0;
            double vertM = input.VerticalMm / 1000.0;
            int upperArm = 1;
            if (reachM > 0.60 || vertM < 0.35) upperArm = 4;
            else if (reachM > 0.45 || vertM < 0.55) upperArm = 3;
            else if (reachM > 0.30 || vertM > 1.20) upperArm = 2;
            if (reachM > 0.55) upperArm = Math.Min(6, upperArm + 1);

            // Lower Arm score (1-2)
            int lowerArm = (reachM >= 0.30 && reachM <= 0.50 && vertM >= 0.60 && vertM <= 1.10) ? 1 : 2;

            // Wrist score (1-4)
            int wrist = 1;
            if (input.Coupling == CouplingQuality.Poor || input.Grip == HandGripType.PinchGrip) wrist = 3;
            else if (input.Coupling == CouplingQuality.Fair) wrist = 2;

            // Wrist Twist score (1-2)
            int twist = (input.Grip == HandGripType.PinchGrip || input.AsymmetryDeg > 20.0) ? 2 : 1;

            // Lookup Table A
            int tableAScore = RulaTableA[upperArm - 1, lowerArm - 1, wrist - 1, twist - 1];

            // Muscle use and load
            int muscle = (input.FrequencyLiftsPerMin >= 4.0 || input.EffectiveDurationSec > 60.0) ? 1 : 0;
            int force = 0;
            if (input.LoadKg > 10.0) force = 3;
            else if (input.LoadKg >= 2.0) force = (muscle == 1) ? 2 : 1;

            int scoreC = Math.Max(1, Math.Min(8, tableAScore + muscle + force));

            // Lookup Table B
            int tableBScore = RulaTableB[neck - 1, trunk - 1, legs - 1];
            int scoreD = Math.Max(1, Math.Min(7, tableBScore + muscle + force));

            // Grand score from Table C
            int grandScore = RulaTableC[scoreC - 1, scoreD - 1];

            string desc;
            if (grandScore <= 2) desc = "Action Level 1: Acceptable posture if not maintained or repeated.";
            else if (grandScore <= 4) desc = "Action Level 2: Further investigation needed, changes may be required.";
            else if (grandScore <= 6) desc = "Action Level 3: Investigation and changes required soon.";
            else desc = "Action Level 4: Investigate and implement change immediately.";

            return new RulaResult
            {
                UpperArm = upperArm,
                LowerArm = lowerArm,
                Wrist = wrist,
                WristTwist = twist,
                TableAScore = tableAScore,
                ScoreC = scoreC,
                Neck = neck,
                Trunk = trunk,
                Legs = legs,
                TableBScore = tableBScore,
                ScoreD = scoreD,
                GrandScore = grandScore,
                ActionLevelDescription = desc
            };
        }

        // 12. Discrete REBA Assessment (Hignett & McAtamney 2000)
        public struct RebaResult
        {
            public int Trunk;
            public int Neck;
            public int Legs;
            public int TableAScore;
            public int ScoreAPrime;
            public int UpperArm;
            public int LowerArm;
            public int Wrist;
            public int TableBScore;
            public int ScoreBPrime;
            public int ScoreC;
            public int GrandScore;
            public string RiskLevelDescription;
        }

        // Standard REBA Table A [Trunk 1..5, Neck 1..3, Legs 1..4]
        private static readonly int[,,] RebaTableA = new int[5, 3, 4]
        {
            // Trunk 1
            { {1, 2, 3, 4}, {2, 3, 4, 5}, {2, 4, 5, 6} },
            // Trunk 2
            { {2, 3, 4, 5}, {3, 4, 5, 6}, {4, 5, 6, 7} },
            // Trunk 3
            { {2, 4, 5, 6}, {4, 5, 6, 7}, {5, 6, 7, 8} },
            // Trunk 4
            { {3, 5, 6, 7}, {5, 6, 7, 8}, {6, 7, 8, 9} },
            // Trunk 5
            { {4, 6, 7, 8}, {6, 7, 8, 9}, {7, 8, 9, 9} }
        };

        // Standard REBA Table B [UpperArm 1..6, LowerArm 1..2, Wrist 1..3]
        private static readonly int[,,] RebaTableB = new int[6, 2, 3]
        {
            // Upper Arm 1
            { {1, 2, 2}, {1, 2, 3} },
            // Upper Arm 2
            { {1, 2, 3}, {2, 3, 4} },
            // Upper Arm 3
            { {3, 4, 5}, {4, 5, 5} },
            // Upper Arm 4
            { {4, 5, 5}, {5, 6, 7} },
            // Upper Arm 5
            { {6, 7, 8}, {7, 8, 8} },
            // Upper Arm 6
            { {7, 8, 8}, {8, 9, 9} }
        };

        // Standard REBA Table C [Score A' 1..12, Score B' 1..12]
        private static readonly int[,] RebaTableC = new int[12, 12]
        {
            { 1, 1, 1, 2, 3, 3, 4, 5, 6, 7, 7, 7 },
            { 1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 7, 8 },
            { 2, 3, 3, 3, 4, 5, 6, 7, 7, 8, 8, 8 },
            { 3, 4, 4, 4, 5, 6, 7, 8, 8, 9, 9, 9 },
            { 4, 4, 4, 5, 6, 7, 8, 8, 9, 9, 9, 9 },
            { 6, 6, 6, 7, 8, 8, 9, 9, 10, 10, 10, 10 },
            { 7, 7, 7, 8, 9, 9, 9, 10, 10, 11, 11, 11 },
            { 8, 8, 8, 9, 10, 10, 10, 10, 10, 11, 11, 11 },
            { 9, 9, 9, 10, 10, 10, 11, 11, 11, 12, 12, 12 },
            { 10, 10, 10, 11, 11, 11, 11, 12, 12, 12, 12, 12 },
            { 11, 11, 11, 11, 12, 12, 12, 12, 12, 12, 12, 12 },
            { 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12 }
        };

        public static RebaResult CalculateREBA(PostureInputs input, double trunkDeg, double kneeDeg, bool isBalanced)
        {
            // Trunk (1-5)
            int trunk = 1;
            if (trunkDeg > 60.0) trunk = 4;
            else if (trunkDeg > 20.0) trunk = 3;
            else if (trunkDeg > 5.0) trunk = 2;
            if (input.AsymmetryDeg > 10.0) trunk = Math.Min(5, trunk + 1);

            // Neck (1-3)
            double neckDeg = trunkDeg * 0.5;
            int neck = (neckDeg > 20.0) ? 2 : 1;
            if (input.AsymmetryDeg > 15.0) neck = Math.Min(3, neck + 1);

            // Legs (1-4)
            int legs = isBalanced ? 1 : 2;
            if (kneeDeg > 60.0) legs += 2;
            else if (kneeDeg >= 30.0) legs += 1;
            legs = Math.Min(4, legs);

            int tableAScore = RebaTableA[trunk - 1, neck - 1, legs - 1];

            // Load / Force score (0-2)
            int load = 0;
            if (input.LoadKg > 10.0) load = 2;
            else if (input.LoadKg >= 5.0) load = 1;

            int scoreAPrime = Math.Max(1, Math.Min(12, tableAScore + load));

            // Upper Arm (1-6)
            double reachM = input.ReachMm / 1000.0;
            double vertM = input.VerticalMm / 1000.0;
            int upperArm = 1;
            if (reachM > 0.60 || vertM < 0.35) upperArm = 4;
            else if (reachM > 0.45 || vertM < 0.55) upperArm = 3;
            else if (reachM > 0.30 || vertM > 1.20) upperArm = 2;
            if (reachM > 0.55) upperArm = Math.Min(6, upperArm + 1);

            // Lower Arm (1-2)
            int lowerArm = (reachM >= 0.30 && reachM <= 0.50 && vertM >= 0.60 && vertM <= 1.10) ? 1 : 2;

            // Wrist (1-3)
            int wrist = 1;
            if (input.Coupling == CouplingQuality.Poor) wrist = 2;
            if (input.AsymmetryDeg > 15.0 || input.Grip == HandGripType.PinchGrip) wrist = Math.Min(3, wrist + 1);

            int tableBScore = RebaTableB[upperArm - 1, lowerArm - 1, wrist - 1];

            // Coupling score (0-3)
            int coupling = 0;
            if (input.Coupling == CouplingQuality.Poor) coupling = 2;
            else if (input.Coupling == CouplingQuality.Fair) coupling = 1;

            int scoreBPrime = Math.Max(1, Math.Min(12, tableBScore + coupling));

            // Table C lookup
            int scoreC = RebaTableC[scoreAPrime - 1, scoreBPrime - 1];

            // Activity score (+1 if static or rapid)
            int activity = (input.FrequencyLiftsPerMin >= 4.0 || input.EffectiveDurationSec > 60.0) ? 1 : 0;
            int grandScore = Math.Max(1, Math.Min(15, scoreC + activity));

            string desc;
            if (grandScore == 1) desc = "Level 0: Negligible risk.";
            else if (grandScore <= 3) desc = "Level 1: Low risk, change may be needed.";
            else if (grandScore <= 7) desc = "Level 2: Medium risk, further investigation, change soon.";
            else if (grandScore <= 10) desc = "Level 3: High risk, investigate and implement change.";
            else desc = "Level 4: Very high risk, implement change immediately.";

            return new RebaResult
            {
                Trunk = trunk,
                Neck = neck,
                Legs = legs,
                TableAScore = tableAScore,
                ScoreAPrime = scoreAPrime,
                UpperArm = upperArm,
                LowerArm = lowerArm,
                Wrist = wrist,
                TableBScore = tableBScore,
                ScoreBPrime = scoreBPrime,
                ScoreC = scoreC,
                GrandScore = grandScore,
                RiskLevelDescription = desc
            };
        }

        // 13. Multi-Subtask Job Analysis (Gibson & Potvin 2016, Work(s) Ergo Composite Job)
        public static CompositeJobResult CalculateCompositeJob(IEnumerable<PostureInputs> subtasks, double shiftHours = 8.0)
        {
            double totalLcfcd = 0.0;
            double totalEffortSec = 0.0;
            double peakCompression = 0.0;
            double highestTaskDcr = 0.0;
            string primaryFactor = "None";
            var summaryList = new List<SubtaskSummary>();

            int idx = 1;
            foreach (var task in subtasks)
            {
                var res = Evaluate(task);
                double dailyCycles = task.FrequencyPerDay > 0.0 ? task.FrequencyPerDay : (task.FrequencyLiftsPerMin * 60.0 * (task.DurationHours > 0.0 ? task.DurationHours : 1.0));
                double ctf = CalculateCyclesToFailure(res.LumbarCompressionN, res.LumbarTLV_N);
                double lcfcd = dailyCycles / Math.Max(50.0, ctf);
                totalLcfcd += lcfcd;

                double effortSec = task.EffectiveDurationSec > 0.0 ? task.EffectiveDurationSec : 0.922;
                totalEffortSec += (dailyCycles * effortSec);

                if (res.LumbarCompressionN > peakCompression) peakCompression = res.LumbarCompressionN;
                if (res.OverallDCR > highestTaskDcr)
                {
                    highestTaskDcr = res.OverallDCR;
                    primaryFactor = $"Subtask #{idx} ({task.Task}): {res.PrimaryLimitingFactor}";
                }

                summaryList.Add(new SubtaskSummary
                {
                    Name = $"Subtask #{idx}",
                    Task = task.Task,
                    CyclesPerDay = Math.Round(dailyCycles, 0),
                    PeakCompressionN = res.LumbarCompressionN,
                    CtF = Math.Round(ctf, 0),
                    LCFCD = Math.Round(lcfcd, 3),
                    TaskDCR = res.OverallDCR,
                    PrimaryLimitingFactor = res.PrimaryLimitingFactor
                });
                idx++;
            }

            double totalShiftSec = shiftHours * 3600.0;
            double compositeDutyCycle = Math.Round(totalEffortSec / Math.Max(1.0, totalShiftSec), 4);
            double compositePotvinMae = CalculatePotvinMAE(compositeDutyCycle);

            double refTlv = 3575.0; // Conservative 42yo female
            double peakCompDcr = Math.Round(peakCompression / refTlv, 3);
            double compositeOverallDcr = Math.Max(peakCompDcr, Math.Max(totalLcfcd, highestTaskDcr));
            compositeOverallDcr = Math.Round(compositeOverallDcr, 3);

            string riskCategory;
            if (compositeOverallDcr <= 0.85) riskCategory = "Low (Safe)";
            else if (compositeOverallDcr <= 1.00) riskCategory = "Moderate (Warning)";
            else riskCategory = "High (Hazard)";

            return new CompositeJobResult
            {
                CompositeLCFCD = Math.Round(totalLcfcd, 3),
                CompositeDutyCycle = compositeDutyCycle,
                CompositePotvinMAE = compositePotvinMae,
                PeakCompressionN = Math.Round(peakCompression, 1),
                PeakCompressionDCR = peakCompDcr,
                CompositeOverallDCR = compositeOverallDcr,
                RiskCategory = riskCategory,
                PrimaryLimitingFactor = primaryFactor,
                Subtasks = summaryList
            };
        }

        // Comprehensive 7-Axis Evaluation Pipeline
        public static ErgonomicOutputs Evaluate(PostureInputs input)
        {
            double dutyCycle = CalculateDutyCycle(input);
            double mae = CalculatePotvinMAE(dutyCycle);

            var lumbar = CalculateLumbarL5S1(input);
            double dailyCycles = input.FrequencyPerDay > 0.0 ? input.FrequencyPerDay : (input.FrequencyLiftsPerMin * 60.0 * (input.DurationHours > 0.0 ? input.DurationHours : 1.0));
            double cumulCompDcr = CalculateCumulativeCompression(lumbar.CompN, lumbar.TLV, dailyCycles);

            var niosh = CalculateNIOSH(input);
            var lmmmhLift = CalculateLMMMH(input);
            var pushPull = CalculateLMMMH_PushPull(input);
            var carry = CalculateLMMMH_Carry(input);
            var aff = CalculateAFF(input, mae);
            var hand = CalculateHandPak(input, mae);
            var neck = CalculateNeck(input, mae);

            // Compute discrete RULA & REBA
            var rula = CalculateRULA(input, lumbar.TrunkFlexionDeg, lumbar.KneeFlexionDeg, lumbar.IsBalanced);
            var reba = CalculateREBA(input, lumbar.TrunkFlexionDeg, lumbar.KneeFlexionDeg, lumbar.IsBalanced);

            // Select active MMH DCR based on task type
            double taskSpecificMmhDcr = lmmmhLift.DCR;
            if (input.Task == TaskType.PushingPulling)
            {
                taskSpecificMmhDcr = pushPull.PushPullDCR;
            }
            else if (input.Task == TaskType.Carrying)
            {
                taskSpecificMmhDcr = carry.CarryDCR;
            }

            // Overall DCR represents the weakest link across all 7 axes
            double overallDcr = Math.Max(lumbar.DCR,
                                Math.Max(cumulCompDcr,
                                Math.Max(taskSpecificMmhDcr,
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
            if (taskSpecificMmhDcr > maxVal)
            {
                maxVal = taskSpecificMmhDcr;
                if (input.Task == TaskType.PushingPulling)
                    primaryFactor = (pushPull.InitialDCR >= pushPull.SustainedDCR) ? "Push/Pull Initial Force Threshold" : "Push/Pull Sustained Force Threshold";
                else if (input.Task == TaskType.Carrying)
                    primaryFactor = "Carry Distance / Metabolic Weight Limit (MAWC)";
                else
                    primaryFactor = "Liberty Mutual MMH Population Capacity";
            }
            if (aff.ArmDCR > maxVal) { maxVal = aff.ArmDCR; primaryFactor = "Shoulder/Arm Strength (AFF ANN)"; }
            if (hand.HandDCR > maxVal) { maxVal = hand.HandDCR; primaryFactor = "HandPak Grip/Pinch Fatigue"; }
            if (neck.NeckDCR > maxVal) { maxVal = neck.NeckDCR; primaryFactor = "Cervical Spine Neck Moment"; }

            // Actionable engineering recommendations
            string recommendation;
            if (overallDcr <= 0.85)
            {
                recommendation = "Task parameters are within safe ergonomic limits. No engineering redesign required.";
            }
            else if (input.Task == TaskType.PushingPulling && pushPull.PushPullDCR > 0.85)
            {
                recommendation = pushPull.InitialDCR >= pushPull.SustainedDCR
                    ? $"Initial push force ({pushPull.AppliedInitialN:F0} N) exceeds limit ({pushPull.InitialLimitN:F0} N). Install larger diameter low-friction swivel wheels or motorized tugger."
                    : $"Sustained push force ({pushPull.AppliedSustainedN:F0} N) exceeds limit ({pushPull.SustainedLimitN:F0} N). Reduce cart payload or reduce travel distance.";
            }
            else if (input.Task == TaskType.Carrying && carry.CarryDCR > 0.85)
            {
                recommendation = $"Carry weight ({input.LoadKg:F1} kg) exceeds acceptable carry limit ({carry.MAWC_Kg:F1} kg) for {input.CarryDistanceM:F1} m. Utilize a mobile cart, roller conveyor or reduce carry distance.";
            }
            else if (input.VerticalMm < 550.0)
            {
                recommendation = $"Raise pickup point by {Math.Round(800.0 - input.VerticalMm)} mm (using scissor lift or pallet riser) to eliminate deep trunk flexion.";
            }
            else if (input.ReachMm > 450.0)
            {
                recommendation = $"Bring load {Math.Round(input.ReachMm - 300.0)} mm closer to body center to reduce lumbar torque and arm strain.";
            }
            else if (input.LoadKg > lmmmhLift.MAWL_Kg)
            {
                recommendation = $"Reduce unit package weight from {input.LoadKg:F1} kg to \u2264 {lmmmhLift.MAWL_Kg:F1} kg, or introduce a vacuum hoist/balancer.";
            }
            else
            {
                recommendation = "Reduce cycle frequency or introduce job rotation to lower duty-cycle fatigue.";
            }

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
                SnookMAWLKg = lmmmhLift.MAWL_Kg,
                SnookDCR = lmmmhLift.DCR,
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
                PushInitialForceN = pushPull.AppliedInitialN,
                PushSustainedForceN = pushPull.AppliedSustainedN,
                PushInitialLimitN = pushPull.InitialLimitN,
                PushSustainedLimitN = pushPull.SustainedLimitN,
                PushInitialDCR = pushPull.InitialDCR,
                PushSustainedDCR = pushPull.SustainedDCR,
                PushPullDCR = pushPull.PushPullDCR,
                CarryLimitKg = carry.MAWC_Kg,
                CarryDCR = carry.CarryDCR,
                RulaScore = rula.GrandScore,
                RulaTableAScore = rula.TableAScore,
                RulaTableBScore = rula.TableBScore,
                RulaScoreC = rula.ScoreC,
                RulaScoreD = rula.ScoreD,
                RebaScore = reba.GrandScore,
                RebaTableAScore = reba.TableAScore,
                RebaTableBScore = reba.TableBScore,
                RebaScoreC = reba.ScoreC,
                RiskCategory = riskCategory,
                PrimaryLimitingFactor = primaryFactor,
                Recommendation = recommendation
            };
        }
    }
}
