using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;



[System.Serializable]
public struct QuestionStruct
{
    public int id;
    public string question;
    public string[] answers;
    public int correctAnswer;
    public string explain;
    public string showDirector;
    public string correctDirector;

}


[CreateAssetMenu]
public class Questions : ScriptableObject
{
    [SerializeField] public QuestionStruct[] questions;

    public QuestionStruct getIndex(int i)
    {
        return questions[i];
    }

    public int getLength()
    {
        return questions.Length;
    }

}
