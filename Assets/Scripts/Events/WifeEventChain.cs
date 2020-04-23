using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifeEventChain
{
    public static void Init()
    {
        Debug.Log("pushing son event");
        EventState.PushEvent(new WifeEventOne(), 1);
    }
    public const string NAME = "Susie";

    private class WifeEventOne : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "\"Hi there! Is that an Ancadian Peacock?\"";
                    EventState.currentEventOptions = new string[]
                        {"Umm, yeah!","I don’t actually know..."};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1, EventStage.S1 };
                    break;
                case EventStage.S1:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "\"It’s quite majestic. Oh, my name is " + NAME + " by the way.\"";
                    EventState.currentEventOptions = new string[]
                        {"It’s a fancy fantastical pheasant!","Magestic? I suppose it is."};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2, EventStage.S2 };
                    break;
                case EventStage.S2:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "\"I've never seen one of this breed before. Do you mind if I study it for a little while?\"";
                    EventState.currentEventOptions = new string[]
                        {"Of course! It’s quite friendly."};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S3 };
                    break;
                case EventStage.S3:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "She is staring intently at the peacock and seems to have forgotten about you for now.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new WifeEventTwo(), GameData.singleton.quarter + 1); // schedule another event for next quarter
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }

    private class WifeEventTwo : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "\"Hello again!\"";
                    EventState.currentEventOptions = new string[]
                        {"Welcome back!"};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    break;
                case EventStage.S1:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "\"Thanks for letting me observe your peacock last time. I was writing a paper on the species, and seeing it was really helpful.\"";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    break;
                case EventStage.S2:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "\"Would you like to go out for tea after work?\"";
                    EventState.currentEventOptions = new string[] { "Sure!", "No thanks, I'm too busy." };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                    break;
                case EventStage.ACCEPT:
                    EventState.currentEventImage = "faceWifeHappy";
                    GameData.singleton.wifeRelationship += 10f;
                    EventState.currentEventText = "\"Awesome, I know a great place!\"\n<i>She runs off in excitement</i>";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new WifeEventGrowClose(), GameData.singleton.quarter + 2); // schedule another event for next quarter
                    return EventResult.DONE;
                case EventStage.REFUSE:
                    EventState.currentEventImage = "faceWifeSad";
                    EventState.currentEventText = "\"Aw, okay.\nOn an unrelated note, do you sell love potions?\"";
                    if (GameData.singleton.inventory[(int)ProductType.PT_LOVE_POTION] > 0)
                    {
                        EventState.currentEventOptions = new string[] { "Yes" };
                        mCurrentOptionOutcomes = new EventStage[] { EventStage.HAS_POTION };
                    }
                    else
                    {
                        EventState.currentEventOptions = new string[] { "\"Yes, but we don't have any right now\"" };
                        mCurrentOptionOutcomes = new EventStage[] { EventStage.NO_POTION };
                    }
                    break;
                case EventStage.HAS_POTION:
                    EventState.currentEventImage = "faceWifeNeutral";
                    GameData.singleton.wifeRelationship += 1f;
                    EventState.currentEventText = "\"I'll buy one love potion then!\"\n<i>She pays the price and heads out.</i>";
                    BusinessSystem.SellProduct((int)ProductType.PT_LOVE_POTION);
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new WifeEventLovePotion(), GameData.singleton.quarter + 1); // schedule another event for next quarter
                    return EventResult.DONE;
                case EventStage.NO_POTION:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "\"Alright. Well, I'll be around.\"";
                    BusinessSystem.SellProduct((int)ProductType.PT_LOVE_POTION);
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new WifeEventLovePotion(), GameData.singleton.quarter + 1); // schedule another event for next quarter
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }

    /**
    private class WifeEventDate : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "Hello again!";
                    EventState.currentEventOptions = new string[]
                        {"Welcome back!"};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    break;
            }
            return EventResult.CONTINUE;
        }
    }
    */

    private class WifeEventLovePotion : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "\"Hi!\"";
                    EventState.currentEventOptions = new string[]
                        {"..."};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    break;
                case EventStage.S1:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "\"I noticed how hard you work so I brought you some tea, here!\"";
                    EventState.currentEventOptions = new string[]
                        {"Drink it"};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    break;
                case EventStage.S2:
                    GameData.singleton.wifeUsedLovePotion = true;
                    EventState.currentEventImage = "facePlayerSurprise";
                    EventState.currentEventText = "Ah! It tastes sweeter than I expected!\nI like it though.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S3 };
                    break;
                case EventStage.S3:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = NAME + " winks at you, then heads out.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new WifeEventGrowClose(), GameData.singleton.quarter + 1); // schedule another event for next quarter
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }

    private class WifeEventGrowClose : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceWifeHappy";
                    if (GameData.singleton.wifeUsedLovePotion)
                    {
                        EventState.currentEventText = "Despite being under the influence of a love potion for the first date, you and " + NAME + " ended up going on many more dates and growing quite close.";
                    }
                    else
                    {
                        EventState.currentEventText = "You and " + NAME + " ended up going on many dates and growing quite close.";
                    }
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    break;
                case EventStage.S1:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "\"Hi again! Did you know that Ancadian Peacocks will grow more feathers of a given color depending on the season?\"";
                    EventState.currentEventOptions = new string[] { "Neat!", "Oh yea, I noticed that"};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2, EventStage.S2 };
                    break;
                case EventStage.S2:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "\"It's a remarkable bird. There are other factors that affect the color of their feathers too, but I haven't figured it all out yet.\"";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S3 };
                    break;
                case EventStage.S3:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "\"Anyways, enough about the bird. I'm looking forward to seeing you tonight!\"";
                    EventState.currentEventOptions = new string[] { "Me too!" };
                    // Queue an event for the start of the next quarter
                    EventState.PushEvent(new WifeEventMarriage(), GameData.singleton.quarter + 1, 0f);
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }

    private class WifeEventMarriage : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                case EventStage.S1:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "10 years have passed.\nYou and " + NAME + " ended up getting married!";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    break;
                case EventStage.S2:
                    EventState.currentEventImage = "faceSonHappy";
                    EventState.currentEventText = "You also had a son together!\nHe's growing up quickly.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S3 };
                    break;
                case EventStage.S3:
                    EventState.currentEventImage = "";
                    EventState.currentEventText = "You've been running the potion shop together with " + NAME + ", and it's still doing about as well as it was 10 years ago.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    // Time skip
                    GameData.singleton.yearsSkipped = 10;
                    GameData.singleton.wifeMarried = true;
                    GameData.singleton.sonWasBorn = true;
                    // Start queueing events for the son
                    SonEventChain.Init();
                    // The son event will likely happen next, so delay the next wife event a bit longer
                    EventState.PushEvent(new WifeEventFive(), GameData.singleton.quarter + 2, 0f);
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }

    private class WifeEventFive : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "facePlayerNeutral";
                    EventState.currentEventText = "\"Ah, " + NAME + "'s birthday is coming up!\"";
                    EventState.currentEventOptions = new string[]
                        {"Buy her a gift (-$300)","I don't have any money to spare"};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                    break;
                case EventStage.ACCEPT:
                    BusinessState.MoneyChangeFromEvent(-300);
                    GameData.singleton.wifeRelationship += 11f;
                    EventState.currentEventImage = "facePlayerHappy";
                    EventState.currentEventText = "\"I know just the thing to get! I hope she likes it.\"";
                    EventState.currentEventOptions = new string[]
                        {"Ok"};
                    return EventResult.DONE;
                case EventStage.REFUSE:
                    GameData.singleton.wifeRelationship -= 1f;
                    EventState.currentEventImage = "facePlayerNeutral";
                    EventState.currentEventText = "\"Hopefully she doesn't mind.\"";
                    EventState.currentEventOptions = new string[]
                        {"Ok"};
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }
}
