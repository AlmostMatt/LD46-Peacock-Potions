using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
 * Unity coroutines to change position or alpha of an object over time.
 */
public class SimpleAnimations
{
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
