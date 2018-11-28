using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class CGameManager : MonoBehaviour
{
    private CLanguageManager _languageManager;
    private string _strLocalPath;

    private int _iSelectedVideo = 0;
    public int noVideos = 1;
    private List<CVideo> _ListVideo;

    private CAudioManager _audioManager;
    private GameObject _goCrosshair;
	private bool _bShowCrosshair = true;

	private bool _bIsInitialized = false;

	public bool CardboardMode = false;

	public void ToggleCardboardMode()
	{
		CardboardMode = !CardboardMode;
		//GvrViewer.Instance.VRModeEnabled = CardboardMode;
		//GvrViewer.Instance.Recenter();
		//_loadingCircle.enabled = CardboardMode;
		//_goCrosshair.GetComponent<MeshRenderer>().enabled = CardboardMode && _bShowCrosshair;
	}

	public void ToggleCrosshair(bool a_bValue )
	{
		_bShowCrosshair = a_bValue;
		_goCrosshair.GetComponent<MeshRenderer>().enabled = CardboardMode && _bShowCrosshair;
	}

    ///-----------------------------------------------------------------------------------
    /// 																			
    ///-----------------------------------------------------------------------------------
    private LoadingCircle _loadingCircle;

    public void SetLoadingCircle(LoadingCircle a_loadingCircle)
    {
        _loadingCircle = a_loadingCircle;
    }

    ///-----------------------------------------------------------------------------------
    /// 																			Statics
    ///-----------------------------------------------------------------------------------
    public static bool IsStartSceneInit()
    {
        return GameObject.Find("_game_manager") != null;
    }

    public static CGameManager Get()
    {
        return GameObject.Find("_game_manager").GetComponent<CGameManager>();
    }

    public static void LoadLevel(string a_strLevel)
    {

#if UNITY_5_2
		            Application.LoadLevel(a_strLevel);
#else
        SceneManager.LoadScene(a_strLevel);
#endif
    }

    /////GET SETS////////////////
    #region GetSet
    public List<CVideo> Get_VideoList()
    {
        return _ListVideo;
    }

    public int Get_SelectedVideo()
    {
        return _iSelectedVideo;
    }

    public void Set_SelectedVideo(int a_iSelectedVideo)
    {
        _iSelectedVideo = a_iSelectedVideo;
    }
    #endregion

    ///-----------------------------------------------------------------------------------
    /// 																			INIT
    ///-----------------------------------------------------------------------------------
    public void Initialize()
    {
		_goCrosshair = GameObject.Find("Camera/TrackingSpace/CenterEyeAnchor/crosshair");
        if (_goCrosshair == null)
        {
            _goCrosshair = GameObject.Find("Camera/TrackingSpace/Main/crosshair");
		}

		CardboardMode = true;
		ToggleCardboardMode ();

		_bIsInitialized = true;
    }

    public bool HasLoadingCircleClicked()
    {
        return _loadingCircle.HasClicked();
    }

    public void IncreaseTimer()
    {
        _loadingCircle.IncreaseTimer();
    }

    public void ResetCursor()
    {
        _loadingCircle.ResetCursor();
    }

    ///-----------------------------------------------------------------------------------
    /// 
    ///-----------------------------------------------------------------------------------
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    ///-----------------------------------------------------------------------------------
    /// 
    ///-----------------------------------------------------------------------------------
    void Start()
    {
        _audioManager = CAudioManager.Get();
        _languageManager = GameObject.Find("_language_manager").GetComponent<CLanguageManager>();
        _languageManager.SetLanguage("en"); //Set English as Default
        Time.timeScale = 1;

        LoadLevel("Apartement");
    }

    public bool IsInitialized()
    {
        return _bIsInitialized;
    }
    void InitVideos()
    {
        _ListVideo = new List<CVideo>();
        CVideo _video;
        for (int i = 0; i < noVideos; i++)
        {
            _video = new CVideo(i, "video_" + i.ToString(), "");
            _ListVideo.Add(_video);
        }

    }
    ///-----------------------------------------------------------------------------------
    /// 
    ///-----------------------------------------------------------------------------------
    public void FindVideoInLocal(int videoID)
    {
        //string _strFileName = _ListVideo[videoID].GetName() + ".m4v";
        string _strFileName = "video_" + videoID + ".mp4";
        string strPersistentDataPath = System.IO.Path.Combine(Application.persistentDataPath, _strFileName);
        string strStreammingAssetsPath = System.IO.Path.Combine(Application.streamingAssetsPath, _strFileName);
        string strExternalStoragePath = "/mnt/extSdCard/IHVideo/" + _strFileName;  //For Samsung Note 4
        string strExternalStoragePath2 = "/sdcard/IHVideo/" + _strFileName;

        if (System.IO.File.Exists(strExternalStoragePath))
        {
            _strLocalPath = strExternalStoragePath;
        }
        else if (System.IO.File.Exists(strExternalStoragePath2))
        {
            _strLocalPath = strExternalStoragePath2;
        }
        else if (System.IO.File.Exists(strStreammingAssetsPath))
        {
            _strLocalPath = _strFileName;
        }
        else if (System.IO.File.Exists(strPersistentDataPath))
        {
            _strLocalPath = strPersistentDataPath;
        }

        // Samsung Galaxy S7 special case
        if (System.IO.Directory.Exists("/storage"))
        {
            foreach (string strSubFolder in System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory() + "/storage"))
            {

                string strExternalFilePath = strSubFolder + "/IHVideo/" + _strFileName;

                if (System.IO.File.Exists(strExternalFilePath))
                {
                    _strLocalPath = strExternalFilePath;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(_strLocalPath))
            Debug.Log("Video Not Found");
        else
            Debug.Log(_strLocalPath);
        _ListVideo[videoID].SetPath(_strLocalPath);
    }


}
