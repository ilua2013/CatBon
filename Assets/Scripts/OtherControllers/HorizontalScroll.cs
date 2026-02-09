using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ColorCardsGroup
{
    public List<ColorCard> cards;
}
public class HorizontalScroll : MonoBehaviour
{
    public List<ColorCardsGroup> availableCards;
    public ScrollSlot[] slots;
    public int currentCardGroup = 0;
    public int showGroupOnStart = 0;
    public Button next;
    public Button prev;

    private void Start()
    {
        if (showGroupOnStart >= 0)
        {
            ShowCardsGroup(showGroupOnStart);
        }
    }


    public void Next()
    {
        currentCardGroup++;
        ShowCardsGroup(currentCardGroup);
    }

    public void Prev()
    {
        currentCardGroup--;
        ShowCardsGroup(currentCardGroup);
    }

    public void ShowCardsGroup(int index)
    {
        currentCardGroup = index;
        if (prev)
        {
            if (currentCardGroup == 0) prev.interactable = false;
            else prev.interactable = true;
        }
        if (next)
        {
            if (currentCardGroup == availableCards.Count - 1) next.interactable = false;
            else next.interactable = true;
        }
        for (int i = 0; i < availableCards[index].cards.Count; i++)
        {
            slots[i].SetCard(availableCards[index].cards[i]);
        }
    }
}
