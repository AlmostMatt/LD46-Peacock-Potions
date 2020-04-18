using UnityEngine;
using System.Collections;

/**
 * Manipulates UI objects based on the game state.
 * Also receives user input from the UI.
 */
public class UIControllerSystem : MonoBehaviour
{
    public GameObject SummaryView;
    public GameObject EventView;

    // Use this for initialization
    void Start()
    {
        SummaryView.SetActive(true);
        EventView.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SummaryScreenOK()
    {
        // done with the summary
        SummaryView.SetActive(false);
        EventView.SetActive(true);
    }

    public void MakeDecision()
    {
        // Made a choice in response to a decision
        SummaryView.SetActive(true);
        EventView.SetActive(false);
    }
}
