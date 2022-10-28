// //////////////////////////////////////////////////////
// THIS IS PROTOTYPE CODE AND NOT INTENDED FOR PRODUCTION
// TODO: Make this production ready
// //////////////////////////////////////////////////////
using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Exploratorium.Prototype
{
    [RequireComponent(typeof(ToggleGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class Pages : UIBehaviour, ILayoutController, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [Serializable]
        public class PageEvent : UnityEvent<int>
        {
        }

        [SerializeField] private RectTransform pagesContainer;
        [SerializeField] private RectTransform clipRect;
        [SerializeField] private ToggleGroup toggleGroup;

        [SerializeField] private Button next;
        [SerializeField] private Button previous;
        [SerializeField] private Vector2 pageSize = new Vector2(100, 100);
        [SerializeField] private bool loop = true;
        [SerializeField] private float animationBeat = 0.35f;

        [SerializeField] private bool enableDragging;
        [SerializeField] private int initialPage;
        [SerializeField] private Toggle paginationPrefab;
        [SerializeField] private LayoutGroup paginationLayout;

        [Min(.5f)]
        [SerializeField] private float range = 0.5f;

        [SerializeField] private float triggerRange = 0.2f;

        [SerializeField] private PageEvent onPageChanged;
        public event Action<int> PageChanged;

        public UnityEvent onClick;

        private Tweener _activeTransition;
        [ShowInInspector]
        private int _currentPage;

        /// <remarks>Mirrors the drag eventState outside of event handlers and indicates whether <see cref="_dragDelta"/> and <see cref="_dragStart"/> are being updated.</remarks>
        private bool _isDragging;

        /// <remarks>Only updated while <see cref="_isDragging"/> is true</remarks>
        private Vector2 _dragDelta;

        /// <remarks>Only updated while <see cref="_isDragging"/> is true</remarks>
        private Vector2 _dragStart;

        /// <remarks>Only updated while <see cref="_isDragging"/> is true</remarks>
        private Vector2 _constrainedDragDelta;

        [SerializeField] private bool deactivatePages = false;
        [Tooltip("Whether to reset the current page on enable.")]
        [SerializeField] private bool resetOnEnable;

        public RectTransform GetPage(int index) => PagesContainer.GetChild(index).GetComponent<RectTransform>();

        public int CurrentPage
        {
            get => _currentPage;
            private set
            {
                _currentPage = value;
                SilentlySetToggleState(value);
            }
        }

        private void SilentlySetToggleState(int on)
        {
            for (var i = 0; i < paginationLayout.transform.childCount; i++)
                paginationLayout.transform
                    .GetChild(i)?
                    .GetComponent<Toggle>()?
                    .SetIsOnWithoutNotify(i == on);
        }

        public int PageCount => PagesContainer.childCount;

        public RectTransform PagesContainer
        {
            get => pagesContainer;
            private set => pagesContainer = value;
        }

        public bool Loop
        {
            get => loop;
            set => loop = value;
        }

        public bool EnableDragging
        {
            get => enableDragging;
            set => enableDragging = value;
        }

        public Vector2 PageSize => pageSize;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (!Application.isPlaying)
                SetPage(_currentPage);
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            //PagesContainer.pivot = new Vector2(.5f, .5f);
            _currentPage = 0;
            GoToPage(_currentPage);
            next.onClick.AddListener(NextEventHandler);
            previous.onClick.AddListener(PreviousEventHandler);
            if (toggleGroup == null)
                toggleGroup = GetComponent<ToggleGroup>();
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            CreatePagination();

            
            if (!resetOnEnable)
                GoToPage(initialPage);
            
            var rectTransform = GetComponent<RectTransform>();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        private void NextEventHandler()
        {
            if (PageCount == 2)
                LayoutNeighborhood(CurrentPage, CurrentPage, CurrentPage + 1, PageCount, loop, PagesContainer, false);
            NextPage();
        }

        private void PreviousEventHandler()
        {
            if (PageCount == 2)
                LayoutNeighborhood(CurrentPage, CurrentPage - 1, CurrentPage, PageCount, loop, PagesContainer, false);
            PreviousPage();
        }

        [ContextMenu("Create Pagination")]
        [Button]
        private void CreatePagination()
        {
            RemovePagination();
            for (int i = 0; i < PageCount; i++)
                InstantiatePaginationToggle(i, paginationPrefab, paginationLayout, toggleGroup);
        }

        [ContextMenu("Remove Pagination")]
        [Button]
        private void RemovePagination()
        {
            for (var i = paginationLayout.transform.childCount - 1; i >= 0 ; i--)
            {
                if (Application.isPlaying)
                    Destroy(paginationLayout.transform.GetChild(i).gameObject);
                else
                    DestroyImmediate(paginationLayout.transform.GetChild(i).gameObject);
            }
        }

        private Toggle InstantiatePaginationToggle(int page, Toggle prefab, LayoutGroup parent, ToggleGroup group)
        {
            Toggle toggle = Instantiate(prefab, parent.transform);
            toggle.@group = group;
            group.allowSwitchOff = false;
            var pageIndex = page;
            toggle.isOn = page == CurrentPage;

            void ValueChangedHandler(bool isOn)
            {
                if (isOn)
                    GoToPage(pageIndex);
            }

            toggle.onValueChanged.AddListener(ValueChangedHandler);

            return toggle;
        }

        public void NextPage()
        {
            //Debug.Log("NextPage");
            GoToPage(CurrentPage + 1);
        }

        public void PreviousPage()
        {
            //Debug.Log("PreviousPage");
            GoToPage(CurrentPage - 1);
        }

        public RectTransform[] GetNeighborsOf(int index, int pageCount, bool isLooping)
        {
            var current = GetNormalIndex(index, pageCount, isLooping);
            var left = GetNormalIndex(index - 1, pageCount, isLooping);
            var right = GetNormalIndex(index + 1, pageCount, isLooping);
            var hasLeft = left != current && pageCount > 0;
            var hasRight = right != current && pageCount > 0;
            //Debug.Log($"GetNeighborsOf {index} -- L:{left} R:{right}");

            return new[]
            {
                hasLeft ? GetPage(left) : null,
                hasRight ? GetPage(right) : null
            };
        }

        public void LayoutNeighborhood(int index, int left, int right, int pageCount, bool isLooping,
            RectTransform pages, bool freezeContainer = true)
        {
            if (freezeContainer)
                PagesContainer.localPosition = Vector3.zero;
            var indexPage = GetPage(GetNormalIndex(index, pageCount, isLooping));
            var leftPage = GetPage(GetNormalIndex(left, pageCount, isLooping));
            var rightPage = GetPage(GetNormalIndex(right, pageCount, isLooping));
            foreach (RectTransform page in pages)
            {
                var isLeftNeighbor = leftPage == page;
                var isRightNeighbor = rightPage == page;
                var isCurrent = indexPage == page;
                if (isCurrent)
                {
                    // putting this first ensures that the current page always ends up in the middle i.e. when left or right == index
                    page.gameObject.SetActive(true);
                    page.localPosition = GetPagePosAtIndex(1, 3) * Vector2.left;
                }
                else if (isLeftNeighbor)
                {
                    page.gameObject.SetActive(true);
                    page.localPosition = GetPagePosAtIndex(0, 3) * Vector2.left;
                }
                else if (isRightNeighbor)
                {
                    page.gameObject.SetActive(true);
                    page.localPosition = GetPagePosAtIndex(2, 3) * Vector2.left;
                }
                else
                {
                    // non-neighbors
                    if (deactivatePages)
                        page.gameObject.SetActive(false);
                }
            }
        }

        public void LayoutNeighborhood(int index, int pageCount, bool isLooping, Transform pages)
        {
            PagesContainer.anchoredPosition = Vector3.zero;
            int modIndex = GetNormalIndex(index, pageCount, isLooping);
            RectTransform[] neighbors = GetNeighborsOf(modIndex, pageCount, isLooping);
            foreach (RectTransform page in pages)
            {
                var isLeftNeighbor = page != null && neighbors[0] == page;
                var isRightNeighbor = page != null && neighbors[1] == page;
                var isCurrent = GetPage(modIndex) == page;
                if (isLeftNeighbor)
                {
                    page.gameObject.SetActive(true);
                    page.localPosition = GetPagePosAtIndex(0, 3) * Vector2.left;
                }
                else if (isCurrent)
                {
                    page.gameObject.SetActive(true);
                    page.localPosition = GetPagePosAtIndex(1, 3) * Vector2.left;
                }
                else if (isRightNeighbor)
                {
                    page.gameObject.SetActive(true);
                    page.localPosition = GetPagePosAtIndex(2, 3) * Vector2.left;
                }
                // non-neighbors
                else
                {
                    page.gameObject.SetActive(false);
                }
            }
        }

        public void NearestPage() => GoToPage(CurrentPage - 1);

        /// <summary>
        /// Transitions to a page.
        /// </summary>
        public void GoToPage(int to)
        {
            //Debug.Log($"GoToPage {to}.");

            void OnComplete()
            {
                CurrentPage = GetNormalIndex(to, PageCount, loop);
                LayoutNeighborhood(to, PageCount, loop, PagesContainer);
                SilentlySetToggleState(to);
                UpdateControls();
                onPageChanged?.Invoke(to);
                PageChanged?.Invoke(to);
            }

            var ignore = CurrentPage;
            var toNormal = GetNormalIndex(to, PageCount, loop);

            if (to == CurrentPage)
                PlayTransitionAnimation(pageIndex: 1, pageCount: 3, onComplete: OnComplete);
            else if (to < CurrentPage - 1 || to > CurrentPage + 1)
            {
                // for longer jumps and 2-page layouts we always pretend the pages are adjacent
                if (toNormal < CurrentPage)
                {
                    LayoutNeighborhood(CurrentPage, toNormal, ignore, PageCount, false, PagesContainer);
                    PlayTransitionAnimation(pageIndex: 0, pageCount: 3, onComplete: OnComplete);
                }
                else
                {
                    LayoutNeighborhood(CurrentPage, ignore, toNormal, PageCount, false, PagesContainer);
                    PlayTransitionAnimation(pageIndex: 2, pageCount: 3, onComplete: OnComplete);
                }
            }
            else if (to == CurrentPage - 1)
            {
                PlayTransitionAnimation(pageIndex: 0, pageCount: 3, onComplete: OnComplete);
            }
            else if (to == CurrentPage + 1)
            {
                PlayTransitionAnimation(pageIndex: 2, pageCount: 3, onComplete: OnComplete);
            }
        }

        private void UpdateControls()
        {
            if (PageCount > 1)
            {
                var current = GetNormalIndex(CurrentPage, PageCount, Loop);
                var left = GetNormalIndex(CurrentPage - 1, PageCount, Loop);
                var right = GetNormalIndex(CurrentPage + 1, PageCount, Loop);
                var hasLeft = left != current;
                var hasRight = right != current;
                previous.gameObject.SetActive(hasLeft);
                next.gameObject.SetActive(hasRight);
                paginationLayout.gameObject.SetActive(true);
            }
            else
            {
                previous.gameObject.SetActive(false);
                next.gameObject.SetActive(false);
                paginationLayout.gameObject.SetActive(false);
            }
        }

        private void PlayTransitionAnimation(int pageIndex, int pageCount, Action onComplete)
        {
            Vector3 pagePos = GetPagePosAtIndex(pageIndex, pageCount);
            Vector3 currentPos = PagesContainer.localPosition;
            Vector3 moveBy = (currentPos - pagePos);
            moveBy.Scale(Vector3.left);
            // goto can only trigger when there is no transition running (in case of frequent button tapping)
            if (_activeTransition == null || !_activeTransition.IsActive() || !_activeTransition.IsPlaying())
                _activeTransition = PagesContainer
                    .DOBlendableLocalMoveBy(moveBy, animationBeat, false)
                    .OnComplete(onComplete.Invoke);
            else
            {
                //Debug.Log("animation blocked");
            }
        }

        public void GoToPageFloating(float pageIndex)
        {
            SetPageFloating(pageIndex);
        }

        /// <summary>
        /// Immediately activates a page without animation.
        /// </summary>
        private void SetPage(int pageIndex)
        {
            //Debug.Log($"SetPage {pageIndex}");
            CurrentPage = GetNormalIndex(pageIndex, PageCount, loop);
            LayoutNeighborhood(pageIndex, PageCount, loop, PagesContainer);
            SilentlySetToggleState(pageIndex);
        }

        private static int Mod(int a, int n)
        {
            if (n == 0)
                return a;
            return (a % n + n) % n;
        }

        private static int GetNormalIndex(int pageIndex, int pageCount, bool isLooping)
        {
            int clampedIndex;

            if (isLooping)
                clampedIndex = Mod(pageIndex, pageCount);
            else
                clampedIndex = Mathf.Clamp(pageIndex, 0, pageCount - 1);

            return clampedIndex;
        }

        private void SetPageFloating(float pageIndex)
        {
            throw new NotImplementedException();
        }

        public void SetLayoutHorizontal()
        {
        }

        public void SetLayoutVertical()
        {
        }

        #region IDrag

        static float Constrain(float origin, float position, float range)
        {
            if (position >= origin + range)
                return origin + range;
            if (position <= origin - range)
                return origin - range;
            return position;
        }

        private float ConstrainSmooth(float origin, float position, float range)
        {
            var delta = position - origin;
            var sign = delta < 0 ? -1f : 1f;
            var t = Mathf.Clamp(Mathf.Abs(delta / range), 0, 1.0f);
            var smoothedPosition = origin + sign * range * Mathf.Atan(t);

            /*if (smoothedPosition >= origin + range)
            return origin + range;
        if (smoothedPosition <= origin - range)
            return origin - range;*/

            return smoothedPosition;
        }

        private Vector3 Constrain(Vector3 origin, Vector3 position, Vector3 range)
        {
            return new Vector3(
                Constrain(origin.x, position.x, range.x),
                Constrain(origin.y, position.y, range.y),
                Constrain(origin.z, position.z, range.z)
            );
        }

        private Vector3 ConstrainSmooth(Vector3 origin, Vector3 position, Vector3 range)
        {
            return new Vector3(
                ConstrainSmooth(origin.x, position.x, range.x),
                ConstrainSmooth(origin.y, position.y, range.y),
                ConstrainSmooth(origin.z, position.z, range.z)
            );
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!EnableDragging || PageCount <= 1)
                return;

            _dragDelta = eventData.position - _dragStart;

            if (eventData.pointerDrag == gameObject && eventData.dragging && _isDragging)
            {
                Vector2 homePos = Vector2.zero;
                Vector3 constrainedPos = ConstrainSmooth(homePos, homePos + _dragDelta, (Vector3) PageSize * range);
                _constrainedDragDelta = (Vector2) constrainedPos - homePos;
                var moveBy = new Vector2(
                    constrainedPos.x - PagesContainer.localPosition.x,
                    constrainedPos.y - PagesContainer.localPosition.y
                );
                
                MovePageRectBy(moveBy);
                
                if (PageCount == 2)
                {
                    if (_dragDelta.x < 0)
                        LayoutNeighborhood(CurrentPage, CurrentPage, CurrentPage + 1, PageCount, loop, PagesContainer, false);
                    else
                        LayoutNeighborhood(CurrentPage, CurrentPage - 1, CurrentPage, PageCount, loop, PagesContainer, false);
                }

            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!EnableDragging || PageCount <= 1)
                return;

            //Debug.Log("OnBeginDrag");

            DOTween.Kill(PagesContainer);
            _isDragging = true;
            _dragStart = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!EnableDragging || PageCount <= 1)
                return;

            //Debug.Log("OnEndDrag");

            _isDragging = false;

            // drag is a swipe motion, so in any case it will only ever turn one page (no cover flow)

            // so all we do is detect whether or not a swipe was pronounce enough to trigger a call to a next
            // page command. While we observe the drag, we also give visual feedback by moving the viewed page in sync
            // with the drag, already revealing the next item below or next to it (but no more than one). If a swipe would
            // put an item outside of the containing frame, it is stopped and highlighted to indicate that the swipe
            // input complete. On release of the drag, the position of the dragged item is either reverted to its original
            // position or completed such that the next item is fully visible.


            _dragDelta = eventData.position - _dragStart;
            var isLeft = _constrainedDragDelta.x > pageSize.x * +triggerRange;
            var isRight = _constrainedDragDelta.x < pageSize.x * -triggerRange;
            if (isLeft)
                PreviousPage();
            else if (isRight)
                NextPage();
            else
                GoToPage(CurrentPage);

            /*
        // continue moving after drag
        // todo: animation smoothness/predictability  would benefit from a smoothed velocity value (moving average)

        // <<< tip: read this block in reverse order
        Vector2 velocity = _dragDelta / Time.deltaTime; // px per sec
        Vector2 distanceToStop = velocity * animationBeat;
        Vector3 currentPos = PagesRect.localPosition;
        Vector3 stopPos = currentPos + (Vector3) distanceToStop;
        int nearestPageIndex = GetPageIndexAtPos(stopPos);
        Vector2 nearestPagePos = GetPagePosAtIndex(nearestPageIndex);
        Vector3 moveBy = (Vector3) nearestPagePos - currentPos;
        moveBy.Scale(Vector3.right);
        Vector3 to = Vector3.zero;
        // >>>

        */

            // blendable moveBy tween
/*
        RectTransform target = PagesRect;
        DOTween.To(
                () => to,
                x =>
                {
                    Vector3 vector3 = x - to;
                    to = x;
                    target.localPosition += vector3;
                },
                moveBy,
                runOut * Mathf.Sqrt(velocity.x * 2.0f)
            )
           .Blendable()
           .SetOptions(true)
           .SetTarget(target);
           */
        }

        private Vector2 GetPagePosAtIndex(int i, int pageCount)
        {
            //var normalIndex = GetNormalIndex(i, pageCount, isLooping);
            Vector2 pageExtent = pageSize * 0.5f;
            Vector2 pagesRectExtent = pageExtent * pageCount;
            Vector2 pagePos = pageExtent + (i * pageSize) - pagesRectExtent;
            Vector2 pos = pagePos * -1.0f;
            //Debug.Log($"{nameof(GetPagePosAtIndex)}({i}, {pageCount}) : {pos}");
            return pos;
        }

        private int GetPageIndexAtPos(Vector2 pos, int pageCount, bool isLooping)
        {
            Vector2 pageExtent = pageSize * 0.5f;
            Vector2 pagesRectExtent = pageExtent * pageCount;
            float floatingIndex = (pos.x - pageExtent.x + pagesRectExtent.x) / -pageSize.x;
            int index = GetNormalIndex(Mathf.RoundToInt(floatingIndex), pageCount, isLooping);
            //Debug.Log($"{nameof(GetPageIndexAtPos)}({pos}) : float {floatingIndex} : {index}");
            return index;
        }

        #endregion

        private void MovePageRectBy(Vector2 delta)
        {
            Vector3 originalPos = PagesContainer.localPosition;
            Vector3 pagePos = new Vector3(originalPos.x + delta.x, 0, 0);
            Vector3 compositePos = new Vector3(pagePos.x, originalPos.y, originalPos.z);
            Vector3 moveBy = delta;
            moveBy.Scale(Vector3.right);
            PagesContainer.localPosition += moveBy;
//        PagesRect.DOBlendableLocalMoveBy(moveBy, animationBeat, true);
            //Debug.Log($"{nameof(MovePageRectBy)}({delta})");
        }

        private void SnapToNearest()
        {
            throw new NotImplementedException();
        }

        /*
        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"isDragging {_isDragging}");
            GUILayout.Label($"dragDelta {_dragDelta}");
            GUILayout.Label($"constrainedDragDelta{_constrainedDragDelta}");
            GUILayout.Label($"dragStart {_dragStart}");
            GUILayout.EndVertical();
        }
        */
        
        public void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
        }
    }
}