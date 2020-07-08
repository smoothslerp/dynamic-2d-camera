using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class DebugLines : MonoBehaviour {
    
    Camera cam;
    CameraControl cc;
    private Material mat;
    public bool showCameraLines;

    void OnEnable() {
        this.cam = GetComponent<Camera>();
        try {
            this.cc = GetComponent<CameraControl>();
        } catch {
            Debug.Log("Unable to get camera control instance");
        }

        if (!mat) {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
        }    
    }

    private void OnPostRender() {
        CameraDebugLines();
    }

    void CameraDebugLines() {

        if (!showCameraLines) return;

        CameraLimits cL;
        if (Application.isPlaying) {
            cL = cc.GetCurrentCameraLimits();
        } else {
            cL = cc.GetAnchoredLimits();
        }

        GL.PushMatrix();
        GL.LoadPixelMatrix();

        mat.SetPass(0);

        GL.Begin(GL.LINES);
        // vertical lines to depict horizontal limits
        //left
        GL.Color(Color.red);
        GL.Vertex3(this.cam.pixelWidth * (cL.leftLimit), 0f, 0f);
        GL.Vertex3(this.cam.pixelWidth * (cL.leftLimit), this.cam.pixelHeight, 0f);
        
        //right
        GL.Color(Color.green);
        GL.Vertex3(this.cam.pixelWidth * (cL.rightLimit), 0f, 0f);
        GL.Vertex3(this.cam.pixelWidth * (cL.rightLimit), this.cam.pixelHeight, 0f);

        // horizontal lines to depict vertical limits
        // up
        GL.Color(Color.green);
        GL.Vertex3(0f, this.cam.pixelHeight * (cL.upLimit), 0f);
        GL.Vertex3(this.cam.pixelWidth, this.cam.pixelHeight * (cL.upLimit), 0f);

        //down
        GL.Color(Color.red);
        GL.Vertex3(0f, this.cam.pixelHeight * (cL.downLimit), 0f);
        GL.Vertex3(this.cam.pixelWidth, this.cam.pixelHeight * (cL.downLimit), 0f);

        GL.End();
        GL.PopMatrix();
    }

}
