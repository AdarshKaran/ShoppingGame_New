using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item")]
public class ItemDetailsObject : ScriptableObject
{
    public string itemName;
    public int cost;
    public Sprite image;
    public string type;
    public float[] quantityOptions;
}
