using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider experienceSlider;

    private LevelSystem levelSystem;

    private void Start()
    {
        if (LevelSystemManager.Instance != null)
        {
            SetLevelSystem(LevelSystemManager.Instance.levelSystem);
        }
        else
        {
            Debug.LogError("LevelSystemManager.Instance is null. Make sure a LevelSystemManager exists in the scene.");
        }
    }

    public void SetLevelSystem(LevelSystem ls)
    {
        this.levelSystem = ls;

        // Update the starting values
        UpdateLevelText();
        UpdateExperienceBar();

        // Subscribe to the events
        levelSystem.OnLevelChanged += LevelSystem_OnLevelChanged;
        levelSystem.OnExperienceChanged += LevelSystem_OnExperienceChanged;
    }

    private void OnDestroy()
    {
        if (levelSystem != null)
        {
            levelSystem.OnLevelChanged -= LevelSystem_OnLevelChanged;
            levelSystem.OnExperienceChanged -= LevelSystem_OnExperienceChanged;
        }
    }

    private void LevelSystem_OnLevelChanged(object sender, System.EventArgs e)
    {
        UpdateLevelText();
    }

    private void LevelSystem_OnExperienceChanged(object sender, System.EventArgs e)
    {
        UpdateExperienceBar();
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "LEVEL " + levelSystem.GetLevelNumber();
        }
    }

    private void UpdateExperienceBar()
    {
        if (experienceSlider != null)
        {
            experienceSlider.value = levelSystem.GetExperienceNormalized();
        }
    }
} 