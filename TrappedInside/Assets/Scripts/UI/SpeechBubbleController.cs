using System.Linq;
using UnityEngine;

/// <summary>
/// The public interface to the Speech Bubble prefab which produces
/// a visual speech bubble.
/// 
/// Assumes that the Speech Bubble prefab has four text fields:
/// speaker, text, leftChoice, rightChoice. They should be marked with
/// appropriate tags.
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
            backgroundSpriteTransform.sizeDelta = extent.size;
            backgroundSprite.size = extent.size;
            gameObject.transform.localPosition = extent.position;

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

            leftChoiceField.rectTransform.localPosition = new Vector2(-0.2f, -0.1f);
            rightChoiceField.rectTransform.localPosition = new Vector2(0.2f, -0.1f);
        }
    }
    private Rect extent;

    public void Start()
    {
        backgroundSprite = GetComponentsInChildren<SpriteRenderer>().Single();
        Debug.Assert(backgroundSprite != null);
        backgroundSpriteTransform = backgroundSprite.GetComponent<RectTransform>();
        Debug.Assert(backgroundSpriteTransform != null);

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
