using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CMainManager : MonoBehaviour {

    private CGameManager _gameManager;
    private CLanguageManager _languageManager;
    private GameObject[] _hotspotArray; 

    private GameObject _goFadeSphere;
    private float _fFade;
    private bool _bFaded;
    private bool _bFullScreen;

    private Camera _MainCamera;

    private bool _bChangedFS = false;
    private bool _bChangedUS = true;

    private TextMesh[] lbl_VideoButton;

    private Canvas _cnvHotspotScreen;
    private Canvas _cnvHudScreen;

    public GameObject[] _SpawnPoints;
    private int _iCurrentRoom;
    private bool _bIsOverButton;
    //private Button _btnNextRoom;
    //private Button _btnPrevRoom;

    private bool _bChangedDownloading = false;
    private bool _bChangedFinishDownload = false;

    private Texture2D[] _textPlaces = new Texture2D[7];
    public string[] _stringURL;

    public static CMainManager Get()
    {
        return GameObject.Find("_main").GetComponent<CMainManager>();
    }
    //------------------------------------------------------------
    //
    //------------------------------------------------------------
    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(0.1f);
                
        _fFade = _fFade - 0.1f;
        
        if (_fFade > 0)
        {
            _goFadeSphere.GetComponent<Renderer>().material.color = new Color(0,0,0,_fFade);
            StartCoroutine(FadeOut());
        }
        else
        {
            _cnvHudScreen.enabled = true;
            _gameManager.HideLoadingScreen();
            _goFadeSphere.SetActive(false);
            _bFaded = true;
        }
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.1f);

        _fFade = _fFade + 0.1f;

        if (_fFade < 1)
        {
            _goFadeSphere.GetComponent<Renderer>().material.color = new Color(0, 0, 0, _fFade);
            StartCoroutine(FadeIn());
        }
        else
        {
            //_goFadeSphere.SetActive(false);
            _bFaded = true;
        }
    }

    IEnumerator Teleport(GameObject a_NewParent)
    {
        _cnvHudScreen.enabled = false;
        yield return new WaitForSeconds(0.1f);

        _fFade = _fFade + 0.1f;

        if (_fFade < 1)
        {
            _goFadeSphere.GetComponent<Renderer>().material.color = new Color(0, 0, 0, _fFade);
            StartCoroutine(Teleport(a_NewParent));
        }
        else
        {
            GameObject.Find("Player").transform.SetParent(a_NewParent.transform);
            GameObject.Find("Player").transform.localPosition = Vector3.zero;
            ResetPlacesExcept(a_NewParent);
            StartCoroutine(FadeOut());
        }
    }

    void Awake()
    {
        if (!CGameManager.IsStartSceneInit())
        {
            CGameManager.LoadLevel("Start");
            return;
        }
        _gameManager = CGameManager.Get();
        _languageManager = GameObject.Find("_language_manager").GetComponent<CLanguageManager>();

       // _gameManager.DownloadTexture(_SpawnPoints, _textPlaces, _stringURL);
        
    }


    public void GoToSpawn(GameObject a_Spawn)
    {
        int iPlace = a_Spawn.GetComponent<CPlace>()._iPlace;
        _cnvHotspotScreen.enabled = false;
        _bFaded = false;
        _gameManager.DownloadOneTexture(a_Spawn,_textPlaces[iPlace], _stringURL[iPlace]);
    }

    public void StartTheSpawn(GameObject a_Spawn)
    {
        a_Spawn.SetActive(true);
        _cnvHotspotScreen.enabled = false;
        _bFaded = false;
        _goFadeSphere.SetActive(true);
        StartCoroutine(Teleport(a_Spawn));
    }

	// Use this for initialization
	void Start () {
        _bIsOverButton = false;
        GetObjects();
        Init();
	}

    void GetObjects()
    {
        _MainCamera = GameObject.Find("Camera").GetComponentInChildren<Camera>();
        _hotspotArray = GameObject.FindGameObjectsWithTag("hotspot");
        _goFadeSphere = GameObject.Find("Fade").gameObject;
        _cnvHotspotScreen = GameObject.Find("hotspot_screen").GetComponent<Canvas>();
        _cnvHudScreen = GameObject.Find("hud_screen").GetComponent<Canvas>();

        // _btnNextRoom = _cnvHudScreen.transform.Find("container/button_right").GetComponent<Button>();
        // _btnPrevRoom = _cnvHudScreen.transform.Find("container/button_left").GetComponent<Button>();
    }

    void Init()
    {
        ResetPlaces();
        _fFade = 1.0F;
        _iCurrentRoom = 1;
        _SpawnPoints[_iCurrentRoom].SetActive(true);
        _bFullScreen = false;
        _bFaded = false;
        _cnvHotspotScreen.enabled = false;
        _gameManager.InitialDownloadSettings();
        _gameManager.ResizeText(ref _cnvHotspotScreen);
        StartCoroutine(FadeOut());
    }
	
	// Update is called once per frame
    public void IsOverButton(bool a_Value)
    {
        _bIsOverButton = a_Value;
    }
	void Update () {

        //Size Text Full Screen
        if(Screen.fullScreen)
        {
            if(!_bChangedFS)
            {
                ShowCanvas(false);
                _bChangedUS = false;
                _bChangedFS = true;
                //_gameManager.ResizeText(ref _cnvHotspotScreen);
                //GameObject myObject;
                //myObject = _cnvHotspotScreen.transform.Find("container").gameObject;
               // _gameManager.ResizeFullScreen(_cnvHotspotScreen.transform.Find("container").gameObject);
            }
        }
        else
        {
            if(!_bChangedUS)
            {
                _bChangedUS = true;
                _bChangedFS = false;
                ShowCanvas(false);
                //_gameManager.CalculateDeviceScale();
                //_gameManager.ResizeText(ref _cnvHotspotScreen);
                //_gameManager.ResizeUnFullScreen(_cnvHotspotScreen.transform.Find("container").gameObject);
            }
        }

        if (_bFaded)
        {
            if (!_bIsOverButton)
            {
                    RaycastHit hit;
                    Ray ray = _MainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        GameObject goHit = hit.transform.gameObject;

                        if (goHit.tag == "hotspot")
                        {
                            if (Input.GetMouseButtonDown(0))  //Select
                            {
                                Debug.Log("You selected the <color=green>" + goHit.transform.name + "</color>"); // ensure you picked right object
                                goHit.GetComponent<CHotspot>().OnClick();
                            }
                            else
                            {
                                goHit.GetComponent<CHotspot>().OnHover();
                            }
                        }
                        else
                        {
                            ResetButtons();
                        }
                    }
                    else
                    {
                        ResetButtons();
                    }
                    //-----------

                }
        }

        if (!_bChangedDownloading && _gameManager.IsDownloadingTextures())
        { 
            _bChangedDownloading = true;
            //DisabledSpawnHotspots();
        }

        else if(!_bChangedFinishDownload && !_gameManager.IsDownloadingTextures())
        {
            _bChangedFinishDownload = true;
            //EnabledSpawnHotspots();
        }

        if (_gameManager.IsDownloadingTextures())
        {
            //_btnPrevRoom.enabled = false;
            //_btnPrevRoom.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            //_btnNextRoom.enabled = false;
            //_btnNextRoom.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else
        {
            //if (_iCurrentRoom == 0)
            //{
            //    _btnPrevRoom.enabled = false;
            //    _btnPrevRoom.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            //}
            //else if (_iCurrentRoom == 6)
            //{
            //    _btnNextRoom.enabled = false;
            //    _btnNextRoom.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            //}
            //else
            //{
            //    _btnNextRoom.enabled = true;
            //    _btnPrevRoom.enabled = true;

            //    _btnPrevRoom.GetComponent<Image>().color = Color.white;
            //    _btnNextRoom.GetComponent<Image>().color = Color.white;
            //}
        }
	}

    //---------------------------------------------------------------------------------------------------
    // BUTTONS
    //--------------------------------------------------------------------------------------------------
    #region OnClick
    public void GoNextRoom()
    {
        _iCurrentRoom++;
        _SpawnPoints[_iCurrentRoom].SetActive(true);
        if (_cnvHotspotScreen.transform.Find("container/Video").GetComponent<AudioSource>())
        {
            _cnvHotspotScreen.transform.Find("container/Video").GetComponent<StreamVideo>().StopVideo();
        }
        GoToSpawn(_SpawnPoints[_iCurrentRoom]);
    }

    public void GoPrevRoom()
    {
        _iCurrentRoom--;
        _SpawnPoints[_iCurrentRoom].SetActive(true);
        if (_cnvHotspotScreen.transform.Find("container/Video").GetComponent<AudioSource>())
        {
            _cnvHotspotScreen.transform.Find("container/Video").GetComponent<StreamVideo>().StopVideo();
        }
        GoToSpawn(_SpawnPoints[_iCurrentRoom]);
    }
    #endregion

    //---------------------------------------------------------------------------------------------------
    // CANVAS
    //--------------------------------------------------------------------------------------------------
    public void ShowCanvas(bool a_Value)
    {
        _cnvHotspotScreen.enabled = a_Value;

        if(!a_Value)
        {
            if (_cnvHotspotScreen.transform.Find("container/Video").GetComponent<AudioSource>())
            {
                _cnvHotspotScreen.transform.Find("container/Video").GetComponent<StreamVideo>().StopVideo();
            }

            _cnvHotspotScreen.gameObject.GetComponent<AudioSource>().Stop();
        }

        ResetCanvas();
        
    }

    void ResetCanvas()
    {
        _cnvHotspotScreen.transform.Find("container/label_description").gameObject.SetActive(false);
        _cnvHotspotScreen.transform.Find("container/Image").gameObject.SetActive(false);
        _cnvHotspotScreen.transform.Find("container/Video").gameObject.SetActive(false);
        _cnvHotspotScreen.transform.Find("container/image_plus").gameObject.SetActive(false);
        _cnvHotspotScreen.transform.Find("container/label_description_plus").gameObject.SetActive(false);
        _cnvHotspotScreen.transform.Find("container/button_re-play").gameObject.SetActive(false);
        _cnvHotspotScreen.transform.Find("container/overlay_video").gameObject.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------------
    // 
    //--------------------------------------------------------------------------------------------------
    void ResetButtons()
    {
        for (int i = 0; i < _hotspotArray.Length; i++)
        {
            _hotspotArray[i].gameObject.GetComponent<CHotspot>().Reset();
        }
    }
    //---------------------------------------------------------------------------------------------------
    // 
    //--------------------------------------------------------------------------------------------------
    void ResetPlaces()
    {
        for(int i = 0; i < _SpawnPoints.Length; i++)
        {
            _SpawnPoints[i].SetActive(false);
        }
    }
    //---------------------------------------------------------------------------------------------------
    // 
    //--------------------------------------------------------------------------------------------------
    void DisabledSpawnHotspots()
    {
        for (int i = 0; i < _hotspotArray.Length; i++)
        {
            if (_hotspotArray[i].gameObject.GetComponent<CHotspot>().GetHotspotType() == Enum_HotspotType.spawn)
            {
                _hotspotArray[i].gameObject.GetComponent<BoxCollider>().enabled = false;
                _hotspotArray[i].gameObject.transform.Find("icon").GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
    //---------------------------------------------------------------------------------------------------
    // 
    //--------------------------------------------------------------------------------------------------
    void EnabledSpawnHotspots()
    {
        for (int i = 0; i < _hotspotArray.Length; i++)
        {
            if (_hotspotArray[i].gameObject.GetComponent<CHotspot>().GetHotspotType() == Enum_HotspotType.spawn)
            {
                _hotspotArray[i].gameObject.GetComponent<BoxCollider>().enabled = true;
                _hotspotArray[i].gameObject.transform.Find("icon").GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
    //---------------------------------------------------------------------------------------------------
    // 
    //--------------------------------------------------------------------------------------------------
    void ResetPlacesExcept(GameObject a_Exception)
    {
        for (int i = 0; i < _SpawnPoints.Length; i++)
        {
            if(a_Exception != _SpawnPoints[i])
                _SpawnPoints[i].SetActive(false);
        }
    }
    //---------------------------------------------------------------------------------------------------
    // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    //--------------------------------------------------------------------------------------------------
}
