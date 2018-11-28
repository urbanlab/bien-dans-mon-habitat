using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class CAudioManager : MonoBehaviour 
{
    [SerializeField] private AudioSource _asGUIAudioSource;
    [SerializeField] private AudioSource _asMusicSource;
    [SerializeField] private AudioSource _asVoiceSource;

	[SerializeField] private AudioClip _clickSound;
	[SerializeField] private AudioClip _hoverSound;
    


	public static CAudioManager Get()
	{
		return GameObject.Find ("_audio_manager").GetComponent<CAudioManager>();
	}
	
	///-----------------------------------------------------------------------------------
	/// 
	///-----------------------------------------------------------------------------------
	void Awake ( )
	{
		DontDestroyOnLoad( this );
	}


    public void PlayClickSound( )
	{	
		_asGUIAudioSource.PlayOneShot( _clickSound );
	}
	
	public void PlayHoverSound( )
	{
		_asGUIAudioSource.PlayOneShot( _hoverSound );
	}

    public void PlayVoice(AudioClip _clip, bool _override = true)
    {
        if (_clip != null 
            && (_override || !_asVoiceSource.isPlaying)
            && _clip != _asVoiceSource.clip)
        {
            _asVoiceSource.clip = _clip;
            _asVoiceSource.Play();
        }
    }

    public void StopVoice()
    {
        _asVoiceSource.Stop();
    }
}