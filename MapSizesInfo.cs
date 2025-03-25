using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSizesInfo : MonoBehaviour
{
    public bool unlocked;
    public bool selected;
    public MapSize mapSize;
    public GameObject mapUnlockIcon;
    [SerializeField] private TextMeshProUGUI mapSizeName;
    public GameObject cost;
    [SerializeField] private GameObject gold;
    [SerializeField] private GameObject diamond;
    [SerializeField] private GameObject emerald;
    [SerializeField] private TextMeshProUGUI mapContent;
    public GameObject mapSizeFrame;
    public Button button; 

    private void OnEnable()
    {
        mapSizeName.text = mapSize.sizeName;
        mapContent.text = mapSize.mapContent.ToString();

        gold.GetComponentInChildren<TextMeshProUGUI>().text = mapSize.goldCost.ToString();
        diamond.GetComponentInChildren<TextMeshProUGUI>().text = mapSize.diamondCost.ToString();
        emerald.GetComponentInChildren<TextMeshProUGUI>().text = mapSize.emeraldCost.ToString();

        if(mapSize.emeraldCost <= 0)
        {
            emerald.SetActive(false);
        }
    }
}
