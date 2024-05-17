using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Text A1Text;
    public Text A2Text;
    public Button A1IncrementButton;
    public Button A1DecrementButton;
    public Button A2IncrementButton;
    public Button A2DecrementButton;
    public int rows;
    public int cols;

    int[] A1 = { 2, 3, 4, 5, 6, 7, 8, 9 };
    int[] A2 = { 2, 3, 4, 5, 6, 7, 8, 9 };

    List<int> validA1Selections = new List<int>();
    List<int> validA2Selections = new List<int>();

    int currentA1Index = 0;
    int currentA2Index = 0;

    void Start()
    {
        validA1Selections.AddRange(A1);
        validA2Selections.AddRange(A2);

        A1IncrementButton.onClick.AddListener(IncrementA1);
        A1DecrementButton.onClick.AddListener(DecrementA1);
        A2IncrementButton.onClick.AddListener(IncrementA2);
        A2DecrementButton.onClick.AddListener(DecrementA2);

        UpdateA1Text();
        UpdateA2Text();
    }

    void IncrementA1()
    {
        do
        {
            currentA1Index = (currentA1Index + 1) % validA1Selections.Count;
        } while (!IsValidProduct(validA1Selections[currentA1Index], int.Parse(A2Text.text)));

        UpdateA1Text();
        UpdateValidA2Selections();
    }

    void DecrementA1()
    {
        do
        {
            currentA1Index = (currentA1Index - 1 + validA1Selections.Count) % validA1Selections.Count;
        } while (!IsValidProduct(validA1Selections[currentA1Index], int.Parse(A2Text.text)));

        UpdateA1Text();
        UpdateValidA2Selections();
    }

    void IncrementA2()
    {
        do
        {
            currentA2Index = (currentA2Index + 1) % validA2Selections.Count;
        } while (!IsValidProduct(int.Parse(A1Text.text), validA2Selections[currentA2Index]));

        UpdateA2Text();
        UpdateValidA1Selections();
    }

    void DecrementA2()
    {
        do
        {
            currentA2Index = (currentA2Index - 1 + validA2Selections.Count) % validA2Selections.Count;
        } while (!IsValidProduct(int.Parse(A1Text.text), validA2Selections[currentA2Index]));

        UpdateA2Text();
        UpdateValidA1Selections();
    }

    void UpdateA1Text()
    {
        A1Text.text = validA1Selections[currentA1Index].ToString();
        rows = validA1Selections[currentA1Index];
    }

    void UpdateA2Text()
    {
        A2Text.text = validA2Selections[currentA2Index].ToString();
        cols = validA2Selections[currentA2Index];
    }

    void UpdateValidA1Selections()
    {
        validA1Selections.Clear();
        int selectedA2 = int.Parse(A2Text.text);
        foreach (int value in A1)
        {
            if (IsValidProduct(value, selectedA2))
            {
                validA1Selections.Add(value);
            }
        }
        currentA1Index = validA1Selections.IndexOf(int.Parse(A1Text.text));
    }

    void UpdateValidA2Selections()
    {
        validA2Selections.Clear();
        int selectedA1 = int.Parse(A1Text.text);
        foreach (int value in A2)
        {
            if (IsValidProduct(selectedA1, value))
            {
                validA2Selections.Add(value);
            }
        }
        currentA2Index = validA2Selections.IndexOf(int.Parse(A2Text.text));
    }

    bool IsValidProduct(int a1, int a2)
    {
        int product = a1 * a2;
        return product <= 30 && product % 2 == 0;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene("mainScene");
        DontDestroyOnLoad(GameObject.Find("MenuManager").gameObject);
    }
}
