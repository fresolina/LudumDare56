using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CoinCounter : MonoBehaviour
{
    //public PlayerCreatureSpawnController spawner;

    private int _numberofCoins = 0;
    public TMP_Text coinText;    

    public void pickedupcoin() {
        _numberofCoins++;
        CoinUI();
        if (_numberofCoins >= 3) {
            //spawner.SpawnCreature(1);
            _numberofCoins = 0;
            CoinUI();
        }
    }
    void CoinUI() { 
         coinText.text = "Coins: " + _numberofCoins.ToString() + "/ 3";
        
    }
   
}
