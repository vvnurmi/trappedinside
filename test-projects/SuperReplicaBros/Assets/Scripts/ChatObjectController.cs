using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatObjectController : MonoBehaviour
{
    private bool playerInsideChatArea = false;
    private TextBoxController textBox;

    public string[] inputLines = new string[2];

    // Use this for initialization
    void Start()
    {
        textBox = FindObjectOfType<TextBoxController>();
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
