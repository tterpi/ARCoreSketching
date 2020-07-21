//-----------------------------------------------------------------------
// <copyright file="PawnManipulator.cs" company="Google LLC">
//
// Copyright 2019 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace Sketching
{
    using GoogleARCore;
    using UnityEngine;
    using GoogleARCore.Examples.ObjectManipulation;
    using UnityEngine.EventSystems;
    using System.Collections.Generic;

    /// <summary>
    /// Controls the placement of objects via a tap gesture.
    /// </summary>
    public class SketchObjectManipulator : Manipulator
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject SketchObjectPrefab;

        /// <summary>
        /// Manipulator prefab to attach placed objects to.
        /// </summary>
        public GameObject ManipulatorPrefab;

        private Anchor worldAnchor;
        private LineSketchObject currentLineSketchObject;
        private bool addingControlPoints = false;
        private Stack<LineSketchObject> LineSketchObjects = new Stack<LineSketchObject>();

        protected override void Update()
        {
            base.Update();

            if (addingControlPoints)
            {
                //Debug.Log("Continuing tap gesture");
                if (currentLineSketchObject)
                {
                    //Debug.Log("Attempting to add control point.");
                    currentLineSketchObject.addControlPointContinuous(FirstPersonCamera.transform.position + FirstPersonCamera.transform.forward * .3f);
                }
            }

        }

        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>True if the manipulation can be started.</returns>
        protected override bool CanStartManipulationForGesture(DragGesture gesture)
        {
            // Should not handle input if the player is pointing on UI.
            if (Session.Status != SessionStatus.Tracking || EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Debug.Log("Not starting tap gesture");
                return false;
            }

            return true;
        }

        protected override void OnStartManipulation(DragGesture gesture)
        {
            //base.OnStartManipulation(gesture);
            //see if an anchor exists
            if (!worldAnchor) {
                Debug.Log("Create world anchor");
                worldAnchor = Session.CreateAnchor(Frame.Pose);
            }
            Debug.Log("Creating sketch object");
            // Instantiate game object at the hit pose.
            var gameObject = Instantiate(SketchObjectPrefab, worldAnchor.gameObject.transform);
            currentLineSketchObject = gameObject.GetComponent<LineSketchObject>();

            addingControlPoints = true;

            Select();
            if (ManipulationSystem.Instance.SelectedObject != this.gameObject)
            {
                Debug.Log("Selecting manipulator failed. selected game object: " + ManipulationSystem.Instance.SelectedObject.name);
            }
            else {
                Debug.Log("Sketch object manipulator selected.");
            }
        }

        /// <summary>
        /// Function called when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(DragGesture gesture)
        {
            addingControlPoints = false;
            if (currentLineSketchObject) {
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
