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
        if (chatLines.Count > 0)
        {
            SetTextBoxText(chatLines.Dequeue());
        }
        else
        {
            PlayerController.EnableControls();
            IsChatActive = false;
            text.text = string.Empty;
            SetTextPanelVisibility(false);
        }
    }

    internal void StartChat(ChatLine[] lines)
    {
        chatLines = new Queue<ChatLine>(lines);
        SetTextBoxText(chatLines.Dequeue());
        PlayerController.DisableControls();
        IsChatActive = true;
        SetTextPanelVisibility(true);
    }

    private void SetTextBoxText(ChatLine chatLine)
    {
        text.color = chatLine.color;
        text.text = chatLine.text;
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
