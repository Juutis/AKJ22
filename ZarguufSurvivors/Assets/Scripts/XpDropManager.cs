using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class XpDropManager : MonoBehaviour
{
    public static XpDropManager Instance;
    public List<XpDrop> Drops = new();

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
}
