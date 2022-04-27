using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private Text textGameOver;

    private void Awake()
    {
        textGameOver = GetComponent<Text>();
        textGameOver.text = "";
    }

    private void Update()
    {
        if (Bartok.S.phase != eTurnState.gameOver)
        {
            textGameOver.text = "";
            return;
        }

        if (Bartok.CURRENT_PLAYER == null) return;
        if (Bartok.CURRENT_PLAYER.type == ePlayerType.human)
        {
            textGameOver.text = "You won!";
        }
        else
        {
            textGameOver.text = "Game Over";
        }
    }
}
