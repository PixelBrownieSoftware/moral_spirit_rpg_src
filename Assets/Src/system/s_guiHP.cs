using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_guiHP : MonoBehaviour
{
    public Slider HP;
    public Slider SP;
    public Text characterName;
    public o_battleCharData chara = null;
    public float hp;
    public float sp;


    public void Update()
    {if (chara != null) {
            if (chara.maxHitPoints > 0)
            {
                hp = (((float)chara.hitPoints / (float)chara.maxHitPoints) * 100);
                //sp = (((float)chara.skillPoints / (float)chara.maxSkillPoints) * 100);
                HP.value = hp;
                //SP.value = sp;
                characterName.text = chara.name;
            }
        }
    }
}
