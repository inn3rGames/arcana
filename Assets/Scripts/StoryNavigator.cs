using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryNavigator : MonoBehaviour
{
    public void StartStory()
    {
        SceneManager.LoadScene(1);
    }

    public void EndStory()
    {
        SceneManager.LoadScene(0);
    }
}
