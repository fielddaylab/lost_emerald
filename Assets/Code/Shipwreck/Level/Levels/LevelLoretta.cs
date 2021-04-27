using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shipwreck.Level
{
    public class LevelLoretta : LevelBase
    {
        public string levelName = "loretta";

        override public bool CheckboxStatus(PlayerProgress progress, string checkboxKey)
        {
            bool check = false;
            if (LevelHelper.CurrentScene() == "LaSalleTestScene_RealtimeLighting")
            {
                if (checkboxKey == "dive-photo" && progress.IsUnlocked("photo-birds-eye"))
                {
                    check = true;
                }
                if (checkboxKey == "photo-ship-name" && progress.IsUnlocked("photo-ship-name"))
                {
                    check = true;
                }
            }
            return check;
        }

        override public bool NotificationStatus(PlayerProgress progress, string notificationKey)
        {
            bool showNotification = false;
            if (notificationKey == "lou-intro")
            {
                showNotification = !progress.IsUnlocked("intro-transcript");
            }
            if (notificationKey == "any-contact")
            {
                showNotification = progress.HasConversation("lou") || progress.HasConversation("amy") || progress.HasConversation("rusty");
            }
            else if (notificationKey == "lou")
            {
                showNotification = progress.HasConversation("lou");
            }
            else if (notificationKey == "amy")
            {
                showNotification = progress.HasConversation("amy");
            }
            else if (notificationKey == "rusty")
            {
                showNotification = progress.HasConversation("rusty");
            }
            else if (notificationKey == "lou-coords-transcript")
            {
                showNotification = !progress.IsUnlocked("viewed-intro-transcript");
            }
            else if(notificationKey == "amy-newspaper") 
            {
                showNotification = progress.IsUnlocked("newspaper");
            }
            else if (notificationKey == "any-document" && !(progress.IsUnlocked("verified-loretta") && progress.IsUnlocked("verified-canaller")))
            {
                if (progress.IsUnlocked("verified-canaller"))
                {
                    showNotification = progress.IsUnlocked("photo-iron-knees-dragged");
                }
                else
                {
                    showNotification = progress.IsUnlocked("photo-birds-eye-dragged");
                }
            }
            else if (notificationKey == "wrecks" && !progress.IsUnlocked("viewed-wreck-table"))
            {
                showNotification = progress.IsUnlocked("wreck-table");
            }
            else if (notificationKey == "ship-out" && (!progress.IsUnlocked("photo-birds-eye")))
            {
                showNotification = progress.IsUnlocked("viewed-wreck-table");
            }
            else if (notificationKey == "evidence-builder" && progress.IsUnlocked("photo-birds-eye") && !progress.IsUnlocked("EvidenceBuilder"))
            {
                if (progress.IsUnlocked("rusty-transcript"))
                {
                    showNotification = !progress.IsUnlocked("verified-loretta");
                }
                else
                {
                    showNotification = !progress.IsUnlocked("verified-canaller");
                }
            }
            else if (notificationKey == "rusty-convo-doc")
            {
                showNotification = progress.IsUnlocked("photo-iron-knees-dragged") && !progress.IsUnlocked("verified-loretta");
            }
            else if (notificationKey == "image" && progress.IsUnlocked("photo-birds-eye"))
            {
                if (progress.IsUnlocked("rusty-transcript") && !progress.IsUnlocked("photo-iron-knees-dragged"))
                {
                    showNotification = !progress.IsUnlocked("verified-loretta");
                }
                else
                {
                    showNotification = !progress.IsUnlocked("verified-canaller") && !progress.IsUnlocked("photo-birds-eye-dragged");
                }
            }
            else if (notificationKey == "evidence")
            {
                showNotification = (progress.IsUnlocked("verified-canaller") && !progress.FilledLog("TypeBox")) || (progress.IsUnlocked("verified-loretta") && !progress.FilledLog("NameBox"));
            }
            else if (notificationKey == "perspective")
            {
                showNotification = !progress.IsUnlocked("photo-ship-name") && progress.IsUnlocked("photo-birds-eye") && !progress.IsUnlocked("CAMERA_SIDE");
            }
            else if (notificationKey == "bird-view-thought")
            {
                showNotification = !progress.IsUnlocked("photo-birds-eye");
            }
            else if (notificationKey == "dive-ready")
            {
                showNotification = progress.IsUnlocked("sonar-complete") && progress.GetPrevSceneName() != "LaSalleTestScene_RealtimeLighting";
            }
            return showNotification;
        }

        override public string PickConversation(PlayerProgress progress, string charName, out string bubble)
        {
            bubble = null;
            if (charName == "lou")
            {
                if (!progress.IsUnlocked("intro-transcript"))
                {
                    return "intro";
                }
                else if (!progress.ChapterComplete())
                {
                    bubble = "I don't need to talk to Lou right now.";
                    return null;
                }
                else if (!progress.IsUnlocked("informed-lou"))
                {
                    return "ending";
                }
            }
            else if (charName == "amy")
            {
                if (!progress.FilledLog("TypeBox"))
                {
                    bubble = "I don't need to talk to Amy right now.";
                    return null;
                }
                else if (!progress.IsUnlocked("wreck-table"))
                {
                    return "amy";
                }
                else if (!progress.FilledLog("CargoBox"))
                {
                    bubble = "I don't need to talk to Amy right now.";
                    return null;
                }
                else if (!progress.IsUnlocked("newspaper"))
                {
                    return "amy-newspaper";
                }
                else
                {
                    bubble = "I don't need to talk to Amy right now.";
                    return null;
                }
            }
            return null;
        }

        override public string CurrentThought(PlayerProgress progress)
        {
            ThoughtBubble bubble = progress.GetThoughtBubble();

            // stuff on non-document scenes
            if (LevelHelper.CurrentScene() == "ShipMechanics")
            {
                if (!progress.IsUnlocked("ship-on-lake"))
                {
                    return "The red dot shows the GPS location of the ship";
                }
                else if (!progress.IsUnlocked("sonar-complete") && progress.IsUnlocked("ship-on-lake"))
                {
                    return "Use the sonar to find the ship!";
                }
                else if (progress.IsUnlocked("sonar-complete") && !progress.IsUnlocked("been-to-dive"))
                {
                    return "Yes, there it is.\nI'll drop a buoy to mark the location.";
                }
            }
            if (LevelHelper.CurrentScene() == "LaSalleTestScene_RealtimeLighting")
            {
                if (!progress.IsUnlocked("photo-birds-eye"))
                {
                    return "There it is! Better start with a photo from above.";
                }
                else if (!progress.IsUnlocked("photo-ship-name"))
                {
                    if (progress.GetDivePerspective() == "CAMERA_SIDE")
                    {
                        return "Wow, it's so clean. No mussels!\nMust've been covered in sand til now.";
                    }
                    else
                    {
                        return "Better dive down and take more photos.";
                    }
                }
                // else
                // {
                //     return "I have all the pictures I need. Time to head back to the office!";
                // }
            }

            // intro sequence, before going off to the sonar
            if (!progress.IsUnlocked("intro-transcript"))
            {
                if (bubble != null && bubble.gameObject.scene.name != "OfficeDesk")
                {
                    // fall through for testing purposes
                }
                else
                {
                    return "Oh, a notification!";
                }
            }
            if (!progress.FilledLog("LocationBox"))
            {
                if (!progress.IsUnlocked("viewed-intro-transcript"))
                {
                    return "My chat with Lou should be in my Documents folder.";
                }
                else
                {
                    return "I need to drag over the GPS location Lou gave me.";
                }
            }
            if (!progress.IsUnlocked("photo-birds-eye") || !progress.IsUnlocked("photo-ship-name"))
            {
                return "That's the place! Right by Rawley Point.\nTime to ship out!";
            }

            // coming back from the dive
            if (!progress.IsUnlocked("photo-birds-eye"))
            {
                return "I need to go get some photos of the ship!";
            }

            if (!progress.IsUnlocked("verified-canaller") && LevelHelper.CurrentScene() != "DocumentScene")
            {
                return null;
            }

            if (!progress.IsUnlocked("verified-canaller") && LevelHelper.CurrentScene() == "DocumentScene")
            {
                return "Ok, let's see. What type of ship are you?\nBetter use my Evidence Builder!";
            }
            if (progress.IsUnlocked("EvidenceBuilder") && !progress.IsUnlocked("verified-canaller"))
            {
                return "I need to drag over my top-down photo and match it up with the right Ship Type.";
            }
            if (!progress.FilledLog("TypeBox"))
            {
                return "Now I can drag the new Evidence over to my ship file.";
            }
            if (!progress.IsUnlocked("wreck-table"))
            {
                return "Time to text the archivist for more info.";
            }

            // after first dialog with Amy
            if (!progress.IsUnlocked("verified-loretta"))
            {
                return "It must be one of these. Gotta figure out which one.\nTime to use the Evidence Builder.";
            }
            if (!progress.FilledLog("NameBox"))
            {
                return "Yes! It’s the Loretta!\nNow I can drag over the Ship Name from my evidence folder.";
            }
            if (!progress.FilledLog("CargoBox"))
            {
                return "Ok, got the ship name!\nNow I can go back to that list and drag over the cargo.";
            }
            if (!progress.IsUnlocked("newspaper"))
            {
                return "Hmm. I still don’t know why the ship sank. I bet Amy can help.";
            }

            // after second dialog with Amy
            if (!progress.FilledLog("CauseBox"))
            {
                return "The article should be in my Documents.\nAnything here about why the ship sank?";
            }

            if (progress.ChapterComplete() && !progress.IsUnlocked("informed-lou"))
            {
                return "I've filled in everything!\nI should call Lou and let her know!";
            }

            return null;
        }

        override public bool CanShipOut(PlayerProgress progress)
        {
            return progress.FilledLog("LocationBox");
        }

        override public bool ChapterComplete(PlayerProgress progress)
        {
            bool result = progress.FilledLog("LocationBox")
                && progress.FilledLog("TypeBox")
                && progress.FilledLog("NameBox")
                //&& progress.FilledLog("FeatureBox")
                && progress.FilledLog("CauseBox")
                && progress.FilledLog("CargoBox");
            //&& progress.FilledLog("SecretBox");

            if(result) {
                PlayerProgress.instance?.AddSaveData(levelName);
            }

            return result;
        }
    }

}
