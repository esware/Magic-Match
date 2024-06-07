using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class PreWinAnimations:GameState
    {
        public override void EnterState()
        {
            StartCoroutine(PreWinAnimationsCor());
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
        
        List<Item> GetAllExtaItems()
        {
            List<Item> list = new List<Item>();
            GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
            foreach (GameObject item in items)
            {
                if (item.GetComponent<Item>().currentType != ItemsTypes.NONE)
                {
                    list.Add(item.GetComponent<Item>());
                }
            }

            return list;
        }
        private IEnumerator PreWinAnimationsCor()
        {
            var levelManager = GameManager.Instance;
            GameObject.Find("Canvas").transform.Find("CompleteLabel").gameObject.SetActive(true);
            yield return new WaitForSeconds(1);

            List<Item> items = levelManager.GetRandomItems(Mathf.Clamp( levelManager.limitType == LIMIT.MOVES ?  levelManager.limit : 8, 0, 15));
            foreach (Item item in items)
            {
                if (levelManager.limitType == LIMIT.MOVES)
                    levelManager.limit--;
                item.NextType = (ItemsTypes)Random.Range(1, 3);
                item.ChangeType();
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.3f);
            while (GetAllExtaItems().Count > 0)
            { 
                Item item = GetAllExtaItems()[0];
                item.DestroyItem();
                levelManager.DragBlocked = true;
                yield return new WaitForSeconds(0.1f);
                levelManager.FindMatches();
                yield return new WaitForSeconds(1f);
                while (levelManager.DragBlocked)
                    yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(1f);
            while (levelManager.DragBlocked ||  levelManager.GetMatches().Count > 0)
                yield return new WaitForSeconds(0.2f);

            GameObject.Find("Canvas").transform.Find("CompleteLabel").gameObject.SetActive(false);
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.complete[0]);

            GameObject.Find("Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            GameObject.Find("Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(false);


            //TODO Change game state gameStatus = GameState.Win;
            GameManager.Instance.ChangeState<Win>();
        }
    }
}