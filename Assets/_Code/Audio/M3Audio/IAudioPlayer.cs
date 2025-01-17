﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for classes with complex audio functionality
/// </summary>
public interface IAudioPlayer
{
	/// <summary>
	/// For longer sounds
	/// </summary>
	/// <param name="clipID"></param>
	void PlayAudio(string clipID, bool loop = false);

	/// <summary>
	/// Returns whether audio is being played
	/// </summary>
	/// <returns></returns>
	bool IsPlayingAudio();

	/// <summary>
	/// Stops the current audio
	/// </summary>
	void StopAudio();

	/// <summary>
	/// Resumes previously stopped audio
	/// </summary>
	void ResumeAudio();

	/// <summary>
	/// Saves the current audio for later
	/// </summary>
	void StashAudio();

	/// <summary>
	/// Resumes previously stashed audio
	/// </summary>
	void ResumeStashedAudio();

	/// <summary>
	/// Mutes audio source
	/// </summary>
	void MuteAudio(bool isMute);

	/// <summary>
	/// returns whether the audio player is currently muted
	/// </summary>
	/// <returns></returns>
	bool IsMute();

	/// <summary>
	/// crossfades the current audio into the given audio
	/// </summary>
	/// <param name="clipID"></param>
	void CrossFadeAudio(string clipID, float time, bool loop = false);
}
