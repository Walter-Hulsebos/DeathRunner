using System;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.Utilities;

using Object = System.Object;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using UnityEditor;
#endif  

public sealed class ProbabilityAttribute : Attribute
{
    public readonly String ColorName;
    public readonly String DataMember;

    public ProbabilityAttribute(String dataMember, String colors = "Fall")
    {
        DataMember = dataMember;
        ColorName = colors;
    }
}
#if UNITY_EDITOR


public sealed class ProbabilityAttributeDrawer : OdinAttributeDrawer<ProbabilityAttribute, Single[]>
{
    private String errorMessage;
    private ValueResolver<Object[]> baseItemDataGetter;

    private State state;
    private Vector2 selectSzie = new(30, 30);
    private List<Single> selectValues = new();

    private List<Rect> ranges = new();

    private Int32 selectId = -1;
    private Color[] colors;

    private Single[] SetData<T>(T[] datas, ref List<Single> select, Single[] value)
    {
        if (datas == null || datas.Length == 0)
        {
            select.Clear();
            ranges.Clear();
            return Array.Empty<Single>();
        }

        if (datas.Length == 1)
        {
            select.Clear();
            ranges.Clear();
            for (Int32 i = 0; i < datas.Length; i++)
            {
                ranges.Add(new Rect());
            }
            return Array.Empty<Single>();
        }

        if (datas.Length - 1 != value.Length)
        {
            select.Clear();
            for (Int32 i = 0; i < datas.Length; i++)
            {
                if (i != datas.Length - 1)
                {
                    select.Add(1 / (Single)datas.Length * (i + 1));
                }
            }
            ranges.Clear();
            for (Int32 i = 0; i < datas.Length; i++)
            {
                ranges.Add(new Rect());
            }
        }
        else if (datas.Length - 1 != select.Count)
        {
            select.Clear();
            for (Int32 i = 0; i < datas.Length; i++)
            {
                if (i != datas.Length - 1)
                {
                    select.Add(value[i]);
                }
            }
            ranges.Clear();
            for (Int32 i = 0; i < datas.Length; i++)
            {
                ranges.Add(new Rect());
            }
        }
        return select.ToArray();
    }

    protected override void Initialize()
    {
        if (Attribute.DataMember != null)
        {
            baseItemDataGetter = ValueResolver.Get<Object[]>(Property, Attribute.DataMember);
            if (errorMessage != null)
            {
                errorMessage = baseItemDataGetter.ErrorMessage;
            }
        }

        if (baseItemDataGetter.GetValue() != null && baseItemDataGetter.GetValue().Length != 0)
        {
            ValueEntry.SmartValue = SetData(baseItemDataGetter.GetValue(), ref selectValues, ValueEntry.SmartValue);
        }

        foreach (ColorPalette __t in ColorPaletteManager.Instance.ColorPalettes)
        {
            if (__t.Name == Attribute.ColorName)
            {
                colors = __t.Colors.ToArray();
                break;
            }
        }
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        DrawAll(baseItemDataGetter);
    }

