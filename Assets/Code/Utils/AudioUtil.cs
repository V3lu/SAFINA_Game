using UnityEngine;

namespace Assets.Code.Utils
{
    public static class AudioUtil
    {
        /// <summary>
        /// Plays an SFX at the given position with optional pitch modulation and priority.
        /// </summary>
        public static void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f, float pitchRandomness = 0f, int priority = 128)
        {
            if (clip == null) return;

            GameObject go = new GameObject("SFX_" + clip.name);
            go.transform.position = position;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.priority = priority; // Lower number = higher priority. Essential sounds like player damage should use 0.
            
            // Pitch modulation
            if (pitchRandomness > 0f)
            {
                source.pitch = 1f + Random.Range(-pitchRandomness, pitchRandomness);
            }

            source.Play();

            // Destroy the temporary GameObject after the clip finishes playing, adjusting for pitch.
            // Note: Destroy uses scaled time, so if Time.timeScale is lowered (e.g. HitStop), 
            // the object will simply live a bit longer, which is perfectly safe.
            Object.Destroy(go, (clip.length / source.pitch) + 0.1f);
        }

        /// <summary>
        /// Plays an SFX that persists across scene loads. Useful for Boss death sounds or UI.
        /// </summary>
        public static void PlayPersistentSFX(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;
            
            GameObject go = new GameObject("PersistentSFX_" + clip.name);
            go.transform.position = position;
            Object.DontDestroyOnLoad(go);
            
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.playOnAwake = false;
            source.spatialBlend = 0f; // 2D sound for boss death/UI transitions
            source.Play();
            
            Object.Destroy(go, clip.length + 0.1f);
        }
    }
}
