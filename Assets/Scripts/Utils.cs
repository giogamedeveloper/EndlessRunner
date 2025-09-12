using UnityEngine;

static class Utils
{
    /// <summary>
    /// Activate/deactivate a canvas group.
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="isEnable"></param>
    public static void Active(this CanvasGroup canvasGroup, bool isEnable)
    {
        canvasGroup.alpha = isEnable ? 1 : 0;
        canvasGroup.interactable = isEnable;
        canvasGroup.blocksRaycasts = isEnable;
    }
}
