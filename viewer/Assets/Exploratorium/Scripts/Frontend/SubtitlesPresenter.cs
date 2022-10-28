using RenderHeads.Media.AVProVideo;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Based on SubtitlesUGUI by RenderHeads Ltd.

namespace Exploratorium.Frontend
{
    /// <summary>
    /// Update a standard uGUI Text element with subtitle text as it plays from the MediaPlayer
    /// </summary>
    public class SubtitlesPresenter : MonoBehaviour
    {
        [FormerlySerializedAs("_mediaPlayer")] [SerializeField]
        MediaPlayer mediaPlayer = null;

        [FormerlySerializedAs("_text")] [SerializeField]
        TMP_Text text = null;

        [FormerlySerializedAs("_backgroundImage")] [SerializeField]
        Image backgroundImage = null;

        [FormerlySerializedAs("_backgroundHorizontalPadding")] [SerializeField]
        int backgroundHorizontalPadding = 32;

        [FormerlySerializedAs("_backgroundVerticalPadding")] [SerializeField]
        int backgroundVerticalPadding = 16;

        [FormerlySerializedAs("_maxCharacters")] [SerializeField, Range(-1, 1024)]
        int maxCharacters = 256;

        public MediaPlayer Player
        {
            set => ChangeMediaPlayer(value);
            get => mediaPlayer;
        }

        public TMP_Text Text
        {
            set => text = value;
            get => text;
        }

        void Start()
        {
            ChangeMediaPlayer(mediaPlayer);
        }

        void OnDestroy()
        {
            ChangeMediaPlayer(null);
        }

        void Update()
        {
            // TODO: Currently we need to call this each frame, as when it is called right after SetText() 
            // the ContentSizeFitter hasn't run yet, so effectively the box is a frame behind.
            UpdateBackgroundRect();
        }

        public void ChangeMediaPlayer(MediaPlayer newPlayer)
        {
            // When changing the media player, handle event subscriptions
            if (mediaPlayer != null)
            {
                mediaPlayer.Events.RemoveListener(OnMediaPlayerEvent);
                mediaPlayer = null;
            }

            SetText(string.Empty);

            if (newPlayer != null)
            {
                newPlayer.Events.AddListener(OnMediaPlayerEvent);
                mediaPlayer = newPlayer;
            }
        }

        private void SetText(string text)
        {
            this.text.text = text;
            UpdateBackgroundRect();
        }

        private string PrepareText(string text)
        {
            // Crop text that is too long
            if (maxCharacters >= 0 && text.Length > maxCharacters)
            {
                text = text.Substring(0, maxCharacters);
            }

            // Change RichText for Unity uGUI Text
            text = text.Replace("<font color=", "<color=");
            text = text.Replace("</font>", "</color>");
            text = text.Replace("<u>", string.Empty);
            text = text.Replace("</u>", string.Empty);
            return text;
        }

        private void UpdateBackgroundRect()
        {
            if (backgroundImage != null)
            {
                if (string.IsNullOrEmpty(text.text))
                {
                    backgroundImage.enabled = false;
                }
                else
                {
                    backgroundImage.enabled = true;
                    backgroundImage.rectTransform.sizeDelta = text.rectTransform.sizeDelta;
                    backgroundImage.rectTransform.anchoredPosition = text.rectTransform.anchoredPosition;
                    backgroundImage.rectTransform.offsetMin -=
                        new Vector2(backgroundHorizontalPadding, backgroundVerticalPadding);
                    backgroundImage.rectTransform.offsetMax +=
                        new Vector2(backgroundHorizontalPadding, backgroundVerticalPadding);
                }
            }
        }

        // Callback function to handle events
        private void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
        {
            switch (et)
            {
                case MediaPlayerEvent.EventType.Closing:
                {
                    SetText(string.Empty);
                    break;
                }
                case MediaPlayerEvent.EventType.SubtitleChange:
                {
                    SetText(PrepareText(mediaPlayer.Subtitles.GetSubtitleText()));
                    break;
                }
            }
        }
    }
}