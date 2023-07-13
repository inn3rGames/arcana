using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryNavigator : MonoBehaviour
{
    public static void StartStory()
    {
        SceneManager.LoadScene(1);
    }

    public static void EndStory()
    {
        SceneManager.LoadScene(0);
    }
}
