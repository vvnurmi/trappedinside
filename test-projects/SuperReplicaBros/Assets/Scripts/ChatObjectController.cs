using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ChatLine
{
    public Color color;
    public string text;
}

public class ChatObjectController : MonoBehaviour
{
    private bool playerInsideChatArea = false;
    private TextBoxController textBox;

    public ChatLine[] inputLines;

    // Use this for initialization
    void Start()
    {
        textBox = FindObjectOfType<TextBoxController>();
        inputLines = new ChatLine[] 
        {
            new ChatLine { color = new Color(1, 1, 0), text = "Beware of the monsters ahead!" },
            new ChatLine { color = new Color(1, 0, 0), text = "No worries, I will punch 'em!" }

        };
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            if (playerInsideChatArea && !textBox.IsChatActive)
            {
                textBox.StartChat(inputLines);
            }
            else if (textBox.IsChatActive)
            {
                textBox.ProgressChat();
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ColliderIsPlayer(collision))
        {
            playerInsideChatArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (ColliderIsPlayer(collision))
        {
            playerInsideChatArea = false;
        }
    }

    private bool ColliderIsPlayer(Collider2D collision)
    {
        return collision.gameObject.tag == "Player";
    }

}
