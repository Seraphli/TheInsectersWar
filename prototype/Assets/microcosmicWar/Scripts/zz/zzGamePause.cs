using UnityEngine;

public class zzGamePause:MonoBehaviour
{
    [SerializeField]
    bool _paused = false;
    public float timeScaleBeforePaused;

    public void pauseGame()
    {
        if (!_paused && !Application.runInBackground)
        {
            timeScaleBeforePaused = Time.timeScale;
            Time.timeScale = 0f;
            _paused = true;
        }
    }

    public void runGame()
    {
        if (_paused)
        {
            Time.timeScale = timeScaleBeforePaused;
            _paused = false;
        }
    }

    public void switchState()
    {
        if (_paused)
            runGame();
        else
            pauseGame();
    }

    public bool paused
    {
        get { return _paused; }
        set
        {
            if (_paused != value)
                switchState();
        }
    }
}