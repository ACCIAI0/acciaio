using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acciaio
{
    public class PubSubSystem : BaseSystem<PubSubSystem>
    {
		private readonly PubSubBoard _globalBoard = new PubSubBoard();
		private readonly Dictionary<string, PubSubBoard> _boards = new Dictionary<string, PubSubBoard>();

		private bool _isRunning = false;

        public override bool IsRunning => _isRunning;

		private PubSubBoard GetOrCreate(string boardName)
		{
			if (!_boards.ContainsKey(boardName)) _boards.Add(boardName, new PubSubBoard());
			return _boards[boardName];
		}

		public void Subscribe<T>(string eventName, Action<T> subscription) => 
				_globalBoard.Subscribe(eventName, subscription);

        public void Subscribe(string eventName, Action subscription) => 
				_globalBoard.Subscribe(eventName, subscription);

        public bool Unsubscribe<T>(string eventName, Action<T> subscription) => 
				_globalBoard.Unsubscribe(eventName, subscription);
        
        public void Unsubscribe(string eventName, Action subscription) => 
				_globalBoard.Unsubscribe(eventName, subscription);

        public void Trigger<T>(string eventName, T args) 
		{
			if (!IsRunning) 
			{
				Debug.LogError($"[Pub-Sub System] System shut down, won't trigger the event '{eventName}'.");
				return;
			}
			_globalBoard.Trigger(eventName, args);
		}

        public void Trigger(string eventName) 
		{
			if (!IsRunning) 
			{
				Debug.LogError($"[Pub-Sub System] System shut down, won't trigger the event '{eventName}'.");
				return;
			}
			_globalBoard.Trigger(eventName);
		}

		public void Subscribe<T>(string board, string eventName, Action<T> subscription) => 
				GetOrCreate(board).Subscribe(eventName, subscription);

        public void Subscribe(string board, string eventName, Action subscription) => 
				GetOrCreate(board).Subscribe(eventName, subscription);

        public bool Unsubscribe<T>(string board, string eventName, Action<T> subscription) => 
				GetOrCreate(board).Unsubscribe(eventName, subscription);
        
        public void Unsubscribe(string board, string eventName, Action subscription) => 
				GetOrCreate(board).Unsubscribe(eventName, subscription);

        public void Trigger<T>(string board, string eventName, T args) 
		{
			if (!IsRunning) 
			{
				Debug.LogError($"[Pub-Sub System] System shut down, won't trigger the event '{eventName}'.");
				return;
			}
			GetOrCreate(board).Trigger(eventName, args);
		}

        public void Trigger(string board, string eventName) 
		{
			if (!IsRunning) 
			{
				Debug.LogError($"[Pub-Sub System] System shut down, won't trigger the event '{eventName}'.");
				return;
			}
			GetOrCreate(board).Trigger(eventName);
		}

        protected override IEnumerator RunRoutine()
        {
            Debug.Log("[Pub-Sub System] Online");
			_isRunning = true;
			yield break;
        }

        protected override IEnumerator ShutdownRoutine()
        {
            Debug.Log("[Pub-Sub System] Shutdown");
			_isRunning = false;
			yield break;
        }
    }
}