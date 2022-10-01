using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [FirestoreData]
    public struct CharacterData
    {
        [FirestoreProperty]
        public string userName { get; set; }

        [FirestoreProperty]
        public int currentLevel { get; set; }
        [FirestoreProperty]
        public int min { get; set; }
        [FirestoreProperty]
        public int sec { get; set; }
        [FirestoreProperty]
        public bool enabled { get; set; }
}

