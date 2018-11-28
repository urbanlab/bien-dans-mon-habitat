using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum Enum_HotspotType
{
    text,
    image,
    video,
    spawn,
    textPlus
};

public class CHotspot : MonoBehaviour {

    private CMainManager _main;
    private Canvas _cnvHotspotScreen;
    private GameObject _goPlayer;
    private CGameManager _gameManager;
    private bool _bIsFirstTime;
    //Atributos
    public Enum_HotspotType _enumType;
    public string _strTitle;
    [TextArea]
    public string _strDescription;
    public Sprite _spriteIcon;
    public VideoClip _clipVideo;
    public string _strURLVideo;
    public GameObject _goSpawnHotspot;
    public AudioClip _voiceTitle;
    public AudioClip _voiceDescription;
	//--------------------------------------------------
    //
    //--------------------------------------------------
	void Start () {
        _main = CMainManager.Get();
        _gameManager = CGameManager.Get();
        _cnvHotspotScreen = GameObject.Find("hotspot_screen").GetComponent<Canvas>();
        _goPlayer = GameObject.Find("Player");
        _bIsFirstTime = true;
        transform.Find("hover").gameObject.SetActive(false);
	}
    //--------------------------------------------------
    //
    //--------------------------------------------------
    public void OnClick()
    {
        _main.ShowCanvas(false);
        _main.ShowCanvas(true);
        _cnvHotspotScreen.gameObject.GetComponent<AudioSource>().Stop();
        _cnvHotspotScreen.transform.Find("container/label_title").GetComponent<Text>().text = _strTitle.ToUpper();

        switch(_enumType)
        {
            case Enum_HotspotType.text:
                int iTextLength = _strDescription.Length;
                //G: Min(0.25,0.125) Max(0.75,0.825)
                //S: Min(0.3,0.3) Max(0.7,0.8)
                Debug.Log("Letters:" + iTextLength);
                if(iTextLength < 150)
                {
                    _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMax = new Vector2(0.725f, 0.8f);
                    _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMin = new Vector2(0.225f, 0.35f);
                }
                else if(iTextLength > 400)
                {
                    _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMax = new Vector2(0.8f, 0.875f);
                    _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMin = new Vector2(0.2f, 0.075f);
                }
                else
                {
                    _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMax = new Vector2(0.75f, 0.85f);
                    _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMin = new Vector2(0.25f, 0.25f);
                }

                _cnvHotspotScreen.transform.Find("container/label_description").gameObject.SetActive(true);
                _cnvHotspotScreen.transform.Find("container/label_description").GetComponent<Text>().text = _strDescription;

                if (_voiceDescription != null)
                {
                    _cnvHotspotScreen.gameObject.GetComponent<AudioSource>().PlayOneShot(_voiceDescription);
                }

                break;

            case Enum_HotspotType.image:
                _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMax = new Vector2(0.8f, 0.9f);
                _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMin = new Vector2(0.2f, 0.1f);
                _cnvHotspotScreen.transform.Find("container/Image").gameObject.SetActive(true);
                _cnvHotspotScreen.transform.Find("container/Image").GetComponent<Image>().sprite = _spriteIcon;
                break;

            case Enum_HotspotType.video:
                _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.9f);
                _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMin = new Vector2(0.1f, 0.05f);
                _cnvHotspotScreen.transform.Find("container/Video").gameObject.SetActive(true);
                _cnvHotspotScreen.transform.Find("container/Video").GetComponent<StreamVideo>()._strURL = _strURLVideo;
                _cnvHotspotScreen.transform.Find("container/Video").GetComponent<StreamVideo>().videoToPlay = _clipVideo;
                _cnvHotspotScreen.transform.Find("container/Video").GetComponent<StreamVideo>().PlayVideo(); 
                break;

            case Enum_HotspotType.spawn:
                _main.GoToSpawn(_goSpawnHotspot);
                break;

            case Enum_HotspotType.textPlus:
                _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMax = new Vector2(0.725f, 0.8f);
                _cnvHotspotScreen.transform.Find("container").GetComponent<RectTransform>().anchorMin = new Vector2(0.225f, 0.45f);
                _cnvHotspotScreen.transform.Find("container/label_description_plus").gameObject.SetActive(true);
                _cnvHotspotScreen.transform.Find("container/image_plus").gameObject.SetActive(true);
                _cnvHotspotScreen.transform.Find("container/label_description_plus").GetComponent<Text>().text = _strDescription;
                _cnvHotspotScreen.transform.Find("container/image_plus").GetComponent<Image>().sprite = _spriteIcon;

                if (_voiceDescription != null)
                {
                    _cnvHotspotScreen.gameObject.GetComponent<AudioSource>().PlayOneShot(_voiceDescription);
                }

                break;
        }
    }
    //--------------------------------------------------
    //
    //--------------------------------------------------
    public void OnHover()
    {
        transform.Find("hover").gameObject.SetActive(true);

        if (_voiceTitle != null && _bIsFirstTime)
        {
            _bIsFirstTime = false;
            if (!_cnvHotspotScreen.gameObject.GetComponent<AudioSource>().isPlaying)
            {
                //_bIsFirstTime = false;
                _cnvHotspotScreen.gameObject.GetComponent<AudioSource>().PlayOneShot(_voiceTitle);
            }
        }
    }
    //--------------------------------------------------
    //
    //--------------------------------------------------
    public void Reset()
    {
        _bIsFirstTime = true;
        transform.Find("hover").gameObject.SetActive(false);
    }
    //--------------------------------------------------
    //
    //--------------------------------------------------
    public Enum_HotspotType GetHotspotType()
    {
        return _enumType;
    }
    //--------------------------------------------------
    //
    //--------------------------------------------------
    void ResetCanvas()
    {

    }
    //--------------------------------------------------
    //
    //--------------------------------------------------
    void Update () {
		
	}
}
