using System.Linq;
using UnityEngine;

/// <summary>
/// The public interface to the Speech Bubble prefab which produces
/// a visual speech bubble.
/// 
/// Assumes that the Speech Bubble prefab has
/// - four TextMeshProUGUIs: speaker, text, leftChoice, rightChoice
/// - the text fields are marked with tags TiaSpeak.Tag*.
/// - three SpriteRenderers: speechBubble, stem, prompt
/// - the smallest sprite is the prompt, the largest is speechBubble
/// 
/// Maintains the relative positioning of the text fields in the prefab
/// as the desired size of the bubble changes.
/// </summary>
public class SpeechBubbleController : MonoBehaviour
{
    /// <summary>
    /// Maximum line width in the text field, in speech bubble coordinates.
    /// </summary>
    public float maxLineWidth = 1;

    [Tooltip("Maximum displacement of the prompt as it waves, in speech bubble coordinates.")]
    public float promptWaveAmplitude = 0.01f;
    [Tooltip("How many back-and-forth waves the prompt does in a second.")]
    public float promptWaveFrequency = 1.0f;

    private GameObject prompt;
    private Renderer promptRenderer;
    private SpriteRenderer backgroundSprite;
    private RectTransform backgroundSpriteTransform;
    private TMPro.TextMeshProUGUI speakerField;
    private TMPro.TextMeshProUGUI textField;
    private TMPro.TextMeshProUGUI leftChoiceField;
    private TMPro.TextMeshProUGUI rightChoiceField;
    
    #region Margin fields

    /// <summary>
    /// Distance from the root object position to the bottom edge of the background sprite,
    /// in speech bubble coordinates.
    /// </summary>
    private float backgroundBottomMargin;

    /// <summary>
    /// Distance from the top edge of the speech bubble sprite to the top edge of the speaker field,
    /// in speech bubble coordinates.
    /// </summary>
    private float speakerTopMargin;

    /// <summary>
    /// Distance from the left edge of the speech bubble sprite to the left edge of the speaker field,
    /// in speech bubble coordinates.
    /// </summary>
    private float speakerLeftMargin;

    /// <summary>
    /// Distance from the right edge of the speech bubble sprite to the right edge of the speaker field,
    /// in speech bubble coordinates.
    /// </summary>
    private float speakerRightMargin;

    /// <summary>
    /// Distance from the top edge of the speech bubble sprite to the top edge of the text field,
    /// in speech bubble coordinates.
    /// </summary>
    private float textTopMargin;

    /// <summary>
    /// Distance from the bottom edge of the speech bubble sprite to the bottom edge of the text field,
    /// in speech bubble coordinates.
    /// </summary>
    private float textBottomMargin;

    /// <summary>
    /// Distance from the left edge of the speech bubble sprite to the left edge of the text field,
    /// in speech bubble coordinates.
    /// </summary>
    private float textLeftMargin;

    /// <summary>
    /// Distance from the right edge of the speech bubble sprite to the right edge of the text field,
    /// in speech bubble coordinates.
    /// </summary>
    private float textRightMargin;

    /// <summary>
    /// Distance from the right edge of the speech bubble sprite to the prompt origin,
    /// in speech bubble coordinates.
    /// </summary>
    private float promptRightMargin;

    #endregion

    public bool IsInitialized { get; private set; }

    private string speakerCache;
    public string Speaker
    {
        get
        {
            return speakerField?.text ?? speakerCache;
        }
        set
        {
            if (speakerField != null) speakerField.text = value;
            speakerCache = value;
        }
    }

    private string textCache;
    public string Text
    {
        get
        {
            return textField?.text ?? textCache;
        }
        set
        {
            if (textField != null) textField.text = value;
            textCache = value;
        }
    }

    private bool isPromptVisibleCache;
    public bool IsPromptVisible
    {
        get
        {
            return promptRenderer?.enabled ?? isPromptVisibleCache;
        }
        set
        {
            if (promptRenderer != null) promptRenderer.enabled = value;
            isPromptVisibleCache = value;
        }
    }

    /// <summary>
    /// Returns an estimate size for the speech bubble to be able to host the given text.
    /// </summary>
    public Vector2 EstimateSize(string text)
    {
        Debug.Assert(textField != null);
        // Note: TextMesh Pro 2.1.4 seems to set the text when calling GetPreferredValues, even though
        // forum posts suggest that the text wouldn't be set.
        var oldText = textField.text;
        var oneLineSize = textField.GetPreferredValues(text);
        textField.text = oldText;
        var linesEstimate = Mathf.Ceil(oneLineSize.x / maxLineWidth);
        var textSize = new Vector2(
            Mathf.Clamp(oneLineSize.x, 0, maxLineWidth),
            oneLineSize.y * linesEstimate);
        return textSize + new Vector2(
            textLeftMargin + textRightMargin,
            textTopMargin + textBottomMargin);
    }

