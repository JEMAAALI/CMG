using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public int cardValue;
    public bool isFlipped = false;
    public bool matchCheck = true;
    private Button button;
    private Image image;

    private Sprite cardFace;
    private Sprite cardBack;

    private Animation animation;


    void Awake()
    {
        //button = this.transform.GetChild(0).GetComponent<Button>();
        button = GetComponent<Button>();
        //image = this.transform.GetChild(0).GetComponent<Image>();
        image = GetComponent<Image>();
        animation = GetComponent<Animation>();
        button.onClick.AddListener(OnCardClicked);
    }

    public void SetCard(int value, Sprite backSprite, Sprite faceSprite)
    {
        cardValue = value;
        cardBack = backSprite;
        cardFace = faceSprite;
        HideCard();
    }

    public void OnCardClicked()
    {
        GetComponent<AudioSource>().Play();
        if (!isFlipped)
        {
            StartCoroutine(FlipCard());
            if (matchCheck == true) { 
            FindObjectOfType<GameController>().CardFlipped(this);
            }
        }
    }

    IEnumerator FlipCard()
    {
        WaitForSeconds _w = new WaitForSeconds(0.5f);
        animation.Play();  
        yield return _w;
        isFlipped = true;
        image.sprite = cardFace;
    }

    public void HideCard()
    {
        isFlipped = false;
        image.sprite = cardBack;
    }

    public void DisableCard()
    {
        button.onClick.RemoveListener(OnCardClicked);
        transform.GetChild(1).gameObject.SetActive(true);
        //Enable hidden button that only play animation & hide it on Enable card
        //button.GetComponent<Animation>().Play();
        //StartCoroutine(DIS());
    }

    public void HiddenButtonListenr()
    {
        StartCoroutine(FlipCardToSeeOnly()); 
    }

    IEnumerator FlipCardToSeeOnly()
    {
        WaitForSeconds _w = new WaitForSeconds(0.5f);
        animation.Play();
        yield return _w;
        image.sprite = cardFace;
        yield return _w;
        image.sprite = cardBack;
    }

    public void DisableForGameOver()
    {
        button.onClick.RemoveListener(OnCardClicked);
        button.interactable = false;
    }

    public void EnableCard()
    {
        button.onClick.AddListener(OnCardClicked);
        transform.GetChild(1).gameObject.SetActive(false);
        //button.interactable = true;
    }


    public bool IsMatched()
    {
        return !button.interactable;
    }
}
