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
        button = GetComponent<Button>();
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
        button.onClick.RemoveListener(OnCardClicked);
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


    public void LoadFlipped()
    { 
        isFlipped = true;
        StartCoroutine(FlipCard());
    }
    IEnumerator Flip()
    {
        WaitForSeconds _w = new WaitForSeconds(0.5f);
        animation.Play();
        yield return _w;
        image.sprite = cardFace;
    }

}
