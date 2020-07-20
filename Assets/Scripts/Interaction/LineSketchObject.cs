using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splines;

public class LineSketchObject : MonoBehaviour
{
    private SplineMesh SplineMesh;

    private MeshFilter meshFilter;

    private MeshCollider meshCollider;

    private SplineMesh LinearSplineMesh;

    [SerializeField]
    private GameObject sphereObject;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        SplineMesh = new SplineMesh(new KochanekBartelsSpline(), meshFilter);
        LinearSplineMesh = new SplineMesh(new LinearInterpolationSpline(), meshFilter);

        //Vector3[] controlPoints = { new Vector3(0, 0, 0), new Vector3(.2f, .2f, 0), new Vector3(.4f, 0, 0) };
        //////SplineMesh.setControlPoints(controlPoints);

        //foreach (Vector3 point in controlPoints)
        //{
        //    SplineMesh.addControlPoint(point);
        //}

        ////SplineMesh.deleteControlPoint(SplineMesh.getNumberOfControlPoints() - 1);
        //deleteControlPoint();
        //deleteControlPoint();

        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    public void addControlPoint(Vector3 point) {
        Vector3 transformedPoint = transform.InverseTransformPoint(point);
        SplineMesh.addControlPoint(transformedPoint);
        chooseDisplayMethod();
        
    }

    public void deleteControlPoint() {
        SplineMesh.deleteControlPoint(SplineMesh.getNumberOfControlPoints() - 1);
        chooseDisplayMethod();
    }

    public void chooseDisplayMethod() {
        sphereObject.SetActive(false);
        if (SplineMesh.getNumberOfControlPoints() == 0) {
            //display nothing
            meshFilter.mesh = new Mesh();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
        else if (SplineMesh.getNumberOfControlPoints() == 1)
        {
            //display sphere
            sphereObject.SetActive(true);
            sphereObject.transform.localPosition = SplineMesh.getControlPoints()[0];
            meshCollider.sharedMesh = sphereObject.GetComponent<MeshFilter>().sharedMesh;
        }
        else if (SplineMesh.getNumberOfControlPoints() == 2)
        {
            //display linearly interpolated segment
            List<Vector3> controlPoints = SplineMesh.getControlPoints();
            LinearSplineMesh.setControlPoints(controlPoints.ToArray());
            meshCollider.sharedMesh = sphereObject.GetComponent<MeshFilter>().sharedMesh;
        }
        else
        {
            //display smoothly interpolated segments
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }

    }

}
