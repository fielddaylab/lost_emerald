using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLoretta : LevelBase
{
    override public bool CheckboxStatus(PlayerProgress progress, string checkboxKey)
    {
        bool check = false;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LaSalleTestScene_RealtimeLighting")
        {
            if (checkboxKey == "dive-photo" && progress.IsUnlocked("photo-birds-eye"))
            {
                check = true;
            }
            if (checkboxKey == "defining-feature" && progress.IsUnlocked("photo-iron-knees"))
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
        else if (notificationKey == "ship-out" && !progress.IsUnlocked("photo-birds-eye"))
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
            showNotification = !progress.IsUnlocked("photo-iron-knees") && progress.IsUnlocked("photo-birds-eye") && !progress.IsUnlocked("CAMERA_SIDE");
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
                bubble = "Nothing I need from Lou right now.";
                return null;
            }
            else if (!progress.IsUnlocked("informed-lou"))
            {
                return "ending";
            }
        }
        else if (charName == "amy")
        {
            if (!progress.FilledLog("LocationBox"))
            {
                bubble = "I need to figure out where the wreck is before I call Amy.";
                return null;
            }
            else if (!progress.IsUnlocked("wreck-table"))
            {
                return "amy";
            }
            else if (!progress.FilledLog("NameBox"))
            {
                bubble = "Nothing I need from Amy right now.";
                return null;
            }
            else if (!progress.IsUnlocked("dark-day"))
            {
                return "amy-newspaper";
            }
            else
            {
                bubble = "Nothing I need from Amy right now.";
                return null;
            }
        }
        else if (charName == "rusty")
        {
            if (!progress.FilledLog("TypeBox"))
            {
                bubble = "Nothing I need from Rusty right now.";
                return null;
            }
            else if (!progress.IsUnlocked("rusty-transcript"))
            {
                return "shipbuilder";
            }
            else
            {
                bubble = "Nothing I need from Rusty right now.";
                return null;
            }
        }
        return null;
    }

    override public string CurrentThought(PlayerProgress progress)
    {
        ThoughtBubble bubble = progress.GetThoughtBubble();

        // stuff on non-document scenes
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "ShipMechanics")
        {
            if (!progress.IsUnlocked("ship-on-lake"))
            {
                return "The red dot shows the GPS location of the ship";
            }
            else if (!progress.IsUnlocked("sonar-complete") && progress.IsUnlocked("ship-on-lake"))
            {
                return "Use your sonar to find the ship!";
            }
            else if (progress.IsUnlocked("sonar-complete") && !progress.IsUnlocked("been-to-dive"))
            {
                return "Yes! There it is!";
            }
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LaSalleTestScene_RealtimeLighting")
        {
            if (!progress.IsUnlocked("photo-birds-eye"))
            {
                return "Better get a picture of the whole wreck while I’m here.\nI'll start with a picture of the ship from above.";
            }
            else if (!progress.IsUnlocked("photo-iron-knees"))
            {
                return "Welp, better dive down further";
            }
            else
            {
                return "I have all the pictures I need. Time to head back to the office!";
            }
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
                return "Let's see. Where's that GPS location?";
            }
            else
            {
                return "Now I can drag the coordinates over to the Location field.";
            }
        }
        if (!progress.IsUnlocked("wreck-table"))
        {
            return "That's right off Rawley Point! There are a bunch of ships\nthat went down around there. Better call the archivist and get a list.";
        }
        if (!progress.IsUnlocked("sonar-complete"))
        {
            if (!progress.IsUnlocked("viewed-wreck-table"))
            {
                return "The List of Wrecks should help me narrow things down.";
            }
            else
            {
                return "It has to be one of these 5 ships.\nTime to Ship Out!";
            }
        }

        // coming back from the dive
        if (!progress.IsUnlocked("photo-birds-eye"))
        {
            return "I need to go get some photos of the ship!";
        }

        if (!progress.IsUnlocked("verified-canaller") && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "DocumentScene")
        {
            return null;
        }

        if (!progress.IsUnlocked("verified-canaller") && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "DocumentScene")
        {
            return "I have a photo of the shape of the ship.\nCan I figure out what type of ship it is?";
        }
        if (!progress.FilledLog("TypeBox"))
        {
            return "Now I can fill in what type of ship it is!";
        }
        if (!progress.IsUnlocked("rusty-transcript"))
        {
            return "Hmm. The list had two canallers. Which one is it?\nI wonder if the ship expert I know has any clues that might help.";
        }

        // after dialog with Rusty
        if (!progress.IsUnlocked("photo-iron-knees"))
        {
            return "I need to go get a photo of the ship's knees!";
        }
        if (!progress.IsUnlocked("verified-loretta"))
        {
            return "I took a photo of the ship's knees.\nWhich of the pictures Rusty sent matches the photo?";
        }
        if (!progress.FilledLog("NameBox"))
        {
            return "Now I can fill in the name of the ship!";
        }
        if (!progress.IsUnlocked("newspaper"))
        {
            return "Now that I know it's the Loretta, I should call the Archivist\nand see if she knows anything else about it!";
        }

        if (progress.ChapterComplete() && !progress.IsUnlocked("informed-lou"))
        {
            return "I've filled in everything!\nI should call Lou and let her know!";
        }

        return null;
    }

    override public bool CanShipOut(PlayerProgress progress)
    {
        return progress.IsUnlocked("viewed-wreck-table");
    }

    override public bool ChapterComplete(PlayerProgress progress)
    {
        return progress.FilledLog("LocationBox")
            && progress.FilledLog("TypeBox")
            && progress.FilledLog("NameBox")
            && progress.FilledLog("FeatureBox")
            && progress.FilledLog("CauseBox")
            && progress.FilledLog("CargoBox")
            && progress.FilledLog("SecretBox");
    }
}
