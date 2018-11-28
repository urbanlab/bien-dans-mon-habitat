using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CVideoPlayer : MonoBehaviour, OnClickListener
{
	// managers
	private CGameManager _gameManager;
	private CAudioManager _audioManager;
    private CMainManager _main;

	private CButton _selectedButton;
    private GameObject[] _hotspotArray;
    private Camera _cMainCamera;

	//private Texture2D _t2dPlay;
	//private Texture2D _t2dPause;

	private Texture2D _t2dCardboardOn;
	private Texture2D _t2dCardboardOff;

	private bool _bAllowUI = true;
	
	private bool _bIsEndOfVideo = false;
	private bool _bIsLoaded = false;

	private LoadingCircle loadingCircle = null;
	private bool _bIsCardboard = true;

	private int _iProximityMask = 1 << 9;
	private int _i3DButtonMask = 1 << 10;

	private ButtonType buttonType;
	public ButtonType GetButtonType( )
	{
		return buttonType;
	}

	private Vector3 _v3PlayPausePosition;
	private Vector3 _v3PlayPauseLocalPosition;
	private Quaternion _qPlayPauseRotation;
	private Quaternion _qPlayPauseLocalRotation;
	private CButton _CardboardModeButton;
	private Vector3 _v3CardboardPosition;
	private Vector3 _v3CardboardLocalPosition;
	private Quaternion _qCardboardRotation;
	private Quaternion _qCardboardLocalRotation;

	// video
	private VIDEO_STATUS _videoStatus;
    
	[SerializeField] private MediaPlayerCtrl _360VideoTexture;
    [SerializeField] private GameObject replayButton;

	private Vector3 _v360ProjectionMeshScale, _v360ProjectionMeshScaleInverted;

	public void UpdateNavigationControllers()
	{

		if (_bIsCardboard) 
		{
			_CardboardModeButton.SetIcon (_t2dCardboardOn);
		} 
		else 
		{
			_CardboardModeButton.SetIcon (_t2dCardboardOff);
		}
	}
    //------------------------------------------------------------
    //
    //------------------------------------------------------------
    public static CVideoPlayer Get()
    {
        return GameObject.Find("_videoPlayer").GetComponent<CVideoPlayer>();
    }
    //-------------------------------------------------------------------------	
    //
    //-------------------------------------------------------------------------	
    void Awake () 
	{
		if(!CGameManager.IsStartSceneInit())
			return;

		loadingCircle = GameObject.FindObjectOfType<LoadingCircle>();

		_CardboardModeButton = transform.Find ("cardboard_mode").GetComponent<CButton> ();
		_v3CardboardPosition = _CardboardModeButton.transform.position;
		_qCardboardRotation = _CardboardModeButton.transform.rotation;

		_CardboardModeButton.transform.parent = Camera.main.transform;
		_v3CardboardLocalPosition = _CardboardModeButton.transform.localPosition;
		_qCardboardLocalRotation = _CardboardModeButton.transform.localRotation;
		_CardboardModeButton.transform.parent = transform;

		_t2dCardboardOff = Resources.Load ("Textures/cardboard_off") as Texture2D;
		_t2dCardboardOn = Resources.Load ("Textures/cardboard_on") as Texture2D;

		// managers initialization
		_gameManager = CGameManager.Get ();
		_audioManager = CAudioManager.Get ();

		_360VideoTexture.OnEnd += OnVideoFinish;
		_360VideoTexture.OnReady += OnVideoReady;
		
		//status
		_videoStatus = VIDEO_STATUS.STOPPED;	
	}
		
	void OnVideoReady( )
	{
		_videoStatus = VIDEO_STATUS.PLAYING;
		_bIsLoaded = true;

		//UpdateNavigationControllers ();
    }
	
	void OnVideoFinish( )
	{
        if ( _bIsLoaded )
		{
            //_bIsEndOfVideo = true;
            StopMovie();
            replayButton.SetActive(true);
            //_main.ShowCanvas(false);
            loadingCircle.ResetCursor();
            _bClicked = false;
        }
    }


    //-------------------------------------------------------------------------	
    //
    //-------------------------------------------------------------------------	
    private bool _bClicked = false;

    public void OnClick()
	{
        _bClicked = true;

        if ( _selectedButton != null && _bAllowUI )
		{
			switch (_selectedButton.GetButtonType()) 
			{
			case ButtonType.ToggleCardboard:
				_audioManager.PlayClickSound ();
				_gameManager.ToggleCardboardMode ();
				break;
			case ButtonType.PlayPause:
				_audioManager.PlayClickSound ();
				PlayOrPauseMovie ();
				break;
			default:
				break;
			}
			loadingCircle.ResetCursor ();
			if (_selectedButton != null) 
			{
				_selectedButton.MarkAsPointed (false);
				_selectedButton = null;
			}
			StartCoroutine (BlockButtons ());
		}
	}

	IEnumerator BlockButtons()
	{
		_bAllowUI = false;
		yield return new WaitForSeconds (0.2f);
		_bAllowUI = true;
        _bClicked = false;

    }

	void Start()
	{
        _main = CMainManager.Get();
        //_v360ProjectionMeshScale = _360VideoTexture.gameObject.transform.localScale;
		//_v360ProjectionMeshScaleInverted = new Vector3(_v360ProjectionMeshScale.x, -_v360ProjectionMeshScale.y, _v360ProjectionMeshScale.z );
        _hotspotArray = GameObject.FindGameObjectsWithTag("hotspot");
        //LaunchVideo ();
	}
		
	//-------------------------------------------------------------------------	
	//
	//-------------------------------------------------------------------------	
	void Update( )
	{
		if( _cMainCamera == null )
		{
			_cMainCamera = Camera.main;
			if(_cMainCamera == null)
			{
				_cMainCamera = GameObject.FindObjectOfType<Camera>();
				// for full screen mode
				_cMainCamera.fieldOfView = 60;
				// dont affect stereo fov
				//GvrViewer.Controller.matchMonoFOV = 0;
				//GvrViewer.Controller.UpdateStereoValues ();
			}
		}

		if (_gameManager != null) 
		{
			if (!_gameManager.IsInitialized ()) 
			{
				_gameManager.Initialize ();
			}
		}

        if ( Input.GetButtonDown ("Fire1"))
		{
			OnClick( );
		}

		if( _gameManager != null)
		if (_bIsCardboard != _gameManager.CardboardMode) 
		{
			if (_gameManager.CardboardMode) 
			{
				_CardboardModeButton.transform.parent = transform;
				_CardboardModeButton.transform.position = _v3CardboardPosition + Vector3.right * 20 + Vector3.down * 60;
				_CardboardModeButton.transform.rotation = _qCardboardRotation;

				_CardboardModeButton.LookAtCamera (true);

                    _CardboardModeButton.gameObject.SetActive(false);
			} 
			else 
			{
                    _CardboardModeButton.gameObject.SetActive(true);

				_CardboardModeButton.LookAtCamera (false);

				_CardboardModeButton.transform.parent = Camera.main.transform;
				_CardboardModeButton.transform.localPosition = _v3CardboardLocalPosition;
				_CardboardModeButton.transform.localRotation = _qCardboardLocalRotation;
			}

			_bIsCardboard = _gameManager.CardboardMode;
			UpdateNavigationControllers ();
		}

		if (_bIsCardboard) 
		{
			ManageCameraRayCast ();
		} 
		else 
		{
            ManageCameraRayCast();
            ManageTouchRayCast ();
			AutoRotateIcons ();
		}

		// exit cardboard mode when pressing physical back button
		if (Input.GetKey (KeyCode.Escape) && _bIsCardboard) 
		{
			_audioManager.PlayClickSound ();
			_gameManager.ToggleCardboardMode ();
		}
	}

	private void AutoRotateIcons()
	{
		Vector3 v3Rotation2 = _CardboardModeButton.transform.GetChild(0).localRotation.eulerAngles;
		v3Rotation2 = new Vector3 (v3Rotation2.x, v3Rotation2.y, _main.getMainCamera().transform.rotation.eulerAngles.z);
		_CardboardModeButton.transform.GetChild(0).localRotation = Quaternion.Euler (v3Rotation2);
	}

	private void ManageTouchRayCast()
	{
		#if UNITY_EDITOR
		Ray oRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		#else
		Ray oRay = new Ray();
		if( Input.touchCount == 1 )
		{
		oRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		}
		#endif
		RaycastHit oHit;

		if( Physics.Raycast ( oRay, out oHit, Mathf.Infinity, _i3DButtonMask) && _selectedButton == null && _bAllowUI)
		{
			if( oHit.collider.gameObject.GetComponent< CButton >( ))
			{
				_selectedButton = oHit.collider.gameObject.GetComponent< CButton > ();
				buttonType = _selectedButton.GetButtonType ();
				#if !UNITY_EDITOR
				OnClick ();
				#endif
			}
			else
			{
				if( _selectedButton != null )
				{
					loadingCircle.ResetCursor();
					_selectedButton.MarkAsPointed( false );
					_selectedButton = null;
					buttonType = ButtonType.Nothing;
				}
			}
		}
		else
		{
			if( _selectedButton != null )
			{
				loadingCircle.ResetCursor();
				_selectedButton.MarkAsPointed( false );
				_selectedButton = null;
				buttonType = ButtonType.Nothing;
			}
		}
	}

	private void ManageCameraRayCast()
	{
		Ray oRay = new Ray( _cMainCamera.GetComponent<Camera>( ).transform.position, _cMainCamera.GetComponent<Camera>().transform.forward );
		RaycastHit oHit;

		if( Physics.Raycast( oRay.origin, oRay.direction, out oHit, Mathf.Infinity, _i3DButtonMask ) && _bAllowUI && _main.IsFaded())
		{
			if( oHit.collider.gameObject.GetComponent< CButton >( ))
			{
                ResetButtons();
                if (loadingCircle != null)
				{
					loadingCircle.IncreaseTimer ();
				}
				if (_selectedButton == null) 
				{
					_selectedButton = oHit.collider.gameObject.GetComponent< CButton >( );
					buttonType = _selectedButton.GetButtonType ();
					_selectedButton.MarkAsPointed (true);
				}
			}
            else if (oHit.collider.gameObject.GetComponent<CHotspot>() && _bAllowUI)
            {
                if (loadingCircle != null)
                {
                    loadingCircle.IncreaseTimer();
                    oHit.collider.gameObject.GetComponent<CHotspot>().OnHover();
                }
                if (_bClicked && _bAllowUI)
                {
                    oHit.collider.gameObject.GetComponent<CHotspot>().OnClick();
                    loadingCircle.ResetCursor();
                    _bClicked = false;
                    ResetButtons();
                }
            }
            else if (oHit.collider.gameObject.tag == "close" && _bAllowUI)
            {
                if (_bClicked && _bAllowUI)
                {
                    //_hotspotArray[0].GetComponent<CHotspot>().StopMovie();
                    StopMovie();
                    replayButton.SetActive(false);
                    _main.ShowCanvas(false);
                    loadingCircle.ResetCursor();
                    _bClicked = false;
                }
                else
                {
                    loadingCircle.IncreaseTimer();
                }
            }
            else if (oHit.collider.gameObject.tag == "closeHotspot" && _bAllowUI)
            {
                if (_bClicked && _bAllowUI)
                {
                    oHit.collider.transform.parent.parent.gameObject.SetActive(false);
                    CAudioManager.Get().StopVoice();
                    loadingCircle.ResetCursor();
                    _bClicked = false;
                }
                else
                {
                    loadingCircle.IncreaseTimer();
                }
            }
            else if (oHit.collider.gameObject.tag == "replay" && _bAllowUI)
            {
                if (_bClicked && _bAllowUI)
                {
                    //oHit.collider.transform.parent.parent.gameObject.SetActive(false);
                    _360VideoTexture.Play();
                    replayButton.SetActive(false);
                    loadingCircle.ResetCursor();
                    _bClicked = false;
                }
                else
                {
                    loadingCircle.IncreaseTimer();
                }
            }
            else
			{
                ResetButtons();
                loadingCircle.ResetCursor();
                if ( _selectedButton != null )
				{
					loadingCircle.ResetCursor();
					_selectedButton.MarkAsPointed( false );
					_selectedButton = null;
					buttonType = ButtonType.Nothing;
				}
			}
		}
		else
		{
            ResetButtons();
            loadingCircle.ResetCursor();
            if ( _selectedButton != null )
			{
				loadingCircle.ResetCursor();
				_selectedButton.MarkAsPointed( false );
				_selectedButton = null;
				buttonType = ButtonType.Nothing;
			}
		}

		//if( Physics.Raycast( oRay.origin, oRay.direction, out oHit, Mathf.Infinity, _iProximityMask ) && _bAllowUI)
		//{
		//	_gameManager.ToggleCrosshair (true);
		//}
		//else
		//{
		//	_gameManager.ToggleCrosshair (false);
		//}
	}


	///-----------------------------------------------------------------------------------
	/// <summary> plays or pauses the movie </summary>
	///-----------------------------------------------------------------------------------
	public void PlayOrPauseMovie( )
	{
		//++++++++++++++++++++
		// play from start
		//++++++++++++++++++++
		if( _videoStatus == VIDEO_STATUS.STOPPED )
		{
			_360VideoTexture.Play( );

			_videoStatus = VIDEO_STATUS.PLAYING;
		}
		
		//++++++++++++++++++
		// pause
		//++++++++++++++++++
		else if( _videoStatus == VIDEO_STATUS.PLAYING )
		{
			_360VideoTexture.Pause( );
			_videoStatus = VIDEO_STATUS.PAUSED;
		}
		
		//+++++++++++++++++
		// unpause
		//+++++++++++++++++
		else if( _videoStatus == VIDEO_STATUS.PAUSED )
		{
			_360VideoTexture.Play( );
			_videoStatus = VIDEO_STATUS.PLAYING;
		}

		UpdateNavigationControllers ();
	}

	/*private int GetVideoDuration( )
	{
		return (int)_360VideoTexture.duration;
	}
	
	private float GetPlayTime( )
	{
		return (float)_360VideoTexture.;
	}

	private void SetPlayTime( float a_fValue )
	{
		_360VideoTexture.SeekTo( Mathf.RoundToInt( a_fValue ));

	}*/
	
	///-----------------------------------------------------------------------------------
	/// <summary> stop the movie and destroys it </summary>
	///-----------------------------------------------------------------------------------
	private void StopMovie(  )
	{
		_videoStatus = VIDEO_STATUS.STOPPED;

		_360VideoTexture.UnLoad ();
		_bIsLoaded = false;

        if ( _bIsEndOfVideo )
		{
            //Application.Quit();
            //_360VideoTexture.transform.parent.gameObject.SetActive(false);
            
		}

    }



	public void LaunchVideo(string a_Video, GameObject myObject)
	{
        myObject.GetComponent<TextMesh>().text = a_Video;

        _videoStatus = VIDEO_STATUS.STOPPED;
		
		_bIsEndOfVideo = false;
		_bIsLoaded = false;

		_360VideoTexture.UnLoad( );
        myObject.GetComponent<TextMesh>().text = "Unload";

        _360VideoTexture.Load (a_Video);
        myObject.GetComponent<TextMesh>().text = "Load";

        _360VideoTexture.GetComponent< Renderer > ().enabled = true;

		//#if UNITY_IPHONE && !UNITY_EDITOR
		//_360VideoTexture.gameObject.transform.localScale = _v360ProjectionMeshScaleInverted;
		//#else
		//_360VideoTexture.gameObject.transform.localScale =_v360ProjectionMeshScale;
		//#endif
		_360VideoTexture.m_objResize = null;
		_360VideoTexture.m_bLoop = false;
		_360VideoTexture.m_bAutoPlay = true;
        myObject.GetComponent<TextMesh>().text = "";
    }


    void ResetButtons()
    {
        for (int i = 0; i < _hotspotArray.Length; i++)
        {
            _hotspotArray[i].gameObject.GetComponent<CHotspot>().Reset();
        }
    }
}

public enum VIDEO_STATUS
{
	STOPPED = 0,
	PAUSED,
	PLAYING,
} 
