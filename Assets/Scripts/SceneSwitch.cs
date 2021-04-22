using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void GotoDocuments()
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        SceneManager.LoadScene("DocumentScene");
    }

    public void GotoConversation(string dialogKey)
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        PlayerProgress.instance.SetDialogKey(dialogKey);
        Logging.instance?.LogViewChat(dialogKey);
        SceneManager.LoadScene("ConversationInterface");
    }

    public void CallCharacter(string charName)
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
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
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        SceneManager.LoadScene("ShipMechanics");
    }

    public void GotoDive()
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        Logging.instance?.LogDiveStart();
        SceneManager.LoadScene("LaSalleTestScene_RealtimeLighting");
    }

    public void GotoDesk()
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        Logging.instance?.LogViewDesk();
        SceneManager.LoadScene("OfficeDesk");
    }

    public void GotoLevelEnding()
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        Logging.instance?.LogMissionComplete();
        SceneManager.LoadScene("LevelEnding");
    }

    public void GotoLevelSelect()
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        SceneManager.LoadScene("LevelSelect");
    }
}
