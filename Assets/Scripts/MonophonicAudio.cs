using UnityEngine;
using System.Collections;

public class MonophonicAudio : MonoBehaviour {
	public static MonophonicAudio instance;
	public int monophonicTracks = 16;

	AudioSource[] audioSources;
	void Awake(){
		instance = this;
//		if(instance == null){
//			
////			DontDestroyOnLoad(gameObject);
//		} else {
//			Destroy(gameObject);
//		}
		audioSources = new AudioSource[monophonicTracks];
		CreateTracks();
	}

	void CreateTracks(){
		for(int i = 0; i < monophonicTracks; i++){
			audioSources[i] = gameObject.AddComponent<AudioSource>();
		}
	}
	public void Stop(){
		Stop(0);
	}
	public void Stop(int track){
		audioSources[track].Stop();
	}
	public void Play(AudioClip clip, int track, float pitch, float volume, float pan){
		//		DebugText.Print("playing monophonic "+clip.name);
		audioSources[track].Stop();
		audioSources[track].pitch = pitch;
		audioSources[track].volume = volume;
		audioSources[track].panStereo = pan;
		audioSources[track].clip = clip;
		audioSources[track].Play();
	}
	public void Play(AudioClip clip, int track, float pitch, float volume){
		Play(clip,track,pitch,volume,0);
	}
	public void Play(AudioClip clip, int track, float pitch){
		Play(clip,track,pitch,1,0);
	}
	public void Play(AudioClip clip, int track){
		Play(clip,track,1,1,0);
	}
	public void Play(AudioClip clip){
		Play(clip,0,1,1,0);
	}
}
