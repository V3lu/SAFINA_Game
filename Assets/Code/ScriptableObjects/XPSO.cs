using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "XPSO", menuName = "Scriptable Objects/XPSO")]
public class XPSO : ScriptableObject
{
    [SerializeField] private int _maxLevel = 80;
    [SerializeField] private double _xpRequiredToLevelUpMultiplicator = 1.5;
    private List<double> _levelCaps = new List<double>{ 0.10, 0.15, 0.23, 0.34, 0.51, 0.76, 1.14, 1.71, 2.56, 3.84, 5.77, 8.65,
                                                        12.97, 19.46, 29.19, 43.79, 65.68, 98.53, 147.79, 221.68, 332.53, 498.79,
                                                        748.18, 1122.27, 1683.41, 2525.12, 3787.68, 5681.51, 8522.27, 12783.40,
                                                        19175.11, 28762.66, 43143.99, 64715.98, 97073.97, 145610.96, 218416.44,
                                                        327624.66, 491436.99, 737155.49, 1105733.23, 1658599.85, 2487899.77,
                                                        3731849.66, 5597774.49, 8396661.73, 12594992.60, 18892488.90, 28338733.34,
                                                        42508100.01, 63762150.02, 95643225.03, 143464837.55, 215197256.32,
                                                        322795884.48, 484193826.72, 726290740.08, 1089436110.12, 1634154165.19,
                                                        2451231247.78, 3676846871.68, 5515270307.52, 8272905461.28,
                                                        12409358191.92, 18614037287.88, 27921055931.82, 41881583897.73,
                                                        62822375846.60, 94233563769.89, 141350345654.84, 212025518482.26,
                                                        318038277723.39, 477057416585.09, 715586124877.63, 1073379187316.45,
                                                        1610068780974.67, 2415103171462.01, 3622654757193.02, 5433982135789.53 };

    public int Maxlevel => _maxLevel;
    public double XPRequiredToLevelUpMultiplicator => _xpRequiredToLevelUpMultiplicator;
    public List<double> LevelCaps => _levelCaps;
}
