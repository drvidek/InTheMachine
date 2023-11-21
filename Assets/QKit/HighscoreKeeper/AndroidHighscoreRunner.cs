using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace QKit
{

    public class AndroidHighscoreKeeper : MonoBehaviour
    {
        public void Run(string path)
        {
            var routine = GetDefaultEntriesOnAndroid(path);
            StartCoroutine(routine);
        }

        public IEnumerator GetDefaultEntriesOnAndroid(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        var text = webRequest.downloadHandler.text;
                        string[] entryArray = text.Split('\n');
                        foreach (var save in entryArray)
                        {
                            if (save == null || save == "")
                                continue;

                            string[] splitData = save.Split(':');
                            //KeyValuePair<string, float> pair = new(splitData[0], float.Parse(splitData[1]));
                            HighscoreKeeper.ValidateNewEntry(splitData[0], float.Parse(splitData[1]));

                        }
                        break;
                }
            }
        }
    }
}