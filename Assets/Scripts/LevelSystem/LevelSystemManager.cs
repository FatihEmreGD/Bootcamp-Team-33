using UnityEngine;

public class LevelSystemManager : MonoBehaviour
{
    public static LevelSystemManager Instance { get; private set; }

    public LevelSystem levelSystem;

    [SerializeField] private int level;
    [SerializeField] private float experienceNormalized;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        levelSystem = new LevelSystem();
        levelSystem.OnLevelChanged += LevelSystem_OnLevelChanged;
        levelSystem.OnExperienceChanged += LevelSystem_OnExperienceChanged;
        UpdateVisuals();
    }

    private void Update()
    {

        //levelSystem.AddExperience(25);
    }

    private void LevelSystem_OnLevelChanged(object sender, System.EventArgs e)
    {
        UpdateVisuals();
        Debug.Log("Leveled up! New level: " + levelSystem.GetLevelNumber());
    }

    private void LevelSystem_OnExperienceChanged(object sender, System.EventArgs e)
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        level = levelSystem.GetLevelNumber();
        experienceNormalized = levelSystem.GetExperienceNormalized();
    }
} 