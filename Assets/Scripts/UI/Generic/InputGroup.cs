using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputGroup : MonoBehaviour
{
    public int minValue;
    public int increments;

    private int mCurrentValue;
    private Button mButton1;
    private Button mButton2;
    private Text mText;
    private bool hasValue = false;

    // Use this for initialization
    void Start()
    {
        mButton1 = transform.Find("Button1").GetComponent<Button>();
        mButton2 = transform.Find("Button2").GetComponent<Button>();
        mText = transform.Find("Value/Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        // TODO: add can-increment and can-decrement functions
    }

    public void ClearValue()
    {
        hasValue = false;
    }

    // Calls SetValue but only if has-value is false
    public void SetInitialValue(int value)
    {
        if (!hasValue)
        {
            SetValue(value);
        }
    }

    public void SetValue(int value)
    {
        hasValue = true;
        if (mText == null)
        {
            // Hack for edge-case where value is set before start
            Start();
        }
        mCurrentValue = value;
        mText.text = value.ToString();
    }

    public int GetValue()
    {
        return mCurrentValue;
    }

    public void IncreaseValue()
    {
        SetValue(mCurrentValue + increments);
        // TODO: have a function callback
    }

    public void DecreaseValue()
    {
        SetValue(mCurrentValue - increments);
        // TODO: have a function callback
    }
}
