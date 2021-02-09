using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void GotoDocuments()
    {
        PlayerProgress.instance.ClearDocumentButtons();
        SceneManager.LoadScene("DocumentScene");
    }

    public void GotoConversation(string dialogKey)
    {
        PlayerProgress.instance.ClearDocumentButtons();
        PlayerProgress.instance.SetDialogKey(dialogKey);
        SceneManager.LoadScene("ConversationInterface");
    }

    public void CallCharacter(string charName)
    {
        GotoConversation(PlayerProgress.instance.PickConversation(charName));
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
            PlayerProgress.instance.ClearDocumentButtons();
            SceneManager.LoadScene("ShipMechanics");
        }
        
    }

    public void GotoDive()
    {
        PlayerProgress.instance.ClearDocumentButtons();
        SceneManager.LoadScene("LaSalleTestScene_RealtimeLighting");
    }
}
