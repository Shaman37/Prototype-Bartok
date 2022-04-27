using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundResultUI : MonoBehaviour
{
    private Text textRoundResult;

    private void Awake()
    {
        textRoundResult = GetComponent<Text>();
        textRoundResult.text = "";
    }

    private void Update()
    {
        if (Bartok.S.phase != eTurnState.gameOver)
        {
            textRoundResult.text = "";
            return;
        }

        Player player = Bartok.CURRENT_PLAYER;
        if (player == null || player.type == ePlayerType.human)
        {
            textRoundResult.text = "";
        }
        else
        {
            textRoundResult.text = "Player " + player.playerNum + " wins!";
        }   
    }
}
