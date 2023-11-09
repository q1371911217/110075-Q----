using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum GameStatus
{
    Wait,
    Start,
    Over,
}

public class Game : MonoBehaviour
{
    public Transform menuLayer;
    public Transform menuCellRoot;

    public GameObject resultLayer;
    public Transform resultWinLayer;
    public Transform resultLoseLayer;

    public Transform levelRoot;

    public AudioClip audioClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    public Transform baozhaEffect;
    public Transform xingxingEffect;
    public Transform caidaiEffect;

    public GameObject btnVolume;
    public Text lblLevel, lblTime;
    public List<Transform> starList;

    public Player player;

    private int curStar;

    private const int totalTime = 30;
    private int residueTime;

    private int curLevel;
    private int maxLevel;

    private const int totalLevel = 5;

    private GameStatus gameStatus;

    public static bool volumOpen = true;

    private AudioSource audioSource;

    private void Awake()
    {
        curLevel = 1;
        maxLevel = PlayerPrefs.GetInt("maxLevel", 1);
    }

    private void Start()
    {
        gameStatus = GameStatus.Wait;

        audioSource = GameObject.Find("GameManager").GetComponent<AudioSource>();
        audioSource.volume = Game.volumOpen ? 1 : 0;
        btnVolume.transform.Find("spDisable").gameObject.SetActive(!Game.volumOpen);

        //gameStart();
        menuLayer.gameObject.SetActive(true);
        menuLayer.transform.localScale = Vector3.zero;
        menuLayer.transform.DOScale(Vector3.one, 0.3f);
        
        updateMenu();
    }

    void clear()
    {
        if (levelGo != null)
            GameObject.Destroy(levelGo);

        if(menuLayer.gameObject.activeSelf)
        {
            menuLayer.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
            {
                menuLayer.gameObject.SetActive(false);
            });
        }

        for (int i = 0; i < starList.Count; i++)
        {
            starList[i].transform.Find("spHighlight").gameObject.SetActive(false);
        }

