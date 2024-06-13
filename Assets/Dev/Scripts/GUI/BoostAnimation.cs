using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Dev.Scripts.GUI
{
    public class BoostAnimation : MonoBehaviour
    {
        public Square square;

        public void ShowEffect ()
        {
            GameObject partcl = Instantiate (Resources.Load ("Prefabs/Effects/Firework"), transform.position, Quaternion.identity) as GameObject;
            partcl.GetComponent<ParticleSystem> ().startColor = GameManager.Instance.scoresColors [square.item.Color];
            if (name.Contains ("random_color_item"))//1.6.1 new lollipop
                partcl.GetComponent<ParticleSystem> ().startColor = Color.white;
            Destroy (partcl, 1f);

            if (name.Contains ("random_color_item")) {//1.6.1 new lollipop
                GameObject p = Instantiate (Resources.Load ("Prefabs/Effects/CircleExpl"), transform.position, Quaternion.identity) as GameObject;
                Destroy (p, 0.4f);

            }
        }

        public void OnFinished (BoostType boostType)
        {
            if (boostType == BoostType.Random_color) {

                List<Item> itemsList = GameManager.Instance.GetItemsAround (square);
                foreach (Item item in itemsList) {
                    if (item != null) {
                        if (item.currentType == ItemsTypes.NONE)
                            item.GenColor (-1, true);
                    }
                }

            }
            if (boostType == BoostType.Bomb) {
                square.item.DestroyItem ();
            }
            GameManager.Instance.StartCoroutine (GameManager.Instance.FindMatchDelay ());
            SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.explosion);

            Destroy (gameObject);
        }
    }
}