namespace SEP490G69.Battle.Combat
{
    using System.Collections;
    using UnityEngine;

    public class CharacterToastSpawner : MonoBehaviour
    {
        public void SpawnReceivedDamageToast(float dmg)
        {
            StartCoroutine(DelaySpawnToast(dmg));
        }

        public void SpawnDodgeToast()
        {
            StartCoroutine(DelaySpawnDodgeToast("MISS"));
        }

        public void SpawnCritToats(float critMul)
        {
            StartCoroutine(DelaySpawnCritToast(critMul));
        }

        private IEnumerator DelaySpawnToast(float dmg)
        {
            yield return new WaitForSeconds(0.15f);

            string message = $"{dmg.ToString()}";

            Vector3 position = transform.position + new Vector3(0, 0.75f, 0f);

            GameToastManager.Singleton.SpawnToast(new SpawnToastSettingsData
            {
                Message = message,
                TextColor = Color.red,
                SpawnPosition = position,
                DelaySpawnTime = 0.01f,
                TextSize = 40f
            });
        }
        private IEnumerator DelaySpawnDodgeToast(string message)
        {
            yield return new WaitForSeconds(0.15f);

            Vector3 position = transform.position + new Vector3(0, 0.75f, 0f);

            GameToastManager.Singleton.SpawnToast(new SpawnToastSettingsData
            {
                Message = message,
                TextColor = Color.green,
                SpawnPosition = position,
                DelaySpawnTime = 0.01f,
                TextSize = 40f
            });
        }
        private IEnumerator DelaySpawnCritToast(float critMul)
        {
            yield return new WaitForSeconds(0.2f);

            string message = $"Crit x{critMul.ToString()}";

            Vector3 position = transform.position + new Vector3(0, 0.75f, 0f);

            GameToastManager.Singleton.SpawnToast(new SpawnToastSettingsData
            {
                Message = message,
                TextColor = Color.yellow,
                SpawnPosition = position,
                DelaySpawnTime = 0.01f,
                TextSize = 40f
            });
        }
    }
}