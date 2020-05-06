using UnityEngine;
using System.Collections;

public class UnlockPotionEvent : GameEvent
{
    [System.Serializable]
    public class UnlockPotionEventSaveData : ScheduledEvent.EventSpecificSaveData
    {
        public UnlockPotionEventSaveData(PotionType PotionType)
        {
            this.PotionType = PotionType;
        }
        public PotionType PotionType;
    }

    private PotionType mPotionType;

    public UnlockPotionEvent(PotionType PotionType)
    {
        mPotionType = PotionType;
    }

    // for reloading from a save file
    public UnlockPotionEvent(UnlockPotionEventSaveData saveData)
    {
        mPotionType = saveData.PotionType;
        Debug.Log("reloaded UnlockPotionEvent for type " + mPotionType);
    }

    public override ScheduledEvent.EventSpecificSaveData GetSaveData()
    {
        Debug.Log("saving UnlockPotionEvent for type " + mPotionType);
        return new UnlockPotionEventSaveData(mPotionType);
    }

    protected override EventResult OnStage(EventStage currentStage)
    {
        switch (currentStage)
        {
            case EventStage.START:
                EventState.currentEventImage = mPotionType.GetImage();
                EventState.currentEventImageColor = mPotionType.GetColor();
                EventState.currentEventText = string.Format("You can now craft {0} potions.", mPotionType.GetName());
                EventState.currentEventOptions = new string[] { "Nice!" };
                mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT };
                // Actually unlock the potion
                GameData.singleton.potionsUnlocked[(int)mPotionType] = true;
                return EventResult.DONE;
        }

        return EventResult.DONE;
    }
}
