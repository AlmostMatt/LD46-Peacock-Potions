using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpecialEffects : MonoBehaviour
{
    public GameObject floatingTextPrefab;

    private static SpecialEffects singletonInstance;

    private static SpecialEffects Get() {
        if (singletonInstance == null) {
            singletonInstance = FindObjectsOfType<SpecialEffects>()[0];
        }
        return singletonInstance;
    }

    public static void ShowNumberChange(Transform targetPosition, int delta, Color color)
    {
        // The format pattern just means show the + or -
        string numericText = string.Format("{0:+#;-#;+0}", delta);
        Get().SpawnFloatingColoredText(targetPosition, numericText, color);
    }

    public void SpawnFloatingColoredText(Transform targetPosition, string textContent, Color color)
    {
        // Spawn at the same position of the target transform
        // but make it a child of the root canvas to avoid messing with layout groups
        GameObject animatedText = GameObject.Instantiate(
            floatingTextPrefab, targetPosition.position, Quaternion.identity, targetPosition.root);
        Text text = animatedText.GetComponent<Text>();
        text.text = textContent;
        text.color = color;
        StartCoroutine(SimpleAnimations.FadeInOut(animatedText, 1f));
        StartCoroutine(SimpleAnimations.MoveOverTime(animatedText, new Vector3(0f, 60f, 0f), 1f));
        // Destroy it 2s later (1s after animation ends)
        GameObject.Destroy(animatedText, 2f);
    }
}
