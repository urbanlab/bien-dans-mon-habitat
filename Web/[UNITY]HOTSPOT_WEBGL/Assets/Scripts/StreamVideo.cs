using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StreamVideo : MonoBehaviour {

    public RawImage image;
    public VideoClip videoToPlay;
    public string _strURL;
    private VideoPlayer videoPlayer;
    private VideoSource videoSource;
    private AudioSource audioSource;
    private int tries = 0;
    private CMainManager _mainManager;
    private Button _btnRePlay;
    private Image _imgOverLayer;

    void Awake()
    {
        _btnRePlay = GameObject.Find("button_re-play").GetComponent<Button>();
        _imgOverLayer = GameObject.Find("overlay_video").GetComponent<Image>();
    }

    // Use this for initialization
    public void PlayVideo()
    {
        GameObject.Find("label_load").GetComponent<Text>().text = "Chargement de la vidéo...";
        tries = 0;
        Application.runInBackground = true;
        StartCoroutine(playVideo());
    }

    public void ReTryVideo()
    {
        GameObject.Find("label_load").GetComponent<Text>().text = "Chargement de la vidéo...";
        StartCoroutine(playVideo());
    }

    public void StopVideo()
    {
        GameObject.Find("label_load").GetComponent<Text>().text = "Chargement de la vidéo...";
        videoPlayer.Stop();
        Destroy(audioSource);
        Destroy(videoPlayer);
    }

    public void RePlayVideo()
    {
        StopVideo();
        _btnRePlay.gameObject.SetActive(false);
        _imgOverLayer.gameObject.SetActive(false);
        PlayVideo();
    }

    IEnumerator playVideo()
    {

        //Add VideoPlayer to the GameObject
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        //Add AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        //Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        audioSource.Pause();

        //We want to play from video clip not from url
        //videoPlayer.source = VideoSource.VideoClip;
        //videoPlayer.clip = videoToPlay;

        // Video clip from Url
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = _strURL;

        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        videoPlayer.Prepare();

        //Wait until video is prepared
        WaitForSeconds waitTime = new WaitForSeconds(1);
        while (!videoPlayer.isPrepared)
        {
            //Debug.Log("Preparing Video");
            //Prepare/Wait for 5 sceonds only
            yield return waitTime;
            //Break out of the while loop after 5 seconds wait
            break;
        }

        Debug.Log("Done Preparing Video");

        //Assign the Texture from Video to RawImage to be displayed
        image.texture = videoPlayer.texture;

        //Play Video
        videoPlayer.Play();

        //Play Sound
        audioSource.Play();

        GameObject.Find("label_load").GetComponent<Text>().text = "";
        Debug.Log("Playing Video");
        while (videoPlayer.isPlaying)
        {
           // Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
            yield return null;
        }

        Debug.Log("Done Playing Video");

        if(videoPlayer.time == 0)
        {
            tries++;

            if(tries < 6)
            {
                Debug.Log("Re-Try " + tries);
                StopVideo();
                ReTryVideo();
            }
            else
            {
                GameObject.Find("label_load").GetComponent<Text>().text = "Impossible de charger la vidéo";
            }
        }
        else
        {
            _btnRePlay.gameObject.SetActive(true);
            _imgOverLayer.gameObject.SetActive(true);
            // _mainManager = CMainManager.Get();
            // _mainManager.ShowCanvas(false);
        }
    }
}