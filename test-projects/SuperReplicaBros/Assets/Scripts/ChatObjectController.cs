using System;
using UnityEngine;

[Serializable]
public struct ChatLine
{
    public Color color;
    [TextArea]
    public string text;
}

public class ChatObjectController : MonoBehaviour
{
    private bool playerInsideChatArea = false;
    private TextBoxController textBox;

    public ChatLine[] inputLines;

    // Use this for initialization
    private void Start()
    {
        textBox = FindObjectOfType<TextBoxController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
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
        playerInsideChatArea = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInsideChatArea = false;
    }
}
