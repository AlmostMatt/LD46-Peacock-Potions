using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonEventChain
{
    private static bool letSonHelpBottle = false;

    public static void Init()
    {
        EventState.PushEvent(new SonEventOne(), GameState.quarter + 1);
    }

    private class SonEventOne : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch(currentStage)
            {
                case EventStage.START:                    
                    EventState.currentEventText = "Your son runs up to you while you're at the counter.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    break;
                case EventStage.S1:
                    EventState.currentEventImage = "faceSonHappy";
                    EventState.currentEventText = "\"Daddy! What are you doing?\"";
                    EventState.currentEventOptions = new string[]
                        {
                        "\"Making magical potions, dear.\"",
                        "\"Adult stuff. Why don't you go play with mom?\""
                        };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2, EventStage.REFUSE };
                    break;
                case EventStage.S2:
                    EventState.currentEventImage = "faceSonSurprise";
                    EventState.currentEventText = "Magical! Can you make one that makes me fly?!";
                    EventState.currentEventOptions = new string[]
                        {
                        "\"Why not try yourself?\"",
                        "\"Maybe later. I'm busy right now.\""
                        };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                    break;
                case EventStage.ACCEPT:
                    EventState.currentEventImage = "";
                    EventState.currentEventText = "Excited, he grabs some empty bottles and runs over to the cauldron.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    RelationshipState.sonRelationship += 10;
                    letSonHelpBottle = true;
                    EventState.PushEvent(new SonDropBottlesEvent(), GameState.quarter);
                    return EventResult.DONE;
                case EventStage.REFUSE:
                    EventState.currentEventImage = "faceSonSad";
                    EventState.currentEventText = "\"Okayy.\" He walks upstairs, looking dejected.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    RelationshipState.sonRelationship -= 10;
                    letSonHelpBottle = false;
                    EventState.PushEvent(new SonEventTwo(), GameState.quarter + 2);
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }

    private class SonDropBottlesEvent : GameEvent
    {
        private List<int> potions;
        protected override EventResult EventStart()
        {
            potions = new List<int>();
            for(int i = 0; i < 3; ++i)
            {
                int potionType = Random.Range(0, (int)ProductType.PT_MAX);
                int maxChecks = (int)ProductType.PT_MAX;
                while(BusinessState.inventory[potionType] == 0 && maxChecks > 0)
                {
                    potionType = (potionType + 1) % (int)ProductType.PT_MAX;
                    --maxChecks;
                }
                if(maxChecks == 0)
                {
                    break;
                }
                potions.Add(potionType);
                Debug.Log("son breaking " + (ProductType)potionType);
                --BusinessState.inventory[potionType];
                BusinessState.quarterlyReport.miscLosses[potionType]++;
            }
            
            if(potions.Count == 0)
            {
                return EventResult.SKIP;
            }

            return EventResult.CONTINUE;
        }

        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventText = "You hear a crash from across the store. You glance over and see that your son knocked over some potions.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    break;
                case EventStage.S1:
                    EventState.currentEventImage = "faceSonSad";
                    EventState.currentEventText = "\"Waaah! I- I didn't mean to!\"";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    EventState.currentEventOptions = new string[]
                    {
                        "Console him and clean up the mess",
                        "Tell him to go upstairs"
                    };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.GO_CLEAN, EventStage.GO_OUTSIDE };                    
                    break;
                case EventStage.GO_CLEAN:
                    EventState.currentEventImage = "faceSonNeutral";
                    EventState.currentEventText = "He fetches a broom and helps you clean up. Then he decides to go outside.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new SonEventTwo(), GameState.quarter + 2); // schedule another event for next quarter
                    RelationshipState.sonRelationship += 5;
                    return EventResult.DONE;
                case EventStage.GO_OUTSIDE:
                    EventState.currentEventImage = "faceSonSad";
                    EventState.currentEventText = "He sniffles and heads upstairs.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new SonEventTwo(), GameState.quarter + 2); // schedule another event for next quarter
                    RelationshipState.sonRelationship -= 5;
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }

    private class SonEventTwo : GameEvent
    {
        private const int toyCost = 200;
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch(currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = string.Format("{0} comes up to you. She suggests buying a special toy for your son's birthday.", WifeEventChain.NAME);
                    if(BusinessState.money >= toyCost)
                    {
                        EventState.currentEventOptions = new string[]
                        {
                            "Give her " + Utilities.FormatMoney(toyCost),
                            "Say you can't afford it"
                        };
                        mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                    }
                    else
                    {
                        EventState.currentEventOptions = new string[]
                        {
                            "Say you can't afford it"
                        };
                        mCurrentOptionOutcomes = new EventStage[] { EventStage.REFUSE };
                    }
                    break;
                case EventStage.ACCEPT:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "\"I think this will make him very happy.\" She gives you a quick kiss and leaves the shop.";
                    EventState.PushEvent(new SonEventBirthday(), GameState.quarter+1);
                    RelationshipState.wifeRelationship += 5;
                    BusinessState.MoneyChangeFromEvent(-toyCost);
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    return EventResult.DONE;                    
                case EventStage.REFUSE:
                    EventState.currentEventImage = "faceWifeSad";
                    if(BusinessState.money <= toyCost * 5)
                    {
                        EventState.currentEventText = "\"That's too bad. I know things are tough right now. He'll understand.\"";
                    }
                    else if(BusinessState.money >= toyCost * 10)
                    {
                        EventState.currentEventText = "\"I think we could spare it. How can you neglect our son like that?\" She goes outside.";
                        RelationshipState.wifeRelationship -= 10;
                    }
                    else
                    {
                        EventState.currentEventText = "\"Okay, I'll see if I can find something else myself...\" She leaves.";
                    }
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    return EventResult.DONE;
            }

            return EventResult.CONTINUE;
        }
    }

    private class SonEventBirthday : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch(currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceSonHappy";
                    EventState.currentEventText = "\"Dad! Thank you for my birthday present!\" He gives you a big hug and then runs outside.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    RelationshipState.sonRelationship += 10;
                    return EventResult.DONE;
            }

            return EventResult.DONE;
        }
    }

}
