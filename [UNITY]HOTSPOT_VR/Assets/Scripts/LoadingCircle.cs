using UnityEngine;
using System.Collections;

public class LoadingCircle : MonoBehaviour
{
	private CGameManager _gameManager;

	private bool _bIsInitialized = false;
	[SerializeField] private bool _bIsEnabled = true;

	[SerializeField] private MeshRenderer _mrCursorPointer;
	private Vector2 _v2CursorCounterTextureOffset;
	private float _fCursorPointerTime = 0.0F;

	private OnClickListener _videoPlayer;

	///-----------------------------------------------------------------------------------
	/// 
	///-----------------------------------------------------------------------------------
	void Awake()
	{
		if(!CGameManager.IsStartSceneInit())
			return;

		if (!_bIsInitialized) 
		{
			Initialize ();
		}

		if(_videoPlayer == null)
		{
			_videoPlayer = GameObject.Find ("_videoPlayer").GetComponent< CVideoPlayer> () as OnClickListener;
		}
	}

	///-----------------------------------------------------------------------------------
	/// 
	///-----------------------------------------------------------------------------------
	
	//private int _click = -1;
	
	private bool _bClick = false;
	
	public bool HasClicked()
	{
		return _bClick;
	//	return _click == Time.frameCount;
	}
	
	void Update( )
	{
		_bClick = false;
		if( _bIsEnabled )
		{
			_v2CursorCounterTextureOffset.x = -0.45F*_fCursorPointerTime;

			if( _fCursorPointerTime >= 2.5F)
			{
				_bClick = true;
				_fCursorPointerTime = 0.0F;
					
				if(_videoPlayer != null  )
				{
					_videoPlayer.OnClick();
					ResetCursor( );
				}
			} 
			_mrCursorPointer.GetComponent<Renderer>().sharedMaterial.mainTextureOffset = _v2CursorCounterTextureOffset;
		}
	}

    public float GetTimer()
    {
        return _fCursorPointerTime;
    }

	public void IncreaseTimer(bool a_bIsKeyboard = false)
	{
		if( _bIsEnabled )
		{
			_fCursorPointerTime = Mathf.Clamp( _fCursorPointerTime +  ( a_bIsKeyboard ? 2.0F : 1.0F )*Time.deltaTime, 0, 3.0F );
		}
	}
	
	public void ResetCursor()
	{
		if (!_bIsInitialized) 
		{
			Initialize ();
		}

		if( _bIsEnabled )
		{
			_fCursorPointerTime = 0.0F;
			_v2CursorCounterTextureOffset.x = 0;
			_mrCursorPointer.GetComponent<Renderer>().sharedMaterial.mainTextureOffset = _v2CursorCounterTextureOffset;
		}
	}

	public bool IsEnabled()
	{
		return _bIsEnabled;
	}

	private void Initialize()
	{
		_gameManager = GameObject.Find ("_game_manager").GetComponent< CGameManager > ();

		_v2CursorCounterTextureOffset = _mrCursorPointer.GetComponent<Renderer>().sharedMaterial.mainTextureOffset;
		_gameManager.SetLoadingCircle(this);
		_bIsInitialized = true;

		ResetCursor ();
	}
}
