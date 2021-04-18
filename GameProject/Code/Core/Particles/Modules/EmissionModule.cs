using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core.Particles {
    public class EmissionModule : IParticleModule {
        public bool Enabled { get; set; }
        public ParticleSystem AttachedSystem { get; set; }


        private List<(float, int)> BurstData;
        public int BurstCount => BurstData.Count;

        public float RateOverTime;
        public float RateOverTimeMultiplier;
        public float TrueRateOverTime => RateOverTime * RateOverTimeMultiplier;

        public float RateOverDistance;
        public float RateOverDistanceMultiplier;
        public float TrueRateOverDistance => RateOverDistance * RateOverDistanceMultiplier;


        // Initializers
        public void Initialize() {
            BurstData = new List<(float, int)>(0);
            
            RateOverTime = 1;
            RateOverDistanceMultiplier = 1;
            RateOverDistance = 0;
            RateOverDistanceMultiplier = 1;
        }

        public void Initialize(float rateOverTime = 1, float rateOverDistance = 0, float rateOverTimeMultiplier = 1, float rateOverDistanceMultiplier = 1) {
            BurstData = new List<(float, int)>(0);

            RateOverTime = rateOverTime;
            RateOverTimeMultiplier = rateOverTimeMultiplier;
            RateOverDistance = rateOverDistance;
            RateOverDistanceMultiplier = rateOverDistanceMultiplier;
        }

        public void Initialize(IList<(float, int)> burstData, float rateOverTime = 1, float rateOverDistance = 0, float rateOverTimeMultiplier = 1, float rateOverDistanceMultiplier = 1) {
            SetBursts(burstData);

            RateOverTime = rateOverTime;
            RateOverTimeMultiplier = rateOverTimeMultiplier;
            RateOverDistance = rateOverDistance;
            RateOverDistanceMultiplier = rateOverDistanceMultiplier;
        }



        // Update Logic
        public bool BurstCheck(float time, out int count) {
            if(BurstData.Count == 1) {
                AddBurst((BurstData[0].Item1 + 1, 0));
            }

            for(int i = 0; i < BurstCount - 1; i++) {
                if(time >= BurstData[i].Item1 && time < BurstData[i + 1].Item1) {
                    count = BurstData[i].Item2;
                    return true;
                }
            }

            count = 0;
            return false;
        }


        // Methods
        public (float, int) GetBurst(int index) {
            return BurstData[index];
        }

        public (float, int)[] GetBursts() {
            (float, int)[] retArr = new (float, int)[BurstCount];
            for(int i = 0; i < BurstCount; i++) {
                retArr[i] = BurstData[i];
            }

            return retArr;
        }

        public void SetBurst(int index, (float, int) data) {
            BurstData[index] = data;
        }

        public void AddBurst((float,int) data) {
            BurstData.Add(data);
        }

        public void RemoveBurst(int index) {
            BurstData.RemoveAt(index);
        }

        public void AddSetBurst(int index, (float, int) data) {
            if (index < 0) return;

            if(index < BurstCount) {
                BurstData[index] = data;
            } else {
                BurstData.Add(data);
            }
        }

        public void SetBursts(IList<(float, int)> data) {
            BurstData = new List<(float, int)>(BurstCount);

            for(int i = 0; i < BurstCount; i++) {
                BurstData.Add(data[i]);
            }
        }
    }

    
}