    private void DrawAll<T>(ValueResolver<T[]> inspectorPropertyValueGetter)
    {
        if (inspectorPropertyValueGetter.GetValue() != null && inspectorPropertyValueGetter.GetValue().Length != 0)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 40);
            DrawRange(rect, baseItemDataGetter);
            DrawSelect(rect, baseItemDataGetter);
        }
    }

    private void DrawRange<T>(Rect rect, ValueResolver<T[]> inspectorPropertyValueGetter)
    {
        if (inspectorPropertyValueGetter.GetValue() != null && inspectorPropertyValueGetter.GetValue().Length != 0)
        {
            SirenixEditorGUI.DrawSolidRect(rect, new Color(0.7f, 0.7f, 0.7f, 1f));
            GUIStyle style = new();
            style.alignment = TextAnchor.UpperCenter;
            GUIStyle percentageStyle = new();
            percentageStyle.alignment = TextAnchor.LowerCenter;

            ValueEntry.SmartValue = SetData(inspectorPropertyValueGetter.GetValue(), ref selectValues, ValueEntry.SmartValue);

            T[] datas = inspectorPropertyValueGetter.GetValue();
            GameObject[] go = datas as GameObject[];

            for (Int32 i = 0; i < ranges.Count; i++)
            {
                if (ranges.Count == 1)
                {
                    ranges[i] = rect.SetXMin(rect.xMin).SetXMax(rect.xMax);
                    SirenixEditorGUI.DrawSolidRect(ranges[i], colors[i]);
                }
                else
                {
                    if (i == 0)
                    {
                        ranges[i] = rect.SetXMin(rect.xMin).SetXMax(rect.width * selectValues[i] + (selectSzie.x / 2));
                        SirenixEditorGUI.DrawSolidRect(ranges[i], colors[i]);
                    }
                    else if (i == ranges.Count - 1)
                    {
                        ranges[i] = rect.SetXMin(rect.width * selectValues[i - 1] + (selectSzie.x / 2));
                        SirenixEditorGUI.DrawSolidRect(ranges[i], colors[i]);
                    }
                    else
                    {
                        ranges[i] = rect.SetXMin(rect.width * selectValues[i - 1] + (selectSzie.x / 2)).SetXMax(rect.width * selectValues[i] + (selectSzie.x / 2));
                        SirenixEditorGUI.DrawSolidRect(ranges[i], colors[i]);
                    }
                    SirenixEditorGUI.DrawSolidRect(ranges[i], colors[i]);
                }

                GUIHelper.PushColor(Color.black);
                if (inspectorPropertyValueGetter.GetValue()[i] != null)
                {
                    if (go != null)
                    {
                        if (go[i] != null)
                        {
                            GUI.Label(ranges[i].AlignCenterX(ranges[i].width / 2), go[i].name, style);
                        }
                    }
                    else
                    {
                        GUI.Label(ranges[i].AlignCenterX(ranges[i].width / 2), inspectorPropertyValueGetter.GetValue()[i].ToString(), style);
                    }
                }
                Single percentage = (ranges[i].width / rect.width) * 100;

                GUI.Label(ranges[i].AlignCenterX(ranges[i].width / 2), Mathf.Clamp(Mathf.RoundToInt(percentage), 0, 100) + "%", percentageStyle);
                GUIHelper.PopColor();
            }
        }
    }

    private void DrawSelect<T>(Rect rect, ValueResolver<T[]> inspectorPropertyValueGetter)
    {
        if (inspectorPropertyValueGetter.GetValue() != null && inspectorPropertyValueGetter.GetValue().Length != 0)
        {
            Event currentEvent = Event.current;
            Rect[] selectPoint = new Rect[selectValues.Count];
            for (Int32 i = 0; i < selectPoint.Length; i++)
            {
                selectPoint[i] = new Rect(rect.width * selectValues[i], rect.y, selectSzie.x, selectSzie.y);
                GUI.Label(selectPoint[i], EditorIcons.Eject.Raw);
            }

            for (Int32 i = 0; i < selectPoint.Length; i++)
            {
                if (selectPoint[i].Contains(currentEvent.mousePosition))
                {
                    if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                    {
                        ChangeState(State.Down);
                        selectId = i;
                    }
                }
            }
            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
            {
                ChangeState(State.Up);
                selectId = -1;
            }
            ///////////////////Mouse Event/////////////////////
            switch (state)
            {
                case State.Down:
                    if (currentEvent.type == EventType.MouseDrag && currentEvent.button == 0)
                    {
                        ChangeState(State.Drag);
                    }
                    break;
                case State.Up:
                    break;
                case State.Drag:
                    if (Event.current.type != EventType.Repaint) return;
                    Single value = currentEvent.mousePosition.x;
                    value = Mathf.Clamp(value, rect.xMin, rect.xMax);
                    value = (value - rect.xMin) / (rect.xMax - rect.xMin);

                    if (selectValues.Count == 1)
                    {
                        selectValues[selectId] = value;
                    }
                    else
                    {
                        if (selectId == 0)
                        {
                            value = Mathf.Clamp(value, 0, selectValues[selectId + 1]);
                            selectValues[selectId] = value;
                        }
                        else if (selectId == selectPoint.Length - 1)
                        {
                            value = Mathf.Clamp(value, selectValues[selectId - 1], 1);
                            selectValues[selectId] = value;
                        }
                        else
                        {
                            value = Mathf.Clamp(value, selectValues[selectId - 1], selectValues[selectId + 1]);
                            selectValues[selectId] = value;
                        }
                    }
                    break;
                case State.Out:
                    break;
                default:
                    break;
            }
        }
    }

    public enum State { None, Down, Up, Drag, Out }

    private void ChangeState(State state)
    {
        this.state = state;
    }
}

#endif
