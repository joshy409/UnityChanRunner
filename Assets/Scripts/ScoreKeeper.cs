using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour {

    //[ReadOnly(true)]
    [SerializeField] float score;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Text damageText; // hp

    // hp storage object
   // public PlayerDeath hpRef;

    private void Awake() {
       // hpRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeath>();
    }

    // change values of corresponding elements
    public void UpdateUIElements() {
        if (scoreText != null)
        {
            //scoreText.text = ScoreClass.Score.ToString();
        } else
        {
            //scoreText = GameObject.Find("Score Text").GetComponent<TextMeshProUGUI>();
            //scoreText.text = ScoreClass.Score.ToString();
        }

        if (damageText != null)
        {
            //damageText.text = hpRef.CurrentHP + " / " + hpRef.totalHP;
        } else
        {
            //damageText = GameObject.Find("HPText").GetComponent<Text>();
            //damageText.text = (hpRef.CurrentHP + " / " + hpRef.totalHP).ToString();
        }
    }
}


public static class ScoreClass  
{
    public static float Score { get; private set; }
    public static float HP { get; private set; }

    static ScoreKeeper skRef;

    static ScoreClass()
    {
        Score = 0;
        
    }

    public static void RefreshScoreKeeper()
    {
        skRef = GameObject.FindObjectOfType<ScoreKeeper>();
    }

    public static void AddPoints(float pts)
    {
        if (pts > 0) { Score += pts; }
        skRef.UpdateUIElements();
    }

    // called when hp updates
    public static void UpdateUIElements() {
        skRef.UpdateUIElements();
    }

    public static void ResetScore()
    {
        Score = 0;
    }
}