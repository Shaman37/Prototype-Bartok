using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCBState
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
    public eCBState         state = eCBState.drawpile;
    public List<Vector3>    bezierPts;
    public List<Quaternion> bezierRots;
    public float            timeStart;
    public float            timeDuration;
    public GameObject       reportFinishTo = null;

    private void Update() {
        switch (state)
        {
            case eCBState.toHand:
            case eCBState.toTarget:
            case eCBState.toDrawpile:
            case eCBState.to:
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

                    if (state == eCBState.toHand) state = eCBState.hand;
                    if (state == eCBState.toTarget) state = eCBState.target;
                    if (state == eCBState.toDrawpile) state = eCBState.drawpile;
                    if (state == eCBState.to) state = eCBState.idle;

                    transform.localPosition = bezierPts[bezierPts.Count - 1];
                    transform.rotation = bezierRots[bezierRots.Count - 1];

                    timeStart = 0;

                    if (reportFinishTo != null)
                    {
                        reportFinishTo.SendMessage("CBCallback", this);
                        reportFinishTo = null;
                    }
                    else{ }
                }
                else
                {
                    Vector3 pos = Utils.Bezier(uC, bezierPts);
                    transform.localPosition = pos;
                    Quaternion rotQ = Utils.Bezier(uC, bezierRots);
                    transform.rotation = rotQ;
                }

                break;
        }
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

        state = eCBState.to;
    }

    public void MoveTo(Vector3 ePos)
    {
        MoveTo(ePos, Quaternion.identity);
    }
}
