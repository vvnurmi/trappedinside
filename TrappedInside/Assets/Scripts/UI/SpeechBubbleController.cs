using UnityEngine;

/// <summary>
/// The public interface to the Speech Bubble prefab which produces
/// a visual speech bubble.
/// 
/// Assumes that the Speech Bubble prefab has
/// - four TextMeshProUGUIs: speaker, text, leftChoice, rightChoice
/// - the text fields are marked with tags TiaSpeak.Tag*.
/// - two SpriteRenderers: speechBubble, stem
/// - the smaller sprite is the stem
/// 
/// Maintains the relative positioning of the text fields in the prefab
/// as the desired size of the bubble changes.
/// </summary>
public class SpeechBubbleController : MonoBehaviour
{
    private SpriteRenderer backgroundSprite;
    private RectTransform backgroundSpriteTransform;
    private TMPro.TextMeshProUGUI speakerField;
    private TMPro.TextMeshProUGUI textField;
    private TMPro.TextMeshProUGUI leftChoiceField;
    private TMPro.TextMeshProUGUI rightChoiceField;

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
    /// The location and dimensions of the speech bubble, in world coordinates.
    /// </summary>
    public Rect Extent
    {
        get => extent;
        set
        {
            extent = value;

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

            // TODO: Align the left and right choice fields.
            leftChoiceField.rectTransform.localPosition = new Vector2(-0.2f, -0.1f);
            rightChoiceField.rectTransform.localPosition = new Vector2(0.2f, -0.1f);
        }
    }
    private Rect extent;

    public void Start()
    {
        {
            var sprites = GetComponentsInChildren<SpriteRenderer>();
            Debug.Assert(sprites.Length == 2);
            var backgroundSpriteIndex = sprites[0].size.x < sprites[1].size.x ? 1 : 0;
            backgroundSprite = sprites[backgroundSpriteIndex];
            backgroundSpriteTransform = backgroundSprite.GetComponent<RectTransform>();
            Debug.Assert(backgroundSpriteTransform != null);
        }

        var textFields = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
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
    }
}
