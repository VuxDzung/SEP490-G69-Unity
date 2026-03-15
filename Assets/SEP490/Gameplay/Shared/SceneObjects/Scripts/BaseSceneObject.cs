namespace SEP490G69
{
    using UnityEngine;

    public class BaseSceneObject : MonoBehaviour
    {
        [SerializeField] private string m_ObjectId;

        public string ObjectId => m_ObjectId;
    }
}