using System;
using System;
using System.Collections;
using System.Collections;
using System.Linq;
using System.Linq;
using DG.Tweening;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening.Plugins.Options;
using Directus.Connect.v9;
using Directus.Connect.v9;
using Exploratorium.Utility;
using Exploratorium.Utility;
using HtmlAgilityPack;
using HtmlAgilityPack;
using Markdig;
using Markdig;
using Markdig.Parsers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax;
using TMPro;
using TMPro;
using UnityEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;

namespace Exploratorium.Frontend
{
    internal class SlidePresenter : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text copyright;
        [SerializeField] private Openable infoGroup;
        [SerializeField] private Openable copyrightGroup;
        [SerializeField] private Transition transition = Transition.Blend;

        private Tween _t;

        public string Title
        {
            set
            {
                if (title != null)
                    title.text = value ?? "";
            }
        }

        public MarkdownDocument Description
        {
            set
            {
                if (description != null)
                {
                    if (description.richText)
                    {
                        if (value != null)
                        {
                            description.text = value?
                                .ToHtml()
                                .ParseHtml()
                                .RemoveTags(in HtmlExtensions.SkbgTags)
                                .ReplaceTag(in HtmlExtensions.SkbgTagsToTextMeshProTags)
                                .DocumentNode
                                .InnerHtml;
                        }
                        else
                        {
                            description.text = "";
                        }
                    }
                    else if (value != null)
                    {
                        description.text = ParsingUtils.GetRawText(value);
                    }
                    else
                    {
                        description.text = "";
                    }
                }
            }
        }

        public Image Image => image;

        public string Copyright
        {
            set
            {
                if (copyright != null)
                    copyright.text = value ?? "";
            }
        }

        public bool IsInfoVisible => infoGroup != null && infoGroup.IsOpen;

        public bool IsCopyrightVisible => copyrightGroup != null && copyrightGroup.IsOpen;

        private void Awake()
        {
            if (image == null)
                image = GetComponentInChildren<Image>();
            if (canvasGroup == null)
                canvasGroup = GetComponentInChildren<CanvasGroup>();
        }

        private void Start()
        {
            if (infoGroup != null)
                infoGroup.Open();
        }

        public void Out(Vector2 direction, float duration, float delay = 0)
        {
            switch (transition)
            {
                case Transition.Blend:
                    FadeOut(duration, delay);
                    break;
                case Transition.Slide:
                    SlideOut(direction, duration, delay);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void In(Vector2 direction, float duration, float delay = 0)
        {
            switch (transition)
            {
                case Transition.Blend:
                    FadeIn(duration, delay);
                    break;
                case Transition.Slide:
                    SlideIn(direction, duration, delay);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SlideIn(Vector2 direction, float duration, float delay)
        {
            _t.Kill(true);
            gameObject.SetActive(true);
            canvasGroup.alpha = 1.0f;
            canvasGroup.transform.localPosition = direction;
            if (canvasGroup != null)
                _t = canvasGroup
                    .transform.DOLocalMove(Vector3.zero, duration)
                    .SetDelay(delay)
                    .SetEase(Ease.OutQuad);
        }

        private void SlideOut(Vector2 direction, float duration, float delay)
        {
            _t.Kill(true);
            gameObject.SetActive(true);
            canvasGroup.alpha = 1.0f;
            canvasGroup.transform.localPosition = Vector3.zero;
            if (canvasGroup != null)
                _t = canvasGroup
                        .transform.DOLocalMove(direction, duration)
                        .SetDelay(delay)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() => gameObject.SetActive(false))
                    /*
                .OnPlay(() => Debug.Log("PLAY", this))
                .OnRewind(() => Debug.Log("REWIND", this))
                .OnStart(() => Debug.Log("START", this))
                .OnUpdate(() => Debug.Log("UPDATE", this))
                .OnKill(() => Debug.Log("KILL", this))
                .SetId("SlideOut")
                */
                    ;
        }

        private void FadeIn(float duration, float delay)
        {
            _t.Kill(true);
            gameObject.SetActive(true);
            canvasGroup.alpha = 0;
            if (canvasGroup != null)
                _t = canvasGroup
                    .DOFade(1.0f, duration)
                    .SetDelay(delay)
                    .SetEase(Ease.InOutQuad);
        }

        private void FadeOut(float duration, float delay)
        {
            _t.Kill(true);
            gameObject.SetActive(true);
            canvasGroup.alpha = 1.0f;
            if (canvasGroup != null)
                _t = TweenSettingsExtensions.SetDelay<TweenerCore<float, float, FloatOptions>>(canvasGroup
                        .DOFade(0, duration), delay)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => gameObject.SetActive(false));
        }

        public void SetInfoVisible(bool isVisible)
        {
            if (infoGroup == null)
                return;

            if (isVisible)
                infoGroup.Open();
            else
                infoGroup.Close();
        }

        public void SetCopyrightVisible(bool isVisible)
        {
            if (copyrightGroup == null)
                return;

            if (isVisible)
                copyrightGroup.Open();
            else
                copyrightGroup.Close();
        }
    }

    internal enum Transition
    {
        Blend,
        Slide
    }
}