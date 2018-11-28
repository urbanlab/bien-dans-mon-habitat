using UnityEngine;
using System.Collections;

public class CButton : MonoBehaviour
{
	private CGameManager _gameManager;
	private CAudioManager _audioManager;

	private bool _bIsButtonEnabled = true;
	private bool _bIsBeenPointed = false;
	private bool _bUseSecondaryIcon = false;

	private Renderer _rIcon;
	[SerializeField] private Texture2D _txt2Icon;

	[SerializeField] private ButtonType _buttonType;

	[SerializeField] private bool _bScaleContentOnHover = true;
	[SerializeField] private float _fHoverScale = 1.05F;
	private bool _bIsHover = false;
	private Transform _tContent; 
	private Vector3 _v3ContentBaseScale; 

	private bool _bLookAtCamera = false;

	public void LookAtCamera( bool a_bValue )
	{
		_bLookAtCamera = a_bValue;

		if (!_bLookAtCamera)
			_tContent.localRotation = Quaternion.identity;
	}

	public ButtonType GetButtonType( )
	{
		return _buttonType;
	}

	public void SetIcon( Texture2D icon )
	{
		_txt2Icon = icon;

		if( _rIcon != null )
		{
			if( _rIcon.material.mainTexture != _txt2Icon )
			{
				_rIcon.material.mainTexture = _txt2Icon;
			}
		}
	}

	public bool IsEnabled( )
	{
		return _bIsButtonEnabled;
	}
		
	void Awake( )
	{
		if(!CGameManager.IsStartSceneInit())
			return;
			
		_gameManager = CGameManager.Get ();
	}

	void Start () 
	{
		_tContent = transform.Find ("content");
		if( _tContent != null )
		{
			_v3ContentBaseScale = _tContent.localScale;
		}
		if( transform.Find ("content/icon") != null )
		{
			_rIcon = transform.Find ("content/icon").GetComponent<Renderer> ();
		}
		else
		{
			_rIcon = null;
		}
		if( _rIcon != null )
			_rIcon.material.mainTexture = _txt2Icon;
	}
		
	void Update () 
	{

		if( _bLookAtCamera && Camera.main != null )
			_tContent.LookAt( _tContent.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up );
	}

	public void MarkAsPointed(bool a_bValue)
	{
		if( a_bValue && !_bIsHover )
		{
			EnableHover ();
		}
		else if( !a_bValue && _bIsHover )
		{
			DisableHover ();
		}
	}

	private void EnableHover()
	{
		_bIsHover = true;
		_audioManager.PlayHoverSound( );

		if( _bScaleContentOnHover && _tContent != null )
		{
			_tContent.localScale = _fHoverScale * _v3ContentBaseScale;
		}
	}

	private void DisableHover()
	{
		_bIsHover = false;

		if( _rIcon != null )
		{
			if( _rIcon.material.mainTexture != _txt2Icon )
			{
				_rIcon.material.mainTexture = _txt2Icon;
			}
		}

		if( _bScaleContentOnHover && _tContent != null )
		{
			_tContent.localScale = _v3ContentBaseScale;
		}
	}
}

public enum ButtonType
{
	PlayPause,
	ToggleCardboard,
    Hotspot,
	Nothing
}
