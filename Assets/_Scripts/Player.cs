using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayerType
{
    human,
    ai
}

[System.Serializable]
public class Player
{
    public ePlayerType      type = ePlayerType.ai;
    public int              playerNum;
    public SlotDef          handSlotDef;
    public List<CardBartok> hand;


    public CardBartok AddCard(CardBartok card)
    {
        if (hand == null)
        {
            hand = new List<CardBartok>();
        }

        hand.Add(card);

        if (type == ePlayerType.human)
        {
            CardBartok[] cards = hand.ToArray();
            cards = cards.OrderBy(cd => cd.rank).ToArray();

            hand = new List<CardBartok>(cards);
        }

        card.SetSortingLayerName("10");
        card.eventualSortLayer = handSlotDef.layerName;

        FanHand();

        return card;
    }


    public CardBartok RemoveCard(CardBartok card)
    {
        if (hand == null || !hand.Contains(card))
        {
            return null;
        }

        hand.Remove(card);
        FanHand();

        return card;
    }
    public void FanHand()
    {
        float startRot = 0;
        startRot = handSlotDef.rot;
        if (hand.Count > 1)
        {
            startRot += Bartok.S.handFanDegrees * (hand.Count - 1) / 2;
        }

        Vector3 pos;
        float rot;
        Quaternion rotQ;

        for (int i = 0; i < hand.Count; i++)
        {
            rot = startRot - Bartok.S.handFanDegrees * i;
            rotQ = Quaternion.Euler(0, 0, rot);

            pos = Vector3.up * CardBartok.CARD_HEIGHT / 2f;
            pos = rotQ * pos;
            pos += handSlotDef.pos;
            pos.z = -0.5f * i;

            hand[i].MoveTo(pos, rotQ);
            hand[i].state = eCardState.toHand;
            hand[i].faceUp = (type == ePlayerType.human);
            hand[i].eventualSortOrder = i * 4;
        }
    }
}
