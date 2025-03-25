using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteStateSwap : MonoBehaviour
{
    private SpriteState activeState;
    private SpriteState inactiveState;

    [Header("Active State")]
    [SerializeField] private Sprite activeHighlightedSprite;
    [SerializeField] private Sprite activePressedSprite;
    [SerializeField] private Sprite activeSelectedSprite;
    [SerializeField] private Sprite activeDefaultSprite;

    [Header("Inactive State")]
    [SerializeField] private Sprite inactiveHighlightedSprite;
    [SerializeField] private Sprite inactivePressedSprite;
    [SerializeField] private Sprite inactiveSelectedSprite;
    [SerializeField] private Sprite inactiveDefaultSprite;

    private void Awake()
    {
        ActiveStateSprites();
        InactiveStateSprites();
    }

    private void ActiveStateSprites()
    {
        activeState.highlightedSprite = activeHighlightedSprite;
        activeState.pressedSprite = activePressedSprite;
        activeState.selectedSprite = activeSelectedSprite;
    }

    private void InactiveStateSprites()
    {
        inactiveState.highlightedSprite = inactiveHighlightedSprite;
        inactiveState.pressedSprite = inactivePressedSprite;
        inactiveState.selectedSprite = inactiveSelectedSprite;
    }

    public void ActiveState(Button button)
    {
        button.image.sprite = activeDefaultSprite;
        button.spriteState = activeState;
    }

    public void InactiveState(Button button)
    {
        button.image.sprite = inactiveDefaultSprite;
        button.spriteState = inactiveState;
    }
}
