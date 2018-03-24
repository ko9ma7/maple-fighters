﻿using System;
using System.Collections.Generic;
using GameServerProvider.Client.Common;
using Scripts.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Windows
{
    public class GameServerSelectorWindow : UserInterfaceBaseFadeEffect
    {
        public event Action JoinButtonClicked;
        public event Action RefreshButtonClicked;
        public event Action<string> GameServerButtonClicked;

        [Header("Buttons")]
        [SerializeField] private Button joinButton;
        [SerializeField] private Button refreshButton;

        [Header("Parent")]
        [SerializeField] private Transform gameServerList;

        private readonly Dictionary<string, ClickableGameServerButton> gameServerButtons = new Dictionary<string, ClickableGameServerButton>();

        private void Start()
        {
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            joinButton.onClick.AddListener(OnJoinButtonClicked);
            refreshButton.onClick.AddListener(OnRefreshButtonClicked);
        }

        private void UnsubscribeFromEvents()
        {
            joinButton.onClick.RemoveListener(OnJoinButtonClicked);
            refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
        }

        public void CreateGameServerList(GameServerInformationParameters[] gameServerList)
        {
            StopRefreshing();
            RemoveGameServerList();

            if (gameServerList.Length == 0)
            {
                return;
            }

            foreach (var gameServer in gameServerList)
            {
                var gameServerButton = UserInterfaceContainer.Instance.Add<ClickableGameServerButton>(ViewType.Foreground, Index.Last, this.gameServerList);
                gameServerButton.ServerName = gameServer.Name;
                gameServerButton.Connections = gameServer.Connections;
                gameServerButton.MaxConnections = gameServer.MaxConnections;
                gameServerButton.ServerButtonClicked += () => OnGameServerButtonClicked(gameServer.Name);

                gameServerButtons.Add(gameServer.Name, gameServerButton);
            }
        }

        private void RemoveGameServerList()
        {
            if (gameServerButtons.Count == 0)
            {
                return;
            }

            foreach (var gameServerButton in gameServerButtons)
            {
                var gameServerButtonGameObject = gameServerButton.Value.gameObject;
                Destroy(gameServerButtonGameObject);
            }

            gameServerButtons.Clear();
        }

        public void DeselectAllGameServerButtons()
        {
            foreach (var gameServerButton in gameServerButtons)
            {
                gameServerButton.Value.Selected = false;
            }
        }

        private void OnGameServerButtonClicked(string serverName)
        {
            var gameServerButton = gameServerButtons[serverName];
            var isSelected = gameServerButton.Selected;

            DeselectAllGameServerButtons();

            if (isSelected)
            {
                joinButton.interactable = false;
                return;
            }

            gameServerButton.Selected = true;
            joinButton.interactable = gameServerButton.Selected;

            GameServerButtonClicked?.Invoke(serverName);
        }

        private void OnJoinButtonClicked()
        {
            JoinButtonClicked?.Invoke();
        }

        private void OnRefreshButtonClicked()
        {
            StartRefreshing();

            RefreshButtonClicked?.Invoke();
        }

        private void StartRefreshing()
        {
            joinButton.interactable = false;
            refreshButton.interactable = false;
        }

        private void StopRefreshing()
        {
            joinButton.interactable = false;
            refreshButton.interactable = true;
        }
    }
}