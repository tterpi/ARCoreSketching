//-----------------------------------------------------------------------
//
// Für den AR-Beleg neu erstellt.
//
//-----------------------------------------------------------------------

namespace Sketching
{
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections.Generic;

    /// <summary>
    /// Controls the creation of line sketch objects via a tap gesture.
    /// </summary>
    public class SketchObjectManipulator : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        /// </summary>
        public Camera FirstPersonCamera;

        public GameObject SketchObjectPrefab;

        private Anchor worldAnchor;
        private LineSketchObject currentLineSketchObject;
        private Stack<LineSketchObject> LineSketchObjects = new Stack<LineSketchObject>();
        private bool canStartTouchManipulation = false;

        public GameObject pointMarker;
        private bool touchBegan = false;

        public void Start()
        {
            pointMarker.transform.SetParent(FirstPersonCamera.transform);
            pointMarker.transform.localPosition = Vector3.forward * .3f;
        }

        public void Update()
        {
            if (Input.touchCount > 0) {
                Touch currentTouch = Input.GetTouch(0);
                if (currentTouch.phase == TouchPhase.Began) {
                        canStartTouchManipulation = CanStartTouchManipulation();
                }

                if (canStartTouchManipulation) {
                    if (currentTouch.phase == TouchPhase.Began)
                    {
                        touchBegan = true;
                    }
                    else if (currentTouch.phase == TouchPhase.Stationary)
                    {

                        if (touchBegan) {
                            OnStartTouchManipulation();
                            touchBegan = false;
                        }
                        else if (currentLineSketchObject)
                        {
                            //Debug.Log("Attempting to add control point.");
                            currentLineSketchObject.addControlPointContinuous(FirstPersonCamera.transform.position + FirstPersonCamera.transform.forward * .3f);
                        }
                    }
                    else if (currentTouch.phase == TouchPhase.Ended) {
                        //only delete if the current touch is not a hold touch
                        //Debug.Log("swipe distance: " + (currentTouch.position.y - currentTouch.rawPosition.y)+ " screen width: " +  Screen.width);
                        //Debug.Log("end fingerid: " + currentTouch.fingerId);
                        //Debug.Log("Ending line creation");
                        
                        OnEndTouchManipulation();
                        //if swipe left occured, delete the last sketch object
                        if ((currentTouch.position - currentTouch.rawPosition).magnitude > Screen.width * 0.05)
                        {
                            //Debug.Log("Deleting last object");
                            DeleteLastLineSketchObject();
                        }

                        canStartTouchManipulation = false;
                    }
                }
            }
        }

        private bool CanStartTouchManipulation()
        {
            // Should not handle input if the player is pointing on UI.
            if (Session.Status != SessionStatus.Tracking || EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Debug.Log("Not starting tap gesture");
                return false;
            }
            return true;
        }

        private void OnStartTouchManipulation()
        {
            //see if an anchor exists
            if (!worldAnchor) {
                Debug.Log("Create world anchor");
                worldAnchor = Session.CreateAnchor(Frame.Pose);
            }
            Debug.Log("Creating sketch object");
            // Instantiate game object at the hit pose.
            var gameObject = Instantiate(SketchObjectPrefab, worldAnchor.gameObject.transform);
            currentLineSketchObject = gameObject.GetComponent<LineSketchObject>();
        }

        private void OnEndTouchManipulation()
        {
            if (currentLineSketchObject && !LineSketchObjects.Contains(currentLineSketchObject)) {
                LineSketchObjects.Push(currentLineSketchObject);
            }

        }
        public void DeleteLastLineSketchObject() {
            if (LineSketchObjects.Count != 0) {
                Destroy(LineSketchObjects.Pop().gameObject);
            }
        }
    }
}
