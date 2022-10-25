using System.Collections;
using AxieMixer.Unity;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
 
namespace Game
{
    public class demoTeamSetup : MonoBehaviour
    {
        [SerializeField] demoFigure _birdFigure1;
        [SerializeField] demoFigure _birdFigure2;

        bool _isPlaying = false;

        // Start is called before the first frame update
        void Start()
        {
            Time.timeScale = 0f;

            Mixer.Init();

            StartCoroutine(GetAxiesGenes("4191804", _birdFigure1));
            StartCoroutine(GetAxiesGenes("2724598", _birdFigure2));
        }

        // Update is called once per frame
        void Update()
        {
            if (!_isPlaying)
            {
                    _isPlaying = true;
                    Time.timeScale = 1f;
            }
            
        }

        //Get model
        public IEnumerator GetAxiesGenes(string axieId, demoFigure _birdFigure)
        {
            string searchString = "{ axie (axieId: \"" + axieId + "\") { id, genes, newGenes}}";
            JObject jPayload = new JObject();
            jPayload.Add(new JProperty("query", searchString));

            var wr = new UnityWebRequest("https://graphql-gateway.axieinfinity.com/graphql", "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jPayload.ToString().ToCharArray());
            wr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            wr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            wr.SetRequestHeader("Content-Type", "application/json");
            wr.timeout = 10;
            yield return wr.SendWebRequest();
            if (wr.error == null)
            {
                var result = wr.downloadHandler != null ? wr.downloadHandler.text : null;
                if (!string.IsNullOrEmpty(result))
                {
                    JObject jResult = JObject.Parse(result);
                    string genesStr = (string)jResult["data"]["axie"]["newGenes"];
                    PlayerPrefs.SetString("selectingId", axieId);
                    PlayerPrefs.SetString("selectingGenes", genesStr);
                    _birdFigure.SetGenes(axieId, genesStr);
                }
            }
        }
    }
}
