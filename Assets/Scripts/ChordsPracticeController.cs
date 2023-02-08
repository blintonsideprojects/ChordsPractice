using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChordsPracticeController : MonoBehaviour
{
    private int _totalPracticeTime;
    private int _minimumSwitchTime;
    private int _maximumSwitchTime;

    private bool _practiceStart = false;

    public GameObject ChordsSelection;
    public GameObject TimingSelection;
    public GameObject CurrentlyPlaying;

    public TextMeshProUGUI MinimumTime;
    public TextMeshProUGUI MaximumTime;
    public TextMeshProUGUI OverallTime;

    public Button ChordSelectionButton;
    public Button TimingSelectionButton;
    public Button StopButton;

    public List<string> CurrentChords = new List<string>();
    public List<ChordBox> ChordBoxes;

    public TextMeshProUGUI CurrentChord;
    public TextMeshProUGUI TotalTimeText;
    public TextMeshProUGUI ChordTimeText;

    public ChordAudio ChordAudio;
    public AudioSource AudioSource;

    float currentTime = 0;
    float totalTime = 0;

    int minTime;
    int maxTime;

    private void Awake()
    {
        TimingSelectionButton.onClick.AddListener(OnTimingSelected);
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Null check these
        ChordsSelection.SetActive(true);
        TimingSelection.SetActive(false);
        CurrentlyPlaying.SetActive(false);

        ChordSelectionButton.onClick.AddListener(OnChordsSelected);
        StopButton.onClick.AddListener(OnStopButtonPressed);
    }

    void SelectChord()
    {
        CurrentChord.text = CurrentChords[UnityEngine.Random.Range(0, CurrentChords.Count - 1)];
        currentTime = 0;

        totalTime = UnityEngine.Random.Range(minTime, maxTime);

        List<AudioClip> chordClips = new List<AudioClip>();

        AudioClip test = GetMainChordClip(CurrentChord.text);
        if (test != null)
        {
            chordClips.Add(test);
        }

        AudioClip newtest = GetHalfStepClip(CurrentChord.text);
        if (newtest != null)
        {
            chordClips.Add(newtest);
        }


        string qualityString = CurrentChord.text.Substring(newtest == null ? 1 : 2);

        AudioClip reallynewtest = GetChordQualityClip(qualityString);
        if (reallynewtest != null)
        {
            chordClips.Add(reallynewtest);
        }

        Debug.Log(qualityString);
        StartCoroutine(PlayAudio(chordClips));

    }
    // Update is called once per frame
    void Update()
    {
        if (_practiceStart)
        {
            if (currentTime > totalTime)
            {
                SelectChord();
            }
            else
            {
                currentTime += Time.deltaTime;
            }
        }
    }

    public void OnStopButtonPressed()
    {
        _practiceStart = false;
        currentTime = 0;
        totalTime = 0;
        CurrentChords.Clear();
        CurrentlyPlaying.SetActive(false);
        ChordsSelection.SetActive(true);
    }

    public AudioClip GetChordQualityClip(string chordString)
    {
        switch (chordString)
        {
            case "major": return ChordAudio.major;
            case "minor": return ChordAudio.minor;
            case "5": return ChordAudio._5;
            case "7": return ChordAudio._7;
            case "maj7": return ChordAudio.maj7;
            case "m7": return ChordAudio.m7;
            case "sus4": return ChordAudio.sus4;
            case "add9": return ChordAudio.add9;
            case "sus2": return ChordAudio.sus2;
            case "7sus4": return ChordAudio._7sus4;
            case "7#9": return ChordAudio._7sharp9;
            case "9": return ChordAudio._9;
        }

        return null;
    }

    public AudioClip GetMainChordClip (string chordString)
    {
        switch (chordString[0])
        {
            case 'A': return ChordAudio.A;
            case 'B': return ChordAudio.B;
            case 'C': return ChordAudio.C;
            case 'D': return ChordAudio.D;
            case 'E': return ChordAudio.E;
            case 'F': return ChordAudio.F;
            case 'G': return ChordAudio.G;
        }

        return null;
    }

    public AudioClip GetHalfStepClip (string chordString)
    {
        Debug.Log(chordString);
        if (chordString[1] == '#')
        {
            return ChordAudio.Sharp;
        }
        else if (chordString[1] == 'b')
        {
            return ChordAudio.Flat;
        }

        return null;
    }

    public void OnTimingSelected()
    {
        TimingSelection.SetActive(false);
        CurrentlyPlaying.SetActive(true);
        SelectChord();
        minTime = Convert.ToInt32(MinimumTime.text.Substring(0, MinimumTime.text.Length - 1));
        maxTime = Convert.ToInt32(MaximumTime.text.Substring(0, MaximumTime.text.Length - 1));
        _totalPracticeTime = Convert.ToInt32(OverallTime.text.Substring(0, OverallTime.text.Length - 1));
        _practiceStart = true;


    }

    private IEnumerator PlayAudio(List<AudioClip> clips)
    {
        Debug.Log(clips.Count);
        foreach (AudioClip clip in clips)
        {
            AudioSource.clip = clip;
            AudioSource.Play();
            yield return new WaitForSeconds(AudioSource.clip.length);

        }
    }

    public void OnChordsSelected()
    {
        foreach (ChordBox cb in ChordBoxes)
        {
            Toggle[] toggles = cb.GetComponentsInChildren<Toggle>();
            foreach (Toggle t in toggles)
            {
                if (t.isOn)
                {
                    Text txt = t.GetComponentInChildren<Text>();
                    CurrentChords.Add(cb.ChordName.text + txt.text);
                }
            }
        }

        Debug.Log(CurrentChords.Count);
        ChordsSelection.SetActive(false);
        TimingSelection.SetActive(true);
    }
}
