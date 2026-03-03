using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音频配置")]
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;    // 音效控制
    [SerializeField] private AudioMixerGroup _musicMixerGroup;  // 音乐控制
    [SerializeField] private int _poolSize = 10;                // 对象池大小

    [Header("音频剪辑")]
    [SerializeField] private List<AudioClip> _audioClipList; // 在Inspector中方便添加
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    // 对象池
    private Queue<AudioSource> _audioSourcePool = new Queue<AudioSource>();
    private List<AudioSource> _activeAudioSources = new List<AudioSource>();

    // 背景音乐相关
    private AudioSource _musicSource;
    private AudioSource _ambientSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        EventManager.Instance.OnShot += PlayShotSound;
        EventManager.Instance.OnExplode += PlayExplodeSound;
    }

    private void InitializeAudioManager()
    {
        // 将列表中的音频剪辑添加到字典
        foreach (var clip in _audioClipList)
        {
            if (clip != null && !_audioClips.ContainsKey(clip.name))
            {
                _audioClips.Add(clip.name, clip);
            }
        }

        // 创建对象池
        CreateAudioSourcePool();

        // 创建背景音乐专用的AudioSource
        CreateMusicSource();
    }

    private void CreateAudioSourcePool()
    {
        GameObject poolParent = new GameObject("AudioSource Pool");
        poolParent.transform.SetParent(transform);

        for (int i = 0; i < _poolSize; i++)
        {
            AudioSource source = CreateAudioSource("Pooled AudioSource", poolParent.transform);
            _audioSourcePool.Enqueue(source);
        }
    }

    private AudioSource CreateAudioSource(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);

        AudioSource source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 1f; // 默认3D音效
        source.minDistance = 5f;
        source.maxDistance = 30f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;

        return source;
    }

    private void CreateMusicSource()
    {
        // 背景音乐源
        GameObject musicGO = new GameObject("Music Source");
        musicGO.transform.SetParent(transform);
        _musicSource = musicGO.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
        _musicSource.spatialBlend = 0f; // 2D音效

        if (_musicMixerGroup != null)
            _musicSource.outputAudioMixerGroup = _musicMixerGroup;

        // 环境音源
        GameObject ambientGO = new GameObject("Ambient Source");
        ambientGO.transform.SetParent(transform);
        _ambientSource = ambientGO.AddComponent<AudioSource>();
        _ambientSource.playOnAwake = false;
        _ambientSource.loop = true;
        _ambientSource.spatialBlend = 0f; // 2D音效

        if (_musicMixerGroup != null)
            _ambientSource.outputAudioMixerGroup = _musicMixerGroup;
    }

    /// <summary>
    /// 通过名称播放音效
    /// </summary>
    public void PlaySound(string clipName)
    {
        PlaySound(clipName, Vector3.zero, 1f);
    }

    /// <summary>
    /// 通过名称在指定位置播放音效
    /// </summary>
    public void PlaySound(string clipName, Vector3 position)
    {
        PlaySound(clipName, position, 1f);
    }

    /// <summary>
    /// 通过名称播放音效并设置音量
    /// </summary>
    public void PlaySound(string clipName, Vector3 position, float volumeScale)
    {
        if (_audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            PlaySound(clip, position, volumeScale);
        }
        else
        {
            Debug.LogWarning($"Audio clip '{clipName}' not found!");
        }
    }

    /// <summary>
    /// 直接播放AudioClip
    /// </summary>
    public void PlaySound(AudioClip clip, Vector3 position)
    {
        PlaySound(clip, position, 1f);
    }

    private void PlayShotSound()
    {
        string clipName = _audioClipList[0].name; // 假设第一个音频剪辑是射击声音 TODO:后续看怎么改
        PlaySound2D(clipName, 0.8f);
    }

    private void PlayExplodeSound(Vector3 _, float a, float b)
    {
        string clipName = _audioClipList[1].name; // 假设第一个音频剪辑是射击声音 TODO:后续看怎么改
        PlaySound2D(clipName, 1.5f);
    }
    /// <summary>
    /// 直接播放AudioClip并设置音量
    /// </summary>
    public void PlaySound(AudioClip clip, Vector3 position, float volumeScale)
    {
        if (clip == null)
        {
            Debug.LogWarning("Attempted to play null AudioClip!");
            return;
        }

        // 尝试从对象池获取AudioSource
        AudioSource source = GetPooledAudioSource();

        if (source != null)
        {
            source.transform.position = position;
            source.clip = clip;
            source.volume = volumeScale;

            if (_sfxMixerGroup != null)
                source.outputAudioMixerGroup = _sfxMixerGroup;

            source.Play();

            // 启动协程，播放完后回收
            StartCoroutine(RecycleAudioSource(source, clip.length));
        }
        else
        {
            // 对象池已满，使用PlayClipAtPoint作为后备方案
            AudioSource.PlayClipAtPoint(clip, position, volumeScale);
        }
    }

    /// <summary>
    /// 播放2D音效（无空间感）
    /// </summary>
    public void PlaySound2D(string clipName, float volumeScale = 1f)
    {
        if (_audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            AudioSource source = GetPooledAudioSource();
            if (source != null)
            {
                source.transform.position = Vector3.zero;
                source.clip = clip;
                source.volume = volumeScale;
                source.spatialBlend = 0f; // 2D

                if (_sfxMixerGroup != null)
                    source.outputAudioMixerGroup = _sfxMixerGroup;

                source.Play();
                StartCoroutine(RecycleAudioSource(source, clip.length));
            }
        }
    }

    private AudioSource GetPooledAudioSource()
    {
        if (_audioSourcePool.Count > 0)
        {
            AudioSource source = _audioSourcePool.Dequeue();
            source.gameObject.SetActive(true);
            _activeAudioSources.Add(source);
            return source;
        }

        // 如果对象池空了，创建一个临时的
        AudioSource tempSource = CreateAudioSource("Temp AudioSource", transform);
        _activeAudioSources.Add(tempSource);
        return tempSource;
    }

    private System.Collections.IEnumerator RecycleAudioSource(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay + 0.1f); // 多等0.1秒确保播放完毕

        if (source != null)
        {
            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
            source.transform.SetParent(transform);

            _activeAudioSources.Remove(source);

            // 如果这个source是从对象池借的，放回池中
            if (_audioSourcePool.Count < _poolSize && !_audioSourcePool.Contains(source))
            {
                _audioSourcePool.Enqueue(source);
            }
            else
            {
                // 临时创建的，销毁
                Destroy(source.gameObject);
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayMusic(string clipName, bool fade = false, float fadeDuration = 1f)
    {
        if (_audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            if (fade)
            {
                StartCoroutine(FadeMusic(clip, fadeDuration));
            }
            else
            {
                _musicSource.clip = clip;
                _musicSource.Play();
            }
        }
    }

    private System.Collections.IEnumerator FadeMusic(AudioClip newClip, float fadeDuration)
    {
        // 淡出当前音乐
        float startVolume = _musicSource.volume;
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeDuration);
            yield return null;
        }

        _musicSource.Stop();
        _musicSource.clip = newClip;
        _musicSource.Play();

        // 淡入新音乐
        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(0, startVolume, timer / fadeDuration);
            yield return null;
        }

        _musicSource.volume = startVolume;
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopMusic(bool fade = false, float fadeDuration = 1f)
    {
        if (fade)
        {
            StartCoroutine(FadeOutMusic(fadeDuration));
        }
        else
        {
            _musicSource.Stop();
        }
    }

    private System.Collections.IEnumerator FadeOutMusic(float fadeDuration)
    {
        float startVolume = _musicSource.volume;
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeDuration);
            yield return null;
        }

        _musicSource.Stop();
        _musicSource.volume = startVolume;
    }

    /// <summary>
    /// 设置主音量
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 暂停所有音效
    /// </summary>
    public void PauseAllSounds()
    {
        foreach (var source in _activeAudioSources)
        {
            if (source != null && source.isPlaying)
                source.Pause();
        }
        _musicSource.Pause();
        _ambientSource.Pause();
    }

    /// <summary>
    /// 恢复所有音效
    /// </summary>
    public void ResumeAllSounds()
    {
        foreach (var source in _activeAudioSources)
        {
            if (source != null)
                source.UnPause();
        }
        _musicSource.UnPause();
        _ambientSource.UnPause();
    }

    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAllSounds()
    {
        foreach (var source in _activeAudioSources)
        {
            if (source != null)
            {
                source.Stop();
                StartCoroutine(RecycleAudioSource(source, 0));
            }
        }
        _musicSource.Stop();
        _ambientSource.Stop();
    }

    /// <summary>
    /// 动态添加音频剪辑
    /// </summary>
    public void AddAudioClip(string name, AudioClip clip)
    {
        if (!_audioClips.ContainsKey(name))
        {
            _audioClips.Add(name, clip);
        }
    }

    private void OnDestroy()
    {
        // 清理资源
        StopAllCoroutines();
    }
}