using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LevelerAggregateType { Sum, Max, Min}
public enum LevelerRequirementType { BankAmount, BankFrequency, LevelerLevel}
[Serializable]
public class Level
{
    public Transform Model;
    public List<Transform> Enables;
    public List<LevelRequirement> requirements;
}
[Serializable]
public class LevelRequirement { 
    public float Requirement;
    public LevelerRequirementType requirementType;
    public List<Bank> banks;
    public List<LevelerBehaviour> levelers;
    public LevelerAggregateType aggregateType;
}

public class LevelerBehaviour : MonoBehaviour {

    public int currentLevel = 0;
    public List<Level> levels = new List<Level>();
    public Transform modelParent;

	// Use this for initialization
	void Start () {
        UpdateModel();
	}
	
	// Update is called once per frame
	void Update () {
        if (LevelMeetsGoal(levels[currentLevel]) && levels.Count > currentLevel+1){
            //promote
            currentLevel += 1;
            UpdateModel();
            EnableEnables();
        }
        else if (currentLevel > 0 && !LevelMeetsGoal(levels[currentLevel-1]))
        {
            //demote
            DisableEnables();
            currentLevel -= 1;
            UpdateModel();
        }
	}

    private void UpdateModel()
    {
        if (currentLevel < 0 || currentLevel >= levels.Count)
            return;
        foreach (Transform child in modelParent)
        {
            child.gameObject.SetActive(false);
        }
        levels[currentLevel].Model.gameObject.SetActive(true);
        //Transform newModel = Instantiate(levels[currentLevel].Model, modelParent.position, modelParent.rotation, modelParent);
    }

    private void EnableEnables()
    {
        var enables = levels[currentLevel].Enables;
        foreach(var enable in enables)
        {
            enable.gameObject.SetActive(true);
        }
    }

    private void DisableEnables()
    {
        var enables = levels[currentLevel].Enables;
        foreach (var enable in enables)
        {
            enable.gameObject.SetActive(false);
        }
    }

    private bool LevelMeetsGoal(Level level)
    {
        return !level.requirements.Any(x => !RequirementMeetsGoal(x));
    }

    private bool RequirementMeetsGoal(LevelRequirement levelRequirement)
    {
        if (levelRequirement.requirementType == LevelerRequirementType.BankFrequency)
        {
            var amount = 0f;
            switch (levelRequirement.aggregateType)
            {
                case LevelerAggregateType.Sum:
                    amount = levelRequirement.banks.Where(x=>x.gameObject.activeInHierarchy).Sum(x => x.DeplositFrequency);
                    break;
                case LevelerAggregateType.Min:
                    amount = levelRequirement.banks.Where(x => x.gameObject.activeInHierarchy).Min(x => x.DeplositFrequency);
                    break;
                case LevelerAggregateType.Max:
                    amount = levelRequirement.banks.Where(x => x.gameObject.activeInHierarchy).Max(x => x.DeplositFrequency);
                    break;
                default:
                    amount = levelRequirement.banks.Where(x => x.gameObject.activeInHierarchy).Sum(x => x.DeplositFrequency);
                    break;
            }
            return amount >= levelRequirement.Requirement;
        }
        
        else if(levelRequirement.requirementType == LevelerRequirementType.BankAmount)
        {
            var amount = 0f;
            switch (levelRequirement.aggregateType)
            {
                case LevelerAggregateType.Sum:
                    amount = levelRequirement.banks.Where(x => x.gameObject.activeInHierarchy).Sum(x => x.Amount);
                    break;
                case LevelerAggregateType.Min:
                    amount = levelRequirement.banks.Where(x => x.gameObject.activeInHierarchy).Min(x => x.Amount);
                    break;
                case LevelerAggregateType.Max:
                    amount = levelRequirement.banks.Where(x => x.gameObject.activeInHierarchy).Max(x => x.Amount);
                    break;
                default:
                    amount = levelRequirement.banks.Where(x => x.gameObject.activeInHierarchy).Sum(x => x.Amount);
                    break;
            }
            return amount >= levelRequirement.Requirement;
        }

        else//if (levelRequirement.requirementType == LevelerRequirementType.LevelerLevel)
        {
            var amount = 0f;
            switch (levelRequirement.aggregateType)
            {
                case LevelerAggregateType.Sum:
                    amount = levelRequirement.levelers.Where(x => x.gameObject.activeInHierarchy).Sum(x => x.currentLevel);
                    break;
                case LevelerAggregateType.Min:
                    amount = levelRequirement.levelers.Where(x => x.gameObject.activeInHierarchy).Min(x => x.currentLevel);
                    break;
                case LevelerAggregateType.Max:
                    amount = levelRequirement.levelers.Where(x => x.gameObject.activeInHierarchy).Max(x => x.currentLevel);
                    break;
                default:
                    amount = levelRequirement.levelers.Where(x => x.gameObject.activeInHierarchy).Sum(x => x.currentLevel);
                    break;
            }
            return amount >= levelRequirement.Requirement;
        }
    }
}
