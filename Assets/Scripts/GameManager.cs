using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Ghost[] ghosts;
    [SerializeField] private Pacman pacman;
    [SerializeField] private Transform pellets;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text gameStartText;
    [SerializeField] private AudioSource eaten;
    [SerializeField] private AudioSource death;
    [SerializeField] private AudioSource ghostdeath;
    [SerializeField] private AudioSource startgame;
    [SerializeField] private bool firstRound=true;
    private bool isGameready = false;//

    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    private int ghostMultiplier = 1;

    
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    private void StartGmae()
    {
        gameStartText.enabled = false;
        
    }
    private void Start()
    {
        NewGame();      // ゲームオーバー後、キー入力で再スタート
    }

    private void Update()
    {
        if (lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
        
    }

    private void NewGame()
    {
        firstRound = true;
        SetScore(0);
        SetLives(3);
        NewRound();
        
    }

    private void NewRound()
    {
        //Debug.Log("New Round");
        gameOverText.enabled = false;
        gameStartText.text = "Ready!"; //
        gameStartText.enabled = true; //
        isGameready = false;
        Stop();
        if (firstRound)
        {
            foreach (Transform pellet in pellets)
            {
                pellet.gameObject.SetActive(true);
            }
        }
        

        ResetState();           // キャラクターの状態をリセット
        //Time.timeScale = 0f;//
        Invoke(nameof(HideGameStart), 1.5f);//
    }
    private void StartGame()
    {
        gameStartText.enabled = false;
        //Time.timeScale = 2f;//
    }
    private void HideGameStart ()
    {
        gameStartText.enabled = false;
        KeepMove();
    }

    private void ResetState()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].ResetState();         // ゴーストの状態をリセット
        }

        pacman.ResetState();            // パックマンの状態をリセット
    }

    private void Stop()
    {
        //Debug.Log(pacman.gameObject.GetComponent<Movement>().speed);

        foreach (Ghost ghost in ghosts)
        {
            //Debug.Log(ghost.gameObject.GetComponent<Movement>().speed);
            ghost.gameObject.GetComponent<Movement>().speed = 0;
        }
        pacman.gameObject.GetComponent<Movement>().speed = 0;
        
    }
    private void KeepMove()
    {

        foreach (Ghost ghost in ghosts)
        {
            //Debug.Log(ghost.gameObject.GetComponent<Movement>().speed);
            ghost.gameObject.GetComponent<Movement>().speed = ghost.gameObject.GetComponent<Movement>().MaxSpeed;
        }
        pacman.gameObject.GetComponent<Movement>().speed = pacman.gameObject.GetComponent<Movement>().MaxSpeed;

    }
    private void GameOver()
    {
        gameOverText.enabled = true;

        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);      // ゴーストを非表示
        }

        pacman.gameObject.SetActive(false);             // パックマンを非表示

    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = " " + lives.ToString();
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(2, '0');
    }

    public void PacmanEaten()
    {
        pacman.DeathSequence();         // パックマンの死亡処理
        firstRound = false;
        SetLives(lives - 1);            // 残機を減らす
        if (death != null)
        {
            death.Play();
        }
        if (lives > 0)
        {
            Invoke(nameof(NewRound), 1.5f);     // 1.5秒後にリセット
        }
        else
        {
            GameOver();             // ゲームオーバー処理
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * ghostMultiplier;            // 倒したゴーストのスコア計算
        SetScore(score + points);
        if (ghostdeath != null)
        {
            ghostdeath.Play();
        }
        ghostMultiplier++;           // ゴーストのスコア倍率を増加
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);         // ペレットを非表示

        SetScore(score + pellet.points);            // スコアを加算
        if (eaten != null)
        {
            eaten.Play();
        }
        //gameStartText.enabled = false;
        if (!HasRemainingPellets())                 // すべてのペレットを食べたか確認
        {
            pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].frightened.Enable(pellet.duration);        // ゴーストを一定時間怖がらせる
        }

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));          // 以前のリセット予約をキャンセル
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);  // 一定時間後にスコア倍率をリセット
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;            // まだペレットが残っている場合
            }
        }

        return false;           // すべてのペレットを食べた
    }   

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;        // ゴーストのスコア倍率をリセット
    }


}