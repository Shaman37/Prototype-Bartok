using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState
{
    toDrawpile,
    drawpile,
    toHand,
    hand,
    toTarget,
    target,
    discard,
    to,
    idle
}

public class CardBartok : Card
{
    static public float  MOVE_DURATION = 0.5f;
    static public string MOVE_EASING = Easing.InOut;
    static public float  CARD_HEIGHT = 3.5f;
    static public float  CARD_WIDTH = 2f;

    [Header("Set Dynamically: CardBartok")]
    public eCardState         state = eCardState.drawpile;
    public List<Vector3>    bezierPts;
    public List<Quaternion> bezierRots;
    public float            timeStart;
    public float            timeDuration;
    public GameObject       reportFinishTo = null;
    public int              eventualSortOrder;
    public string           eventualSortLayer;
    
    [System.NonSerialized] 
    public Player           callbackPlayer = null;

    private void Update() {
        switch (state)
        {
            case eCardState.toHand:
            case eCardState.toTarget:
            case eCardState.toDrawpile:
            case eCardState.to:
                float u = (Time.time - timeStart) / timeDuration;
                float uC = Easing.Ease(u, MOVE_EASING);

                if (u < 0)
                {
                    transform.localPosition = bezierPts[0];
                    transform.rotation = bezierRots[0];
                    return;
                }
                else if (u >= 1)
                {
                    uC = 1;

                    if (state == eCardState.toHand) state = eCardState.hand;
                    if (state == eCardState.toTarget) state = eCardState.target;
                    if (state == eCardState.toDrawpile) state = eCardState.drawpile;
                    if (state == eCardState.to) state = eCardState.idle;

                    transform.localPosition = bezierPts[bezierPts.Count - 1];
                    transform.rotation = bezierRots[bezierRots.Count - 1];

                    timeStart = 0;

                    if (reportFinishTo != null)
                    {
                        reportFinishTo.SendMessage("CBCallback", this);
                        reportFinishTo = null;
                    }
                    else if (callbackPlayer != null)
                    {
                        callbackPlayer.CBCallback(this);
                        callbackPlayer = null;
                    }
                }
                else
                {
                    Vector3 pos = Utils.Bezier(uC, bezierPts);
                    transform.localPosition = pos;
                    Quaternion rotQ = Utils.Bezier(uC, bezierRots);
                    transform.rotation = rotQ;

                    if (u > 0.5f)
                    {
                        SpriteRenderer sRend = spriteRenderers[0];
                        if (sRend.sortingOrder != eventualSortOrder)
                        {
                            SetSortOrder(eventualSortOrder);
                        }

                        if (sRend.sortingLayerName != eventualSortLayer)
                        {
                            SetSortingLayerName(eventualSortLayer);
                        }
                    }
                }

                break;
        }
    }

    override public void OnMouseUpAsButton()
    {
        Bartok.S.CardClicked(this);
        base.OnMouseUpAsButton();
    }

    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        bezierPts = new List<Vector3>();
        bezierPts.Add(transform.localPosition);
        bezierPts.Add(ePos);

        bezierRots = new List<Quaternion>();
        bezierRots.Add(transform.rotation);
        bezierRots.Add(eRot);

        if (timeStart == 0)
        {
            timeStart = Time.time;
        }
        timeDuration = MOVE_DURATION;

        state = eCardState.to;
    }

    public void MoveTo(Vector3 ePos)
    {
        MoveTo(ePos, Quaternion.identity);
    }
}
