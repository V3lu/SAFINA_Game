using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "XPSO", menuName = "Scriptable Objects/XPSO")]
public class XPSO : ScriptableObject
{
    [SerializeField] int _maxLevel = 80;
    [SerializeField] double _xpRequiredToLevelUpMultiplicator = 1.5;
    List<long> _levelCaps = new(){ 10, 15, 23, 34, 51, 76, 114, 171, 256, 384, 577, 865,
                                            1297, 1946, 2919, 4379, 6568, 9853, 14779, 22168, 33253, 49879,
                                            74818, 112227, 168341, 252512, 378768, 568151, 852227, 1278340,
                                            1917511, 2876266, 4314399, 6471598, 9707397, 14561096, 21841644,
                                            32762466, 49143699, 73715549, 110573323, 165859985, 248789977,
                                            373184966, 559777449, 839666173, 1259499260, 1889248890, 2833873334,
                                            4250810001, 6376215002, 9564322503, 14346483755, 21519725632,
                                            32279588448, 48419382672, 72629074008, 108943611012, 163415416519,
                                            245123124778, 367684687168, 551527030752, 827290546128,
                                            1240935819192, 1861403728788, 2792105593182, 4188158389773,
                                            6282237584660, 9423356376989, 14135034565484, 21202551848226,
                                            31803827772339, 47705741658509, 71558612487763, 107337918731645,
                                            161006878097467, 241510317146201, 362265475719302, 543398213578953 };

    public int Maxlevel => _maxLevel;
    public double XPRequiredToLevelUpMultiplicator => _xpRequiredToLevelUpMultiplicator;
    public List<long> LevelCaps => _levelCaps;
}
