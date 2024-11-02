using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public Sprite icon;
    [TextArea] public string description;
    public Color gradeColor;
    public ItemGrade itemGrade;

    public StatusData statusData;

    public string activeEffectName = string.Empty; // Ȱ��ȭ ȿ�� ��Ī
}
public enum ItemGrade
{
    Common,
    Rare,
    Epic,
    Unique,
    Legend
}