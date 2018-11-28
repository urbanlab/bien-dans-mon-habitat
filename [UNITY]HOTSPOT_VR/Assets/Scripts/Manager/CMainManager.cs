using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMainManager : MonoBehaviour {

    private CGameManager _gameManager;
    private CLanguageManager _languageManager;
    private GameObject[] _hotspotArray;

    private GameObject _goFadeSphere;
    private float _fFade;
    private bool _bFaded;

    private Camera _MainCamera;
    private GameObject _goMainObject;
    private LoadingCircle _LoadingCircle;
    private int _iButtonLayer = 1 << 8;

    private TextMesh[] lbl_VideoButton;
    private List<CVideo> _ListVideo;

    private GameObject _goScreen;

    private GameObject _latestScreen;
    private Transform _latestScreenParent;
    //------------------------------------------------------------
    //
    //------------------------------------------------------------
    public static CMainManager Get()
    {
        return GameObject.Find("_main").GetComponent<CMainManager>();
    }
    //------------------------------------------------------------
    //
    //------------------------------------------------------------
    public GameObject GetScreen()
    {
        return _goScreen;
    }

    public Camera getMainCamera()
    {
        return _MainCamera;
    }

    public bool IsFaded()
    {
        return _bFaded;
    }
    //------------------------------------------------------------
    //
    //------------------------------------------------------------
    #region Start
    void Awake()
    {
        if (!CGameManager.IsStartSceneInit())
        {
            CGameManager.LoadLevel("Start");
            return;
        }

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
            GameObject.Find("Fade").GetComponent<Renderer>().material.color = new Color(0, 0, 0, _fFade);
            StartCoroutine(FadeOut());
        }
        else
        {
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
            GameObject.Find("Fade").GetComponent<Renderer>().material.color = new Color(0, 0, 0, _fFade);
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
        yield return new WaitForSeconds(0.1f);

        _fFade = _fFade + 0.1f;

        if (_fFade < 1)
        {
            GameObject.Find("Fade").GetComponent<Renderer>().material.color = new Color(0, 0, 0, _fFade);
            StartCoroutine(Teleport(a_NewParent));
        }
        else
        {
            _goMainObject.transform.position = a_NewParent.transform.position;
            StartCoroutine(FadeOut());
        }
    }
    //------------------------------------------------------------
    //
    //------------------------------------------------------------
    void Start()
    {
        _gameManager = CGameManager.Get();
        _languageManager = GameObject.Find("_language_manager").GetComponent<CLanguageManager>();
        _ListVideo = _gameManager.Get_VideoList();
        _fFade = 1.0F;
        _bFaded = false;
        GetObjects();
        _goFadeSphere.SetActive(true);
        _goFadeSphere.GetComponent< Renderer > ().material.color = new Color(0, 0, 0, _fFade);
        _goScreen.SetActive(false);
        StartCoroutine(FadeOut());
    }
    //------------------------------------------------------------
    //
    //------------------------------------------------------------
    void GetObjects()
    {
        _goMainObject = GameObject.Find("Camera");
        _MainCamera = GameObject.Find("Camera").GetComponentInChildren<Camera>();
        _LoadingCircle = GameObject.Find("Camera").GetComponentInChildren<LoadingCircle>();
        _goFadeSphere = GameObject.Find("Fade").gameObject;
        _hotspotArray = GameObject.FindGameObjectsWithTag("hotspot");
        _goScreen = GameObject.Find("hotspot_3dScreen");
    }
    #endregion
    //------------------------------------------------------------
    //
    //------------------------------------------------------------
    public void ShowCanvas(bool a_Value)
    {
        _goScreen.SetActive(a_Value);

        if (!a_Value)
        {
            //if (_cnvHotspotScreen.transform.Find("container/Video").GetComponent<AudioSource>())
            //{
            //    _cnvHotspotScreen.transform.Find("container/Video").GetComponent<StreamVideo>().StopVideo();
            //}
        }

        ResetCanvas();

    }

    void ResetCanvas()
    {
        _goScreen.transform.Find("screen/label_description").gameObject.SetActive(false);
        _goScreen.transform.Find("screen/image").gameObject.SetActive(false);
        _goScreen.transform.Find("screen/video").gameObject.SetActive(false);
    }

    public void GoToSpawn(GameObject a_Spawn)
    {
        _fFade = 0.0f;
        _goScreen.SetActive(false);
        _bFaded = false;
        _goFadeSphere.SetActive(true);
        StartCoroutine(Teleport(a_Spawn));
    }

    public void SetLatestScreen(GameObject latestScreen, Transform screenParent)
    {
        _latestScreen = latestScreen;
        _latestScreenParent = screenParent;
    }
    public void HideLatestScreen()
    {
        if (_latestScreen != null)
        {
            _latestScreen.transform.parent = _latestScreenParent.transform;
            _latestScreen.SetActive(false);
        }
            
    }
    //------------------------------------------------------------
    //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    //------------------------------------------------------------
}
