using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyUIAnimations
{
    private static FancyUIAnimations sSingleton = new FancyUIAnimations();

    private const float FADE_IN_TIME = 0.5f;
    private float mHurryUp = 0;

    public enum AnimationType
    {
        FADE_IN
    }

    private class UIAnimation
    {
        public AnimationType mAnimType;
        public GameObject mUIObject;
        public float mDelay;        

        public UIAnimation(AnimationType animType, GameObject go, float delay)
        {
            mAnimType = animType;
            mUIObject = go;
            mDelay = delay;
        }
    }

    private List<UIAnimation> mAnimations = new List<UIAnimation>();
    private UIAnimation mActiveAnimation;

    public static void PushAnimation(AnimationType animType, GameObject go, float delay = 0)
    {
        sSingleton._PushAnimation(animType, go, delay);
    }

    private void _PushAnimation(AnimationType animType, GameObject go, float delay = 0)
    {
        UIAnimation anim = new UIAnimation(animType, go, delay);
        mAnimations.Add(anim);
    }

    public static void Update()
    {
        sSingleton._Update();
    }
    
    private void _Update()
    {
        mHurryUp = Mathf.Max(mHurryUp - Time.deltaTime, 0);
        bool hurryUp = Input.GetButtonDown("Submit") || Input.GetButtonDown("Fire1");
        bool finishAllAnimationsNow = false;
        if(hurryUp)
        {
            mHurryUp += 1;
            
            if(mHurryUp >= 2)
            {
                finishAllAnimationsNow = true;
            }
        }

        do
        {
            if(mActiveAnimation == null)
            {
                if(mAnimations.Count > 0)
                {
                    mActiveAnimation = mAnimations[0];
                    mAnimations.RemoveAt(0);
                }
            }
            else
            {
                if(mActiveAnimation.mDelay > 0 && !hurryUp)
                {
                    mActiveAnimation.mDelay -= Time.deltaTime;
                }
                else
                {
                    switch(mActiveAnimation.mAnimType)
                    {
                        case AnimationType.FADE_IN:
                            float newAlpha = hurryUp ? 1 : Mathf.Min(1, mActiveAnimation.mUIObject.GetComponent<CanvasGroup>().alpha + (Time.deltaTime / FADE_IN_TIME));
                            mActiveAnimation.mUIObject.GetComponent<CanvasGroup>().alpha = newAlpha;
                            if(newAlpha >= 1)
                            {
                                mActiveAnimation = null;
                            }
                            break;
                    }
                }
            }
        }
        while(finishAllAnimationsNow && mAnimations.Count > 0);

        if(finishAllAnimationsNow)
        {
            mHurryUp = 0;
        }
    }
}
