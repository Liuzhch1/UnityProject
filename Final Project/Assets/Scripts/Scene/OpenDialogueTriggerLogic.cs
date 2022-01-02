using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDialogueTriggerLogic : MonoBehaviour
{

    bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
		if (!isTriggered)
		{
            UIManager.Instance.displayDialogue(Speaker.Commander, "OpenDialogue1", 3.0f);
            UIManager.Instance.displayDialogue(Speaker.Agent, "OpenDialogue2", 3.0f);
            UIManager.Instance.displayDialogue(Speaker.Commander, "OpenDialogue3", 3.0f);
            UIManager.Instance.displayDialogue(Speaker.Commander, "OpenDialogue4", 3.0f);
            UIManager.Instance.displayDialogue(Speaker.Agent, "OpenDialogue5", 3.0f);
            UIManager.Instance.displayDialogue(Speaker.Commander, "OpenDialogue6", 3.0f);
            UIManager.Instance.displayDialogue(Speaker.Agent, "OpenDialogue7", 3.0f);
            isTriggered = true;
		}
    }
}
