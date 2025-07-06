using System;

public class LevelSystem
{
    public event EventHandler OnLevelChanged;
    public event EventHandler OnExperienceChanged;

    private int level;
    private int experience;
    private int experienceToNextLevel;

    public LevelSystem()
    {
        level = 1;
        experience = 0;
        experienceToNextLevel = 100;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        while (experience >= experienceToNextLevel)
        {
            level++;
            experience -= experienceToNextLevel;
            experienceToNextLevel = CalculateExperienceToNextLevel(level);
            if (OnLevelChanged != null) OnLevelChanged(this, EventArgs.Empty);
        }
        if (OnExperienceChanged != null) OnExperienceChanged(this, EventArgs.Empty);
    }

    private int CalculateExperienceToNextLevel(int currentLevel)
    {
        return 100 * currentLevel;
    }

    public int GetLevelNumber()
    {
        return level;
    }

    public float GetExperienceNormalized()
    {
        return (float)experience / experienceToNextLevel;
    }
} 