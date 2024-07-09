using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MyRandom {

     public MyRandom(int seed) {

          // Рандомный сид.
          if (seed == 0) {
               seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
          }

          this.seed = seed;
          this.last = seed;
          this.m = 325648;
          this.c = 270312;
          this.a = 123856;
     }

     public MyRandom() : this(UnityEngine.Random.Range(int.MinValue, int.MaxValue)) {
     }



     public float Value(string key = null) {
          int num;
          if (string.IsNullOrEmpty(key)) {
               this.last = (this.a * this.last + this.c) % this.m;
               num = this.last;
          } else {
               num = this.seed + MyRandom.GetCode(key);
          }
          return Math.Abs(1f * (float)(this.a * num + this.c) % (float)this.m / (float)this.m);
     }

     public T ValueRange<T>(string key, params T[] values) {
          if (values.Length == 0) {
               return default(T);
          }
          return values[this.Range(0, values.Length - 1, key)];
     }

     public bool Chance(float probability, string key = null) {
          float num = this.Value(key);
          return probability > num;
     }

     public float Range(float min, float max, string key = null) {
          if (min >= max) {
               float num = min;
               min = max;
               max = num;
          }
          float num2 = this.Value(key);
          return min + (max - min) * num2;
     }

     public int Range(int min, int max, string key = null) {
          if (min >= max) {
               int num = min;
               min = max;
               max = num;
          }
          return (int)Math.Floor((double)this.Range((float)min, (float)max + 1f, key));
     }

     //public float Range(FloatRange range, string key = null) {
     //     return this.Range(range.min, range.max, key);
     //}

     //public int Range(IntRange range, string key = null) {
     //     return this.Range(range.min, range.max, key);
     //}

     public int Seed(string key = null) {
          int num;
          if (string.IsNullOrEmpty(key)) {
               this.last = (this.a * this.last + this.c) % this.m;
               num = this.last;
          } else {
               num = this.seed + MyRandom.GetCode(key);
          }
          return (this.a * num + this.c) % this.m;
     }

     public MyRandom NewRandom(string key = null) {
          return new MyRandom(this.Seed(key));
     }

     private static int GetCode(string key) {
          return (int)(Mathf.Pow((float)(key.GetHashCode() % 9651348), 3f) % 7645289f);
     }

     public T ValueByProbability<T>(List<MyRandom.Event<T>> values, string key) {
          if (values != null) {
               float num = values.Sum((MyRandom.Event<T> x) => x.probability) * this.Value(key);
               foreach (MyRandom.Event<T> @event in values) {
                    num -= @event.probability;
                    if (num <= 0f) {
                         return @event.info;
                    }
               }
          }
          return default(T);
     }

     public readonly int seed;

     public static MyRandom main = new MyRandom();

     private int m;

     private int c;

     private int a;

     private int last;

     public class Event<T> {
          public Event(T info, float probability) {
               this.probability = probability;
               this.info = info;
          }

          internal T info;

          internal float probability;
     }
}
