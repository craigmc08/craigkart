using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class GetName : MonoBehaviour
{
    void Update() {
        GetComponent<TextMeshProUGUI>().text = gameObject.transform.parent.name;
    }
}
