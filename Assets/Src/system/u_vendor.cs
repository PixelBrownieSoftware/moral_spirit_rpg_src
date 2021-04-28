using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.System;
using UnityEditor;
using MagnumFoundation2.System.Core;

[System.Serializable]
public struct shop_item {
    public rpg_item item;
    public float price;
}

public class u_vendor : s_object
{
    public enum SHOP_STATE { INACTIVE, ACTIVE, BUY, TALK };
    public SHOP_STATE shopState;

    public shop_item[] shopItems;
    public int menuchoice;

    public BoxCollider2D trigger;

    private new void Update()
    {
        base.Update();
        string str = "";
        switch (shopState) {
            case SHOP_STATE.INACTIVE:
                c_player p = IfTouchingGetCol<c_player>(trigger);
                if (p != null)
                {
                    rpg_globals.gl.shopText.text = "Press the selection button to buy.";
                    if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
                    {
                        p.control = false;
                        p.rbody2d.velocity = Vector2.zero;
                        rpg_globals.gl.shopBox.enabled = true;
                        shopState = SHOP_STATE.ACTIVE;
                        rpg_globals.gl.menuAble = false;
                    }
                }
                else
                {
                    rpg_globals.gl.shopText.text = "";
                }
                break;

            case SHOP_STATE.ACTIVE:
                if (menuchoice == 0)
                    str += "-> ";
                str += "Shop\n";
                if (menuchoice == 1)
                    str += "-> ";
                str += "Talk\n";
                if (menuchoice == 2)
                    str += "-> ";
                str += "Leave\n";

                if (Input.GetKeyDown(s_globals.GetKeyPref("down")))
                {
                    menuchoice ++;
                }
                if (Input.GetKeyDown(s_globals.GetKeyPref("up")))
                {
                    menuchoice--;
                }
                menuchoice = Mathf.Clamp(menuchoice, 0, 3);

                rpg_globals.gl.shopText.text = str;
                if (Input.GetKeyDown(s_globals.GetKeyPref("select"))) {
                    switch (menuchoice) {
                        case 0:
                            print("ok");
                            shopState = SHOP_STATE.BUY;
                            break;
                        case 2:
                            rpg_globals.gl.shopBox.enabled = false;
                            rpg_globals.gl.player.control = true;
                            rpg_globals.gl.menuAble = true;
                            shopState = SHOP_STATE.INACTIVE;
                            break;
                    }
                    menuchoice = 0;
                }
                break;

            case SHOP_STATE.BUY:
                Dictionary<string, int> tempINV = rpg_globals.gl.inventory;
                if (Input.GetKeyDown(s_globals.GetKeyPref("down")))
                {
                    menuchoice++;
                }
                if (Input.GetKeyDown(s_globals.GetKeyPref("up")))
                {
                    menuchoice--;
                }
                menuchoice = Mathf.Clamp(menuchoice, 0, shopItems.Length);
                for (int i = 0; i < shopItems.Length; i++)
                {
                    int num = 0;
                    if (tempINV.ContainsKey(shopItems[i].item.name))
                    {
                        num = tempINV[shopItems[i].item.name];
                    }
                    if (menuchoice == i)
                        str += "-> ";
                    str += shopItems[i].item.name + " x " + num + " - " + "£" + shopItems[i].price + "\n";
                }
                str += "Press " + s_globals.GetKeyPref("back").ToString() + " to exit.";
                rpg_globals.gl.shopText.text = str;
                if (Input.GetKeyDown(s_globals.GetKeyPref("select"))) {

                    menuchoice = menuchoice % shopItems.Length;
                    rpg_globals.gl.AddItem(shopItems[menuchoice].item);
                }
                if (Input.GetKeyDown(s_globals.GetKeyPref("back")))
                {
                    menuchoice = 0;
                    shopState = SHOP_STATE.ACTIVE;
                }
                break;
        }
    }

}