        StopAllCoroutines();
        //if (!audioSource.isPlaying)
            audioSource.Play();
    }
    GameObject levelGo;
    Transform curLevelTrans;
    void gameStart()
    {
        clear();

        residueTime = totalTime;
        curStar = 0;

        lblLevel.text = string.Format("Level {0}", curLevel);
        lblTime.text = string.Format("Time {0}", residueTime);

        GameObject levelPrefs = Resources.Load<GameObject>(string.Format(string.Format("level/level_{0}", curLevel)));
        levelGo = GameObject.Instantiate(levelPrefs);
        Transform levelTrans = levelGo.transform;

        levelTrans.SetParent(levelRoot);
        levelTrans.localPosition = new Vector3(-11, -64, 0);
        levelTrans.localRotation = Quaternion.identity;
        levelTrans.localScale = Vector3.one;
        levelTrans.GetComponent<Image>().SetNativeSize();
        levelTrans.SetAsFirstSibling();
        player.transform.localPosition = levelTrans.Find("birth").localPosition;
        curLevelTrans = levelTrans;
        gameStatus = GameStatus.Start;
        player.setStart(true);
        StartCoroutine(coutdown());        
    }

    IEnumerator coutdown()
    {
        while(residueTime > 0 && gameStatus == GameStatus.Start)
        {
            yield return new WaitForSeconds(1);
            residueTime -= 1;
            lblTime.text = string.Format("Time {0}", residueTime);     
            if(residueTime == 0 && gameStatus == GameStatus.Start)
            {
                StartCoroutine(gameOver(false));
            }
        }
    }

    void updateMenu()
    {
        for(int i = 1; i <= maxLevel; i++)
        {
            Transform cell = menuCellRoot.GetChild(i - 1);
            cell.Find("spSelect").gameObject.SetActive(i == curLevel);
            cell.Find("lblLevel").GetComponent<Text>().text = i.ToString();
            cell.Find("spLock").gameObject.SetActive(false);
            int starCount = PlayerPrefs.GetInt(string.Format("starLevel{0}", i), 0);
            for(int j = 0; j < starCount; j++)
            {
                cell.Find(string.Format("star_{0}/spHighlight", j + 1)).gameObject.SetActive(true);
            }
            Button btn = cell.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            int tmpLevel = i;
            btn.onClick.AddListener(() =>
            {
                curLevel = tmpLevel;
                gameStart();
            });
        }
    }

    public void onBtnClick(string name)
    {
        audioSource.PlayOneShot(audioClip);
        if(name == "btnMenu")
        {
            displayResultLayer(false);
            menuLayer.gameObject.SetActive(true);
            menuLayer.transform.localScale = Vector3.zero;
            menuLayer.transform.DOScale(Vector3.one, 0.3f);
            updateMenu();
        }
        else if(name == "btnVolume")
        {
            Game.volumOpen = !Game.volumOpen;
            audioSource.volume = Game.volumOpen ? 1 : 0;
            btnVolume.transform.Find("spDisable").gameObject.SetActive(!Game.volumOpen);
        }
        else if(name == "btnRestart")
        {
            displayResultLayer(false);
            gameStart();
        }
        else if(name == "btnNext")
        {
            displayResultLayer(false);
            curLevel += 1;
            if (curLevel > totalLevel)
                curLevel = 1;
            gameStart();
        }else if(name == "btnHome")
        {
            SceneManager.LoadSceneAsync("LoginScene");
        }
    }

    public void onReceiveStar(Transform starTrans)
    {
        curStar += 1;
        starList[curStar - 1].transform.Find("spHighlight").gameObject.SetActive(true);

        xingxingEffect.gameObject.SetActive(false);
        xingxingEffect.gameObject.SetActive(true);
        xingxingEffect.localPosition = player.transform.localPosition;
    }

    public void onReceiveDestination()
    {
        if (gameStatus != GameStatus.Start) return;
        caidaiEffect.gameObject.SetActive(false);
        caidaiEffect.gameObject.SetActive(true);
        caidaiEffect.localPosition = curLevelTrans.Find("target").localPosition;
        StartCoroutine(gameOver(true));
    }

    public void onReceiveEnemy()
    {
        if (gameStatus != GameStatus.Start) return;
        baozhaEffect.gameObject.SetActive(false);
        baozhaEffect.gameObject.SetActive(true);
        baozhaEffect.transform.localPosition = player.transform.localPosition;
        player.transform.localPosition = new Vector3(5000, 5000, 0);
        StartCoroutine(gameOver(false));
    }

    IEnumerator gameOver(bool isWin)
    {
        gameStatus = GameStatus.Over;
        menuLayer.gameObject.SetActive(false);
        player.setStart(false);
        if(isWin)
        {
            if(curLevel + 1 > maxLevel)
            {
                maxLevel = curLevel + 1;
                if (maxLevel > totalLevel)
                    maxLevel = totalLevel;
                PlayerPrefs.SetInt("maxLevel", maxLevel);
            }
            int lastCount = PlayerPrefs.GetInt(string.Format("starLevel{0}", curLevel), 0);
            if(curStar > lastCount)
                PlayerPrefs.SetInt(string.Format("starLevel{0}", curLevel), curStar);
        }
        yield return new WaitForSeconds(0.5f);
        resultLayer.gameObject.SetActive(true);
        resultLayer.transform.localScale = Vector3.zero;
        displayResultLayer(true);

        audioSource.Stop();

        if (isWin)
        {
            audioSource.PlayOneShot(winClip);
            resultWinLayer.gameObject.SetActive(true);
            for (int i = 1; i <= 3; i++)
            {
                resultWinLayer.Find(string.Format("star_{0}", i)).gameObject.SetActive(i <= curStar);
            }
        }
        else
        {
            audioSource.PlayOneShot(loseClip);
            resultLoseLayer.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(2.5f);
        audioSource.Play();
    }

    void displayResultLayer(bool isShow)
    {
        Vector3 targetScale = isShow ? Vector3.one : Vector3.zero;
        if (!resultLayer.activeSelf) return;
        resultLayer.transform.DOScale(targetScale, 0.3f).OnComplete(() =>
        {
            if(!isShow)
            {
                resultWinLayer.gameObject.SetActive(false);
                resultLoseLayer.gameObject.SetActive(false);
                resultLayer.gameObject.SetActive(false);
            }
        });
    }
}
