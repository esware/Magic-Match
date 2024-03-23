using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.Tiles
{
    public enum TileType
    {
        Blue,
        Green,
        Purple,
    }

    public class BaseTile : MonoBehaviour
    {
        public Vector2 Position { get; set; }
        public int X, Y; 

        [Header("Tile Settings")]
        public TileType tileType;
        public ParticleSystem matchEffect;
        public Sprite tileIcon;
        public AudioClip tileSound;
        
        private void Start()
        {
            InitializeTile();
        }

        private void InitializeTile()
        {
            GetComponent<Image>().sprite = tileIcon;
        }

        public virtual void OnMatch()
        {
            PlayMatchEffect();
        }

        private void PlayMatchEffect()
        {
            if (matchEffect != null)
            {
                matchEffect.Play();
            }
        }
        
    }

}