using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class CGameManager : MonoBehaviour {

    private CLanguageManager _languageManager;
    private string _strLocalPath;
    private Canvas _cnvLoadingScreen;
    private Image _imgProgress;
   // private Text _textDownload;
    private Image _imgDownload;

    public Color[] _colors;

    // TextScale
    private float _fDeviceScale;
#if UNITY_ANDROID
    private int _iReferenceWidth = 1365;
    private int _iReferenceHeight = 2048;
#endif

#if UNITY_WEBGL
    private int _iReferenceWidth = 1280;
    private int _iReferenceHeight = 720;
#endif

    void Awake()
    {
        DontDestroyOnLoad(this);
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
    ///-----------------------------------------------------------------------------------------------------------------
    /// 														
    ///-----------------------------------------------------------------------------------------------------------------
#region TextScale
    public void CalculateDeviceScale()
    {
        Debug.Log("Height: " + Screen.height + " Width: " + Screen.width);
        float fScreenFactor = Screen.height * Mathf.Pow(Screen.width, -1);

        if (Screen.fullScreen)
        {
            _fDeviceScale = (Screen.width * Mathf.Pow(_iReferenceWidth, -1)) * 1.5f;
        }
        else
        {
            _fDeviceScale = Screen.height * Mathf.Pow(_iReferenceWidth, -1);
        }

        Debug.Log("Device Scale: " + _fDeviceScale);
    }
    ///-----------------------------------------------------------------------------------
    ///                                                                         
    ///-----------------------------------------------------------------------------------
    public float GetDeviceScale()
    {
        return _fDeviceScale;
    }
    #endregion
    ///-----------------------------------------------------------------------------------
    /// Download Images
    ///-----------------------------------------------------------------------------------
    private bool _bIsDownloadingTextures;

    public bool IsDownloadingTextures()
    {
        return _bIsDownloadingTextures;
    }

    IEnumerator DownloadTextureCoroutine(GameObject[] a_Places, Texture2D[] a_Textures, string[] a_URLs)
    {
        _imgProgress = GameObject.Find("download_progress").GetComponent<Image>();
        _imgDownload = GameObject.Find("icon_wait").GetComponent<Image>();
        //_textDownload = GameObject.Find("label_downloadTextures").GetComponent<Text>();
        _bIsDownloadingTextures = true;

        yield return new WaitForSeconds(.1f);
        _imgDownload.color = Color.white;
		//_textDownload.text = "TÉLÉCHARGER DES TEXTURES ... 0 of 7";

        for (int i = 0; i < a_Places.Length; i++)
        {
            if (!a_Places[i].transform.Find("ProjectionMesh").GetComponent<Renderer>().material.mainTexture)
            {
                a_Textures[i] = new Texture2D(4, 4, TextureFormat.DXT1, false);
                yield return new WaitForSeconds(0.01f);
                //_textDownload.text = "TÉLÉCHARGER DES TEXTURES ... " + (i+1).ToString() +  " of " + a_Places.Length.ToString();
                using (WWW www = new WWW(a_URLs[i]))
                {
                    StartCoroutine(ShowProgress(www, _colors[i]));
                    yield return www;
                    StopCoroutine(ShowProgress(www, _colors[i]));
                    www.LoadImageIntoTexture(a_Textures[i]);
                    a_Places[i].transform.Find("ProjectionMesh").GetComponent<Renderer>().material.mainTexture = a_Textures[i];
                    Debug.Log("Downloaded and changed Texture");
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
				//_textDownload.text = "TÉLÉCHARGER DES TEXTURES ... " + (i + 1).ToString() + " of " + a_Places.Length.ToString();
                Debug.Log("<color=yellow>Texture " + i.ToString() + " There's already a texture</color>");
            }
        }

		_imgProgress.gameObject.SetActive (false);
        _bIsDownloadingTextures = false;

    }

    public void InitialDownloadSettings()
    {
        _imgProgress = GameObject.Find("download_progress").GetComponent<Image>();
        _imgDownload = GameObject.Find("icon_wait").GetComponent<Image>();
        _bIsDownloadingTextures = false;
        _imgProgress.gameObject.SetActive(false);
        _imgDownload.gameObject.SetActive(false);
    }

    IEnumerator DownloadOneTextureCoroutine(GameObject a_Places, Texture2D a_Textures, string a_URLs)
    {
        CMainManager _main = CMainManager.Get();
        ShowLoadingScreen();
        // _imgProgress = GameObject.Find("download_progress").GetComponent<Image>();
        // _imgDownload = GameObject.Find("icon_wait").GetComponent<Image>();
        //_textDownload = GameObject.Find("label_downloadTextures").GetComponent<Text>();
        _imgProgress.gameObject.SetActive(true);
        _imgDownload.gameObject.SetActive(true);
        _bIsDownloadingTextures = true;

        yield return new WaitForSeconds(.1f);
        _imgDownload.color = Color.white;
        //_textDownload.text = "TÉLÉCHARGER DES TEXTURES ... 0 of 7";
            if (!a_Places.transform.Find("ProjectionMesh").GetComponent<Renderer>().material.mainTexture)
            {
                a_Textures = new Texture2D(4, 4, TextureFormat.DXT1, false);
                yield return new WaitForSeconds(0.01f);
                //_textDownload.text = "TÉLÉCHARGER DES TEXTURES ... " + (i+1).ToString() +  " of " + a_Places.Length.ToString();
                using (WWW www = new WWW(a_URLs))
                {
                    StartCoroutine(ShowProgress(www, _colors[0]));
                    yield return www;
                    StopCoroutine(ShowProgress(www, _colors[0]));
                    www.LoadImageIntoTexture(a_Textures);
                    a_Places.transform.Find("ProjectionMesh").GetComponent<Renderer>().material.mainTexture = a_Textures;
                    Debug.Log("Downloaded and changed Texture");
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                //_textDownload.text = "TÉLÉCHARGER DES TEXTURES ... " + (i + 1).ToString() + " of " + a_Places.Length.ToString();
                Debug.Log("<color=yellow>There's already a texture</color>");
            }

        _imgProgress.gameObject.SetActive(false);
        _bIsDownloadingTextures = false;
        HideLoadingScreen();
        _main.StartTheSpawn(a_Places);
    }

    private IEnumerator ShowProgress(WWW www, Color a_Color)
    {
        //_imgProgress.color = a_Color;

            while (!www.isDone)
            { 
                    _imgProgress.fillAmount = www.progress;
                    Debug.Log(string.Format("Downloaded {0:P1}", www.progress));
                    yield return new WaitForSeconds(0.001f);
            }
        _imgProgress.fillAmount = 0;
        //Debug.Log("Done");
    }

    public void DownloadTexture(GameObject[] a_Places, Texture2D[] a_Textures, string[] a_URLs)
    {
        StartCoroutine(DownloadTextureCoroutine(a_Places, a_Textures, a_URLs));
    }

    public void DownloadOneTexture(GameObject a_Place, Texture2D a_Texture, string a_URL)
    {
        StartCoroutine(DownloadOneTextureCoroutine(a_Place, a_Texture, a_URL));
    }
    ///-----------------------------------------------------------------------------------
    /// 																			Start
    ///-----------------------------------------------------------------------------------

    void Start () {
        _languageManager = GameObject.Find("_language_manager").GetComponent<CLanguageManager>();
        _cnvLoadingScreen = transform.Find("cnv_loadingScreen").GetComponent<Canvas>();
        _languageManager.SetLanguage("en"); //Set English as Default
        CalculateDeviceScale();
        ShowLoadingScreen();
        ResizeText(ref _cnvLoadingScreen);
        LoadLevel("Main");
	}

    /////GET SETS////////////////
    #region GetSet

    #endregion

    void Update()
    {
        if(_cnvLoadingScreen.enabled)
        {
            _cnvLoadingScreen.transform.Find("background/loading_icon").GetComponent<Image>().transform.Rotate(Vector3.forward * -5.0F);
        }
        if(_imgDownload != null)
        {
            if(_imgDownload.color != new Color(0,0,0,0))
            {
                _imgDownload.transform.Rotate(Vector3.forward * -5.0F);
            }
        }
    }

    public void ShowLoadingScreen()
    {
        _cnvLoadingScreen.enabled = true;
    }

    public void HideLoadingScreen()
    {
        _cnvLoadingScreen.enabled = false;
    }
    ///-----------------------------------------------------------------------------------
    ///                                                                        
    ///-----------------------------------------------------------------------------------
    public void ResizeFullScreen(GameObject a_Container)
    {

        //a_Container.transform.Find("label_title").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_title").GetComponent<Text>().fontSize / _fDeviceScale);
        //a_Container.transform.Find("label_description").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_description").GetComponent<Text>().fontSize / _fDeviceScale);
        //a_Container.transform.Find("label_description_plus").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_description_plus").GetComponent<Text>().fontSize / _fDeviceScale);

        //CalculateDeviceScale();

        a_Container.transform.Find("label_title").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_title").GetComponent<Text>().fontSize * 1.25f);
        a_Container.transform.Find("label_description").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_description").GetComponent<Text>().fontSize * 1.5f);
        a_Container.transform.Find("label_description_plus").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_description_plus").GetComponent<Text>().fontSize * 1.34f);
    }

    public void ResizeUnFullScreen(GameObject a_Container)
    {
        //a_Container.transform.Find("label_title").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_title").GetComponent<Text>().fontSize / 1f);
        //a_Container.transform.Find("label_description").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_description").GetComponent<Text>().fontSize / 1.25f);
        //a_Container.transform.Find("label_description_plus").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_description_plus").GetComponent<Text>().fontSize / 1.2f);

        //CalculateDeviceScale();

        a_Container.transform.Find("label_title").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_title").GetComponent<Text>().fontSize * _fDeviceScale);
        a_Container.transform.Find("label_description").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_description").GetComponent<Text>().fontSize * _fDeviceScale);
        a_Container.transform.Find("label_description_plus").GetComponent<Text>().fontSize = (int)(a_Container.transform.Find("label_description_plus").GetComponent<Text>().fontSize * _fDeviceScale);
    }
    ///-----------------------------------------------------------------------------------
    ///                                                                        
    ///-----------------------------------------------------------------------------------
    public void ResizeText(ref Canvas a_canvas)
    {
        Component[] textObjects;
        textObjects = a_canvas.GetComponentsInChildren<Text>();

        foreach (Text text in textObjects)
        {
            text.fontSize = (int)((_fDeviceScale * text.fontSize));
        }
    }
    ///-----------------------------------------------------------------------------------
    ///                                                                         
    ///-----------------------------------------------------------------------------------
    public void ResizeText(ref GameObject a_object)
    {
        Component[] textObjects;
        textObjects = a_object.GetComponentsInChildren<Text>();

        foreach (Text text in textObjects)
        {
            text.fontSize = (int)((_fDeviceScale * text.fontSize) / 1.25f);
            text.fontSize = (int)((_fDeviceScale * text.fontSize) / 1.25f);
        }
    }
    //--------------------------------------------------------------------
    // WIP: Save images in  locally to avoid download
    //---------------------------------------------------------------------
    IEnumerator loadPic(WWW www, string thefile, string folder)
    {
        yield return www;
        string venue = Application.persistentDataPath + folder;
        string path = Application.persistentDataPath + folder + "/" + thefile;

        if (!System.IO.Directory.Exists(venue))
        {
            System.IO.Directory.CreateDirectory(venue);
        }

        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }

        System.IO.File.WriteAllBytes(path, www.bytes);
    }

    void SaveTextureToFile(Texture2D texture, string filename)
    {
        System.IO.File.WriteAllBytes(filename, texture.EncodeToPNG());
    }

    //LOAD TEXTURE
    Texture2D load_s01_texture;
    void LoadTextureToFile(string filename)
    {
        byte[] bytes;
        bytes = System.IO.File.ReadAllBytes(Application.dataPath + "/Save/" + filename);
        load_s01_texture = new Texture2D(1, 1);
        load_s01_texture.LoadImage(bytes);
    }

    string filename = "s01_texture.png";
    Texture2D loaded_s01_texture;
    void LoadTextureFromFile2(Texture2D texture, string filename)
    {
        System.IO.FileStream fileLoad;
        fileLoad = new FileStream(Application.dataPath + "/Save/" + filename, FileMode.Open, FileAccess.Read, FileShare.None);
        //loaded_s01_texture = fileLoad.ReadByte();
        fileLoad.Close();
    }
    //----------------------------------------------------------------------
    //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    //---------------------------------------------------------------------
}
