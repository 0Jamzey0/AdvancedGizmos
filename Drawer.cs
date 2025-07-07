using System;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    [Header("Draw IsometricText")] 
    public bool DrawIsometricText ;
    public Transform ITp0;
    public string Text = "\"Sample Text\"";
    
    [Header("Draw 3D-Sphere")] 
    public bool Draw3DSphere;
    public Transform Sp0;
    public float r=6;
    
    
    [Header("Draw Mesh")] 
    public bool DrawMesh;
    public Mesh Mesh;

    [Header("Bezier Path")] 
    public bool DrawBezier;
    public Transform p0;
    public Transform p1;
    public Transform p2;
    public Transform p3;
    public int BezierControlPointCount = 30;
    
    [Header("Distance Label")]
    public bool DrawDistanceLabel = false;
    public string Label = "Distance";
    //public float DistanceBeforeFade = 15f;
    public Transform DLp0;
    public Transform DLp1;
    


    [Header("Draw Transform-Axis")] 
    public bool DrawTransformAxis ;
    public Transform TAp0;
    public float axislength = 2f;
    
    [Header("Draw Frustum")]
    public bool DrawFrustum;
    public Transform Fp0;
    public float FOV = 50f;
    public float MaxRange = 50f;
    public float AspectRatio = 1.4f;
    void OnDrawGizmos()
    {
        if (Draw3DSphere)
        AdvancedGizmos.DrawFullSphere(Sp0.position, r,Color.green);
        
       // AdvancedGizmos.DrawCapsule(transform.position, transform.rotation , 4f, .5f,Color.red);
        //AdvancedGizmos.DrawPolygon(transform.position + (transform.right*7),3f,3,Color.magenta);
        //AdvancedGizmos.DrawRegularTriangle3D(transform.position + (transform.right*7),transform.up*10,10f,Color.green);
        Font font = new Font("Verdana");
        if(DrawIsometricText)
        AdvancedGizmos.DrawIsometricText(ITp0.position,Text, Color.white,font,FontStyle.BoldAndItalic,40);
        //AdvancedGizmos.DrawCylinder(transform.position, transform.rotation , 4f, .5f,Color.cyan);
        if(DrawMesh)
        AdvancedGizmos.DrawWireMesh(Mesh,transform.position,transform.rotation,new Vector3(1,1,1),Color.cyan);
        if(DrawBezier)
        AdvancedGizmos.DrawBezier(p0.position,p1.position,p2.position,p3.position,Color.cyan,BezierControlPointCount);
        if(DrawDistanceLabel)
        AdvancedGizmos.DrawDistanceLabel(DLp0.position,DLp1.position,Color.white,Label);
        if(DrawTransformAxis)
        AdvancedGizmos.DrawTransformAxes(TAp0.position,TAp0.rotation,axislength);
        if(DrawFrustum)
        AdvancedGizmos.DrawFrustum(Fp0.position,Fp0.rotation,FOV,MaxRange,AspectRatio,Color.blue);


    }
}