    public void Hide()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    public void Show()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);
    }

    /// <summary>
    /// Set the location and dimensions of the speech bubble, in world coordinates.
    /// </summary>
    public void SetExtent(Rect extent)
    {
        Debug.Assert(IsInitialized);

        // Position the speech bubble root object.
        gameObject.transform.localPosition = extent.position;

        // Set the size of the background sprite and its game object.
        backgroundSprite.size = extent.size;
        backgroundSpriteTransform.sizeDelta = extent.size;

        // Enforce the fixed margin of the background sprite to the root object
        // by its bottom edge.
        backgroundSpriteTransform.localPosition = new Vector3(
            backgroundSpriteTransform.localPosition.x,
            extent.size.y / 2 + backgroundBottomMargin,
            backgroundSpriteTransform.localPosition.z);

        // Enforce the fixed margins of the speaker field to the background sprite
        // by the top, left, and right edges.
        {
            var top = backgroundSprite.size.y / 2 - speakerTopMargin;
            var bottom = top - speakerField.rectTransform.sizeDelta.y;
            var left = -backgroundSprite.size.x / 2 + speakerLeftMargin;
            var right = backgroundSprite.size.x / 2 - speakerRightMargin;
            speakerField.rectTransform.sizeDelta = new Vector2(right - left, top - bottom);
            speakerField.rectTransform.localPosition = new Vector3(
                (right + left) / 2,
                (top + bottom) / 2,
                speakerField.rectTransform.localPosition.z);
        }

        // Enforce the fixed margins of the text field to the background sprite
        // by the top, bottom, left, and right edges.
        {
            var top = backgroundSprite.size.y / 2 - textTopMargin;
            var bottom = -backgroundSprite.size.y / 2 + textBottomMargin;
            var left = -backgroundSprite.size.x / 2 + textLeftMargin;
            var right = backgroundSprite.size.x / 2 - textRightMargin;
            textField.rectTransform.sizeDelta = new Vector2(right - left, top - bottom);
            textField.rectTransform.localPosition = new Vector3(
                (right + left) / 2,
                (top + bottom) / 2,
                textField.rectTransform.localPosition.z);
        }

        // Enforce the fixed margin of the prompt.
        UpdatePromptPosition();

        // TODO: Align the left and right choice fields.
        leftChoiceField.rectTransform.localPosition = new Vector2(-0.2f, -0.1f);
        rightChoiceField.rectTransform.localPosition = new Vector2(0.2f, -0.1f);
    }

    private void UpdatePromptPosition()
    {
        var time = Time.time;
        var waveDisplacement = Mathf.Sin(time * Mathf.PI * 2 * promptWaveFrequency) * promptWaveAmplitude;
        prompt.transform.localPosition = new Vector3(
            backgroundSprite.size.x / 2 - promptRightMargin + waveDisplacement,
            prompt.transform.localPosition.y,
            prompt.transform.localPosition.z);
    }

    #region MonoBehaviour overrides

    public void Start()
    {
        // Identify child sprites.
        {
            var sprites = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            Debug.Assert(sprites.Length == 3);
            var spriteIndicesSorted =
                Enumerable.Range(0, sprites.Length)
                .OrderBy(i => sprites[i].size.x)
                .ToArray();
            var promptSpriteIndex = spriteIndicesSorted[0];
            var backgroundSpriteIndex = spriteIndicesSorted[2];
            prompt = sprites[promptSpriteIndex].gameObject;
            promptRenderer = prompt.GetComponent<Renderer>();
            Debug.Assert(promptRenderer != null);
            backgroundSprite = sprites[backgroundSpriteIndex];
            backgroundSpriteTransform = backgroundSprite.GetComponent<RectTransform>();
            Debug.Assert(backgroundSpriteTransform != null);
        }

        // Identify child text fields.
        var textFields = GetComponentsInChildren<TMPro.TextMeshProUGUI>(includeInactive: true);
        foreach (var field in textFields)
        {
            if (field.gameObject.CompareTag(TiaSpeak.TagText))
                textField = field;
            if (field.gameObject.CompareTag(TiaSpeak.TagSpeaker))
                speakerField = field;
            if (field.gameObject.CompareTag(TiaSpeak.TagLeft))
                leftChoiceField = field;
            if (field.gameObject.CompareTag(TiaSpeak.TagRight))
                rightChoiceField = field;
        }
        Debug.Assert(textField != null);
        Debug.Assert(speakerField != null);
        Debug.Assert(leftChoiceField != null);
        Debug.Assert(rightChoiceField != null);

        // Make note of preset margins to be able to preserve them later.
        backgroundBottomMargin = -backgroundSprite.size.y / 2
            + backgroundSpriteTransform.localPosition.y;

        speakerTopMargin = backgroundSprite.size.y / 2
            - speakerField.rectTransform.sizeDelta.y / 2
            - speakerField.rectTransform.localPosition.y;
        speakerLeftMargin = backgroundSprite.size.x / 2
            - speakerField.rectTransform.sizeDelta.x / 2
            + speakerField.rectTransform.localPosition.x;
        speakerRightMargin = backgroundSprite.size.x / 2
            - speakerField.rectTransform.sizeDelta.x / 2
            - speakerField.rectTransform.localPosition.x;

        textTopMargin = backgroundSprite.size.y / 2
            - textField.rectTransform.sizeDelta.y / 2
            - textField.rectTransform.localPosition.y;
        textBottomMargin = backgroundSprite.size.y / 2
            - textField.rectTransform.sizeDelta.y / 2
            + textField.rectTransform.localPosition.y;
        textLeftMargin = backgroundSprite.size.x / 2
            - textField.rectTransform.sizeDelta.x / 2
            + textField.rectTransform.localPosition.x;
        textRightMargin = backgroundSprite.size.x / 2
            - textField.rectTransform.sizeDelta.x / 2
            - textField.rectTransform.localPosition.x;

        promptRightMargin = backgroundSprite.size.x / 2
            - prompt.transform.localPosition.x;

        // Set child object states according to our cached values.
        speakerField.text = speakerCache;
        textField.text = textCache;
        IsPromptVisible = isPromptVisibleCache;

        IsInitialized = true;
    }

    public void FixedUpdate()
    {
        if (IsPromptVisible)
            UpdatePromptPosition();
    }

    #endregion
}
