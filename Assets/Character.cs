using UnityEngine;

public class Character : MonoBehaviour
{

    public enum GroupColor
    {
        Red,
        Blue,
    };
    public GroupColor groupColor;
    public TextMesh textMesh;

    // Use this for initialization
    void Start()
    {
        string text = string.Empty;
        Color col = Color.white;
        switch (groupColor)
        {
            case GroupColor.Red:
                text = "味方";
                col = Color.red;
                break;
            case GroupColor.Blue:
                text = "敵";
                col = Color.blue;
                break;
        }
        textMesh.text = text;
        textMesh.color = col;
    }
}