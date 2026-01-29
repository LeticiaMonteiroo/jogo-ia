using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    [TextArea(2, 5)]
    public string questionText;
    
    public string[] answers;
    public int correctAnswerIndex;

    [Header("Ensino do Blinky")]
    [TextArea(3, 10)]
    public string blinkyExplanation; 
}