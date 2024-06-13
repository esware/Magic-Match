
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{
    public class Level : MonoBehaviour
    {
        public int number;
        public Text label;
        public GameObject lockimage;
        
        void Start ()
        {
            if( PlayerPrefs.GetInt(PlayerPrefsKeys.Score + (number-1) ) > 0  )
            {
                lockimage.gameObject.SetActive( false );
                label.text = "" + number;
            }
            else
            {
                lockimage.gameObject.SetActive( true );
            }

            int stars = PlayerPrefs.GetInt($"Level.{number:000}.StarsCount", 0 );

            if( stars > 0 )
            {
                for( int i = 1; i <= stars; i++ )
                {
                    transform.Find( "Star" + i ).gameObject.SetActive( true );
                }

            }

        }
    }
}