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
    spawn
};

public class CHotspot : MonoBehaviour {

    private CMainManager _main;
    private CGameManager _gameManager;
    private GameObject _goScreen;
    private CVideoPlayer _videoPlayer;
    //Atributos
    public Enum_HotspotType _enumType;
    [TextArea]
    public string _strTitle;
    [TextArea]
    public string _strDescription;
    public Texture _textIcon;
    public VideoClip _clipVideo;
    public AudioClip _audioTitle;
    public AudioClip _audioDescription;
    public string _strURLVideo;
    public GameObject _goSpawnHotspot;
    private MediaPlayerCtrl _EasyVideoTexture;
    private bool _bHaveVideo;
    public GameObject TextScreen;

    private bool _bIsHovering = false;

    //--------------------------------------------------
    //
    //--------------------------------------------------
    void Start () {
        _gameManager = CGameManager.Get();
        _main = CMainManager.Get();
        _videoPlayer = CVideoPlayer.Get();
        transform.Find("hover").gameObject.SetActive(false);
        _bHaveVideo = false;
    }
    //--------------------------------------------------
    //
    //--------------------------------------------------
    public void OnClick()
    {
        _main.HideLatestScreen();
        _main.ShowCanvas(false);
        if (_enumType != Enum_HotspotType.text)
        {
            _main.ShowCanvas(true);
            _goScreen = _main.GetScreen();
            //_goScreen.transform.LookAt(2 * _goScreen.transform.position - _main.GetMainCamera().transform.position);
            _goScreen.transform.forward = _main.getMainCamera().transform.forward;
            _goScreen.transform.Find("screen/label_title").GetComponent<TextMesh>().text = _strTitle;
            _goScreen.transform.Find("screen/replay_button").gameObject.SetActive(false);
        }

        switch(_enumType)
        {
            case Enum_HotspotType.text:
                // _goScreen.transform.Find("screen/label_description").gameObject.SetActive(true);
                // _goScreen.transform.Find("screen/label_description").GetComponent<TextMesh>().text = _strDescription;
                TextScreen.SetActive(true);
                _main.SetLatestScreen(TextScreen, TextScreen.transform.parent);
                TextScreen.transform.Find("screen/label_description").GetComponent<TextMesh>().text = _strDescription;
                TextScreen.transform.Find("screen/label_title").GetComponent<TextMesh>().text = _strTitle;
                TextScreen.transform.forward = _main.getMainCamera().transform.forward;
                TextScreen.transform.parent = _main.getMainCamera().transform.parent.parent;
                //TextScreen.transform.localPosition = new Vector3(-6f, 0f, 0f);//-0.65f, -0.15f, 0f
                TextScreen.transform.position = _main.getMainCamera().transform.position + _main.getMainCamera().transform.forward * 30.0f;
                CAudioManager.Get().PlayVoice(_audioDescription);
                break;

            case Enum_HotspotType.image:
                _goScreen.transform.Find("screen/image").gameObject.SetActive(true);
                _goScreen.transform.Find("screen/image").GetComponent<MeshRenderer>().material.mainTexture = _textIcon;
                break;

            case Enum_HotspotType.video:
                _goScreen.transform.Find("screen/video").gameObject.SetActive(true);
                _goScreen.transform.Find("screen/label_description").gameObject.SetActive(true);
                _EasyVideoTexture = _goScreen.transform.Find("screen/video").GetComponent<MediaPlayerCtrl>();
                _videoPlayer.LaunchVideo(_strURLVideo, _goScreen.transform.Find("screen/label_description").gameObject);
                //LoadMovie();
                //_cnvHotspotScreen.transform.Find("container/Video").GetComponent<StreamVideo>()._strURL = _strURLVideo;
                //_cnvHotspotScreen.transform.Find("container/Video").GetComponent<StreamVideo>().videoToPlay = _clipVideo;
                //_cnvHotspotScreen.transform.Find("container/Video").GetComponent<StreamVideo>().PlayVideo(); 
                break;

            case Enum_HotspotType.spawn:
                _main.GoToSpawn(_goSpawnHotspot);
                break;
        }
    }
    //--------------------------------------------------
    //
    //--------------------------------------------------
    public void OnHover()
    {
        transform.Find("hover").gameObject.SetActive(true);
        CAudioManager.Get().PlayVoice(_audioTitle, false);
    }
    ////--------------------------------------------------
    ////
    ////--------------------------------------------------
    public void Reset()
    {
        transform.Find("hover").gameObject.SetActive(false);
    }
    //--------------------------------------------------
    //
    //--------------------------------------------------
    /*
    void Update()
    {
        if (_EasyVideoTexture != null)
        {
            if (_EasyVideoTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
            {
                _EasyVideoTexture.Stop();
            }
        }
    }
    #region StatusVideo
    ///-----------------------------------------------------------------------------------
    /// <summary> stop the movie and destroys it </summary>
    ///-----------------------------------------------------------------------------------

    public void PlayOrPauseMovie()
    {
        if (_EasyVideoTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
        {
            _EasyVideoTexture.Pause();
        }
        else if (_EasyVideoTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
        {
            _EasyVideoTexture.Play();
        }
        else if (_EasyVideoTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED)
        {
            _EasyVideoTexture.Play();
        }
    }

    public void StopMovie()
    {
        if(_bHaveVideo)
            _EasyVideoTexture.Stop();
        _EasyVideoTexture = null;
    }

    #endregion

    #region LoadVideo
    ///-----------------------------------------------------------------------------------
    /// <summary> loads a movie and plays it </summary>
    ///-----------------------------------------------------------------------------------
    private void LoadMovie()
    {
#if UNITY_ANDROID
        _EasyVideoTexture.Load("video_" + _gameManager.Get_SelectedVideo() + ".mp4");
        if (_EasyVideoTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY)
        {
            _bHaveVideo = true;
            _EasyVideoTexture.Play();
        }
        else
        {

            //Find Local
            _gameManager.FindVideoInLocal(_gameManager.Get_SelectedVideo());
            _EasyVideoTexture.Load("file://" + _gameManager.Get_VideoList()[_gameManager.Get_SelectedVideo()].GetPath());
            if (_EasyVideoTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY)
            {
                _bHaveVideo = true;
                _EasyVideoTexture.Play();
            }
            else
            {
                Debug.Log("Can't find the Video " + _gameManager.Get_SelectedVideo().ToString());
            }
        }
#endif
    }
    #endregion
    */
}
