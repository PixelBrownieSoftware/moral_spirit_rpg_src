using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_menuGui : MonoBehaviour
{
    public class s_button {
        public Image img;
        public Text txt;
    }
    List<s_button> buttons;
    public List<GameObject> buttonObjs;

    private void Awake()
    {
        foreach (GameObject ob in buttonObjs) {
            s_button button = new s_button();
            button.img = ob.transform.GetChild(0).GetComponent<Image>();
            button.img = ob.transform.GetChild(1).GetComponent<Image>();
            buttons.Add(button);
        }
    }

    public void ChangeButton(ref int ind, string textName) {
        buttons[ind].img.color = Color.white;
        buttons[ind].txt.text = textName;
        ind++;
    }

    public void ClearButtons() {
        foreach (s_button b in buttons) {
            b.img.color = Color.clear;
            b.txt.text = "";
        }
    }
}
