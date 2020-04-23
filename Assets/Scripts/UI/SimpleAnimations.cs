using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
 * Unity coroutines to change position or alpha of an object over time.
 */
public class SimpleAnimations
{
    public static void SpawnFloatingColoredText(GameObject textPrefab, Transform targetPosition, string textContent, Color color)
    {
        // Spawn at the same position of the target transform
        // but make it a child of the root canvas to avoid messing with layout groups
        GameObject animatedText = GameObject.Instantiate(textPrefab, targetPosition.position, Quaternion.identity, targetPosition.root);
        Text text = animatedText.GetComponent<Text>();
        text.text = textContent;
        text.color = color;
        text.StartCoroutine(SimpleAnimations.FadeInOut(animatedText, 1f));
        text.StartCoroutine(SimpleAnimations.MoveOverTime(animatedText, new Vector3(0f, 60f, 0f), 1f));
        // Destroy it 2s later (1s after animation ends)
        GameObject.Destroy(animatedText, 2f);
    }

    public static IEnumerator MoveOverTime(GameObject animatedText, Vector3 offset, float duration)
    {
        float timePassed = 0f;
        Vector3 startPosition = animatedText.transform.position;
        Vector3 endPosition = startPosition + offset;
        while (timePassed <= duration)
        {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            animatedText.transform.position = Vector3.Lerp(startPosition, endPosition, timePassed / duration);
        }
        animatedText.transform.position = endPosition;
    }
    public static IEnumerator FadeInOut(GameObject animatedText, float duration)
    {
        // Fade in over 0.3s
        animatedText.GetComponent<Text>().CrossFadeAlpha(1f, duration * 0.3f, false);
        // 0.7s from now, fade out
        yield return new WaitForSeconds(duration * 0.7f);
        animatedText.GetComponent<Text>().CrossFadeAlpha(0f, duration * 0.3f, false);
    }
}
