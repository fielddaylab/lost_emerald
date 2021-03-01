using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void GotoDocuments()
    {
        PlayerProgress.instance.ClearRegistrations();
        SceneManager.LoadScene("DocumentScene");
    }

    public void GotoConversation(string dialogKey)
    {
        PlayerProgress.instance.ClearRegistrations();
        PlayerProgress.instance.SetDialogKey(dialogKey);
        SceneManager.LoadScene("ConversationInterface");
    }

    public void CallCharacter(string charName)
    {
        string conversation = PlayerProgress.instance.PickConversation(charName, out string bubble);
        if (bubble != null)
        {
            PlayerProgress.instance.TemporaryBubble(bubble);
        }
        if (conversation != null)
        {
            GotoConversation(conversation);
        }
    }

    public void GotoSonar()
    {
        PlayerProgress.instance?.Unlock("been-to-sonar");
        if (Ship.count > 80)
        {
            GotoDive();
        }
        else
        {
            PlayerProgress.instance.ClearRegistrations();
            SceneManager.LoadScene("ShipMechanics");
        }

    }

    public void GotoDive()
    {
        PlayerProgress.instance.ClearRegistrations();
        SceneManager.LoadScene("LaSalleTestScene_RealtimeLighting");
    }

    public void GotoDesk()
    {
        PlayerProgress.instance.ClearRegistrations();
        SceneManager.LoadScene("OfficeDesk");
    }

    public void GotoLevelEnding()
    {
        PlayerProgress.instance.ClearRegistrations();
        SceneManager.LoadScene("LevelEnding");
    }
}
