using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_QuestPointer : MonoBehaviour {

    public Camera uiCamera;
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite crossSprite;

    public Vector3 targetPosition = Vector3.zero;
    private RectTransform pointerRectTransform;
    private SpriteRenderer pointerSpriteRenderer;


    public void Initialize(Camera uiCamera, Vector3 targetPosition) {
        pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
        // find the sprite renderer
        pointerSpriteRenderer = transform.Find("Pointer").GetComponent<SpriteRenderer>();

        if (pointerSpriteRenderer == null) Debug.LogError("pointerSpriteRenderer is null!");
        
        this.uiCamera = uiCamera;
        this.targetPosition = targetPosition;

        Show(targetPosition);
    }


    private void Update() {
        float borderSize = 100f;
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

        if (isOffScreen) {
            RotatePointerTowardsTargetPosition();

            pointerSpriteRenderer.sprite = arrowSprite;
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
            if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
            if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;

            Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
        } else {
            pointerSpriteRenderer.sprite = crossSprite;
            Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(targetPositionScreenPoint);
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);

            pointerRectTransform.localEulerAngles = Vector3.zero;
        }
    }

    private float GetAngleFromVectorFloat(Vector3 dir) {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    private void RotatePointerTowardsTargetPosition() {
        Vector3 toPosition = targetPosition;
        Vector3 fromPosition = Camera.main.transform.position;
        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = GetAngleFromVectorFloat(dir);
        pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show(Vector3 targetPosition) {
        gameObject.SetActive(true);
        this.targetPosition = targetPosition;
    }


}