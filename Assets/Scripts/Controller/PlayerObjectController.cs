using UnityEngine;

namespace Controller
{
    public class PlayerObjectController : MonoBehaviour
    {
        public GameObject objs;
        private void Start()
        {
            
        }

        public void SetActiveObjects(bool value)
        {
            objs.SetActive(value);
        }
    }
}