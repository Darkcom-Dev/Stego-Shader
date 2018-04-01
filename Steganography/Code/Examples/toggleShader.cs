using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class toggleShader : MonoBehaviour
{

    public Button toggleShaderButton;
    public Renderer rend;
    private bool switchedOn = false;

    // Use this for initialization
    void Start()
    {
        Button butt = toggleShaderButton.GetComponent<Button>();
        butt.onClick.AddListener(ToggleShader);
    }

    public void ToggleShader()
    {
        rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Tutorial/Textured Colored");
        switchedOn = !switchedOn;
        rend.material.SetFloat("_Shift", switchedOn ? 16 : 1);

    }

    void Update()
    {
        if (Input.GetKey("escape"))
            Application.Quit();

    }

}
