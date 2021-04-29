using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Shipwreck;

namespace Shipwreck.Scene
{
public class SceneSwitch : MonoBehaviour
{

    #region SceneHelper
    public static class SceneHelper
    {
            private static bool IsReload = false;

            public static void LoadScene(string sceneName, bool IsReloaded=false)
            {
                IsReload = IsReloaded;
                SceneManager.LoadScene(sceneName);
            }
    
            public static bool IsReloaded() {
                return IsReload;
            }
    }

    public void GotoCurrentDocument() {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        SceneManager.LoadScene("DocumentScene");

    }

    #endregion //SceneHelper
    public void GotoDocuments(string levelID, bool IsReload=false)
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        if (levelID == "loretta")
        {
            SceneHelper.LoadScene("DocumentScene", IsReload);
        }
        else if(levelID == "level2")
        {
            SceneHelper.LoadScene("Document2Scene", IsReload);
        }
    }

    public void GotoConversation(string dialogKey)
    {
        if(PlayerProgress.instance.GetCurrentLevel() == "level2") dialogKey = "rya";
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

    public void GoToFolders()
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        SceneManager.LoadScene("LevelDocument");
    }

    public void GotoLevelEnding()
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        Logging.instance?.LogMissionComplete();
        SceneManager.LoadScene("LevelEndCutscene");
    }

    public void GotoLevelSelect()
    {
        PlayerProgress.instance.SetPrevSceneName(SceneManager.GetActiveScene().name);
        PlayerProgress.instance.ClearRegistrations();
        SceneManager.LoadScene("LevelSelect");
    }
}
}
