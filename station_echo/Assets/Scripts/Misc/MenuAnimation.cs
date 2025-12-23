using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    public int slideAmount;
    public float slideDuration = 0.5f;
    public int totalElements;
    private int currentIndex = 0;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void MoveLeft()
    {
        if (totalElements == -1)
        {
            Debug.Log("Total number of elements was not set");
            return;
        }
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = totalElements - 1;
        StartCoroutine(SlideToIndex(currentIndex));
    }

    public void MoveRight()
    {
        if (totalElements == -1)
        {
            Debug.Log("Total number of elements was not set");
            return;
        }
        currentIndex++;
        if (currentIndex >= totalElements)
            currentIndex = 0;
        StartCoroutine(SlideToIndex(currentIndex));
    }

    private System.Collections.IEnumerator SlideToIndex(int index)
    {
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = new Vector2(-slideAmount * index, startPos.y);
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / slideDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        rect.anchoredPosition = endPos;
    }
}
