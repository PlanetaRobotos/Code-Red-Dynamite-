using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        // // members (члены)
        // public static AudioManager Instance;

        public bool debug;

        public AudioTrack[] tracks;

        private Hashtable _mAudioTable; //relationship between audio types(key) and audio tracks(value)

        private Hashtable
            _mJobTable; //relationship between audio types(key) and jobs(value-Coroutine) // JOBS: playAudio, restartAudio ...

        // Using for know max Source volume
        private float _maxSourceVolume;

        // Using only if I need to some audioTypes can't superimpose with other audioTypes
        private AudioJob _currentAudioJob;

        [Serializable]
        public class AudioObject
        {
            public AudioType type;
            public AudioClip clip;
        }

        [Serializable]
        public class AudioTrack
        {
            public AudioSource source;
            public AudioObject[] audio;
        }

        private class AudioJob
        {
            public readonly AudioAction Action;
            public readonly AudioType Type;

            public readonly bool Fade;

            // How long we should check before change audio volume
            public readonly float FadeDelay;
            public readonly float DelayBeforeNextAudio;

            public AudioJob(AudioAction action, AudioType type, bool fade, float fadeDelay, float delayBeforeNextAudio)
            {
                Action = action;
                Type = type;
                Fade = fade;
                FadeDelay = fadeDelay;
                DelayBeforeNextAudio = delayBeforeNextAudio;
            }
        }

        private enum AudioAction
        {
            Start,
            Restart,
            Stop
        }

        #region Unity Functions

        private void Awake()
        {
            Configure();
        }

        #endregion

        #region Own Public

        public void PlayAudio(AudioType type, bool fade = false, float fadeDelay = 0, float delayBeforeNextAudio = 0) => 
            AddJob(new AudioJob(AudioAction.Start, type, fade, fadeDelay, delayBeforeNextAudio));

        public void RestartAudio(AudioType type, bool fade = false, float fadeDelay = 0,
            float delayBeforeNextAudio = 0) =>
            AddJob(new AudioJob(AudioAction.Restart, type, fade, fadeDelay, delayBeforeNextAudio));

        public void StopAudio(AudioType type, bool fade = false, float fadeDelay = 0, float delayBeforeNextAudio = 0) =>
            AddJob(new AudioJob(AudioAction.Stop, type, fade, fadeDelay, delayBeforeNextAudio));

        #endregion

        #region Private Functions

        private void Configure()
        {
            _mAudioTable = new Hashtable();
            _mJobTable = new Hashtable();
            CreateAudioTable();
        }

        private IEnumerator Dispose()
        {
            foreach (var job in from DictionaryEntry entry in _mJobTable select (IEnumerator) entry.Value)
                StopCoroutine(job);
            yield return null;
        }

        private void CreateAudioTable()
        {
            foreach (var track in tracks)
            {
                foreach (var audioObject in track.audio)
                {
                    // do not duplicate key
                    if (_mAudioTable.ContainsKey(audioObject.type))
                        LogWarning($"Oups. You are trying to register audio" +
                                   $" [{audioObject.type}] that has already been registered");
                    else
                    {
                        _mAudioTable.Add(audioObject.type, track);
                        Log($"Registering audio [{audioObject.type}]");
                    }
                }
            }
        }

        private void AddJob(AudioJob job)
        {
            // remove conflicting jobs
            RemoveConflictingJobs(job.Type);
            // starting job
            IEnumerator jobRunner = RunAudioJob(job);
            _mJobTable.Add(job.Type, jobRunner);
            StartCoroutine(jobRunner);
            Log($"Starting job on [{job.Type}] with operation [{job.Action}]");
        }

        private IEnumerator RunAudioJob(AudioJob job)
        {
            yield return new WaitForSeconds(job.FadeDelay);

            if (_currentAudioJob != null && _currentAudioJob.DelayBeforeNextAudio != 0)
                //  && job.DelayBeforeNextAudio == 0 - if i want to superimpose these clips
            {
                yield return new WaitForSeconds(_currentAudioJob.DelayBeforeNextAudio);
                _currentAudioJob = null;
                yield break;
            }

            AudioTrack track = (AudioTrack) _mAudioTable[job.Type];
            track.source.clip = GetAudioClipFromAudioTrack(job.Type, track);

            _currentAudioJob = job;

            switch (job.Action)
            {
                case AudioAction.Start:
                    track.source.Play();
                    break;
                case AudioAction.Stop:
                    if (!job.Fade)
                        track.source.Stop();
                    break;
                case AudioAction.Restart:
                    track.source.Stop();
                    track.source.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // if this sound should fading - doing it)
            // when we do Stop action Sound will smoothing decrease volume.
            // when we do Start action Sound will smoothing increase volume

            if (job.Fade)
            {
                // var initial = job.Action is AudioAction.Stop ? 1 : 0;
                // var target = initial is 0 ? 1 : 0;

                if (Math.Abs(track.source.volume) > .0001f)
                    _maxSourceVolume = track.source.volume;
                var initial = job.Action is AudioAction.Stop ? _maxSourceVolume : 0;
                var target = initial is 0 ? _maxSourceVolume : 0;

                // How long we turn off or turn on music
                const float duration = 1;
                var timer = 0f;

                while (timer <= duration)
                {
                    track.source.volume = Mathf.Lerp(initial, target, timer / duration);
                    timer += Time.deltaTime;
                    yield return null;
                }

                track.source.volume = target;

                if (job.Action is AudioAction.Stop)
                    track.source.Stop();
            }

            _mJobTable.Remove(job.Type);
            Log($"Jobs count = {_mJobTable.Count}");
            yield return null;
        }


        private void RemoveConflictingJobs(AudioType type)
        {
            if (_mJobTable.ContainsKey(type))
                RemoveJob(type);
            AudioType conflictAudio = AudioType.None;
            foreach (var audioType in _mJobTable.Cast<DictionaryEntry>()
                .Select(entry => (AudioType) entry.Key)
                .Select(audioType => new {audioType, audioTrackInUse = (AudioTrack) _mAudioTable[audioType]})
                .Select(@t => new {@t, audioTrackNeeded = (AudioTrack) _mAudioTable[type]})
                .Where(@t => @t.audioTrackNeeded.source == @t.@t.audioTrackInUse.source)
                .Select(@t => @t.@t.audioType))
            {
                // conflict
                conflictAudio = audioType;
            }

            if (conflictAudio is AudioType.None) return;
            // if we change property conflictAudio
            RemoveJob(conflictAudio);
        }

        private void RemoveJob(AudioType type)
        {
            if (!_mJobTable.ContainsKey(type))
            {
                LogWarning($"Trying to remove a job [{type}] that isn't running");
                return;
            }

            IEnumerator runningJob = (IEnumerator) _mJobTable[type];
            StopCoroutine(runningJob);
            _mJobTable.Remove(type);
        }

        /// <summary>
        /// go across all audioObjects in track.audio and return element with correct type value or else null 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="track"></param>
        /// <returns></returns>
        private static AudioClip GetAudioClipFromAudioTrack(AudioType type, AudioTrack track) =>
            (from audioObject in track.audio where audioObject.type == type select audioObject.clip).FirstOrDefault();

        private void Log(string msg)
        {
            if (!debug) return;
            Debug.Log($"AudioController: {msg}");
        }

        private void LogWarning(string msg)
        {
            if (!debug) return;
            Debug.LogWarning($"AudioController: {msg}");
        }

        #endregion
    }
}