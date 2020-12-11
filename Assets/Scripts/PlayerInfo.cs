using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class PlayerStat
{
    public int currentTier
    {
        get { return currentTier; }
        private set { }
    }

    public string descriptionText;

    public void IncrementTier()
    {
        currentTier++;
    }
}

[Serializable]
public class VitalityStat : PlayerStat
{
    public int standardHealthIncrease;
    public int capstoneHealthIncrease;
    public float capstoneRegenSpeed;
    //Capstone makes you regen health
}

[Serializable]
public class StrengthStat : PlayerStat
{
    public int standardDamageIncrease;
    public int capstoneDamageDecrease;
    public float capstoneAttackSpeed;
    public int capstonePoisonDamagePerTick;
    public float capstonePoisonDuration;
    //Capstone poison damage over time on hit
}

[Serializable]
public class AcrobaticsStat : PlayerStat
{
    public float standardSpeedIncrease;
    public float standardDurationDecrease;
    public float standardCooldownDecrease;
    public float capstoneSpeedIncrease;
    public float capstoneDurationDecrease;
    public float capstoneCooldownDecrease;
    //Capstone makes you immune to damage while rolling
}

[Serializable]
public class DexterityStat : PlayerStat
{
    public int standardParryDamageIncrease;
    public float capstoneDeflectDamagePercent;
    //Capstone allows deflecting damage back
}

[Serializable]
public class IllusionStat : PlayerStat
{
    public float standardCooldownDecrease;
    public float standardDurationIncrease;
    //Capstone makes it mass
}

[Serializable]
public class WillpowerStat : PlayerStat
{
    public int standardHealingIncrease;
    public float standardCooldownDecrease;
    public int capstoneHealingIncrease;
    public float capstoneCooldownDecrease;
    public int capstoneTemporaryHealth;
    //Capstone grants temporary health
}

[Serializable]
public class StealthStat : PlayerStat
{

    //Capstone
}

public class PlayerInfo : MonoBehaviour {

    public int maxLevel;
    public int[] xpReqPerLevel;

    [Header("Stat Leveling Options")]
    public int startingStatPoints;
    public int maxStatTier;
    public int[] minLevelPerStatTier;
    public int[] pointCostPerStatTier;

    [Space]
    public VitalityStat vitality;
    public StrengthStat strength;
    public AcrobaticsStat acrobatics;
    public DexterityStat dexterity;
    public StealthStat stealth;
    public IllusionStat illusion;
    public WillpowerStat willpower;
    
    int level;
    int currentXP;
    int statPoints;
    int gold;

    Vector3 spawnPosition;

    public void AddXP(int newXP)
    {
        // If you reach max level, just record XP
        if (level + 1 > maxLevel)
        {
            currentXP += newXP;
            return;
        }
        
        if (currentXP + newXP >= xpReqPerLevel[level])
        {
            int extraXP = newXP + currentXP - xpReqPerLevel[level];

            currentXP = xpReqPerLevel[level];

            LevelUp();

            AddXP(extraXP);
        }
        else
        {
            currentXP += newXP;
        }
    }

    public void AddGold(int gold)
    {
        this.gold += gold;
    }

    public void SetSpawnPosition(Vector3 spawnPosition)
    {
        this.spawnPosition = spawnPosition;
    }

    void OnLevelWasLoaded(int level)
    {
        transform.position = spawnPosition;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
	
	void Update()
    {

	}

    void LevelUp()
    {
        level++;

        statPoints++;
    }
}
