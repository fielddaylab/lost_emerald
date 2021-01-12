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

    public void GotoConversation1()
    {
        SceneManager.LoadScene("ConversationInterface");
    }

    public void GotoSonar()
    {
        SceneManager.LoadScene("ShipMechanics");
    }

    public void GotoDive()
    {
        SceneManager.LoadScene("LaSalleTestScene_RealtimeLighting");
    }
}
