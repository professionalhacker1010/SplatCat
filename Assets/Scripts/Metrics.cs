using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Metrics : MonoBehaviour
{
    public int d3 = 0;
    public int d4 = 0;
    public int d5 = 0;
    public int d6 = 0;
    public int d7 = 0;
    public int d8 = 0;
    private WWWForm form;

    private bool written3, written4, written5, written6, written7, written8;

    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSeX398-B2BJZ-8Rg26Moq4u2ri6XUl2R4pJmctElmmv_HJdKA/formResponse";



    // Start is called before the first frame update
    void Start()
    {
        form = new WWWForm();
        written3 = false;
        written4 = false;
        written5 = false;
        written6 = false;
        written7 = false;
        written8 = false;
    }

    public void Update()
    {
        switch (LevelManager.currLevel)
        {
            case 4:
                if (!written3)
                {
                    StartCoroutine(Post3(d3));
                    written3 = true;
                }
                break;
            case 5:
                if (!written4)
                {
                    StartCoroutine(Post4(d4));
                    written4 = true;
                }
                break;
            case 6:
                if (!written5)
                {
                    StartCoroutine(Post5(d5));
                    written5 = true;
                }
                break;
            case 7:
                if (!written6)
                {
                    StartCoroutine(Post6(d6));
                    written6 = true;
                }
                break;
            case 8:
                if (!written7)
                {
                    StartCoroutine(Post7(d7));
                    written7 = true;
                }
                break;
            case 9:
                if (!written8)
                {
                    StartCoroutine(Post8(d8));
                    written8 = true;
                }
                break;
            default:
                break;
        }
    }

    void OnApplicationQuit()
    {
        StartCoroutine(EndingPost());
    }

    IEnumerator Post3(int deaths3)
    {
        form.AddField("entry.846273522", deaths3);
        yield return null;
    }

    IEnumerator Post4(int deaths4)
    {
        form.AddField("entry.862647202", deaths4);
        yield return null;
    }

    IEnumerator Post5(int deaths5)
    {
        form.AddField("entry.1691556163", deaths5);
        yield return null;
    }

    IEnumerator Post6(int deaths6)
    {
        form.AddField("entry.382044568", deaths6);
        yield return null;
    }

    IEnumerator Post7(int deaths7)
    {
        form.AddField("entry.1785866450", deaths7);
        yield return null;
    }

    IEnumerator Post8(int deaths8)
    {
        form.AddField("entry.691596227", deaths8);
        yield return EndingPost();
    }

    IEnumerator EndingPost()
    {
        using (UnityWebRequest www = UnityWebRequest.Post(BASE_URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}
