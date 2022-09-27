using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using static VdmDesktopManager;

public class VdmTest : MonoBehaviour
{

    public RawImage[] m_displayInCanvas;
    public Material [] m_displayInSharedMaterial;
    [Tooltip("Monitor Color Space")]
    public bool m_linearColorSpace = false;

    [Header("Received")]
    public int m_displayCount;
    public Texture2D[] m_screenTexture;


    public RawImage m_displayInCanvasMouseFocus;
    public Material m_displayInMaterialMouseFocus;

    public bool[] m_isMouseInScreen = new bool[0];

    public void Update()
    {
        for (int i = 0; i < m_displayCount; i++)
        {
            if (i < m_isMouseInScreen.Length) { 
                m_isMouseInScreen[i] = DesktopCapturePlugin_IsPointerVisible(i);
                if (m_isMouseInScreen[i]) {

                    if (i < m_displayInCanvas.Length)
                        m_displayInCanvasMouseFocus.texture = m_screenTexture[i];
                    if (i < m_displayInSharedMaterial.Length)
                        m_displayInMaterialMouseFocus.mainTexture = m_screenTexture[i];
                }
            }
            
        }
    }



    IEnumerator Start()
    {

        yield return new WaitForSeconds(1);
        { 
            m_displayCount = DesktopCapturePlugin_GetNDesks();
            m_isMouseInScreen = new bool[m_displayCount];
            DesktopCapturePlugin_Initialize();
            m_screenTexture = new Texture2D[m_displayCount];
            for (int i = 0; i < m_displayCount; i++)
            {
                int width = DesktopCapturePlugin_GetWidth(i);
                int height = DesktopCapturePlugin_GetHeight(i);
                var tex = new Texture2D(width, height, TextureFormat.BGRA32, false, m_linearColorSpace);
                //m_screenTexture[i] = new Texture2D(2, 2);
                m_screenTexture[i] = tex;
                if (i < m_displayInCanvas.Length)
                    m_displayInCanvas[i].texture = tex;
                if (i < m_displayInSharedMaterial.Length)
                    m_displayInSharedMaterial[i].mainTexture = tex;

                DesktopCapturePlugin_SetTexturePtr(i, m_screenTexture[i].GetNativeTexturePtr());
            }
            yield return new WaitForSeconds(1);
        }
       StartCoroutine(OnRender());
    }
    IEnumerator OnRender()
    {
        for (;;)
        {
            yield return new WaitForEndOfFrame();

            GL.IssuePluginEvent(DesktopCapturePlugin_GetRenderEventFunc(), 0);
        }
    }

  
    [DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();
    [DllImport("DesktopCapture")]
    private static extern void DesktopCapturePlugin_Initialize();
    [DllImport("DesktopCapture")]
    private static extern int DesktopCapturePlugin_GetNDesks();
    [DllImport("DesktopCapture")]
    private static extern int DesktopCapturePlugin_GetWidth(int iDesk);
    [DllImport("DesktopCapture")]
    private static extern int DesktopCapturePlugin_GetHeight(int iDesk);
    [DllImport("DesktopCapture")]
    private static extern int DesktopCapturePlugin_GetNeedReInit();
    [DllImport("DesktopCapture")]
    private static extern bool DesktopCapturePlugin_IsPointerVisible(int iDesk);
    [DllImport("DesktopCapture")]
    private static extern int DesktopCapturePlugin_GetPointerX(int iDesk);
    [DllImport("DesktopCapture")]
    private static extern int DesktopCapturePlugin_GetPointerY(int iDesk);
    [DllImport("DesktopCapture")]
    private static extern int DesktopCapturePlugin_SetTexturePtr(int iDesk, IntPtr ptr);
    [DllImport("DesktopCapture")]
    private static extern IntPtr DesktopCapturePlugin_GetRenderEventFunc();


}
