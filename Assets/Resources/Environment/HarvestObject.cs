using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HarvestObject", menuName = "ScriptableObjects/HarvestObject", order = 1)]
public class HarvestObject : ScriptableObject
{
    public enum HarvestLevel
    {
        tierZero,
        tierOne,
        tierTwo,
        tierThree,
        tierFour
    }

    public enum HarvestType
    {
        Wood,
        Stone,
        Metal,
        Food,
        Tree,
        Other
    }

    public string objectName;
    public int minAmount;
    public int maxAmount;
    public HarvestLevel harvestLevel;
    public int harvestXP;
    public int defaultHealth;
    public HarvestType harvestType;
}