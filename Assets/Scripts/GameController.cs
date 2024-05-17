using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardParent;
    public Sprite cardBackSprite;
    public List<Sprite> cardFaceSprites;
    public Text matchText;
    public Text turnText;
    public Text timerText;
    public Text messageText;
    public Text scoreText; // Added for displaying the score
    public GridLayoutGroup layoutGroup;

    private int rows;
    private int cols;
    private CardController firstCard;
    private CardController secondCard;
    private int matches = 0;
    private int turns = 0;
    private int score = 0; // Added to track the score
    private float timeRemaining = 180f; // 3 minutes
    private bool gameEnded = false;
    private bool isCheckingMatch = false;
    private List<CardController> clickableCards = new List<CardController>();
    private List<CardController> allCards = new List<CardController>();

    public AudioClip matchSound;
    public AudioClip notMatchSound;
    public Button SaveBTN;
    public Button LoadBTN;

    void Start()
    {
        Time.timeScale = 1;
        rows = GameObject.Find("MenuManager").GetComponent<Menu>().rows;
        cols = GameObject.Find("MenuManager").GetComponent<Menu>().cols;
        SetupBoard();
    }

    void Update()
    {
        if (!gameEnded)
        {
            UpdateTimer();
        }
    }

    void SetupBoard()
    {
        int totalCards = rows * cols;
        int numPairs = totalCards / 2;

        if (cardFaceSprites.Count < numPairs)
        {
            Debug.LogError("Not enough card face sprites assigned.");
            return;
        }

        List<int> cardValues = new List<int>();
        for (int i = 0; i < numPairs; i++)
        {
            cardValues.Add(i);
            cardValues.Add(i);
        }
        Shuffle(cardValues);
        int cardNumber = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject cardObject = Instantiate(cardPrefab, cardParent);
                cardNumber++;
                cardObject.name = "card_" + cardNumber;
                CardController card = cardObject.GetComponent<CardController>();
                int index = row * cols + col;
                card.SetCard(cardValues[index], cardBackSprite, cardFaceSprites[cardValues[index]]);
                clickableCards.Add(card);
                allCards.Add(card);
            }
        }
        UpdateGridLayout();
        UpdateUI();
    }

    

    public void CardFlipped(CardController card)
    {
        if (isCheckingMatch)
        {
            return;
        }

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null)
        {
            secondCard = card;
            turns++;
            isCheckingMatch = true;
            DisableAllClickableCards();

            if (firstCard.cardValue == secondCard.cardValue)
            {
                matches++;
                score += 10; // Increment score on match
                firstCard.LoadFlipped();
                secondCard.LoadFlipped();
                StartCoroutine(MatchEffects(firstCard));
                StartCoroutine(MatchEffects(secondCard));
                EnableAllClickableCards();
                firstCard.DisableCard();
                firstCard.transform.GetChild(1).gameObject.SetActive(false);
                secondCard.DisableCard();
                secondCard.transform.GetChild(1).gameObject.SetActive(false);
                clickableCards.Remove(firstCard);
                clickableCards.Remove(secondCard);
                firstCard = null;
                secondCard = null;
                isCheckingMatch = false;
            }
            else
            {
                StartCoroutine(HideCardsAfterDelay());
            }
        }
    }

    IEnumerator HideCardsAfterDelay()
    {
        yield return new WaitForSeconds(1);
        GetComponent<AudioSource>().PlayOneShot(notMatchSound);
        firstCard.HideCard();
        secondCard.HideCard();
        firstCard = null;
        secondCard = null;
        UpdateUI();
        EnableAllClickableCards(); // Enable all cards again after hiding
        isCheckingMatch = false;
    }

    IEnumerator MatchEffects(CardController card)
    {
        WaitForSeconds _w1 = new WaitForSeconds(1f);
        WaitForSeconds _w2 = new WaitForSeconds(2f);
        yield return _w1;
        UpdateUI();
        GetComponent<AudioSource>().PlayOneShot(matchSound);
        card.transform.GetChild(0).gameObject.SetActive(true);
        yield return _w2;
        card.transform.GetChild(0).gameObject.SetActive(false); 
        CheckWinCondition();
    }

    void UpdateTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            gameEnded = true;
            messageText.text = "Game Over!\nTime's up!";
            timerText.text = string.Format("{0:00}:{1:00}", 0, 0);
            matchText.text = "";
            turnText.text = "";
            scoreText.text = "";
            DisableForGameOver();
            SaveBTN.interactable=false;
            LoadBTN.interactable=false;
        }
    }

    void CheckWinCondition()
    {
        int totalCards = rows * cols;
        int numPairs = totalCards / 2;

        if (matches == numPairs)
        {
            gameEnded = true;
            matchText.text = "";
            turnText.text = "";
            scoreText.text = "";
            SaveBTN.interactable = false;
            LoadBTN.interactable = false;
            messageText.text = "Congratulations! \n You Win!";
        }
    }

    void UpdateUI()
    {
        matchText.text = "Matches: " + matches;
        turnText.text = "Turns: " + turns;
        scoreText.text = "Score: " + score;
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void SetGridSize(int r, int c)
    {
        rows = r;
        cols = c;
    }

    public void DisableAllClickableCards()
    {
        foreach (CardController card in clickableCards)
        {
            card.DisableCard();
        }
    }

    public void EnableAllClickableCards()
    {
        foreach (CardController card in clickableCards)
        {
            card.EnableCard();
        }
    }

    public void DisableForGameOver()
    {
        foreach (CardController card in clickableCards)
        {
            card.DisableForGameOver();
        }
    }
    /// <summary>
    /// ////////////////////////////SAVE & LOAD//////////////////////
    /// </summary>
     

    public void SaveGame()
    {
        PlayerPrefs.SetInt("rows", rows);
        PlayerPrefs.SetInt("cols", cols);
        PlayerPrefs.SetInt("turns", turns);
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetInt("matches", matches);
        PlayerPrefs.SetFloat("timeRemaining", timeRemaining);
        foreach (CardController card in allCards)
        {
            PlayerPrefs.SetInt(""+card.gameObject.name+"_value", card.GetComponent<CardController>().cardValue);
            if (card.GetComponent<CardController>().isFlipped == true)
            {
                PlayerPrefs.SetInt("" + card.gameObject.name + "_isFlipped", 1);
            }
            else
            {
                PlayerPrefs.SetInt("" + card.gameObject.name + "_isFlipped", 0);
            } 

        }
        //PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        rows = PlayerPrefs.GetInt("rows");
        cols = PlayerPrefs.GetInt("cols");
        GameObject.Find("MenuManager").GetComponent<Menu>().rows = rows;
        GameObject.Find("MenuManager").GetComponent<Menu>().cols = cols;
        turns =PlayerPrefs.GetInt("turns");
        score=PlayerPrefs.GetInt("score");
        matches=PlayerPrefs.GetInt("matches");
        timeRemaining= PlayerPrefs.GetFloat("timeRemaining");
        foreach (CardController card in allCards)
        {
            Destroy(card.gameObject);
        }
        clickableCards.Clear();
        allCards.Clear();
        firstCard = null;
        secondCard = null;
        SetupBoard();
        foreach (CardController card in allCards)
        {
            card.GetComponent<CardController>().cardValue = PlayerPrefs.GetInt("" + card.gameObject.name + "_value");
            card.SetCard(card.GetComponent<CardController>().cardValue, cardBackSprite, cardFaceSprites[card.GetComponent<CardController>().cardValue]);
            if (PlayerPrefs.GetInt("" + card.gameObject.name + "_isFlipped") == 1)
            {
                card.LoadFlipped();
                UpdateUI();
            }
             
            
            
        }
    }
    /// <summary>
    /// //////////////////////////////////////////////////////////////
    /// </summary>
    void UpdateGridLayout()
    {
        float layoutWidth = layoutGroup.GetComponent<RectTransform>().rect.width;
        float layoutHeight = layoutGroup.GetComponent<RectTransform>().rect.height;

        float maxCellWidth = layoutWidth / cols;
        float maxCellHeight = layoutHeight / rows;
        float cellSize = Mathf.Min(maxCellWidth, maxCellHeight);
        
        layoutGroup.cellSize = new Vector2(cellSize, cellSize);
        layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layoutGroup.constraintCount = cols;

        foreach (CardController card in clickableCards)
        {
            card.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta= new Vector2(cellSize, cellSize);  
            card.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta= new Vector2(cellSize, cellSize);

        }


    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void Replay()
    {
        
        SceneManager.LoadScene("mainScene");
        DontDestroyOnLoad(GameObject.Find("MenuManager").gameObject);
        Time.timeScale = 1;
    }
    public void Quit()
    {
        Destroy(GameObject.Find("MenuManager").gameObject);
        SceneManager.LoadScene("mainMenu");
    }
}