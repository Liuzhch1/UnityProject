using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoorLogic : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }

    void EquipWeapon(GameObject interactionObject)
    {
        // Disable Gravity on RigidBody
        Rigidbody rigidBody = interactionObject.GetComponent<Rigidbody>();
        if (rigidBody)
        {
            rigidBody.useGravity = false;
        }

        // Disable first BoxCollider
        BoxCollider boxCollider = interactionObject.GetComponent<BoxCollider>();
        if (boxCollider)
        {
            boxCollider.enabled = false;
        }

        // Set Position and Direction of Gun
        interactionObject.transform.position = m_GunAttachmentTransform.position;
        interactionObject.transform.forward = m_GunAttachmentTransform.forward;

        // Parent Gun to Attach Point
        interactionObject.transform.parent = m_GunAttachmentTransform;

        GunLogic gunLogic = interactionObject.GetComponent<GunLogic>();
        if (gunLogic)
        {
            gunLogic.SetEquipped(true);
        }

        m_equippedObject = m_interactionObject;
    }
}
