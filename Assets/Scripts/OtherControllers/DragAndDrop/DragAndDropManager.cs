using UnityEngine.Events;

[System.Serializable]
public class CardEvent : UnityEvent<DragNDropCard> { };

public class DragAndDropManager : Singleton<DragAndDropManager>
{
    public CardEvent onDragStart = new CardEvent();
    public CardEvent onDragStop = new CardEvent();
    public DragNDropCard lastCard;

    public void OnDragStarted(DragNDropCard card)
    {
        lastCard = card;
        onDragStart.Invoke(card);
    }
    public void OnDragEnded(DragNDropCard card)
    {
        onDragStop.Invoke(card);
    }

    public void GetLastCardToDefaultPosition()
    {
        lastCard.GoToDefaultPosition();
    }
}
