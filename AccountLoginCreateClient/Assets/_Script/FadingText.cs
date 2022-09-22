using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FadingText : MonoBehaviour
{
    [SerializeField]
    private Text textComp;

    [Range(0, 10000000)]
    public float fadeTime;

    private void Awake()
    {
        textComp = GetComponent<Text>();
        Color colour = textComp.color;
        colour.a = 0;
        textComp.color = colour;
    }

    public void InvokeText()
    {
        Color colour = textComp.color;
        colour.a = 1;
        textComp.color = colour;
        StartCoroutine(Fading());
    }
    public void ChangeText(string newText)
    {
        textComp.text = newText;
    }

    private IEnumerator Fading()
    {
        while (textComp.color.a >= 0)
        {
            yield return new WaitForFixedUpdate();
            Color colour = textComp.color;
            colour.a -= (1/ fadeTime) * Time.fixedDeltaTime;
            textComp.color = colour;
        }
        Debug.Log("a");
    }

}
