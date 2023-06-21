using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowMessage : MonoBehaviour
{
    [NonSerialized]
    public TextMeshProUGUI textMesh;
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    public void Show(string message)
    {
        textMesh.text = message;
    }
}
