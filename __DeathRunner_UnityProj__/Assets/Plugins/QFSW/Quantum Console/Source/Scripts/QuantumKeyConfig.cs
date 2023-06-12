using UnityEngine;
using UnityEngine.Serialization;

namespace QFSW.QC
{
    [CreateAssetMenu(fileName = "Untitled Key Config", menuName = "Quantum Console/Key Config")]
    public class QuantumKeyConfig : ScriptableObject
    {
        public KeyCode SubmitCommandKey = KeyCode.Return;
        public ModifierKeyCombo ShowConsoleKey = KeyCode.None;
        public ModifierKeyCombo HideConsoleKey = KeyCode.None;
        public ModifierKeyCombo ToggleConsoleVisibilityKey = KeyCode.Escape;

        public ModifierKeyCombo ZoomInKey = new() { Key = KeyCode.Equals, Ctrl = true };
        public ModifierKeyCombo ZoomOutKey = new() { Key = KeyCode.Minus, Ctrl = true };
        public ModifierKeyCombo DragConsoleKey = new() { Key = KeyCode.Mouse0, Shift = true };

        [FormerlySerializedAs("SuggestNextCommandKey")]
        public ModifierKeyCombo SelectNextSuggestionKey = KeyCode.Tab;
        [FormerlySerializedAs("SuggestPreviousCommandKey")]
        public ModifierKeyCombo SelectPreviousSuggestionKey = new() { Key = KeyCode.Tab, Shift = true };

        public KeyCode NextCommandKey = KeyCode.UpArrow;
        public KeyCode PreviousCommandKey = KeyCode.DownArrow;

        public ModifierKeyCombo CancelActionsKey = new() { Key = KeyCode.C, Ctrl = true };
    }
}
