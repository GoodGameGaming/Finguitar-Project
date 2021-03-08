using System.Collections.Generic;
using UnityEngine;

namespace RhythmGameStarter
{
    public class TapEffectPool : MonoBehaviour
    {
        public int poolSize;
        public GameObject effects;

        private List<NoteEffect> effectsPools = new List<NoteEffect>();

        private Transform effectsParent;

        void Awake()
        {
            effectsParent = new GameObject("Effects").transform;
            effectsParent.SetParent(transform);
            effectsParent.position = transform.position;

            for (int i = 0; i < poolSize; i++)
            {
                GetNewEffect();
            }
        }

        private NoteEffect GetNewEffect()
        {
            var g = Instantiate(effects);
            g.transform.SetParent(effectsParent);
            g.transform.position = effectsParent.position;

            var effect = g.GetComponent<NoteEffect>();

            effectsPools.Add(effect);

            return effect;
        }

        private NoteEffect GetUnUsedEffect()
        {
            var effect = effectsPools.Find(x => !x.inUse);

            if (effect == null)
            {
                effect = GetNewEffect();
            }

            effect.inUse = true;
            return effect;
        }

        public void EmitEffects(Transform target)
        {
            var effect = GetUnUsedEffect();
            effect.StartEffect(target);
        }
    }
}