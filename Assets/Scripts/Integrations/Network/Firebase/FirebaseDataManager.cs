using System;
using Firebase.Firestore;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dev.Scripts.Integrations;
using Firebase.Extensions;

namespace Integrations.Network.Firebase
{
    public class FirebaseDataManager : IDataManager
    {
        private FirebaseFirestore db;
        private string userId;

        public FirebaseDataManager(string userId)
        {
            db = FirebaseFirestore.DefaultInstance;
            this.userId = userId;
        }

        public void SetPlayerScore(int level, int score)
        {
            DocumentReference docRef = db.Collection("users").Document(userId).Collection("levels").Document(level.ToString());
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "score", score }
            };
            docRef.SetAsync(updates, SetOptions.MergeAll);
        }

        public void SetPlayerLevel(int level)
        {
            DocumentReference docRef = db.Collection("users").Document(userId);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "level", level }
            };
            docRef.SetAsync(updates, SetOptions.MergeAll);
        }

        public void GetPlayerLevel(Action<int> Callback)
        {
            DocumentReference docRef = db.Collection("users").Document(userId);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists && snapshot.TryGetValue("level", out int level))
                    {
                        Callback(level);
                    }
                    else
                    {
                        Callback(0); 
                    }
                }
                else
                {
                    Callback(0); 
                }
            });
        }

        public void GetPlayerScore(Action<int> Callback)
        {
            DocumentReference docRef = db.Collection("users").Document(userId).Collection("levels").Document("1");
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists && snapshot.TryGetValue("score", out int score))
                    {
                        Callback(score);
                    }
                    else
                    {
                        Callback(0); 
                    }
                }
                else
                {
                    Callback(0); 
                }
            });
        }

        public void SetStars(int Stars, int Level)
        {
            DocumentReference docRef = db.Collection("users").Document(userId).Collection("levels").Document(Level.ToString());
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "stars", Stars }
            };
            docRef.SetAsync(updates, SetOptions.MergeAll);
        }

        public void GetStars(Action<Dictionary<string, int>> Callback)
        {
            CollectionReference colRef = db.Collection("users").Document(userId).Collection("levels");
            colRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    QuerySnapshot snapshot = task.Result;
                    Dictionary<string, int> starsData = new Dictionary<string, int>();
                    foreach (DocumentSnapshot doc in snapshot.Documents)
                    {
                        if (doc.TryGetValue("stars", out int stars))
                        {
                            starsData[doc.Id] = stars;
                        }
                    }
                    Callback(starsData);
                }
                else
                {
                    Callback(new Dictionary<string, int>()); // Hata durumunda boş sözlük
                }
            });
        }

        public void SetTotalStars()
        {
            // Örnek olarak toplam yıldızları hesaplamak için
            GetStars(starsData =>
            {
                int totalStars = 0;
                foreach (var stars in starsData.Values)
                {
                    totalStars += stars;
                }
                DocumentReference docRef = db.Collection("users").Document(userId);
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "totalStars", totalStars }
                };
                docRef.SetAsync(updates, SetOptions.MergeAll);
            });
        }

        public void SetBoosterData(Dictionary<string, string> dic)
        {
            DocumentReference docRef = db.Collection("users").Document(userId);
            Dictionary<string, object> updates = new Dictionary<string, object>();
            foreach (var item in dic)
            {
                updates[item.Key] = item.Value;
            }
            docRef.SetAsync(updates, SetOptions.MergeAll);
        }

        public void GetBoosterData(Action<Dictionary<string, int>> Callback)
        {
            DocumentReference docRef = db.Collection("users").Document(userId);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    Dictionary<string, int> boosterData = new Dictionary<string, int>();
                    foreach (var item in snapshot.ToDictionary())
                    {
                        if (item.Key.StartsWith("booster_"))
                        {
                            boosterData[item.Key] = Convert.ToInt32(item.Value);
                        }
                    }
                    Callback(boosterData);
                }
                else
                {
                    Callback(new Dictionary<string, int>()); // Hata durumunda boş sözlük
                }
            });
        }

        public void Logout()
        {
            // Kullanıcı oturumu kapatma işlemleri burada yapılabilir.
        }
    }
}
