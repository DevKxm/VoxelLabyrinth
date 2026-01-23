using UnityEngine;
using TMPro;

public class ScoreRow : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nickText;
    public TMP_Text timeText;

    public void SetData(int rank, string nick, string time)
    {
        rankText.text = rank.ToString() + ".";
        nickText.text = nick;
        timeText.text = time;
    }
}