using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CEditorCamera : MonoBehaviour {

    public float onKeyFactor = 10f;
    private bool mouseTurnActivated;
    private Vector3 _v3TotalRotation = new Vector3(0, 0, 0);
    private float _fOrbitX = 0;
    private float _fOrbitY = 0;
    public GameObject _camera;

    private bool m_bTurnRight = false;
    private bool m_bTurnLeft = false;
    private bool m_bTurnUp = false;
    private bool m_bTurnDown = false;
    private AudioSource _audioSource;
    private bool _bAudio;
    private Image _imgButtonAudio;

    public Sprite _sprAudioOn;
    public Sprite _sprAudioOff;


    private Vector3 totalRotation = new Vector3 (0, 0, 0);
	
	public GameObject GetGameObject()
    {
		return gameObject;
	}

    void Awake()
    {
        _audioSource = GameObject.Find("hotspot_screen").GetComponent<AudioSource>();
    }

    void Start()
    {
        _imgButtonAudio = GameObject.Find("button_audio").GetComponent<Image>();
        _bAudio = true;
        mouseTurnActivated = true;
        
        _fOrbitX = _camera.transform.localEulerAngles.y;
        _fOrbitY = _camera.transform.localEulerAngles.x;
    }

    private void TurnCameraRight(float factor)
	{
		totalRotation.y += Time.deltaTime * factor;
	}

    private void TurnCameraLeft(float factor)
	{
		totalRotation.y -= Time.deltaTime * factor;
	}

    private void TurnCameraUp(float factor)
	{
		totalRotation.x -= Time.deltaTime * factor;
	}

    private void TurnCameraDown(float factor)
	{
		totalRotation.x += Time.deltaTime * factor;
	}
    //-----------------------------------------------------------------
    //
    //-----------------------------------------------------------------
    public void SetTurnRight(bool a_Value)
    {
        m_bTurnRight = a_Value;
    }

    public void SetTurnLeft(bool a_Value)
    {
        m_bTurnLeft = a_Value;
    }

    public void SetTurnUp(bool a_Value)
    {
        m_bTurnUp = a_Value;
    }

    public void SetTurnDown(bool a_Value)
    {
        m_bTurnDown = a_Value;
    }

    public void SetAudio()
    {
        _bAudio = !_bAudio;

        if (_bAudio)
        {
            _audioSource.volume = 1.0f;
            _imgButtonAudio.sprite = _sprAudioOn;
        }
        else
        {
            _audioSource.volume = 0.0f;
            _imgButtonAudio.sprite = _sprAudioOff;
        }
    }

    //-----------------------------------------------------------------
    //
    //-----------------------------------------------------------------
    void Update () {
		
		if(Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D) || m_bTurnRight)
		{
			TurnCameraRight(onKeyFactor);
		}
		if(Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.Q) || m_bTurnLeft)
		{
			TurnCameraLeft(onKeyFactor);
		}
		if(Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.Z) || m_bTurnUp)
		{
			TurnCameraUp(onKeyFactor);
		}
		if(Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S) || m_bTurnDown)
		{
			TurnCameraDown(onKeyFactor);
		}

        MouseCameraMove();

        gameObject.transform.localEulerAngles = totalRotation;
	}

    void MouseCameraMove()
    {
        if (mouseTurnActivated && Input.GetMouseButton(0))
        {
            //Debug.Log(_fOrbitX);
            _fOrbitX += Input.GetAxis("Mouse X") * 2.5f;
            _fOrbitY -= Input.GetAxis("Mouse Y") * 2.5f;

            if (_fOrbitY < -360)
                _fOrbitY += 360;
            if (_fOrbitY > 360)
                _fOrbitY -= 360;
            _fOrbitY = Mathf.Clamp(_fOrbitY, -80, 80);

            _v3TotalRotation.x = _fOrbitY;
            _v3TotalRotation.y = _fOrbitX;
        }

        _camera.transform.localEulerAngles = new Vector3(_v3TotalRotation.x, _v3TotalRotation.y, _v3TotalRotation.z);
    }
	
	public void Set(Quaternion quat)
	{
		totalRotation = quat.eulerAngles;
	}
}
