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
        NewGame();      // �Q�[���I�[�o�[��A�L�[���͂ōăX�^�[�g
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
        

        ResetState();           // �L�����N�^�[�̏�Ԃ����Z�b�g
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
            ghosts[i].ResetState();         // �S�[�X�g�̏�Ԃ����Z�b�g
        }

        pacman.ResetState();            // �p�b�N�}���̏�Ԃ����Z�b�g
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
            ghosts[i].gameObject.SetActive(false);      // �S�[�X�g���\��
        }

        pacman.gameObject.SetActive(false);             // �p�b�N�}�����\��

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
        pacman.DeathSequence();         // �p�b�N�}���̎��S����
        firstRound = false;
        SetLives(lives - 1);            // �c�@�����炷
        if (death != null)
        {
            death.Play();
        }
        if (lives > 0)
        {
            Invoke(nameof(NewRound), 1.5f);     // 1.5�b��Ƀ��Z�b�g
        }
        else
        {
            GameOver();             // �Q�[���I�[�o�[����
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * ghostMultiplier;            // �|�����S�[�X�g�̃X�R�A�v�Z
        SetScore(score + points);
        if (ghostdeath != null)
        {
            ghostdeath.Play();
        }
        ghostMultiplier++;           // �S�[�X�g�̃X�R�A�{���𑝉�
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);         // �y���b�g���\��

        SetScore(score + pellet.points);            // �X�R�A�����Z
        if (eaten != null)
        {
            eaten.Play();
        }
        //gameStartText.enabled = false;
        if (!HasRemainingPellets())                 // ���ׂẴy���b�g��H�ׂ����m�F
        {
            pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].frightened.Enable(pellet.duration);        // �S�[�X�g����莞�ԕ|���点��
        }

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));          // �ȑO�̃��Z�b�g�\����L�����Z��
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);  // ��莞�Ԍ�ɃX�R�A�{�������Z�b�g
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;            // �܂��y���b�g���c���Ă���ꍇ
            }
        }

        return false;           // ���ׂẴy���b�g��H�ׂ�
    }   

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;        // �S�[�X�g�̃X�R�A�{�������Z�b�g
    }


}