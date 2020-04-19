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
    private bool mHasValue = false;
    private System.Action<int, int> mOnChangeCallback = null;

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
        mHasValue = false;
    }

    // Calls SetValue but only if has-value is false
    public void SetInitialValue(int value)
    {
        if (!mHasValue)
        {
            SetValue(value);
        }
    }

    public void SetValue(int newValue)
    {
        if (mText == null)
        {
            // Hack for edge-case where value is set before start
            Start();
        }
        int oldValue = mCurrentValue;
        mCurrentValue = newValue;
        mText.text = newValue.ToString();
        // Only call the callback function if the InputGroup already had a value (onChange)
        if (mHasValue && mOnChangeCallback != null)
        {
            mOnChangeCallback(newValue - oldValue, newValue);
        }
        mHasValue = true;
    }

    public int GetValue()
    {
        return mCurrentValue;
    }

    public void SetCanDecrement(bool canDecrement)
    {
        mButton1.interactable = canDecrement;
    }

    public void SetCanIncrement(bool canIncrement)
    {
        mButton2.interactable = canIncrement;
    }

    // Sets a function that will be called when the value changes.
    // The function will be called with args (valueDelta, newValue)
    public void SetOnChangeCallback(System.Action<int, int> onChangeCallback)
    {
        mOnChangeCallback = onChangeCallback;
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
