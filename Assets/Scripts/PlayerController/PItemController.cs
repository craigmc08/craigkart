using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Items;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PDriverController))]
public class PItemController : MonoBehaviour
{
    PDriverController driver;

    public Item heldItem = null;

    bool m_Use = false;

    void Start()
    {
        driver = GetComponent<PDriverController>();
    }

    void FixedUpdate()
    {
        if (m_Use) {
            if (heldItem != null) {
                bool notDone = heldItem.ExecuteEffect(driver);
                if (!notDone) heldItem = null;
            }
            m_Use = false;
        }
    }

    public void OnUseItem(InputValue value) {
        m_Use = value.Get<float>() > 0.5f;
        if (!driver.controllable) m_Use = false;
    }

    public void RollItem() {
        throw new NotImplementedException();
    }

    // Returns false if the item was not given because
    // the player is already holding one
    // Set `force` to true to ensure the item is set
    public bool GiveItem(Item item, bool force=false) {
        if (heldItem != null && !force) return false;

        heldItem = item;
        return true;
    }
}
