using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void GotoDocuments()
    {
        SceneManager.LoadScene("DocumentScene");
    }

    public void GotoConversation(string dialogKey)
    {
        PlayerProgress.instance.SetDialogKey(dialogKey);
        SceneManager.LoadScene("ConversationInterface");
    }

    public void CallCharacter(string charName)
    {
        GotoConversation(PlayerProgress.instance.PickConversation(charName));
    }

    public void GotoSonar()
    {
        if (Ship.count > 80)
        {
            GotoDive();
        }
        else
        {
            SceneManager.LoadScene("ShipMechanics");
        }
        
    }

    public void GotoDive()
    {
        SceneManager.LoadScene("LaSalleTestScene_RealtimeLighting");
    }
}
