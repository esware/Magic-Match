using Firebase.Firestore;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dev.Scripts.Integrations.Network;
using Firebase.Extensions;

namespace Integrations.Network.Firebase
{


    public class FirebaseCurrencyManager : ICurrencyManager
    {
        private FirebaseFirestore db;
        private string userId;

        public FirebaseCurrencyManager(string userId)
        {
            db = FirebaseFirestore.DefaultInstance;
            this.userId = userId;
        }

        public void IncBalance(int amount)
        {
            GetBalance(balance =>
            {
                SetBalance(balance + amount);
            });
        }

        public void DecBalance(int amount)
        {
            GetBalance(balance =>
            {
                SetBalance(balance - amount);
            });
        }

        public void SetBalance(int newBalance)
        {
            DocumentReference docRef = db.Collection("users").Document(userId);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "balance", newBalance }
            };
            docRef.SetAsync(updates, SetOptions.MergeAll);
        }

        public void GetBalance(Action<int> Callback)
        {
            DocumentReference docRef = db.Collection("users").Document(userId);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists && snapshot.TryGetValue("balance", out int balance))
                    {
                        Callback(balance);
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
    }

}