using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CustomBar : VisualElement
{
    private float _barValue;
    [UxmlAttribute, CreateProperty]
    public float barValue { get => _barValue;  set { _barValue = value; UpdateBar(); } }

    private float _minValue;
    [UxmlAttribute, CreateProperty]
    public float minValue { get => _minValue;  set { _minValue = value; UpdateBar(); } }

    private int _maxValue;
    [UxmlAttribute, CreateProperty]
    public int maxValue { get => _maxValue;  set { _maxValue = value; UpdateBar(); } }


    public bool editable = false;


    private VisualElement track;
    private VisualElement fill;

    public CustomBar()
    {
        AddToClassList("custom-bar");

        // TRACK
        track = new VisualElement { name = "track" };
        track.AddToClassList("custom-bar__track");
        track.style.position = Position.Relative;
        hierarchy.Add(track);

        // FILL
        fill = new VisualElement { name = "fill" };
        fill.AddToClassList("custom-bar__fill");
        fill.style.position = Position.Absolute;
        fill.style.color = Color.green;
        track.Add(fill);


        RegisterCallback<GeometryChangedEvent>(_ => UpdateBar());
        RegisterCallback<AttachToPanelEvent>(_ => UpdateBar());
    }

    void UpdateBar()
    {
        if (maxValue <= minValue) return;
        float percent = (float)((barValue - minValue) / (maxValue - minValue));
        float width = track.resolvedStyle.width;

        fill.style.width = width * percent;
    }
}