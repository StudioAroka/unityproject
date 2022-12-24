using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    #region singleTone
    private static ParticleManager _instance = null;
    public static ParticleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(ParticleManager)) as ParticleManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion

    public ParticleSystem instaHeartParticle;
    public enum ParticleTypeName
    {
        Confetti,
        Paw
    }

    [System.Serializable]
    public class ParticlePlan
    {
        public ParticleTypeName particleTypeName;
        public GameObject particlePrefab;

        public GameObject ParticleInstantiatePlay(Transform tr, Vector3 worldPos)
        {
            GameObject inst_particlePrefab = Instantiate(particlePrefab, tr);
            inst_particlePrefab.transform.position = worldPos;
            inst_particlePrefab.GetComponent<ParticleSystem>().Play();
            return inst_particlePrefab;
        }

        public GameObject ParticleInstantiatePlay(Vector3 worldPos, float destroyDelay = 5f)
        {
            GameObject inst_particlePrefab = Instantiate(particlePrefab, null);
            inst_particlePrefab.transform.position = worldPos;
            inst_particlePrefab.GetComponent<ParticleSystem>().Play();

            if (inst_particlePrefab != null)
                Destroy(inst_particlePrefab, destroyDelay);

            return inst_particlePrefab;
        }

        public GameObject ParticleInstantiatePlay(Transform tr, Vector3 worldPos, Quaternion rot)
        {
            GameObject inst_particlePrefab = Instantiate(particlePrefab, tr);
            inst_particlePrefab.transform.position = worldPos;
            inst_particlePrefab.transform.rotation = rot;

            inst_particlePrefab.GetComponent<ParticleSystem>().Play();
            return inst_particlePrefab;
        }


        public void ParticlePlay(bool useDestroy = false, float destroyDelaySec = 5f)
        {
            GameObject inst_particlePrefab = particlePrefab;
            inst_particlePrefab.GetComponent<ParticleSystem>().Play();
            if (useDestroy)
                Destroy(inst_particlePrefab, destroyDelaySec);
        }
    }


    public ParticlePlan[] particlePlans;
    public ParticleSystem shiningHexagonEffect;
    public ParticleSystem mapShineParticle;

    public void SetShineParticle(bool b)
    {
        if (b)
        {
            mapShineParticle.Play();
        }
        else
        {
            mapShineParticle.Stop();
            mapShineParticle.Clear();
        }
    }

    public ParticlePlan GetParticle(ParticleTypeName particleTypeName)
    {
        for (int i = 0; i < particlePlans.Length; i++)
        {
            if (particlePlans[i].particleTypeName == particleTypeName)
            {
                return particlePlans[i];
            }
        }
        return null;
    }
}