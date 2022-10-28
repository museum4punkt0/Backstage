using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    [RequireComponent(typeof(Graphic))]
    public class ColorTheme : UIBehaviour
    {
        [SerializeField]
        private enum BlockItem
        {
            Normal,
            Highlighted,
            Pressed,
            Selected,
            Disabled
        }

        //[SerializeField] private Graphic graphic;
        [SerializeField] private BlockItem pickColor;
        [SerializeField] private ColorUnityEvent onColorChanged;
        [SerializeField] private ColorBlockUnityEvent onBlockChanged;


        private static ColorBlock _colors;
        public static event Action<ColorBlock> ColorChanged;

        public static ColorBlock Colors => _colors;

        public static void SetColor(ColorBlock color)
        {
            _colors = color;
            ColorChanged?.Invoke(Colors);
        }

        //protected override void Awake() => graphic = GetComponent<Graphic>();

        protected override void OnEnable()
        {
            ColorChanged += OnColorChanged;
            OnColorChanged(_colors);
        }

        protected override void OnDisable() => ColorChanged -= OnColorChanged;
    
        private void OnColorChanged(ColorBlock colors)
        {
            // graphic.color = colors.highlightedColor;
            onBlockChanged.Invoke(colors);
            switch (pickColor)
            {
                case BlockItem.Normal:
                    onColorChanged.Invoke(_colors.normalColor);
                    break;
                case BlockItem.Highlighted:
                    onColorChanged.Invoke(_colors.highlightedColor);
                    break;
                case BlockItem.Pressed:
                    onColorChanged.Invoke(_colors.pressedColor);
                    break;
                case BlockItem.Selected:
                    onColorChanged.Invoke(_colors.selectedColor);
                    break;
                case BlockItem.Disabled:
                    onColorChanged.Invoke(_colors.disabledColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}