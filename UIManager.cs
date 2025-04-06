using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] private XPBarScript xPBarScript;
    public InfoMessage infoMessage;
    public GameObject inventoryGamepadSelected;
    [SerializeField] private Color disabledTextColor;
    [SerializeField] private Color enabledTextColor;
    [SerializeField] private TooltipHandler tooltipHandler;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI maxHealth;
    [SerializeField] private TextMeshProUGUI playerDamage;
    [SerializeField] private TextMeshProUGUI playerDamageModifier;
    [SerializeField] private TextMeshProUGUI movementSpeed;

    [Header("Resistances")]
    [SerializeField] private TextMeshProUGUI physicalRes;
    [SerializeField] private TextMeshProUGUI iceRes;
    [SerializeField] private TextMeshProUGUI fireRes;
    [SerializeField] private TextMeshProUGUI poisonRes;
    [SerializeField] private TextMeshProUGUI lightningRes;
    [SerializeField] private TextMeshProUGUI darkRes;
    [SerializeField] private TextMeshProUGUI magicRes;

    [Header("PlayerClass")]
    [SerializeField] private TextMeshProUGUI className;
    [SerializeField] private TextMeshProUGUI stamina;
    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI staminaBenefitValue;
    [SerializeField] private TextMeshProUGUI strengthBenefitValue;

    [Header("Experience")]
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI currentLevelXPBar;
    [SerializeField] private TextMeshProUGUI currentXP;
    [SerializeField] private TextMeshProUGUI RemainingToNextLvlXP;
    [SerializeField] private TextMeshProUGUI RemainingToNextLvlXPbar;
    [SerializeField] private GameObject levelUpIcons;

    [Header("Treasures")]
    [SerializeField] private TextMeshProUGUI goldCount;
    [SerializeField] private TextMeshProUGUI diamondCount;
    [SerializeField] private TextMeshProUGUI emeraldCount;

    [Header("Quests")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questText;
    [SerializeField] private TextMeshProUGUI questObjectives;
    [SerializeField] private GameObject questFinishedPanel;
    [SerializeField] private TextMeshProUGUI questFinishedName;
    [SerializeField] private TextMeshProUGUI questFinishedText;
    [SerializeField] private GameObject questRewardPrefab;
    [SerializeField] private GameObject questRewardShinyXpPrefab;
    [SerializeField] private RectTransform questRewardAnchor;
    [SerializeField] private RectTransform questRewardShinyXpAnchor;
    [SerializeField] private RectTransform questFinishedRewardAnchor;
    [SerializeField] private RectTransform questFinishedRewardShinyXpAnchor;
    [SerializeField] private List<GameObject> questRewards;
    [SerializeField] private List<Sprite> shinyXpSprites;
    [SerializeField] private GameObject questObjectivesCompleteIcon;
 
    //QuestTracker
    public QuestTracker questTracker;

    [Header("Quest Tab")]
    [SerializeField] private GameObject questButtonPrefab;
    [SerializeField] private RectTransform questButtonsAnchor;
     [SerializeField] private RectTransform questTabRewardAnchor;
    [SerializeField] private RectTransform questTabRewardShinyXpAnchor;
    [SerializeField] private TextMeshProUGUI questTabTitle;
    [SerializeField] private TextMeshProUGUI questTabText;
    [SerializeField] private TextMeshProUGUI questTabObjectives;
    [SerializeField] private TextMeshProUGUI questTabActiveQuestsText;
    [SerializeField] private List<GameObject> questButtons;
    [SerializeField] private GameObject selectedQuestInfo;
    [SerializeField] private ScrollRect questScrollRect;

    [Header("Cartographer Tab")]
    [SerializeField] private GameObject cartographerPanel;

    [Header("Store")]
    [SerializeField] private GameObject storeUI;

    [Header("Crafting")]
    [SerializeField] private GameObject craftingUI;
    public CrafterScript crafterScript;
    [SerializeField] TabGroup craftingTabs;

    [Header("Talent Fixer")]
    [SerializeField] private GameObject talentResetPanel;
    [SerializeField] private TextMeshProUGUI talentResetText;
    [SerializeField] private Button talentResetButton;
    [SerializeField] private TextMeshProUGUI[] talentCost;

    private void Start()
    {
        questButtons = new List<GameObject>();
        GameManager.instance.inventory.gamepadFirstSelected = inventoryGamepadSelected;
        UpdateXPBar();
    }
    private void UpdateStatsDisplay()
    {
        PlayerClass playerClass = GameManager.instance.playerClass;

        maxHealth.text = GameManager.instance.health.currentHealth.ToString("0") + " / " + GameManager.instance.health.maxHealth.ToString("0");
        playerDamage.text = GameManager.instance.playerVariables.damage.ToString("0.0");
        playerDamageModifier.text = (GameManager.instance.playerVariables.damageModifier * 100).ToString("0") + " %";
        TextColor(GameManager.instance.playerVariables.damageModifier, true, playerDamageModifier);
        movementSpeed.text = (GameManager.instance.playerVariables.speedMultiplier * 100).ToString("0") + " %";
        TextColor(GameManager.instance.playerVariables.speedMultiplier, true, movementSpeed);

        stamina.text = playerClass.stamina.ToString();
        strength.text = playerClass.strength.ToString();
        staminaBenefitValue.text = playerClass.staminaBenefitValue.ToString();
        strengthBenefitValue.text = playerClass.strengthBenefitValue.ToString();
        className.text = playerClass.className.ToString();
    }
    private void UpdateResistanceDisplay()
    {
        var resistance = GameManager.instance.playerVariables;

        physicalRes.text = (100 - resistance.physicalDR * 100).ToString("0") + " %";
        TextColor(resistance.physicalDR, false, physicalRes);

        iceRes.text = (100 - resistance.frostDR * 100).ToString("0") + " %";
        TextColor(resistance.frostDR, false, iceRes);

        fireRes.text = (100 - resistance.fireDR * 100).ToString("0") + " %";
        TextColor(resistance.fireDR, false, fireRes);

        poisonRes.text = (100 - resistance.poisonDR * 100).ToString("0") + " %";
        TextColor(resistance.poisonDR, false, poisonRes);

        lightningRes.text = (100 - resistance.electricDR * 100).ToString("0") + " %";
        TextColor(resistance.electricDR, false, lightningRes);

        darkRes.text = (100 - resistance.darkDR * 100).ToString("0") + " %";
        TextColor(resistance.darkDR, false, darkRes);

        magicRes.text = (100 - resistance.magicDR * 100).ToString("0") + " %";
        TextColor(resistance.magicDR, false, magicRes);
    }

    private void TextColor(float value, bool valueOverHundredIsGreen, TextMeshProUGUI text)
    {
        switch (value)
        {
            case 1: 
                text.color = Color.white;
                break;
            case > 1: 
                if(valueOverHundredIsGreen)
                {
                    text.color = Color.green;
                }
                else
                {
                    text.color = Color.red;
                }
                break;
            case < 1:
                if(valueOverHundredIsGreen)
                {
                    text.color = Color.red;
                }
                else
                {
                    text.color = Color.green;
                }
                break;
        }
    }

    private void UpdateXPDisplay()
    {
        currentLevel.text = string.Format("Level: {0}", GameManager.instance.playerXP.currentLevel.ToString("0"));
        currentXP.text = string.Format("XP: {0} / {1}", GameManager.instance.playerXP.currentXP.ToString("0"), GameManager.instance.playerXP.nextLevelXP.ToString("0"));
        RemainingToNextLvlXP.text = string.Format("XP to level: {0}", (GameManager.instance.playerXP.nextLevelXP - GameManager.instance.playerXP.currentXP).ToString("0"));
    }

    public void UpdateTreasureDisplay()
    {
        goldCount.text = GameManager.instance.treasureCounter.goldCounter.ToString("0");
        diamondCount.text = GameManager.instance.treasureCounter.diamondCounter.ToString("0");
        emeraldCount.text = GameManager.instance.treasureCounter.emeraldCounter.ToString("0");
    }

    public void UpdateXPBar()
    {
        xPBarScript.SetXPSlider(GameManager.instance.playerXP.currentXP, GameManager.instance.playerXP.nextLevelXP);
        currentLevelXPBar.text = GameManager.instance.playerXP.currentLevel.ToString("0");
        RemainingToNextLvlXPbar.text = "LVL UP: " + (GameManager.instance.playerXP.nextLevelXP - GameManager.instance.playerXP.currentXP).ToString("0") + " XP";
        UpdateLevelUpStatus();
    }

    public void UpdateLevelUpStatus(bool inventoryOpen = false)
    {
        if(inventoryOpen)
        {
            levelUpIcons.SetActive(false);
        }
        else
        {
            {
                if (GameManager.instance.playerXP.HasUnspentTalentPoints())
                {
                    levelUpIcons.SetActive(true);
                }
                else
                {
                    levelUpIcons.SetActive(false);
                }
            }
        }
    }

    //TAB FUNCTIONS
    public void InventoryTabWasOpened()
    {
        tooltipHandler.ForceTooltipDisable();
        UpdateStatsDisplay();
        UpdateResistanceDisplay();
        UpdateXPDisplay();
        UpdateTreasureDisplay();
        DestroyQuestButtons();
        DestroyQuestRewards();
    }

    public void TalentsTabWasOpened()
    {   
        tooltipHandler.ForceTooltipDisable();
        DestroyQuestButtons();
        DestroyQuestRewards();
    }

    public void QuestsTabWasOpened()
    {
        tooltipHandler.ForceTooltipDisable();
        questScrollRect.verticalScrollbar.value = 1;
        selectedQuestInfo.SetActive(false);
        //questTabActiveQuestsText.text = "";
        DestroyQuestButtons();
        PopulateQuestsTab();
        if(questButtons.Count == 0)
        {
            questTabActiveQuestsText.text = "No active quests";
        }
        else
        {
            questTabActiveQuestsText.text = "Select a quest";
        }
    }

    public void TrackQuest(Quest quest)
    {
        questTracker.ActivateMovingText(quest);
    }

    //START QUEST
    public void OpenQuestPanel(Quest quest)
    {
        questName.text = quest.name;
        questText.text = quest.questDescription;
        questObjectives.text = BuildObjectiveText(quest);
        BuildShinyXPRewards(quest, questRewardShinyXpAnchor);
        BuildItemRewards(quest, questRewardAnchor);
        questPanel.SetActive(true);
    }
    public void CloseQuestPanel(bool acceptQuest)
    {
        questPanel.SetActive(false);
        if (acceptQuest)
        {
            FindObjectOfType<QuestGiverScript>().AddQuestToPlayer();
        }
        DestroyQuestRewards();
    }

    //END QUEST
    public void OpenQuestFinishedPanel(Quest quest)
    {
        questFinishedName.text = quest.name;
        questFinishedText.text = quest.completedText;
        BuildShinyXPRewards(quest, questFinishedRewardShinyXpAnchor);
        BuildItemRewards(quest, questFinishedRewardAnchor);
        questFinishedPanel.SetActive(true);
    }

    public void CloseQuestFinishedPanel(bool giveReward)
    {
        questFinishedPanel.SetActive(false);
        if(giveReward)
        {
            FindObjectOfType<QuestGiverScript>().GiveRewards();
        }
        DestroyQuestRewards();
    }

    //QUEST RELATED
    public void PopulateQuestsTab()
    {
        foreach (Quest quest in GameManager.instance.playerQuests.activeQuests)
        {
            GameObject newButton = Instantiate(questButtonPrefab, questButtonsAnchor);
            newButton.GetComponent<QuestButton>().SetButton(quest);
            questButtons.Add(newButton);
        }
    }
    public void QuestButtonWasClicked(Quest quest)
    {
        questObjectivesCompleteIcon.SetActive(false);
        if(quest.questIsCompleted)
        {
            questObjectivesCompleteIcon.SetActive(true);
        }
        questTabActiveQuestsText.text = "";
        DestroyQuestRewards();
        questTabTitle.text = quest.name;
        questTabText.text = quest.questDescription;
        questTabObjectives.text = BuildObjectiveTextQuestTab(quest);
        BuildShinyXPRewards(quest, questTabRewardShinyXpAnchor);
        BuildItemRewards(quest, questTabRewardAnchor);
        selectedQuestInfo.SetActive(true);
    }

    public void DestroyQuestButtons()
    {
        if (questButtons.Count == 0) return;
        foreach (GameObject button in questButtons)
        {
            Destroy(button);
        }
        questButtons.Clear();
    }

    public static string BuildObjectiveText(Quest quest)
    {
        string combinedObjectiveText = "";
        foreach (QuestObjective objective in quest.objectives)
        {
            combinedObjectiveText += objective.panelText.ToString() + "\n";
        }
        return combinedObjectiveText;
    }

    public static string BuildObjectiveTextQuestTab(Quest quest)
    {
        string combinedObjectiveText = "";
        foreach (QuestObjective objective in quest.objectives)
        {
            combinedObjectiveText += objective.objectiveText.ToString() + ": " + objective.progress.ToString() + " / " + objective.completeAmount.ToString() + "\n";
        }
        return combinedObjectiveText;
    }

    public void BuildShinyXPRewards(Quest quest, RectTransform anchorObject)
    {
        if (quest.xpReward > 0) BuildShinyXPItem(0, quest.xpReward, anchorObject);
        if (quest.goldReward > 0) BuildShinyXPItem(1, quest.goldReward, anchorObject);
        if (quest.diamondReward > 0) BuildShinyXPItem(2, quest.diamondReward, anchorObject);
    }

    private void BuildShinyXPItem(int spriteNumber, int reward, RectTransform anchorObject)
    {
        GameObject questReward = Instantiate(questRewardShinyXpPrefab, anchorObject);
        questReward.GetComponent<QuestRewardItem>().SetQuestShinyXpReward(shinyXpSprites[spriteNumber], reward.ToString()); 
        questRewards.Add(questReward);
    }

    public void BuildItemRewards(Quest quest, RectTransform anchorObject)
    {
        foreach(KeyValuePair<InventoryItemSO, int> item in quest.rewardsDictionary)
        {
            GameObject questReward = Instantiate(questRewardPrefab, anchorObject);
            questReward.GetComponent<QuestRewardItem>().SetQuestItemReward(item.Key.thisItem, tooltipHandler.titleRarityColor[(int)item.Key.itemRarity], item.Value.ToString());
            questRewards.Add(questReward);
        }
    }

    public void DestroyQuestRewards()
    {
        if(questRewards.Count == 0) return;

        foreach (GameObject questReward in questRewards)
        {
            Destroy(questReward);
        }
        questRewards.Clear();
    }

    //STORE
    public void OpenStoreUI(StoreSellerScript storeSeller)
    {
        storeUI.SetActive(true);
        storeUI.GetComponent<StoreUIScript>().BuildSlots(storeSeller);
    }

    //CRAFTING
    public void ToggleCraftingPanel()
    {
        if (!craftingUI.activeSelf)
        {
            craftingTabs.selectedTab = craftingTabs.defaultTab;
            craftingTabs.OnTabSelected(craftingTabs.defaultTab);
            craftingUI.SetActive(true);
            craftingUI.GetComponent<CraftingUIScript>().RefreshCraftingUI(crafterScript.craftable);
        }
        else
        {
            craftingUI.SetActive(false);
        }
    }

    //TALENT RESET
    public void OpenTalentResetPanel(int goldCost, int diamondCost)
    {
        talentResetPanel.SetActive(true);
        talentCost[0].text = goldCost.ToString();
        talentCost[1]. text = diamondCost.ToString();
        if (GameManager.instance.abilityUnlocks.unlockedTalents.Count > 0 && GameManager.instance.treasureCounter.goldCounter >= goldCost
            && GameManager.instance.treasureCounter.diamondCounter >= diamondCost)
        {
            talentResetButton.interactable = true;
            talentResetButton.GetComponentInChildren<TextMeshProUGUI>().color = enabledTextColor;
        }
        else
        {
            talentResetButton.interactable = false;
            talentResetButton.GetComponentInChildren<TextMeshProUGUI>().color = disabledTextColor;
        }
    }

    public void ResetTalents()
    {
        FindObjectOfType<TalentFixerScript>().ResetTalents();
        infoMessage.AddMessage("Talents reset!");
    }

    //USED BY Store, talents
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

}
