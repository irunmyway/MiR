using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteAlways]
    public sealed class TutorialOverlay : Graphic, IPointerClickHandler, ICanvasRaycastFilter
    {
        [SerializeField] bool HoleEnabled;
        [SerializeField] Vector2 HolePosition;
        [SerializeField] Vector2 HoleSize;

        bool MessageEnabled;

        public TextMeshProUGUI Text;
        public RectTransform Message;
        public RectTransform Finger;

        [Inject] ITutorialManager tutorialManager = default;

        protected override void Start()
        {
            base.Start();

            if (Application.IsPlaying(this)) {
                MessageEnabled = false;
                HoleEnabled = false;
                Message.gameObject.SetActive(false);
                Finger.gameObject.SetActive(false);
            }
        }

      #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetVerticesDirty();
        }
      #endif

        public void DisableHole()
        {
            if (HoleEnabled) {
                HoleEnabled = false;
                Finger.gameObject.SetActive(false);
                SetVerticesDirty();
            }
        }

        public void EnableHole(RectTransform target, bool showFinger)
        {
            if (!HoleEnabled) {
                HoleEnabled = true;

                /*
                Vector3[] corners = new Vector3[4];
                target.GetWorldCorners(corners);

                var camera = Camera.main;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, camera.WorldToScreenPoint(corners[0]), null, out var bl);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, camera.WorldToScreenPoint(corners[1]), null, out var tl);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, camera.WorldToScreenPoint(corners[2]), null, out var tr);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, camera.WorldToScreenPoint(corners[3]), null, out var br);
                */

                var camera = Camera.main;
                var min = camera.WorldToScreenPoint(target.TransformPoint(target.rect.min));
                var max = camera.WorldToScreenPoint(target.TransformPoint(target.rect.max));

                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, min, null, out var min2d);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, max, null, out var max2d);

                var center = (min2d + max2d) * 0.5f;
                var size = new Vector2(Mathf.Abs(max2d.x - min2d.x), Mathf.Abs(max2d.y - min2d.y));

                HolePosition = center;
                HoleSize = size;

                Finger.gameObject.SetActive(showFinger);

                SetVerticesDirty();
            }
        }

        public void DisableMessage()
        {
            if (MessageEnabled) {
                MessageEnabled = false;
                Message.gameObject.SetActive(false);
            }
        }

        public void EnableMessage(string message)
        {
            if (!MessageEnabled) {
                MessageEnabled = true;
                Text.text = message;
                Message.gameObject.SetActive(true);
            }
        }

        bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screen, Camera camera)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screen, camera, out Vector2 local))
                return false;

            Vector2 p1 = HolePosition - HoleSize * 0.5f;
            Vector2 p2 = HolePosition + HoleSize * 0.5f;

            //Debug.Log($"{p1} {p2}");

            var p = local + rectTransform.pivot * rectTransform.rect.size;
            return (p.x < p1.x || p.x > p2.x || p.y < p1.y || p.y > p2.y);
        }

        public void OnPointerClick(PointerEventData data)
        {
            foreach (var it in tutorialManager.OnTutorialOverlayClicked.Enumerate())
                it.Do();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Vector2 tl = (Vector2.zero - rectTransform.pivot) * rectTransform.rect.size;
            Vector2 br = (Vector2.one  - rectTransform.pivot) * rectTransform.rect.size;

            if (!HoleEnabled)
                PopulateRect(vh, tl.x, tl.y, br.x, br.y, color);
            else {
                Vector2 c = tl + HolePosition;
                Vector2 holeTL = c - HoleSize * 0.5f;
                Vector2 holeBR = c + HoleSize * 0.5f;

                PopulateRect(vh, tl.x,     tl.y,     br.x,     holeTL.y, color);
                PopulateRect(vh, tl.x,     holeBR.y, br.x,     br.y,     color);
                PopulateRect(vh, tl.x,     holeTL.y, holeTL.x, holeBR.y, color);
                PopulateRect(vh, holeBR.x, holeTL.y, br.x,     holeBR.y, color);

                if (Finger != null)
                    Finger.anchoredPosition = c;
            }
        }

        void PopulateRect(VertexHelper vh, float x1, float y1, float x2, float y2, Color color)
        {
            int index = vh.currentVertCount;

            UIVertex v0 = UIVertex.simpleVert;
            v0.position = new Vector2(x1, y1);
            v0.color = color;
            vh.AddVert(v0);

            UIVertex v1 = UIVertex.simpleVert;
            v1.position = new Vector2(x2, y1);
            v1.color = color;
            vh.AddVert(v1);

            UIVertex v2 = UIVertex.simpleVert;
            v2.position = new Vector2(x2, y2);
            v2.color = color;
            vh.AddVert(v2);

            UIVertex v3 = UIVertex.simpleVert;
            v3.position = new Vector2(x1, y2);
            v3.color = color;
            vh.AddVert(v3);

            vh.AddTriangle(index + 0, index + 1, index + 3);
            vh.AddTriangle(index + 1, index + 2, index + 3);
        }
    }
}
