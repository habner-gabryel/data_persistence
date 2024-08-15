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
    private bool m_Started = false;
    private int m_Points;
    private bool m_GameOver = false;
    public static MainManager Instance;
    public string playerName;
    public string recordPlayer;
    public int recordPoints;
    private static string path = Application.persistentDataPath + "/savefile.json";
    private string persistentDataPath;

    private void Awake(){
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public int score;
    }

    public void SaveRecord()
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }

        SaveData data = new SaveData();
        data.playerName = playerName;
        data.score = m_Points;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(path, json);
    }

    public void SaveNewRecord() {
        getRecordScore();

        if(m_Points > recordPoints){
            SaveRecord();
        }
    }

    public void getRecordScore() {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            recordPoints = data.score;
            recordPlayer = data.playerName;
        }

        if(!recordPlayer.Equals("") && recordPoints != 0){
            BestScoreText.text = "Record : " + recordPlayer + " - " + recordPoints;
        } else {
            BestScoreText.text = "Record : ";
        }
    }

    void Start()
    {
        persistentDataPath = Application.persistentDataPath;

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
