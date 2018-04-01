using UnityEngine;
using UnityEditor;
using System.IO;


/*
 
    Para poder usar estas texturas es necesario que la textura sea del tipo de filtro Point y no bilinear o trilinear
    sin mipmaps, compresion RGB 16bits o trueColor RGB 24bits
    y un tamaño mayor o igual a 1024

    no vale la pena usarla para comprimir texturas o para optimizacion, solo sirve para ocultar informacion, pues sale al triple de la compresion
    asi que solo valdrá para algun juego con alguna tematica de esteganografia
     */

public class SteganographicTexturizer: EditorWindow {
    
    string texture, normal, savePath;
    bool groupEnabled;
    public Rect windowRect = new Rect(100, 100, 200, 200);
    

    // Agregar menu llamado ventana
    [MenuItem("Window/Tools/Steganographic Texturizer %#&g")]
    static void Init()
	{
        // consigue abrir una ventana o si no, hace una nueva
        SteganographicTexturizer window = EditorWindow.GetWindow(typeof(SteganographicTexturizer)) as SteganographicTexturizer;
        window.Show();
	}

    void OnGUI() {
        GUILayout.Label("Base Setting", EditorStyles.boldLabel);

        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Difusse")) texture = EditorUtility.OpenFilePanel("Open Difusse Texture", "", "png");
        if (GUILayout.Button("Normal")) normal = EditorUtility.OpenFilePanel("Open Normal Texture", "", "png");

        EditorGUILayout.LabelField(texture);
        EditorGUILayout.LabelField(normal);

        EditorGUILayout.HelpBox("Para poder usar estas texturas es necesario que la textura sea del tipo de \n - filtro Point y no bilinear o trilinear\n - sin mipmaps.\n - compresion RGB 16bits o trueColor RGB 24bits.\n - tamaño mayor o igual a 1024",MessageType.Info);

        groupEnabled = texture != "" && normal != "" && savePath != "";

        EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        if (GUILayout.Button("Apply Steganography") )
        {
            if (texture != "" && normal != "" && savePath != "")
            {
                byte[] normalBytes = File.ReadAllBytes(normal);
                byte[] textureBytes = File.ReadAllBytes(texture);

                StegoTexturing.ApplyStegoTexturing(textureBytes, normalBytes, savePath);
            }
            else  Debug.LogWarning("Please fill data correctly");
        }

        if (GUILayout.Button("Apply Decode"))
        {
            if (texture != "" && savePath != "")
            {
                byte[] textureBytes = File.ReadAllBytes(texture);
                StegoTexturing.DecodeStegoTextureTest(textureBytes, savePath);
            }
            else Debug.LogWarning("Please fill data correctly");
        }

        if (GUILayout.Button("Apply Test"))
        {
            if (texture != "" && savePath != "")
            {
                byte[] textureBytes = File.ReadAllBytes(texture);
                StegoTexturing.ApplyStegoTexturingTest(textureBytes, savePath);
            }
            else Debug.LogWarning("Please fill data correctly");
        }

        EditorGUILayout.EndToggleGroup();
    }

}
