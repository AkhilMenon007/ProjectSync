using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NewMenu/ItemRef")]
public class ItemReference : ScriptableObject
{
    [SerializeField]
    private Item item = null;
    private void OnEnable()
    {
        Debug.Log(item.name);
    }
}
