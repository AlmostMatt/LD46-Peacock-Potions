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
        FADE_IN,
        TRANSLATE
    }

    private abstract class UIAnimation
    {
        private float mDelay = 0f;
        protected GameObject mUIObject;

        public UIAnimation(GameObject go, float delay)
        {
            mUIObject = go;
            mDelay = delay;
        }

        public bool Update(bool forceComplete)
        {
            if(mDelay > 0 && !forceComplete)
            {
                mDelay -= Time.deltaTime;
                return false;
            }
            else
            {
                return UpdateInternal(forceComplete);
            }
        }
        protected abstract bool UpdateInternal(bool forceComplete);
    }

    private class FadeInAnimation : UIAnimation
    {
        public FadeInAnimation(GameObject go, float delay) :
            base(go, delay)
        { }

        protected override bool UpdateInternal(bool forceComplete)
        {
            float newAlpha = forceComplete ? 1 : Mathf.Min(1, mUIObject.GetComponent<CanvasGroup>().alpha + (Time.deltaTime / FADE_IN_TIME));
            mUIObject.GetComponent<CanvasGroup>().alpha = newAlpha;
            if(newAlpha >= 1)
            {
                return true;
            }

            return false;
        }
    }

    private class TranslationAnimation : UIAnimation
    {        
        private Vector2 mVelocity;
        private Vector2 mTargetPos;
        private float mTimeLeft;

        private RectTransform mTransform;

        public TranslationAnimation(GameObject go, Vector2 targetPos, float time, float delay) :
            base(go, delay)
        {
            mTargetPos = targetPos;
            mTransform = go.GetComponent<RectTransform>();
            mVelocity = (targetPos - mTransform.anchoredPosition) * (1 / time);
            mTimeLeft = time;
        }

        protected override bool UpdateInternal(bool forceComplete)
        {
            mTimeLeft -= Time.deltaTime;

            if(mTimeLeft <= 0 || forceComplete)
            {
                mTransform.anchoredPosition = mTargetPos;
                mTimeLeft = 0;
                return true;
            }
            
            mTransform.anchoredPosition += mVelocity * Time.deltaTime;
            return false;
        }
    }

    private List<UIAnimation> mAnimations = new List<UIAnimation>();
    private UIAnimation mActiveAnimation;

    public static void PushFadeIn(GameObject go, float delay = 0)
    {
        FadeInAnimation anim = new FadeInAnimation(go, delay);
        sSingleton._PushAnimation(anim);
    }

    public static void PushTranslation(GameObject go, Vector2 targetPos, float time, float delay = 0)
    {
        TranslationAnimation anim = new TranslationAnimation(go, targetPos, time, delay);
        sSingleton._PushAnimation(anim);
    }

    private void _PushAnimation(UIAnimation anim)
    {
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
            
            if(mHurryUp >= 1.5)
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
                if(mActiveAnimation.Update(hurryUp))
                {
                    mActiveAnimation = null;
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
