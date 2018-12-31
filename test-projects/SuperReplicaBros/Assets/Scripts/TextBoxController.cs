using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxController : MonoBehaviour
{
    private Text text;
    private Queue<ChatLine> chatLines = new Queue<ChatLine>();
    private Image textPanel;
    private bool isTyping = false;
    private bool cancelTyping = false;

    public float typeSpeed = 0.1f;

    private PlayerController PlayerController { get { return FindObjectOfType<PlayerController>(); } }

    // Use this for initialization
    void Start()
    {
        text = GetComponentInChildren<Text>();
        textPanel = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    internal bool IsChatActive { get; private set; }

    internal void ProgressChat()
    {
        if (!isTyping)
        {
            if (chatLines.Count > 0)
            {
                StartScrollingText();
            }
            else
            {
                PlayerController.EnableControls();
                IsChatActive = false;
                text.text = string.Empty;
                SetTextPanelVisibility(false);
            }
        }
        else
        {
            cancelTyping = true;
        }

    }

    private IEnumerator TextScroll(ChatLine chatLine)
    {
        isTyping = true;
        text.text = string.Empty;
        text.color = chatLine.color;
        foreach(var c in chatLine.text)
        {
            if(cancelTyping)
            {
                break;
            }
            text.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        text.text = chatLine.text;
        isTyping = false;
        cancelTyping = false;

    }

    internal void StartChat(ChatLine[] lines)
    {
        chatLines = new Queue<ChatLine>(lines);
        StartScrollingText();
        PlayerController.DisableControls();
        IsChatActive = true;
        SetTextPanelVisibility(true);
    }

    private void StartScrollingText()
    {
        StartCoroutine(TextScroll(chatLines.Dequeue()));
    }

    private void SetTextPanelVisibility(bool visible)
    {
        if (visible)
        {
            textPanel.color = new Color(0, 0, 0, 0.8f);
        }
        else
        {
            textPanel.color = new Color(0, 0, 0, 0);
        }
    }
}
