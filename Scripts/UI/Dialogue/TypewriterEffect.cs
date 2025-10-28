using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    
    public float typewriterSpeed;

    public bool IsRunning { get; private set; }

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>(){'.', '!', '?' }, .6f ),
        new Punctuation(new HashSet<char>(){',', ';', ':'}, .3f )
    };


    private Coroutine typingCoroutine;

    public void Run(string textToType, TMP_Text textLabel)
    {
        typingCoroutine = StartCoroutine(TypeText(textToType, textLabel));
    }

    public void Stop()
    {
        StopCoroutine(typingCoroutine);
        IsRunning = false;
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        IsRunning = true;
        float t = 0;
        int charIndex = 0;

       // textLabel.text = string.Empty; // Every time a new textobject comes into play, this will make sure the screen is empty

        textLabel.maxVisibleCharacters = 0;
        textLabel.text = textToType;

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;

            t += Time.fixedDeltaTime * typewriterSpeed; // TIME>DELTATIME TIME.DELTATIME TIME.DELTATIME TIME.DELTATIME TIME.DELTATIME TIME.DELTATIME

            charIndex = Mathf.FloorToInt(t); // Rounds down to a whole number
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length); // Makes sure that charIndex is never greater than textToType.Length


            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= textToType.Length - 1;

                //textLabel.text = textToType.Substring(0, i + 1); // Starts at the first letter
                textLabel.maxVisibleCharacters = i + 1;

                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                {
                    yield return new WaitForSeconds(waitTime);
                }

            }

            yield return new WaitForSeconds(.02f);
        }

        textLabel.maxVisibleCharacters = textToType.Length;

        IsRunning = false;
        // After while loop is done typing letters, make sure the text we want and the text we have is the same
        textLabel.text = textToType;
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach(Punctuation punctionCategory in punctuations)
        {
            if (punctionCategory.Punctuations.Contains(character)) // Stuff 1     Look down
            {
                waitTime = punctionCategory.WaitTime; // Stuff 2
                return true;
            }
        }
        waitTime = default;
        return false;
    }

    private readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations; // Stuff 1    Look up
        public readonly float WaitTime; // Stuff 2

        public Punctuation(HashSet<char> punctuations, float waitTime)
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
}
