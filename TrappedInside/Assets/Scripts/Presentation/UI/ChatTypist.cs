using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChatTypist : MonoBehaviour
{
    public float lineInterval;
    public string[] lines;

    private Queue<string> _lines = new Queue<string>();
    private float _previousTypingTime;
    private TMPro.TextMeshProUGUI _textField;

    void Start()
    {
        _previousTypingTime = Time.time;
        foreach(var line in lines)
            _lines.Enqueue(line);
    }

    void Awake()
    {
        _textField = GetComponentsInChildren<TMPro.TextMeshProUGUI>(includeInactive: true)[0];
        Debug.Assert(_textField != null);
    }

    void FixedUpdate()
    {
        if(Time.time < _previousTypingTime + lineInterval) return;

        if(_lines.Count > 0)
            _textField.text += $"{_lines.Dequeue()}\n";

        _previousTypingTime = Time.time;
    }

}
