using UnityEngine;

[System.Serializable]
public class QuestionData
{
    [TextArea(2, 5)]
    public string questionText;
    
    public string[] options;
    public int correctIndex;

    [TextArea(3, 5)]
    public string blinkyHint;
}