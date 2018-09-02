using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class GoalBeatDisplayAdapter : MonoBehaviour 
{
    public TuringScene sceneController;
    public GoalBeat goalBeatController;
    public SoundBank soundBankController;
    public Sprite spacingIcon;
    public RectTransform mainPanel;
    public RectTransform backgroundPanel;
    public Transform prefabRestImage;
    public Transform prefabHitImage;
    public SoundBank soundBank;
    public float mainPanelHeightPerItem;
    private float initialMainPanelHeight;
    private List<SoundScore> scores;
    private List<Transform> scoreDisplays;
    private List<scoreDisplayItem> scoreDisplayItems;
    public Transform ScorePanelPrefab;
    public Canvas canvas;
    private int scoreLength;
    private int highlightPos = 0;
    private bool highlightEnabled = false;
    private float eventPanelWidth;
    private float eventCellWidth;
    private float eventPanelHeight;
    private bool showPanels;
    public float mainPanelHideOffset = -300f;
    private float mainPanelYOffset = 0; 
    public class scoreDisplayItem
    {
        public Vector2 goalPosition;
        public Transform scoreDisplay;
        public List<Transform> events = new List<Transform>();
        public SoundScore score;
    }



    // Use this for initialization
    void Start()
    {
        scores = goalBeatController.GetBeatScore();
        scoreLength = scores.Select(x => x.score.Count).OrderByDescending(x=>x).First();
        scoreDisplays = new List<Transform>();
        scoreDisplayItems = new List<scoreDisplayItem>();
        initialMainPanelHeight = mainPanel.offsetMin.y;
        int scoreIndex = 0;
        //foreach (var score in scores)
        foreach (var score in scores)
        {
            var inst = Instantiate<Transform>(ScorePanelPrefab, this.transform);
            inst.GetComponent<Canvas>().overrideSorting = true;
            var rect = inst.GetComponent<RectTransform>();
            var prefabRect = ScorePanelPrefab.GetComponent<RectTransform>();
            scoreDisplays.Add(inst);
            scoreDisplayItems.Add(new scoreDisplayItem { goalPosition = new Vector2(prefabRect.anchoredPosition.x, prefabRect.anchoredPosition.y), scoreDisplay = inst, score = score });

            var eventPanel = inst.GetComponent<Transform>().Find("eventPanel");
            for(int ev = 0; ev < scoreLength; ev++)
            {
                Transform itemToCreate;
                if (score.score[ev % score.score.Count])
                    itemToCreate = prefabHitImage;
                else
                    itemToCreate = prefabRestImage;

                var _event = Instantiate<Transform>(itemToCreate, eventPanel);
                var _eventRect = _event.GetComponent<RectTransform>();

                eventPanelWidth = eventPanel.GetComponent<RectTransform>().rect.width * canvas.scaleFactor;
                eventPanelHeight = eventPanel.GetComponent<RectTransform>().rect.height * canvas.scaleFactor;
                 
                _eventRect.anchoredPosition = new Vector2(calculateEventXPosition(ev, scoreLength, eventPanelWidth, 0), _eventRect.anchoredPosition.y);

                scoreDisplayItems[scoreDisplayItems.Count - 1].events.Add(_event);
            }

            var iconTransform = inst.GetComponent<Transform>().Find("Icon");
            var iconImageComponent = iconTransform.GetComponent<Image>();
            var soundBankImage = soundBank.GetSoundBankItem(score.name).Icon;
            iconImageComponent.sprite = soundBankImage;

            //inst.parent = mainPanel.transform;
            scoreIndex += 1;
        }

    }

    // Update is called once per frame
    void Update()
    {
        var prefabRect = ScorePanelPrefab.GetComponent<RectTransform>();
        //positioning scores
        for (var i = 0; i < scoreDisplayItems.Count; i++)
        {
            var inst = scoreDisplayItems[i];
            var rect = inst.scoreDisplay.GetComponent<RectTransform>();

            inst.goalPosition = new Vector2(inst.goalPosition.x, calculateAnchoredYPosition(i));

            rect.anchoredPosition = inst.goalPosition;

            //update dots in case of window resize
            var eventPanel = inst.scoreDisplay.GetComponent<Transform>().Find("eventPanel");
            for (int ev = 0; ev < inst.events.Count(); ev++)
            {
                var _event = inst.events[ev];
                var _eventRect = _event.GetComponent<RectTransform>();
                eventPanelWidth = eventPanel.GetComponent<RectTransform>().rect.width * canvas.scaleFactor;
                _eventRect.anchoredPosition = new Vector2(calculateEventXPosition(ev, scoreLength, eventPanelWidth, 0), _eventRect.anchoredPosition.y);
            }
        }

        if (showPanels)
            mainPanelYOffset = mainPanelYOffset / 2f;
        else
            mainPanelYOffset += mainPanelHideOffset - mainPanelYOffset / 2f;
        //mainPanelYOffset = 0f;

        backgroundPanel.anchoredPosition = new Vector2(mainPanel.anchoredPosition.x, calculateAnchoredYPosition((scoreDisplayItems.Count-1)/2));
        //backgroundPanel.offsetMax = new Vector2(backgroundPanel.offsetMax.x, 0f - mainPanelYOffset);
        mainPanel.offsetMax = new Vector2(mainPanel.offsetMax.x, 0f - mainPanelYOffset);
        mainPanel.offsetMin = new Vector2(mainPanel.offsetMin.x, Mathf.Min(-(48), -1*((mainPanelYOffset+48)+((scores.Count-2)*72))));
    }

    public void ToggleShow(bool show)
    {
        showPanels = show;
    }

    public void SetHighlight(int? index)
    {
        if (index == null)
            highlightEnabled = false;
        else
            highlightEnabled = true;

        if (index!=null)
            highlightPos = (int)index;

        var highlightxpos = calculateEventXPosition(highlightPos, scoreLength, eventPanelWidth,0);
        var highlightwidth = calculateEventCellWidth(scoreLength, eventPanelWidth, 0);

        foreach(var scoreItem in scoreDisplayItems)
        {
            var highlight = scoreItem.scoreDisplay.Find("eventPanel").Find("highlight");
            var highlightrect = highlight.GetComponent<RectTransform>();
            highlightrect.sizeDelta = new Vector2( highlightwidth / canvas.scaleFactor, eventPanelHeight);
            highlightrect.anchoredPosition = new Vector2(highlightxpos, highlightrect.anchoredPosition.y);
            highlight.gameObject.SetActive(highlightEnabled);
        }
    }

    private float calculateAnchoredYPosition(float item)
    {
        var prefabRect = ScorePanelPrefab.GetComponent<RectTransform>();
        return (((item + 1f) * 2f) - 1f) * prefabRect.anchoredPosition.y;
    }

    private float calculateEventXPosition(float item, float total, float width, float padding)
    {
        var cellWidth =calculateEventCellWidth(total, width, padding);
        var cellPos = ((item / total) * (width-padding)) - ((width-padding)/2f);
        var pos = cellPos + cellWidth / 2f;
        return pos;
    }

    private float calculateEventCellWidth(float total, float width, float padding)
    {
        return ((1 / total) * (width - padding));
    }
}
