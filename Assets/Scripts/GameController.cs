using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

    private int rows = 4;
    private int cols = 4;
    private CardController firstCard;
    private CardController secondCard;
    private int matches = 0;
    private int turns = 0;
    private int score = 0; // Added to track the score
    private float timeRemaining = 180f; // 3 minutes
    private bool gameEnded = false;
    private bool isCheckingMatch = false;
    private List<CardController> clickableCards = new List<CardController>();

    public AudioClip matchSound;
    public AudioClip notMatchSound;

    void Start()
    {
       // LoadScore();
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

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject cardObject = Instantiate(cardPrefab, cardParent);
                CardController card = cardObject.GetComponent<CardController>();
                int index = row * cols + col;
                card.SetCard(cardValues[index], cardBackSprite, cardFaceSprites[cardValues[index]]);
                clickableCards.Add(card);
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
                UpdateUI();
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
            int randomIndex = Random.Range(0, list.Count);
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

    public void SaveScore()
    {
        PlayerPrefs.SetInt("HighScore", score);
        PlayerPrefs.Save();
    }

    public void LoadScore()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            score = PlayerPrefs.GetInt("HighScore");
        }
    }

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
}