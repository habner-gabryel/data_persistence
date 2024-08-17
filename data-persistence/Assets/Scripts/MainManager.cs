using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;
    protected bool m_Started = false;
    private int m_Points;
    protected bool m_GameOver = false;
    public static MainManager Instance;
    public string playerName;
    public string recordPlayer;
    public int recordPoints;
    private string path => Path.Combine(Application.persistentDataPath, "/savefile.json");
    private string pathName => Path.Combine(Application.persistentDataPath, "/save-playername-file.json");

    [System.Serializable]
    class ScoreData
    {
        public string playerName;
        public int score;
    }

    [System.Serializable]
    class NameData
    {
        public string playerName;
    }

    private void Awake(){
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveRecord()
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }

        ScoreData data = new ScoreData();
        data.playerName = playerName;
        data.score = m_Points;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(path, json);
    }

    public void SavePlayerName(string name) {
        NameData data = new NameData();
        data.playerName = name;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(pathName, json);
    }

    public void SaveNewRecord() {
        getRecordScore();

        if(m_Points > recordPoints){
            getPlayerName();
            SaveRecord();
        }
    }

    public void getRecordScore() {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);

            recordPoints = data.score;
            recordPlayer = data.playerName;
        }

        if(recordPlayer != null && !recordPlayer.Equals("") && recordPoints != 0){
            BestScoreText.text = "Record : " + recordPlayer + " - " + recordPoints;
        } else {
            BestScoreText.text = "Record :";
        }
    }

    public void getPlayerName() {
        if (File.Exists(pathName))
        {
            string json = File.ReadAllText(pathName);
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);

            playerName = data.playerName;
        }
    }

    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        m_Points = 0;
        m_Started = false;
        m_GameOver = false;
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        getRecordScore();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Destroy(gameObject);
                SceneManager.LoadScene(0);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        SaveNewRecord();
        m_GameOver = true;
        GameOverText.SetActive(true);
    }
}
