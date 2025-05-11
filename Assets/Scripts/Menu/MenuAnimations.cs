using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MenuAnimations : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private float staggerDelay = 0.1f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverDuration = 0.2f;

    [Header("Optional Effects")]
    [SerializeField] private bool useRotationEffect = true;
    [SerializeField] private float rotationAmount = 2f;
    [SerializeField] private bool useFadeEffect = true;
    [SerializeField] private CanvasGroup[] buttonGroups;

    private void Start()
    {
        InitializeButtons();
        AnimateButtonsIn();
    }

    private void InitializeButtons()
    {
        // Initialize all buttons to be invisible
        foreach (var button in buttons)
        {
            button.localScale = Vector3.zero;
            
            // Add hover listeners
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
            
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => { OnButtonHover(button); });
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { OnButtonExit(button); });
            trigger.triggers.Add(exitEntry);
        }

        if (useFadeEffect && buttonGroups != null)
        {
            foreach (var group in buttonGroups)
            {
                if (group != null) group.alpha = 0;
            }
        }
    }

    private void AnimateButtonsIn()
    {
        // Animate each button sequentially
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            
            // Scale animation
            button.DOScale(1f, animationDuration)
                 .SetDelay(i * staggerDelay)
                 .SetEase(Ease.OutBack);

            // Optional rotation effect
            if (useRotationEffect)
            {
                button.DORotate(new Vector3(0, 0, Random.Range(-rotationAmount, rotationAmount)), animationDuration)
                      .SetDelay(i * staggerDelay)
                      .SetEase(Ease.OutBack);
            }

            // Optional fade effect
            if (useFadeEffect && buttonGroups != null && i < buttonGroups.Length)
            {
                buttonGroups[i].DOFade(1f, animationDuration)
                             .SetDelay(i * staggerDelay)
                             .SetEase(Ease.OutCirc);
            }
        }
    }

    private void OnButtonHover(RectTransform button)
    {
        button.DOComplete(); // Complete any running animations
        button.DOScale(hoverScale, hoverDuration)
              .SetEase(Ease.OutBack);

        if (useRotationEffect)
        {
            button.DORotate(new Vector3(0, 0, -rotationAmount), hoverDuration)
                  .SetEase(Ease.OutBack);
        }
    }

    private void OnButtonExit(RectTransform button)
    {
        button.DOComplete(); // Complete any running animations
        button.DOScale(1f, hoverDuration)
              .SetEase(Ease.OutBack);

        if (useRotationEffect)
        {
            button.DORotate(Vector3.zero, hoverDuration)
                  .SetEase(Ease.OutBack);
        }
    }

    private void OnDestroy()
    {
        // Clean up DOTween animations when the object is destroyed
        foreach (var button in buttons)
        {
            if (button != null)
            {
                button.DOKill();
            }
        }
    }
}
