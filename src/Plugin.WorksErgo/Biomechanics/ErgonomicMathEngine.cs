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

    public struct PostureInputs
    {
        public DHMPercentile Percentile;
        public TaskType Task;
        public double LoadKg;
        public double ReachMm;        // Horizontal distance from ankles/lumbar to load (H)
        public double VerticalMm;     // Vertical height of load from floor (V)
        public double TravelMm;       // Vertical travel distance (D)
        public double AsymmetryDeg;   // Twisting angle in degrees (A)
        public double FrequencyLiftsPerMin;
        public double DurationHours;
        public CouplingQuality Coupling;
    }

    public struct ErgonomicOutputs
    {
        public double OverallDCR;
        public double LumbarCompressionN;
        public double LumbarDCR;
        public double NioshRWLKg;
        public double NioshLI;
        public double SnookMAWLKg;
        public double SnookDCR;
        public double ArmDCR;
        public double PotvinMAE;
        public int RulaScore;
        public int RebaScore;
        public string RiskCategory;     // "Low (Safe)", "Moderate (Warning)", "High (Hazard)"
        public string PrimaryLimitingFactor;
        public string Recommendation;
    }

    public static class ErgonomicMathEngine
    {
        // Anthropometry Constants (Weight kg, Height cm, Upper body mass fraction)
        public static (double MassKg, double StatureCm) GetAnthropometry(DHMPercentile p)
        {
            switch (p)
            {
                case DHMPercentile.Female5th: return (50.0, 152.0);
                case DHMPercentile.Female50th: return (62.0, 162.0);
                case DHMPercentile.Female95th: return (76.0, 172.0);
                case DHMPercentile.Male5th: return (64.0, 165.0);
                case DHMPercentile.Male50th: return (78.0, 175.0);
                case DHMPercentile.Male95th: return (98.0, 187.0);
                default: return (78.0, 175.0);
            }
        }

        // 1. Lumbar Spine L5/S1 Compression (Jäger 2023 / 3400 N NIOSH limit)
        public static (double CompN, double DCR) CalculateLumbarL5S1(PostureInputs input)
        {
            var anthro = GetAnthropometry(input.Percentile);
            double bodyMass = anthro.MassKg;
            double upperBodyMass = bodyMass * 0.60; // 60% of body mass above L5/S1
            double g = 9.81;

            // Moment arms (meters)
            double hMeters = Math.Max(0.20, Math.Min(0.85, input.ReachMm / 1000.0));
            double trunkMomentArm = hMeters * 0.45; // Trunk center of mass
            double loadMomentArm = hMeters;

            // External moment around L5/S1 in Nm
            double trunkTorque = upperBodyMass * g * trunkMomentArm;
            double loadTorque = (input.LoadKg * g) * loadMomentArm;
            double totalTorque = trunkTorque + loadTorque;

            // Erector spinae muscle force with ~5 cm lever arm
            double muscleArm = 0.05; // 50 mm
            double erectorForce = totalTorque / muscleArm;

            // Inclination angle estimate from vertical height V
            double standingHipHeight = (anthro.StatureCm * 10.0) * 0.53;
            double vDiff = standingHipHeight - input.VerticalMm;
            double forwardLeanDeg = Math.Max(0.0, Math.Min(75.0, vDiff / 8.0));
            double thetaRad = forwardLeanDeg * (Math.PI / 180.0);

            // Compressive force = muscle force + upper body weight component + load component
            double compressionN = erectorForce + (upperBodyMass * g + input.LoadKg * g) * Math.Cos(thetaRad);

            // NIOSH Action Limit = 3400 N
            double dcr = compressionN / 3400.0;
            return (Math.Round(compressionN, 1), Math.Round(dcr, 3));
        }

        // 2. NIOSH Revised Lifting Equation (1991 / 2021)
        public static (double RWL, double LI) CalculateNIOSH(PostureInputs input)
        {
            double lc = 23.0; // Load Constant (kg)

            // Horizontal Multiplier (HM = 25 / H, cm)
            double hCm = Math.Max(25.0, Math.Min(63.0, input.ReachMm / 10.0));
            double hm = 25.0 / hCm;

            // Vertical Multiplier (VM = 1 - 0.003 * |V - 75|, cm)
            double vCm = Math.Max(0.0, Math.Min(175.0, input.VerticalMm / 10.0));
            double vm = Math.Max(0.0, 1.0 - 0.003 * Math.Abs(vCm - 75.0));

            // Distance Multiplier (DM = 0.82 + 4.5 / D, cm)
            double dCm = Math.Max(25.0, Math.Min(200.0, input.TravelMm / 10.0));
            double dm = Math.Max(0.0, Math.Min(1.0, 0.82 + (4.5 / dCm)));

            // Asymmetric Multiplier (AM = 1 - 0.0032 * A, deg)
            double aDeg = Math.Max(0.0, Math.Min(135.0, input.AsymmetryDeg));
            double am = Math.Max(0.0, 1.0 - 0.0032 * aDeg);

            // Frequency Multiplier (FM)
            double freq = input.FrequencyLiftsPerMin;
            double fm = 1.0;
            if (freq <= 0.2) fm = 1.0;
            else if (freq <= 1.0) fm = 0.94;
            else if (freq <= 2.0) fm = 0.91;
            else if (freq <= 4.0) fm = 0.84;
            else if (freq <= 6.0) fm = 0.75;
            else if (freq <= 9.0) fm = 0.52;
            else if (freq <= 12.0) fm = 0.37;
            else fm = 0.20;

            if (input.DurationHours > 2.0) fm *= 0.85;

            // Coupling Multiplier (CM)
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

        // 3. Liberty Mutual MMH (Snook & Ciriello MAWL)
        public static (double MAWL, double DCR) CalculateSnookMMH(PostureInputs input)
        {
            var anthro = GetAnthropometry(input.Percentile);
            bool isFemale = input.Percentile.ToString().StartsWith("Female");

            // Baseline acceptable weight (kg) for 75% female / 90% male population
            double baseMawl = isFemale ? 14.5 : 22.0;

            // Height and reach attenuation
            double reachFactor = Math.Max(0.5, 1.0 - (input.ReachMm - 300.0) / 1000.0);
            double freqFactor = Math.Max(0.4, 1.0 - (input.FrequencyLiftsPerMin * 0.04));

            double mawl = baseMawl * reachFactor * freqFactor;
            if (input.DurationHours > 4.0) mawl *= 0.88;

            mawl = Math.Max(1.0, mawl);
            double dcr = input.LoadKg / mawl;

            return (Math.Round(mawl, 2), Math.Round(dcr, 3));
        }

        // 4. Potvin Maximum Acceptable Effort (MAE 2012)
        public static double CalculatePotvinMAE(double dutyCycleFraction)
        {
            double dc = Math.Max(0.005, Math.Min(0.99, dutyCycleFraction));
            // MAE = 1 - ((DC - 0.003) / 0.997)^0.603
            double baseVal = (dc - 0.003) / 0.997;
            double mae = 1.0 - Math.Pow(baseVal, 0.603);
            return Math.Round(Math.Max(0.05, Math.Min(1.0, mae)), 3);
        }

        // 5. Arm Force Field (AFF) Shoulder & Arm Capacity
        public static double CalculateArmDCR(PostureInputs input)
        {
            var anthro = GetAnthropometry(input.Percentile);
            bool isFemale = input.Percentile.ToString().StartsWith("Female");
            double baseArmCapacityN = isFemale ? 110.0 : 190.0;

            // Reach extension penalty (lever arm on shoulder)
            double reachRatio = input.ReachMm / 650.0; // 650 mm nominal arm reach
            double effectiveCapacityN = baseArmCapacityN / Math.Max(0.6, reachRatio);

            // Duty cycle from frequency (assuming 3 sec per lift)
            double dutyCycle = Math.Min(0.90, (input.FrequencyLiftsPerMin * 3.0) / 60.0);
            double mae = CalculatePotvinMAE(dutyCycle);
            effectiveCapacityN *= mae;

            double appliedForceN = (input.LoadKg * 9.81) / 2.0; // Two hands shared
            double armDcr = appliedForceN / Math.Max(10.0, effectiveCapacityN);

            return Math.Round(armDcr, 3);
        }

        // 6. RULA & REBA Rapid Posture Scoring
        public static (int Rula, int Reba) CalculatePostures(PostureInputs input)
        {
            int rula = 1;
            int reba = 1;

            // Trunk flexion score
            if (input.VerticalMm < 500.0) { rula += 3; reba += 3; }
            else if (input.VerticalMm < 800.0) { rula += 2; reba += 2; }
            else if (input.VerticalMm > 1400.0) { rula += 2; reba += 2; }

            // Reach score
            if (input.ReachMm > 500.0) { rula += 2; reba += 2; }
            else if (input.ReachMm > 350.0) { rula += 1; reba += 1; }

            // Twist score
            if (input.AsymmetryDeg > 30.0) { rula += 1; reba += 1; }

            // Load score
            if (input.LoadKg > 10.0) { rula += 2; reba += 2; }
            else if (input.LoadKg > 4.0) { rula += 1; reba += 1; }

            rula = Math.Min(7, Math.Max(1, rula));
            reba = Math.Min(12, Math.Max(1, reba));

            return (rula, reba);
        }

        // Comprehensive Evaluation Pipeline
        public static ErgonomicOutputs Evaluate(PostureInputs input)
        {
            var lumbar = CalculateLumbarL5S1(input);
            var niosh = CalculateNIOSH(input);
            var snook = CalculateSnookMMH(input);
            double armDcr = CalculateArmDCR(input);
            double mae = CalculatePotvinMAE(Math.Min(0.85, (input.FrequencyLiftsPerMin * 3.0) / 60.0));
            var postures = CalculatePostures(input);

            // Overall DCR is maximum of the critical biomechanical axes
            double overallDcr = Math.Max(lumbar.DCR, Math.Max(niosh.LI, Math.Max(snook.DCR, armDcr)));
            overallDcr = Math.Round(overallDcr, 3);

            string riskCategory;
            if (overallDcr <= 0.85) riskCategory = "Low (Safe)";
            else if (overallDcr <= 1.00) riskCategory = "Moderate (Warning)";
            else riskCategory = "High (Hazard)";

            // Determine primary limiting factor
            string primaryFactor = "Lumbar L5/S1 Compression";
            double maxVal = lumbar.DCR;

            if (niosh.LI > maxVal) { maxVal = niosh.LI; primaryFactor = "NIOSH Lifting Index (Geometry/Frequency)"; }
            if (snook.DCR > maxVal) { maxVal = snook.DCR; primaryFactor = "Snook & Ciriello Population Capacity"; }
            if (armDcr > maxVal) { maxVal = armDcr; primaryFactor = "Shoulder/Arm Strength (AFF)"; }

            // Actionable engineering recommendations
            string recommendation;
            if (overallDcr <= 0.85)
            {
                recommendation = "Task parameters are within safe ergonomic limits. No engineering redesign required.";
            }
            else if (input.VerticalMm < 600.0)
            {
                recommendation = $"Raise pickup point by {Math.Round(800.0 - input.VerticalMm)} mm (using scissor lift or pallet riser) to eliminate deep trunk flexion.";
            }
            else if (input.ReachMm > 450.0)
            {
                recommendation = $"Bring load {Math.Round(input.ReachMm - 300.0)} mm closer to body center to reduce lumbar torque and arm strain.";
            }
            else if (input.LoadKg > niosh.RWL)
            {
                recommendation = $"Reduce unit package weight from {input.LoadKg:F1} kg to \u2264 {niosh.RWL:F1} kg, or introduce a vacuum hoist/balancer.";
            }
            else
            {
                recommendation = "Reduce cycle frequency or introduce job rotation to lower duty-cycle fatigue.";
            }

            return new ErgonomicOutputs
            {
                OverallDCR = overallDcr,
                LumbarCompressionN = lumbar.CompN,
                LumbarDCR = lumbar.DCR,
                NioshRWLKg = niosh.RWL,
                NioshLI = niosh.LI,
                SnookMAWLKg = snook.MAWL,
                SnookDCR = snook.DCR,
                ArmDCR = armDcr,
                PotvinMAE = mae,
                RulaScore = postures.Rula,
                RebaScore = postures.Reba,
                RiskCategory = riskCategory,
                PrimaryLimitingFactor = primaryFactor,
                Recommendation = recommendation
            };
        }
    }
}
