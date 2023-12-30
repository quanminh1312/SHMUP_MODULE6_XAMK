using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class AnimatedNumber : MonoBehaviour
{

    public int numberToDisplay = 0;
    public AnimatedChar[] chars;
    // Start is called before the first frame update
    void Start()
    {
        updateNumber(numberToDisplay);
    }
    void updateNumber(int newNumberToDisplay)
    {
        numberToDisplay = newNumberToDisplay;
        string numbers = numberToDisplay.ToString();
        int d = numbers.Length - 1;
        for (int i=0; i < chars.Length; i++)
        {
            int number = 0;
            if (d>=0) number = numbers[d] - '0';
            chars[i].digit = number;
            d--;
        }
    }
}
